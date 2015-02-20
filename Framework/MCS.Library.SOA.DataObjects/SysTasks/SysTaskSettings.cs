using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 用户系统设置配置信息
	/// </summary>
	public sealed class SysTaskSettings : ConfigurationSection
	{
		public static SysTaskSettings GetSettings()
		{
			SysTaskSettings settings = (SysTaskSettings)ConfigurationBroker.GetSection("sysTaskSettings");

			if (settings == null)
				settings = new SysTaskSettings();

			return settings;
		}

		/// <summary>
		/// 得到任务执行器
		/// </summary>
		/// <param name="taskType"></param>
		/// <returns></returns>
		public ISysTaskExecutor GetExecutor(string taskType)
		{
			taskType.CheckStringIsNullOrEmpty("taskType");

			TypeMappings.ContainsKey(taskType).FalseThrow("不能在配置信息sysTaskSettings/typeMappings中找到任务类型为{0}的实现类", taskType);

			return TypeMappings[taskType].CreateInstance<ISysTaskExecutor>();
		}

		/// <summary>
		/// TaskType和实现类的关系
		/// </summary>
		[ConfigurationProperty("typeMappings")]
		private TypeConfigurationCollection TypeMappings
		{
			get
			{
				return (TypeConfigurationCollection)this["typeMappings"];
			}
		}
	}
}
