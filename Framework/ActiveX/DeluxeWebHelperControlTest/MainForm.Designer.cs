namespace DeluxeWebHelperControlTest
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
            this.buttonShowWindowsTemp = new System.Windows.Forms.Button();
            this.buttonOpenWord = new System.Windows.Forms.Button();
            this.buttonOpenDialog = new System.Windows.Forms.Button();
            this.buttonGetAuthor = new System.Windows.Forms.Button();
            this.buttonGetMACFromUuid = new System.Windows.Forms.Button();
            this.buttonGetAllMAC = new System.Windows.Forms.Button();
            this.buttonGetAllEncryptedMACAddress = new System.Windows.Forms.Button();
            this.buttonGetAllDecryptedMACAddress = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonShowWindowsTemp
            // 
            this.buttonShowWindowsTemp.Location = new System.Drawing.Point(45, 29);
            this.buttonShowWindowsTemp.Name = "buttonShowWindowsTemp";
            this.buttonShowWindowsTemp.Size = new System.Drawing.Size(141, 23);
            this.buttonShowWindowsTemp.TabIndex = 0;
            this.buttonShowWindowsTemp.Text = "Windows Temp...";
            this.buttonShowWindowsTemp.UseVisualStyleBackColor = true;
            this.buttonShowWindowsTemp.Click += new System.EventHandler(this.buttonShowWindowsTemp_Click);
            // 
            // buttonOpenWord
            // 
            this.buttonOpenWord.Location = new System.Drawing.Point(212, 29);
            this.buttonOpenWord.Name = "buttonOpenWord";
            this.buttonOpenWord.Size = new System.Drawing.Size(141, 23);
            this.buttonOpenWord.TabIndex = 1;
            this.buttonOpenWord.Text = "Open Word...";
            this.buttonOpenWord.UseVisualStyleBackColor = true;
            this.buttonOpenWord.Click += new System.EventHandler(this.buttonOpenWord_Click);
            // 
            // buttonOpenDialog
            // 
            this.buttonOpenDialog.Location = new System.Drawing.Point(45, 72);
            this.buttonOpenDialog.Name = "buttonOpenDialog";
            this.buttonOpenDialog.Size = new System.Drawing.Size(141, 23);
            this.buttonOpenDialog.TabIndex = 2;
            this.buttonOpenDialog.Text = "Open Dialog...";
            this.buttonOpenDialog.UseVisualStyleBackColor = true;
            this.buttonOpenDialog.Click += new System.EventHandler(this.buttonOpenDialog_Click);
            // 
            // buttonGetAuthor
            // 
            this.buttonGetAuthor.Location = new System.Drawing.Point(212, 72);
            this.buttonGetAuthor.Name = "buttonGetAuthor";
            this.buttonGetAuthor.Size = new System.Drawing.Size(141, 23);
            this.buttonGetAuthor.TabIndex = 3;
            this.buttonGetAuthor.Text = "Get Author...";
            this.buttonGetAuthor.UseVisualStyleBackColor = true;
            this.buttonGetAuthor.Click += new System.EventHandler(this.buttonGetAuthor_Click);
            // 
            // buttonGetMACFromUuid
            // 
            this.buttonGetMACFromUuid.Location = new System.Drawing.Point(45, 118);
            this.buttonGetMACFromUuid.Name = "buttonGetMACFromUuid";
            this.buttonGetMACFromUuid.Size = new System.Drawing.Size(141, 23);
            this.buttonGetMACFromUuid.TabIndex = 4;
            this.buttonGetMACFromUuid.Text = "Get MAC(uuid)...";
            this.buttonGetMACFromUuid.UseVisualStyleBackColor = true;
            this.buttonGetMACFromUuid.Click += new System.EventHandler(this.buttonGetMACFromUuid_Click);
            // 
            // buttonGetAllMAC
            // 
            this.buttonGetAllMAC.Location = new System.Drawing.Point(212, 118);
            this.buttonGetAllMAC.Name = "buttonGetAllMAC";
            this.buttonGetAllMAC.Size = new System.Drawing.Size(141, 23);
            this.buttonGetAllMAC.TabIndex = 5;
            this.buttonGetAllMAC.Text = "Get All MAC...";
            this.buttonGetAllMAC.UseVisualStyleBackColor = true;
            this.buttonGetAllMAC.Click += new System.EventHandler(this.buttonGetAllMAC_Click);
            // 
            // buttonGetAllEncryptedMACAddress
            // 
            this.buttonGetAllEncryptedMACAddress.Location = new System.Drawing.Point(45, 161);
            this.buttonGetAllEncryptedMACAddress.Name = "buttonGetAllEncryptedMACAddress";
            this.buttonGetAllEncryptedMACAddress.Size = new System.Drawing.Size(141, 23);
            this.buttonGetAllEncryptedMACAddress.TabIndex = 6;
            this.buttonGetAllEncryptedMACAddress.Text = "Get All Enc MAC...";
            this.buttonGetAllEncryptedMACAddress.UseVisualStyleBackColor = true;
            this.buttonGetAllEncryptedMACAddress.Click += new System.EventHandler(this.buttonGetAllEncryptedMACAddress_Click);
            // 
            // buttonGetAllDecryptedMACAddress
            // 
            this.buttonGetAllDecryptedMACAddress.Location = new System.Drawing.Point(212, 161);
            this.buttonGetAllDecryptedMACAddress.Name = "buttonGetAllDecryptedMACAddress";
            this.buttonGetAllDecryptedMACAddress.Size = new System.Drawing.Size(141, 23);
            this.buttonGetAllDecryptedMACAddress.TabIndex = 7;
            this.buttonGetAllDecryptedMACAddress.Text = "Get All Dec MAC...";
            this.buttonGetAllDecryptedMACAddress.UseVisualStyleBackColor = true;
            this.buttonGetAllDecryptedMACAddress.Click += new System.EventHandler(this.buttonGetAllDecryptedMACAddress_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(698, 266);
            this.Controls.Add(this.buttonGetAllDecryptedMACAddress);
            this.Controls.Add(this.buttonGetAllEncryptedMACAddress);
            this.Controls.Add(this.buttonGetAllMAC);
            this.Controls.Add(this.buttonGetMACFromUuid);
            this.Controls.Add(this.buttonGetAuthor);
            this.Controls.Add(this.buttonOpenDialog);
            this.Controls.Add(this.buttonOpenWord);
            this.Controls.Add(this.buttonShowWindowsTemp);
            this.Name = "MainForm";
            this.Text = "测试组件";
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonShowWindowsTemp;
		private System.Windows.Forms.Button buttonOpenWord;
		private System.Windows.Forms.Button buttonOpenDialog;
		private System.Windows.Forms.Button buttonGetAuthor;
		private System.Windows.Forms.Button buttonGetMACFromUuid;
		private System.Windows.Forms.Button buttonGetAllMAC;
        private System.Windows.Forms.Button buttonGetAllEncryptedMACAddress;
        private System.Windows.Forms.Button buttonGetAllDecryptedMACAddress;
	}
}

