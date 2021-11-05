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
/// 作业包装类
/// </summary>
internal sealed class JobWrapper
{
    /// <summary>
    /// 调度计划的 <see cref="Crontab"/> 对象
    /// </summary>
    internal Crontab? Schedule { get; set; }

    /// <summary>
    /// 调度任务
    /// </summary>
    internal IJob? Task { get; set; }

    /// <summary>
    /// 最近运行时间
    /// </summary>
    internal DateTime LastRunTime { get; set; }

    /// <summary>
    /// 下一次运行时间
    /// </summary>
    internal DateTime NextRunTime { get; set; }

    /// <summary>
    /// 运行次数
    /// </summary>
    internal long NumberOfRuns { get; set; }

    /// <summary>
    /// 调度任务统计增量
    /// </summary>
    internal void Increment()
    {
        NumberOfRuns++;
        LastRunTime = NextRunTime;
        NextRunTime = Schedule!.GetNextOccurrence(NextRunTime);
    }

    /// <summary>
    /// 是否符合条件执行处理程序
    /// </summary>
    /// <param name="currentTime">当前时间</param>
    /// <returns><see cref="bool"/> 实例</returns>
    internal bool ShouldRun(DateTime currentTime)
    {
        return NextRunTime < currentTime && LastRunTime != NextRunTime;
    }
}