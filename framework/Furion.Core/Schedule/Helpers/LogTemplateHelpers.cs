// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Text;

namespace Furion.Schedule;

/// <summary>
/// 日志模板版主类
/// </summary>
internal static class LogTemplateHelpers
{
    /// <summary>
    /// Schedule 模块服务运行日志模板
    /// </summary>
    internal static readonly string ScheduleRunningTemplate;

    /// <summary>
    /// Schedule 模块服务准备停止日志模板
    /// </summary>
    internal static readonly string ScheduleStoppingTemplate;

    /// <summary>
    /// Schedule 模块服务停止日志模板
    /// </summary>
    internal static readonly string ScheduleStoppedTemplate;

    /// <summary>
    /// 作业执行日志模板
    /// </summary>
    internal static readonly string JobExecutionTemplate;

    /// <summary>
    /// 作业执行异常日志模板
    /// </summary>
    internal static readonly string JobExecutionFailedTemplate;

    /// <summary>
    /// 添加作业调度器日志模板
    /// </summary>
    internal static readonly string AddSchedulerJobTemplate;

    /// <summary>
    /// 移除作业调度器日志模板
    /// </summary>
    internal static readonly string RemoveSchedulerJobTemplate;

    /// <summary>
    /// 开始所有作业调度器日志模板
    /// </summary>
    internal static readonly string StartAllSchedulerJobTemplate;

    /// <summary>
    /// 暂停所有作业调度器日志模板
    /// </summary>
    internal static readonly string PauseAllSchedulerJobTemplate;

    /// <summary>
    /// 静态构造函数
    /// </summary>
    static LogTemplateHelpers()
    {
        ScheduleRunningTemplate = GetTemplate(new[] { "JobCount" }, true, "Schedule Hosted Service is running.");
        ScheduleStoppingTemplate = GetTemplate(new[] { "Schedule Hosted Service is stopping." });
        ScheduleStoppedTemplate = GetTemplate(new[] { "Schedule Hosted Service is stopped." });

        AddSchedulerJobTemplate = GetTemplate(new[] {
            "JobId"
            , "Description"
            , "JobType"
            , "Triggers"
            , "EarliestNextRunTime"
            , "StartMode"
            , "ExecutionMode"}, true, "A job is successfully added to the schedule.");
        RemoveSchedulerJobTemplate = GetTemplate(new[] { "The <{JobId}> job has been removed from the schedule." });

        JobExecutionTemplate = GetTemplate(new[] {
            "JobId"
            , "Description"
            , "JobType"
            , "ExecutionMode"
            , "TriggerId"
            , "TriggerType"
            , "Args"
            , "NextRunTime"
            , "NumberOfRuns"
            , "NumberOfErrors"
            , "ExecutingTime"
            , "ExecutedTime"
            , "ExecutionTime"
            , "Exception"}
        , true
        , "Job execution information.");
        JobExecutionFailedTemplate = GetTemplate(new[] { "Exception" }
        , true
        , "Error occurred executing of <{JobId}>.");

        StartAllSchedulerJobTemplate = GetTemplate(new[] { "JobCount" }, true, "All jobs started.");
        PauseAllSchedulerJobTemplate = GetTemplate(new[] { "JobCount" }, true, "All jobs paused.");
    }

    /// <summary>
    /// 获取日志模板
    /// </summary>
    /// <param name="items">列表项</param>
    /// <param name="showProp">是否显示属性名</param>
    /// <param name="description">描述</param>
    /// <param name="title">标题</param>
    /// <returns><see cref="string"/></returns>
    internal static string GetTemplate(string[] items, bool showProp = false, string? description = default, string title = "Schedule Console")
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"┏━━━━━━━━━━━ {title} ━━━━━━━━━━━\r\n");

        // 添加描述
        if (!string.IsNullOrWhiteSpace(description))
        {
            stringBuilder.Append($"┣ {description}\r\n┣ \r\n");
        }

        // 判断是否显示属性
        if (!showProp)
        {
            Array.ForEach(items, item => stringBuilder.Append($"┣ {item}\r\n"));
        }
        else
        {
            // 取最大长度的字符串
            var maxLength = items.Max(u => u.Length) + 5;
            Array.ForEach(items, item =>
            {
                var newItem = $"{item}:";
                stringBuilder.Append($"┣ {newItem.PadRight(maxLength, ' ')}{{{item}}}\r\n");
            });
        }

        stringBuilder.Append($"┗━━━━━━━━━━━ {title} ━━━━━━━━━━━");
        return stringBuilder.ToString();
    }
}