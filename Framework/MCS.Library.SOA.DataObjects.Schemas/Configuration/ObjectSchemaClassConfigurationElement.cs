using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects.Schemas.Configuration
{
	public class ObjectSchemaClassConfigurationElement : NamedConfigurationElement
	{
		[ConfigurationProperty("groupName", IsRequired = true)]
		public string GroupName
		{
			get
			{
				return (string)this["groupName"];
			}
		}
	}

	public class ObjectSchemaClassConfigurationElementCollection : NamedConfigurationElementCollection<ObjectSchemaClassConfigurationElement>
	{
	}
}
