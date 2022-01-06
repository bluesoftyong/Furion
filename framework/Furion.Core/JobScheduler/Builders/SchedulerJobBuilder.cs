// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.TimeCrontab;
using System.Collections.Concurrent;

namespace Furion.JobScheduler;

/// <summary>
/// 作业调度器构建器
/// </summary>
public sealed class SchedulerJobBuilder
{
    /// <summary>
    /// 作业触发器构建器集合
    /// </summary>
    private readonly ConcurrentDictionary<string, JobTriggerBuilder> _jobTriggerBuilders;

    /// <summary>
    /// 作业信息构建器
    /// </summary>
    private readonly JobDetailBuilder _jobDetailBuilder;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <param name="jobType">作业类型</param>
    internal SchedulerJobBuilder(string jobId, Type jobType)
    {
        JobId = jobId;
        JobType = jobType;
        _jobTriggerBuilders = new();
        _jobDetailBuilder = new(jobId, jobType);
    }

    /// <summary>
    /// 作业 Id
    /// </summary>
    public string JobId { get; }

    /// <summary>
    /// 作业类型
    /// </summary>
    public Type JobType { get; }

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
    /// <param name="jobTriggerId">作业触发器 Id</param>
    /// <param name="interval">间隔时间（毫秒）</param>
    /// <param name="configureJobTriggerBuilder">作业触发器构建器委托</param>
    /// <returns><see cref="SchedulerJobBuilder"/></returns>
    public SchedulerJobBuilder AddPeriodTrigger(string jobTriggerId, int interval, Action<JobTriggerBuilder>? configureJobTriggerBuilder = default)
    {
        AddTrigger(jobTriggerId, typeof(PeriodTrigger), new object[] { interval }, configureJobTriggerBuilder);

        return this;
    }

    /// <summary>
    /// 添加 Cron 表达式作业触发器
    /// </summary>
    /// <param name="jobTriggerId">作业触发器 Id</param>
    /// <param name="schedule">调度计划（Cron 表达式）</param>
    /// <param name="format">Cron 表达式格式化类型</param>
    /// <param name="configureJobTriggerBuilder">作业触发器构建器委托</param>
    /// <returns><see cref="SchedulerJobBuilder"/></returns>
    public SchedulerJobBuilder AddCronTrigger(string jobTriggerId, string schedule, CronStringFormat format = CronStringFormat.Default, Action<JobTriggerBuilder>? configureJobTriggerBuilder = default)
    {
        AddTrigger(jobTriggerId, typeof(CronTrigger), new object[] { schedule, format }, configureJobTriggerBuilder);

        return this;
    }

    /// <summary>
    /// 添加作业触发器
    /// </summary>
    /// <typeparam name="TJobTrigger"><see cref="JobTrigger"/> 派生类</typeparam>
    /// <param name="jobTriggerId">作业触发器 Id</param>
    /// <param name="args">作业触发器构造函数参数</param>
    /// <param name="configureJobTriggerBuilder">作业触发器构建器委托</param>
    /// <returns><see cref="SchedulerJobBuilder"/></returns>
    public SchedulerJobBuilder AddTrigger<TJobTrigger>(string jobTriggerId, object[] args, Action<JobTriggerBuilder>? configureJobTriggerBuilder = default)
        where TJobTrigger : JobTrigger
    {
        AddTrigger(jobTriggerId, typeof(TJobTrigger), args, configureJobTriggerBuilder);

        return this;
    }

    /// <summary>
    /// 添加作业触发器
    /// </summary>
    /// <param name="jobTriggerId">作业触发器 Id</param>
    /// <param name="jobTriggerType">作业触发器类型</param>
    /// <param name="args">作业触发器构造函数参数</param>
    /// <param name="configureJobTriggerBuilder">作业触发器构建器委托</param>
    /// <returns><see cref="SchedulerJobBuilder"/></returns>
    public SchedulerJobBuilder AddTrigger(string jobTriggerId, Type jobTriggerType, object[] args, Action<JobTriggerBuilder>? configureJobTriggerBuilder = default)
    {
        // 检查 jobTriggerType 类型是否派生自 JobTrigger
        if (!typeof(JobTrigger).IsAssignableFrom(jobTriggerType))
        {
            throw new InvalidOperationException("The <jobTriggerType> is not a valid JobTrigger type.");
        }

        // 创建作业触发器构建器
        var jobTriggerBuilder = new JobTriggerBuilder(jobTriggerId, jobTriggerType, args);

        // 检查作业触发器唯一性
        if (!_jobTriggerBuilders.TryAdd(jobTriggerId, jobTriggerBuilder))
        {
            throw new InvalidOperationException($"The JobTrigger <{jobTriggerId}> has been registered. Repeated registration is prohibited.");
        }

        // 外部配置
        configureJobTriggerBuilder?.Invoke(jobTriggerBuilder);

        return this;
    }

    /// <summary>
    /// 构建作业调度器对象
    /// </summary>
    /// <param name="job">作业对象</param>
    /// <param name="referenceTime">初始引用时间</param>
    /// <returns><see cref="SchedulerJob"/></returns>
    internal SchedulerJob Build(IJob job, DateTime referenceTime)
    {
        // 构建作业信息对象
        var jobDetail = _jobDetailBuilder.Build();

        // 构建作业触发器集合
        var jobTriggers = _jobTriggerBuilders.Values.Select(t => t.Build(referenceTime)).ToList();

        // 创建作业调度器对象
        return new SchedulerJob(JobId)
        {
            Job = job,
            JobDetail = jobDetail,
            Triggers = jobTriggers
        };
    }
}