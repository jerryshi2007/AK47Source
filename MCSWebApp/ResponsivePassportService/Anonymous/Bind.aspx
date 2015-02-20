<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Bind.aspx.cs" Inherits="ResponsivePassportService.Anonymous.Bind" %>

<!DOCTYPE>
<html>
<head runat="server">
	<title>用户绑定</title>
	<link href="../Resources/Bootstrap/css/bootstrap.css" rel="stylesheet" type="text/css" />
	<link href="../Resources/Styles/layout.css" rel="stylesheet" type="text/css" />
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<mcs:SignInControl runat="server" ID="signInControl" 
			TemplatePath="../Template/ResponsiveBindingTemplate.ascx" 
			onaftersignin="signInControl_AfterSignIn" 
			oninitsignincontrol="signInControl_InitSignInControl" />
	</div>
	</form>
</body>
</html>
