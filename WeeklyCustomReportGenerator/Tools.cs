#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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

        for (var j = 3; j < 7; j++)
        {
            weeklyRegexList.Add(
                @"(?i)^(?!.*\b(?:makbuz|acs|eng|hayat|yeşil)\b)(?:(?!.*\bzeyil|zeyili\b)|(?=.*\bİptal\b)).*(\d{2}\.\d{2}\."
                + $"202{j}).*");
        }

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

                weeklyRegexList.Add(staticPrefix + datePart + ".*\\.pdf$");
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

        for (var j = 3; j < 7; j++)
        {
            weeklyRegexList.Add(
                @"(?i)^(?!.*\b(?:makbuz|acs|eng|hayat|yeşil)\b)(?:(?!.*\bzeyil|zeyili\b)|(?=.*\bİptal\b)).*(\d{2}\.\d{2}\."
                + $"202{j}).*");
        }

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

    public static void CheckForPreviousYearLocalPolicy(List<PolicyItem> items)
    {
        var localDrivePath = Form1.DriveDirectory;
        var activeItems = items.Where(x => !x.IsCancel).ToList();

        if (!Directory.Exists(localDrivePath))
        {
            MessageBox.Show(@$"Hata: Belirtilen dizin bulunamadı: {localDrivePath}", @"Dizin Hatası",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var allFileNames = SearchFiles(localDrivePath, new Regex("."))
            .Select(Path.GetFileNameWithoutExtension)
            .Where(n => n != null)
            .ToList();

        foreach (var currentItem in activeItems)
        {
            if (string.IsNullOrWhiteSpace(currentItem.CustomerName))
            {
                continue;
            }

            var previousYear = currentItem.Date.Year - 1;
            var escapedName = Regex.Escape(currentItem.CustomerName.ToLower());
            var escapedPlate = Regex.Escape(currentItem.Plate.ToLower());
            var escapedCategory = Regex.Escape(currentItem.Category.ToLower());

            var pattern = string.IsNullOrEmpty(currentItem.Plate)
                ? $"(?i)(?:{previousYear}).*?{escapedName}.*?{escapedCategory}"
                : $"(?i)(?:{previousYear}).*?{escapedName}.*?(?:(?:{escapedPlate}.*?{escapedCategory}.*)|(?:{escapedCategory}.*?{escapedPlate}.*))$";

            var searchRegex = new Regex(pattern, RegexOptions.CultureInvariant);

            var isFound = allFileNames.Any(fileName => searchRegex.IsMatch(fileName.ToLower()));

            if (isFound)
            {
                currentItem.IsRenewal = true;
            }
        }
    }

    public static string CleanText(string rawText)
    {
        if (string.IsNullOrEmpty(rawText)) return string.Empty;

        var clean = Regex.Replace(rawText, @"[\u00A0\uFEFF\u200B\t]", " ");

        clean = Regex.Replace(clean, @"[ ]{2,}", " ");
        clean = clean.Replace("\t", "");

        var lines = clean.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        var trimmedLines = lines.Select(l => l.Trim()).Where(l => !string.IsNullOrWhiteSpace(l));

        return string.Join("\n", trimmedLines);
    }

    public static void WriteToLogFile(string pdfContent, string pdfPath, Company company)
    {
        try
        {
            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var logFilePath = Path.Combine(appDirectory, "pdf_processing_log.txt");

            var logContent = $"İşlem Zamanı: {DateTime.Now:dd.MM.yyyy HH:mm:ss}\n";
            logContent += $"PDF Dosya Yolu: {pdfPath}\n";
            logContent += $"PDF İçeriği:\n{pdfContent}\n";
            logContent += new string('@', 50) + "\n\n";

            File.WriteAllText(logFilePath, logContent);

            Console.WriteLine($@"Log dosyası oluşturuldu: {pdfPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"Log dosyası yazılırken hata oluştu: {ex.Message}");
        }
    }

    public static void AppendToLogFile(string pdfContent, string pdfPath, Company company)
    {
        try
        {
            var logFilePath = Path.Combine("C:\\", "pdf_processing_log.txt");

            var logContent = $"İşlem Zamanı: {DateTime.Now:dd.MM.yyyy HH:mm:ss}\n";
            logContent += $"PDF Dosya Yolu: {pdfPath}\n";
            logContent += $"Bulunan Firma:\n{company.CompanyName}\n";
            logContent += $"Kullanıclan Arama Metni:\n{company.CompanySearchText}\n";
            logContent += $"Kullanıclan pattern:\n{company.TotalPriceRegexPattern}\n";
            logContent += $"PDF İçeriği:\n{pdfContent}\n";
            logContent += new string('-', 50) + "\n\n";

            File.AppendAllText(logFilePath, logContent);

            Console.WriteLine(@$"Log dosyasına eklendi: {pdfPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(@$"Log dosyasına yazılırken hata oluştu: {ex.Message}");
        }
    }

    public static void AppendToLogFileForUndefined(List<PolicyItem> policyItems)
    {
        var logPdfUndefinedFile = Path.Combine("C:\\", "pdf_undefined_log.txt");
        var logContent = "";
        try
        {
            if (!policyItems.Any()) return;
            foreach (var policyItem in policyItems)
            {
                logContent += $"Kategori: {policyItem.Category}\n";
                logContent += $"Firma: {policyItem.Company}\n";
                logContent += $"Dosya Adı: {policyItem.FullLine}\n";
                logContent += new string('-', 50) + "\n\n";
            }

            File.AppendAllText(logPdfUndefinedFile, logContent);
            Console.WriteLine(@$"Tanımsız pdf'ler {logPdfUndefinedFile} konumuna kaydedildi.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(@$"{logPdfUndefinedFile} dosyası kaydedilirken hata oluştu: {ex.Message}");
        }
    }

    public static bool SearchCompanyText(string pdfContent, string companySearchText)
    {
        var searchParts = companySearchText.Split(['|'], StringSplitOptions.RemoveEmptyEntries);
        return searchParts.Any(part => pdfContent.IndexOf(part.Trim(), StringComparison.OrdinalIgnoreCase) >= 0);
    }

    private static decimal ParseTotalPrice(string totalPrice)
    {
        var cleanPrice = totalPrice.Replace(" TL", "").Replace(" ", "").Replace("-", "").Trim();

        if (string.IsNullOrEmpty(cleanPrice))
            return 0m;


        if (cleanPrice.Contains(',') && cleanPrice.LastIndexOf(',') > cleanPrice.LastIndexOf('.'))
        {
            cleanPrice = cleanPrice.Replace(".", "");
            cleanPrice = cleanPrice.Replace(",", ".");
        }
        else if (cleanPrice.Contains('.') && cleanPrice.LastIndexOf('.') > cleanPrice.LastIndexOf(','))
        {
            cleanPrice = cleanPrice.Replace(",", "");
        }
        else
        {
            cleanPrice = cleanPrice.Replace(",", "").Replace(".", "");
        }

        return decimal.TryParse(cleanPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out var price)
            ? Math.Abs(price)
            : 0m;
    }

    public static void GenerateCategoryAnalysis(List<PolicyItem> items, StringBuilder sb, bool isCancel = false)
    {
        var isActiveText = isCancel ? "İPTAL" : "ÜRETİM";
        var analysisData = items
            .GroupBy(p => p.Category)
            .Select(g =>
            {
                var pricedItems = g.Where(p => ParseTotalPrice(p.TotalPrice) > 0m).ToList();
                var totalPricedCount = pricedItems.Count;
                var totalPricedSum = pricedItems.Sum(p => ParseTotalPrice(p.TotalPrice));

                return new
                {
                    Category = g.Key,
                    TotalCount = g.Count(),
                    TotalNew = g.Count(p => p.IsRenewal == false),
                    TotalRenew = g.Count(p => p.IsRenewal),
                    TotalGallery = g.Count(p => p.IsGalleryCustomer),
                    TotalPrice = g.Sum(p => ParseTotalPrice(p.TotalPrice)),
                    AveragePrice = totalPricedCount > 0 ? totalPricedSum / totalPricedCount : 0m
                };
            })
            .OrderByDescending(x => x.TotalPrice)
            .ToList();

        sb.AppendLine($"## 📑 POLİÇE TÜRÜ DETAYLI ANALİZ SONUÇLARI ({isActiveText})");
        sb.AppendLine("=====================================================================================");

        const int colWidthCategory = 15;
        const int colWidthTotalCount = 10;
        const int colWidthTotalRenew = 10;
        const int colWidthTotalNew = 10;
        const int colWidthTotalGallery = 10;
        const int colWidthTotalPrice = 15;
        const int colWidthAvgPrice = 15;

        sb.Append("Poliçe".PadRight(colWidthCategory));
        sb.Append("Toplam".PadLeft(colWidthTotalCount));
        sb.Append("Yenileme".PadLeft(colWidthTotalRenew));
        sb.Append("Yeni".PadLeft(colWidthTotalNew));
        sb.Append("Galeri".PadLeft(colWidthTotalGallery));
        sb.Append("Toplam (TL)".PadLeft(colWidthTotalPrice));
        sb.Append("Ortalama (TL)".PadLeft(colWidthAvgPrice));
        sb.AppendLine();
        sb.AppendLine("------------------ ------ --------- --------- --------- -------------- --------------");

        var culture = new CultureInfo("tr-TR");

        foreach (var item in analysisData)
        {
            var formattedTotalPrice = item.TotalPrice.ToString("N0", culture);
            var formattedAveragePrice = item.AveragePrice.ToString("N0", culture);

            sb.Append(item.Category.Trim().PadRight(colWidthCategory));
            sb.Append(item.TotalCount.ToString().PadLeft(colWidthTotalCount));
            sb.Append(item.TotalRenew.ToString().PadLeft(colWidthTotalRenew));
            sb.Append(item.TotalNew.ToString().PadLeft(colWidthTotalNew));
            sb.Append(item.TotalGallery.ToString().PadLeft(colWidthTotalGallery));
            sb.Append(formattedTotalPrice.PadLeft(colWidthTotalPrice));
            sb.Append(formattedAveragePrice.PadLeft(colWidthAvgPrice));

            sb.AppendLine();
        }
    }

    public static List<ExelItem> GenerateCategoryCompanyDetails(List<PolicyItem> items)
    {
        var detailsData = items
            .GroupBy(p => new { p.Category, p.Company })
            .Select(g => new
            {
                g.Key.Category,
                g.Key.Company,
                CompanyCount = g.Count(),
                TotalPrice = g.Sum(p => ParseTotalPrice(p.TotalPrice)),
                IsRenewed = g.Count(p => p.IsRenewal),
                IsNew = g.Count(p => p.IsRenewal == false),
                IsGallery = g.Count(p => p.IsGalleryCustomer),
            })
            .OrderBy(x => x.Category)
            .ThenByDescending(x => x.TotalPrice)
            .ToList();

        var activeItems = new List<ExelItem>();

        foreach (var item in detailsData)
        {
            var formattedTotalPrice = item.TotalPrice.ToString("N0", new CultureInfo("tr-TR"));

            activeItems.Add(new ExelItem()
            {
                Category = item.Category.Trim(),
                Company = item.Company.Trim(),
                CompanyCount = item.CompanyCount,
                NewCount = item.IsNew,
                RenewCount = item.IsRenewed,
                GalleryCount = item.IsGallery,
                TotalPrice = Convert.ToDouble(formattedTotalPrice)
            });
        }

        return activeItems;
    }

    public static string SelectedDir()
    {
        using var dialog = new FolderBrowserDialog();
        dialog.Description = @"Lütfen raporların kaydedileceği dizini seçin.";
        dialog.ShowNewFolderButton = true;
        var result = dialog.ShowDialog();
        return result == DialogResult.OK ? dialog.SelectedPath : string.Empty;
    }
}