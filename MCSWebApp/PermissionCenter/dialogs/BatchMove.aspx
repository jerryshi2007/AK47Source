<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BatchMove.aspx.cs" Inherits="PermissionCenter.Dialogs.BatchMove" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" scroll="no" style="overflow: hidden">
<head runat="server">
	<title>移动/复制分支操作</title>
	<base target="_self" />
	<link rel="icon" href="../favicon.ico" mce_href="favicon.ico" type="image/x-icon" />
	<link rel="Shortcut Icon" href="../favicon.ico" mce_href="favicon.ico" />
	<link href="../styles/dlg.css" rel="stylesheet" type="text/css" />
</head>
<body style="width: 620px">
	<form id="form1" runat="server" target="_self">
	<asp:ScriptManager runat="server" />
	<div class="pcdlg-sky">
		<h1 class="pc-caption">
			分支操作选择</h1>
	</div>
	<div class="pcdlg-content">
		<div class="pc-container5">
			<h1 class="caption">
				可用的操作</h1>
			<p>
				操作限制：您必须对所在组织有删除权限才可以进行移动操作，根组织中的一级组织无法移动，根组织无法作为目标。</p>
			<asp:Repeater runat="server" ID="actions">
				<HeaderTemplate>
					<ul class="pc-metro-list">
				</HeaderTemplate>
				<ItemTemplate>
					<li class="pc-metro-item"><a href="#" class="pc-metro-content" data-type='<%#Eval("Mode") %>'
						onclick="doSelect(this)">
						<%#Server.HtmlDecode((string)Eval("Description")) %></a> </li>
				</ItemTemplate>
				<FooterTemplate>
					</ul>
				</FooterTemplate>
			</asp:Repeater>
		</div>
	</div>
	<div class="pcdlg-floor">
		<div class="pcdlg-button-bar">
			<input type="button" class="pcdlg-button" value="取消" onclick="doCancel();" />
		</div>
	</div>
	</form>
	<script type="text/javascript">
		$pc.ui.traceWindowWidth();
		function doSelect(elem) {
			if (elem.getAttribute) {
				window.returnValue = elem.getAttribute("data-type");
			} else {
				window.returnValue = elem['data-type'];
			}
			window.close();
		}

		function doCancel() {
			window.returnValue = '';
			window.close();
		}
	</script>
</body>
</html>
