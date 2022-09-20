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

using Furion.Schedule;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Schedule 模块服务拓展
/// </summary>
[SuppressSniffer]
public static class ScheduleServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Schedule 模块注册
    /// </summary>
    /// <param name="services">服务集合对象</param>
    /// <param name="configureOptionsBuilder">定时任务配置选项构建器委托</param>
    /// <returns>服务集合实例</returns>
    public static IServiceCollection AddSchedule(this IServiceCollection services, Action<ScheduleOptionsBuilder> configureOptionsBuilder)
    {
        // 创建初始定时任务配置选项构建器
        var scheduleOptionsBuilder = new ScheduleOptionsBuilder();
        configureOptionsBuilder.Invoke(scheduleOptionsBuilder);

        return services.AddSchedule(scheduleOptionsBuilder);
    }

    /// <summary>
    /// 添加 Schedule 模块注册
    /// </summary>
    /// <param name="services">服务集合对象</param>
    /// <param name="scheduleOptionsBuilder">定时任务配置选项构建器</param>
    /// <returns>服务集合实例</returns>
    public static IServiceCollection AddSchedule(this IServiceCollection services, ScheduleOptionsBuilder scheduleOptionsBuilder = default)
    {
        // 初始化定时任务配置项
        scheduleOptionsBuilder ??= new ScheduleOptionsBuilder();

        // 注册内部服务
        services.AddInternalService(scheduleOptionsBuilder);

        // 通过工厂模式创建
        //services.AddHostedService(serviceProvider =>
        //{
        //});

        // 构建定时任务服务
        var jobSchedulers = scheduleOptionsBuilder.Build(services);

        return services;
    }

    /// <summary>
    /// 注册内部服务
    /// </summary>
    /// <param name="services">服务集合对象</param>
    /// <param name="scheduleOptionsBuilder">定时任务配置选项构建器</param>
    /// <returns>服务集合实例</returns>
    private static IServiceCollection AddInternalService(this IServiceCollection services, ScheduleOptionsBuilder scheduleOptionsBuilder)
    {
        return services;
    }
}