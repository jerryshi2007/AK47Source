using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects.Security.Transfer
{
	/// <summary>
	/// Schema所包含的属性映射配置
	/// </summary>
	public class SchemaMappingConfigurationElement : NamedConfigurationElement
	{
		[ConfigurationProperty("inherited", IsRequired = false, DefaultValue = "")]
		public string Inherited
		{
			get
			{
				return (string)this["inherited"];
			}
		}

		[ConfigurationProperty("modifyOperations", IsRequired = false)]
		public SetterObjectMappingConfigurationElementCollection ModifyOperations
		{
			get
			{
				return (SetterObjectMappingConfigurationElementCollection)this["modifyOperations"];
			}
		}

		[ConfigurationProperty("comparedProperties", IsRequired = false)]
		public ComparerPropertyMappingConfigurationElementCollection ComparedProperties
		{
			get
			{
				return (ComparerPropertyMappingConfigurationElementCollection)this["comparedProperties"];
			}
		}

		[ConfigurationProperty("modifiedProperties", IsRequired = false)]
		public SetterPropertyMappingConfigurationElementCollection ModifiedProperties
		{
			get
			{
				return (SetterPropertyMappingConfigurationElementCollection)this["modifiedProperties"];
			}
		}

		[ConfigurationProperty("comparerName", IsRequired = false)]
		public string ComparerName
		{
			get
			{
				return (string)this["comparerName"];
			}
		}

		[ConfigurationProperty("prefix", IsRequired = false)]
		public string Prefix
		{
			get
			{
				return (string)this["prefix"];
			}
		}

		/// <summary>
		/// 属性映射信息
		/// </summary>
		/// <returns></returns>
		public SchemaMappingInfo GetSchemaMappingInfo()
		{
			SchemaMappingInfo result = new SchemaMappingInfo() { Name = this.Name, ComparerName = this.ComparerName, Prefix = this.Prefix };

			result.ModifyOperations.CopyFrom(this.ModifyOperations);
			result.ComparedProperties.CopyFrom(this.ComparedProperties);
			result.ModifiedProperties.CopyFrom(this.ModifiedProperties);

			return result;
		}
	}

	/// <summary>
	/// Schema所包含的属性映射配置集合
	/// </summary>
	public class SchemaMappingConfigurationElementCollection : NamedConfigurationElementCollection<SchemaMappingConfigurationElement>
	{
		/// <summary>
		/// 根据Schema得到属性信息
		/// </summary>
		/// <param name="schemaType"></param>
		/// <returns></returns>
		public SchemaMappingInfo GetSchemaMappingInfo(string schemaType)
		{
			this.ContainsKey(schemaType).FalseThrow("不能找到Schema为{0}的属性映射信息", schemaType);

			SchemaMappingConfigurationElement mappingElement = this[schemaType];

			SchemaMappingInfo result = mappingElement.GetSchemaMappingInfo();

			MergeInheritedElement(result, mappingElement);

			return result;
		}

		private void MergeInheritedElement(SchemaMappingInfo mappingInfo, SchemaMappingConfigurationElement mappingElement)
		{
			if (mappingElement.Inherited.IsNotEmpty())
			{
				this.ContainsKey(mappingElement.Inherited).FalseThrow("不能找到Schema为{0}的属性映射信息", mappingElement.Inherited);

				SchemaMappingConfigurationElement inheritedMappingElement = this[mappingElement.Inherited];

				mappingInfo.ComparedProperties.CopyFrom(inheritedMappingElement.ComparedProperties);
				mappingInfo.ModifiedProperties.CopyFrom(inheritedMappingElement.ModifiedProperties);

				MergeInheritedElement(mappingInfo, inheritedMappingElement);
			}
		}
	}
}
