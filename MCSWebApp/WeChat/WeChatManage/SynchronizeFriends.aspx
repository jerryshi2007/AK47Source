<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SynchronizeFriends.aspx.cs" Inherits="WeChatManage.SynchronizeFriends" %>

<%@ Register assembly="MCS.Library.SOA.Web.WebControls" namespace="MCS.Web.WebControls" tagprefix="msc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>同步微信好友</title>
    <link href="css/Style.css" rel="stylesheet" type="text/css" />
	<link href="css/form.css" rel="stylesheet" type="text/css" />
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
