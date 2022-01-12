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
public class JobDetail
{
    /// <summary>
    /// 作业 Id
    /// </summary>
    public string? JobId { get; internal set; }

    /// <summary>
    /// 作业类型完整限定名
    /// </summary>
    public string? JobType { get; internal set; }

    /// <summary>
    /// 作业类型所在程序集名称
    /// </summary>
    public string? AssemblyName { get; internal set; }

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
    public JobStartMode StartMode { get; internal set; } = JobStartMode.Run;

    /// <summary>
    /// 作业锁方式
    /// </summary>
    public JobLockMode LockMode { get; internal set; } = JobLockMode.Parallel;

    /// <summary>
    /// 是否打印执行日志
    /// </summary>
    public bool WithExecutionLog { get; internal set; } = false;

    /// <summary>
    /// 是否创建新的服务作用域执行作业
    /// </summary>
    public bool WithScopeExecution { get; internal set; } = false;
}