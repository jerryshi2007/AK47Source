namespace MCS.Library.Services
{
    partial class WebMethodServerCountersInstaller
    {
        internal System.Diagnostics.PerformanceCounterInstaller webMethodServerCounterInstaller;
        internal System.Diagnostics.EventLogInstaller webMethodEventLogInstaller;

        /// <summary> 
        /// 清空已经存在安装信息
        /// </summary>
        /// <param name="disposing">是否清空</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.webMethodServerCounterInstaller != null))
            {
                this.webMethodServerCounterInstaller.Dispose();
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
            this.webMethodServerCounterInstaller = new System.Diagnostics.PerformanceCounterInstaller();
            this.webMethodEventLogInstaller = new System.Diagnostics.EventLogInstaller();
            // 
            // webMethodServerCounterInstaller
            // 
            this.webMethodServerCounterInstaller.CategoryHelp = "DeluxeWorks WebMethod是Web Service方法调用的性能指针。每一个方法可以具有一个独立的实例。";
            this.webMethodServerCounterInstaller.CategoryName = "DeluxeWorks WebMethod";
            this.webMethodServerCounterInstaller.CategoryType = System.Diagnostics.PerformanceCounterCategoryType.MultiInstance;
            this.webMethodServerCounterInstaller.Counters.AddRange(new System.Diagnostics.CounterCreationData[] {
            new System.Diagnostics.CounterCreationData("Request count", "总请求数。", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Request fail count", "请求失败的次数。", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Request success count", "请求成功的次数。", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Requests per second", "每秒调用次数", System.Diagnostics.PerformanceCounterType.RateOfCountsPerSecond32),
            new System.Diagnostics.CounterCreationData("Request average duration(ms)", "", System.Diagnostics.PerformanceCounterType.RawFraction),
            new System.Diagnostics.CounterCreationData("Request average duration base", "平均执行时间的基数", System.Diagnostics.PerformanceCounterType.RawBase)});
            // 
            // webMethodEventLogInstaller
            // 
            this.webMethodEventLogInstaller.CategoryCount = 0;
            this.webMethodEventLogInstaller.CategoryResourceFile = null;
            this.webMethodEventLogInstaller.Log = "HB2008";
            this.webMethodEventLogInstaller.MessageResourceFile = null;
            this.webMethodEventLogInstaller.ParameterResourceFile = null;
            this.webMethodEventLogInstaller.Source = "HB2008";
            // 
            // WebMethodServerCountersInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.webMethodServerCounterInstaller,
            this.webMethodEventLogInstaller});

        }

        #endregion
    }
}
