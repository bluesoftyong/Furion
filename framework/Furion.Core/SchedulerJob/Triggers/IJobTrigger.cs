// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.SchedulerJob;

/// <summary>
/// 作业触发器
/// </summary>
public interface IJobTrigger : IJobCounter
{
    /// <summary>
    /// 速率
    /// </summary>
    /// <remarks>
    /// <para>对于周期任务，速率表示 Interval（间隔时间）</para>
    /// <para>对于 Cron 表达式任务，速率表示 Delay（轮询时间）</para>
    /// </remarks>
    TimeSpan Rates { get; }

    /// <summary>
    /// 增量
    /// </summary>
    void Increment();

    /// <summary>
    /// 是否符合执行逻辑
    /// </summary>
    /// <param name="identity">作业标识器</param>
    /// <param name="currentTime">当前时间</param>
    /// <returns><see cref="bool"/> 实例</returns>
    bool ShouldRun(IJobIdentity identity, DateTime currentTime);
}