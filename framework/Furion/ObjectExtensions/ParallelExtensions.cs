// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Linq;
using System.Threading.Tasks;

namespace Furion.ObjectExtensions;

/// <summary>
/// Parallel 拓展
/// </summary>
public static class ParallelExtensions
{
    /// <summary>
    /// ParallelLoopResult 延续拓展
    /// </summary>
    /// <param name="result"></param>
    /// <param name="configureDelegate"></param>
    public static void ContinueWith(this ParallelLoopResult result, Func<ParallelLoopResult>[] actions, Action configureDelegate)
    {
        if (configureDelegate == default)
        {
            throw new ArgumentNullException(nameof(configureDelegate));
        }

        var i = 0;
        while (true)
        {
            if (result.IsCompleted)
            {
                if (i++ < actions.Length)
                {
                    actions[0]().ContinueWith(actions.Skip(1).ToArray(), configureDelegate);
                }

                configureDelegate();
                break;
            }
        }
    }
}