#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	DomainMappingSettings.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          ����ǿ      2008-12-2       ���ע��
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
    /// ����ӳ������
    /// </summary>
    public sealed class DomainMappingSettings : ConfigurationSection
    {
        /// <summary>
        /// ��ȡ����ӳ������
        /// </summary>
        /// <returns>����ӳ������</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\ConfigurationTest.cs" region="DomainMappingConfigTest" lang="cs" title="��ȡ��ӳ��������Ϣ" />
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
        /// �Ƿ�������
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
        /// ����ӳ�伯��
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
    /// ����ӳ�伯����
    /// </summary>
    public sealed class DomainMappingConfigurationElementCollection : ConfigurationElementCollection
    {
        private DomainMappingConfigurationElementCollection()
        {
        }
        /// <summary>
        /// ����һ������ӳ�����ʵ��
        /// </summary>
        /// <returns>����ӳ�����ʵ��</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new DomainMappingConfigurationElement();
        }
        /// <summary>
        /// ����ӳ�����ʵ����key��ԭ����
        /// </summary>
        /// <param name="element">����ʵ��</param>
        /// <returns>ԭ����</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DomainMappingConfigurationElement)element).SourceDomain.ToLower();
        }
        /// <summary>
        /// ����ԭ��������ӳ�������
        /// </summary>
        /// <param name="srcDomain">ԭ����</param>
        /// <returns>ӳ������</returns>
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
    /// ����ӳ��Ļ���
    /// </summary>
    public sealed class DomainMappingConfigurationElement : ConfigurationElement
    {
        internal DomainMappingConfigurationElement()
        {
        }
        /// <summary>
        /// ԭ����
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
        /// ӳ�������
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
