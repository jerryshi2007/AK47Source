<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmptyCategoryTest.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.PropertyForm.EmptyCategoryTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
        <SOA:PropertyForm runat="server" ID="propertyForm" Width="100%" Height="300px" CssClass="styleproertyform" />
    </div>
    <p>
    </p>
    <asp:CheckBoxList ID="CheckBoxList1" runat="server">
    </asp:CheckBoxList>
   
    <script type="text/javascript" src="../PropertyGrid/CustomObjectListPropertyEditor.js">
    </script>
    </form>
</body>
</html>
