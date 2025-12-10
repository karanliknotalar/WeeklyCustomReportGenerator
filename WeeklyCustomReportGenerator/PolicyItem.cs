#nullable enable
using System;

namespace WeeklyCustomReportGenerator;

public class PolicyItem
{
    public DateTime Date { get; set; }
    public string FullLine { get; set; } = "";
    public string Category { get; set; } = "";
    public string Company { get; set; } = "";
    public string TotalPrice { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public string Plate { get; set; } = "";
    public bool IsRenewal { get; set; } = false;
    public bool IsGalleryCustomer { get; set; } = false;
    public bool IsCancel { get; set; }
}