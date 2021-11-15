// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.SchedulerJob;

/// <summary>
/// 周期（间隔）触发器
/// </summary>
internal sealed class SimpleTrigger : JobTriggerBase, IJobTrigger
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="rates">速率</param>
    internal SimpleTrigger(TimeSpan rates)
    {
        Rates = rates;
    }

    /// <summary>
    /// 速率
    /// </summary>
    public TimeSpan Rates { get; }

    /// <summary>
    /// 增量
    /// </summary>
    public void Increment()
    {
        NumberOfRuns++;
        LastRunTime = NextRunTime;
        NextRunTime = NextRunTime.AddMilliseconds(Rates.TotalMilliseconds);
    }

    /// <summary>
    /// 是否符合执行逻辑
    /// </summary>
    /// <param name="identity">作业标识器</param>
    /// <param name="currentTime">当前时间</param>
    /// <returns><see cref="bool"/> 实例</returns>
    public bool ShouldRun(IJobIdentity identity, DateTime currentTime)
    {
        return NextRunTime < currentTime && LastRunTime != NextRunTime;
    }
}