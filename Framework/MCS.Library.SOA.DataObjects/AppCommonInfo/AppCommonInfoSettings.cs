using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects
{
	public sealed class AppCommonInfoSettings : ConfigurationSection
	{
		private AppCommonInfoSettings()
		{ }

		public static AppCommonInfoSettings GetConfig()
		{
			AppCommonInfoSettings result =
				(AppCommonInfoSettings)ConfigurationBroker.GetSection("appCommonInfoSettings");

			if (result == null)
				result = new AppCommonInfoSettings();

			return result;
		}

		/// <summary>
		/// 消息操作类
		/// </summary>
		public IEnumerable<IAppCommonInfoUpdater> Operations
		{
			get
			{
				if (OperationElements.Count > 0)
				{
					foreach (TypeConfigurationElement elem in OperationElements)
						yield return (IAppCommonInfoUpdater)elem.CreateInstance();
				}
				else
					yield return DefaultAppCommonInfoUpdater.Instance;
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
