using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Configuration
{
	/// <summary>
	/// 管理范围对象的配置，注意其Name属性应该是
	/// </summary>
	public class AUAdminScopeConfigurationItem : NamedConfigurationElement
	{
		/// <summary>
		/// 服务类型的完全限定名称。注意此类型应该以XX开头
		/// </summary>
		[ConfigurationProperty("serviceTypeName", DefaultValue = "_, _", IsRequired = true, Options = ConfigurationPropertyOptions.IsAssemblyStringTransformationRequired)]
		[RegexStringValidator(@"((\w+)\.)*\w+, ((\w+)\.)*\w+")]
		public string ServiceTypeName
		{
			get
			{
				return (string)this["serviceTypeName"];
			}
		}
	}
}
