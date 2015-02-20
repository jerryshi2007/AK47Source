using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Configuration.Install;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[RunInstaller(true)]
	public partial class WfCountersInstaller : Installer
	{
		public WfCountersInstaller()
		{
			InitializeComponent();
		}
	}
}
