// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;

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
    /// <returns></returns>
    public static WebApplicationBuilder UseFurion(this WebApplicationBuilder webApplicationBuilder)
    {
        var services = webApplicationBuilder.Services;
        var configuration = webApplicationBuilder.Configuration;

        // 添加框架初始配置
        webApplicationBuilder.AddAppConfiguration();

        // 注册 HttpContext 访问器
        services.AddHttpContextAccessor();

        // 注册 App 全局应用对象服务
        services.AddApp(configuration);

        return webApplicationBuilder;
    }
}