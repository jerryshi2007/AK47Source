<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NoRedirectTest.aspx.cs"
	Inherits="MCS.Web.Passport.TestPages.NoRedirectTest" %>

<%@ Register TagPrefix="CCPC" Namespace="MCS.Library.Web.Controls" Assembly="MCS.Library.Passport" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>不自动跳转到认证页面的测试</title>
	<style type="text/css">
		.captionCell
		{
			text-align: right;
			font-weight: bold;
		}
		.table
		{
			border: 1px solid gray;
		}
	</style>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<CCPC:SignInLogoControl runat="server" ID="SignInLogo" AutoRedirect="false" LogOffAll="true">
		</CCPC:SignInLogoControl>
	</div>
	<div runat="server" id="principalInfo">
	</div>
	<div>
		Simulated Time:
		<mcs:TimePointDisplayControl ID="TimePointDisplayControl1" runat="server" />
	</div>
	</form>
</body>
</html>
