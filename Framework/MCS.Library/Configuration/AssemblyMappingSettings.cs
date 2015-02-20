using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace MCS.Library.Configuration
{
	/// <summary>
	///  Assembly名字映射的配置节
	/// </summary>
	public sealed class AssemblyMappingSettings : ConfigurationSection
	{
		private AssemblyMappingSettings()
		{
		}

		/// <summary>
		/// Assembly名字映射的配置信息
		/// </summary>
		/// <returns></returns>
		public static AssemblyMappingSettings GetConfig()
		{
			AssemblyMappingSettings result = (AssemblyMappingSettings)ConfigurationBroker.GetSection("assemblyMappingSettings");

			if (result == null)
				result = new AssemblyMappingSettings();

			return result;
		}

		/// <summary>
		/// 映射
		/// </summary>
		[ConfigurationProperty("mapping")]
		public AssemblyMappingConfigurationElementCollection Mapping
		{
			get
			{
				return (AssemblyMappingConfigurationElementCollection)this["mapping"];
			}
		}
	}

	/// <summary>
	/// Assembly名字映射条目
	/// </summary>
	public class AssemblyMappingConfigurationElement : NamedConfigurationElement
	{
		/// <summary>
		/// 类型描述信息
		/// </summary>
		/// <remarks>一般采用QualifiedName （QuanlifiedTypeName, AssemblyName）方式</remarks>
		[ConfigurationProperty("mapTo", IsRequired = true)]
		public virtual string MapTo
		{
			get
			{
				return (string)this["mapTo"];
			}
		}
	}

	/// <summary>
	/// Assembly名字映射条目集合
	/// </summary>
	public class AssemblyMappingConfigurationElementCollection : NamedConfigurationElementCollection<AssemblyMappingConfigurationElement>
	{
	}
}
