<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoleSelectTest.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.RoleControl.RoleSelectTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>角色选择控件测试</title>
	<script type="text/javascript">
		function onSelectRole() {
			var selector = $find("roleSelector");

			selector.showDialog()

			$get("selectedResult").innerText = selector.get_selectedFullCodeName();
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<SOA:RoleGraphControl ID="roleSelector" runat="server" DialogTitle="选择角色" />
		<input type="button" id="selectRoleBtn" value="选择角色..." onclick="onSelectRole();" />
		<asp:Button runat="server" ID="postButton" Text="PostBack" />
	</div>
	<div id="selectedResult" runat="server">
	</div>
	</form>
</body>
</html>
