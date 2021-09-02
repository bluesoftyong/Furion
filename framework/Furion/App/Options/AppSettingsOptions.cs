// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.Options;

namespace Furion;

/// <summary>
/// App 全局应用对象配置
/// </summary>
[AppOptions(sectionKey, ErrorOnUnknownConfiguration = true)]
public sealed class AppSettingsOptions : IAppOptions<AppSettingsOptions>
{
    /// <summary>
    /// 节点 Key
    /// </summary>
    internal const string sectionKey = "AppSettings";

    /// <summary>
    /// 环境变量配置 Key 前缀
    /// </summary>
    internal const string environmentVariablesPrefix = "FURION_";

    /// <summary>
    /// 环境变量配置 Key 前缀
    /// </summary>
    public string? EnvironmentVariablesPrefix { get; set; }

    /// <summary>
    /// 配置文件列表
    /// </summary>
    /// <remarks>
    /// <para>限定特定配置文件：[...]; 不加载配置：null</para>
    /// <para>@或~: 程序根目录（默认值）; /或!：绝对路径; &或.：程序执行目录（bin）</para>
    /// <para>支持 *.json;*.xml;*.ini</para>
    /// <para>支持配置 includeEnvironment、optional 和 reloadOnChange</para>
    /// </remarks>
    /// <example>["furion.json", "@furion.json", "&furion.json", "/D:/furion.json", "furion.json optional=true reloadOnChange=true"]</example>
    public string[]? CustomizeConfigurationFiles { get; set; }

    /// <summary>
    /// 后期配置
    /// </summary>
    /// <param name="options"></param>
    void IAppOptions<AppSettingsOptions>.PostConfigure(AppSettingsOptions options)
    {
        options.EnvironmentVariablesPrefix ??= environmentVariablesPrefix;
    }
}