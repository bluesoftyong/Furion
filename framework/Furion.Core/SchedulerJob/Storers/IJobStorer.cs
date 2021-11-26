﻿// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
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
    /// 同步存储器作业数据
    /// </summary>
    /// <param name="jobIds">已注册的作业 Id 集合</param>
    /// <param name="cancellationToken">取消任务 Token</param>
    /// <returns><see cref="Task{TResult}"/> 实例</returns>
    Task<IEnumerable<JobDetailBinder>> SyncAsync(string[] jobIds, CancellationToken cancellationToken = default);
}