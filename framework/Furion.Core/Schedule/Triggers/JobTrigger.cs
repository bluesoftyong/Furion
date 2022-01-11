// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.Schedule;

/// <summary>
/// 作业触发器基类
/// </summary>
public abstract class JobTrigger
{
    /// <summary>
    /// 作业触发器 Id
    /// </summary>
    public string? TriggerId { get; internal set; }

    /// <summary>
    /// 作业触发器类型完整限定名
    /// </summary>
    public string? TriggerType { get; internal set; }

    /// <summary>
    /// 作业触发器类型所在程序集名称
    /// </summary>
    public string? AssemblyName { get; internal set; }

    /// <summary>
    /// 作业触发器参数（JSON 字符串）
    /// </summary>
    /// <remarks>object?[]? 类型</remarks>
    public string? Args { get; internal set; }

    /// <summary>
    /// 作业触发器描述
    /// </summary>
    public string? Description { get; internal set; }

    /// <summary>
    /// 最近运行时间
    /// </summary>
    public DateTime? LastRunTime { get; internal set; }

    /// <summary>
    /// 下一次运行时间
    /// </summary>
    public DateTime? NextRunTime { get; internal set; }

    /// <summary>
    /// 触发次数
    /// </summary>
    public long NumberOfRuns { get; internal set; } = 0;

    /// <summary>
    /// 最大执行次数
    /// </summary>
    /// <remarks>不限制：-1；0：不执行；> 0：大于 0 次</remarks>
    public long MaxNumberOfRuns { get; internal set; } = -1;

    /// <summary>
    /// 出错次数
    /// </summary>
    public long NumberOfErrors { get; internal set; } = 0;

    /// <summary>
    /// 最大出错次数
    /// </summary>
    /// <remarks>小于或等于0：不限制；> 0：大于 0 次</remarks>
    public long MaxNumberOfErrors { get; internal set; } = -1;

    /// <summary>
    /// 作业 Id
    /// </summary>
    public string? JobId { get; internal set; }

    /// <summary>
    /// 是否加入调度计划时自执行一次
    /// </summary>
    public bool ExecuteOnAdded { get; internal set; } = false;

    /// <summary>
    /// 获取下一个触发时间
    /// </summary>
    /// <returns><see cref="DateTime"/></returns>
    public abstract DateTime? GetNextOccurrence();

    /// <summary>
    /// 是否符合执行逻辑
    /// </summary>
    /// <param name="currentTime">当前时间</param>
    /// <returns><see cref="bool"/> 实例</returns>
    public abstract bool ShouldRun(DateTime currentTime);

    /// <summary>
    /// 将触发器转换成字符串输出
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public abstract new string? ToString();

    /// <summary>
    /// 计算当前触发器增量信息
    /// </summary>
    internal void Increment()
    {
        NumberOfRuns++;
        LastRunTime = NextRunTime;
        NextRunTime = GetNextOccurrence();
    }

    /// <summary>
    /// 递增错误次数
    /// </summary>
    internal void IncrementErrors()
    {
        NumberOfErrors++;
    }

    /// <summary>
    /// 是否符合执行逻辑（内部检查）
    /// </summary>
    /// <param name="currentTime">当前时间</param>
    /// <returns><see cref="bool"/> 实例</returns>
    internal bool InternalShouldRun(DateTime currentTime)
    {
        // 最大次数控制
        if (MaxNumberOfRuns == 0 || (MaxNumberOfRuns != -1 && NumberOfRuns >= MaxNumberOfRuns))
        {
            return false;
        }

        // 最大错误数控制
        if (MaxNumberOfErrors > 0 && NumberOfErrors >= MaxNumberOfErrors)
        {
            return false;
        }

        // 调用实现类方法
        return ShouldRun(currentTime);
    }
}