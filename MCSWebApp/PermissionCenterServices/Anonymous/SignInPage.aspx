<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignInPage.aspx.cs" Inherits="PermissionCenter.Anonymous.SignInPage" %>

<%@ Register TagPrefix="MCS" Namespace="MCS.Library.Web.Controls" Assembly="MCS.Library.Passport" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<meta http-equiv="X-UA-Compatible" content="IE=7" />
	<title>用户登录</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div style="text-align: center;">
		<MCS:SignInControl runat="server" TemplatePath="~/Template/SCSignInTemplate.ascx"
			ID="SignInControl"></MCS:SignInControl>
	</div>
	</form>
</body>
</html>
