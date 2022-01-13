// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.Schedule.Extensions;

namespace Furion.Schedule;

/// <summary>
/// 作业调度程序
/// </summary>
internal sealed class Scheduler : IScheduler
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="jobType">作业类型</param>
    /// <param name="jobDetail">作业信息</param>
    /// <param name="triggers">作业触发器</param>
    internal Scheduler(Type jobType, JobDetail jobDetail, IList<JobTrigger> triggers)
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
    /// <param name="jobDetail">作业信息</param>
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
    /// <param name="configureJobBuilder">作业信息构建器委托</param>
    public void UpdateDetail(Action<JobBuilder> configureJobBuilder)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configureJobBuilder);

        // 将 JobDetail 转换成 JobBuilder
        var jobBuilder = JobDetail.ToBuilder();

        // 外部调用
        configureJobBuilder(jobBuilder);

        // 构建新的 JobDetail
        var (newJobDetail, jobType) = jobBuilder.Build();

        // 替换旧 JobDetail
        JobDetail = newJobDetail;
        JobType = jobType;
    }

    /// <summary>
    /// 更新作业触发器
    /// </summary>
    /// <param name="triggerId">作业触发器 Id</param>
    /// <param name="configureTriggerBuilder">作业触发器构建器委托</param>
    /// <returns><see cref="bool"/></returns>
    public bool UpdateTrigger(string triggerId, Action<TriggerBuilder> configureTriggerBuilder)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configureTriggerBuilder);

        // 根据作业触发器 Id 查找触发器
        var jobTrigger = Triggers.SingleOrDefault(u => u.TriggerId == triggerId);
        if (jobTrigger == null)
        {
            return false;
        }

        // 将 JobTrigger 转换成 TriggerBuilder
        var triggerBuilder = jobTrigger.ToBuilder();

        // 外部调用
        configureTriggerBuilder(triggerBuilder);

        // 构建新的 JobTrigger
        var newJobTrigger = triggerBuilder.Build(JobDetail.JobId!, null);

        // 替换旧 JobTrigger
        var isRemove = Triggers.Remove(jobTrigger);
        if (isRemove)
        {
            Triggers.Add(newJobTrigger);
        }

        return isRemove;
    }

    /// <summary>
    /// 添加作业调度器
    /// </summary>
    /// <param name="triggerBuilder">作业触发器构建器</param>
    /// <param name="startAt">启动时间</param>
    public void AddTrigger(TriggerBuilder triggerBuilder, DateTime? startAt)
    {
        var jobTrigger = triggerBuilder.Build(JobDetail.JobId!, startAt);
        Triggers.Add(jobTrigger);
    }

    /// <summary>
    /// 删除作业触发器
    /// </summary>
    /// <param name="triggerId">作业触发器 Id</param>
    /// <returns><see cref="bool"/></returns>
    public bool RemoveTrigger(string triggerId)
    {
        // 根据作业触发器 Id 查找触发器
        var jobTrigger = Triggers.SingleOrDefault(u => u.TriggerId == triggerId);
        if (jobTrigger == null)
        {
            return false;
        }

        return Triggers.Remove(jobTrigger);
    }

    /// <summary>
    /// 启动
    /// </summary>
    public void Start()
    {
        if (JobDetail.Status != JobStatus.Normal)
        {
            JobDetail.Status = JobStatus.Normal;
        }
    }

    /// <summary>
    /// 暂停
    /// </summary>
    public void Pause()
    {
        if (JobDetail.Status != JobStatus.Pause)
        {
            JobDetail.Status = JobStatus.Pause;
        }
    }
}