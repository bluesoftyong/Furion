// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.Schedule;

/// <summary>
/// 作业调度器工厂依赖接口
/// </summary>
internal interface ISchedulerFactory
{
    /// <summary>
    /// 作业调度器集合
    /// </summary>
    ICollection<Scheduler> SchedulerJobs { get; }

    /// <summary>
    /// 根据作业 Id 获取作业调度器
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <param name="schedulerJob">作业调度器</param>
    /// <returns><see cref="bool"/></returns>
    bool TryGet(string jobId, out Scheduler? schedulerJob);

    /// <summary>
    /// 向工厂中追加作业调度器
    /// </summary>
    /// <param name="schedulerJob">调度作业对象</param>
    void Append(Scheduler schedulerJob);

    /// <summary>
    /// 尝试删除作业调度器
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <param name="schedulerJob">作业调度器</param>
    /// <returns><see cref="bool"/></returns>
    bool TryRemove(string jobId, out Scheduler? schedulerJob);

    /// <summary>
    /// 启动所有作业调度器
    /// </summary>
    void StartAll();

    /// <summary>
    /// 暂停所有作业调度器
    /// </summary>
    void PauseAll();

    /// <summary>
    /// 休眠至适合时机唤醒
    /// </summary>
    /// <param name="delay">休眠时间（毫秒）</param>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns><see cref="Task"/></returns>
    Task SleepAsync(double delay, CancellationToken stoppingToken);

    /// <summary>
    /// 让作业调度器工厂感知变化
    /// </summary>
    /// <remarks>主要用于动态添加作业调度器，唤醒调度激活等作用</remarks>
    Task NotifyChanges(CancellationToken cancellationToken = default);
}