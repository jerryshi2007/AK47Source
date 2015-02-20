using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Web.WebControls
{
	public sealed class PropertyEditorSettings : ConfigurationSection
	{
		public static PropertyEditorSettings GetConfig()
		{
			PropertyEditorSettings result = (PropertyEditorSettings)ConfigurationBroker.GetSection("propertyEditorSettings");

			if (result == null)
				result = new PropertyEditorSettings();

			return result;
		}

		private PropertyEditorSettings()
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
