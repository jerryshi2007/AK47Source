using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using MCS.Library.Core;
namespace MCS.Library.Configuration
{
	/// <summary>
	/// 定义基本用户信息凭据的配置元素.    
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
		/// 用户编号
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
		/// 密码
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
		/// 用户所在的域
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
		/// 是否采用集成认证方式
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
		/// 转换为LogOnIdentity
		/// </summary>
		/// <returns></returns>
		public LogOnIdentity ToLogOnIdentity()
		{
			return new LogOnIdentity(this.UserId, this.Password, this.Domain);
		}
	}


	/// <summary>
	/// 装载基本用户信息凭据的配置元素集合
	/// </summary>
	[ConfigurationCollection(typeof(IdentityConfigurationElement),
	CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public class IdentityConfigurationElementCollection : NamedConfigurationElementCollection<IdentityConfigurationElement>
	{
	}
}
