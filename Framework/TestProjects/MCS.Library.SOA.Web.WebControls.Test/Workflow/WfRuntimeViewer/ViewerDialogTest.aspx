<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewerDialogTest.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.Workflow.WfRuntimeViewer.ViewerDialogTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script>
        function StartClick(activityid) {
            var actstr = 'activityID=';
            var dialog = $find('viewerDialog');
            var url = dialog._dialogUrl;
            url += '&owner_activityid=' + activityid;

            var result = dialog.start(url);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <input type="button" onclick="StartClick('0a45522b-c4dd-9266-4612-4bf51df88bbb');" value="Start..." />
        <cc1:WfRuntimeViewerDialog runat="server" ID="viewerDialog" ActivityID="0a45522b" DialogTitle="请选择分支流程" />
    </div>
    </form>
</body>
</html>
