<%@ Page Language="C#" AutoEventWireup="true" Codebehind="PassportLogon.aspx.cs"
	Inherits="MCS.Applications.AppAdmin.PassportLogon" %>

<%@ Register Assembly="MCS.Library.Passport" Namespace="MCS.Library.Web.Controls"
	TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>通用授权平台登录</title>
	<link rel="Shortcut Icon" href="./images/icon/key.ico">
</head>
<body>
	<form id="form1" runat="server">
		<div>
			<cc1:SignInControl ID="SignInControl1" TemplatePath="./Template/PassportLogonUC.ascx"
				runat="server" />
		</div>
	</form>
</body>
</html>
