// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// 配置主机构建器拓展类
/// </summary>
internal static class ConfigurationHostBuilderExtensions
{
    /// <summary>
    /// 添加框架初始配置
    /// </summary>
    /// <param name="hostBuilder">主机构建器</param>
    /// <returns></returns>
    internal static IHostBuilder AddAppConfiguration(this IHostBuilder hostBuilder)
    {
        hostBuilder.Properties.Add("NamedServiceCollection", new Dictionary<string, Type>());
        hostBuilder.Properties.Add("AdditionAssemblies", new Dictionary<Assembly, Assembly>());

        hostBuilder.ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
        {
            hostingContext.Properties.Add(hostingContext, hostingContext);
            configurationBuilder.Configure(hostingContext.Configuration, hostingContext.HostingEnvironment);
        });

        return hostBuilder;
    }
}