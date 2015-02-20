using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

using MCS.Library.Configuration;

namespace MCS.Library.Services
{
	public sealed class ServiceMainSettings : ConfigurationSection
	{
		private const string ServiceMainConfigName = "serviceMainConfig";

		public const string SERVICE_NAME = "MCSServiceMain";

		public static ServiceMainSettings GetConfig()
		{
			ServiceMainSettings result = (ServiceMainSettings)ConfigurationBroker.GetSection(ServiceMainConfigName);

			if (result == null)
				result = new ServiceMainSettings();

			return result;
		}

		private ServiceMainSettings()
		{
		}

		public ThreadParamCollection ThreadParams
		{
			get
			{
				return ServiceThreadConfigurations.ThreadParams;
			}
		}

		[ConfigurationProperty("outputDebugString")]
		public bool OutputDebugString
		{
			get
			{
				return (bool)this["outputDebugString"];
			}
		}

		[ConfigurationProperty("serviceThreads", IsRequired = true)]
		internal ServiceThreadConfigurationCollection ServiceThreadConfigurations
		{
			get
			{
				return (ServiceThreadConfigurationCollection)this["serviceThreads"];
			}
		}

		[ConfigurationProperty("addIns")]
		public TypeConfigurationCollection AddIns
		{
			get
			{
				return (TypeConfigurationCollection)this["addIns"];
			}
		}

		public ThreadParam GetThreadParam(string name)
		{
			ServiceThreadConfigurationElement ele = ServiceThreadConfigurations[name];

			return new ThreadParam(ele);
		}
	}

	internal sealed class ServiceThreadConfigurationElement : TypeConfigurationElement
	{
		[ConfigurationProperty("enabled", DefaultValue = false)]
		public bool Enabled
		{
			get
			{
				return (bool)this["enabled"];
			}
		}

		[ConfigurationProperty("activateDuration", DefaultValue = "00:00:30")]
		public TimeSpan ActivateDuration
		{
			get
			{
				return (TimeSpan)this["activateDuration"];
			}
		}

		[ConfigurationProperty("disposeDuration", DefaultValue = "00:05:00")]
		public TimeSpan DisposeDuration
		{
			get
			{
				return (TimeSpan)this["disposeDuration"];
			}
		}

		[ConfigurationProperty("canForceStop", DefaultValue = true)]
		public bool CanForceStop
		{
			get
			{
				return (bool)this["canForceStop"];
			}
		}

		[ConfigurationProperty("batchCount", DefaultValue = 10)]
		public int BatchCount
		{
			get
			{
				return (int)this["batchCount"];
			}
		}

		[ConfigurationProperty("useDefaultLogger", DefaultValue = true)]
		public bool UseDefaultLogger
		{
			get
			{
				return (bool)this["useDefaultLogger"];
			}
		}

		[ConfigurationProperty("ownerServiceName", IsRequired = false, DefaultValue = ServiceMainSettings.SERVICE_NAME)]
		public string OwnerServiceName
		{
			get
			{
				return (string)this["ownerServiceName"];
			}
		}

		/// <summary>
		/// 任务调度
		/// </summary>
		[ConfigurationProperty("schedule")]
		public WorkScheduleConfigureElement Schedule
		{
			get
			{
				return (WorkScheduleConfigureElement)this["schedule"];
			}
		}
	}

	internal sealed class ServiceThreadConfigurationCollection : NamedConfigurationElementCollection<ServiceThreadConfigurationElement>
	{
		private ThreadParamCollection threadParams = new ThreadParamCollection();

		public ThreadParamCollection ThreadParams
		{
			get
			{
				threadParams.Clear();

				foreach (ServiceThreadConfigurationElement ele in this)
					threadParams.Add(new ThreadParam(ele));

				return threadParams;
			}
		}
	}

	public sealed class ServiceStatusSettings : ConfigurationSection
	{
		private const string ServiceStatusConfigName = "serviceStatusConfig";

		private ServiceStatusSettings()
		{
		}

		public static ServiceStatusSettings GetConfig()
		{
			ServiceStatusSettings result = (ServiceStatusSettings)ConfigurationBroker.GetSection(ServiceStatusConfigName);

			if (result == null)
				result = new ServiceStatusSettings();

			return result;
		}

		/// <summary>
		/// 表单的类型
		/// </summary>
		[ConfigurationProperty("servers")]
		public ServiceStatusConfigElementCollection Servers
		{
			get
			{
				return (ServiceStatusConfigElementCollection)this["servers"];
			}
		}

		//[ConfigurationProperty("uri", DefaultValue = "http://localhost:8989/MCS.Library.Services.ThreadStatusService")]
		//public string Uri
		//{
		//    get
		//    {
		//        return (string)this["uri"];
		//    }
		//}
	}

	/// <summary>
	/// 
	/// </summary>
	public sealed class ServiceStatusConfigElementCollection : NamedConfigurationElementCollection<ServiceStatusConfigElement>
	{

	}

	/// <summary>
	/// 申请表单类型的配置项
	/// </summary>
	public sealed class ServiceStatusConfigElement : NamedConfigurationElement
	{
		public ServiceStatusConfigElement()
		{
		}
	}
}
