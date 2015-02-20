<%@ Page Language="c#" Codebehind="UserLogList.aspx.cs" AutoEventWireup="True" Inherits="MCS.Applications.AppAdmin_LOG.logList.UserLogList" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns:hgui>
<head>
	<title>��־�б�</title>
	<meta http-equiv="Content-Type" content="text/html; charset=gb2312">
	<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<?import namespace="hGui" implementation="../htc/Calendar.htc" />
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<link href="../css/Input.css" type="text/css" rel="stylesheet">

	<script type="text/javascript" language="javascript" src="../script/validate.js"></script>

	<script type="text/javascript" language="javascript" src="../script/xmlHttp.js"></script>

	<script type="text/javascript" language="javascript" src="../script/xAD.js"></script>

	<script type="text/javascript" language="javascript" src="../Script/ApplicationRoot.js"></script>

	<script type="text/javascript" language="javascript" src="../script/htcCommon.js"></script>

	<script type="text/javascript" language="javascript" src="../script/uiScript.js"></script>

	<script type="text/javascript" language="javascript" src="../script/dbGrid.js"></script>

	<script type="text/javascript" language="javascript" src="dbGridPublic.js"></script>

	<script type="text/javascript" language="javascript" src="UserLogList.js"></script>

</head>
<body leftmargin="0" topmargin="0" onload="onDocumentLoad();">
	<xml id="headUserXml" src="../xml/UserLogHead.xml"></xml>
	<xml id="AppNameXml">
			<asp:Xml ID="HiddenXml" Runat="server"></asp:Xml>
		</xml>
	<input type="hidden" id="secFrm" runat="server" name="secFrm">
	<input type="hidden" id="inputParam" onpropertychange="onInputParamChange();">
	<hgui:calendar id="hCalendar" style="z-index: 101; left: 10px; visibility: hidden;
		overflow: hidden; width: 180px; position: absolute; top: 15px; height: 200px">
		<strong><font size="4" color="#ff0099"></font></strong>
	</hgui:calendar>
	<div class="toolbarPanel" style="width: 100%; height: 18px" onclick="onToolbarClick();"
		onmouseover="onToolbarMouserOver();" onmouseout="onToolbarMouserOut();" id="toolBar">
		<span id="newSpan">
			<img align="absMiddle" id="newObj" src="../images/insert.gif" title="�½�" class="toolButton">
			<span class="toolSplit">|</span></span><span><img align="absMiddle" id="updateObj"
				src="../images/edit.gif" title="�޸�����" class="toolButton"><span class="toolSplit">|</span></span><span><img
					align="absMiddle" id="refresh" src="../images/refresh.gif" title="ˢ��" class="toolButton"><span
						class="toolSplit">|</span> </span><span>
							<img align="absMiddle" id="delete" src="../images/delete.gif" title="ɾ����ǰӦ�û��������"
								class="toolButton"></span>
	</div>
	<table style="width: 100%" cellspacing="0" cellpadding="0" align="center">
		<tr valign="top" height="35">
			<td>
				<form id="frmInput">
					<table style="width: 90%; height: 100%" cellspacing="0" cellpadding="0" align="center">
						<tr>
							<td>
								<table style="width: 100%; height: 41px" cellspacing="0" cellpadding="0" align="center">
									<tr align="middle" height="20px">
										<td id="app_title" style="width: 15%">
											<strong>Ӧ������ :</strong>
										</td>
										<td id="op_title" style="width: 15%">
											<strong>������������ :</strong>
										</td>
										<td id="time_title" style="width: 25%">
											<strong>ʱ�� :</strong>
										</td>
										<td id="user_title" style="width: 20%">
											<strong>�û� :</strong>
										</td>
										<td id="fileID_title" style="width: 15%">
											<strong>�ļ���� :</strong>
										</td>
									</tr>
									<tr valign="center" align="middle">
										<td id="app_td">
											<select datafld="APPNAME" id="appName" style="width: 80%" datasrc="USER_OPEATION_LOG"
												onchange="onSelectClick()">
											</select>
										</td>
										<td id="opType_td">
											<select datafld="DISPLAYNAME" id="opTypeName" style="width: 80%" datasrc="USER_OPEATION_LOG">
											</select>
										</td>
										<td id="time_td">
											<span>
												<table height="100%" cellspacing="0" cellpadding="0" width="100%" align="center">
													<tr valign="center" align="middle">
														<td style="width: 45%">
															<input id="start_time" style="width: 80%" type="text" maxlength="10">
														</td>
														<td width="10">
															<font color="#ff0000" size="2"><strong>��</strong></font>
														</td>
														<td>
															<input id="end_time" style="width: 80%" type="text" maxlength="10">
														</td>
													</tr>
												</table>
											</span>
										</td>
										<td id="uer_td">
											<input id="userInput" type="text" maxlength="64" width="90%"></td>
										<td id="fileID_td">
											<input id="fileIDInput" type="text" maxlength="64" width="90%">
										</td>
										<td colspan="2" width="20%" align="center">
											<input type="button" id="search" onclick="onSearchClick();" height="100%" width="50%"
												value="GO(G)!" accesskey="G" style="color: white; background-color: #cc0000">
										</td>
									</tr>
								</table>
							</td>
						</tr>
					</table>
				</form>
			</td>
		</tr>
		<tr id="mainRow" style="height: 450px" valign="top">
			<td>
				<table style="width: 95%" cellspacing="0" cellpadding="0" align="center">
					<tr>
						<td>
							<hgui:dbgrid id="adminDbGrid" style="behavior: url(../htc/dbGrid.htc); width: 100%"
								oncalcdata="onGridCalcCell();" onnextpage="onGridNextPage();" showtitle="true"
								showpageindex="true" limitrows="20" bordercollapse="collapse" border="1" bordercolor="black"
								showcaption="true">
							</hgui:dbgrid></td>
					</tr>
				</table>
			</td>
		</tr>
		<td height="40%" valign="bottom" align="center">
			<br>
			<hr color="#00cc66" width="90%" size="3">
			<br>
			<span>����λ��: �û���־��� <a href="SysLogList.aspx" target="_self">������ӵ�ϵͳ��־���ҳ��</a> </span>
		</td>
		</tr>
	</table>
</body>
</html>
