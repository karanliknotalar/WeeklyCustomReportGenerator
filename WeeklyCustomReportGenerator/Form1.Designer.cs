namespace WeeklyCustomReportGenerator
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtInput = new System.Windows.Forms.RichTextBox();
            this.txtOutput = new System.Windows.Forms.RichTextBox();
            this.txtDir = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listRegexPattern = new System.Windows.Forms.ListBox();
            this.txtProducts = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.48035F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.375546F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.18253F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.86229F));
            this.tableLayoutPanel1.Controls.Add(this.txtInput, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtOutput, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtDir, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.label1, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.listRegexPattern, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtProducts, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23.79679F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 76.20321F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 290F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1155, 704);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // txtInput
            // 
            this.txtInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInput.Location = new System.Drawing.Point(8, 8);
            this.txtInput.Name = "txtInput";
            this.tableLayoutPanel1.SetRowSpan(this.txtInput, 2);
            this.txtInput.Size = new System.Drawing.Size(343, 377);
            this.txtInput.TabIndex = 9;
            this.txtInput.Text = "";
            this.txtInput.TextChanged += new System.EventHandler(this.txtInput_TextChanged);
            // 
            // txtOutput
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txtOutput, 2);
            this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutput.Location = new System.Drawing.Point(430, 8);
            this.txtOutput.Name = "txtOutput";
            this.tableLayoutPanel1.SetRowSpan(this.txtOutput, 3);
            this.txtOutput.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(717, 667);
            this.txtOutput.TabIndex = 12;
            this.txtOutput.Text = "";
            // 
            // txtDir
            // 
            this.txtDir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDir.Location = new System.Drawing.Point(558, 681);
            this.txtDir.Name = "txtDir";
            this.txtDir.Size = new System.Drawing.Size(589, 20);
            this.txtDir.TabIndex = 13;
            this.txtDir.Text = "C:\\Users\\ASUS\\Drive’ım";
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(430, 678);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 21);
            this.label1.TabIndex = 14;
            this.label1.Text = "Dizin Konumu:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // listRegexPattern
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.listRegexPattern, 2);
            this.listRegexPattern.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listRegexPattern.FormattingEnabled = true;
            this.listRegexPattern.HorizontalExtent = 800;
            this.listRegexPattern.HorizontalScrollbar = true;
            this.listRegexPattern.Location = new System.Drawing.Point(8, 391);
            this.listRegexPattern.Name = "listRegexPattern";
            this.tableLayoutPanel1.SetRowSpan(this.listRegexPattern, 2);
            this.listRegexPattern.Size = new System.Drawing.Size(416, 305);
            this.listRegexPattern.TabIndex = 15;
            this.listRegexPattern.SelectedIndexChanged += new System.EventHandler(this.listRegexPattern_SelectedIndexChanged);
            this.listRegexPattern.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listRegexPattern_KeyDown);
            // 
            // txtProducts
            // 
            this.txtProducts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProducts.Location = new System.Drawing.Point(357, 8);
            this.txtProducts.Multiline = true;
            this.txtProducts.Name = "txtProducts";
            this.tableLayoutPanel1.SetRowSpan(this.txtProducts, 2);
            this.txtProducts.Size = new System.Drawing.Size(67, 377);
            this.txtProducts.TabIndex = 10;
            this.txtProducts.Text = "Trafik\r\nKasko\r\nYeşilsigorta\r\nZ.Koltuk\r\nYol Yardım\r\nIMM\r\nTSS\r\nYSS\r\nDASK\r\nKONUT\r\nİŞ" + "YERİ";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1155, 704);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Weekly Custom Report Generator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.ListBox listRegexPattern;

        private System.Windows.Forms.Label label1;

        private System.Windows.Forms.TextBox txtDir;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

        private System.Windows.Forms.RichTextBox txtOutput;

        private System.Windows.Forms.RichTextBox txtInput;

        private System.Windows.Forms.TextBox txtProducts;
        
      

        #endregion
    }
}