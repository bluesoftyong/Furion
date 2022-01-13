// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.Schedule;

/// <summary>
/// 作业调度程序构建器
/// </summary>
public sealed class SchedulerBuilder
{
    /// <summary>
    /// 构造函数
    /// </summary>
    private SchedulerBuilder()
    {
        TriggerBuilders = new();
    }

    /// <summary>
    /// 作业信息构建器
    /// </summary>
    private JobBuilder? JobBuilder { get; set; }

    /// <summary>
    /// 作业触发器构建器集合
    /// </summary>
    private List<TriggerBuilder> TriggerBuilders { get; }

    /// <summary>
    /// 开始时间
    /// </summary>
    private DateTime? StartTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 创建作业调度程序构建器
    /// </summary>
    /// <param name="jobBuilder">作业信息构建器</param>
    /// <param name="triggerBuilders">作业触发器构建器</param>
    /// <returns><see cref="SchedulerBuilder"/></returns>
    public static SchedulerBuilder Create(JobBuilder jobBuilder, params TriggerBuilder[] triggerBuilders)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(jobBuilder);
        ArgumentNullException.ThrowIfNull(triggerBuilders);

        // 创建作业调度程序构建器
        var schedulerBuilder = new SchedulerBuilder()
        {
            JobBuilder = jobBuilder,
        };
        schedulerBuilder.TriggerBuilders.AddRange(triggerBuilders);

        return schedulerBuilder;
    }

    /// <summary>
    /// 设置起始时间
    /// </summary>
    /// <param name="startTime">起始时间</param>
    /// <returns><see cref="SchedulerBuilder"/></returns>
    public SchedulerBuilder StartAt(DateTime? startTime)
    {
        StartTime = startTime;
        return this;
    }

    /// <summary>
    /// 构建 <see cref="Scheduler"/>
    /// </summary>
    /// <returns><see cref="Scheduler"/></returns>
    internal Scheduler Build()
    {
        // 构建作业信息对象
        var (jobDetail, jobType) = JobBuilder!.Build();

        // 构建作业触发器集合
        var jobTriggers = TriggerBuilders.Select(t => t.Build(jobDetail.JobId!, StartTime))
                                                       .ToList();

        // 创建作业调度程序对象
        return new Scheduler(jobType!
            , jobDetail
            , jobTriggers);
    }
}