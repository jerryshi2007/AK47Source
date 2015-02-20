<%@ Page Codebehind="LogViewer.aspx.cs" Language="c#" AutoEventWireup="True" Inherits="MCS.Applications.AppAdmin_LOG.LogViewer" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xmlns:hgui>
<head>
	<title>机构人员和授权日志审计系统</title>
	<link rel="Shortcut Icon" href="./images/icon/key.ico">
	<meta http-equiv="Content-Type" content="text/html; charset=gb2312">
	<meta name="GENERATOR" content="Microsoft Visual Studio 7.0">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<link type="text/css" rel="stylesheet" href="./css/Input.css">

	<script type="text/javascript" language="javascript" src="./script/validate.js"></script>

	<script type="text/javascript" language="javascript" src="./script/xmlHttp.js"></script>

	<script type="text/javascript" language="javascript" src="./script/uiScript.js"></script>

	<script type="text/javascript" language="javascript" src="./script/xAD.js"></script>

	<script type="text/javascript" language="javascript" src="./Script/ApplicationRoot.js"></script>

	<script type="text/javascript" language="javascript" src="./script/dbGrid.js"></script>

	<script type="text/javascript" language="javascript" src="./script/splitter.js"></script>

	<script type="text/javascript" language="javascript" src="./script/organizeTree.js"></script>

	<script type="text/javascript" language="javascript" src="LogViewer.js"></script>

</head>
<body onload="onDocumentLoad();" onresize="onWindowResize();">
	<input type="hidden" id="currentUserName" name="currentUserName" title="用于记录系统中当前登录用户的身份"
		runat="server"><input type="hidden" id="rolesValue" name="rolesValue" title="用于判断当前用户权限"
			runat="server">
	<input type="hidden" id="paramValue" name="paramValue"><input type="hidden" id="refreshPage"
		onpropertychange="onDataChanged();">
	<table style="width: 100%; height: 100%" cellspacing="0" id="Table1">
		<tr style="height: 60px">
			<td colspan="3" style="border-bottom: black 1px solid">
				<font face="SimSun" style="font-weight: bold; font-size: 16pt"><span style="background-image: url(./images/history.gif);
					width: 32px; margin-right: 4px; background-repeat: no-repeat; height: 32px"></span>
					机构人员和授权日志审计系统</font>
			</td>
		</tr>
		<tr>
			<td style="width: 220px">
				<hgui:htree id="tv" style="behavior: url(./htc/hTree.htc); overflow: auto; width: 100%;
					height: 100%" onnodeselected="tvNodeSelected();" onnodeexpand="tvNodeExpand();"
					onnoderightclick="tvNodeRightClick();">
				</hgui:htree>
			</td>
			<td id="splitterContainer">
			</td>
			<td id="innerDocTD">
				<iframe id='frmContainer' style="width: 100%; height: 100%" frameborder='0' scrolling='auto'>
				</iframe>
			</td>
		</tr>
	</table>
</body>
</html>
