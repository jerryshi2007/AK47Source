#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	PassportAuthenticationModule.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          ����ǿ      2008-12-2       ���ע��
// -------------------------------------------------
#endregion
using System;
using System.Web;
using System.Text;
using System.Drawing.Imaging;
using System.Security.Principal;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Caching;
using MCS.Library.Principal;
using MCS.Library.Passport.Properties;

namespace MCS.Library.Passport
{
    /// <summary>
    /// �����¼��֤��
    /// </summary>
    public sealed class PassportAuthenticationModule : AuthenticationModuleBase
    {
        /// <summary>
        /// ��ʼ������
        /// </summary>
        /// <param name="context">HttpApplication</param>
        public override void Init(HttpApplication context)
        {
            base.Init(context);
            context.BeginRequest += new EventHandler(context_BeginRequest);
        }

        /// <summary>
        /// ������֤�������û���
        /// </summary>
        /// <param name="ticket"><see cref="ITicket"/> ����</param>
        /// <returns>�û���</returns>
        protected override string GetLogOnName(out ITicket ticket)
        {
            string userID = string.Empty;
            ticket = CheckAuthenticatedAndGetTicket();

            if (ticket != null)
            {
                if (PassportClientSettings.GetConfig().IdentityWithoutDomainName)
                    userID = ticket.SignInInfo.UserID;
                else
                    userID = ticket.SignInInfo.UserIDWithDomain;

                AuthenticationModuleBase.CanLogOff = true;
            }

            return userID;
        }

        private ITicket CheckAuthenticatedAndGetTicket()
        {
            AuthenticateDirElement aDir =
                AuthenticateDirSettings.GetConfig().AuthenticateDirs.GetMatchedElement<AuthenticateDirElement>();

            bool autoRedirect = (aDir == null || aDir.AutoRedirect);

            PassportManager.CheckAuthenticated(autoRedirect);

            bool fromCookie = false;

            return PassportManager.GetTicket(out fromCookie);
        }

        private void context_BeginRequest(object sender, EventArgs e)
        {
            DoPreAuthenticateOP();
        }

        private void DoPreAuthenticateOP()
        {
            HttpRequest request = HttpContext.Current.Request;

            if (request.Path.IndexOf(Common.C_LOGOFF_CALLBACK_VIRTUAL_PATH, StringComparison.OrdinalIgnoreCase) != -1)
                DoLogOff();
        }

        private void DoLogOff()
        {
            PassportManager.ClearAppSignInCookie();

            HttpResponse response = HttpContext.Current.Response;

            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            response.ContentType = "image/gif";
            try
            {
                Resource.success.Save(response.OutputStream, ImageFormat.Gif);
            }
            catch (System.Exception)
            {
                Resource.fail.Save(response.OutputStream, ImageFormat.Gif);
            }
            finally
            {
                response.AddHeader("P3P", "CP-TST");
                response.Flush();
                response.End();
            }
        }
    }
}
