// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.SchedulerTask;

/// <summary>
/// 调度任务包装抽象类
/// </summary>
internal abstract class SchedulerTaskWrapper
{
    /// <summary>
    /// 具体执行任务对象
    /// </summary>
    public IScheduledTask? Task { get; set; }

    /// <summary>
    /// 最近运行时间
    /// </summary>
    public DateTime LastRunTime { get; set; }

    /// <summary>
    /// 下一次运行时间
    /// </summary>
    public DateTime NextRunTime { get; set; }

    /// <summary>
    /// 设置最近运行时间和下一次运行时间增量
    /// </summary>
    public abstract void Increment();

    /// <summary>
    /// 是否开始执行任务
    /// </summary>
    /// <param name="currentTime">当前时间</param>
    /// <returns>bool</returns>
    public virtual bool ShouldRun(DateTime currentTime)
    {
        return NextRunTime < currentTime && LastRunTime != NextRunTime;
    }
}