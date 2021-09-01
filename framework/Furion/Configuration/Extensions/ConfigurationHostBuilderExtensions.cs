// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// 配置主机构建器拓展类
/// </summary>
public static class ConfigurationHostBuilderExtensions
{
    /// <summary>
    /// 添加框架初始配置
    /// </summary>
    /// <param name="hostBuilder">主机构建器</param>
    /// <returns></returns>
    public static IHostBuilder AddAppConfiguration(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureAppConfiguration((hostingContext, configurationBuilder) => configurationBuilder.Configure(hostingContext.Configuration, hostingContext.HostingEnvironment));

        return hostBuilder;
    }
}