<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="MCS.Web.Passport.AuthorizationPages._default" %>

<%@ Register TagPrefix="CCPC" Namespace="MCS.Library.Web.Controls" Assembly="MCS.Library.Passport" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<CCPC:SignInLogoControl runat="server" ID="SignInLogo" AutoRedirect="true"></CCPC:SignInLogoControl>
	</div>
	<div>
		<ol>
			<li><a href="roleConfigPortalAdminAcess.aspx?processKey=Hello">简单角色检查</a></li>
			<li><a href="roleConfigPortalAdminAcess.aspx?processAdmin">RolesDefineConfig角色检查</a></li>
		</ol>
	</div>
	</form>
</body>
</html>
