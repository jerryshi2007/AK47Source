<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubmitButtonTestForm.aspx.cs"
    Inherits="MCS.Web.Responsive.WebControls.Test.SubmitButton.SubmitButtonTestForm" %>

<%@ Register TagPrefix="mcsr" Namespace="MCS.Web.Responsive.WebControls" Assembly="MCS.Web.Responsive.WebControls" %>
<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>SubmitButton Test</title>
    <script type="text/javascript">
        function test() {
            alert();
        }

        function fefefefe() {
            document.getElementById("submitBtn").click();
        }
    </script>
</head>
<body>
    <form id="serverForm" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <h1>
        标题</h1>
    <div>
        <a runat="server" href="#" id="relSubmit" onclick="event.returnValue = !event.srcElement.disabled;">
            提交按钮的相关控件</a>
        <input id="fefefe" value="我做测试" onclick="fefefefe()" type="button" />
        <mcsr:SubmitButton runat="server" ID="submitBtn" Text="Submit Button" PopupCaption="正在提交..."
            OnClick="submitBtn_Click" OnClientClick="test()" />
        <mcsr:SubmitButton runat="server" ID="saveBtn" Text="Save Button" PopupCaption="正在保存..."
            OnClick="submitBtn_Click" />
        <mcsr:SubmitButton runat="server" ID="SubmitButton1" Text="Long Term Save Button"
            ProgressInterval="00:00:00.400" PopupCaption="这个等候时间比较长..." OnClick="SubmitButton1_Click" />
    </div>
    <br />
    <div>
        <script type="text/javascript">
            //在UpdatePanel更新完毕后执行指定的方法，重置SubmitButton的状态
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(blah);
            function blah() {
                SubmitButton.resetAllStates();
            }
        </script>
        <mcsr:SubmitButton runat="server" ID="SubmitButton2" Text="查询" PopupCaption="查询中。。。"
            OnClick="SubmitButton2_Click" />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div>
                    <asp:TextBox runat="server" ID="tbx_msg"></asp:TextBox>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="SubmitButton2" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
