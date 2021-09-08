// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion;
using Furion.ObjectExtensions;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// 框架初始配置主机拓展类
/// </summary>
internal static class AppConfigurationHostBuilderExtensions
{
    /// <summary>
    /// 配置框架初始化配置
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <returns></returns>
    internal static IHostBuilder ConfigureAppConfiguration(this IHostBuilder hostBuilder)
    {
        // 添加框架初始配置
        hostBuilder.ConfigureAppConfiguration((context, configurationBuilder) =>
        {
            // 添加 Furion 框架环境变量配置支持
            configurationBuilder.AddEnvironmentVariables(prefix: context.Configuration.GetValue($"{AppSettingsOptions.sectionKey}:{nameof(AppSettingsOptions.EnvironmentVariablesPrefix)}", AppSettingsOptions.environmentVariablesPrefix))
                                .AddCustomizeConfigurationFiles(context.Configuration, context.HostingEnvironment);
        });

        return hostBuilder;
    }

    /// <summary>
    /// 添加自定义配置
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    internal static IConfigurationBuilder AddCustomizeConfigurationFiles(this IConfigurationBuilder configurationBuilder, IConfiguration configuration, IHostEnvironment? environment = default)
    {
        var userConfigurationFiles = configuration.Get<string[]>($"{AppSettingsOptions.sectionKey}:{nameof(AppSettingsOptions.CustomizeConfigurationFiles)}");
        if (userConfigurationFiles.IsEmpty())
        {
            return configurationBuilder;
        }

        // 遍历添加
        Array.ForEach(userConfigurationFiles, filePath
            => configurationBuilder.AddFile(filePath, environment));

        return configurationBuilder;
    }
}