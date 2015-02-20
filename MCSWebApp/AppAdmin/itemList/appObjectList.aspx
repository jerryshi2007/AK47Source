<%@ Page Language="c#" Codebehind="appObjectList.aspx.cs" AutoEventWireup="True"
	Inherits="AccreditAdmin.itemList.appObjectList" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns:hgui>
<head>
	<title>��֯��Ԫ����Ա�б�</title>
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<meta http-equiv="Content-Type" content="text/html; charset=gb2312">
	<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="C#" name="CODE_LANGUAGE">

	<script type="text/javascript" language="javascript" src="../script/validate.js"></script>

	<script type="text/javascript" language="javascript" src="../script/xmlHttp.js"></script>

	<script type="text/javascript" language="javascript" src="../script/dbGrid.js"></script>

	<script type="text/javascript" language="javascript" src="../script/uiScript.js"></script>

	<script type="text/javascript" language="javascript" src="../script/appCommon.js"></script>

	<script type="text/javascript" language="javascript" src="../script/appQuery.js"></script>

	<script type="text/javascript" language="javascript" src="../Script/ApplicationRoot.js"></script>

	<script type="text/javascript" language="javascript" src="appObjectList.js"></script>

	<link href="../CSS/Input.css" type="text/css" rel="stylesheet">
</head>
<body onload="onDocumentLoad()">
	<input id="roleCodeName" type="hidden" name="roleCodeName" runat="server">
	<input id="secFrm" type="hidden" name="secFrm" runat="server">
	<input id="inputParam" onpropertychange="onInputParamChange();" type="hidden"><input
		id="listMaxCount" type="hidden" runat="server">
	<input style="visibility: hidden" onclick="btnClick()" type="button" value="Button">
	<hgui:smenu id="menuObject" style="z-index: 100; visibility: hidden; behavior: url(../htc/hMenu.htc);
		position: absolute" onbeforepopup="menuObjectBeforePopup();" onmenuclick="menuObjectClick()">
	</hgui:smenu>
	<xml id="LEVEL_CODE" src="../xml/level_code.xml"></xml>
	<xml id="ColumnTitleApp">
			<ColumnTitle>
				<Column name="" title="���" dataFld="SORT_ID" dataSrc="ROLE_FUNCTION" width="18px" height="20px" />
				<Column name="����" dataFld="NAME" height="18px" dataSrc="ROLE_FUNCTION" width="30%" />
				<Column name="Ӣ�ı�ʶ" dataFld="CODE_NAME" dataSrc="ROLE_FUNCTION" width="30%" />
				<Column name="����" dataFld="DESCRIPTION" dataSrc="ROLE_FUNCTION" />
			</ColumnTitle>
		</xml>
	<xml id="ColumnTitleAppSec">
			<ColumnTitle>
				<Column name="" title="���" dataFld="SORT_ID" dataSrc="ROLE_FUNCTION" width="18px" height="20px" />
				<Column name="����" dataFld="NAME" height="18px" dataSrc="ROLE_FUNCTION" width="30%" />
				<Column name="����" dataFld="DESCRIPTION" dataSrc="ROLE_FUNCTION" />
			</ColumnTitle>
		</xml>
	<xml id="ColumnTitleRolesFuncs">
			<ColumnTitle>
				<Column name="" title="���" dataFld="SORT_ID" dataSrc="ROLE_FUNCTION" width="18px" height="20px" />
				<Column name="����" dataFld="NAME" height="18px" dataSrc="ROLE_FUNCTION" width="30%" />
				<Column name="����" dataFld="DESCRIPTION" dataSrc="ROLE_FUNCTION" width="65%" />
				<Column name="" dataFld="FUNC_ID" width="20px" dataSrc="" />
			</ColumnTitle>
		</xml>
	<xml id="ColumnTitleFunSetFuncs">
			<ColumnTitle>
				<Column name="" title="���" dataFld="SORT_ID" dataSrc="ROLE_FUNCTION" width="18px" height="20px" />
				<Column name="����" dataFld="NAME" height="18px" dataSrc="ROLE_FUNCTION" width="30%" />
				<Column name="����" dataFld="DESCRIPTION" dataSrc="ROLE_FUNCTION" width="65%" />
				<Column name="" dataFld="FUNC_SET_ID" width="20px" dataSrc="" />
			</ColumnTitle>
		</xml>
	<xml id="ColumnTitleUserScopes">
			<ColumnTitle>
				<Column name="" title="���" dataFld="SORT_ID" dataSrc="ROLE_FUNCTION" width="18px" height="20px" />
				<Column name="����" dataFld="NAME" height="18px" dataSrc="ROLE_FUNCTION" width="30%" />
				<Column name="����" dataFld="DESCRIPTION" dataSrc="ROLE_FUNCTION" width="65%" />
				<Column name="" dataFld="EXP_ID" width="20px" dataSrc="" />
			</ColumnTitle>
		</xml>
	<xml id="ColumnTitleRolesUsers">
			<ColumnTitle>
				<Column name="" title="���" dataFld="SORT_ID" dataSrc="ROLE_FUNCTION" width="18px" height="20px" />
				<Column name="����" dataFld="NAME" height="18px" dataSrc="ROLE_FUNCTION" width="20%" />
				<Column name="����" dataFld="ALL_PATH_NAME" dataSrc="ROLE_FUNCTION" />
				<Column name="�����޶�" dataFld="ACCESS_LEVEL_NAME" dataSrc="ROLE_FUNCTION" width="72px" />
			</ColumnTitle>
		</xml>
	<xml id="ColumnTitleUsers">
			<ColumnTitle>
				<Column name="" title="���" dataFld="SORT_ID" dataSrc="ROLE_FUNCTION" width="18px" height="20px" />
				<Column name="����" dataFld="NAME" height="18px" dataSrc="ROLE_FUNCTION" width="15%" />
				<Column name="����" dataFld="DESCRIPTION" dataSrc="ROLE_FUNCTION" />
			</ColumnTitle>
		</xml>
	<xml id="ColumnTitleAppScopes">
			<ColumnTitle>
				<Column name="" title="���" dataFld="SORT_ID" dataSrc="SCOPES" width="18px" height="20px" />
				<Column name="����" dataFld="NAME" height="18px" dataSrc="SCOPES" width="15%" />
				<Column name="����" dataFld="DESCRIPTION" dataSrc="SCOPES" />
			</ColumnTitle>
		</xml>
	<xml id="preDefineObj" src="../XML/preDefineObj.xml"></xml>
	<table id="Table1" style="width: 100%">
		<tr>
			<td>
				<img style="display: none" height="16" src="../images/function.gif" width="16">
				<img style="display: none" height="16" src="../images/role.gif" width="16">
				<img style="display: none" height="16" src="../images/application.gif" width="16">
				<div id="tempDiv" style="display: none">
				</div>
			</td>
		</tr>
		<tr>
			<td style="height: 18px">
				<table class="toolbarPanel" style="width: 100%" cellspacing="0" cellpadding="0" align="center"
					border="0">
					<tr>
						<td align="left">
							<div id="toolBar" onmouseover="onToolbarMouserOver();" style="display: none; width: 100%"
								onclick="onToolbarClick();" onmouseout="onToolbarMouserOut();">
								<img class="toolButton" id="newScope2Copy" title="�̳����ݷ���Χ" src="../images/newScope2Copy.gif"
									align="absMiddle">
								<img class="toolButton" id="newScope2" title="�½����ݷ���Χ" src="../images/newScope2.gif"
									align="absMiddle">
								<img class="toolButton" id="newFuncCopy" title="�̳й���" src="../images/insertFuncCopy.gif"
									align="absMiddle">
								<img class="toolButton" id="newFunc" title="�½�����" src="../images/insertFunc.gif"
									align="absMiddle">
								<img class="toolButton" id="newObjCopy" title="�̳�" src="../images/insertCopy.gif"
									align="absMiddle">
								<img class="toolButton" id="newObj" title="�½�" src="../images/insert.gif" align="absMiddle">
								<span class="toolSplit">|</span>
								<img class="toolButton" id="refresh" title="ˢ��" src="../images/refresh.gif" align="absMiddle">
								<span class="toolSplit">|</span>
								<img class="toolButton" id="deleteObj" title="ɾ����ѡ�Ķ���" src="../images/delete.gif"
									align="absMiddle">
								<img class="toolButton" id="checkUser" title="ɾ����ɫ����Ч�ı���Ȩ����" src="../images/checkUser.gif"
									align="absMiddle">
								<span id="saveSpan" style="display: none">���Ѿ��޸������ݣ���Ҫ����<img class="toolButton" id="save"
									title="����" src="../images/save.gif" align="absMiddle"></span>
							</div>
						</td>
						<td align="right" width="410" id="paginationTD">
							����<span id="rowCount">99999</span> [<a href="#" onclick="onGoPage('first')">��ҳ</a>][<a
								href="#" onclick="onGoPage('pre')">��ҳ</a>][<a href="#" onclick="onGoPage('next')">��ҳ</a>][<a
									href="#" onclick="onGoPage('last')">βҳ</a>] [<span id="curPage">99999</span>/<span
										id="totalPage">99999</span>] ת��:
							<input onkeypress="checkNumber()" id="curPageIndex" style="width: 50px; text-align: center"
								type="text" value="1">ҳ<img class="toolButton" id="goPage" title="ת��" src="../images/go.gif"
									align="absMiddle" onclick="onGoPage()">
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td>
				<table id="roleFunctionTable" style="width: 100%">
					<thead id="roleFunctionTableHead">
					</thead>
					<tbody oncontextmenu="oBodyContextMenu();" id="roleFunctionTableBody">
						<tr>
						</tr>
					</tbody>
				</table>
			</td>
		</tr>
	</table>
</body>
</html>
