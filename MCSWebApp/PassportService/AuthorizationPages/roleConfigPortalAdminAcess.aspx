<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="roleConfigPortalAdminAcess.aspx.cs" Inherits="MCS.Web.Passport.AuthorizationPages.roleConfigPortalAdminAcess" %>
<%@ Register TagPrefix="CCPC" Namespace="MCS.Library.Web.Controls" Assembly="MCS.Library.Passport" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
	<style type="text/css">
        .captionCell { text-align: right; font-weight: bold; }
        .table { border: 1px solid gray}
    </style>
</head>
<body>
    <form id="serverForm" runat="server">
	<div>
		<CCPC:SignInLogoControl runat="server" ID="SignInLogo" AutoRedirect="true"></CCPC:SignInLogoControl>
	</div>
	<div runat="server" id="principalInfo">
	</div>
	</form>
</body>
</html>
