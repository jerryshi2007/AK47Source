<%@ Page Language="c#" Codebehind="OGUSelect.aspx.cs" AutoEventWireup="True" Inherits="MCS.Applications.AccreditAdmin.exports.OGUSelect" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
	<title>请选择人员或部门</title>
	<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
	<meta http-equiv="Content-Type" content="text/html; charset=gb2312">
	<meta name="CODE_LANGUAGE" content="C#">
	<meta name="vs_defaultClientScript" content="JavaScript">
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	<link href="../CSS/Input.css" type="text/css" rel="stylesheet">

	<script type="text/javascript" language="javascript" src="../oguScript/validate.js"></script>

	<script type="text/javascript" language="javascript" src="../Script/ApplicationRoot.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/uiScript.js"></script>

	<script type="text/javascript" language="javascript" src="OGUSelect.js"></script>

</head>
<body class="modal" onload="onDocumentLoad();">
	<table style="width: 100%; height: 100%" id="inputTable">
		<tr>
			<td colspan="2" style="height: 32px">
				<strong><font size="4"></font></strong><strong><font size="4"></font></strong><span
					id="logo" style="background-position: center center; 
					background-image: url(../images/32/person.gif); width: 32px; background-repeat: no-repeat;
					height: 32px"></span><font size="4"><strong id="Caption">请选择人员或部门</strong></font>
				<hr>
			</td>
		</tr>
		<tr>
			<td colspan="2">
				<table class="modalEditable" style="width: 100%; height: 100%" cellpadding="0" cellspacing="4"
					id="Table2">
					<tr>
						<td style="height: 20px">
							<strong>人员或部门列表:</strong>
						</td>
					</tr>
					<tr>
						<td>
							<select size="2" id="dnList" ondblclick="onOKClick();" style="width: 100%; height: 100%">
							</select>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		</TR>
		<tr>
			<td colspan="2" style="height: 10px">
				<hr>
			</td>
		</tr>
		<tr>
			<td colspan="2" style="height: 24px">
				<table style="width: 100%; height: 100%" cellspacing="0" id="Table1">
					<tr>
						<td align="middle">
							<input accesskey="O" style="width: 80px" type="button" value="确定(O)" id="btnOK" onclick="onOKClick();">
						</td>
						<td align="middle">
							<input accesskey="C" style="width: 80px" type="button" value="取消(C)" id="btnCancel"
								onclick="onCancelClick();">
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</body>
</html>
