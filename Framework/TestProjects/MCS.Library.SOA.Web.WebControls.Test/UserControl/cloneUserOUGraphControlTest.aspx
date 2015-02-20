<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="cloneUserOUGraphControlTest.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.UserControl.cloneUserOUGraphControlTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Clone User Graph Control</title>
	<script type="text/javascript">
		function onCloneComponent() {
			var parent = $get("container");

			var template = $find("graphTemplate");

			template.cloneAndAppendToContainer(parent);
		}

		function onTreeNodeSelecting(sender, e) {
			addMessage(e.object.fullPath);
			e.cancel = false;
		}

		function addMessage(msg) {
			result.innerHTML += "<p style='margin:0'>" + msg + "</p>";
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<SOA:UserOUGraphControl ID="graphTemplate" runat="server" Width="420px" Height="300px"
			SelectMask="User" ShowingMode="Normal" BorderStyle="solid" BorderColor="black"
			BorderWidth="1" OnNodeSelecting="onTreeNodeSelecting" RootExpanded="true" />
	</div>
	<div>
		<input type="button" onclick="onCloneComponent();" value="Clone Component" />
	</div>
	<div id="resultContainer">
		<div id="result" contenteditable="true" style="overflow: auto; border: 1px silver solid;
			width: 100%; height: 200px" runat="server">
		</div>
	</div>
	<div id="container">
	</div>
	</form>
</body>
</html>
