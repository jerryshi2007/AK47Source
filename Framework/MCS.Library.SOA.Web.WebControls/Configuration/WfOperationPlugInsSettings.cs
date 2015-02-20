using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 工作流流转控件操作的插件
	/// </summary>
	public class WfControlOperationPlugInsSettings : ConfigurationSection
	{
		/// <summary>
		/// 得到配置信息
		/// </summary>
		/// <returns></returns>
		public static WfControlOperationPlugInsSettings GetConfig()
		{
			WfControlOperationPlugInsSettings settings = (WfControlOperationPlugInsSettings)ConfigurationBroker.GetSection("wfControlOperationPlugInsSettings");

			if (settings == null)
				settings = new WfControlOperationPlugInsSettings();

			return settings;
		}

		private WfControlOperationPlugInsSettings()
		{
		}

		/// <summary>
		/// 插件配置信息
		/// </summary>
		[ConfigurationProperty("plugIns", IsRequired = false)]
		public TypeConfigurationCollection PlugIns
		{
			get
			{
				return (TypeConfigurationCollection)this["plugIns"];
			}
		}
	}
}
