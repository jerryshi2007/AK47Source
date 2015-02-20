<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientMsgTest.aspx.cs"
    Inherits="MCS.Web.Responsive.WebControls.Test.ModalBox.ClientMsgTest" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>模态窗口</title>
    <script type="text/javascript">
        function onShowError() {
            var error = Error.create("Hello ExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionException");
            error.description = "Hello ExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionException Hello ExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionException Hello ExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionException";
            $showError(error);
            //$HGClientMsg.inform("Hello ExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionException", "Hello ExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionException Hello ExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionException Hello ExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionException", "Info");
        }

        function onShowInform() {
            var msg = "Hello ExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionException";
            var description = "Hello ExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionException Hello ExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionException Hello ExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionException";
            $HGClientMsg.inform(msg, description);
        }

        function onShowAlert() {
            var msg = "Hello ExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionException";
            var description = "Hello ExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionException Hello ExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionException Hello ExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionExceptionException";
            $HGClientMsg.alert(msg, description);
        }

        function onShowConfirm() {
            var msg = "确认还是取消？";
            var description = "取消吧。。。";
            $HGClientMsg.confirm(msg, description, "", "", "", function () { alert("确认！"); }, function () { alert("取消！"); });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
        </asp:ScriptManager>
    </div>
    <div style="padding-left: 10px;padding-top: 10px;">
        <input type="button" value="Show Error" class="btn btn-default" onclick="onShowError();" />
        <input type="button" value="Inform" class="btn btn-default" onclick="onShowInform();" />
        <input type="button" value="Alert" class="btn btn-default" onclick="onShowAlert();" />
        <input type="button" value="Confirm" class="btn btn-default" onclick="onShowConfirm();" />
        </div>
    </form>
    </body>
</html>
