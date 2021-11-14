// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.SchedulerJob;

/// <summary>
/// 作业监视器
/// </summary>
public interface IJobMonitor
{
    /// <summary>
    /// 调度任务执行前监视
    /// </summary>
    /// <param name="cancellationToken">取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    Task OnExecutingAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 调度任务执行后监视
    /// </summary>
    /// <param name="exception">异常信息</param>
    /// <param name="cancellationToken">取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    Task OnExecutedAsync(Exception? exception, CancellationToken cancellationToken);
}