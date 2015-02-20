<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WfCollectParameterControlTest.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.WfCollectParameterControlTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOAControl" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <SOAControl:WfCollectParameterControl runat="server" ID="collectParameterControl"
            AutoCollectDataWhenPostBack="true"></SOAControl:WfCollectParameterControl>
    </div>
    <div>
        <asp:Button runat="server" ID="bt_s" Text="测试" OnClick="bt_s_Click" CausesValidation="true" />
    </div>
    <%-- <div>
        <SOAControl:HBDropDownList runat="server" ID="testControl" DataValueField="ID" DataTextField="Name">
        </SOAControl:HBDropDownList>
    </div>
    <div>
        <MCS:DeluxeCalendar runat="server" ID="DeluxeCalendar1">
        </MCS:DeluxeCalendar>
    </div>--%>
    </form>
</body>
</html>
