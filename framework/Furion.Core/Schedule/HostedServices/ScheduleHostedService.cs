// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Furion.Schedule;

/// <summary>
/// 调度计划后台主机服务
/// </summary>
internal sealed class ScheduleHostedService : BackgroundService
{
    /// <summary>
    /// 避免由 CLR 的终结器捕获该异常从而终止应用程序，让所有未觉察异常被觉察
    /// </summary>
    internal event EventHandler<UnobservedTaskExceptionEventArgs>? UnobservedTaskException;

    /// <summary>
    /// 日志对象
    /// </summary>
    private readonly ILogger<ScheduleHostedService> _logger;

    /// <summary>
    /// 服务提供器
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// 作业监视器
    /// </summary>
    private IJobMonitor? Monitor { get; }

    /// <summary>
    /// 作业执行器
    /// </summary>
    private IJobExecutor? Executor { get; }

    /// <summary>
    /// 作业调度器工厂
    /// </summary>
    private ISchedulerJobFactory Factory { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志对象</param>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="factory">作业调度器工厂</param>
    /// <param name="schedulerJobs">作业调度器集合</param>
    public ScheduleHostedService(ILogger<ScheduleHostedService> logger
        , IServiceProvider serviceProvider
        , ISchedulerJobFactory factory
        , IList<SchedulerJob> schedulerJobs)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        Monitor = serviceProvider.GetService<IJobMonitor>();
        Executor = serviceProvider.GetService<IJobExecutor>();
        Factory = factory;

        // 逐条将作业调度器载入作业调度器工厂中
        foreach (var schedulerJob in schedulerJobs)
        {
            factory.Append(schedulerJob);
        }
    }

    /// <summary>
    /// 执行后台任务
    /// </summary>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(LogTemplateHelpers.ScheduleRunningTemplate, Factory.SchedulerJobs.Count);

        // 调度器服务停止监听
        stoppingToken.Register(() =>
             _logger.LogDebug(LogTemplateHelpers.ScheduleStoppingTemplate));

        // 监听调度器服务是否取消
        while (!stoppingToken.IsCancellationRequested)
        {
            // 执行具体作业
            await BackgroundProcessing(stoppingToken);
        }

        _logger.LogCritical(LogTemplateHelpers.ScheduleStoppedTemplate);
    }

    /// <summary>
    /// 判断是否是有效的作业
    /// </summary>
    private static readonly Func<JobDetail?, bool> IsEffectiveJob =
        u => u == null || !(u.Status == JobStatus.None || u.Status == JobStatus.Pause || (u.LockMode == JobLockMode.Serial && u.Status == JobStatus.Blocked) || u.StartMode == JobStartMode.Wait);

    /// <summary>
    /// 后台调用作业处理程序
    /// </summary>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        var referenceTime = DateTime.UtcNow;

        // 获取所有有效的作业调度器
        var schedulerJobsThatShouldRun = Factory.SchedulerJobs.Where(u => IsEffectiveJob(u.JobDetail));

        // 创建一个任务工厂并保证作业处理程序使用当前的计划程序
        var taskFactory = new TaskFactory(TaskScheduler.Current);

        // 遍历所有作业调度器
        foreach (var schedulerJobThatShouldRun in schedulerJobsThatShouldRun)
        {
            // 解构作业调度器信息
            (var jobId, var jobType, var jobDetail, var jobTriggers) = schedulerJobThatShouldRun;

            // 查询所有符合触发的触发器
            var triggersThatShouldRun = jobTriggers.Where(t => t.InternalShouldRun(referenceTime));

            // 逐一创建新线程并触发
            foreach (var jobTrigger in triggersThatShouldRun)
            {
                // 计算当前触发器增量信息
                jobTrigger.Increment();

                // 创建新的线程执行
                await taskFactory.StartNew(async () =>
                {
                    // 创建共享上下文数据对象
                    var properties = new Dictionary<object, object>();

                    // 创建执行前上下文
                    var jobExecutingContext = new JobExecutingContext(jobDetail, jobTrigger, properties)
                    {
                        ExecutingTime = referenceTime
                    };

                    // 执行异常对象
                    InvalidOperationException? executionException = default;

                    // 作业服务作用域范围
                    IServiceScope? serviceScope = null;

                    try
                    {
                        // 创建作业处理程序
                        var job = CreateJobInstance(_serviceProvider
                            , jobType
                            , jobDetail.WithScopeExecution
                            , ref serviceScope);

                        // 调用执行前监视器
                        if (Monitor != default)
                        {
                            await Monitor.OnExecutingAsync(jobExecutingContext, stoppingToken);
                        }

                        // 判断是否自定义了执行器
                        if (Executor == default)
                        {
                            await job.ExecuteAsync(jobExecutingContext, stoppingToken);
                        }
                        else
                        {
                            await Executor.ExecuteAsync(jobExecutingContext, job, stoppingToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 递增错误次数
                        jobTrigger.IncrementErrors();

                        // 输出异常日志
                        _logger.LogError(ex, LogTemplateHelpers.JobExecutionFailedTemplate, jobId, ex.Message);

                        // 标记异常
                        executionException = new InvalidOperationException(string.Format("Error occurred executing <{0}>.", jobId), ex);

                        // 捕获 Task 任务异常信息并统计所有异常
                        if (UnobservedTaskException != default)
                        {
                            var args = new UnobservedTaskExceptionEventArgs(
                                ex as AggregateException ?? new AggregateException(ex));

                            UnobservedTaskException.Invoke(this, args);
                        }
                    }
                    finally
                    {
                        // 释放作业服务作用域范围
                        serviceScope?.Dispose();

                        // 当前时间
                        var executedTime = DateTime.UtcNow;

                        // 输出触发完成日志
                        if (jobDetail!.PrintExecutionLog)
                        {
                            _logger.LogInformation(LogTemplateHelpers.JobExecutionTemplate
                                , jobId
                                , jobDetail.Description
                                , jobDetail.JobType
                                , jobDetail.LockMode
                                , jobTrigger.TriggerId
                                , jobTrigger.TriggerType
                                , jobTrigger.Args
                                , jobTrigger.NextRunTime
                                , jobTrigger.NumberOfRuns
                                , jobTrigger.NumberOfErrors
                                , referenceTime
                                , executedTime
                                , $"{Math.Round((executedTime - referenceTime).TotalMilliseconds, 2)}ms"
                                , executionException?.Message);
                        }

                        // 调用执行后监视器
                        if (Monitor != default)
                        {
                            // 创建执行后上下文
                            var jobExecutedContext = new JobExecutedContext(jobDetail, jobTrigger, properties)
                            {
                                ExecutedTime = executedTime,
                                Exception = executionException
                            };

                            await Monitor.OnExecutedAsync(jobExecutedContext, stoppingToken);
                        }
                    }
                }, stoppingToken);
            }
        }

        // 休眠线程并等待被唤醒
        await WaitingAwakenAsync(referenceTime, stoppingToken);
    }

    /// <summary>
    /// 休眠线程并等待被唤醒
    /// </summary>
    /// <param name="referenceTime">当前后台服务检查时间</param>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    private async Task WaitingAwakenAsync(DateTime referenceTime, CancellationToken stoppingToken)
    {
        /*
         * 为了避免程序在未知情况下存在不必要的耗时操作从而导致时间出现偏差
         * 所以这里采用 DateTimeKind.Unspecified 转换当前时间并忽略毫秒部分
         */
        var unspecifiedTime = new DateTime(referenceTime.Year, referenceTime.Month, referenceTime.Day, referenceTime.Hour, referenceTime.Minute, referenceTime.Second);

        // 查找下一次符合触发时机的所有作业触发器
        var closestJobTriggers = Factory.SchedulerJobs.Where(u => IsEffectiveJob(u.JobDetail))
                                                           .SelectMany(u => u.Triggers!.Where(t => t.NextRunTime != null && t.NextRunTime >= unspecifiedTime));

        // 获取最早执行的作业触发器时间
        var earliestTriggerTime = closestJobTriggers.Any()
            ? closestJobTriggers.Min(t => t.NextRunTime)!.Value
            : DateTime.MaxValue;    // 如果没有感知到需要执行的作业，则一直休眠

        // 计算出总的休眠时间，这里采用 Math.Min 解决 Timer 最大值不能超过 int.MaxValue 的问题
        var interval = Math.Min(int.MaxValue, (earliestTriggerTime - referenceTime).TotalMilliseconds);

        // 将当前线程休眠至下一次触发前或感知到作业调度器工厂变化时
        await Factory.SleepAsync(interval, stoppingToken);
    }

    /// <summary>
    /// 创建作业处理程序
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="jobType">作业类型</param>
    /// <param name="withScopeExecution">是否创建新的服务作用域执行作业</param>
    /// <param name="serviceScope">服务作用域范围</param>
    /// <returns><see cref="IJob"/></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static IJob CreateJobInstance(IServiceProvider serviceProvider
        , Type jobType
        , bool withScopeExecution
        , ref IServiceScope? serviceScope)
    {
        // 获取构造函数
        var constructors = jobType.GetConstructors();

        // 最多只能包含一个构造函数
        if (constructors.Length > 1)
        {
            throw new InvalidOperationException("A job type can contain at most one constructor.");
        }

        // 判断是否创建新的服务作用域执行
        serviceScope = !withScopeExecution ? null : serviceProvider.CreateScope();
        var newServiceProvider = serviceScope?.ServiceProvider ?? serviceProvider;

        // 反射创建作业执行程序
        var job = (constructors.Length == 0
            ? Activator.CreateInstance(jobType)
            : ActivatorUtilities.CreateInstance(newServiceProvider, jobType)) as IJob;

        // 空检查
        ArgumentNullException.ThrowIfNull(job);

        return job!;
    }
}