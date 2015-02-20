using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.Logging;
using MCS.Library.SOA.DataObjects.Security.Actions;

namespace MCS.Library.SOA.DataObjects.Security.Configuration
{
	public class SCObjectOperationActionSettings : ConfigurationSection
	{
		public static SCObjectOperationActionSettings GetConfig()
		{
			SCObjectOperationActionSettings settings =
				(SCObjectOperationActionSettings)ConfigurationBroker.GetSection("scObjectOperationActionSettings");

			if (settings == null)
				settings = new SCObjectOperationActionSettings();

			return settings;
		}

		private SCObjectOperationActionSettings()
		{
		}

		public SCObjectOperationActionCollection GetActions()
		{
			SCObjectOperationActionCollection actions = new SCObjectOperationActionCollection();

			foreach (SCObjectOperationActionConfigurationElement actionElem in Actions)
			{
				try
				{
					actions.Add((ISCObjectOperationAction)actionElem.CreateInstance());	
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

				logger.Write(strB.ToString(), LogPriority.Normal, 8004, TraceEventType.Error, "WfRuntime 获取SCOperationAction出错");
			}
		}
	}
}
