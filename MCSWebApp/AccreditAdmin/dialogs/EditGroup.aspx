<%@ Page Language="c#" Codebehind="EditGroup.aspx.cs" AutoEventWireup="True" Inherits="MCS.Applications.AccreditAdmin.dialogs.EditGroup" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns:hgui>
<head>
	<title>人员组信息</title>
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<link href="../CSS/Input.css" type="text/css" rel="stylesheet">

	<script type="text/javascript" language="javascript" src="../oguScript/validate.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/xmlHttp.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/xsdAccess.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/dateTime.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/uiScript.js"></script>

	<script type="text/javascript" language="javascript" src="../SelfScript/accreditAdmin.js"></script>

	<script type="text/javascript" language="javascript" src="../Script/ApplicationRoot.js"></script>

	<script type="text/javascript" language="javascript" src="EditGroup.js"></script>

</head>
<body onload="onDocumentLoad();" class="modal">
	<xml id="GROUP_XSD" src="../xsd/PageGroup.xsd"></xml>
	<xml id="AConfig" src="../xml/AccreditConfig.xml"></xml>
	<hgui:calendar id="hCalendar" style="z-index: 101; left: 10px; visibility: hidden;
		behavior: url(../htc/calendar.htc); overflow: hidden; width: 160px; position: absolute;
		top: 15px; height: 200px">
		<strong><font color="#ff0099" size="4"></font></strong>
	</hgui:calendar>
	<table id="elemTable" style="width: 99%; height: 100%" align="center">
		<tr>
			<td style="height: 32px">
				<span style="background-position: center center; background-image: url(../images/32/group.gif);
					width: 32px; background-repeat: no-repeat; height: 32px"></span><font size="4"><strong>
						人员组信息</strong></font>
				<hr>
			</td>
		</tr>
		<tr>
			<td>
				<form id="frmInput" method="get">
					<input type="hidden" id="GroupData" runat="server" name="GroupData">
					<input type="hidden" id="parentAllPathName" runat="server" name="parentAllPathName">
					<input type="hidden" id="searchName" datafld="SEARCH_NAME" datasrc="GROUPS">
					<input type="hidden" id="opPermission" runat="server" name="opPermission">
					<table class="modalEditable" style="width: 100%; height: 100%" id="groupContentTable">
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>对象名称</strong>:
							</td>
							<td>
								<input datafld="OBJ_NAME" id="OBJ_NAME" style="width: 95%" datasrc="GROUPS" type="text"
									onchange="changeObjName()" name="OBJ_NAME">
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>显示名称</strong>:
							</td>
							<td>
								<input datafld="DISPLAY_NAME" id="DISPLAY_NAME" style="width: 95%" datasrc="GROUPS"
									type="text" name="DISPLAY_NAME">
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>系统位置</strong>:
							</td>
							<td>
								<input datafld="ALL_PATH_NAME" id="ALL_PATH_NAME" style="width: 95%" datasrc="GROUPS"
									readonly type="text" name="ALL_PATH_NAME">
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>创建日期</strong>:
							</td>
							<td>
								<input datafld="CREATE_TIME" id="CREATE_TIME" datasrc="GROUPS" readonly type="text"
									name="CREATE_TIME">
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>备注信息</strong>:
							</td>
							<td>
								<textarea datafld="DESCRIPTION" id="DESCRIPTION" style="width: 95%; height: 100%"
									datasrc="GROUPS" name="DESCRIPTION"></textarea>
							</td>
						</tr>
					</table>
				</form>
			</td>
		</tr>
		<tr>
			<td style="height: 2%">
				<hr>
			</td>
		</tr>
		<tr>
			<td style="height: 10%">
				<table style="width: 100%; height: 100%" cellspacing="0" cellpadding="0">
					<tr>
						<td align="center">
							<input id="btnOK" style="width: 80px" accesskey="O" onclick="onSaveData()" type="button"
								value="确定(O)">
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
