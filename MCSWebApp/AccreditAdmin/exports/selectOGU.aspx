<%@ Page Language="c#" Codebehind="selectOGU.aspx.cs" AutoEventWireup="True" Inherits="MCS.Applications.AccreditAdmin.exports.selectOGU" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
	<title>组织结构</title>
	<meta http-equiv="Content-Type" content="text/html; charset=gb2312">
	<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
	<meta name="CODE_LANGUAGE" content="C#">
	<meta name="vs_defaultClientScript" content="JavaScript">
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	<link href="../CSS/Input.css" type="text/css" rel="stylesheet">

	<script type="text/javascript" language="javascript" src="../oguScript/validate.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/xmlHttp.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/uiScript.js"></script>

	<script type="text/javascript" language="javascript" src="../Script/ApplicationRoot.js"></script>

	<script type="text/javascript" language="javascript" src="../selfScript/accreditAdmin.js"></script>

	<script type="text/javascript" language="javascript" src="selectOGU.js"></script>

</head>
<body class="modal" onload="onDocumentLoad();" onunload="onDocumentUnload();">
	<input type="hidden" id="requestParam" runat="server">
	<input type="hidden" id="dnSelected">
	<form id="innerForm" name="innerForm" style="display: none" method="post" target="innerFrame"
		action="./OGUTree.aspx">
		<input type="hidden" id="hidTopControl" name="hidTopControl"><input type="hidden"
			id="hidMaxLevel" name="hidMaxLevel">
		<input type="hidden" id="hidBackColor" name="hidBackColor"><input type="hidden" id="hidTarget"
			name="hidTarget">
		<input type="hidden" id="hidShowButtons" name="hidShowButtons"><input type="hidden"
			id="hidMultiSelect" name="hidMultiSelect">
		<input type="hidden" id="hidListObjType" name="hidListObjType"><input type="hidden"
			id="hidListObjDelete" name="hidListObjDelete">
		<input type="hidden" id="hidSelectObjType" name="hidSelectObjType"><input type="hidden"
			id="hidRootOrg" name="hidRootOrg">
		<input type="hidden" id="hidAutoExpand" name="hidAutoExpand"><input type="hidden"
			id="hidExtAttr" name="hidExtAttr">
		<input type="hidden" id="hidOrgAccessLevel" name="hidOrgAccessLevel"><input type="hidden"
			id="hidUserAccessLevel" name="hidUserAccessLevel">
		<input type="hidden" id="hidHideType" name="hidHideType"><input type="hidden" id="hidSelectSort"
			name="hidSelectSort">
		<input type="hidden" id="hidCanSelectRoot" name="hidCanSelectRoot"><input type="hidden"
			id="hidNodesSelected" name="hidNodesSelected">
	</form>
	<table style="width: 100%; height: 100%">
		<tr>
			<td style="height: 32px">
				<span style="background-position: center center; background-image: url(../images/32/ou.gif);
					width: 32px; background-repeat: no-repeat; height: 32px"></span><font size="4"><strong>
						选择组织结构或用户</strong></font>
				<hr>
			</td>
		</tr>
		<tr>
			<td>
				<table class="modalEditable" style="width: 100%; height: 100%" cellpadding="0" cellspacing="0">
					<tr>
						<td>
							<div id="frameContainer" style="width: 100%; height: 100%">
								<iframe id='innerFrame' name="innerFrame" onreadystatechange='onFrameStateChange();'
									style='border-right: black 1px solid; padding-right: 0px; border-top: black 1px solid;
									padding-left: 0px; padding-bottom: 0px; margin: 0px; border-left: black 1px solid;
									width: 100%; padding-top: 0px; border-bottom: black 1px solid; height: 100%;
									background-color: transparent" frameborder="no" scrolling="no"' src=""></iframe>
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
				<form id="frmInput">
					<table style="width: 100%; height: 100%" cellspacing="0">
						<tr>
							<td align="center">
								<input id="btnOK" style="width: 80px" disabled accesskey="O" onclick="onSaveClick();"
									type="button" value="确定(O)">
							</td>
							<td align="center">
								<input id="btnCancel" style="width: 80px" accesskey="C" onclick="onCancelClick();"
									type="button" value="取消(C)">
							</td>
						</tr>
					</table>
				</form>
			</td>
		</tr>
	</table>
</body>
</html>
