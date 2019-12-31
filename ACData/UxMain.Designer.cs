namespace ACData
{
    partial class UxMain
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
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.ListBoxSourceFiles = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TextBoxDestinationPath = new System.Windows.Forms.TextBox();
            this.ButtonDestination = new System.Windows.Forms.Button();
            this.ButtonProcess = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ButtonClearFiles = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ListBoxSourceFiles
            // 
            this.ListBoxSourceFiles.AllowDrop = true;
            this.ListBoxSourceFiles.BackColor = System.Drawing.Color.Gray;
            this.ListBoxSourceFiles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ListBoxSourceFiles.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListBoxSourceFiles.ForeColor = System.Drawing.Color.White;
            this.ListBoxSourceFiles.FormattingEnabled = true;
            this.ListBoxSourceFiles.ItemHeight = 15;
            this.ListBoxSourceFiles.Location = new System.Drawing.Point(10, 78);
            this.ListBoxSourceFiles.Name = "ListBoxSourceFiles";
            this.ListBoxSourceFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.ListBoxSourceFiles.Size = new System.Drawing.Size(779, 437);
            this.ListBoxSourceFiles.TabIndex = 1;
            this.ListBoxSourceFiles.Click += new System.EventHandler(this.ListBoxSourceFiles_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(7, 536);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Destination Path";
            // 
            // TextBoxDestinationPath
            // 
            this.TextBoxDestinationPath.BackColor = System.Drawing.Color.Gray;
            this.TextBoxDestinationPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TextBoxDestinationPath.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBoxDestinationPath.ForeColor = System.Drawing.Color.White;
            this.TextBoxDestinationPath.Location = new System.Drawing.Point(110, 529);
            this.TextBoxDestinationPath.Multiline = true;
            this.TextBoxDestinationPath.Name = "TextBoxDestinationPath";
            this.TextBoxDestinationPath.Size = new System.Drawing.Size(396, 26);
            this.TextBoxDestinationPath.TabIndex = 6;
            // 
            // ButtonDestination
            // 
            this.ButtonDestination.BackColor = System.Drawing.Color.Silver;
            this.ButtonDestination.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonDestination.Location = new System.Drawing.Point(512, 529);
            this.ButtonDestination.Name = "ButtonDestination";
            this.ButtonDestination.Size = new System.Drawing.Size(77, 25);
            this.ButtonDestination.TabIndex = 7;
            this.ButtonDestination.Text = "Select Path";
            this.ButtonDestination.UseVisualStyleBackColor = false;
            this.ButtonDestination.Click += new System.EventHandler(this.ButtonDestination_Click);
            // 
            // ButtonProcess
            // 
            this.ButtonProcess.BackColor = System.Drawing.Color.Black;
            this.ButtonProcess.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ButtonProcess.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonProcess.Location = new System.Drawing.Point(681, 521);
            this.ButtonProcess.Name = "ButtonProcess";
            this.ButtonProcess.Size = new System.Drawing.Size(108, 35);
            this.ButtonProcess.TabIndex = 12;
            this.ButtonProcess.Text = "Convert";
            this.ButtonProcess.UseVisualStyleBackColor = false;
            this.ButtonProcess.Click += new System.EventHandler(this.ButtonProcess_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.label4.Location = new System.Drawing.Point(12, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(196, 18);
            this.label4.TabIndex = 19;
            this.label4.Text = "Click box or drag and drop files";
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label6.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(15, 8);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(748, 49);
            this.label6.TabIndex = 22;
            this.label6.Text = "This is a tool for converting Asheron\'s Call server data formats.\r\n JSON -> SQL o" +
    "r SQL -> JSON\r\n\r\n\r\n";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ButtonClearFiles
            // 
            this.ButtonClearFiles.BackColor = System.Drawing.Color.Silver;
            this.ButtonClearFiles.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonClearFiles.Location = new System.Drawing.Point(595, 530);
            this.ButtonClearFiles.Name = "ButtonClearFiles";
            this.ButtonClearFiles.Size = new System.Drawing.Size(75, 24);
            this.ButtonClearFiles.TabIndex = 23;
            this.ButtonClearFiles.Text = "Clear Files";
            this.ButtonClearFiles.UseVisualStyleBackColor = false;
            this.ButtonClearFiles.Click += new System.EventHandler(this.ButtonClearFiles_Click);
            // 
            // UxMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(795, 561);
            this.Controls.Add(this.ButtonClearFiles);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ButtonProcess);
            this.Controls.Add(this.ButtonDestination);
            this.Controls.Add(this.TextBoxDestinationPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ListBoxSourceFiles);
            this.Name = "UxMain";
            this.Text = "AC Data Conversion Tool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ListBox ListBoxSourceFiles;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TextBoxDestinationPath;
        private System.Windows.Forms.Button ButtonDestination;
        private System.Windows.Forms.Button ButtonProcess;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button ButtonClearFiles;
        protected System.Windows.Forms.Label label4;
    }
}