// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.EventBus;

/// <summary>
/// 事件订阅者处理程序过滤器
/// </summary>
/// <remarks>须和 <see cref="IEventSubscriber"/> 一起使用</remarks>
public interface IEventSubscriberFilter
{
    /// <summary>
    /// 事件订阅者处理程序执行前
    /// </summary>
    /// <param name="context">上下文</param>
    /// <returns></returns>
    Task OnHandlerExecutingAsync(EventSubscribeExecutingContext context);

    /// <summary>
    /// 事件订阅者处理程序执行后
    /// </summary>
    /// <param name="context">上下文</param>
    /// <returns></returns>
    Task OnHandlerExecutedAsync(EventSubscribeExecutedContext context);
}