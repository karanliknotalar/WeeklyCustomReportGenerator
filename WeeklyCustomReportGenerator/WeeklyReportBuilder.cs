#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace WeeklyCustomReportGenerator;

public class WeeklyReportBuilder()
{
    private List<string>? ProductKeywords { get; set; }
    private CultureInfo? TrCulture { get; set; }
    

    public WeeklyReportBuilder(string[] productOrder) : this()
    {
        ProductKeywords = productOrder.ToList();
        TrCulture = CultureInfo.GetCultureInfo("tr-TR");
    }

    public List<PolicyItem> ParseFiles(IEnumerable<string> lines)
    {
        var items = new List<PolicyItem>();

        foreach (var path in lines)
        {
            if (string.IsNullOrWhiteSpace(path)) continue;
            
            var fileName = Path.GetFileNameWithoutExtension(path);
            
            var isCancel = TrCulture!.CompareInfo.IndexOf(fileName, "iptal", CompareOptions.IgnoreCase) >= 0;
            
            var date = DateTime.MinValue;
            var parts = fileName.Split([" - ", " "], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                DateTime.TryParse(parts[0], out date);
            }

            var category = "DİĞER";

            if (!isCancel) 
            {
                foreach (var keyword in ProductKeywords!)
                {
                    if (TrCulture.CompareInfo.IndexOf(fileName, keyword, CompareOptions.IgnoreCase) < 0) continue;
                    category = keyword.ToUpper(TrCulture);
                    break;
                }
            }

            items.Add(new PolicyItem
            {
                Date = date,
                FullLine = fileName,
                Category = category,
                IsCancel = isCancel
            });
        }

        return items;
    }

    public string BuildReport(List<PolicyItem> items)
    {
        var activeItems = items.Where(x => !x.IsCancel).ToList();
        var cancelledItems = items.Where(x => x.IsCancel).ToList();

        var sb = new StringBuilder();
        sb.AppendLine($"ÜRETİMLER ({activeItems.Count}):");
        sb.AppendLine();
        
        var groupings = activeItems.GroupBy(x => x.Category).ToList();

        foreach (var keyword in ProductKeywords!)
        {
            var categoryKey = keyword.ToUpper(TrCulture!);

            var group = groupings.FirstOrDefault(g => g.Key == categoryKey);

            if (group != null)
            {
                PrintGroup(sb, categoryKey, group.ToList());
            }
        }

        var otherGroup = groupings.FirstOrDefault(g => g.Key == "DİĞER");
        if (otherGroup != null)
        {
            PrintGroup(sb, "DİĞER", otherGroup.ToList());
        }
        else
        {
            sb.AppendLine($"\tDİĞER (00):");
            sb.AppendLine();
        }

        // --- İPTALLER ---
        sb.AppendLine();
        sb.AppendLine($"İPTALLER ({cancelledItems.Count:D2}):");
        sb.AppendLine();

        if (cancelledItems.Any())
        {
            foreach (var item in cancelledItems.OrderBy(x => x.Date).ThenBy(x => x.FullLine))
            {
                sb.AppendLine($"\t{item.FullLine}");
            }
        }

        return sb.ToString();
    }

    private static void PrintGroup(StringBuilder sb, string categoryName, List<PolicyItem> list)
    {
        var sortedList = list.OrderBy(x => x.Date).ThenBy(x => x.FullLine).ToList();

        sb.AppendLine($"\t{categoryName} ({sortedList.Count:D2}):");

        foreach (var item in sortedList)
        {
            sb.AppendLine($"\t\t{item.FullLine}");
        }

        sb.AppendLine();
    }
    

    public static List<string> GenerateYearlyWeeklyRegexPatterns()
    {
        const string staticPrefix =
            @"(?i)^(?!.*\b(?:makbuz|acs|eng|hayat|yeşil)\b)(?:(?!.*\bzeyil|zeyili\b)|(?=.*\bİptal\b)).*";

        var today = DateTime.Now.Date;
        var yearStart = new DateTime(today.Year, 1, 1);

        var firstSundayOfYear = yearStart;
        while (firstSundayOfYear.DayOfWeek != DayOfWeek.Sunday)
        {
            firstSundayOfYear = firstSundayOfYear.AddDays(1);
        }

        var currentSunday = firstSundayOfYear;
        var weeklyRegexList = new List<string>();

        while (currentSunday <= today)
        {
            var weekStart = currentSunday;

            var weekEnd = weekStart.AddDays(6);
            if (weekEnd > today)
            {
                weekEnd = today;
            }

            var dateList = new List<string>();

            for (var date = weekStart; date <= weekEnd; date = date.AddDays(1))
            {
                dateList.Add(date.ToString("dd\\.MM\\.yyyy"));
            }

            if (dateList.Count > 0)
            {
                var datePart = "(" + string.Join("|", dateList) + ")";

                weeklyRegexList.Add(staticPrefix + datePart + ".*");
            }

            currentSunday = currentSunday.AddDays(7);
        }

        return weeklyRegexList;
    }

    public static List<string> GenerateYearlyWeeklyRegexPatternsShort()
    {
        const string staticPrefix =
            @"(?i)^(?!.*\b(?:makbuz|acs|eng|hayat|yeşil)\b)(?:(?!.*\bzeyil|zeyili\b)|(?=.*\bİptal\b)).*";

        var today = DateTime.Now.Date;
        var yearStart = new DateTime(today.Year, 1, 1);

        var firstSundayOfYear = yearStart;
        while (firstSundayOfYear.DayOfWeek != DayOfWeek.Sunday)
        {
            firstSundayOfYear = firstSundayOfYear.AddDays(1);
        }

        var currentSunday = firstSundayOfYear;
        var weeklyRegexList = new List<string>();

        while (currentSunday <= today)
        {
            var weekStart = currentSunday;
            var weekEnd = weekStart.AddDays(6);

            if (weekEnd > today)
            {
                weekEnd = today;
            }

            var dateRegexPart = GetCompactDateRegex(weekStart, weekEnd);

            if (!string.IsNullOrEmpty(dateRegexPart))
            {
                weeklyRegexList.Add(staticPrefix + dateRegexPart + ".*");
            }

            currentSunday = currentSunday.AddDays(7);
        }

        return weeklyRegexList;
    }

    private static string GetCompactDateRegex(DateTime start, DateTime end)
    {
        var parts = new List<string>();

        var current = start;
        while (current <= end)
        {
            var endOfMonth = new DateTime(current.Year, current.Month,
                DateTime.DaysInMonth(current.Year, current.Month));
            var segmentEnd = endOfMonth < end ? endOfMonth : end;

            var dayPart = GenerateDayRangePattern(current.Day, segmentEnd.Day);
            var monthPart = current.Month.ToString("00");
            var yearPart = current.Year.ToString();

            parts.Add($"{dayPart}\\.{monthPart}\\.{yearPart}");
            current = segmentEnd.AddDays(1);
        }

        if (parts.Count > 1)
        {
            return "(" + string.Join("|", parts) + ")";
        }

        return parts[0];
    }

    private static string GenerateDayRangePattern(int startDay, int endDay)
    {
        var ranges = new List<string>();

        for (var i = startDay; i <= endDay;)
        {
            var tens = i / 10;
            var units = i % 10;

            var endOfCurrentTen = (tens * 10) + 9;

            var effectiveEnd = Math.Min(endDay, endOfCurrentTen);
            var effectiveEndUnits = effectiveEnd % 10;

            ranges.Add(units == effectiveEndUnits ? i.ToString("00") : $"{tens}[{units}-{effectiveEndUnits}]");

            i = effectiveEnd + 1;
        }


        var result = string.Join("|", ranges);
        return ranges.Count > 1 ? $"({result})" : result;
    }
}