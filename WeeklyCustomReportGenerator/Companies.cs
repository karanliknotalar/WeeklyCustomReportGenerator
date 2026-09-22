using System;
using System.Collections.Generic;

namespace WeeklyCustomReportGenerator;

public partial class TextPdfReader
{
    public TextPdfReader()
    {
        _companies = ExternalData.LoadCompanies();
        SortCompaniesByName();
    }

    // Şirket listesi artık burada gömülü değil; ExternalData.LoadCompanies()
    // aracılığıyla dış dosyadan (veya o dosya yoksa uygulamaya gömülü
    // varsayılandan) okunuyor. Detaylar için ExternalData.cs dosyasına bakınız.
    private readonly List<Company> _companies;

    private void SortCompaniesByName()
    {
        _companies.Sort((a, b) => string.Compare(a.CompanyName, b.CompanyName, StringComparison.OrdinalIgnoreCase));
    }
}
