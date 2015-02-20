<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ControlPlaceHolderTest.aspx.cs"
	Inherits="MCS.Web.Responsive.WebControls.Test.PlaceHolderTest.ControlPlaceHolderTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>占位信息包含控件的测试</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<res:DeluxePlaceHolder runat="server" TemplatePath="~/Templates/HeaderControl.ascx">
		</res:DeluxePlaceHolder>
	</div>
	<div>
		正文</div>
	</form>
</body>
</html>
