using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;

namespace MCS.Web.Library.Performance
{
	[RunInstaller(true)]
	public partial class CountersInstaller : Installer
	{
		/// <summary>
		/// 
		/// </summary>
		public CountersInstaller()
		{
			InitializeComponent();
		}
	}
}