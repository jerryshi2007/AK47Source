using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using System.Configuration;

namespace MCS.Web.Responsive.WebControls.Configuration
{
	public sealed class DropdownPropertyDataSourceConfigurationElement : TypeConfigurationElement
	{
		[ConfigurationProperty("bindingValue", IsRequired = true)]
		public string BindingValue
		{
			get
			{
				return (string)this["bindingValue"];
			}
		}

		[ConfigurationProperty("bindingText", IsRequired = true)]
		public string BindingText
		{
			get
			{
				return (string)this["bindingText"];
			}
		}

		[ConfigurationProperty("method", IsRequired = true)]
		public string Method
		{
			get
			{
				return (string)this["method"];
			}
		}
	}

	public sealed class DropdownPropertyDataSourceConfigurationCollection : NamedConfigurationElementCollection<DropdownPropertyDataSourceConfigurationElement>
	{
	}
}
