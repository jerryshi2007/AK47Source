#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	ConfigurationBroker.cs
// Remark	��	Broker class to manage all local config file as well as mapping config file(s) Remote config mapping could via 
//              ConfigurationFileMap and retrive by ConfigurationManager.OpenMappedMachineConfiguration
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070430		����
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
	/// Broker��������б��������ļ���ӳ�������ļ���
	/// Զ�������ļ�ӳ����ConfigurationFileMap��ConfigurationManager.OpenMappedMachineConfiguration����.
	/// 
	/// Լ��:
	///     <list type="bullet">
	///         <item>
	///         ӳ���ļ�������ConfigurationSectionGroup��ConfigurationSection��ʼ.
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
		// ����ʧЧʱ��
		private static readonly TimeSpan SlidingTime = TimeSpan.FromSeconds(15.0);

		#endregion private const and field

		/// <summary>
		/// ���캯��
		/// </summary>
		static ConfigurationBroker()
		{
		}

		/// <summary>
		/// meta�����ļ�λ��ö��
		/// </summary>
		private enum MetaConfigurationPosition
		{
			LocalFile,
			MetaFile
		}

		/// <summary>
		/// �ڲ��࣬���ڴ�š�����machine��local��meta��global�����ļ��ĵ�ַ��
		/// meta�ļ�λ�ã�ö�٣� 
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
		/// ����configuration����Ļ���keyֵ
		/// </summary>
		/// <param name="fileNames">�ļ��б�</param>
		/// <returns>cache key</returns>
		private static string CreateConfigurationCacheKey(params string[] fileNames)
		{
			StringBuilder key = new StringBuilder(256);

			for (int i = 0; i < fileNames.Length; i++)
			{
				// ֻȡ�ļ�����ȥ������·��
				key.Append(Path.GetFileName(fileNames[i]).ToLower());
			}

			return key.ToString();
		}

		/// <summary>
		/// ����machine��local�����ļ���meta�����ļ���meta�е����ýڣ����仺�沢��������ʧЧ������
		/// ���Ҳ��� ConfigFilesSetting ��ʵ���м�¼machine��local��meta��global�����ļ��ĵ�ַ��
		/// meta�����ļ�λ�ã�ö�٣�
		/// </summary>
		/// <returns>ConfigFilesSetting ��ʵ��</returns>
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
		/// ��ȡmeta�����е� sourceMappings �ڵ�
		/// </summary>
		/// <param name="fileSettings">ConfigFilesSetting ��ʵ��</param>
		/// <returns>meta�����е� sourceMappings �ڵ�</returns>
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
		/// �ӱ���config�ļ��ж�ȡmeta����
		/// </summary>
		/// <param name="fileSettings">ConfigFilesSetting ��ʵ��</param>
		/// <returns>MetaConfigurationSourceInstanceSection ʵ��</returns>
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
		/// �ӵ�����meta.config�ļ��ж�ȡmeta����
		/// </summary>
		/// <param name="fileSettings">ConfigFilesSetting ʵ��</param>
		/// <returns>MetaConfigurationSourceInstanceSection ʵ��</returns>
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
		/// ��ȡmeta�ļ��ĵ�ַ��λ��
		/// </summary>
		/// <param name="fileSettings">ConfigFilesSetting ��ʵ��</param>
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
		/// ��ȡ���� local �� global �ϲ���� Configuration
		/// </summary>
		/// <param name="fileSettings">ConfigFilesSetting ��ʵ��</param>
		/// <param name="globalConfig">����ȫ�����õ�Config</param>
		/// <returns>local �� global �ϲ���� Configuration</returns>
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
		/// ��ȡ����config��AppSettings�ڵ�
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
		/// ȡ�õ���config�ļ��е� Configuration
		/// </summary>
		/// <param name="fileName">�ļ���ַ</param>
		/// <param name="fileDependencies">���������ļ�</param>
		/// <param name="ignoreFileNotExist">�Ƿ���Բ����ڵ��ļ�</param>
		/// <returns>Configuration����</returns>
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
		/// ��ȡ��׼WebӦ�ó����������Ϣ���ϲ�Web.config��global�����ļ�
		/// </summary>
		/// <param name="machineConfigPath">global�����ļ���ַ</param>
		/// <param name="ignoreFileNotExist">�Ƿ���Բ����ڵ��ļ�</param>
		/// <param name="fileDependencies">���������ļ�</param>
		/// <returns>Web.config��global�����ļ��ϲ����Configuration����</returns>
		private static System.Configuration.Configuration GetStandardWebConfiguration(string machineConfigPath, bool ignoreFileNotExist, params string[] fileDependencies)
		{
			System.Configuration.Configuration globalConfig = null;

			return GetStandardWebAndGlobalConfiguration(machineConfigPath, ignoreFileNotExist, out globalConfig, fileDependencies);
		}

		/// <summary>
		/// ��ȡ��׼WebӦ�ó����������Ϣ���ϲ�Web.config��global�����ļ�
		/// </summary>
		/// <param name="machineConfigPath">global�����ļ���ַ</param>
		/// <param name="ignoreFileNotExist">�Ƿ���Բ����ڵ��ļ�</param>
		/// <param name="globalConfig">���ص�ȫ������</param>
		/// <param name="fileDependencies">���������ļ�</param>
		/// <returns>Web.config��global�����ļ��ϲ����Configuration����</returns>
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
		/// �õ�ȫ�������ļ���config����
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
		/// ��ȡ��׼WinFormӦ�ó����������Ϣ���ϲ�App.config��global�����ļ�
		/// </summary>
		/// <param name="machineConfigPath">global�����ļ���ַ</param>
		/// <param name="localConfigPath">����Ӧ�ó��������ļ���ַ</param>
		/// <param name="ignoreFileNotExist">�Ƿ���Բ����ڵ��ļ�</param>
		/// <param name="fileDependencies">���������ļ�</param>
		/// <returns>App.config��global�����ļ��ϲ����Configuration����</returns>
		private static System.Configuration.Configuration GetStandardExeConfiguration(string machineConfigPath, string localConfigPath, bool ignoreFileNotExist, params string[] fileDependencies)
		{
			System.Configuration.Configuration globalConfig = null;

			return GetStandardExeAndGlobalConfiguration(machineConfigPath, localConfigPath, ignoreFileNotExist, out globalConfig, fileDependencies);
		}

		/// <summary>
		/// ��ȡ��׼WinFormӦ�ó����������Ϣ���ϲ�App.config��global�����ļ�
		/// </summary>
		/// <param name="machineConfigPath">global�����ļ���ַ</param>
		/// <param name="localConfigPath">����Ӧ�ó��������ļ���ַ</param>
		/// <param name="globalConfig">�õ�ȫ�ֵ�����</param>
		/// <param name="ignoreFileNotExist">�Ƿ���Բ����ڵ��ļ�</param>
		/// <param name="fileDependencies">���������ļ�</param>
		/// <returns>App.config��global�����ļ��ϲ����Configuration����</returns>
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
		/// ��Configuration������뻺�棬����ʱ����ļ��Ļ������
		/// </summary>
		/// <param name="cacheKey">cache key</param>
		/// <param name="config">�������Configuration����</param>
		/// <param name="ignoreFileNotExist">�Ƿ���Բ����ڵ��ļ�</param>
		/// <param name="files">�����������ļ�</param>
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
		/// ���ڵ����ƴ�������Ϣ��ȡ�ýڵ㣬�����ڵ���Ϣ���棬�����ļ�����
		/// </summary>
		/// <param name="sectionName">�ڵ�����</param>
		/// <param name="checkNullSection">�������null���Ƿ��׳��쳣</param>
		/// <returns>���ýڵ�</returns>
		/// <remarks>
		/// �����ƻ�ȡ���ýڵ���Ϣ������ConfigurationSection��������ʵ�壬ʵ���������û��Զ��塣
		/// <code source="..\Framework\TestProjects\Deluxeworks.Library.WebTest\Configuration\Default.aspx.cs" region="Using Broker" lang="cs" title="ʹ�����ù���" />
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
