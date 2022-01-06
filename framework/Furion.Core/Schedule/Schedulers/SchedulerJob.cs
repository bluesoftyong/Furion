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
    /// <param name="job">作业执行程序</param>
    /// <param name="jobDetail">作业信息对象</param>
    /// <param name="jobTriggers">作业触发器</param>
    internal void Deconstruct(out string jobId
        , out IJob job
        , out JobDetail? jobDetail
        , out IList<JobTrigger> jobTriggers)
    {
        jobId = JobId;
        job = Job!;
        jobDetail = JobDetail;
        jobTriggers = Triggers!;
    }

    /// <summary>
    /// 作业 Id
    /// </summary>
    internal string JobId { get; }

    /// <summary>
    /// 作业执行程序
    /// </summary>
    internal IJob? Job { get; set; }

    /// <summary>
    /// 作业触发器集合
    /// </summary>
    internal IList<JobTrigger>? Triggers { get; set; }

    /// <summary>
    /// 作业信息
    /// </summary>
    internal JobDetail? JobDetail { get; set; }
}