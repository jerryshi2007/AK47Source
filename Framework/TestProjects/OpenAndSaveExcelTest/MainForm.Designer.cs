namespace OpenAndSaveExcelTest
{
    partial class MainForm
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
            this.openExceFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.buttonOpenAndSaveFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // openExceFileDialog
            // 
            this.openExceFileDialog.Filter = "Excel files|*.xlsx|All files|*.*";
            // 
            // buttonOpenAndSaveFile
            // 
            this.buttonOpenAndSaveFile.Location = new System.Drawing.Point(50, 26);
            this.buttonOpenAndSaveFile.Name = "buttonOpenAndSaveFile";
            this.buttonOpenAndSaveFile.Size = new System.Drawing.Size(132, 25);
            this.buttonOpenAndSaveFile.TabIndex = 0;
            this.buttonOpenAndSaveFile.Text = "打开并保存文件...";
            this.buttonOpenAndSaveFile.UseVisualStyleBackColor = true;
            this.buttonOpenAndSaveFile.Click += new System.EventHandler(this.buttonOpenAndSaveFile_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(451, 269);
            this.Controls.Add(this.buttonOpenAndSaveFile);
            this.Name = "MainForm";
            this.Text = "打开和保存Excel测试";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openExceFileDialog;
        private System.Windows.Forms.Button buttonOpenAndSaveFile;
    }
}

