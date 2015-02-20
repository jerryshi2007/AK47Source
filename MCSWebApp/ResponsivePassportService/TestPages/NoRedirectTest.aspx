<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NoRedirectTest.aspx.cs"
	Inherits="ResponsivePassportService.TestPages.NoRedirectTest" %>

<!DOCTYPE>
<html>
<head runat="server">
	<title>不自动跳转到认证页面的测试</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<mcs:SignInLogoControl ID="signInLogo" runat="server" AutoRedirect="false" />
	</div>
	<dl runat="server" id="principalInfo" class="dl-horizontal">
	</dl>
	<div class="row">
		<div class="form-horizontal" runat="server" id="bindingsContainer">
		</div>
	</div>
	</form>
</body>
</html>
