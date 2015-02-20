<%@ Register TagPrefix="cc1" Namespace="System.Web.UI" Assembly="System.Web, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" %>

<%@ Page Codebehind="AppAdmin.aspx.cs" Language="c#" AutoEventWireup="True" Inherits="MCS.Applications.AppAdmin.AppAdmin" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xmlns:hgui>
<head>
	<title>通用授权管理系统</title>
	<link rel="Shortcut Icon" href="./images/icon/key.ico">
	<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
	<meta name="ProgId" content="VisualStudio.HTML">
	<meta name="Originator" content="Microsoft Visual Studio .NET 7.1">
	<meta http-equiv="Content-Type" content="text/html; charset=gb2312">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<link type="text/css" rel="stylesheet" href="./CSS/Input.css">

	<script type="text/javascript" language="javascript" src="./script/validate.js"></script>

	<script type="text/javascript" language="javascript" src="./script/xmlHttp.js"></script>

	<script type="text/javascript" language="javascript" src="./script/uiScript.js"></script>

	<script type="text/javascript" language="javascript" src="./Script/splitter.js"></script>

	<script type="text/javascript" language="javascript" src="./Script/appQuery.js"></script>

	<script type="text/javascript" language="javascript" src="./Script/appCommon.js"></script>

	<script type="text/javascript" language="javascript" src="./Script/ApplicationRoot.js"></script>

	<script type="text/javascript" language="javascript" src="AppAdmin.js"></script>

</head>
<body onload="onDocumentLoad();" onresize="onWindowResize();">
	<xml id="preDefineObj" src="../XML/preDefineObj.xml"></xml>
	<img src="./images/application.gif" style="display: none" width="16" height="16">
	<img src="./images/ResourceFolder.gif" style="display: none" width="16" height="16">
	<img src="./images/ResourceItem.gif" style="display: none" width="16" height="16">
	<img src="./images/ResourceFile.gif" style="display: none" width="16" height="16">
	<img src="./images/folder.gif" style="display: none" width="16" height="16">
	<img src="./images/computer.gif" style="display: none" width="16" height="16">
	<input type="hidden" id="paramValue" name="paramValue">
	<input type="hidden" id="syncData" onpropertychange="onSyncDataChange();" name="syncData">
	<input type="hidden" id="inputXML" name="inputXML" onfocus="autoRefresh();">
	<input type="hidden" id="inputXNode" name="inputXNode">
	<input id="userInfo" runat="server" type="hidden">
	<hgui:smenu id="menuTree" style="visibility: hidden; behavior: url(./htc/hMenu.htc);
		position: absolute" onmenuclick="menuTreeClick()" onbeforepopup="menuBeforePopup()">
			刷新,,,,./images/refresh.gif,refresh;
			-,0,,,,;
			人员的角色,,,,,queryUserAppRoles;
			角色的功能,,,,,modifyFuncForRole;
			-,0,,,,;
			显示功能集合中内容...,,,,,showFuncSetContent;
			属性,,,,./images/property.gif,property
		</hgui:smenu>
	<img src="./images/application.gif" style="display: none" width="16" height="16">
	<!--
			刷新,,,,./images/refresh.gif,refresh;
			-,0,,,,;
			导出角色和功能...,,,,,export;
			导入角色和功能...,,,,,import;
			显示角色中的用户...,,,,,showUsersInRole;
			-,0,,,,;
			资源定位...,,,,./images/ResourceItem.gif,searchResource;
			属性,,,,./images/property.gif,property
		-->
	<table style="width: 100%; height: 100%" cellspacing="0" id="Table1">
		<tr style="height: 64px">
			<td colspan="3" style="border-bottom: black 1px solid">
				<font face="SimSun" style="font-weight: bold; font-size: 18pt">
					<%--<img class="shadowAlpha" align="absMiddle" src="./images/customs.gif" width="48"
						height="48">--%><span id="innerTitle">通用授权管理系统</span></font>
			</td>
		</tr>
		<tr>
			<td style="width: 180px">
				<hgui:jstree3 id="tv" style="behavior: url(./htc/hTree.htc); overflow: auto; width: 100%;
					height: 100%" onnodeselected="tvNodeSelected();" onnodeexpand="tvNodeExpand();"
					onnoderightclick="tvNodeRightClick();" oncontextmenu="event.returnValue = false;">
				</hgui:jstree3>
			</td>
			<td id="splitterContainer">
			</td>
			<td id="innerDocTD">
				<iframe id='frmContainer' style='width: 100%; height: 100%' frameborder='0' scrolling='auto'>
				</iframe>
			</td>
		</tr>
	</table>
</body>
</html>
