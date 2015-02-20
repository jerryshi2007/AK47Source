namespace MCS.Services.Messages
{
	partial class SendMailControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBoxAuth = new System.Windows.Forms.GroupBox();
			this.textBoxPassword = new System.Windows.Forms.TextBox();
			this.labelPassword = new System.Windows.Forms.Label();
			this.textBoxLogOnName = new System.Windows.Forms.TextBox();
			this.labelLogonName = new System.Windows.Forms.Label();
			this.labelAuthType = new System.Windows.Forms.Label();
			this.comboBoxAuthenticateType = new System.Windows.Forms.ComboBox();
			this.textBoxDest = new System.Windows.Forms.TextBox();
			this.labelDest = new System.Windows.Forms.Label();
			this.textBoxMessage = new System.Windows.Forms.TextBox();
			this.labelMessage = new System.Windows.Forms.Label();
			this.buttonSend = new System.Windows.Forms.Button();
			this.labelPort = new System.Windows.Forms.Label();
			this.textBoxPort = new System.Windows.Forms.TextBox();
			this.textBoxServer = new System.Windows.Forms.TextBox();
			this.labelServer = new System.Windows.Forms.Label();
			this.labelCaption = new System.Windows.Forms.Label();
			this.textBoxSignInAddress = new System.Windows.Forms.TextBox();
			this.labelSignInName = new System.Windows.Forms.Label();
			this.buttonResetFromConfig = new System.Windows.Forms.Button();
			this.buttonSendOneCandidate = new System.Windows.Forms.Button();
			this.groupBoxAuth.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxAuth
			// 
			this.groupBoxAuth.Controls.Add(this.textBoxPassword);
			this.groupBoxAuth.Controls.Add(this.labelPassword);
			this.groupBoxAuth.Controls.Add(this.textBoxLogOnName);
			this.groupBoxAuth.Controls.Add(this.labelLogonName);
			this.groupBoxAuth.Location = new System.Drawing.Point(99, 133);
			this.groupBoxAuth.Name = "groupBoxAuth";
			this.groupBoxAuth.Size = new System.Drawing.Size(336, 88);
			this.groupBoxAuth.TabIndex = 47;
			this.groupBoxAuth.TabStop = false;
			this.groupBoxAuth.Text = "认证";
			// 
			// textBoxPassword
			// 
			this.textBoxPassword.Location = new System.Drawing.Point(80, 54);
			this.textBoxPassword.Name = "textBoxPassword";
			this.textBoxPassword.PasswordChar = '*';
			this.textBoxPassword.Size = new System.Drawing.Size(240, 20);
			this.textBoxPassword.TabIndex = 21;
			// 
			// labelPassword
			// 
			this.labelPassword.AutoSize = true;
			this.labelPassword.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.labelPassword.Location = new System.Drawing.Point(32, 56);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(38, 12);
			this.labelPassword.TabIndex = 20;
			this.labelPassword.Text = "口令:";
			// 
			// textBoxLogOnName
			// 
			this.textBoxLogOnName.Location = new System.Drawing.Point(80, 22);
			this.textBoxLogOnName.Name = "textBoxLogOnName";
			this.textBoxLogOnName.Size = new System.Drawing.Size(240, 20);
			this.textBoxLogOnName.TabIndex = 19;
			// 
			// labelLogonName
			// 
			this.labelLogonName.AutoSize = true;
			this.labelLogonName.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.labelLogonName.Location = new System.Drawing.Point(16, 24);
			this.labelLogonName.Name = "labelLogonName";
			this.labelLogonName.Size = new System.Drawing.Size(51, 12);
			this.labelLogonName.TabIndex = 18;
			this.labelLogonName.Text = "登录名:";
			// 
			// labelAuthType
			// 
			this.labelAuthType.AutoSize = true;
			this.labelAuthType.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.labelAuthType.Location = new System.Drawing.Point(27, 101);
			this.labelAuthType.Name = "labelAuthType";
			this.labelAuthType.Size = new System.Drawing.Size(64, 12);
			this.labelAuthType.TabIndex = 45;
			this.labelAuthType.Text = "认证方式:";
			// 
			// comboBoxAuthenticateType
			// 
			this.comboBoxAuthenticateType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxAuthenticateType.Location = new System.Drawing.Point(99, 99);
			this.comboBoxAuthenticateType.Name = "comboBoxAuthenticateType";
			this.comboBoxAuthenticateType.Size = new System.Drawing.Size(176, 21);
			this.comboBoxAuthenticateType.TabIndex = 46;
			this.comboBoxAuthenticateType.SelectionChangeCommitted += new System.EventHandler(this.comboBoxAuthenticateType_SelectionChangeCommitted);
			// 
			// textBoxDest
			// 
			this.textBoxDest.Location = new System.Drawing.Point(99, 299);
			this.textBoxDest.Name = "textBoxDest";
			this.textBoxDest.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
			this.textBoxDest.Size = new System.Drawing.Size(240, 20);
			this.textBoxDest.TabIndex = 44;
			this.textBoxDest.Text = "zhaodan@msoatest.com";
			// 
			// labelDest
			// 
			this.labelDest.AutoSize = true;
			this.labelDest.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.labelDest.Location = new System.Drawing.Point(43, 301);
			this.labelDest.Name = "labelDest";
			this.labelDest.Size = new System.Drawing.Size(51, 12);
			this.labelDest.TabIndex = 42;
			this.labelDest.Text = "收件人:";
			// 
			// textBoxMessage
			// 
			this.textBoxMessage.Location = new System.Drawing.Point(99, 267);
			this.textBoxMessage.Name = "textBoxMessage";
			this.textBoxMessage.Size = new System.Drawing.Size(240, 20);
			this.textBoxMessage.TabIndex = 41;
			this.textBoxMessage.Text = "Hello world";
			// 
			// labelMessage
			// 
			this.labelMessage.AutoSize = true;
			this.labelMessage.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.labelMessage.Location = new System.Drawing.Point(27, 269);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new System.Drawing.Size(64, 12);
			this.labelMessage.TabIndex = 40;
			this.labelMessage.Text = "邮件标题:";
			// 
			// buttonSend
			// 
			this.buttonSend.Location = new System.Drawing.Point(348, 298);
			this.buttonSend.Name = "buttonSend";
			this.buttonSend.Size = new System.Drawing.Size(75, 23);
			this.buttonSend.TabIndex = 43;
			this.buttonSend.Text = "发送(&S)";
			this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
			// 
			// labelPort
			// 
			this.labelPort.AutoSize = true;
			this.labelPort.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.labelPort.Location = new System.Drawing.Point(291, 69);
			this.labelPort.Name = "labelPort";
			this.labelPort.Size = new System.Drawing.Size(38, 12);
			this.labelPort.TabIndex = 37;
			this.labelPort.Text = "端口:";
			// 
			// textBoxPort
			// 
			this.textBoxPort.Location = new System.Drawing.Point(336, 67);
			this.textBoxPort.Name = "textBoxPort";
			this.textBoxPort.Size = new System.Drawing.Size(176, 20);
			this.textBoxPort.TabIndex = 38;
			// 
			// textBoxServer
			// 
			this.textBoxServer.Location = new System.Drawing.Point(99, 67);
			this.textBoxServer.Name = "textBoxServer";
			this.textBoxServer.Size = new System.Drawing.Size(176, 20);
			this.textBoxServer.TabIndex = 34;
			// 
			// labelServer
			// 
			this.labelServer.AutoSize = true;
			this.labelServer.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.labelServer.Location = new System.Drawing.Point(43, 69);
			this.labelServer.Name = "labelServer";
			this.labelServer.Size = new System.Drawing.Size(51, 12);
			this.labelServer.TabIndex = 33;
			this.labelServer.Text = "服务器:";
			// 
			// labelCaption
			// 
			this.labelCaption.AutoSize = true;
			this.labelCaption.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.labelCaption.Location = new System.Drawing.Point(11, 29);
			this.labelCaption.Name = "labelCaption";
			this.labelCaption.Size = new System.Drawing.Size(144, 16);
			this.labelCaption.TabIndex = 36;
			this.labelCaption.Text = "测试发送邮件通知";
			// 
			// textBoxSignInAddress
			// 
			this.textBoxSignInAddress.Location = new System.Drawing.Point(99, 235);
			this.textBoxSignInAddress.Name = "textBoxSignInAddress";
			this.textBoxSignInAddress.Size = new System.Drawing.Size(240, 20);
			this.textBoxSignInAddress.TabIndex = 39;
			// 
			// labelSignInName
			// 
			this.labelSignInName.AutoSize = true;
			this.labelSignInName.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.labelSignInName.Location = new System.Drawing.Point(19, 237);
			this.labelSignInName.Name = "labelSignInName";
			this.labelSignInName.Size = new System.Drawing.Size(77, 12);
			this.labelSignInName.TabIndex = 35;
			this.labelSignInName.Text = "发送人地址:";
			// 
			// buttonResetFromConfig
			// 
			this.buttonResetFromConfig.Location = new System.Drawing.Point(293, 98);
			this.buttonResetFromConfig.Name = "buttonResetFromConfig";
			this.buttonResetFromConfig.Size = new System.Drawing.Size(126, 23);
			this.buttonResetFromConfig.TabIndex = 48;
			this.buttonResetFromConfig.Text = "从配置文件重置(&R)";
			this.buttonResetFromConfig.Click += new System.EventHandler(this.buttonResetFromConfig_Click);
			// 
			// buttonSendOneCandidate
			// 
			this.buttonSendOneCandidate.Location = new System.Drawing.Point(429, 298);
			this.buttonSendOneCandidate.Name = "buttonSendOneCandidate";
			this.buttonSendOneCandidate.Size = new System.Drawing.Size(101, 23);
			this.buttonSendOneCandidate.TabIndex = 49;
			this.buttonSendOneCandidate.Text = "发送一条候选";
			this.buttonSendOneCandidate.Click += new System.EventHandler(this.buttonSendOneCandidate_Click);
			// 
			// SendMailControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.buttonSendOneCandidate);
			this.Controls.Add(this.buttonResetFromConfig);
			this.Controls.Add(this.groupBoxAuth);
			this.Controls.Add(this.labelAuthType);
			this.Controls.Add(this.comboBoxAuthenticateType);
			this.Controls.Add(this.textBoxDest);
			this.Controls.Add(this.labelDest);
			this.Controls.Add(this.textBoxMessage);
			this.Controls.Add(this.labelMessage);
			this.Controls.Add(this.buttonSend);
			this.Controls.Add(this.labelPort);
			this.Controls.Add(this.textBoxPort);
			this.Controls.Add(this.textBoxServer);
			this.Controls.Add(this.labelServer);
			this.Controls.Add(this.labelCaption);
			this.Controls.Add(this.textBoxSignInAddress);
			this.Controls.Add(this.labelSignInName);
			this.Name = "SendMailControl";
			this.Size = new System.Drawing.Size(533, 350);
			this.groupBoxAuth.ResumeLayout(false);
			this.groupBoxAuth.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBoxAuth;
		private System.Windows.Forms.TextBox textBoxPassword;
		private System.Windows.Forms.Label labelPassword;
		private System.Windows.Forms.TextBox textBoxLogOnName;
		private System.Windows.Forms.Label labelLogonName;
		private System.Windows.Forms.Label labelAuthType;
		private System.Windows.Forms.ComboBox comboBoxAuthenticateType;
		private System.Windows.Forms.TextBox textBoxDest;
		private System.Windows.Forms.Label labelDest;
		private System.Windows.Forms.TextBox textBoxMessage;
		private System.Windows.Forms.Label labelMessage;
		private System.Windows.Forms.Button buttonSend;
		private System.Windows.Forms.Label labelPort;
		private System.Windows.Forms.TextBox textBoxPort;
		private System.Windows.Forms.TextBox textBoxServer;
		private System.Windows.Forms.Label labelServer;
		private System.Windows.Forms.Label labelCaption;
		private System.Windows.Forms.TextBox textBoxSignInAddress;
		private System.Windows.Forms.Label labelSignInName;
		private System.Windows.Forms.Button buttonResetFromConfig;
		private System.Windows.Forms.Button buttonSendOneCandidate;
	}
}
