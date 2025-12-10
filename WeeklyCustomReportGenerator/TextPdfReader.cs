#nullable enable
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace WeeklyCustomReportGenerator;

public class TextPdfReader
{
    private readonly List<Company> _companies =
    [
        new Company
        {
            CompanySearchText = "anadolusigorta.com|ANADOLU ANONİM TÜRK SİGORTA ŞİRKETİ\nSayfa",
            CompanyName = "ANADOLU",
            // TotalPriceRegexPattern = @"(?:Ödenecek\s+(?:Döviz\s+\S+\s*\(TL\)\s*:\s*|Tutar.*?\s*[:]?\s*(?!.*?TL|.*?EUR))[-]?|İade\s+Edilecek\s+Tutar\s*[:]?\s*|İptal\s+Edilen.*?\s*[:]?\s*)([\d\.\,]+)(?:[^:\r\n]*?TL|.*)?"
            TotalPriceRegexPattern =
                @"(?i)(?:İptal Edilen Prim Tutarõ|İade Edilecek Tutar|Ödenecek Döviz Karşõlõğõ \(TL\)|Ödenecek Tutar|Brüt Prim)(?!.*EUR)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText = "aksigorta.com",
            CompanyName = "AK",
            // TotalPriceRegexPattern = @"(?:Ödenecek[\s\u00A0]*:|ÖDENECEK[\s\u00A0]*PRİM[\s\u00A0]*:|İPTAL[\s\u00A0]*PRİMİ[\s\u00A0]*:)[\s\u00A0\t\n\r]*([-]?[\d\.]+,\d{2})[\s\u00A0\t\n\r]*(?:Prim)?"
            TotalPriceRegexPattern =
                @"(?i)(?:Ödenecek\s+Prim|İptal\s+PRİMİ|Ödenecek[\s\u00A0]*:)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText = "allianzsigorta|Allianz Sigorta A.Ş. işletenin yapmış olduğu",
            CompanyName = "ALLIANZ",
            TotalPriceRegexPattern = @"(?i)(?:peşinat|PEŞİNAT)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText = "axasigorta",
            CompanyName = "AXA",
            TotalPriceRegexPattern = @"(?i)(?:Ödenecek\s+Prim)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText = "0069003970664932|9589169656164926",
            CompanyName = "ANKARA",
            // TotalPriceRegexPattern = @"(?i:ÖDENECEK\s+TUTAR\s*[:]?\s*|İADE\s+TUTAR\s*[:]?\s*|ÖDENECEK\s*[:]?\s*)(-?[\d\.\,]+\.[\d]{2})\s*(?:TL)?"
            TotalPriceRegexPattern = @"(?i)(?:ÖDENECEK\s+TUTAR|İADE\s+TUTAR|ÖDENECEK)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText = "Corpus Yardım",
            CompanyName = "CORPUS",
            // TotalPriceRegexPattern = @"(?i:BRÜT\s+PRİM)\s*[:]?\s*(-?[\d\.\,]+)\s*(?:TL)?"
            TotalPriceRegexPattern = @"(?i)(?:BRÜT\s+PRİM)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText = "0470003295900010|www.hdisigorta.com.tr|SİGORTA ŞİRKETİ ÜNVANI : HDI SİGORTA A.Ş.",
            CompanyName = "HDI",
            TotalPriceRegexPattern =
                @"(?i)(?:(?m)^\s*(?<tutar>-?\d[\d.,]*(?:\.\d+)?)\s*TL?\s*\r?\n\s*BRÜT\s+PRİM\s*:?\s*$|ödenecek\s*poliçe\s*[:]?\s*(?<tutar>-?\d[\d.,]*(?:\.\d+)?)\s*(?:TL)?\b|toplam\s*[:]+\s*(?<tutar>-?\d[\d.,]*(?:,\d+)?)\s*(?:TL)?\b|(?:brüt\s*prim|toplam\s*(?:ödenecek\s*)?prim|ödenecek\s*tutar)\s*[:]?\s*(?<tutar>-?\d[\d.,]*(?:,\d+)?)\s*(?:TL)?\b)"
        },
        new Company
        {
            CompanySearchText = "hepiyi.com.tr|Hepiyi Çözüm Merkezi",
            CompanyName = "HEPIYI",
            TotalPriceRegexPattern = @"(?i)(?:brüt\s*prim|iade\s*edilecek\s*prim)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText = "korusigorta",
            CompanyName = "KORU",
            TotalPriceRegexPattern = @"(?i)(?:brüt\s*prim)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText = "magdeburger.com.tr|0610002277300019",
            CompanyName = "MAGDEBURGER",
            TotalPriceRegexPattern =
                @"(?i)(?:brüt\s*prim|ödenecek\s*toplam\s*prim|iade\s*edilecek\s*prim)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText = "NEOVA KATILIM SİGORTA A.Ş.",
            CompanyName = "NEOVA",
            // TotalPriceRegexPattern = @"(?i)BRÜT\s*KATKI\s*PRİMİ\s*[:]?\s*(-?\d[\d.,]*(?:,\d+)?)"
            TotalPriceRegexPattern = @"(?i)(?:BRÜT\s*KATKI\s*PRİMİ)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText = "quicksigorta",
            CompanyName = "QUICK",
            // TotalPriceRegexPattern = @"(?i)brüt\s*prim\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)\s*(?:TL)?\b|(?m)^\s*(-?\d[\d.,]*(?:\.\d+)?)\s*\r?\n\s*BRÜT\s+PRİM\s*:?\s*$"
            TotalPriceRegexPattern =
                @"(?i)(?:Brüt\s*Prim)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)|(?m)^\s*(-?\d[\d.,]*(?:\.\d+)?)\s*\r?\n\s*BRÜT\s+PRİM\s*:?\s*$"
        },
        new Company
        {
            CompanySearchText = "raysigorta",
            CompanyName = "RAY",
            TotalPriceRegexPattern = @"(?i)(?:brüt\s*prim)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText = "referanssigorta|REFERANS SİGORTA A.Ş.",
            CompanyName = "REFERANS",
            TotalPriceRegexPattern = @"(?i)(?:brüt\s*prim)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText =
                "somposigorta.com.tr|SOMPO SİGORTA TRAFİK|Grup Kodu -\nSOMPO SİGORTA A.Ş.|Sigorta Şirketi Ünvanı : SOMPO SİGORTA A.Ş.|SOMPO SİGORTA A.Ş.\nYILDIZ",
            CompanyName = "SOMPO",
            TotalPriceRegexPattern =
                @"(?i)(?:brüt\s*prim|toplam\s*brüt(?:\s*prim)?|ödenecek\s*prim)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText = "turkiyesigorta|Sigortalı / Sigorta Ettiren TÜRKİYE SİGORTA AŞ",
            CompanyName = "TÜRKİYE",
            TotalPriceRegexPattern =
                @"(?i)(?:toplam\s*brüt\s*prim|kuruş\s*toplam)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText = "dask.gov.tr",
            CompanyName = "DASK",
            // TotalPriceRegexPattern = @"(?i:Poliçe\s+Primi)\s*[:]?\s*([\d\.\,]+)\s*(?:TL)?"
            TotalPriceRegexPattern = @"(?i)(?:Poliçe\s+Primi|SİGORTA PRİMİ : ₺)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)",
        },
        new Company
        {
            CompanySearchText = "atlasmutuel.com.tr",
            CompanyName = "ATLAS",
            TotalPriceRegexPattern = @"(?i)(?:BRÜT\s*PRİM|ÖDENECEK\s*TUTAR)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText =
                "Zurich öncelikli servis ağı hakkında|POLİÇE BİLGİLERİ: Z.K.YOLU TAŞIMACILIK M.S.POLİÇE BİLGİLERİ:",
            CompanyName = "ZURICH",
            TotalPriceRegexPattern =
                @"(?is)(?:POLİÇE\s*TOPLAM\s*PRİM\s*BİLGİLERİ.*?TOPLAM\s*:\s*|BRÜT\s*PRİM\(TL\)\s*:\s*)([\d\.\,]+)"
        },
        new Company
        {
            CompanySearchText = "dogasigorta.com",
            CompanyName = "DOĞA",
            TotalPriceRegexPattern = @"(?i)(?:brüt\s*prim)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText = "unicosigorta.com",
            CompanyName = "UNICO",
            TotalPriceRegexPattern = @"(?i)(?:brüt\s*prim)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText = "turknippon.com|TÜRK NİPPON SİGORTA KASKO ZEYİLNAMESİ",
            CompanyName = "TÜRKNİPPON",
            TotalPriceRegexPattern = @"(?i)(?:TOPLAM\s*PRİM)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText =
                "sekersigorta.com|Sigorta Şirketi Ünvanı : ŞEKER SİGORTA A.Ş.|Sigorta Şirketi Ünvanı ŞEKER SİGORTA A.Ş.",
            CompanyName = "SBN",
            TotalPriceRegexPattern = @"(?i)(?:Brüt|Toplam\s*Brüt\sPrim)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText = "aveonglobalsigorta.com|Sigorta Şirketi Ünvanı : Aveon Global Sigorta A.Ş.",
            CompanyName = "AVEON",
            TotalPriceRegexPattern = @"(?i)(?:Brüt\sPrim)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText =
                "Sigorta Şirket Unvanı MAPFRE Sigorta A.Ş|mapfre.com.tr|MAPFRE SİGORTA A.Ş.\nSigorta Şirketi Ünvanı",
            CompanyName = "MAPFRE",
            TotalPriceRegexPattern =
                @"(?i)(?:Ödenecek\sTutar\s\(TL\)|BRÜT\sPRIM|BRÜT\sPRİM)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText = "gig.com.tr|0871052362300018|gulfsigorta.com.tr",
            CompanyName = "GULF/GIG",
            TotalPriceRegexPattern = @"(?i)(?:BRÜT\sPRİM|Poliçe\sPrimi)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        },
        new Company
        {
            CompanySearchText = "0144003789300010",
            CompanyName = "GROUPAMA",
            TotalPriceRegexPattern = @"(?i)(?:BRÜT\sPRİM)\s*[:]?\s*(-?\d[\d.,]*(?:\.\d+)?)"
        }
    ];

    public PdfReadResult ProcessPdf(string pdfPath)
    {
        var pdfReadResult = new PdfReadResult();

        try
        {
            var pdfContent = ReadPdf_IText7_Advanced(pdfPath);
            Tools.AppendToLogFile(pdfContent, pdfPath, new Company());

            foreach (var company in _companies)
            {
                if (Tools.SearchCompanyText(pdfContent, company.CompanySearchText))
                {
                    pdfReadResult.FoundCompany = company.CompanyName;
                    Console.WriteLine(@"---------------------------------------------------------");
                    Console.WriteLine($@"İşlenen Dosya: {pdfPath}");
                    Console.WriteLine($@"FİRMA ADI BULUNDU: {company.CompanyName}");
                    Console.WriteLine($@"Aramada Kullanılan Metin: {company.CompanySearchText}");
                    Console.WriteLine(@"---------------------------------------------------------");
                    Console.WriteLine("");

                    // Tools.AppendToLogFile(pdfContent, pdfPath, company);

                    var match = Regex.Match(pdfContent, company.TotalPriceRegexPattern, RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        pdfReadResult.FoundTotalPrice = !string.IsNullOrEmpty(match.Groups[1].Value)
                            ? match.Groups[1].Value
                            : match.Groups[2].Value;
                        pdfReadResult.IsSuccess = true;
                        
                    }

                    break;
                }
            }
        }
        catch (Exception ex)
        {
            pdfReadResult.IsSuccess = false;
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
                for (var i = 1; i <= pdf.GetNumberOfPages(); i++)
                {
                    var strategy = new LocationTextExtractionStrategy();

                    var text = PdfTextExtractor.GetTextFromPage(pdf.GetPage(i), strategy);

                    sb.AppendLine(text);
                }
            }

            return Tools.CleanText(sb.ToString());
        }
        catch (Exception ex)
        {
            var msg = new StringBuilder();
            msg.AppendLine("PDF OKUMA HATASI OLUŞTU:");
            msg.AppendLine("---------------------------------------");
            msg.AppendLine("Hata Tipi: " + ex.GetType().FullName);
            msg.AppendLine("Mesaj: " + ex.Message);
            msg.AppendLine("---------------------------------------");

            if (ex.InnerException != null)
            {
                msg.AppendLine("InnerException Tipi: " + ex.InnerException.GetType().FullName);
                msg.AppendLine("InnerException Mesajı: " + ex.InnerException.Message);
                msg.AppendLine("---------------------------------------");
            }

            msg.AppendLine("StackTrace:");
            msg.AppendLine(ex.StackTrace);

            Console.WriteLine(msg.ToString());
            return msg.ToString();
        }
    }
}