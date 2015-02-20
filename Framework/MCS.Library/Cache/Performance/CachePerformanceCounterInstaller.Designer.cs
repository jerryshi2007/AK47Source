namespace MCS.Library.Caching
{
    /// <summary>
    /// 
    /// </summary>
    partial class CachePerformanceCounterInstaller
    {

        /// <summary> 
        /// ����Ѿ����ڰ�װ��Ϣ
        /// </summary>
        /// <param name="disposing">�Ƿ����</param>
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
            this.cacheCounterInstaller.CategoryHelp = "DeluxeWorks Caching��Cache���������ָ�롣ÿһ�����͵�Cache���Ծ���һ��������ʵ����";
            this.cacheCounterInstaller.CategoryName = "DeluxeWorks Caching";
            this.cacheCounterInstaller.CategoryType = System.Diagnostics.PerformanceCounterCategoryType.MultiInstance;
            this.cacheCounterInstaller.Counters.AddRange(new System.Diagnostics.CounterCreationData[] {
            new System.Diagnostics.CounterCreationData("Cache Entries", "Ӧ���л����ڵ���������", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Cache Hits", "Ӧ�õĻ�����������", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Cache Misses", "Ӧ�õĻ���δ��������", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Cache Hit Ratio", "Ӧ�õĻ���������", System.Diagnostics.PerformanceCounterType.RawFraction),
            new System.Diagnostics.CounterCreationData("Cache Hit Ratio Base", "", System.Diagnostics.PerformanceCounterType.RawBase)});
            this.cacheCounterInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.performanceCounterInstaller_AfterInstall);
            // 
            // cacheNotifierCounterInstaller
            // 
            this.cacheNotifierCounterInstaller.CategoryHelp = "DeluxeWorks Caceh Notifier��Udp��Mmf֪ͨ������ָ�롣ÿһ�����͵�Cache���Ծ���һ��������ʵ����";
            this.cacheNotifierCounterInstaller.CategoryName = "DeluxeWorks Cache Notifier";
            this.cacheNotifierCounterInstaller.CategoryType = System.Diagnostics.PerformanceCounterCategoryType.MultiInstance;
            this.cacheNotifierCounterInstaller.Counters.AddRange(new System.Diagnostics.CounterCreationData[] {
            new System.Diagnostics.CounterCreationData("Udp Sent Items", "Udp֪ͨ�ķ��ʹ���", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Udp Received Items", "Udp֪ͨ�Ľ��մ���", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Mmf Sent Items", "Mmf֪ͨ�ķ��ʹ���", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Mmf Received Items", "Mmf֪ͨ�Ľ��մ���", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Mmf Current Pointer", "Mmf�ĵ�ǰָ��λ��", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Forwarded Udp To Mmf Items", "��Udpת����Mmf��Cache֪ͨ�ĸ���", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Udp Sent Count Per Second", "ÿ�뷢�͵�Udp֪ͨ", System.Diagnostics.PerformanceCounterType.RateOfCountsPerSecond32),
            new System.Diagnostics.CounterCreationData("Udp Received Count Per Second", "ÿ����յ�Udp֪ͨ", System.Diagnostics.PerformanceCounterType.RateOfCountsPerSecond32),
            new System.Diagnostics.CounterCreationData("Mmf Sent Count Per Second", "ÿ�뷢�͵�Mmf֪ͨ", System.Diagnostics.PerformanceCounterType.RateOfCountsPerSecond32),
            new System.Diagnostics.CounterCreationData("Mmf Received Count Per Second", "ÿ����յ�Mmf֪ͨ", System.Diagnostics.PerformanceCounterType.RateOfCountsPerSecond32),
            new System.Diagnostics.CounterCreationData("Forward Udp To Mmf Count Per Second", "ÿ���Udpת����Mmf��Cache֪ͨ�ĸ���", System.Diagnostics.PerformanceCounterType.RateOfCountsPerSecond32)});
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