namespace MCS.Library.Services
{
    partial class MainForm
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
			this.panelBottom = new System.Windows.Forms.Panel();
			this.btnClose = new System.Windows.Forms.Button();
			this.tabControlClient = new System.Windows.Forms.TabControl();
			this.tabPageService = new System.Windows.Forms.TabPage();
			this.panelInnerBottom = new System.Windows.Forms.Panel();
			this.statusControl = new MCS.Library.Services.StatusControl();
			this.panelInnerClient = new System.Windows.Forms.Panel();
			this.textBoxLog = new System.Windows.Forms.TextBox();
			this.panelInnerTop = new System.Windows.Forms.Panel();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.panelBottom.SuspendLayout();
			this.tabControlClient.SuspendLayout();
			this.tabPageService.SuspendLayout();
			this.panelInnerBottom.SuspendLayout();
			this.panelInnerClient.SuspendLayout();
			this.panelInnerTop.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelBottom
			// 
			this.panelBottom.Controls.Add(this.btnClose);
			this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelBottom.Location = new System.Drawing.Point(0, 426);
			this.panelBottom.Name = "panelBottom";
			this.panelBottom.Size = new System.Drawing.Size(696, 34);
			this.panelBottom.TabIndex = 0;
			// 
			// btnClose
			// 
			this.btnClose.Location = new System.Drawing.Point(502, 6);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(75, 24);
			this.btnClose.TabIndex = 0;
			this.btnClose.Text = "关闭(&C)";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// tabControlClient
			// 
			this.tabControlClient.Controls.Add(this.tabPageService);
			this.tabControlClient.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlClient.Location = new System.Drawing.Point(0, 0);
			this.tabControlClient.Name = "tabControlClient";
			this.tabControlClient.SelectedIndex = 0;
			this.tabControlClient.Size = new System.Drawing.Size(696, 426);
			this.tabControlClient.TabIndex = 1;
			this.tabControlClient.SelectedIndexChanged += new System.EventHandler(this.tabControlClient_SelectedIndexChanged);
			// 
			// tabPageService
			// 
			this.tabPageService.Controls.Add(this.panelInnerBottom);
			this.tabPageService.Controls.Add(this.panelInnerClient);
			this.tabPageService.Controls.Add(this.panelInnerTop);
			this.tabPageService.Location = new System.Drawing.Point(4, 22);
			this.tabPageService.Name = "tabPageService";
			this.tabPageService.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageService.Size = new System.Drawing.Size(688, 400);
			this.tabPageService.TabIndex = 0;
			this.tabPageService.Text = "服务测试";
			this.tabPageService.UseVisualStyleBackColor = true;
			// 
			// panelInnerBottom
			// 
			this.panelInnerBottom.Controls.Add(this.statusControl);
			this.panelInnerBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelInnerBottom.Location = new System.Drawing.Point(3, 262);
			this.panelInnerBottom.Name = "panelInnerBottom";
			this.panelInnerBottom.Size = new System.Drawing.Size(682, 135);
			this.panelInnerBottom.TabIndex = 4;
			// 
			// statusControl
			// 
			this.statusControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.statusControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.statusControl.Location = new System.Drawing.Point(0, 0);
			this.statusControl.Name = "statusControl";
			this.statusControl.Size = new System.Drawing.Size(682, 135);
			this.statusControl.TabIndex = 0;
			// 
			// panelInnerClient
			// 
			this.panelInnerClient.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelInnerClient.Controls.Add(this.textBoxLog);
			this.panelInnerClient.Location = new System.Drawing.Point(6, 41);
			this.panelInnerClient.Name = "panelInnerClient";
			this.panelInnerClient.Size = new System.Drawing.Size(679, 216);
			this.panelInnerClient.TabIndex = 3;
			// 
			// textBoxLog
			// 
			this.textBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxLog.Location = new System.Drawing.Point(0, 0);
			this.textBoxLog.Multiline = true;
			this.textBoxLog.Name = "textBoxLog";
			this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxLog.Size = new System.Drawing.Size(679, 216);
			this.textBoxLog.TabIndex = 1;
			// 
			// panelInnerTop
			// 
			this.panelInnerTop.Controls.Add(this.btnStop);
			this.panelInnerTop.Controls.Add(this.btnStart);
			this.panelInnerTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelInnerTop.Location = new System.Drawing.Point(3, 3);
			this.panelInnerTop.Name = "panelInnerTop";
			this.panelInnerTop.Size = new System.Drawing.Size(682, 32);
			this.panelInnerTop.TabIndex = 0;
			// 
			// btnStop
			// 
			this.btnStop.Location = new System.Drawing.Point(108, 3);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(75, 24);
			this.btnStop.TabIndex = 1;
			this.btnStop.Text = "停止(&T)";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(14, 3);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(75, 24);
			this.btnStart.TabIndex = 0;
			this.btnStart.Text = "启动(&S)";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(696, 460);
			this.Controls.Add(this.tabControlClient);
			this.Controls.Add(this.panelBottom);
			this.Name = "MainForm";
			this.Text = "服务测试程序";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.panelBottom.ResumeLayout(false);
			this.tabControlClient.ResumeLayout(false);
			this.tabPageService.ResumeLayout(false);
			this.panelInnerBottom.ResumeLayout(false);
			this.panelInnerClient.ResumeLayout(false);
			this.panelInnerClient.PerformLayout();
			this.panelInnerTop.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.TabControl tabControlClient;
        private System.Windows.Forms.TabPage tabPageService;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.Panel panelInnerTop;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Panel panelInnerClient;
        private System.Windows.Forms.Panel panelInnerBottom;
        private StatusControl statusControl;
    }
}