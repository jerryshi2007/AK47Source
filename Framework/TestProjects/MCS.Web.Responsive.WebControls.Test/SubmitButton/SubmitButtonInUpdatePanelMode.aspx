<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubmitButtonInUpdatePanelMode.aspx.cs"
    Inherits="MCS.Web.Responsive.WebControls.Test.SubmitButton.SubmitButtonInUpdatePanelMode" %>

<%@ Register TagPrefix="mcsr" Namespace="MCS.Web.Responsive.WebControls" Assembly="MCS.Web.Responsive.WebControls" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>测试在UpdatePanel模式下的SubmitButton</title>
</head>
<body>
    <form id="serverForm" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
        </asp:ScriptManager>
    </div>
    <div>
        <h1>
            测试UpdatePanel</h1>
    </div>
    <div>
        <input type="text" id="serverResult" />
    </div>
    <div>
        <asp:UpdatePanel runat="server" ID="panel">
            <ContentTemplate>
                <mcsr:SubmitButton runat="server" Text="提交..." PopupCaption="正在提交..." ID="submitBtn"
                    OnClick="submitBtn_Click" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
