// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.Schedule;

/// <summary>
/// 作业调度器
/// </summary>
internal sealed class SchedulerJob
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="jobType">作业类型</param>
    /// <param name="jobDetail">作业信息</param>
    /// <param name="triggers">作业触发器</param>
    internal SchedulerJob(Type jobType, JobDetail jobDetail, IList<JobTrigger> triggers)
    {
        JobType = jobType;
        JobDetail = jobDetail;
        Triggers = triggers;
    }

    /// <summary>
    /// 解构函数
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <param name="jobType">作业类型</param>
    /// <param name="jobDetail">作业信息对象</param>
    /// <param name="jobTriggers">作业触发器</param>
    internal void Deconstruct(out string jobId
        , out Type jobType
        , out JobDetail jobDetail
        , out IList<JobTrigger> jobTriggers)
    {
        jobId = JobDetail.JobId!;
        jobType = JobType;
        jobDetail = JobDetail;
        jobTriggers = Triggers!;
    }

    /// <summary>
    /// 作业类型
    /// </summary>
    internal Type JobType { get; }

    /// <summary>
    /// 作业触发器集合
    /// </summary>
    internal IList<JobTrigger> Triggers { get; }

    /// <summary>
    /// 作业信息
    /// </summary>
    internal JobDetail JobDetail { get; }

    /// <summary>
    /// 查看最早触发时间
    /// </summary>
    /// <returns></returns>
    internal DateTime? GetEarliestNextRunTime()
    {
        if (Triggers.Count == 0) return null;

        // 查看最早触发记录
        return Triggers.Min(u => u.NextRunTime);
    }
}