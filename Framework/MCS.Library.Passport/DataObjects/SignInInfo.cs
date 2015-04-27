#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	SignInInfo.cs
// Remark	��		�û���¼��Ϣ
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0				���				20070430		����
// 1.1				yuanyong		20080926		���ע��ͷ
// -------------------------------------------------
#endregion
using System;
using System.Web;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Passport.Properties;

namespace MCS.Library.Passport
{
    /// <summary>
    /// �û���¼��֤��Ϣ
    /// </summary>
    public class SignInInfo : ISignInInfo
    {
        private string userID = string.Empty;
        private string originalUserID = string.Empty;
        private string domain = string.Empty;
        private string signInSessionID = string.Empty;
        private string authenticateServer = string.Empty;
        private bool windowsIntegrated;
        private DateTime signInTime;
        private DateTime signInTimeout = DateTime.MinValue;

        //add by yuanyong 20090416����һ����չ�������ݣ�����Ӧ�ô洢��Ӧ������
        private readonly Dictionary<string, object> properties = new Dictionary<string, object>();

        /// <summary>
        /// ��Cookie�ж�ȡ��֤��Ϣ
        /// </summary>
        /// <returns></returns>
        public static ISignInInfo LoadFromCookie()
        {
            SignInInfo signInInfo = null;

            Common.CheckHttpContext();

            HttpRequest request = HttpContext.Current.Request;

            HttpCookie cookie = request.Cookies[GetLoadingCookieKey()];

            if (cookie != null)
            {
                string strSignIn = cookie.Value;

                try
                {
                    signInInfo = new SignInInfo(Common.DecryptString(strSignIn));
                }
                catch (System.Exception)
                {
                    //���cookie�ĸ�ʽ���󣬲������
                }
            }

            return signInInfo;
        }

        /// <summary>
        /// ����ISignInUserInfo����SignInInfo
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="bDontSaveUserID"></param>
        /// <param name="bAutoSignIn"></param>
        /// <returns></returns>
        public static ISignInInfo Create(ISignInUserInfo userInfo, bool bDontSaveUserID, bool bAutoSignIn)
        {
            userInfo.NullCheck("userInfo");

            XmlDocument signInXml = Common.GenerateSignInInfo(userInfo, bDontSaveUserID, bAutoSignIn);

            return new SignInInfo(signInXml.InnerXml);
        }

        /// <summary>
        /// ����ISignInUserInfo����SignInInfo
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static ISignInInfo Create(ISignInUserInfo userInfo)
        {
            return Create(userInfo, false, false);
        }

        /// <summary>
        /// ����UserID����SignInInfo
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="bDontSaveUserID"></param>
        /// <param name="bAutoSignIn"></param>
        /// <returns></returns>
        public static ISignInInfo Create(string userID, bool bDontSaveUserID, bool bAutoSignIn)
        {
            LogOnIdentity loi = new LogOnIdentity(userID);

            DefaultSignInUserInfo userInfo = new DefaultSignInUserInfo();

            userInfo.UserID = loi.LogOnNameWithoutDomain;
            userInfo.Domain = loi.Domain;

            return Create(userInfo, bDontSaveUserID, bAutoSignIn);
        }

        /// <summary>
        /// ����UserID����SignInInfo
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static ISignInInfo Create(string userID)
        {
            return Create(userID, false, false);
        }

        /// <summary>
        /// ������
        /// </summary>
        public SignInInfo()
        {
        }
        /// <summary>
        /// ������
        /// </summary>
        /// <param name="strXml">SignInInfo��Xml��Ϣ</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\DataObjectsTest.cs" region="SignInInfoTest" lang="cs" title="SignInInfo�����Xml������ת��" />
        /// </remarks>
        public SignInInfo(string strXml)
        {
            InitFromXml(strXml);
        }

        #region ISignInInfo ��Ա
        /// <summary>
        /// �û���
        /// </summary>
        public string UserID
        {
            get
            {
                return this.userID;
            }
            set
            {
                this.userID = value;
            }
        }

        /// <summary>
        /// ����ǰ�ĵ�¼��
        /// </summary>
        public string OriginalUserID
        {
            get
            {
                string result = this.userID;

                if (string.IsNullOrEmpty(this.originalUserID) == false)
                    result = this.originalUserID;

                return result;
            }
            set
            {
                this.originalUserID = value;
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        public string Domain
        {
            get
            {
                return this.domain;
            }
        }

        /// <summary>
        /// ���������û�ID
        /// </summary>
        public string UserIDWithDomain
        {
            get
            {
                string result = UserID;

                if (Domain.IsNotEmpty())
                {
                    result = string.Format("{0}@{1}", UserID, Domain);
                }

                return result;
            }
        }

        /// <summary>
        /// �Ƿ�Windows����
        /// </summary>
        public bool WindowsIntegrated
        {
            get
            {
                return this.windowsIntegrated;
            }
        }

        /// <summary>
        /// ��¼��SessionID
        /// </summary>
        public string SignInSessionID
        {
            get
            {
                return this.signInSessionID;
            }
        }

        /// <summary>
        /// ��¼ʱ��
        /// </summary>
        public DateTime SignInTime
        {
            get
            {
                return this.signInTime;
            }
            set
            {
                this.signInTime = value;
            }
        }

        /// <summary>
        /// ע��ʱ��
        /// </summary>
        public DateTime SignInTimeout
        {
            get
            {
                return this.signInTimeout;
            }
            set
            {
                this.signInTimeout = value;
            }
        }

        /// <summary>
        /// ��֤������
        /// </summary>
        public string AuthenticateServer
        {
            get
            {
                return this.authenticateServer;
            }
        }

        /// <summary>
        /// �Ƿ�����ʱ
        /// </summary>
        public bool ExistsSignInTimeout
        {
            get
            {
                return this.signInTimeout != DateTime.MaxValue && this.signInTimeout != DateTime.MinValue;
            }
        }

        /// <summary>
        /// �⻧����
        /// </summary>
        public string TenantCode
        {
            get
            {
                return this.Properties.GetValue("TenantCode", string.Empty);
            }
            set
            {
                this.Properties["TenantCode"] = value;
            }
        }

        /// <summary>
        /// ������Cookie��
        /// </summary>
        public void SaveToCookie()
        {
            Common.CheckHttpContext();

            HttpResponse response = HttpContext.Current.Response;

            XmlDocument xmlDoc = SaveToXml();
            string strData = xmlDoc.InnerXml;

            HttpCookie cookie = new HttpCookie(this.GetSavingCookieKey());

            cookie.Value = Common.EncryptString(strData);
            cookie.Expires = this.signInTimeout;
            cookie.HttpOnly = true;

            response.Cookies.Add(cookie);
        }

        /// <summary>
        /// �洢��xml�ṹ������
        /// </summary>
        /// <returns>xml�ṹ��SignInInfo</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\DataObjectsTest.cs" region="SignInInfoTest" lang="cs" title="SignInInfo�����Xml������ת��" />
        /// </remarks>
        public System.Xml.XmlDocument SaveToXml()
        {
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml("<SignInInfo/>");

            XmlElement root = xmlDoc.DocumentElement;

            XmlHelper.AppendNode(root, "SSID", this.signInSessionID);
            XmlHelper.AppendNode(root, "UID", this.userID);
            XmlHelper.AppendNode(root, "DO", this.domain);
            XmlHelper.AppendNode(root, "WI", this.windowsIntegrated);
            XmlHelper.AppendNode(root, "AS", this.authenticateServer);
            XmlHelper.AppendNode(root, "STime", Common.DateTimeStandardFormat(this.signInTime));
            XmlHelper.AppendNode(root, "STimeout", Common.DateTimeStandardFormat(this.signInTimeout));
            XmlHelper.AppendNode(root, "OUID", this.OriginalUserID);

            if (this.properties.Count > 0)
            {
                XmlNode nodeProps = XmlHelper.AppendNode(root, Resource.SignInInfoExtraProperties);

                foreach (KeyValuePair<string, object> kp in this.properties)
                {
                    XmlNode nodeProp = XmlHelper.AppendNode(nodeProps, "add");

                    XmlHelper.AppendAttr(nodeProp, "key", kp.Key);
                    XmlHelper.AppendAttr(nodeProp, "value", kp.Value.ToString());
                }
            }

            return xmlDoc;
        }

        /// <summary>
        /// SignInInfo�Ƿ�Ϸ�
        /// </summary>
        /// <returns>true ���� false</returns>
        public bool IsValid()
        {
            bool bValid = false;

            try
            {
                ExceptionHelper.TrueThrow(AreDifferentTenantCode(), Resource.DifferentTenantCode);
                ExceptionHelper.TrueThrow(IsAbsoluteTimeExpired(), Resource.AbsoluteTimeExpired);
                ExceptionHelper.TrueThrow(IsSlidingExpired(), Resource.SlidingTimeExpired);
                ExceptionHelper.TrueThrow(IsDifferentAuthenticateServer(), Resource.DifferentAthenticateServer);

                bValid = true;
            }
            catch (System.ApplicationException ex)
            {
                Trace.WriteLine(string.Format(Resource.TicketInvalidReason, this.UserID, ex.Message));
            }

            return bValid;
        }

        /// <summary>
        /// ��չ����
        /// </summary>
        /// <remarks>
        /// �洢Ӧ������Ҫ��չ����Ӧ������������
        /// </remarks>
        public Dictionary<string, object> Properties // Add By Yuanyong 20090416
        {
            get
            {
                return this.properties;
            }
        }
        #endregion

        /// <summary>
        /// �õ�����ʱ��Cookie��Key
        /// </summary>
        /// <returns></returns>
        public static string GetLoadingCookieKey()
        {
            string result = PassportSignInSettings.GetConfig().SignInCookieKey;

            //��ʱ�������⻧��Cookie
            //if (TenantContext.Current.Enabled)
            //    result += "-" + HttpUtility.UrlEncode(TenantContext.Current.TenantCode);

            return result;
        }

        private bool AreDifferentTenantCode()
        {
            bool result = false;

            string envTenantCode = PassportManager.GetTenantCodeFromContext();

            if (TenantContext.Current.Enabled && envTenantCode.IsNotEmpty())
            {
                result = string.Compare(envTenantCode, this.TenantCode, true) != 0;
            }

            return result;
        }

        private bool IsAbsoluteTimeExpired()
        {
            DateTime newExpireDate = GetConfigExpireDate();

            bool bExpired = (DateTime.Now >= newExpireDate);
#if DELUXEWORKSTEST
            Debug.WriteLineIf(bExpired, "Absolute Time Expired", "SignInPage Check");
#endif
            return (bExpired);		//����ʱ���Ƿ����
        }

        private DateTime GetConfigExpireDate()
        {
            DateTime dt = DateTime.MaxValue;

            PassportSignInSettings settings = PassportSignInSettings.GetConfig();

            if (settings.DefaultTimeout >= TimeSpan.Zero)
                dt = SignInTime.Add(settings.DefaultTimeout);

            return dt;
        }

        private bool IsSlidingExpired()
        {
            bool bExpired = false;

            PassportSignInSettings settings = PassportSignInSettings.GetConfig();

            if (settings.HasSlidingExpiration)
            {
                DateTime dtTO = this.SignInTime.Add(settings.SlidingExpiration);
                bExpired = (DateTime.Now >= dtTO);		//���ʱ�����
            }
#if DELUXEWORKSTEST
            Debug.WriteLineIf(bExpired, "Sliding Expired", "SignInPage Check");
#endif
            return bExpired;
        }

        private bool IsDifferentAuthenticateServer()
        {
            HttpRequest request = HttpContext.Current.Request;

            return string.Compare(this.AuthenticateServer, request.Url.Host + ":" + request.Url.Port, true) != 0;
        }

        private void InitFromXml(string strXml)
        {
            XmlDocument xmlDoc = XmlHelper.CreateDomDocument(strXml);

            XmlElement root = xmlDoc.DocumentElement;

            this.signInSessionID = XmlHelper.GetSingleNodeText(root, "SSID");
            this.userID = XmlHelper.GetSingleNodeText(root, "UID");
            this.domain = XmlHelper.GetSingleNodeText(root, "DO");
            this.windowsIntegrated = XmlHelper.GetSingleNodeValue(root, "WI", false);
            this.authenticateServer = XmlHelper.GetSingleNodeText(root, "AS");

            this.signInTime = XmlHelper.GetSingleNodeValue(root, "STime", DateTime.MinValue);
            this.signInTimeout = XmlHelper.GetSingleNodeValue(root, "STimeout", DateTime.MinValue);
            this.originalUserID = XmlHelper.GetSingleNodeValue(root, "OUID", this.userID);

            // Add By Yuanyong 20090416
            XmlNode node = root.SelectSingleNode(Resource.SignInInfoExtraProperties);
            if (node != null)
            {
                foreach (XmlNode nodeProp in node.ChildNodes)
                {
                    this.properties.Add(XmlHelper.GetAttributeText(nodeProp, "key"), XmlHelper.GetAttributeText(nodeProp, "value"));
                }
            }
        }

        private string GetSavingCookieKey()
        {
            string result = PassportSignInSettings.GetConfig().SignInCookieKey;

            //��ʱ�������⻧��Cookie
            //if (TenantContext.Current.Enabled)
            //    result += "-" + HttpUtility.UrlEncode(this.TenantCode);

            return result;
        }
    }
}
