using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using MCS.Library.Configuration;
using MCS.Library.Workflow.Engine;
using MCS.Library.Workflow.Descriptors;

namespace MCS.Library.Workflow.Configuration
{
	/// <summary>
	/// 关于Workflow的配置信息
	/// </summary>
	public class WorkflowSettings : ConfigurationSection
	{
		/// <summary>
		/// 得到Workflow的配置信息
		/// </summary>
		/// <returns>WorkflowSettings</returns>
		public static WorkflowSettings GetConfig()
		{
			return (WorkflowSettings)ConfigurationBroker.GetSection("workflowSettings");
		}

		/// <summary>
		/// 缺省的连接串名称
		/// </summary>
		[ConfigurationProperty("connectionName", DefaultValue = "WFConnString")]
		public string ConnectionName
		{
			get
			{
				return (string)this["connectionName"];
			}
		}

		/// <summary>
		/// 流程加载器
		/// </summary>
		public IWorkflowReader Reader
		{
			get
			{
				return (IWorkflowReader)TypeFactories["reader"].CreateInstance();
			}
		}

		/// <summary>
		/// 流程的写入器
		/// </summary>
		public IWorkflowWriter Writer
		{
			get
			{
				return (IWorkflowWriter)TypeFactories["writer"].CreateInstance();
			}
		}

		/// <summary>
		/// 将Workflow的WorkItem加入到队列中
		/// </summary>
		public IEnqueueWorkItem EnqueueWorkItemExecutor
		{
			get
			{
				return (IEnqueueWorkItem)TypeFactories["enqueueWorkItemExecutor"].CreateInstance();
			}
		}

		/// <summary>
		/// 流程描述管理器
		/// </summary>
		public IWfProcessDescriptorManager ProcessDescriptorManager
		{
			get
			{
				return (IWfProcessDescriptorManager)TypeFactories["processDescriptorManager"].CreateInstance();
			}
		}

		/// <summary>
		/// 流程描述的序列化器
		/// </summary>
		public IProcessDescriptorSerializer ProcessDescriptorSerializer
		{
			get
			{
				return (IProcessDescriptorSerializer)TypeFactories["processDescriptorSerializer"].CreateInstance();
			}
		}

		[ConfigurationProperty("typeFactories", IsRequired = true)]
		private TypeConfigurationCollection TypeFactories
		{
			get
			{
				return (TypeConfigurationCollection)this["typeFactories"];
			}
		}

		/// <summary>
		/// 序列化流程信息时是否序列化节点描述信息
		/// 默认是不序列化
		/// </summary>
		[ConfigurationProperty("isSerializeDesc", DefaultValue = "False")]
		public bool IsSerializeDesc
		{
			get
			{
				return (bool)this["isSerializeDesc"];
			}
		}

		/// <summary>
		/// 是否序列化流程信息(如ViewState)
		/// 默认是序列化
		/// </summary>
		[ConfigurationProperty("isSerializeProcess", DefaultValue = "True")]
		public bool IsSerializeProcess
		{
			get
			{
				return (bool)this["isSerializeProcess"];
			}
		}

		/// <summary>
		/// 是否使用AppDomain级的Cache，否则使用Context级的Cache
		/// </summary>
		[ConfigurationProperty("useGlobalCache", DefaultValue = "False")]
		public bool UseGlobalCache
		{
			get
			{
				return (bool)this["useGlobalCache"];
			}
		}

		/// <summary>
		/// 是否使用AppDomain级的Cache，否则使用Context级的Cache
		/// </summary>
		[ConfigurationProperty("persistDescriptor", DefaultValue = "False")]
		public bool PersistDescriptor
		{
			get
			{
				return (bool)this["persistDescriptor"];
			}
		}

		/// <summary>
		/// AppDomain级Cache的有效期
		/// </summary>
		[ConfigurationProperty("globalCacheTimeOut", DefaultValue = "00:30:00")]
		public TimeSpan GlobalCacheTimeOut
		{
			get
			{
				return (TimeSpan)this["globalCacheTimeOut"];
			}
		}
	}
}
