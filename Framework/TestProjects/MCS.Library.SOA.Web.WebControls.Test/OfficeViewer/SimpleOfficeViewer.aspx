<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SimpleOfficeViewer.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.OfficeViewer.SimpleOfficeViewer" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>简单Office文档显示控件测试</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<input type="button" value="切换工具栏" onclick="$find('viewerWrapper').get_viewer().toolBars = !$find('viewerWrapper').get_viewer().toolBars" />
	</div>
	<div>
		<SOA:OfficeViewerWrapper runat="server" ID="viewerWrapper" Width="100%" Height="640px" DefaultOpenUrl="Expense.xls"
			ShowToolbars="false"></SOA:OfficeViewerWrapper>
	</div>
	</form>
</body>
</html>
