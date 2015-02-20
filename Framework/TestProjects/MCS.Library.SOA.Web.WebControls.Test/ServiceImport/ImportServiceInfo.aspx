<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportServiceInfo.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.ServiceImport.ImportServiceInfo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Import Service Info</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<asp:Button runat="server" Text="Import" ID="importButton" 
			onclick="importButton_Click"/>
	</div>
	</form>
</body>
</html>
