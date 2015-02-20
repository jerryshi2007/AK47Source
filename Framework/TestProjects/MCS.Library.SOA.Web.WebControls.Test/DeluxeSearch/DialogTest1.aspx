<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DialogTest1.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.DeluxeSearch.DialogTest1" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>高级搜索</title>
    <base target="_self" />
    <style type="text/css">
        .formButton
        {
            font-weight: normal;
            border: 1px solid #b5b3b3;
            height: 24px;
            min-width: 80px;
            line-height: 22px;
            color: black;
            margin-left: 10px;
            margin-top: 3px;
            text-align: center;
            letter-spacing: 3px;
            padding: 0px 2px;
        }
    </style>
</head>
<body>
    <form id="frmAdvancedSearch" runat="server" target="iframe">
    <cc1:DeluxeSearchClient ID="Search1" runat="server" />
    <cc1:DataBindingControl runat="server" ID="bindingControl" AllowClientCollectData="True">
        <ItemBindings>
            <cc1:DataBindingItem ControlID="ddlScope" DataPropertyName="SupplierRegionalCode">
            </cc1:DataBindingItem>
            <cc1:DataBindingItem ControlID="ddlType" DataPropertyName="Status">
            </cc1:DataBindingItem>
        </ItemBindings>
    </cc1:DataBindingControl>
    <table style=" width: 100%; height: 100%; padding: 0px;border-collapse:collapse;border:0;margin:0;">
        <tr>
            <td style="height: 32px">
                行业领域
            </td>
            <td style="height: 32px">
                <asp:TextBox runat="server" ID="txtArea" Width="220px"></asp:TextBox>
            </td>
            <td style="height: 32px">
                供应商名称
            </td>
            <td style="height: 32px">
                <asp:TextBox runat="server" ID="txtName" Width="220px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="height: 32px">
                区域
            </td>
            <td style="height: 32px">
                <cc1:HBDropDownList runat="server" ID="ddlScope" Width="220px">
                </cc1:HBDropDownList>
            </td>
            <td style="height: 32px">
                供应商类别
            </td>
            <td style="height: 32px">
                <cc1:HBDropDownList runat="server" ID="ddlType" Width="220px">
                </cc1:HBDropDownList>
            </td>
        </tr>
        <tr>
            <td style="height: 32px">
                总部地址
            </td>
            <td style="height: 32px">
                <asp:TextBox runat="server" ID="txtAddress" Width="220px"></asp:TextBox>
            </td>
            <td style="height: 32px">
                注册资本(万元)
            </td>
            <td style="height: 32px">
                <asp:TextBox runat="server" ID="txtRegisteredCapital" Width="220px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="height: 2px; padding: 4px;" colspan="4">
                <hr size="1" />
            </td>
        </tr>
        <tr>
            <td style="height: 32px; padding-bottom: 8px" colspan="4">
                <table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%">
                    <tr>
                        <td style="text-align: center">
                            <asp:Button runat="server" ID="btnOK" Text="确认(O)" OnClick="BtnOkClick" AccessKey="O"
                                CssClass="formButton" />
                        </td>
                        <td style="text-align: center">
                            <input accesskey="C" value="取消(C)" type="button" class="formButton" onclick="window.close();"
                                id="cancelButton" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
    <iframe width="100%" id="iframe" name="iframe" />
</body>
</html>
