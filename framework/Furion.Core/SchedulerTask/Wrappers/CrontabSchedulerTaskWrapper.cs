// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.TimeCrontab;

namespace Furion.SchedulerTask;

/// <summary>
/// Cron 调度任务包装类
/// </summary>
internal sealed class CrontabSchedulerTaskWrapper : SchedulerTaskWrapper
{
    /// <summary>
    /// Cron 表达式
    /// </summary>
    internal CrontabSchedule? Schedule { get; set; }

    /// <summary>
    /// 设置最近运行时间和下一次运行时间增量
    /// </summary>
    internal override void Increment()
    {
        LastRunTime = NextRunTime;
        NextRunTime = Schedule!.GetNextOccurrence(NextRunTime);
    }
}