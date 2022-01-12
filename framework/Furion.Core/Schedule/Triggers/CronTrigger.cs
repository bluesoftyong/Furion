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
/// Cron 表达式触发器
/// </summary>
/// <remarks>Cron 表达式解析使用：https://gitee.com/dotnetchina/TimeCrontab</remarks>
internal sealed class CronTrigger : JobTrigger
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="schedule">调度计划（Cron 表达式）</param>
    /// <param name="format">Cron 表达式格式化类型，默认 <see cref="CronStringFormat.Default"/></param>
    public CronTrigger(string schedule, int format = 0)
    {
        ScheduleCrontab = Crontab.Parse(schedule, (CronStringFormat)format);
    }

    /// <summary>
    /// 调度计划 <see cref="Crontab"/> 对象
    /// </summary>
    private Crontab ScheduleCrontab { get; }

    /// <summary>
    /// 获取下一个触发时间
    /// </summary>
    /// <returns><see cref="DateTime"/></returns>
    public override DateTime? GetNextOccurrence()
    {
        return NextRunTime == null ? null : ScheduleCrontab.GetNextOccurrence(NextRunTime.Value);
    }

    /// <summary>
    /// 是否符合执行逻辑
    /// </summary>
    /// <param name="baseTime">起始时间</param>
    /// <returns><see cref="bool"/> 实例</returns>
    public override bool ShouldRun(DateTime baseTime)
    {
        return NextRunTime != null && NextRunTime.Value < baseTime && LastRunTime != NextRunTime;
    }

    /// <summary>
    /// 将触发器转换成字符串输出
    /// </summary>
    public override string ToString()
    {
        return ScheduleCrontab.ToString();
    }
}