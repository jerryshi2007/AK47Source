<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="activityEditorTest.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.Workflow.ActivityEditor.activityEditorTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>活动编辑器测试</title>
	<script type="text/javascript">
		function onShowActivityEditorEditor() {
			$find("activityEditor").showDialog("N1", "Add");
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<SOA:WfActivityDescriptorEditor runat="server" ID="activityEditor" />
	</div>
	<div>
		<input type="button" value="Show Editor..." onclick="onShowActivityEditorEditor();" />
	</div>
	</form>
</body>
</html>
