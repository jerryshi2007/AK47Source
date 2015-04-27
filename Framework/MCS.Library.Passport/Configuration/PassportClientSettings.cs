#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	PassportClientSettings.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          ����ǿ      2008-12-2       ���ע��
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
    /// Ticket����֤���Ĵ��ݹ�����ģʽ
    /// </summary>
    public enum TicketTransferMethod
    {
        /// <summary>
        /// Get��ʽ
        /// </summary>
        HttpGet = 0,

        /// <summary>
        /// Post��ʽ
        /// </summary>
        HttpPost = 1,
    }

    /// <summary>
    /// �����¼�ͻ���Ӧ������
    /// </summary>
    public sealed class PassportClientSettings : ConfigurationSection
    {
        private const string C_TICKET_COOKIE_KEY = "HPassportTicket";

        /// <summary>
        /// �����¼�ͻ���Ӧ��������Ϣ
        /// </summary>
        /// <returns>PassportClientSettings����</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\ConfigurationTest.cs" region="ClientConfigTest" lang="cs" title="��ȡ��֤�ͻ���������Ϣ" />
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
        /// Ӧ�õľ��Թ���ʱ�䡣-1,��ʾ��Զ��Ч��-2��ʾ��ǰ�������Session
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
        /// Ӧ�õĻ�������ʱ��
        /// </summary>
        public TimeSpan AppSlidingExpiration
        {
            get
            {
                return TimeSpan.FromSeconds(AppSlidingExpirationSeconds);
            }
        }

        /// <summary>
        /// Ӧ�õ�ID��
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
        /// ���ɵ��û���ʶ����Ҫ������Ĭ����û�еġ�
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
        /// Ticket���浽Cookieʱ��Key
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
        /// �Ƿ񻬶�����
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
        /// ע���ص���ַ
        /// </summary>
        public Uri LogOffCallBackUrl
        {
            get
            {
                return Paths["logOffCallBackUrl"].Uri;
            }
        }

        /// <summary>
        /// ��֤��ַ
        /// </summary>
        public Uri SignInUrl
        {
            get
            {
                return Paths["signInUrl"].Uri;
            }
        }

        /// <summary>
        /// Ʊ�ݴ���֤���Ĵ��ݹ����ķ�ʽ
        /// </summary>
        public TicketTransferMethod Method
        {
            get
            {
                return (TicketTransferMethod)Enum.Parse(typeof(TicketTransferMethod), MethodString, true);
            }
        }

        /// <summary>
        /// ע����ַ
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
    /// Ӧ��Rsa����
    /// </summary>
    public class ClientRsaKeyValueConfigurationElement : ConfigurationElement
    {
        private static string sValue = string.Empty;

        /// <summary>
        /// ���õ�string ֵ
        /// </summary>
        public string Value
        {
            get
            {
                return ClientRsaKeyValueConfigurationElement.sValue;
            }
        }

        /// <summary>
        /// ����������Ϣ
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
