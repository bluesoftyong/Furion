// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Furion.TaskQueue;

/// <summary>
/// 任务队列后台主机服务
/// </summary>
/// <remarks>用于长时间监听任务项入队后进行出队调用</remarks>
internal sealed class TaskQueuedHostedService : BackgroundService
{
    /// <summary>
    /// 日志对象
    /// </summary>
    private readonly ILogger<TaskQueuedHostedService> _logger;

    /// <summary>
    /// 后台任务队列
    /// </summary>
    private readonly IBackgroundTaskQueue _taskQueue;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志对象</param>
    /// <param name="taskQueue">后台任务队列</param>
    public TaskQueuedHostedService(ILogger<TaskQueuedHostedService> logger
        , IBackgroundTaskQueue taskQueue)
    {
        _logger = logger;
        _taskQueue = taskQueue;
    }

    /// <summary>
    /// 执行后台任务
    /// </summary>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns>Task</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TaskQueued Hosted Service is running.");

        // 注册后台主机服务停止监听
        stoppingToken.Register(() =>
            _logger.LogDebug($"TaskQueued Hosted Service is stopping."));

        // 监听服务是否取消
        while (!stoppingToken.IsCancellationRequested)
        {
            // 出队并调用
            await BackgroundProcessing(stoppingToken);
        }

        _logger.LogDebug($"TaskQueued Hosted Service is stopped.");
    }

    /// <summary>
    /// 管道中任务出队并调用
    /// </summary>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns>Task</returns>
    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        // 出队
        var taskHandler = await _taskQueue.DequeueAsync(stoppingToken);

        try
        {
            // 调用任务处理委托
            await taskHandler(stoppingToken);
        }
        catch (Exception ex)
        {
            // 输出异常日志
            _logger.LogError(ex, "Error occurred executing {TaskHandler}.", nameof(taskHandler));
        }
    }
}