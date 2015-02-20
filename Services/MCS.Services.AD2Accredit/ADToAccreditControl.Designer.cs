namespace MCS.Services.AD2Accredit
{
	partial class ADToAccreditControl
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ADToAccreditControl));
			this.buttonConvert = new System.Windows.Forms.Button();
			this.panelTop = new System.Windows.Forms.Panel();
			this.labelTitle = new System.Windows.Forms.Label();
			this.imageListTree = new System.Windows.Forms.ImageList(this.components);
			this.buttonRefresh = new System.Windows.Forms.Button();
			this.textBoxLog = new System.Windows.Forms.TextBox();
			this.panelRight = new System.Windows.Forms.Panel();
			this.panelClient = new System.Windows.Forms.Panel();
			this.treeView = new System.Windows.Forms.TreeView();
			this.labelStatus = new System.Windows.Forms.Label();
			this.panelTop.SuspendLayout();
			this.panelRight.SuspendLayout();
			this.panelClient.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonConvert
			// 
			this.buttonConvert.Location = new System.Drawing.Point(15, 69);
			this.buttonConvert.Name = "buttonConvert";
			this.buttonConvert.Size = new System.Drawing.Size(91, 23);
			this.buttonConvert.TabIndex = 1;
			this.buttonConvert.Text = "&Convert";
			this.buttonConvert.UseVisualStyleBackColor = true;
			this.buttonConvert.Click += new System.EventHandler(this.buttonConvert_Click);
			// 
			// panelTop
			// 
			this.panelTop.Controls.Add(this.labelTitle);
			this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelTop.Location = new System.Drawing.Point(0, 0);
			this.panelTop.Name = "panelTop";
			this.panelTop.Size = new System.Drawing.Size(406, 36);
			this.panelTop.TabIndex = 4;
			// 
			// labelTitle
			// 
			this.labelTitle.AutoSize = true;
			this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.labelTitle.Location = new System.Drawing.Point(16, 10);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(163, 20);
			this.labelTitle.TabIndex = 0;
			this.labelTitle.Text = "AD OUs And Users";
			// 
			// imageListTree
			// 
			this.imageListTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTree.ImageStream")));
			this.imageListTree.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListTree.Images.SetKeyName(0, "ou.gif");
			this.imageListTree.Images.SetKeyName(1, "user.gif");
			this.imageListTree.Images.SetKeyName(2, "group.gif");
			this.imageListTree.Images.SetKeyName(3, "disabledUser.gif");
			// 
			// buttonRefresh
			// 
			this.buttonRefresh.Location = new System.Drawing.Point(15, 25);
			this.buttonRefresh.Name = "buttonRefresh";
			this.buttonRefresh.Size = new System.Drawing.Size(91, 23);
			this.buttonRefresh.TabIndex = 0;
			this.buttonRefresh.Text = "&Refresh Tree";
			this.buttonRefresh.UseVisualStyleBackColor = true;
			this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
			// 
			// textBoxLog
			// 
			this.textBoxLog.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.textBoxLog.Location = new System.Drawing.Point(0, 265);
			this.textBoxLog.Multiline = true;
			this.textBoxLog.Name = "textBoxLog";
			this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxLog.Size = new System.Drawing.Size(406, 105);
			this.textBoxLog.TabIndex = 7;
			// 
			// panelRight
			// 
			this.panelRight.Controls.Add(this.labelStatus);
			this.panelRight.Controls.Add(this.buttonConvert);
			this.panelRight.Controls.Add(this.buttonRefresh);
			this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.panelRight.Location = new System.Drawing.Point(406, 0);
			this.panelRight.Name = "panelRight";
			this.panelRight.Size = new System.Drawing.Size(120, 370);
			this.panelRight.TabIndex = 5;
			// 
			// panelClient
			// 
			this.panelClient.Controls.Add(this.treeView);
			this.panelClient.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelClient.Location = new System.Drawing.Point(0, 36);
			this.panelClient.Name = "panelClient";
			this.panelClient.Size = new System.Drawing.Size(406, 229);
			this.panelClient.TabIndex = 8;
			// 
			// treeView
			// 
			this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView.ImageIndex = 0;
			this.treeView.ImageList = this.imageListTree;
			this.treeView.Location = new System.Drawing.Point(0, 0);
			this.treeView.Name = "treeView";
			this.treeView.SelectedImageIndex = 0;
			this.treeView.Size = new System.Drawing.Size(406, 229);
			this.treeView.TabIndex = 2;
			this.treeView.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterExpand);
			// 
			// labelStatus
			// 
			this.labelStatus.AutoSize = true;
			this.labelStatus.Location = new System.Drawing.Point(12, 146);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(0, 13);
			this.labelStatus.TabIndex = 2;
			// 
			// ADToAccreditControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelClient);
			this.Controls.Add(this.panelTop);
			this.Controls.Add(this.textBoxLog);
			this.Controls.Add(this.panelRight);
			this.Name = "ADToAccreditControl";
			this.Size = new System.Drawing.Size(526, 370);
			this.panelTop.ResumeLayout(false);
			this.panelTop.PerformLayout();
			this.panelRight.ResumeLayout(false);
			this.panelRight.PerformLayout();
			this.panelClient.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonConvert;
		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Label labelTitle;
		private System.Windows.Forms.ImageList imageListTree;
		private System.Windows.Forms.Button buttonRefresh;
		private System.Windows.Forms.TextBox textBoxLog;
		private System.Windows.Forms.Panel panelRight;
		private System.Windows.Forms.Panel panelClient;
		private System.Windows.Forms.TreeView treeView;
		private System.Windows.Forms.Label labelStatus;
	}
}
