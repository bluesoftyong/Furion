// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Furion.EventBus;

internal sealed class EventBusHostedService : BackgroundService
{
    /// <summary>
    /// 避免由 CLR 的终结器捕获该异常从而终止应用程序，让所有未觉察异常被觉察
    /// </summary>
    internal event EventHandler<UnobservedTaskExceptionEventArgs>? UnobservedTaskException;

    /// <summary>
    /// 日志对象
    /// </summary>
    private readonly ILogger<EventBusHostedService> _logger;

    private readonly IEventChannel _eventChannel;

    /// <summary>
    /// 事件处理程序集合
    /// </summary>
    private readonly HashSet<EventHandlerWrapper> _eventHandlers = new();

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志对象</param>
    /// <param name="eventChannel"></param>
    /// <param name="eventHandlers"></param>
    public EventBusHostedService(ILogger<EventBusHostedService> logger
        , IEventChannel eventChannel
        , IEnumerable<IEventHandler> eventHandlers)
    {
        _logger = logger;
        _eventChannel = eventChannel;

        // 添加事件处理程序到 HashSet 中
        foreach (var eventHandler in eventHandlers)
        {
            // 查找所有公开、非静态实例方法，且贴有 [EventSource] 特性，且符合 Func<EventSource, CancellationToken, Task> 签名
            var eventHandlerType = eventHandler.GetType();
            // 这里粗略判断
            var methods = eventHandlerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(u => u.IsDefined(typeof(EventSubscribeAttribute), false));

            foreach (var method in methods)
            {
                var func = method.CreateDelegate<Func<EventSource, CancellationToken, Task>>(eventHandler);
                var eventSubscribeAttributes = method.GetCustomAttributes<EventSubscribeAttribute>(false);

                foreach (var eventSource in eventSubscribeAttributes)
                {
                    _eventHandlers.Add(new EventHandlerWrapper(eventSource.EventId) { Handler = func });
                }
            }
        }
    }

    /// <summary>
    /// 执行后台任务
    /// </summary>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EventBus Hosted Service is running.");

        // 注册后台主机服务停止监听
        stoppingToken.Register(() =>
            _logger.LogDebug($"EventBus Hosted Service is stopping."));

        // 监听服务是否取消
        while (!stoppingToken.IsCancellationRequested)
        {
            // 执行具体任务
            await BackgroundProcessing(stoppingToken);

            // 最低限制，不阻塞延迟1分钟检查，不再接受秒级调度任务，避免频繁检查导致 CPU 占用过高
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        _logger.LogDebug($"EventBus Hosted Service is stopped.");
    }

    /// <summary>
    /// 后台调用具体任务
    /// </summary>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        // 创建一个任务工厂
        var taskFactory = new TaskFactory(TaskScheduler.Current);

        var eventSource = await _eventChannel.ReadAsync(stoppingToken);
        var handlerTahtShouldRun = _eventHandlers.Where(t => t.ShouldRun(eventSource.EventId));

        // 逐条创建新线程调用
        foreach (var handlerThatShouldRun in handlerTahtShouldRun)
        {
            // 创建新的线程执行
            await taskFactory.StartNew(async () =>
            {
                try
                {
                    // 调用任务处理程序（这里的 Token 有点问题）
                    await handlerThatShouldRun.Handler!(eventSource, eventSource.CancellationToken);
                }
                catch (Exception ex)
                {
                    // 捕获 Task 任务异常信息并统计所有异常
                    var args = new UnobservedTaskExceptionEventArgs(
                            ex as AggregateException ?? new AggregateException(ex));

                    UnobservedTaskException?.Invoke(this, args);

                    // 输出异常日志
                    _logger.LogError(ex, "Error occurred executing {Task}.", handlerThatShouldRun.Handler!.ToString());
                }
            }, stoppingToken);
        }
    }
}