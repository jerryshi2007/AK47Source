using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.Configuration
{
	public class SCObjectOperationActionConfigurationElement : TypeConfigurationElement
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

	public class SCOperationActionConfigurationElementCollection : NamedConfigurationElementCollection<SCObjectOperationActionConfigurationElement>
	{
	}
}
