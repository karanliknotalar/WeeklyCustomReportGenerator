using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WeeklyCustomReportGenerator;

public static class Tools
{
    public static List<string> SearchFiles(string rootPath, Regex regex)
    {
        var searchFiles = new List<string>();
        var dirs = new Stack<string>(20);

        if (!Directory.Exists(rootPath))
        {
            MessageBox.Show($@"HATA: Belirtilen dizin bulunamadı -> {rootPath}");
            return [];
        }

        dirs.Push(rootPath);

        while (dirs.Count > 0)
        {
            var currentDir = dirs.Pop();

            try
            {
                var subDirs = Directory.GetDirectories(currentDir);
                foreach (var str in subDirs)
                {
                    dirs.Push(str);
                }

                var files = Directory.GetFiles(currentDir);

                searchFiles.AddRange(from file in files
                    let fileName = Path.GetFileName(file)
                    where regex.IsMatch(fileName)
                    select file);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show($@"Erişim Reddedildi (Atlanıyor): {currentDir}");
            }
            catch (PathTooLongException)
            {
                MessageBox.Show($@"Dosya yolu çok uzunsa atla ({currentDir})");
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"Hata ({currentDir}): {ex.Message}");
            }
        }

        return searchFiles;
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