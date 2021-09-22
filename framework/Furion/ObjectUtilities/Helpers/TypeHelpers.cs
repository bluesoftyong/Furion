// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.Extensions.ObjectUtilities;

namespace Furion.Helpers.ObjectUtilities;

/// <summary>
/// 类型帮助类
/// </summary>
internal static class TypeHelpers
{
    /// <summary>
    /// 创建 Action 委托类型
    /// </summary>
    /// <param name="inputArguments">输入参数</param>
    /// <returns>Type</returns>
    internal static Type CreateActionDelegate(params Type[]? inputArguments)
    {
        var actionType = typeof(Action);
        if (inputArguments.IsEmpty())
        {
            return actionType;
        }

        return actionType.Assembly.GetType($"{actionType.FullName}`{inputArguments!.Length}")!.MakeGenericType(inputArguments);
    }

    /// <summary>
    /// 创建 Func 委托类型
    /// </summary>
    /// <param name="outputType">输出类型</param>
    /// <param name="inputArguments">输入参数</param>
    /// <returns>Type</returns>
    internal static Type CreateFuncDelegate(Type outputType, params Type[]? inputArguments)
    {
        var funcType = typeof(Func<>);
        if (inputArguments.IsEmpty())
        {
            return funcType.MakeGenericType(outputType);
        }

        return funcType.Assembly.GetType($"{(funcType.FullName![0..^2])}`{inputArguments!.Length + 1}")!.MakeGenericType(inputArguments.Concat(new[] { outputType }).ToArray());
    }
}