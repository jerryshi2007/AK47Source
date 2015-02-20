<%@ Page Language="c#" Codebehind="SystemDataStat.aspx.cs" AutoEventWireup="True"
	Inherits="MCS.Applications.AccreditAdmin.dialogs.SystemDataStat" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
	<title>数据导出结果</title>
	<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
	<meta name="CODE_LANGUAGE" content="C#">
	<meta name="vs_defaultClientScript" content="JavaScript">
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	<link href="../CSS/Input.css" type="text/css" rel="stylesheet">

	<script type="text/javascript" language="javascript" src="../oguScript/validate.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/dateTime.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/xmlHttp.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/xsdAccess.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/uiScript.js"></script>

	<script type="text/javascript" language="javascript" src="../Script/ApplicationRoot.js"></script>

	<script type="text/javascript" language="javascript" src="./SystemDataStat.js"></script>

</head>
<body ms_positioning="GridLayout" onload="onDocumentLoad()">
	<xml id="xmlResultClient">
			<asp:Xml id="xmlResultServer" runat="server"></asp:Xml>
		</xml>
	<xml id="xmlResultClientParam">
			<asp:Xml ID="xmlResultServerParam" Runat="server"></asp:Xml>
		</xml>
	<form id="frmInput" method="post" runat="server" style="width: 100%">
		<table cellpadding="0" cellspacing="0" border="0" align="center" id="allDataTable">
			<thead height="32px">
				<tr>
					<td align="center">
						<font size="4">
							<asp:Literal ID="resultTitle" runat="server"></asp:Literal></font><hr>
					</td>
				</tr>
				<tr height="32px">
					<td align="center">
						<input type="button" value="Excel标准导出(S)" accesskey="S" style="width: 120px" onclick="onPrintClick()">
						<input type="button" value="excel展现(E)" accesskey="E" style="width: 120px" onclick="onExcelSpanClick()">
						<input type="button" value="Word展现(W)" accesskey="W" style="width: 120px" onclick="onWordSpanClick()">
						<input type="button" value="关闭(X)" accesskey="X" style="width: 120px" onclick="window.close()">
						<hr>
					</td>
				</tr>
			</thead>
			<tbody>
				<tr>
					<td>
						<table cellpadding="0" cellspacing="0" border="1" id="bTable" runat="server">
						</table>
					</td>
				</tr>
			</tbody>
			<tfoot height="32px">
				<tr>
					<td align="center">
						<hr>
						<input type="button" value="Excel标准导出(S)" accesskey="S" style="width: 120px" onclick="onPrintClick()">
						<input type="button" value="excel展现(E)" accesskey="E" style="width: 120px" onclick="onExcelSpanClick()">
						<input type="button" value="Word展现(W)" accesskey="W" style="width: 120px" onclick="onWordSpanClick()">
						<input type="button" value="关闭(X)" accesskey="X" style="width: 120px" onclick="window.close()">
					</td>
				</tr>
			</tfoot>
		</table>
	</form>
</body>
</html>
