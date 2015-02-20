using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;
using MCS.Library.SOA.DataObjects.Security.Configuration;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.Logging;
using MCS.Library.Core;
using System.Diagnostics;
using MCS.Library.SOA.DataObjects.Schemas.Actions;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Configuration
{
    [Obsolete]
    public class AUUpdateActionConfigurationSection : ConfigurationSection
    {
        public static AUUpdateActionConfigurationSection GetConfig()
        {
            AUUpdateActionConfigurationSection settings =
                (AUUpdateActionConfigurationSection)ConfigurationBroker.GetSection("auUpdateActions") ?? new AUUpdateActionConfigurationSection();

            return settings;
        }

        private AUUpdateActionConfigurationSection()
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
