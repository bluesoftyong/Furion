// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading.Channels;

namespace Furion.TaskQueue;

/// <summary>
/// 后台任务队列默认实现
/// </summary>
internal sealed class BackgroundTaskQueue : IBackgroundTaskQueue
{
    /// <summary>
    /// 队列通道
    /// </summary>
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="capacity">队列通道默认容量，超过该容量进入等待</param>
    public BackgroundTaskQueue(int capacity)
    {
        // 配置通道，设置超出默认容量后进入等待
        var boundedChannelOptions = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };

        // 创建有限容量通道
        _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(boundedChannelOptions);
    }

    /// <summary>
    /// 任务项入队
    /// </summary>
    /// <param name="workItem">任务处理委托</param>
    /// <returns>ValueTask</returns>
    /// <exception cref="ArgumentNullException">如果 workItem 为空，则抛空异常</exception>
    public async ValueTask EnqueueAsync(Func<CancellationToken, ValueTask> workItem)
    {
        // 空检查
        if (workItem == default)
        {
            throw new ArgumentNullException(nameof(workItem));
        }

        await _queue.Writer.WriteAsync(workItem);
    }

    /// <summary>
    /// 任务项出队
    /// </summary>
    /// <param name="cancellationToken">取消任务 Token</param>
    /// <returns>ValueTask{Func{CancellationToken, ValueTask}}</returns>
    public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
    {
        var workItem = await _queue.Reader.ReadAsync(cancellationToken);
        return workItem;
    }
}