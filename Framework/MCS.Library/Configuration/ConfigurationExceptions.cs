using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Properties;

namespace MCS.Library.Configuration
{
	/// <summary>
	/// 和Config异常有关的异常辅助方法
	/// </summary>
	public static class ConfigurationExceptionHelper
	{
		/// <summary>
		/// 检查section是否为空，如果为空，会抛出异常
		/// </summary>
		/// <param name="section">section对象</param>
		/// <param name="sectionName">section的名称，会在异常信息中出现</param>
		public static void CheckSectionNotNull(this ConfigurationSection section, string sectionName)
		{
			if (section == null)
				throw new ConfigurationErrorsException(string.Format(Resource.CanNotFoundConfigSection, sectionName));
		}

		/// <summary>
		/// 检查section的source是否为空，如果为空，会抛出异常。这个检查会先执行CheckSectionNotNull
		/// </summary>
		/// <param name="section">section对象</param>
		/// <param name="sectionName">section的名称，会在异常信息中出现</param>
		public static void CheckSectionSource(this ConfigurationSection section, string sectionName)
		{
			CheckSectionNotNull(section, sectionName);

			if (section.ElementInformation.Source == null)
				throw new ConfigurationErrorsException(string.Format(Resource.CanNotFoundConfigSectionElement, sectionName));
		}
	}
}
