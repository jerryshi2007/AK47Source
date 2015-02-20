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
using MCS.Library.Passport;

namespace MCS.Web.Passport.TestPages
{
    public partial class PassportManagerTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PassportManager.CheckAuthenticated();

			bool fromCookie = false;
            ITicket ticket = PassportManager.GetTicket(out fromCookie);

            ShowTicketInfo(ticket);
        }

        private void ShowTicketInfo(ITicket ticket)
        {
            HtmlTable table = new HtmlTable();

            ShowSingleTicketInfo(table, "UserID", ticket.SignInInfo.UserID);
            ShowSingleTicketInfo(table, "SignInTime", ticket.SignInInfo.SignInTime.ToString());
            ShowSingleTicketInfo(table, "SignInSessionID", ticket.SignInInfo.SignInSessionID);
            ShowSingleTicketInfo(table, "AuthenticateServer", ticket.SignInInfo.AuthenticateServer);
            ShowSingleTicketInfo(table, "SignInTimeout", ticket.SignInInfo.SignInTimeout.ToString());
			ShowSingleTicketInfo(table, "Windows Integrated", ticket.SignInInfo.WindowsIntegrated.ToString());

            ShowSingleTicketInfo(table, "AppSignInTime", ticket.AppSignInTime.ToString());
            ShowSingleTicketInfo(table, "AppSignInTimeout", ticket.AppSignInTimeout.ToString());

            ShowSingleTicketInfo(table, "AppSignInSessionID", ticket.AppSignInSessionID);
            ShowSingleTicketInfo(table, "AppSignInIP", ticket.AppSignInIP);

            ticketInfo.Controls.Add(table);
        }

        private void ShowSingleTicketInfo(Control parent, string name, string data)
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
