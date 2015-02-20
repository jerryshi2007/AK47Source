<%@ Page Language="c#" Codebehind="delegationForm.aspx.cs" AutoEventWireup="True"
	Inherits="MCS.Applications.AppAdmin.Dialogs.delegationForm" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns:oa>
<head>
	<title>权限委派</title>
	<link href="../Css/Input.css" type="text/css" rel="stylesheet">
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<?import namespace="hGui" implementation="../htc/calendar.htc" />
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<meta http-equiv="Content-Type" content="text/html; charset=gb2312">

	<script type="text/javascript" language="javascript" src="../script/validate.js"></script>

	<script type="text/javascript" language="javascript" src="../script/xmlHttp.js"></script>

	<script type="text/javascript" language="javascript" src="../script/xsdAccess.js"></script>

	<script type="text/javascript" language="javascript" src="../script/uiScript.js"></script>

	<script type="text/javascript" language="javascript" src="../Script/ApplicationRoot.js"></script>

	<script type="text/javascript" language="javascript" src="delegationForm.js"></script>

</head>
<body class="modal" onload="onDocumentLoad();" ms_positioning="GridLayout">
	<xml id="DF_XSD" src="../xsd/delegations.xsd"></xml>
	<input id="SOURCE_ID" type="hidden" runat="server">
	<input id="SOURCE_LOGON_NAME" type="hidden" runat="server">
	<input id="appID" type="hidden" runat="server">
	<input id="idType" type="hidden" runat="server">
	<input id="roleID" type="hidden" runat="server">
	<input id="funID" type="hidden" runat="server">
	<input id="sourceRootOrg" type="hidden" runat="server">
	<input id="targetRootOrg" type="hidden" runat="server">
	<input id="appCodeName" type="hidden" runat="server">
	<oa:calendar id="hCalendar" style="z-index: 102; left: 10px; visibility: hidden;
		overflow: hidden; width: 160px; position: absolute; top: 15px; height: 200px" />
	<table style="width: 100%; height: 100%">
		<tr>
			<td style="height: 32px">
				<span style="background-position: center center; background-image: url(../images/32/delegation.gif);
					width: 32px; background-repeat: no-repeat; height: 32px"></span><font size="4"><strong>
						权限委派</strong></font>
				<hr>
			</td>
		</tr>
		<tr>
			<td>
				<table class="modalEditable" style="width: 100%; height: 100%">
					<tr id="sourceDnRow" runat="server">
						<td align="center">
							<div style="width: 80%">
								<div align="left">
									<div align="left">
										<strong>委派何人(组织机构)的权限</strong></div>
								</div>
							</div>
						</td>
					</tr>
					<tr style="height: 24px">
						<td align="center">
							<div style="width: 80%">
								<div align="left">
									<strong>请选择应用</strong><select id="appSelect" name="appSelect" onchange="onAppSelectChange();"></select>
								</div>
							</div>
						</td>
						<td>
						</td>
					</tr>
					<tr style="height: 24px">
						<td align="center">
							<div style="width: 80%">
								<div align="left">
									<strong>请选择角色</strong><select id="roleSelect" onchange="onRoleSelectChange();"></select>
								</div>
							</div>
						</td>
					</tr>
					<tr>
						<td align="center">
							<div style="width: 80%">
								<div align="left">
									<div align="left">
										<strong>将<span id="userNameSpan">您</span>的权限委派给何人(组织机构)</strong></div>
									<oa:oguinput id="OGUInput" onchange="changeTargetOGU();" style="behavior: url(/AccreditAdmin/htc/oguInput.htc);
										width: 320px" listmask="2" valuetype="GUID" autogetproperties="true">
										<strong></strong>
									</oa:oguinput>
									<!--<input id="dnInput" style="WIDTH: 320px" onchange="changeTargetID();">
										<span id="targetID" style="BACKGROUND-POSITION: center center; BACKGROUND-IMAGE: url(../Images/organization.gif); WIDTH: 16px; BACKGROUND-REPEAT: no-repeat; HEIGHT: 16px; CURSOR: hand" onclick="onTargetIDClick()">abc</span>
										-->
								</div>
							</div>
						</td>
					</tr>
					<tr style="height: 25%">
						<td align="center">
							<div style="width: 80%">
								<div align="left">
									<div align="left">
										<strong>委派有效期</strong></div>
									<input datafld="START_TIME" id="START_TIME" style="width: 100px" type="text" datatype="dataTime"
										opmode="=">
									到
									<input datafld="END_TIME" id="END_TIME" style="width: 100px" datasrc="DELEGATIONS"
										type="text" datatype="dataTime" opmode="=">
								</div>
							</div>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td style="height: 10px" colspan="2">
				<hr>
			</td>
		</tr>
		<tr>
			<td style="height: 24px" colspan="2">
				<table style="width: 100%; height: 100%" cellspacing="0">
					<tr>
						<td align="center">
							<input id="btnOK" style="width: 80%" disabled accesskey="O" onclick="onSaveClick();"
								type="button" value="保存(O)" name="btnOK">
						</td>
						<td align="center">
							<input id="btnCancel" style="width: 80%" accesskey="C" onclick="window.close();"
								type="button" value="关闭(C)" name="btnCancel">
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</body>
</html>
