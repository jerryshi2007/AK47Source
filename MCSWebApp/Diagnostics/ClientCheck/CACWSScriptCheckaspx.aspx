<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CACWSScriptCheckaspx.aspx.cs"
	Inherits="Diagnostics.ClientCheck.CACWSScriptCheckaspx" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Common AutoCompleteWithSelector Script Check</title>
	<script type="text/javascript">
		function cacwsScriptCheck() {
			alert(typeof ($HBRootNS.CommonAutoCompleteWithSelectorControl));
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
		<SOA:CommonAutoCompleteWithSelectorControl runat="server" />
	</div>
	<div>
		<input type="button" value="Common AutoCompleteWithSelector Script Check" onclick="cacwsScriptCheck();" />
	</div>
	</form>
</body>
</html>
