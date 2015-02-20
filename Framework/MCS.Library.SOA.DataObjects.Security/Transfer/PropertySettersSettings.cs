using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.Transfer
{
	public class PropertySettersSettings :ConfigurationSection
	{
		public static PropertySettersSettings GetConfig()
		{
			PropertySettersSettings result = (PropertySettersSettings)ConfigurationBroker.GetSection("propertySettersSettings");

			if (result == null)
				result = new PropertySettersSettings();

			return result;
		}

		[ConfigurationProperty("objectSetters", IsRequired = false)]
		public ObjectSetterConfigurationElementCollection ObjectSetters
		{
			get
			{
				return (ObjectSetterConfigurationElementCollection)this["objectSetters"];
			}
		}

		[ConfigurationProperty("propertySetters", IsRequired = false)]
		public PropertySetterConfigurationElementCollection PropertySetters
		{
			get
			{
				return (PropertySetterConfigurationElementCollection)this["propertySetters"];
			}
		}
	}
}