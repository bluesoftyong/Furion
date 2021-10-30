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
    /// 转换
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static Crontab Parse(string expression, CronStringFormat format = CronStringFormat.Default)
    {
        return new Crontab
        {
            Format = format,
            Parsers = ParseToDictionary(expression, format)
        };
    }

    /// <summary>
    /// 转换
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static Crontab? TryParse(string expression, CronStringFormat format = CronStringFormat.Default)
    {
        try
        {
            return Parse(expression, format);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static void CheckForIllegalParsers(Dictionary<CrontabFieldKind, List<ICronParser>> parsers)
    {
        var monthSingle = GetSpecificParsers(parsers, CrontabFieldKind.Month);
        var daySingle = GetSpecificParsers(parsers, CrontabFieldKind.Day);

        if (monthSingle.Any() && monthSingle.All(x => x.SpecificValue == 2))
        {
            if (daySingle.Any() && daySingle.All(x => (x.SpecificValue == 30) || (x.SpecificValue == 31)))
                throw new TimeCrontabException("Nice try, but February 30 and 31 don't exist.");
        }
    }

    private static List<SpecificParser> GetSpecificParsers(Dictionary<CrontabFieldKind, List<ICronParser>> parsers, CrontabFieldKind kind)
    {
        return parsers[kind].Where(x => x.GetType() == typeof(SpecificParser)).Cast<SpecificParser>().Union(
            parsers[kind].Where(x => x.GetType() == typeof(RangeParser)).SelectMany(x => ((RangeParser)x).SpecificParsers)
            ).Union(
                parsers[kind].Where(x => x.GetType() == typeof(StepParser)).SelectMany(x => ((StepParser)x).SpecificParsers)
            ).ToList();
    }

    private static Dictionary<CrontabFieldKind, List<ICronParser>> ParseToDictionary(string cron, CronStringFormat format)
    {
        if (string.IsNullOrWhiteSpace(cron))
            throw new TimeCrontabException("The provided cron string is null, empty or contains only whitespace");

        var fields = new Dictionary<CrontabFieldKind, List<ICronParser>>();

        var instructions = cron.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        var expectedCount = Constants.ExpectedFieldCounts[format];
        if (instructions.Length > expectedCount)
            throw new TimeCrontabException(string.Format("The provided cron string <{0}> has too many parameters", cron));
        if (instructions.Length < expectedCount)
            throw new TimeCrontabException(string.Format("The provided cron string <{0}> has too few parameters", cron));

        var defaultFieldOffset = 0;
        if (format == CronStringFormat.WithSeconds || format == CronStringFormat.WithSecondsAndYears)
        {
            fields.Add(CrontabFieldKind.Second, ParseField(instructions[0], CrontabFieldKind.Second));
            defaultFieldOffset = 1;
        }

        fields.Add(CrontabFieldKind.Minute, ParseField(instructions[defaultFieldOffset + 0], CrontabFieldKind.Minute));
        fields.Add(CrontabFieldKind.Hour, ParseField(instructions[defaultFieldOffset + 1], CrontabFieldKind.Hour));
        fields.Add(CrontabFieldKind.Day, ParseField(instructions[defaultFieldOffset + 2], CrontabFieldKind.Day));
        fields.Add(CrontabFieldKind.Month, ParseField(instructions[defaultFieldOffset + 3], CrontabFieldKind.Month));
        fields.Add(CrontabFieldKind.DayOfWeek, ParseField(instructions[defaultFieldOffset + 4], CrontabFieldKind.DayOfWeek));

        if (format == CronStringFormat.WithYears || format == CronStringFormat.WithSecondsAndYears)
            fields.Add(CrontabFieldKind.Year, ParseField(instructions[defaultFieldOffset + 5], CrontabFieldKind.Year));

        CheckForIllegalParsers(fields);

        return fields;
    }

    private static List<ICronParser> ParseField(string field, CrontabFieldKind kind)
    {
        try
        {
            return field.Split(',').Select(parser => ParseParser(parser, kind)).ToList();
        }
        catch (Exception e)
        {
            throw new TimeCrontabException(string.Format("There was an error parsing '{0}' for the {1} field", field, Enum.GetName(typeof(CrontabFieldKind), kind)), e);
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
                    return new LastWeekdayOfMonthParser(kind);
                else
                    return new LastDayOfMonthParser(kind);
            }

            if (newParser == "?")
                return new BlankDayOfMonthOrWeekParser(kind);

            var firstValue = GetValue(ref newParser, kind);

            if (string.IsNullOrEmpty(newParser))
            {
                if (kind == CrontabFieldKind.Year)
                    return new SpecificYearParser(firstValue, kind);
                else
                    return new SpecificParser(firstValue, kind);
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
                            throw new TimeCrontabException(string.Format("Invalid parser '{0}'", parser));

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
        var maxValue = Constants.MaximumDateTimeValues[kind];

        if (string.IsNullOrEmpty(parser))
            throw new TimeCrontabException("Expected number, but parser was empty.");

        int i;
        var isDigit = char.IsDigit(parser[0]);
        var isLetter = char.IsLetter(parser[0]);

        // Because this could either numbers, or letters, but not a combination,
        // check each condition separately.
        for (i = 0; i < parser.Length; i++)
            if ((isDigit && !char.IsDigit(parser[i])) || (isLetter && !char.IsLetter(parser[i]))) break;

        var valueToParse = parser[..i];
        if (int.TryParse(valueToParse, out var value))
        {
            parser = parser[i..];
            var returnValue = value;
            if (returnValue > maxValue)
                throw new TimeCrontabException(string.Format("Value for {0} parser exceeded maximum value of {1}", Enum.GetName(typeof(CrontabFieldKind), kind), maxValue));
            return returnValue;
        }
        else
        {
            List<KeyValuePair<string, int>>? replaceVal = null;

            if (kind == CrontabFieldKind.DayOfWeek)
                replaceVal = Constants.Days.Where(x => valueToParse.StartsWith(x.Key)).ToList();
            else if (kind == CrontabFieldKind.Month)
                replaceVal = Constants.Months.Where(x => valueToParse.StartsWith(x.Key)).ToList();

            if (replaceVal != null && replaceVal.Count == 1)
            {
                // missingParser addresses when a parser string of "SUNL" is passed in,
                // which causes the isDigit/isLetter loop above to iterate through the end
                // of the string.  This catches the edge case, and re-appends L to the end.
                var missingParser = "";
                if (parser.Length == i && parser.EndsWith("L") && kind == CrontabFieldKind.DayOfWeek)
                    missingParser = "L";

                parser = parser[i..] + missingParser;
                var returnValue = replaceVal.First().Value;
                if (returnValue > maxValue)
                    throw new TimeCrontabException(string.Format("Value for {0} parser exceeded maximum value of {1}", Enum.GetName(typeof(CrontabFieldKind), kind), maxValue));
                return returnValue;
            }
        }

        throw new TimeCrontabException("Parser does not contain expected number");
    }
}