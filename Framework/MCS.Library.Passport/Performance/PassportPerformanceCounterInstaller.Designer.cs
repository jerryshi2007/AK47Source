namespace MCS.Library.Passport.Performance
{
	partial class PassportPerformanceCounterInstaller
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
			this.signInPerformanceCounter = new System.Diagnostics.PerformanceCounterInstaller();

			this.signInPerformanceCounter.CategoryHelp = "PassportService的认证过程性能监控指针";
			this.signInPerformanceCounter.CategoryName = "PassportService";
			this.signInPerformanceCounter.CategoryType = System.Diagnostics.PerformanceCounterCategoryType.MultiInstance;

			this.signInPerformanceCounter.Counters.AddRange(new System.Diagnostics.CounterCreationData[] {
			new System.Diagnostics.CounterCreationData("SignIn Count", "认证次数", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
			new System.Diagnostics.CounterCreationData("SignIn Fail Count", "认证失败次数", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
			new System.Diagnostics.CounterCreationData("SignIn Success Count", "认证成功次数", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
			new System.Diagnostics.CounterCreationData("SignIn Average Duration(ms)", "平均认证时间", System.Diagnostics.PerformanceCounterType.RawFraction),
			new System.Diagnostics.CounterCreationData("SignIn Average Duration Base", "", System.Diagnostics.PerformanceCounterType.RawBase),
			new System.Diagnostics.CounterCreationData("SignIn Count Per Second", "每秒的认证次数", System.Diagnostics.PerformanceCounterType.RateOfCountsPerSecond32)
			});

			// 
			// CountersInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
				this.signInPerformanceCounter
			});
		}

		#endregion

		internal System.Diagnostics.PerformanceCounterInstaller signInPerformanceCounter;
	}
}