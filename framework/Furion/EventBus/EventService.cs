// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.EventBus;

/// <summary>
/// 事件服务默认实现
/// </summary>
internal sealed partial class EventService : IEventService
{
    /// <summary>
    /// 事件存储器
    /// </summary>
    private readonly IEventStoreChannel _eventStoreChannel;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="eventStoreChannel">事件存储器</param>
    public EventService(IEventStoreChannel eventStoreChannel)
    {
        _eventStoreChannel = eventStoreChannel;
    }

    /// <summary>
    /// 发送一条消息
    /// </summary>
    /// <param name="eventId">事件 Id</param>
    /// <param name="payload">事件承载（携带）数据</param>
    /// <param name="cancellationToken">取消任务 Token</param>
    /// <returns><see cref="Task"/></returns>
    public async Task EmitAsync(string eventId, object? payload, CancellationToken cancellationToken = default)
    {
        await _eventStoreChannel.WriteAsync(new EventSource(eventId)
        {
            Payload = payload,
            CancellationToken = cancellationToken
        });
    }
}