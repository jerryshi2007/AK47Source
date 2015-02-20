using System.Configuration;
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DocServiceClient;

namespace SyncMaterialFileToDocCenterService
{
	public class SyncMaterialFileTaskConfigSetting : DeluxeConfigurationSection
	{
		private const string LockSettingsName = "syncMaterialFileTaskSettings";

		public static SyncMaterialFileTaskConfigSetting GetConfig()
		{
			var result = (SyncMaterialFileTaskConfigSetting)ConfigurationBroker.GetSection(LockSettingsName);

			ConfigurationExceptionHelper.CheckSectionNotNull(result, LockSettingsName);

			return result;
		}

		public ISyncMaterialFileTaskExecutor GetExecutor(string taskType)
		{
			taskType.CheckStringIsNullOrEmpty("taskType");

			TypeMappings.ContainsKey(taskType).FalseThrow("不能在配置信息syncMaterialFileTaskSettings/typeMappings中找到任务类型为{0}的实现类", taskType);

			return TypeMappings[taskType].CreateInstance<ISyncMaterialFileTaskExecutor>();
		}

		[ConfigurationProperty("typeMappings")]
		private TypeConfigurationCollection TypeMappings
		{
			get
			{
				return (TypeConfigurationCollection)this["typeMappings"];
			}
		}

		[ConfigurationProperty("servers")]
		public DocCenterServiceConfigurationElementCollection Servers
		{
			get
			{
				return (DocCenterServiceConfigurationElementCollection)this["servers"];
			}
		}
	}
}