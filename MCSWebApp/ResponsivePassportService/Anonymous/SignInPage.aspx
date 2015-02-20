<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignInPage.aspx.cs" Inherits="ResponsivePassportService.Anonymous.SignInPage" %>

<!DOCTYPE html>
<html lang="zh-cn">
<head id="Head1" runat="server">
	<meta http-equiv="X-UA-Compatible" content="IE=edge" />
	<meta name="viewport" content="width=device-width, initial-scale=1.0" />
	<title>登录</title>
	<script type="text/javascript" src="../Resources/Jquery/jquery-2.0.3.js"></script>
</head>
<body>
	<form id="serverForm" runat="server" role="form">
	<div>
		<mcs:SignInControl runat="server" ID="signInControl" 
			TemplatePath="../Template/ResponsiveSignInTemplate.ascx" 
			onbeforeauthenticate="signInControl_BeforeAuthenticate" />
	</div>
	</form>
</body>
</html>
