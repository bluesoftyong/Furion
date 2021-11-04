// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.TimeCrontab;

namespace Furion.Scheduler;

/// <summary>
/// 调度计划特性
/// </summary>
/// <remarks>配置 <see cref="IScheduledTask.Schedule"/> Cron 表达式格式化类型</remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class ScheduleFormatAttribute : Attribute
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="format">Cron 表达式格式化类型</param>
    public ScheduleFormatAttribute(CronStringFormat format)
    {
        Format = format;
    }

    /// <summary>
    /// Cron 表达式格式化类型
    /// </summary>
    public CronStringFormat Format { get; }
}