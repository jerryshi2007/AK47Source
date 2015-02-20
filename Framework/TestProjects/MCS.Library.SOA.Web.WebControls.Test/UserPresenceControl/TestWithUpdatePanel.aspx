<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestWithUpdatePanel.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.TestWithUpdatePanel" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>测试UserPrecense控件</title>
	
</head>
<body>
	<form id="serverForm" runat="server"><asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
	<div>
        
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <SOA:UserPresence runat="server" ID="userPresence" 
    ShowUserDisplayName="true" /><br/>
            <asp:Button runat="server" ID="btnPostBack" Text="PostBack" onclick="btnPostBack_Click" />
            </ContentTemplate>
           
        </asp:UpdatePanel>
            <SOA:UserPresence runat="server" ID="userPresence1" 
    ShowUserDisplayName="true" />
	</div>
	</form>
</body>
</html>
