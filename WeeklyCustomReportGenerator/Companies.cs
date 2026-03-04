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
            TlTotalPriceRegexPattern =
                @"(?i)(?:İade Edilecek Döviz Karşõlõğõ|İptal Edilen Prim Tutarõ|İade Edilecek Tutar|Ödenecek Döviz Karşõlõğõ \(TL\)|Ödenecek Tutar|Brüt Prim)(?!.*EUR)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "aksigorta.com",
            CompanyName = "AK",
            TlTotalPriceRegexPattern =
                @"(?i)(?:Ödenecek\s+Prim|İptal\s+PRİMİ|Ödenecek[\s\u00A0]*:)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "allianzsigorta|Allianz Sigorta A.Ş. işletenin yapmış olduğu",
            CompanyName = "ALLIANZ",
            TlTotalPriceRegexPattern = @"(?i)(?:peşinat|PEŞİNAT)\s*[:]?\s*[\d/]+\s*([-\d.,]+)"
        },
        new Company
        {
            CompanySearchText = "axasigorta",
            CompanyName = "AXA",
            TlTotalPriceRegexPattern = @"(?i)(?:Ödenecek\s+Prim)\s*[:]?\s*(-?\d[\d.,]*)|(?m)(-?\d[\d.,]*)\s*TL\s*\r?\n\s*BRÜT\s+PRİM\s*:?\s*$"
        },
        new Company
        {
            CompanySearchText = "0069003970664932|9589169656164926",
            CompanyName = "ANKARA",
            TlTotalPriceRegexPattern = @"(?i)(?:ÖDENECEK\s+TUTAR|İADE\s+TUTAR|ÖDENECEK)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "Corpus Yardım",
            CompanyName = "CORPUS",
            TlTotalPriceRegexPattern = @"(?i)(?:BRÜT\s+PRİM)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "0470003295900010|www.hdisigorta.com.tr|SİGORTA ŞİRKETİ ÜNVANI : HDI SİGORTA A.Ş.",
            CompanyName = "HDI",
            TlTotalPriceRegexPattern =
                @"(?i)(?:Brüt Prim \(TL\)|Brüt Prim|Toplam Prim|Ödenecek Poliçe|TOPLAM\s*:|Ödenecek Tutar|Toplam Ödenecek Prim|Prim Toplam Prim :\n|Brüt\s+Prim\s*\(TL\))(?!.*EUR)\s*[:]?\s*(-?\d[\d.,]*)|(?m)(-?\d[\d.,]*)\s*TL\s*\r?\n\s*BRÜT\s+PRİM\s*:?\s*$",
            EurTotalPriceRegexPattern = @"(?i)(?:Toplam Ödenecek Prim)\s*[:]?\s*(-?\d[\d.,]*) EUR",
            EuroConversion = EuroConversionMode.FallbackToEurWhenPathContains,
            EuroConversionPathKeywords = ["Yeşilsigorta", "YSS"],
            // DefinitelyEuroConversionPathKeywords = ["FFL"]
            
        },
        new Company
        {
            CompanySearchText = "hepiyi.com.tr|Hepiyi Çözüm Merkezi",
            CompanyName = "HEPIYI",
            TlTotalPriceRegexPattern = @"(?i)(?:brüt\s*prim|iade\s*edilecek\s*prim)\s*[:]?\s*(-?\d[\d.,]*)",
            EuroConversion = EuroConversionMode.WhenPathContains,
            EuroConversionPathKeywords = ["YSS"]
        },
        new Company
        {
            CompanySearchText = "korusigorta",
            CompanyName = "KORU",
            TlTotalPriceRegexPattern = @"(?i)(?:brüt\s*prim)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "magdeburger.com.tr|0610002277300019",
            CompanyName = "MAGDEBURGER",
            TlTotalPriceRegexPattern =
                @"(?i)(?:brüt\s*prim|ödenecek\s*toplam\s*prim|iade\s*edilecek\s*prim)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "NEOVA KATILIM SİGORTA A.Ş.",
            CompanyName = "NEOVA",
            TlTotalPriceRegexPattern = @"(?i)(?:BRÜT\s*KATKI\s*PRİMİ)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "quicksigorta",
            CompanyName = "QUICK",
            TlTotalPriceRegexPattern =
                @"(?i)(?:Brüt\s*Prim)\s*[:]?\s*(-?\d[\d.,]*)|(?m)^\s*(-?\d[\d.,]*)\s*\r?\n\s*BRÜT\s+PRİM\s*:?\s*$"
        },
        new Company
        {
            CompanySearchText = "raysigorta",
            CompanyName = "RAY",
            TlTotalPriceRegexPattern = @"(?i)(?:brüt\s*prim)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "referanssigorta|REFERANS SİGORTA A.Ş.",
            CompanyName = "REFERANS",
            TlTotalPriceRegexPattern = @"(?i)(?:brüt\s*prim)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText =
                "somposigorta.com.tr|SOMPO SİGORTA TRAFİK|Grup Kodu -\nSOMPO SİGORTA A.Ş.|Sigorta Şirketi Ünvanı : SOMPO SİGORTA A.Ş.|SOMPO SİGORTA A.Ş.\nYILDIZ|SOMPO SİGORTA A.Ş.\nİSTANBUL GRUP",
            CompanyName = "SOMPO",
            TlTotalPriceRegexPattern =
                @"(?i)(?:brüt\s*prim|toplam\s*brüt(?:\s*prim)?|ödenecek\s*prim)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "turkiyesigorta|Sigortalı / Sigorta Ettiren TÜRKİYE SİGORTA AŞ",
            CompanyName = "TÜRKİYE",
            TlTotalPriceRegexPattern =
                @"(?i)(?:toplam\s*brüt\s*prim|kuruş\s*toplam)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "dask.gov.tr",
            CompanyName = "DASK",
            TlTotalPriceRegexPattern = @"(?i)(?:Poliçe\s+Primi|SİGORTA PRİMİ : ₺)\s*[:]?\s*(-?\d[\d.,]*)",
        },
        new Company
        {
            CompanySearchText = "atlasmutuel.com.tr",
            CompanyName = "ATLAS",
            TlTotalPriceRegexPattern = @"(?i)(?:BRÜT\s*PRİM|ÖDENECEK\s*TUTAR)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText =
                "Zurich öncelikli servis ağı hakkında|ZURICH SiGORTA A.Ş.\nACENTESİ",
            CompanyName = "ZURICH",
            TlTotalPriceRegexPattern =
                @"(?is)(?:POLİÇE\s*TOPLAM\s*PRİM\s*BİLGİLERİ.*?TOPLAM\s*:\s*|BRÜT\s*PRİM\(TL\))\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "dogasigorta.com",
            CompanyName = "DOĞA",
            TlTotalPriceRegexPattern = @"(?i)(?:brüt\s*prim)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "unicosigorta.com",
            CompanyName = "UNICO",
            TlTotalPriceRegexPattern = @"(?i)(?:brüt\s*prim)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "turknippon.com|TÜRK NİPPON SİGORTA KASKO ZEYİLNAMESİ",
            CompanyName = "TÜRKNİPPON",
            TlTotalPriceRegexPattern = @"(?i)(?:TOPLAM\s*PRİM)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText =
                "sekersigorta.com|Sigorta Şirketi Ünvanı : ŞEKER SİGORTA A.Ş.|Sigorta Şirketi Ünvanı ŞEKER SİGORTA A.Ş.",
            CompanyName = "SBN",
            TlTotalPriceRegexPattern = @"(?i)(?:Brüt|Toplam\s*Brüt\sPrim)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "aveonglobalsigorta.com|Sigorta Şirketi Ünvanı : Aveon Global Sigorta A.Ş.",
            CompanyName = "AVEON",
            TlTotalPriceRegexPattern = @"(?i)(?:Brüt\sPrim)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText =
                "Sigorta Şirket Unvanı MAPFRE Sigorta A.Ş|mapfre.com.tr|MAPFRE SİGORTA A.Ş.\nSigorta Şirketi Ünvanı",
            CompanyName = "MAPFRE",
            TlTotalPriceRegexPattern =
                @"(?i)(?:Ödenecek\sTutar\s\(TL\)|BRÜT\sPRIM|BRÜT\sPRİM)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "gig.com.tr|0871052362300018|gulfsigorta.com.tr",
            CompanyName = "GULF/GIG",
            TlTotalPriceRegexPattern = @"(?i)(?:BRÜT\sPRİM|Poliçe\sPrimi)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "0144003789300010",
            CompanyName = "GROUPAMA",
            TlTotalPriceRegexPattern = @"(?i)(?:BRÜT\sPRİM)\s*[:]?\s*(-?\d[\d.,]*)"
        },
        new Company
        {
            CompanySearchText = "Point Asistans|5560532445|POİNT ASİSTANS",
            CompanyName = "POINT",
            TlTotalPriceRegexPattern = @"(?i)(?:YOL YARDIM [PAKET[İI]|YOL YARDIM).*?(\d{1,3}(?:,\d{3})*\.\d{2})(?!.*\d)"
        },
        new Company
        {
            CompanySearchText = "ANADOLU ASSİST|0069142738800001",
            CompanyName = "ANADOLU",
            TlTotalPriceRegexPattern = @"(?m)(-?\d[\d.,]*)\s*\r?\n\s*YOL YARDIM"
        },
        new Company
        {
            CompanySearchText = "TEZ ASİSTANS|tezyolyardım",
            CompanyName = "TEZ",
            TlTotalPriceRegexPattern = @"(?i)(?:Toplam Tutar)\s*[:]?\s*(-?\d[\d.,]*)"
        }
    ];
}