#nullable enable
using System;
using System.Collections.Generic;
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

        public static string DriveDirectory = "";
        private string _saveDirectory = "";
        private string _year = "";
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
        }

        private void listRegexPattern_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetDirList();
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            RunProcess();
        }

        private void GetDirList()
        {
            if (listRegexPattern.SelectedItem == null) return;

            var selectedPattern = listRegexPattern.SelectedItem.ToString();
            var targetDirectory = txtDriveDir.Text;

            try
            {
                var regex = new Regex(selectedPattern, RegexOptions.Compiled);

                txtInput.Lines = Tools.SearchFiles(targetDirectory, regex).ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void RunProcess()
        {
            try
            {
                var files = txtInput.Text.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);
                var productOrder = txtProducts.Lines;
                var builder = new WeeklyReportBuilder(productOrder);
                _policyItems = builder.ParseFiles(files);

                txtOutput.Text = builder.BuildReport(_policyItems);
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"Kritik Hata: {ex.Message}");
            }
        }

        private void listRegexPattern_KeyDown(object sender, KeyEventArgs e)
        {
            var selectedItem = listRegexPattern.SelectedItem.ToString();
            var match = Regex.Match(selectedItem, @"\.(\d{4})");

            MessageBox.Show(match.Groups[1].Value);

            if (match.Success)
            {
                _year = match.Groups[1].Value;
            }

            if (!e.Control || e.KeyCode != Keys.C) return;
            if (listRegexPattern.SelectedItem == null) return;
            Clipboard.SetText(selectedItem);
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

            MiniExcel.SaveAs(Path.Combine(_saveDirectory, $"Brans_Sirket_Uretim_Ozeti_{_year}.xlsx"),
                new Dictionary<string, object>
                {
                    { "ÜRETİMLER", activeItemSheet },
                    { "İPTALLER", cancelledItemSheet }
                }, overwriteFile: true);
            File.WriteAllText(Path.Combine(_saveDirectory, $"{_year} İstatistik.txt"), txtOutput.Text);
            MessageBox.Show(@"Kaydedildi");
        }

        private void listRegexPattern_Click(object sender, EventArgs e)
        {
            if (listRegexPattern.SelectedItem == null) return;
            var selectedItem = listRegexPattern.SelectedItem.ToString();
            var match = Regex.Match(selectedItem, @"\.(\d{4})");
            if (match.Success) _year = match.Groups[1].Value;
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
    }
}