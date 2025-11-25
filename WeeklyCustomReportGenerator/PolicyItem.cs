#nullable enable
using System;

namespace WeeklyCustomReportGenerator;

public class PolicyItem
{
    public DateTime Date { get; set; } 
    public string FullLine { get; set; } = ""; 
    public string Category { get; set; } = ""; 
    public bool IsCancel { get; set; } 
}