<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs"
	Inherits="MCS.Web.Passport.Forms.ChangePassword" %>

<%@ Register TagPrefix="MCS" Namespace="MCS.Library.Web.Controls" Assembly="MCS.Library.Passport" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>修改密码</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<div>
			<MCS:ChangePasswordControl runat="server" TemplatePath="../Template/DefaultChangePasswordTemplate.ascx"
				ID="changePasswordControl"></MCS:ChangePasswordControl>
		</div>
	</div>
	</form>
</body>
</html>
