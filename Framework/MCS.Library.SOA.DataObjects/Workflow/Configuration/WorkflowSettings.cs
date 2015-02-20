using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public sealed class WorkflowSettings : ConfigurationSection
	{
		private IWfProcessDescriptorManager _DescriptorManager = null;
		private IWfProcessPersistManager _PersistManager = null;

		private object _DescriptorManagerSyncObj = new object();
		private object _PersistManagerSyncObj = new object();

		public static WorkflowSettings GetConfig()
		{
			return (WorkflowSettings)ConfigurationBroker.GetSection("workflowSettings", true);
		}

		[ConfigurationProperty("connectionName", DefaultValue = "HB2008", IsRequired = false)]
		public string ConnectionName
		{
			get
			{
				return (string)this["connectionName"];
			}
		}

		/// <summary>
		/// 序列化流程定义时，是否输出shortType属性
		/// </summary>
		[ConfigurationProperty("outputShortType", DefaultValue = true)]
		public bool OutputShortType
		{
			get
			{
				return (bool)this["outputShortType"];
			}
		}

		/// <summary>
		/// 流程存储是否压缩
		/// </summary>
		[ConfigurationProperty("saveRelativeData", DefaultValue = true, IsRequired = false)]
		public bool SaveRelativeData
		{
			get
			{
				return (bool)this["saveRelativeData"];
			}
		}

		/// <summary>
		/// 流程存储是否压缩
		/// </summary>
		[ConfigurationProperty("compressed", DefaultValue = true, IsRequired = false)]
		public bool Compressed
		{
			get
			{
				return (bool)this["compressed"];
			}
		}

		/// <summary>
		/// 是否启用流程活动点上计划时间
		/// </summary>
		[ConfigurationProperty("useActivityPlanTime", DefaultValue = true)]
		public bool UseActivityPlanTime
		{
			get
			{
				return (bool)this["useActivityPlanTime"];
			}
		}

		public IWfProcessDescriptorManager GetDescriptorManager()
		{
			if (this._DescriptorManager == null)
			{
				lock (this._DescriptorManagerSyncObj)
				{
					if (this._DescriptorManager == null)
						this._DescriptorManager = GetTypeInstance<IWfProcessDescriptorManager>("processDescriptorManager");
				}
			}

			return this._DescriptorManager;
		}

		public IWfProcessPersistManager GetPersistManager()
		{
			if (this._PersistManager == null)
			{
				lock (this._PersistManagerSyncObj)
				{
					if (this._PersistManager == null)
						this._PersistManager = GetTypeInstance<IWfProcessPersistManager>("processPersistManager");
				}
			}

			return this._PersistManager;
		}

		private T GetTypeInstance<T>(string factoryName)
		{
			factoryName.CheckStringIsNullOrEmpty("factoryName");

			TypeConfigurationElement factoryElement = (TypeConfigurationElement)TypeFactories[factoryName];

			factoryElement.NullCheck<SystemSupportException>("不能在配置节workflowSettings/typeFactories找到{0}的配置项", factoryName);

			return (T)factoryElement.CreateInstance();
		}

		[ConfigurationProperty("typeFactories")]
		private TypeConfigurationCollection TypeFactories
		{
			get
			{
				return (TypeConfigurationCollection)this["typeFactories"];
			}
		}
	}
}
