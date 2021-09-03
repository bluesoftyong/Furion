// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// 配置 Furion 框架初始化
/// </summary>
public static class FurionWebApplicationBuilderExtensions
{
    /// <summary>
    /// 初始化框架服务
    /// </summary>
    /// <param name="webApplicationBuilder">WebApplication 构建器</param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static WebApplicationBuilder UseFurion(this WebApplicationBuilder webApplicationBuilder, Action<HostBuilderContext, ServiceProviderOptions>? configure = default)
    {
        webApplicationBuilder.Host.UseFurion(configure);

        return webApplicationBuilder;
    }
}