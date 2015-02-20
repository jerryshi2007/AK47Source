<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WfExternalUserEditor.aspx.cs" Inherits="WorkflowDesigner.ModalDialog.WfExternalUserEditor" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>外部人员编辑</title>
    <style type="text/css">

    </style>
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
        var wfExtuser = {};
        var existExtUser = {};
        function onDocumentLoad() {
            var paraData = window.dialogArguments;
            existExtUser = Sys.Serialization.JavaScriptSerializer.deserialize(paraData.existExternalUserJsonStr);
            if (!window.dialogArguments) 
                return;
            if (!paraData.jsonStr) {
                wfExtuser = Sys.Serialization.JavaScriptSerializer.deserialize(document.getElementById("hiddenExtUserTemplate").value);
            } else {
                wfExtuser = Sys.Serialization.JavaScriptSerializer.deserialize(paraData.jsonStr);
                setPage(wfExtuser.extUserobj);
            }
        }
        function setPage(extUserobj) {

            var extUserKey = document.getElementById("extUserKey");
            extUserKey.disabled = true;
            var extUserName = document.getElementById("extUserName");
            var extUserGender = document.getElementById("ddlGender");
            var extUserPhone = document.getElementById("phone");
            var extUserMobilePhone = document.getElementById("mobilePhone");
            var extUserTitle = document.getElementById("title");
            var extUserEmail = document.getElementById("emailAddress");
            extUserKey.value = extUserobj.Key;
            extUserName.value = extUserobj.Name;
            extUserGender.value = extUserobj.Gender;
            extUserPhone.value = extUserobj.Phone;
            extUserMobilePhone.value = extUserobj.MobilePhone;
            extUserTitle.value = extUserobj.Title;
            extUserEmail.value = extUserobj.Email;
        }
        function onbtnOKClick() {
            //document.getElementById("form1").submit();
            var extUserKey = document.getElementById("extUserKey");
            var extUserName = document.getElementById("extUserName");
            var extUserGender = document.getElementById("ddlGender");
            var extUserPhone = document.getElementById("phone");
            var extUserMobilePhone = document.getElementById("mobilePhone");
            var extUserTitle = document.getElementById("title");
            var extUserEmail = document.getElementById("emailAddress");
            if (!validate()) {
                return false;
            }
            if (wfExtuser.extUserobj) {
                wfExtuser.extUserobj.Key = extUserKey.value;

                wfExtuser.extUserobj.Name = extUserName.value;
                wfExtuser.extUserobj.Gender = extUserGender.value;
                wfExtuser.extUserobj.Phone = extUserPhone.value;
                wfExtuser.extUserobj.MobilePhone = extUserMobilePhone.value;
                wfExtuser.extUserobj.Title = extUserTitle.value;
                wfExtuser.extUserobj.Email = extUserEmail.value;
            } else {
                for (var i = 0; i < existExtUser.length; i++) {
                    if (existExtUser[i].Key == extUserKey.value) {
                        alert("外部相关人员Key重复。");
                        return;
                    }
                }
                wfExtuser.Key = extUserKey.value;
                wfExtuser.Name = extUserName.value;
                wfExtuser.Gender = extUserGender.value;
                wfExtuser.Phone = extUserPhone.value;
                wfExtuser.MobilePhone = extUserMobilePhone.value;
                wfExtuser.Title = extUserTitle.value;
                wfExtuser.Email = extUserEmail.value;
            }
            window.returnValue = { jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(wfExtuser) };
            top.close();
        }
        function validate() {
            var boolResult;

            var procKey = document.getElementById("extUserKey").value;
            var reg = /(\'|\\|\/|\*|\:|\?|\"|\<|\>|\|)+/

            if (reg.test(procKey)) {
                alert('Key中不能存在以下字符：\' \\ \/ \: \? \" \< \> \| \*');
                return false;
            }

            boolResult = ($("#form1").validate().element("#extUserKey") &&
            $("#form1").validate().element("#extUserName") &&
            $("#form1").validate().element("#ddlGender") &&
            $("#form1").validate().element("#phone") &&
            $("#form1").validate().element("#mobilePhone") &&
            $("#form1").validate().element("#emailAddress"))
            return boolResult;
        }
    </script>
</head>
<body onload="onDocumentLoad();">
    <form id="form1" runat="server" method="post">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <table width="100%" style="height: 100%; width: 100%">
        <tr>
            <td class="gridHead">
                <div class="dialogTitle">
                    <span class="dialogLogo">外部人员编辑</span>
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
                            <td style="width:100px;">
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" valign="middle">
                                Key:
                            </td>
                            <td valign="middle">
                                <input id="extUserKey" class="required"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" valign="middle">
                                名称:
                            </td>
                            <td valign="middle">
                                <input id="extUserName" class="required" /></td>
                        </tr>
                        <tr>
                            <td class="label" valign="middle">
                                性别</td>
                            <td valign="middle">
                                <cc1:hbdropdownlist ID="ddlGender" runat="server" class="required">
                                </cc1:hbdropdownlist>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" valign="middle">
                                固定电话:
                            </td>
                            <td valign="middle">
                                <input id="phone" maxlength="8" class="number required" minlength="8"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" valign="middle">
                                移动电话:
                            </td>
                            <td valign="middle">
                                <input id="mobilePhone" maxlength="11" class="number required" minlength="11"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" valign="middle">
                                头衔:
                            </td>
                            <td valign="middle">
                                <input id="title" /></td>
                        </tr>
                        <tr>
                            <td class="label" valign="middle">
                                邮箱地址:
                            </td>
                            <td valign="middle">
                                <input id="emailAddress" class="required email "/></td>
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
                            <input id="btnOK" type="button" value="确定(O)" accesskey="O" onclick="onbtnOKClick();" class="formButton" />
                        </td>
                        <td style="text-align: center;">
                            <input id="btnCancel" type="button" value="取消(C)" accesskey="C" onclick="top.close();"
                                class="formButton" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <input type="hidden" runat="server" id="hiddenExtUserTemplate" />
        </form>
</body>
</html>
