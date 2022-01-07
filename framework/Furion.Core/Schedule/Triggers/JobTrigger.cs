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
    public virtual string? JobTriggerId { get; internal set; }

    /// <summary>
    /// 作业触发器类型完整限定名（含程序集名称）
    /// </summary>
    /// <remarks>格式：程序集名称;作业触发器类型完整限定名，如：Furion;Furion.Jobs.MyTrigger</remarks>
    public string? TriggerTypeWithAssembly { get; internal set; }

    /// <summary>
    /// 作业 Id
    /// </summary>
    public string? JobId { get; internal set; }

    /// <summary>
    /// 作业触发器描述
    /// </summary>
    public virtual string? Description { get; internal set; }

    /// <summary>
    /// 最近运行时间
    /// </summary>
    public virtual DateTime LastRunTime { get; internal set; }

    /// <summary>
    /// 下一次运行时间
    /// </summary>
    public virtual DateTime NextRunTime { get; internal set; }

    /// <summary>
    /// 触发次数
    /// </summary>
    public virtual long NumberOfRuns { get; internal set; } = 0;

    /// <summary>
    /// 最大执行次数
    /// </summary>
    /// <remarks>不限制：-1；0：不执行；> 0：大于 0 次</remarks>
    public virtual long MaxNumberOfRuns { get; internal set; } = -1;

    /// <summary>
    /// 计算当前触发器增量信息
    /// </summary>
    public abstract void Increment();

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

        // 调用自定义处理逻辑
        return ShouldRun(currentTime);
    }
}