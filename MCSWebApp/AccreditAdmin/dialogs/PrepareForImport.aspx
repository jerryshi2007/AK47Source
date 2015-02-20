<%@ Page Language="c#" Codebehind="PrepareForImport.aspx.cs" AutoEventWireup="True"
	Inherits="MCS.Applications.AccreditAdmin.dialogs.PrepareForImport" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
	<title>数据导入准备</title>
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
</head>
<body ms_positioning="GridLayout">
	<form id="Form1" method="post" runat="server">
		<table id="" style="width: 95%; height: 100%" cellspacing="0" cellpadding="0" align="center"
			border="0">
			<thead style="height: 32px">
				<tr>
					<td colspan="9">
						<span style="background-position: center center; background-image: url(../images/32/person.gif);
							width: 32px; background-repeat: no-repeat; height: 32px"></span><font size="4"><strong
								id="userTitle">数据导入准备</strong></font>
						<hr style="height: 2px">
					</td>
				</tr>
			</thead>
			<tbody>
				<tr style="height: 16px">
					<td>
						<strong>请选择需要导入的Excel文件</strong>:</td>
				</tr>
				<tr>
					<td id="middleTD" align="center" runat="server">
						<input id="importFile" style="width: 95%" type="file" runat="server"></td>
				</tr>
			</tbody>
			<tfoot style="height: 50px" valign="middle">
				<tr>
					<td align="center" colspan="9">
						<hr style="height: 2px">
						<asp:Button ID="btnOK" AccessKey="O" Text="确定(O)" runat="server" Width="80"></asp:Button><span
							style="width: 100px"></span><input id="btnCancel" style="width: 80px" accesskey="C"
								onclick="window.close()" type="button" value="关闭(C)" name="btnCancel">
					</td>
				</tr>
			</tfoot>
		</table>
	</form>
</body>
</html>
