// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 依赖注入服务拓展
/// </summary>
public static class DependencyInjectionServiceCollectionExtensions
{
    /// <summary>
    /// 创建依赖注入构建器
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="contextProperties"></param>
    /// <returns></returns>
    public static IDependencyInjectionBuilder AsServiceBuilder(this IServiceCollection services, IConfiguration configuration, IDictionary<object, object> contextProperties)
    {
        var namedServiceDescriptors = contextProperties[nameof(NamedServiceProvider)] as IDictionary<string, Type>;
        var builder = new DependencyInjectionBuilder(services, configuration, namedServiceDescriptors!);
        return builder;
    }
}