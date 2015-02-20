<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="AUCenter.SchemaAdmin.Test"
    Async="true" AsyncTimeout="35" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <span id="TaskMessage" runat="server"></span>
        <asp:Button Text="随机生成①" runat="server" OnClick="RandomGen1" />
        <asp:Button runat="server" OnClick="ClearData" Text="清除数据" />
        架构角色Key<asp:TextBox ID="txtSchemaRoleID" runat="server">94fe0cc1-acac-9966-4c7a-f17a6270e926</asp:TextBox>
        <asp:TextBox ID="txtAURoleID" runat="server"></asp:TextBox>
        <asp:Button Text="矩阵" runat="server" OnClick="MatrixTest" />
    </div>
    </form>
</body>
</html>
