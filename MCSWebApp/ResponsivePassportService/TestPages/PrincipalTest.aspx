<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrincipalTest.aspx.cs"
	Inherits="ResponsivePassportService.TestPages.PrincipalTest" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Principal测试</title>
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
	<div runat="server" class="row" id="principalOP" visible="false">
		<div class="form-horizontal" runat="server">
			<div class="col-md-12">
				<asp:Button runat="server" CssClass="btn btn-primary" ID="clearPrincipal" Text="清除Principal缓存"
					OnClick="clearPrincipal_Click" />
			</div>
		</div>
	</div>
	<div>
		<a href="NoRedirectTest.aspx">跳转到NoRedirectTest.aspx</a>
	</div>
	</form>
</body>
</html>
