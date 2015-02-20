using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.Transfer
{
	/// <summary>
	/// 属性比较器的设置
	/// </summary>
	public class PropertyComparersSettings : ConfigurationSection
	{
		public static PropertyComparersSettings GetConfig()
		{
			PropertyComparersSettings result = (PropertyComparersSettings)ConfigurationBroker.GetSection("propertyComparersSettings");

			if (result == null)
				result = new PropertyComparersSettings();

			return result;
		}

		[ConfigurationProperty("objectComparers", IsRequired = false)]
		public TypeConfigurationCollection ObjectComparers
		{
			get
			{
				return (TypeConfigurationCollection)this["objectComparers"];
			}
		}

		[ConfigurationProperty("propertyComparers", IsRequired = false)]
		public PropertyComparerConfigurationCollection PropertyComparers
		{
			get
			{
				return (PropertyComparerConfigurationCollection)this["propertyComparers"];
			}
		}
	}
}
