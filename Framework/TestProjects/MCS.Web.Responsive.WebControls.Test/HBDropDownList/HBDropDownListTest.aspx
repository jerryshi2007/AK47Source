<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HBDropDownListTest.aspx.cs" Inherits="MCS.Web.Responsive.WebControls.Test.HBDropDownList.HBDropDownListTest" %>

<!DOCTYPE html>

<html>
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>HBDropDownList Test</title>
    <script src="../jquery/jquery-2.0.3.js" type="text/javascript"></script>
    <script type="text/javascript">
        function test() {
            $HBDropDownList.setValue("HBDropDownList1", "1");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      
        <res:HBDropDownList ID="HBDropDownList1" ReadOnly="True" KeepControlWhenReadOnly="True" runat="server">
            <asp:ListItem>1</asp:ListItem>
            <asp:ListItem Selected="True">2</asp:ListItem>
        </res:HBDropDownList>
      <input type="button" onclick="test();" value="Test"/>
        <asp:Button ID="Button1" runat="server" Text="Button" onclick="Button1_Click" />
    </div>
    </form>
</body>
</html>