<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HBRadioButtonListTest.aspx.cs"
    Inherits="MCS.Web.Responsive.WebControls.Test.HBRadioButtonList.HBRadioButtonListTest" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>HBRadioButtonList Test</title>
    <script type="text/javascript">
        function onchange(value) {
            alert("我改变了！——" + value);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <res:HBRadioButtonList ID="HBRadioButtonList1" runat="server" OnClientValueChanged="onchange">
            <asp:ListItem>1</asp:ListItem>
            <asp:ListItem Selected="True">2</asp:ListItem>
        </res:HBRadioButtonList>
        <res:HBRadioButtonList ID="HBRadioButtonList2" runat="server" OnClientValueChanged="onchange"
            ReadOnly="true" KeepControlWhenReadOnly="true">
            <asp:ListItem>1</asp:ListItem>
            <asp:ListItem Selected="True">2</asp:ListItem>
        </res:HBRadioButtonList>
    </div>
    <asp:Button ID="btnPostBack" runat="server" OnClick="btnPostBack_Click" Text="PostBack" />
    </form>
</body>
</html>
