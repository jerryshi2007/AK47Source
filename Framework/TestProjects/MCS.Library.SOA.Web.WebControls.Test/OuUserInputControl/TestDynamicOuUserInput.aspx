<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestDynamicOuUserInput.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.Dynamic.TestDynamicOuUserInput" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server" EnableScriptGlobalization="true">
    </asp:ScriptManager>
    <div runat="server" id="dynamicDiv" style="width: 50%">
    </div>
    <div style="display: none">
        <SOA:OuUserInputControl runat="server" ID="OuUserInputControl_template" Width="300px" />
    </div>
    </form>
</body>
</html>
