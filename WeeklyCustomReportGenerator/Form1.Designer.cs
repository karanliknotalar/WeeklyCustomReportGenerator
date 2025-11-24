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
            this.btnProcess = new System.Windows.Forms.Button();
            this.txtRegexRich = new System.Windows.Forms.RichTextBox();
            this.txtInput = new System.Windows.Forms.RichTextBox();
            this.txtProducts = new System.Windows.Forms.TextBox();
            this.txtOutput = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.32811F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.006964F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 64.66492F));
            this.tableLayoutPanel1.Controls.Add(this.btnProcess, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtRegexRich, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtInput, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtProducts, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtOutput, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23.79679F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 76.20321F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 196F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1155, 704);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // btnProcess
            // 
            this.btnProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnProcess.Location = new System.Drawing.Point(320, 8);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(85, 112);
            this.btnProcess.TabIndex = 11;
            this.btnProcess.Text = "GRUPLA";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // txtRegexRich
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txtRegexRich, 2);
            this.txtRegexRich.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRegexRich.Location = new System.Drawing.Point(8, 505);
            this.txtRegexRich.Name = "txtRegexRich";
            this.txtRegexRich.Size = new System.Drawing.Size(397, 191);
            this.txtRegexRich.TabIndex = 9;
            this.txtRegexRich.Text = "";
            // 
            // txtInput
            // 
            this.txtInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInput.Location = new System.Drawing.Point(8, 8);
            this.txtInput.Name = "txtInput";
            this.tableLayoutPanel1.SetRowSpan(this.txtInput, 2);
            this.txtInput.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtInput.Size = new System.Drawing.Size(306, 491);
            this.txtInput.TabIndex = 9;
            this.txtInput.Text = "";
            // 
            // txtProducts
            // 
            this.txtProducts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProducts.Location = new System.Drawing.Point(320, 126);
            this.txtProducts.Multiline = true;
            this.txtProducts.Name = "txtProducts";
            this.txtProducts.Size = new System.Drawing.Size(85, 373);
            this.txtProducts.TabIndex = 10;
            this.txtProducts.Text = "Trafik\r\nKasko\r\nYeşilsigorta\r\nZ.Koltuk\r\nYol Yardım\r\nIMM\r\nTSS\r\nYSS\r\nDASK\r\nKONUT\r\nİŞ" + "YERİ";
            // 
            // txtOutput
            // 
            this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutput.Location = new System.Drawing.Point(411, 8);
            this.txtOutput.Name = "txtOutput";
            this.tableLayoutPanel1.SetRowSpan(this.txtOutput, 3);
            this.txtOutput.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(736, 688);
            this.txtOutput.TabIndex = 12;
            this.txtOutput.Text = "";
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

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

        private System.Windows.Forms.Button btnProcess;

        private System.Windows.Forms.RichTextBox txtOutput;

        private System.Windows.Forms.RichTextBox txtInput;

        private System.Windows.Forms.RichTextBox txtRegexRich;

        private System.Windows.Forms.TextBox txtProducts;
        
      

        #endregion
    }
}