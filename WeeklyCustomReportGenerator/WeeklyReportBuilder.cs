#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace WeeklyCustomReportGenerator;

public class WeeklyReportBuilder(IEnumerable<string> products)
{
    private List<string> ProductDefinitions { get; set; } = products.ToList();
    private CultureInfo TrCulture { get; set; } = CultureInfo.GetCultureInfo("tr-TR");

    public List<PolicyItem> ParseFiles(IEnumerable<string> lines)
    {
        var items = new List<PolicyItem>();

        foreach (var path in lines)
        {
            if (string.IsNullOrWhiteSpace(path)) continue;

            var fileName = Path.GetFileNameWithoutExtension(path);

            var isCancel = TrCulture.CompareInfo.IndexOf(fileName, "İptal", CompareOptions.IgnoreCase) >= 0;

            var parts = fileName.Split([" - "], StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2) continue;

            if (!DateTime.TryParse(parts[0].Trim(), out var date)) continue;

            var customer = parts[1].Trim();
            var plate = "";
            var typeFromFile = "";

            switch (parts.Length)
            {
                case 3:
                    // Format: Tarih - İsim - Tür
                    typeFromFile = parts[2].Trim();
                    break;
                case 4:
                    // Format: Tarih - İsim - Plaka - Tür
                    plate = parts[2].Trim();
                    typeFromFile = parts[3].Trim();
                    break;
                case > 4:
                    // Format: Tarih - İsim - Plaka - Tür - Açıklama - Zeyil vs.
                    plate = parts[2].Trim();

                    // Geri kalan her şeyi birleştirip tür/açıklama yapıyoruz
                    typeFromFile = string.Join(" - ", parts.Skip(3));
                    break;
            }

            var category = DetectCategory(typeFromFile);

            items.Add(new PolicyItem
            {
                Date = date,
                Customer = customer,
                Plate = plate,
                TypeFromFile = typeFromFile,
                Category = category,
                IsCancel = isCancel,
                FileNameWithoutExt = fileName
            });
        }

        return items;
    }

    private string DetectCategory(string typeFromFile)
    {
        var typeLower = typeFromFile.ToLower(TrCulture);

        foreach (var prod in ProductDefinitions)
        {
            var prodLower = prod.ToLower(TrCulture);

            if (typeLower.Contains(prodLower))
            {
                if (prod.Equals("Konut", StringComparison.OrdinalIgnoreCase) ||
                    prod.Equals("İşyeri", StringComparison.OrdinalIgnoreCase))
                {
                    return "Konut&İşyeri";
                }

                return prod.ToUpper(TrCulture);
            }
        }

        return "DİĞER";
    }

    public string BuildReport(List<PolicyItem> items)
    {
        // İptal olanları ve olmayanları ayır
        var activeItems = items.Where(x => !x.IsCancel).ToList();
        var cancelledItems = items.Where(x => x.IsCancel).ToList();

        var sb = new StringBuilder();
        sb.AppendLine($"ÜRETİMLER ({activeItems.Count})");
        sb.AppendLine();

        // Kategori listesini oluştur
        var definedCategories = ProductDefinitions
            .Select(p =>
                (p.Equals("Konut", StringComparison.OrdinalIgnoreCase) ||
                 p.Equals("İşyeri", StringComparison.OrdinalIgnoreCase))
                    ? "Konut&İşyeri"
                    : p.ToUpper(TrCulture))
            .Distinct()
            .ToList();

        var groupings = activeItems.GroupBy(x => x.Category).ToList();

        // 1. Tanımlı ürünleri sırayla yazdır
        foreach (var categoryName in definedCategories)
        {
            var group = groupings.FirstOrDefault(g => g.Key == categoryName);
            if (group != null)
            {
                PrintGroup(sb, group.Key, group.ToList());
            }
        }

        // 2. Diğer grubunu yazdır
        var otherGroup = groupings.FirstOrDefault(g => !definedCategories.Contains(g.Key));
        if (otherGroup != null)
        {
            PrintGroup(sb, "DİĞER", otherGroup.ToList());
        }
        else
        {
            sb.AppendLine($"\tDİĞER (00):");
            sb.AppendLine();
        }

        // --- İPTALLER BÖLÜMÜ ---
        sb.AppendLine();
        sb.AppendLine($"İPTALLER ({cancelledItems.Count:D2})");

        if (cancelledItems.Any())
        {
            sb.AppendLine($"\tİptal ({cancelledItems.Count:D2}): ");

            // İptalleri tarihe göre sırala ve yazdır
            foreach (var item in cancelledItems.OrderBy(x => x.Date))
            {
                // İptallerde dosya isminin tamamını (uzantısız) olduğu gibi yazdır
                sb.AppendLine($"\t\t{item.FileNameWithoutExt}");
            }
        }

        return sb.ToString();
    }

    private static void PrintGroup(StringBuilder sb, string categoryName, List<PolicyItem> list)
    {
        var sortedList = list.OrderBy(x => x.Date).ThenBy(x => x.Customer).ToList();

        sb.AppendLine($"\t{categoryName} ({sortedList.Count:D2}):");

        foreach (var item in sortedList)
        {
            var platePart = string.IsNullOrWhiteSpace(item.Plate) ? "" : $" - {item.Plate}";
            sb.AppendLine($"\t\t{item.Date:dd.MM.yyyy} - {item.Customer}{platePart} - {item.TypeFromFile}");
        }

        sb.AppendLine();
    }

    public static string GetDynamicRegex()
    {
        const string staticPart =
            @"(?i)^(?!.*\bmakbuz\b)(?!.*\beng\b)(?!.*\bhayat\b)(?!.*\byeşil\b)(?:(?!.*\bzeyil\b)|(?=.*\bİptal\b)).*";

        var today = DateTime.Now.Date;

        var lastSunday = today.AddDays(-(int)today.DayOfWeek);

        var dateList = new List<string>();

        for (var date = lastSunday; date <= today; date = date.AddDays(1))
        {
            dateList.Add(date.ToString("dd\\.MM\\.yyyy"));
        }

        var datePart = "(" + string.Join("|", dateList) + ")";

        return staticPart + datePart + ".*";
    }
}