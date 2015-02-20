<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubmitButtonWithProgress.aspx.cs"
    Inherits="MCS.Web.Responsive.WebControls.Test.SubmitButton.SubmitButtonWithProgress" %>

<%@ Register TagPrefix="mcsr" Namespace="MCS.Web.Responsive.WebControls" Assembly="MCS.Web.Responsive.WebControls" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>向Frame提交，并且存在过程变化</title>
</head>
<body>
    <form id="serverForm" runat="server" target="innerFrame">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
    </div>
    <div>
        <mcsr:SubmitButton runat="server" ID="submitWithProcess" Text="带进度的提交" PopupCaption="正在提交..."
            OnClick="submitWithProcess_Click" ProgressMode="BySteps" />
        <mcsr:SubmitButton runat="server" ID="SubmitButton1" Text="只是提交，进度条固定" PopupCaption="正在提交..."
            OnClick="submitWithFrose_Click" ProgressMode="BySteps" />
        <mcsr:SubmitButton runat="server" ID="SubmitButton2" Text="异步调用方法" PopupCaption="正在提交..."
            OnClick="submitWithProcess_Click" ProgressMode="ByTimeInterval" AsyncInvoke="asyncFunction" />
    </div>
    <div>
        <iframe id="innerFrame" name="innerFrame"></iframe>
    </div>
    </form>
    <script type="text/javascript">
        function asyncFunction() {
           return confirm("请选择返回结果：");
        }
    
    </script>
</body>
</html>
