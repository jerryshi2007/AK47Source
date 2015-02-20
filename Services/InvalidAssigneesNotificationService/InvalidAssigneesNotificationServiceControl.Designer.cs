namespace InvalidAssigneesNotificationService
{
	partial class InvalidAssigneesNotificationServiceControl
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

		#region 组件设计器生成的代码

		/// <summary> 
		/// 设计器支持所需的方法 - 不要
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.Bt_Send = new System.Windows.Forms.Button();
			this.GV_InvalidAssigneesNotifications = new System.Windows.Forms.DataGridView();
			((System.ComponentModel.ISupportInitialize)(this.GV_InvalidAssigneesNotifications)).BeginInit();
			this.SuspendLayout();
			// 
			// Bt_Send
			// 
			this.Bt_Send.Location = new System.Drawing.Point(20, 13);
			this.Bt_Send.Name = "Bt_Send";
			this.Bt_Send.Size = new System.Drawing.Size(75, 23);
			this.Bt_Send.TabIndex = 0;
			this.Bt_Send.Text = "给管理员发送代办";
			this.Bt_Send.UseVisualStyleBackColor = true;
			this.Bt_Send.Click += new System.EventHandler(this.Bt_Send_Click);
			// 
			// GV_InvalidAssigneesNotifications
			// 
			this.GV_InvalidAssigneesNotifications.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.GV_InvalidAssigneesNotifications.Location = new System.Drawing.Point(20, 52);
			this.GV_InvalidAssigneesNotifications.Name = "GV_InvalidAssigneesNotifications";
			this.GV_InvalidAssigneesNotifications.RowTemplate.Height = 23;
			this.GV_InvalidAssigneesNotifications.Size = new System.Drawing.Size(377, 130);
			this.GV_InvalidAssigneesNotifications.TabIndex = 1;
			// 
			// InvalidAssigneesNotificationServiceControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.GV_InvalidAssigneesNotifications);
			this.Controls.Add(this.Bt_Send);
			this.Name = "InvalidAssigneesNotificationServiceControl";
			this.Size = new System.Drawing.Size(425, 227);
			((System.ComponentModel.ISupportInitialize)(this.GV_InvalidAssigneesNotifications)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button Bt_Send;
		private System.Windows.Forms.DataGridView GV_InvalidAssigneesNotifications;
	}
}
