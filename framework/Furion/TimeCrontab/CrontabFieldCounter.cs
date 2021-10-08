// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Globalization;
using System.Runtime.Serialization;

namespace Furion.TimeCrontab;

/// <summary>
/// Cron 表达式字段分析器
/// </summary>
internal sealed class CrontabFieldCounter : IObjectReference
{
    /// <summary>
    /// Cron 表达式 分钟 字段分析器
    /// </summary>
    internal static readonly CrontabFieldCounter Minute = new(CrontabFieldKind.Minute, 0, 59);

    /// <summary>
    /// Cron 表达式 时 字段分析器
    /// </summary>
    internal static readonly CrontabFieldCounter Hour = new(CrontabFieldKind.Hour, 0, 23);

    /// <summary>
    /// Cron 表达式 天 字段分析器
    /// </summary>
    internal static readonly CrontabFieldCounter Day = new(CrontabFieldKind.Day, 1, 31);

    /// <summary>
    /// Cron 表达式 月 字段分析器
    /// </summary>
    internal static readonly CrontabFieldCounter Month = new(CrontabFieldKind.Month, 1, 12,
        new[]
        {
                "January", "February", "March", "April",
                "May", "June", "July", "August",
                "September", "October", "November",
                "December"
        });

    /// <summary>
    /// Cron 表达式 周 字段分析器
    /// </summary>
    internal static readonly CrontabFieldCounter DayOfWeek = new(CrontabFieldKind.DayOfWeek, 0, 6,
        new[]
        {
                "Sunday", "Monday", "Tuesday",
                "Wednesday", "Thursday", "Friday",
                "Saturday"
        });

    private static readonly CrontabFieldCounter[] FieldByKind = { Minute, Hour, Day, Month, DayOfWeek };

    private static readonly CompareInfo Comparer = CultureInfo.InvariantCulture.CompareInfo;
    private static readonly char[] Comma = { ',' };

    private CrontabFieldCounter(CrontabFieldKind kind, int minValue, int maxValue, string[]? names = default)
    {
        Kind = kind;
        MinValue = minValue;
        MaxValue = maxValue;
        ValueCount = maxValue - minValue + 1;
        Names = names;
    }

    public CrontabFieldKind Kind { get; }

    public int MinValue { get; }

    public int MaxValue { get; }

    public int ValueCount { get; }

    public string[]? Names { get; }

    object IObjectReference.GetRealObject(StreamingContext context)
    {
        return FromKind(Kind);
    }

    public static CrontabFieldCounter FromKind(CrontabFieldKind kind)
    {
        if (!Enum.IsDefined(typeof(CrontabFieldKind), kind))
        {
            throw new ArgumentException(string.Format(
                "Invalid crontab field kind. Valid values are {0}.",
                string.Join(", ", Enum.GetNames(typeof(CrontabFieldKind)))), nameof(kind));
        }

        return FieldByKind[(int)kind];
    }

    public void Format(CrontabField field, TextWriter writer, bool noNames)
    {
        if (field == null)
            throw new ArgumentNullException(nameof(field));

        if (writer == null)
            throw new ArgumentNullException(nameof(writer));

        var next = field.GetFirst();
        var count = 0;

        while (next != -1)
        {
            var first = next;
            int last;

            do
            {
                last = next;
                next = field.Next(last + 1);
            } while (next - last == 1);

            if (count == 0
                && first == MinValue && last == MaxValue)
            {
                writer.Write('*');
                return;
            }

            if (count > 0)
                writer.Write(',');

            if (first == last)
            {
                FormatValue(first, writer, noNames);
            }
            else
            {
                FormatValue(first, writer, noNames);
                writer.Write('-');
                FormatValue(last, writer, noNames);
            }

            count++;
        }
    }

    private void FormatValue(int value, TextWriter writer, bool noNames)
    {
        if (noNames || Names == null)
        {
            if (value >= 0 && value < 100)
            {
                FastFormatNumericValue(value, writer);
            }
            else
            {
                writer.Write(value.ToString(CultureInfo.InvariantCulture));
            }
        }
        else
        {
            var index = value - MinValue;
            writer.Write((string)Names[index]);
        }
    }

    private static void FastFormatNumericValue(int value, TextWriter writer)
    {
        if (value >= 10)
        {
            writer.Write((char)('0' + (value / 10)));
            writer.Write((char)('0' + (value % 10)));
        }
        else
        {
            writer.Write((char)('0' + value));
        }
    }

    public void Parse(string str, Action<int, int, int> accumulator)
    {
        if (accumulator == null)
            throw new ArgumentNullException(nameof(accumulator));

        if (string.IsNullOrEmpty(str))
            return;

        try
        {
            InternalParse(str, accumulator);
        }
        catch (FormatException e)
        {
            ThrowParseException(e, str);
        }
    }

    private static void ThrowParseException(Exception innerException, string str)
    {
        throw new FormatException(string.Format("'{0}' is not a valid crontab field expression.", str),
            innerException);
    }

    private void InternalParse(string str, Action<int, int, int> accumulator)
    {
        if (str.Length == 0)
            throw new FormatException("A crontab field value cannot be empty.");

        //
        // Next, look for a list of values (e.g. 1,2,3).
        //

        var commaIndex = str.IndexOf(",", StringComparison.Ordinal);

        if (commaIndex > 0)
        {
            foreach (var token in str.Split(Comma))
                InternalParse(token, accumulator);
        }
        else
        {
            var every = 1;

            //
            // Look for stepping first (e.g. */2 = every 2nd).
            // 

            var slashIndex = str.IndexOf("/", StringComparison.Ordinal);

            if (slashIndex > 0)
            {
                every = int.Parse(str[(slashIndex + 1)..], CultureInfo.InvariantCulture);
                str = str.Substring(0, slashIndex);
            }

            //
            // Next, look for wildcard (*).
            //

            if (str.Length == 1 && str[0] == '*')
            {
                accumulator(-1, -1, every);
                return;
            }

            //
            // Next, look for a range of values (e.g. 2-10).
            //

            var dashIndex = str.IndexOf("-", StringComparison.Ordinal);

            if (dashIndex > 0)
            {
                var first = ParseValue(str.Substring(0, dashIndex));
                var last = ParseValue(str[(dashIndex + 1)..]);

                accumulator(first, last, every);
                return;
            }

            //
            // Finally, handle the case where there is only one number.
            //

            var value = ParseValue(str);

            if (every == 1)
            {
                accumulator(value, value, 1);
            }
            else
            {
                accumulator(value, MaxValue, every);
            }
        }
    }

    private int ParseValue(string str)
    {
        if (str.Length == 0)
            throw new FormatException("A crontab field value cannot be empty.");

        var firstChar = str[0];

        if (firstChar >= '0' && firstChar <= '9')
            return int.Parse(str, CultureInfo.InvariantCulture);

        if (Names == null)
        {
            throw new FormatException(string.Format(
                "'{0}' is not a valid value for this crontab field. It must be a numeric value between {1} and {2} (all inclusive).",
                str, MinValue, MaxValue));
        }

        for (var i = 0; i < Names.Length; i++)
        {
            if (Comparer.IsPrefix(Names[i], str, CompareOptions.IgnoreCase))
                return i + MinValue;
        }

        throw new FormatException(string.Format(
            "'{0}' is not a known value name. Use one of the following: {1}.",
            str, string.Join(", ", Names)));
    }
}