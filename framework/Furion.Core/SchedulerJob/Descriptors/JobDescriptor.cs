// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.SchedulerJob;

/// <summary>
/// 内置作业描述器
/// </summary>
internal sealed class JobDescriptor : IJobDescriptor
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="identity">唯一标识</param>
    internal JobDescriptor(string identity)
    {
        Identity = identity;
    }

    /// <summary>
    /// 唯一标识
    /// </summary>
    public string Identity { get; }

    /// <summary>
    /// 作业描述
    /// </summary>
    public string? Description { get; internal set; }

    /// <summary>
    /// 作业附加属性
    /// </summary>
    public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();
}