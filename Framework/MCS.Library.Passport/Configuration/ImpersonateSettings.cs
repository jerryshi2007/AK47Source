#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	ImpersonateSettings.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          ����ǿ      2008-12-2       ���ע��
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Library.Passport
{
	/// <summary>
	/// �����û���������Ҫ��������Ϣ
	/// </summary>
	public class ImpersonateSettings : ConfigurationSection
	{
		/// <summary>
		/// ��ȡ�û�����������Ϣ
		/// </summary>
		/// <returns>�û�����������Ϣ</returns>
		/// <remarks>
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\ConfigurationTest.cs" region="ImpersonateConfigTest" lang="cs" title="��ȡ��ݰ�����Ϣ" />
		/// </remarks>
		public static ImpersonateSettings GetConfig()
		{
			ImpersonateSettings settings = (ImpersonateSettings)ConfigurationBroker.GetSection("impersonateSettings");

			if (settings == null)
				settings = new ImpersonateSettings();

			return settings;
		}

		private ImpersonateSettings()
		{
		}

		/// <summary>
		/// �Ƿ�ʹ��HttpHeader�е�testUserID��
		/// </summary>
		[ConfigurationProperty("enableTestUser", IsRequired = false, DefaultValue = false)]
		public bool EnableTestUser
		{
			get
			{
				return (bool)this["enableTestUser"];
			}
		}

		/// <summary>
		/// ��Ҫ���ݵ��û�����
		/// </summary>
		[ConfigurationProperty("impersonation")]
		public ImpersonateConfigurationElementCollection Impersonation
		{
			get
			{
				return (ImpersonateConfigurationElementCollection)this["impersonation"];
			}
		}
	}
	/// <summary>
	/// �û��������ü���
	/// </summary>
	public class ImpersonateConfigurationElementCollection : ConfigurationElementCollection
	{
		private ImpersonateConfigurationElementCollection()
		{
		}
		/// <summary>
		/// �����µ������
		/// </summary>
		/// <returns>�µ������</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new ImpersonateConfigurationElement();
		}
		/// <summary>
		/// ��ȡ�������ý��е�ԭ�û���
		/// </summary>
		/// <param name="element">�û��������ý�</param>
		/// <returns>ԭ�û���</returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ImpersonateConfigurationElement)element).SourceUserID.ToLower();
		}

		/// <summary>
		/// �õ��û�sourceUserID�����ݵ�Ŀ���û������û�����ã��򷵻�sourceUserID
		/// </summary>
		/// <param name="sourceUserID">Դ�û�</param>
		/// <returns>���ݵ��û�</returns>
		public new string this[string sourceUserID]
		{
			get
			{
				ExceptionHelper.CheckStringIsNullOrEmpty(sourceUserID, "sourceUserID");
				ImpersonateConfigurationElement element = (ImpersonateConfigurationElement)BaseGet(sourceUserID.ToLower());
				string result = sourceUserID;

				if (element != null)
					result = element.DestinationUserID;

				return result;
			}
		}
	}
	/// <summary>
	/// �û����ݻ���
	/// </summary>
	public class ImpersonateConfigurationElement : ConfigurationElement
	{
		internal ImpersonateConfigurationElement()
		{
		}
		/// <summary>
		/// �������û�ID
		/// </summary>
		[ConfigurationProperty("sourceUserID", IsRequired = true, IsKey = true)]
		public string SourceUserID
		{
			get
			{
				return (string)this["sourceUserID"];
			}
		}
		/// <summary>
		/// ���������û�ID
		/// </summary>
		[ConfigurationProperty("destinationUserID", IsRequired = true)]
		public string DestinationUserID
		{
			get
			{
				return (string)this["destinationUserID"];
			}
		}
	}
}
