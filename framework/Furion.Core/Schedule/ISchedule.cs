// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.Schedule;

/// <summary>
/// Schedule 模块接口
/// </summary>
public interface ISchedule
{
    /// <summary>
    /// 根据作业 Id 获取作业调度器
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <returns><see cref="ISchedulerJob"/></returns>
    ISchedulerJob? GetJob(string jobId);

    /// <summary>
    /// 动态添加作业
    /// </summary>
    /// <typeparam name="TJob"><see cref="IJob"/> 实现类</typeparam>
    /// <param name="configureSchedulerJobBuilder">调度作业构建器委托</param>
    void AddJob<TJob>(Action<SchedulerJobBuilder> configureSchedulerJobBuilder)
        where TJob : class, IJob;

    /// <summary>
    /// 动态添加作业
    /// </summary>
    /// <param name="jobType">作业类型</param>
    /// <param name="configureSchedulerJobBuilder">调度作业构建器委托</param>
    void AddJob(Type jobType, Action<SchedulerJobBuilder> configureSchedulerJobBuilder);

    /// <summary>
    /// 尝试删除作业
    /// </summary>
    /// <param name="jobId">作业唯一 Id</param>
    /// <returns><see cref="bool"/></returns>
    bool TryRemoveJob(string jobId);

    /// <summary>
    /// 启动所有作业
    /// </summary>
    void StartAllJobs();

    /// <summary>
    /// 暂停所有作业
    /// </summary>
    void PauseAllJobs();
}