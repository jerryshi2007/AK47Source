#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	PassportSignInSettings.cs
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
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;

namespace MCS.Library.Passport
{
    /// <summary>
    /// �����¼��������
    /// </summary>
    public sealed class PassportSignInSettings : ConfigurationSection
    {
        private const string C_SIGNIN_COOKIE_KEY = "HPassportSignIn";

        private const string C_SIGNIN_PAGEDATA_COOKIE_KEY = "SignInPageData";

        /// <summary>
        /// ��ȡ�����¼��������
        /// </summary>
        /// <returns>��֤������������Ϣ</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\ConfigurationTest.cs" region="SignInConfigTest" lang="cs" title="��ȡ��֤�����������Ϣ" />
        /// </remarks>
        public static PassportSignInSettings GetConfig()
        {
            PassportSignInSettings result =
                (PassportSignInSettings)ConfigurationBroker.GetSection("passportSignInSettings");

            ConfigurationExceptionHelper.CheckSectionNotNull(result, "passportSignInSettings");

            return result;
        }

        private PassportSignInSettings()
        {
        }

        /// <summary>
        /// ȱʡ�Ĺ���ʱ��
        /// </summary>
        public TimeSpan DefaultTimeout
        {
            get
            {
                return TimeSpan.FromSeconds((double)DefaultTimeoutInt);
            }
        }

        /// <summary>
        /// ȱʡ�Ļ�������ʱ��
        /// </summary>
        public TimeSpan SlidingExpiration
        {
            get
            {
                return TimeSpan.FromSeconds((double)SlidingExpirationInt);
            }
        }
        /// <summary>
        /// �Ƿ�Windows������֤
        /// </summary>
        [ConfigurationProperty("isWindowsIntegrated", DefaultValue = false)]
        public bool IsWindowsIntegrated
        {
            get
            {
                return (bool)this["isWindowsIntegrated"];
            }
        }

        /// <summary>
        /// Windows��֤�����쳣ʱ���Ƿ��׳���������׳�������ʾȱʡ����֤ҳ��
        /// </summary>
        [ConfigurationProperty("throwWindowsSignInError", DefaultValue = true)]
        public bool ThrowWindowsSignInError
        {
            get
            {
                return (bool)this["throwWindowsSignInError"];
            }
        }

        /// <summary>
        /// �Ƿ�ʹ��ģ���ʱ��
        /// </summary>
        [ConfigurationProperty("useSimulateTime", IsRequired = false, DefaultValue = false)]
        public bool UseSimulateTime
        {
            get
            {
                return (bool)this["useSimulateTime"];
            }
        }

        /// <summary>
        /// ��¼��Ϣʹ�õ�Cookie��Key
        /// </summary>
        [ConfigurationProperty("signInCookieKey", IsRequired = false, DefaultValue = PassportSignInSettings.C_SIGNIN_COOKIE_KEY)]
        public string SignInCookieKey
        {
            get
            {
                return (string)this["signInCookieKey"];
            }
        }

        /// <summary>
        /// ��֤ҳ�����ݵ�Cookie��Key
        /// </summary>
        [ConfigurationProperty("signInPageDataCookieKey", IsRequired = false, DefaultValue = PassportSignInSettings.C_SIGNIN_PAGEDATA_COOKIE_KEY)]
        public string SignInPageDataCookieKey
        {
            get
            {
                return (string)this["signInPageDataCookieKey"];
            }
        }

        /// <summary>
        /// ��֤��Ϣ�ı��������Ƿ���Session��ʽ��
        /// </summary>
        public bool IsSessionBased
        {
            get
            {
                return DefaultTimeout.TotalSeconds == -2;
            }
        }

        /// <summary>
        /// ������֤�Ĳ��
        /// </summary>
        public IAuthenticator Authenticator
        {
            get
            {
                return (IAuthenticator)TypeFactories["authenticator"].CreateInstance();
            }
        }

        /// <summary>
        /// ������֤�Ĳ��2
        /// </summary>
        public IAuthenticator2 Authenticator2
        {
            get
            {
                IAuthenticator2 result = null;

                object authenticator = this.TypeFactories["authenticator"].CreateInstance();

                result = authenticator as IAuthenticator2;

                return result;
            }
        }

        /// <summary>
        /// SignInInfo��Ticket�ĳ־û���
        /// </summary>
        public IPersistSignInInfo PersistSignInInfo
        {
            get
            {
                return (IPersistSignInInfo)TypeFactories["persistSignInInfo"].CreateInstance();
            }
        }

        /// <summary>
        /// OpenIDBinding�ĳ־û���
        /// </summary>
        public IPersistOpenIDBinding PersistOpenIDBinding
        {
            get
            {
                return (IPersistOpenIDBinding)TypeFactories["persistOpenIDBinding"].CreateInstance();
            }
        }

        /// <summary>
        /// �û���Ϣ���޸���
        /// </summary>
        public IUserInfoUpdater UserInfoUpdater
        {
            get
            {
                return (IUserInfoUpdater)TypeFactories["userInfoUpdater"].CreateInstance();
            }
        }

        /// <summary>
        /// �û�ID��ת����
        /// </summary>
        public IUserIDConverter UserIDConverter
        {
            get
            {
                return (IUserIDConverter)TypeFactories["userIDConverter"].CreateInstance();
            }
        }

        /// <summary>
        /// һЩ���������
        /// </summary>
        [ConfigurationProperty("typeFactories", IsRequired = true)]
        private TypeConfigurationCollection TypeFactories
        {
            get
            {
                return (TypeConfigurationCollection)this["typeFactories"];
            }
        }

        /// <summary>
        /// ֪ͨ��������
        /// </summary>
        [ConfigurationProperty("signInNotifiers")]
        public TypeConfigurationCollection SignInNotifiers
        {
            get
            {
                return (TypeConfigurationCollection)this["signInNotifiers"];
            }
        }

        ///// <summary>
        ///// Windows���ɻ���������ʹ�õ��򻷾����ã�Ҫ�����ó�����[value]�Ͷ�����[name]
        ///// </summary>
        //[ConfigurationProperty("domains")]
        //public NameValueConfigurationCollection Domains
        //{
        //    get
        //    {
        //        return (NameValueConfigurationCollection)this["domains"];
        //    }
        //}

        /// <summary>
        /// �Ƿ���ڻ�������ʱ��
        /// </summary>
        public bool HasSlidingExpiration
        {
            get
            {
                return SlidingExpiration != TimeSpan.Zero;
            }
        }

        /// <summary>
        /// ����Ticket��ʹ��
        /// </summary>
        public string RsaKeyValue
        {
            get
            {
                return RsaKeyValueElement.Value;
            }
        }

        #region ˽������
        [ConfigurationProperty("slidingExpiration", DefaultValue = 0)]
        private int SlidingExpirationInt
        {
            get
            {
                return (int)this["slidingExpiration"];
            }
        }

        /// <summary>
        /// -1,��ʾ��Զ��Ч��-2��ʾ��ǰ�������Session
        /// </summary>
        [ConfigurationProperty("defaultTimeout", DefaultValue = -2)]
        private int DefaultTimeoutInt
        {
            get
            {
                return (int)this["defaultTimeout"];
            }
        }

        [ConfigurationProperty("rsaKeyValue", IsRequired = true)]
        private SignInRsaKeyValueConfigurationElement RsaKeyValueElement
        {
            get
            {
                return (SignInRsaKeyValueConfigurationElement)this["rsaKeyValue"];
            }
        }
        #endregion ˽������
    }

    /// <summary>
    /// Rsa����������Ϣ
    /// </summary>
    public class SignInRsaKeyValueConfigurationElement : ConfigurationElement
    {
        private static string sValue = string.Empty;

        /// <summary>
        /// Rsa���õ�stringֵ
        /// </summary>
        public string Value
        {
            get
            {
                return SignInRsaKeyValueConfigurationElement.sValue;
            }
        }
        /// <summary>
        /// ����Rsa������Ϣ
        /// </summary>
        /// <param name="reader">XmlReader</param>
        /// <param name="serializeCollectionKey"></param>
        protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            lock (typeof(SignInRsaKeyValueConfigurationElement))
            {
                if (SignInRsaKeyValueConfigurationElement.sValue == string.Empty)
                    SignInRsaKeyValueConfigurationElement.sValue = reader.ReadOuterXml();
                else
                    reader.ReadOuterXml();
            }
        }
    }
}
