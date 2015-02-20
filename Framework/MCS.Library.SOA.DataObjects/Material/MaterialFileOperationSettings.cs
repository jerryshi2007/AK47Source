using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using System.Configuration;

namespace MCS.Library.SOA.DataObjects
{
	public sealed class MaterialFileOperationSettings : DeluxeConfigurationSection
	{
		public static MaterialFileOperationSettings GetConfig()
		{
			MaterialFileOperationSettings settings = (MaterialFileOperationSettings)ConfigurationBroker.GetSection("materialFileOperationSettings");

			if (settings == null)
				settings = new MaterialFileOperationSettings();

			return settings;
		}

		/// <summary>
		/// 返回附件文件操作类的集合
		/// </summary>
		public IEnumerable<IMaterialFileOperation> Operations
		{
			get
			{
				if (this.TypeFactories.Count > 0)
				{
					foreach (TypeConfigurationElement element in TypeFactories)
						yield return (IMaterialFileOperation)element.CreateInstance();
				}
				else
					yield return DefaultMaterialFileOperation.Instance;
			}
		}

		[ConfigurationProperty("operations", IsRequired = false)]
		private TypeConfigurationCollection TypeFactories
		{
			get
			{
				return (TypeConfigurationCollection)this["operations"];
			}
		}
	}
}
