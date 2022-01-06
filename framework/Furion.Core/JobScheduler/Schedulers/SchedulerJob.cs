// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.JobScheduler;

/// <summary>
/// 调度作业
/// </summary>
public sealed class SchedulerJob
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    internal SchedulerJob(string jobId)
    {
        JobId = jobId;
    }

    /// <summary>
    /// 解构函数
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <param name="jobHandler">作业</param>
    /// <param name="jobDetail">作业详细信息</param>
    /// <param name="jobTriggers">作业触发器</param>
    internal void Deconstruct(out string jobId
        , out IJob jobHandler
        , out JobDetail? jobDetail
        , out IEnumerable<JobTrigger> jobTriggers)
    {
        jobId = JobId;
        jobHandler = Job!;
        jobDetail = JobDetail;
        jobTriggers = Triggers!;
    }

    /// <summary>
    /// 作业 Id
    /// </summary>
    internal string JobId { get; }

    /// <summary>
    /// 作业
    /// </summary>
    internal IJob? Job { get; set; }

    /// <summary>
    /// 作业触发器
    /// </summary>
    internal IEnumerable<JobTrigger>? Triggers { get; set; }

    /// <summary>
    /// 作业详细信息
    /// </summary>
    internal JobDetail? JobDetail { get; set; }
}