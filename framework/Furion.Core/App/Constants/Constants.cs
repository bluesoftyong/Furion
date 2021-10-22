// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.App;

/// <summary>
/// App 模块常量
/// </summary>
internal static class Constants
{
    /// <summary>
    /// 键常量
    /// </summary>
    internal static class Keys
    {
        /// <summary>
        /// App 模块配置根节点
        /// </summary>
        internal const string Root = "AppSettings";

        /// <summary>
        /// 环境配置变量前缀节点
        /// </summary>
        internal const string EnvironmentVariablesPrefix = $"{Root}:EnvironmentVariablesPrefix";

        /// <summary>
        /// 自定义配置文件节点
        /// </summary>
        internal const string CustomizeConfigurationFiles = $"{Root}:CustomizeConfigurationFiles";
    }

    /// <summary>
    /// 值常量
    /// </summary>
    internal static class Values
    {
        /// <summary>
        /// <see cref="Keys.EnvironmentVariablesPrefix"/> 默认值
        /// </summary>
        internal const string EnvironmentVariablesPrefix = "FURION_";
    }
}