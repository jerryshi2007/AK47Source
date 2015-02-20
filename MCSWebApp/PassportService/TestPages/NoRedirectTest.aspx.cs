using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Principal;

namespace MCS.Web.Passport.TestPages
{
	public partial class NoRedirectTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			SignInLogo.ReturnUrl = Request.Url.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.SafeUnescaped);

			if (this.Request.IsAuthenticated)
			{
				ShowPrincipalInfo();
			}
		}

		private void ShowPrincipalInfo()
		{
			HtmlTable table = new HtmlTable();
			table.Attributes["class"] = "table";

			ShowSinglePrincipalInfo(table, "User Logon Name", DeluxeIdentity.CurrentUser.LogOnName);
			ShowSinglePrincipalInfo(table, "User Display Name", DeluxeIdentity.CurrentUser.DisplayName);

			if (DeluxeIdentity.CurrentUser.TopOU != null)
				ShowSinglePrincipalInfo(table, "Top OU",
					string.Format("{0}({1})",
					DeluxeIdentity.CurrentUser.TopOU.DisplayName,
					DeluxeIdentity.CurrentUser.TopOU.FullPath));

			ShowSinglePrincipalInfo(table, "Simulated Time", DateTime.Now.SimulateTime().ToString("yyyy-MM-dd HH:mm:ss"));
			ShowSinglePrincipalInfo(table, "Use Current Time", TimePointContext.Current.UseCurrentTime.ToString());
			ShowSinglePrincipalInfo(table, "TimePoint Simulated Time", TimePointContext.Current.SimulatedTime.ToString("yyyy-MM-dd HH:mm:ss"));

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