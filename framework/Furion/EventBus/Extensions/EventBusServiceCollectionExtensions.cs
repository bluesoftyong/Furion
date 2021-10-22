// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.EventBus;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// EventBus 模块服务拓展
/// </summary>
public static class EventBusServiceCollectionExtensions
{
    /// <summary>
    /// 注册事件订阅者
    /// </summary>
    /// <param name="services">服务集合对象</param>
    /// <returns>服务集合对象</returns>
    public static IServiceCollection AddEventSubscriber<TEventSubscriber>(this IServiceCollection services)
        where TEventSubscriber : class, IEventSubscriber
    {
        // 将事件订阅者注册为单例
        services.AddSingleton<IEventSubscriber, TEventSubscriber>();

        return services;
    }

    /// <summary>
    /// 添加 EventBus 模块注册
    /// </summary>
    /// <param name="services">服务集合对象</param>
    /// <<param name="configuration">配置对象</param>
    /// <param name="configureOptions">事件总线配置选项委托</param>
    /// <returns>服务集合实例</returns>
    public static IServiceCollection AddEventBus(this IServiceCollection services, Action<EventBusOptions>? configureOptions = default)
    {
        // 创建初始事件总线配置选项
        var eventBusOptions = new EventBusOptions();
        configureOptions?.Invoke(eventBusOptions);

        // 注册内部服务
        services.AddInternalService(eventBusOptions);

        // 通过工厂模式创建
        return services.AddHostedService(serviceProvider =>
        {
            // 创建事件总线后台服务对象
            var eventBusHostedService = ActivatorUtilities.CreateInstance<EventBusHostedService>(serviceProvider);

            // 订阅未察觉任务异常事件
            var unobservedTaskExceptionHandler = eventBusOptions.UnobservedTaskExceptionHandler;
            if (unobservedTaskExceptionHandler != default)
            {
                eventBusHostedService.UnobservedTaskException += unobservedTaskExceptionHandler;
            }

            return eventBusHostedService;
        });
    }

    /// <summary>
    /// 注册内部服务
    /// </summary>
    /// <param name="services">服务集合对象</param>
    /// <param name="eventBusOptions">事件总线配置选项</param>
    /// <returns>服务集合实例</returns>
    private static IServiceCollection AddInternalService(this IServiceCollection services, EventBusOptions eventBusOptions)
    {
        // 注册后台任务队列接口/实例为单例，采用工厂方式创建
        services.AddSingleton<IEventStoreChannel>(_ =>
        {
            // 创建事件存取器对象
            return new EventStoreChannel(eventBusOptions.ChannelCapacity);
        });

        // 注册事件发布者
        services.AddSingleton<IEventPulisher, EventPulisher>();

        return services;
    }
}