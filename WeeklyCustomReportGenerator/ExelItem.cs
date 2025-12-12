using MiniExcelLibs.Attributes;

namespace WeeklyCustomReportGenerator;

public class ExelItem
{
    [ExcelColumn(Name = "ÜRÜN", Width = 15)]
    public string Category { get; set; } = string.Empty;

    [ExcelColumn(Name = "FİRMA", Width = 15)]
    public string Company { get; set; } = string.Empty;

    [ExcelColumn(Name = "TOPLAM ADET", Width = 15)]
    public int CompanyCount { get; set; } = 0;

    [ExcelColumn(Name = "YENİ", Width = 15)]
    public int NewCount { get; set; } = 0;

    [ExcelColumn(Name = "YENİLEME", Width = 15)]
    public int RenewCount { get; set; } = 0;

    [ExcelColumn(Name = "GALERİ ADET", Width = 15)]
    public int GalleryCount { get; set; } = 0;

    [ExcelColumn(Name = "TOPLAM FİYAT (TL)", Width = 15)]
    public double TotalPrice { get; set; } = double.MinValue;
}