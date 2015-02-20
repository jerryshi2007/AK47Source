<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServiceThreadStatusDetailMessage.aspx.cs"
	Inherits="MCS.OA.CommonPages.ThreadStatus.ServiceThreadStatusDetailMessage" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>详细信息</title>
	<script type="text/javascript">
		function onDocumentLoad() {
			if (window.dialogArguments) {
				var msg = window.dialogArguments.message;
				serverForm.detailMessage.innerText = msg;
			}
		}
	</script>
</head>
<body style="margin: 8px" onload="onDocumentLoad()">
	<form id="serverForm" runat="server">
	<table style="width: 100%; height: 100%" cellpadding="0" cellspacing="0">
		<tr>
			<td style="height: 32px">
				<table style="width: 100%; height: 100%" cellpadding="0" cellspacing="0">
					<tr>
						<td>
							<span id="Span1" style="font-weight: bold; font-size: 16px" runat="server" category="OACommons">
								详细信息</span>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td>
				<textarea id="detailMessage" style="border: silver 1px solid; width: 99%; height: 100%;"
					readonly></textarea>
			</td>
		</tr>
		<tr>
			<td style="height: 4px">
				<hr size="1" />
			</td>
		</tr>
		<tr>
			<td style="height: 32px">
				<table style="width: 100%; height: 100%" cellspacing="0" cellpadding="0">
					<tr>
						<td style="text-align: center;">
							<input id="btnClose" class="formButton" accesskey="C" type="button" style="width: 80px"
								value="关闭(C)" onclick="window.close();" />
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
