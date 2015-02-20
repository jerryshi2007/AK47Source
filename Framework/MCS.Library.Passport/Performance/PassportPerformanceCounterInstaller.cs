using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace MCS.Library.Passport.Performance
{
	/// <summary>
	/// 
	/// </summary>
	[RunInstaller(true)]
	public partial class PassportPerformanceCounterInstaller : System.Configuration.Install.Installer
	{
		/// <summary>
		/// 全局实例
		/// </summary>
		public static readonly PassportPerformanceCounterInstaller Instance = new PassportPerformanceCounterInstaller();

		/// <summary>
		/// 
		/// </summary>
		public PassportPerformanceCounterInstaller()
		{
			InitializeComponent();
		}
	}
}
