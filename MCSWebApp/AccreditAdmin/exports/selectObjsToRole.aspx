<%@ Page Language="c#" Codebehind="selectObjsToRole.aspx.cs" AutoEventWireup="True"
	Inherits="MCS.Applications.AccreditAdmin.exports.selectObjsToRole" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns:hgui>
<head>
	<title>组织结构</title>
	<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
	<meta name="CODE_LANGUAGE" content="C#">
	<meta name="vs_defaultClientScript" content="JavaScript">
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	<?import namespace="hGui" implementation="../htc/calendar.htc" />
	<meta http-equiv="Content-Type" content="text/html; charset=gb2312">
	<link href="../CSS/Input.css" type="text/css" rel="stylesheet">

	<script type="text/javascript" language="javascript" src="../oguScript/validate.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/xmlHttp.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/xsdAccess.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/uiScript.js"></script>

	<script type="text/javascript" language="javascript" src="../Script/ApplicationRoot.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/dbGrid.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/dateTime.js"></script>

	<script type="text/javascript" language="javascript" src="../selfScript/accreditAdmin.js"></script>

	<script type="text/javascript" language="javascript" src="../selfScript/organizeTree.js"></script>

	<script type="text/javascript" language="javascript" src="../itemList/oguListCommon.js"></script>

	<script type="text/javascript" language="javascript" src="selectObjsToRole.js"></script>

</head>
<body class="modal" onload="onDocumentLoad();" onunload="onDocumentUnload()" onkeypress="onDocumentKeyPress();">
	<div id="tempDiv" style="display: none">
	</div>
	<hgui:calendar id="hCalendar" style="z-index: 101; left: 10px; visibility: hidden;
		overflow: hidden; width: 160px; position: absolute; top: 15px; height: 200px">
		<strong><font size="4" color="#ff0099"></font></strong>
	</hgui:calendar>
	<hgui:hmenu id="listMenu" style="visibility: hidden; behavior: url(../htc/hMenu.htc);
		position: absolute" onbeforepopup="listMenuBeforePopup();" onmenuclick="listMenuClick()">
			删除,,,,../images/delete.gif,delete;
			移动,,,,../images/move.gif,move;
			-,,,,,;
			属性,,,,../images/property.gif,property
		</hgui:hmenu>
	<input type="hidden" id="persistObj" class="userData">
	<input type="hidden" id="dnSelected">
	<!--xml id="LEVEL_CODE" src="../xml/level_code.xml"></xml-->
	<xml id="ColumnTitleOU">
			<ColumnTitle>
				<Column name="" title="序号" dataFld="SORT_ID" width="18px" dataSrc="USER_OU" />
				<Column name="" title="是否已经建立邮箱" dataFld="E_MAIL" dataSrc="USER_OU" width="18px" height="20px"
					image="../images/mailbox.gif" />
				<Column name="名称" dataFld="DISPLAY_NAME" height="18px" dataSrc="USER_OU" width="30%" />
				<Column name="系统位置" dataFld="ALL_PATH_NAME" dataSrc="USER_OU" />
				<Column name="" dataFld="" width="20px" dataSrc="" />
			</ColumnTitle>
		</xml>
	<form id="frmInput" runat="server">
		<table style="width: 100%; height: 100%">
			<tr>
				<td colspan="2" style="height: 32px">
					<strong><font size="4"></font></strong><strong><font size="4"></font></strong><span
						id="logo" style="background-position: center center; display: none; 
						background-image: url(../images/32/ou.gif); width: 32px; background-repeat: no-repeat;
						height: 32px"></span><font size="4"><strong id="Caption" style="display: none">在角色中加入组织结构或用户</strong></font>
					<hr>
				</td>
			</tr>
			<tr height="24px">
				<td colspan="2" align="left">
					<table style="width: 100%; height: 100%">
						<tr height="24px">
							<td nowrap align="right" style="width: 10%">
								<strong>隶属于</strong>:
							</td>
							<td nowrap style="width: 40%">
								<!--input id="belongTo" type="text" onchange="belongToChange()" datafld="ALL_PATH_NAME" datasrc="Condition"
										style="width:100%;display:none"-->
								<hgui:oguinput id="ALL_PATH_NAME" listmask="1" autogetproperties="true" datafld="ALL_PATH_NAME"
									datasrc="Condition" style="behavior: url(../htc/OGUInput.htc); width: 100%; display: none">
									<strong></strong>
								</hgui:oguinput>
								<select id="selectBelongTo" style="display: none; width: 100%" datafld="ALL_PATH_NAME"
									datasrc="Condition" onchange="onRankChanged()">
								</select>
							</td>
							<td nowrap align="left" style="width: 50%">
								<div id="showOrgRank" style="width: 50%; display: none">
									<span align="right" style="width: 40%"><strong>机构级别</strong>:</span>
									<asp:DropDownList ID="ORGANIZATION_RANK_SEARCH" runat="server" datafld="RANK_CODE"
										datasrc="Condition">
									</asp:DropDownList>
								</div>
								<div id="showUserRank" style="width: 50%; display: none">
									<span align="right" style="width: 40%"><strong>人员级别</strong>:</span>
									<asp:DropDownList ID="USER_RANK_SEARCH" runat="server" datafld="RANK_CODE" datasrc="Condition">
									</asp:DropDownList>
								</div>
							</td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td colspan="2">
					<table class="modalEditable" style="width: 100%; height: 100%" cellpadding="0" cellspacing="0">
						<tr>
							<td colspan="2">
								<div id="frameContainer" style="display: none; width: 100%; height: 100%">
									<!--
										<iframe id='innerFrame' onreadystatechange='onFrameStateChange();' style='BORDER-RIGHT: black 1px solid; PADDING-RIGHT: 0px; BORDER-TOP: black 1px solid; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; BORDER-LEFT: black 1px solid; WIDTH: 100%; PADDING-TOP: 0px; BORDER-BOTTOM: black 1px solid; HEIGHT: 100%'>
										</iframe>
										-->
								</div>
								<div id="querySearchDiv" style="border-right: 1px solid; border-top: 1px solid; border-left: 1px solid;
									width: 100%; border-bottom: 1px solid; height: 100%">
									<table style="width: 100%; height: 100%" cellpadding="0" cellspacing="4">
										<tr style="height: 24px">
											<td nowrap align="right" style="width: 10%">
												<strong>限制:</strong>
											</td>
											<td nowrap style="40%">
												<input id="ORGANIZATIONS" style="border-right: medium none; border-top: medium none;
													border-left: medium none; border-bottom: medium none" onclick="onRadioButtonClick();"
													datafld="ORGANIZATIONS" datasrc="Condition" type="radio" name="limitRadio" checked>组织机构
												<input id="USERS" style="border-right: medium none; border-top: medium none; border-left: medium none;
													border-bottom: medium none" onclick="onRadioButtonClick();" datafld="USERS" datasrc="Condition"
													type="radio" name="limitRadio">人员
												<input id="GROUPS" style="border-right: medium none; border-top: medium none; border-left: medium none;
													border-bottom: medium none" onclick="onRadioButtonClick();" datafld="GROUPS"
													datasrc="Condition" type="radio" name="limitRadio">组
												<input id="firstPerson" style="border-right: medium none; border-top: medium none;
													border-left: medium none; border-bottom: medium none" datafld="firstPerson" datasrc="Condition"
													disabled type="checkbox">仅列出一把手
											</td>
											<td nowrap align="right" style="width: 10%">
												<strong id="nameTypeLabel">名称:</strong> </SPAN>
											</td>
											<td nowrap style="width: 25%">
												<input id="queryName" type="text" size="16" style="width: 100%" datafld="name" datasrc="Condition">
											</td>
											<td nowrap style="width: 15%">
												<a href="" title="执行查询(CTRL+E)" onclick="onQueryClick();"><span style="background-position: left 50%;
													background-image: url(../images/go.gif); width: 16px; background-repeat: no-repeat;
													height: 16px"></span>查询</a>
											</td>
										</tr>
										<tr style="width: 100%; height: 3px">
											<td colspan="9">
												<hr>
											</td>
										</tr>
										<tr>
											<td colspan="9">
												<div style="overflow: auto; width: 100%; height: 100%">
													<table id="ouUserTable" style="width: 100%" cellpadding="0" cellspacing="0">
														<thead id="ouUserHeader">
														</thead>
														<tbody id="ouUserListBody" onselectstart="return true" oncontextmenu="oBodyContextMenu()">
															<tr>
															</tr>
														</tbody>
													</table>
												</div>
											</td>
										</tr>
									</table>
								</div>
							</td>
						</tr>
					</table>
				</td>
			</tr>
			<tr style="height: 24px" id="bottomRow">
				<td>
					<strong title="在选中的组织机构中，哪些级别的人属于该角色">组织机构中的级别限制：</strong>
					<asp:DropDownList ID="USER_RANK" runat="server">
					</asp:DropDownList>
					<!--<select id="personalTitle" datafld="personalTitle">
						</select>-->
				</td>
				<td align="right">
					<input type="radio" style="border-right: medium none; border-top: medium none; border-left: medium none;
						border-bottom: medium none" id="queryTree" name="searchMethod" checked onclick="onQueryTypeChick();"><strong>组织结构树</strong>&nbsp;
					<input type="radio" style="border-right: medium none; border-top: medium none; border-left: medium none;
						border-bottom: medium none" id="querySearch" name="searchMethod" onclick="onQueryTypeChick();"><strong>查询</strong>
				</td>
			</tr>
			<tr id="timeLimitTR" style="display: none; height: 16px">
				<td colspan="2" align="center">
					<table cellpadding="0" cellspacing="0" border="0" style="width: 100%; height: 16px">
						<tr>
							<td align="right" style="width: 25%">
								<strong>开始时间</strong>:</td>
							<td align="left" style="width: 25%">
								<input type="text" id="startDate" style="width: 80px"></td>
							<td align="right" style="width: 25%">
								<strong>结束时间</strong>:</td>
							<td align="left" style="width: 25%">
								<input type="text" id="endDate" style="width: 80px"></td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td colspan="2" style="height: 10px">
					<hr>
				</td>
			</tr>
			<tr>
				<td colspan="2" style="height: 24px">
					<table style="width: 100%; height: 100%" cellspacing="0">
						<tr>
							<td align="center">
								<input accesskey="O" style="width: 80px" disabled type="button" value="确定(O)" id="btnOK"
									onclick="onSaveClick();">
							</td>
							<td align="center">
								<input accesskey="C" style="width: 80px" type="button" value="取消(C)" id="btnCancel"
									onclick="onCancelClick();">
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</form>
</body>
</html>
