#nullable enable
using System.Collections.Generic;
using System.Linq;
using MiniExcelLibs;

namespace WeeklyCustomReportGenerator;

public static class CustomerAnalysisExcelWriter
{
    private class CustomerAnalysisItem
    {
        public string CustomerName { get; set; } = "";
        public Dictionary<string, decimal> CategoryTotals { get; set; } = new();
        public Dictionary<string, int> CategoryCounts { get; set; } = new();
        public decimal GrandTotal { get; set; }
        public int TotalPolicyCount { get; set; }
        public int RenewCount { get; set; }
        public bool IsGalleryCustomer { get; set; }
    }

    public static void WriteCustomerAnalysisSheet(
        string filePath,
        string sheetName,
        List<PolicyItem> items)
    {
        var (rows, categories) = GenerateCustomerAnalysis(items);

        var excelRows = new List<IDictionary<string, object?>>();

        foreach (var row in rows)
        {
            var dict = new Dictionary<string, object?>
            {
                ["Müşteri Adı"] = row.CustomerName
            };

            foreach (var category in categories)
            {
                var count = row.CategoryCounts[category];
                var total = row.CategoryTotals[category];
                
                dict[$"{category} - Adet"] = count > 0
                    ? count
                    : "";

                dict[$"{category} - Tutar"] = count > 0
                    ? FormatPrice(total)
                    : "";
            }

            dict["Toplam Tutar"] = FormatPrice(row.GrandTotal);
            dict["Toplam Poliçe Adedi"] = row.TotalPolicyCount;
            dict["Yenileme Adedi"] = row.RenewCount;
            dict["Galeri Müşterisi"] = row.IsGalleryCustomer ? "Evet" : "Hayır";

            excelRows.Add(dict);
        }

        var sheets = new Dictionary<string, object>
        {
            [sheetName] = excelRows
        };

        MiniExcel.SaveAs(filePath, sheets, overwriteFile: true);
    }

    private static string FormatPrice(decimal price)
        => price.ToString("N2", new System.Globalization.CultureInfo("tr-TR")) + " ₺";
    
    private static (List<CustomerAnalysisItem> Rows, List<string> Categories) GenerateCustomerAnalysis(
        List<PolicyItem> items)
    {
        var categories = items
            .Select(p => p.Category.Trim())
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        var grouped = items
            .GroupBy(p => p.CustomerName.Trim())
            .OrderBy(g => g.Key)
            .ToList();

        var result = new List<CustomerAnalysisItem>();

        foreach (var customerGroup in grouped)
        {
            var row = new CustomerAnalysisItem
            {
                CustomerName = customerGroup.Key,
                TotalPolicyCount = customerGroup.Count(),
                RenewCount = customerGroup.Count(p => p.IsRenewal),
                IsGalleryCustomer = customerGroup.Any(p => p.IsGalleryCustomer),
                GrandTotal = customerGroup.Sum(p => Tools.ParseTotalPrice(p.TotalPrice)),
            };

            foreach (var category in categories)
            {
                var categoryPolicies = customerGroup
                    .Where(p => p.Category.Trim() == category)
                    .ToList();

                row.CategoryTotals[category] = categoryPolicies.Sum(p => Tools.ParseTotalPrice(p.TotalPrice));
                row.CategoryCounts[category] = categoryPolicies.Count;
            }

            result.Add(row);
        }

        return (result, categories);
    }
    
}