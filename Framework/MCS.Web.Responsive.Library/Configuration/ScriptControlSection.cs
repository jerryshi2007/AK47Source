using System;
using System.Text;
using System.Collections.Generic;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Web.Responsive.Library
{
	/// <summary>
	/// 脚本控件相关的配置节
	/// </summary>
	public sealed class ScriptControlSection : DeluxeConfigurationSection
	{
		/// <summary>
		/// 得到脚本控件相关的配置节
		/// </summary>
		/// <returns></returns>
		public static ScriptControlSection GetSection()
		{
			ScriptControlSection result = (ScriptControlSection)ConfigurationBroker.GetSection("resScriptControlSection");

			if (result == null)
				result = new ScriptControlSection();

			return result;
		}

		private ScriptControlSection()
		{
		}

		/// <summary>
		/// 是否使用内嵌在Assembly中的脚本，如果不是脚本会在虚目录(LocalScriptVirtualDir)中生成
		/// </summary>
		[ConfigurationProperty("useScriptReferencesInAssembly", IsRequired = false, DefaultValue = true)]
		public bool UseScriptReferencesInAssembly
		{
			get
			{
				return (bool)this["useScriptReferencesInAssembly"];
			}
		}

		/// <summary>
		/// 从Assembly中生成脚本的虚目录
		/// </summary>
		[ConfigurationProperty("localScriptVirtualDir", IsRequired = false, DefaultValue = "/assemblyScripts")]
		public string LocalScriptVirtualDir
		{
			get
			{
				return (string)this["localScriptVirtualDir"];
			}
		}
	}
}
