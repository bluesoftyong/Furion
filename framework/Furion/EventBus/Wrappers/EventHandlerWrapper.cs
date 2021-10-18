// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.EventBus;

internal sealed class EventHandlerWrapper
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="eventId">事件Id</param>
    public EventHandlerWrapper(string eventId)
    {
        EventId = eventId;
    }

    /// <summary>
    /// 事件Id
    /// </summary>
    internal string EventId { get; set; }

    /// <summary>
    /// 事件处理程序
    /// </summary>
    internal Func<EventSource, CancellationToken, Task>? Handler { get; set; }

    /// <summary>
    /// 判断当前事件处理程序是否符合执行
    /// </summary>
    /// <param name="eventId"></param>
    /// <returns></returns>
    internal bool ShouldRun(string eventId)
    {
        return EventId == eventId;
    }
}