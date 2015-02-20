using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 对象的值向PropertyValue转换的转换器的配置信息
	/// </summary>
	public class ObjectValueToPropertyValueConfigurationElement : TypeConfigurationElement
	{
		/// <summary>
		/// 得到对象的属性类型
		/// </summary>
		/// <returns></returns>
		public Type GetObjectValueType()
		{
			this.ObjectValueTypeDescription.CheckStringIsNullOrEmpty("objectValueTypeDescription");

			return TypeCreator.GetTypeInfo(this.ObjectValueTypeDescription);
		}

		[ConfigurationProperty("objectValueTypeDescription")]
		private string ObjectValueTypeDescription
		{
			get
			{
				return (string)this["objectValueTypeDescription"];
			}
		}
	}

	public class ObjectValueToPropertyValueConfigurationElementCollection : NamedConfigurationElementCollection<ObjectValueToPropertyValueConfigurationElement>
	{
	}
}
