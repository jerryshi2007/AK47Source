<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExpressionEditor.aspx.cs"
    Inherits="PermissionCenter.Dialogs.ExpressionEditor" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" scroll="no" style="overflow: hidden">
<head runat="server">
    <title>条件表达式窗口</title>
    <link href="../styles/dlg.css" rel="stylesheet" type="text/css" />
    <base target="_self" />
</head>
<body class="pcdlg" style="min-height: 200px; min-width: 0; height: 200px;">
    <form id="form1" runat="server">
    <pc:SceneControl ID="SceneControl1" runat="server">
    </pc:SceneControl>
    <div class="pcdlg-sky">
        <h1 class="pc-caption">
            条件表达式编辑<span class="pc-timepointmark">
                <mcs:TimePointDisplayControl ID="TimePointDisplayControl1" runat="server" />
            </span>
        </h1>
    </div>
    <div class="pcdlg-content">
        <div class="pc-container5">
            <span>描述</span><asp:RequiredFieldValidator ID="validatorDesc" runat="server" ControlToValidate="txtDesc"
                ErrorMessage="此项不可省略"></asp:RequiredFieldValidator>
            &nbsp;<div>
                <asp:TextBox runat="server" CssClass="" ID="txtDesc" />
                <span class="pc-required" id="txtMsg"></span>
            </div>
            <span>表达式</span><asp:RequiredFieldValidator ID="validatorExp" runat="server" ControlToValidate="txtExp"
                ErrorMessage="此项不可省略"></asp:RequiredFieldValidator><a href="javascript:void(0);"
                    onclick="showHelp(); return false;"><img src="../Images/help.png" alt="表达式编写提示" /></a>
            &nbsp;<div>
                <asp:TextBox runat="server" TextMode="MultiLine" Rows="10" Columns="50" ID="txtExp" />
            </div>
        </div>
    </div>
    <div class="pcdlg-floor">
        <div class="pcdlg-button-bar">
            <input type="button" id="okButton" runat="server" onclick="handleOk()" accesskey="S"
                class="pcdlg-button btn-def" value="确定(O)" causesvalidation="true" />
            <input type="button" accesskey="C" class="pcdlg-button btn-cancel" onclick="handleCancel();"
                value="关闭(C)" /><input type="button" accesskey="K" class="pcdlg-button" value="检查表达式(K)"
                    onclick="handleCheck();" /><i id="ldicon" class="pc-icon-loader pc-hide">
            </i>
        </div>
    </div>
    </form>
    <script type="text/javascript">

        $pc.ui.traceWindowWidth();

        function showHelp() {
            if (_adapter) {
                var schemaType = _adapter.getSchemaType();
                $pc.showDialog("ExpressionHint.aspx?schemaType=" + schemaType, '', null, false, 800, 600);
            }
        }

        function validateEntry(validationGroup) {

            var isValidEntry = true;

            if (typeof (Page_Validators) != 'undefined') {

                for (var i = 0; i < Page_Validators.length; i++) {

                    if (Page_Validators[i].validationGroup == validationGroup) {

                        // call validator function

                        var func = Page_Validators[i].evaluationfunction;

                        Page_Validators[i].isvalid = func(Page_Validators[i]);

                        if (!Page_Validators[i].isvalid) {

                            isValidEntry = false;

                            Page_Validators[i].style.visibility = '';
                        }
                    }
                }
            }

            return isValidEntry;
        }

        var _adapter = null;
        function registerAdapter(adapter) {
            if (adapter) {
                if (typeof (adapter.doOk) === 'undefined')
                    throw new "adapter必须提供函数数doOk";
                if (typeof (adapter.doCheck) === 'undefined')
                    throw new "adapter必须提供函数数doCheck";
                if (typeof (adapter.doCancel) === 'undefined')
                    throw new "adapter必须提供函数数doCancel";
                _adapter = adapter;
            } else {
                throw new "必须提供参数adapter";
            }
        }

        function setData(desc, exp) {
            document.getElementById("txtDesc").value = desc;
            document.getElementById("txtExp").value = exp;
        }

        function getData() {
            var _desc = document.getElementById("txtDesc").value;
            var _exp = document.getElementById("txtExp").value;
            var obj = { desc: _desc, exp: _exp };
            _desc = null;
            _exp = null;
            return obj;
        }

        function setMessage(msg) {
            msg = msg || "";
            var span = document.getElementById("txtMsg");
            if (span.firstChild) {
                span.replaceChild(document.createTextNode(msg), span.firstChild);
            } else {
                span.appendChild(document.createTextNode(msg));
            }
        }

        function setCheckResult(success, message) {
            document.getElementById("ldicon").style.display = "none";
            if (message) {
                alert(message);
            } else {
                alert(success ? "校验通过" : "校验未通过或操作遇到错误");
            }

        }

        function clearCheckResult() {
            document.getElementById("ldicon").style.display = "none";
        }

        function handleOk() {

            if (validateEntry()) {

                if (_adapter)
                    _adapter.doOk();
            }
        }

        function handleCheck() {
            if (_adapter) {
                document.getElementById("ldicon").style.display = "inline-block";
                _adapter.doCheck();
            }
        }

        function handleCancel() {
            if (_adapter)
                _adapter.doCancel();

        }

        if (window.dialogArguments) {
            //启用对话框模式
            window.dialogArguments.clientWindow = window;
            window.dialogArguments.initExpressionEditor(window);
            window.dialogArguments.onDialogLoad();
        } else if (window.parent.initExpressionEditor) {
            window.parent.initExpressionEditor.call(window.parent, window);
        }

        window.onbeforeunload = function () {
            if (_adapter)
                if (_adapter.doUnloadDialog)
                    _adapter.doUnloadDialog();
        }

    </script>
</body>
</html>
