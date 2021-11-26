// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.SchedulerJob;

/// <summary>
/// 调度作业包装类
/// </summary>
/// <remarks>主要用于主机服务启动时将所有作业和作业触发器进行包装绑定</remarks>
internal sealed class SchedulerJobWrapper
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    internal SchedulerJobWrapper(string jobId)
    {
        JobId = jobId;
    }

    /// <summary>
    /// 解构函数
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <param name="jobHandler">作业</param>
    /// <param name="jobDetail">作业详细信息</param>
    /// <param name="jobTrigger">作业触发器</param>
    /// <param name="triggerString">触发器输出字符串</param>
    internal void Deconstruct(out string jobId
        , out IJob jobHandler
        , out JobDetail? jobDetail
        , out JobTrigger jobTrigger
        , out string? triggerString)
    {
        jobId = JobId;
        jobHandler = Job!;
        jobDetail = JobDetail;
        jobTrigger = Trigger!;
        triggerString = Trigger!.ToString(JobId);
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
    internal JobTrigger? Trigger { get; set; }

    /// <summary>
    /// 作业详细信息
    /// </summary>
    internal JobDetail? JobDetail { get; set; }
}