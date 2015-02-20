using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Configuration;
using MCS.Library.Configuration;

namespace MCS.Web.Library
{
	/// <summary>
	/// 获取ConfigSection的工厂类
	/// </summary>
	public static class ConfigSectionFactory
	{
		/// <summary>
		/// 获取WebControlsSection
		/// </summary>
		/// <returns>WebControlsSection</returns>
		/// <remarks>获取WebControlsSection</remarks>
		public static WebControlsSection GetWebControlsSection()
		{
			WebControlsSection section = (WebControlsSection)ConfigurationBroker.GetSection("deluxe.web/webcontrols");

			if (section == null)
				section = new WebControlsSection();

			return section;
		}

		/// <summary>
		/// 获取ContentTypesSection
		/// </summary>
		/// <returns>ContentTypesSection</returns>
		/// <remarks>获取ContentTypesSection</remarks>
		public static ContentTypesSection GetContentTypesSection()
		{
			ContentTypesSection section = (ContentTypesSection)ConfigurationBroker.GetSection("deluxe.web/contentTypes");

			if (section == null)
				section = new ContentTypesSection();

			return section;
		}

		/// <summary>
		/// 获取JsonSerializationSection
		/// </summary>
		/// <returns>JsonSerializationSection</returns>
		/// <remarks>获取JsonSerializationSection</remarks>
		[Obsolete("已经废弃，搬移到MCS.Web.Library.Script.Json的JSONSerializerFactory中")]
		public static ScriptingJsonSerializationSection GetJsonSerializationSection()
		{
			//原来的配置节的名称为jsonSerialization，现在更换为scriptJsonSerialization
			ScriptingJsonSerializationSection section = (ScriptingJsonSerializationSection)ConfigurationBroker.GetSection("deluxe.web/scriptJsonSerialization");

			if (section == null)
				section = new ScriptingJsonSerializationSection();

			return section;
		}

		/// <summary>
		/// 获取PageExtensionSection
		/// </summary>
		/// <returns></returns>
		public static PageContentSection GetPageExtensionSection()
		{
			PageContentSection section = (PageContentSection)ConfigurationBroker.GetSection("deluxe.web/pageContent");

			if (section == null)
				section = new PageContentSection();

			return section;
		}

		/// <summary>
		/// 获取HttpModulesSection
		/// </summary>
		/// <returns></returns>
		public static HttpModulesSection GetHttpModulesSection()
		{
			HttpModulesSection section = (HttpModulesSection)ConfigurationBroker.GetSection("deluxe.web/httpModules");

			if (section == null)
				section = new HttpModulesSection();

			return section;
		}

		/// <summary>
		/// 获取PageModulesSection
		/// </summary>
		/// <returns></returns>
		public static PageModulesSection GetPageModulesSection()
		{
			PageModulesSection section = (PageModulesSection)ConfigurationBroker.GetSection("deluxe.web/pageModules");

			if (section == null)
				section = new PageModulesSection();

			return section;
		}
	}
}
