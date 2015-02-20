using CIIC.HSR.TSP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace CIIC.HSR.TSP.WF.Ctrl.Test
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private const string _IssueName = "CIIC";
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            IoCConfig.Start();
        }
        public override void Init()
        {
            base.PostAuthenticateRequest += MvcApplication_PostAuthenticateRequest;
            base.Init();
        }
        void MvcApplication_PostAuthenticateRequest(object sender, EventArgs e)
        {
            if (!System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return;
            }
            string cookieName = FormsAuthentication.FormsCookieName;
            HttpApplication application = sender as HttpApplication;
            HttpContext Context = application.Context;

            HttpCookie authCookie = Context.Request.Cookies[cookieName];

            if (null == authCookie)
            {
                return;
            }
            FormsAuthenticationTicket authTicket = null;
            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch// (Exception ex)
            {
                return;
            }

            if (null == authTicket)
            {
                // Cookie failed to decrypt.
                return;
            }

            // When the ticket was created, the UserData property was assigned a
            // pipe delimited string of role names.
            //string[] roles = authTicket.UserData.Split('|');

            // Create an Identity object
            AAUserIdentity identity = new AAUserIdentity(authTicket);

            identity.AddClaims(new List<Claim>
                {
                    //加入登录用加入Token，可以加入更多的信息，如部门、地址、年龄等
                    new Claim(ClaimTypes.Name, identity.UserCookieData.UserID.ToString(),ClaimValueTypes.String,_IssueName),
                    new Claim(ClaimTypes.NameIdentifier, identity.UserCookieData.UserID.ToString(),ClaimValueTypes.String,_IssueName),
                    //new Claim(ClaimTypes.Email, user.Email,ClaimValueTypes.String,_IssueName),
                    new Claim(ClaimTypeExtension.TenantId, identity.UserCookieData.TenentCode,ClaimValueTypes.String,_IssueName),
                    new Claim(ClaimTypeExtension.InnerUserId,identity.UserCookieData.UserID.ToString(),ClaimValueTypes.String,_IssueName),
                    new Claim(ClaimTypeExtension.InnerDisplayName, identity.UserCookieData.UserName,ClaimValueTypes.String,_IssueName),
                    new Claim(ClaimTypeExtension.InnerUserAccount, identity.UserCookieData.Account,ClaimValueTypes.String,_IssueName),
                    new Claim(ClaimTypeExtension.ExternalUserAccount, identity.UserCookieData.Account,ClaimValueTypes.String,_IssueName),
                    new Claim(ClaimTypeExtension.ExternalDisplayName, identity.UserCookieData.UserName,ClaimValueTypes.String,_IssueName)
                });

            // This principal will flow throughout the request.
            AAUserPrincipal principal = new AAUserPrincipal(identity);

            // Attach the new principal object to the current HttpContext object
            System.Threading.Thread.CurrentPrincipal = principal;

            Context.User = principal;
        }
    }
}
