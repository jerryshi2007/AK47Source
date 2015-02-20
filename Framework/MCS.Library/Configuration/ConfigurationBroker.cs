#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	ConfigurationBroker.cs
// Remark	：	Broker class to manage all local config file as well as mapping config file(s) Remote config mapping could via 
//              ConfigurationFileMap and retrive by ConfigurationManager.OpenMappedMachineConfiguration
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    王翔	    20070430		创建
// -------------------------------------------------
#endregion
#region using

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Configuration;
using System.Text.RegularExpressions;

using MCS.Library.Core;
using MCS.Library.Caching;
using MCS.Library.Properties;

#endregion

namespace MCS.Library.Configuration
{
	/// <summary>
	/// <remarks>
	/// Broker类管理所有本地配置文件和映射配置文件。
	/// 远程配置文件映射由ConfigurationFileMap和ConfigurationManager.OpenMappedMachineConfiguration处理.
	/// 
	/// 约束:
	///     <list type="bullet">
	///         <item>
	///         映射文件必须以ConfigurationSectionGroup或ConfigurationSection开始.
	///         </item>
	///         <item>
	///         </item>
	///     </list>
	/// </remarks>
	/// </summary>
	public static class ConfigurationBroker
	{
		#region private const and field
		/// <summary>
		/// Private const
		/// </summary>
		private const string LocalItem = "local";
		private const string MetaConfigurationItem = "MCS.MetaConfiguration";
		private const string MetaConfigurationSectionGroupItem = "mcs.library.metaConfig";

		/// <summary>
		/// Private static field
		/// </summary>
		private static readonly string MachineConfigurationFile = ConfigurationManager.OpenMachineConfiguration().FilePath;
		private static readonly string LocalConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
		private static readonly string GlobalConfigurationFile = ConfigurationBroker.MachineConfigurationFile;
		// 缓存失效时间
		private static readonly TimeSpan SlidingTime = TimeSpan.FromSeconds(15.0);

		#endregion private const and field

		/// <summary>
		/// 构造函数
		/// </summary>
		static ConfigurationBroker()
		{
		}

		/// <summary>
		/// meta配置文件位置枚举
		/// </summary>
		private enum MetaConfigurationPosition
		{
			LocalFile,
			MetaFile
		}

		/// <summary>
		/// 内部类，用于存放、传递machine、local、meta、global配置文件的地址和
		/// meta文件位置（枚举） 
		/// </summary>
		private class ConfigFilesSetting
		{
			private string machineConfigurationFile = string.Empty;
			private string localConfigurationFile = string.Empty;
			private string metaConfigurationFile = string.Empty;
			private string globalConfigurationFile = string.Empty;
			private MetaConfigurationPosition metaFilePosition = MetaConfigurationPosition.LocalFile;

			public MetaConfigurationPosition MetaFilePosition
			{
				get { return this.metaFilePosition; }
				set { this.metaFilePosition = value; }
			}

			public string GlobalConfigurationFile
			{
				get { return this.globalConfigurationFile; }
				set { this.globalConfigurationFile = value; }
			}

			public string MetaConfigurationFile
			{
				get { return this.metaConfigurationFile; }
				set { this.metaConfigurationFile = value; }
			}

			public string LocalConfigurationFile
			{
				get { return this.localConfigurationFile; }
				set { this.localConfigurationFile = value; }
			}

			public string MachineConfigurationFile
			{
				get { return this.machineConfigurationFile; }
				set { this.machineConfigurationFile = value; }
			}
		}

		#region private static method
		/// <summary>
		/// 生成configuration对象的缓存key值
		/// </summary>
		/// <param name="fileNames">文件列表</param>
		/// <returns>cache key</returns>
		private static string CreateConfigurationCacheKey(params string[] fileNames)
		{
			StringBuilder key = new StringBuilder(256);

			for (int i = 0; i < fileNames.Length; i++)
			{
				// 只取文件名，去掉完整路径
				key.Append(Path.GetFileName(fileNames[i]).ToLower());
			}

			return key.ToString();
		}

		/// <summary>
		/// 加载machine、local配置文件，meta配置文件，meta中的配置节，将其缓存并建立缓存失效依赖。
		/// 查找并在 ConfigFilesSetting 类实例中记录machine、local、meta和global配置文件的地址和
		/// meta配置文件位置（枚举）
		/// </summary>
		/// <returns>ConfigFilesSetting 类实例</returns>
		private static ConfigFilesSetting LoadFilesSetting()
		{
			ConfigFilesSetting settings = new ConfigFilesSetting();
			settings.MachineConfigurationFile = ConfigurationBroker.MachineConfigurationFile;
			settings.LocalConfigurationFile = ConfigurationBroker.LocalConfigurationFile;
			settings.GlobalConfigurationFile = ConfigurationBroker.GlobalConfigurationFile;

			MetaConfigurationSourceInstanceSection metaSection = ConfigurationBroker.GetMetaSourceInstanceSection(settings);

			if (metaSection != null)
			{
				string currPath;

				if (EnvironmentHelper.IsUsingWebConfig)
					currPath = HostingEnvironment.ApplicationVirtualPath;
				else
					currPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

				settings.GlobalConfigurationFile = EnvironmentHelper.ReplaceEnvironmentVariablesInString(metaSection.Instances.GetMatchedPath(currPath));

				if (string.IsNullOrEmpty(settings.GlobalConfigurationFile))
					settings.GlobalConfigurationFile = ConfigurationBroker.GlobalConfigurationFile;
				else
				{
					if (false == Path.IsPathRooted(settings.GlobalConfigurationFile))
						settings.GlobalConfigurationFile =
							AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\" + settings.GlobalConfigurationFile;

					ExceptionHelper.FalseThrow(File.Exists(settings.GlobalConfigurationFile), Resource.GlobalFileNotFound, settings.GlobalConfigurationFile);
				}
			}

			return settings;
		}

		/// <summary>
		/// 获取meta配置中的 sourceMappings 节点
		/// </summary>
		/// <param name="fileSettings">ConfigFilesSetting 类实例</param>
		/// <returns>meta配置中的 sourceMappings 节点</returns>
		private static MetaConfigurationSourceInstanceSection GetMetaSourceInstanceSection(ConfigFilesSetting fileSettings)
		{
			ConfigurationSection section;

			string cacheKey = ConfigurationBroker.CreateConfigurationCacheKey(fileSettings.MachineConfigurationFile,
				MetaConfigurationSourceInstanceSection.Name);

			if (ConfigurationSectionCache.Instance.TryGetValue(cacheKey, out section) == false)
			{
				ConfigurationBroker.GetMetaFileSettings(fileSettings);

				if (fileSettings.MetaFilePosition == MetaConfigurationPosition.LocalFile)
					section = ConfigurationBroker.LoadMetaSourceInstanceSectionFromLocal(fileSettings);
				else
					section = ConfigurationBroker.LoadMetaSourceInstanceSectionFromMetaFile(fileSettings);

				FileCacheDependency fileDependency = new FileCacheDependency(
					true,
					fileSettings.MachineConfigurationFile,
					fileSettings.LocalConfigurationFile,
					fileSettings.MetaConfigurationFile);

				SlidingTimeDependency timeDependency = new SlidingTimeDependency(ConfigurationBroker.SlidingTime);

				ConfigurationSectionCache.Instance.Add(cacheKey, section, new MixedDependency(fileDependency, timeDependency));
			}

			return (MetaConfigurationSourceInstanceSection)section;
		}

		/// <summary>
		/// 从本地config文件中读取meta配置
		/// </summary>
		/// <param name="fileSettings">ConfigFilesSetting 类实例</param>
		/// <returns>MetaConfigurationSourceInstanceSection 实体</returns>
		private static MetaConfigurationSourceInstanceSection LoadMetaSourceInstanceSectionFromLocal(ConfigFilesSetting fileSettings)
		{
			System.Configuration.Configuration config;

			if (EnvironmentHelper.IsUsingWebConfig)
				config = ConfigurationBroker.GetStandardWebConfiguration(fileSettings.MachineConfigurationFile, true);
			else
				config = ConfigurationBroker.GetStandardExeConfiguration(fileSettings.MachineConfigurationFile, fileSettings.LocalConfigurationFile, true);

			MetaConfigurationSectionGroup group =
				(MetaConfigurationSectionGroup)config.GetSectionGroup(ConfigurationBroker.MetaConfigurationSectionGroupItem);
			MetaConfigurationSourceInstanceSection section = null;

			if (group != null)
				section = group.SourceConfigurationMapping;

			return section;
		}

		/// <summary>
		/// 从单独的meta.config文件中读取meta配置
		/// </summary>
		/// <param name="fileSettings">ConfigFilesSetting 实体</param>
		/// <returns>MetaConfigurationSourceInstanceSection 实体</returns>
		private static MetaConfigurationSourceInstanceSection LoadMetaSourceInstanceSectionFromMetaFile(ConfigFilesSetting fileSettings)
		{
			System.Configuration.Configuration config = ConfigurationBroker.GetSingleFileConfiguration(
					fileSettings.MetaConfigurationFile,
					true,
					fileSettings.MachineConfigurationFile,
					fileSettings.LocalConfigurationFile);

			MetaConfigurationSectionGroup group =
				config.GetSectionGroup(ConfigurationBroker.MetaConfigurationSectionGroupItem) as MetaConfigurationSectionGroup;

			MetaConfigurationSourceInstanceSection section = null;

			if (group != null)
				section = group.SourceConfigurationMapping;

			return section;
		}

		/// <summary>
		/// 获取meta文件的地址和位置
		/// </summary>
		/// <param name="fileSettings">ConfigFilesSetting 类实例</param>
		private static void GetMetaFileSettings(ConfigFilesSetting fileSettings)
		{
			AppSettingsSection section = ConfigurationBroker.GetLocalAppSettingsSection();

			if (section != null)
			{
				if (section.Settings[ConfigurationBroker.MetaConfigurationItem] == null)
					fileSettings.MetaConfigurationFile = ConfigurationBroker.LocalConfigurationFile;
				else
					fileSettings.MetaConfigurationFile =
						EnvironmentHelper.ReplaceEnvironmentVariablesInString(section.Settings[ConfigurationBroker.MetaConfigurationItem].Value);
			}

			if (string.IsNullOrEmpty(fileSettings.MetaConfigurationFile) == true)
			{
				fileSettings.MetaFilePosition = MetaConfigurationPosition.LocalFile;
				fileSettings.MetaConfigurationFile = ConfigurationBroker.LocalConfigurationFile;
			}
			else
			{
				fileSettings.MetaFilePosition = MetaConfigurationPosition.MetaFile;

				if (false == Path.IsPathRooted(fileSettings.MetaConfigurationFile))
					fileSettings.MetaConfigurationFile =
						AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\" + fileSettings.MetaConfigurationFile;

				ExceptionHelper.FalseThrow(File.Exists(fileSettings.MetaConfigurationFile), Resource.MetaFileNotFound, fileSettings.MetaConfigurationFile);
			}
		}

		/// <summary>
		/// 获取最终 local 和 global 合并后的 Configuration
		/// </summary>
		/// <param name="fileSettings">ConfigFilesSetting 类实例</param>
		/// <param name="globalConfig">返回全局配置的Config</param>
		/// <returns>local 和 global 合并后的 Configuration</returns>
		private static System.Configuration.Configuration GetFinalConfiguration(ConfigFilesSetting fileSettings, out System.Configuration.Configuration globalConfig)
		{
			System.Configuration.Configuration config;

			if (EnvironmentHelper.IsUsingWebConfig)
				config = ConfigurationBroker.GetStandardWebAndGlobalConfiguration(
							fileSettings.GlobalConfigurationFile,
							true,
							out globalConfig,
							fileSettings.LocalConfigurationFile,
							fileSettings.MachineConfigurationFile,
							fileSettings.MetaConfigurationFile);
			else
				config = ConfigurationBroker.GetStandardExeAndGlobalConfiguration(fileSettings.GlobalConfigurationFile,
							fileSettings.LocalConfigurationFile,
							true,
							out globalConfig,
							fileSettings.MachineConfigurationFile,
							fileSettings.MetaConfigurationFile);

			return config;
		}

		/// <summary>
		/// 获取本地config的AppSettings节点
		/// </summary>
		/// <returns>AppSettingsSection</returns>
		private static AppSettingsSection GetLocalAppSettingsSection()
		{
			System.Configuration.Configuration config = null;

			if (EnvironmentHelper.IsUsingWebConfig)
				config = ConfigurationBroker.GetStandardWebConfiguration(ConfigurationBroker.MachineConfigurationFile, true);
			else
				config = ConfigurationBroker.GetStandardExeConfiguration(ConfigurationBroker.MachineConfigurationFile, ConfigurationBroker.LocalConfigurationFile, true);

			return config.AppSettings;
		}

		/// <summary>
		/// 取得单独config文件中的 Configuration
		/// </summary>
		/// <param name="fileName">文件地址</param>
		/// <param name="fileDependencies">缓存依赖文件</param>
		/// <param name="ignoreFileNotExist">是否忽略不存在的文件</param>
		/// <returns>Configuration对象</returns>
		private static System.Configuration.Configuration GetSingleFileConfiguration(string fileName, bool ignoreFileNotExist, params string[] fileDependencies)
		{
			string cacheKey = ConfigurationBroker.CreateConfigurationCacheKey(fileName);

			System.Configuration.Configuration config;

			if (ConfigurationCache.Instance.TryGetValue(cacheKey, out config) == false)
			{
				config = ConfigurationManager.OpenMappedMachineConfiguration(new ConfigurationFileMap(fileName));

				Array.Resize<string>(ref fileDependencies, fileDependencies.Length + 1);
				fileDependencies[fileDependencies.Length - 1] = fileName;

				ConfigurationBroker.AddConfigurationToCache(cacheKey, config, ignoreFileNotExist, fileDependencies);
			}

			return config;
		}

		/// <summary>
		/// 获取标准Web应用程序的配置信息，合并Web.config和global配置文件
		/// </summary>
		/// <param name="machineConfigPath">global配置文件地址</param>
		/// <param name="ignoreFileNotExist">是否忽略不存在的文件</param>
		/// <param name="fileDependencies">缓存依赖文件</param>
		/// <returns>Web.config和global配置文件合并后的Configuration对象</returns>
		private static System.Configuration.Configuration GetStandardWebConfiguration(string machineConfigPath, bool ignoreFileNotExist, params string[] fileDependencies)
		{
			System.Configuration.Configuration globalConfig = null;

			return GetStandardWebAndGlobalConfiguration(machineConfigPath, ignoreFileNotExist, out globalConfig, fileDependencies);
		}

		/// <summary>
		/// 获取标准Web应用程序的配置信息，合并Web.config和global配置文件
		/// </summary>
		/// <param name="machineConfigPath">global配置文件地址</param>
		/// <param name="ignoreFileNotExist">是否忽略不存在的文件</param>
		/// <param name="globalConfig">返回的全局配置</param>
		/// <param name="fileDependencies">缓存依赖文件</param>
		/// <returns>Web.config和global配置文件合并后的Configuration对象</returns>
		private static System.Configuration.Configuration GetStandardWebAndGlobalConfiguration(string machineConfigPath, bool ignoreFileNotExist, out System.Configuration.Configuration globalConfig, params string[] fileDependencies)
		{
			string cacheKey = ConfigurationBroker.CreateConfigurationCacheKey(machineConfigPath);

			System.Configuration.Configuration config;
			globalConfig = null;

			WebConfigurationFileMap fileMap = new WebConfigurationFileMap();

			fileMap.MachineConfigFilename = machineConfigPath;
			VirtualDirectoryMapping vDirMap = new VirtualDirectoryMapping(
					HostingEnvironment.ApplicationPhysicalPath,
					true);

			fileMap.VirtualDirectories.Add("/", vDirMap);

			if (ConfigurationCache.Instance.TryGetValue(cacheKey, out config) == false)
			{
				config = WebConfigurationManager.OpenMappedWebConfiguration(fileMap, "/",
					HostingEnvironment.SiteName);

				Array.Resize<string>(ref fileDependencies, fileDependencies.Length + 1);
				fileDependencies[fileDependencies.Length - 1] = machineConfigPath;

				ConfigurationBroker.AddConfigurationToCache(cacheKey, config, ignoreFileNotExist, fileDependencies);
			}

			globalConfig = GetGlobalConfiguration(machineConfigPath, ignoreFileNotExist,
					() => WebConfigurationManager.OpenMappedMachineConfiguration(fileMap),
					fileDependencies);

			return config;
		}

		/// <summary>
		/// 得到全局配置文件的config对象
		/// </summary>
		/// <param name="configFile"></param>
		/// <param name="ignoreFileNotExist"></param>
		/// <param name="getConfig"></param>
		/// <param name="fileDependencies"></param>
		/// <returns></returns>
		private static System.Configuration.Configuration GetGlobalConfiguration(string configFile, bool ignoreFileNotExist, Func<System.Configuration.Configuration> getConfig, params string[] fileDependencies)
		{
			System.Configuration.Configuration config;

			if (ConfigurationFileCache.Instance.TryGetValue(configFile, out config) == false)
			{
				if (getConfig != null)
					config = getConfig();

				MixedDependency dependency = new MixedDependency(
					new FileCacheDependency(ignoreFileNotExist, fileDependencies),
					new SlidingTimeDependency(ConfigurationBroker.SlidingTime));

				ConfigurationFileCache.Instance.Add(configFile, config, dependency);
			}

			return config;
		}

		/// <summary>
		/// 获取标准WinForm应用程序的配置信息，合并App.config和global配置文件
		/// </summary>
		/// <param name="machineConfigPath">global配置文件地址</param>
		/// <param name="localConfigPath">本地应用程序配置文件地址</param>
		/// <param name="ignoreFileNotExist">是否忽略不存在的文件</param>
		/// <param name="fileDependencies">缓存依赖文件</param>
		/// <returns>App.config和global配置文件合并后的Configuration对象</returns>
		private static System.Configuration.Configuration GetStandardExeConfiguration(string machineConfigPath, string localConfigPath, bool ignoreFileNotExist, params string[] fileDependencies)
		{
			System.Configuration.Configuration globalConfig = null;

			return GetStandardExeAndGlobalConfiguration(machineConfigPath, localConfigPath, ignoreFileNotExist, out globalConfig, fileDependencies);
		}

		/// <summary>
		/// 获取标准WinForm应用程序的配置信息，合并App.config和global配置文件
		/// </summary>
		/// <param name="machineConfigPath">global配置文件地址</param>
		/// <param name="localConfigPath">本地应用程序配置文件地址</param>
		/// <param name="globalConfig">得到全局的配置</param>
		/// <param name="ignoreFileNotExist">是否忽略不存在的文件</param>
		/// <param name="fileDependencies">缓存依赖文件</param>
		/// <returns>App.config和global配置文件合并后的Configuration对象</returns>
		private static System.Configuration.Configuration GetStandardExeAndGlobalConfiguration(string machineConfigPath, string localConfigPath, bool ignoreFileNotExist, out System.Configuration.Configuration globalConfig, params string[] fileDependencies)
		{
			string cacheKey = ConfigurationBroker.CreateConfigurationCacheKey(machineConfigPath, localConfigPath);

			System.Configuration.Configuration config;
			globalConfig = null;

			ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
			fileMap.MachineConfigFilename = machineConfigPath;
			fileMap.ExeConfigFilename = localConfigPath;

			if (ConfigurationCache.Instance.TryGetValue(cacheKey, out config) == false)
			{
				config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

				Array.Resize<string>(ref fileDependencies, fileDependencies.Length + 2);
				fileDependencies[fileDependencies.Length - 2] = machineConfigPath;
				fileDependencies[fileDependencies.Length - 1] = localConfigPath;

				ConfigurationBroker.AddConfigurationToCache(cacheKey, config, ignoreFileNotExist, fileDependencies);
			}

			globalConfig = GetGlobalConfiguration(machineConfigPath, ignoreFileNotExist,
					() => ConfigurationManager.OpenMappedMachineConfiguration(fileMap),
					fileDependencies);

			return config;
		}

		/// <summary>
		/// 把Configuration对象放入缓存，建立时间和文件的混合依赖
		/// </summary>
		/// <param name="cacheKey">cache key</param>
		/// <param name="config">待缓存的Configuration对象</param>
		/// <param name="ignoreFileNotExist">是否忽略不存在的文件</param>
		/// <param name="files">缓存依赖的文件</param>
		private static void AddConfigurationToCache(string cacheKey, System.Configuration.Configuration config, bool ignoreFileNotExist, params string[] files)
		{
			MixedDependency dependency = new MixedDependency(
				new FileCacheDependency(ignoreFileNotExist, files),
				new SlidingTimeDependency(ConfigurationBroker.SlidingTime));

			ConfigurationCache.Instance.Add(cacheKey, config, dependency);
		}
		#endregion private static method

		#region Public static method

		/// <summary>
		/// 按节点名称从配置信息中取得节点，并将节点信息缓存，建立文件依赖
		/// </summary>
		/// <param name="sectionName">节点名称</param>
		/// <param name="checkNullSection">如果返回null，是否抛出异常</param>
		/// <returns>配置节点</returns>
		/// <remarks>
		/// 按名称获取配置节点信息。返回ConfigurationSection的派生类实体，实体类需由用户自定义。
		/// <code source="..\Framework\TestProjects\Deluxeworks.Library.WebTest\Configuration\Default.aspx.cs" region="Using Broker" lang="cs" title="使用配置管理" />
		/// </remarks>
		public static ConfigurationSection GetSection(string sectionName, bool checkNullSection = false)
		{
			ConfigurationSection section;

			if (false == ConfigurationSectionCache.Instance.TryGetValue(sectionName, out section))
			{
				ConfigFilesSetting settings = LoadFilesSetting();

				System.Configuration.Configuration globalConfig = null;
				System.Configuration.Configuration config = GetFinalConfiguration(settings, out globalConfig);

				section = config.GetSectionRecursively(sectionName);

				List<string> dependentFiles = new List<string>();

				dependentFiles.Add(settings.MachineConfigurationFile);
				dependentFiles.Add(settings.LocalConfigurationFile);
				dependentFiles.Add(settings.MetaConfigurationFile);
				dependentFiles.Add(settings.GlobalConfigurationFile);

				string configSourceFile = globalConfig.GetSectionRelativeFile(section);

				if (configSourceFile.IsNotEmpty())
					dependentFiles.Add(configSourceFile);

				FileCacheDependency dependency = new FileCacheDependency(true, dependentFiles.ToArray());

				ConfigurationSectionCache.Instance.Add(sectionName, section, dependency);
			}

			if (checkNullSection)
				section.CheckSectionNotNull(sectionName);

			return section;
		}
		#endregion
	} // class end
} // namespace end
