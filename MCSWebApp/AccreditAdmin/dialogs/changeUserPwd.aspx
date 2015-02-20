<%@ Page Language="c#" Codebehind="changeUserPwd.aspx.cs" AutoEventWireup="True"
	Inherits="MCS.Applications.AccreditAdmin.dialogs.changeUserPwd" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
	<title>修改密码</title>
	<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
	<meta name="CODE_LANGUAGE" content="C#">
	<meta name="vs_defaultClientScript" content="JavaScript">
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	<meta http-equiv="Content-Type" content="text/html; charset=gb2312">
	<link href="../CSS/Input.css" type="text/css" rel="stylesheet">

	<script type="text/javascript" language="javascript" src="../oguScript/validate.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/xmlHttp.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/xsdAccess.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/uiScript.js"></script>

	<script type="text/javascript" language="javascript" src="../selfScript/accreditAdmin.js"></script>

	<script type="text/javascript" language="javascript" src="../Script/ApplicationRoot.js"></script>

	<script type="text/javascript" language="javascript" src="changeUserPwd.js"></script>

</head>
<body class="modal" onload="onDocumentLoad()">
	<table style="width: 100%; height: 100%">
		<tr>
			<td style="height: 32px">
				<span style="background-position: center center; background-image: url(../images/32/user.gif);
					width: 32px; background-repeat: no-repeat; height: 32px"></span><font size="4"><strong>
						修改（<strong id="userName" runat="server"></strong>）密码</strong></font>
				<hr>
			</td>
		</tr>
		<tr>
			<td>
				<form id="frmInput" runat="server">
					<input type="hidden" id="UserGuid" name="UserGuid" runat="server">
					<table class="modalEditable" style="width: 100%; height: 100%">
						<tr>
							<td style="width: 80px" align="right">
								<strong>原密码</strong>:
							</td>
							<td>
								<input type="password" id="oldPwd" style="width: 100%">
							</td>
						</tr>
						<tr style="display: none">
							<td style="width: 80px" align="right">
								<strong>旧加密算法</strong>:
							</td>
							<td>
								<asp:DropDownList ID="oldPwdType" runat="server">
								</asp:DropDownList>
							</td>
						</tr>
						<tr>
							<td style="width: 80px" align="right">
								<strong>新密码</strong>:
							</td>
							<td>
								<input type="password" id="userPassword" style="width: 100%">
							</td>
						</tr>
						<tr>
							<td style="width: 80px" align="right">
								<strong>确认密码</strong>:
							</td>
							<td>
								<input type="password" id="pwdRetype" style="width: 100%">
							</td>
						</tr>
						<tr style="display: none">
							<td style="width: 80px" align="right">
								<strong>新加密算法</strong>:
							</td>
							<td>
								<asp:DropDownList ID="newPwdType" runat="server">
								</asp:DropDownList>
							</td>
						</tr>
					</table>
				</form>
			</td>
		</tr>
		<tr>
			<td style="height: 10px">
				<hr>
			</td>
		</tr>
		<tr>
			<td style="height: 24px">
				<table style="width: 100%; height: 100%" cellspacing="0">
					<tr>
						<td align="center">
							<input accesskey="O" style="width: 80px" type="button" value="确定(O)" id="btnOK" onclick="onSaveClick();">
						</td>
						<td align="center">
							<input accesskey="C" style="width: 80px" type="button" value="取消(C)" id="btnCancel"
								onclick="window.close();">
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</body>
</html>
