<%@ Page Language="c#" Codebehind="FunctionForRole.aspx.cs" AutoEventWireup="True"
	Inherits="MCS.Applications.AppAdmin.Dialogs.FunctionForRole" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
	<title>FunctionForRole</title>
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<link href="../Css/Input.css" type="text/css" rel="stylesheet">

	<script type="text/javascript" language="javascript" src="../script/validate.js"></script>

	<script type="text/javascript" language="javascript" src="../script/xmlHttp.js"></script>

	<script type="text/javascript" language="javascript" src="../script/xsdAccess.js"></script>

	<script type="text/javascript" language="javascript" src="../script/uiScript.js"></script>

	<script type="text/javascript" language="javascript" src="../script/dbGrid.js"></script>

	<script type="text/javascript" language="javascript" src="../Script/ApplicationRoot.js"></script>

	<script type="text/javascript" language="javascript" src="FunctionForRole.js"></script>

</head>
<body onload="onDocumentLoad()" ms_positioning="GridLayout">
	<form id="Form1" method="post" runat="server">
		<table id="Table1" cellspacing="1" cellpadding="1" width="100%" align="center" border="1">
			<tr>
				<td colspan="4">
					<span id="logoSpan" style="background-position: center center; 
						background-image: url(../images/32/role.gif); width: 32px; background-repeat: no-repeat;
						height: 32px"></span><strong><font size="4">��ɫ-���ܹ�ϵ�༭</font></strong><input id="hdRoleID"
							style="width: 24px; height: 16px" type="hidden" size="1" name="Hidden1" runat="server"><input
								id="hdSupAppCount" style="width: 16px; height: 16px" type="hidden" size="1" name="Hidden1"
								runat="server"><input id="hdRead" style="width: 24px; height: 16px" type="hidden"
									size="1" name="Hidden1" runat="server"></td>
			</tr>
			<tr>
				<td valign="middle" align="right" width="15%">
					<img height="16" alt="" src="../images/role.gif" width="16"></td>
				<td id="tdRoleName" valign="middle" align="left" colspan="3" runat="server">
					��ɫ����</td>
			</tr>
			<tr>
				<td align="center" colspan="4">
					<asp:DataGrid ID="dgFuncList" runat="server" EnableViewState="False" Width="100%"
						AutoGenerateColumns="False">
						<Columns>
							<asp:TemplateColumn>
								<HeaderTemplate>
									<asp:CheckBox ID="chkAll" runat="server" Text="ȫ��"></asp:CheckBox>
								</HeaderTemplate>
								<ItemTemplate>
									<asp:CheckBox ID="chkItem" runat="server"></asp:CheckBox>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:BoundColumn Visible="False" DataField="ID" HeaderText="ID"></asp:BoundColumn>
							<asp:BoundColumn DataField="NAME" HeaderText="����(����)��"></asp:BoundColumn>
							<asp:BoundColumn DataField="CODE_NAME" HeaderText="Ӣ�ı�ʶ"></asp:BoundColumn>
							<asp:BoundColumn DataField="DESCRIPTION" HeaderText="����"></asp:BoundColumn>
							<asp:BoundColumn Visible="False" DataField="RESOURCE_LEVEL" HeaderText="RESOURCE_LEVEL">
							</asp:BoundColumn>
							<asp:BoundColumn Visible="False" DataField="TYPE" HeaderText="TYPE"></asp:BoundColumn>
							<asp:BoundColumn Visible="False" DataField="APP_ID" HeaderText="APP_ID"></asp:BoundColumn>
						</Columns>
					</asp:DataGrid></td>
			</tr>
			<tr>
				<td align="center" colspan="4">
					<input type="button" value="����" id="btnSave" disabled onclick="SaveClick()">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input
						id="btnCancel" type="button" value="ȡ��" onclick="window.close()"></td>
			</tr>
		</table>
	</form>
</body>
</html>
