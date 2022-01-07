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
    private ISchedulerFactory Factory { get; }

    /// <summary>
    /// 调度器休眠后再度被激活前多少ms完成耗时操作
    /// </summary>
    private int TimeBeforeSync { get; }

    /// <summary>
    /// 最小存储器同步间隔（秒）
    /// </summary>
    private int MinimumSyncInterval { get; }

    /// <summary>
    /// 记录存储器最近同步时间
    /// </summary>
    private DateTime? LastSyncTime { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志对象</param>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="factory">作业调度器工厂</param>
    /// <param name="schedulerJobBuilders">调度作业构建器集合</param>
    /// <param name="timeBeforeSync">调度器休眠后再度被激活前多少ms完成耗时操作</param>
    /// <param name="minimumSyncInterval">最小存储器同步间隔（秒）</param>
    public ScheduleHostedService(ILogger<ScheduleHostedService> logger
        , IServiceProvider serviceProvider
        , ISchedulerFactory factory
        , IEnumerable<SchedulerJobBuilder> schedulerJobBuilders
        , int timeBeforeSync
        , int minimumSyncInterval)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        Monitor = serviceProvider.GetService<IJobMonitor>();
        Executor = serviceProvider.GetService<IJobExecutor>();
        Factory = factory;
        TimeBeforeSync = timeBeforeSync;
        MinimumSyncInterval = minimumSyncInterval;

        var referenceTime = DateTime.UtcNow;

        // 逐条对调度作业构建器进行构建
        foreach (var schedulerJobBuilder in schedulerJobBuilders)
        {
            // 将调度作业存储起来
            factory.AddSchedulerJob(schedulerJobBuilder.Build(referenceTime));
        }
    }

    /// <summary>
    /// 执行后台任务
    /// </summary>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(LogTemplateHelpers.ScheduleRunningTemplate, Factory.GetSchedulerJobs().Count);

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
        u => u == null || !(u.Status == JobStatus.None || u.Status == JobStatus.Pause || (u.ExecutionMode == JobExecutionMode.Serial && u.Status == JobStatus.Blocked) || u.StartMode == JobStartMode.Wait);

    /// <summary>
    /// 后台调用作业处理程序
    /// </summary>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        var referenceTime = DateTime.UtcNow;

        // 获取所有有效的作业调度器
        var schedulerJobsThatShouldRun = Factory.GetSchedulerJobs().Where(u => IsEffectiveJob(u.JobDetail));

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

                    try
                    {
                        // 创建作业处理程序
                        var job = CreateJobInstance(_serviceProvider, jobType);

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
                        // 递增错误次数
                        jobTrigger.IncrementErrors();

                        // 当前时间
                        var executedTime = DateTime.UtcNow;

                        // 输出触发完成日志
                        if (jobDetail!.WithExecutionLog)
                        {
                            _logger.LogInformation(LogTemplateHelpers.JobExecutionTemplate
                                , jobId
                                , jobDetail.Description
                                , jobDetail.JobType
                                , jobDetail.ExecutionMode
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

        // 将当前线程休眠直至最快触发的作业之前
        await WaitingClosestTrigger(stoppingToken);
    }

    /// <summary>
    /// 将当前线程休眠直至最快触发的作业之前
    /// </summary>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    private async Task WaitingClosestTrigger(CancellationToken stoppingToken)
    {
        /*
         * 为了避免程序在未知情况下存在不必要的耗时操作从而导致时间出现偏差
         * 所以这里采用 DateTimeKind.Unspecified 转换当前时间并忽略毫秒部分
         */
        var referenceTime = DateTime.UtcNow;
        var unspecifiedTime = new DateTime(referenceTime.Year, referenceTime.Month, referenceTime.Day, referenceTime.Hour, referenceTime.Minute, referenceTime.Second);

        // 查找下一次符合触发时机的所有作业触发器
        var closestJobTriggers = Factory.GetSchedulerJobs().Where(u => IsEffectiveJob(u.JobDetail))
                                                           .SelectMany(u => u.Triggers!.Where(t => t.NextRunTime >= unspecifiedTime));

        // 获取最早执行的作业触发器时间
        var closestNextRunTime = closestJobTriggers.Any()
            ? closestJobTriggers.Min(t => t.NextRunTime)
            : referenceTime.AddSeconds(MinimumSyncInterval);    // 避免无运行作业导致调度器处于永久休眠状态

        // 计算出总的休眠时间，在这段时间内可以做耗时操作
        var interval = (closestNextRunTime - referenceTime).TotalMilliseconds;

        /*
         * 在最早触发器触发之前同步存储器作业数据，并设定超时时间 = interval - TimeBeforeSync;
         * 如果在未超时时间内完成同步，则更新内存中的包装器
         * 否则取消同步，等待调度器工厂被再次激活，进入下一轮同步
         */
        var syncTimeout = interval - TimeBeforeSync;
        if (syncTimeout > 0
            // 最低频率同步
            && (LastSyncTime == null || (referenceTime - LastSyncTime.Value).TotalSeconds >= MinimumSyncInterval))
        {
            // 存储最近同步时间
            LastSyncTime = referenceTime;

            // 同步存储器作业数据
            _ = SynchronizationStorer(TimeSpan.FromMilliseconds(syncTimeout), stoppingToken);
        }

        // 避免刚好触发的情况
        if (interval > 0)
        {
            // 将当前线程休眠至下一次触发前，也就是可以休眠到执行前
            var delay = TimeSpan.FromMilliseconds(interval);
            await Task.Delay(delay, stoppingToken);
        }
    }

    /// <summary>
    /// 同步存储器作业数据
    /// </summary>
    /// <param name="timeout">超时时间戳</param>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    private async Task SynchronizationStorer(TimeSpan timeout, CancellationToken stoppingToken)
    {
        // 创建超时关联任务取消 Token
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken
              , new CancellationTokenSource(timeout).Token);
        var cancellationToken = cts.Token;

        // 创建同步任务
        var syncTask = Task.Run(async () =>
        {
            // 开始同步
            _logger.LogInformation("The scheduler starts synchronizing the storer......");

            Console.WriteLine("模拟数据库操作.");
            await Task.Delay(10);

            // 同步成功
            if (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("The scheduler sync storer completed.");
            }
        }, cancellationToken);

        // 判断是否超时
        if (await Task.WhenAny(syncTask, Task.Delay(timeout, cancellationToken)) == syncTask)
        {
            cts.Cancel();
            await syncTask;
        }
        else
        {
            _logger.LogWarning("The scheduler synchronization storer timed out and the operation was cancelled.");
        }
    }

    /// <summary>
    /// 创建作业处理程序
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="jobType">作业类型</param>
    /// <returns><see cref="IJob"/></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static IJob CreateJobInstance(IServiceProvider serviceProvider, Type jobType)
    {
        // 获取构造函数
        var constructors = jobType.GetConstructors();

        // 最多只能包含一个构造函数
        if (constructors.Length > 1)
        {
            throw new InvalidOperationException("A job type can contain at most one constructor.");
        }

        // 反射创建作业执行程序
        var job = (constructors.Length == 0
            ? Activator.CreateInstance(jobType)
            : ActivatorUtilities.CreateInstance(serviceProvider, jobType)) as IJob;

        // 空检查
        ArgumentNullException.ThrowIfNull(job);

        return job!;
    }
}