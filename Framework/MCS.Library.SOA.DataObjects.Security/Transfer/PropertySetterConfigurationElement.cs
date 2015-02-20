using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects.Security.Transfer
{
	/// <summary>
	/// 属性设置器的配置项
	/// </summary>
	public class PropertySetterConfigurationElement : TypeConfigurationElement
	{
	}

	/// <summary>
	/// 属性设置器的配置项集合
	/// </summary>
	public class PropertySetterConfigurationElementCollection : NamedConfigurationElementCollection<PropertySetterConfigurationElement>
	{
	}
}
