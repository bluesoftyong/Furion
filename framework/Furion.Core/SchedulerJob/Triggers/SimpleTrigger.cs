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
internal sealed class SimpleTrigger : JobTrigger
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="interval">间隔时间（毫秒）</param>
    public SimpleTrigger(int interval)
    {
        Interval = interval;
    }

    /// <summary>
    /// 间隔时间（毫秒）
    /// </summary>
    private int Interval { get; }

    /// <summary>
    /// 计算当前触发器增量信息
    /// </summary>
    public override void Increment()
    {
        NumberOfRuns++;
        LastRunTime = NextRunTime;
        NextRunTime = NextRunTime.AddMilliseconds(Interval);
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
        return $"{Interval}ms";
    }
}