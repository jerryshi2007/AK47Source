<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CloneControlSetProperties.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.PropertyForm.CloneControlSetProperties" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true" />
    </div>
    <div>
        <SOA:PropertyForm runat="server" ID="propertyForm" ReadOnly="false" Width="100%" Height="300px" CssClass="styleproertyform" />
    </div>
    <asp:Button ID="Button1" runat="server" Text="Button" onclick="Button1_Click" />
    </form>
</body>
</html>
