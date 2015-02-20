<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecryptTicketTest.aspx.cs"
	Inherits="MCS.Web.Passport.TestPages.DecryptTicketTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>票据解密服务的测试</title>
	<script type="text/javascript">
		var serviceUrl = "../Services/TicketServiceHandler.ashx?method=DecryptTicket";

		function onGetDecryptedTicket() {
			var url = serviceUrl + "&encodedTicket=" + encodeURIComponent(document.getElementById("ticketString").innerText);

			document.getElementById("resultFrame").src = url;
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		票据
	</div>
	<div>
		<asp:TextBox runat="server" ID="ticketString" TextMode="MultiLine" Columns="80" Rows="25"></asp:TextBox>
	</div>
	<div>
		<input id="getDecryptedTicketBtn" type="button" onclick="onGetDecryptedTicket();"
			value="使用HttpGet解密Ticket" />
	</div>
	<div>
		<iframe id="resultFrame" name="resultFrame" style="width: 400px; height: 300px">
		</iframe>
	</div>
	</form>
</body>
</html>
