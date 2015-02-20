using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using MCS.Library.Passport;
using System.Text;

namespace MCS.Web.Passport
{
	internal static class Helper
	{
		public static void ShowTicketInfo(ITicket ticket, Control container)
		{
			HtmlTable table = new HtmlTable();

			ShowSingleTicketInfo(table, "UserID", ticket.SignInInfo.UserID);
			ShowSingleTicketInfo(table, "Domain", ticket.SignInInfo.Domain);
			ShowSingleTicketInfo(table, "SignInTime", ticket.SignInInfo.SignInTime.ToString());
			ShowSingleTicketInfo(table, "SignInSessionID", ticket.SignInInfo.SignInSessionID);
			ShowSingleTicketInfo(table, "AuthenticateServer", ticket.SignInInfo.AuthenticateServer);
			ShowSingleTicketInfo(table, "SignInTimeout", ticket.SignInInfo.SignInTimeout.ToString());

			ShowSingleTicketInfo(table, "AppSignInTime", ticket.AppSignInTime.ToString());
			ShowSingleTicketInfo(table, "AppSignInTimeout", ticket.AppSignInTimeout.ToString());

			ShowSingleTicketInfo(table, "AppSignInSessionID", ticket.AppSignInSessionID);
			ShowSingleTicketInfo(table, "AppSignInIP", ticket.AppSignInIP);

			container.Controls.Add(table);
		}

		private static void ShowSingleTicketInfo(Control parent, string name, string data)
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
