namespace MCS.Library.SOA.DataObjects.Workflow
{
	partial class WfCountersInstaller
	{
		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.wfcInstaller = new System.Diagnostics.PerformanceCounterInstaller();
			// 
			// wfcInstaller
			// 
			this.wfcInstaller.CategoryHelp = "MCSWorkflow流转性能监控指针";
			this.wfcInstaller.CategoryName = "MCSWorkflow";
			this.wfcInstaller.CategoryType = System.Diagnostics.PerformanceCounterCategoryType.MultiInstance;

			this.wfcInstaller.Counters.AddRange(new System.Diagnostics.CounterCreationData[] {
			new System.Diagnostics.CounterCreationData("WF Count", "工作流的流转次数", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
			new System.Diagnostics.CounterCreationData("WF fail count", "工作流的失败流转次数", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
			new System.Diagnostics.CounterCreationData("WF success count", "工作流的成功流转次数", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
			new System.Diagnostics.CounterCreationData("WF Average Duration(ms)", "工作流平均流转时间", System.Diagnostics.PerformanceCounterType.RawFraction),
			new System.Diagnostics.CounterCreationData("WF Average Duration Base", "", System.Diagnostics.PerformanceCounterType.RawBase),
			new System.Diagnostics.CounterCreationData("WF with Tx Average duration(ms)", "工作流流转中事务的平均时间", System.Diagnostics.PerformanceCounterType.RawFraction),
			new System.Diagnostics.CounterCreationData("WF with Tx Average duration base", "", System.Diagnostics.PerformanceCounterType.RawBase),
			new System.Diagnostics.CounterCreationData("WF Move to count per second", "工作流每秒的流转次数", System.Diagnostics.PerformanceCounterType.RateOfCountsPerSecond32)});
			// 
			// CountersInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
				this.wfcInstaller
			});
		}

		#endregion

		private System.Diagnostics.PerformanceCounterInstaller wfcInstaller;
	}
}
