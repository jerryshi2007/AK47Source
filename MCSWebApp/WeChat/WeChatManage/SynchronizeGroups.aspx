<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SynchronizeGroups.aspx.cs" Inherits="WeChatManage.SynchronizeGroups" %>
<%@ Register assembly="MCS.Library.SOA.Web.WebControls" namespace="MCS.Web.WebControls" tagprefix="msc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>同步微信组</title>
</head>
<body>
    <form id="form1" runat="server" target="innerFrame">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
		</asp:ScriptManager>
    <div>
      选择帐号：<asp:DropDownList ID="ddlAccount" runat="server"> 
        </asp:DropDownList>
        <msc:SubmitButton ID="btnConfirm" Text="同步" PopupCaption="正在同步..." 
            runat="server"  ProgressMode="BySteps" onclick="btnConfirm_Click"/>
    </div>
    <div style="display:none">
		<iframe id="innerFrame" name="innerFrame"></iframe>
	</div>
    </form>
</body>
</html>
