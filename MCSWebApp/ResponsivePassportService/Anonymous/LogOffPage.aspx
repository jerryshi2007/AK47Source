<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogOffPage.aspx.cs" Inherits="ResponsivePassportService.Anonymous.LogOffPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>注销</title>
	<link href="../Resources/Bootstrap/css/bootstrap.css" rel="stylesheet" type="text/css" />
	<link href="../Resources/Styles/layout.css" rel="stylesheet" type="text/css" />
	<link href="../Resources/Font-awesome/css/font-awesome.css" rel="stylesheet" />
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
<body onload="onDocumentLoad();">
	<form id="serverForm" runat="server">
	<input type="hidden" id="autoRedirect" runat="server" name="autoRedirect" />
	<div class="container" style="height: 100%">
		<div style="height: 100px">
		</div>
		<div class="logout-container">
			<div class="row logout-title">
				<div class="col-lg-8 col-md-8 col-sm-10 col-xs-12 logout-logo">
				</div>
				<div class="col-lg-4 col-md-4 col-sm-2 hidden-xs">
				</div>
			</div>
			<div class="login-container-inner">
				<div class="logout-panel">
					<div class="logout-panel-content">
						<span>用户注销</span>
						<div class="form-horizontal" runat="server" id="appsContainer">
							<%--<div class="form-group">
								<div class="col-lg-2 col-md-2 col-sm-3 col-xs-2 control-label">
									<i class="icon-ok"></i>
								</div>
								<div class="col-lg-10 col-md-10 col-sm-9 col-xs-10">
								</div>
							</div>
							<div class="form-group">
								<div class="col-lg-2 col-md-2 col-sm-3 col-xs-2 control-label">
									<i class="icon-ok"></i>
								</div>
								<div class="col-lg-10 col-md-10 col-sm-9 col-xs-10">
									<label>Hello</label>
								</div>
							</div>--%>
						</div>
					</div>
					<div class="logout-panel-content">
						<div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
							<a style="font-weight: bold; visibility: hidden; line-height: 200%" id="returnHref"
								runat="server">注销完成，点击这里返回应用</a>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
	</form>
</body>
</html>
