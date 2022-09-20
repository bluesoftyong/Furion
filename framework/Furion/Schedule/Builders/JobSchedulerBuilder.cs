// MIT License
//
// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd and Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Concurrent;

namespace Furion.Schedule;

/// <summary>
/// 作业调度计划构建器
/// </summary>
[SuppressSniffer]
public sealed class JobSchedulerBuilder : JobScheduler
{
    /// <summary>
    /// 构造函数
    /// </summary>
    private JobSchedulerBuilder()
    {
    }

    /// <summary>
    /// 作业信息构建器
    /// </summary>
    private JobBuilder JobBuilder { get; set; }

    /// <summary>
    /// 作业触发器构建器集合
    /// </summary>
    private List<JobTriggerBuilder> JobTriggerBuilders { get; set; } = new();

    /// <summary>
    /// 创建作业调度程序构建器
    /// </summary>
    /// <param name="jobBuilder">作业信息构建器</param>
    /// <param name="triggerBuilders">作业触发器构建器</param>
    /// <returns><see cref="JobSchedulerBuilder"/></returns>
    public static JobSchedulerBuilder Create(JobBuilder jobBuilder, params JobTriggerBuilder[] triggerBuilders)
    {
        // 空检查
        if (jobBuilder == null) throw new ArgumentNullException(nameof(jobBuilder));
        if (triggerBuilders == null) throw new ArgumentNullException(nameof(triggerBuilders));

        // 创建作业调度计划构建器
        var jobSchedulerBuilder = new JobSchedulerBuilder()
        {
            JobBuilder = jobBuilder,
        };
        jobSchedulerBuilder.JobTriggerBuilders.AddRange(triggerBuilders);

        return jobSchedulerBuilder;
    }

    /// <summary>
    /// 将 <see cref="JobScheduler"/> 转换成 <see cref="JobSchedulerBuilder"/>
    /// </summary>
    /// <param name="jobScheduler">作业调度计划</param>
    /// <returns><see cref="JobSchedulerBuilder"/></returns>
    public static JobSchedulerBuilder From(JobScheduler jobScheduler)
    {
        var jobSchedulerBuilder = (JobSchedulerBuilder)jobScheduler;
        jobSchedulerBuilder.JobBuilder = JobBuilder.From(jobScheduler.JobDetail);
        jobSchedulerBuilder.JobTriggerBuilders = jobScheduler.JobTriggers.Select(t => JobTriggerBuilder.From(t.Value)).ToList();

        return jobSchedulerBuilder;
    }

    /// <summary>
    /// 构建 <see cref="JobScheduler"/> 对象
    /// </summary>
    /// <returns><see cref="JobScheduler"/></returns>
    internal JobScheduler Build()
    {
        // 构建作业信息和作业触发器
        var jobDetail = JobBuilder.Build();

        // 构建作业触发器
        var jobTriggers = new ConcurrentDictionary<string, JobTriggerBase>();
        JobTriggerBuilders.ForEach(builder =>
        {
            var jobTrigger = builder.Build(jobDetail.JobId);
            var succeed = jobTriggers.TryAdd(jobTrigger.TriggerId, jobTrigger);

            if (!succeed) throw new InvalidOperationException($"The TriggerId of <{jobTrigger.TriggerId}> already exists.");
        });

        // 创建作业调度计划实例
        var jobScheduler = new JobScheduler()
        {
            JobId = jobDetail.JobId,
            JobDetail = jobDetail,
            JobTriggers = jobTriggers
        };

        return this;
    }
}