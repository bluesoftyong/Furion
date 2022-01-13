// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.TimeCrontab;
using System.Reflection;
using System.Text.Json;

namespace Furion.Schedule;

/// <summary>
/// 作业触发器构建器
/// </summary>
public sealed class TriggerBuilder
{
    /// <summary>
    /// 构造函数
    /// </summary>
    private TriggerBuilder()
    {
    }

    /// <summary>
    /// 作业触发器 Id
    /// </summary>
    private string? TriggerId { get; set; }

    /// <summary>
    /// 作业触发器类型
    /// </summary>
    /// <remarks>存储的是类型的 FullName</remarks>
    private string? TriggerType { get; set; }

    /// <summary>
    /// 作业触发器类型所在程序集
    /// </summary>
    /// <remarks>存储的是程序集 Name</remarks>
    private string? AssemblyName { get; set; }

    /// <summary>
    /// 作业触发器参数
    /// </summary>
    /// <remarks>运行时将反序列化为 object?[]? 类型并作为构造函数参数</remarks>
    private string? Args { get; set; }

    /// <summary>
    /// 描述信息
    /// </summary>
    private string? Description { get; set; }

    /// <summary>
    /// 最近运行时间
    /// </summary>
    private DateTime? LastRunTime { get; set; }

    /// <summary>
    /// 下一次运行时间
    /// </summary>
    private DateTime? NextRunTime { get; set; }

    /// <summary>
    /// 触发次数
    /// </summary>
    private long NumberOfRuns { get; set; } = 0;

    /// <summary>
    /// 最大触发次数
    /// </summary>
    /// <remarks>
    /// <para>-1：不限制</para>
    /// <para>0：不执行</para>
    /// <para>>0：N 次</para>
    /// </remarks>
    private long MaxNumberOfRuns { get; set; } = -1;

    /// <summary>
    /// 出错次数
    /// </summary>
    private long NumberOfErrors { get; set; } = 0;

    /// <summary>
    /// 最大出错次数
    /// </summary>
    /// <remarks>
    /// <para>lt/eq 0：不限制</para>
    /// <para>>0：N 次</para>
    /// </remarks>
    private long MaxNumberOfErrors { get; set; } = -1;

    /// <summary>
    /// 运行时作业触发器类型
    /// </summary>
    private Type? RuntimeTriggerType { get; set; }

    /// <summary>
    /// 运行时作业触发器参数
    /// </summary>
    private object?[]? RuntimeArgs { get; set; }

    /// <summary>
    /// 创建作业 周期（间隔）触发器构建器
    /// </summary>
    /// <param name="interval">间隔（毫秒）</param>
    /// <returns><see cref="TriggerBuilder"/></returns>
    public static TriggerBuilder Period(int interval)
    {
        return Create(typeof(PeriodTrigger)).WithArgs(new object[] { interval });
    }

    /// <summary>
    /// 创建作业 Cron 触发器构建器
    /// </summary>
    /// <param name="schedule">Cron 表达式</param>
    /// <param name="format">Cron 表达式格式化类型，默认 <see cref="CronStringFormat.Default"/></param>
    /// <returns><see cref="TriggerBuilder"/></returns>
    public static TriggerBuilder Cron(string schedule, CronStringFormat format = CronStringFormat.Default)
    {
        return Create(typeof(CronTrigger)).WithArgs(new object[] { schedule, (int)format });
    }

    /// <summary>
    /// 创建作业触发器构建器
    /// </summary>
    /// <typeparam name="TJobTrigger"><see cref="JobTrigger"/> 派生类</typeparam>
    /// <returns><see cref="TriggerBuilder"/></returns>
    public static TriggerBuilder Create<TJobTrigger>()
        where TJobTrigger : JobTrigger
    {
        return Create(typeof(TJobTrigger));
    }

    /// <summary>
    /// 创建作业触发器构建器
    /// </summary>
    /// <param name="assemblyName">作业触发器类型所在程序集 Name</param>
    /// <param name="triggerType">作业触发器类型 FullName</param>
    /// <returns><see cref="TriggerBuilder"/></returns>
    public static TriggerBuilder Create(string assemblyName, string triggerType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(assemblyName);
        ArgumentNullException.ThrowIfNull(triggerType);

        // 加载 GAC 全局应用程序缓存中的程序集及类型
        var runtimeTriggerType = Assembly.Load(assemblyName).GetType(triggerType);
        ArgumentNullException.ThrowIfNull(runtimeTriggerType);

        return Create(runtimeTriggerType);
    }

    /// <summary>
    /// 创建作业触发器构建器
    /// </summary>
    /// <param name="triggerType"><see cref="JobTrigger"/> 派生类</param>
    /// <returns><see cref="TriggerBuilder"/></returns>
    public static TriggerBuilder Create(Type triggerType)
    {
        // 检查 triggerType 类型是否派生自 JobTrigger
        if (!typeof(JobTrigger).IsAssignableFrom(triggerType))
        {
            throw new InvalidOperationException("The <TriggerType> is not a valid JobTrigger type.");
        }

        // 最多只能包含一个构造函数
        if (triggerType.GetConstructors().Length > 1)
        {
            throw new InvalidOperationException("The <TriggerType> can contain at most one constructor.");
        }

        // 创建作业触发器构建器
        var triggerBuilder = new TriggerBuilder()
        {
            AssemblyName = triggerType.Assembly.GetName().Name,
            TriggerType = triggerType.FullName,
            RuntimeTriggerType = triggerType
        };

        return triggerBuilder;
    }

    /// <summary>
    /// 设置作业触发器 Id
    /// </summary>
    /// <param name="triggerId">作业触发器 Id</param>
    /// <returns><see cref="TriggerBuilder"/></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public TriggerBuilder WithIdentity(string triggerId)
    {
        // 空检查
        if (string.IsNullOrWhiteSpace(triggerId))
        {
            throw new ArgumentNullException(nameof(triggerId));
        }

        TriggerId = triggerId;

        return this;
    }

    /// <summary>
    /// 设置作业触发器参数
    /// </summary>
    /// <param name="args">作业触发器参数</param>
    /// <returns><see cref="TriggerBuilder"/></returns>
    public TriggerBuilder WithArgs(object?[]? args)
    {
        Args = args == null || args.Length == 0
            ? null
            : JsonSerializer.Serialize(args);
        RuntimeArgs = args;

        return this;
    }

    /// <summary>
    /// 设置作业触发器参数
    /// </summary>
    /// <param name="args">作业触发器参数</param>
    /// <returns><see cref="TriggerBuilder"/></returns>
    public TriggerBuilder WithArgs(string? args)
    {
        RuntimeArgs = string.IsNullOrWhiteSpace(args?.Trim())
            ? null
            : JsonSerializer.Deserialize<object?[]?>(args, new JsonSerializerOptions
            {
                Converters =
                {
                    // 处理 JSON 反序列化后类型丢失问题
                    new JobTriggerConstructorParameterTypesConverter()
                }
            });
        Args = args;

        return this;
    }

    /// <summary>
    /// 设置描述信息
    /// </summary>
    /// <param name="description">描述信息</param>
    /// <returns><see cref="TriggerBuilder"/></returns>
    public TriggerBuilder SetDescription(string? description)
    {
        Description = description;

        return this;
    }

    /// <summary>
    /// 设置最近运行时间
    /// </summary>
    /// <param name="lastRunTime">最近运行时间</param>
    /// <returns><see cref="TriggerBuilder"/></returns>
    public TriggerBuilder SetLastRunTime(DateTime? lastRunTime)
    {
        LastRunTime = lastRunTime;

        return this;
    }

    /// <summary>
    /// 设置下一次运行时间
    /// </summary>
    /// <param name="nextRunTime">下一次运行时间</param>
    /// <returns><see cref="TriggerBuilder"/></returns>
    public TriggerBuilder SetNextRunTime(DateTime? nextRunTime)
    {
        NextRunTime = nextRunTime;

        return this;
    }

    /// <summary>
    /// 设置触发次数
    /// </summary>
    /// <param name="numberOfRuns">触发次数</param>
    /// <returns><see cref="TriggerBuilder"/></returns>
    public TriggerBuilder SetNumberOfRuns(long numberOfRuns)
    {
        NumberOfRuns = numberOfRuns;

        return this;
    }

    /// <summary>
    /// 设置最大执行次数
    /// </summary>
    /// <param name="maxNumberOfRuns">最大触发次数</param>
    /// <returns><see cref="TriggerBuilder"/></returns>
    public TriggerBuilder SetMaxNumberOfRuns(long maxNumberOfRuns)
    {
        MaxNumberOfRuns = maxNumberOfRuns;

        return this;
    }

    /// <summary>
    /// 设置出错次数
    /// </summary>
    /// <param name="numberOfErrors">出错次数</param>
    /// <returns><see cref="TriggerBuilder"/></returns>
    public TriggerBuilder SetNumberOfErrors(long numberOfErrors)
    {
        NumberOfErrors = numberOfErrors;

        return this;
    }

    /// <summary>
    /// 设置最大出错次数
    /// </summary>
    /// <param name="maxNumberOfErrors">最大出错次数</param>
    /// <returns><see cref="TriggerBuilder"/></returns>
    public TriggerBuilder SetMaxNumberOfErrors(long maxNumberOfErrors)
    {
        MaxNumberOfErrors = maxNumberOfErrors;

        return this;
    }

    /// <summary>
    /// 构建 <see cref="JobTrigger"/>
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <param name="startAt">起始时间</param>
    /// <returns><see cref="JobTrigger"/></returns>
    internal JobTrigger Build(string jobId, DateTime? startAt)
    {
        // 判断是否带参数
        var withArgs = !(RuntimeArgs == null || RuntimeArgs.Length == 0);

        // 反射创建作业触发器对象
        var jobTrigger = (!withArgs
            ? Activator.CreateInstance(RuntimeTriggerType!)
            : Activator.CreateInstance(RuntimeTriggerType!, RuntimeArgs)) as JobTrigger;

        // 初始化作业触发器属性
        jobTrigger!.JobId = jobId;
        jobTrigger!.TriggerId = string.IsNullOrWhiteSpace(TriggerId) ? $"trigger_{Guid.NewGuid():N}" : TriggerId;
        jobTrigger!.TriggerType = TriggerType;
        jobTrigger!.AssemblyName = AssemblyName;
        jobTrigger!.Args = Args;
        jobTrigger!.Description = Description;
        jobTrigger!.LastRunTime = LastRunTime;
        jobTrigger!.NextRunTime = jobTrigger.GetNextOccurrence(startAt) ?? NextRunTime;
        jobTrigger!.NumberOfRuns = NumberOfRuns;
        jobTrigger!.MaxNumberOfRuns = MaxNumberOfRuns;
        jobTrigger!.NumberOfErrors = NumberOfErrors;
        jobTrigger!.MaxNumberOfErrors = MaxNumberOfErrors;

        return jobTrigger!;
    }
}