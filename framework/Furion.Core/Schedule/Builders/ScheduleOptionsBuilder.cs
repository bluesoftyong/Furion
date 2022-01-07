// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;

namespace Furion.Schedule;

/// <summary>
/// Schedule 模块配置选项构建器
/// </summary>
public sealed class ScheduleOptionsBuilder
{
    /// <summary>
    /// 作业调度器构建器集合
    /// </summary>
    private readonly IList<SchedulerJobBuilder> _schedulerJobBuilders;

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
    public ScheduleOptionsBuilder()
    {
        _schedulerJobBuilders = new List<SchedulerJobBuilder>();
    }

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
    /// <param name="configureSchedulerJobBuilder">调度作业构建器委托</param>
    /// <returns><see cref="ScheduleOptionsBuilder"/></returns>
    public ScheduleOptionsBuilder AddJob<TJob>(Action<SchedulerJobBuilder> configureSchedulerJobBuilder)
        where TJob : class, IJob
    {
        return AddJob(typeof(TJob), configureSchedulerJobBuilder);
    }

    /// <summary>
    /// 注册作业
    /// </summary>
    /// <param name="jobType">作业类型</param>
    /// <param name="configureSchedulerJobBuilder">调度作业构建器委托</param>
    /// <returns><see cref="ScheduleOptionsBuilder"/></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ScheduleOptionsBuilder AddJob(Type jobType, Action<SchedulerJobBuilder> configureSchedulerJobBuilder)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configureSchedulerJobBuilder);

        // jobType 须实现 IJob 接口
        if (!typeof(IJob).IsAssignableFrom(jobType))
        {
            throw new InvalidOperationException("The <jobType> does not implement <IJob> interface.");
        }

        // 创建作业调度器对象
        var schedulerJobBuilder = new SchedulerJobBuilder(jobType);

        // 外部调用
        configureSchedulerJobBuilder(schedulerJobBuilder);

        // 添加到作业调度器构建器集合中
        _schedulerJobBuilders.Add(schedulerJobBuilder);

        return this;
    }

    /// <summary>
    /// 注册作业监视器
    /// </summary>
    /// <typeparam name="TJobMonitor">实现自 <see cref="IJobMonitor"/></typeparam>
    /// <returns><see cref="ScheduleOptionsBuilder"/> 实例</returns>
    public ScheduleOptionsBuilder AddMonitor<TJobMonitor>()
        where TJobMonitor : class, IJobMonitor
    {
        _jobMonitor = typeof(TJobMonitor);
        return this;
    }

    /// <summary>
    /// 注册作业执行器
    /// </summary>
    /// <typeparam name="TJobExecutor">实现自 <see cref="IJobExecutor"/></typeparam>
    /// <returns><see cref="ScheduleOptionsBuilder"/> 实例</returns>
    public ScheduleOptionsBuilder AddExecutor<TJobExecutor>()
        where TJobExecutor : class, IJobExecutor
    {
        _jobExecutor = typeof(TJobExecutor);
        return this;
    }

    /// <summary>
    /// 构建调度作业配置选项
    /// </summary>
    /// <param name="services">服务集合对象</param>
    /// <returns>作业调度器构建器集合</returns>
    internal IEnumerable<SchedulerJobBuilder> Build(IServiceCollection services)
    {
        var schedulerJobBuilders = _schedulerJobBuilders;

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

        return schedulerJobBuilders;
    }
}