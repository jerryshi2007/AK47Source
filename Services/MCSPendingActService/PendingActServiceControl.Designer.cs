namespace MCSPendingActService
{
	partial class PendingActServiceControl
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
			this.btnView = new System.Windows.Forms.Button();
			this.txtActivityID = new System.Windows.Forms.TextBox();
			this.txtAppName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtProgramName = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.txtActID = new System.Windows.Forms.TextBox();
			this.btnSingleActivity = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnView
			// 
			this.btnView.Location = new System.Drawing.Point(26, 11);
			this.btnView.Name = "btnView";
			this.btnView.Size = new System.Drawing.Size(180, 30);
			this.btnView.TabIndex = 0;
			this.btnView.Text = "查看挂起的ActivityID";
			this.btnView.UseVisualStyleBackColor = true;
			this.btnView.Click += new System.EventHandler(this.btnView_Click);
			// 
			// txtActivityID
			// 
			this.txtActivityID.Location = new System.Drawing.Point(26, 47);
			this.txtActivityID.Multiline = true;
			this.txtActivityID.Name = "txtActivityID";
			this.txtActivityID.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtActivityID.Size = new System.Drawing.Size(485, 193);
			this.txtActivityID.TabIndex = 1;
			// 
			// txtAppName
			// 
			this.txtAppName.Location = new System.Drawing.Point(78, 258);
			this.txtAppName.Name = "txtAppName";
			this.txtAppName.Size = new System.Drawing.Size(100, 21);
			this.txtAppName.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(16, 264);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 12);
			this.label1.TabIndex = 3;
			this.label1.Text = "AppName:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(213, 264);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(77, 12);
			this.label2.TabIndex = 4;
			this.label2.Text = "ProgramName:";
			// 
			// txtProgramName
			// 
			this.txtProgramName.Location = new System.Drawing.Point(298, 259);
			this.txtProgramName.Name = "txtProgramName";
			this.txtProgramName.Size = new System.Drawing.Size(100, 21);
			this.txtProgramName.TabIndex = 5;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(424, 259);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 6;
			this.button1.Text = "处理";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(16, 312);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(71, 12);
			this.label3.TabIndex = 7;
			this.label3.Text = "ActivityID:";
			// 
			// txtActID
			// 
			this.txtActID.Location = new System.Drawing.Point(106, 303);
			this.txtActID.Name = "txtActID";
			this.txtActID.Size = new System.Drawing.Size(292, 21);
			this.txtActID.TabIndex = 8;
			// 
			// btnSingleActivity
			// 
			this.btnSingleActivity.Location = new System.Drawing.Point(424, 301);
			this.btnSingleActivity.Name = "btnSingleActivity";
			this.btnSingleActivity.Size = new System.Drawing.Size(75, 23);
			this.btnSingleActivity.TabIndex = 9;
			this.btnSingleActivity.Text = "处理";
			this.btnSingleActivity.UseVisualStyleBackColor = true;
			this.btnSingleActivity.Click += new System.EventHandler(this.btnSingleActivity_Click);
			// 
			// PendingActServiceControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnSingleActivity);
			this.Controls.Add(this.txtActID);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.txtProgramName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtAppName);
			this.Controls.Add(this.txtActivityID);
			this.Controls.Add(this.btnView);
			this.Name = "PendingActServiceControl";
			this.Size = new System.Drawing.Size(539, 373);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnView;
		private System.Windows.Forms.TextBox txtActivityID;
		private System.Windows.Forms.TextBox txtAppName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtProgramName;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtActID;
		private System.Windows.Forms.Button btnSingleActivity;
	}
}