// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.TimeCrontab;
using System.Collections.Concurrent;

namespace Furion.SchedulerJob;

/// <summary>
/// 作业详情构建器
/// </summary>
public sealed class JobDetailBuilder
{
    /// <summary>
    /// 动态作业触发器元数据集合
    /// </summary>
    private readonly ConcurrentDictionary<Type, object[]> _dynamicTriggers;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <param name="jobType">作业类型</param>
    public JobDetailBuilder(string jobId, Type jobType)
    {
        _dynamicTriggers = new(new RepeatKeysEqualityComparer());
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
    /// 添加简单作业触发器
    /// </summary>
    /// <param name="interval">间隔时间（毫秒）</param>
    /// <returns><see cref="JobDetailBuilder"/></returns>
    public JobDetailBuilder AddSimpleTrigger(int interval)
    {
        _dynamicTriggers.TryAdd(typeof(SimpleTrigger), new object[] { interval });

        return this;
    }

    /// <summary>
    /// 添加 Cron 表达式作业触发器
    /// </summary>
    /// <param name="schedule">调度计划（Cron 表达式）</param>
    /// <param name="format">Cron 表达式格式化类型</param>
    /// <returns><see cref="JobDetailBuilder"/></returns>
    public JobDetailBuilder AddCronTrigger(string schedule, CronStringFormat format = CronStringFormat.Default)
    {
        _dynamicTriggers.TryAdd(typeof(CronTrigger), new object[] { schedule, format });

        return this;
    }

    /// <summary>
    /// 添加作业触发器
    /// </summary>
    /// <typeparam name="TJobTrigger"><see cref="JobTrigger"/> 派生类</typeparam>
    /// <param name="args">触发器构造函数参数</param>
    /// <returns><see cref="JobDetailBuilder"/></returns>
    public JobDetailBuilder AddTrigger<TJobTrigger>(params object[] args)
        where TJobTrigger : JobTrigger
    {
        _dynamicTriggers.TryAdd(typeof(TJobTrigger), args);

        return this;
    }

    /// <summary>
    /// 添加作业触发器
    /// </summary>
    /// <param name="triggerType">作业触发器类型</param>
    /// <param name="args">触发器构造函数参数</param>
    /// <returns><see cref="JobDetailBuilder"/></returns>
    public JobDetailBuilder AddTrigger(Type triggerType, params object[] args)
    {
        // 检查 triggerType 类型是否派生自 JobTrigger
        if (!typeof(JobTrigger).IsAssignableFrom(triggerType))
        {
            throw new InvalidOperationException("The <triggerType> is not a valid JobTrigger type.");
        }

        _dynamicTriggers.TryAdd(triggerType, args);

        return this;
    }

    /// <summary>
    /// 构建作业详情构建器
    /// </summary>
    /// <returns>作业详情及触发器</returns>
    internal (JobDetail JobDetail, List<JobTrigger> JobTriggers) Build()
    {
        var jobDetail = new DefaultJobDetail
        {
            JobId = JobId,
        };

        var jobTriggers = new List<JobTrigger>();

        // 动态创建作业触发器
        foreach (var (triggerType, args) in _dynamicTriggers)
        {
            // 反射创建作业触发器
            var jobTrigger = (args == null || args.Length == 0
                ? Activator.CreateInstance(triggerType)
                : Activator.CreateInstance(triggerType, args)) as JobTrigger;

            // 设置作业触发器 Id（不可更改）
            jobTrigger!.JobTriggerId = $"{JobId}_trigger_{jobTriggers.Count + 1}";

            jobTriggers.Add(jobTrigger);
        }

        return (jobDetail, jobTriggers);
    }
}