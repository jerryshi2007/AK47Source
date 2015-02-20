#region Using
using System;
using System.Configuration;
#endregion
namespace MCS.Library.Data.Configuration
{
	/// <summary>
	/// 自定义数据库执行过程事件参数的配置项
	/// </summary>
	class DbEventArgsConfigurationElement : ConfigurationElement
	{
		[ConfigurationProperty("type", IsRequired = true)]
		public virtual string Type
		{
			get
			{
				return (string)this["type"];
			}
		}
	}
}
