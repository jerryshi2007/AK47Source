using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 邮件消息相关的配置
	/// </summary>
	public sealed class EmailMessageSettings : ConfigurationSection
	{
		/// <summary>
		/// 得到邮件配置的信息
		/// </summary>
		/// <returns></returns>
		public static EmailMessageSettings GetConfig()
		{
			EmailMessageSettings settings = (EmailMessageSettings)ConfigurationBroker.GetSection("emailMessageSettings");

			ConfigurationExceptionHelper.CheckSectionNotNull(settings, "emailMessageSettings");

			return settings;
		}

		/// <summary>
		/// 使用的连接串名称是什么，默认是HB2008
		/// </summary>
		[ConfigurationProperty("connectionName", DefaultValue = "", IsRequired = false)]
		public string ConnectionName
		{
			get
			{
				return (string)this["connectionName"];
			}
		}

		/// <summary>
		/// 对应的ServerInfo配置项的名称
		/// </summary>
		[ConfigurationProperty("serverInfoName")]
		public string ServerInfoName
		{
			get
			{
				return (string)this["serverInfoName"];
			}
		}

		[ConfigurationProperty("useDefaultCredentials")]
		public bool UseDefaultCredentials
		{
			get
			{
				return (bool)this["useDefaultCredentials"];
			}
		}

		/// <summary>
		/// 是否保存发送后的邮件
		/// </summary>
		[ConfigurationProperty("afterSentOP", DefaultValue = EmailMessageAfterSentOP.MoveToSentTable, IsRequired = false)]
		public EmailMessageAfterSentOP AfterSentOP
		{
			get
			{
				return (EmailMessageAfterSentOP)this["afterSentOP"];
			}
		}

		[ConfigurationProperty("defaultSender", DefaultValue = "", IsRequired = false)]
		public string DefaultSender
		{
			get
			{
				return (string)this["defaultSender"];
			}
		}

		public SmtpParameters ToSmtpParameters()
		{
			ServerInfoConfigSettings.GetConfig().ServerInfos.ContainsKey(ServerInfoName).FalseThrow("生成Smtp信息时，不能找到'{0}'对应的ServerInfo信息",
				ServerInfoName);

			SmtpParameters result = new SmtpParameters();

			result.ServerInfo = ServerInfoConfigSettings.GetConfig().ServerInfos[ServerInfoName].ToServerInfo();
			result.AfterSentOP = this.AfterSentOP;
			result.UseDefaultCredentials = this.UseDefaultCredentials;
			result.DefaultSender = EmailAddress.FromDescription(this.DefaultSender);

			return result;
		}
	}
}
