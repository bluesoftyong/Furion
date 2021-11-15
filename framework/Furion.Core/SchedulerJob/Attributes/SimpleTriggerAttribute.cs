// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.SchedulerJob;

/// <summary>
/// 周期作业触发器特性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class SimpleTriggerAttribute : JobTriggerAttribute
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="identity">作业唯一标识</param>
    /// <param name="interval">间隔时间</param>
    public SimpleTriggerAttribute(string identity, int interval)
        : base(identity)
    {
        Interval = interval;
    }

    /// <summary>
    /// 间隔时间（毫秒）
    /// </summary>
    public int Interval { get; set; }
}