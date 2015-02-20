<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PropertyEditorBaseTest.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.PropertyForm.PropertyEditorBaseTest" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true" />
    <div>
        <ul>
            <li style="font-family: 宋体, Arial, Helvetica, sans-serif; font-size: larger">PropertyGrid
                测试</li>
        </ul>
    </div>
    <div>
        <SOA:PropertyGrid runat="server" ID="propertyGrid" Width="300px" DisplayOrder="ByAlphabet" />
    </div>
    <div>
        <ul>
            <li style="font-family: 宋体, Arial, Helvetica, sans-serif; font-size: larger">PropertyForm
                测试</li>
        </ul>
    </div>
    <div style="vertical-align: bottom">
        <SOA:PropertyForm runat="server" ID="propertyForm" Width="900px" Height="300px" />
    </div>
    <script type="text/javascript" src="../PropertyGrid/CustomObjectListPropertyEditor.js"></script>
    </form>
</body>
</html>
