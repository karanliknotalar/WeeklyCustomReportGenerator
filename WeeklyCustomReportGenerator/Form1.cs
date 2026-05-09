#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MiniExcelLibs;

namespace WeeklyCustomReportGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static string DriveDirectory = string.Empty;
        private string _saveDirectory = string.Empty;
        private string _year = DateTime.Now.Year.ToString();
        private string _month = string.Empty;
        public static List<string> CustomerGalleryList = [];
        private List<PolicyItem> _policyItems = [];

        private void Form1_Load(object sender, EventArgs e)
        {
            listRegexPattern.Items.AddRange(Tools.GenerateYearlyWeeklyRegexPatterns().AsEnumerable().Reverse()
                .ToArray<object>());
            DriveDirectory = txtDriveDir.Text;
            _saveDirectory = txtSaveDir.Text;
            CustomerGalleryList = txtGalleryCustomerList.Lines.ToList();
            var logFilePath = Path.Combine("C:\\", "pdf_processing_log.txt");
            var logPdfUndefinedFilePath = Path.Combine("C:\\", "pdf_undefined_log.txt");
            if (File.Exists(logFilePath))
                File.Delete(logFilePath);
            if (File.Exists(logPdfUndefinedFilePath))
                File.Delete(logPdfUndefinedFilePath);

            cBoxYear.Items.AddRange(Enumerable.Range(DateTime.Now.Year - 2, 3).Select(y => y.ToString())
                .ToArray<object>());
        }

        private void listRegexPattern_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetDirList();
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            RunProcess();
        }

        private void GetDirList(string regexPattern = "")
        {
            if (listRegexPattern.SelectedItem == null && string.IsNullOrEmpty(regexPattern)) return;
            
            EnableControls(false);

            var selectedPattern = listRegexPattern.SelectedItem?.ToString() ?? regexPattern;

            var targetDirectory = txtDriveDir.Text;

            try
            {
                var regex = new Regex(selectedPattern, RegexOptions.Compiled);

                string[] excludeKeywords =
                    ["A Belgeler", "DIŞ_KAYNAK_HESAPLAR_LİSTESİ", "LİSTELER", "PORTFÖY", "appsheet"];

                txtInput.Lines = Tools.SearchFiles(targetDirectory, regex)
                    .Where(path => !excludeKeywords.Any(k => path.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToArray();

                if (txtInput.Lines.Length <= 0)
                {
                    EnableControls(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async void RunProcess()
        {
            try
            {
                var files = txtInput.Text.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);
                var productOrder = txtProducts.Lines;
                var builder = new WeeklyReportBuilder(productOrder);
                _policyItems = await builder.ParseFiles(files);

                txtOutput.Text = builder.BuildReport(_policyItems);
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"Kritik Hata: {ex.Message}");
            }
            finally
            {
                EnableControls(true);
            }
        }

        private void txtDir_TextChanged(object sender, EventArgs e)
        {
            DriveDirectory = txtDriveDir.Text;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!_policyItems.Any()) return;


            var activeItems = _policyItems.Where(x => !x.IsCancel).ToList();
            var cancelledItems = _policyItems.Where(x => x.IsCancel).ToList();

            var activeItemSheet = Tools.GenerateCategoryCompanyDetails(activeItems);
            var cancelledItemSheet = Tools.GenerateCategoryCompanyDetails(cancelledItems);
            var monthStr = !string.IsNullOrEmpty(_month) ? $"{_month}.Ay_" : "";

            MiniExcel.SaveAs(Path.Combine(_saveDirectory, $"{monthStr}{_year}_Branş_Şirket_Üretim_Özeti.xlsx"),
                new Dictionary<string, object>
                {
                    { "ÜRETİMLER", activeItemSheet },
                    { "İPTALLER", cancelledItemSheet }
                }, overwriteFile: true);
            File.WriteAllText(Path.Combine(_saveDirectory, $"{monthStr}{_year}_İstatistik.txt"), txtOutput.Text);

            CustomerAnalysisExcelWriter.WriteCustomerAnalysisSheet(
                filePath: Path.Combine(_saveDirectory, $"{monthStr}{_year}_Müşteri_Branş_Üretim_Özeti.xlsx"),
                sheetName: "Müşteri Analizi",
                items: activeItems
            );

            MessageBox.Show(@"Kaydedildi");
        }

        private void listRegexPattern_Click(object sender, EventArgs e)
        {
            // if (listRegexPattern.SelectedItem == null) return;
            // var selectedItem = listRegexPattern.SelectedItem.ToString();
            //
            // var match = Regex.Match(selectedItem, @"\.(\d{4})");
            // if (match.Success) _year = match.Groups[1].Value;
            //
            // for (var i = 0; i < 5; i++)
            // {
            //     try
            //     {
            //         Clipboard.SetText(selectedItem);
            //         break;
            //     }
            //     catch (Exception)
            //     {
            //         System.Threading.Thread.Sleep(100);
            //     }
            // }
        }

        private void lblSaveDir_Click(object sender, EventArgs e)
        {
            var selectedDir = Tools.SelectedDir();
            if (string.IsNullOrEmpty(selectedDir)) return;
            txtSaveDir.Text = selectedDir;
            _saveDirectory = selectedDir;
        }


        private void lblPdfDir_Click(object sender, EventArgs e)
        {
            var selectedDir = Tools.SelectedDir();
            if (string.IsNullOrEmpty(selectedDir)) return;
            txtDriveDir.Text = selectedDir;
            DriveDirectory = selectedDir;
        }

        private void txtSaveDir_TextChanged(object sender, EventArgs e)
        {
            _saveDirectory = txtSaveDir.Text;
        }

        private void cBoxYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            cBoxMonth.Enabled = true;
        }

        private void cBoxMonth_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var year = cBoxYear.SelectedItem.ToString();
            var selectedMonth = cBoxMonth.SelectedItem.ToString();
            var month = selectedMonth == "all" ? $"\\d{{2}}\\" : selectedMonth;
            listRegexPattern.SelectedIndex = -1;

            _year = year;
            _month = selectedMonth == "all" ? string.Empty : selectedMonth;

            var pattern =
                @$"(?i)^(?!.*\b(?:makbuz|acs|eng|hayat|yeşil)\b)(?:(?!.*\bzeyil|zeyili\b)|(?=.*\bİptal\b)).*(\d{{2}}\.{month}.{year}).*\.pdf$";

            GetDirList(pattern);
        }

        private void EnableControls(bool state)
        {
            Control[] controls =
                [listRegexPattern, cBoxYear, cBoxMonth, btnSave, txtInput, txtProducts, txtGalleryCustomerList];
            foreach (var ctrl in controls)
            {
                ctrl.Enabled = state;
            }
        }

        private void cBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void listRegexPattern_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            e.DrawBackground();
            var realStr = listRegexPattern.Items[e.Index].ToString();


            var shortedStr = realStr.Substring(89, realStr.Length - 97);

            var brush = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) 
                ? Brushes.White 
                : Brushes.Black;

            e.Graphics.DrawString(shortedStr, e.Font, brush, e.Bounds);
            e.DrawFocusRectangle();
        }
    }
}