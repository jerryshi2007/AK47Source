<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnitBrowseDialog.aspx.cs"
	Inherits="PermissionCenter.UnitBrowseDialog" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>机构人员选择</title>
	<link rel="icon" href="../favicon.ico" type="image/x-icon" />
	<link rel="Shortcut Icon" href="../favicon.ico" />
	<base target="_self" />
	<link href="../styles/dlg.css" rel="stylesheet" type="text/css" />
	<pc:HeaderControl ID="HeaderControl1" runat="server">
	</pc:HeaderControl>
</head>
<body class="pcdlg">
	<form id="form1" runat="server">
	<div class="pcdlg-sky">
		<h1 class="pc-caption">
			机构人员选择</h1>
	</div>
	<div class="pcdlg-content">
		<div class="pc-container5">
			<mcs:DeluxeTree runat="server" ID="tree" CssClass="pc-mcstree" CollapseImage="" ExpandImage=""
				InvokeWithoutViewState="True" NodeCloseImg="" NodeOpenImg="" OnNodeSelecting="onNodeSelecting"
				OnGetChildrenData="tree_GetChildrenData">
				<Nodes>
				</Nodes>
			</mcs:DeluxeTree>
		</div>
	</div>
	<div class="pcdlg-floor">
		<div class="pcdlg-button-bar">
			<asp:Button ID="Button1" Text="确定" runat="server" CssClass="pcdlg-button" OnClientClick="doConfirm()" /><input
				type="button" class="pcdlg-button" value="取消" onclick="doCancel();" />
		</div>
	</div>
	<script type="text/javascript">

		function onNodeSelecting(sender, e) {


		}

		function finishDialog(rstString) {
			if (typeof (window.dialogArguments) == "object") {
				if ("fillElem" in window.dialogArguments) {
					window.dialogArguments.fillElem.value = rstString;
					window.returnValue = true;
					window.close();
				} else {
					alert("调用方式不正确");
				}
			}
		}

		function doConfirm() {
			var nodes = $find("tree").get_multiSelectedNodes();
			if (nodes.length) {
				var arr = [];
				for (var i = nodes.length - 1; i >= 0; i--) {
					arr.push(nodes[i].get_value());
				}

				finishDialog(arr.join(','));
			} else {
				alert('至少选择一项');
			}
		}

		function doCancel() {
			window.returnValue = false;
			window.close();
		}

	</script>
	</form>
</body>
</html>
