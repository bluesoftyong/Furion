// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.SchedulerJob;

/// <summary>
/// 作业详细信息
/// </summary>
public interface IJobDetail
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    IJobIdentity Identity { get; }

    /// <summary>
    /// 作业计数器
    /// </summary>
    IJobCounter Counter { get; }

    /// <summary>
    /// 作业状态
    /// </summary>
    JobStatus Status { get; }

    /// <summary>
    /// 作业附加数据
    /// </summary>
    IDictionary<object, object> Properties { get; }
}