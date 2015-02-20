<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewerTest.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.Workflow.WfRuntimeViewer.ViewerTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<asp:ScriptManager runat="server" EnableScriptGlobalization="true">
		</asp:ScriptManager>
	</div>
	<div>
		<cc1:WfRuntimeViewer ID="viewer1" runat="server"></cc1:WfRuntimeViewer>
		<cc1:WfRuntimeViewer ID="viewer2" runat="server"></cc1:WfRuntimeViewer>
	</div>
	</form>
</body>
</html>
