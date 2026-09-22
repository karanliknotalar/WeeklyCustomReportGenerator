#nullable enable
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WeeklyCustomReportGenerator;

public class Company
{
    public string CompanySearchText { get; set; } = "";
    public string CompanyName { get; set; } = "";
    public string TlTotalPriceRegexPattern { get; set; } = "";

    public string? EurTotalPriceRegexPattern { get; set; }

    public EuroConversionMode EuroConversion { get; set; } = EuroConversionMode.None;

    public List<string> EuroConversionPathKeywords { get; set; } = [];
    public List<string> DefinitelyEuroConversionPathKeywords { get; set; } = [];
}

// JsonConverter sayesinde bu alan json dosyasında 0/1/2 yerine
// "None" / "WhenPathContains" / "FallbackToEurWhenPathContains" olarak okunaklı yazılır.
[JsonConverter(typeof(StringEnumConverter))]
public enum EuroConversionMode
{
    None,
    WhenPathContains,
    FallbackToEurWhenPathContains // TL pattern tutmazsa EUR'a dene
}