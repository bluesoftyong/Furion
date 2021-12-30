// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.SchedulerJob;

/// <summary>
/// 作业详情构建器
/// </summary>
public sealed class JobDetailBuilder
{
    /// <summary>
    /// 作业触发器
    /// </summary>
    private readonly IList<JobTrigger> _triggers;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    public JobDetailBuilder(string jobId)
    {
        _triggers = new List<JobTrigger>();
        JobId = jobId;
    }

    /// <summary>
    /// 作业 Id
    /// </summary>
    public string JobId { get; }

    /// <summary>
    /// 添加作业触发器
    /// </summary>
    /// <typeparam name="TJobTrigger"><see cref="JobTrigger"/> 派生类</typeparam>
    /// <param name="args">触发器构造函数参数</param>
    public void AddTrigger<TJobTrigger>(params object[] args)
        where TJobTrigger : JobTrigger
    {
        AddTrigger(typeof(TJobTrigger), args);
    }

    /// <summary>
    /// 添加作业触发器
    /// </summary>
    /// <param name="triggerType">作业触发器类型</param>
    /// <param name="args">触发器构造函数参数</param>
    public void AddTrigger(Type triggerType, params object[] args)
    {
        // 检查 triggerType 类型是否派生自 JobTrigger
        if (!typeof(JobTrigger).IsAssignableFrom(triggerType))
        {
            throw new InvalidOperationException("The <triggerType> is not a valid JobTrigger type.");
        }

        // 反射创建作业触发器
        var jobTrigger = (args == null || args.Length == 0
            ? Activator.CreateInstance(triggerType)
            : Activator.CreateInstance(triggerType, args)) as JobTrigger;

        // 设置作业触发器 Id（不可更改）
        jobTrigger!.JobTriggerId = $"{JobId}_trigger_{_triggers.Count + 1}";

        _triggers.Add(jobTrigger!);
    }

    /// <summary>
    /// 构建作业详情构建器
    /// </summary>
    /// <returns>作业触发器集合</returns>
    public IEnumerable<JobTrigger> Build()
    {
        return _triggers;
    }
}