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
public interface IOptionsBuilder<TOptions> : IOptionsBuilder
    where TOptions : class
{
    /// <summary>
    /// 选项配置
    /// </summary>
    /// <param name="options">选项实例</param>
    void Configure(TOptions options);

    /// <summary>
    /// 选项后期配置
    /// </summary>
    /// <param name="options">选项实例</param>
    void PostConfigure(TOptions options);

    /// <summary>
    /// 选项复杂验证
    /// </summary>
    /// <param name="options">选项实例</param>
    /// <returns>bool</returns>
    bool Validate(TOptions options);
}

/// <summary>
/// 选项构建器依赖接口
/// </summary>
/// <typeparam name="TOptions">选项类型</typeparam>
/// <typeparam name="TDep">依赖服务</typeparam>
public interface IOptionsBuilder<TOptions, TDep> : IOptionsBuilder
    where TOptions : class
    where TDep : class
{
    /// <summary>
    /// 选项配置
    /// </summary>
    /// <param name="options">选项实例</param>
    /// <param name="dep">依赖服务</param>
    void Configure(TOptions options, TDep dep);

    /// <summary>
    /// 选项后期配置
    /// </summary>
    /// <param name="options">选项实例</param>
    /// <param name="dep">依赖服务</param>
    void PostConfigure(TOptions options, TDep dep);

    /// <summary>
    /// 选项复杂验证
    /// </summary>
    /// <param name="options">选项实例</param>
    /// <param name="dep">依赖服务</param>
    /// <returns>bool</returns>
    bool Validate(TOptions options, TDep dep);
}

/// <summary>
/// 选项构建器依赖接口
/// </summary>
/// <typeparam name="TOptions">选项类型</typeparam>
/// <typeparam name="TDep1">依赖服务</typeparam>
/// <typeparam name="TDep2">依赖服务</typeparam>
public interface IOptionsBuilder<TOptions, TDep1, TDep2> : IOptionsBuilder
    where TOptions : class
    where TDep1 : class
    where TDep2 : class
{
    /// <summary>
    /// 选项配置
    /// </summary>
    /// <param name="options">选项实例</param>
    /// <param name="dep1">依赖服务</param>
    /// <param name="dep2">依赖服务</param>
    void Configure(TOptions options, TDep1 dep1, TDep2 dep2);

    /// <summary>
    /// 选项后期配置
    /// </summary>
    /// <param name="options">选项实例</param>
    /// <param name="dep1">依赖服务</param>
    /// <param name="dep2">依赖服务</param>
    void PostConfigure(TOptions options, TDep1 dep1, TDep2 dep2);

    /// <summary>
    /// 选项复杂验证
    /// </summary>
    /// <param name="options">选项实例</param>
    /// <param name="dep1">依赖服务</param>
    /// <param name="dep2">依赖服务</param>
    /// <returns>bool</returns>
    bool Validate(TOptions options, TDep1 dep1, TDep2 dep2);
}

/// <summary>
/// 选项构建器依赖接口
/// </summary>
/// <typeparam name="TOptions">选项类型</typeparam>
/// <typeparam name="TDep1">依赖服务</typeparam>
/// <typeparam name="TDep2">依赖服务</typeparam>
/// <typeparam name="TDep3">依赖服务</typeparam>
public interface IOptionsBuilder<TOptions, TDep1, TDep2, TDep3> : IOptionsBuilder
    where TOptions : class
    where TDep1 : class
    where TDep2 : class
    where TDep3 : class
{
    /// <summary>
    /// 选项配置
    /// </summary>
    /// <param name="options">选项实例</param>
    /// <param name="dep1">依赖服务</param>
    /// <param name="dep2">依赖服务</param>
    /// <param name="dep3">依赖服务</param>
    void Configure(TOptions options, TDep1 dep1, TDep2 dep2, TDep3 dep3);

    /// <summary>
    /// 选项后期配置
    /// </summary>
    /// <param name="options">选项实例</param>
    /// <param name="dep1">依赖服务</param>
    /// <param name="dep2">依赖服务</param>
    /// <param name="dep3">依赖服务</param>
    void PostConfigure(TOptions options, TDep1 dep1, TDep2 dep2, TDep3 dep3);

    /// <summary>
    /// 选项复杂验证
    /// </summary>
    /// <param name="options">选项实例</param>
    /// <param name="dep1">依赖服务</param>
    /// <param name="dep2">依赖服务</param>
    /// <param name="dep3">依赖服务</param>
    /// <returns>bool</returns>
    bool Validate(TOptions options, TDep1 dep1, TDep2 dep2, TDep3 dep3);
}

/// <summary>
/// 选项构建器依赖接口
/// </summary>
/// <typeparam name="TOptions">选项类型</typeparam>
/// <typeparam name="TDep1">依赖服务</typeparam>
/// <typeparam name="TDep2">依赖服务</typeparam>
/// <typeparam name="TDep3">依赖服务</typeparam>
/// <typeparam name="TDep4">依赖服务</typeparam>
public interface IOptionsBuilder<TOptions, TDep1, TDep2, TDep3, TDep4> : IOptionsBuilder
    where TOptions : class
    where TDep1 : class
    where TDep2 : class
    where TDep3 : class
    where TDep4 : class
{
    /// <summary>
    /// 选项配置
    /// </summary>
    /// <param name="options">选项实例</param>
    /// <param name="dep1">依赖服务</param>
    /// <param name="dep2">依赖服务</param>
    /// <param name="dep3">依赖服务</param>
    /// <param name="dep4">依赖服务</param>
    void Configure(TOptions options, TDep1 dep1, TDep2 dep2, TDep3 dep3, TDep4 dep4);

    /// <summary>
    /// 选项后期配置
    /// </summary>
    /// <param name="options">选项实例</param>
    /// <param name="dep1">依赖服务</param>
    /// <param name="dep2">依赖服务</param>
    /// <param name="dep3">依赖服务</param>
    /// <param name="dep4">依赖服务</param>
    void PostConfigure(TOptions options, TDep1 dep1, TDep2 dep2, TDep3 dep3, TDep4 dep4);

    /// <summary>
    /// 选项复杂验证
    /// </summary>
    /// <param name="options">选项实例</param>
    /// <param name="dep1">依赖服务</param>
    /// <param name="dep2">依赖服务</param>
    /// <param name="dep3">依赖服务</param>
    /// <param name="dep4">依赖服务</param>
    /// <returns>bool</returns>
    bool Validate(TOptions options, TDep1 dep1, TDep2 dep2, TDep3 dep3, TDep4 dep4);
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
public interface IOptionsBuilder<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5> : IOptionsBuilder
    where TOptions : class
    where TDep1 : class
    where TDep2 : class
    where TDep3 : class
    where TDep4 : class
    where TDep5 : class
{
    /// <summary>
    /// 选项配置
    /// </summary>
    /// <param name="options">选项实例</param>
    /// <param name="dep1">依赖服务</param>
    /// <param name="dep2">依赖服务</param>
    /// <param name="dep3">依赖服务</param>
    /// <param name="dep4">依赖服务</param>
    /// <param name="dep5">依赖服务</param>
    void Configure(TOptions options, TDep1 dep1, TDep2 dep2, TDep3 dep3, TDep4 dep4, TDep5 dep5);

    /// <summary>
    /// 选项后期配置
    /// </summary>
    /// <param name="options">选项实例</param>
    /// <param name="dep1">依赖服务</param>
    /// <param name="dep2">依赖服务</param>
    /// <param name="dep3">依赖服务</param>
    /// <param name="dep4">依赖服务</param>
    /// <param name="dep5">依赖服务</param>
    void PostConfigure(TOptions options, TDep1 dep1, TDep2 dep2, TDep3 dep3, TDep4 dep4, TDep5 dep5);

    /// <summary>
    /// 选项复杂验证
    /// </summary>
    /// <param name="options">选项实例</param>
    /// <param name="dep1">依赖服务</param>
    /// <param name="dep2">依赖服务</param>
    /// <param name="dep3">依赖服务</param>
    /// <param name="dep4">依赖服务</param>
    /// <param name="dep5">依赖服务</param>
    /// <returns>bool</returns>
    bool Validate(TOptions options, TDep1 dep1, TDep2 dep2, TDep3 dep3, TDep4 dep4, TDep5 dep5);
}

/// <summary>
/// 选项构建器依赖空接口
/// </summary>
public interface IOptionsBuilder
{
}