#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	LogOnIdentity.cs
// Remark	��	��װ�û���¼��Ϣ���࣬������¼�����ɰ�������������¼���������������������Ϳ�� 
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Collections.Generic;
using System.Net;

namespace MCS.Library.Core
{
	/// <summary>
	/// ��װ�û���¼��Ϣ����  
	/// </summary>
	/// <remarks>��װ�û���¼��Ϣ���࣬������¼�����ɰ�������������¼���������������������Ϳ��</remarks>
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
		/// ���캯��
		/// </summary>
		public LogOnIdentity()
		{
		}

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="logonName">��¼�������Ժ�����</param>
		/// <remarks>���캯��
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LogOnIdentityTest.cs" region="LogOnIdentityParserTest" lang="cs" title="����һ��LogOnIdentityʵ��" />
		/// </remarks>
		public LogOnIdentity(string logonName)
		{
			LogOnName = logonName;
		}

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="logonUserName">��¼�������Ժ�����</param>
		/// <param name="pwd">����</param>
		/// <remarks>
		/// ���캯��
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LogOnIdentityTest.cs" region="LogOnIdentityParserTest" lang="cs" title="����һ��LogOnIdentityʵ��" />
		/// </remarks>
		public LogOnIdentity(string logonUserName, string pwd)
		{
			LogOnName = logonUserName;

			this.password = pwd;
		}

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="logonUserName">��¼�������Ժ�����</param>
		/// <param name="pwd">����</param>
		/// <param name="logonDomain">����</param>
		/// <remarks>
		/// ���캯��
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LogOnIdentityTest.cs" region="LogOnIdentityParserTest" lang="cs" title="����һ��LogOnIdentityʵ��" />
		/// </remarks>
		public LogOnIdentity(string logonUserName, string pwd, string logonDomain)
		{
			LogOnName = logonUserName;

			this.password = pwd;
			if (string.IsNullOrEmpty(logonDomain) == false)
				this.domain = logonDomain;
		}

		/// <summary>
		/// ��¼�������Ժ�����
		/// </summary>
		/// <remarks>�������ǿɶ���д��</remarks>
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
		/// ���������ĵ�¼��
		/// </summary>
		/// <remarks>��������ֻ����</remarks>
		public string LogOnNameWithoutDomain
		{
			get
			{
				return this.logOnNameWithoutDomain;
			}
		}

		/// <summary>
		/// ���������ĵ�¼��
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
		/// ����
		/// </summary>
		/// <remarks>��������ֻ����</remarks>
		public string Domain
		{
			get
			{
				return this.domain;
			}
		}

		/// <summary>
		/// ����
		/// </summary>
		/// <remarks>�������ǿɶ���д��</remarks>
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
		/// ��������Ϣ
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
		/// ת����NetworkCredential
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
