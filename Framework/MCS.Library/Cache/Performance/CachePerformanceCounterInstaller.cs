#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	CachePerformanceCounterInstaller.cs
// Remark	：	初始化性能计数器类
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    万振龙	    20070430		创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;

namespace MCS.Library.Caching
{
    [RunInstaller(true)]
    public partial class CachePerformanceCounterInstaller : Installer
    {
		/// <summary>
		/// 性能监控指针的安装类实例，用于初始化PerformanceCounters类
		/// </summary>
		public static readonly CachePerformanceCounterInstaller Instance = new CachePerformanceCounterInstaller();

        /// <summary>
        /// 初始化性能计数器安装
        /// </summary>
        public CachePerformanceCounterInstaller()
        {
            InitializeComponent();
        }

		private void performanceCounterInstaller_AfterInstall(object sender, InstallEventArgs e)
		{

		}
    }
}