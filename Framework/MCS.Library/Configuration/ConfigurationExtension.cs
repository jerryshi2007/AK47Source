using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Configuration
{
	/// <summary>
	/// Configuration有关的扩展方法
	/// </summary>
	static class ConfigurationExtension
	{
		/// <summary>
		/// 得到Section的相关文件
		/// </summary>
		/// <param name="config"></param>
		/// <param name="section"></param>
		/// <returns></returns>
		public static string GetSectionRelativeFile(this System.Configuration.Configuration config, ConfigurationSection section)
		{
			string result = string.Empty;

			if (config != null && section != null)
			{
				string configSource = section.SectionInformation.ConfigSource;

				if (configSource.IsNullOrEmpty())
				{
					ConfigurationSection sectionInConfig = GetSectionRecursively(config, section.SectionInformation.SectionName);

					if (sectionInConfig != null)
						configSource = sectionInConfig.SectionInformation.ConfigSource;
				}

				if (configSource.IsNotEmpty() && config.FilePath.IsNotEmpty())
				{
					string configDir = Path.GetDirectoryName(config.FilePath);

					result = Path.Combine(configDir, configSource);
				}
			}

			return result;
		}

		/// <summary>
		/// 从Configuration中读取某个Section，递归查找组
		/// </summary>
		/// <param name="config"></param>
		/// <param name="sectionName"></param>
		/// <returns></returns>
		public static ConfigurationSection GetSectionRecursively(this System.Configuration.Configuration config, string sectionName)
		{
			ConfigurationSection result = null;

			if (config != null)
			{
				result = config.GetSection(sectionName);

				//在Configuration对象中不能直接拿到Section对象时，遍历所有Group查找Section
				if (result == null || result is DefaultSection)
					result = GetSectionFromGroups(sectionName, config.SectionGroups);
			}

			return result;
		}

		/// <summary>
		/// 从SectionGroup中读取Section。在Section写在Group里时使用
		/// </summary>
		/// <param name="sectionName">section name</param>
		/// <param name="groups">SectionGroup</param>
		/// <returns>ConfigurationSection</returns>
		private static ConfigurationSection GetSectionFromGroups(string sectionName, ConfigurationSectionGroupCollection groups)
		{
			ConfigurationSection section = null;

			for (int i = 0; i < groups.Count; i++)
			{
				try
				{
					ConfigurationSectionGroup group = groups[i];

					if (group.SectionGroups.Count > 0)
						section = GetSectionFromGroups(sectionName, group.SectionGroups);
					else
						section = group.Sections[sectionName];

					if (section != null)
						break;
				}
				catch (System.IO.FileNotFoundException)
				{
				}
			}

			return section;
		}
	}
}
