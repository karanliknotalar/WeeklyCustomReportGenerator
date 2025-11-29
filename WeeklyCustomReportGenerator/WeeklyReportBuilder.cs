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

            foreach (var keyword in ProductKeywords!)
            {
                if (TrCulture.CompareInfo.IndexOf(fileName, keyword, CompareOptions.IgnoreCase) < 0) continue;
                category = keyword.ToUpper(TrCulture);
                break;
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
        var sb = new StringBuilder();

        var activeItems = items.Where(x => !x.IsCancel).ToList();
        var cancelledItems = items.Where(x => x.IsCancel).ToList();

        var groupActive = activeItems.GroupBy(x => x.Category).ToList();
        var groupCancelled = cancelledItems.GroupBy(x => x.Category).ToList();
        var groupOther = groupActive.FirstOrDefault(g => g.Key == "DİĞER");

        // --- ÜRETİMLER ---
        sb.AppendLine($"ÜRETİMLER ({activeItems.Count}):");
        sb.AppendLine();
        GenerateGroup(groupActive, sb);

        // --- DİĞER GRUPLANMAYANLAR --- 
        if (groupOther != null)
        {
            PrintGroup(sb, "DİĞER", groupOther.ToList());
        }

        // --- İPTALLER ---
        sb.AppendLine();
        sb.AppendLine($"İPTALLER ({cancelledItems.Count:D2}):");
        sb.AppendLine();

        GenerateGroup(groupCancelled, sb);

        return sb.ToString();
    }

    private void GenerateGroup(List<IGrouping<string, PolicyItem>> groups, StringBuilder sb)
    {
        foreach (var keyword in ProductKeywords!)
        {
            var categoryKey = keyword.ToUpper(TrCulture!);

            var group = groups.FirstOrDefault(g => g.Key == categoryKey);

            if (group != null)
            {
                PrintGroup(sb, categoryKey, group.ToList());
            }
        }
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