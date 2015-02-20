#region using
using System;
using System.Configuration;
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.Data.Properties;
#endregion
namespace MCS.Library.Data.Configuration
{
	/// <summary>
	/// ���Ӵ����������Ԫ��
	/// </summary>
	/// <remarks>����Builders���ڸýڵ㹹����Ϻ���У���˶Ը����ԵĴ�����ú���ش��õİ취</remarks>
	sealed class ConnectionStringConfigurationElement : ConnectionStringConfigurationElementBase
	{
	}

	[ConfigurationCollection(typeof(ConnectionStringConfigurationElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	sealed class ConnectionStringConfigurationElementCollection : NamedConfigurationElementCollection<ConnectionStringConfigurationElement>
	{
	}
}