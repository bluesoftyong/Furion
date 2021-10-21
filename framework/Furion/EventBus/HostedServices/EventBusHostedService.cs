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
    /// 事件订阅者处理程序集合
    /// </summary>
    private readonly HashSet<EventSubscribeWrapper> _eventSubscribes = new();

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志对象</param>
    /// <param name="eventStoreChannel">事件存取器</param>
    /// <param name="eventSubscribers">事件订阅者集合</param>
    public EventBusHostedService(ILogger<EventBusHostedService> logger
        , IEventStoreChannel eventStoreChannel
        , IEnumerable<IEventSubscriber> eventSubscribers)
    {
        _logger = logger;
        _eventStoreChannel = eventStoreChannel;

        // 逐条获取事件订阅者处理程序并进行包装
        foreach (var eventSubscriber in eventSubscribers)
        {
            // 获取事件订阅者类型
            var eventSubscriberType = eventSubscriber.GetType();

            // 判断并获取事件订阅者过滤器
            var filter = typeof(IEventSubscriberFilter).IsAssignableFrom(eventSubscriberType)
                ? eventSubscriber as IEventSubscriberFilter
                : default;

            // 查找所有公开且贴有 [EventSubscribe] 的实例方法
            var bindingAttr = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            var eventSubscribeMethods = eventSubscriberType.GetMethods(bindingAttr)
                .Where(u => u.IsDefined(typeof(EventSubscribeAttribute), false));

            // 遍历所有事件订阅者处理方法
            foreach (var eventSubscribeMethod in eventSubscribeMethods)
            {
                // 将方法转换成 Func<EventSubscribeExecutingContext, CancellationToken, Task> 委托
                var @delegate = eventSubscribeMethod.CreateDelegate<Func<EventSubscribeExecutingContext, CancellationToken, Task>>(eventSubscriber);

                // 处理同一个事件处理程序支持多个事件 Id 情况
                var eventSubscribeAttributes = eventSubscribeMethod.GetCustomAttributes<EventSubscribeAttribute>(false);

                // 逐条包装并添加到 HashSet 集合中
                foreach (var eventSubscribeAttribute in eventSubscribeAttributes)
                {
                    _eventSubscribes.Add(new EventSubscribeWrapper(eventSubscribeAttribute.EventId)
                    {
                        Handler = @delegate,
                        Filter = filter
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
        // 从事件存取器中读取一条
        var eventSource = await _eventStoreChannel.ReadAsync(stoppingToken);

        // 查找事件 Id 匹配的事件处理程序
        var eventHandlersThatShouldRun = _eventSubscribes.Where(t => t.ShouldRun(eventSource.EventId));

        // 空订阅
        if (!eventHandlersThatShouldRun.Any())
        {
            _logger.LogDebug("Subscriber with event ID <{EventId}> was not found.", eventSource.EventId);

            return;
        }

        // 创建一个任务工厂
        var taskFactory = new TaskFactory(TaskScheduler.Current);

        // 逐条创建新线程调用
        foreach (var eventHandlerThatShouldRun in eventHandlersThatShouldRun)
        {
            // 创建新的线程执行
            await taskFactory.StartNew(async () =>
            {
                // 创建共享上下文数据对象
                var properties = new Dictionary<object, object>();

                // 创建执行前上下文
                var eventSubscribeExecutingContext = new EventSubscribeExecutingContext(eventSource, properties)
                {
                    ExecutingTime = DateTime.UtcNow
                };

                // 执行异常
                InvalidOperationException? executionException = default;

                try
                {
                    // 调用执行前过滤器
                    if (eventHandlerThatShouldRun.Filter != default)
                    {
                        await eventHandlerThatShouldRun.Filter.OnHandlerExecutingAsync(eventSubscribeExecutingContext);
                    }

                    // 调用事件处理程序
                    await eventHandlerThatShouldRun.Handler!(eventSubscribeExecutingContext, eventSource.CancellationToken);
                }
                catch (Exception ex)
                {
                    // 标记异常
                    executionException = new InvalidOperationException(string.Format("Error occurred executing {0}.", eventSource.EventId), ex);

                    // 捕获 Task 任务异常信息并统计所有异常
                    var args = new UnobservedTaskExceptionEventArgs(
                            ex as AggregateException ?? new AggregateException(ex));

                    UnobservedTaskException?.Invoke(this, args);

                    // 输出异常日志
                    _logger.LogError(ex, "Error occurred executing {EventId}.", eventSource.EventId);
                }
                finally
                {
                    // 调用执行后过滤器
                    if (eventHandlerThatShouldRun.Filter != default)
                    {
                        // 创建执行后上下文
                        var eventSubscribeExecutedContext = new EventSubscribeExecutedContext(eventSource, properties)
                        {
                            ExecutedTime = DateTime.UtcNow,
                            Exception = executionException
                        };

                        await eventHandlerThatShouldRun.Filter.OnHandlerExecutedAsync(eventSubscribeExecutedContext);
                    }
                }
            }, stoppingToken);
        }
    }
}