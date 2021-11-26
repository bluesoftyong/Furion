// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.TimeCrontab;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Concurrent;
using System.Reflection;

namespace Furion.SchedulerJob;

/// <summary>
/// 调度作业配置选项构建器
/// </summary>
public sealed class SchedulerJobOptionsBuilder
{
    /// <summary>
    /// 作业存储器实现工厂
    /// </summary>
    private Func<IServiceProvider, IJobStorer>? _jobStorerImplementationFactory;

    /// <summary>
    /// 作业监视器
    /// </summary>
    private Type? _jobMonitor;

    /// <summary>
    /// 作业执行器
    /// </summary>
    private Type? _jobExecutor;

    /// <summary>
    /// 构造函数
    /// </summary>
    public SchedulerJobOptionsBuilder()
    {
        JobTriggerBinders = new();
    }

    /// <summary>
    /// 作业和作业触发器绑定器集合
    /// </summary>
    internal ConcurrentDictionary<string, JobTriggerBinder> JobTriggerBinders { get; }

    /// <summary>
    /// 设置调度器休眠后再度被激活前多少ms完成耗时操作
    /// </summary>
    /// <remarks>通常用于同步存储器作业数据</remarks>
    public int TimeBeforeSync { get; set; } = 30;

    /// <summary>
    /// 最小存储器同步间隔（秒）
    /// </summary>
    public int MinimumSyncInterval { get; set; } = 30;

    /// <summary>
    /// 未察觉任务异常处理程序
    /// </summary>
    public EventHandler<UnobservedTaskExceptionEventArgs>? UnobservedTaskExceptionHandler { get; set; }

    /// <summary>
    /// 注册作业
    /// </summary>
    /// <typeparam name="TJob"><see cref="IJob"/> 实例</typeparam>
    /// <param name="jobTrigger">作业触发器</param>
    /// <returns><see cref="SchedulerJobOptionsBuilder"/> 实例</returns>
    public SchedulerJobOptionsBuilder AddJob<TJob>(JobTrigger? jobTrigger = default)
        where TJob : class, IJob
    {
        return AddJob(typeof(TJob), jobTrigger);
    }

    /// <summary>
    /// 注册作业
    /// </summary>
    /// <param name="jobType">作业类型，必须实现 <see cref="IJob"/> 接口</param>
    /// <param name="jobTrigger">作业触发器</param>
    /// <returns><see cref="SchedulerJobOptionsBuilder"/> 实例</returns>
    public SchedulerJobOptionsBuilder AddJob(Type jobType, JobTrigger? jobTrigger = default)
    {
        // jobType 须实现 IJob 接口
        if (!typeof(IJob).IsAssignableFrom(jobType))
        {
            throw new InvalidOperationException("The <jobType> does not implement <IJob> interface.");
        }

        // 判断是否贴有 [Job] 或其派生类特性
        if (!jobType.IsDefined(typeof(JobAttribute), false))
        {
            throw new InvalidOperationException("The [Job] attribute is not added to the current job.");
        }

        // 获取 [Job] 特性具体类型
        var jobAttribute = jobType.GetCustomAttribute<JobAttribute>(false)!;
        var jobId = jobAttribute.JobId;

        // 创建作业触发器
        JobTrigger trigger;
        if (jobTrigger != default)
        {
            trigger = jobTrigger;
        }
        else
        {
            // 将 [CronJob] 特性转换成 CronTrigger 对象
            if (jobAttribute is CronJobAttribute cronJobAttribute)
            {
                trigger = new CronTrigger(Crontab.Parse(cronJobAttribute.Schedule, cronJobAttribute.Format));
            }
            // 将 [SimpleJob] 特性转换成 SimpleTrigger 对象
            else if (jobAttribute is SimpleJobAttribute simpleJobAttribute)
            {
                trigger = new SimpleTrigger(simpleJobAttribute.Interval);
            }
            else
            {
                throw new InvalidOperationException("Job trigger not found.");
            }
        }

        // 作业 Id 须唯一
        if (!JobTriggerBinders.TryAdd(jobId, new JobTriggerBinder(jobType, trigger)))
        {
            throw new InvalidOperationException($"The job <{jobId}> has been registered. Repeated registration is prohibited.");
        }

        return this;
    }

    /// <summary>
    /// 替换作业存储器
    /// </summary>
    /// <param name="implementationFactory">自定义作业存储器工厂</param>
    /// <returns><see cref="SchedulerJobOptionsBuilder"/> 实例</returns>
    public SchedulerJobOptionsBuilder ReplaceStorer(Func<IServiceProvider, IJobStorer> implementationFactory)
    {
        _jobStorerImplementationFactory = implementationFactory;
        return this;
    }

    /// <summary>
    /// 注册作业监视器
    /// </summary>
    /// <typeparam name="TJobMonitor">实现自 <see cref="IJobMonitor"/></typeparam>
    /// <returns><see cref="SchedulerJobOptionsBuilder"/> 实例</returns>
    public SchedulerJobOptionsBuilder AddMonitor<TJobMonitor>()
        where TJobMonitor : class, IJobMonitor
    {
        _jobMonitor = typeof(TJobMonitor);
        return this;
    }

    /// <summary>
    /// 注册作业执行器
    /// </summary>
    /// <typeparam name="TJobExecutor">实现自 <see cref="IJobExecutor"/></typeparam>
    /// <returns><see cref="SchedulerJobOptionsBuilder"/> 实例</returns>
    public SchedulerJobOptionsBuilder AddExecutor<TJobExecutor>()
        where TJobExecutor : class, IJobExecutor
    {
        _jobExecutor = typeof(TJobExecutor);
        return this;
    }

    /// <summary>
    /// 构建调度作业配置选项
    /// </summary>
    /// <param name="services">服务集合对象</param>
    internal void Build(IServiceCollection services)
    {
        // 注册作业
        foreach (var jobTriggerMap in JobTriggerBinders.Values)
        {
            services.AddSingleton(typeof(IJob), jobTriggerMap.JobType);
        }

        // 替换作业存储器
        if (_jobStorerImplementationFactory != default)
        {
            services.Replace(ServiceDescriptor.Singleton(_jobStorerImplementationFactory));
        }

        // 注册作业监视器
        if (_jobMonitor != default)
        {
            services.AddSingleton(typeof(IJobMonitor), _jobMonitor);
        }

        // 注册作业执行器
        if (_jobExecutor != default)
        {
            services.AddSingleton(typeof(IJobExecutor), _jobExecutor);
        }
    }
}