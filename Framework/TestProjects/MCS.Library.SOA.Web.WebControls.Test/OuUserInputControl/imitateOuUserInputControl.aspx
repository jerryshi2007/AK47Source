<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="imitateOuUserInputControl.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.OuUserInputControl.imitateOuUserInputControl" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">

        var p = null;

        function getData() {
            //if (event.keyCode == 13 && $get("tbx_userName").value.length > 0) {
            //    var ctrl = $find("OuUserInputControl1");
            //    var f = ctrl._validateInput($get("tbx_userName").value);
            //}

            $find("OuUserInputControl1").set_selectedOuUserData(p);
            $find("OuUserInputControl1").setInputAreaText();
        }

        function setData() {
            p = $find("OuUserInputControl1").get_selectedOuUserData();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="padding: 100px">
        <div>
            <input id="Text1" onclick="setData();" type="button" />
            <input id="tbx_userName" onclick="getData();" type="button" />
        </div>
        <br />
        <div>
            <cc1:OuUserInputControl ID="OuUserInputControl1" runat="server" InvokeWithoutViewState="true"
                MultiSelect="false" CanSelectRoot="false" Width="300px" />
        </div>
    </div>
    </form>
</body>
</html>
