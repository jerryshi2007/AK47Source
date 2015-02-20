<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrontDeleteDialog.aspx.cs"
	Inherits="PermissionCenter.Dialogs.FrontDeleteDialog" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" scroll="no" style="overflow: hidden">
<head runat="server">
	<link rel="icon" href="../favicon.ico" type="image/x-icon" />
	<link rel="Shortcut Icon" href="../favicon.ico" />
	<link href="../styles/dlg.css" rel="stylesheet" type="text/css" />
	<title>批量删除</title>
	<style type="text/css">
		.pc-deletelist
		{
			list-style: none;
			margin: 0;
		}
		.pc-deletelist li
		{
			display: block;
			clear: both;
			padding: 0;
			height: 32px;
			border-bottom: 1px dotted #0f0f0f;
		}
	</style>
	<style type="text/css"></style>
</head>
<body class="pcdlg">
	<form id="form1" runat="server">
	<div class="pcdlg-sky">
		<h1 class="pc-caption">
			确认要删除的对象</h1>
	</div>
	<div class="pcdlg-content">
		<div class="pc-container5" style="overflow: auto">
			<asp:HiddenField runat="server" ID="context" />
			<asp:Repeater ID="mainList" runat="server">
				<HeaderTemplate>
					<ul class="pc-deletelist">
				</HeaderTemplate>
				<ItemTemplate>
					<li data-key="<%# PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID")) %>">
						<div>
							<asp:Label runat="server" Text='<%# Server.HtmlEncode((string)Eval("DisplayName")) %>'> </asp:Label>
						</div>
					</li>
				</ItemTemplate>
				<FooterTemplate>
					</ul>
				</FooterTemplate>
			</asp:Repeater>
		</div>
	</div>
	<div class="pcdlg-floor">
		<div class="pcdlg-button-bar">
			<input type="button" onclick="return onOkClick()" accesskey="S" class="pcdlg-button btn-def"
				value="删除以上对象(S)" /><input type="button" accesskey="C" class="pcdlg-button btn-cancel"
					onclick="return onCancelClick();" value="取消(C)" />
		</div>
	</div>
	</form>
	<script type="text/javascript">
		function onOkClick() {

		}

		function onCancelClick() {

		}
	</script>
</body>
</html>
