<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubmitButtonTestForm.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.SubmitButton.SubmitButtonTestForm" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
        <HB:SubmitButton runat="server" ID="submitBtn" Text="Submit Button" PopupCaption="正在提交..."
            OnClick="submitBtn_Click" OnClientClick="test()" />
        <HB:SubmitButton runat="server" ID="saveBtn" Text="Save Button" PopupCaption="正在保存..."
            OnClick="submitBtn_Click" />
        <HB:SubmitButton runat="server" ID="SubmitButton1" Text="Long Term Save Button" ProgressInterval="00:00:00.400" PopupCaption="这个等候时间比较长..."
            OnClick="SubmitButton1_Click" />
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
        <HB:SubmitButton runat="server" ID="SubmitButton2" Text="查询" PopupCaption="查询中。。。"
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
