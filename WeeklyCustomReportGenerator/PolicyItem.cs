#nullable enable
using System;

namespace WeeklyCustomReportGenerator;

public class PolicyItem
{
    public DateTime Date { get; set; }
    public string Customer { get; set; } = "";
    public string Plate { get; set; } = "";
    public string TypeFromFile { get; set; } = "";
    public string Category { get; set; } = "";
    public bool IsCancel { get; set; }
    public string FileNameWithoutExt { get; set; } = "";
}