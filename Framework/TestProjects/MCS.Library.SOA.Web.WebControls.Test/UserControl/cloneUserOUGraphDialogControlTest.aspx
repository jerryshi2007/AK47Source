<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="cloneUserOUGraphDialogControlTest.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.UserControl.cloneUserOUGraphDialogControlTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Clone User Graph Dialog Control</title>
	<script type="text/javascript">
		function onUserSelectDialogConfirmed(sender) {
			displaySelectedObjects(sender.get_selectedObjects());
		}

		function displaySelectedObjects(objs) {
			for (var i = 0; i < objs.length; i++)
				addMessage(objs[i].fullPath);
		}

		function addMessage(msg) {
			result.innerHTML += "<p style='margin:0'>" + msg + "</p>";
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div style="display: none">
		<SOA:UserOUGraphControl ID="userMultiSelector" runat="server" Width="420px" Height="400px"
			SelectMask="All" ShowingMode="Dialog" BorderStyle="solid" BorderColor="black"
			BorderWidth="1" MultiSelect="true" ControlIDToShowDialog="LinkButtonMultiUserSelector"
			OnDialogConfirmed="onUserSelectDialogConfirmed" RootExpanded="true" MergeSelectResult="false"
			ShowDeletedObjects="false" />
	</div>
	<div>
		<asp:LinkButton ID="LinkButtonMultiUserSelector" runat="server">Show Dialog</asp:LinkButton>
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
