<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="roleMatrixDialogTest.aspx.cs"
	Inherits="WorkflowDesigner.MatrixModalDialog.Test.roleMatrixDialogTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>测试矩阵弹出对话框</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<div>
			请点击编辑Schema属性...</div>
		<SOA:RoleMatrixEntryControl ID="schemaMatrixEntryControl" runat="server" Visible="true"
			ReadOnly="false" Enabled="true" EnableAccessTicket="true" />
	</div>
	<div>
		<div>
			请点击编辑Instance属性...</div>
		<SOA:RoleMatrixEntryControl ID="instanceMatrixEntryControl" runat="server" Visible="true"
			ReadOnly="false" Enabled="true" EnableAccessTicket="true" />
	</div>
	<div>
		<div>
			请点击查看Instance属性...</div>
		<SOA:RoleMatrixEntryControl ID="instanceReadOnlyMatrixEntryControl" runat="server"
			Visible="true" ReadOnly="false" Enabled="true" EnableAccessTicket="true" />
	</div>
	<div>
		<div>
			请点击查看空定义和空矩阵的属性...</div>
		<SOA:RoleMatrixEntryControl ID="emptyMatrixEntryControl" runat="server"
			Visible="true" ReadOnly="false" Enabled="true" EnableAccessTicket="false" />
	</div>
	</form>
</body>
</html>
