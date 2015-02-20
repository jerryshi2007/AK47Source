<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PropertyGridTest.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.PropertyGrid.PropertyGridTest" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<%--<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">--%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Test Property Grid</title>
    <script type="text/javascript" src="../WorkitemControl/jquery-1.4.3.js"></script>
    <script type="text/javascript" src="../WorkitemControl/jquery.json-2.2.js"></script>
    <script type="text/javascript">
        function collectPropertiesValue() {
            var strB = new Sys.StringBuilder();

            var properties = $find("propertyGrid").get_properties();

            for (var i = 0; i < properties.length; i++) {
                if (strB.isEmpty() == false)
                    strB.append("\n");

                var prop = properties[i];

                strB.append(prop.name + ": " + prop.value ? prop.value : 'no value');
            }

            return strB.toString();
        }

        function onShowPropertiesValueClick() {
            $get("propertyResult").innerText = collectPropertiesValue();
        }

        function onEnterEditor(sender, e) {
            $get("propertyResult").innerText += "\nEnter Property: " + e.propertyValue.value;
        }

        function onClickEditor(sender, e) {
            var activeEditor = sender.get_activeEditor();
            //activeEditor.commitValue([{ RandyWang: "Randy Yang" }, { RandyWang: "Randy1"}]);
        }

        function OnBindEditorDropdownList(sender, e) {
            var enumTypes = jQuery.evalJSON(jQuery('#enumTypeStore').val());
            //var item = enumTypes[e.property.editorKey];
            var result = enumTypes[e.property.editorParams];

            if (result)
                e.enumDesc = result;
        }

        //        function OnClientRenderObjectListView(sender, e) {
        //            e.viewName = "RandyWang";
        //        }

        function onClickValidated(sender, e) {

        }

        function onPopertyClientShow(sender, e) {
            var activeEditor = sender;
        }

    </script>
</head>
<body>
    <form id="serverForm" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true" />
        <MCS:DeluxeCalendar ID="DeluxeCalendar1" runat="server">
        </MCS:DeluxeCalendar>
  
    </div>
    <div>
        <SOA:PropertyGrid runat="server" ID="propertyGrid" Width="300px" Height="600px" OnClientEditorValidated="onClickValidated"
            OnClientClickEditor="onClickEditor" DisplayOrder="ByCategory"  ReadOnly="false"/>
        <%--OnBindEditorDropdownList="OnBindEditorDropdownList"--%>
    </div>
    <p>
    </p>
    <div>
        <asp:Button runat="server" ID="bT" Text="回调" OnClick="bT_Click" />
    </div>
    <%--<div>
        <textarea runat="server" id="propertyResult" style="width: 400px; height: 200px"></textarea>
    </div>--%>
    <script type="text/javascript" src="CustomObjectListPropertyEditor.js" />
    </form>
</body>
</html>
