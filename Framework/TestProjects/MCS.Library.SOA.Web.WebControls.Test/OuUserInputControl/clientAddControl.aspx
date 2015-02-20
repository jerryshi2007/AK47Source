<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="clientAddControl.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.OuUserInputControl.clientAddControl" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function OnCellCreatingEditor() {
            var Container = document.getElementById("div_Container")

            var div = $HGDomElement.createElementFromTemplate({ nodeName: "div" }, Container);
            div.id = "div_userSelectControl_1";

            var userSelectControl = new $HBRootNS.OuUserInputControl(div);
            userSelectControl.id = "div_userSelectControl_1_div_userSelectControl_1";

            userSelectControl.initialize();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <HB:OuUserInputControl runat="server" ID="OuUserInputControl_template" Width="300px" />
    </div>
    <br />
    <input type="button" id="btn_add" value="添加" onclick="OnCellCreatingEditor()" />
    <div id="div_Container">
    </div>
    </form>
</body>
</html>
