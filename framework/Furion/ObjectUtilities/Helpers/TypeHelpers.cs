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
    /// 创建委托类型
    /// </summary>
    /// <param name="inputTypes">输入类型</param>
    /// <param name="outputType">输出类型</param>
    /// <returns>Type</returns>
    internal static Type CreateDelegate(Type[] inputTypes, Type? outputType = default)
    {
        var isFuncDelegate = outputType != default;

        var baseDelegateType = !isFuncDelegate ? typeof(Action) : typeof(Func<>);

        if (inputTypes.IsEmpty())
        {
            return !isFuncDelegate ? baseDelegateType : baseDelegateType.MakeGenericType(outputType!);
        }

        return !isFuncDelegate
            ? baseDelegateType.Assembly.GetType($"{baseDelegateType.FullName}`{inputTypes!.Length}")!.MakeGenericType(inputTypes)
            : baseDelegateType.Assembly.GetType($"{(baseDelegateType.FullName![0..^2])}`{inputTypes!.Length + 1}")!.MakeGenericType(inputTypes.Concat(new[] { outputType! }).ToArray());
    }
}