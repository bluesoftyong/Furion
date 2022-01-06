// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.JobScheduler;

/// <summary>
/// 作业触发器构建器
/// </summary>
public sealed class JobTriggerBuilder
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="jobTriggerId">作业触发器 Id</param>
    /// <param name="jobTriggerType">作业触发器类型</param>
    /// <param name="args">作业触发器构造函数参数</param>
    internal JobTriggerBuilder(string jobTriggerId, Type jobTriggerType, params object[] args)
    {
        // 空检查
        if (string.IsNullOrWhiteSpace(jobTriggerId))
        {
            throw new ArgumentNullException(nameof(jobTriggerId));
        }

        JobTriggerId = jobTriggerId;
        JobTriggerType = jobTriggerType;
        Args = args;
        TriggerTypeWithAssembly = $"{jobTriggerType.Assembly.GetName().Name};{jobTriggerType.FullName}";
    }

    /// <summary>
    /// 作业触发器 Id
    /// </summary>
    public string JobTriggerId { get; }

    /// <summary>
    /// 作业触发器类型名（含程序集名）
    /// </summary>
    /// <remarks>格式：程序集名;命名空间.类型名，如：Furion;Furion.Jobs.MyJob</remarks>
    public string? TriggerTypeWithAssembly { get; }

    /// <summary>
    /// 作业触发器
    /// </summary>
    public Type JobTriggerType { get; }

    /// <summary>
    /// 作业触发器构造函数参数
    /// </summary>
    public object[] Args { get; }

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
    /// 构建作业触发器
    /// </summary>
    /// <param name="referenceTime">初始引用时间</param>
    /// <returns><see cref="JobTrigger"/></returns>
    internal JobTrigger Build(DateTime referenceTime)
    {
        // 检查 jobTriggerType 类型是否派生自 JobTrigger
        if (!typeof(JobTrigger).IsAssignableFrom(JobTriggerType))
        {
            throw new InvalidOperationException("The <jobTriggerType> is not a valid JobTrigger type.");
        }

        // 反射创建作业触发器
        var jobTrigger = (Args == null || Args.Length == 0
            ? Activator.CreateInstance(JobTriggerType)
            : Activator.CreateInstance(JobTriggerType, Args)) as JobTrigger;

        // 初始化作业触发器参数
        jobTrigger!.JobTriggerId = JobTriggerId;
        jobTrigger!.Description = Description;
        jobTrigger!.MaxNumberOfRuns = MaxNumberOfRuns;
        jobTrigger!.NextRunTime = referenceTime;
        jobTrigger!.TriggerTypeWithAssembly = TriggerTypeWithAssembly;

        return jobTrigger!;
    }
}