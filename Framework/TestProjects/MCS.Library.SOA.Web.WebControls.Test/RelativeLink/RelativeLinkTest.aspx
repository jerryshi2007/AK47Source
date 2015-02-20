<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RelativeLinkTest.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.RelativeLink.RelativeLinkTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Relative link</title>
	<script type="text/javascript">
		function onPopupForm() {
			window.open("", "popup");
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<a href="popupForm.aspx" runat="server" target="popup" id="popupLink" onclick="onPopupForm()">
			Click here...</a>
	</div>
	</form>
</body>
</html>
