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
/// 调度任务包装类
/// </summary>
/// <remarks>主要用于主机服务启动时讲所有调度任务进行包装绑定</remarks>
internal sealed class SchedulerTaskWrapper
{
    /// <summary>
    /// Crontab 调度对象
    /// </summary>
    internal Crontab? Schedule { get; set; }

    /// <summary>
    /// 任务处理程序
    /// </summary>
    internal IScheduledTask? Task { get; set; }

    /// <summary>
    /// 最近运行时间
    /// </summary>
    internal DateTime LastRunTime { get; set; }

    /// <summary>
    /// 下一次运行时间
    /// </summary>
    internal DateTime NextRunTime { get; set; }

    /// <summary>
    /// 执行次数
    /// </summary>
    internal long Times { get; set; }

    /// <summary>
    /// 设置最近运行时间和下一次运行时间增量
    /// </summary>
    internal void Increment()
    {
        Times++;
        LastRunTime = NextRunTime;
        NextRunTime = Schedule!.GetNextOccurrence(NextRunTime);
    }

    /// <summary>
    /// 是否开始执行任务
    /// </summary>
    /// <param name="currentTime">当前时间</param>
    /// <returns>bool</returns>
    internal bool ShouldRun(DateTime currentTime)
    {
        return NextRunTime < currentTime && LastRunTime != NextRunTime;
    }
}