<%@ Page Language="c#" Codebehind="ViewUserSideline.aspx.cs" AutoEventWireup="True"
	Inherits="MCS.Applications.AccreditAdmin.dialogs.ViewUserSideline" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
	<title>用户兼职情况</title>
	<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
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

	<script type="text/javascript" language="javascript" src="ViewUserSideline.js"></script>

</head>
<body ms_positioning="GridLayout" onload="onDocumentLoad()">
	<xml id="ColumnTitle">
			<ColumnTitle>
				<Column name="" title="序号" dataFld="SORTID" dataSrc="USER_OU" height="18px" width="3%" />
				<Column name="" title="是否兼职" dataFld="MAIN_DUTY" dataSrc="USER_OU" width="3%" image="../images/sideline.gif"/>
				<Column name="名称" dataFld="DISPLAY_NAME" dataSrc="USER_OU" width="16%" />
				<Column name="职务/级别" dataFld="RankName" dataSrc="USER_OU" width="16%" />
				<Column name="系统位置" dataFld="ALL_PATH_NAME" dataSrc="USER_OU" width="40%" />
				<Column name="描述" dataFld="DESCRIPTION" dataSrc="USER_OU" />
			</ColumnTitle>
		</xml>
	<input type="hidden" id="rsUserMessDetail" runat="server">
	<table style="width: 100%; height: 100%">
		<tr>
			<td style="height: 32px">
				<span style="background-position: center center; background-image: url(../images/32/user.gif);
					width: 32px; background-repeat: no-repeat; height: 32px"></span><font size="4"><strong
						id="logoGroup">用户兼职情况</strong></font>
			</td>
		</tr>
		<tr valign="top" style="height: 100%">
			<td align="center">
				<div style="overflow: auto; width: 100%; height: 100%">
					<table id="ouUserTable" style="width: 100%">
						<thead id="ouUserHeader">
						</thead>
						<tbody id="ouUserListBody">
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
