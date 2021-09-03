// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// 框架内服务提供器工厂主机拓展类
/// </summary>
internal static class AppServiceProviderFactoryHostBuilderExtensions
{
    /// <summary>
    /// 使用框架提供的服务提供器工厂
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    internal static IHostBuilder UseAppServiceProviderFactory(this IHostBuilder hostBuilder, Action<HostBuilderContext, ServiceProviderOptions>? configure = default)
    {
        // 替换 .NET 默认工厂
        return hostBuilder.UseServiceProviderFactory((HostBuilderContext context) =>
        {
            // 创建默认配置选项
            var serviceProviderOptions = new ServiceProviderOptions();
            var validateOnBuild = (serviceProviderOptions.ValidateScopes = context.HostingEnvironment.IsDevelopment());
            serviceProviderOptions.ValidateOnBuild = validateOnBuild;

            configure?.Invoke(context, serviceProviderOptions);

            return new AppServiceProviderFactory(serviceProviderOptions);
        });
    }
}