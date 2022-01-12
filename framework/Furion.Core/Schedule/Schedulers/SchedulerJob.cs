// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.Schedule;

/// <summary>
/// 作业调度器
/// </summary>
internal sealed class SchedulerJob : ISchedulerJob
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="jobType">作业类型</param>
    /// <param name="jobDetail">作业信息</param>
    /// <param name="triggers">作业触发器</param>
    internal SchedulerJob(Type jobType, JobDetail jobDetail, IList<JobTrigger> triggers)
    {
        JobType = jobType;
        JobDetail = jobDetail;
        Triggers = triggers;
    }

    /// <summary>
    /// 解构函数
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <param name="jobType">作业类型</param>
    /// <param name="jobDetail">作业信息对象</param>
    /// <param name="jobTriggers">作业触发器</param>
    internal void Deconstruct(out string jobId
        , out Type jobType
        , out JobDetail jobDetail
        , out IList<JobTrigger> jobTriggers)
    {
        jobId = JobDetail.JobId!;
        jobType = JobType;
        jobDetail = JobDetail;
        jobTriggers = Triggers!;
    }

    /// <summary>
    /// 作业类型
    /// </summary>
    internal Type JobType { get; set; }

    /// <summary>
    /// 作业触发器集合
    /// </summary>
    internal IList<JobTrigger> Triggers { get; set; }

    /// <summary>
    /// 作业信息
    /// </summary>
    internal JobDetail JobDetail { get; set; }

    /// <summary>
    /// 查看最早触发时间
    /// </summary>
    /// <returns><see cref="DateTime"/></returns>
    public DateTime? GetNextOccurrence()
    {
        // 查看所有有效的作业触发器
        var effectiveTriggers = Triggers.Where(u => u.NextRunTime != null);
        if (!effectiveTriggers.Any())
        {
            return null;
        }

        // 获取最早触发器的作业触发器时间
        return effectiveTriggers.Min(u => u.NextRunTime);
    }

    /// <summary>
    /// 更新作业信息
    /// </summary>
    /// <param name="configureJobDetailBuilder">作业信息构建器委托</param>
    public void UpdateDetail(Action<JobDetailBuilder> configureJobDetailBuilder)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configureJobDetailBuilder);

        var jobDetailBuilder = new JobDetailBuilder();
        jobDetailBuilder.LoadTo(JobDetail);

        // 外部调用
        configureJobDetailBuilder(jobDetailBuilder);

        // 替换内存中的 JobDetail
        JobDetail = jobDetailBuilder.Build();
        JobType = jobDetailBuilder.RuntimeJobType!;
    }

    /// <summary>
    /// 开始作业调度器
    /// </summary>
    public void Start()
    {
        if (JobDetail.Status != JobStatus.Normal)
        {
            JobDetail.Status = JobStatus.Normal;
        }
    }

    /// <summary>
    /// 暂停作业调度器
    /// </summary>
    public void Pause()
    {
        if (JobDetail.Status != JobStatus.Pause)
        {
            JobDetail.Status = JobStatus.Pause;
        }
    }
}