using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;
using MCS.Library.Core;
using System.Collections;
using MCS.Library.Logging;
using System.Diagnostics;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 工作流Activity配置
	/// </summary>
	public sealed class WfActivitySettings : ConfigurationSection
	{
		private Hashtable _Context = new Hashtable();

		public static WfActivitySettings GetConfig()
		{
			WfActivitySettings settings = (WfActivitySettings)ConfigurationBroker.GetSection("wfActivitySettings");

			if (settings == null)
				settings = new WfActivitySettings();

			return settings;
		}

		public Hashtable Context
		{
			get { return _Context; }
		}

		public WfActivityBuilderBase GetActivityBuilder(IWfActivityDescriptor actDesp)
		{
			(actDesp != null).FalseThrow<ArgumentNullException>("actDesp");

			return GetActivityBuilder(actDesp.ActivityType);
		}

		public WfActivityBuilderBase GetActivityBuilder(WfActivityType activityType)
		{
			WfActivityConfigurationElement activityElement = Activities[activityType.ToString()];

			(activityElement != null).FalseThrow<SettingsPropertyNotFoundException>("不能根据{0}找到对应的ActivityBuilder", activityType);

			return (WfActivityBuilderBase)activityElement.CreateInstance();
		}

		[ConfigurationProperty("propertyGroups", IsRequired = false)]
		public PropertyGroupConfigurationElementCollection PropertyGroups
		{
			get
			{
				return (PropertyGroupConfigurationElementCollection)this["propertyGroups"];
			}
		}

		[ConfigurationProperty("activities", IsRequired = false)]
		public WfActivityConfigurationCollection Activities
		{
			get
			{
				return (WfActivityConfigurationCollection)this["activities"];
			}
		}

		/// <summary>
		/// 中止流程的操作类
		/// </summary>
		/// <returns></returns>
		public WfActionCollection GetCancelProcessActions()
		{
			return WfActivityConfigurationElement.GetActions(this.CancelProcessActionsString);
		}

		/// <summary>
		/// 恢复终止流程的操作类
		/// </summary>
		/// <returns></returns>
		public WfActionCollection GetRestoreProcessActions()
		{
			return WfActivityConfigurationElement.GetActions(this.RestoreProcessActionsString);
		}

		/// <summary>
		/// 流程撤回的操作类
		/// </summary>
		/// <returns></returns>
		public WfActionCollection GetWithdrawActions()
		{
			return WfActivityConfigurationElement.GetActions(this.WithdrawActionsString);
		}

		/// <summary>
		/// 流程状态改变的操作类
		/// </summary>
		/// <returns></returns>
		public WfActionCollection GetProcessStatusChangeActions()
		{
			return WfActivityConfigurationElement.GetActions(this.ProcessStatusChangeActionsString);
		}

		[ConfigurationProperty("cancelProcessActions", IsRequired = false, DefaultValue = "CancelProcessUserTaskAction,CancelExecuteInvokeServiceAction")]
		private string CancelProcessActionsString
		{
			get
			{
				return (string)this["cancelProcessActions"];
			}
		}

		[ConfigurationProperty("restoreProcessActions", IsRequired = false, DefaultValue = "RestoreProcessUserTaskAction")]
		private string RestoreProcessActionsString
		{
			get
			{
				return (string)this["restoreProcessActions"];
			}
		}

		[ConfigurationProperty("withdrawActions", IsRequired = false, DefaultValue = "WithdrawUserTaskAction")]
		private string WithdrawActionsString
		{
			get
			{
				return (string)this["withdrawActions"];
			}
		}

		[ConfigurationProperty("processStatusChangeActions", IsRequired = false, DefaultValue = "ProcessStatusChangeAction")]
		private string ProcessStatusChangeActionsString
		{
			get
			{
				return (string)this["processStatusChangeActions"];
			}
		}

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			return true;
		}

		protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
		{
			return true;
		}
	}

	public class WfActivityConfigurationElement : TypeConfigurationElement
	{
		public WfActivityConfigurationElement()
		{
		}

		public WfActionCollection GetEnterActions()
		{
			return GetActions(EnterActionNames);
		}

		public WfActionCollection GetLeaveActions()
		{
			return GetActions(LeaveActionNames);
		}

		internal static WfActionCollection GetActions(string actionNames)
		{
			WfActionCollection actions = new WfActionCollection();

			if (actionNames.IsNotEmpty())
			{
				string[] actionStrArray = actionNames.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

				WfActionSettings actionSettings = WfActionSettings.GetConfig();

				foreach (string actionName in actionStrArray)
				{
					try
					{
						actions.Add(actionSettings.GetAction(actionName));
					}
					catch (Exception ex)
					{
						WriteToLog(ex);
					}
				}
			}

			return actions;
		}

		//ydz 2012/06/07 将获取
		private static void WriteToLog(Exception ex)
		{
			Logger logger = LoggerFactory.Create("WfRuntime");

			if (logger != null)
			{
				StringBuilder strB = new StringBuilder(1024);

				strB.AppendLine(ex.Message);

				strB.AppendLine(EnvironmentHelper.GetEnvironmentInfo());
				strB.AppendLine(ex.StackTrace);

				logger.Write(strB.ToString(), LogPriority.Normal, 8004, TraceEventType.Error, "WfRuntime 获取Actions出错");
			}
		}

		[ConfigurationProperty("enterActions", IsRequired = false, DefaultValue = "")]
		private string EnterActionNames
		{
			get
			{
				return (string)this["enterActions"];
			}
		}

		[ConfigurationProperty("leaveActions", IsRequired = false, DefaultValue = "")]
		private string LeaveActionNames
		{
			get
			{
				return (string)this["leaveActions"];
			}
		}
	}

	public class WfActivityConfigurationCollection : NamedConfigurationElementCollection<WfActivityConfigurationElement>
	{ }
}
