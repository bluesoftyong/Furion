// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.Extensions.ObjectUtilities;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// App 模块自定义配置主机拓展类
/// </summary>
public static class AppHostBuilderExtensions
{
    /// <summary>
    /// App 模块配置节点
    /// </summary>
    private const string ConfigurationKey = "AppSettings";

    /// <summary>
    /// 配置 App 模块初始配置
    /// </summary>
    /// <param name="hostBuilder">主机构建器</param>
    /// <returns>IHostBuilder</returns>
    public static IHostBuilder ConfigureAppConfiguration(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureAppConfiguration((context, configurationBuilder) =>
         {
             var configuration = context.Configuration;
             var environment = context.HostingEnvironment;

             // 读取 AppSettings:EnvironmentVariablesPrefix 节点
             var environmentVariablesPrefix = configuration.GetValue($"{ConfigurationKey}:EnvironmentVariablesPrefix", "FURION_");
             // 添加环境配置支持及前缀支持
             configurationBuilder.AddEnvironmentVariables(prefix: environmentVariablesPrefix)
                                 // 添加自定义配置文件
                                 .AddCustomizeConfigurationFiles(configuration, environment);
         });
    }

    /// <summary>
    /// 添加自定义配置文件
    /// </summary>
    /// <param name="configurationBuilder">配置构建对象</param>
    /// <param name="configuration">配置对象</param>
    /// <param name="environment">环境对象</param>
    /// <returns>IConfigurationBuilder</returns>
    private static IConfigurationBuilder AddCustomizeConfigurationFiles(this IConfigurationBuilder configurationBuilder, IConfiguration configuration, IHostEnvironment? environment = default)
    {
        // 读取 AppSettings:CustomizeConfigurationFiles 节点
        var customizeConfigurationFiles = configuration.Get<string[]>($"{ConfigurationKey}:CustomizeConfigurationFiles");

        // 配置为空则跳过
        if (customizeConfigurationFiles.IsEmpty())
        {
            return configurationBuilder;
        }

        // 逐条添加自定义配置
        Array.ForEach(customizeConfigurationFiles, fileName =>
        {
            configurationBuilder.AddFile(fileName, environment);
        });

        return configurationBuilder;
    }
}