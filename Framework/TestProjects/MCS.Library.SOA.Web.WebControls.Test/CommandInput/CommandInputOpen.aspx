<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CommandInputOpen.aspx.cs" Inherits="MCS.SOA.Web.WebControls.Test.CommandInput.CommandInputOpen" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:Button ID="Button1" runat="server" Text="刷新父窗口" OnClick="Button1_Click" />
    <asp:Button ID="Button2" runat="server" Text="关闭窗口" OnClick="Button2_Click" />
    </div>
        
        
    </form>
</body>
</html>
