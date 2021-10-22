// Copyright (c) 2020-2021 °ÙÐ¡É®, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

public sealed class CrontabException : Exception
{
    public CrontabException()
        : base()
    {
    }

    public CrontabException(string message)
        : base(message)
    {
    }

    public CrontabException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}