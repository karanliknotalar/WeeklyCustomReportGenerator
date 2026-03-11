#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using MiniExcelLibs;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

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

                dict[$"{category}\nAdet"] = count > 0
                    ? count
                    : "";

                dict[$"{category}\nTutar"] = count > 0
                    ? FormatPrice(total)
                    : "";
            }

            dict["Toplam\nTutar"] = FormatPrice(row.GrandTotal);
            dict["Toplam\nPoliçe\nAdedi"] = row.TotalPolicyCount;
            dict["Yenileme\nAdedi"] = row.RenewCount;
            dict["Galeri\nMüşterisi"] = row.IsGalleryCustomer ? "Evet" : "Hayır";

            excelRows.Add(dict);
        }

        var sheets = new Dictionary<string, object>
        {
            [sheetName] = excelRows
        };

        MiniExcel.SaveAs(filePath, sheets, overwriteFile: true);
        ApplyWrapTextToHeaders(filePath, sheetName);
        ApplyColumnWidths(filePath, sheetName);
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

    private static void ApplyWrapTextToHeaders(string filePath, string sheetName)
    {
        using var doc = SpreadsheetDocument.Open(filePath, isEditable: true);
        var wb = doc.WorkbookPart!;

        var sheet = wb.Workbook?.Sheets!
            .Elements<Sheet>()
            .First(s => s.Name == sheetName);

        var wsPart = (WorksheetPart)wb.GetPartById(sheet?.Id!);
        var sheetData = wsPart.Worksheet?.GetFirstChild<SheetData>()!;

        var stylesPart = wb.WorkbookStylesPart
                         ?? wb.AddNewPart<WorkbookStylesPart>();

        stylesPart.Stylesheet ??= new Stylesheet();

        var stylesheet = stylesPart.Stylesheet;

        stylesheet.CellFormats ??= new CellFormats();

        var headerRow = sheetData.Elements<Row>().First();
        foreach (var cell in headerRow.Elements<Cell>())
        {
            var existingStyleIndex = cell.StyleIndex?.Value ?? 0;
            var existingFormat = stylesheet.CellFormats
                .Elements<CellFormat>()
                .ElementAt((int)existingStyleIndex)
                .CloneNode(true) as CellFormat;

            existingFormat!.Alignment ??= new Alignment();
            existingFormat.Alignment.WrapText = true;
            existingFormat.ApplyAlignment = true;

            stylesheet.CellFormats.Append(existingFormat);
            var newIndex = (uint)(stylesheet.CellFormats.Count() - 1);

            cell.StyleIndex = newIndex;
        }

        stylesheet.Save();
        wsPart.Worksheet?.Save();
    }

    private static void ApplyColumnWidths(string filePath, string sheetName)
    {
        using var doc = SpreadsheetDocument.Open(filePath, isEditable: true);
        var wb = doc.WorkbookPart!;

        var sheet = wb.Workbook?.Sheets!
            .Elements<Sheet>()
            .First(s => s.Name == sheetName);

        var wsPart = (WorksheetPart)wb.GetPartById(sheet?.Id!);
        var worksheet = wsPart.Worksheet!;
        var sheetData = worksheet.GetFirstChild<SheetData>()!;

        var columnMaxLengths = new Dictionary<int, int>();

        foreach (var row in sheetData.Elements<Row>())
        {
            var colIndex = 0;
            foreach (var cell in row.Elements<Cell>())
            {
                var text = GetCellText(cell, wb);
                var maxLineLength = text
                    .Split('\n')
                    .Max(line => line.Length);

                if (!columnMaxLengths.ContainsKey(colIndex) || columnMaxLengths[colIndex] < maxLineLength)
                    columnMaxLengths[colIndex] = maxLineLength;

                colIndex++;
            }
        }

        var columns = worksheet.GetFirstChild<Columns>() ?? new Columns();
        columns.RemoveAllChildren();

        foreach (var kvp in columnMaxLengths)
        {
            var width = Math.Max(10, Math.Min(kvp.Value * 1, 25));
            columns.Append(new Column
            {
                Min = (uint)(kvp.Key + 1),
                Max = (uint)(kvp.Key + 1),
                Width = width,
                CustomWidth = true,
                BestFit = true
            });
        }

        if (worksheet.GetFirstChild<Columns>() == null)
            worksheet.InsertBefore(columns, sheetData);

        worksheet.Save();
    }

    private static string GetCellText(Cell cell, WorkbookPart wb)
    {
        if (cell.DataType?.Value != CellValues.SharedString) return cell.InnerText;
        var sst = wb.SharedStringTablePart?.SharedStringTable;
        var index = int.Parse(cell.InnerText);
        return sst?.ElementAt(index).InnerText ?? "";

    }
}