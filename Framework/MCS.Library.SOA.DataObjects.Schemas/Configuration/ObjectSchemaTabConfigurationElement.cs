using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects.Schemas.Configuration
{
	/// <summary>
	/// 属性的页签配置
	/// </summary>
	public class ObjectSchemaTabConfigurationElement : NamedConfigurationElement
	{
	}

	public class ObjectSchemaTabConfigurationElementCollection : NamedConfigurationElementCollection<ObjectSchemaTabConfigurationElement>
	{
	}
}
