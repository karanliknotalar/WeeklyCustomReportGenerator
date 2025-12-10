#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WeeklyCustomReportGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static string DriveDirectory = "";
        public static List<string> CustomerGalleryList = [];

        private void Form1_Load(object sender, EventArgs e)
        {
            listRegexPattern.Items.AddRange(Tools.GenerateYearlyWeeklyRegexPatterns().AsEnumerable().Reverse()
                .ToArray<object>());
            DriveDirectory = txtDir.Text;
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
            var targetDirectory = txtDir.Text;

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
                var items = builder.ParseFiles(files);

                txtOutput.Text = builder.BuildReport(items);
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"Kritik Hata: {ex.Message}");
            }
        }

        private void listRegexPattern_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Control || e.KeyCode != Keys.C) return;
            if (listRegexPattern.SelectedItem == null) return;
            Clipboard.SetText(listRegexPattern.SelectedItem.ToString());
        }

        private void txtDir_TextChanged(object sender, EventArgs e)
        {
            DriveDirectory = txtDir.Text;
        }
    }
}