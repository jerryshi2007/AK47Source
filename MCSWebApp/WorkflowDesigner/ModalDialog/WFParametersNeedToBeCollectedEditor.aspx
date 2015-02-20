<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WFParametersNeedToBeCollectedEditor.aspx.cs"
    Inherits="WorkflowDesigner.ModalDialog.WFParametersNeedToBeCollectedEditor" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="soaControl" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>自动收集流程参数编辑</title>
    <script type="text/javascript" src="../js/jquery-1.4.3.js"></script>
    <script type="text/javascript" src="../js/jquery.validate.js"></script>
    <script type="text/javascript">
        var cnmsg =
         { required: "必须输入",
             remote: "请修正该字段",
             email: "请输入正确格式的电子邮件",
             number: "请输入合法的数字",
             digits: "只能输入整数",
             maxlength: jQuery.format("号码长度最多为 {0} "),
             minlength: jQuery.format("号码长度最少是 {0} "),
             rangelength: jQuery.format("请输入一个长度介于 {0} 和 {1} 之间的字符串"),
             range: jQuery.format("请输入一个介于 {0} 和 {1} 之间的值"),
             max: jQuery.format("请输入一个最大为 {0} 的值"),
             min: jQuery.format("请输入一个最小为 {0} 的值")
         };
        jQuery.extend(jQuery.validator.messages, cnmsg);

        var enumList = new Array();
        var parameters = new Array();
        var currentWFParameter = new Object();
        function onPageLoad() {
            if (!window.dialogArguments)
                return;
            var paraData = window.dialogArguments;
            parameters = Sys.Serialization.JavaScriptSerializer.deserialize(paraData.parametersStr);
            enumList = Sys.Serialization.JavaScriptSerializer.deserialize(paraData.enumParameterTypeListStr);

            if (paraData.jsonStr == null) {
                currentWFParameter = Sys.Serialization.JavaScriptSerializer.deserialize(document.getElementById("hiddenWfParameterTemplate").value);
                setPage(currentWFParameter);
            } else {
                currentWFParameter = Sys.Serialization.JavaScriptSerializer.deserialize(paraData.jsonStr);
                setPage(currentWFParameter.wfParameter);
            }

            onBinadDropDownList();
        }

        function setPage(currentWFParameter) {
            if (currentWFParameter.parameterName) {
                var tbParameterName = document.getElementById("tb_parameterName");
                tbParameterName.value = currentWFParameter.parameterName;
            }

            if (currentWFParameter.controlID) {
                var tbControlID = document.getElementById("tb_controlID");
                tbControlID.value = currentWFParameter.controlID;
            }

            if (currentWFParameter.controlPropertyName) {
                var tbControlPropertyName = document.getElementById("tb_controlPropertyName");
                tbControlPropertyName.value = currentWFParameter.controlPropertyName;
            }

            if (currentWFParameter.autoCollect) {
                var drAutoCollect = document.getElementById("dr_autoCollect");
                drAutoCollect.value = currentWFParameter.autoCollect;
            }
        }

        function onbtnOKClick() {
            //document.getElementById("form1").submit();
            var tbParameterName = document.getElementById("tb_parameterName");
            //tbParameterName.disabled = true;
            var tbControlID = document.getElementById("tb_controlID");
            var tbControlPropertyName = document.getElementById("tb_controlPropertyName");
            var drEnumParameterType = document.getElementById("se_enumParameterType");
            var drAutoCollect = document.getElementById("dr_autoCollect");
            if (!validate()) {
                return false;
            }
            if (currentWFParameter.wfParameter) {
                currentWFParameter.wfParameter.parameterName = tbParameterName.value;
                currentWFParameter.wfParameter.controlID = tbControlID.value;
                currentWFParameter.wfParameter.controlPropertyName = tbControlPropertyName.value;
                currentWFParameter.wfParameter.parameterType = drEnumParameterType.value;
                currentWFParameter.wfParameter.autoCollect = drAutoCollect.value;
            } else {
                for (var i = 0; i < parameters.length; i++) {
                    if (parameters[i].parameterName == tbParameterName.value) {
                        alert("已存在相同的参数名称。");
                        return;
                    }
                }
                currentWFParameter.parameterName = tbParameterName.value;
                currentWFParameter.controlID = tbControlID.value;
                currentWFParameter.controlPropertyName = tbControlPropertyName.value;
                currentWFParameter.parameterType = drEnumParameterType.value;
                currentWFParameter.autoCollect = drAutoCollect.value;
            }

            window.returnValue = { jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(currentWFParameter) };

            top.close();
        }

        function validate() {
            var boolResult;

            var procKey = document.getElementById("tb_parameterName").value;
            var reg = /(\'|\\|\/|\*|\:|\?|\"|\<|\>|\|)+/

            if (reg.test(procKey)) {
                alert('参数名称中不能存在以下字符：\' \\ \/ \: \? \" \< \> \| \*');
                return false;
            }

            procKey = document.getElementById("tb_controlID").value;
            if (reg.test(procKey)) {
                alert('控件ID中不能存在以下字符：\' \\ \/ \: \? \" \< \> \| \*');
                return false;
            }

            procKey = document.getElementById("tb_controlPropertyName").value;
            if (reg.test(procKey)) {
                alert('控件属性中不能存在以下字符：\' \\ \/ \: \? \" \< \> \| \*');
                return false;
            }

            boolResult = ($("#form1").validate().element("#tb_parameterName") &&
            $("#form1").validate().element("#tb_controlID") &&
            $("#form1").validate().element("#tb_controlPropertyName"));

            return boolResult;
        }

        function onBinadDropDownList() {
            var dropDownList = document.getElementById("se_enumParameterType");
            for (var i = 0; i < enumList.length; i++) {
                var item = enumList[i];
                var itemOption = new Option(item.Description, item.EnumValue);
                if (typeof (currentWFParameter.index) == "undefined") {
                    if (currentWFParameter.parameterType == item.EnumValue) {
                        itemOption.selected = true;
                    }
                } else {
                    if (currentWFParameter.wfParameter.parameterType == item.EnumValue) {
                        itemOption.selected = true;
                    }
                }
                dropDownList.options.add(itemOption);
            }
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <table width="100%" style="height: 100%; width: 100%">
        <tr>
            <td class="gridHead">
                <div class="dialogTitle">
                    <span class="dialogLogo">自动收集流程参数编辑</span>
                </div>
            </td>
        </tr>
        <tr>
            <td style="vertical-align: center">
                <div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
                    height: 100%; overflow: auto">
                    <!--Put your dialog content here... -->
                    <table width="100%" style="height: 100%; width: 100%;">
                        <tr style="height: 10px;">
                            <td style="width: 100px;">
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" valign="middle">
                                参数名称:
                            </td>
                            <td valign="middle">
                                <input id="tb_parameterName" style="width: 150px" class="required" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" valign="middle">
                                收集控件ID:
                            </td>
                            <td valign="middle">
                                <input id="tb_controlID" style="width: 150px" class="required" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" valign="middle">
                                收集控件属性
                            </td>
                            <td valign="middle">
                                <input id="tb_controlPropertyName" style="width: 150px" class="required" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" valign="middle">
                                数据类型:
                            </td>
                            <td valign="middle">
                                <select id="se_enumParameterType" style="width: 150px" class="required">
                                </select>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" valign="middle">
                                是否自动收集:
                            </td>
                            <td valign="middle">
                                <select id="dr_autoCollect" style="width: 150px" class="required">
                                    <option value="true" defaultselected="true">是</option>
                                    <option value="false">否</option>
                                </select>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td class="gridfileBottom">
            </td>
        </tr>
        <tr>
            <td style="height: 40px; text-align: center; vertical-align: middle">
                <table style="width: 100%; height: 100%">
                    <tr>
                        <td style="text-align: center;">
                            <input id="btnOK" type="button" value="确定(O)" accesskey="O" onclick="onbtnOKClick();"
                                class="formButton" />
                        </td>
                        <td style="text-align: center;">
                            <input id="btnCancel" type="button" value="取消(C)" accesskey="C" onclick="top.close();"
                                class="formButton" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="height: 2px">
                <input type="hidden" runat="server" id="hiddenWfParameterTemplate" />
            </td>
        </tr>
    </table>
    </form>
    <script type="text/javascript">
        Sys.Application.add_load(function () {
            onPageLoad();
        });
    </script>
</body>
</html>
