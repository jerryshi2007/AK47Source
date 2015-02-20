using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 处里人员不存在流程相关设置
	/// </summary>
	public class SendInvalidNotificationSettingsSection : ConfigurationSection
	{
		/// <summary>
		/// 获取Cache的配置信息
		/// </summary>
		/// <returns>Cache的配置信息</returns>
		public static SendInvalidNotificationSettingsSection GetConfig()
		{
			SendInvalidNotificationSettingsSection result = (SendInvalidNotificationSettingsSection)ConfigurationBroker.GetSection("sendInvalidNotificationSettings");

			if (result == null)
				result = new SendInvalidNotificationSettingsSection();

			return result;
		}

		/// <summary>
		/// 最大发送待办条数
		/// </summary>
		[ConfigurationProperty("maxSendCount", DefaultValue = 100)]
		public int MaxSendCount
		{
			get
			{
				return (int)this["maxSendCount"];
			}
		}
	}
}
