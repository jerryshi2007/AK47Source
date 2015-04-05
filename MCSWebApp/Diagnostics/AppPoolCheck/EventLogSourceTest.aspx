<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EventLogSourceTest.aspx.cs" Inherits="Diagnostics.AppPoolCheck.EventLogSourceTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="serverForm" runat="server">
        <div>
            <h1>建立EventLog Source测试</h1>
        </div>
        <div>
            <asp:Button runat="server" ID="createEventLogSource" Text="创建EventLog Source" OnClick="createEventLogSource_Click" />
            <asp:Button runat="server" ID="writeEventLogSource" Text="写入EventLog Source" OnClick="writeEventLogSource_Click" />
        </div>
        <div>
            <div runat="server" id="createMessage" />
            <div runat="server" id="writeMessage" />
        </div>
    </form>
</body>
</html>
