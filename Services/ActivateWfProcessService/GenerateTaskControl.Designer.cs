namespace ActivateWfProcessService
{
	partial class GenerateTaskControl
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
			this.panelTop = new System.Windows.Forms.Panel();
			this.btnGenerateTask = new System.Windows.Forms.Button();
			this.panelTop.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelTop
			// 
			this.panelTop.Controls.Add(this.btnGenerateTask);
			this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelTop.Location = new System.Drawing.Point(0, 0);
			this.panelTop.Name = "panelTop";
			this.panelTop.Size = new System.Drawing.Size(521, 48);
			this.panelTop.TabIndex = 0;
			// 
			// btnGenerateTask
			// 
			this.btnGenerateTask.Location = new System.Drawing.Point(14, 10);
			this.btnGenerateTask.Name = "btnGenerateTask";
			this.btnGenerateTask.Size = new System.Drawing.Size(75, 25);
			this.btnGenerateTask.TabIndex = 7;
			this.btnGenerateTask.Text = "生成任务";
			this.btnGenerateTask.UseVisualStyleBackColor = true;
			this.btnGenerateTask.Click += new System.EventHandler(this.btnGenerateTask_Click);
			// 
			// GenerateTaskControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelTop);
			this.Name = "GenerateTaskControl";
			this.Size = new System.Drawing.Size(521, 381);
			this.panelTop.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Button btnGenerateTask;
	}
}
