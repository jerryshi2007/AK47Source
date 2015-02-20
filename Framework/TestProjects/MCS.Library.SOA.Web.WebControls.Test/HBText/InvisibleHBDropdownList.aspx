<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvisibleHBDropdownList.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.HBText.InvisibleHBDropdownList" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>测试不可见的DropdownList</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<HB:HBDropDownList ID="dropdownList" runat="server" ReadOnly="false" Visible="false">
			<asp:ListItem Text="One" Value="1"></asp:ListItem>
			<asp:ListItem Text="Two" Value="2"></asp:ListItem>
		</HB:HBDropDownList>
	</div>
	<div>
		Selected Text:
		<asp:Label runat="server" ID="selectedText" />
	</div>
	<div>
		<asp:Button runat="server" ID="postbackBtn" Text="Post Back" />
	</div>
	</form>
</body>
</html>
