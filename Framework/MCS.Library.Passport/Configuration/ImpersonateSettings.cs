#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	ImpersonateSettings.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          胡自强      2008-12-2       添加注释
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
	/// 设置用户扮演所需要的配置信息
	/// </summary>
	public class ImpersonateSettings : ConfigurationSection
	{
		/// <summary>
		/// 读取用户扮演配置信息
		/// </summary>
		/// <returns>用户扮演配置信息</returns>
		/// <remarks>
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\ConfigurationTest.cs" region="ImpersonateConfigTest" lang="cs" title="获取身份扮演信息" />
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
		/// 是否使用HttpHeader中的testUserID项
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
		/// 需要扮演的用户集合
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
	/// 用户扮演配置集合
	/// </summary>
	public class ImpersonateConfigurationElementCollection : ConfigurationElementCollection
	{
		private ImpersonateConfigurationElementCollection()
		{
		}
		/// <summary>
		/// 创建新的配置项。
		/// </summary>
		/// <returns>新的配置项。</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new ImpersonateConfigurationElement();
		}
		/// <summary>
		/// 获取基本配置节中的原用户名
		/// </summary>
		/// <param name="element">用户扮演配置节</param>
		/// <returns>原用户名</returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ImpersonateConfigurationElement)element).SourceUserID.ToLower();
		}

		/// <summary>
		/// 得到用户sourceUserID将扮演的目标用户，如果没有配置，则返回sourceUserID
		/// </summary>
		/// <param name="sourceUserID">源用户</param>
		/// <returns>扮演的用户</returns>
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
	/// 用户扮演基类
	/// </summary>
	public class ImpersonateConfigurationElement : ConfigurationElement
	{
		internal ImpersonateConfigurationElement()
		{
		}
		/// <summary>
		/// 扮演者用户ID
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
		/// 被扮演者用户ID
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
