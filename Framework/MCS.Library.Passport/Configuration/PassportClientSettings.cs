#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	PassportClientSettings.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          胡自强      2008-12-2       添加注释
// -------------------------------------------------
#endregion
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.Passport.Properties;
using MCS.Library.Principal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;

namespace MCS.Library.Passport
{
    /// <summary>
    /// Ticket从认证中心传递过来的模式
    /// </summary>
    public enum TicketTransferMethod
    {
        /// <summary>
        /// Get方式
        /// </summary>
        HttpGet = 0,

        /// <summary>
        /// Post方式
        /// </summary>
        HttpPost = 1,
    }

    /// <summary>
    /// 单点登录客户端应用配置
    /// </summary>
    public sealed class PassportClientSettings : ConfigurationSection
    {
        private const string C_TICKET_COOKIE_KEY = "HPassportTicket";

        /// <summary>
        /// 单点登录客户端应用配置信息
        /// </summary>
        /// <returns>PassportClientSettings对象</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\ConfigurationTest.cs" region="ClientConfigTest" lang="cs" title="获取认证客户端配置信息" />
        /// </remarks>
        public static PassportClientSettings GetConfig()
        {
            PassportClientSettings result =
                (PassportClientSettings)ConfigurationBroker.GetSection("passportClientSettings");

            ConfigurationExceptionHelper.CheckSectionNotNull(result, "passportClientSettings");

            return result;
        }

        private PassportClientSettings()
        {
        }

        /// <summary>
        /// 应用的绝对过期时间。-1,表示永远有效；-2表示当前浏览器的Session
        /// </summary>
        [ConfigurationProperty("appSignInTimeout", DefaultValue = -2)]
        public int AppSignInTimeout
        {
            get
            {
                return (int)this["appSignInTimeout"];
            }
        }

        [ConfigurationProperty("appSlidingExpiration", DefaultValue = 0)]
        private int AppSlidingExpirationSeconds
        {
            get
            {
                return (int)this["appSlidingExpiration"];
            }
        }


        /// <summary>
        /// 应用的滑动过期时间
        /// </summary>
        public TimeSpan AppSlidingExpiration
        {
            get
            {
                return TimeSpan.FromSeconds(AppSlidingExpirationSeconds);
            }
        }

        /// <summary>
        /// 应用的ID号
        /// </summary>
        [ConfigurationProperty("appID", IsRequired = true)]
        public string AppID
        {
            get
            {
                return (string)this["appID"];
            }
        }

        /// <summary>
        /// 生成的用户标识不需要域名，默认是没有的。
        /// </summary>
        [ConfigurationProperty("identityWithoutDomainName", IsRequired = false, DefaultValue = true)]
        public bool IdentityWithoutDomainName
        {
            get
            {
                return (bool)this["identityWithoutDomainName"];
            }
        }

        /// <summary>
        /// Ticket保存到Cookie时的Key
        /// </summary>
        [ConfigurationProperty("ticketCookieKey", IsRequired = false, DefaultValue = PassportClientSettings.C_TICKET_COOKIE_KEY)]
        public string TicketCookieKey
        {
            get
            {
                return (string)this["ticketCookieKey"];
            }
        }

        /// <summary>
        /// 是否滑动过期
        /// </summary>
        public bool HasSlidingExpiration
        {
            get
            {
                return AppSlidingExpiration != TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Rsa key
        /// </summary>
        public string RsaKeyValue
        {
            get
            {
                return RsaKeyValueElement.Value;
            }
        }

        /// <summary>
        /// 注销回调地址
        /// </summary>
        public Uri LogOffCallBackUrl
        {
            get
            {
                return Paths["logOffCallBackUrl"].Uri;
            }
        }

        /// <summary>
        /// 认证地址
        /// </summary>
        public Uri SignInUrl
        {
            get
            {
                return Paths["signInUrl"].Uri;
            }
        }

        /// <summary>
        /// 票据从认证中心传递过来的方式
        /// </summary>
        public TicketTransferMethod Method
        {
            get
            {
                return (TicketTransferMethod)Enum.Parse(typeof(TicketTransferMethod), MethodString, true);
            }
        }

        /// <summary>
        /// 注销地址
        /// </summary>
        public Uri LogOffUrl
        {
            get
            {
                return Paths["logOffUrl"].Uri;
            }
        }

        [ConfigurationProperty("method", IsRequired = false, DefaultValue = "HttpGet")]
        private string MethodString
        {
            get
            {
                return (string)this["method"];
            }
        }

        [ConfigurationProperty("typeFactories", IsRequired = false)]
        private TypeConfigurationCollection TypeFactories
        {
            get
            {
                return (TypeConfigurationCollection)this["typeFactories"];
            }
        }

        [ConfigurationProperty("rsaKeyValue", IsRequired = true)]
        private ClientRsaKeyValueConfigurationElement RsaKeyValueElement
        {
            get
            {
                return (ClientRsaKeyValueConfigurationElement)this["rsaKeyValue"];
            }
        }

        [ConfigurationProperty("paths", IsRequired = true)]
        private UriConfigurationCollection Paths
        {
            get
            {
                return (UriConfigurationCollection)this["paths"];
            }
        }
    }

    /// <summary>
    /// 应用Rsa配置
    /// </summary>
    public class ClientRsaKeyValueConfigurationElement : ConfigurationElement
    {
        private static string sValue = string.Empty;

        /// <summary>
        /// 配置的string 值
        /// </summary>
        public string Value
        {
            get
            {
                return ClientRsaKeyValueConfigurationElement.sValue;
            }
        }

        /// <summary>
        /// 读入配置信息
        /// </summary>
        /// <param name="reader">XmlReader</param>
        /// <param name="serializeCollectionKey"></param>
        protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            lock (typeof(ClientRsaKeyValueConfigurationElement))
            {
                if (ClientRsaKeyValueConfigurationElement.sValue == string.Empty)
                    ClientRsaKeyValueConfigurationElement.sValue = reader.ReadOuterXml();
                else
                    reader.ReadOuterXml();
            }
        }
    }
}
