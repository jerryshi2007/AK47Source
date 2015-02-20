using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Net.Mail;
using System.Net;

namespace MCS.Library.SOA.DataObjects
{
	public enum EmailMessageAfterSentOP
	{
		/// <summary>
		/// 移动到已发送表中
		/// </summary>
		MoveToSentTable = 0,

		/// <summary>
		/// 不保存
		/// </summary>
		NotPersisted = 1,

		/// <summary>
		/// 仅保存错误的消息
		/// </summary>
		OnlyPersistErrorMessages = 2
	}

	/// <summary>
	/// SMTP参数
	/// </summary>
	[Serializable]
	public class SmtpParameters
	{
		private ServerInfo _ServerInfo = null;

		public ServerInfo ServerInfo
		{
			get { return this._ServerInfo; }
			set { this._ServerInfo = value; }
		}

		public SmtpDeliveryMethod DeliveryMethod
		{
			get;
			set;
		}

		private bool _UseDefaultCredentials = true;

		public bool UseDefaultCredentials
		{
			get
			{
				return this._UseDefaultCredentials;
			}
			set
			{
				this._UseDefaultCredentials = value;
			}
		}

		/// <summary>
		/// 是否保存发送后的邮件
		/// </summary>
		public EmailMessageAfterSentOP AfterSentOP
		{
			get;
			set;
		}

		public void CheckParameters()
		{
			(this._ServerInfo != null).FalseThrow("ServerInfo不能为空");

			_ServerInfo.ServerName.IsNotEmpty().FalseThrow("ServerInfo中的ServerName不能为空");
		}

		/// <summary>
		/// 默认的发送人
		/// </summary>
		public EmailAddress DefaultSender
		{
			get;
			set;
		}

		public SmtpClient ToSmtpClient()
		{
			CheckParameters();

			int port = this.ServerInfo.Port;

			if (port == 0)
				port = 25;

			SmtpClient client = new SmtpClient(this.ServerInfo.ServerName, port);

			client.DeliveryMethod = this.DeliveryMethod;
			client.UseDefaultCredentials = this.UseDefaultCredentials;

			if (this.UseDefaultCredentials)
			{
				client.Credentials = CredentialCache.DefaultNetworkCredentials;
			}
			else
			{
				(this.ServerInfo.Identity != null).FalseThrow("使用认证时，ServerInfo的Identity属性不能为空");

				client.Credentials = this.ServerInfo.Identity.ToNetworkCredentials();
			}

			return client;
		}
	}
}
