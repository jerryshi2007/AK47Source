using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MCS.Library.Services
{
	/// <summary>
	/// Web Service方法调用的Performance Counter的安装类
	/// </summary>
	 [RunInstaller(true)]
	public partial class WebMethodServerCountersInstaller : Installer
	{
		/// <summary>
		/// 性能监控指针的安装类实例，用于初始化PerformanceCounters类
		/// </summary>
		public static readonly WebMethodServerCountersInstaller Instance = new WebMethodServerCountersInstaller();

		/// <summary>
		/// 
		/// </summary>
		public WebMethodServerCountersInstaller()
		{
			InitializeComponent();
		}
	}
}
