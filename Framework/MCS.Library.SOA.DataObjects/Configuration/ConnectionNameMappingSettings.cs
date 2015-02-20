using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// ��������ӳ���ϵ�����ý�
	/// </summary>
	public sealed class ConnectionNameMappingSettings : ConfigurationSection
	{
		/// <summary>
		/// �õ�����ConnectionNameMapping��������Ϣ
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
		/// ӳ���ϵ
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
		/// ����ӳ���ϵ��ѯ��������
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue">���û���ҵ����򷵻�ȱʡֵ</param>
		/// <returns>���Ӵ�������</returns>
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
	/// ӳ���ϵ��ļ���
	/// </summary>
	public sealed class ConnectionNameMappingElementCollection : NamedConfigurationElementCollection<ConnectionNameMappingElement>
	{
	}

	/// <summary>
	/// ӳ���ϵ��
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
