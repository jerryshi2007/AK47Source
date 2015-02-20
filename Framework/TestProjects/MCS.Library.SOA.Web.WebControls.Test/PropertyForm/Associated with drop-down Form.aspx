<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Associated with drop-down Form.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.PropertyForm.Associated_with_drop_down_Form" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<%--<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">--%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>下拉连动</title>
    <script type="text/javascript">

        var SHENGEnum = [];
        var SHIEnum = {};
        var XIANEnum = {};

        //  window.load = function onDocumentLoad() {

        function onDocumentLoad(sender, args) {
            InitBindData();
        }

        function InitBindData() {
            var enumData = Sys.Serialization.JavaScriptSerializer.deserialize(document.getElementById("HI_SHENG_JSON").value);

            for (var i = 0; i < enumData.length; i++) {
                SHENGEnum.push({ "text": enumData[i], "value": enumData[i] });
            }

            var shiObj = Sys.Serialization.JavaScriptSerializer.deserialize(document.getElementById("HI_SHI_JSON").value);

            for (var disName in shiObj) {
               // if (typeof (shiObj[disName]) == "Array") {
                if (Object.prototype.toString.apply(shiObj[disName]) === "[object Array]") {
                    var arrEnum = [];
                    for (var i = 0; i < shiObj[disName].length; i++) {
                        arrEnum.push({ "text": shiObj[disName][i], "value": shiObj[disName][i] });
                    }
                    shiObj[disName] = arrEnum;
                }
            }
            SHIEnum = shiObj;

            var xinaObj = Sys.Serialization.JavaScriptSerializer.deserialize(document.getElementById("HI_XIAN_JSON").value);
          
            for (var xianDisName in xinaObj) {
                if (Object.prototype.toString.apply(xinaObj[xianDisName]) === "[object Array]") {
                    var arrEnum = [];
                    for (var i = 0; i < xinaObj[xianDisName].length; i++) {
                        arrEnum.push({ "text": xinaObj[xianDisName][i], "value": xinaObj[xianDisName][i] });
                    }
                    xinaObj[xianDisName] = arrEnum;
                }
               
               // itemobjToBindObj(xinaObj, xianDisName);
            }
            XIANEnum = xinaObj;
        }

        function itemobjToBindObj(bindObj, disName) {
            if (Object.prototype.toString.apply(bindObj[disName]) === "[object Array]") {
                var arrEnum = [];
                for (var i = 0; i < bindObj[disName].length; i++) {
                    arrEnum.push({ "text": bindObj[disName][i], "value": shibindObjObj[disName][i] });
                }
                bindObj[disName] = arrEnum;
            }
        }


        function onBindEditorDropdownList(sender, e) {
            if (SHENGEnum.length == 0) {
                InitBindData();
            }
            var propertyGrid = sender;
            switch (e.property.name) {
                case "SHENG":
                    e.enumDesc = SHENGEnum;
                    break;
                case "SHI":
                    var shenParent = propertyGrid._propertyEditors.SHENG.editor.get_property();
                    if (shenParent.value != "") {
                        e.enumDesc = SHIEnum[shenParent.value];
                    }
                    break;
                case "XIAN":
                    var SHIParent = propertyGrid._propertyEditors.SHI.editor.get_property();
                    if (SHIParent.value != "") {
                        e.enumDesc = XIANEnum[SHIParent.value];
                    }
                    break;
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true" />
    </div>
    <div>
        <SOA:PropertyForm runat="server" ID="propertyForm" Width="100%" OnBindEditorDropdownList="onBindEditorDropdownList"
            Height="100px" />
    </div>
    <div style="display: none">
        <input type="hidden" runat="server" id="HI_SHENG_JSON" />
        <input type="hidden" runat="server" id="HI_SHI_JSON" />
        <input type="hidden" runat="server" id="HI_XIAN_JSON" />
    </div>
    </form>
    <script type="text/javascript">
        Sys.Application.add_load(onDocumentLoad);
    </script>
</body>
</html>
