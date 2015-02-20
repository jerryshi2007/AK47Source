<%@ Page Language="c#" Codebehind="UsersInGroup.aspx.cs" AutoEventWireup="True" Inherits="MCS.Applications.AccreditAdmin.dialogs.UsersInGroup" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns:hgui>
<head>
	<title>���Ա�б�</title>
	<meta name="GENERATOR" content="Microsoft Visual Studio 7.1">
	<meta name="CODE_LANGUAGE" content="C#">
	<meta name="vs_defaultClientScript" content="JavaScript">
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	<link href="../CSS/Input.css" type="text/css" rel="stylesheet">

	<script type="text/javascript" language="javascript" src="../oguScript/validate.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/xmlHttp.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/dbGrid.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/uiScript.js"></script>

	<script type="text/javascript" language="javascript" src="../SelfScript/accreditAdmin.js"></script>

	<script type="text/javascript" language="javascript" src="../itemList/oguListCommon.js"></script>

	<script type="text/javascript" language="javascript" src="../Script/ApplicationRoot.js"></script>

	<script type="text/javascript" language="javascript" src="UsersInGroup.js"></script>

</head>
<body ms_positioning="GridLayout" onload="onDocumentLoad()">
	<input type="hidden" id="userPermission" runat="server" name="userPermission"><input
		type="hidden" id="groupGuid" runat="server">
	<input type="hidden" id="txtPageSize" name="txtPageSize" runat="server">
	<hgui:hmenu id="listMenu" style="visibility: hidden; behavior: url(../htc/hMenu.htc);
		position: absolute" onbeforepopup="listMenuBeforePopup();" onmenuclick="listMenuClick()">
			�½��û�...,,,,../images/newUser.gif,newUser;
			ˢ��,,,,../images/refresh.gif,refresh;
			-,,,,,;
			��������...,,,,../images/KeyIcon.gif,resetPassword;
			�����û�����,,,,,initPassword;
			���浱ǰ˳��,,,,../images/sort.gif,sort;
			-,,,,,;
			ɾ��,,,,../images/delete.gif,delete;
			-,,,,,;
			����...,,,,../images/property.gif,property
		</hgui:hmenu>
	<!--
			����...,,,,../images/find.gif,search;
		-->
	<div id="tempDiv" style="display: none">
	</div>
	<xml id="ColumnTitle">
			<ColumnTitle>
				<Column name="" title="���" dataFld="SORT_ID" dataSrc="USER_OU" height="18px" width="4%" />
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
						id="logoGroup">���Ա�б�(<asp:Literal ID="GroupDisplayName" runat="server"></asp:Literal>)</strong></font>
			</td>
		</tr>
		<tr>
			<td style="width: 100%; height: 18px" class="toolbarPanel">
				<table style="width: 100%; height100%" border="0" cellpadding="0" cellspacing="0"
					align="center">
					<tr>
						<td align="left">
							<div class="toolbarPanel" style="width: 100%; height: 18px" onselectstart="return false;"
								onclick="onToolbarClick();" onmouseover="onToolbarMouserOver();" onmouseout="onToolbarMouserOut();">
								<img align="absMiddle" id="newUser" src="../images/newUser.gif" title="�½��û�" class="toolButton">
								<img align="absMiddle" id="delete" src="../images/delete.gif" title="ɾ����ѡ�Ķ��󣨳���ɾ����"
									class="toolButton">
								<img align="absMiddle" id="sort" src="../images/sort.gif" title="���浱ǰ˳��" class="toolButton">
								<span class="toolSplit">|</span>
								<img align="absMiddle" id="refresh" src="../images/refresh.gif" title="ˢ��" class="toolButton">
								<input id="txtSearchObj" name="txtSearchObj" maxlength="32" style="width: 80px">
								<img align="absMiddle" id="search" src="../images/find.gif" title="����..." class="toolButton">
								<span id="saveSpan" class="toolSplit" style="display: none; color: black">���޸�����ʾ˳����Ҫ����</span>
								<img align="absMiddle" id="save" src="../images/save.gif" title="����" class="toolButton"
									style="display: none">
							</div>
						</td>
						<td align="right">
							����<span id="spUserCount"></span> <span><a href="#" onclick="onGotoPage('firstPage')"
								id="firstPage">[��ҳ]</a> <a href="#" onclick="onGotoPage('prePage')" id="prePage">[��ҳ]</a>
								<a href="#" onclick="onGotoPage('nextPage')" id="nextPage">[��ҳ]</a> <a href="#" onclick="onGotoPage('lastPage')"
									id="lastPage">[βҳ]</a> [<span id="spUserPage"></span>] ת����<input id="txtAimPage"
										name="txtAimPage" style="width: 20px" value="1">
								<span style="background-position: left 50%; background-image: url(../images/go.gif);
									width: 18px; background-repeat: no-repeat; height: 18px" onclick="onGotoPage('aimPage')"
									title="��ת" id="aimPage"></span></span>
						</td>
					</tr>
				</table>
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
		<tr valign="middle">
			<td style="height: 50px" align="center" valign="middle">
				<hr>
				<input accesskey="C" style="width: 80px" type="button" value="�ر�(C)" id="btnOK" onclick="window.top.close();"><br>
			</td>
		</tr>
	</table>
</body>
</html>
