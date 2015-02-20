<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="command.aspx.cs" Inherits="PermissionCenter.CommandConsole.command" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>命令执行页面</title>
    <style type="text/css">
        #header
        {
            font-size: 16px;
            font-weight: bold;
        }
        #commandInput
        {
            width: 280px;
            ime-mode: inactive;
        }
        #resultRow
        {
            font-weight: bold;
            font-size: 12px;
            line-height: 24px;
        }
        #resultContainer
        {
            border: 1px solid black;
            font-size: 12px;
            width: 480px;
            height: 320px;
            overflow: scroll;
            font-family: Consolas;
        }
        .normalResult
        {
            color: Black;
        }
        .errorResult
        {
            color: red;
        }
        .hidden
        {
            display: none;
        }
        .inline
        {
            display: inline;
        }
    </style>
    <script type="text/javascript">
        function onExecCommandBtnClick() {
            var service = new PermissionCenter.CommandConsole.CommandService();
            service.ExecCommand($get("commandInput").value, onSuccess, onFail);

            disableExecStatus();
        }

        function onSuccess(e) {
            enableExecStatus();

            $get("result").className = "normalResult";
            $get("result").innerText = e;
        }

        function onFail(e) {
            enableExecStatus();

            $get("result").className = "errorResult";
            $get("result").innerText = e.get_message();
        }

        function onKeyDown() {
            if (event.keyCode === 13) {
                event.returnValue = false;

                if ($get("execCommandBtn").disabled != true)
                    $get("execCommandBtn").click();
            }
        }

        function enableExecStatus() {
            $get("execCommandBtn").disabled = false;
            $get("status").className = "hidden";
        }

        function disableExecStatus() {
            $get("execCommandBtn").disabled = true;
            $get("status").className = "inline";
        }


        /* FF ,Firefox没有 insertAdjacentElement 方法 */

        if (typeof HTMLElement != "undefined" && !HTMLElement.prototype.insertAdjacentElement) {
            HTMLElement.prototype.insertAdjacentElement = function (where, parsedNode) {
                switch (where) {
                    case 'beforeBegin':
                        this.parentNode.insertBefore(parsedNode, this)
                        break;
                    case 'afterBegin':
                        this.insertBefore(parsedNode, this.firstChild);
                        break;
                    case 'beforeEnd':
                        this.appendChild(parsedNode);
                        break;
                    case 'afterEnd':
                        if (this.nextSibling) this.parentNode.insertBefore(parsedNode, this.nextSibling);
                        else this.parentNode.appendChild(parsedNode);
                        break;
                }
            }

            HTMLElement.prototype.insertAdjacentHTML = function (where, htmlStr) {
                var r = this.ownerDocument.createRange();
                r.setStartBefore(this);
                var parsedHTML = r.createContextualFragment(htmlStr);
                this.insertAdjacentElement(where, parsedHTML)
            }

            HTMLElement.prototype.insertAdjacentText = function (where, txtStr) {
                var parsedText = document.createTextNode(txtStr)
                this.insertAdjacentElement(where, parsedText)
            }
        }
    </script>
</head>
<body onload="$get('commandInput').focus();">
    <form id="serverForm" runat="server">
    <div>
        <asp:ScriptManager runat="server" EnableScriptGlobalization="true">
            <Services>
                <asp:ServiceReference Path="~/CommandConsole/CommandService.asmx" />
                <asp:ServiceReference Path="~/Services/UserDisplacingService.asmx" />
            </Services>
        </asp:ScriptManager>
    </div>
    <div>
        <div id="header">
            请输入命令
        </div>
        <div>
            <asp:TextBox ID="commandInput" runat="server" onkeypress="onKeyDown();" />
            <MCS:AutoCompleteExtender ID="extenderTemplate" runat="server" TargetControlID="commandInput"
                AutoCallBack="true" MinimumPrefixLength="1" DataValueField="Value" DataTextFormatString="{0}"
                OnGetDataSource="extenderTemplate_GetDataSource" />
            <input id="execCommandBtn" type="button" accesskey="E" value="执行(E)" onclick="onExecCommandBtnClick();" />
            <div id="status" class="hidden">
                <img src="../Images/hourglass.gif" style="vertical-align: middle" />正在执行...</div>
        </div>
        <div id="resultRow">
            结果
        </div>
        <div>
            <div id="resultContainer">
                <div id="result" class="normalResult">
                </div>
            </div>
        </div>
        <div>
            <a href="DeluxeCacheInfo.axd" target="_blank">查看缓存...</a>
        </div>
       
    </div>
    </form>
</body>
</html>
