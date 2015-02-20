using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects.Security.Transfer
{
	/// <summary>
	/// 属性映射的配置信息项
	/// </summary>
	public abstract class PropertyMappingConfigurationElementBase : NamedConfigurationElement
	{
		[ConfigurationProperty("targetPropertyName")]
		public string TargetPropertyName
		{
			get
			{
				return (string)this["targetPropertyName"];
			}
		}

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
	/// 需要比较的属性映射配置项
	/// </summary>
	public class ComparerPropertyMappingConfigurationElement : PropertyMappingConfigurationElementBase
	{
		public PropertyComparerBase Comparer
		{
			get
			{
				string cacheKey = this.GetType().FullName + "." + OperationName;

				return (PropertyComparerBase)ObjectContextCache.Instance.GetOrAddNewValue(cacheKey, (cache, key) =>
				{
					PropertyComparerBase comparer = (PropertyComparerBase)PropertyComparersSettings.GetConfig().PropertyComparers[this.OperationName].CreateInstance();

					cache.Add(cacheKey, comparer);

					return comparer;
				});
			}
		}
	}

	/// <summary>
	/// 需要比较的属性映射的配置信息项集合
	/// </summary>
	public class ComparerPropertyMappingConfigurationElementCollection : NamedConfigurationElementCollection<ComparerPropertyMappingConfigurationElement>
	{
		public void CopyFrom(ComparerPropertyMappingConfigurationElementCollection src)
		{
			foreach (ComparerPropertyMappingConfigurationElement element in src)
			{
				if (this.ContainsKey(element.Name) == false)
					this.Add(element);
			}
		}

		public void Add(ComparerPropertyMappingConfigurationElement element)
		{
			base.BaseAdd(element);
		}
	}

	/// <summary>
	/// 需要设置的属性映射配置项
	/// </summary>
	public class SetterPropertyMappingConfigurationElement : PropertyMappingConfigurationElementBase
	{
		[ConfigurationProperty("delay", DefaultValue = false)]
		public bool Delay
		{
			get
			{
				return (bool)this["delay"];
			}
		}

		public PropertySetterBase Setter
		{
			get
			{
				string cacheKey = this.GetType().FullName + "." + OperationName;

				return (PropertySetterBase)ObjectContextCache.Instance.GetOrAddNewValue(cacheKey, (cache, key) =>
				{
					PropertySetterBase comparer = (PropertySetterBase)PropertySettersSettings.GetConfig().PropertySetters[this.OperationName].CreateInstance();

					cache.Add(cacheKey, comparer);

					return comparer;
				});
			}
		}
	}

	/// <summary>
	/// 需要设置的属性映射的配置信息项集合
	/// </summary>
	public class SetterPropertyMappingConfigurationElementCollection : NamedConfigurationElementCollection<SetterPropertyMappingConfigurationElement>
	{
		public void CopyFrom(SetterPropertyMappingConfigurationElementCollection src)
		{
			foreach (SetterPropertyMappingConfigurationElement element in src)
			{
				if (this.ContainsKey(element.Name) == false)
					this.Add(element);
			}
		}

		public void Add(SetterPropertyMappingConfigurationElement element)
		{
			base.BaseAdd(element);
		}
	}
}
