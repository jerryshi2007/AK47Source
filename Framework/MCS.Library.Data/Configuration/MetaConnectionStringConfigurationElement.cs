#region using
using System;
using System.Configuration;
using System.Data;
using System.Collections.Generic;
using System.Data.Common;
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.Data.Properties;
#endregion
namespace MCS.Library.Data.Configuration
{
	///// <summary>
	///// Ԫ���Ӵ����������Ԫ��
	///// </summary>
	//class MetaConnectionStringConfigurationElement : ConnectionStringConfigurationElementBase
	//{
	//    /// <summary>
	//    /// ��ʾ��ǰBuilder�����Meta���������ݿ��о���ĳ�����Ӵ�
	//    /// </summary>
	//    private const string MetaSectionPrefix = "META:";
	//    private const string ItemSectionPrefix = "ITEM:";
	//    private const string BothSectionPrefix = "BOTH:";

	//    /// <summary>
	//    /// �������д�Meta���õ����Ӵ�
	//    /// </summary>
	//    private static IDictionary<string, ConnectionStringElement> metaConnectionSettings = null;

	//    /// <summary>
	//    /// �Ƿ��Meta������Ӵ�������
	//    /// </summary>
	//    private static bool isMetaConnectionParsed = false;

	//    private void ParseSettingBuildersByPrefix(string prefix, ref ConnectionStringElement setting)
	//    {
	//        if ((Builders != null) && (Builders.Count > 0))
	//        {
	//            foreach (BuilderConfigurationElement elementBuilder in Builders)
	//            {
	//                if ((elementBuilder.Name.StartsWith(prefix)) || (elementBuilder.Name.StartsWith(BothSectionPrefix)))
	//                {
	//                    ConnectionStringBuilderBase builder = (ConnectionStringBuilderBase)TypeCreator.CreateInstance(elementBuilder.Type);
	//                    switch (elementBuilder.AttributeName)
	//                    {
	//                        case "connectionString":
	//                            setting.ConnectionString = builder.BuildUp(setting.ConnectionString); break;
	//                        case "name":
	//                            setting.Name = builder.BuildUp(setting.Name); break;
	//                        case "providerName":
	//                            setting.ProviderName = builder.BuildUp(setting.ProviderName); break;
	//                    }
	//                }
	//            }
	//        }
	//    }

	//    //private DataTable LoadMetaConnections()
	//    //{
	//    //    DataTable resultTable = null;

	//    //    //�����ӣ����ProvicerNameΪ�գ��򲻽��в�ѯ
	//    //    if (string.IsNullOrEmpty(settings.ProviderName) == false)
	//    //    {
	//    //        DbProviderFactory factory = DbProviderFactories.GetFactory(settings.ProviderName);

	//    //        //����Ӧ�������⣬Ԭ�¼�¼ 20080320
	//    //        using (DbConnection connection = factory.CreateConnection())
	//    //        {
	//    //            connection.ConnectionString = settings.ConnectionString;
	//    //            DbCommand command = connection.CreateCommand();
	//    //            command.CommandType = CommandType.Text;
	//    //            command.CommandText = "SELECT * FROM CONNECTIONS";

	//    //            using (DbDataAdapter adapter = factory.CreateDataAdapter())
	//    //            {
	//    //                adapter.SelectCommand = command;
	//    //                DataSet result = new DataSet();
	//    //                adapter.Fill(result);
	//    //                if ((result == null) || (result.Tables.Count == 0) || (result.Tables[0].Rows.Count == 0))
	//    //                    return null;

	//    //                resultTable = result.Tables[0];
	//    //            }
	//    //        }
	//    //    }

	//    //    return resultTable;
	//    //}

	//    //private void TranslateMetaConnections()
	//    //{
	//    //    DataTable result = LoadMetaConnections();
	//    //    if (result == null) 
	//    //        return;

	//    //    metaConnectionSettings = new Dictionary<string, ConnectionStringElement>();

	//    //    foreach (DataRow row in result.Rows)
	//    //    {
	//    //        ConnectionStringElement setting = new ConnectionStringElement();
	//    //        setting.Name = row["NAME"] as string;
	//    //        setting.ConnectionString = row["CONNECTION_STRING"] as string;
	//    //        setting.ProviderName = row["PROVIDER_NAME"] as string;
	//    //        setting.EventArgsType = row["EVENT_ARGUMENT_TYPE"] as string;

	//    //        ParseSettingBuildersByPrefix(ItemSectionPrefix, ref setting);
	//    //        metaConnectionSettings.Add(setting.Name, setting);
	//    //    }
	//    //}

	//    //protected override void ParseBuilders()
	//    //{
	//    //    if (isMetaConnectionParsed)
	//    //        return;

	//    //    isMetaConnectionParsed = true;

	//    //    // �����ļ��Ľ�����������⣬���յİ취����������tryһ�����½���
	//    //    if (settings == null)
	//    //    {
	//    //        settings = new ConnectionStringElement();
	//    //        settings.Name = (string)this["name"];
	//    //        settings.ConnectionString = (string)this["connectionString"];
	//    //        settings.ProviderName = (string)this["providerName"];
	//    //    }

	//    //    ParseSettingBuildersByPrefix(MetaSectionPrefix, ref settings);
	//    //    TranslateMetaConnections();
	//    //}

	//    ///// <summary>
	//    ///// ��Ϊ�ⲿ�ֶ��������ݿ��ڲ������Բ���Ҫ����
	//    ///// </summary>
	//    //protected override void ParseEventArgs()
	//    //{
	//    //}

	//    /// <summary>
	//    /// ����Ԫ������û�ȡ���Ӵ�
	//    /// </summary>
	//    /// <param name="name">���Ӵ�����</param>
	//    /// <returns>���Ӵ�����</returns>
	//    //public ConnectionStringElement GetConnectionStringElement(string name)
	//    //{
	//    //    ParseBuilders();
	//    //    if ((metaConnectionSettings == null) || (metaConnectionSettings.Count == 0))
	//    //        return null;
	//    //    ConnectionStringElement element;
	//    //    if (false == metaConnectionSettings.TryGetValue(name, out element))
	//    //        return null;
	//    //    else
	//    //        return element;
	//    //}
	//}
}
