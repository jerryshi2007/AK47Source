using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects.Schemas.Configuration
{
	public class SchemaObjectUpdateActionConfigurationElement : TypeConfigurationElement
	{
		[ConfigurationProperty("operation", IsRequired = false)]
		public string Operation
		{
			get
			{
				return (string)this["operation"];
			}
		}
	}

	public class SchemaObjectUpdateActionConfigurationElementCollection : NamedConfigurationElementCollection<SchemaObjectUpdateActionConfigurationElement>
	{
	}
}
