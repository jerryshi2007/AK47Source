using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Threading;
using System.Collections;
using System.Diagnostics;

using MCS.Library.Core;
using System.Web.Script.Serialization;

namespace MCS.Library.Services
{
	/// <summary>
	/// �̲߳���
	/// </summary>
	[Serializable]
	public class ThreadParam : IThreadParam
	{
		private string name = string.Empty;
		private string description = string.Empty;
		private bool canForceStop = true;
		private int batchCount = 10;
		private bool useDefaultLogger = true;
		private string ownerServiceName = string.Empty;

		[NonSerialized]
		private ServiceLog log;

		private ServiceEntryType entryType = ServiceEntryType.Service;
		private TimeSpan activateDuration = TimeSpan.MaxValue;
		private TimeSpan disposeDuration = TimeSpan.MaxValue;

		[NonSerialized]
		private ManualResetEvent exitEvent = null;

		[NonSerialized]
		private ThreadTaskBase threadTask = null;
		//private string loggerName = this.name;

		#region IThreadParam ��Ա

		/// <summary>
		/// �߳�����
		/// </summary>
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>
		/// �߳���������
		/// </summary>
		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		/// <summary>
		/// ��־����
		/// </summary>
		public ServiceLog Log
		{
			get
			{
				//try
				//{
				if (this.log == null)
					this.log = new ServiceLog(this.Name, this.useDefaultLogger);
				//}
				//catch (Exception ex)
				//{
				//    string strMsg = ex.Message;

				//    if (string.IsNullOrEmpty(ex.StackTrace) == false)
				//        strMsg += "\n" + ex.StackTrace;

				//    try
				//    {
				//        EventLog.WriteEntry(ServiceMainSettings.SERVICE_NAME, strMsg, EventLogEntryType.Warning, ServiceLogEventID.SERVICEBASE_CREATELOGGER);
				//    }
				//    catch (Exception)
				//    {
				//    }
				//    finally
				//    {
				//        this.log = new ServiceLog(ServiceMainSettings.SERVICE_NAME);
				//    }
				//}

				return this.log;
			}
			set
			{
				ExceptionHelper.FalseThrow(value != null, "Log���Բ���Ϊ��");

				this.log = value;
			}
		}

		[ScriptIgnore]
		public string OwnerServiceName
		{
			get
			{
				return this.ownerServiceName;
			}
			set
			{
				this.ownerServiceName = value;
			}
		}

		/// <summary>
		/// �����̵߳��������
		/// </summary>
		public ServiceEntryType EntryType
		{
			get
			{
				return this.entryType;
			}
			set
			{
				this.entryType = value;
			}
		}

		/// <summary>
		/// ��ѯ��ʱ����
		/// </summary>
		public TimeSpan ActivateDuration
		{
			get
			{
				return this.activateDuration;
			}
			set
			{
				this.activateDuration = value;
			}
		}

		/// <summary>
		/// �ͷ���Դ��ʱ���������磬��ʱ��Ϣ���͹����У���ʱ������ʹ�õ�COM���
		/// </summary>
		public TimeSpan DisposeDuration
		{
			get
			{
				return this.disposeDuration;
			}
			set
			{
				this.disposeDuration = value;
			}
		}

		/// <summary>
		/// ���߳������Ƿ����ǿ����ֹ
		/// </summary>
		public bool CanForceStop
		{
			get
			{
				return this.canForceStop;
			}
			set
			{
				this.canForceStop = value;
			}
		}

		/// <summary>
		/// �߳�����һ��ִ�д���ļ�¼����ȱʡΪ10
		/// </summary>
		public int BatchCount
		{
			get
			{
				return this.batchCount;
			}
			set
			{
				this.batchCount = value;
			}
		}

		/// <summary>
		/// �Ƿ�ʹ�÷�����ȱʡ����־Logger
		/// </summary>
		public bool UseDefaultLogger
		{
			get
			{
				return this.useDefaultLogger;
			}
			set
			{
				this.useDefaultLogger = value;
			}
		}

		public WorkScheduleConfigureElement Schedule
		{
			get
			{
				return ServiceElement.Schedule;
			}
		}

		internal ServiceThreadConfigurationElement ServiceElement
		{
			get
			{
				ServiceThreadConfigurationElement result = ServiceMainSettings.GetConfig().ServiceThreadConfigurations[this.Name];

				if (result == null)
					result = new ServiceThreadConfigurationElement();

				return result;
			}
		}

		public bool Enabled
		{
			get
			{
				return ServiceElement.Enabled;
			}
		}
		#endregion

		/// <summary>
		/// �߳�����
		/// </summary>
		public ThreadTaskBase ThreadTask
		{
			get
			{
				return this.threadTask;
			}
		}

		/// <summary>
		/// �̵߳��˳��¼����������������ṩ
		/// </summary>
		public ManualResetEvent ExitEvent
		{
			get
			{
				return this.exitEvent;
			}
			set
			{
				this.exitEvent = value;
			}
		}

		internal ThreadParam(ServiceThreadConfigurationElement ele)
		{
			this.name = ele.Name;
			this.description = ele.Description;
			this.activateDuration = ele.ActivateDuration;
			this.disposeDuration = ele.DisposeDuration;
			this.canForceStop = ele.CanForceStop;
			this.batchCount = ele.BatchCount;
			this.useDefaultLogger = ele.UseDefaultLogger;
			this.ownerServiceName = ele.OwnerServiceName;

			try
			{
				this.threadTask = (ThreadTaskBase)ele.CreateInstance();
			}
			catch (Exception ex)
			{
				this.Log.Write(string.Format("�����߳�����\"{0}\"����", this.Name), ex, ServiceLogEventID.SERVICEBASE_CREATETHREADPARAM);
			}
		}

		/// <summary>
		/// ֱ�Ӵ����̲߳������󣬲������ù���
		/// </summary>
		/// <param name="name">�߳�����</param>
		public ThreadParam(string name)
		{
			this.name = name;
			this.description = name;

			//this.ActivateDuration = new TimeSpan(0, 0, 5);
			//this.DisposeDuration = new TimeSpan(0, 5, 0);

			this.log = new ServiceLog(name);

			this.exitEvent = new ManualResetEvent(false);
		}
	}

	/// <summary>
	/// �����߳�����Ļ���
	/// </summary>
	public class ThreadTaskBase : IDisposable
	{
		private ThreadParam threadParam = null;

		public ThreadParam Params
		{
			get
			{
				return this.threadParam;
			}
			set
			{
				this.threadParam = value;
			}
		}

		#region IDisposable ��Ա

		public virtual void Dispose()
		{
			//TODO:
		}

		#endregion

		public virtual void Initialize()
		{

		}

		/// <summary>
		/// �߳����񼤻�ʱ����
		/// </summary>
		public virtual void OnThreadTaskStart()
		{

		}
	}

	public class ThreadParamCollection : CollectionBase
	{
		public void Add(ThreadParam tp)
		{
			InnerList.Add(tp);
		}

		public void Remove(ThreadParam tp)
		{
			InnerList.Remove(tp);
		}

		public ThreadParam this[int index]
		{
			get
			{
				return (ThreadParam)InnerList[index];
			}
		}
	}
}
