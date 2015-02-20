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
	/// ����Workflow��������Ϣ
	/// </summary>
	public class WorkflowSettings : ConfigurationSection
	{
		/// <summary>
		/// �õ�Workflow��������Ϣ
		/// </summary>
		/// <returns>WorkflowSettings</returns>
		public static WorkflowSettings GetConfig()
		{
			return (WorkflowSettings)ConfigurationBroker.GetSection("workflowSettings");
		}

		/// <summary>
		/// ȱʡ�����Ӵ�����
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
		/// ���̼�����
		/// </summary>
		public IWorkflowReader Reader
		{
			get
			{
				return (IWorkflowReader)TypeFactories["reader"].CreateInstance();
			}
		}

		/// <summary>
		/// ���̵�д����
		/// </summary>
		public IWorkflowWriter Writer
		{
			get
			{
				return (IWorkflowWriter)TypeFactories["writer"].CreateInstance();
			}
		}

		/// <summary>
		/// ��Workflow��WorkItem���뵽������
		/// </summary>
		public IEnqueueWorkItem EnqueueWorkItemExecutor
		{
			get
			{
				return (IEnqueueWorkItem)TypeFactories["enqueueWorkItemExecutor"].CreateInstance();
			}
		}

		/// <summary>
		/// ��������������
		/// </summary>
		public IWfProcessDescriptorManager ProcessDescriptorManager
		{
			get
			{
				return (IWfProcessDescriptorManager)TypeFactories["processDescriptorManager"].CreateInstance();
			}
		}

		/// <summary>
		/// �������������л���
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
		/// ���л�������Ϣʱ�Ƿ����л��ڵ�������Ϣ
		/// Ĭ���ǲ����л�
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
		/// �Ƿ����л�������Ϣ(��ViewState)
		/// Ĭ�������л�
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
		/// �Ƿ�ʹ��AppDomain����Cache������ʹ��Context����Cache
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
		/// �Ƿ�ʹ��AppDomain����Cache������ʹ��Context����Cache
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
		/// AppDomain��Cache����Ч��
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
