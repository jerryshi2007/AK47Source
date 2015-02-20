using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Schemas.Client.Configuration
{
	/// <summary>
	/// SchemaObject的服务设置类的基类
	/// </summary>
	public class SchemaObjectServiceSettingsBase : DeluxeConfigurationSection
	{
		/// <summary>
		/// 更新操作时是否带锁
		/// </summary>
		[ConfigurationProperty("withLock", DefaultValue = true, IsRequired = false)]
		public bool WithLock
		{
			get
			{
				return (bool)this["withLock"];
			}
		}
		/// <summary>
		/// 是否使用对象的本地缓存
		/// </summary>
		[ConfigurationProperty("useLocalCache", DefaultValue = true, IsRequired = false)]
		public bool UseLocalCache
		{
			get
			{
				return (bool)this["useLocalCache"];
			}
		}

		/// <summary>
		/// 是否使用服务器端缓存。这需要服务方支持。此属性会通过ServiceBrokerSoapHeader放置在SoapHeader中
		/// </summary>
		[ConfigurationProperty("useServerCache", DefaultValue = true, IsRequired = false)]
		public bool UseServerCache
		{
			get
			{
				return (bool)this["useServerCache"];
			}
		}

		/// <summary>
		/// 连接WebService的超时时间
		/// </summary>
		[ConfigurationProperty("timeout", DefaultValue = "00:01:30", IsRequired = false)]
		public TimeSpan Timeout
		{
			get
			{
				return (TimeSpan)this["timeout"];
			}
		}

		[ConfigurationProperty("paths", IsRequired = true)]
		protected UriConfigurationCollection Paths
		{
			get
			{
				return (UriConfigurationCollection)this["paths"];
			}
		}

		/// <summary>
		/// 客户端传递到Web服务的连接串映射信息集合
		/// </summary>
		[ConfigurationProperty("connectionMappings", IsRequired = false)]
		public OguConnectionMappingElementCollection ConnectionMappings
		{
			get
			{
				return (OguConnectionMappingElementCollection)this["connectionMappings"];
			}
		}
	}
}
