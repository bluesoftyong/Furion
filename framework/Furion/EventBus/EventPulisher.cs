// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.EventBus;

/// <summary>
/// 事件发布服务默认实现
/// </summary>
internal sealed partial class EventPulisher : IEventPulisher
{
    /// <summary>
    /// 事件存取器
    /// </summary>
    private readonly IEventStoreChannel _eventStoreChannel;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="eventStoreChannel">事件存取器</param>
    public EventPulisher(IEventStoreChannel eventStoreChannel)
    {
        _eventStoreChannel = eventStoreChannel;
    }

    /// <summary>
    /// 发布一条消息
    /// </summary>
    /// <param name="eventSource">事件源</param>
    /// <returns><see cref="Task"/> 实例</returns>
    public async Task PublishAsync(IEventSource eventSource)
    {
        await _eventStoreChannel.WriteAsync(eventSource);
    }
}