<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="cloneRoleSelectorControlTest.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.RoleControl.cloneRoleSelectorControlTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>测试Clone RoleGraphControl</title>
	<script type="text/javascript">
		function onSelectRole() {
			var selector = $find("roleSelector");

			selector.showDialog();

			$get("selectedResult").innerText = selector.get_selectedFullCodeName();
		}

		function onCloneComponent() {
			var parent = $get("container");

			var template = $find("roleSelector");

			var selector = template.cloneAndAppendToContainer(parent);

			selector.showDialog();

			$get("selectedResult").innerText = selector.get_selectedFullCodeName();
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div id="container">
		<SOA:RoleGraphControl ID="roleSelector" runat="server" DialogTitle="选择角色" />
	</div>
	<div>
		<input type="button" id="selectRoleBtn" value="选择角色..." onclick="onSelectRole();" />
	</div>
	<div>
		<input type="button" onclick="onCloneComponent();" value="Clone Component" />
	</div>
	<div id="selectedResult" runat="server">
	</div>
	</form>
</body>
</html>
