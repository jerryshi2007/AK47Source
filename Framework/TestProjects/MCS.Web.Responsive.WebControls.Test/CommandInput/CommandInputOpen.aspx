<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CommandInputOpen.aspx.cs"
	Inherits="MCS.Web.Responsive.WebControls.Test.CommandInput.CommandInputOpen" %>

<!DOCTYPE html>
<html>
<head runat="server">
	<title>弹出窗口，用于测试CommandInput</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<asp:Button ID="refreshParentWindow" runat="server" Text="刷新父窗口" 
			onclick="refreshParentWindow_Click" />
		<asp:Button ID="closeWindow" runat="server" Text="关闭窗口" 
			onclick="closeWindow_Click" />
	</div>
	</form>
</body>
</html>
