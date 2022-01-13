// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.Schedule.Extensions;

/// <summary>
/// Schedule 模块拓展类
/// </summary>
internal static class ScheduleExtensions
{
    /// <summary>
    /// 将 JobDetal 转成 JobBuilder 对象
    /// </summary>
    /// <param name="jobDetail">作业信息</param>
    /// <returns><see cref="JobBuilder"/></returns>
    internal static JobBuilder ToBuilder(this JobDetail jobDetail)
    {
        return JobBuilder.Create(jobDetail.AssemblyName!, jobDetail.JobType!)
                         .WithIdentity(jobDetail.JobId!)
                         .SetDescription(jobDetail.Description)
                         .SetStatus(jobDetail.Status)
                         .SetStartMode(jobDetail.StartMode)
                         .SetLockMode(jobDetail.LockMode)
                         .SetPrintExecutionLog(jobDetail.PrintExecutionLog)
                         .SetWithScopeExecution(jobDetail.WithScopeExecution);
    }

    /// <summary>
    /// 将 JobTrigger 转成 TriggerBuilder 对象
    /// </summary>
    /// <param name="jobTrigger">作业触发器</param>
    /// <returns><see cref="JobBuilder"/></returns>
    internal static TriggerBuilder ToBuilder(this JobTrigger jobTrigger)
    {
        return TriggerBuilder.Create(jobTrigger.AssemblyName!, jobTrigger.TriggerType!)
                         .WithIdentity(jobTrigger.TriggerId!)
                         .WithArgs(jobTrigger.Args)
                         .SetDescription(jobTrigger.Description)
                         .SetLastRunTime(jobTrigger.LastRunTime)
                         .SetNextRunTime(jobTrigger.NextRunTime)
                         .SetNumberOfRuns(jobTrigger.NumberOfRuns)
                         .SetMaxNumberOfRuns(jobTrigger.MaxNumberOfRuns)
                         .SetNumberOfErrors(jobTrigger.NumberOfErrors)
                         .SetMaxNumberOfErrors(jobTrigger.MaxNumberOfErrors)
                         .SetExecuteOnAdded(jobTrigger.ExecuteOnAdded);
    }
}