// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.Options;

/// <summary>
/// 选项构建器依赖接口
/// </summary>
/// <typeparam name="TOptions">选项类型</typeparam>
public interface IOptionsBuilder<TOptions> : IConfigureOptions<TOptions>, IPostConfigureOptions<TOptions>, IValidateConfigureOptions<TOptions>
    where TOptions : class
{
}

/// <summary>
/// 选项构建器依赖接口
/// </summary>
/// <typeparam name="TOptions">选项类型</typeparam>
/// <typeparam name="TDep">依赖服务</typeparam>
public interface IOptionsBuilder<TOptions, TDep> : IConfigureOptions<TOptions, TDep>, IPostConfigureOptions<TOptions, TDep>, IValidateConfigureOptions<TOptions, TDep>
    where TOptions : class
    where TDep : class
{
}

/// <summary>
/// 选项构建器依赖接口
/// </summary>
/// <typeparam name="TOptions">选项类型</typeparam>
/// <typeparam name="TDep1">依赖服务</typeparam>
/// <typeparam name="TDep2">依赖服务</typeparam>
public interface IOptionsBuilder<TOptions, TDep1, TDep2> : IConfigureOptions<TOptions, TDep1, TDep2>, IPostConfigureOptions<TOptions, TDep1, TDep2>, IValidateConfigureOptions<TOptions, TDep1, TDep2>
    where TOptions : class
    where TDep1 : class
    where TDep2 : class
{
}

/// <summary>
/// 选项构建器依赖接口
/// </summary>
/// <typeparam name="TOptions">选项类型</typeparam>
/// <typeparam name="TDep1">依赖服务</typeparam>
/// <typeparam name="TDep2">依赖服务</typeparam>
/// <typeparam name="TDep3">依赖服务</typeparam>
public interface IOptionsBuilder<TOptions, TDep1, TDep2, TDep3> : IConfigureOptions<TOptions, TDep1, TDep2, TDep3>, IPostConfigureOptions<TOptions, TDep1, TDep2, TDep3>, IValidateConfigureOptions<TOptions, TDep1, TDep2, TDep3>
    where TOptions : class
    where TDep1 : class
    where TDep2 : class
    where TDep3 : class
{
}

/// <summary>
/// 选项构建器依赖接口
/// </summary>
/// <typeparam name="TOptions">选项类型</typeparam>
/// <typeparam name="TDep1">依赖服务</typeparam>
/// <typeparam name="TDep2">依赖服务</typeparam>
/// <typeparam name="TDep3">依赖服务</typeparam>
/// <typeparam name="TDep4">依赖服务</typeparam>
public interface IOptionsBuilder<TOptions, TDep1, TDep2, TDep3, TDep4> : IConfigureOptions<TOptions, TDep1, TDep2, TDep3, TDep4>, IPostConfigureOptions<TOptions, TDep1, TDep2, TDep3, TDep4>, IValidateConfigureOptions<TOptions, TDep1, TDep2, TDep3, TDep4>
    where TOptions : class
    where TDep1 : class
    where TDep2 : class
    where TDep3 : class
    where TDep4 : class
{
}

/// <summary>
/// 选项构建器依赖接口
/// </summary>
/// <typeparam name="TOptions">选项类型</typeparam>
/// <typeparam name="TDep1">依赖服务</typeparam>
/// <typeparam name="TDep2">依赖服务</typeparam>
/// <typeparam name="TDep3">依赖服务</typeparam>
/// <typeparam name="TDep4">依赖服务</typeparam>
/// <typeparam name="TDep5">依赖服务</typeparam>
public interface IOptionsBuilder<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5> : IConfigureOptions<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5>, IPostConfigureOptions<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5>, IValidateConfigureOptions<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5>
    where TOptions : class
    where TDep1 : class
    where TDep2 : class
    where TDep3 : class
    where TDep4 : class
    where TDep5 : class
{
}

/// <summary>
/// 选项构建器依赖空接口
/// </summary>
public interface IOptionsBuilder
{
}