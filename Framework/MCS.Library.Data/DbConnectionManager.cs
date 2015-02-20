#region using
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Configuration;

using MCS.Library.Core;
using MCS.Library.Data.Properties;
using MCS.Library.Configuration;
using MCS.Library.Data.Configuration;
#endregion
namespace MCS.Library.Data
{
    /// <summary>
    /// ���������ļ�����ȡ��Ӧ��ConnectionString��ProviderName��������ȡ���ݿ����Ӻ�Provider��Ϣ�Ĺ�����
    /// </summary>
    public static class DbConnectionManager
    {
		/// <summary>
		/// ��ȡָ�����ӵ�ConnectionString
		/// </summary>
		/// <param name="name">���ݿ��߼�����</param>
		/// <returns>ConnectionString</returns>
		public static string GetConnectionString(string name)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

			return GetConnection(name).ConnectionString;
		}

		/// <summary>
		/// ���ݿ������Ƿ���
		/// </summary>
		/// <param name="name"></param>
		public static bool ConnectionNameIsConfiged(string name)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

			ConnectionStringElement settings = GetConfiguration(name);

			return settings != null;
		}

        private static ConnectionStringElement GetConfiguration(string name)
        {
            ConnectionManagerConfigurationSection section =
                ConnectionManagerConfigurationSection.GetConfig();

            ConnectionStringConfigurationElement configElement = section.ConnectionStrings[name];
            ConnectionStringElement element = null;

            // ���������ļ������Ӵ��������ȼ�����Meta���е�
            if (configElement != null)
            {
                element = new ConnectionStringElement();
                element.ConnectionString = configElement.ConnectionString;
                element.ProviderName = configElement.ProviderName;
                element.EventArgsType = configElement.EventArgs.Type;
                element.Name = configElement.Name;
				element.CommandTimeout = configElement.CommandTimeout;
            }
			//else
			//{
			//    // ��ѡͨ���Ǵ�Meta���ȡ
			//    MetaConnectionStringConfigurationElement metaElement = section.MetaConnectionString;
			//    if (metaElement != null)
			//        element = metaElement.GetConnectionStringElement(name);
			//}

            return element;
        }

        /// <summary>
        /// �������ݿ��߼����ƻ������ʵ��
        /// </summary>
        /// <param name="name">���ݿ��߼�����</param>
        /// <returns>���ݿ�����ʵ��</returns>
        internal static DbConnection GetConnection(string name)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

            ConnectionStringElement settings = GetConfiguration(name);

			if (settings == null)
				throw new ConfigurationErrorsException(string.Format(Resource.CanNotFindConnectionName, name));

            DbConnection dbConnection = DbProviderFactories.GetFactory(settings.ProviderName).CreateConnection();
            dbConnection.ConnectionString = settings.ConnectionString;

            return dbConnection;
        }

        /// <summary>
        /// ��ȡָ�����ӵ�ProviderName
        /// </summary>
        /// <param name="name">���ݿ��߼�����</param>
        /// <returns>ProviderName</returns>
        //internal static string GetDbProviderName(string name) Modify By Yuanyong 20080320
        private static string GetDbProviderName(string name)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

            return GetConfiguration(name).ProviderName;
        }

        /// <summary>
        /// ��ȡָ�����ӵ�Database����
        /// </summary>
        /// <param name="name">���ݿ��߼�����</param>
        /// <returns>���ӵ�Database����</returns>
        internal static Database GetDataProvider(string name)
        {
            string providerName = GetDbProviderName(name);

            ConnectionManagerConfigurationSection section = ConnectionManagerConfigurationSection.GetConfig();

            DataProviderConfigurationElement configProvider = section.DataProviders[providerName];

            //������(2008-08-26)�����dataProvider�����Ƿ���ȷ
            ExceptionHelper.FalseThrow<ConfigurationException>(configProvider != null, Resource.CanNotFindProviderName, name, providerName);

            return (Database)configProvider.CreateInstance(name);
        }

		/// <summary>
		/// ��ȡָ�����ӵ�Commandִ�г�ʱʱ��
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		internal static TimeSpan GetCommandTimeout(string name)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

			return GetConfiguration(name).CommandTimeout;
		}

        /// <summary>
        /// ��ȡָ�����ӵ�DbProviderFactory����(Added by Shen Zheng)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static DbProviderFactory GetDbProviderFactory(string name)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

            ConnectionStringElement elem = GetConfiguration(name);

			if (elem == null)
				throw new ConfigurationErrorsException(string.Format(Resource.CanNotFindConnectionName, name));

            return DbProviderFactories.GetFactory(elem.ProviderName);
        }

        // Del By Yuanyong 20080320
        ///// <summary>
        ///// ��ȡָ�����ӵ�DbProviderFactory
        ///// </summary>
        ///// <param name="name">���ݿ��߼�����</param>
        ///// <returns>DbProviderFactoryʵ��</returns>
        //internal static DbProviderFactory GetDbProviderFactory(string name)
        //{
        //    ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");
        //    return DbProviderFactories.GetFactory(GetDbProviderName(name));
        //}

        /// <summary>
        /// ����������Ϣ���ָ���߼����ݿ���Զ����¼���������
        /// </summary>
        /// <param name="name">���ݿ��߼�����</param>
        /// <returns>�¼���������</returns>
        internal static DbEventArgs GetEventArgsType(string name)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

            string typeName = GetConfiguration(name).EventArgsType;

            return string.IsNullOrEmpty(typeName) ? null : (DbEventArgs)TypeCreator.CreateInstance(typeName);
        }
    }
}
