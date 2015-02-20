<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignInPage.aspx.cs" Inherits="MCS.Web.Passport.SignInPage" %>

<%@ Register TagPrefix="MCS" Namespace="MCS.Library.Web.Controls" Assembly="MCS.Library.Passport" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<meta http-equiv="X-UA-Compatible" content="IE=7" />
	<title>用户登录</title>
	<style type="text/css">
<%--		body
		{
			overflow: hidden;
			/*background-color: #83bcda;*/
			margin: 0;
		}--%>
	</style>
</head>
<body>
	<form id="serverForm" runat="server">
	<div style="text-align: center;">
		<MCS:SignInControl runat="server" TemplatePath="../Template/DefaultSignInTemplate.ascx"
			ID="SignInControl" onbeforeauthenticate="SignInControl_BeforeAuthenticate"></MCS:SignInControl>
	</div>
	</form>
</body>
</html>
