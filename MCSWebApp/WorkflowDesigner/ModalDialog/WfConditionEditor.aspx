<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WfConditionEditor.aspx.cs"
    Inherits="WorkflowDesigner.ModalDialog.WfConditionEditor" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>条件表达式编辑</title>
    <script type="text/javascript" src="../js/jquery-1.4.3.js"></script>
    <script type="text/javascript" src="../js/jquery.json-2.2.js"></script>
    <script type="text/javascript">
        var condition;
        var index;
        $().ready(function () {
            var args = window.dialogArguments;

            condition = jQuery.parseJSON(args.jsonStr);

            index = args.index;

            if (condition == undefined || condition == null) {
                top.close(); return;
            }

            if (condition.Condition) {
                $("#txtName").val(condition.Name);
                $("#txtExpression").val(condition.Condition.Expression);
            } else {
                $("#txtExpression").val(condition.Expression);
            }
            if (condition.Name == null || condition.Name == "") {
                $("#txtName").removeAttr('readonly');
            }
        });

        function Submit() {

            if (IsValidExpression() == false) return;

            if (condition.Condition) {
                condition.Name = $("#txtName").val();
                if (condition.Name == '') {
                    $("#txtName").focus();
                    return;
                }
                condition.Condition.Expression = $("#txtExpression").val();
            }
            else {
                condition.Expression = $("#txtExpression").val();
            }

            window.returnValue = { jsonStr: jQuery.toJSON(condition), index: index };
            top.close();
        }

        function IsValidExpression() {
            var url = '../CommonHttpHandler.ashx';

            jQuery.post(url,
				{ express: $("#txtExpression").val() },
				function (rtn) {
				    var rtnObj = jQuery.parseJSON(rtn);
				    if (eval(rtnObj.Success)) {
				        alert('表达式设置正确！');
				    }
				    else {
				        alert('表达式设置错误！原因：' + rtnObj.Message);
				    }
				});
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <table width="100%" style="height: 100%; width: 100%">
            <tr>
                <td class="gridHead">
                    <div class="dialogTitle">
                        <span class="dialogLogo">条件表达式编辑</span>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="vertical-align: center">
                    <div class="dialogContent" id="dialogContent" runat="server" style="width: 100%; height: 100%; overflow: auto">
                        <!--Put your dialog content here... -->
                        <table width="100%" style="height: 100%; width: 100%;">
                            <tr style="height: 10px;" id="nameTr" runat="server">
                                <td class="label">
                                    <div runat="server" id="divName">名称</div>
                                </td>
                                <td>
                                    <input type="text" runat="server" id="txtName" value="" readonly />
                                </td>
                            </tr>
                            <tr>
                                <td class="label" valign="top" style="width: 108px">表达式(布尔值)
                                </td>
                                <td valign="top">
                                    <textarea id="txtExpression" style="width: 99%; height: 99%;"></textarea>
                                </td>
                            </tr>
                            <%--<tr>
                            <td class="label" valign="top">
                                选择函数
                            </td>
                            <td valign="top">
                                <select id="listFunction">
                                    <option id="Option1" value="">SUM</option>
                                    <option id="Option2" value="">AVERAGE</option>
                                    <option id="Option3" value="">MAX</option>
                                    <option id="Option4" value="">MIN</option>
                                </select>
                            </td>
                        </tr>--%>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td class="gridfileBottom"></td>
            </tr>
            <tr>
                <td style="height: 40px; text-align: center; vertical-align: middle">
                    <table style="width: 100%; height: 100%">
                        <tr>
                            <td colspan="2" style="text-align: center;">
                                <input id="btnOK" type="button" value="确定(O)" accesskey="O" onclick="Submit();" class="formButton" />
                                &nbsp;&nbsp;
						    <input id="btnCancel" type="button" value="取消(C)" accesskey="C" onclick="top.close();"
                                class="formButton" />&nbsp;&nbsp;
                            <input id="btnVerify" type="button" value="校验表达式" accesskey="V" onclick="IsValidExpression();"
                                class="formButton" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
