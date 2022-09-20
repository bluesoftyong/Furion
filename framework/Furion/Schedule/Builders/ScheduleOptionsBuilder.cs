// MIT License
//
// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd and Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Concurrent;

namespace Furion.Schedule;

/// <summary>
/// 定时任务配置选项构建器
/// </summary>
[SuppressSniffer]
public sealed class ScheduleOptionsBuilder
{
    /// <summary>
    /// 作业调度计划构建器集合
    /// </summary>
    private readonly ConcurrentDictionary<string, JobSchedulerBuilder> _jobSchedulerBuilders = new();

    /// <summary>
    /// 默认内置事件源存储器内存通道容量
    /// </summary>
    /// <remarks>超过 n 条待处理消息，第 n+1 条将进入等待，默认为 3000</remarks>
    public int ChannelCapacity { get; set; } = 3000;

    /// <summary>
    /// 是否使用 UTC 时间戳，默认 false
    /// </summary>
    public bool UseUtcTimestamp { get; set; } = false;

    /// <summary>
    /// 未察觉任务异常事件处理程序
    /// </summary>
    public EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskExceptionHandler { get; set; }

    /// <summary>
    /// 注册作业
    /// </summary>
    /// <param name="jobSchedulerBuilder">作业调度程序构建器</param>
    /// <returns><see cref="ScheduleOptionsBuilder"/></returns>
    public ScheduleOptionsBuilder AddJob(JobSchedulerBuilder jobSchedulerBuilder)
    {
        // 空检查
        if (jobSchedulerBuilder == null) throw new ArgumentNullException(nameof(jobSchedulerBuilder));

        // 将作业调度计划添加到集合中
        var succeed = _jobSchedulerBuilders.TryAdd(jobSchedulerBuilder.JobId, jobSchedulerBuilder);

        // 检查 Id 重复
        if (!succeed) throw new InvalidOperationException($"The JobId of <{jobSchedulerBuilder.JobId}> already exists.");

        return this;
    }

    /// <summary>
    /// 构建配置选项
    /// </summary>
    /// <param name="services">服务集合对象</param>
    internal ConcurrentDictionary<string, JobScheduler> Build(IServiceCollection services)
    {
        var jobSchedulers = new ConcurrentDictionary<string, JobScheduler>();

        // 构建作业调度计划和注册作业处理程序类型
        foreach (var (jobId, jobSchedulerBuilder) in _jobSchedulerBuilders)
        {
            var jobScheduler = jobSchedulerBuilder.Build();
            var jobType = jobScheduler.JobDetail.RuntimeJobType;

            _ = jobSchedulers.TryAdd(jobId, jobScheduler);

            // 注册作业处理程序为单例
            services.TryAddSingleton(jobType, jobType);
        }

        return jobSchedulers;
    }
}