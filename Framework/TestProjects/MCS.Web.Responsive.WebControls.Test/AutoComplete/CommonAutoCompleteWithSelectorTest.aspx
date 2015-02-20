<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CommonAutoCompleteWithSelectorTest.aspx.cs"
    Inherits="MCS.Web.Responsive.WebControls.Test.CommonAutoCompleteWithSelectorTest" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>自动完成控件测试</title>
    <script type="text/javascript">

        var page = {
            selectData: function (parameters) {
                alert(0);
            }
        };

        function test() {
            alert($find("CommonAutoCompleteWithSelectorControl1").get_selectedData().length);
        }

        function selectData(sender, e) {
            //这个方法仅作为演示，实际应用中实现应该是从Server查询，返回序列化结果，注意要带__type。

            var result = [];

            for (var i = 0; i < 5; i++) {
                var obj = {};
                obj.__type = "MCS.Web.Responsive.WebControls.Test.AutoCompleteWithSelectorControl.CommonData, MCS.Web.Responsive.WebControls.Test";
                obj.Code = "id" + i;
                obj.Name = "name" + i;
                result.push(obj);
            }

            var control = $find("CommonAutoCompleteWithSelectorControl1");
            var sFeature = "dialogWidth:500px; dialogHeight:300px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            result.nameTable = $NT;
            result.keyName = control.get_dataKeyName();
            result.displayPropName = control.get_dataDisplayPropName();

            var resultStr = window.showModalDialog(control.get_selectObjectDialogUrl(), result, sFeature);

            var obj = result[resultStr];


            e.resultValue = obj == null ? "" : Sys.Serialization.JavaScriptSerializer.serialize([obj]);

        }

        function dataChanged(data) {

        }

        function onCloneComponent() {
            var parent = $get("container");

            var template = $find("CommonAutoCompleteWithSelectorControl1");

            template.cloneAndAppendToContainer(parent);
        }

        function onSelectData(sender, e) {
            //这个方法仅作为演示，实际应用根据具体情况实现，返回序列化结果，注意要带__type。
            var curData = e.Data;
            var dialogUrl = "url"; //这里是应用提供的对话框页面路径。
            var sFeature = "dialogWidth:500px; dialogHeight:300px;center:yes;help:no;resizable:yes;scroll:no;status:no";
            var resultStr = window.showModalDialog(dialogUrl, curData, sFeature);
            var result = Sys.Serialization.JavaScriptSerializer.serialize(resultStr);
            e.resultValue = result;
        }

        function btnSetReadOnly_onclick() {
            var control = $find("CommonAutoCompleteWithSelectorControl1");
            var readOnly = control.get_readOnly();

            control.set_readOnly(!readOnly);
        }

        function btnSetDisabled_onclick() {
            var control = $find("CommonAutoCompleteWithSelectorControl1");
            var enabled = control.get_enabled();

            control.set_enabled(!enabled);
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <p>
                可输入1、2、3、*测试
            </p>
            <div style="position: relative">
                <res:CommonAutoCompleteWithSelectorControl ID="CommonAutoCompleteWithSelectorControl1"
                    runat="server" ReadOnly="False" ClientDataKeyName="Code" ClientDataDisplayPropName="Name"
                    ClientDataDescriptionPropName="Detail" DataTextFields="Name,Detail" OnClientSelectData="page.selectData"
                    OnGetDataSource="CommonAutoCompleteWithSelectorControl1_GetDataSource" OnValidateInput="CommonAutoCompleteWithSelectorControl1_ValidateInput"
                    Width="400px" />

            </div>
            <br />
            <asp:Button ID="Button1" runat="server" Text="PostBack" OnClick="Button1_Click" />
            <input id="btnSetReadOnly" type="button" value="Set ReadOnly" onclick="return btnSetReadOnly_onclick()" />
            <input id="btnSetDisabled" type="button" value="Set Disabled" onclick="return btnSetDisabled_onclick()" />
        </div>

        <div>
            <input type="button" onclick="onCloneComponent();" value="Clone Component" />

        </div>
        <div id="container">
        </div>
    </form>
</body>
</html>
