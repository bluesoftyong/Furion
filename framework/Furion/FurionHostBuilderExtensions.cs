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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Diagnostics;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// 配置 Furion 框架初始化
/// </summary>
public static class FurionHostBuilderExtensions
{
    /// <summary>
    /// 初始化框架服务
    /// </summary>
    /// <param name="hostBuilder">主机构建器</param>
    /// <param name="configureDelegate">服务提供器配置</param>
    /// <returns></returns>
    public static IHostBuilder UseFurion(this IHostBuilder hostBuilder, Action<HostBuilderContext, ServiceProviderOptions>? configureDelegate = default)
    {
        // 配置框架诊断监听器
        DiagnosticListener.AllListeners.Subscribe(new FurionDiagnosticObserver());

        // 配置框架初始化配置
        hostBuilder.ConfigureAppConfiguration();

        // 替换 .NET 内置默认服务提供器工厂
        hostBuilder.UseAppServiceProviderFactory(configureDelegate);

        // 配置初始服务
        hostBuilder.ConfigureServices((context, services) =>
        {
            // 注册 App 全局应用对象服务
            services.AddApp(context.Configuration);

            // 注册框架服务提供器
            services.TryAddTransient<IAppServiceProvider, AppServiceProvider>();
        });

        return hostBuilder;
    }

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