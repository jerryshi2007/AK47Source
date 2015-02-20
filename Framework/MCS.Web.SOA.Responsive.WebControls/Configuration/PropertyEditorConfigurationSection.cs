using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Web.Responsive.WebControls.Configuration
{
	public sealed class PropertyEditorConfigurationSection : ConfigurationSection
	{
		public static PropertyEditorConfigurationSection GetConfig()
		{
			PropertyEditorConfigurationSection result = (PropertyEditorConfigurationSection)ConfigurationBroker.GetSection("propertyEditorConfig");

			if (result == null)
				result = new PropertyEditorConfigurationSection();

			return result;
		}

		private PropertyEditorConfigurationSection()
		{
		}

		[ConfigurationProperty("editors", IsRequired = false)]
		public TypeConfigurationCollection Editors
		{
			get
			{
				return (TypeConfigurationCollection)this["editors"];
			}
		}
	}
}
