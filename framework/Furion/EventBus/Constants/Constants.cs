// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.EventBus;

/// <summary>
/// EventBus 模块常量
/// </summary>
internal static class Constants
{
    /// <summary>
    /// 键常量
    /// </summary>
    internal static class Keys
    {
        /// <summary>
        /// EventBus 模块配置根节点
        /// </summary>
        internal const string Root = "EventBusSettings";

        /// <summary>
        /// 队列通道容量
        /// </summary>
        internal const string Capacity = $"{Root}:Capacity";
    }

    /// <summary>
    /// 值常量
    /// </summary>
    internal static class Values
    {
        /// <summary>
        /// <see cref="Keys.Capacity"/> 默认值
        /// </summary>
        internal const int Capacity = 100;
    }
}