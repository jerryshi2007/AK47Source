using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 连接名称映射关系的配置节
	/// </summary>
	public sealed class ConnectionNameMappingSettings : ConfigurationSection
	{
		/// <summary>
		/// 得到配置ConnectionNameMapping的配置信息
		/// </summary>
		/// <returns></returns>
		public static ConnectionNameMappingSettings GetConfig()
		{
			ConnectionNameMappingSettings settings = (ConnectionNameMappingSettings)ConfigurationBroker.GetSection("soaConnectionNameMappingSettings");

			if (settings == null)
				settings = new ConnectionNameMappingSettings();

			return settings;
		}

		private ConnectionNameMappingSettings()
		{
		}

		/// <summary>
		/// 映射关系
		/// </summary>
		[ConfigurationProperty("mappings", IsRequired = true)]
		public ConnectionNameMappingElementCollection Mappings
		{
			get
			{
				return (ConnectionNameMappingElementCollection)this["mappings"];
			}
		}

		/// <summary>
		/// 根据映射关系查询连接名称
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue">如果没有找到，则返回缺省值</param>
		/// <returns>连接串的名称</returns>
		public string GetConnectionName(string key, string defaultValue)
		{
			string result = string.Empty;

			ConnectionNameMappingElement elem = Mappings[key];

			if (elem != null)
				result = elem.ConnectionName;

			if (string.IsNullOrEmpty(result))
				result = defaultValue;

			return result;
		}
	}

	/// <summary>
	/// 映射关系项的集合
	/// </summary>
	public sealed class ConnectionNameMappingElementCollection : NamedConfigurationElementCollection<ConnectionNameMappingElement>
	{
	}

	/// <summary>
	/// 映射关系项
	/// </summary>
	public sealed class ConnectionNameMappingElement : NamedConfigurationElement
	{
		[ConfigurationProperty("connectionName", DefaultValue = "")]
		public string ConnectionName
		{
			get
			{
				return (string)this["connectionName"];
			}
		}
	}
}
