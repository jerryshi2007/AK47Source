<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogOffPage.aspx.cs" Inherits="PermissionCenter.Anonymous.LogOffPage" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>注销</title>
	<style type="text/css">
		BODY
		{
			font-size: 9pt;
			color: black;
			font-family: SimSun;
		}
		TABLE
		{
			font-size: 9pt;
		}
		.INPUT
		{
			border-right: #c4c4c4 1px solid;
			border-top: #999999 1px solid;
			font-size: 9pt;
			border-left: #999999 1px solid;
			border-bottom: #c4c4c4 1px solid;
			font-family: SimSun;
		}
		BUTTON
		{
			border-right: #c4c4c4 1px solid;
			border-top: #999999 1px solid;
			font-size: 9pt;
			border-left: #999999 1px solid;
			border-bottom: #c4c4c4 1px solid;
			font-family: SimSun;
		}
		TEXTAREA
		{
			border-right: #c4c4c4 1px solid;
			border-top: #999999 1px solid;
			font-size: 9pt;
			border-left: #999999 1px solid;
			border-bottom: #c4c4c4 1px solid;
			font-family: SimSun;
		}
	</style>

	<script type="text/javascript">
		function clearWindowsAuthenticateCache() {
			try {
				document.execCommand("ClearAuthenticationCache");
			}
			catch (ex) {
			}
		}

		function onDocumentLoad() {
			clearWindowsAuthenticateCache();
			if (returnHref.href.length > 0) {
				if (serverForm.autoRedirect.value.toLowerCase() == "true")
					window.setTimeout(redirectToTargetPage, 0);
				else
					returnHref.style.visibility = "visible";
			}
		}

		function redirectToTargetPage() {
			window.navigate(returnHref.href);
		}
	</script>

</head>
<body onload="onDocumentLoad();" style="background-image: url(../Images/bg.gif)">
	<table style="width: 100%; height: 100%" cellpadding="0" cellspacing="0">
		<tr>
			<td align="center" valign="middle">
				<form id="serverForm" method="post" runat="server">
				<input type="hidden" id="autoRedirect" runat="server" name="autoRedirect" />
				<div style="width: 300px">
					<table style="width: 100%; background-color: #EBF6FD" cellspacing="0" cellpadding="0"
						border="0">
						<tr>
							<td style="height: 50px; border: none 0 none">
								<table style="width: 100%; border: none 0px none" cellpadding="0" cellspacing="0">
									<tr>
										<td>
											<!--<img src="../images/hb2004banner.gif">-->
											<img src="../Images/sinooceanlandsso.png" alt="Sino-Ocean Land Passport System" />
										</td>
									</tr>
								</table>
							</td>
						</tr>
						<tr style="border: 0">
							<td style="border-right-style: groove; border-right-width: 1px; border-right-color: #B9C1C6;
								border-left-style: solid; border-left-width: 1px; border-left-color: #B9C1C6;
								height: 1px">
							</td>
						</tr>
						<tr>
							<td valign="top" style="border-right-style: groove; border-right-width: 1px; border-color: #B9C1C6;
								border-left-style: solid; border-left-width: 1px; border-left-color: #B9C1C6;
								height: 130px">
								<table style="width: 100%" cellpadding="0" cellspacing="0">
									<tr>
										<td style="width: 16px">
										</td>
										<td id="tableContainer" runat="server" style="border-right: silver 1px solid; border-top: silver 1px solid;
											overflow: auto; border-left: silver 1px solid; border-bottom: silver 1px solid;
											height: 200px" valign="top">
											&nbsp;
										</td>
										<td style="width: 24px">
										</td>
									</tr>
									<tr>
										<td>
										</td>
										<td align="center">
											<a style="font-weight: bold; visibility: hidden; line-height: 200%" id="returnHref"
												runat="server">注销完成，点击这里返回应用</a>
										</td>
										<td>
										</td>
									</tr>
								</table>
							</td>
						</tr>
						<tr>
							<td style="background-image: url(../Images/bottombg.png); height: 6px; width: 300px;">
							</td>
						</tr>
					</table>
				</div>
				</form>
			</td>
		</tr>
	</table>
</body>
</html>
