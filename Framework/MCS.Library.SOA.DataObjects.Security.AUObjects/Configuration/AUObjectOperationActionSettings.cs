using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Configuration;
using MCS.Library.Logging;
using System.Diagnostics;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Actions;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Configuration
{
	public class AUObjectOperationActionSettings : ConfigurationSection
	{
		public static AUObjectOperationActionSettings GetConfig()
		{
			AUObjectOperationActionSettings settings =
				(AUObjectOperationActionSettings)ConfigurationBroker.GetSection("auObjectOperationActionSettings");

			if (settings == null)
				settings = new AUObjectOperationActionSettings();

			return settings;
		}

		private AUObjectOperationActionSettings()
		{
		}

		public AUObjectOperationActionCollection GetActions()
		{
			AUObjectOperationActionCollection actions = new AUObjectOperationActionCollection();

			foreach (SCObjectOperationActionConfigurationElement actionElem in Actions)
			{
				try
				{
					actions.Add((IAUObjectOperationAction)actionElem.CreateInstance());
				}
				catch (Exception ex)
				{
					WriteToLog(ex);
				}
			}

			return actions;
		}

		[ConfigurationProperty("actions")]
		private SCOperationActionConfigurationElementCollection Actions
		{
			get
			{
				return (SCOperationActionConfigurationElementCollection)this["actions"];
			}
		}

		private static void WriteToLog(Exception ex)
		{
			Logger logger = LoggerFactory.Create("WfRuntime");

			if (logger != null)
			{
				StringBuilder strB = new StringBuilder(1024);

				strB.AppendLine(ex.Message);

				strB.AppendLine(EnvironmentHelper.GetEnvironmentInfo());
				strB.AppendLine(ex.StackTrace);

				logger.Write(strB.ToString(), LogPriority.Normal, 8004, TraceEventType.Error, "(管理单元配置)WfRuntime 获取AUOperationAction出错");
			}
		}
	}
}
