using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 文件流程描述的配置节
	/// </summary>
	public sealed class FileProcessDescriptorSettings : ConfigurationSection
	{
		public static FileProcessDescriptorSettings GetConfig()
		{
			FileProcessDescriptorSettings settings =
				(FileProcessDescriptorSettings)ConfigurationBroker.GetSection("fileProcessDescriptorSettings");

			ConfigurationExceptionHelper.CheckSectionNotNull(settings, "fileProcessDescriptorSettings");

			return settings;
		}

		/// <summary>
		/// 流程定义文件的根目录
		/// </summary>
		[ConfigurationProperty("rootPath", IsRequired = true)]
		public string RootPath
		{
			get
			{
				return (string)base["rootPath"];
			}
		}
	}
}
