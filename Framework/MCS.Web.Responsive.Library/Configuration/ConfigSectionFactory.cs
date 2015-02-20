using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Configuration;
using MCS.Library.Configuration;
using System.Configuration;

namespace MCS.Web.Responsive.Library
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
            return GetSection<WebControlsSection>("resWebControls");
        }

        /// <summary>
        /// 获取ContentTypesSection
        /// </summary>
        /// <returns>ContentTypesSection</returns>
        /// <remarks>获取ContentTypesSection</remarks>
        public static ContentTypesSection GetContentTypesSection()
        {
            return GetSection<ContentTypesSection>("resContentTypes");
        }

        /// <summary>
        /// 获取JsonSerializationSection
        /// </summary>
        /// <returns>JsonSerializationSection</returns>
        /// <remarks>获取JsonSerializationSection</remarks>
        public static ScriptingJsonSerializationSection GetJsonSerializationSection()
        {
            return GetSection<ScriptingJsonSerializationSection>("resJsonSerialization");
        }

        /// <summary>
        /// 获取PageExtensionSection
        /// </summary>
        /// <returns></returns>
        public static PageContentSection GetPageExtensionSection()
        {
            return GetSection<PageContentSection>("resPageContent");
        }

        /// <summary>
        /// 获取HttpModulesSection
        /// </summary>
        /// <returns></returns>
        public static HttpModulesSection GetHttpModulesSection()
        {
            return GetSection<HttpModulesSection>("resHttpModules");
        }

        /// <summary>
        /// 获取PageModulesSection
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
