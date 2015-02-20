using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MCS.Library.Configuration;

namespace MCS.Library.Services.Configuration
{
	/// <summary>
	/// Service的配置元素
	/// </summary>
	public sealed class ServiceConfigurationElement : NamedConfigurationElement
	{
		/// <summary>
		/// 方法的定义集合
		/// </summary>
		[ConfigurationProperty("methods", IsRequired = false)]
		public ServiceMethodConfigurationElementCollection Methods
		{
			get
			{
				return (ServiceMethodConfigurationElementCollection)this["methods"];
			}
		}
	}

	/// <summary>
	/// Service配置元素的集合
	/// </summary>
	public sealed class ServiceConfigurationElementCollection : NamedConfigurationElementCollection<ServiceConfigurationElement>
	{
	}
}