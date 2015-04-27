#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	Ticket.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          胡自强      2008-12-2       添加注释
// -------------------------------------------------
#endregion
using System;
using System.Xml;
using System.Web;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Cryptography;

using MCS.Library.Core;
using MCS.Library.Passport.Properties;

namespace MCS.Library.Passport
{
    /// <summary>
    /// 票据信息类
    /// </summary>
    public class Ticket : ITicket
    {
        private ISignInInfo signInInfo = null;
        private string appID = string.Empty;
        private string appSignInSessionID = string.Empty;
        private DateTime appSignInTime;
        private DateTime appSignInTimeout = DateTime.MinValue;
        private string appSignInIP = string.Empty;

        /// <summary>
        /// 从Cookie中读取ITicket信息
        /// </summary>
        /// <returns><see cref="ITicket"/> 对象。</returns>
        public static ITicket LoadFromCookie()
        {
            ITicket ticket = null;

            Common.CheckHttpContext();

            HttpRequest request = HttpContext.Current.Request;

            HttpCookie cookie = request.Cookies[GetLoadingCookieKey()];

            if (cookie != null && cookie.Value != null && cookie.Value != string.Empty)
                ticket = new Ticket(Common.DecryptString(cookie.Value));

            return ticket;
        }

        /// <summary>
        /// 从Url中的参数读取Ticket信息
        /// </summary>
        /// <returns>对象。</returns>
        public static ITicket LoadFromUrl()
        {
            return LoadFromUrl(PassportManager.TicketParamName);
        }

        /// <summary>
        /// 从Url中的参数读取Ticket信息
        /// </summary>
        /// <param name="reqParamName">url中对应ticket的参数名称</param>
        /// <returns>对象。</returns>
        public static ITicket LoadFromUrl(string reqParamName)
        {
            ITicket ticket = null;

            HttpRequest request = HttpContext.Current.Request;

            string strTicket = request.QueryString[reqParamName];

            try
            {
                if (strTicket != null)
                    ticket = DecryptTicket(strTicket);	//从URL中加载Ticket

                if (ticket != null)
                    Trace.WriteLine(string.Format("从url中找到用户{0}的ticket", ticket.SignInInfo.UserID), "PassportSDK");
            }
            catch (CryptographicException)
            {
            }
            catch (FormatException)
            {
            }

            return ticket;
        }

        /// <summary>
        /// 从Form中的参数读取Ticket信息
        /// </summary>
        /// <returns>对象。</returns>
        public static ITicket LoadFromForm()
        {
            return LoadFromForm(PassportManager.TicketParamName);
        }

        /// <summary>
        /// 从Form中的参数读取Ticket信息
        /// </summary>
        /// <param name="reqParamName">Form中对应ticket的参数名称</param>
        /// <returns>对象。</returns>
        public static ITicket LoadFromForm(string reqParamName)
        {
            ITicket ticket = null;

            HttpRequest request = HttpContext.Current.Request;

            string strTicket = request.Form[reqParamName];

            try
            {
                if (strTicket != null)
                    ticket = DecryptTicket(strTicket);	//从URL中加载Ticket

                if (ticket != null)
                    Trace.WriteLine(string.Format("从Form中找到用户{0}的ticket", ticket.SignInInfo.UserID), "PassportSDK");
            }
            catch (CryptographicException)
            {
            }

            return ticket;
        }

        /// <summary>
        /// 创建票据
        /// </summary>
        /// <param name="signInInfo"></param>
        /// <param name="clientIP"></param>
        /// <returns></returns>
        public static ITicket Create(ISignInInfo signInInfo, string clientIP)
        {
            signInInfo.NullCheck("signInInfo");

            string strIP = clientIP;

            if (string.IsNullOrEmpty(strIP))
            {
                if (HttpContext.Current != null)
                {
                    HttpRequest request = HttpContext.Current.Request;
                    strIP = request.UserHostAddress;
                }
            }

            return new Ticket(Common.GenerateTicketString(signInInfo, strIP));
        }

        /// <summary>
        /// 创建票据
        /// </summary>
        /// <param name="signInInfo"></param>
        /// <returns></returns>
        public static ITicket Create(ISignInInfo signInInfo)
        {
            return Create(signInInfo, string.Empty);
        }

        /// <summary>
        /// 构造类
        /// </summary>
        public Ticket()
        {
        }
        /// <summary>
        /// 构造类
        /// </summary>
        /// <param name="strXml">xml结构的Ticket数据</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\DataObjectsTest.cs" region="TicketTest" lang="cs" title="Ticket对象和Xml对象间的转换" />
        /// </remarks>
        public Ticket(string strXml)
        {
            this.InitFromXml(strXml);
        }

        #region ITicket 成员
        /// <summary>
        /// 用户登录信息
        /// </summary>
        public ISignInInfo SignInInfo
        {
            get
            {
                return this.signInInfo;
            }
        }
        /// <summary>
        /// 应用登录的SessionID
        /// </summary>
        public string AppSignInSessionID
        {
            get
            {
                return this.appSignInSessionID;
            }
        }
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppID
        {
            get
            {
                return this.appID;
            }
        }
        /// <summary>
        /// 应用登录时间
        /// </summary>
        public DateTime AppSignInTime
        {
            get
            {
                return this.appSignInTime;
            }
            set
            {
                this.appSignInTime = value;
            }
        }
        /// <summary>
        /// 应用登录超时时间
        /// </summary>
        public DateTime AppSignInTimeout
        {
            get
            {
                return this.appSignInTimeout;
            }
            set
            {
                this.appSignInTimeout = value;
            }
        }
        /// <summary>
        /// 应用登录IP
        /// </summary>
        public string AppSignInIP
        {
            get
            {
                return this.appSignInIP;
            }
        }
        /// <summary>
        /// Ticket信息保存入Cookie中
        /// </summary>
        public void SaveToCookie()
        {
            Common.CheckHttpContext();

            HttpResponse response = HttpContext.Current.Response;
            HttpRequest request = HttpContext.Current.Request;

            string strData = SaveToXml().InnerXml;

            HttpCookie cookie = new HttpCookie(this.GetSavingCookieKey(), Common.EncryptString(strData));

            cookie.Expires = this.appSignInTimeout;
            cookie.HttpOnly = true;

            response.Cookies.Add(cookie);
        }

        /// <summary>
        /// Ticket信息存成Xml结构
        /// </summary>
        /// <returns>Xml结构的Ticket信息</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\DataObjectsTest.cs" region="TicketTest" lang="cs" title="Ticket对象和Xml对象间的转换" />
        /// </remarks>
        public System.Xml.XmlDocument SaveToXml()
        {
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml("<Ticket/>");

            XmlElement root = xmlDoc.DocumentElement;

            if (this.signInInfo != null)
            {
                XmlNode nodeSignInInfo = XmlHelper.AppendNode(root, "SignInInfo");
                XmlDocument xmlSignInInfo = this.signInInfo.SaveToXml();

                nodeSignInInfo.InnerXml = xmlSignInInfo.DocumentElement.InnerXml;
            }

            XmlHelper.AppendNode(root, "AppSSID", this.appSignInSessionID);
            XmlHelper.AppendNode(root, "AppID", this.appID);
            XmlHelper.AppendNode(root, "AppSTime", Common.DateTimeStandardFormat(this.appSignInTime));
            XmlHelper.AppendNode(root, "AppSTimeout", Common.DateTimeStandardFormat(this.appSignInTimeout));
            XmlHelper.AppendNode(root, "IP", this.appSignInIP);

            return xmlDoc;
        }

        /// <summary>
        /// 判断Ticket是否合法
        /// </summary>
        /// <returns>bool</returns>
        public bool IsValid()
        {
            bool bValid = false;

            try
            {
                ExceptionHelper.TrueThrow<AuthenticateException>(AreDifferentTenantCode(), Resource.DifferentTenantCode);
                ExceptionHelper.TrueThrow<AuthenticateException>(IsAbsoluteTimeExpired(), Resource.AbsoluteTimeExpired);
                ExceptionHelper.TrueThrow<AuthenticateException>(IsSlidingExpired(), Resource.SlidingTimeExpired);
                ExceptionHelper.TrueThrow<AuthenticateException>(IsIPInvalid(), Resource.IPInvalid);
                ExceptionHelper.TrueThrow<AuthenticateException>(IsDifferentAuthenticateServer(), Resource.DifferentAthenticateServer);

                bValid = true;
            }
            catch (AuthenticateException ex)
            {
                //TODO:增加我们自己的Trace
                Trace.WriteLine(string.Format(Resource.TicketInvalidReason, this.SignInInfo.UserID, ex.Message));
            }

            return bValid;
        }

        /// <summary>
        /// 生成加密过的字符串
        /// </summary>
        /// <returns></returns>
        public string ToEncryptString()
        {
            return Common.EncryptTicket(this);
        }
        #endregion

        /// <summary>
        /// 得到加载时的Cookie的Key
        /// </summary>
        /// <returns></returns>
        public static string GetLoadingCookieKey()
        {
            string result = PassportClientSettings.GetConfig().TicketCookieKey;

            //暂时不按照租户分Cookie
            //if (TenantContext.Current.Enabled)
            //    result += "-" + HttpUtility.UrlEncode(TenantContext.Current.TenantCode);

            return result;
        }

        /// <summary>
        /// 解密票据形成ITicket对象
        /// </summary>
        /// <param name="strTicketEncoded"></param>
        /// <returns></returns>
        public static ITicket DecryptTicket(string strTicketEncoded)
        {
            ITicket ticket = null;

            ITicketEncryption et = PassportEncryptionSettings.GetConfig().TicketEncryption;

            try
            {
                byte[] data = Convert.FromBase64String(strTicketEncoded);

                ticket = et.DecryptTicket(data, PassportClientSettings.GetConfig().RsaKeyValue);
            }
            catch (System.Exception ex)
            {
                if (ex is CryptographicException || ex is System.IO.EndOfStreamException || ex is SystemSupportException || ex is FormatException)
                    throw;
            }

            return ticket;
        }

        private bool AreDifferentTenantCode()
        {
            bool result = false;

            if (TenantContext.Current.Enabled && TenantContext.Current.TenantCode.IsNotEmpty())
            {
                result = string.Compare(TenantContext.Current.TenantCode, this.SignInInfo.TenantCode, true) != 0;
            }

            return result;
        }

        /// <summary>
        /// 绝对时间是否过期
        /// </summary>
        /// <returns></returns>
        private bool IsAbsoluteTimeExpired()
        {
            DateTime newExpireDate = GetConfigExpireDate();

            return DateTime.Now >= newExpireDate;
        }

        private DateTime GetConfigExpireDate()
        {
            DateTime dt = DateTime.MaxValue;

            PassportClientSettings settings = PassportClientSettings.GetConfig();

            if (settings.AppSignInTimeout >= 0)
                dt = this.AppSignInTime.AddSeconds(settings.AppSignInTimeout);

            return dt;
        }

        private bool IsIPInvalid()
        {
            return string.Compare(HttpContext.Current.Request.UserHostAddress, this.AppSignInIP, true) != 0;
        }

        /// <summary>
        /// 相对时间过期
        /// </summary>
        /// <returns></returns>
        private bool IsSlidingExpired()
        {
            bool bExpired = false;

            PassportClientSettings settings = PassportClientSettings.GetConfig();

            if (settings.HasSlidingExpiration)
            {
                DateTime dtTO = this.AppSignInTime.Add(settings.AppSlidingExpiration);
                bExpired = (DateTime.Now >= dtTO);
            }

            return bExpired;
        }

        /// <summary>
        /// 认证服务器是否一致
        /// </summary>
        /// <returns></returns>
        private bool IsDifferentAuthenticateServer()
        {
            Uri url = PassportClientSettings.GetConfig().SignInUrl;

            return string.Compare(this.SignInInfo.AuthenticateServer, url.Host + ":" + url.Port, true) != 0;
        }

        private void InitFromXml(string strXml)
        {
            XmlDocument xmlDoc = XmlHelper.CreateDomDocument(strXml);

            XmlElement root = xmlDoc.DocumentElement;

            XmlNode nodeSignInInfo = root.SelectSingleNode("SignInInfo");

            if (nodeSignInInfo != null)
                this.signInInfo = new SignInInfo(nodeSignInInfo.OuterXml);

            this.appSignInSessionID = XmlHelper.GetSingleNodeText(root, "AppSSID");

            this.appID = XmlHelper.GetSingleNodeText(root, "AppID");
            this.appSignInTime = XmlHelper.GetSingleNodeValue(root, "AppSTime", DateTime.MinValue);
            this.appSignInTimeout = XmlHelper.GetSingleNodeValue(root, "AppSTimeout", DateTime.MinValue);
            this.appSignInIP = XmlHelper.GetSingleNodeText(root, "IP");
        }

        /// <summary>
        /// 得到保存时的Cookie的Key
        /// </summary>
        /// <returns></returns>
        private string GetSavingCookieKey()
        {
            string result = PassportClientSettings.GetConfig().TicketCookieKey;

            //暂时不按照租户分Cookie
            //if (TenantContext.Current.Enabled)
            //    result += "-" + HttpUtility.UrlEncode(this.SignInInfo.TenantCode);

            return result;
        }
    }
}
