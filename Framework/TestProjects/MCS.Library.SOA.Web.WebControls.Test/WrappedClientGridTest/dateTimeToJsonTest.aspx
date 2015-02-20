<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dateTimeToJsonTest.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest.dateTimeToJsonTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="border: 1px solid #eee">
        <asp:Label ID="Label0" runat="server" Text=""></asp:Label>
        <br />
        <asp:Label ID="Label1" runat="server" Text="Json串:"></asp:Label>
        <br />
        <br />
        <asp:Button ID="Button1" runat="server" Text="POST" OnClick="Button1_Click" />&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button2" runat="server" Text="ReLoad" OnClick="Button2_Click" />
        <br />
        <br />
        <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
    </div>
    <br />
    </form>
</body>
</html>
