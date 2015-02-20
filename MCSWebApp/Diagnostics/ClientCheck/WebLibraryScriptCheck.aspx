<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebLibraryScriptCheck.aspx.cs"
	Inherits="Diagnostics.ClientCheck.WebLibraryScriptCheck" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Web.Library Script Check</title>
	<script type="text/javascript">
		function checkWindowFeature() {
			alert(typeof ($HGRootNS._WindowFeatureFunction));
		}

		function checkArrayContains() {
			alert(typeof (Array.contains));
		}

		function checkClientMsg() {
			alert(typeof ($HGRootNS._ClientMsg));
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<asp:ScriptManager runat="server" EnableScriptGlobalization="true">
		</asp:ScriptManager>
	</div>
	<div>
		<input type="button" value="checkWindowFeature" onclick="checkWindowFeature();" />
		<input type="button" value="checkArrayContains" onclick="checkArrayContains();" />
		<input type="button" value="checkClientMsg" onclick="checkClientMsg();" />
	</div>
	</form>
</body>
</html>
