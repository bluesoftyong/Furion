// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.JobScheduler;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// JobScheduler 模块服务拓展
/// </summary>
public static class SchedulerJobServiceCollectionExtensions
{
    /// <summary>
    /// 添加 JobScheduler 模块注册
    /// </summary>
    /// <param name="services">服务集合对象</param>
    /// <param name="configureOptionsBuilder">调度作业配置选项构建器委托</param>
    /// <returns>服务集合实例</returns>
    public static IServiceCollection AddJobScheduler(this IServiceCollection services, Action<JobSchedulerOptionsBuilder> configureOptionsBuilder)
    {
        // 创建初始调度作业配置选项构建器
        var jobSchedulerOptionsBuilder = new JobSchedulerOptionsBuilder();
        configureOptionsBuilder.Invoke(jobSchedulerOptionsBuilder);

        return services.AddJobScheduler(jobSchedulerOptionsBuilder);
    }

    /// <summary>
    /// 添加 JobScheduler 模块注册
    /// </summary>
    /// <param name="services">服务集合对象</param>
    /// <param name="jobSchedulerOptionsBuilder">调度作业配置选项构建器</param>
    /// <returns>服务集合实例</returns>
    public static IServiceCollection AddJobScheduler(this IServiceCollection services, JobSchedulerOptionsBuilder? jobSchedulerOptionsBuilder = default)
    {
        // 初始化调度作业配置项
        jobSchedulerOptionsBuilder ??= new JobSchedulerOptionsBuilder();

        // 注册内部服务
        services.AddInternalService();

        // 构建作业调度器集合
        var schedulerJobBuilders = jobSchedulerOptionsBuilder.Build(services);

        // 通过工厂模式创建
        services.AddHostedService(serviceProvider =>
        {
            // 创建调度器工厂后台服务对象
            var schedulerFactoryHostedService = ActivatorUtilities.CreateInstance<SchedulerFactoryHostedService>(serviceProvider
                , schedulerJobBuilders
                , jobSchedulerOptionsBuilder.TimeBeforeSync
                , jobSchedulerOptionsBuilder.MinimumSyncInterval);

            // 订阅未察觉任务异常事件
            var unobservedTaskExceptionHandler = jobSchedulerOptionsBuilder.UnobservedTaskExceptionHandler;
            if (unobservedTaskExceptionHandler != default)
            {
                schedulerFactoryHostedService.UnobservedTaskException += unobservedTaskExceptionHandler;
            }

            return schedulerFactoryHostedService;
        });

        return services;
    }

    /// <summary>
    /// 注册内部服务
    /// </summary>
    /// <param name="services">服务集合对象</param>
    /// <returns>服务集合实例</returns>
    private static IServiceCollection AddInternalService(this IServiceCollection services)
    {
        // 注册作业存储器，采用工厂方式创建
        services.AddSingleton<IJobStorer>(_ =>
        {
            // 创建基于运行时内存作业存储实现
            return new RuntimeJobStorer();
        });

        // 注册作业工厂
        services.AddSingleton<ISchedulerFactory, SchedulerFactory>();

        // 注册作业调度器
        services.AddSingleton<IScheduler, Scheduler>();

        return services;
    }
}