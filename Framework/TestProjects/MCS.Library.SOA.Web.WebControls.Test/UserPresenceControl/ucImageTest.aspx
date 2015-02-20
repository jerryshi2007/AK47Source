<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ucImageTest.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.UserPresenceControl.ucImageTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>UC 状态图片测试</title>
	<style type="text/css">
		#imnaway
		{
			background-position: 0,0;
			width: 12;
			height: 12;
		}
	</style>
	<script type="text/javascript">
		function onDocumentLoad() {
			$get("imnaway").style.backgroundImage = "url(" + $get("ucStatus").src + ")";
		}
	</script>
</head>
<body onload="onDocumentLoad();">
	<form id="serverForm" runat="server">
	<div>
		<asp:ScriptManager runat="server">
		</asp:ScriptManager>
	</div>
	<div>
		<img id="ucStatus" src="" runat="server" />
	</div>
	<div>
		<img id="imnaway"></img>
	</div>
	</form>
</body>
</html>
