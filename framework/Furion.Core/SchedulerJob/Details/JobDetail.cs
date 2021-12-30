// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.SchedulerJob;

/// <summary>
/// 作业详细信息基类
/// </summary>
public abstract class JobDetail
{
    /// <summary>
    /// 作业 Id
    /// </summary>
    public virtual string? JobId { get; set; }

    /// <summary>
    /// 作业描述
    /// </summary>
    public virtual string? Description { get; set; }

    /// <summary>
    /// 作业状态
    /// </summary>
    public virtual JobStatus Status { get; set; } = JobStatus.Normal;

    /// <summary>
    /// 作业执行方式
    /// </summary>
    public virtual JobMode Mode { get; set; } = JobMode.Parallel;

    /// <summary>
    /// 记录详细日志
    /// </summary>
    public virtual bool WithExecutionLog { get; set; } = false;
}