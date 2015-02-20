using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;

namespace MCS.Library.SOA.DataObjects.Schemas.Configuration
{
	public sealed class ObjectSchemaSettings : ConfigurationSection
	{
		public static ObjectSchemaSettings GetConfig()
		{
			ObjectSchemaSettings settings = (ObjectSchemaSettings)ConfigurationBroker.GetSection("objectSchemaSettings");

			if (settings == null)
				settings = new ObjectSchemaSettings();

			return settings;
		}

		private ObjectSchemaSettings()
		{
		}

		public IRole GetAdminRole()
		{
			OguRole result = null;

			if (this.AdminRoleFullCodeName.IsNotEmpty())
				result = new OguRole(this.AdminRoleFullCodeName);

			return result;
		}

		/// <summary>
		/// 管理员角色的全CodeName。表示为AppCodeName:RoleCodeName
		/// </summary>
		[ConfigurationProperty("adminRoleFullCodeName", IsRequired = false)]
		public string AdminRoleFullCodeName
		{
			get
			{
				return (string)this["adminRoleFullCodeName"];
			}
		}

		[ConfigurationProperty("schemas", IsRequired = false)]
		public ObjectSchemaConfigurationElementCollection Schemas
		{
			get
			{
				return (ObjectSchemaConfigurationElementCollection)this["schemas"];
			}
		}
	}
}
