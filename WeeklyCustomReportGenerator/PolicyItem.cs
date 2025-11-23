#nullable enable
using System;

namespace WeeklyCustomReportGenerator;

public class PolicyItem
{
    public DateTime Date { get; set; }        // Sıralama yapmak için
    public string FullLine { get; set; } = ""; // Rapora basılacak metnin tamamı (Dosya adı)
    public string Category { get; set; } = ""; // Hangi başlık altında çıkacak
    public bool IsCancel { get; set; }        // İptal mi?
}