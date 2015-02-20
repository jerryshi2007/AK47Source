<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.OuUserInputControl.test" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="cc1" %>
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
		      <cc1:OuUserInputControl ID="OuUserInputControl1" runat="server" InvokeWithoutViewState="true" ShowCheckButton="True"
            MultiSelect="false" CanSelectRoot="false"  Enabled="true" />
		<asp:Button ID="Button2" runat="server" Text="Button" OnClick="Button2_Click" />
		</ContentTemplate>
			
		</asp:UpdatePanel>
  
    </div>
    <div>
        <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" /></div>
    </form>
</body>
</html>
