<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OpenIDBridge.aspx.cs" Inherits="ResponsivePassportService.Anonymous.OpenIDBridge" %>

<!DOCTYPE>
<html>
<head runat="server" autoload="false">
	<title>获取OpenID后认证的回调</title>
	<script type="text/javascript">
		function onDocumentLoad() {
			var openID = document.getElementById("openIDField").value;

			if (top.opener) {
				var topBridgeDataField = top.opener.document.getElementById("bridgeDataField");

				if (topBridgeDataField) {
					topBridgeDataField.value = openID;

					var topBridgeButton = top.opener.document.getElementById("bridgeButton");

					if (topBridgeButton)
						topBridgeButton.click();
				}
			}
		}
	</script>
</head>
<body onload="onDocumentLoad();">
	<form id="serverForm" runat="server">
	<div>
		<input type="hidden" runat="server" id="openIDField" />
	</div>
	</form>
</body>
</html>
