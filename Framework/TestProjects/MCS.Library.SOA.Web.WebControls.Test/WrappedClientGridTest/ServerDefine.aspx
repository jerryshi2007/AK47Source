<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServerDefine.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest.ServerDefine" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
    </asp:ScriptManager>
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <HB:ClientGrid runat="server" ID="clientGrid1" Caption="" Width="570px" ShowEditBar="true">
                </HB:ClientGrid>
                <asp:HiddenField ID="HiddenField1" runat="server" />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Button1" EventName="click" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
    <div>
        <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" /></div>
    </form>
</body>
</html>
