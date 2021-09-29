// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.SchedulerTask;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// SchedulerTask 模块服务拓展
/// </summary>
public static class SchedulerTaskServiceCollectionExtensions
{
    /// <summary>
    /// 添加 SchedulerTask 模块注册
    /// </summary>
    /// <param name="services">服务集合对象</param>
    /// <returns>服务集合实例</returns>
    public static IServiceCollection AddSchedulerTask(this IServiceCollection services)
    {
        return services.AddHostedService<SchedulerTaskHostedService>();
    }

    /// <summary>
    /// 添加 SchedulerTask 模块注册
    /// </summary>
    /// <param name="services">服务集合对象</param>
    /// <param name="unobservedTaskExceptionHandler">未察觉任务异常事件处理程序</param>
    /// <returns>服务集合实例</returns>
    public static IServiceCollection AddScheduler(this IServiceCollection services, EventHandler<UnobservedTaskExceptionEventArgs> unobservedTaskExceptionHandler)
    {
        // 通过工厂模式创建
        return services.AddHostedService(serviceProvider =>
        {
            // 创建服务对象
            var schedulerTaskHostedService = ActivatorUtilities.CreateInstance<SchedulerTaskHostedService>(serviceProvider);

            // 订阅未察觉任务异常事件
            schedulerTaskHostedService.UnobservedTaskException += unobservedTaskExceptionHandler;

            return schedulerTaskHostedService;
        });
    }
}