<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OpenIDIdentityTest.aspx.cs"
	Inherits="ResponsivePassportService.TestPages.OpenIDIdentityTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>显示OpenID的信息</title>
	<link href="../Resources/Bootstrap/css/bootstrap.css" rel="stylesheet" type="text/css" />
</head>
<body>
	<form id="serverForm" runat="server">
	<dl runat="server" id="openIDInfo" class="dl-horizontal">
		<dt>OpenID</dt>
		<dd runat="server" id="openIDContainer">
		</dd>
		<dt>User ID</dt>
		<dd runat="server" id="userIDContainer">
		</dd>
		<dt>无参数链接</dt>
		<dd>
			<a href="OpenIDIdentityTest.aspx">OpenIDIdentityTest.aspx</a></dd>
	</dl>
	</form>
</body>
</html>
