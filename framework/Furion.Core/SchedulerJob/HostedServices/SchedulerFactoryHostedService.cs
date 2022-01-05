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
using System.Collections.Concurrent;

namespace Furion.SchedulerJob;

/// <summary>
/// 调度器工厂后台主机服务
/// </summary>
internal sealed class SchedulerFactoryHostedService : BackgroundService
{
    /// <summary>
    /// 避免由 CLR 的终结器捕获该异常从而终止应用程序，让所有未觉察异常被觉察
    /// </summary>
    internal event EventHandler<UnobservedTaskExceptionEventArgs>? UnobservedTaskException;

    /// <summary>
    /// 日志对象
    /// </summary>
    private readonly ILogger<SchedulerFactoryHostedService> _logger;

    /// <summary>
    /// 调度作业集合
    /// </summary>
    private readonly HashSet<SchedulerJobWrapper> _schedulerJobs = new();

    /// <summary>
    /// 作业监视器
    /// </summary>
    private IJobMonitor? Monitor { get; }

    /// <summary>
    /// 作业执行器
    /// </summary>
    private IJobExecutor? Executor { get; }

    /// <summary>
    /// 作业存储器
    /// </summary>
    private IJobStorer Storer { get; }

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
    /// <param name="jobs">作业集合</param>
    /// <param name="storer">作业存储器</param>
    /// <param name="jobDetailBuilders">作业详情构建器集合</param>
    /// <param name="timeBeforeSync">调度器休眠后再度被激活前多少ms完成耗时操作</param>
    /// <param name="minimumSyncInterval">最小存储器同步间隔（秒）</param>
    public SchedulerFactoryHostedService(ILogger<SchedulerFactoryHostedService> logger
        , IServiceProvider serviceProvider
        , IEnumerable<IJob> jobs
        , IJobStorer storer
        , ConcurrentDictionary<string, JobDetailBuilder> jobDetailBuilders
        , int timeBeforeSync
        , int minimumSyncInterval)
    {
        _logger = logger;
        Monitor = serviceProvider.GetService<IJobMonitor>();
        Executor = serviceProvider.GetService<IJobExecutor>();
        Storer = storer;
        TimeBeforeSync = timeBeforeSync;
        MinimumSyncInterval = minimumSyncInterval;

        // 逐条对作业详情构建器进行包装
        foreach (var (jobId, jobDetailBuilder) in jobDetailBuilders)
        {
            // 查询作业处理程序实例
            var jobHandler = jobs.SingleOrDefault(j => j.GetType() == jobDetailBuilder.JobType);
            if (jobHandler == null) continue;

            // 构建作业详情构建器
            var (jobDetail, jobTriggers) = jobDetailBuilder.Build();

            // 逐条包装并添加到 HashSet 集合中
            _schedulerJobs.Add(new SchedulerJobWrapper(jobId)
            {
                Job = jobHandler,
                JobDetail = jobDetail,
                Triggers = jobTriggers,
            });
        }
    }

    /// <summary>
    /// 执行后台任务
    /// </summary>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SchedulerJob Hosted Service is running.");

        // 调度器服务停止监听
        stoppingToken.Register(() =>
             _logger.LogDebug($"SchedulerJob Hosted Service is stopping."));

        // 监听调度器服务是否取消
        while (!stoppingToken.IsCancellationRequested)
        {
            // 执行具体作业
            await BackgroundProcessing(stoppingToken);
        }

        _logger.LogCritical($"SchedulerJob Hosted Service is stopped.");
    }

    /// <summary>
    /// 判断是否是有效的作业
    /// </summary>
    private static readonly Func<JobDetail?, bool> IsEffectiveJob =
        u => u == null || !(u.Status == JobStatus.None || u.Status == JobStatus.Pause || (u.Mode == JobMode.Serial && u.Status == JobStatus.Blocked));

    /// <summary>
    /// 后台调用作业处理程序
    /// </summary>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        var referenceTime = DateTime.UtcNow;

        // 获取所有符合触发时机的作业
        var jobsThatShouldRun = _schedulerJobs.Where(u => IsEffectiveJob(u.JobDetail));

        // 创建一个任务工厂并保证作业处理程序使用当前的计划程序
        var taskFactory = new TaskFactory(TaskScheduler.Current);

        // 逐条创建新线程调用
        foreach (var jobThatShouldRun in jobsThatShouldRun)
        {
            // 解构包装器信息
            (var jobId, var jobHandler, var jobDetail, var jobTriggers) = jobThatShouldRun;

            // 查询所有符合触发的触发器
            var triggersThatShouldRun = jobTriggers.Where(t => t.ShouldRun(referenceTime));

            // 逐一触发
            foreach (var jobTrigger in triggersThatShouldRun)
            {
                // 计算当前触发器增量信息
                jobTrigger.Increment();

                // 创建新的线程执行
                await taskFactory.StartNew(async () =>
                {
                    // 输出触发日志
                    if (jobDetail!.WithExecutionLog)
                    {
                        _logger.LogInformation("Scheduler starts triggering job of <{JobId}>, {Trigger}.", jobId, jobTrigger.ToString());
                    }

                    // 创建共享上下文数据对象
                    var properties = new Dictionary<object, object>();

                    // 创建执行前上下文
                    var jobExecutingContext = new JobExecutingContext(jobId, properties)
                    {
                        ExecutingTime = DateTime.UtcNow
                    };

                    // 执行异常对象
                    InvalidOperationException? executionException = default;

                    try
                    {
                        // 调用执行前监视器
                        if (Monitor != default)
                        {
                            await Monitor.OnExecutingAsync(jobExecutingContext, stoppingToken);
                        }

                        // 判断是否自定义了执行器
                        if (Executor == default)
                        {
                            await jobHandler.ExecuteAsync(jobExecutingContext, stoppingToken);
                        }
                        else
                        {
                            await Executor.ExecuteAsync(jobExecutingContext, jobHandler, stoppingToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 输出异常日志
                        _logger.LogError(ex, "Error occurred executing of <{JobId}>.", jobId);

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
                        // 输出触发完成日志
                        if (jobDetail!.WithExecutionLog)
                        {
                            _logger.LogInformation("Scheduler completes a job of <{JobId}> trigger.", jobId);
                        }

                        // 调用执行后监视器
                        if (Monitor != default)
                        {
                            // 创建执行后上下文
                            var jobExecutedContext = new JobExecutedContext(jobId, properties)
                            {
                                ExecutedTime = DateTime.UtcNow,
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
         * 为了避免程序在未知情况下存在不必要的耗时操作从而导致事件出现偏差
         * 所以这里采用 DateTimeKind.Unspecified 转换当前时间并忽略毫秒部分
         */
        var referenceTime = DateTime.UtcNow;
        var unspecifiedTime = new DateTime(referenceTime.Year, referenceTime.Month, referenceTime.Day, referenceTime.Hour, referenceTime.Minute, referenceTime.Second);

        // 查找下一次符合触发时机的所有作业触发器
        var closestJobTriggers = _schedulerJobs.Where(u => IsEffectiveJob(u.JobDetail))
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
            LastSyncTime = DateTime.UtcNow;

            // 同步存储器作业数据
            _ = SynchronizationStorer(TimeSpan.FromMilliseconds(syncTimeout), stoppingToken);
        }

        // 避免刚好触发的情况
        if (interval > 0)
        {
            // 将当前线程休眠至下一次触发前，采用 Math.Floor 向下取整，也就是可以休眠到执行前
            await Task.Delay(TimeSpan.FromMilliseconds(interval), stoppingToken);
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
}