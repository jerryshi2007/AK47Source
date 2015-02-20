<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CirculateControlTest.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.Workflow.Circulate.CirculateControlTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="MCS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <input runat="server" id="circulate" value="test" type="button" />
        <MCS:WfCirculateControl runat="server" ID="circulateCtl" TargetControlID="circulate" />
    </div>
    </form>
</body>
</html>
