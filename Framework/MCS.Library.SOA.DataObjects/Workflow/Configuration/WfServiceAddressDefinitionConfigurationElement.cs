using MCS.Library.Configuration;
using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 服务地址的配置信息项
    /// </summary>
    public class WfServiceAddressDefinitionConfigurationElement : UriConfigurationElement
    {
        [ConfigurationProperty("requestMethod", DefaultValue = WfServiceRequestMethod.Get, IsRequired = false)]
        public WfServiceRequestMethod RequestMethod
        {
            get
            {
                return (WfServiceRequestMethod)this["requestMethod"];
            }
        }

        [ConfigurationProperty("contentType", DefaultValue = WfServiceContentType.Form, IsRequired = false)]
        public WfServiceContentType ContentType
        {
            get
            {
                return (WfServiceContentType)this["contentType"];
            }
        }

        [ConfigurationProperty("serviceNS", DefaultValue = "", IsRequired = false)]
        public string ServiceNS
        {
            get
            {
                return (string)this["serviceNS"];
            }
        }

        /// <summary>
        /// 根据IdentityName获取Identity对象
        /// </summary>
        public LogOnIdentity Identity
        {
            get
            {
                LogOnIdentity result = null;

                if (this.IdentityName.IsNotEmpty())
                {
                    IdentityConfigurationElement idElem = IdentityConfigSettings.GetConfig().Identities[this.IdentityName];

                    ExceptionHelper.FalseThrow(idElem != null,
                        "不能在identityConfigSettings配置节中找到WfServiceAddressDefinitionConfigurationElement的配置项{0}中配置的Identity: {1}", this.Name, this.IdentityName);

                    result = idElem.ToLogOnIdentity();
                }

                return result;
            }
        }

        /// <summary>
        /// 登录服务器的Identity配置项的名称
        /// </summary>
        [ConfigurationProperty("identityName", IsRequired = false)]
        private string IdentityName
        {
            get
            {
                return (string)this["identityName"];
            }
        }
    }

    /// <summary>
    /// 服务地址的配置信息项集合
    /// </summary>
    public class WfServiceAddressDefinitionConfigurationElementCollection : NamedConfigurationElementCollection<WfServiceAddressDefinitionConfigurationElement>
    {
    }
}
