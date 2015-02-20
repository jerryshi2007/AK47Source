<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestTemplate.aspx.cs" Inherits="ResponsivePassportService.TestPages.TestTemplate" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<resp:DeluxePlaceHolder runat="server" TemplatePath="/MCSWebApp/Templates/HeaderControl.ascx">
		</resp:DeluxePlaceHolder>
	</div>
	<div>
		正文
	</div>
	</form>
</body>
</html>
