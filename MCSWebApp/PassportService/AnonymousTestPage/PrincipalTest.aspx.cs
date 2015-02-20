using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MCS.Library.Principal;
using MCS.Library.Passport;

namespace MCS.Web.Passport.Anonymous
{
    public partial class PrincipalTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SignInLogo.ReturnUrl = SignInLogo.ReturnUrl = Request.Url.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.SafeUnescaped);
            SignInLogo.AutoRedirect = false;

            if (Request.IsAuthenticated)
            {    
                ShowPrincipalInfo();
            }
        }

        private void ShowPrincipalInfo()
        {
            HtmlTable table = new HtmlTable();
            table.Attributes["class"] = "table";

            ShowSinglePrincipalInfo(table, "User Logon name", DeluxeIdentity.CurrentUser.LogOnName);
            ShowSinglePrincipalInfo(table, "User Display name", DeluxeIdentity.CurrentUser.DisplayName);

            principalInfo.Controls.Add(table);
        }

        private void ShowSinglePrincipalInfo(Control parent, string name, string data)
        {
            HtmlTableRow row = new HtmlTableRow();

            HtmlTableCell cell = new HtmlTableCell();
            cell.InnerText = name + ":";
            cell.Attributes["class"] = "captionCell";
            row.Controls.Add(cell);

            cell = new HtmlTableCell();
            cell.InnerText = data;
            row.Controls.Add(cell);

            parent.Controls.Add(row);
        }
    }
}
