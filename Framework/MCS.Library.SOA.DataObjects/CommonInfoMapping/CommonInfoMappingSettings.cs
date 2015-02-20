using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects
{
	public class CommonInfoMappingSettings : ConfigurationSection
	{
		private CommonInfoMappingSettings()
		{}

		public static CommonInfoMappingSettings GetConfig()
		{
			CommonInfoMappingSettings result =
				(CommonInfoMappingSettings)ConfigurationBroker.GetSection("commonInfoMappingSettings");

			if (result == null)
				result = new CommonInfoMappingSettings();

			return result;
		}

		/// <summary>
		/// 消息操作类
		/// </summary>
		public IEnumerable<ICommonInfoMappingUpdater> Operations
		{
			get
			{
				if (OperationElements.Count > 0)
				{
					foreach (TypeConfigurationElement elem in OperationElements)
						yield return (ICommonInfoMappingUpdater)elem.CreateInstance();
				}
				else
					yield return DefaultCommonInfoMappingUpdater.Instance;
			}
		}

		[ConfigurationProperty("operations")]
		private TypeConfigurationCollection OperationElements
		{
			get
			{
				return (TypeConfigurationCollection)this["operations"];
			}
		}
	}
}
