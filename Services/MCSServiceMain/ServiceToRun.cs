using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using MCS.Library.Core;
using MCS.Library.Logging;
using System.Runtime.InteropServices;

namespace MCS.Library.Services
{
	static class ServiceToRun
	{
		[System.Runtime.InteropServices.DllImport("kernel32.dll")]
		private static extern bool AttachConsole(int dwProcessId);
		[System.Runtime.InteropServices.DllImport("kernel32.dll")]
		private static extern bool FreeConsole();

		private static readonly string HelpMessage = @"
Start Service
Command line： ServiceMain.exe [-serviceName=SERVICENAME] [-displayName=DISPLAYNAME]
Arguments:
-serviceName=SERVICENAME	Specifies the service name of the service instance.
-displayName=DISPLAYNAME	Specifies the display name of the service instance.";

		private const int ATTACH_PARENT_PROCESS = -1;
		internal const int ATTACH_CONSOLE_FAILED = -1;
		internal const int FREE_CONSOLE_FAILED = -2;

		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		static int Main(string[] args)
		{
			//#if DEBUG
			//            Debugger.Launch();
			//#endif
			ServiceBase[] ServicesToRun;

			// 同一进程中可以运行多个用户服务。若要将
			// 另一个服务添加到此进程中，请更改下行以
			// 创建另一个服务对象。例如，
			//
			//   ServicesToRun = new ServiceBase[] {new Service1(), new MySecondUserService()};
			//


			if (IsRequestHelp(args))
			{
				ShowHelp();
			}
			else
			{
				InitServiceArguments(ArgumentsParser.Parse(args));

				try
				{
					try
					{
						if (ServiceArguments.Current.Port.IsNullOrEmpty())
							RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "RemoteSettings.config", false);
						else
							ManualInitRemotingConfig();
					}
					catch (System.Runtime.Remoting.RemotingException ex)
					{
						if (ServiceArguments.Current.EntryType == ServiceEntryType.Application)
							MCSServiceMain.Instance.Log.Write(ex.Message, LogPriority.Normal, ServiceLogEventID.SERVICEMAIN_MAIN, TraceEventType.Warning, "RemotingConfiguration.Configure异常");
						else
							throw;
					}

					if (ServiceArguments.Current.EntryType == ServiceEntryType.Application)
					{
						MCSServiceMain.Instance.args = args;

						Application.Run(new MainForm(MCSServiceMain.Instance));
					}
					else
					{
						ServicesToRun = new ServiceBase[] { MCSServiceMain.Instance };

						ServiceBase.Run(ServicesToRun);
					}
				}
				catch (Exception ex)
				{
					try
					{
						MCSServiceMain.Instance.Log.Write(ex, ServiceLogEventID.SERVICEMAIN_MAIN);
					}
					catch
					{
						EventLog.WriteEntry(ServiceMainSettings.SERVICE_NAME, ex.Message, EventLogEntryType.Warning, ServiceLogEventID.SERVICEMAIN_MAIN);
					}

					if (ServiceArguments.Current.EntryType == ServiceEntryType.Application)
						MessageBox.Show(ex.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			return 0;
		}

		private static void ShowHelp()
		{
			//if (!AttachConsole(ATTACH_PARENT_PROCESS))
			//    throw new ConsoleException(ATTACH_CONSOLE_FAILED);

			//if (!FreeConsole())
			//    throw new ConsoleException(FREE_CONSOLE_FAILED);

			MessageBox.Show(HelpMessage, "帮助", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private static bool IsRequestHelp(string[] args)
		{
			bool result = false;

			for (int i = args.Length - 1; i >= 0; i--)
			{
				if (args[i].StartsWith("/?", StringComparison.Ordinal))
				{
					result = true;
					break;
				}
			}

			return result;
		}

		private static void ManualInitRemotingConfig()
		{
			HttpChannel channel = new HttpChannel(int.Parse(ServiceArguments.Current.Port));

			ChannelServices.RegisterChannel(channel, false);
			RemotingConfiguration.RegisterWellKnownServiceType(typeof(ThreadStatusService), "MCS.Library.Services.ThreadStatusService", WellKnownObjectMode.Singleton);
		}

		private static void InitServiceArguments(StringDictionary arguments)
		{
			ServiceArguments sa = ServiceArguments.Current;

			if (arguments.ContainsKey("service"))
				sa.EntryType = ServiceEntryType.Service;
			else
				sa.EntryType = ServiceEntryType.Application;

			sa.ServiceName = arguments["serviceName"];

			if (sa.ServiceName.IsNullOrEmpty())
				sa.ServiceName = ServiceMainSettings.SERVICE_NAME;

			sa.Port = arguments["port"];
		}
	}

	[Serializable]
	public class ConsoleException : Exception
	{
		private int reason = 0;

		private static string ReasonToString(int code)
		{
			switch (code)
			{
				case 0:
					return "正常";
				case ServiceToRun.ATTACH_CONSOLE_FAILED:
					return "无法附加到控制台";
				case ServiceToRun.FREE_CONSOLE_FAILED:
					return "无法释放控制台";
				default:
					return "未知错误";
			}
		}

		public ConsoleException() : base("正常") { }
		public ConsoleException(int code)
			: base(ReasonToString(code))
		{
			this.reason = code;
		}

		public ConsoleException(int code, string message)
			: base(message)
		{
			this.reason = code;
		}

		public ConsoleException(string message, Exception inner) : base(message, inner) { }
		protected ConsoleException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }

		public int ReasonCode { get { return this.reason; } }
	}
}