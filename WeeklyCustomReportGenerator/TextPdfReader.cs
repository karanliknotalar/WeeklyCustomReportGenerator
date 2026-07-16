#nullable enable
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace WeeklyCustomReportGenerator;

public partial class TextPdfReader
{
    public async Task<PdfReadResult> ProcessPdf(string pdfPath)
    {
        var pdfReadResult = new PdfReadResult();

        try
        {
            var pdfContent = ReadPdf_IText7_Advanced(pdfPath);

            Tools.AppendToLogFile(pdfContent, pdfPath, new Company());


            foreach (var company in _companies.Where(company =>
                         Tools.SearchCompanyText(pdfContent, company.CompanySearchText)))
            {
                pdfReadResult.FoundCompany = company.CompanyName;

                Console.WriteLine(@"---------------------------------------------------------");
                Console.WriteLine($@"İşlenen Dosya: {pdfPath}");
                Console.WriteLine($@"FİRMA ADI BULUNDU: {company.CompanyName}");
                Console.WriteLine($@"Aramada Kullanılan Metin: {company.CompanySearchText}");
                Console.WriteLine(@"---------------------------------------------------------");
                Console.WriteLine(@"");

                var needsEuroConversion = company.EuroConversion == EuroConversionMode.WhenPathContains &&
                                          company.EuroConversionPathKeywords.Any(pdfPath.Contains);
                var definitelyNeedsEuroConversion = company.DefinitelyEuroConversionPathKeywords.Any(pdfPath.Contains);
                //definitelyNeedsEuroConversion tl pattern ile bulduğu euro olan primi, tl olarak yazmak yerine zorunlu olarak euro ya çevirir.

                var patternToUse = needsEuroConversion && company.EurTotalPriceRegexPattern != null
                    ? company.EurTotalPriceRegexPattern
                    : company.TlTotalPriceRegexPattern;

                // var match = Regex.Match(pdfContent, patternToUse, RegexOptions.IgnoreCase);
                var matches = Regex.Matches(pdfContent, patternToUse, RegexOptions.IgnoreCase);
                if (matches.Count > 0)
                {
                    // var foundPrice = !string.IsNullOrEmpty(match.Groups[1].Value)
                    //     ? match.Groups[1].Value
                    //     : match.Groups[2].Value;

                    var bestMatch = matches.Cast<Match>()
                        .OrderByDescending(m => m.Value.Contains("(TL)"))
                        .ThenByDescending(m => m.Value.Contains("Döviz Karşõlõğõ"))
                        .ThenBy(m => m.Index)
                        .FirstOrDefault();

                    // var foundPrice = !string.IsNullOrEmpty(bestMatch?.Groups[1].Value)
                    //     ? bestMatch?.Groups[1].Value
                    //     : bestMatch?.Groups[2].Value;

                    var foundPrice = bestMatch?.Groups[1].Value is { Length: > 0 } g1 ? g1 : bestMatch?.Groups[2].Value;

                    if (needsEuroConversion || definitelyNeedsEuroConversion)
                    {
                        pdfReadResult.FoundTotalPrice = await EuroToTlConversion(pdfPathForDate: pdfPath, foundPrice!);
                    }
                    else
                    {
                        pdfReadResult.FoundTotalPrice = foundPrice;
                    }

                    pdfReadResult.IsSuccess = true;
                }
                else if (company.EuroConversion == EuroConversionMode.FallbackToEurWhenPathContains
                         && company.EurTotalPriceRegexPattern != null
                         && company.EuroConversionPathKeywords.Any(pdfPath.Contains))
                {
                    var matchEur = Regex.Match(pdfContent, company.EurTotalPriceRegexPattern, RegexOptions.IgnoreCase);
                    if (matchEur.Success)
                    {
                        var foundPrice = !string.IsNullOrEmpty(matchEur.Groups[1].Value)
                            ? matchEur.Groups[1].Value
                            : matchEur.Groups[2].Value;

                        pdfReadResult.FoundTotalPrice = await EuroToTlConversion(pdfPathForDate: pdfPath, foundPrice);
                        pdfReadResult.IsSuccess = true;
                    }
                }

                break;
            }
        }
        catch (Exception ex)
        {
            pdfReadResult.FoundCompany = "Error: " + ex.Message;
            Console.WriteLine(ex.Message);
        }

        return pdfReadResult;
    }

    private static string ReadPdf_IText7_Advanced(string filePath)
    {
        try
        {
            var sb = new StringBuilder();

            using (var reader = new PdfReader(filePath))
            using (var pdf = new PdfDocument(reader))
            {
                for (var i = 1; i <= Math.Min(pdf.GetNumberOfPages(), 15); i++)
                {
                    var strategy = new LocationTextExtractionStrategy();
                    var processor = new SafePdfCanvasProcessor(strategy);
                    processor.ProcessPageContent(pdf.GetPage(i));
                    sb.AppendLine(strategy.GetResultantText());
                }
            }

            return Tools.CleanText(sb.ToString());
        }
        catch (Exception ex)
        {
            return Tools.PrintError(ex, "PDF OKUMA HATASI OLUŞTU:");
        }
    }

    private static async Task<string> EuroToTlConversion(string pdfPathForDate, string foundPrice)
    {
        var euroRate = await EuroRateFetcher.GetEuroRateFromFilePathAsync(pdfPathForDate);
        var tlAmount = Tools.ParseTotalPrice(foundPrice) * euroRate;
        return tlAmount.ToString("N2", new CultureInfo("tr-TR"));
    }
}