<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="testConnectIn.aspx.cs"
	Inherits="WeChatConnectInService.TestPages.testConnectIn" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>测试接入服务</title>
	<script type="text/javascript">
		function onVerifyUrlClick() {
			var url = "../services/ConnectIn.ashx?signature=b6e2c549862f11f4b32de6c020ad935372250852&echostr=2349438381043187455&timestamp=1402415798&nonce=1982888647";

			frameContainer.innerHTML = "<iframe id=\"resultFrame\" style=\"width: 400px; height: 300px\" src=\"" + url + "\"></iframe>";
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<input type="button" value="Verify Url" onclick="onVerifyUrlClick();" />
	</div>
	<div id="frameContainer">
		<iframe id="resultFrame" style="width: 400px; height: 300px" name="resultFrame">
		</iframe>
	</div>
	</form>
</body>
</html>
