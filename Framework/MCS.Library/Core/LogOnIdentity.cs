#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	LogOnIdentity.cs
// Remark	：	封装用户登录信息的类，包括登录名（可包含域名），登录名（不含域名），域名和口令。 
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    沈峥	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Collections.Generic;
using System.Net;

namespace MCS.Library.Core
{
	/// <summary>
	/// 封装用户登录信息的类  
	/// </summary>
	/// <remarks>封装用户登录信息的类，包括登录名（可包含域名），登录名（不含域名），域名和口令。</remarks>
	[Serializable]
	[XElementSerializable]
	public class LogOnIdentity
	{
		private string logOnName = string.Empty;
		private string logOnNameWithoutDomain = string.Empty;
		private string domain = string.Empty;
		private string password = string.Empty;

		private Dictionary<string, object> context = null;

		/// <summary>
		/// 构造函数
		/// </summary>
		public LogOnIdentity()
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="logonName">登录名，可以含域名</param>
		/// <remarks>构造函数
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LogOnIdentityTest.cs" region="LogOnIdentityParserTest" lang="cs" title="创建一个LogOnIdentity实例" />
		/// </remarks>
		public LogOnIdentity(string logonName)
		{
			LogOnName = logonName;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="logonUserName">登录名，可以含域名</param>
		/// <param name="pwd">口令</param>
		/// <remarks>
		/// 构造函数
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LogOnIdentityTest.cs" region="LogOnIdentityParserTest" lang="cs" title="创建一个LogOnIdentity实例" />
		/// </remarks>
		public LogOnIdentity(string logonUserName, string pwd)
		{
			LogOnName = logonUserName;

			this.password = pwd;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="logonUserName">登录名，可以含域名</param>
		/// <param name="pwd">口令</param>
		/// <param name="logonDomain">域名</param>
		/// <remarks>
		/// 构造函数
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LogOnIdentityTest.cs" region="LogOnIdentityParserTest" lang="cs" title="创建一个LogOnIdentity实例" />
		/// </remarks>
		public LogOnIdentity(string logonUserName, string pwd, string logonDomain)
		{
			LogOnName = logonUserName;

			this.password = pwd;
			if (string.IsNullOrEmpty(logonDomain) == false)
				this.domain = logonDomain;
		}

		/// <summary>
		/// 登录名，可以含域名
		/// </summary>
		/// <remarks>该属性是可读可写的</remarks>
		public string LogOnName
		{
			get
			{
				return this.logOnName;
			}
			set
			{
				this.logOnName = value;
				AnalysisLogOnName(this.logOnName);
			}
		}

		/// <summary>
		/// 不含域名的登录名
		/// </summary>
		/// <remarks>该属性是只读的</remarks>
		public string LogOnNameWithoutDomain
		{
			get
			{
				return this.logOnNameWithoutDomain;
			}
		}

		/// <summary>
		/// 包含域名的登录名
		/// </summary>
		public string LogOnNameWithDomain
		{
			get
			{
				string result = this.logOnNameWithoutDomain;

				if (string.IsNullOrEmpty(this.domain) == false)
				{
					if (this.domain.IndexOf(".") >= 0)
						result = this.logOnNameWithoutDomain + "@" + this.domain;
					else
						result = this.domain + "\\" + this.logOnNameWithoutDomain;
				}

				return result;
			}
		}

		/// <summary>
		/// 域名
		/// </summary>
		/// <remarks>该属性是只读的</remarks>
		public string Domain
		{
			get
			{
				return this.domain;
			}
		}

		/// <summary>
		/// 口令
		/// </summary>
		/// <remarks>该属性是可读可写的</remarks>
		public string Password
		{
			get
			{
				return this.password;
			}
			set
			{
				this.password = value;
			}
		}

		/// <summary>
		/// 上下文信息
		/// </summary>
		public Dictionary<string, object> Context
		{
			get
			{
				if (this.context == null)
					this.context = new Dictionary<string, object>();

				return this.context;
			}
		}

		/// <summary>
		/// 转换成NetworkCredential
		/// </summary>
		/// <returns></returns>
		public NetworkCredential ToNetworkCredentials()
		{
			return new NetworkCredential(this.LogOnNameWithoutDomain, this.Password, this.Domain);
		}

		private void AnalysisLogOnName(string strLogOnName)
		{
			this.logOnNameWithoutDomain = string.Empty;
			this.domain = string.Empty;

			if (string.IsNullOrEmpty(strLogOnName) == false)
			{
				string[] nameParts = strLogOnName.Split('/', '\\');

				string strNameWithoutDomain = string.Empty;

				if (nameParts.Length > 1)
				{
					this.domain = nameParts[0];
					strNameWithoutDomain = nameParts[1];
				}
				else
					strNameWithoutDomain = nameParts[0];

				string[] nameParts2 = strNameWithoutDomain.Split('@');

				this.logOnNameWithoutDomain = nameParts2[0];

				if (nameParts2.Length > 1)
					if (string.IsNullOrEmpty(this.domain))
						this.domain = nameParts2[1];
			}
		}
	}
}
