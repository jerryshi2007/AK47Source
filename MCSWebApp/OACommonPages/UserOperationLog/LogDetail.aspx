<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogDetail.aspx.cs" Inherits="MCS.OA.CommonPages.UserOperationLog.LogDetail" %>

<%@ Import Namespace="MCS.Library.Globalization" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title runat="server" category="OACommons">详细日志</title>
    <link href="../CSS/Ajax.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/Dialog.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/ItemDetail.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/Login.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/Other.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/style.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div id="dcontainer">
            <table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;" border="0">
                <tr style="height: 45px;">
                    <td valign="top">
                        <div id="dheader">
                            <h1>
                                <mcs:TranslatorLabel runat="server" Category="OACommons" Text="详细日志" />
                            </h1>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div id="dcontent">
                            <table cellspacing="0" cellpadding="0" style="height: 100%; width: 96%;">
                                <tr>
                                    <td class="fim_l" style="width: 100px;">
                                        <mcs:TranslatorLabel runat="server" Category="OACommons" Text="标题" />:
                                    </td>
                                    <td>
                                        <asp:Label runat="server" ID="subject"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="fim_l" style="width: 100px;">
                                        <mcs:TranslatorLabel runat="server" Category="OACommons" Text="应用名称" />:
                                    </td>
                                    <td>
                                        <asp:Literal ID="appName" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="fim_l" style="width: 100px;">
                                        <mcs:TranslatorLabel runat="server" Category="OACommons" Text="环节名称" />:
                                    </td>
                                    <td>
                                        <asp:Literal ID="actName" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="fim_l" style="width: 100px;">
                                        <mcs:TranslatorLabel runat="server" Category="OACommons" Text="动作名称" />:
                                    </td>
                                    <td>
                                        <asp:Literal ID="actionName" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="fim_l" style="width: 100px;">
                                        <mcs:TranslatorLabel runat="server" Category="OACommons" Text="操作人" />:
                                    </td>
                                    <td>
                                        <asp:Literal ID="opUserID" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="fim_l" style="width: 100px;">
                                        <mcs:TranslatorLabel runat="server" Category="OACommons" Text="操作人组织" />:
                                    </td>
                                    <td>
                                        <asp:Literal ID="topDeptID" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="fim_l" style="width: 100px;">
                                        <mcs:TranslatorLabel runat="server" Category="OACommons" Text="操作时间" />:
                                    </td>
                                    <td>
                                        <asp:Literal ID="opDateTime" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="fim_l" style="width: 100px;">
                                        <mcs:TranslatorLabel runat="server" Category="OACommons" Text="详细信息" />:
                                    </td>
                                    <td>
                                        <asp:Literal ID="description" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="fim_l" style="width: 100px; vertical-align: top;">
                                        <mcs:TranslatorLabel runat="server" Category="OACommons" Text="待办人列表" />:
                                    </td>
                                    <td style="vertical-align: top">
                                        <mcs:DeluxeGrid ID="dataGrid" runat="server" AutoGenerateColumns="False" DataSourceID="objectDataSource" Category="OACommons"
                                            DataSourceMaxRow="0" AllowPaging="True" PageSize="10" Width="100%" DataKeyNames="SendToUserID"
                                            CssClass="dataList" ShowCheckBoxs="False" ShowExportControl="False" EmptyDataText="暂时没有您需要的数据">
                                            <Columns>
                                                <asp:TemplateField HeaderText="待办接收人" SortExpression="SEND_TO_USER_NAME">
                                                    <ItemStyle HorizontalAlign="Left" />
                                                    <ItemTemplate>
                                                        <span style="margin-left: 16px">
                                                            <soa:UserPresence ID="userPresence" runat="server" UserID='<%# Eval("SendToUserID") %>'
                                                                UserDisplayName='<%#Server.HtmlEncode((string)Eval("SendToUserName"))%>' />
                                                        </span>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <PagerStyle CssClass="pager" />
                                            <RowStyle CssClass="item" />
                                            <CheckBoxTemplateItemStyle CssClass="checkbox" />
                                            <CheckBoxTemplateHeaderStyle CssClass="checkbox" />
                                            <HeaderStyle CssClass="head" />
                                            <AlternatingRowStyle CssClass="aitem" />
                                            <%--<EmptyDataTemplate>
                                                暂时没有您需要的数据
                                            </EmptyDataTemplate>--%>
                                            <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" Mode="NextPreviousFirstLast"
                                                NextPageText="下一页" Position="Bottom" PreviousPageText="上一页"></PagerSettings>
                                        </mcs:DeluxeGrid>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td style="height: 80px;" valign="middle">
                        <div id="dfooter">
                            <p style="vertical-align: middle; height: 40px;">
                                <input accesskey="C" type="button" class="portalButton" runat="server" category="OACommons" value="关闭(C)" onclick="top.close();" />
                            </p>
                        </div>
                    </td>
                </tr>
            </table>
            <div style="display: none">
                <soa:DeluxeObjectDataSource ID="objectDataSource" runat="server" EnablePaging="True"
                    TypeName="MCS.OA.CommonPages.UserOperationLog.OperationTasksLogDataSource">
                </soa:DeluxeObjectDataSource>
            </div>
        </div>
    </form>
</body>
</html>
