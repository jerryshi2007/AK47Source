<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DesKeyGenerator.aspx.cs"
	Inherits="MCS.Web.Passport.TestPages.DesKeyGenerator" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Generate DES key</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<asp:Button ID="generateButton" runat="server" Text="Button" OnClick="generateButton_Click" />
	</div>
	<div>
		Key
	</div>
	<div id="keyValue" runat="server">
	</div>
	<div>
		IV
	</div>
	<div id="IVValue" runat="server">
	</div>
	</form>
</body>
</html>
