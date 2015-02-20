<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="testUserOUGraphControl2.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.UserControl.testUserOUGraphControl2" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">

        function ShowDialog() {
            $find("UserOUGraphDialogControl").showDialog();

        }
        function onUserSelectDialogConfirmed(sender) {
            displaySelectedObjects(sender.get_selectedObjects());
        }
        function displaySelectedObjects(objs) {
            for (var i = 0; i < objs.length; i++)
                addMessage(objs[i].fullPath);
        }
        function addMessage(msg) {
            result.innerHTML += "<p style='margin:0'>" + msg + "</p>";
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <input type="button" value="选择" onclick="ShowDialog();" id="button1" />
        <HB:UserOUGraphControl ID="UserOUGraphDialogControl" runat="server" Width="100%"
            SelectMask="Organization" ListMask="Organization" RootExpanded="true"
            ShowingMode="Dialog" MultiSelect="true" DialogTitle="请选择机构" OnDialogConfirmed="onUserSelectDialogConfirmed"
            OnLoadingObjectToTreeNode="userSelector_LoadingObjectToTreeNode" />
    </div>
    <div id="resultContainer">
        <div id="result" contenteditable="true" style="overflow: auto; border: 1px silver solid;
            width: 100%; height: 200px" runat="server">
        </div>
    </div>
    </form>
</body>
</html>
