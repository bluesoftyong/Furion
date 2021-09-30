// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Furion.SchedulerTask;

/// <summary>
/// 任务调度后台主机服务
/// </summary>
internal sealed class SchedulerTaskHostedService : BackgroundService
{
    /// <summary>
    /// 避免由 CLR 的终结器捕获该异常从而终止应用程序，让所有未觉察异常被觉察
    /// </summary>
    internal event EventHandler<UnobservedTaskExceptionEventArgs>? UnobservedTaskException;

    /// <summary>
    /// 日志对象
    /// </summary>
    private readonly ILogger<SchedulerTaskHostedService> _logger;

    /// <summary>
    /// 定时任务集合
    /// </summary>
    private readonly HashSet<SchedulerTaskWrapper> _scheduledTasks = new();

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志对象</param>
    /// <param name="intervalScheduledTasks">间隔定时任务集合</param>
    /// <param name="cronScheduledTasks">Cron 表达式定时任务集合</param>
    public SchedulerTaskHostedService(ILogger<SchedulerTaskHostedService> logger
        , IEnumerable<IIntervalScheduledTask> intervalScheduledTasks
        , IEnumerable<ICronScheduledTask> cronScheduledTasks)
    {
        _logger = logger;

        var referenceTime = DateTime.UtcNow;

        // 添加间隔任务到 HashSet 中
        foreach (var scheduledTask in intervalScheduledTasks)
        {
            _scheduledTasks.Add(new IntervalSchedulerTaskWrapper
            {
                Interval = scheduledTask.IInterval,
                Task = scheduledTask,
                NextRunTime = referenceTime
            });
        }

        // 添加 Cron 表达式任务到 HashSet 中
        foreach (var scheduledTask in cronScheduledTasks)
        {
            _scheduledTasks.Add(new CronSchedulerTaskWrapper
            {
                Schedule = CrontabSchedule.Parse(scheduledTask.Schedule),
                Task = scheduledTask,
                NextRunTime = referenceTime
            });
        }
    }

    /// <summary>
    /// 执行后台任务
    /// </summary>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns>Task</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SchedulerTask Hosted Service is running.");

        // 注册后台主机服务停止监听
        stoppingToken.Register(() =>
            _logger.LogDebug($"SchedulerTask Hosted Service is stopping."));

        // 监听服务是否取消
        while (!stoppingToken.IsCancellationRequested)
        {
            // 执行具体任务
            await BackgroundProcessing(stoppingToken);

            // 最低限制，不阻塞延迟1分钟检查，不再接受秒级调度任务，避免频繁检查导致 CPU 占用过高
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        _logger.LogDebug($"SchedulerTask Hosted Service is stopped.");
    }

    /// <summary>
    /// 后台调用具体任务
    /// </summary>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns>Task</returns>
    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        var referenceTime = DateTime.UtcNow;

        // 创建一个任务工厂
        var taskFactory = new TaskFactory(TaskScheduler.Current);

        // 获取所有到达执行时间的任务
        var tasksThatShouldRun = _scheduledTasks.Where(t => t.ShouldRun(referenceTime));

        // 逐条创建新线程调用
        foreach (var taskThatShouldRun in tasksThatShouldRun)
        {
            // 记录执行时间增量
            taskThatShouldRun.Increment();

            // 创建新的线程执行
            await taskFactory.StartNew(async () =>
            {
                try
                {
                    // 调用任务处理程序
                    await taskThatShouldRun.Task!.ExecuteAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    // 捕获 Task 任务异常信息并统计所有异常
                    var args = new UnobservedTaskExceptionEventArgs(
                            ex as AggregateException ?? new AggregateException(ex));

                    UnobservedTaskException?.Invoke(this, args);

                    // 输出异常日志
                    _logger.LogError(ex, "Error occurred executing {Task}.", taskThatShouldRun.Task!.ToString());
                }
            }, stoppingToken);
        }
    }
}