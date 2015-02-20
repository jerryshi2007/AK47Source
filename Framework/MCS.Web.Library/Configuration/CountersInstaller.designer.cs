namespace MCS.Web.Library.Performance
{
	/// <summary>
	/// 
	/// </summary>
	partial class CountersInstaller
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
			this.pcInstaller = new System.Diagnostics.PerformanceCounterInstaller();
			// 
			// pcInstaller
			// 
			this.pcInstaller.CategoryHelp = "MCS Library页面性能监控指针";
			this.pcInstaller.CategoryName = "MCSLibraryPageCounters";
			this.pcInstaller.CategoryType = System.Diagnostics.PerformanceCounterCategoryType.MultiInstance;
			this.pcInstaller.Counters.AddRange(new System.Diagnostics.CounterCreationData[] {
			new System.Diagnostics.CounterCreationData("Page access count", "页面访问计数", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
			new System.Diagnostics.CounterCreationData("Page access error count", "页面访问错误计数", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
			new System.Diagnostics.CounterCreationData("Page access success count", "页面访问成功计数", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
			new System.Diagnostics.CounterCreationData("Page access success ratio", "页面访问成功率", System.Diagnostics.PerformanceCounterType.RawFraction),
			new System.Diagnostics.CounterCreationData("Page access success ratio base", "", System.Diagnostics.PerformanceCounterType.RawBase),
			new System.Diagnostics.CounterCreationData("Page access current average", "页面访问当前平均时间", System.Diagnostics.PerformanceCounterType.AverageTimer32),
			new System.Diagnostics.CounterCreationData("Page access current average base", "", System.Diagnostics.PerformanceCounterType.AverageBase),
			new System.Diagnostics.CounterCreationData("Page access total average(ms)", "页面访问总平均时间", System.Diagnostics.PerformanceCounterType.RawFraction),
			new System.Diagnostics.CounterCreationData("Page access total average base", "", System.Diagnostics.PerformanceCounterType.RawBase),
			new System.Diagnostics.CounterCreationData("Page access count per second", "每秒页面访问次数", System.Diagnostics.PerformanceCounterType.RateOfCountsPerSecond32)});
			// 
			// CountersInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
			this.pcInstaller});

		}

		#endregion

		private System.Diagnostics.PerformanceCounterInstaller pcInstaller;

	}
}