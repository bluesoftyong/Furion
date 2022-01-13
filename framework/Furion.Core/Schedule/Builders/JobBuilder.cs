// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Reflection;

namespace Furion.Schedule;

/// <summary>
/// 作业信息构建器
/// </summary>
public sealed class JobBuilder
{
    /// <summary>
    /// 构造函数
    /// </summary>
    private JobBuilder()
    {
    }

    /// <summary>
    /// 作业 Id
    /// </summary>
    private string? JobId { get; set; }

    /// <summary>
    /// 作业类型
    /// </summary>
    /// <remarks>存储的是类型的 FullName</remarks>
    private string? JobType { get; set; }

    /// <summary>
    /// 作业类型所在程序集
    /// </summary>
    /// <remarks>存储的是程序集 Name</remarks>
    private string? AssemblyName { get; set; }

    /// <summary>
    /// 描述信息
    /// </summary>
    private string? Description { get; set; }

    /// <summary>
    /// 运行状态
    /// </summary>
    private JobStatus Status { get; set; } = JobStatus.Normal;

    /// <summary>
    /// 启动方式
    /// </summary>
    private JobStartMode StartMode { get; set; } = JobStartMode.Run;

    /// <summary>
    /// 执行锁方式
    /// </summary>
    private JobLockMode LockMode { get; set; } = JobLockMode.Parallel;

    /// <summary>
    /// 是否打印执行日志
    /// </summary>
    private bool PrintExecutionLog { get; set; } = false;

    /// <summary>
    /// 是否带独立服务作用域
    /// </summary>
    /// <remarks>设置为 true，<see cref="IJob"/> 实现类将创建新的服务作用域解析构造函数服务</remarks>
    private bool SandboxScope { get; set; } = false;

    /// <summary>
    /// 运行时作业类型
    /// </summary>
    private Type? RuntimeJobType { get; set; }

    /// <summary>
    /// 创建作业信息构建器
    /// </summary>
    /// <typeparam name="TJob"><see cref="IJob"/> 实现类</typeparam>
    /// <returns><see cref="JobBuilder"/></returns>
    public static JobBuilder Create<TJob>()
        where TJob : class, IJob
    {
        return Create(typeof(TJob));
    }

    /// <summary>
    /// 创建作业信息构建器
    /// </summary>
    /// <param name="assemblyName">作业类型所在程序集 Name</param>
    /// <param name="jobType">作业类型 FullName</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public static JobBuilder Create(string assemblyName, string jobType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(assemblyName);
        ArgumentNullException.ThrowIfNull(jobType);

        // 加载 GAC 全局应用程序缓存中的程序集及类型
        var runtimeJobType = Assembly.Load(assemblyName).GetType(jobType);
        ArgumentNullException.ThrowIfNull(runtimeJobType);

        return Create(runtimeJobType);
    }

    /// <summary>
    /// 创建作业信息构建器
    /// </summary>
    /// <param name="jobType"><see cref="IJob"/> 实现类</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public static JobBuilder Create(Type jobType)
    {
        // 检查 jobType 类型是否实现 IJob 接口
        if (!typeof(IJob).IsAssignableFrom(jobType))
        {
            throw new InvalidOperationException("The <jobType> does not implement IJob interface.");
        }

        // 创建作业信息构建器
        var jobBuilder = new JobBuilder
        {
            SandboxScope = jobType.IsDefined(typeof(SandboxScopeAttribute), false),
            AssemblyName = jobType.Assembly.GetName().Name,
            JobType = jobType.FullName,
            RuntimeJobType = jobType
        };

        return jobBuilder;
    }

    /// <summary>
    /// 设置作业 Id
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <returns><see cref="JobBuilder"/></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public JobBuilder WithIdentity(string jobId)
    {
        // 空检查
        if (string.IsNullOrWhiteSpace(jobId))
        {
            throw new ArgumentNullException(nameof(jobId));
        }

        JobId = jobId;

        return this;
    }

    /// <summary>
    /// 设置描述信息
    /// </summary>
    /// <param name="description">描述信息</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public JobBuilder SetDescription(string? description)
    {
        Description = description;

        return this;
    }

    /// <summary>
    /// 设置运行状态
    /// </summary>
    /// <param name="status">运行状态</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public JobBuilder SetStatus(JobStatus status)
    {
        Status = status;

        return this;
    }

    /// <summary>
    /// 设置启动方式
    /// </summary>
    /// <param name="startMode">启动方式</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public JobBuilder SetStartMode(JobStartMode startMode)
    {
        StartMode = startMode;

        return this;
    }

    /// <summary>
    /// 设置执行锁方式
    /// </summary>
    /// <param name="lockMode">执行锁方式</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public JobBuilder SetLockMode(JobLockMode lockMode)
    {
        LockMode = lockMode;

        return this;
    }

    /// <summary>
    /// 设置是否打印执行日志
    /// </summary>
    /// <param name="printExecutionLog">是否打印执行日志</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public JobBuilder SetPrintExecutionLog(bool printExecutionLog)
    {
        PrintExecutionLog = printExecutionLog;

        return this;
    }

    /// <summary>
    /// 是否带独立服务作用域
    /// </summary>
    /// <param name="sandboxScope">是否带独立服务作用域</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public JobBuilder SetSandboxScope(bool sandboxScope)
    {
        SandboxScope = sandboxScope;

        return this;
    }

    /// <summary>
    /// 绑定作业触发器构建器集合
    /// </summary>
    /// <param name="triggerBuilders">作业触发器构建器集合</param>
    /// <returns><see cref="SchedulerBuilder"/></returns>
    public SchedulerBuilder BindTriggers(params TriggerBuilder[] triggerBuilders)
    {
        return SchedulerBuilder.Create(this, triggerBuilders);
    }

    /// <summary>
    /// 构建 <see cref="JobDetail"/> 及 JobType
    /// </summary>
    /// <returns>(<see cref="JobDetail"/>, <see cref="Type"/>)</returns>
    internal (JobDetail JobDetail, Type JobType) Build()
    {
        // 创建作业信息对象
        var jobDetail = new JobDetail();

        // 初始化作业信息属性
        jobDetail!.JobId = string.IsNullOrWhiteSpace(JobId) ? $"job_{Guid.NewGuid():N}" : JobId;
        jobDetail!.JobType = JobType;
        jobDetail!.AssemblyName = AssemblyName;
        jobDetail!.Description = Description;
        // 只要启动方式为 JobStatus.Wait（等待启动），那么作业状态设置为 JobStatus.None
        jobDetail!.Status = StartMode == JobStartMode.Wait ? JobStatus.None : Status;
        jobDetail!.StartMode = StartMode;
        jobDetail!.LockMode = LockMode;
        jobDetail!.PrintExecutionLog = PrintExecutionLog;
        jobDetail!.SandboxScope = SandboxScope;

        return (jobDetail!, RuntimeJobType!);
    }
}