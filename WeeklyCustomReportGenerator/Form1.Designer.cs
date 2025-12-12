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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtGalleryCustomerList = new System.Windows.Forms.TextBox();
            this.txtInput = new System.Windows.Forms.RichTextBox();
            this.txtOutput = new System.Windows.Forms.RichTextBox();
            this.txtDriveDir = new System.Windows.Forms.TextBox();
            this.lblPdfDir = new System.Windows.Forms.Label();
            this.listRegexPattern = new System.Windows.Forms.ListBox();
            this.txtProducts = new System.Windows.Forms.TextBox();
            this.lblSaveDir = new System.Windows.Forms.Label();
            this.txtSaveDir = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
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
            this.tableLayoutPanel1.Controls.Add(this.txtGalleryCustomerList, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.txtInput, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtOutput, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtDriveDir, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblPdfDir, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.listRegexPattern, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtProducts, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblSaveDir, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.txtSaveDir, 3, 5);
            this.tableLayoutPanel1.Controls.Add(this.btnSave, 2, 6);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.tableLayoutPanel1.RowCount = 9;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23.79679F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 76.20321F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 290F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1155, 704);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // txtGalleryCustomerList
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txtGalleryCustomerList, 2);
            this.txtGalleryCustomerList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtGalleryCustomerList.Location = new System.Drawing.Point(8, 601);
            this.txtGalleryCustomerList.Multiline = true;
            this.txtGalleryCustomerList.Name = "txtGalleryCustomerList";
            this.tableLayoutPanel1.SetRowSpan(this.txtGalleryCustomerList, 5);
            this.txtGalleryCustomerList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtGalleryCustomerList.Size = new System.Drawing.Size(416, 95);
            this.txtGalleryCustomerList.TabIndex = 16;
            this.txtGalleryCustomerList.Text = resources.GetString("txtGalleryCustomerList.Text");
            // 
            // txtInput
            // 
            this.txtInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInput.Location = new System.Drawing.Point(8, 8);
            this.txtInput.Name = "txtInput";
            this.tableLayoutPanel1.SetRowSpan(this.txtInput, 2);
            this.txtInput.Size = new System.Drawing.Size(343, 277);
            this.txtInput.TabIndex = 9;
            this.txtInput.Text = "";
            this.txtInput.TextChanged += new System.EventHandler(this.txtInput_TextChanged);
            // 
            // txtOutput
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txtOutput, 2);
            this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutput.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtOutput.Location = new System.Drawing.Point(430, 8);
            this.txtOutput.Name = "txtOutput";
            this.tableLayoutPanel1.SetRowSpan(this.txtOutput, 3);
            this.txtOutput.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(717, 567);
            this.txtOutput.TabIndex = 12;
            this.txtOutput.Text = "";
            // 
            // txtDriveDir
            // 
            this.txtDriveDir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDriveDir.Location = new System.Drawing.Point(558, 581);
            this.txtDriveDir.Name = "txtDriveDir";
            this.txtDriveDir.Size = new System.Drawing.Size(589, 20);
            this.txtDriveDir.TabIndex = 13;
            this.txtDriveDir.Text = "C:\\Users\\ASUS\\GDrive";
            this.txtDriveDir.TextChanged += new System.EventHandler(this.txtDir_TextChanged);
            // 
            // lblPdfDir
            // 
            this.lblPdfDir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPdfDir.Location = new System.Drawing.Point(430, 578);
            this.lblPdfDir.Name = "lblPdfDir";
            this.lblPdfDir.Size = new System.Drawing.Size(122, 20);
            this.lblPdfDir.TabIndex = 14;
            this.lblPdfDir.Text = "PDF\'ler dizini";
            this.lblPdfDir.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblPdfDir.Click += new System.EventHandler(this.lblPdfDir_Click);
            // 
            // listRegexPattern
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.listRegexPattern, 2);
            this.listRegexPattern.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listRegexPattern.FormattingEnabled = true;
            this.listRegexPattern.HorizontalExtent = 800;
            this.listRegexPattern.HorizontalScrollbar = true;
            this.listRegexPattern.Location = new System.Drawing.Point(8, 291);
            this.listRegexPattern.Name = "listRegexPattern";
            this.tableLayoutPanel1.SetRowSpan(this.listRegexPattern, 2);
            this.listRegexPattern.Size = new System.Drawing.Size(416, 304);
            this.listRegexPattern.TabIndex = 15;
            this.listRegexPattern.Click += new System.EventHandler(this.listRegexPattern_Click);
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
            this.txtProducts.Size = new System.Drawing.Size(67, 277);
            this.txtProducts.TabIndex = 10;
            this.txtProducts.Text = "Trafik\r\nKasko\r\nYeşilsigorta\r\nZ.Koltuk\r\nYol Yardım\r\nIMM\r\nTSS\r\nYSS\r\nDASK\r\nKONUT\r\nİŞ" + "YERİ";
            // 
            // lblSaveDir
            // 
            this.lblSaveDir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSaveDir.Location = new System.Drawing.Point(430, 618);
            this.lblSaveDir.Name = "lblSaveDir";
            this.lblSaveDir.Size = new System.Drawing.Size(122, 20);
            this.lblSaveDir.TabIndex = 17;
            this.lblSaveDir.Text = "Kaydedilecek Dizin:";
            this.lblSaveDir.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSaveDir.Click += new System.EventHandler(this.lblSaveDir_Click);
            // 
            // txtSaveDir
            // 
            this.txtSaveDir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSaveDir.Location = new System.Drawing.Point(558, 621);
            this.txtSaveDir.Name = "txtSaveDir";
            this.txtSaveDir.Size = new System.Drawing.Size(589, 20);
            this.txtSaveDir.TabIndex = 18;
            this.txtSaveDir.Text = "C:\\Users\\ASUS\\GDrive\\A Belgeler\\İSTANBUL GRUP SİGORTA\\İstatistikler";
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSave.Location = new System.Drawing.Point(430, 641);
            this.btnSave.Name = "btnSave";
            this.tableLayoutPanel1.SetRowSpan(this.btnSave, 3);
            this.btnSave.Size = new System.Drawing.Size(122, 55);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "RAPORU\r\nKAYDET";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
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

        private System.Windows.Forms.Label lblSaveDir;

        private System.Windows.Forms.Button btnSave;

        private System.Windows.Forms.TextBox txtSaveDir;

        private System.Windows.Forms.TextBox txtGalleryCustomerList;

        private System.Windows.Forms.ListBox listRegexPattern;

        private System.Windows.Forms.Label lblPdfDir;

        private System.Windows.Forms.TextBox txtDriveDir;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

        private System.Windows.Forms.RichTextBox txtOutput;

        private System.Windows.Forms.RichTextBox txtInput;

        private System.Windows.Forms.TextBox txtProducts;
        
      

        #endregion
    }
}