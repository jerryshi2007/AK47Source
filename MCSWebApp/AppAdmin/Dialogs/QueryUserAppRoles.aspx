<%@ Page Language="c#" Codebehind="QueryUserAppRoles.aspx.cs" AutoEventWireup="True"
	Inherits="MCS.Applications.AppAdmin.Dialogs.QueryUserAppRoles" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns:oa>
<head>
	<title>用户应用角色查询</title>
	<link href="../Css/Input.css" type="text/css" rel="stylesheet">
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">

	<script type="text/javascript" language="javascript" src="../script/validate.js"></script>

	<script type="text/javascript" language="javascript" src="../script/xmlHttp.js"></script>

	<script type="text/javascript" language="javascript" src="../script/xsdAccess.js"></script>

	<script type="text/javascript" language="javascript" src="../script/uiScript.js"></script>

	<script type="text/javascript" language="javascript" src="../script/dbGrid.js"></script>

	<script type="text/javascript" language="javascript" src="../script/appCommon.js"></script>

	<script type="text/javascript" language="javascript" src="../Script/ApplicationRoot.js"></script>

	<script type="text/javascript" language="javascript" src="QueryUserAppRoles.js"></script>

</head>
<body ms_positioning="GridLayout">
	<form id="Form1" method="post" target="_self" runat="server">
		<table id="Table1" cellspacing="2" cellpadding="2" width="100%" border="1">
			<tr>
				<td colspan="2">
					<input id="hdUserGuid" type="hidden" name="hdUserGuid" runat="server"><input id="hdAllPathName"
						type="hidden" runat="server"><input id="hdConfig" type="hidden" runat="server"></td>
			</tr>
			<tr>
				<td align="center" colspan="2">
					<strong><font size="3">用户应用角色查询
						<div id="lbAppSocpe" style="display: inline; height: 15px" runat="server" ms_positioning="FlowLayout">
						</div>
					</font></strong>
				</td>
			</tr>
			<tr>
				<td align="right" width="20%">
					<font size="3">请选择用户：</font></td>
				<td width="80%">
					<input id="txtUser" style="font-size: larger; width: 60%" type="text"><input style="font-size: larger"
						onclick="getUser();" type="button" value="选择"></td>
			</tr>
			<tr>
				<td align="center" colspan="2">
					<input style="font-size: larger;" accesskey="btnQuery" onclick="preSubmit();"
						type="submit" value="进行查询" name="btnQuery"></td>
			</tr>
			<tr>
				<td align="center" colspan="2">
					<hr width="100%" size="1">
				</td>
			</tr>
			<tr>
				<td align="center" colspan="2">
					<div id="lbTitle" style="display: inline; font-size: larger; width: 100%; height: 15px"
						runat="server" ms_positioning="FlowLayout">
					</div>
				</td>
			</tr>
			<tr>
				<td colspan="2">
					<table id="appRolesTable" bordercolordark="#000066" width="100%" align="center" bordercolorlight="#000000"
						border="1" runat="server">
						<tr>
							<td>
								<strong><font size="3">应用代码 </font></strong>
							</td>
							<td>
								<strong><font size="3">应用名称 </font></strong>
							</td>
							<td>
								<strong><font size="3">角色代码 </font></strong>
							</td>
							<td>
								<strong><font size="3">角色名称 </font></strong>
							</td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td colspan="2">
					<hr width="100%" size="1">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>
