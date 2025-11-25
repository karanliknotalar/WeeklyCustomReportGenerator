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
}