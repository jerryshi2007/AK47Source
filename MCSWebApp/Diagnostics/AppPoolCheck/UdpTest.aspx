<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UdpTest.aspx.cs" Inherits="Diagnostics.AppPoolCheck.UdpTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Udp发送和接受测试</title>
</head>
<body>
    <form id="serverForm" runat="server">
        <div>
            <h1>文件通知测试</h1>
        </div>
        <div>
            <asp:Button runat="server" ID="sendNotify" OnClick="sendNotify_Click" Text="发送通知" />
            <asp:Button runat="server" ID="receiveNotify" OnClick="receiveNotify_Click" Text="接收送通知" />
        </div>
        <div>
            <div runat="server" id="sentMessage" />
            <div runat="server" id="receivedMessage" />
        </div>
    </form>
</body>
</html>
