﻿// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.JobScheduler;

/// <summary>
/// 作业执行上下文
/// </summary>
public abstract class JobExecutionContext
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <param name="properties">共享上下文数据</param>
    public JobExecutionContext(string jobId, IDictionary<object, object> properties)
    {
        JobId = jobId;
        Properties = properties;
    }

    /// <summary>
    /// 作业 Id
    /// </summary>
    public string JobId { get; }

    /// <summary>
    /// 作业详细信息
    /// </summary>
    public JobDetail? JobDetail { get; }

    /// <summary>
    /// 共享上下文数据
    /// </summary>
    public IDictionary<object, object> Properties { get; }
}