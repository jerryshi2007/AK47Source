using MCS.Library.Core;
using MCS.Library.Passport.Properties;
using MCS.Web.Responsive.Library;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;

namespace MCS.Library.Passport
{
    /// <summary>
    /// Passport�����ࡣ
    /// </summary>
    public static class PassportManager
    {
        /// <summary>
        /// Ticket��url�еĲ�������
        /// </summary>
        public const string TicketParamName = "t";

        /// <summary>
        /// �⻧����Ĳ�������
        /// </summary>
        public const string TenantCodeParamName = "tenantCode";

        private static readonly string[] ReservedParams = { PassportManager.TicketParamName, "ru", "to", "aid", "ip", "lou", PassportManager.TenantCodeParamName };

        #region ��̬����
        /// <summary>
        /// �����֤�����Cookie
        /// </summary>
        public static void ClearSignInCookie()
        {
            Common.CheckHttpContext();

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            HttpCookie cookie = request.Cookies[SignInInfo.GetLoadingCookieKey()];

            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);

                cookie.Value = null;
                response.SetCookie(cookie);
            }
        }

        /// <summary>
        /// ���Ӧ�õ�Cookie
        /// </summary>
        public static void ClearAppSignInCookie()
        {
            Common.CheckHttpContext();

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            HttpCookie cookie = request.Cookies[Ticket.GetLoadingCookieKey()];

            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                cookie.Value = null;
                cookie.HttpOnly = true;

                response.SetCookie(cookie);
            }
        }

        #endregion
        /// <summary>
        /// ��ȡע�����ض����ַ
        /// </summary>
        /// <returns>ע�����ض���url</returns>
        public static Uri GetLogOffCallBackUrl()
        {
            string strLouUrl = string.Empty;
            string locu = PassportClientSettings.GetConfig().LogOffCallBackUrl.ToString();

            HttpRequest request = HttpContext.Current.Request;

            if (locu == string.Empty)
                strLouUrl = request.Url.GetComponents(
                                UriComponents.SchemeAndServer, UriFormat.SafeUnescaped) +
                                (request.ApplicationPath == "/" ?
                                    request.ApplicationPath + Common.C_LOGOFF_CALLBACK_VIRTUAL_PATH :
                                    request.ApplicationPath + "/" + Common.C_LOGOFF_CALLBACK_VIRTUAL_PATH);
            else
                strLouUrl = ChangeUrlToCurrentServer(locu);

            NameValueCollection uriParams = UriHelper.GetUriParamsCollection(strLouUrl);

            uriParams[TenantCodeParamName] = TenantContext.Current.TenantCode;

            return new Uri(UriHelper.CombineUrlParams(strLouUrl, true, uriParams), UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// ��Cookie�еõ�Ticket
        /// </summary>
        /// <returns><see cref="ITicket"/> ����</returns>
        public static ITicket GetTicket(out bool fromCookie)
        {
            fromCookie = false;

            Common.CheckHttpContext();

            HttpContext context = HttpContext.Current;

            ITicket ticket = null;

            if (PassportClientSettings.GetConfig().Method == TicketTransferMethod.HttpPost
                        && string.Compare(context.Request.HttpMethod, "POST", true) == 0)
                ticket = Ticket.LoadFromForm();
            else
                ticket = Ticket.LoadFromUrl();

            if (IsTicketValid(ticket) == false)
            {
                ticket = Ticket.LoadFromCookie();	//��Cookie�м���Ticket

                if (ticket != null)
                {
                    fromCookie = true;
                    Trace.WriteLine(string.Format("��cookie���ҵ��û�{0}��ticket", ticket.SignInInfo.UserID), "PassportSDK");
                }
            }

            if (IsTicketValid(ticket) == true)
                AdjustSignInTimeout(ticket);

            return ticket;
        }

        /// <summary>
        /// �õ���֤ҳ���URL
        /// </summary>
        /// <param name="strReturlUrl">���ص�URL</param>
        /// <returns>�õ���֤ҳ���URL</returns>
        public static string GetSignInPageUrl(string strReturlUrl)
        {
            return GetSignInPageUrl(strReturlUrl, string.Empty);
        }

        /// <summary>
        /// ��ȡ��֤ҳ���url
        /// </summary>
        /// <param name="strReturlUrl"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static string GetSignInPageUrl(string strReturlUrl, string uid)
        {
            Common.CheckHttpContext();

            string url = PassportClientSettings.GetConfig().SignInUrl.ToString() + GetExtraRequestParams(strReturlUrl);

            if (uid.IsNotEmpty())
                url += "&uid=" + HttpUtility.UrlEncode(Common.EncryptString(uid));

            return url;
        }

        /// <summary>
        /// ����UserID
        /// </summary>
        /// <param name="encUid"></param>
        /// <returns></returns>
        public static string DecryptUserID(string encUid)
        {
            string uid = string.Empty;

            if (encUid.IsNotEmpty())
                uid = Common.DecryptString(encUid);

            return uid;
        }

        /// <summary>
        /// ��ȡ��¼��ע����url������url�е���֤���ض����returnUrl
        /// </summary>
        /// <param name="returnUrl">��֤ͨ�����ض����ַ</param>
        /// <returns>��¼����ע��url</returns>
        public static string GetLogOnOrLogOffUrl(string returnUrl)
        {
            return GetLogOnOrLogOffUrl(returnUrl, true, true);
        }

        /// <summary>
        /// ��ȡ��¼��ע����url������url�е���֤���ض����returnUrl������ע�����ض����logOffAutoRedirect
        /// </summary>
        /// <param name="returnUrl">��֤���ض���ĵ�ַ</param>
        /// <param name="logOffAutoRedirect">�Ƿ�ע�����ض���</param>
        /// <param name="logOffAll">�Ƿ�ע������Ӧ��</param>
        /// <returns>��¼����ע��url</returns>
        public static string GetLogOnOrLogOffUrl(string returnUrl, bool logOffAutoRedirect, bool logOffAll)
        {
            Common.CheckHttpContext();
            string strResult = string.Empty;

            bool fromCookie = false;
            ITicket ticket = GetTicket(out fromCookie);

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;

            PassportClientSettings settings = PassportClientSettings.GetConfig();
            string strPassportPath = settings.SignInUrl.ToString();

            int nSplit = strPassportPath.LastIndexOf("/");
            strPassportPath = strPassportPath.Substring(0, nSplit + 1);

            if (IsTicketValid(ticket) == true)
            {
                StringBuilder strB = new StringBuilder(1024);

                strB.Append(settings.LogOffUrl);

                NameValueCollection parameters = new NameValueCollection();

                parameters.Add("asid", ticket.SignInInfo.SignInSessionID);
                parameters.Add("ru", returnUrl);
                parameters.Add("lar", logOffAutoRedirect.ToString().ToLower());
                parameters.Add("appID", ticket.AppID);
                parameters.Add("lou", GetLogOffCallBackUrl().ToString());
                parameters.Add("loa", logOffAll.ToString().ToLower());
                parameters.Add("wi", ticket.SignInInfo.WindowsIntegrated.ToString().ToLower());
                parameters.Add("lu", ticket.SignInInfo.OriginalUserID);

                if (TenantContext.Current.Enabled)
                    parameters.Add(PassportManager.TenantCodeParamName, TenantContext.Current.TenantCode);

                strB.Append("?" + parameters.ToUrlParameters(true));

                //strB.AppendFormat("?asid={0}&ru={1}&lar={2}&appID={3}&lou={4}&loa={5}&wi={6}&lu={7}",
                //    ticket.SignInInfo.SignInSessionID,
                //    HttpUtility.UrlEncode(returnUrl),
                //    logOffAutoRedirect.ToString().ToLower(),
                //    ticket.AppID,
                //    HttpUtility.UrlEncode(GetLogOffCallBackUrl().ToString()),
                //    logOffAll.ToString().ToLower(),
                //    ticket.SignInInfo.WindowsIntegrated.ToString().ToLower(),
                //    ticket.SignInInfo.OriginalUserID
                //    );

                strResult = strB.ToString();
            }
            else
                strResult = GetSignInPageUrl(returnUrl);

            return strResult;
        }

        /// <summary>
        /// ���ݵ�ǰ��Web���󣬵õ���֤����Ҫ�ض����url���ڴ˹����м��"t"�����Ƿ���ڣ�������ڣ����׳��쳣
        /// </summary>
        /// <returns>��֤����Ҫ�ض����url</returns>
        public static string GetReturnUrl()
        {
            Common.CheckHttpContext();
            HttpRequest request = HttpContext.Current.Request;

            StringBuilder strB = new StringBuilder(2048);

            strB.Append(request.Url.GetLeftPart(UriPartial.Path));

            bool bFirstParam = true;

            foreach (string strKey in request.QueryString)
            {
                if (strKey != PassportManager.TicketParamName)
                {
                    ExceptionHelper.TrueThrow<AuthenticateException>(
                        Array.Exists<string>(PassportManager.ReservedParams, delegate(string data)
                        {
                            return string.Compare(strKey, data, true) == 0;
                        }),
                        Resource.ParamIsReserved, strKey);

                    if (bFirstParam)
                    {
                        strB.Append("?");
                        bFirstParam = false;
                    }
                    else
                        strB.Append("&");

                    strB.Append(strKey + "=" + request.QueryString[strKey]);
                }
            }

            return strB.ToString();
        }

        /// <summary>
        /// ��RouteTable����QueryString�л�ȡTenantCode
        /// </summary>
        /// <returns></returns>
        public static string GetTenantCodeFromContext()
        {
            string tenantCode = HttpContext.Current.GetRouteDataValue(PassportManager.TenantCodeParamName, string.Empty);

            if (tenantCode.IsNullOrEmpty())
                tenantCode = Request.GetRequestQueryString(PassportManager.TenantCodeParamName, string.Empty);

            return tenantCode;
        }

        /// <summary>
        /// ���Ӧ�õ���֤Cookie�Ƿ���Ч�����ʧЧ�����Զ�ת����֤ҳ��
        /// </summary>
        public static void CheckAuthenticated()
        {
            CheckAuthenticated(true);
        }

        /// <summary>
        /// ���Ӧ�õ���֤Cookie�Ƿ���Ч�����ʧЧ������autoRedirect�����������Ƿ�ת����֤ҳ��
        /// </summary>
        /// <param name="autoRedirect">�Ƿ��Զ�ת����֤ҳ��</param>
        public static void CheckAuthenticated(bool autoRedirect)
        {
            Common.CheckHttpContext();
            HttpContext context = HttpContext.Current;

            bool fromCookie = false;
            ITicket ticket = GetTicket(out fromCookie);

            if (IsTicketValid(ticket) == false)
            {
                if (autoRedirect)
                    context.Response.Redirect(GetSignInPageUrl(GetReturnUrl()));
            }
            else
            {
                ticket.SaveToCookie();

                if (fromCookie == false)
                {
                    if (PassportClientSettings.GetConfig().Method == TicketTransferMethod.HttpPost
                        && string.Compare(context.Request.HttpMethod, "POST", true) == 0)
                        context.Response.Redirect(context.Request.Url.ToString());
                }
            }
        }

        /// <summary>
        /// ������֤�ķ��񣬴���Ticket�ض���Ӧ�õ�url
        /// </summary>
        /// <param name="ticket"></param>
        public static void SignInServiceRedirectToApp(ITicket ticket)
        {
            HttpRequest request = HttpContext.Current.Request;
            HttpResponse response = HttpContext.Current.Response;

            string strReturnUrl = HttpUtility.UrlDecode(request.QueryString["ru"]);
            string strLogOffUrl = request.QueryString["lou"];
            string strAppID = request.QueryString["aid"];

            if (strAppID == null)
                strAppID = PassportClientSettings.GetConfig().AppID;

            System.Uri uri = request.Url;

            if (strReturnUrl != null)
                uri = new Uri(strReturnUrl, UriKind.RelativeOrAbsolute);

            NameValueCollection uriParams = uri.GetUriParamsCollection();

            uriParams[TenantCodeParamName] = TenantContext.Current.TenantCode;

            uri = new Uri(UriHelper.CombineUrlParams(uri.ToString(), true, uriParams), UriKind.RelativeOrAbsolute);

            if (strLogOffUrl == null)
                strLogOffUrl = "#";

            Uri logOffUri = new Uri(UriHelper.CombineUrlParams(strLogOffUrl, true, uriParams), UriKind.RelativeOrAbsolute);

            PassportSignInSettings.GetConfig().PersistSignInInfo.SaveTicket(
                ticket,
                uri,
                logOffUri);

            string ticketString = Common.EncryptTicket(ticket);

            TicketTransferMethod method = request.QueryString.GetValue("m", TicketTransferMethod.HttpGet);

            if (method == TicketTransferMethod.HttpGet)
                RedirectTicketToApp(uri, ticketString);
            else
                SubmitTicketToApp(uri, ticketString);
        }

        #region ˽�з���
        private static void RedirectTicketToApp(Uri url, string ticketString)
        {
            HttpResponse response = HttpContext.Current.Response;

            string targetParams = string.Format("{0}={1}",
                                    PassportManager.TicketParamName,
                                    HttpUtility.UrlEncode(ticketString));

            if (url.IsAbsoluteUri)
            {
                if (url.Query.Length > 0)
                    response.Redirect(url.ToString() + "&" + targetParams);
                else
                    response.Redirect(url.ToString() + "?" + targetParams);
            }
            else
            {
                string uriString = url.ToString();

                if (uriString.IndexOf("?") >= 0)
                    response.Redirect(uriString + "&" + targetParams);
                else
                    response.Redirect(uriString + "?" + targetParams);
            }
        }

        private static void SubmitTicketToApp(Uri url, string ticketString)
        {
            string html = Assembly.GetExecutingAssembly().LoadStringFromResource(typeof(PassportManager).Namespace + ".Mechanism.afterSignInPost.htm");

            html = string.Format(html,
                HttpUtility.HtmlAttributeEncode(url.ToString()),
                HttpUtility.HtmlAttributeEncode(ticketString));

            HttpResponse response = HttpContext.Current.Response;

            response.Write(html);
            response.End();
        }

        private static bool IsTicketValid(ITicket ticket)
        {
            return ticket != null && ticket.IsValid();
        }

        private static void AdjustSignInTimeout(ITicket ticket)
        {
            if (PassportClientSettings.GetConfig().HasSlidingExpiration)
                ticket.AppSignInTime = DateTime.Now;
        }

        /// <summary>
        /// ��ָ�����·����Urlӳ�䵽��ǰ������������һ������·��
        /// </summary>
        /// <param name="strUrl">���·����Url</param>
        /// <returns>����·����Url</returns>
        private static string ChangeUrlToCurrentServer(string strUrl)
        {
            string result = string.Empty;
            Uri url = new Uri(strUrl, UriKind.RelativeOrAbsolute);

            if (url.IsAbsoluteUri == false)
                result = HttpContext.Current.Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped)
                    + Path.Combine("/", url.ToString());
            else
                result = url.ToString();

            return result;
        }

        /// <summary>
        /// ƴ����֤url�еĶ������
        /// </summary>
        /// <param name="strReturlUrl"></param>
        /// <returns></returns>
        private static string GetExtraRequestParams(string strReturlUrl)
        {
            HttpRequest request = HttpContext.Current.Request;

            PassportClientSettings clientConfig = PassportClientSettings.GetConfig();

            NameValueCollection parameters = new NameValueCollection();

            parameters.Add("ru", strReturlUrl);
            parameters.Add("to", clientConfig.AppSignInTimeout.ToString());
            parameters.Add("aid", clientConfig.AppID);
            parameters.Add("ip", request.UserHostAddress);
            parameters.Add("lou", GetLogOffCallBackUrl().ToString());
            parameters.Add("m", clientConfig.Method.ToString());

            if (TenantContext.Current.Enabled)
                parameters.Add(PassportManager.TenantCodeParamName, TenantContext.Current.TenantCode);
            //string result = "?ru=" + HttpUtility.UrlEncode(strReturlUrl)
            //                + "&to=" + HttpUtility.UrlEncode(clientConfig.AppSignInTimeout.ToString())
            //                + "&aid=" + HttpUtility.UrlEncode(clientConfig.AppID)
            //                + "&ip=" + HttpUtility.UrlEncode(request.UserHostAddress)
            //                + "&lou=" + HttpUtility.UrlEncode(GetLogOffCallBackUrl().ToString())
            //                + "&m=" + HttpUtility.UrlEncode(clientConfig.Method.ToString());

            return "?" + parameters.ToUrlParameters(true);
        }
        #endregion ˽�з���
    }
}
