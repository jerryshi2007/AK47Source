<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.DeluxeTree.Test" %>

<%@ Register assembly="MCS.Web.WebControls" namespace="MCS.Web.WebControls" tagprefix="DeluxeWorks" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    	 <DeluxeWorks:DeluxeTree ID="templateTree" runat="server" Width="200px" Height="300px"
            BorderStyle="Solid" BorderWidth="1px" CollapseImage="" ExpandImage="" NodeCloseImg="closeImg.gif"
            NodeOpenImg="openImg.gif" CallBackContext="Test Context" NodeIndent="16">
            <Nodes>
                <DeluxeWorks:DeluxeTreeNode Text="第一个节点" ShowCheckBox="True" CssClass="" NodeCloseImg=""
                    NodeOpenImg="" SelectedCssClass="" Checked="True" NavigateUrl="" Target="" />
            </Nodes>
        </DeluxeWorks:DeluxeTree>

    
    </div>
    </form>
</body>
</html>
