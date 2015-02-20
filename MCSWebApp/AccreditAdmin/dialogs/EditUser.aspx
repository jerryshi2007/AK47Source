<%@ Page Language="c#" Codebehind="EditUser.aspx.cs" AutoEventWireup="True" Inherits="MCS.Applications.AccreditAdmin.dialogs.EditUser" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns:hgui>
<head>
	<title>�û���Ϣ</title>
	<meta http-equiv="Content-Type" content="text/html; charset=gb2312">
	<?import namespace="hGui" implementation="../htc/calendar.htc" />
	<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<link href="../CSS/Input.css" type="text/css" rel="stylesheet">

	<script type="text/javascript" language="javascript" src="../oguScript/validate.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/dateTime.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/xmlHttp.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/xsdAccess.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/uiScript.js"></script>

	<script type="text/javascript" language="javascript" src="../SelfScript/AccreditAdmin.js"></script>

	<script type="text/javascript" language="javascript" src="../Script/ApplicationRoot.js"></script>

	<script type="text/javascript" language="javascript" src="EditUser.js"></script>

</head>
<body class="modal" onload="onDocumentLoad();">
	<xml id="USER_XSD" src="../xsd/PageUser.xsd"></xml>
	<xml id="AConfig" src="../xml/AccreditConfig.xml"></xml>
	<hgui:hmenu id="pinyinMenu" style="visibility: hidden; behavior: url(../htc/hMenu.htc);
		position: absolute" onmenuclick="pinyinMenuClick();">
	</hgui:hmenu>	
	<hgui:calendar id="hCalendar" style="z-index: 101; left: 10px; visibility: hidden;
		overflow: hidden; width: 160px; position: absolute; top: 15px; height: 200px">
		<strong><font size="4" color="#ff0099"></font></strong>
	</hgui:calendar>
	<table style="width: 99%; height: 100%" align="center">
		<tr>
			<td style="height: 32px">
				<span style="background-position: center center; background-image: url(../images/32/user.gif);
					width: 32px; background-repeat: no-repeat; height: 32px"></span><font size="4"><strong
						id="userTitle">�û���Ϣ</strong></font>
				<hr>
			</td>
		</tr>
		<tr>
			<td>
				<form id="frmInput" method="post" runat="server">
					<input type="hidden" id="parentAllPathName" runat="server">
					<input type="hidden" id="userData" runat="server" value="0">
					<input datafld="SIDELINE" id="SIDELINE" datasrc="USERS" type="hidden">
					<input type="hidden" id="opPermission" runat="server" name="opPermission">
					<input type="hidden" id="searchName" datafld="SEARCH_NAME" datasrc="USERS">
					<table class="modalEditable" style="width: 100%; height: 100%" id="userContentTable">
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>��</strong>:
							</td>
							<td>
								<input datafld="LAST_NAME" id="LAST_NAME" style="width: 90%" datasrc="USERS" type="text"
									onchange="onNameChange()" name="LAST_NAME">
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>��</strong>:
							</td>
							<td>
								<input datafld="FIRST_NAME" id="FIRST_NAME" style="width: 90%" datasrc="USERS" type="text"
									onchange="onNameChange()" name="FIRST_NAME">
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>��������</strong>:
							</td>
							<td>
								<input datafld="OBJ_NAME" id="OBJ_NAME" style="width: 90%" datasrc="USERS" type="text"
									onchange="onObjNameChange()" name="OBJ_NAME">
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>��ʾ����</strong>:
							</td>
							<td>
								<input datafld="DISPLAY_NAME" id="DISPLAY_NAME" style="width: 90%" datasrc="USERS"
									name="DISPLAY_NAME">
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>��¼����</strong>:
							</td>
							<td>
								<input datafld="LOGON_NAME" id="LOGON_NAME" style="width: 90%" datasrc="USERS" name="LOGON_NAME">
								<span id="pinyinSpan" style="background-image: url(../images/downArrow.gif); width: 16px;
									cursor: hand; background-repeat: no-repeat; height: 16px" onclick="onPinyinSpanPopup();">
								</span>
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>��Ա���</strong>:
							</td>
							<td>
								<input datafld="PERSON_ID" id="PERSON_ID" style="width: 90%" datasrc="USERS" name="PERSON_ID">
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>ƴ��</strong>:
							</td>
							<td>
								<input datafld="PINYIN" id="PINYIN" style="width: 90%" datasrc="USERS" name="PINYIN">
								<span id="newPinyinSpan" style="background-image: url(../images/downArrow.gif); width: 16px;
									cursor: hand; background-repeat: no-repeat; height: 16px" onclick="onNewPinyinSpanPopup();">
								</span>
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>IC�����</strong>:</td>
							<td>
								<input datafld="IC_CARD" id="IC_CARD" style="width: 90%" datasrc="USERS" name="IC_CARD">
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>ϵͳλ��</strong>:
							</td>
							<td>
								<input datafld="ALL_PATH_NAME" id="ALL_PATH_NAME" style="width: 90%" datasrc="USERS"
									disabled name="ALL_PATH_NAME">
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>��������</strong>:
							</td>
							<td>
								<asp:DropDownList dataFld="RANK_CODE" ID="RANK_CODE" dataSrc="USERS" runat="server">
								</asp:DropDownList>
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>����ְ��</strong>:
							</td>
							<td>
								<input datafld="RANK_NAME" id="RANK_NAME" style="width: 90%" datasrc="USERS" name="RANK_NAME">
							</td>
						</tr>
						<!--tr>
								<td style="WIDTH: 80px; HEIGHT: 24px" align="right">
									<STRONG>��ְ���</STRONG>:
								</td>
								<td>
									<input dataFld="SIDELINE" id="SIDELINE" style="BORDER-RIGHT:medium none; BORDER-TOP:medium none; BORDER-LEFT:medium none; BORDER-BOTTOM:medium none"
										dataSrc="USERS" type="checkbox">��ְ�ɲ�
								</td>
							</tr-->
						<tr style="display: none;">
							<td style="width: 80px; height: 24px" align="right">
								<strong>��Ա����</strong>:
							</td>
							<td>
								<!--select id="ATTRIBUTES" dataFld="ATTRIBUTES" dataSrc="USERS">
										<option value="0" selected>
											һ���Ա
										</option>
										<option value="1">
											�����Ա
										</option>
										<option value="2">
											��ܸɲ�
										</option>
										<option value="4">
											�����ɲ�
										</option>
										<option value="8">
											����ɲ�
										</option>
									</select-->
								<input id="ATTRIBUTES" datafld="ATTRIBUTES" type="hidden" datasrc="USERS">
								<table style="width: 100%; height: 100%">
									<tr>
										<td style="width: 33%" align="left">
											<input id="ATTRIBUTES_0" type="checkbox" style="border: none" onclick="onSetUserAttributes()"
												value="1"><strong>��ܸɲ�</strong></td>
										<td style="width: 34%" align="left">
											<input id="ATTRIBUTES_1" type="checkbox" style="border: none" onclick="onSetUserAttributes()"
												value="2"><strong>�����Ա</strong></td>
										<td style="width: 33%" align="left">
											<input id="ATTRIBUTES_2" type="checkbox" style="border: none" onclick="onSetUserAttributes()"
												value="4"><strong>�����ɲ�</strong></td>
									</tr>
									<tr>
										<td style="width: 33%" align="left">
											<input id="ATTRIBUTES_3" type="checkbox" style="border: none" onclick="onSetUserAttributes()"
												value="8"><strong>����ɲ�</strong></td>
										<td style="width: 34%" align="left">
											<input id="ATTRIBUTES_4" type="checkbox" style="border: none" onclick="onSetUserAttributes()"
												value="16"><strong>���쵼ְ��</strong></td>
										<td style="width: 33%" align="left">
											<input id="ATTRIBUTES_5" type="checkBox" style="border: none" onclick="onSetUserAttributes()"
												value="32"><strong>�Ǻ�����Ա</strong></td>
									</tr>
								</table>
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>��������</strong>:
							</td>
							<td>
								<input datafld="CREATE_TIME" id="CREATE_TIME" datasrc="USERS" readonly type="text">
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>����ʱ��</strong>:
							</td>
							<td>
								<input datafld="START_TIME" id="START_TIME" datasrc="USERS" type="text">
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>����ʱ��</strong>:
							</td>
							<td>
								<input datafld="END_TIME" id="END_TIME" datasrc="USERS" type="text">
							</td>
						</tr>
						<!--tr>
								<td style="WIDTH: 80px; HEIGHT: 24px" align="right">
									<STRONG>��AD�˺�</STRONG>:
								</td>
								<td>
									<input dataFld="AD_COUNT" id="AD_COUNT" style="BORDER-RIGHT:medium none; BORDER-TOP:medium none; BORDER-LEFT:medium none; BORDER-BOTTOM:medium none"
										dataSrc="USERS" type="checkbox">��AD�˺�
								</td>
							</tr-->
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>�˺�Ҫ��</strong>:
							</td>
							<td>
								<select datafld="POSTURAL" id="POSTURAL" datasrc="USERS" onchange="onPosturalChange()">
									<option value="1">�˺Ž���</option>
									<option value="2" selected>Ҫ���û��´ε�¼ʱ�޸�����</option>
									<option value="4">�û�������������</option>
								</select>
							</td>
						</tr>
						<tr>
							<td style="width: 80px; height: 24px" align="right">
								<strong>��ע��Ϣ</strong>:
							</td>
							<td>
								<textarea datafld="DESCRIPTION" id="DESCRIPTION" style="width: 90%; height: 100%"
									datasrc="USERS"></textarea>
							</td>
						</tr>
					</table>
				</form>
			</td>
		</tr>
		<tr>
			<td style="height: 10px">
				<hr>
			</td>
		</tr>
		<tr>
			<td style="height: 24px">
				<table style="width: 100%; height: 100%" cellspacing="0">
					<tr>
						<td align="center">
							<input id="btnOK" accesskey="O" value="ȷ��(O)" onclick="onSaveClick()" type="button"
								style="width: 80px">
						</td>
						<td align="center">
							<input id="btnCancel" style="width: 80px" accesskey="C" onclick="window.close();"
								type="button" value="ȡ��(C)">
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</body>
</html>
