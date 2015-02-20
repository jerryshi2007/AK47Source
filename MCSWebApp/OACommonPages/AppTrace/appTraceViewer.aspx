<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="appTraceViewer.aspx.cs"
    Inherits="MCS.OA.CommonPages.AppTrace.appTraceViewer" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>流程跟踪</title>
</head>
<body>
    <form id="serverForm" runat="server">
    <div>
        <SOA:WfRuntimeViewerWrapperControl ID="processViewer" runat="server"  UseIndependentPage="false"
            TargetControlID="toolbarAppTrace" DialogTitle="流程跟踪" ShowingMode="Dialog" />
    </div>
    </form>
</body>
</html>
