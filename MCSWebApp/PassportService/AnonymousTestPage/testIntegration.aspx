<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="testIntegration.aspx.cs"
	Inherits="MCS.Web.Passport.TestPages.testIntegration" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>测试集成认证</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<div runat="server" id="ticketInfo">
		</div>
		<div>
			<textarea id="ticketXml" runat="server" style="width: 400px; height: 300px">
				<cols>
				</cols>
			</textarea>
		</div>
	</div>
	</form>
</body>
</html>
