<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CallServiceTest.aspx.cs"
	Inherits="WfFormTemplate.Services.CallServiceTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>服务调用测试</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<asp:Button runat="server" ID="buttonGetServiceName" Text="调用服务器时间" OnClick="buttonGetServiceName_Click" />
		<asp:Button runat="server" ID="buttonCallBranchProcess" Text="调用分支流程的服务" 
			onclick="buttonCallBranchProcess_Click" />
	</div>
	<div>
		<asp:Label runat="server" ID="resultLabel"></asp:Label>
	</div>
	</form>
</body>
</html>
