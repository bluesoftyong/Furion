// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 框架内置服务提供器工厂
/// </summary>
public sealed class AppServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
{
    /// <summary>
    /// 服务提供器选项
    /// </summary>
    private readonly ServiceProviderOptions _options;

    /// <summary>
    /// 构造函数
    /// </summary>
    public AppServiceProviderFactory()
        : this(new ServiceProviderOptions())
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options"></param>
    public AppServiceProviderFactory(ServiceProviderOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// 创建服务集合
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public IServiceCollection CreateBuilder(IServiceCollection services)
    {
        return services;
    }

    /// <summary>
    /// 构建服务提供器
    /// </summary>
    /// <param name="containerBuilder"></param>
    /// <returns></returns>
    public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
    {
        return new AppServiceProvider(containerBuilder.BuildServiceProvider(_options));
    }
}