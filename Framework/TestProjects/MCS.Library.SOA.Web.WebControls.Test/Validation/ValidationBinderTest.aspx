<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ValidationBinderTest.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.Validation.ValidationBinderTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Validation Binder Test</title>
    <script type="text/javascript">
        function onDocumentLoad() {
            bindControl($get("integerInput"), $HBRootNS.ValidationDataType.Integer, "{0:N0}");
            bindControl($get("numericInput"), $HBRootNS.ValidationDataType.Decimal, "{0:N2}");
        }

        function bindControl(control, dataType, formatString) {

            var binder = new $HBRootNS.TextBoxValidationBinder();
            binder.set_control(control);
            binder.set_dataType(dataType);
            binder.set_formatString(formatString);
            binder.add_dataChange(onDataChange);//里头抛出的事件
            binder.bind();

            return binder;
        }

        function onDataChange(binder, e) {
            addMessage("normalizedText:" + e.normalizedText);
            addMessage("strongTypeValue:" + e.strongTypeValue);
            addMessage("-------------------------------------");
        }

        function addMessage(msg) {
            result.innerHTML += "<p style='margin:0'>" + msg + "</p>";
        }
    </script>
</head>
<body onload="onDocumentLoad();">
    <form id="serverForm" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
        </asp:ScriptManager>
    </div>
    <div>
        Integer:
        <input id="integerInput" type="text" />
    </div>
    <div>
        Number:
        <input id="numericInput" type="text" />
    </div>
    <br />
    <div id="resultContainer">
        <div id="result" contenteditable="true" style="overflow: auto; border: 1px silver solid;
            width: 60%; height: 200px" runat="server">
        </div>
    </div>
    </form>
</body>
</html>
