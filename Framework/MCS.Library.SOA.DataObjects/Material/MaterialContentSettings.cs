using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 附件内容的配置信息
	/// </summary>
	public sealed class MaterialContentSettings : DeluxeConfigurationSection
	{
		public static MaterialContentSettings GetConfig()
		{
			MaterialContentSettings settings = (MaterialContentSettings)ConfigurationBroker.GetSection("materialContentSettings");

			if (settings == null)
				settings = new MaterialContentSettings();

			return settings;
		}

		private MaterialContentSettings()
		{
		}

		[ConfigurationProperty("connectionName", IsRequired = true)]
		public string ConnectionName
		{
			get
			{
				return (string)this["connectionName"];
			}
		}

		/// <summary>
		/// 页面ViewState的持久化器
		/// </summary>
		public IMaterialContentPersistManager PersistManager
		{
			get
			{
				IMaterialContentPersistManager result = null;

				if (TypeFactories.ContainsKey("persistManager"))
					result = (IMaterialContentPersistManager)TypeFactories["persistManager"].CreateInstance();
				else
					result = new FileMaterialContentPersistManager();

				return result;
			}
		}

		[ConfigurationProperty("typeFactories", IsRequired = false)]
		private TypeConfigurationCollection TypeFactories
		{
			get
			{
				return (TypeConfigurationCollection)this["typeFactories"];
			}
		}
	}
}
