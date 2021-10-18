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

/// <summary>
/// 事件总线后台主机服务
/// </summary>
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

    /// <summary>
    /// 事件存取器
    /// </summary>
    private readonly IEventStoreChannel _eventStoreChannel;

    /// <summary>
    /// 事件处理程序集合
    /// </summary>
    private readonly HashSet<EventHandlerWrapper> _eventHandlers = new();

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志对象</param>
    /// <param name="eventStoreChannel">事件存取器</param>
    /// <param name="eventHandlers">事件处理程序（未包装）</param>
    public EventBusHostedService(ILogger<EventBusHostedService> logger
        , IEventStoreChannel eventStoreChannel
        , IEnumerable<IEventHandler> eventHandlers)
    {
        _logger = logger;
        _eventStoreChannel = eventStoreChannel;

        // 添加事件处理程序到 HashSet 中
        foreach (var eventHandler in eventHandlers)
        {
            // 获取事件处理程序类型
            var eventHandlerType = eventHandler.GetType();

            // 查找所有公开且贴有 [EventSubscribe] 的实例方法
            var bindingAttr = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            var eventHandlerMethods = eventHandlerType.GetMethods(bindingAttr)
                .Where(u => u.IsDefined(typeof(EventSubscribeAttribute), false));

            // 遍历所有处理程序方法
            foreach (var eventHandlerMethod in eventHandlerMethods)
            {
                // 将方法转换成 Func<EventSource, CancellationToken, Task> 委托
                var @delegate = eventHandlerMethod.CreateDelegate<Func<EventSource, CancellationToken, Task>>(eventHandler);

                // 处理同一个事件处理程序支持多个事件 Id 情况
                var eventSubscribeAttribute = eventHandlerMethod.GetCustomAttributes<EventSubscribeAttribute>(false);

                // 逐条包装并添加到 HashSet 集合中
                foreach (var eventSubscribe in eventSubscribeAttribute)
                {
                    _eventHandlers.Add(new EventHandlerWrapper(eventSubscribe.EventId)
                    {
                        Handler = @delegate
                    });
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
        }

        _logger.LogDebug($"EventBus Hosted Service is stopped.");
    }

    /// <summary>
    /// 后台调用事件处理程序
    /// </summary>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        // 创建一个任务工厂
        var taskFactory = new TaskFactory(TaskScheduler.Current);

        // 从事件存取器中读取一条
        var eventSource = await _eventStoreChannel.ReadAsync(stoppingToken);

        // 查找事件 Id 匹配的事件处理程序
        var eventHandlersThatShouldRun = _eventHandlers.Where(t => t.ShouldRun(eventSource.EventId));

        // 逐条创建新线程调用
        foreach (var eventHandlerThatShouldRun in eventHandlersThatShouldRun)
        {
            // 创建新的线程执行
            await taskFactory.StartNew(async () =>
            {
                try
                {
                    // 调用事件处理程序
                    await eventHandlerThatShouldRun.Handler!(eventSource, eventSource.CancellationToken);
                }
                catch (Exception ex)
                {
                    // 捕获 Task 任务异常信息并统计所有异常
                    var args = new UnobservedTaskExceptionEventArgs(
                            ex as AggregateException ?? new AggregateException(ex));

                    UnobservedTaskException?.Invoke(this, args);

                    // 输出异常日志
                    _logger.LogError(ex, "Error occurred executing {Task}.", eventHandlerThatShouldRun.Handler!.ToString());
                }
            }, stoppingToken);
        }
    }
}