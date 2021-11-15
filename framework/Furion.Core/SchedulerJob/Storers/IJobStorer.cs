// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.SchedulerJob;

/// <summary>
/// 作业存储器
/// </summary>
public interface IJobStorer
{
    /// <summary>
    /// 根据唯一标识获取作业描述器
    /// </summary>
    /// <param name="identity">唯一标识</param>
    /// <param name="cancellationToken">取消任务 Token</param>
    /// <returns><see cref="IJobDescriptor"/> 实例</returns>
    Task<IJobDescriptor> GetAsync(string identity, CancellationToken cancellationToken);

    /// <summary>
    /// 更新作业描述器
    /// </summary>
    /// <param name="descriptor">作业描述器</param>
    /// <param name="cancellationToken">取消任务 Token</param>
    /// <returns></returns>
    Task UpdateAsync(IJobDescriptor descriptor, CancellationToken cancellationToken);

    /// <summary>
    /// 更新作业状态
    /// </summary>
    /// <param name="identity">唯一标识</param>
    /// <param name="status">作业状态</param>
    /// <param name="cancellationToken">取消任务 Token</param>
    /// <returns><see cref="Task"/></returns>
    Task UpdateStatusAsync(string identity, JobStatus status, CancellationToken cancellationToken);
}