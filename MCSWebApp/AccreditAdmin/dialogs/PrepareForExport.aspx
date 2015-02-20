<%@ Page Language="c#" Codebehind="PrepareForExport.aspx.cs" AutoEventWireup="True"
	Inherits="MCS.Applications.AccreditAdmin.dialogs.PrepareForExport" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
	<title>数据导出准备</title>
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<link href="../CSS/Input.css" type="text/css" rel="stylesheet">

	<script type="text/javascript" language="javascript" src="../oguScript/validate.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/dateTime.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/xmlHttp.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/xsdAccess.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/uiScript.js"></script>

	<script type="text/javascript" language="javascript" src="../Script/ApplicationRoot.js"></script>

	<script type="text/javascript" language="javascript" src="./PrepareForExport.js"></script>

</head>
<body onload="onDocumentLoad()" ms_positioning="GridLayout">
	<xml id="xmlOption" src="../xml/ExportOrImport.xml"></xml>
	<form id="frmInput" action="SystemDataStat.aspx" method="post" target="_blank">
		<table id="" style="width: 95%; height: 100%" cellspacing="0" cellpadding="0" align="center"
			border="0">
			<thead style="height: 32px">
				<tr>
					<td colspan="9">
						<span style="background-position: center center; background-image: url(../images/32/person.gif);
							width: 32px; background-repeat: no-repeat; height: 32px"></span><font size="4"><strong
								id="userTitle">数据导出准备</strong></font>
						<hr style="height: 2px">
					</td>
				</tr>
			</thead>
			<tbody>
				<tr style="height: 20px">
					<td align="right">
						<strong>系统位置</strong>:<input id="rootOrganizationGuid" name="rootOrganizationGuid"
							type="hidden" runat="server"></td>
					<td>
						<asp:Literal ID="sysPositionShow" runat="server"></asp:Literal></td>
				</tr>
				<tr style="height: 20px">
					<td valign="middle" align="right">
						<strong>类型选择</strong>:</td>
					<td valign="middle">
						<!--input id="dataType" name="dataType" type="hidden"-->
						<input id="ExportOrganization" name="ExportOrganization" style="border-right: 0px;
							border-top: 0px; border-left: 0px; border-bottom: 0px" type="checkbox" value="1">机构<span
								style="width: 30px"></span>
						<input id="ExportGroup" name="ExportGroup" style="border-right: 0px; border-top: 0px;
							border-left: 0px; border-bottom: 0px" type="checkbox" value="4">人员组<span style="width: 30px"></span>
						<input id="ExportUser" name="ExportUser" style="border-right: 0px; border-top: 0px;
							border-left: 0px; border-bottom: 0px" type="checkbox" value="2">人员
					</td>
				</tr>
				<tr style="height: 20px">
					<td align="right">
						<strong>数据项选择</strong>:</td>
					<td>
						&nbsp;</td>
				</tr>
				<tr>
					<td colspan="2">
						<table style="width: 100%; height: 100%" cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td align="center">
									<select id="selOriginal" style="width: 180px; height: 100%" multiple>
									</select>
								</td>
								<td style="width: 60px" valign="middle" align="center">
									<input style="width: 50px" onclick="onSelectMoveAll(selOriginal, selSelected)" type="button"
										value=">>"><br>
									<br>
									<input style="width: 50px" onclick="onSelectMoveMulit(selOriginal, selSelected)"
										type="button" value=">"><br>
									<br>
									<input style="width: 50px" onclick="onSelectMoveMulit(selSelected, selOriginal)"
										type="button" value="<"><br>
									<br>
									<input style="width: 50px" onclick="onSelectMoveAll(selSelected, selOriginal)" type="button"
										value="<<">
								</td>
								<td align="center" valign="middle">
									<input type="hidden" id="dataColumns" name="dataColumns">
									<select id="selSelected" multiple style="width: 180px; height: 100%">
									</select>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</tbody>
			<tfoot style="height: 50px" valign="middle">
				<tr>
					<td align="center" colspan="9">
						<hr style="height: 2px">
						<input id="btnOK" name="btnOK" style="width: 80px" type="button" value="确定(O)" accesskey="O"
							onclick="onOKCick()"><span style="width: 100px"></span>
						<input id="btnCancel" name="btnCancel" style="width: 80px" type="button" value="取消(C)"
							accesskey="C" onclick="onCancelClick()">
					</td>
				</tr>
			</tfoot>
		</table>
	</form>
</body>
</html>
