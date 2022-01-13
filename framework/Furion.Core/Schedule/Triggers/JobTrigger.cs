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
    /// 作业触发器类型
    /// </summary>
    /// <remarks>存储的是类型的 FullName</remarks>
    public string? TriggerType { get; internal set; }

    /// <summary>
    /// 作业触发器类型所在程序集
    /// </summary>
    /// <remarks>存储的是程序集 Name</remarks>
    public string? AssemblyName { get; internal set; }

    /// <summary>
    /// 作业触发器参数
    /// </summary>
    /// <remarks>运行时将反序列化为 object?[]? 类型并作为构造函数参数</remarks>
    public string? Args { get; internal set; }

    /// <summary>
    /// 描述信息
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
    public long NumberOfRuns { get; internal set; }

    /// <summary>
    /// 最大触发次数
    /// </summary>
    /// <remarks>
    /// <para>-1：不限制</para>
    /// <para>0：不执行</para>
    /// <para>>0：N 次</para>
    /// </remarks>
    public long MaxNumberOfRuns { get; internal set; }

    /// <summary>
    /// 出错次数
    /// </summary>
    public long NumberOfErrors { get; internal set; }

    /// <summary>
    /// 最大出错次数
    /// </summary>
    /// <remarks>
    /// <para>lt/eq 0：不限制</para>
    /// <para>>0：N 次</para>
    /// </remarks>
    public long MaxNumberOfErrors { get; internal set; }

    /// <summary>
    /// 作业 Id
    /// </summary>
    public string? JobId { get; internal set; }

    /// <summary>
    /// 计算下一个触发时间
    /// </summary>
    /// <param name="startAt">起始时间</param>
    /// <returns><see cref="DateTime"/>?</returns>
    public abstract DateTime? GetNextOccurrence(DateTime? startAt);

    /// <summary>
    /// 执行条件检查
    /// </summary>
    /// <param name="checkTime">受检时间</param>
    /// <returns><see cref="bool"/></returns>
    public abstract bool ShouldRun(DateTime checkTime);

    /// <summary>
    /// 作业触发器转字符串输出
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public abstract new string? ToString();

    /// <summary>
    /// 记录运行信息和计算下一个触发时间
    /// </summary>
    internal void Increment()
    {
        NumberOfRuns++;
        LastRunTime = NextRunTime;
        NextRunTime = GetNextOccurrence(NextRunTime);
    }

    /// <summary>
    /// 记录错误次数
    /// </summary>
    internal void IncrementErrors()
    {
        NumberOfErrors++;
    }

    /// <summary>
    /// 执行条件检查（内部检查）
    /// </summary>
    /// <param name="checkTime">受检时间</param>
    /// <returns><see cref="bool"/></returns>
    internal bool InternalShouldRun(DateTime checkTime)
    {
        // 最大次数判断
        if (MaxNumberOfRuns == 0 || (MaxNumberOfRuns != -1 && NumberOfRuns >= MaxNumberOfRuns))
        {
            return false;
        }

        // 最大错误数判断
        if (MaxNumberOfErrors > 0 && NumberOfErrors >= MaxNumberOfErrors)
        {
            return false;
        }

        // 调用派生类 ShouldRun 方法
        return ShouldRun(checkTime);
    }
}