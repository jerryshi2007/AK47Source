<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SimplePlaceHolderTest.aspx.cs"
	Inherits="MCS.Web.Responsive.WebControls.Test.PlaceHolderTest.SimplePlaceHolderTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>简单的占位符设置</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<res:DeluxePlaceHolder runat="server" TemplatePath="~/Templates/SimpleHeader.ascx">
		</res:DeluxePlaceHolder>
	</div>
	<div>
		正文部分
	</div>
	<div>
		<res:DeluxePlaceHolder runat="server" TemplatePath="~/Templates/SimpleFooter.ascx">
		</res:DeluxePlaceHolder>
	</div>
	</form>
</body>
</html>
