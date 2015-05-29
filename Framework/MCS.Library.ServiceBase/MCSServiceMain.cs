using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.IO;
using System.Runtime.Remoting;
using System.Windows.Forms;

namespace MCS.Library.Services
{
	public partial class MCSServiceMain : ServiceBase
	{
		private ServiceThreadCollection threads = new ServiceThreadCollection();
		private ManualResetEvent exitEvent = new ManualResetEvent(false);
		private ServiceEntryType entryType = ServiceEntryType.Service;
		private ServiceStatusType serviceStatus = ServiceStatusType.Stopped;
		private ServiceLog log = null;
		private static MCSServiceMain instance = new MCSServiceMain();

		public string[] args;

		private MCSServiceMain()
		{
			InitializeComponent();

			this.ServiceName = ServiceArguments.Current.ServiceName;

			this.AutoLog = false;
		}

		//public ServiceEntryType EntryType
		//{
		//    get
		//    {
		//        return this.entryType;
		//    }
		//    internal set
		//    {
		//        this.entryType = value;
		//    }
		//}

		public ServiceStatusType ServiceStatus
		{
			get
			{
				return this.serviceStatus;
			}
		}

		public event CreateThreadDelegete CreateThreadEvent;

		public static MCSServiceMain Instance
		{
			get
			{
				return instance;
			}
		}

		public ServiceLog Log
		{
			get
			{
				if (this.log == null)
					this.log = new ServiceLog(this.ServiceName);

				return this.log;
			}
		}

		public ServiceThreadCollection Threads
		{
			get
			{
				return this.threads;
			}
		}

		public void StarService()
		{
			this.exitEvent.Reset();

			OnStart(args);

			this.serviceStatus = ServiceStatusType.Running;
		}

		public void StopService()
		{
			OnStop();

			this.serviceStatus = ServiceStatusType.Stopped;
		}

		protected override void OnStart(string[] args)
		{
			try
			{
				//RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "RemoteSettings.config", false);

				CreateAllThreads();

				this.Log.Write("服务启动", GetStartupInfo(), ServiceLogEventID.SERVICEMAIN_STARTUPINFO);

				this.threads.StartAllThreads();
			}
			catch (Exception ex)
			{
				this.Log.Write(ex, ServiceLogEventID.SERVICEMAIN_ONSTART);

				throw;
			}
		}

		protected override void OnStop()
		{
			this.exitEvent.Set();

			this.threads.AbortAllThreads();
			this.threads.Clear();

			if (ServiceArguments.Current.EntryType == ServiceEntryType.Service)
				this.Log.Write("服务停止", "服务停止", ServiceLogEventID.SERVICEMAIN_ONSTOP);
		}

		private void CreateAllThreads()
		{
			ThreadParamCollection threadParams = GetAllThreadParams();

			for (int i = 0; i < threadParams.Count; i++)
			{
				ThreadParam tp = threadParams[i];

				if (string.Compare(tp.OwnerServiceName, this.ServiceName, true) == 0)
					AddThread(tp);
			}
		}

		private void AddThread(ThreadParam tp)
		{
			try
			{
				tp.ExitEvent = this.exitEvent;
				tp.EntryType = this.entryType;

				if (tp.ThreadTask != null)
				{
					tp.ThreadTask.Params = tp;

					this.threads.Add(ServiceThread.CreateThread(tp, CreateThreadEvent));
				}
			}
			catch (Exception ex)
			{
				this.Log.Write(string.Format("载入线程\"{0}\"出错", tp.Name), ex, ServiceLogEventID.SERVICEMAIN_ADDTHREAD);

				throw;
			}
		}

		private ThreadParamCollection GetAllThreadParams()
		{
			return ServiceMainSettings.GetConfig().ThreadParams;
		}

		private string GetStartupInfo()
		{
			StringBuilder strB = new StringBuilder(1024);

			StringWriter sw = new StringWriter(strB);

			try
			{
				sw.WriteLine("服务启动");
				sw.WriteLine("应用程序的根目录：{0}", AppDomain.CurrentDomain.SetupInformation.ApplicationBase);

				foreach (ServiceThread thread in this.threads)
					sw.WriteLine("\t Thread： {0}, Status： {1} ", thread.Params.Name, thread.Status);
			}
			finally
			{
				sw.Close();
			}

			return strB.ToString();
		}
	}
}
