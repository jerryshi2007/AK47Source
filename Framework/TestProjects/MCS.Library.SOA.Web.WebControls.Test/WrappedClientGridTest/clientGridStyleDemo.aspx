<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="clientGridStyleDemo.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest.clientGridStyleDemo" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <SOA:ClientGrid runat="server" ID="detailGrid" Caption="" Width="100%" ReadOnly="true"
            AllowPaging="true" PageSize="5">
            <Columns>
                <SOA:ClientGridColumn SelectColumn="true" ShowSelectAll="true" />
                <SOA:ClientGridColumn DataField="SelectedUserId" HeaderText="员工编码" DataType="string"
                    ItemStyle="{width:'25%'}">
                    <EditTemplate EditMode="TextBox" />
                </SOA:ClientGridColumn>
                <SOA:ClientGridColumn DataField="SelectedUserDisplayName" HeaderText="员工姓名" DataType="string"
                    ItemStyle="{width:'25%'}">
                    <EditTemplate EditMode="TextBox" />
                </SOA:ClientGridColumn>
                <SOA:ClientGridColumn DataField="SelectedUserUserFullPath" HeaderText="人员标示" DataType="string"
                    ItemStyle="{width:'50%'}" EditorStyle="{width:'95%'}">
                    <EditTemplate EditMode="TextBox" />
                </SOA:ClientGridColumn>
            </Columns>
        </SOA:ClientGrid>
    </div>
    </form>
</body>
</html>
