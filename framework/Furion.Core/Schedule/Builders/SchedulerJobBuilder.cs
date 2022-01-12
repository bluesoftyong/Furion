// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.Schedule;

/// <summary>
/// 作业调度器构建器
/// </summary>
public sealed class SchedulerJobBuilder
{
    /// <summary>
    /// 构造函数
    /// </summary>
    private SchedulerJobBuilder()
    {
    }

    /// <summary>
    /// 作业信息构建器
    /// </summary>
    private JobBuilder? JobBuilder { get; set; }

    /// <summary>
    /// 作业触发器构建器集合
    /// </summary>
    private List<TriggerBuilder> TriggerBuilders { get; set; } = new();

    /// <summary>
    /// 开始时间
    /// </summary>
    private DateTime? StartTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 创建作业调度器构建器
    /// </summary>
    /// <param name="jobBuilder">作业信息构建器</param>
    /// <param name="triggerBuilders">作业触发器构建器</param>
    /// <returns></returns>
    public static SchedulerJobBuilder Create(JobBuilder jobBuilder, params TriggerBuilder[] triggerBuilders)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(jobBuilder);
        ArgumentNullException.ThrowIfNull(triggerBuilders);

        // 创建作业调度器构建器
        var schedulerJobBuilder = new SchedulerJobBuilder()
        {
            JobBuilder = jobBuilder,
        };
        schedulerJobBuilder.TriggerBuilders.AddRange(triggerBuilders);

        return schedulerJobBuilder;
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
    /// <returns><see cref="SchedulerJobBuilder"/></returns>
    internal SchedulerJob Build()
    {
        // 构建作业信息对象
        var (jobDetail, jobType) = JobBuilder!.Build();

        // 构建作业触发器集合
        var jobTriggers = TriggerBuilders.Select(t => t.Build(jobDetail.JobId!, StartTime))
                                                           .ToList();

        // 创建作业调度器对象
        return new SchedulerJob(jobType!
            , jobDetail
            , jobTriggers);
    }
}