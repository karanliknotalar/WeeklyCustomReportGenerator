namespace WeeklyCustomReportGenerator;

public class PdfReadResult
{
    public string FoundCompany { get; set; } = string.Empty;
    public string FoundTotalPrice { get; set; } = string.Empty;
    public bool IsSuccess { get; set; } = false;
}