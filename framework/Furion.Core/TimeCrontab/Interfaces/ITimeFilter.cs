// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

/// <summary>
/// 时间过滤器
/// </summary>
internal interface ITimeFilter
{
    /// <summary>
    /// 计算当前 Cron 字段种类（时间）下一个符合值
    /// </summary>
    /// <param name="currentValue">当前值</param>
    /// <returns><see cref="int"/></returns>
    int? Next(int currentValue);

    /// <summary>
    /// 获取当前 Cron 字段种类（时间）起始值
    /// </summary>
    /// <returns><see cref="int"/></returns>
    int First();
}