<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WeChatManage.Default" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="soa" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <ul>
        <li><a href="ConditionGroups.aspx">条件组</a></li>
        <li><a href="GroupMembers.aspx">组成员</a></li>
        <li><a href="SynchronizeGroups.aspx">同步组</a></li>
        <li><a href="SynchronizeFriends.aspx">同步好友</a></li>
        <li><a href="SynchronizeRecentMessages.aspx">同步最近消息</a></li>
        <li><a href="MassSendAppMessage.aspx">发送图文消息</a></li>
    </ul> 
      
    </div>
    </form>
</body>
</html>
