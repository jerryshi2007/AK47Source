<%@ Page Language="c#" Codebehind="OGUList.aspx.cs" AutoEventWireup="True" Inherits="MCS.Applications.AccreditAdmin.itemList.OGUList" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns:hgui>
<head>
	<title>��֯��Ԫ����Ա�б�</title>
	<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
	<meta name="CODE_LANGUAGE" content="C#">
	<meta name="vs_defaultClientScript" content="JavaScript">
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	<meta http-equiv="Content-Type" content="text/html; charset=gb2312">

	<script type="text/javascript" language="javascript" src="../oguScript/validate.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/xmlHttp.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/dbGrid.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/uiScript.js"></script>

	<script type="text/javascript" language="javascript" src="../SelfScript/organizeTree.js"></script>

	<script type="text/javascript" language="javascript" src="../Script/ApplicationRoot.js"></script>

	<script type="text/javascript" language="javascript" src="../SelfScript/accreditAdmin.js"></script>

	<script type="text/javascript" language="javascript" src="oguListCommon.js"></script>

	<script type="text/javascript" language="javascript" src="OGUList.js"></script>

	<link type="text/css" rel="stylesheet" href="../CSS/Input.css">
</head>
<body onload="onDocumentLoad()" onselectstart="onDocumentSelectStart()">	
	<input type="hidden" id="userPermission" runat="server" name="userPermission">
	<hgui:hmenu id="listMenu" style="visibility: hidden; behavior: url(../htc/hMenu.htc);
		position: absolute" onbeforepopup="listMenuBeforePopup();" onmenuclick="listMenuClick()">
	</hgui:hmenu>
	<!--
			-;
			��������...,,,,../images/mailbox.gif,mailbox;
		-->
	<div id="tempDiv" style="display: none">
	</div>
	<xml id="ADSearchConfig" src="../xml/ADSearchConfig.xml"></xml>
	<input type="hidden" id="inputParam" onpropertychange="onInputParamChange();">
	<xml id="ColumnTitleOU">
			<ColumnTitle>
				<Column name="" title="���" dataFld="SORT_ID" dataSrc="USER_OU" width="18px" height="20px" />
				<Column name="" title="�Ƿ��Ѿ���������" dataFld="E_MAIL" dataSrc="USER_OU" width="18px" image="../images/mailbox.gif" />
				<Column name="����" dataFld="DISPLAY_NAME" height="18px" dataSrc="USER_OU" width="20%" />
				<Column name="ְ��/����" dataFld="RankName" dataSrc="USER_OU" width="30%" />
				<Column name="����ʱ��" dataFld="END_TIME" dataSrc="USER_OU" width="18%" />
				<Column name="��ע" dataFld="DESCRIPTION" dataSrc="USER_OU" />
			</ColumnTitle>
		</xml>
	<div class="toolbarPanel" style="width: 100%; height: 18px" onselectstart="return false;"
		onclick="onToolbarClick();" onmouseover="onToolbarMouserOver();" onmouseout="onToolbarMouserOut();">
		<table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%">
			<tr>
				<td>
					<div id="listToolbar" style="display: none">
						<img align="absMiddle" id="newUser" src="../images/newUser.gif" title="�½��û�" class="toolButton">
						<img align="absMiddle" id="newGroup" src="../images/newGroup.gif" title="�½���Ա��" class="toolButton">
						<img align="absMiddle" id="newOrg" src="../images/newOrg.gif" title="�½����Ż���" class="toolButton">
						<span class="toolSplit" id="splitNew">|</span>
						<img align="absMiddle" id="move" src="../images/move.gif" title="�ƶ���ѡ��Ķ���" class="toolButton">
						<img align="absMiddle" id="sort" src="../images/sort.gif" title="���浱ǰ˳��" class="toolButton">
						<img align="absMiddle" id="logicDelete" src="../images/trash.gif" title="ɾ����ѡ�����������վ��"
							class="toolButton">
						<span class="toolSplit" id="splitSearch">|</span>
					</div>
					<img align="absMiddle" id="refresh" src="../images/refresh.gif" title="ˢ��" class="toolButton">
					<img align="absMiddle" id="search" src="../images/find.gif" title="����..." class="toolButton">
					<div id="trashToolBar" style="display: none">
						<span class="toolSplit">|</span>
						<img align="absMiddle" id="furbishDelete" src="../images/checkUser.gif" title="�ָ���ԭϵͳ"
							class="toolButton">
						<img align="absMiddle" id="delete" src="../images/delete.gif" title="ɾ����ѡ�Ķ��󣨳���ɾ����"
							class="toolButton">
					</div>
					<span id="saveSpan" class="toolSplit" style="display: none; color: black">���޸�����ʾ˳����Ҫ����</span><img
						align="absMiddle" id="save" src="../images/save.gif" title="����" class="toolButton"
						style="display: none">                  
				</td>
				<td align="right">
					<div id="subTitle" style="font-weight: bold; overflow: hidden; color: silver">
					</div>
				</td>
			</tr>
		</table>
	</div>
	<table id="ouUserTable" style="width: 100%">
		<thead id="ouUserHeader">
		</thead>
		<tbody id="ouUserListBody" oncontextmenu="oBodyContextMenu();">
		</tbody>
	</table>
</body>
</html>
