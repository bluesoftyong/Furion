// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Furion.Schedule;

/// <summary>
/// 处理 JSON 反序列化时作业触发器构造函数参数类型转换
/// </summary>
internal class JobTriggerConstructorParameterTypesConverter : JsonConverter<object?>
{
    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="typeToConvert"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public override object? Read(ref Utf8JsonReader reader
        , Type typeToConvert
        , JsonSerializerOptions options)
    {
        object? obj = reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Number when reader.TryGetInt32(out var l) => l,
            JsonTokenType.Number => reader.GetDouble(),
            JsonTokenType.String when reader.TryGetDateTime(out var datetime) => datetime,
            JsonTokenType.String => reader.GetString(),
            _ => JsonDocument.ParseValue(ref reader).RootElement.Clone()
        };

        return obj;
    }

    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="objectToWrite"></param>
    /// <param name="options"></param>
    /// <exception cref="NotImplementedException"></exception>
    public override void Write(Utf8JsonWriter writer
        , object? objectToWrite
        , JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}