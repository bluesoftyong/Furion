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
/// 调度任务细节配置
/// </summary>
/// <remarks>默认 Cron 格式化类型为 <see cref="CronStringFormat.Default"/></remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ScheduledAttribute : Attribute
{
    /// <summary>
    /// 任务唯一标识
    /// </summary>
    private string Identity { get; set; }

    /// <summary>
    /// 调度计划
    /// </summary>
    public string Schedule { get; set; }

    /// <summary>
    /// 任务描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Cron 表达式类型格式化
    /// </summary>
    public CronStringFormat Format { get; set; }
}