// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

public partial class Crontab
{
    /// <summary>
    /// 解析 Cron 表达式
    /// </summary>
    /// <param name="expression">Cron 表达式</param>
    /// <param name="format">Cron 表达式格式化</param>
    /// <returns></returns>
    /// <exception cref="TimeCrontabException"></exception>
    private static Dictionary<CrontabFieldKind, List<ICronParser>> ParseToDictionary(string expression, CronStringFormat format)
    {
        // Cron 表达式非空非空白检查
        if (string.IsNullOrWhiteSpace(expression))
        {
            throw new TimeCrontabException("The provided cron string is null, empty or contains only whitespace.");
        }

        // Cron 表达式格式化字段个数检查
        var instructions = expression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var expectedCount = Constants.ExpectedFieldCounts[format];

        if (instructions.Length > expectedCount)
        {
            throw new TimeCrontabException(string.Format("The provided cron string <{0}> has too many parameters.", expression));
        }
        if (instructions.Length < expectedCount)
        {
            throw new TimeCrontabException(string.Format("The provided cron string <{0}> has too few parameters.", expression));
        }

        var defaultFieldOffset = 0;
        var fieldParsers = new Dictionary<CrontabFieldKind, List<ICronParser>>();

        // 处理带秒字段解析器
        if (format == CronStringFormat.WithSeconds || format == CronStringFormat.WithSecondsAndYears)
        {
            fieldParsers.Add(CrontabFieldKind.Second, ParseField(instructions[0], CrontabFieldKind.Second));
            defaultFieldOffset = 1;
        }

        // 必须字段解析器
        fieldParsers.Add(CrontabFieldKind.Minute, ParseField(instructions[defaultFieldOffset + 0], CrontabFieldKind.Minute));
        fieldParsers.Add(CrontabFieldKind.Hour, ParseField(instructions[defaultFieldOffset + 1], CrontabFieldKind.Hour));
        fieldParsers.Add(CrontabFieldKind.Day, ParseField(instructions[defaultFieldOffset + 2], CrontabFieldKind.Day));
        fieldParsers.Add(CrontabFieldKind.Month, ParseField(instructions[defaultFieldOffset + 3], CrontabFieldKind.Month));
        fieldParsers.Add(CrontabFieldKind.DayOfWeek, ParseField(instructions[defaultFieldOffset + 4], CrontabFieldKind.DayOfWeek));

        // 处理带年字段解析器
        if (format == CronStringFormat.WithYears || format == CronStringFormat.WithSecondsAndYears)
        {
            fieldParsers.Add(CrontabFieldKind.Year, ParseField(instructions[defaultFieldOffset + 5], CrontabFieldKind.Year));
        }

        CheckForIllegalParsers(fieldParsers);

        return fieldParsers;
    }

    private static List<ICronParser> ParseField(string field, CrontabFieldKind kind)
    {
        try
        {
            return field.Split(',').Select(parser => ParseParser(parser, kind)).ToList();
        }
        catch (Exception ex)
        {
            throw new TimeCrontabException(string.Format("There was an error parsing '{0}' for the {1} field", field, Enum.GetName(typeof(CrontabFieldKind), kind)), ex);
        }
    }

    private static ICronParser ParseParser(string parser, CrontabFieldKind kind)
    {
        var newParser = parser.ToUpper();

        try
        {
            if (newParser.StartsWith("*", StringComparison.OrdinalIgnoreCase))
            {
                newParser = newParser[1..];
                if (newParser.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    newParser = newParser[1..];
                    var steps = GetValue(ref newParser, kind);
                    return new StepParser(0, steps, kind);
                }

                return new AnyParser(kind);
            }

            // * * LW * *
            // * * L * *
            if (newParser.StartsWith("L") && kind == CrontabFieldKind.Day)
            {
                newParser = newParser[1..];
                if (newParser == "W")
                {
                    return new LastWeekdayOfMonthParser(kind);
                }
                else
                {
                    return new LastDayOfMonthParser(kind);
                }
            }

            if (newParser == "?")
            {
                return new BlankDayOfMonthOrWeekParser(kind);
            }

            var firstValue = GetValue(ref newParser, kind);

            if (string.IsNullOrEmpty(newParser))
            {
                if (kind == CrontabFieldKind.Year)
                {
                    return new SpecificYearParser(firstValue, kind);
                }
                else
                {
                    return new SpecificParser(firstValue, kind);
                }
            }

            switch (newParser[0])
            {
                case '/':
                    {
                        newParser = newParser[1..];
                        var secondValue = GetValue(ref newParser, kind);
                        return new StepParser(firstValue, secondValue, kind);
                    }
                case '-':
                    {
                        newParser = newParser[1..];
                        var secondValue = GetValue(ref newParser, kind);
                        int? steps = null;
                        if (newParser.StartsWith("/"))
                        {
                            newParser = newParser[1..];
                            steps = GetValue(ref newParser, kind);
                        }

                        return new RangeParser(firstValue, secondValue, steps, kind);
                    }
                case '#':
                    {
                        newParser = newParser[1..];
                        var secondValue = GetValue(ref newParser, kind);

                        if (!string.IsNullOrEmpty(newParser))
                        {
                            throw new TimeCrontabException(string.Format("Invalid parser '{0}.'", parser));
                        }

                        return new SpecificDayOfWeekInMonthParser(firstValue, secondValue, kind);
                    }
                default:
                    if (newParser == "L" && kind == CrontabFieldKind.DayOfWeek)
                    {
                        return new LastDayOfWeekInMonthParser(firstValue, kind);
                    }
                    else if (newParser == "W" && kind == CrontabFieldKind.Day)
                    {
                        return new NearestWeekdayParser(firstValue, kind);
                    }
                    break;
            }

            throw new TimeCrontabException(string.Format("Invalid parser '{0}'", parser));
        }
        catch (Exception e)
        {
            throw new TimeCrontabException(string.Format("Invalid parser '{0}'.  See inner exception for details.", parser), e);
        }
    }

    private static int GetValue(ref string parser, CrontabFieldKind kind)
    {
        var maximum = Constants.MaximumDateTimeValues[kind];

        if (string.IsNullOrEmpty(parser))
        {
            throw new TimeCrontabException("Expected number, but parser was empty.");
        }

        int i;
        var isDigit = char.IsDigit(parser[0]);
        var isLetter = char.IsLetter(parser[0]);

        // Because this could either numbers, or letters, but not a combination,
        // check each condition separately.
        for (i = 0; i < parser.Length; i++)
        {
            if ((isDigit && !char.IsDigit(parser[i])) || (isLetter && !char.IsLetter(parser[i])))
            {
                break;
            }
        }

        var valueToParse = parser[..i];
        if (int.TryParse(valueToParse, out var value))
        {
            parser = parser[i..];
            var returnValue = value;
            if (returnValue > maximum)
            {
                throw new TimeCrontabException(string.Format("Value for {0} parser exceeded maximum value of {1}.", Enum.GetName(typeof(CrontabFieldKind), kind), maximum));
            }
            return returnValue;
        }
        // 处理星期和月命名字段
        else
        {
            List<KeyValuePair<string, int>>? replaceVal = null;

            if (kind == CrontabFieldKind.DayOfWeek)
            {
                replaceVal = Constants.Days.Where(x => valueToParse.StartsWith(x.Key)).ToList();
            }
            else if (kind == CrontabFieldKind.Month)
            {
                replaceVal = Constants.Months.Where(x => valueToParse.StartsWith(x.Key)).ToList();
            }

            if (replaceVal != null && replaceVal.Count == 1)
            {
                // missingParser addresses when a parser string of "SUNL" is passed in,
                // which causes the isDigit/isLetter loop above to iterate through the end
                // of the string.  This catches the edge case, and re-appends L to the end.
                var missingParser = "";
                if (parser.Length == i && parser.EndsWith("L") && kind == CrontabFieldKind.DayOfWeek)
                {
                    missingParser = "L";
                }

                parser = parser[i..] + missingParser;
                var returnValue = replaceVal.First().Value;
                if (returnValue > maximum)
                {
                    throw new TimeCrontabException(string.Format("Value for {0} parser exceeded maximum value of {1}.", Enum.GetName(typeof(CrontabFieldKind), kind), maximum));
                }

                return returnValue;
            }
        }

        throw new TimeCrontabException("Parser does not contain expected number.");
    }

    /// <summary>
    /// 检查非法解析器
    /// </summary>
    /// <param name="parsers"></param>
    /// <exception cref="TimeCrontabException"></exception>
    private static void CheckForIllegalParsers(Dictionary<CrontabFieldKind, List<ICronParser>> parsers)
    {
        var monthSingle = GetSpecificParsers(parsers, CrontabFieldKind.Month);
        var daySingle = GetSpecificParsers(parsers, CrontabFieldKind.Day);

        if (monthSingle.Any() && monthSingle.All(x => x.SpecificValue == 2))
        {
            if (daySingle.Any() && daySingle.All(x => (x.SpecificValue == 30) || (x.SpecificValue == 31)))
            {
                throw new TimeCrontabException("Nice try, but February 30 and 31 don't exist.");
            }
        }
    }

    private static List<SpecificParser> GetSpecificParsers(Dictionary<CrontabFieldKind, List<ICronParser>> parsers, CrontabFieldKind kind)
    {
        return parsers[kind].Where(x => x.GetType() == typeof(SpecificParser)).Cast<SpecificParser>()
            .Union(
            parsers[kind].Where(x => x.GetType() == typeof(RangeParser)).SelectMany(x => ((RangeParser)x).SpecificParsers)
            ).Union(
                parsers[kind].Where(x => x.GetType() == typeof(StepParser)).SelectMany(x => ((StepParser)x).SpecificParsers)
            ).ToList();
    }

    /// <summary>
    /// 内部计算
    /// </summary>
    /// <param name="baseValue"></param>
    /// <param name="endValue"></param>
    /// <returns></returns>
    private DateTime InternalGetNextOccurence(DateTime baseValue, DateTime endValue)
    {
        var newValue = baseValue;
        var overflow = true;

        var isSecondFormat = Format == CronStringFormat.WithSeconds || Format == CronStringFormat.WithSecondsAndYears;
        var isYearFormat = Format == CronStringFormat.WithYears || Format == CronStringFormat.WithSecondsAndYears;

        // First things first - trim off any time components we don't need
        newValue = newValue.AddMilliseconds(-newValue.Millisecond);
        if (!isSecondFormat)
        {
            newValue = newValue.AddSeconds(-newValue.Second);
        }

        var minuteParsers = Parsers[CrontabFieldKind.Minute].Where(x => x is ITimeParser).Cast<ITimeParser>().ToList();
        var hourParsers = Parsers[CrontabFieldKind.Hour].Where(x => x is ITimeParser).Cast<ITimeParser>().ToList();

        var firstSecondValue = newValue.Second;
        var firstMinuteValue = minuteParsers.Select(x => x.First()).Min();
        var firstHourValue = hourParsers.Select(x => x.First()).Min();

        var newSeconds = newValue.Second;
        if (isSecondFormat)
        {
            var secondParsers = Parsers[CrontabFieldKind.Second].Where(x => x is ITimeParser).Cast<ITimeParser>().ToList();
            firstSecondValue = secondParsers.Select(x => x.First()).Min();
            newSeconds = Increment(secondParsers, newValue.Second, firstSecondValue, out overflow);
            newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newValue.Hour, newValue.Minute, newSeconds);

            if (!overflow && !IsMatch(newValue))
            {
                newSeconds = firstSecondValue;
                newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newValue.Hour, newValue.Minute, newSeconds);
                overflow = true;
            }

            if (!overflow)
            {
                return MinDate(newValue, endValue);
            }
        }

        var newMinutes = Increment(minuteParsers, newValue.Minute + (overflow ? 0 : -1), firstMinuteValue, out overflow);
        newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newValue.Hour, newMinutes, overflow ? firstSecondValue : newSeconds);

        if (!overflow && !IsMatch(newValue))
        {
            newSeconds = firstSecondValue;
            newMinutes = firstMinuteValue;
            newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newValue.Hour, newMinutes, firstSecondValue);
            overflow = true;
        }

        if (!overflow)
        {
            return MinDate(newValue, endValue);
        }

        var newHours = Increment(hourParsers, newValue.Hour + (overflow ? 0 : -1), firstHourValue, out overflow);
        newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newHours,
            overflow ? firstMinuteValue : newMinutes,
            overflow ? firstSecondValue : newSeconds);

        if (!overflow && !IsMatch(newValue))
        {
            newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, firstHourValue, firstMinuteValue, firstSecondValue);
            overflow = true;
        }

        if (!overflow)
        {
            return MinDate(newValue, endValue);
        }

        List<ITimeParser>? yearParsers = null;
        if (isYearFormat)
        {
            yearParsers = Parsers[CrontabFieldKind.Year].Where(x => x is ITimeParser).Cast<ITimeParser>().ToList();
        }

        // Sooo, this is where things get more complicated.
        // Since the parsering of days relies on what month/year you're in
        // (for weekday/nth day parsers), we'll only increment the day, and
        // check all day/month/year parsers.  Might be a litle slow, but we
        // won't miss any days that way.

        // Also, if we increment to the next day, we need to set the hour, minute and second
        // fields to their "first" values, since that would be the earliest they'd run.  We
        // only have to do this after the initial AddDays call.  FYI - they're already at their
        // first values if overflowHour = True.  :-)

        // This feels so dirty.  This is to catch the odd case where you specify
        // 12/31/9999 23:59:59.999 as your end date, and you don't have any matches,
        // so it reaches the max value of DateTime and throws an exception.
        try
        {
            newValue = newValue.AddDays(1);
        }
        catch
        {
            return endValue;
        }

        while (!(IsMatch(newValue, CrontabFieldKind.Day)
            && IsMatch(newValue, CrontabFieldKind.DayOfWeek)
            && IsMatch(newValue, CrontabFieldKind.Month)
            && (!isYearFormat || IsMatch(newValue, CrontabFieldKind.Year))))
        {
            if (newValue >= endValue)
            {
                return MinDate(newValue, endValue);
            }

            // In instances where the year is parsered, this will speed up the path to get to endValue
            // (without having to actually go to endValue)
            if (isYearFormat && yearParsers!.Select(x => x.Next(newValue.Year - 1)).All(x => x == null))
            {
                return endValue;
            }

            // Ugh...have to do the try/catch again...
            try
            {
                newValue = newValue.AddDays(1);
            }
            catch
            {
                return endValue;
            }
        }

        return MinDate(newValue, endValue);
    }

    /// <summary>
    /// 获取下一个值
    /// </summary>
    /// <param name="parsers"></param>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <param name="overflow"></param>
    /// <returns></returns>
    private static int Increment(IEnumerable<ITimeParser> parsers, int value, int defaultValue, out bool overflow)
    {
        var nextValue = parsers.Select(x => x.Next(value)).Where(x => x > value).Min() ?? defaultValue;
        overflow = nextValue <= value;
        return nextValue;
    }

    /// <summary>
    /// 最小日期
    /// </summary>
    /// <param name="newValue"></param>
    /// <param name="endValue"></param>
    /// <returns></returns>
    private static DateTime MinDate(DateTime newValue, DateTime endValue)
    {
        return newValue >= endValue ? endValue : newValue;
    }

    /// <summary>
    /// 是否匹配
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private bool IsMatch(DateTime value)
    {
        return Parsers.All(fieldKind =>
            fieldKind.Value.Any(parser => parser.IsMatch(value))
        );
    }

    /// <summary>
    /// 是否匹配
    /// </summary>
    /// <param name="value"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    private bool IsMatch(DateTime value, CrontabFieldKind kind)
    {
        return Parsers.Where(x => x.Key == kind).SelectMany(x => x.Value).Any(parser => parser.IsMatch(value));
    }

    private void JoinParsers(List<string> paramList, CrontabFieldKind kind)
    {
        paramList.Add(
            string.Join(",", Parsers
                .Where(x => x.Key == kind)
                .SelectMany(x => x.Value.Select(y => y.ToString())).ToArray()
            )
        );
    }
}