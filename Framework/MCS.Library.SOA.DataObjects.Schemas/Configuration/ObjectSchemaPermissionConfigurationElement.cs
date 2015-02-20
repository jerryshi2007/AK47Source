using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Schemas.Configuration
{
	/// <summary>
	/// 权限条目的配置信息
	/// </summary>
	public class ObjectSchemaPermissionConfigurationElement : NamedConfigurationElement
	{
		[ConfigurationProperty("displayName", IsRequired = false, DefaultValue = "")]
		public string DisplayName
		{
			get
			{
				return (string)this["displayName"];
			}
		}
	}

	/// <summary>
	/// 权限条目集合的配置信息
	/// </summary>
	public class ObjectSchemaPermissionConfigurationElementCollection : NamedConfigurationElementCollection<ObjectSchemaPermissionConfigurationElement>
	{
	}
}
