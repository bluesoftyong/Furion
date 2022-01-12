// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Reflection;
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
    internal JobTriggerBuilder()
    {
    }

    /// <summary>
    /// 作业触发器 Id
    /// </summary>
    public string? TriggerId { get; private set; }

    /// <summary>
    /// 作业触发器类型完整限定名
    /// </summary>
    public string? TriggerType { get; private set; }

    /// <summary>
    /// 运行时作业触发器类型
    /// </summary>
    internal Type? RuntimeTriggerType { get; private set; }

    /// <summary>
    /// 作业触发器类型所在程序集名称
    /// </summary>
    public string? AssemblyName { get; private set; }

    /// <summary>
    /// 作业触发器参数（JSON 字符串）
    /// </summary>
    /// <remarks>object?[]? 类型</remarks>
    public string? Args { get; private set; }

    /// <summary>
    /// 运行时作业触发器参数
    /// </summary>
    internal object?[]? RuntimeArgs { get; private set; }

    /// <summary>
    /// 作业触发器描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 最近运行时间
    /// </summary>
    public DateTime? LastRunTime { get; set; }

    /// <summary>
    /// 下一次运行时间
    /// </summary>
    public DateTime? NextRunTime { get; set; }

    /// <summary>
    /// 触发次数
    /// </summary>
    public long NumberOfRuns { get; set; } = 0;

    /// <summary>
    /// 最大执行次数
    /// </summary>
    /// <remarks>不限制：-1；0：不执行；> 0：大于 0 次</remarks>
    public long MaxNumberOfRuns { get; set; } = -1;

    /// <summary>
    /// 出错次数
    /// </summary>
    public long NumberOfErrors { get; set; } = 0;

    /// <summary>
    /// 最大出错次数
    /// </summary>
    /// <remarks>小于或等于0：不限制；> 0：大于 0 次</remarks>
    public long MaxNumberOfErrors { get; set; } = -1;

    /// <summary>
    /// 作业 Id
    /// </summary>
    internal string? JobId { get; private set; }

    /// <summary>
    /// 是否加入调度计划时自执行一次
    /// </summary>
    public bool ExecuteOnAdded { get; set; } = false;

    /// <summary>
    /// 设置作业触发器类型
    /// </summary>
    /// <typeparam name="TJobTrigger"><see cref="JobTrigger"/> 派生类</typeparam>
    /// <returns><see cref="JobTriggerBuilder"/></returns>
    public JobTriggerBuilder SetTriggerType<TJobTrigger>()
        where TJobTrigger : JobTrigger
    {
        return SetTriggerType(typeof(TJobTrigger));
    }

    /// <summary>
    /// 设置作业触发器类型
    /// </summary>
    /// <param name="assembly">程序集全名</param>
    /// <param name="triggerType">作业类型完整名称</param>
    /// <returns><see cref="JobDetailBuilder"/></returns>
    public JobTriggerBuilder SetTriggerType(string assembly, string triggerType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(triggerType);

        // 加载 GAC 全局缓存中的程序集
        var runtimeTriggerType = Assembly.Load(assembly).GetType(triggerType);
        ArgumentNullException.ThrowIfNull(runtimeTriggerType);

        return SetTriggerType(runtimeTriggerType);
    }

    /// <summary>
    /// 设置作业触发器类型
    /// </summary>
    /// <param name="triggerType">作业触发器类型</param>
    /// <returns><see cref="JobTriggerBuilder"/></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public JobTriggerBuilder SetTriggerType(Type triggerType)
    {
        // 检查 triggerType 类型是否派生自 JobTrigger
        if (!typeof(JobTrigger).IsAssignableFrom(triggerType))
        {
            throw new InvalidOperationException("The <TriggerType> is not a valid JobTrigger type.");
        }

        // 设置作业触发器类型关联的属性初始值
        AssemblyName = triggerType.Assembly.GetName().Name;
        TriggerType = triggerType.FullName;
        RuntimeTriggerType = triggerType;

        return this;
    }

    /// <summary>
    /// 设置作业触发器构造函数参数
    /// </summary>
    /// <param name="args">作业触发器构造函数参数</param>
    /// <returns><see cref="JobTriggerBuilder"/></returns>
    public JobTriggerBuilder WithArgs(object?[]? args)
    {
        Args = args == null || args.Length == 0 ? null : JsonSerializer.Serialize(args);
        RuntimeArgs = args;

        return this;
    }

    /// <summary>
    /// 设置作业触发器构造函数参数
    /// </summary>
    /// <param name="args">作业触发器构造函数参数</param>
    /// <returns><see cref="JobTriggerBuilder"/></returns>
    public JobTriggerBuilder WithArgs(string args)
    {
        RuntimeArgs = string.IsNullOrWhiteSpace(args?.Trim()) ? null : JsonSerializer.Deserialize<object?[]?>(args);
        Args = args;

        return this;
    }

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
    /// 构建作业触发器对象
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <param name="baseTime">起始时间</param>
    /// <returns><see cref="JobTrigger"/></returns>
    internal JobTrigger Build(string jobId, DateTime? baseTime)
    {
        // 判断是否带构造函数参数
        var withArgs = !(RuntimeArgs == null || RuntimeArgs.Length == 0);

        // 反射创建作业触发器对象
        var jobTrigger = (!withArgs
            ? Activator.CreateInstance(RuntimeTriggerType!)
            : Activator.CreateInstance(RuntimeTriggerType!, RuntimeArgs)) as JobTrigger;

        // 初始化作业触发器属性
        jobTrigger!.TriggerId = string.IsNullOrWhiteSpace(TriggerId) ? $"trigger_{Guid.NewGuid():N}" : TriggerId;
        jobTrigger!.TriggerType = TriggerType;
        jobTrigger!.AssemblyName = AssemblyName;
        jobTrigger!.Args = Args;
        jobTrigger!.Description = Description;
        jobTrigger!.LastRunTime = LastRunTime;
        jobTrigger!.NextRunTime = NextRunTime ?? baseTime;
        jobTrigger!.NumberOfRuns = NumberOfRuns;
        jobTrigger!.MaxNumberOfRuns = MaxNumberOfRuns;
        jobTrigger!.NumberOfErrors = NumberOfErrors;
        jobTrigger!.MaxNumberOfErrors = MaxNumberOfErrors;
        jobTrigger!.JobId = jobId;
        jobTrigger!.ExecuteOnAdded = ExecuteOnAdded;

        // 处理是否加入调度计划时自执行一次（只有触发次数为 0 才有效）
        if (!(ExecuteOnAdded && NumberOfRuns == 0))
        {
            jobTrigger!.NextRunTime = jobTrigger.GetNextOccurrence();
        }

        return jobTrigger!;
    }
}