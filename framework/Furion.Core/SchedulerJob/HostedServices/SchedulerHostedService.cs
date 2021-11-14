// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.TimeCrontab;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Furion.SchedulerJob;

/// <summary>
/// 任务调度后台主机服务
/// </summary>
public sealed class SchedulerHostedService : BackgroundService
{
    /// <summary>
    /// 避免由 CLR 的终结器捕获该异常从而终止应用程序，让所有未觉察异常被觉察
    /// </summary>
    internal event EventHandler<UnobservedTaskExceptionEventArgs>? UnobservedTaskException;

    /// <summary>
    /// 日志对象
    /// </summary>
    private readonly ILogger<SchedulerHostedService> _logger;

    /// <summary>
    /// 调度任务集合
    /// </summary>
    private readonly HashSet<JobWrapper> _scheduledTasks = new();

    /// <summary>
    /// 处理程序监视器
    /// </summary>
    private IJobMonitor? Monitor { get; }

    /// <summary>
    /// 处理程序执行器
    /// </summary>
    private IJobExecutor? Executor { get; }

    /// <summary>
    ///
    /// </summary>
    /// <param name="logger">日志对象</param>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="scheduledTasks">调度任务集合</param>
    public SchedulerHostedService(ILogger<SchedulerHostedService> logger
        , IServiceProvider serviceProvider
         , IEnumerable<IJob> scheduledTasks)
    {
        _logger = logger;
        Monitor = serviceProvider.GetService<IJobMonitor>();
        Executor = serviceProvider.GetService<IJobExecutor>();

        var referenceTime = DateTime.UtcNow;

        var bindingAttr = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        foreach (var scheduledTask in scheduledTasks)
        {
            var scheduledTaskType = scheduledTask.GetType();

            var scheduleProperty = scheduledTaskType.GetProperty(nameof(IJob.Schedule), bindingAttr)!;
            var scheduleFormatAttribute = scheduleProperty.IsDefined(typeof(ScheduledAttribute), false)
                ? scheduleProperty.GetCustomAttribute<ScheduledAttribute>(false) :
                default;

            _scheduledTasks.Add(new JobWrapper
            {
                Schedule = Crontab.Parse(scheduledTask.Schedule, scheduleFormatAttribute?.Format ?? CronStringFormat.Default),
                Task = scheduledTask,
                NextRunTime = referenceTime,
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
        _logger.LogInformation("Scheduler Hosted Service is running.");

        // 注册后台主机服务停止监听
        stoppingToken.Register(() =>
            _logger.LogDebug($"Scheduler Hosted Service is stopping."));

        // 监听服务是否取消
        while (!stoppingToken.IsCancellationRequested)
        {
            // 执行具体任务
            await BackgroundProcessing(stoppingToken);

            // 最低限制，不阻塞延迟1分钟检查，不再接受秒级调度任务，避免频繁检查导致 CPU 占用过高
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        _logger.LogCritical($"Scheduler Hosted Service is stopped.");
    }

    /// <summary>
    /// 后台调用事件处理程序
    /// </summary>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        var referenceTime = DateTime.UtcNow;

        // 查找所有到达执行时间的任务
        var tasksThatShouldRun = _scheduledTasks.Where(t => t.ShouldRun(referenceTime));

        // 创建一个任务工厂并保证执行任务都使用当前的计划程序
        var taskFactory = new TaskFactory(TaskScheduler.Current);

        // 逐条创建新线程调用
        foreach (var taskThatShouldRun in tasksThatShouldRun)
        {
            // 记录执行时间增量
            taskThatShouldRun.Increment();

            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            var cancellationToken = cancellationTokenSource.Token;
            cancellationToken.Register(() =>
            {
                _logger.LogInformation("{Identity} is cancel.", "Identity");
            });

            // 创建新的线程执行
            await taskFactory.StartNew(async () =>
            {
                // 执行异常对象
                InvalidOperationException? executionException = default;

                try
                {
                    // 调用执行前监视器
                    if (Monitor != default)
                    {
                        await Monitor.OnExecutingAsync(cancellationToken);
                    }

                    // 判断是否自定义了执行器
                    if (Executor == default)
                    {
                        await taskThatShouldRun.Task!.ExecuteAsync(cancellationToken);
                    }
                    else
                    {
                        await Executor.ExecuteAsync(taskThatShouldRun.Task!, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    // 输出异常日志
                    _logger.LogError(ex, "Error occurred executing {Identity}.", "Identity");

                    // 标记异常
                    executionException = new InvalidOperationException(string.Format("Error occurred executing {0}.", "Identity"), ex);

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
                    // 调用执行后监视器
                    if (Monitor != default)
                    {
                        await Monitor.OnExecutedAsync(executionException, cancellationToken);
                    }
                }
            }, stoppingToken);
        }
    }
}