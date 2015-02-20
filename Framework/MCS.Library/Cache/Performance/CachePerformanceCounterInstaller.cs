#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	CachePerformanceCounterInstaller.cs
// Remark	��	��ʼ�����ܼ�������
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ������	    20070430		����
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
		/// ���ܼ��ָ��İ�װ��ʵ�������ڳ�ʼ��PerformanceCounters��
		/// </summary>
		public static readonly CachePerformanceCounterInstaller Instance = new CachePerformanceCounterInstaller();

        /// <summary>
        /// ��ʼ�����ܼ�������װ
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