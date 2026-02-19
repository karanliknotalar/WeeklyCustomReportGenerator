using System.Collections.Generic;

namespace WeeklyCustomReportGenerator;

public partial class TextPdfReader
{
    private readonly List<Company> _companies =
    [
        new Company
        {
            CompanySearchText = "anadolusigorta.com|ANADOLU ANONİM TÜRK SİGORTA ŞİRKETİ\nSayfa",
            CompanyName = "ANADOLU",
            TotalPriceRegexPattern =
                @"(?i)(?:İptal Edilen Prim Tutarõ|İade Edilecek Tutar|Ödenecek Döviz Karşõlõğõ \(TL\)|Ödenecek Tutar|Brüt Prim)(?!.*EUR)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "aksigorta.com",
            CompanyName = "AK",
            TotalPriceRegexPattern =
                @"(?i)(?:Ödenecek\s+Prim|İptal\s+PRİMİ|Ödenecek[\s\u00A0]*:)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "allianzsigorta|Allianz Sigorta A.Ş. işletenin yapmış olduğu",
            CompanyName = "ALLIANZ",
            TotalPriceRegexPattern = @"(?i)(?:peşinat|PEŞİNAT)\s*[:]?\s*[\d/]+\s*([-\d.,]+)"
        },
        new Company
        {
            CompanySearchText = "axasigorta",
            CompanyName = "AXA",
            TotalPriceRegexPattern = @"(?i)(?:Ödenecek\s+Prim)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "0069003970664932|9589169656164926",
            CompanyName = "ANKARA",
            TotalPriceRegexPattern = @"(?i)(?:ÖDENECEK\s+TUTAR|İADE\s+TUTAR|ÖDENECEK)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "Corpus Yardım",
            CompanyName = "CORPUS",
            TotalPriceRegexPattern = @"(?i)(?:BRÜT\s+PRİM)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "0470003295900010|www.hdisigorta.com.tr|SİGORTA ŞİRKETİ ÜNVANI : HDI SİGORTA A.Ş.",
            CompanyName = "HDI",
            TotalPriceRegexPattern =
                @"(?i)(?:Toplam Prim|Ödenecek Poliçe|TOPLAM\s*:|Ödenecek Tutar|Toplam Ödenecek Prim|Brüt Prim|Prim Toplam Prim :\n)(?!.*EUR)\s*[:]?\s*(-?\d[\d.,]*)|(?m)(-?\d[\d.,]*)\s*TL\s*\r?\n\s*BRÜT\s+PRİM\s*:?\s*$",
            EurPriceRegexPattern = @"(?i)(?:Toplam Ödenecek Prim)\s*[:]?\s*(-?\d[\d.,]*) EUR",
            EuroConversion = EuroConversionMode.FallbackToEurWhenPathContains,
            EuroConversionPathKeywords = ["Yeşilsigorta", "YSS"]
        },
        new Company
        {
            CompanySearchText = "hepiyi.com.tr|Hepiyi Çözüm Merkezi",
            CompanyName = "HEPIYI",
            TotalPriceRegexPattern = @"(?i)(?:brüt\s*prim|iade\s*edilecek\s*prim)\s*[:]?\s*(-?\d[\d.,]*)",
            EuroConversion = EuroConversionMode.WhenPathContains,
            EuroConversionPathKeywords = ["YSS"]
        },
        new Company
        {
            CompanySearchText = "korusigorta",
            CompanyName = "KORU",
            TotalPriceRegexPattern = @"(?i)(?:brüt\s*prim)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "magdeburger.com.tr|0610002277300019",
            CompanyName = "MAGDEBURGER",
            TotalPriceRegexPattern =
                @"(?i)(?:brüt\s*prim|ödenecek\s*toplam\s*prim|iade\s*edilecek\s*prim)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "NEOVA KATILIM SİGORTA A.Ş.",
            CompanyName = "NEOVA",
            TotalPriceRegexPattern = @"(?i)(?:BRÜT\s*KATKI\s*PRİMİ)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "quicksigorta",
            CompanyName = "QUICK",
            TotalPriceRegexPattern =
                @"(?i)(?:Brüt\s*Prim)\s*[:]?\s*(-?\d[\d.,]*)|(?m)^\s*(-?\d[\d.,]*)\s*\r?\n\s*BRÜT\s+PRİM\s*:?\s*$"
        },
        new Company
        {
            CompanySearchText = "raysigorta",
            CompanyName = "RAY",
            TotalPriceRegexPattern = @"(?i)(?:brüt\s*prim)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "referanssigorta|REFERANS SİGORTA A.Ş.",
            CompanyName = "REFERANS",
            TotalPriceRegexPattern = @"(?i)(?:brüt\s*prim)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText =
                "somposigorta.com.tr|SOMPO SİGORTA TRAFİK|Grup Kodu -\nSOMPO SİGORTA A.Ş.|Sigorta Şirketi Ünvanı : SOMPO SİGORTA A.Ş.|SOMPO SİGORTA A.Ş.\nYILDIZ|SOMPO SİGORTA A.Ş.\nİSTANBUL GRUP",
            CompanyName = "SOMPO",
            TotalPriceRegexPattern =
                @"(?i)(?:brüt\s*prim|toplam\s*brüt(?:\s*prim)?|ödenecek\s*prim)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "turkiyesigorta|Sigortalı / Sigorta Ettiren TÜRKİYE SİGORTA AŞ",
            CompanyName = "TÜRKİYE",
            TotalPriceRegexPattern =
                @"(?i)(?:toplam\s*brüt\s*prim|kuruş\s*toplam)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "dask.gov.tr",
            CompanyName = "DASK",
            TotalPriceRegexPattern = @"(?i)(?:Poliçe\s+Primi|SİGORTA PRİMİ : ₺)\s*[:]?\s*(-?\d[\d.,]*)",
        },
        new Company
        {
            CompanySearchText = "atlasmutuel.com.tr",
            CompanyName = "ATLAS",
            TotalPriceRegexPattern = @"(?i)(?:BRÜT\s*PRİM|ÖDENECEK\s*TUTAR)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText =
                "Zurich öncelikli servis ağı hakkında|ZURICH SiGORTA A.Ş.\nACENTESİ",
            CompanyName = "ZURICH",
            TotalPriceRegexPattern =
                @"(?is)(?:POLİÇE\s*TOPLAM\s*PRİM\s*BİLGİLERİ.*?TOPLAM\s*:\s*|BRÜT\s*PRİM\(TL\))\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "dogasigorta.com",
            CompanyName = "DOĞA",
            TotalPriceRegexPattern = @"(?i)(?:brüt\s*prim)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "unicosigorta.com",
            CompanyName = "UNICO",
            TotalPriceRegexPattern = @"(?i)(?:brüt\s*prim)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "turknippon.com|TÜRK NİPPON SİGORTA KASKO ZEYİLNAMESİ",
            CompanyName = "TÜRKNİPPON",
            TotalPriceRegexPattern = @"(?i)(?:TOPLAM\s*PRİM)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText =
                "sekersigorta.com|Sigorta Şirketi Ünvanı : ŞEKER SİGORTA A.Ş.|Sigorta Şirketi Ünvanı ŞEKER SİGORTA A.Ş.",
            CompanyName = "SBN",
            TotalPriceRegexPattern = @"(?i)(?:Brüt|Toplam\s*Brüt\sPrim)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "aveonglobalsigorta.com|Sigorta Şirketi Ünvanı : Aveon Global Sigorta A.Ş.",
            CompanyName = "AVEON",
            TotalPriceRegexPattern = @"(?i)(?:Brüt\sPrim)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText =
                "Sigorta Şirket Unvanı MAPFRE Sigorta A.Ş|mapfre.com.tr|MAPFRE SİGORTA A.Ş.\nSigorta Şirketi Ünvanı",
            CompanyName = "MAPFRE",
            TotalPriceRegexPattern =
                @"(?i)(?:Ödenecek\sTutar\s\(TL\)|BRÜT\sPRIM|BRÜT\sPRİM)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "gig.com.tr|0871052362300018|gulfsigorta.com.tr",
            CompanyName = "GULF/GIG",
            TotalPriceRegexPattern = @"(?i)(?:BRÜT\sPRİM|Poliçe\sPrimi)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "0144003789300010",
            CompanyName = "GROUPAMA",
            TotalPriceRegexPattern = @"(?i)(?:BRÜT\sPRİM)\s*[:]?\s*(-?\d[\d.,]*)"
        }
    ];
}