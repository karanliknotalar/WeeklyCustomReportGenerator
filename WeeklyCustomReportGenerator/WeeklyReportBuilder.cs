#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

        try
        {
            const string platePattern = @"(?<Plaka>(\d{2}[A-Z]{1,3}\d{1,5}|\d{2}[A-Z]{2}))";

            var pdfReader = new TextPdfReader();

            foreach (var path in lines)
            {
                if (string.IsNullOrWhiteSpace(path)) continue;

                var result = pdfReader.ProcessPdf(path);

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
                    if (Form1.CustomerGalleryList.Any(n => n == customerName))
                    {
                        isGalleryCustomer = true;
                    }
                }

                var category = "DİĞER";


                foreach (var keyword in ProductKeywords!)
                {
                    var pattern = $@"\b{Regex.Escape(keyword.ToLower())}\b";

                    if (!Regex.IsMatch(fileName.ToLower(), pattern,
                            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) continue;
                    category = keyword;
                    break;
                }

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
        var groupOther = groupActive.FirstOrDefault(g => g.Key == "DİĞER");

        // --- ÜRETİMLER ---
        sb.AppendLine($"ÜRETİMLER ({activeItems.Count:D2}):");
        sb.AppendLine();
        GenerateGroup(groupActive, sb, true);

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

        PrintStatistics(items, sb);
        Tools.AppendToLogFileForUndefined(items.Where(x => string.IsNullOrEmpty(x.TotalPrice)).ToList());
        return sb.ToString();
    }

    private void PrintStatistics(List<PolicyItem> items, StringBuilder sb)
    {
        var activeItems = items.Where(x => !x.IsCancel).ToList();
        var cancelledItems = items.Where(x => x.IsCancel).ToList();
        const int colWidthLabel = 30;
        const int colWidthValue = 17;

        var approvedCount = activeItems.Count;
        var cancelledCount = cancelledItems.Count;
        var totalCount = items.Count;

        var galleryCount = activeItems.Count(x => x.IsGalleryCustomer);
        var galleryCancelledCount = cancelledItems.Count(x => x.IsGalleryCustomer);

        var renewalCount = activeItems.Count(x => x.IsRenewal);
        var newCount = approvedCount - renewalCount; 

        sb.AppendLine("\r\n\r\n");
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
        Tools.GenerateCategoryCompanyDetails(activeItems, sb);
    }

    private void GenerateGroup(List<IGrouping<string, PolicyItem>> groups, StringBuilder sb, bool printStatus = false)
    {
        foreach (var keyword in ProductKeywords!)
        {
            var group = groups.FirstOrDefault(g => g.Key == keyword);

            if (group != null)
            {
                PrintGroup(sb, keyword.ToUpper(), group.ToList(), printStatus);
            }
        }
    }

    private static void PrintGroup(StringBuilder sb, string categoryName, List<PolicyItem> list,
        bool printStatus = false)
    {
        var sortedList = list.OrderBy(x => x.Date).ThenBy(x => x.FullLine).ToList();
        var renewListCount = list.Where(x => x.IsRenewal).ToList().Count;
        var galleryListCount = list.Where(x => x.IsGalleryCustomer).ToList().Count;

        var renewTxt = renewListCount != 0 ? $"|{renewListCount:D2} Yenileme" : "";
        var newTxt = (sortedList.Count - renewListCount) != 0 ? $"|{sortedList.Count - renewListCount:D2} Yeni" : "";
        var galleryTxt = galleryListCount != 0 ? $"{galleryListCount:D2} Tanesi Galeri" : "";


        sb.AppendLine(printStatus
            ? $"\t{categoryName} ({sortedList.Count:D2}) {newTxt} {galleryTxt}{renewTxt}|"
            : $"\t{categoryName} ({sortedList.Count:D2}) |{galleryTxt}");


        foreach (var item in sortedList)
        {
            var isRenew = item.IsRenewal ? "+" : "-";
            var renewStatus = printStatus ? $"{isRenew}|" : "";
            var gTxt = item.IsGalleryCustomer ? "(G) " : "";
            var companyInfo = !string.IsNullOrEmpty(item.Company)
                ? $"\u27a4{item.TotalPrice} \u275a{item.Company}"
                : "";
            sb.AppendLine($"\t\t{renewStatus} {item.FullLine} {gTxt}{companyInfo}");
        }

        sb.AppendLine();
    }
}