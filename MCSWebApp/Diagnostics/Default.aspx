<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Diagnostics._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Client compatibility check</title>
	
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<ul>
			<li>
				<a href="ClientCheck/WebLibraryScriptCheck.aspx">WebLibraryScript Check</a>
			</li>
			<li>
				<a href="ClientCheck/HBCommonScriptCheck.aspx">HBCommonScript Check</a>
			</li>
			<li>
				<a href="ClientCheck/CACWSScriptCheckaspx.aspx">Common AutoCompleteWithSelector Script Check Check</a>
			</li>
			<li>
				<a href="ClientCheck/ActiveXCheck.aspx">ActiveX Check 1</a>
			</li>
			<li>
				<a href="ClientCheck/check.aspx">ActiveX Check 2</a>
			</li>
		</ul>
	</div>
	</form>
</body>
</html>
