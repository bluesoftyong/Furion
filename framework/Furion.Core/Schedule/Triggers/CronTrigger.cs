// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.TimeCrontab;

namespace Furion.Schedule;

/// <summary>
/// Cron 作业触发器
/// </summary>
internal sealed class CronTrigger : JobTrigger
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="schedule">Cron 表达式</param>
    /// <param name="format">Cron 表达式格式化类型</param>
    public CronTrigger(string schedule, int format)
    {
        ScheduleCrontab = Crontab.Parse(schedule, (CronStringFormat)format);
    }

    /// <summary>
    /// <see cref="Crontab"/> 对象
    /// </summary>
    private Crontab ScheduleCrontab { get; }

    /// <summary>
    /// 计算下一个触发时间
    /// </summary>
    /// <param name="startAt">起始时间</param>
    /// <returns><see cref="DateTime"/>?</returns>
    public override DateTime? GetNextOccurrence(DateTime? startAt)
    {
        return startAt == null ? null : ScheduleCrontab.GetNextOccurrence(startAt.Value);
    }

    /// <summary>
    /// 执行条件检查
    /// </summary>
    /// <param name="checkTime">受检时间</param>
    /// <returns><see cref="bool"/></returns>
    public override bool ShouldRun(DateTime checkTime)
    {
        return NextRunTime != null
            && NextRunTime.Value < checkTime
            && LastRunTime != NextRunTime;
    }

    /// <summary>
    /// 作业触发器转字符串输出
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public override string ToString()
    {
        return ScheduleCrontab.ToString();
    }
}