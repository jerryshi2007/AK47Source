using System;
using System.Text;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Configuration;
using MCS.Library.Data;
using MCS.Library;
using MCS.Library.SOA.DataObjects.Properties;

namespace MCS.Library.SOA.DataObjects
{

	#region AppPath

	/// <summary>
	/// 路径配置类
	/// </summary>
	public sealed class AppPathConfigSettings : DeluxeConfigurationSection
	{
        private AppPathConfigSettings() 
        { }

		private const string AppPathConfigName = "appPathSettings";

		/// <summary>
		/// 获得路径配置信息
		/// </summary>
		/// <returns>路径配置信息</returns>
		public static AppPathConfigSettings GetConfig()
		{
			AppPathConfigSettings result =
				(AppPathConfigSettings)ConfigurationBroker.GetSection(AppPathConfigName);


			ConfigurationExceptionHelper.CheckSectionNotNull(result, AppPathConfigName);

			return result;
		}

		/// <summary>
		/// 路径集合
		/// </summary>
		[ConfigurationProperty(AppPathSettingsCollection.Name)]
		public AppPathSettingsCollection Paths
		{
			get
			{
				AppPathSettingsCollection pathConfigCollection
					= (AppPathSettingsCollection)base[AppPathSettingsCollection.Name];

				return pathConfigCollection;
			}
		}

		public bool IsInConfigedPath(string path)
		{
			bool result = false;

			string fullPath = Path.GetFullPath(path).ToLower();

			for (int i = 0; i < this.Paths.Count; i++)
			{
				string lowerConfigedPath = this.Paths[i].Dir.ToLower();

				if (fullPath.IndexOf(lowerConfigedPath) == 0)
				{
					result = true;
					break;
				}
			}

			return result;
		}

		public void CheckPathIsConfiged(string path)
		{
            ExceptionHelper.FalseThrow(IsInConfigedPath(path), string.Format(Resource.IllegalPath, path));
		}
	}

	/// <summary>
	/// 路径集合
	/// </summary>
	[ConfigurationCollection(typeof(AppPathSettingsElement))]
	public sealed class AppPathSettingsCollection : NamedConfigurationElementCollection<AppPathSettingsElement>
	{
		//路径集合名称
		public const string Name = "paths";
	}

	/// <summary>
	/// 路径
	/// </summary>
	public class AppPathSettingsElement : NamedConfigurationElement
	{
		private string dir = null;
		/// <summary>
		/// 路径
		/// </summary>
		[ConfigurationProperty("dir", IsRequired = true)]
		public string Dir
		{
			get
			{
				if (this.dir == null)
				{
					this.dir = (string)this["dir"];

					ExceptionHelper.CheckStringIsNullOrEmpty(this.dir, "dir");

					if (this.dir.EndsWith("\\") == false)
						this.dir += "\\";
				}

				return this.dir;
			}
		}
	}

	#endregion

	#region HBWebHelperControlConfigSetting

	public sealed class HBWebHelperControlConfigSetting : DeluxeConfigurationSection
	{
		private const string LockSettingsName = "hBWebHelperControlSettings";

		public static HBWebHelperControlConfigSetting GetConfig()
		{
			HBWebHelperControlConfigSetting result =
				   (HBWebHelperControlConfigSetting)ConfigurationBroker.GetSection(LockSettingsName);

			ConfigurationExceptionHelper.CheckSectionNotNull(result, LockSettingsName);

			return result;
		}

		/// <summary>
		/// CAB路径
		/// </summary>
		[ConfigurationProperty("path", IsRequired = true)]
		public string Path
		{
			get
			{
				return  this["path"].ToString();
			}
		}
	}

	#endregion

}
