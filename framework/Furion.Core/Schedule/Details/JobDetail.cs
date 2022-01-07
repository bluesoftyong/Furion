// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.Schedule;

/// <summary>
/// 作业信息
/// </summary>
public sealed class JobDetail
{
    /// <summary>
    /// 作业 Id
    /// </summary>
    public string? JobId { get; internal set; }

    /// <summary>
    /// 作业类型完整限定名（含程序集名称）
    /// </summary>
    /// <remarks>格式：程序集名称;作业类型完整限定名，如：Furion;Furion.Jobs.MyJob</remarks>
    public string? JobType { get; internal set; }

    /// <summary>
    /// 作业描述信息
    /// </summary>
    public string? Description { get; internal set; }

    /// <summary>
    /// 作业状态
    /// </summary>
    public JobStatus Status { get; internal set; } = JobStatus.Normal;

    /// <summary>
    /// 作业启动方式
    /// </summary>
    public JobStartMode StartMode { get; internal set; } = JobStartMode.Now;

    /// <summary>
    /// 作业执行方式
    /// </summary>
    public JobExecutionMode ExecutionMode { get; internal set; } = JobExecutionMode.Parallel;

    /// <summary>
    /// 是否打印执行日志
    /// </summary>
    public bool WithExecutionLog { get; internal set; } = false;
}