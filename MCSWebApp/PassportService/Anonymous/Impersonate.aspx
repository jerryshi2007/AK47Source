<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Impersonate.aspx.cs" Inherits="MCS.Web.Passport.Anonymous.Impersonate" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		请输入用户登录名:
	</div>
	<div>
		<asp:TextBox runat="server" ID="userID" />
		<asp:Button runat="server" ID="" Text="转到..." />
	</div>
	</form>
</body>
</html>
