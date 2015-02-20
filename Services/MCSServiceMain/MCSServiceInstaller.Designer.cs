namespace MCS.Library.Services
{
    partial class MCSServiceInstaller
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
			this.MCSServiceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
			this.MCSServiceInstaller1 = new System.ServiceProcess.ServiceInstaller();
			// 
			// MCSServiceProcessInstaller1
			// 
			this.MCSServiceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			this.MCSServiceProcessInstaller1.Password = null;
			this.MCSServiceProcessInstaller1.Username = null;
			// 
			// MCSServiceInstaller1
			// 
			this.MCSServiceInstaller1.DisplayName = "MCS后台服务";
			this.MCSServiceInstaller1.ServiceName = "MCSServiceMain";
			this.MCSServiceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
			// 
			// MCSServiceInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.MCSServiceProcessInstaller1,
            this.MCSServiceInstaller1});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller MCSServiceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller MCSServiceInstaller1;
    }
}