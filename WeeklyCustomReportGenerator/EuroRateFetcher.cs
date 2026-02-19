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
    private static readonly HttpClient _httpClient = new();

    /// <summary>
    /// Dosya yolundan tarihi parse eder, o tarihe ait TCMB Euro satış kurunu döner.
    /// </summary>
    public static async Task<decimal> GetEuroRateFromFilePathAsync(string filePath)
    {
        var date = ParseDateFromFilePath(filePath);
        return await GetTcmbEuroSellRateAsync(date);
    }

    /// <summary>
    /// "14.02.2026" formatındaki tarihi dosya yolundan çeker.
    /// </summary>
    public static DateTime ParseDateFromFilePath(string filePath)
    {
        // Dosya adını al: "14.02.2026 - Gürsoyplus..."
        var fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);

        var match = Regex.Match(fileName, @"(\d{2})\.(\d{2})\.(\d{4})");
        if (!match.Success)
            throw new FormatException($"Dosya adında tarih bulunamadı: {fileName}");

        int day   = int.Parse(match.Groups[1].Value);
        int month = int.Parse(match.Groups[2].Value);
        int year  = int.Parse(match.Groups[3].Value);

        return new DateTime(year, month, day);
    }

    /// <summary>
    /// TCMB XML servisinden verilen tarihe ait Euro satış kurunu çeker.
    /// Hafta sonu / tatil günü ise bir önceki iş gününe geri gider (max 5 gün).
    /// </summary>
    public static async Task<decimal> GetTcmbEuroSellRateAsync(DateTime date)
    {
        for (int i = 0; i < 5; i++)
        {
            var checkDate = date.AddDays(-i);

            // TCMB URL formatı: https://www.tcmb.gov.tr/kurlar/YYYYMM/DDMMYYYY.xml
            var url = $"https://www.tcmb.gov.tr/kurlar/" +
                      $"{checkDate:yyyy}{checkDate:MM}/" +
                      $"{checkDate:dd}{checkDate:MM}{checkDate:yyyy}.xml";

            try
            {
                var xml = await _httpClient.GetStringAsync(url);
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
                // O gün veri yok (hafta sonu/tatil), bir gün geri git
                continue;
            }
        }

        throw new Exception($"{date:dd.MM.yyyy} tarihi için TCMB Euro kuru bulunamadı.");
    }
}