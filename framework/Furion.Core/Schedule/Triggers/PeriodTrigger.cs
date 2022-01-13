// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.Schedule;

/// <summary>
/// 周期（间隔）作业触发器
/// </summary>
internal sealed class PeriodTrigger : JobTrigger
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="interval">间隔（毫秒）</param>
    public PeriodTrigger(int interval)
    {
        Interval = interval;
    }

    /// <summary>
    /// 间隔（毫秒）
    /// </summary>
    private int Interval { get; }

    /// <summary>
    /// 计算下一个触发时间
    /// </summary>
    /// <param name="startAt">起始时间</param>
    /// <returns><see cref="DateTime"/>?</returns>
    public override DateTime? GetNextOccurrence(DateTime? startAt)
    {
        return startAt?.AddMilliseconds(Interval);
    }

    /// <summary>
    /// 执行条件检查
    /// </summary>
    /// <param name="checkTime">受检时间</param>
    /// <returns><see cref="bool"/></returns>
    public override bool ShouldRun(DateTime checkTime)
    {
        return NextRunTime != null
            && NextRunTime.Value < checkTime
            && LastRunTime != NextRunTime;
    }

    /// <summary>
    /// 作业触发器转字符串输出
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public override string ToString()
    {
        return $"{Interval}ms";
    }
}