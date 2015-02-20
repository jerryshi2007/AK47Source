using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Caching;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects.Security.Transfer
{
	/// <summary>
	/// 包含在SchemaMapping中的对象操作配置项
	/// </summary>
	public class SetterObjectMappingConfigurationElement : NamedConfigurationElement
	{
		[ConfigurationProperty("opName")]
		public string OperationName
		{
			get
			{
				return (string)this["opName"];
			}
		}

		[ConfigurationProperty("context", IsRequired = false, DefaultValue = "")]
		public string Context
		{
			get
			{
				return (string)this["context"];
			}
		}
	}

	/// <summary>
	/// 包含在SchemaMapping中的对象操作配置项集合
	/// </summary>
	public class SetterObjectMappingConfigurationElementCollection : NamedConfigurationElementCollection<SetterObjectMappingConfigurationElement>
	{
		public void CopyFrom(SetterObjectMappingConfigurationElementCollection src)
		{
			foreach (SetterObjectMappingConfigurationElement element in src)
			{
				if (this.ContainsKey(element.Name) == false)
					this.Add(element);
			}
		}

		public void Add(SetterObjectMappingConfigurationElement element)
		{
			base.BaseAdd(element);
		}
	}
}
