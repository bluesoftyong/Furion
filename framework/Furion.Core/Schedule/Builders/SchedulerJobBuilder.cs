// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.TimeCrontab;

namespace Furion.Schedule;

/// <summary>
/// 作业调度器构建器
/// </summary>
public sealed class SchedulerJobBuilder
{
    /// <summary>
    /// 作业触发器构建器集合
    /// </summary>
    private readonly IList<JobTriggerBuilder> _jobTriggerBuilders;

    /// <summary>
    /// 作业信息构建器
    /// </summary>
    private readonly JobDetailBuilder _jobDetailBuilder;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="jobType">作业类型</param>
    internal SchedulerJobBuilder(Type jobType)
    {
        JobType = jobType;
        _jobTriggerBuilders = new List<JobTriggerBuilder>();
        _jobDetailBuilder = new JobDetailBuilder().SetJobType(jobType);
    }

    /// <summary>
    /// 作业类型
    /// </summary>
    private Type JobType { get; }

    /// <summary>
    /// 开始时间
    /// </summary>
    private DateTime? StartTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 配置作业 Id
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    public SchedulerJobBuilder WithIdentity(string jobId)
    {
        // 空检查
        if (string.IsNullOrWhiteSpace(jobId))
        {
            throw new ArgumentNullException(nameof(jobId));
        }

        _jobDetailBuilder.WithIdentity(jobId);

        return this;
    }

    /// <summary>
    /// 配置作业信息
    /// </summary>
    /// <param name="configureJobDetailBuilder">作业信息构建器委托</param>
    /// <returns><see cref="SchedulerJobBuilder"/></returns>
    public SchedulerJobBuilder ConfigureDetail(Action<JobDetailBuilder> configureJobDetailBuilder)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configureJobDetailBuilder);

        // 外部调用
        configureJobDetailBuilder(_jobDetailBuilder);

        return this;
    }

    /// <summary>
    /// 添加周期作业触发器
    /// </summary>
    /// <param name="interval">间隔时间（毫秒）</param>
    /// <param name="configureJobTriggerBuilder">作业触发器构建器委托</param>
    /// <returns><see cref="SchedulerJobBuilder"/></returns>
    public SchedulerJobBuilder AddPeriodTrigger(int interval, Action<JobTriggerBuilder>? configureJobTriggerBuilder = default)
    {
        AddTrigger(typeof(PeriodTrigger)
            , new object[] { interval }
            , configureJobTriggerBuilder);

        return this;
    }

    /// <summary>
    /// 添加 Cron 表达式作业触发器
    /// </summary>
    /// <param name="schedule">调度计划（Cron 表达式）</param>
    /// <param name="format">Cron 表达式格式化类型</param>
    /// <param name="configureJobTriggerBuilder">作业触发器构建器委托</param>
    /// <returns><see cref="SchedulerJobBuilder"/></returns>
    public SchedulerJobBuilder AddCronTrigger(string schedule
        , CronStringFormat format = CronStringFormat.Default
        , Action<JobTriggerBuilder>? configureJobTriggerBuilder = default)
    {
        AddTrigger(typeof(CronTrigger)
            , new object[] { schedule, format }
            , configureJobTriggerBuilder);

        return this;
    }

    /// <summary>
    /// 添加作业触发器
    /// </summary>
    /// <typeparam name="TJobTrigger"><see cref="JobTrigger"/> 派生类</typeparam>
    /// <param name="args">作业触发器构造函数参数</param>
    /// <param name="configureJobTriggerBuilder">作业触发器构建器委托</param>
    /// <returns><see cref="SchedulerJobBuilder"/></returns>
    public SchedulerJobBuilder AddTrigger<TJobTrigger>(object?[]? args = default, Action<JobTriggerBuilder>? configureJobTriggerBuilder = default)
        where TJobTrigger : JobTrigger
    {
        AddTrigger(typeof(TJobTrigger)
            , args
            , configureJobTriggerBuilder);

        return this;
    }

    /// <summary>
    /// 添加作业触发器
    /// </summary>
    /// <param name="triggerType">作业触发器类型</param>
    /// <param name="args">作业触发器构造函数参数</param>
    /// <param name="configureJobTriggerBuilder">作业触发器构建器委托</param>
    /// <returns><see cref="SchedulerJobBuilder"/></returns>
    public SchedulerJobBuilder AddTrigger(Type triggerType
        , object?[]? args = default
        , Action<JobTriggerBuilder>? configureJobTriggerBuilder = default)
    {
        // 检查 triggerType 类型是否派生自 JobTrigger
        if (!typeof(JobTrigger).IsAssignableFrom(triggerType))
        {
            throw new InvalidOperationException("The <triggerType> is not a valid JobTrigger type.");
        }

        // 创建作业触发器构建器
        var jobTriggerBuilder = new JobTriggerBuilder().SetTriggerType(triggerType).WithArgs(args);

        // 外部配置
        configureJobTriggerBuilder?.Invoke(jobTriggerBuilder);

        // 添加到作业触发器构建器集合中
        _jobTriggerBuilders.Add(jobTriggerBuilder);

        return this;
    }

    /// <summary>
    /// 设置起始时间
    /// </summary>
    /// <param name="startTime">起始时间</param>
    /// <returns></returns>
    public SchedulerJobBuilder StartAt(DateTime? startTime)
    {
        StartTime = startTime;
        return this;
    }

    /// <summary>
    /// 构建作业调度器对象
    /// </summary>
    /// <returns><see cref="SchedulerJob"/></returns>
    internal SchedulerJob Build()
    {
        // 构建作业信息对象
        var jobDetail = _jobDetailBuilder.Build();

        // 构建作业触发器集合
        var jobTriggers = _jobTriggerBuilders.Select(t => t.Build(jobDetail.JobId!, StartTime))
                                                           .ToList();

        // 创建作业调度器对象
        return new SchedulerJob(JobType
            , jobDetail
            , jobTriggers);
    }
}