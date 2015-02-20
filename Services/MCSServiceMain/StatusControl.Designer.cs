namespace MCS.Library.Services
{
    partial class StatusControl
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
			this.components = new System.ComponentModel.Container();
			this.listViewThreadStatus = new System.Windows.Forms.ListView();
			this.timerStatus = new System.Windows.Forms.Timer(this.components);
			this.queryStatusWorker = new System.ComponentModel.BackgroundWorker();
			this.SuspendLayout();
			// 
			// listViewThreadStatus
			// 
			this.listViewThreadStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listViewThreadStatus.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewThreadStatus.Location = new System.Drawing.Point(0, 0);
			this.listViewThreadStatus.Name = "listViewThreadStatus";
			this.listViewThreadStatus.Size = new System.Drawing.Size(502, 342);
			this.listViewThreadStatus.TabIndex = 0;
			this.listViewThreadStatus.UseCompatibleStateImageBehavior = false;
			this.listViewThreadStatus.View = System.Windows.Forms.View.Details;
			// 
			// timerStatus
			// 
			this.timerStatus.Interval = 2000;
			this.timerStatus.Tick += new System.EventHandler(this.timerStatus_Tick);
			// 
			// queryStatusWorker
			// 
			this.queryStatusWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.queryStatusWorker_DoWork);
			this.queryStatusWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.queryStatusWorker_RunWorkerCompleted);
			// 
			// StatusControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.listViewThreadStatus);
			this.Name = "StatusControl";
			this.Size = new System.Drawing.Size(502, 342);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewThreadStatus;
        private System.Windows.Forms.Timer timerStatus;
		private System.ComponentModel.BackgroundWorker queryStatusWorker;
    }
}
