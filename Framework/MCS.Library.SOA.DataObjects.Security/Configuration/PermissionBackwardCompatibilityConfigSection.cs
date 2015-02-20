using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects.Security.Configuration
{
	/// <summary>
	/// 表示向后兼容原有权限中心所需的配置
	/// </summary>
	public sealed class PermissionBackwardCompatibilityConfigSection : ConfigurationSection
	{
		private static ConfigurationPropertyCollection _Properties;
		private static bool _ReadOnly;

		private static readonly ConfigurationProperty _StandardManageDimensionality = new ConfigurationProperty("smdId", typeof(string), "", ConfigurationPropertyOptions.IsRequired); // 标准管理维度

		public static PermissionBackwardCompatibilityConfigSection GetConfig()
		{
			PermissionBackwardCompatibilityConfigSection settings = (PermissionBackwardCompatibilityConfigSection)ConfigurationBroker.GetSection("pcBCConfig");

			if (settings == null)
				settings = new PermissionBackwardCompatibilityConfigSection();

			return settings;
		}

		public PermissionBackwardCompatibilityConfigSection()
		{
			_Properties =
			  new ConfigurationPropertyCollection();
			_Properties.Add(_StandardManageDimensionality);
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return _Properties;
			}
		}

		private new bool IsReadOnly
		{
			get
			{
				return _ReadOnly;
			}
		}

		/// <summary>
		/// 获取或设置标准管理维度的ID
		/// </summary>
		[StringValidator(InvalidCharacters = " ~!@#$%^&*()[]{}/;'\"|\\",
		 MinLength = 1, MaxLength = 60)]
		public string StandardManageDimensionalityID
		{
			get
			{
				return (string)this["smdId"];
			}

			set
			{
				// With this you disable the setting.

				// Remember that the _ReadOnly flag must

				// be set to true in the GetRuntimeObject.

				ThrowIfReadOnly("smdId");
				this["smdId"] = value;
			}
		}

		private void ThrowIfReadOnly(string propertyName)
		{
			if (IsReadOnly)
				throw new ConfigurationErrorsException(
					"属性 " + propertyName + " 为只读");
		}

		// 防止运行时修改属性
		protected override object GetRuntimeObject()
		{
			// To enable property setting just assign true to

			// the following flag.

			_ReadOnly = true;
			return base.GetRuntimeObject();
		}
	}
}
