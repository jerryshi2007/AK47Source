using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Configuration;
using MCS.Library.Configuration;
using System.Configuration;

namespace MCS.Web.Responsive.Library
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
            return GetSection<WebControlsSection>("resWebControls");
        }

        /// <summary>
        /// ��ȡContentTypesSection
        /// </summary>
        /// <returns>ContentTypesSection</returns>
        /// <remarks>��ȡContentTypesSection</remarks>
        public static ContentTypesSection GetContentTypesSection()
        {
            return GetSection<ContentTypesSection>("resContentTypes");
        }

        /// <summary>
        /// ��ȡJsonSerializationSection
        /// </summary>
        /// <returns>JsonSerializationSection</returns>
        /// <remarks>��ȡJsonSerializationSection</remarks>
        public static ScriptingJsonSerializationSection GetJsonSerializationSection()
        {
            return GetSection<ScriptingJsonSerializationSection>("resJsonSerialization");
        }

        /// <summary>
        /// ��ȡPageExtensionSection
        /// </summary>
        /// <returns></returns>
        public static PageContentSection GetPageExtensionSection()
        {
            return GetSection<PageContentSection>("resPageContent");
        }

        /// <summary>
        /// ��ȡHttpModulesSection
        /// </summary>
        /// <returns></returns>
        public static HttpModulesSection GetHttpModulesSection()
        {
            return GetSection<HttpModulesSection>("resHttpModules");
        }

        /// <summary>
        /// ��ȡPageModulesSection
        /// </summary>
        /// <returns></returns>
        public static PageModulesSection GetPageModulesSection()
        {
            return GetSection<PageModulesSection>("resPageModules");
        }

        private static T GetSection<T>(string sectionName) where T : ConfigurationSection, new()
        {
            T section = (T)ConfigurationBroker.GetSection(sectionName);

            if (section == null)
                section = new T();

            return section;
        }
    }
}
