using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using MCS.Library.Configuration;
using MCS.Library.Core;

namespace MCS.Web.Responsive.Library
{
    /// <summary>
    /// PageModules配置信息
    /// </summary>
	public class PageModulesSection : ConfigurationSection
    {
        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public PageModuleElementCollection PageModules
        {
            get
            {
                return (PageModuleElementCollection)this[string.Empty];
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
        internal Dictionary<string, IPageModule> Create()
        {
            PageModuleElementCollection configModules = PageModules;
            Dictionary<string, IPageModule> modules = new Dictionary<string, IPageModule>(configModules.Count);

            foreach (PageModuleElement element in configModules)
            {
                IPageModule module = (IPageModule)TypeCreator.CreateInstance(element.Type);
                modules.Add(element.Name, module);
            }

            return modules;
        }
    }

    /// <summary>
    /// PageModule配置节集合
    /// </summary>
    public class PageModuleElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// CreateNewElement
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new PageModuleElement();
        }

        /// <summary>
        /// GetElementKey
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PageModuleElement)element).Name;
        }
    }

    /// <summary>
    /// PageModule配置节
    /// </summary>
    public class PageModuleElement : ConfigurationElement
    {
        /// <summary>
        /// 名称
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = "")]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
        }

        /// <summary>
        /// 类型
        /// </summary>
        [ConfigurationProperty("type", DefaultValue = "")]
        public string Type
        {
            get
            {
                return (string)this["type"];
            }
        }
    }
}
