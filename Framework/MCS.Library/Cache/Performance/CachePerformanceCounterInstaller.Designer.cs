namespace MCS.Library.Caching
{
    /// <summary>
    /// 
    /// </summary>
    partial class CachePerformanceCounterInstaller
    {

        /// <summary> 
        /// 清空已经存在安装信息
        /// </summary>
        /// <param name="disposing">是否清空</param>
        protected override void Dispose(bool disposing)
        {
            //if (disposing && (this.components != null))
            //{
            //    this.components.Dispose();
            //}

            if (disposing && (this.cacheCounterInstaller != null))
            {
                this.cacheCounterInstaller.Dispose();
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
            this.cacheCounterInstaller = new System.Diagnostics.PerformanceCounterInstaller();
            this.cacheNotifierCounterInstaller = new System.Diagnostics.PerformanceCounterInstaller();
            // 
            // cacheCounterInstaller
            // 
            this.cacheCounterInstaller.CategoryHelp = "DeluxeWorks Caching是Cache处理的性能指针。每一种类型的Cache可以具有一个独立的实例。";
            this.cacheCounterInstaller.CategoryName = "DeluxeWorks Caching";
            this.cacheCounterInstaller.CategoryType = System.Diagnostics.PerformanceCounterCategoryType.MultiInstance;
            this.cacheCounterInstaller.Counters.AddRange(new System.Diagnostics.CounterCreationData[] {
            new System.Diagnostics.CounterCreationData("Cache Entries", "应用中缓存内的总项数。", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Cache Hits", "应用的缓存命中数。", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Cache Misses", "应用的缓存未命中数。", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Cache Hit Ratio", "应用的缓存命中率", System.Diagnostics.PerformanceCounterType.RawFraction),
            new System.Diagnostics.CounterCreationData("Cache Hit Ratio Base", "", System.Diagnostics.PerformanceCounterType.RawBase)});
            this.cacheCounterInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.performanceCounterInstaller_AfterInstall);
            // 
            // cacheNotifierCounterInstaller
            // 
            this.cacheNotifierCounterInstaller.CategoryHelp = "DeluxeWorks Caceh Notifier是Udp、Mmf通知的性能指针。每一种类型的Cache可以具有一个独立的实例。";
            this.cacheNotifierCounterInstaller.CategoryName = "DeluxeWorks Cache Notifier";
            this.cacheNotifierCounterInstaller.CategoryType = System.Diagnostics.PerformanceCounterCategoryType.MultiInstance;
            this.cacheNotifierCounterInstaller.Counters.AddRange(new System.Diagnostics.CounterCreationData[] {
            new System.Diagnostics.CounterCreationData("Udp Sent Items", "Udp通知的发送次数", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Udp Received Items", "Udp通知的接收次数", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Mmf Sent Items", "Mmf通知的发送次数", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Mmf Received Items", "Mmf通知的接收次数", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Mmf Current Pointer", "Mmf的当前指针位置", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Forwarded Udp To Mmf Items", "从Udp转发到Mmf的Cache通知的个数", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Udp Sent Count Per Second", "每秒发送的Udp通知", System.Diagnostics.PerformanceCounterType.RateOfCountsPerSecond32),
            new System.Diagnostics.CounterCreationData("Udp Received Count Per Second", "每秒接收的Udp通知", System.Diagnostics.PerformanceCounterType.RateOfCountsPerSecond32),
            new System.Diagnostics.CounterCreationData("Mmf Sent Count Per Second", "每秒发送的Mmf通知", System.Diagnostics.PerformanceCounterType.RateOfCountsPerSecond32),
            new System.Diagnostics.CounterCreationData("Mmf Received Count Per Second", "每秒接收的Mmf通知", System.Diagnostics.PerformanceCounterType.RateOfCountsPerSecond32),
            new System.Diagnostics.CounterCreationData("Forward Udp To Mmf Count Per Second", "每秒从Udp转发到Mmf的Cache通知的个数", System.Diagnostics.PerformanceCounterType.RateOfCountsPerSecond32)});
            this.cacheNotifierCounterInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.performanceCounterInstaller_AfterInstall);
            // 
            // CachePerformanceCounterInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.cacheCounterInstaller,
            this.cacheNotifierCounterInstaller});

        }

        #endregion

        internal System.Diagnostics.PerformanceCounterInstaller cacheCounterInstaller;
        internal System.Diagnostics.PerformanceCounterInstaller cacheNotifierCounterInstaller;
    }
}