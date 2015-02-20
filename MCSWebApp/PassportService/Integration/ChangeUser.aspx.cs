using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Passport;

namespace MCS.Web.Passport.Integration
{
    public partial class ChangeUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            string lastUserID = Request.QueryString["lastUserID"];

            string returnUrl = Request.QueryString["ru"];

            if (string.IsNullOrEmpty(returnUrl) == false)
            {
                string logonUserID = Request.ServerVariables["LOGON_USER"];

                ExceptionHelper.FalseThrow(string.IsNullOrEmpty(logonUserID) == false, "不能取到LOGON_USER，该页面应该设置为禁止匿名访问");

                LogOnIdentity loi = new LogOnIdentity(logonUserID);

                if (string.Compare(loi.LogOnNameWithoutDomain, lastUserID, true) == 0)
                {
                    Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    Response.AddHeader("WWW-Authenticate", "NTLM");
                }
                else
                {
                    Response.Redirect(PassportManager.GetSignInPageUrl(returnUrl, logonUserID));
                }
            }
        }
    }
}
