<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="addExtraUserInfoOuUserInput.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.OuUserInputControl.addExtraUserInfoOuUserInput" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>测试定制选中的用户带扩展信息</title>
    <script type="text/javascript">
        function onSelectedDataChanged(selectedData) {
            displaySelectedObjects(selectedData);
        }

        function displaySelectedObjects(objs) {
            for (var i = 0; i < objs.length; i++) {
                var obj = objs[i];
                var parent = null;

                if (obj.tag && obj.tag != "")
                    parent = Sys.Serialization.JavaScriptSerializer.deserialize(obj.tag);

                var text = String.format("{0}({1})", obj.displayName, parent != null ? parent.displayName : "Null");

                addMessage(text);
            }
        }

        function addMessage(msg) {
            result.innerHTML = "<p style='margin:0'>" + msg + "</p>";
        }
    </script>
</head>
<body>
    <form id="serverForm" runat="server">
    <div>
        <SOA:OuUserInputControl MultiSelect="true" ID="userInput" runat="server" InvokeWithoutViewState="true"
            Width="320px" OnClientSelectedDataChanged="onSelectedDataChanged" OnObjectsLoaded="userInput_ObjectsLoaded"
            OnGetDataSource="userInput_OnGetDataSource" />
        <%--  OnValidateInputOuUser="OnValidateInputOuUser"--%>
    </div>
    <div id="resultContainer">
        <div id="result" contenteditable="true" style="overflow: auto; border: 1px silver solid;
            width: 100%; height: 200px" runat="server">
        </div>
    </div>
    </form>
</body>
</html>
