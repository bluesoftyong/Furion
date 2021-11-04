// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.Scheduler;

/// <summary>
/// 任务执行监视器
/// </summary>
public interface IScheduledTaskMonitor
{
    /// <summary>
    /// 执行前
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task OnExecutingAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 执行后
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task OnExecutedAsync(Exception? exception, CancellationToken cancellationToken);
}