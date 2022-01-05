// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.SchedulerJob;

/// <summary>
/// 调度作业默认实现
/// </summary>
internal sealed class Scheduler : IScheduler
{
    /// <summary>
    /// 作业存储器
    /// </summary>
    private readonly IJobStorer _jobStorer;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="jobStorer">作业存储器</param>
    public Scheduler(IJobStorer jobStorer)
    {
        _jobStorer = jobStorer;
    }

    /// <summary>
    /// 开始作业
    /// </summary>
    /// <param name="identity">作业唯一标识</param>
    /// <param name="cancellationToken"> 取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    public Task StartAsync(string identity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 暂停作业
    /// </summary>
    /// <param name="identity">作业唯一标识</param>
    /// <param name="cancellationToken"> 取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    public Task PauseAsync(string identity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}