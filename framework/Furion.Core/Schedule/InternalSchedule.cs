// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.Schedule;

/// <summary>
/// Schedule 模块内部实现
/// </summary>
internal sealed class InternalSchedule : ISchedule
{
    /// <summary>
    /// 作业调度器工厂
    /// </summary>
    private readonly ISchedulerFactory _schedulerFactory;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="schedulerFactory">作业调度器工厂</param>
    public InternalSchedule(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    /// <summary>
    /// 动态添加作业
    /// </summary>
    /// <typeparam name="TJob"><see cref="IJob"/> 实现类</typeparam>
    /// <param name="configureSchedulerJobBuilder">调度作业构建器委托</param>
    public void AddJob<TJob>(Action<SchedulerJobBuilder> configureSchedulerJobBuilder)
         where TJob : class, IJob
    {
        AddJob(typeof(TJob), configureSchedulerJobBuilder);
    }

    /// <summary>
    /// 动态添加作业
    /// </summary>
    /// <param name="jobType">作业类型</param>
    /// <param name="configureSchedulerJobBuilder">调度作业构建器委托</param>
    public void AddJob(Type jobType, Action<SchedulerJobBuilder> configureSchedulerJobBuilder)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configureSchedulerJobBuilder);

        // jobType 须实现 IJob 接口
        if (!typeof(IJob).IsAssignableFrom(jobType))
        {
            throw new InvalidOperationException("The <jobType> does not implement <IJob> interface.");
        }

        // 创建调度作业对象
        var schedulerJobBuilder = new SchedulerJobBuilder(jobType);

        // 调用委托
        configureSchedulerJobBuilder(schedulerJobBuilder);

        // 解析作业实例
        var schedulerJob = schedulerJobBuilder.Build(DateTime.UtcNow);

        // 将作业调度器添加到作业调度器工厂中
        _schedulerFactory.AddSchedulerJob(schedulerJob);
    }

    /// <summary>
    /// 尝试删除作业
    /// </summary>
    /// <param name="jobId">作业唯一 Id</param>
    /// <returns><see cref="bool"/></returns>
    public bool TryRemoveJob(string jobId)
    {
        return _schedulerFactory.TryRemoveSchedulerJob(jobId, out _);
    }

    /// <summary>
    /// 根据作业 Id 获取作业调度器
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <returns><see cref="ISchedulerJob"/></returns>
    public ISchedulerJob? GetSchedulerJob(string jobId)
    {
        var isExist = _schedulerFactory.TryGetSchedulerJob(jobId, out var schedulerJob);
        return isExist ? schedulerJob : default;
    }

    /// <summary>
    /// 启动所有作业
    /// </summary>
    public void StartAllJobs()
    {
        _schedulerFactory.StartAllSchedulerJobs();
    }

    /// <summary>
    /// 暂停所有作业
    /// </summary>
    public void PauseAllJobs()
    {
        _schedulerFactory.PauseAllSchedulerJobs();
    }
}