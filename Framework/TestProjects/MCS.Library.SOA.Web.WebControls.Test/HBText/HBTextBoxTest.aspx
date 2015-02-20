<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HBTextBoxTest.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.HBText.HBTextBoxTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <HB:HBTextBox runat="server" Text="yyyyyy" ID="tbx1" ReadOnly="true" ></HB:HBTextBox>
    </div>
    <br />
    <div runat="server" id="div_msg" style="padding-top: 20px">
    </div>
    <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" Style="height: 21px" />
    <asp:Repeater runat="server" ID="Repeater1">
        <ItemTemplate>
            <div>
                232323222222
            </div>
            feeeeeeeeeeeeeeeeeeeee
            <%# Eval("feffwffefef") %>
        </ItemTemplate>
    </asp:Repeater>
    </form>
</body>
</html>
