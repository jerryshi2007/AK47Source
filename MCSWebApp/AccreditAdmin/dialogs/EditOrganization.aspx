<%@ Page Language="c#" Codebehind="EditOrganization.aspx.cs" AutoEventWireup="True"
	Inherits="MCS.Applications.AccreditAdmin.dialogs.EditOrganization" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns:hgui>
<head>
	<title>组织机构信息</title>
	<meta http-equiv="Content-Type" content="text/html; charset=gb2312">
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<link href="../CSS/Input.css" type="text/css" rel="stylesheet">

	<script type="text/javascript" language="javascript" src="../oguScript/validate.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/dateTime.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/xmlHttp.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/xsdAccess.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/uiScript.js"></script>

	<script type="text/javascript" language="javascript" src="../SelfScript/accreditAdmin.js"></script>

	<script type="text/javascript" language="javascript" src="../Script/ApplicationRoot.js"></script>

	<script type="text/javascript" language="javascript" src="EditOrganization.js"></script>

</head>
<body class="modal" onload="onDocumentLoad();">
	<xml id="ORGANIZATION_XSD" src="../xsd/PageOrganization.xsd"></xml>
	<xml id="AConfig" src="../xml/AccreditConfig.xml"></xml>
	<hgui:calendar id="hCalendar" style="z-index: 101; left: 10px; visibility: hidden;
		behavior: url(../htc/calendar.htc); overflow: hidden; width: 160px; position: absolute;
		top: 15px; height: 200px">
		<strong><font color="#ff0099" size="4"></font></strong>
	</hgui:calendar>
	<table align="center" style="width: 99%; height: 100%">
		<tr>
			<td style="height: 32px">
				<span style="background-position: center center; background-image: url(../images/32/ou.gif);
					width: 32px; background-repeat: no-repeat; height: 32px"></span><font size="4"><strong>
						组织机构信息</strong></font>
				<hr>
			</td>
		</tr>
		<tr>
			<td>
				<form id="frmInput" method="post" runat="server">
					<input type="hidden" id="organizationData" runat="server">
					<input type="hidden" id="parentAllPathName" runat="server">					
					<input type="hidden" id="searchName" datafld="SEARCH_NAME" datasrc="ORGANIZATIONS">
					<input type="hidden" id="opPermission" runat="server" name="opPermission">
					<table class="modalEditable" style="width: 100%; height: 100%" id="orgContentTable">
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>对象名称</strong>:
							</td>
							<td>
								<input type="text" datafld="OBJ_NAME" id="OBJ_NAME" datasrc="ORGANIZATIONS" onchange="changeObjName()"
									style="width: 95%">
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>显示名称</strong>:
							</td>
							<td>
								<input type="text" datafld="DISPLAY_NAME" id="DISPLAY_NAME" datasrc="ORGANIZATIONS"
									style="width: 95%">
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>关区代码:</strong>
							</td>
							<td>
								<input type="text" datafld="CUSTOMS_CODE" id="CUSTOMS_CODE" datasrc="ORGANIZATIONS"
									style="width: 80px">
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>行政级别</strong>:
							</td>
							<td>
								<asp:DropDownList dataFld="RANK_CODE" ID="RANK_CODE" dataSrc="ORGANIZATIONS" runat="server">
								</asp:DropDownList>
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>机构类别:</strong>
							</td>
							<td>
								<asp:DropDownList dataFld="ORG_CLASS" ID="ORG_CLASS" dataSrc="ORGANIZATIONS" runat="server">
								</asp:DropDownList>
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>机构属性</strong>:
							</td>
							<td>
								<asp:DropDownList dataFld="ORG_TYPE" ID="ORG_TYPE" dataSrc="ORGANIZATIONS" runat="server">
								</asp:DropDownList>
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>系统位置</strong>:
							</td>
							<td>
								<input type="text" datafld="ALL_PATH_NAME" id="ALL_PATH_NAME" datasrc="ORGANIZATIONS"
									style="width: 95%" readonly>
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>创建日期</strong>:
							</td>
							<td>
								<input type="text" datafld="CREATE_TIME" id="CREATE_TIME" datasrc="ORGANIZATIONS"
									readonly>
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>备注信息</strong>:
							</td>
							<td>
								<textarea datafld="DESCRIPTION" id="DESCRIPTION" datasrc="ORGANIZATIONS" style="width: 95%;
									height: 100%"></textarea>
							</td>
						</tr>
					</table>
				</form>
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
							<input id="btnOK" type="button" onclick="onSaveData()" accesskey="O" value="确定(O)"
								style="width: 80px"></td>
						<td align="center">
							<input id="btnCancel" style="width: 80px" accesskey="C" onclick="cancelWindow()"
								type="button" value="取消(C)">
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</body>
</html>
