namespace MCS.Library.Utilities
{
	partial class LoginForm
	{
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.panelTop = new System.Windows.Forms.Panel();
			this.labelCaption = new System.Windows.Forms.Label();
			this.panelClient = new System.Windows.Forms.Panel();
			this.textBoxDC = new System.Windows.Forms.TextBox();
			this.textBoxPassword = new System.Windows.Forms.TextBox();
			this.textBoxUser = new System.Windows.Forms.TextBox();
			this.labelDomain = new System.Windows.Forms.Label();
			this.labelPassword = new System.Windows.Forms.Label();
			this.labelUser = new System.Windows.Forms.Label();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonClose = new System.Windows.Forms.Button();
			this.panelTop.SuspendLayout();
			this.panelClient.SuspendLayout();
			this.panelBottom.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelTop
			// 
			this.panelTop.Controls.Add(this.labelCaption);
			this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelTop.Location = new System.Drawing.Point(0, 0);
			this.panelTop.Name = "panelTop";
			this.panelTop.Size = new System.Drawing.Size(318, 34);
			this.panelTop.TabIndex = 0;
			// 
			// labelCaption
			// 
			this.labelCaption.AutoSize = true;
			this.labelCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.labelCaption.Location = new System.Drawing.Point(12, 9);
			this.labelCaption.Name = "labelCaption";
			this.labelCaption.Size = new System.Drawing.Size(153, 20);
			this.labelCaption.TabIndex = 0;
			this.labelCaption.Text = "登录域控制器的凭据";
			// 
			// panelClient
			// 
			this.panelClient.Controls.Add(this.textBoxDC);
			this.panelClient.Controls.Add(this.textBoxPassword);
			this.panelClient.Controls.Add(this.textBoxUser);
			this.panelClient.Controls.Add(this.labelDomain);
			this.panelClient.Controls.Add(this.labelPassword);
			this.panelClient.Controls.Add(this.labelUser);
			this.panelClient.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelClient.Location = new System.Drawing.Point(0, 34);
			this.panelClient.Name = "panelClient";
			this.panelClient.Size = new System.Drawing.Size(318, 162);
			this.panelClient.TabIndex = 1;
			// 
			// textBoxDC
			// 
			this.textBoxDC.Location = new System.Drawing.Point(73, 76);
			this.textBoxDC.Name = "textBoxDC";
			this.textBoxDC.Size = new System.Drawing.Size(168, 20);
			this.textBoxDC.TabIndex = 6;
			// 
			// textBoxPassword
			// 
			this.textBoxPassword.Location = new System.Drawing.Point(73, 45);
			this.textBoxPassword.Name = "textBoxPassword";
			this.textBoxPassword.PasswordChar = '*';
			this.textBoxPassword.Size = new System.Drawing.Size(168, 20);
			this.textBoxPassword.TabIndex = 4;
			// 
			// textBoxUser
			// 
			this.textBoxUser.Location = new System.Drawing.Point(73, 14);
			this.textBoxUser.Name = "textBoxUser";
			this.textBoxUser.Size = new System.Drawing.Size(168, 20);
			this.textBoxUser.TabIndex = 2;
			// 
			// labelDomain
			// 
			this.labelDomain.AutoSize = true;
			this.labelDomain.Location = new System.Drawing.Point(46, 79);
			this.labelDomain.Name = "labelDomain";
			this.labelDomain.Size = new System.Drawing.Size(22, 13);
			this.labelDomain.TabIndex = 5;
			this.labelDomain.Text = "域:";
			// 
			// labelPassword
			// 
			this.labelPassword.AutoSize = true;
			this.labelPassword.Location = new System.Drawing.Point(34, 48);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(34, 13);
			this.labelPassword.TabIndex = 3;
			this.labelPassword.Text = "密码:";
			// 
			// labelUser
			// 
			this.labelUser.AutoSize = true;
			this.labelUser.Location = new System.Drawing.Point(34, 17);
			this.labelUser.Name = "labelUser";
			this.labelUser.Size = new System.Drawing.Size(34, 13);
			this.labelUser.TabIndex = 1;
			this.labelUser.Text = "用户:";
			// 
			// panelBottom
			// 
			this.panelBottom.Controls.Add(this.buttonOK);
			this.panelBottom.Controls.Add(this.buttonClose);
			this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelBottom.Location = new System.Drawing.Point(0, 156);
			this.panelBottom.Name = "panelBottom";
			this.panelBottom.Size = new System.Drawing.Size(318, 40);
			this.panelBottom.TabIndex = 2;
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(49, 7);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(92, 24);
			this.buttonOK.TabIndex = 7;
			this.buttonOK.Text = "确定(&O)";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonClose
			// 
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonClose.Location = new System.Drawing.Point(188, 6);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(92, 24);
			this.buttonClose.TabIndex = 8;
			this.buttonClose.Text = "取消(&C)";
			this.buttonClose.UseVisualStyleBackColor = true;
			// 
			// LoginForm
			// 
			this.AcceptButton = this.buttonOK;
			this.CancelButton = this.buttonClose;
			this.ClientSize = new System.Drawing.Size(318, 196);
			this.Controls.Add(this.panelBottom);
			this.Controls.Add(this.panelClient);
			this.Controls.Add(this.panelTop);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "LoginForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Load += new System.EventHandler(this.LoginForm_Load);
			this.panelTop.ResumeLayout(false);
			this.panelTop.PerformLayout();
			this.panelClient.ResumeLayout(false);
			this.panelClient.PerformLayout();
			this.panelBottom.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Panel panelClient;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.Label labelCaption;
		private System.Windows.Forms.Label labelDomain;
		private System.Windows.Forms.Label labelPassword;
		private System.Windows.Forms.Label labelUser;
		private System.Windows.Forms.TextBox textBoxDC;
		private System.Windows.Forms.TextBox textBoxPassword;
		private System.Windows.Forms.TextBox textBoxUser;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonClose;
	}
}