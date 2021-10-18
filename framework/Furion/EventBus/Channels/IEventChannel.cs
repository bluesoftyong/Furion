// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.EventBus;

/// <summary>
/// 事件通道依赖接口
/// </summary>
internal interface IEventChannel
{
    /// <summary>
    /// 写入事件源
    /// </summary>
    /// <param name="eventSource">事件源委托</param>
    /// <returns>ValueTask</returns>
    ValueTask WriteAsync(EventSource eventSource);

    /// <summary>
    /// 读取事件源
    /// </summary>
    /// <param name="cancellationToken">取消任务 Token</param>
    /// <returns>ValueTask{Func{CancellationToken, ValueTask}}</returns>
    ValueTask<EventSource> ReadAsync(CancellationToken cancellationToken);
}