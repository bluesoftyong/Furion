// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Furion.Schedule;

/// <summary>
/// 作业调度器工厂默认实现
/// </summary>
internal sealed class SchedulerJobFactory : ISchedulerJobFactory
{
    /// <summary>
    /// 基于 Channel 实现信号灯机制
    /// </summary>
    /// <remarks>
    /// <para>控制作业调度器休眠、激活</para>
    /// </remarks>
    private readonly Channel<int> _signalLampChannel;

    /// <summary>
    /// 作业调度器字典集合
    /// </summary>
    private readonly ConcurrentDictionary<string, SchedulerJob> _schedulerJobs;

    /// <summary>
    /// 日志对象
    /// </summary>
    private readonly ILogger<SchedulerJobFactory> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志对象</param>
    public SchedulerJobFactory(ILogger<SchedulerJobFactory> logger)
    {
        // 配置 Channel 通道最多容纳消息为 1 个，超过 1 个进入等待
        var boundedChannelOptions = new BoundedChannelOptions(1)
        {
            FullMode = BoundedChannelFullMode.Wait
        };

        // 创建基于 Channel 信号灯通道
        _signalLampChannel = Channel.CreateBounded<int>(boundedChannelOptions);

        _schedulerJobs = new();
        _logger = logger;
    }

    /// <summary>
    /// 作业调度器集合
    /// </summary>
    public ICollection<SchedulerJob> SchedulerJobs => _schedulerJobs.Values;

    /// <summary>
    /// 根据作业 Id 获取作业调度器
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <param name="schedulerJob">作业调度器</param>
    /// <returns><see cref="bool"/></returns>
    public bool TryGet(string jobId, out SchedulerJob? schedulerJob)
    {
        return _schedulerJobs.TryGetValue(jobId, out schedulerJob);
    }

    /// <summary>
    /// 向工厂中追加作业调度器
    /// </summary>
    /// <param name="schedulerJob">调度作业对象</param>
    public void Append(SchedulerJob schedulerJob)
    {
        var jobId = schedulerJob.JobDetail.JobId!;

        // 检查作业 Id 唯一性
        if (!_schedulerJobs.TryAdd(jobId, schedulerJob))
        {
            throw new InvalidOperationException($"The job <{jobId}> has been registered. Repeated registration is prohibited.");
        }

        // 打印添加成功日志
        _logger.LogInformation(LogTemplateHelpers.AddSchedulerJobTemplate
            , jobId
            , schedulerJob.JobDetail.Description
            , schedulerJob.JobDetail.JobType
            , string.Join(';', schedulerJob.Triggers.Select(u => u.TriggerType + "/" + u.ToString()))
            , schedulerJob.GetEarliestNextRunTime()
            , schedulerJob.JobDetail.StartMode
            , schedulerJob.JobDetail.ExecutionMode);
    }

    /// <summary>
    /// 尝试删除作业调度器
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <param name="schedulerJob">作业调度器</param>
    /// <returns><see cref="bool"/></returns>
    public bool TryRemove(string jobId, out SchedulerJob? schedulerJob)
    {
        var canRemove = _schedulerJobs.TryRemove(jobId, out schedulerJob);

        // 打印移除日志
        if (canRemove)
        {
            _logger.LogWarning(LogTemplateHelpers.RemoveSchedulerJobTemplate, schedulerJob!.JobDetail.JobId);
        }

        return canRemove;
    }

    /// <summary>
    /// 启动所有作业调度器
    /// </summary>
    public void StartAll()
    {
        foreach (var schedulerJob in _schedulerJobs.Values)
        {
            schedulerJob.Start();
        }

        _logger.LogInformation(LogTemplateHelpers.StartAllSchedulerJobTemplate, SchedulerJobs.Count);
    }

    /// <summary>
    /// 暂停所有作业调度器
    /// </summary>
    public void PauseAll()
    {
        foreach (var schedulerJob in _schedulerJobs.Values)
        {
            schedulerJob.Pause();
        }

        _logger.LogInformation(LogTemplateHelpers.PauseAllSchedulerJobTemplate, SchedulerJobs.Count);
    }

    /// <summary>
    /// 休眠至适合时机唤醒
    /// </summary>
    /// <param name="delay">休眠时间（毫秒）</param>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns><see cref="Task"/></returns>
    public async Task SleepAsync(double delay, CancellationToken stoppingToken)
    {
        // 创建 Timer 定时器，休眠结束后写入信号灯管道
        using var timer = new System.Timers.Timer(delay);
        timer.Elapsed += (o, e) =>
        {
            // 唤醒后台主机服务
            _signalLampChannel.Writer.TryWrite(1);
        };
        timer!.AutoReset = false;
        timer.Start();

        // 阻塞线程读取管道消息
        _ = await _signalLampChannel.Reader.ReadAsync(stoppingToken);

        // 释放定时器
        timer.Dispose();
    }

    /// <summary>
    /// 让作业调度器工厂感知变化
    /// </summary>
    /// <remarks>主要用于动态添加作业调度器，唤醒调度激活等作用</remarks>
    public async Task NotifyChanges(CancellationToken cancellationToken = default)
    {
        await _signalLampChannel.Writer.WriteAsync(1, cancellationToken);
    }
}