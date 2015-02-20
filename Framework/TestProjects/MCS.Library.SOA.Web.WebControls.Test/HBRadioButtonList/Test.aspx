<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.HBRadioButtonList.Test" %>

<%@ Register assembly="MCS.Library.SOA.Web.WebControls" namespace="MCS.Web.WebControls" tagprefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function onchange(value) {
            alert("我改变了！——" + value);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <cc1:HBRadioButtonList ID="HBRadioButtonList1" runat="server" 
            OnClientValueChanged="onchange">
            <asp:ListItem>1</asp:ListItem>
            <asp:ListItem Selected="True">2</asp:ListItem>
        </cc1:HBRadioButtonList>

        <cc1:HBRadioButtonList ID="HBRadioButtonList2" runat="server" 
            OnClientValueChanged="onchange" ReadOnly="true" KeepControlWhenReadOnly="true">
            <asp:ListItem >1</asp:ListItem>
            <asp:ListItem Selected="True">2</asp:ListItem>
        </cc1:HBRadioButtonList>
    
        <br />
        <br />
        <cc1:HBDropDownList ID="HBDropDownList1" runat="server" ReadOnly="true">
            <asp:ListItem >1</asp:ListItem>
            <asp:ListItem Selected="True">2</asp:ListItem>
        </cc1:HBDropDownList>
    
    </div>
    <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Button" />
    </form>
</body>
</html>
