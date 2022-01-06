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
/// 调度作业构建器
/// </summary>
public sealed class SchedulerJobBuilder
{
    /// <summary>
    /// 作业触发器构造函数参数字典
    /// </summary>
    private readonly ConcurrentDictionary<Type, object[]> _jobTriggersData;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <param name="jobType">作业类型</param>
    public SchedulerJobBuilder(string jobId, Type jobType)
    {
        // 支持触发器类型一样，但参数不一样字典
        _jobTriggersData = new(new RepeatKeysEqualityComparer());
        JobId = jobId;
        JobType = jobType;
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
    /// 添加周期作业触发器
    /// </summary>
    /// <param name="interval">间隔时间（毫秒）</param>
    /// <returns><see cref="SchedulerJobBuilder"/></returns>
    public SchedulerJobBuilder AddPeriodTrigger(int interval)
    {
        _jobTriggersData.TryAdd(typeof(PeriodTrigger), new object[] { interval });

        return this;
    }

    /// <summary>
    /// 添加 Cron 表达式作业触发器
    /// </summary>
    /// <param name="schedule">调度计划（Cron 表达式）</param>
    /// <param name="format">Cron 表达式格式化类型</param>
    /// <returns><see cref="SchedulerJobBuilder"/></returns>
    public SchedulerJobBuilder AddCronTrigger(string schedule, CronStringFormat format = CronStringFormat.Default)
    {
        _jobTriggersData.TryAdd(typeof(CronTrigger), new object[] { schedule, format });

        return this;
    }

    /// <summary>
    /// 添加作业触发器
    /// </summary>
    /// <typeparam name="TJobTrigger"><see cref="JobTrigger"/> 派生类</typeparam>
    /// <param name="args">触发器构造函数参数</param>
    /// <returns><see cref="SchedulerJobBuilder"/></returns>
    public SchedulerJobBuilder AddTrigger<TJobTrigger>(params object[] args)
        where TJobTrigger : JobTrigger
    {
        _jobTriggersData.TryAdd(typeof(TJobTrigger), args);

        return this;
    }

    /// <summary>
    /// 添加作业触发器
    /// </summary>
    /// <param name="jobTriggerType">作业触发器类型</param>
    /// <param name="args">触发器构造函数参数</param>
    /// <returns><see cref="SchedulerJobBuilder"/></returns>
    public SchedulerJobBuilder AddTrigger(Type jobTriggerType, params object[] args)
    {
        // 检查 triggerType 类型是否派生自 JobTrigger
        if (!typeof(JobTrigger).IsAssignableFrom(jobTriggerType))
        {
            throw new InvalidOperationException("The <jobTriggerType> is not a valid JobTrigger type.");
        }

        _jobTriggersData.TryAdd(jobTriggerType, args);

        return this;
    }

    /// <summary>
    /// 构建调度作业对象
    /// </summary>
    /// <returns><see cref="SchedulerJob"/></returns>
    internal SchedulerJob Build(IJob job)
    {
        // 创建作业详情对象
        var jobDetail = new JobDetail(JobId);

        var jobTriggers = new List<JobTrigger>();
        var referenceTime = DateTime.UtcNow;

        // 动态创建作业触发器
        foreach (var (triggerType, args) in _jobTriggersData)
        {
            // 反射创建作业触发器
            var jobTrigger = (args == null || args.Length == 0
                ? Activator.CreateInstance(triggerType)
                : Activator.CreateInstance(triggerType, args)) as JobTrigger;

            // 设置作业触发器 Id（不可更改）
            jobTrigger!.JobTriggerId = $"{JobId}_trigger_{jobTriggers.Count + 1}";
            jobTrigger!.NextRunTime = referenceTime;

            jobTriggers.Add(jobTrigger);
        }

        return new SchedulerJob(JobId)
        {
            Job = job,
            JobDetail = jobDetail,
            Triggers = jobTriggers
        };
    }
}