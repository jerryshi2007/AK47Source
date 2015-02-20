<%@ Page Language="c#" Codebehind="SecretariesOfLeader.aspx.cs" AutoEventWireup="True"
	Inherits="MCS.Applications.AccreditAdmin.dialogs.SecretariesOfLeader" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns:hgui>
<head>
	<title>�쵼�����б�</title>
	<meta name="GENERATOR" content="Microsoft Visual Studio 7.1">
	<meta name="CODE_LANGUAGE" content="C#">
	<meta name="vs_defaultClientScript" content="JavaScript">
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	<link href="../CSS/Input.css" type="text/css" rel="stylesheet">

	<script type="text/javascript" language="javascript" src="../oguScript/validate.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/xmlHttp.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/dbGrid.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/uiScript.js"></script>

	<script type="text/javascript" language="javascript" src="../selfScript/accreditAdmin.js"></script>

	<script type="text/javascript" language="javascript" src="../itemList/oguListCommon.js"></script>

	<script type="text/javascript" language="javascript" src="../Script/ApplicationRoot.js"></script>

	<script type="text/javascript" language="javascript" src="SecretariesOfLeader.js"></script>

</head>
<body ms_positioning="GridLayout" onload="onDocumentLoad()">
	<input type="hidden" id="userPermission" runat="server">
	<input type="hidden" id="LeaderGuid" runat="server">
	<hgui:hmenu id="listMenu" style="visibility: hidden; behavior: url(../htc/hMenu.htc);
		position: absolute" onbeforepopup="listMenuBeforePopup();" onmenuclick="listMenuClick()">
			�½�����...,,,,../images/newUser.gif,newUser;
			ˢ��,,,,../images/refresh.gif,refresh;
			����...,,,,../images/find.gif,search;
			�����û�����,,,,,initPassword;
			-,,,,,;
			ɾ��,,,,../images/delete.gif,delete;
			-,,,,,;
			����...,,,,../images/property.gif,property
		</hgui:hmenu>
	<div id="tempDiv" style="display: none">
	</div>
	<xml id="ColumnTitle">
			<ColumnTitle>
				<Column name="" title="���" dataFld="SORT_ID" dataSrc="USER_OU" height="18px" width="3%" />
				<Column name="����" dataFld="DISPLAY_NAME" dataSrc="USER_OU" width="16%" />
				<Column name="ְ��/����" dataFld="RankName" dataSrc="USER_OU" width="16%" />
				<Column name="ϵͳλ��" dataFld="ALL_PATH_NAME" dataSrc="USER_OU" width="40%" />
				<Column name="����" dataFld="DESCRIPTION" dataSrc="USER_OU" />
			</ColumnTitle>
		</xml>
	<xml id="ADSearchConfig" src="../xml/ADSearchConfig.xml"></xml>
	<table style="width: 100%; height: 100%">
		<tr valign="top">
			<td style="height: 32px">
				<span style="background-position: center center; background-image: url(../images/32/group.gif);
					width: 32px; background-repeat: no-repeat; height: 32px"></span><font size="4"><strong
						id="logoGroup">�쵼�����б�(<asp:Literal ID="LeaderDisplayName" runat="server"></asp:Literal>)</strong></font>
			</td>
		</tr>
		<tr>
			<td style="width: 100%; height: 18px">
				<div class="toolbarPanel" style="width: 100%; height: 18px" onselectstart="return false;"
					onclick="onToolbarClick();" onmouseover="onToolbarMouserOver();" onmouseout="onToolbarMouserOut();">
					<img align="absMiddle" id="newUser" src="../images/newUser.gif" title="�½��û�" class="toolButton">
					<img align="absMiddle" id="refresh" src="../images/refresh.gif" title="ˢ��" class="toolButton">
					<img align="absMiddle" id="search" src="../images/find.gif" title="����..." class="toolButton">
					<span class="toolSplit">|</span>
					<img align="absMiddle" id="delete" src="../images/delete.gif" title="ɾ����ѡ�Ķ��󣨳���ɾ����"
						class="toolButton">
				</div>
			</td>
		</tr>
		<tr valign="top" style="height: 100%">
			<td align="center">
				<div style="overflow: auto; width: 100%; height: 100%">
					<table id="ouUserTable" style="width: 100%">
						<thead id="ouUserHeader">
						</thead>
						<tbody id="ouUserListBody" oncontextmenu="oBodyContextMenu();">
						</tbody>
					</table>
				</div>
			</td>
		</tr>
		<tr valign="bottom">
			<td style="height: 50px" align="center" valign="middle">
				<hr>
				<input accesskey="C" style="width: 80px" type="button" value="�ر�(C)" id="btnOK" onclick="window.top.close();"><br>
			</td>
			</TD>
		</tr>
	</table>
</body>
</html>
