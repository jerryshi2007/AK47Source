using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.Configuration;
using MCS.Library.SOA.DataObjects.Security.Transfer;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	/// <summary>
	/// 权限中心到AD同步设置
	/// </summary>
	public sealed class PermissionCenterToADSynchronizeSettings : ConfigurationSection
	{
		private PermissionCenterToADSynchronizeSettings()
		{
		}

		/// <summary>
		/// 得到配置信息
		/// </summary>
		/// <returns></returns>
		public static PermissionCenterToADSynchronizeSettings GetConfig()
		{
			PermissionCenterToADSynchronizeSettings result = (PermissionCenterToADSynchronizeSettings)ConfigurationBroker.GetSection("permissionCenterToADSynchronizeSettings");

			if (result == null)
				result = new PermissionCenterToADSynchronizeSettings();

			return result;
		}

		/// <summary>
		/// 默认开始路径(不填或空则默认为<see cref="SourceRoot"/>)。不得以\开始或结尾
		/// </summary>
		[ConfigurationProperty("defaultStartPath", IsRequired = false)]
		[RegexStringValidator(@"^[^\\]+(\\[^\\]+)*$|^$")]
		public string DefaultStartPath
		{
			get
			{
				return (string)this["defaultStartPath"];
			}
		}

		/// <summary>
		/// 目标根组织
		/// </summary>
		[ConfigurationProperty("targetRootOU", IsRequired = true)]
		[RegexStringValidator(@"^[^\\]+(\\[^\\]+)*$|^$")]
		public string TargetRootOU
		{
			get
			{
				return (string)this["targetRootOU"];
			}
		}

		/// <summary>
		/// 权限中心相对根组织节点路径（用\分隔）
		/// </summary>
		[ConfigurationProperty("sourceRoot", IsRequired = true, DefaultValue = "*")]
		[RegexStringValidator(@"^[^\\]+(\\[^\\]+)*$")]
		public string SourceRoot
		{
			get
			{
				return (string)this["sourceRoot"];
			}
		}

		/// <summary>
		/// 回收站名称
		/// </summary>
		[ConfigurationProperty("recycleBinOU", IsRequired = false, DefaultValue = "回收站")]
		[StringValidator(MinLength = 1, MaxLength = 256, InvalidCharacters = "\\")]
		public string RecycleBinOU
		{
			get
			{
				return (string)this["recycleBinOU"];
			}
		}

		/// <summary>
		/// 默认密码
		/// </summary>
		[ConfigurationProperty("defaultPassword", IsRequired = false, DefaultValue = "p@ssw0rd")]
		public string DefaultPassword
		{
			get
			{
				return (string)this["defaultPassword"];
			}
		}

		/// <summary>
		/// Schema的属性映射
		/// </summary>
		[ConfigurationProperty("schemaMappings", IsRequired = false)]
		public SchemaMappingConfigurationElementCollection SchemaMappings
		{
			get
			{
				return (SchemaMappingConfigurationElementCollection)this["schemaMappings"];
			}
		}

		[ConfigurationProperty("objectMappings", IsRequired = false)]
		public ObjectMappingElementCollection ObjectMappings
		{
			get
			{
				return (ObjectMappingElementCollection)this["objectMappings"];
			}
		}
	}
}