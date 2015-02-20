<%@ Page Language="c#" Codebehind="UsersInGroup.aspx.cs" AutoEventWireup="True" Inherits="MCS.Applications.AccreditAdmin.dialogs.UsersInGroup" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns:hgui>
<head>
	<title>组成员列表</title>
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
			新建用户...,,,,../images/newUser.gif,newUser;
			刷新,,,,../images/refresh.gif,refresh;
			-,,,,,;
			重设密码...,,,,../images/KeyIcon.gif,resetPassword;
			重置用户密码,,,,,initPassword;
			保存当前顺序,,,,../images/sort.gif,sort;
			-,,,,,;
			删除,,,,../images/delete.gif,delete;
			-,,,,,;
			属性...,,,,../images/property.gif,property
		</hgui:hmenu>
	<!--
			查找...,,,,../images/find.gif,search;
		-->
	<div id="tempDiv" style="display: none">
	</div>
	<xml id="ColumnTitle">
			<ColumnTitle>
				<Column name="" title="序号" dataFld="SORT_ID" dataSrc="USER_OU" height="18px" width="4%" />
				<Column name="名称" dataFld="DISPLAY_NAME" dataSrc="USER_OU" width="16%" />
				<Column name="职务/级别" dataFld="RankName" dataSrc="USER_OU" width="16%" />
				<Column name="系统位置" dataFld="ALL_PATH_NAME" dataSrc="USER_OU" width="40%" />
				<Column name="描述" dataFld="DESCRIPTION" dataSrc="USER_OU" />
			</ColumnTitle>
		</xml>
	<xml id="ADSearchConfig" src="../xml/ADSearchConfig.xml"></xml>
	<table style="width: 100%; height: 100%">
		<tr valign="top">
			<td style="height: 32px">
				<span style="background-position: center center; background-image: url(../images/32/group.gif);
					width: 32px; background-repeat: no-repeat; height: 32px"></span><font size="4"><strong
						id="logoGroup">组成员列表(<asp:Literal ID="GroupDisplayName" runat="server"></asp:Literal>)</strong></font>
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
								<img align="absMiddle" id="newUser" src="../images/newUser.gif" title="新建用户" class="toolButton">
								<img align="absMiddle" id="delete" src="../images/delete.gif" title="删除所选的对象（彻底删除）"
									class="toolButton">
								<img align="absMiddle" id="sort" src="../images/sort.gif" title="保存当前顺序" class="toolButton">
								<span class="toolSplit">|</span>
								<img align="absMiddle" id="refresh" src="../images/refresh.gif" title="刷新" class="toolButton">
								<input id="txtSearchObj" name="txtSearchObj" maxlength="32" style="width: 80px">
								<img align="absMiddle" id="search" src="../images/find.gif" title="查找..." class="toolButton">
								<span id="saveSpan" class="toolSplit" style="display: none; color: black">您修改了显示顺序，需要保存</span>
								<img align="absMiddle" id="save" src="../images/save.gif" title="保存" class="toolButton"
									style="display: none">
							</div>
						</td>
						<td align="right">
							共：<span id="spUserCount"></span> <span><a href="#" onclick="onGotoPage('firstPage')"
								id="firstPage">[首页]</a> <a href="#" onclick="onGotoPage('prePage')" id="prePage">[上页]</a>
								<a href="#" onclick="onGotoPage('nextPage')" id="nextPage">[下页]</a> <a href="#" onclick="onGotoPage('lastPage')"
									id="lastPage">[尾页]</a> [<span id="spUserPage"></span>] 转到：<input id="txtAimPage"
										name="txtAimPage" style="width: 20px" value="1">
								<span style="background-position: left 50%; background-image: url(../images/go.gif);
									width: 18px; background-repeat: no-repeat; height: 18px" onclick="onGotoPage('aimPage')"
									title="跳转" id="aimPage"></span></span>
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
				<input accesskey="C" style="width: 80px" type="button" value="关闭(C)" id="btnOK" onclick="window.top.close();"><br>
			</td>
		</tr>
	</table>
</body>
</html>
