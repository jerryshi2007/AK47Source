using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 流程仿真的相关配置
	/// </summary>
	public sealed class WfSimulationSettings : ConfigurationSection
	{
		public static WfSimulationSettings GetConfig()
		{
			WfSimulationSettings result = (WfSimulationSettings)ConfigurationBroker.GetSection("wfSimulationSettings", true);

			if (result == null)
				result = new WfSimulationSettings();

			return result;
		}

		[ConfigurationProperty("enabled", DefaultValue = false, IsRequired = false)]
		public bool Enabled
		{
			get
			{
				return (bool)this["enabled"];
			}
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
		/// 得到Writer的集合
		/// </summary>
		public IEnumerable<IWfSimulationWriter> Writers
		{
			get
			{
				if (WriterElements.Count > 0)
				{
					foreach (TypeConfigurationElement elem in WriterElements)
						yield return (IWfSimulationWriter)elem.CreateInstance();
				}
			}
		}

		[ConfigurationProperty("writers")]
		private TypeConfigurationCollection WriterElements
		{
			get
			{
				return (TypeConfigurationCollection)this["writers"];
			}
		}

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			return true;
		}

		protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
		{
			return true;
		}
	}
}
