using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using MCS.Library.Core;
namespace MCS.Library.Configuration
{
	/// <summary>
	/// ��������û���Ϣƾ�ݵ�����Ԫ��.    
	/// </summary>     
	public class IdentityConfigurationElement : NamedConfigurationElement
	{
		#region Private const
		private const string UserIdItem = "userId";
		private const string PasswordItem = "password";
		private const string DomainItem = "domain";
		private const string IntegratedItem = "integrated";
		#endregion

		/// <summary>
		/// �û����
		/// </summary>
		[ConfigurationProperty(IdentityConfigurationElement.UserIdItem)]
		public string UserId
		{
			get
			{
				return base[IdentityConfigurationElement.UserIdItem] as string;
			}
		}


		/// <summary>
		/// ����
		/// </summary>
		[ConfigurationProperty(IdentityConfigurationElement.PasswordItem)]
		public string Password
		{
			get
			{
				return base[IdentityConfigurationElement.PasswordItem] as string;
			}
		}

		/// <summary>
		/// �û����ڵ���
		/// </summary>
		[ConfigurationProperty(IdentityConfigurationElement.DomainItem)]
		public string Domain
		{
			get
			{
				return base[IdentityConfigurationElement.DomainItem] as string;
			}
		}

		/// <summary>
		/// �Ƿ���ü�����֤��ʽ
		/// </summary>
		[ConfigurationProperty(IdentityConfigurationElement.IntegratedItem)]
		public bool IsIntegrated
		{
			get
			{
				return bool.Parse(base[IdentityConfigurationElement.IntegratedItem] as string);
			}
		}

		/// <summary>
		/// ת��ΪLogOnIdentity
		/// </summary>
		/// <returns></returns>
		public LogOnIdentity ToLogOnIdentity()
		{
			return new LogOnIdentity(this.UserId, this.Password, this.Domain);
		}
	}


	/// <summary>
	/// װ�ػ����û���Ϣƾ�ݵ�����Ԫ�ؼ���
	/// </summary>
	[ConfigurationCollection(typeof(IdentityConfigurationElement),
	CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public class IdentityConfigurationElementCollection : NamedConfigurationElementCollection<IdentityConfigurationElement>
	{
	}
}
