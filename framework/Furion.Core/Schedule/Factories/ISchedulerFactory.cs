// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.Schedule;

/// <summary>
/// 调度工厂依赖接口
/// </summary>
public interface ISchedulerFactory
{
    /// <summary>
    /// 动态添加作业
    /// </summary>
    /// <typeparam name="TJob"><see cref="IJob"/> 实现类</typeparam>
    /// <param name="jobId">作业 Id</param>
    /// <param name="configureSchedulerJobBuilder">调度作业构建器委托</param>
    void AddJob<TJob>(string jobId, Action<SchedulerJobBuilder> configureSchedulerJobBuilder)
        where TJob : class, IJob;

    /// <summary>
    /// 动态添加作业
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <param name="jobType">作业类型</param>
    /// <param name="configureSchedulerJobBuilder">调度作业构建器委托</param>
    void AddJob(string jobId, Type jobType, Action<SchedulerJobBuilder> configureSchedulerJobBuilder);

    /// <summary>
    /// 动态添加作业
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <param name="job">作业对象</param>
    /// <param name="configureSchedulerJobBuilder">调度作业构建器委托</param>
    void AddJob(string jobId, IJob job, Action<SchedulerJobBuilder> configureSchedulerJobBuilder);
}