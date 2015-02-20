<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PropertyGridCloneDeluxeCalendar.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.PropertyGrid.PropertyGridCloneDeluxeCalendar" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function collectPropertiesValue() {
            var strB = new Sys.StringBuilder();

            var properties = $find("propertyGrid").get_properties();

            for (var i = 0; i < properties.length; i++) {
                if (strB.isEmpty() == false)
                    strB.append("\n");

                var prop = properties[i];
                strB.append(prop.name);
                strB.append(":");
                strB.append(prop.value ? prop.value : "no value");
            }

            return strB.toString();
        }

        function onShowPropertiesValueClick() {
            $get("propertyResult").innerText = collectPropertiesValue();
        }
 
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server" ID="scriptManager" EnableScriptGlobalization="true">
    </asp:ScriptManager>
    <div>
        <SOA:PropertyGrid runat="server" ID="propertyGrid" Width="300px" Height="200px" DisplayOrder="ByCategory" />
    </div>
    <div>
        <textarea runat="server" id="propertyResult" style="width: 400px; height: 200px"></textarea>
    </div>
    <div>
        <input type="button" value="Show Properties Value" onclick="onShowPropertiesValueClick();" />
    </form>
</body>
</html>
