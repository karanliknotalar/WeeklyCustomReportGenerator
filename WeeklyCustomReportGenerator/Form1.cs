#nullable enable
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WeeklyCustomReportGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            var files = txtInput.Text.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

            var productOrder = txtProducts.Lines;

            var builder = new WeeklyReportBuilder(productOrder);
            var items = builder.ParseFiles(files);

            txtOutput.Text = builder.BuildReport(items);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // txtRegex.Text = WeeklyReportBuilder.GetDynamicRegex();
            txtRegexRich.Lines = WeeklyReportBuilder.GenerateYearlyWeeklyRegexPatterns().AsEnumerable().Reverse().ToArray();;
            ColorLines();

        }
        
        private void ColorLines()
        {
            var start = 0;

            for (var i = 0; i < txtRegexRich.Lines.Length; i++)
            {
                var line = txtRegexRich.Lines[i];
                var length = line.Length;

                txtRegexRich.Select(start, length);

                txtRegexRich.SelectionBackColor = (i % 2 == 0)
                    ? Color.LightYellow
                    : Color.LightGray;

                start += length + 1;
            }
        }
    }
}