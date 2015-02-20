<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WISignInPage.aspx.cs" Inherits="MCS.Web.Passport.Integration.WISignInPage" %>

<%@ Register TagPrefix="CCPC" Namespace="MCS.Library.Web.Controls" Assembly="MCS.Library.Passport" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>用户登录</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<div>
			<CCPC:SignInControl runat="server" OnInitSignInControl="SignInControl_InitSignInControl"
				ID="SignInControl"></CCPC:SignInControl>
		</div>
	</div>
	</form>
</body>
</html>
