using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.Logging;
using MCS.Library.SOA.DataObjects.Schemas.Actions;

namespace MCS.Library.SOA.DataObjects.Schemas.Configuration
{
	public class SchemaObjectUpdateActionSettings : ConfigurationSection
	{
		public static SchemaObjectUpdateActionSettings GetConfig()
		{
			SchemaObjectUpdateActionSettings settings =
				(SchemaObjectUpdateActionSettings)ConfigurationBroker.GetSection("schemaObjectUpdateActionSettings");

			if (settings == null)
				settings = new SchemaObjectUpdateActionSettings();

			return settings;
		}

		private SchemaObjectUpdateActionSettings()
		{
		}

		public SchemaObjectUpdateActionCollection GetActions(string operation)
		{
			SchemaObjectUpdateActionCollection actions = new SchemaObjectUpdateActionCollection();

			foreach (SchemaObjectUpdateActionConfigurationElement actionElem in this.Actions)
			{
				try
				{
					if (actionElem.Operation == operation)
						actions.Add((ISchemaObjectUpdateAction)actionElem.CreateInstance());
				}
				catch (Exception ex)
				{
					WriteToLog(ex);
				}
			}

			return actions;
		}

		[ConfigurationProperty("actions")]
		private SchemaObjectUpdateActionConfigurationElementCollection Actions
		{
			get
			{
				return (SchemaObjectUpdateActionConfigurationElementCollection)this["actions"];
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

				logger.Write(strB.ToString(), LogPriority.Normal, 8004, TraceEventType.Error, "WfRuntime 获取SchemaObjectUpdateAction出错");
			}
		}
	}
}
