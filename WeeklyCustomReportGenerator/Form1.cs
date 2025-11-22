#nullable enable
using System;
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
            txtRegex.Text = WeeklyReportBuilder.GetDynamicRegex();
        }
    }
}