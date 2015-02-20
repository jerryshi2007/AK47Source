<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NeedAccessTicketPage.aspx.cs"
	Inherits="MCS.Web.Passport.TestPages.NeedAccessTicketPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>需要票据的页面</title>
	<meta http-equiv="X-UA-Compatible" content="IE=7" />
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<mcs:AccessTicketChecker runat="server" CheckPhase="Load" CheckUrl="True" Enabled="true"
			UrlCheckParts="All" Timeout="00:01:00" OnTicketChecking="Unnamed1_TicketChecking" />
	</div>
	<div>
		<asp:Label runat="server" ID="paramValue"></asp:Label>
	</div>
	<div>
		<asp:Button runat="server" Text="Post" />
	</div>
	</form>
</body>
</html>
