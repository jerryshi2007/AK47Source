<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CanActivityKeysEditor.aspx.cs"
    Inherits="WorkflowDesigner.ModalDialog.CanActivityKeysEditor" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOAControl" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base target="_self" />
    <title>流转环节可编辑活动点</title>
    <link href="../css/ListBosStyle.css" rel="Stylesheet" type="text/css" />
    <script type="text/javascript" src="../js/ListBox.js"></script>
    <script type="text/javascript">
        var activitiesListBox;
        var activities = new Array();
        var selectActivitiesKey = new Array();
        function onDocumentLoad(sender, args) {
            var paraData = window.dialogArguments;
            if (paraData) {
                if (paraData.jsonStr) {
                    selectActivitiesKey = paraData.jsonStr.split(",");
                }
                activities = paraData.Activities;
            }
            var arguments = {
                Base: document.getElementById("checkboxList_activities"),
                Height: 300,
                Width: 300,
                ClickEventHandler: OnClick
            };

            activitiesListBox = new ListBox(arguments);

            for (var i = 0; i < activities.length; i++) {
                activitiesListBox.AddItem(String.format("{0} - {1}", activities[i].Key, activities[i].Name), activities[i].Key, Array.contains(selectActivitiesKey, activities[i].Key));
            }
        }

        function OnClick(Sender, EventArgs) {
            if (Sender.checked) {
                if (Array.contains(selectActivitiesKey, EventArgs.Value) == false)
                    selectActivitiesKey.push(EventArgs.Value);
            } else {
                if (Array.contains(selectActivitiesKey, EventArgs.Value) == true)
                    Array.remove(selectActivitiesKey, EventArgs.Value);
            }
        }

        function onDocumentUnLoad() {
            activitiesListBox.Dispose();
            delete activities;
            delete selectActivitiesKey;
        }

        function onClick() {
            window.returnValue = { jsonStr: selectActivitiesKey.join(',') };

            top.close();
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True">
    </asp:ScriptManager>
    <table width="100%" style="width: 100%; height: 100%">
        <tr>
            <td class="gridHead">
                <div class="dialogTitle">
                    <span class="dialogLogo">流转环节可编辑活动点</span>
                </div>
            </td>
        </tr>
        <tr>
            <td style="vertical-align: top">
                <div id="checkboxList_activities" style="width: 100%; height: 100%; background-color: White;
                    text-align: center; table-layout: auto; overflow: auto">
                </div>
            </td>
        </tr>
        <tr>
            <td class="gridfileBottom">
            </td>
        </tr>
        <tr>
            <td style="height: 40px; text-align: center; vertical-align: middle">
                <table style="width: 100%; height: 100%">
                    <tr>
                        <td style="text-align: center;">
                            <input type="button" id="confirmButton" value="确定(O)" accesskey="O" class="formButton"
                                onclick="onClick();" />
                        </td>
                        <td style="text-align: center;">
                            <input type="button" onclick="top.close();" value="取消(C)" id="btnCancel" accesskey="C"
                                class="formButton" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
    <script type="text/javascript">
        Sys.Application.add_load(onDocumentLoad);

        Sys.Application.add_unload(onDocumentUnLoad);

    </script>
</body>
</html>
