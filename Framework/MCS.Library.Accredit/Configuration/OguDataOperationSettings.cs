using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using MCS.Library.Configuration;

namespace MCS.Library.Accredit.Configuration
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class OguDataOperationSettings : ConfigurationSection
	{
		/// <summary>
		/// 读取配置信息内容
		/// </summary>
		/// <returns></returns>
		public static OguDataOperationSettings GetConfig()
		{
			OguDataOperationSettings result = (OguDataOperationSettings)ConfigurationBroker.GetSection("oguDataOperationSettings");

			if (result == null)
				result = new OguDataOperationSettings();

			return result;
		}

		/// <summary>
		/// 机构人员数据操作类
		/// </summary>
		public IEnumerable<IOguDataOperation> Operations
		{
			get
			{
				if (OperationElements.Count > 0)
				{
					foreach (TypeConfigurationElement elem in OperationElements)
						yield return (IOguDataOperation)elem.CreateInstance();
				}
				else
					yield return new OguDBOperation();
			}
		}

		[ConfigurationProperty("operations")]
		private TypeConfigurationCollection OperationElements
		{
			get
			{
				return (TypeConfigurationCollection)this["operations"];
			}
		}
	}
}
