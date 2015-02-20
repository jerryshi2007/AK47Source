using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Win32;

namespace MCS.Library.Services
{
	[RunInstaller(true)]
	public partial class MCSServiceInstaller : Installer
	{
		public MCSServiceInstaller()
		{
			InitializeComponent();
		}

		public override void Install(IDictionary stateSaver)
		{
			this.MCSServiceInstaller1.ServiceName = GetServiceName();
			this.MCSServiceInstaller1.DisplayName = GetDisplayName();

			base.Install(stateSaver);

			RegistryKey system = Registry.LocalMachine.OpenSubKey("System");

			RegistryKey currentControlSet = system.OpenSubKey("CurrentControlSet");

			RegistryKey services = currentControlSet.OpenSubKey("Services");

			RegistryKey service = services.OpenSubKey(this.MCSServiceInstaller1.ServiceName, true);

			service.SetValue("Description", this.MCSServiceInstaller1.Description);

			RegistryKey config = service.CreateSubKey("Parameters");
			string arguments = BuildArguments();
			config.SetValue("Arguments", BuildArguments());

			string path = service.GetValue("ImagePath") + " " + arguments;
			service.SetValue("ImagePath", path);

			//OutputParameters();
		}

		public override void Uninstall(IDictionary savedState)
		{
			this.MCSServiceInstaller1.ServiceName = GetServiceName();
			this.MCSServiceInstaller1.DisplayName = GetDisplayName();

			try
			{
				RegistryKey system = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("System");
				RegistryKey currentControlSet = system.OpenSubKey("CurrentControlSet");
				RegistryKey services = currentControlSet.OpenSubKey("Services");
				RegistryKey service = services.OpenSubKey(this.MCSServiceInstaller1.ServiceName, true);

				service.DeleteSubKeyTree("Parameters");
			}
			finally
			{
				base.Uninstall(savedState);
				//OutputParameters();
			}
		}

		private void OutputParameters()
		{
			foreach (string key in this.Context.Parameters.Keys)
			{
				Console.WriteLine("Key: {0}, Value: {1}", key, this.Context.Parameters[key]);
			}
		}

		private string GetServiceName()
		{
			string result = Context.Parameters["serviceName"];

			if (string.IsNullOrEmpty(result))
				result = this.MCSServiceInstaller1.ServiceName;

			return result;
		}

		private string GetDisplayName()
		{
			string result = Context.Parameters["displayName"];

			if (string.IsNullOrEmpty(result))
				result = this.MCSServiceInstaller1.DisplayName;

			return result;
		}

		private string BuildArguments()
		{
			StringBuilder strB = new StringBuilder();

			using (StringWriter writer = new StringWriter(strB))
			{
				writer.Write("service");

				WriteNotEmptyParameter(writer, "serviceName");
				WriteNotEmptyParameter(writer, "port");
			}

			//Console.WriteLine("Arguments={0}", strB.ToString());
			return strB.ToString();
		}

		private void WriteNotEmptyParameter(TextWriter writer, string parameterName)
		{
			if (string.IsNullOrEmpty(this.Context.Parameters[parameterName]) == false)
				writer.Write(" /{0}={1}", parameterName, this.Context.Parameters[parameterName]);
		}
	}
}