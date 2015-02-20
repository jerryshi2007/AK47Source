using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using MCS.Library.Passport;
using MCS.Library.Principal;
using System.ComponentModel;
using System.Web.UI;

namespace MCSResponsiveOAPortal.WebControls
{
    [Designer("MCS.Web.Responsive.WebControls.Design.GenericDesigner, MCS.Web.Responsive.WebControls")]
    [ToolboxData("<{0}:LogOutLink runat=\"server\" />")]
    public class LogOutLink : System.Web.UI.HtmlControls.HtmlAnchor
    {
        public LogOutLink()
        {
            this.EnableViewState = false;
        }

        protected override void OnPreRender(EventArgs e)
        {
            this.SetAttribute("style", "display:none");
            this.SetAttribute("data-role", "logoutlink");
            if (DeluxeIdentity.CurrentUser != null)
            {
                this.SetAttribute("data-login-name", DeluxeIdentity.CurrentUser.LogOnName);
                this.SetAttribute("data-login-user", DeluxeIdentity.CurrentUser.DisplayName);
            }
            string url = PassportManager.GetLogOnOrLogOffUrl(this.ResolveUrl("~/Default.aspx"), true, true);
            this.HRef = url;
            base.OnPreRender(e);
        }
    }
}