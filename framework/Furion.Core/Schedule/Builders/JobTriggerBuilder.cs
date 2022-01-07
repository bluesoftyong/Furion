// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Text.Json;

namespace Furion.Schedule;

/// <summary>
/// 作业触发器构建器
/// </summary>
public sealed class JobTriggerBuilder
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="triggerType">作业触发器类型</param>
    internal JobTriggerBuilder(Type triggerType)
    {
        TriggerType = triggerType;
    }

    /// <summary>
    /// 作业触发器 Id
    /// </summary>
    internal string? TriggerId { get; private set; }

    /// <summary>
    /// 作业触发器
    /// </summary>
    internal Type TriggerType { get; }

    /// <summary>
    /// 作业触发器构造函数参数
    /// </summary>
    internal object?[]? Args { get; private set; }

    /// <summary>
    /// 作业触发器描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 最大执行次数
    /// </summary>
    /// <remarks>不限制：-1；0：不执行；> 0：大于 0 次</remarks>
    public long MaxNumberOfRuns { get; set; } = -1;

    /// <summary>
    /// 最大出错次数
    /// </summary>
    /// <remarks>小于或等于0：不限制；> 0：大于 0 次</remarks>
    public long MaxNumberOfErrors { get; set; } = -1;

    /// <summary>
    /// 配置作业触发器 Id
    /// </summary>
    /// <param name="triggerId">作业触发器 Id</param>
    public void WithIdentity(string triggerId)
    {
        // 空检查
        if (string.IsNullOrWhiteSpace(triggerId))
        {
            throw new ArgumentNullException(nameof(triggerId));
        }

        TriggerId = triggerId;
    }

    /// <summary>
    /// 添加作业触发器参数
    /// </summary>
    /// <param name="args">作业触发器构造函数参数</param>
    public void WithArgs(params object?[]? args)
    {
        Args = args;
    }

    /// <summary>
    /// 构建作业触发器对象
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <param name="referenceTime">初始引用时间</param>
    /// <returns><see cref="JobTrigger"/></returns>
    internal JobTrigger Build(string jobId, DateTime referenceTime)
    {
        // 检查 TriggerType 类型是否派生自 JobTrigger
        if (!typeof(JobTrigger).IsAssignableFrom(TriggerType))
        {
            throw new InvalidOperationException("The <TriggerType> is not a valid JobTrigger type.");
        }

        var withArgs = !(Args == null || Args.Length == 0);

        // 反射创建作业触发器
        var jobTrigger = (!withArgs
            ? Activator.CreateInstance(TriggerType)
            : Activator.CreateInstance(TriggerType, Args)) as JobTrigger;

        // 初始化作业触发器属性
        jobTrigger!.TriggerId = string.IsNullOrWhiteSpace(TriggerId) ? $"{jobId}_trigger_{Guid.NewGuid():N}" : TriggerId;
        jobTrigger!.TriggerType = TriggerType.FullName;
        jobTrigger!.Assembly = TriggerType.Assembly.GetName().Name;
        jobTrigger!.Args = withArgs ? JsonSerializer.Serialize(Args) : default;
        jobTrigger!.Description = Description;
        jobTrigger!.MaxNumberOfRuns = MaxNumberOfRuns;
        jobTrigger!.MaxNumberOfErrors = MaxNumberOfErrors;
        jobTrigger!.NextRunTime = referenceTime;
        jobTrigger!.JobId = jobId;

        return jobTrigger!;
    }
}