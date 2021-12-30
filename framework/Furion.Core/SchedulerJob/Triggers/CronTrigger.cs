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
/// Cron 表达式触发器
/// </summary>
internal sealed class CronTrigger : JobTrigger
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="schedule">调度计划（Cron 表达式）</param>
    /// <param name="format">Cron 表达式格式化类型</param>
    public CronTrigger(string schedule, CronStringFormat format = CronStringFormat.Default)
    {
        ScheduleCrontab = Crontab.Parse(schedule, format);
    }

    /// <summary>
    /// 调度计划 <see cref="Crontab"/> 对象
    /// </summary>
    private Crontab ScheduleCrontab { get; }

    /// <summary>
    /// 计算当前触发器增量信息
    /// </summary>
    public override void Increment()
    {
        NumberOfRuns++;
        LastRunTime = NextRunTime;
        NextRunTime = ScheduleCrontab.GetNextOccurrence(NextRunTime);
    }

    /// <summary>
    /// 是否符合执行逻辑
    /// </summary>
    /// <param name="currentTime">当前时间</param>
    /// <returns><see cref="bool"/> 实例</returns>
    public override bool ShouldRun(DateTime currentTime)
    {
        return NextRunTime < currentTime && LastRunTime != NextRunTime;
    }

    /// <summary>
    /// 将触发器转换成字符串输出
    /// </summary>
    public override string ToString()
    {
        return ScheduleCrontab.ToString();
    }
}