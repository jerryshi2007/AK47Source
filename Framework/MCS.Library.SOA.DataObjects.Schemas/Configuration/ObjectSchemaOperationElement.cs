using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects.Schemas.Configuration
{
	public class ObjectSchemaOperationElement : NamedConfigurationElement
	{
		[ConfigurationProperty("hasParentParemeter", DefaultValue = false, IsRequired = false)]
		public bool HasParentParemeter
		{
			get
			{
				return (bool)this["hasParentParemeter"];
			}
		}

		/// <summary>
		/// 方法的名称
		/// </summary>
		[ConfigurationProperty("method")]
		public string Method
		{
			get
			{
				return (string)this["method"];
			}
		}
	}

	public class ObjectSchemaOperationElementCollection : NamedConfigurationElementCollection<ObjectSchemaOperationElement>
	{
	}
}
