// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.SchedulerJob;

/// <summary>
/// 作业特性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class JobAttribute : Attribute
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="identity">作业唯一标识</param>
    public JobAttribute(string identity)
    {
        // 空检查
        if (string.IsNullOrWhiteSpace(identity))
        {
            throw new InvalidOperationException("The identity can be not null or empty.");
        }

        Identity = identity;
    }

    /// <summary>
    /// 作业唯一标识
    /// </summary>
    public string Identity { get; set; }

    /// <summary>
    /// 作业描述
    /// </summary>
    public string? Description { get; set; }
}