#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	Ticket.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          ����ǿ      2008-12-2       ���ע��
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
    /// Ʊ����Ϣ��
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
        /// ��Cookie�ж�ȡITicket��Ϣ
        /// </summary>
        /// <returns><see cref="ITicket"/> ����</returns>
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
        /// ��Url�еĲ�����ȡTicket��Ϣ
        /// </summary>
        /// <returns>����</returns>
        public static ITicket LoadFromUrl()
        {
            return LoadFromUrl(PassportManager.TicketParamName);
        }

        /// <summary>
        /// ��Url�еĲ�����ȡTicket��Ϣ
        /// </summary>
        /// <param name="reqParamName">url�ж�Ӧticket�Ĳ�������</param>
        /// <returns>����</returns>
        public static ITicket LoadFromUrl(string reqParamName)
        {
            ITicket ticket = null;

            HttpRequest request = HttpContext.Current.Request;

            string strTicket = request.QueryString[reqParamName];

            try
            {
                if (strTicket != null)
                    ticket = DecryptTicket(strTicket);	//��URL�м���Ticket

                if (ticket != null)
                    Trace.WriteLine(string.Format("��url���ҵ��û�{0}��ticket", ticket.SignInInfo.UserID), "PassportSDK");
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
        /// ��Form�еĲ�����ȡTicket��Ϣ
        /// </summary>
        /// <returns>����</returns>
        public static ITicket LoadFromForm()
        {
            return LoadFromForm(PassportManager.TicketParamName);
        }

        /// <summary>
        /// ��Form�еĲ�����ȡTicket��Ϣ
        /// </summary>
        /// <param name="reqParamName">Form�ж�Ӧticket�Ĳ�������</param>
        /// <returns>����</returns>
        public static ITicket LoadFromForm(string reqParamName)
        {
            ITicket ticket = null;

            HttpRequest request = HttpContext.Current.Request;

            string strTicket = request.Form[reqParamName];

            try
            {
                if (strTicket != null)
                    ticket = DecryptTicket(strTicket);	//��URL�м���Ticket

                if (ticket != null)
                    Trace.WriteLine(string.Format("��Form���ҵ��û�{0}��ticket", ticket.SignInInfo.UserID), "PassportSDK");
            }
            catch (CryptographicException)
            {
            }

            return ticket;
        }

        /// <summary>
        /// ����Ʊ��
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
        /// ����Ʊ��
        /// </summary>
        /// <param name="signInInfo"></param>
        /// <returns></returns>
        public static ITicket Create(ISignInInfo signInInfo)
        {
            return Create(signInInfo, string.Empty);
        }

        /// <summary>
        /// ������
        /// </summary>
        public Ticket()
        {
        }
        /// <summary>
        /// ������
        /// </summary>
        /// <param name="strXml">xml�ṹ��Ticket����</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\DataObjectsTest.cs" region="TicketTest" lang="cs" title="Ticket�����Xml������ת��" />
        /// </remarks>
        public Ticket(string strXml)
        {
            this.InitFromXml(strXml);
        }

        #region ITicket ��Ա
        /// <summary>
        /// �û���¼��Ϣ
        /// </summary>
        public ISignInInfo SignInInfo
        {
            get
            {
                return this.signInInfo;
            }
        }
        /// <summary>
        /// Ӧ�õ�¼��SessionID
        /// </summary>
        public string AppSignInSessionID
        {
            get
            {
                return this.appSignInSessionID;
            }
        }
        /// <summary>
        /// Ӧ��ID
        /// </summary>
        public string AppID
        {
            get
            {
                return this.appID;
            }
        }
        /// <summary>
        /// Ӧ�õ�¼ʱ��
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
        /// Ӧ�õ�¼��ʱʱ��
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
        /// Ӧ�õ�¼IP
        /// </summary>
        public string AppSignInIP
        {
            get
            {
                return this.appSignInIP;
            }
        }
        /// <summary>
        /// Ticket��Ϣ������Cookie��
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
        /// Ticket��Ϣ���Xml�ṹ
        /// </summary>
        /// <returns>Xml�ṹ��Ticket��Ϣ</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\DataObjectsTest.cs" region="TicketTest" lang="cs" title="Ticket�����Xml������ת��" />
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
        /// �ж�Ticket�Ƿ�Ϸ�
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
                //TODO:���������Լ���Trace
                Trace.WriteLine(string.Format(Resource.TicketInvalidReason, this.SignInInfo.UserID, ex.Message));
            }

            return bValid;
        }

        /// <summary>
        /// ���ɼ��ܹ����ַ���
        /// </summary>
        /// <returns></returns>
        public string ToEncryptString()
        {
            return Common.EncryptTicket(this);
        }
        #endregion

        /// <summary>
        /// �õ�����ʱ��Cookie��Key
        /// </summary>
        /// <returns></returns>
        public static string GetLoadingCookieKey()
        {
            string result = PassportClientSettings.GetConfig().TicketCookieKey;

            //��ʱ�������⻧��Cookie
            //if (TenantContext.Current.Enabled)
            //    result += "-" + HttpUtility.UrlEncode(TenantContext.Current.TenantCode);

            return result;
        }

        /// <summary>
        /// ����Ʊ���γ�ITicket����
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
        /// ����ʱ���Ƿ����
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
        /// ���ʱ�����
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
        /// ��֤�������Ƿ�һ��
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
        /// �õ�����ʱ��Cookie��Key
        /// </summary>
        /// <returns></returns>
        private string GetSavingCookieKey()
        {
            string result = PassportClientSettings.GetConfig().TicketCookieKey;

            //��ʱ�������⻧��Cookie
            //if (TenantContext.Current.Enabled)
            //    result += "-" + HttpUtility.UrlEncode(this.SignInInfo.TenantCode);

            return result;
        }
    }
}
