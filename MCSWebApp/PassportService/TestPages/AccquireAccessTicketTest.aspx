<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccquireAccessTicketTest.aspx.cs"
	Inherits="MCS.Web.Passport.TestPages.AccquireAccessTicketTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>申请访问票据的页面</title>
	<meta http-equiv="X-UA-Compatible" content="IE=7" />
	<script type="text/javascript">
		function onAccquiredAccessTicket(source) {
			window.open(source.href);
		}

		function onClientSetHrefClick() {
			event.returnValue = false;

			var ticketAnchor = document.getElementById("anchor2");

			ticketAnchor.href = "NeedAccessTicketPage.aspx?uid=沈天宇";

			ticketAnchor.click();

			return false;
		}

		function onRootRelativeClick() {
			event.returnValue = false;

			var ticketAnchor = document.getElementById("anchor3");

			ticketAnchor.href = "/MCSWebApp/PassportService/TestPages/NeedAccessTicketPage.aspx?uid=沈奇奇";

			ticketAnchor.click();

			return false;
		}

		function onBackRelativeClick() {
			event.returnValue = false;

			var ticketAnchor = document.getElementById("anchor4");

			ticketAnchor.href = "../TestPages/NeedAccessTicketPage.aspx?uid=沈峥的儿子";

			ticketAnchor.click();

			return false;
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<mcs:AccessTicketHtmlAnchor runat="server" href="NeedAccessTicketPage.aspx?uid=沈峥"
			ID="anchor" OnClientAccquiredAccessTicket="onAccquiredAccessTicket">
			Please click the first url...
		</mcs:AccessTicketHtmlAnchor>
	</div>
	<div>
		<mcs:AccessTicketHtmlAnchor runat="server" href="NeedAccessTicketPage.aspx?uid=沈嵘"
			ID="anchor1" OnClientAccquiredAccessTicket="onAccquiredAccessTicket">
			Please click the second url...
		</mcs:AccessTicketHtmlAnchor>
	</div>
	<div>
		<a href="#" id="clientSetHref" onclick="return onClientSetHrefClick();">Please click
			the relative url... </a>
		<mcs:AccessTicketHtmlAnchor runat="server" href="#" style="display: none" ID="anchor2"
			OnClientAccquiredAccessTicket="onAccquiredAccessTicket" AutoMakeAbsolute="true">
		</mcs:AccessTicketHtmlAnchor>
	</div>
	<div>
		<a href="#" id="rootRelative" onclick="return onRootRelativeClick();">Please click the
			root relative url... </a>
		<mcs:AccessTicketHtmlAnchor runat="server" href="#" style="display: none" ID="anchor3"
			OnClientAccquiredAccessTicket="onAccquiredAccessTicket" AutoMakeAbsolute="true">
		</mcs:AccessTicketHtmlAnchor>
	</div>
	<div>
		<a href="#" id="backRelative" onclick="return onBackRelativeClick();">Please click the
			back relative url... </a>
		<mcs:AccessTicketHtmlAnchor runat="server" href="#" style="display: none" ID="anchor4"
			OnClientAccquiredAccessTicket="onAccquiredAccessTicket" AutoMakeAbsolute="true">
		</mcs:AccessTicketHtmlAnchor>
	</div>
	</form>
</body>
</html>
