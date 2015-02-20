<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="testVisibility.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.OuUserInputControl.testVisibility" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="soa" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>人员输入控件的可见性测试</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		请输入人员
	</div>
	<div>
		<soa:OuUserInputControl ID="userSelector" runat="server" InvokeWithoutViewState="true"
			Width="320px" MultiSelect="true" CanSelectRoot="false" SelectMask="All" ListMask="All" />
	</div>
	<div>
		<asp:Button runat="server" ID="setVisibleBtn" Text="可见/不可见" OnClick="setVisibleBtn_Click" />
		<asp:Button runat="server" ID="postBackBtn" Text="Post Back" />
	</div>
	<div>
		<asp:Label ID="result" runat="server"></asp:Label>
	</div>
	</form>
</body>
</html>
