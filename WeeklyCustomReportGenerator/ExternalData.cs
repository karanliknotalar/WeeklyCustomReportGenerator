#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace WeeklyCustomReportGenerator;

/// <summary>
/// Şirket listesi, ürün listesi ve galeri müşteri listesi artık koda gömülü değil;
/// bu sınıf sayesinde diskteki dış dosyalardan okunuyor. Amaç: listeye yeni bir
/// kayıt eklemek için programı yeniden derlemeye gerek kalmaması.
///
/// Çalışma mantığı:
/// - Program her açıldığında / listeler her okunduğunda önce <see cref="DataDirectory"/>
///   altında ilgili dosya var mı diye bakılır.
/// - Dosya varsa, veriler doğrudan o dosyadan okunur (elle düzenlediğin haliyle).
/// - Dosya yoksa, programa gömülü (Embedded Resource) varsayılan içerik önce o dizine
///   dosya olarak yazılır, sonra oradan okunur. Yani dizin silinse veya dosyalar
///   silinse bile program kendini onarıp varsayılanlarla çalışmaya devam eder.
///
/// Listeye kalıcı bir ekleme/değişiklik yapmak için: DataDirectory içindeki ilgili
/// dosyayı (sirketler.json / urunler.txt / galeri_musterileri.txt) düzenlemen yeterli;
/// programı yeniden derlemene gerek yok, sadece bir sonraki çalıştırmada (şirket listesi
/// için bir sonraki rapor oluşturmada) değişiklikler otomatik olarak devreye girer.
/// </summary>
public static class ExternalData
{
    // ------------------------------------------------------------------
    // Dış dosyaların bulunacağı dizin. İstediğin zaman burayı değiştirip
    // programı yeniden derleyebilirsin; program her çalıştığında bu dizini
    // (yoksa) oluşturur ve içini varsayılan dosyalarla doldurur.
    // ------------------------------------------------------------------
    private static readonly string DataDirectory =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WeeklyCustomReportGenerator_Data");

    private const string CompaniesFileName = "sirketler.json";
    private const string ProductsFileName = "urunler.txt";
    private const string GalleryCustomersFileName = "galeri_musterileri.txt";

    private const string CompaniesResourceName =
        "WeeklyCustomReportGenerator.DefaultData.sirketler.default.json";

    private const string ProductsResourceName =
        "WeeklyCustomReportGenerator.DefaultData.urunler.default.txt";

    private const string GalleryCustomersResourceName =
        "WeeklyCustomReportGenerator.DefaultData.galeri_musterileri.default.txt";

    private static string CompaniesPath => Path.Combine(DataDirectory, CompaniesFileName);
    private static string ProductsPath => Path.Combine(DataDirectory, ProductsFileName);
    private static string GalleryCustomersPath => Path.Combine(DataDirectory, GalleryCustomersFileName);

    /// <summary>
    /// Dizinin ve üç dosyanın var olduğundan emin olur; eksik olanları uygulamaya
    /// gömülü varsayılan içerikle diske yazar. Var olan dosyalara asla dokunmaz.
    /// </summary>
    public static void EnsureFilesExist()
    {
        Directory.CreateDirectory(DataDirectory);

        ExtractIfMissing(CompaniesPath, CompaniesResourceName);
        ExtractIfMissing(ProductsPath, ProductsResourceName);
        ExtractIfMissing(GalleryCustomersPath, GalleryCustomersResourceName);
    }

    public static List<Company> LoadCompanies()
    {
        EnsureFilesExist();

        try
        {
            var json = File.ReadAllText(CompaniesPath, Encoding.UTF8);
            var companies = JsonConvert.DeserializeObject<List<Company>>(json);
            if (companies is { Count: > 0 }) return companies;
        }
        catch (Exception ex)
        {
            Tools.PrintError(ex,
                $"Şirket listesi okunamadı ({CompaniesPath}). Gömülü varsayılan liste kullanılacak:");
        }

        // Dosya bozuksa, boşsa ya da okunamıyorsa gömülü varsayılana geri dön.
        var fallbackJson = ReadEmbeddedResource(CompaniesResourceName);
        return JsonConvert.DeserializeObject<List<Company>>(fallbackJson) ?? [];
    }

    public static List<string> LoadProducts() =>
        LoadLines(ProductsPath, ProductsResourceName);

    public static List<string> LoadGalleryCustomers() =>
        LoadLines(GalleryCustomersPath, GalleryCustomersResourceName);

    private static List<string> LoadLines(string path, string fallbackResourceName)
    {
        EnsureFilesExist();

        try
        {
            var lines = File.ReadAllLines(path, Encoding.UTF8)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();
            if (lines.Count > 0) return lines;
        }
        catch (Exception ex)
        {
            Tools.PrintError(ex, $"Liste dosyası okunamadı ({path}). Gömülü varsayılan liste kullanılacak:");
        }

        var fallback = ReadEmbeddedResource(fallbackResourceName);
        return fallback
            .Split(["\r\n", "\n"], StringSplitOptions.None)
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToList();
    }

    private static void ExtractIfMissing(string targetPath, string embeddedResourceName)
    {
        if (File.Exists(targetPath)) return;

        var content = ReadEmbeddedResource(embeddedResourceName);
        File.WriteAllText(targetPath, content, new UTF8Encoding(false));
    }

    private static string ReadEmbeddedResource(string fullResourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(fullResourceName);
        if (stream == null)
        {
            throw new InvalidOperationException(
                $"Gömülü kaynak bulunamadı: {fullResourceName}. " +
                "DefaultData klasöründeki dosyanın .csproj içinde EmbeddedResource " +
                "olarak (doğru LogicalName ile) eklendiğinden emin ol.");
        }

        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }
}
