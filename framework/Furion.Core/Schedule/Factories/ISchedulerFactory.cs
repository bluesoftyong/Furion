// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.Schedule;

/// <summary>
/// 作业调度作业工厂依赖接口
/// </summary>
internal interface ISchedulerFactory
{
    /// <summary>
    /// 添加作业调度器
    /// </summary>
    /// <param name="schedulerJob">调度作业对象</param>
    void AddSchedulerJob(SchedulerJob schedulerJob);

    /// <summary>
    /// 尝试删除作业调度器
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <param name="schedulerJob">作业调度器</param>
    /// <returns><see cref="bool"/></returns>
    bool TryRemoveSchedulerJob(string jobId, out SchedulerJob? schedulerJob);

    /// <summary>
    /// 获取所有作业调度器
    /// </summary>
    /// <returns><see cref="ICollection{T}"/></returns>
    ICollection<SchedulerJob> GetSchedulerJobs();
}