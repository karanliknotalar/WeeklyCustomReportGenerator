namespace WeeklyCustomReportGenerator;

using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

public class SafePdfCanvasProcessor(ITextExtractionStrategy strategy) : PdfCanvasProcessor(strategy)
{
    protected override PdfFont GetFont(PdfDictionary fontDict)
    {
        if (fontDict == null)
            return PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

        try
        {
            return base.GetFont(fontDict);
        }
        catch
        {
            return PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
        }
    }
}