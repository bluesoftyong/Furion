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
    private readonly ISchedulerJobFactory _schedulerFactory;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="schedulerFactory">作业调度器工厂</param>
    public InternalSchedule(ISchedulerJobFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    /// <summary>
    /// 动态添加作业
    /// </summary>
    /// <param name="schedulerJobBuilder">作业调度器构建器</param>
    /// <returns><see cref="bool"/></returns>
    public void AddJob(SchedulerJobBuilder schedulerJobBuilder)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(schedulerJobBuilder);

        // 构建作业调度器对象
        var schedulerJob = schedulerJobBuilder.Build();

        // 将作业调度器添加到作业调度器工厂中
        _schedulerFactory.Append(schedulerJob);
    }

    /// <summary>
    /// 删除作业
    /// </summary>
    /// <param name="jobId">作业唯一 Id</param>
    /// <returns><see cref="bool"/></returns>
    public bool RemoveJob(string jobId)
    {
        return _schedulerFactory.TryRemove(jobId, out _);
    }

    /// <summary>
    /// 根据作业 Id 获取作业调度器
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <returns><see cref="ISchedulerJob"/></returns>
    public ISchedulerJob? GetJob(string jobId)
    {
        var isExist = _schedulerFactory.TryGet(jobId, out var schedulerJob);
        return isExist ? schedulerJob : default;
    }

    /// <summary>
    /// 启动所有作业
    /// </summary>
    public void StartAllJobs()
    {
        _schedulerFactory.StartAll();
    }

    /// <summary>
    /// 暂停所有作业
    /// </summary>
    public void PauseAllJobs()
    {
        _schedulerFactory.PauseAll();
    }
}