<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImageUploadTest.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.ImageUploadTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>
<body>
	<form id="form1" runat="server">
	<div runat="server" id="container" style="">
		<SOA:ImageUploader runat="server" ID="imgUploader" FileMaxSize="409600"
			 Width="310" Height="310" ImageHeight="300" ImageWidth="300"  ResourceID="xyz" ReadOnly="false" AutoUpload="false" />
<%--		<SOA:ImageUploader runat="server" ID="asdasd" FileMaxSize="409600"
			 Width="150" Height="150" ImageHeight="80" ImageWidth="80" ResourceID="xyz" ReadOnly="false"  />
--%>
	</div>
	<br />
	<br />
	<br />
	<br />
    <asp:Button ID="Button1" runat="server" Text="Button" 
        onclick="Button1_Click1" />
	</form>
</body>
</html>
