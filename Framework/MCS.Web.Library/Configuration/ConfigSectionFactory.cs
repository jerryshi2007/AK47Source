using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Configuration;
using MCS.Library.Configuration;

namespace MCS.Web.Library
{
	/// <summary>
	/// ��ȡConfigSection�Ĺ�����
	/// </summary>
	public static class ConfigSectionFactory
	{
		/// <summary>
		/// ��ȡWebControlsSection
		/// </summary>
		/// <returns>WebControlsSection</returns>
		/// <remarks>��ȡWebControlsSection</remarks>
		public static WebControlsSection GetWebControlsSection()
		{
			WebControlsSection section = (WebControlsSection)ConfigurationBroker.GetSection("deluxe.web/webcontrols");

			if (section == null)
				section = new WebControlsSection();

			return section;
		}

		/// <summary>
		/// ��ȡContentTypesSection
		/// </summary>
		/// <returns>ContentTypesSection</returns>
		/// <remarks>��ȡContentTypesSection</remarks>
		public static ContentTypesSection GetContentTypesSection()
		{
			ContentTypesSection section = (ContentTypesSection)ConfigurationBroker.GetSection("deluxe.web/contentTypes");

			if (section == null)
				section = new ContentTypesSection();

			return section;
		}

		/// <summary>
		/// ��ȡJsonSerializationSection
		/// </summary>
		/// <returns>JsonSerializationSection</returns>
		/// <remarks>��ȡJsonSerializationSection</remarks>
		[Obsolete("�Ѿ����������Ƶ�MCS.Web.Library.Script.Json��JSONSerializerFactory��")]
		public static ScriptingJsonSerializationSection GetJsonSerializationSection()
		{
			//ԭ�������ýڵ�����ΪjsonSerialization�����ڸ���ΪscriptJsonSerialization
			ScriptingJsonSerializationSection section = (ScriptingJsonSerializationSection)ConfigurationBroker.GetSection("deluxe.web/scriptJsonSerialization");

			if (section == null)
				section = new ScriptingJsonSerializationSection();

			return section;
		}

		/// <summary>
		/// ��ȡPageExtensionSection
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
		/// ��ȡHttpModulesSection
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
		/// ��ȡPageModulesSection
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
