<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MultiProcessControlWithError.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.MultiProcessControl.MultiProcessControlWithError" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="soa" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>测试带错误的多步操作</title>
	<script type="text/javascript">
		function onBeforeStart(e) {
			e.steps = new Array(10);
			for (var i = 0; i < 10; i++) {
				e.steps[i] = "1";
			}
			return e;
		}

		function onFinished(e) {
			if (e.value == true) {
				var errorMessages = $find("MutifyProcessControl1").get_errorMessages();
				if (errorMessages.length > 0)
					window.alert(errorMessages.join("\n"));
				else
					window.alert("ok");
			}
		}

		function onCancel(e) {
			window.alert("Cancel");
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<asp:Button ID="Start" runat="server" Text="Start...." />
		<soa:MultiProcessControl runat="server" ID="MutifyProcessControl1" DialogTitle="正在转档案" ShowStepErrors="true"
			ControlIDToShowDialog="Start" OnClientBeforeStart="onBeforeStart" OnClientFinishedProcess="onFinished"
			OnExecuteStep="MutifyProcessControl1_ExecuteStep" OnError="MutifyProcessControl1_OnError"
			OnClientCancelProcess="onCancel"></soa:MultiProcessControl>
	</div>
	</form>
</body>
</html>
