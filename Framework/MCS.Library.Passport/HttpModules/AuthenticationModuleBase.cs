using MCS.Library.Caching;
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.Passport.Properties;
using MCS.Library.Principal;
using MCS.Web.Responsive.Library;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace MCS.Library.Passport
{
    /// <summary>
    /// 认证的基类
    /// </summary>
    public abstract class AuthenticationModuleBase : IHttpModule
    {
        #region IHttpModule 成员
        /// <summary>
        /// 析构函数
        /// </summary>
        public virtual void Dispose()
        {

        }
        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="context">HttpApplication</param>
        public virtual void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
            context.AuthenticateRequest += new EventHandler(context_AuthenticateRequest);
            context.AuthorizeRequest += new EventHandler(context_AuthorizeRequest);
            context.PreRequestHandlerExecute += new EventHandler(context_PreRequestHandlerExecute);
        }

        private static void context_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            ProcessTimePoint();
        }

        private static void context_AuthorizeRequest(object sender, EventArgs e)
        {
            AuthorizationDirElement ade = AuthenticateDirSettings.GetConfig().AuthorizationDirs.GetMatchedElement<AuthorizationDirElement>();

            if (ade != null)
            {
                try
                {
                    (HttpContext.Current.User != null).FalseThrow<AuthenticateException>(
                        Translator.Translate(Define.DefaultCategory, "您还没有经过认证，没有权限访问此页面"));

                    ade.IsCurrentUserInRoles().FalseThrow<AuthenticateException>(
                        Translator.Translate(Define.DefaultCategory, "您没有权限访问此页面"));
                }
                catch (AuthenticateException ex)
                {
                    HttpContext.Current.Response.ContentType = "text/html";
                    HttpContext.Current.Response.Write(ex.Message);
                    HttpContext.Current.Response.End();
                }
            }
        }

        private void context_AuthenticateRequest(object sender, EventArgs e)
        {
            if (AuthenticateDirSettings.GetConfig().PageNeedAuthenticate())
                DoAuthentication();
        }

        private static void context_BeginRequest(object sender, EventArgs e)
        {
            //从环境中(url)获取TenantCode
            TenantContext.Current.TenantCode = PassportManager.GetTenantCodeFromContext();

            HttpContext.Current.Items["IsDeluxeWorksAuthenticate"] = true;

            if (HttpContext.Current.Request.QueryString[AccessTicket.AccquireAccessTicketParamName].IsNotEmpty())
                ProcessAccquireAccessTicket();
        }

        /// <summary>
        /// 认证是否通过DeluxeWorksAuthenticate来进行
        /// </summary>
        public static bool IsDeluxeWorksAuthenticate
        {
            get
            {
                return DictionaryHelper.GetValue(HttpContext.Current.Items, "IsDeluxeWorksAuthenticate", false);
            }
        }

        /// <summary>
        /// 是否可以注销
        /// </summary>
        public static bool CanLogOff
        {
            get
            {
                return DictionaryHelper.GetValue(HttpContext.Current.Items, "CanLogOff", false);
            }
            protected set
            {
                HttpContext.Current.Items["CanLogOff"] = value;
            }
        }
        #endregion
        /// <summary>
        /// 获取登录名称。
        /// </summary>
        /// <param name="ticket">登录票据。</param>
        /// <returns>登录名称</returns>
        protected abstract string GetLogOnName(out ITicket ticket);

        /// <summary>
        /// 如果接收到页面访问票据的请求，则生成访问票据
        /// </summary>
        private static void ProcessAccquireAccessTicket()
        {
            HttpRequest request = HttpContext.Current.Request;

            AccessTicket aTicket = new AccessTicket();

            aTicket.GenerateTime = DateTime.Now;

            Uri targetUri = new Uri(request.QueryString[AccessTicket.AccquireAccessTicketParamName], UriKind.RelativeOrAbsolute);

            aTicket.DestinationUrl = targetUri.ToString();

            if (request.QueryString[AccessTicket.AutoMakeAbsoluteParamName] != null && request.QueryString[AccessTicket.AutoMakeAbsoluteParamName].ToLower() == "true")
                aTicket.MakeDestinationUrlAbsolute(request.Url);

            StringBuilder strB = new StringBuilder();

            strB.AppendLine("<script type=\"text/javascript\">");
            strB.AppendFormat("var anchor = parent.document.getElementById(\"{0}\");\n", request.QueryString["_anchorID"]);
            strB.AppendFormat("anchor.href = \"{0}\";\n",
                aTicket.AppendToUrl(targetUri.ToString()));

            strB.AppendLine("var eventSink = anchor.getAttribute(\"OnClientAccquiredAccessTicket\");");
            strB.AppendLine("if (eventSink && eventSink != \"\")");
            strB.AppendLine("\teval(\"parent.\" + eventSink + \"(anchor)\");");
            strB.AppendLine("</script>");

            HttpResponse response = HttpContext.Current.Response;

            try
            {
                response.Cache.SetCacheability(HttpCacheability.NoCache);
                response.Write(strB.ToString());
            }
            catch (System.Exception ex)
            {
                ex.WriteToEventLog("webApplicationError");

                response.Write(ex.ToString());
            }
            finally
            {
                response.End();
            }
        }

        /// <summary>
        /// 处理TimePoint
        /// </summary>
        private static void ProcessTimePoint()
        {
            TimePointSimulationSettings settings = TimePointSimulationSettings.GetConfig();

            if (settings.Enabled)
            {
                if (DeluxePrincipal.IsAuthenticated)
                {
                    DateTime simulatedTime = DateTime.Now.OriginalSimulateTime();

                    try
                    {
                        if (settings.Persister != null)
                            simulatedTime = settings.Persister.LoadTimePoint(DeluxeIdentity.CurrentUser.ID);
                    }
                    catch (System.Exception)
                    {
                    }

                    TimePointContext.Current.UseCurrentTime = (simulatedTime == DateTime.MinValue);
                    TimePointContext.Current.SimulatedTime = simulatedTime;
                }
            }
        }

        private string InternalGetLogOnName(out ITicket ticket)
        {
            string userID = string.Empty;
            ticket = null;

            if (ImpersonateSettings.GetConfig().EnableTestUser)
            {
                //是否使用测试帐户
                userID = HttpContext.Current.Request.Headers["testUserID"];

                if (string.IsNullOrEmpty(userID) == false)
                    HttpContext.Current.Response.AppendHeader("testUserID", userID);
            }

            if (string.IsNullOrEmpty(userID))
                userID = GetLogOnName(out ticket);

            return userID;
        }

        private void DoAuthentication()
        {
            ITicket ticket;

            string logonName = InternalGetLogOnName(out ticket);

            if (logonName.IsNotEmpty())
            {
                logonName = ImpersonateSettings.GetConfig().Impersonation[logonName];

                LogOnIdentity loi = new LogOnIdentity(logonName);

                if (ticket != null)
                    ticket.SignInInfo.UserID = loi.LogOnNameWithDomain;

                SetTenantContext(ticket);
                SetPrincipal(loi.LogOnNameWithDomain, ticket);
            }
        }

        /// <summary>
        /// 设置租户的上下文信息
        /// </summary>
        /// <param name="ticket"></param>
        private void SetTenantContext(ITicket ticket)
        {
            if (TenantContext.Current.TenantCode.IsNullOrEmpty())
                TenantContext.Current.TenantCode = ticket.SignInInfo.TenantCode;
        }

        private static void SetPrincipal(string userID, ITicket ticket)
        {
            IPrincipal principal = GetPrincipalInSession(userID);

            if (principal == null)
            {
                LogOnIdentity loi = new LogOnIdentity(userID);

                string identityID = string.Empty;

                if (PassportClientSettings.GetConfig().IdentityWithoutDomainName)
                    identityID = loi.LogOnNameWithoutDomain;
                else
                    identityID = loi.LogOnName;

                principal = PrincipalSettings.GetConfig().GetPrincipalBuilder().CreatePrincipal(identityID, ticket);

                HttpCookie cookie = new HttpCookie(Common.GetPrincipalSessionKey());
                cookie.Expires = DateTime.MinValue;

                CookieCacheDependency cookieDependency = new CookieCacheDependency(cookie);

                SlidingTimeDependency slidingDependency =
                    new SlidingTimeDependency(Common.GetSessionTimeOut());

                PrincipalCache.Instance.Add(
                    Common.GetPrincipalSessionKey(),
                    principal,
                    new MixedDependency(cookieDependency, slidingDependency));
            }

            PrincipaContextAccessor.SetPrincipal(principal);
        }

        private static IPrincipal GetPrincipalInSession(string userID)
        {
            string strKey = Common.GetPrincipalSessionKey();
            IPrincipal principal;

            if (PrincipalCache.Instance.TryGetValue(strKey, out principal))
                if (string.Compare(principal.Identity.Name, userID, true) != 0)
                    principal = null;

            return principal;
        }
    }
}
