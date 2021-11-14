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
public interface IJobTrigger
{
    /// <summary>
    /// 心跳率
    /// </summary>
    int HeartRate { get; }

    /// <summary>
    /// 触发增量
    /// </summary>
    void Increment();

    /// <summary>
    /// 是否符合条件执行处理程序
    /// </summary>
    /// <param name="currentTime">当前时间</param>
    /// <returns><see cref="bool"/> 实例</returns>
    bool ShouldRun(DateTime currentTime);
}