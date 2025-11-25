#nullable enable
using System;
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


        private void Form1_Load(object sender, EventArgs e)
        {
            listRegexPattern.Items.AddRange(Tools.GenerateYearlyWeeklyRegexPatterns().AsEnumerable().Reverse()
                .ToArray<object>());
        }

        private void listRegexPattern_Click(object sender, EventArgs e)
        {
            RunProcess();
        }

        private void listRegexPattern_SelectedIndexChanged(object sender, EventArgs e)
        {
            RunProcess();
        }

        private void RunProcess()
        {
            if (listRegexPattern.SelectedItem == null) return;

            var selectedPattern = listRegexPattern.SelectedItem.ToString();

            var targetDirectory = txtDir.Text;

            try
            {
                var regex = new Regex(selectedPattern, RegexOptions.Compiled);

                txtInput.Lines = Tools.SearchFiles(targetDirectory, regex).ToArray();

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
    }
}