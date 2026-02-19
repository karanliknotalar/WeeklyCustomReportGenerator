#nullable enable
using System.Collections.Generic;

namespace WeeklyCustomReportGenerator;

public class Company
{
    public string CompanySearchText { get; set; } = "";
    public string CompanyName { get; set; } = "";
    public string TotalPriceRegexPattern { get; set; } = "";

    public string? EurPriceRegexPattern { get; set; }
    
    public EuroConversionMode EuroConversion { get; set; } = EuroConversionMode.None;

    public List<string> EuroConversionPathKeywords { get; set; } = [];
}

public enum EuroConversionMode
{
    None,
    Always,
    WhenPathContains,
    FallbackToEurWhenPathContains  // TL pattern tutmazsa EUR'a dene
}