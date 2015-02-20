<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="testUserOUGraphControl.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.UserControl.testUserOUGraphControl"
    EnableViewState="true" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>简单机构人员树测试</title>
    <script type="text/javascript">
        function onUserSelectDialogConfirmed(sender) {
            displaySelectedObjects(sender.get_selectedObjects());
        }

        function onShowUserSelectorResult() {
            displaySelectedObjects($find("userSelector").get_selectedObjects());
        }

        function onShowUserMultiSelectorResult() {
            var control = $find("userMultiSelector");
            displaySelectedObjects(control.get_selectedObjects());
        }

        function onShowUserSelectorDialogResult() {
            displaySelectedObjects($find("UserOUGraphDialogControl").get_selectedObjects());
        }

        function onShowUserMultiSelectorDialogResult() {
            displaySelectedObjects($find("UserMultiSelectOUGraphDialogControl").get_selectedObjects());
        }

        function displaySelectedObjects(objs) {
            for (var i = 0; i < objs.length; i++)
                addMessage(objs[i].fullPath);
        }

        function addMessage(msg) {
            result.innerHTML += "<p style='margin:0'>" + msg + "</p>";
        }
    </script>
    <script type="text/javascript">
        function onTreeNodeSelecting(sender, e) {
            addMessage(e.object.fullPath);
            e.cancel = false;
        }

        function OnIsChildrenOf(sender, e) {
            alert(e.objSrc);
            alert(e.objTarget);
        }
    </script>
</head>
<body>
    <form id="serverForm" runat="server">
    <table style="width: 100%">
        <tr>
            <td style="width: 50%; height: 420px;">
                <HB:UserOUGraphControl ID="userSelector" runat="server" Width="100%" Height="300px"
                    SelectMask="User" ShowingMode="Normal" BorderStyle="solid"
                    BorderColor="black" BorderWidth="1" OnNodeSelecting="onTreeNodeSelecting" ControlIDToShowDialog="LinkButtonUserSelector"
                    RootExpanded="true" OnLoadingObjectToTreeNode="userSelector_LoadingObjectToTreeNode"
                    OnDialogConfirmed="onUserSelectDialogConfirmed" OnIsChildrenOf="OnIsChildrenOf" />
            </td>
            <td style="height: 420px">
                <HB:UserOUGraphControl ID="userMultiSelector" runat="server" Width="100%" Height="400px"
                    SelectMask="All" ShowingMode="Normal" BorderStyle="solid" BorderColor="black"
                    BorderWidth="1" OnNodeSelecting="onTreeNodeSelecting" MultiSelect="true" ControlIDToShowDialog="LinkButtonMultiUserSelector"
                    RootExpanded="true" OnLoadingObjectToTreeNode="userSelector_LoadingObjectToTreeNode"
                    OnDialogConfirmed="onUserSelectDialogConfirmed" MergeSelectResult="false" ShowDeletedObjects="false" />
            </td>
        </tr>
        <tr>
            <td>
                <input type="button" value="显示选择结果" onclick="onShowUserSelectorResult();" />
            </td>
            <td>
                <input type="button" value="显示选择结果" onclick="onShowUserMultiSelectorResult();" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:LinkButton ID="LinkButtonShowDialog" runat="server">Show Dialog</asp:LinkButton>
                <input type="button" value="显示选择结果" onclick="onShowUserSelectorDialogResult();" />
                <HB:UserOUGraphControl ID="UserOUGraphDialogControl" runat="server" Width="100%"
                    SelectMask="Organization" ControlIDToShowDialog="LinkButtonShowDialog"
                    RootExpanded="true" OnLoadingObjectToTreeNode="userSelector_LoadingObjectToTreeNode"
                    ShowingMode="Dialog" ListMask="Organization" MultiSelect="false" OnDialogConfirmed="onUserSelectDialogConfirmed"
                    DialogTitle="请选择机构" />
            </td>
            <td>
                <asp:LinkButton ID="LinkButtonMultiSelectDialog" runat="server">Show Multi Select Dialog</asp:LinkButton>
                <input type="button" value="显示选择结果" onclick="onShowUserMultiSelectorDialogResult();" />
                <HB:UserOUGraphControl ID="UserMultiSelectOUGraphDialogControl" runat="server" Width="100%"
                    Height="100px" SelectMask="All" DialogTitle="请选择机构" ControlIDToShowDialog="LinkButtonMultiSelectDialog"
                    RootExpanded="true" OnLoadingObjectToTreeNode="userSelector_LoadingObjectToTreeNode"
                    ShowingMode="Dialog" MultiSelect="true" OnDialogConfirmed="onUserSelectDialogConfirmed"
                    MergeSelectResult="true" ShowDeletedObjects="true" />
            </td>
        </tr>
    </table>
    <div>
        <asp:Button runat="server" ID="postBackBtn" Text="Post Back" />
    </div>
    <div id="resultContainer">
        <div id="result" contenteditable="true" style="overflow: auto; border: 1px silver solid;
            width: 100%; height: 200px" runat="server">
        </div>
    </div>
    </form>
</body>
</html>
