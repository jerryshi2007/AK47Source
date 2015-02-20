<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HBCommonScriptCheck.aspx.cs"
	Inherits="Diagnostics.ClientCheck.HBCommonScriptCheck" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>HBCommon Script Check</title>
	<script type="text/javascript">
		function checkSubmitValidators() {
			alert(typeof ($HBRootNS.HBCommon.submitValidators));
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<asp:ScriptManager ID="scriptManager" runat="server" EnableScriptGlobalization="true">
		</asp:ScriptManager>
	</div>
	<div>
		<input type="button" value="checkSubmitValidators" onclick="checkSubmitValidators();" />
	</div>
	</form>
</body>
</html>
