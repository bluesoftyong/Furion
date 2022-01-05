// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Concurrent;

namespace Furion.JobScheduler;

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
        SchedulerJobBuilders = new();
    }

    /// <summary>
    /// 调度作业构建器集合
    /// </summary>
    internal ConcurrentDictionary<string, SchedulerJobBuilder> SchedulerJobBuilders { get; }

    /// <summary>
    /// 设置调度器休眠后再度被激活前多少ms完成耗时操作
    /// </summary>
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
    /// <typeparam name="TJob"><see cref="IJob"/> 实现类</typeparam>
    /// <param name="jobId">作业 Id</param>
    /// <param name="configureDetailBuilder">作业详情构建器委托</param>
    /// <returns><see cref="SchedulerJobOptionsBuilder"/></returns>
    public SchedulerJobOptionsBuilder AddJob<TJob>(string jobId, Action<SchedulerJobBuilder> configureDetailBuilder)
        where TJob : class, IJob
    {
        return AddJob(jobId, typeof(TJob), configureDetailBuilder);
    }

    /// <summary>
    /// 注册作业
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <param name="jobType">作业类型</param>
    /// <param name="configureDetailBuilder">作业详情构建器委托</param>
    /// <returns><see cref="SchedulerJobOptionsBuilder"/></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public SchedulerJobOptionsBuilder AddJob(string jobId, Type jobType, Action<SchedulerJobBuilder> configureDetailBuilder)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configureDetailBuilder);

        // jobType 须实现 IJob 接口
        if (!typeof(IJob).IsAssignableFrom(jobType))
        {
            throw new InvalidOperationException("The <jobType> does not implement <IJob> interface.");
        }

        // 创建作业详情构建器
        var jobDetailBuilder = new SchedulerJobBuilder(jobId, jobType);

        // 作业 Id 须唯一
        if (!SchedulerJobBuilders.TryAdd(jobId, jobDetailBuilder))
        {
            throw new InvalidOperationException($"The job <{jobId}> has been registered. Repeated registration is prohibited.");
        }

        // 调用委托
        configureDetailBuilder(jobDetailBuilder);

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
        foreach (var jobDetailBuilder in SchedulerJobBuilders.Values)
        {
            services.AddSingleton(typeof(IJob), jobDetailBuilder.JobType);
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