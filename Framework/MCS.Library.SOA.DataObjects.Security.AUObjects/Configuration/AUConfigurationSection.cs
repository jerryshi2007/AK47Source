using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using System.Configuration;
using MCS.Library.SOA.DataObjects.Security.Configuration;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Configuration
{
	/// <summary>
	/// 表示管理单元的配置节
	/// </summary>
	public sealed class AUConfigurationSection : ConfigurationSection
	{
		private AUConfigurationSection()
		{
		}

		public static AUConfigurationSection GetConfig()
		{
			AUConfigurationSection settings = (AUConfigurationSection)ConfigurationBroker.GetSection("auConfigurationSection");

			if (settings == null)
				settings = new AUConfigurationSection();

			return settings;
		}

		/// <summary>
		/// 管理员角色的全CodeName。表示为AppCodeName:RoleCodeName
		/// </summary>
		[ConfigurationProperty("masterRoleFullCodeName", IsRequired = false)]
		public string MasterRoleFullCodeName
		{
			get
			{
				return (string)this["masterRoleFullCodeName"];
			}
		}

		[ConfigurationProperty("schemas", IsDefaultCollection = false)]
		[ConfigurationCollection(typeof(AUAdminScopeConfigurationItemCollection))]
		public AUAdminScopeConfigurationItemCollection Schemas
		{
			get
			{
				AUAdminScopeConfigurationItemCollection schemas = (AUAdminScopeConfigurationItemCollection)base["schemas"];
				return schemas;
			}
		}
	}
}
