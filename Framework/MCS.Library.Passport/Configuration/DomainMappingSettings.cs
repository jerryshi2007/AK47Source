#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	DomainMappingSettings.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          胡自强      2008-12-2       添加注释
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Library.Passport
{
    /// <summary>
    /// 域名映射配置
    /// </summary>
    public sealed class DomainMappingSettings : ConfigurationSection
    {
        /// <summary>
        /// 获取域名映射配置
        /// </summary>
        /// <returns>域名映射配置</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\ConfigurationTest.cs" region="DomainMappingConfigTest" lang="cs" title="获取域映射配置信息" />
        /// </remarks>
        public static DomainMappingSettings GetConfig()
        {
            DomainMappingSettings settings =
                (DomainMappingSettings)ConfigurationBroker.GetSection("domainMappingSettings");

            if (settings == null)
                settings = new DomainMappingSettings();

            return settings;
        }

        private DomainMappingSettings()
        {
        }
        /// <summary>
        /// 是否检查域名
        /// </summary>
        [ConfigurationProperty("checkDomainName", DefaultValue = true)]
        public bool CheckDomainName
        {
            get
            {
                return (bool)this["checkDomainName"];
            }
        }
        /// <summary>
        /// 域名映射集合
        /// </summary>
        [ConfigurationProperty("mappings")]
        public DomainMappingConfigurationElementCollection Mappings
        {
            get
            {
                return (DomainMappingConfigurationElementCollection)this["mappings"];
            }
        }
    }
    /// <summary>
    /// 域名映射集合类
    /// </summary>
    public sealed class DomainMappingConfigurationElementCollection : ConfigurationElementCollection
    {
        private DomainMappingConfigurationElementCollection()
        {
        }
        /// <summary>
        /// 创建一个域名映射基类实例
        /// </summary>
        /// <returns>域名映射基类实例</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new DomainMappingConfigurationElement();
        }
        /// <summary>
        /// 返回映射基类实例的key，原域名
        /// </summary>
        /// <param name="element">基类实例</param>
        /// <returns>原域名</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DomainMappingConfigurationElement)element).SourceDomain.ToLower();
        }
        /// <summary>
        /// 根据原域名返回映射的域名
        /// </summary>
        /// <param name="srcDomain">原域名</param>
        /// <returns>映射域名</returns>
        public new string this[string srcDomain]
        {
            get
            {
                ExceptionHelper.CheckStringIsNullOrEmpty(srcDomain, "srcDomain");

                DomainMappingConfigurationElement element =
                    (DomainMappingConfigurationElement)this.BaseGet(srcDomain.ToLower());

                string destDomain = srcDomain;

                if (element != null)
                    destDomain = element.DestinationDomain;

                return destDomain;
            }
        }
    }
    /// <summary>
    /// 域名映射的基类
    /// </summary>
    public sealed class DomainMappingConfigurationElement : ConfigurationElement
    {
        internal DomainMappingConfigurationElement()
        {
        }
        /// <summary>
        /// 原域名
        /// </summary>
        [ConfigurationProperty("sourceDomain", IsRequired = true, IsKey = true)]
        public string SourceDomain
        {
            get
            {
                return (string)this["sourceDomain"];
            }
        }
        /// <summary>
        /// 映射后域名
        /// </summary>
        [ConfigurationProperty("destinationDomain", IsRequired = true)]
        public string DestinationDomain
        {
            get
            {
                return (string)this["destinationDomain"];
            }
        }
    }
}
