#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WeeklyCustomReportGenerator;

public class WeeklyReportBuilder()
{
    private List<string>? ProductKeywords { get; set; }
    private CultureInfo? TrCulture { get; set; }

    private readonly Dictionary<string, Regex> _compiledPatterns = new();

    public WeeklyReportBuilder(string[] productOrder) : this()
    {
        ProductKeywords = productOrder.ToList();
        TrCulture = CultureInfo.GetCultureInfo("tr-TR");

        foreach (var keyword in productOrder)
        {
            var pattern = $@"\b{Regex.Escape(keyword.ToLower(TrCulture))}\b";
            _compiledPatterns[keyword] = new Regex(pattern,
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        }
    }

    public async Task<List<PolicyItem>> ParseFiles(IEnumerable<string> lines)
    {
        var items = new List<PolicyItem>();

        var enumerable = lines.ToList();
        var totalCount = enumerable.Count;
        var counter = 0;

        try
        {
            const string platePattern = @"(?<Plaka>(\d{2}[A-Z]{1,3}\d{1,5}|\d{2}[A-Z]{2}))";

            var pdfReader = new TextPdfReader();

            foreach (var path in enumerable)
            {
                var percentage = (int)((counter + 1) / (float)totalCount * 100);
                ProgressReporter.OnProgressChanged?.Invoke(percentage);

                if (string.IsNullOrWhiteSpace(path)) continue;

                var result = await pdfReader.ProcessPdf(path);

                var fileName = Path.GetFileNameWithoutExtension(path);

                var isCancel = TrCulture!.CompareInfo.IndexOf(fileName, "iptal", CompareOptions.IgnoreCase) >= 0;

                var date = DateTime.MinValue;
                var customerName = "";
                var plate = "";
                var isGalleryCustomer = false;

                var parts = fileName.Split([" - "], StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length > 1)
                {
                    DateTime.TryParse(parts[0], out date);
                    customerName = parts[1];
                }

                var match = Regex.Match(fileName, platePattern,
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                if (match.Success)
                {
                    plate = match.Groups["Plaka"].Value.Trim();
                    if (Form1.CustomerGalleryList.Any(n => n.ToLower(TrCulture) == customerName.ToLower(TrCulture)))
                    {
                        isGalleryCustomer = true;
                    }
                }
                
                var fileNameLower = fileName.ToLower(TrCulture);

                var matchedKeyword = ProductKeywords!.FirstOrDefault(keyword =>
                    _compiledPatterns[keyword].IsMatch(fileNameLower));
                var category = matchedKeyword ?? "DİĞER";

                items.Add(new PolicyItem
                {
                    Date = date,
                    FullLine = fileName,
                    Category = category,
                    IsCancel = isCancel,
                    CustomerName = customerName,
                    Plate = plate,
                    IsGalleryCustomer = isGalleryCustomer,
                    Company = result.FoundCompany,
                    TotalPrice = result.FoundTotalPrice
                });
                counter++;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return items;
    }

    public string BuildReport(List<PolicyItem> items)
    {
        var sb = new StringBuilder();
        Tools.CheckForPreviousYearLocalPolicy(items);

        var activeItems = items.Where(x => !x.IsCancel).ToList();
        var cancelledItems = items.Where(x => x.IsCancel).ToList();

        var groupActive = activeItems.GroupBy(x => x.Category).ToList();
        var groupCancelled = cancelledItems.GroupBy(x => x.Category).ToList();
        var groupOtherActive = groupActive.FirstOrDefault(g => g.Key == "DİĞER");
        var groupOtherCancelled = groupCancelled.FirstOrDefault(g => g.Key == "DİĞER");

        // --- İSTATİSTİK ---
        PrintStatistics(items, sb, activeItems, cancelledItems);

        // --- ÜRETİMLER ---
        sb.AppendLine($"ÜRETİMLER ({activeItems.Count:D2}):");
        sb.AppendLine();
        GenerateGroup(groupActive, sb, true);

        // --- DİĞER AKTİF GRUPLANMAYANLAR --- 
        if (groupOtherActive != null)
        {
            PrintGroup(sb, "DİĞER", groupOtherActive.ToList());
        }

        // --- İPTALLER ---
        sb.AppendLine();
        sb.AppendLine($"İPTALLER ({cancelledItems.Count:D2}):");
        sb.AppendLine();
        GenerateGroup(groupCancelled, sb);

        // --- DİĞER İPTAL GRUPLANMAYANLAR --- 
        if (groupOtherCancelled != null)
        {
            PrintGroup(sb, "DİĞER", groupOtherCancelled.ToList());
        }

        Tools.AppendToLogFileForUndefined(items.Where(x => string.IsNullOrEmpty(x.TotalPrice)).ToList());

        return sb.ToString();
    }

    private void PrintStatistics(List<PolicyItem> items, StringBuilder sb,
        List<PolicyItem> activeItems, List<PolicyItem> cancelledItems)
    {
        const int colWidthLabel = 30;
        const int colWidthValue = 17;

        var approvedCount = activeItems.Count;
        var cancelledCount = cancelledItems.Count;
        var totalCount = items.Count;

        var galleryCount = activeItems.Count(x => x.IsGalleryCustomer);
        var galleryCancelledCount = cancelledItems.Count(x => x.IsGalleryCustomer);

        var renewalCount = activeItems.Count(x => x.IsRenewal);
        var newCount = approvedCount - renewalCount;

        sb.AppendLine("\r\n");
        sb.AppendLine("╔═══════════════════════════════════════════════════╗");
        sb.AppendLine("║            POLİÇE GENEL İSTATİSTİKLERİ            ║");
        sb.AppendLine("╠═══════════════════════════════════════════════════╣");
        sb.Append($"║ {"TOPLAM LİSTELENEN POLİÇE",-colWidthLabel}: ");
        sb.AppendLine(totalCount.ToString().PadLeft(colWidthValue) + " ║");
        sb.AppendLine("╠═══════════════════════════════════════════════════╣");
        sb.Append($"║ {"ONAYLANAN POLİÇE SAYISI",-colWidthLabel}: ");
        sb.AppendLine(approvedCount.ToString().PadLeft(colWidthValue) + " ║");
        sb.AppendLine("╟───────────────────────────────────────────────────╢");
        sb.Append($"║ {"  > Yenilenen Poliçe Sayısı",-colWidthLabel}: ");
        sb.AppendLine(renewalCount.ToString().PadLeft(colWidthValue) + " ║");
        sb.Append($"║ {"  > Yeni Poliçe Sayısı",-colWidthLabel}: ");
        sb.AppendLine(newCount.ToString().PadLeft(colWidthValue) + " ║");
        sb.Append($"║ {"  > Galeri Müşterisi",-colWidthLabel}: ");
        sb.AppendLine(galleryCount.ToString().PadLeft(colWidthValue) + " ║");
        sb.AppendLine("╠═══════════════════════════════════════════════════╣");
        sb.Append($"║ {"İPTAL EDİLEN POLİÇE SAYISI",-colWidthLabel}: ");
        sb.AppendLine(cancelledCount.ToString().PadLeft(colWidthValue) + " ║");
        sb.AppendLine("╟───────────────────────────────────────────────────╢");
        sb.Append($"║ {"  > Galeri Müşterisi (İptal)",-colWidthLabel}: ");
        sb.AppendLine(galleryCancelledCount.ToString().PadLeft(colWidthValue) + " ║");
        sb.AppendLine("╚═══════════════════════════════════════════════════╝");

        sb.AppendLine("\r\n\r\n");
        Tools.GenerateCategoryAnalysis(activeItems, sb);
        sb.AppendLine("\r\n\r\n");
        Tools.GenerateCategoryAnalysis(cancelledItems, sb, true);
        sb.AppendLine("\r\n\r\n");
    }

    private void GenerateGroup(List<IGrouping<string, PolicyItem>> groups, StringBuilder sb, bool printStatus = false)
    {
        foreach (var keyword in ProductKeywords!)
        {
            var group = groups.FirstOrDefault(g => g.Key == keyword);

            if (group != null)
            {
                PrintGroup(sb, keyword.ToUpper(new CultureInfo("tr-TR")), group.ToList(), printStatus);
            }
        }
    }

    private static void PrintGroup(StringBuilder sb, string categoryName, List<PolicyItem> list,
        bool printStatus = false)
    {
        var sortedList = list.OrderBy(x => x.Date).ThenBy(x => x.FullLine).ToList();
        var renewListCount = list.Count(x => x.IsRenewal);
        var galleryListCount = list.Count(x => x.IsGalleryCustomer);
        
        var newCount = sortedList.Count - renewListCount;
        var newNormalCount = newCount - galleryListCount;

        var newDetails = galleryListCount != 0
            ? string.Join(", ", new[]
            {
                newNormalCount != 0 ? $"{newNormalCount:D2} N.Müşteri" : null,
                $"{galleryListCount:D2} Galeri"
            }.Where(x => x != null))
            : "";

        var renewTxt = renewListCount != 0 ? $" | {renewListCount:D2} Yenileme" : "";
        var newTxt = newCount != 0 ? $" | {newCount:D2} Yeni{(newDetails != "" ? $" ({newDetails})" : "")}" : "";
        var galleryTxt = galleryListCount != 0 ? $" {galleryListCount:D2} Galeri" : "";

        sb.AppendLine(printStatus
            ? $"\t{categoryName} ({sortedList.Count:D2}){newTxt}{renewTxt} |"
            : $"\t{categoryName} ({sortedList.Count:D2}) |{galleryTxt}");

        
        var lines = sortedList.Select(item =>
        {
            var isRenew = item.IsRenewal ? "+" : "-";
            var renewStatus = printStatus ? $"{isRenew}|" : "";
            var gTxt = item.IsGalleryCustomer ? "(G) " : "";
            var prefix = $"{renewStatus} {item.FullLine} {gTxt}".TrimEnd().Normalize(NormalizationForm.FormC);
            var companyInfo = !string.IsNullOrEmpty(item.Company)
                ? $"【{item.TotalPrice}】{item.Company}"
                : "";
            return (prefix, companyInfo);
        }).ToList();

        var maxLen = lines.Count != 0 ? lines.Max(l => l.prefix.Length) : 0;

        foreach (var (prefix, companyInfo) in lines)
        {
            sb.AppendLine($"\t\t{prefix.PadRight(maxLen)} {companyInfo}");
        }

        sb.AppendLine();
    }
}