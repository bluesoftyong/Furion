// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.TimeCrontab;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Furion.SchedulerJob;

/// <summary>
/// 调度作业配置选项构建器
/// </summary>
public sealed class SchedulerJobOptionsBuilder
{
    /// <summary>
    /// 作业类型集合
    /// </summary>
    private readonly Dictionary<Type, (IJobDescriptor, IJobTrigger)> _jobs = new();

    /// <summary>
    /// 未察觉任务异常事件处理程序
    /// </summary>
    public EventHandler<UnobservedTaskExceptionEventArgs>? UnobservedTaskExceptionHandler { get; set; }

    /// <summary>
    /// 注册作业
    /// </summary>
    /// <typeparam name="TJob">实现自 <see cref="IJob"/></typeparam>
    /// <param name="jobTrigger">作业触发器</param>
    /// <returns><see cref="SchedulerJobOptionsBuilder"/> 实例</returns>
    public SchedulerJobOptionsBuilder AddJob<TJob>(IJobTrigger? jobTrigger = default)
        where TJob : class, IJob
    {
        var jobType = typeof(TJob);

        // 判断是否贴有 [Job] 或其派生类特性
        if (!jobType.IsDefined(typeof(JobAttribute), false))
        {
            throw new InvalidOperationException("The [Job] attribute is not added to the current job.");
        }

        // 获取 [Job] 特性具体类型
        var jobAttribute = jobType.GetCustomAttribute<JobAttribute>(false)!;

        // 创建作业描述器
        IJobDescriptor descriptor = new JobDescriptor(jobAttribute.Identity)
        {
            Description = jobAttribute.Description
        };

        // 创建作业触发器
        IJobTrigger trigger;
        if (jobTrigger != default)
        {
            trigger = jobTrigger;
        }
        else
        {
            if (jobAttribute is CronJobAttribute cronJobAttribute)
            {
                // 解析速率
                var rates = cronJobAttribute.Format == CronStringFormat.WithSeconds || cronJobAttribute.Format == CronStringFormat.WithSecondsAndYears
                    ? TimeSpan.FromSeconds(1)
                    : TimeSpan.FromMinutes(1);

                trigger = new CronTrigger(rates, Crontab.Parse(cronJobAttribute.Schedule, cronJobAttribute.Format))
                {
                    NextRunTime = DateTime.UtcNow
                };
            }
            else if (jobAttribute is SimpleJobAttribute simpleJobAttribute)
            {
                trigger = new SimpleTrigger(TimeSpan.FromMilliseconds(simpleJobAttribute.Interval))
                {
                    NextRunTime = DateTime.UtcNow
                };
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        _jobs.Add(jobType, (descriptor, trigger));
        return this;
    }

    /// <summary>
    /// 构建调度作业配置选项
    /// </summary>
    /// <param name="services">服务集合对象</param>
    internal void Build(IServiceCollection services)
    {
        // 注册事件订阅者
        foreach (var (jobType, (descriptor, trigger)) in _jobs)
        {
            AddJob(services, jobType, descriptor, trigger);
        }
    }

    /// <summary>
    /// 注册作业
    /// </summary>
    /// <param name="services">服务集合对象</param>
    /// <param name="jobType">作业类型</param>
    /// <param name="descriptor">作业描述器</param>
    /// <param name="trigger">作业触发器</param>
    /// <exception cref="InvalidOperationException"></exception>
    private void AddJob(IServiceCollection services, Type jobType, IJobDescriptor descriptor, IJobTrigger trigger)
    {
        // 将作业注册为单例
        services.AddSingleton(jobType);

        // 创建作业调度器
        services.AddHostedService(serviceProvider =>
        {
            var jobScheduler = new JobScheduler(serviceProvider.GetRequiredService<ILogger<JobScheduler>>()
                , serviceProvider
                , descriptor,
                (serviceProvider.GetRequiredService(jobType) as IJob)!
                , trigger);

            // 订阅未察觉任务异常事件
            var unobservedTaskExceptionHandler = UnobservedTaskExceptionHandler;
            if (unobservedTaskExceptionHandler != default)
            {
                jobScheduler.UnobservedTaskException += unobservedTaskExceptionHandler;
            }

            return jobScheduler;
        });
    }
}