<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DynamicUserTree.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.UserControl.DynamicUserTree" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>动态创建的用户树</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<%--<asp:ScriptManager runat="server" EnableScriptGlobalization="true">
	</asp:ScriptManager>--%>
	<div id="userTreeContainer" runat="server">
	</div>
	<div>
		<HB:UserOUGraphControl ID="UserOUGraphDialogControl" runat="server" Width="100%"
			ShowingMode="Dialog" Visible="false" OnLoadingObjectToTreeNode="userSelector_LoadingObjectToTreeNode" />
	</div>
	<div>
		<asp:LinkButton ID="showDialogBtn" runat="server" Text="Show OU Dialog..." />
	</div>
	</form>
</body>
</html>
