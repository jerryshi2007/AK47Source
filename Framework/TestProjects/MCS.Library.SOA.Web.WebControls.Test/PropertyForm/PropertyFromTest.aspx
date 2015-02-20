<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PropertyFromTest.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.PropertyForm.PropertyFromTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="PropertyForm.css" rel="styleproertyform" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true" />
    </div>
    <div>
        <SOA:PropertyForm runat="server" ID="propertyForm" Width="100%" Height="300px" CssClass="styleproertyform" />
    </div>
    <p>
        <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />
    </p>
    <script type="text/javascript" src="../PropertyGrid/CustomObjectListPropertyEditor.js">
    </script>
    <%-- <script type="text/javascript" src="OUUserInputPropertyEditor.js">
    </script>--%>
    </form>
</body>
</html>
