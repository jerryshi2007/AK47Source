using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects
{
    public sealed class CFCAControlConfigSetting : ConfigurationSection
    {
        private const string LockSettingsName = "cfcaControlConfigSetting";

        public static CFCAControlConfigSetting GetConfig()
        {
            CFCAControlConfigSetting result =
                   (CFCAControlConfigSetting)ConfigurationBroker.GetSection(LockSettingsName);

            ConfigurationExceptionHelper.CheckSectionNotNull(result, LockSettingsName);

            return result;
        }
        /// <summary>
        /// CAB路径
        /// </summary>
        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get
            {
                return this["path"].ToString();
            }
        }
        [ConfigurationProperty("clsid", IsRequired = true)]
        public string CLSID
        {
            get
            {
                return this["clsid"].ToString();
            }
        }
        /// <summary>
        /// 是否验签
        /// </summary>
        [ConfigurationProperty("isverifysignature", IsRequired = true)]
        public bool IsVerifySignature
        {
            get
            {
                return Convert.ToBoolean(this["isverifysignature"].ToString());
            }
        }

        /// <summary>
        /// 主题DN
        /// </summary>
        [ConfigurationProperty("subjectdn", IsRequired = true)]
        public string SubjectDN
        {
            get
            {
                return this["subjectdn"].ToString();
            }
        }

        /// <summary>
        /// 颁发者DN
        /// </summary>
        [ConfigurationProperty("issuerdn", IsRequired = false)]
        public string IssuerDN
        {
            get
            {
                return this["issuerdn"].ToString();
            }
        }
        /// <summary>
        /// 链
        /// </summary>
        [ConfigurationProperty("cachain", IsRequired = true)]
        public string CaChain
        {
            get
            {
                return this["cachain"].ToString();
            }
        }
        /// <summary>
        /// 黑名单
        /// </summary>
        [ConfigurationProperty("crlfilename", IsRequired = true)]
        public string CrlFilename
        {
            get
            {
                return this["crlfilename"].ToString();
            }
        }        
    }
}
