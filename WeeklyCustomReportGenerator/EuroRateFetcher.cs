#nullable enable
using System;
using System.Globalization;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WeeklyCustomReportGenerator;

public static class EuroRateFetcher
{
    private static readonly HttpClient HttpClient = new();
    
    public static async Task<decimal> GetEuroRateFromFilePathAsync(string filePath)
    {
        var date = ParseDateFromFilePath(filePath);
        return await GetTcmbEuroSellRateAsync(date);
    }
    
    private static DateTime ParseDateFromFilePath(string filePath)
    {
        var fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);

        var match = Regex.Match(fileName, @"(\d{2})\.(\d{2})\.(\d{4})");
        if (!match.Success)
            throw new FormatException($"Dosya adında tarih bulunamadı: {fileName}");

        var day   = int.Parse(match.Groups[1].Value);
        var month = int.Parse(match.Groups[2].Value);
        var year  = int.Parse(match.Groups[3].Value);

        return new DateTime(year, month, day);
    }
    
    private static async Task<decimal> GetTcmbEuroSellRateAsync(DateTime date)
    {
        for (var i = 0; i < 5; i++)
        {
            var checkDate = date.AddDays(-i);

            var url = $"https://www.tcmb.gov.tr/kurlar/" +
                      $"{checkDate:yyyy}{checkDate:MM}/" +
                      $"{checkDate:dd}{checkDate:MM}{checkDate:yyyy}.xml";

            try
            {
                var xml = await HttpClient.GetStringAsync(url);
                var doc = XDocument.Parse(xml);

                foreach (var currency in doc.Descendants("Currency"))
                {
                    var code = currency.Attribute("CurrencyCode")?.Value;
                    if (code != "EUR") continue;

                    var sellStr = currency.Element("ForexSelling")?.Value;
                    if (decimal.TryParse(sellStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var rate))
                        return rate;
                }
            }
            catch (HttpRequestException)
            {
                continue;
            }
        }

        throw new Exception($"{date:dd.MM.yyyy} tarihi için TCMB Euro kuru bulunamadı.");
    }
}