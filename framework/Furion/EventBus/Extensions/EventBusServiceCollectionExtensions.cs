// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.EventBus;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// EventBus 模块服务拓展
/// </summary>
public static class EventBusServiceCollectionExtensions
{
    /// <summary>
    /// 添加 EventBus 模块注册
    /// </summary>
    /// <param name="services">服务集合对象</param>
    /// <param name="configuration">配置对象</param>
    /// <returns>服务集合实例</returns>
    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        // 注册 EventStoreChannel 服务
        services.AddEventStoreChannelService(configuration);

        // 注册事件总线后台服务
        services.AddHostedService<EventBusHostedService>();

        return services;
    }

    /// <summary>
    /// 添加 EventBus 模块注册
    /// </summary>
    /// <param name="services">服务集合对象</param>
    /// <<param name="configuration">配置对象</param>
    /// <param name="unobservedTaskExceptionHandler">未察觉任务异常事件处理程序</param>
    /// <returns>服务集合实例</returns>
    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration, EventHandler<UnobservedTaskExceptionEventArgs> unobservedTaskExceptionHandler)
    {
        // 注册 EventStoreChannel 服务
        services.AddEventStoreChannelService(configuration);

        // 通过工厂模式创建
        return services.AddHostedService(serviceProvider =>
        {
            // 创建事件总线后台服务对象
            var eventBusHostedService = ActivatorUtilities.CreateInstance<EventBusHostedService>(serviceProvider);

            // 订阅未察觉任务异常事件
            eventBusHostedService.UnobservedTaskException += unobservedTaskExceptionHandler;

            return eventBusHostedService;
        });
    }

    /// <summary>
    /// 注册 EventStoreChannel 服务
    /// </summary>
    /// <param name="services">服务集合对象</param>
    /// <param name="configuration">配置对象</param>
    /// <returns>服务集合实例</returns>
    private static IServiceCollection AddEventStoreChannelService(this IServiceCollection services, IConfiguration configuration)
    {
        // 注册后台任务队列接口/实例为单例，采用工厂方式创建
        services.AddSingleton<IEventStoreChannel>(_ =>
        {
            // 读取 EventBus 模块配置，并获取队列通道容量，默认为 100
            if (!int.TryParse(configuration[Constants.Keys.Capacity], out var capacity))
            {
                capacity = Constants.Values.Capacity;
            }

            // 创建事件存储器对象
            return new EventStoreChannel(capacity);
        });

        return services;
    }
}