<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExceptionTest.aspx.cs"
	Inherits="MCS.Web.Responsive.WebControls.Test.ModuleTest.ExceptionTest" %>

<!DOCTYPE html>
<html>
<head runat="server">
	<meta name="viewport" content="width=device-width, initial-scale=1.0" />
	<title>异常页面的测试</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<asp:ScriptManager runat="server" ID="scriptManager" EnableScriptGlobalization="true">
		</asp:ScriptManager>
	</div>
	<div>
		<asp:Button runat="server" ID="throwException" Text="Throw Exception" OnClick="throwException_Click" />
	</div>
	</form>
</body>
</html>
