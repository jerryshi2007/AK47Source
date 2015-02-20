#region Using
using System;
using System.Configuration;
using MCS.Library.Configuration;
#endregion

namespace MCS.Library.Data.Configuration
{
	class ConnectionManagerConfigurationSection : ConfigurationSection
	{
        /// <summary>
        /// ��ȡ�����ļ��е����ݿ����ӣ�ConnectionManager������
        /// </summary>
        /// <returns></returns>
		public static ConnectionManagerConfigurationSection GetConfig()
		{
			ConnectionManagerConfigurationSection section =
				(ConnectionManagerConfigurationSection)ConfigurationBroker.GetSection("connectionManager");

			ConfigurationExceptionHelper.CheckSectionNotNull(section, "connectionManager");

			return section;
		}

		[ConfigurationProperty("connectionStrings", IsRequired = false)]
		public ConnectionStringConfigurationElementCollection ConnectionStrings
		{
			get
			{
				return (ConnectionStringConfigurationElementCollection)base["connectionStrings"];
			}
		}

		//[ConfigurationProperty("metaConnection", IsRequired = false)]
		//public MetaConnectionStringConfigurationElement MetaConnectionString
		//{
		//    get
		//    {
		//        return (MetaConnectionStringConfigurationElement)base["metaConnection"];
		//    }
		//}

		//add by yuanyong 20080320
		[ConfigurationProperty("dataProviders", IsRequired = true)]
		public DataProviderConfigurationElementCollection DataProviders
		{
			get
			{
				return (DataProviderConfigurationElementCollection)base["dataProviders"];
			}
		}
	}
}
