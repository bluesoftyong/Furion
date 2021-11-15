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
internal sealed class CronTrigger : IJobTrigger, IJobCounter
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="scheduleCrontab">调度计划 <see cref="Crontab"/> 对象</param>
    internal CronTrigger(Crontab scheduleCrontab)
    {
        ScheduleCrontab = scheduleCrontab;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="rates">速率</param>
    /// <param name="scheduleCrontab">调度计划 <see cref="Crontab"/> 对象</param>
    internal CronTrigger(TimeSpan rates, Crontab scheduleCrontab)
        : this(scheduleCrontab)
    {
        Rates = rates;
    }

    /// <summary>
    /// 调度计划 <see cref="Crontab"/> 对象
    /// </summary>
    private Crontab ScheduleCrontab { get; }

    /// <summary>
    /// 速率
    /// </summary>
    public TimeSpan Rates { get; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// 最近运行时间
    /// </summary>
    public DateTime LastRunTime { get; private set; }

    /// <summary>
    /// 下一次运行时间
    /// </summary>
    public DateTime NextRunTime { get; private set; }

    /// <summary>
    /// 运行次数
    /// </summary>
    public long NumberOfRuns { get; private set; }

    /// <summary>
    /// 增量
    /// </summary>
    public void Increment()
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
    public bool ShouldRun(DateTime currentTime)
    {
        return NextRunTime < currentTime && LastRunTime != NextRunTime;
    }
}