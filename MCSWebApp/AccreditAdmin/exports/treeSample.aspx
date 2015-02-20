<%@ Page Codebehind="treeSample.aspx.cs" Language="c#" AutoEventWireup="True" Inherits="MCS.Applications.AccreditAdmin.exports.treeSample" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns:ad>
<head>
	<title>组织人员树范例</title>
	<meta http-equiv="Content-Type" content="text/html; charset=gb2312">
	<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<link href="../CSS/Input.css" type="text/css" rel="stylesheet">

	<script type="text/javascript" language="javascript" src="../oguScript/validate.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/xmlHttp.js"></script>

	<script type="text/javascript" language="javascript" src="TreeSample.js"></script>

	<style type="text/css"> .storeuserData { BEHAVIOR: url(#default#userData) } </style>
	<!--STYLE> @media all { AD\:dnInput { behavior: url(dnInput.htc); }}
		</STYLE-->
</head>
<body class="modal" onload="onDocumentLoad();" onunload="onDocumentUnload()">
	<xml id="ignoreObjs" src="../xml/HideTypes.xml"></xml>
	<xml id="xmlOrgAttr" src="../xml/OrgAttribute.xml"></xml>
	<!--xml id="OU_LEVEL_CODE" src="../xml/ou_level_code.xml"></xml-->
	<iframe name="innerReceiver" style="display: none"></iframe>
	<input id="dnSelected" type="hidden" name="dnSelected">
	<input type="hidden" id="persistObj" class="storeuserData" name="persistObj">
	<form id="frmXml" method="post" target="innerFrame">
		<input type="hidden" name="firstChildren" id="firstChildren">
	</form>
	<form id="frmInput" runat="server">
		<table style="width: 100%" id="Table1">
			<tr>
				<td style="height: 32px">
					<span style="background-position: center center;  background-image: url(../images/32/ou.gif);
						width: 32px; background-repeat: no-repeat; height: 32px"></span><font size="4"><strong>
							组织人员树范例</strong></font>
					<hr>
				</td>
			</tr>
			<tr style="height: 48px">
				<td>
					<table class="modalEditable" style="width: 100%" cellspacing="4" cellpadding="0"
						id="Table2">
						<tr>
							<td align="right">
								<strong>提交URL：</strong>
							</td>
							<td>
								<input id="postData" size="64" value="TreeSamplePostPage.aspx" name="postData">
							</td>
						</tr>
						<tr>
							<td align="right">
								<strong>根对象：</strong>
							</td>
							<td>
								<ad:oguinput id="OGUInput" onchange="changeSourceDN();" style="behavior: url(../htc/OGUInput.htc);
									width: 320px" size="64">
									<strong></strong>
								</ad:oguinput>
							</td>
						</tr>
						<tr>
							<td align="right">
								<strong>根OU：</strong>
							</td>
							<td>
								<input id="rootOrg" size="64" name="rootOrg">
							</td>
						</tr>
						<tr>
							<td align="right">
								<strong>目标Frame：</strong>
							</td>
							<td>
								<input id="targetFrame" size="64" value="innerReceiver" name="targetFrame">
							</td>
						</tr>
						<tr>
							<td align="right">
								<strong>自动展开的节点：</strong>
							</td>
							<td>
								<textarea id="autoExpand" style="width: 100%; height: 32px" name="autoExpand">&lt;root&gt;&lt;object ALL_PATH_NAME="中国海关\01海关总署\00署领导"/&gt;&lt;object ALL_PATH_NAME="中国海关\03北京海关"/&gt;&lt;/root&gt;</textarea>
							</td>
						</tr>
						<tr>
							<td align="right">
								<strong>自动选定对象</strong>：
							</td>
							<td>
								<textarea id="nodesSelected" style="width: 100%; height: 32px" name="nodesSelected">&lt;NodesSelected&gt;&lt;object ALL_PATH_NAME="中国海关\03北京海关"/&gt;&lt;object ALL_PATH_NAME="中国海关\01海关总署\27信息中心"/&gt;&lt;/NodesSelected&gt;</textarea>
							</td>
						</tr>
						<tr>
							<td align="right">
								<strong>展开级别</strong>：
							</td>
							<td>
								<input id="maxLevel" size="16" name="maxLevel">
							</td>
						</tr>
						<tr>
							<td align="right" style="width: 96px" valign="middle">
								<strong>行政级别限制：</strong>
							</td>
							<td>
								<strong>机构:</strong><asp:DropDownList ID="orgSelectAccessLevel" runat="server">
								</asp:DropDownList>
								<strong>人员:</strong><asp:DropDownList ID="userSelectAccessLevel" runat="server">
								</asp:DropDownList>
							</td>
						</tr>
						<tr>
							<td align="right" rowspan="3">
								<strong>选项：</strong></td>
							<td>
								<input id="chkOU" type="checkbox" checked name="chkOU" style="border: none"><strong>列出OU</strong>
								<input id="chkUser" type="checkbox" checked name="chkUser" style="border: none"><strong>列出User</strong>
								<input id="chkGroup" type="checkbox" checked name="chkGroup" style="border: none"><strong>列出Group</strong>
								<input id="chkSideline" type="checkbox" checked name="chkSideline" style="border: none"><strong>列出兼职</strong>
								<input id="refresh" onclick="onRefreshTree()" type="button" value="Refresh tree"
									name="refresh">
								<input id="chkMultiSelect" type="checkbox" checked name="chkMultiSelect" style="border: none"><strong>多选</strong>
								<input id="chkSelectOU" type="checkbox" checked name="chkSelectOU" style="border: none"><strong>选择OU</strong>
								<input id="chkSelectUser" type="checkbox" checked name="chkSelectUser" style="border: none"><strong>选择User</strong>
								<input id="chkSelectGroup" type="checkbox" checked name="chkSelectGroup" style="border: none"><strong>选择Group</strong>
							</td>
						</tr>
						<tr>
							<td>
								<input id="chkDirDelete" type="checkbox" checked name="chkDirDelete" style="border: none"><strong>列出直接逻辑删除对象</strong>
								<input id="chkOrgDelete" type="checkbox" checked name="chkOrgDelete" style="border: none"><strong>列出间接机构逻辑删除对象</strong>
								<input id="chkUserDelete" type="checkbox" checked name="chkUserDelete" style="border: none"><strong>列出间接人员逻辑删除对象</strong>
								<input id="chkSelectSort" type="checkbox" name="chkSelectSort" title="记录选择对象的选择次序"
									style="border: none"><strong>选择次序</strong>
								<input id="chkHideButton" type="checkbox" name="chkHideButton" title="显示按钮" style="border: none"><strong>显示按钮</strong>
								<input id="chkShowTrash" type="checkbox" name="chkShowTrash" title="显示回收站" style="border: none"><strong>显示回收站</strong>
								<input id="chkShowMyOrg" type="checkbox" name="chkShowMyOrg" title="展开当前操作人员所在的机构"
									style="border: none"><strong>展开操作人员所在机构</strong>
							</td>
						</tr>
						<tr>
							<td>
								<strong>要求屏蔽的数据对象类型</strong>:<select id="selHideType" name="selHideType" title="要求屏蔽的数据对象类型"></select>
								<strong>机构类别(排除)</strong>:<select id="selOrgClass" name="selOrgClass" title="机构类别"></select>
								<strong>机构属性(排除)</strong>:<select id="selOrgType" name="selOrgType" title="机构属性"></select>
							</td>
						</tr>
						<tr>
							<td align="right" valign="top">
								<strong>第一级对象：</strong></td>
							<td>
								<textarea id="xmlArea" style="width: 100%; height: 64px" name="xmlArea"></textarea>
							</td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td>
					<table class="modalEditable" style="width: 100%" cellspacing="0" cellpadding="0"
						id="Table3">
						<tr>
							<td id="frameContainer" style="width: 100%; height: 300px">
								<iframe name="innerFrame" id="innerFrame" onreadystatechange="onFrameStateChange();"
									style="border-right: black 1px solid; padding-right: 0px; border-top: black 1px solid;
									padding-left: 0px; padding-bottom: 0px; margin: 0px; border-left: black 1px solid;
									width: 100%; padding-top: 0px; border-bottom: black 1px solid; height: 100%"></iframe>
							</td>
						</tr>
						<tr>
							<td style="width: 100%; height: 200px">
								<strong>结果：</strong>
								<textarea id="result" style="width: 100%; height: 100%" name="result"></textarea>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</form>
</body>
</html>
