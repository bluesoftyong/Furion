// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Furion.SchedulerJob;

/// <summary>
/// 作业调度器后台主机服务
/// </summary>
internal sealed class SchedulerHostedService : BackgroundService
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
    /// 作业描述器
    /// </summary>
    private IJobDescriptor Descriptor { get; }

    /// <summary>
    /// 作业执行程序
    /// </summary>
    private IJob Job { get; }

    /// <summary>
    /// 作业触发器
    /// </summary>
    private IJobTrigger Trigger { get; }

    /// <summary>
    /// 处理程序监视器
    /// </summary>
    private IJobMonitor? Monitor { get; }

    /// <summary>
    /// 处理程序执行器
    /// </summary>
    private IJobExecutor? Executor { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志对象</param>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="descriptor">作业描述器</param>
    /// <param name="job">作业执行程序</param>
    /// <param name="trigger">作业触发器</param>
    public SchedulerHostedService(ILogger<SchedulerHostedService> logger
        , IServiceProvider serviceProvider
        , IJobDescriptor descriptor
        , IJob job
        , IJobTrigger trigger)
    {
        _logger = logger;
        Descriptor = descriptor;
        Job = job;
        Trigger = trigger;
        Monitor = serviceProvider.GetService<IJobMonitor>();
        Executor = serviceProvider.GetService<IJobExecutor>();
    }

    /// <summary>
    /// 执行后台任务
    /// </summary>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Scheduler of <{Identity} | {Description}> Hosted Service is running.", Descriptor.Identity, Descriptor.Description);

        // 注册后台主机服务停止监听
        stoppingToken.Register(() =>
            _logger.LogDebug("Scheduler of <{Identity} | {Description}> Hosted Service is stopping.", Descriptor.Identity, Descriptor.Description));

        // 监听服务是否取消
        while (!stoppingToken.IsCancellationRequested)
        {
            // 执行具体作业
            await BackgroundProcessing(stoppingToken);

            // 在指定速率内检查
            await Task.Delay(Trigger.Rates, stoppingToken);
        }

        _logger.LogCritical("Scheduler of <{Identity} | {Description}> Hosted Service is stopped.", Descriptor.Identity, Descriptor.Description);
    }

    /// <summary>
    /// 后台调用作业处理程序
    /// </summary>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        var referenceTime = DateTime.UtcNow;

        // 判断是否符合执行作业时机
        if (!Trigger.ShouldRun(referenceTime))
        {
            return;
        }

        // 创建一个任务工厂并保证作业处理程序使用当前的计划程序
        var taskFactory = new TaskFactory(TaskScheduler.Current);

        // 计算下一个触发时机
        Trigger.Increment();

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
                    await Monitor.OnExecutingAsync(stoppingToken);
                }

                // 判断是否自定义了执行器
                if (Executor == default)
                {
                    await Job.ExecuteAsync(stoppingToken);
                }
                else
                {
                    await Executor.ExecuteAsync(Job, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                // 输出异常日志
                _logger.LogError(ex, "Error occurred executing of <{Identity} | {Description}>.", Descriptor.Identity, Descriptor.Description);

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
                    await Monitor.OnExecutedAsync(executionException, stoppingToken);
                }
            }
        }, stoppingToken);
    }
}