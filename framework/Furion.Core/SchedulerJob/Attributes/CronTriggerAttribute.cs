// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.TimeCrontab;

namespace Furion.SchedulerJob;

/// <summary>
/// Cron 表达式作业触发器特性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class CronTriggerAttribute : JobTriggerAttribute
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="identity">作业唯一标识</param>
    /// <param name="schedule">调度计划（Cron 表达式）</param>
    public CronTriggerAttribute(string identity, string schedule)
        : base(identity)
    {
        Schedule = schedule;
    }

    /// <summary>
    /// 调度计划（Cron 表达式）
    /// </summary>
    public string Schedule { get; set; }

    /// <summary>
    /// Cron 表达式格式化类型
    /// </summary>
    public CronStringFormat Format { get; set; } = CronStringFormat.Default;
}