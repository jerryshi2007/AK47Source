<%@ Page Language="c#" Codebehind="OGUTree.aspx.cs" AutoEventWireup="True" Inherits="MCS.Applications.AccreditAdmin.exports.OGUTree" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns:hgui>
<head>
	<title>组织机构树</title>
	<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
	<meta http-equiv="Content-Type" content="text/html; charset=gb2312">
	<meta name="CODE_LANGUAGE" content="C#">
	<meta name="vs_defaultClientScript" content="JavaScript">
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	<link type="text/css" rel="stylesheet" href="../CSS/Input.css">

	<script type="text/javascript" language="javascript" src="../oguScript/validate.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/xmlHttp.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/uiScript.js"></script>

	<script type="text/javascript" language="javascript" src="../Script/ApplicationRoot.js"></script>

	<script type="text/javascript" language="javascript" src="../selfScript/accreditAdmin.js"></script>

	<script type="text/javascript" language="javascript" src="../selfScript/organizeTree.js"></script>

	<script type="text/javascript" language="javascript" src="OGUTree.js"></script>

</head>
<body onload="onDocumentLoad();" style="background-color: transparent" scroll="no"
	bottommargin="0" leftmargin="0" topmargin="0" rightmargin="0">
	<img src="../images/domain.gif" style="display: none">
	<img src="../images/user.gif" style="display: none">
	<img src="../images/organization.gif" style="display: none">
	<img src="../images/group.gif" style="display: none">
	<input type="hidden" id="LMaxLevel" runat="server" name="LMaxLevel">
	<input type="hidden" id="LBackColor" runat="server" name="LBackColor">
	<input type="hidden" id="LSumitType" runat="server" name="LSumitType">
	<input type="hidden" id="LSumitData" runat="server" name="LSumitData">
	<input type="hidden" id="LShowButtons" runat="server" name="LShowButtons">
	<input type="hidden" id="LMultiSelect" runat="server" name="LMultiSelect">
	<input type="hidden" id="LListObjType" runat="server" name="LListObjType">
	<input type="hidden" id="LListObjDelete" runat="server" name="LListObjDelete">
	<input type="hidden" id="LSelectObjType" runat="server" name="LSelectObjType">
	<input type="hidden" id="LRootOrg" runat="server" name="LRootOrg">
	<input type="hidden" id="LAutoExpand" runat="server" name="LAutoExpand">
	<input type="hidden" id="LExtAttr" runat="server" name="LExtAttr">
	<input type="hidden" id="LOrgAccessLevel" runat="server" name="LOrgAccessLevel">
	<input type="hidden" id="LUserAccessLevel" runat="server" name="LUserAccessLevel">
	<input type="hidden" id="LHideType" runat="server" name="LHideType">
	<input type="hidden" id="LSelectSort" runat="server" name="LSelectSort">
	<input type="hidden" id="LCanSelectRoot" runat="server" name="LCanSelectRoot">
	<input type="hidden" id="LNodesSelected" runat="server" name="LNodesSelected">
	<input type="hidden" id="LShowTrash" runat="server" name="LShowTrash">
	<input type="hidden" id="LTarget" runat="server" name="LTarget">
	<input type="hidden" id="LOrgClass" runat="server" name="LOrgClass">
	<input type="hidden" id="LOrgType" runat="server" name="LOrgType">
	<!--input type="hidden" id="LShowMyOrg" name="LShowMyOrg"-->
	<form id="Container" runat="server">
		<asp:TextBox ID="TBSubChildData" Style="display: none" runat="server"></asp:TextBox>
	</form>
	<form id="treeForm" method="post" target="">
		<table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%">
			<tr>
				<td>
					<hgui:htree id="tv" style="behavior: url(../htc/hTree.htc); overflow: auto; width: 100%;
						height: 100%; background-color: transparent" onnodedoubleclick="tvNodeDoubleClick();"
						onnodeselected="tvNodeSelected();" onnodeexpand="tvNodeExpand();">
					</hgui:htree>
				</td>
			</tr>
			<tr id="buttonLine" style="display: none; height: 20px">
				<td>
					<table style="width: 100%; height: 100%">
						<tr>
							<td colspan="2">
								<hr>
							</td>
						</tr>
						<tr>
							<td align="center">
								<input type="hidden" name="data">
								<input accesskey="O" style="width: 80px" type="button" id="btnOK" onclick="onSubmitClick(true)"
									value="确定(O)">
							</td>
							<td align="center">
								<input accesskey="C" style="width: 80px" type="button" id="btnCancel" onclick="onSubmitClick(false)"
									value="取消(O)">
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</form>
</body>
</html>
