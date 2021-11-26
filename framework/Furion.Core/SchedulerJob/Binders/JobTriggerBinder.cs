// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.SchedulerJob;

/// <summary>
/// 作业和作业触发器绑定器
/// </summary>
internal sealed class JobTriggerBinder
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="jobType">作业类型</param>
    /// <param name="trigger">作业触发器</param>
    internal JobTriggerBinder(Type jobType, JobTrigger trigger)
    {
        JobType = jobType;
        Trigger = trigger;
    }

    /// <summary>
    /// 作业类型
    /// </summary>
    internal Type JobType { get; }

    /// <summary>
    /// 作业触发器
    /// </summary>
    internal JobTrigger Trigger { get; }
}