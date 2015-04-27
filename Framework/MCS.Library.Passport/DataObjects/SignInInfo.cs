#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	SignInInfo.cs
// Remark	：		用户登录信息
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0				沈峥				20070430		创建
// 1.1				yuanyong		20080926		添加注释头
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
    /// 用户登录认证信息
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

        //add by yuanyong 20090416增加一个扩展属性内容，用于应用存储相应的数据
        private readonly Dictionary<string, object> properties = new Dictionary<string, object>();

        /// <summary>
        /// 从Cookie中读取认证信息
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
                    //如果cookie的格式错误，不予理睬
                }
            }

            return signInInfo;
        }

        /// <summary>
        /// 根据ISignInUserInfo创建SignInInfo
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
        /// 根据ISignInUserInfo创建SignInInfo
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static ISignInInfo Create(ISignInUserInfo userInfo)
        {
            return Create(userInfo, false, false);
        }

        /// <summary>
        /// 根据UserID创建SignInInfo
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
        /// 根据UserID创建SignInInfo
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static ISignInInfo Create(string userID)
        {
            return Create(userID, false, false);
        }

        /// <summary>
        /// 构造类
        /// </summary>
        public SignInInfo()
        {
        }
        /// <summary>
        /// 构造类
        /// </summary>
        /// <param name="strXml">SignInInfo的Xml信息</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\DataObjectsTest.cs" region="SignInInfoTest" lang="cs" title="SignInInfo对象和Xml对象间的转换" />
        /// </remarks>
        public SignInInfo(string strXml)
        {
            InitFromXml(strXml);
        }

        #region ISignInInfo 成员
        /// <summary>
        /// 用户名
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
        /// 扮演前的登录名
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
        /// 域名
        /// </summary>
        public string Domain
        {
            get
            {
                return this.domain;
            }
        }

        /// <summary>
        /// 带域名的用户ID
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
        /// 是否Windows集成
        /// </summary>
        public bool WindowsIntegrated
        {
            get
            {
                return this.windowsIntegrated;
            }
        }

        /// <summary>
        /// 登录的SessionID
        /// </summary>
        public string SignInSessionID
        {
            get
            {
                return this.signInSessionID;
            }
        }

        /// <summary>
        /// 登录时间
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
        /// 注销时间
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
        /// 认证服务器
        /// </summary>
        public string AuthenticateServer
        {
            get
            {
                return this.authenticateServer;
            }
        }

        /// <summary>
        /// 是否登入后超时
        /// </summary>
        public bool ExistsSignInTimeout
        {
            get
            {
                return this.signInTimeout != DateTime.MaxValue && this.signInTimeout != DateTime.MinValue;
            }
        }

        /// <summary>
        /// 租户编码
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
        /// 保存入Cookie中
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
        /// 存储到xml结构数据中
        /// </summary>
        /// <returns>xml结构的SignInInfo</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\DataObjectsTest.cs" region="SignInInfoTest" lang="cs" title="SignInInfo对象和Xml对象间的转换" />
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
        /// SignInInfo是否合法
        /// </summary>
        /// <returns>true 或者 false</returns>
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
        /// 扩展属性
        /// </summary>
        /// <remarks>
        /// 存储应用中需要扩展的相应属性数据内容
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
        /// 得到加载时的Cookie的Key
        /// </summary>
        /// <returns></returns>
        public static string GetLoadingCookieKey()
        {
            string result = PassportSignInSettings.GetConfig().SignInCookieKey;

            //暂时不按照租户分Cookie
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
            return (bExpired);		//绝对时间是否过期
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
                bExpired = (DateTime.Now >= dtTO);		//相对时间过期
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

            //暂时不按照租户分Cookie
            //if (TenantContext.Current.Enabled)
            //    result += "-" + HttpUtility.UrlEncode(this.TenantCode);

            return result;
        }
    }
}
