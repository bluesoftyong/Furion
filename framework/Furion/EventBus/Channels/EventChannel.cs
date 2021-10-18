// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading.Channels;

namespace Furion.EventBus;

/// <summary>
/// 事件通道依赖接口
/// </summary>
internal sealed partial class EventChannel : IEventChannel
{
    /// <summary>
    /// 事件源通道
    /// </summary>
    private readonly Channel<EventSource> _channel;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="capacity">队列通道默认容量，超过该容量进入等待</param>
    public EventChannel(int capacity)
    {
        // 配置通道，设置超出默认容量后进入等待
        var boundedChannelOptions = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };

        // 创建有限容量通道
        _channel = Channel.CreateBounded<EventSource>(boundedChannelOptions);
    }

    /// <summary>
    /// 写入事件源
    /// </summary>
    /// <param name="eventSource">事件源委托</param>
    /// <returns>ValueTask</returns>
    public async ValueTask WriteAsync(EventSource eventSource)
    {
        // 空检查
        if (eventSource == default)
        {
            throw new ArgumentNullException(nameof(eventSource));
        }

        // 写入管道队列
        await _channel.Writer.WriteAsync(eventSource);
    }

    /// <summary>
    /// 读取事件源
    /// </summary>
    /// <param name="cancellationToken">取消任务 Token</param>
    /// <returns>ValueTask{Func{CancellationToken, ValueTask}}</returns>
    public async ValueTask<EventSource> ReadAsync(CancellationToken cancellationToken)
    {
        // 读取管道队列
        var eventSource = await _channel.Reader.ReadAsync(cancellationToken);
        return eventSource;
    }
}