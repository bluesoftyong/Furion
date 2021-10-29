// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

public partial class CrontabSchedule
{
    /// <summary>
    /// 转换
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static CrontabSchedule Parse(string expression, CronStringFormat format = CronStringFormat.Default)
    {
        return new CrontabSchedule
        {
            Format = format,
            Filters = ParseToDictionary(expression, format)
        };
    }

    /// <summary>
    /// 转换
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static CrontabSchedule? TryParse(string expression, CronStringFormat format = CronStringFormat.Default)
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

    private static void CheckForIllegalFilters(Dictionary<CrontabFieldKind, List<ICronFilter>> filters)
    {
        var monthSingle = GetSpecificFilters(filters, CrontabFieldKind.Month);
        var daySingle = GetSpecificFilters(filters, CrontabFieldKind.Day);

        if (monthSingle.Any() && monthSingle.All(x => x.SpecificValue == 2))
        {
            if (daySingle.Any() && daySingle.All(x => (x.SpecificValue == 30) || (x.SpecificValue == 31)))
                throw new TimeCrontabException("Nice try, but February 30 and 31 don't exist.");
        }
    }

    private static List<SpecificFilter> GetSpecificFilters(Dictionary<CrontabFieldKind, List<ICronFilter>> filters, CrontabFieldKind kind)
    {
        return filters[kind].Where(x => x.GetType() == typeof(SpecificFilter)).Cast<SpecificFilter>().Union(
            filters[kind].Where(x => x.GetType() == typeof(RangeFilter)).SelectMany(x => ((RangeFilter)x).SpecificFilters)
            ).Union(
                filters[kind].Where(x => x.GetType() == typeof(StepFilter)).SelectMany(x => ((StepFilter)x).SpecificFilters)
            ).ToList();
    }

    private static Dictionary<CrontabFieldKind, List<ICronFilter>> ParseToDictionary(string cron, CronStringFormat format)
    {
        if (string.IsNullOrWhiteSpace(cron))
            throw new TimeCrontabException("The provided cron string is null, empty or contains only whitespace");

        var fields = new Dictionary<CrontabFieldKind, List<ICronFilter>>();

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

        CheckForIllegalFilters(fields);

        return fields;
    }

    private static List<ICronFilter> ParseField(string field, CrontabFieldKind kind)
    {
        try
        {
            return field.Split(',').Select(filter => ParseFilter(filter, kind)).ToList();
        }
        catch (Exception e)
        {
            throw new TimeCrontabException(string.Format("There was an error parsing '{0}' for the {1} field", field, Enum.GetName(typeof(CrontabFieldKind), kind)), e);
        }
    }

    private static ICronFilter ParseFilter(string filter, CrontabFieldKind kind)
    {
        var newFilter = filter.ToUpper();

        try
        {
            if (newFilter.StartsWith("*", StringComparison.OrdinalIgnoreCase))
            {
                newFilter = newFilter[1..];
                if (newFilter.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    newFilter = newFilter[1..];
                    var steps = GetValue(ref newFilter, kind);
                    return new StepFilter(0, steps, kind);
                }
                return new AnyFilter(kind);
            }

            // * * LW * *
            // * * L * *
            if (newFilter.StartsWith("L") && kind == CrontabFieldKind.Day)
            {
                newFilter = newFilter[1..];
                if (newFilter == "W")
                    return new LastWeekdayOfMonthFilter(kind);
                else
                    return new LastDayOfMonthFilter(kind);
            }

            if (newFilter == "?")
                return new BlankDayOfMonthOrWeekFilter(kind);

            var firstValue = GetValue(ref newFilter, kind);

            if (string.IsNullOrEmpty(newFilter))
            {
                if (kind == CrontabFieldKind.Year)
                    return new SpecificYearFilter(firstValue, kind);
                else
                    return new SpecificFilter(firstValue, kind);
            }

            switch (newFilter[0])
            {
                case '/':
                    {
                        newFilter = newFilter[1..];
                        var secondValue = GetValue(ref newFilter, kind);
                        return new StepFilter(firstValue, secondValue, kind);
                    }
                case '-':
                    {
                        newFilter = newFilter[1..];
                        var secondValue = GetValue(ref newFilter, kind);
                        int? steps = null;
                        if (newFilter.StartsWith("/"))
                        {
                            newFilter = newFilter[1..];
                            steps = GetValue(ref newFilter, kind);
                        }
                        return new RangeFilter(firstValue, secondValue, steps, kind);
                    }
                case '#':
                    {
                        newFilter = newFilter[1..];
                        var secondValue = GetValue(ref newFilter, kind);

                        if (!string.IsNullOrEmpty(newFilter))
                            throw new TimeCrontabException(string.Format("Invalid filter '{0}'", filter));

                        return new SpecificDayOfWeekInMonthFilter(firstValue, secondValue, kind);
                    }
                default:
                    if (newFilter == "L" && kind == CrontabFieldKind.DayOfWeek)
                    {
                        return new LastDayOfWeekInMonthFilter(firstValue, kind);
                    }
                    else if (newFilter == "W" && kind == CrontabFieldKind.Day)
                    {
                        return new NearestWeekdayFilter(firstValue, kind);
                    }
                    break;
            }

            throw new TimeCrontabException(string.Format("Invalid filter '{0}'", filter));
        }
        catch (Exception e)
        {
            throw new TimeCrontabException(string.Format("Invalid filter '{0}'.  See inner exception for details.", filter), e);
        }
    }

    private static int GetValue(ref string filter, CrontabFieldKind kind)
    {
        var maxValue = Constants.MaximumDateTimeValues[kind];

        if (string.IsNullOrEmpty(filter))
            throw new TimeCrontabException("Expected number, but filter was empty.");

        int i;
        var isDigit = char.IsDigit(filter[0]);
        var isLetter = char.IsLetter(filter[0]);

        // Because this could either numbers, or letters, but not a combination,
        // check each condition separately.
        for (i = 0; i < filter.Length; i++)
            if ((isDigit && !char.IsDigit(filter[i])) || (isLetter && !char.IsLetter(filter[i]))) break;

        var valueToParse = filter[..i];
        if (int.TryParse(valueToParse, out var value))
        {
            filter = filter[i..];
            var returnValue = value;
            if (returnValue > maxValue)
                throw new TimeCrontabException(string.Format("Value for {0} filter exceeded maximum value of {1}", Enum.GetName(typeof(CrontabFieldKind), kind), maxValue));
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
                // missingFilter addresses when a filter string of "SUNL" is passed in,
                // which causes the isDigit/isLetter loop above to iterate through the end
                // of the string.  This catches the edge case, and re-appends L to the end.
                var missingFilter = "";
                if (filter.Length == i && filter.EndsWith("L") && kind == CrontabFieldKind.DayOfWeek)
                    missingFilter = "L";

                filter = filter[i..] + missingFilter;
                var returnValue = replaceVal.First().Value;
                if (returnValue > maxValue)
                    throw new TimeCrontabException(string.Format("Value for {0} filter exceeded maximum value of {1}", Enum.GetName(typeof(CrontabFieldKind), kind), maxValue));
                return returnValue;
            }
        }

        throw new TimeCrontabException("Filter does not contain expected number");
    }
}