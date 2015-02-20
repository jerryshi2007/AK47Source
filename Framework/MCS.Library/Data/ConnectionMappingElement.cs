using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;

namespace MCS.Library.Data.Configuration
{
	/// <summary>
	/// 连接映射的配置节
	/// </summary>
	public class ConnectionMappingElement : NamedConfigurationElement
	{
		/// <summary>
		/// 目标连接串
		/// </summary>
		[ConfigurationProperty("destination", IsRequired = true)]
		public string Destination
		{
			get
			{
				return (string)this["destination"];
			}
		}
	}

	/// <summary>
	/// 接串映射信息集合
	/// </summary>
	public sealed class ConnectionMappingElementCollection : NamedConfigurationElementCollection<ConnectionMappingElement>
	{
	}
}
