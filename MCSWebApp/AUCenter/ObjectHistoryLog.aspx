<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ObjectHistoryLog.aspx.cs"
    Inherits="AUCenter.ObjectHistoryLog" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>对象变更日志</title>
    <link rel="icon" href="favicon.ico" type="image/x-icon" />
    <link rel="Shortcut Icon" href="favicon.ico" />
    <base target="_self" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="pc-banner">
        <h1>
            检索变更历史 (对象类型<label id="schemaTypeLabel" runat="server"></label>)</h1>
    </div>
    <div class="pc-tabs-header">
        <ul>
            <li class="pc-active">
                <asp:HyperLink ID="lnkList" runat="server">列表</asp:HyperLink></li>
        </ul>
    </div>
    <div class="pc-container5" style="table-layout: fixed">
        <mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
            AllowPaging="True" AllowSorting="True" Category="PermissionCenter" GridTitle="历史事件"
            DataKeyNames="VersionStartTime" CssClass="dataList pc-datagrid pc-fixedtable"
            TitleCssClass="title" PagerSettings-Position="Bottom" ShowExportControl="false"
            ShowHeader="true" DataSourceMaxRow="0" TitleColor="141, 143, 149" TitleFontSize="Large"
            OnRowDataBound="HandleRowBound" PageSize="50">
            <EmptyDataTemplate>
                暂时没有您需要的数据
            </EmptyDataTemplate>
            <HeaderStyle CssClass="head" />
            <Columns>
                <asp:TemplateField HeaderText="修改时间">
                    <ItemTemplate>
                        <div>
                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("VersionStartTime", "{0:yyyy-MM-dd HH:mm:ss}") %>'></asp:Label></div>
                        <div>
                            <div class="pc-action-tray" id="d" runat="server" visible='<%# this.gridMain.ExportingDeluxeGrid == false %>'>
                                <a class="pc-item-cmd" data-id='<%# HttpUtility.HtmlAttributeEncode(this.id)%>'
                                    data-time='<%# ((DateTime)Eval("VersionStartTime")).ToUniversalTime().ToString("O") %>'
                                    href="javascript:void(0);" onclick='$pc.popups.historyProperty(this);'>历史属性</a>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="状态">
                    <ItemTemplate>
                        <asp:Label ID="lblStatus" runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="修改者">
                    <ItemTemplate>
                        <soa:UserPresence runat="server" ID="userPresence" UserDisplayName='<%# Server.HtmlEncode((string)Eval("CreatorName")) %>'
                            ShowUserIcon="false" StatusImage="Ball" UserID='<%#Eval("CreatorId") %>'>
                        </soa:UserPresence>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <PagerStyle CssClass="pager" />
            <RowStyle CssClass="item" />
            <CheckBoxTemplateItemStyle CssClass="checkbox" />
            <AlternatingRowStyle CssClass="aitem" />
            <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" Mode="NextPreviousFirstLast"
                NextPageText="下一页" Position="TopAndBottom" PreviousPageText="上一页"></PagerSettings>
            <SelectedRowStyle CssClass="selecteditem" />
            <CheckBoxTemplateHeaderStyle CssClass="checkbox" />
            <PagerTemplate>
            </PagerTemplate>
        </mcs:DeluxeGrid>
    </div>
    <asp:ObjectDataSource ID="dataSourceMain" runat="server" EnablePaging="False" TypeName="MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources.ObjectHistoryDataSource"
        SelectMethod="GetAllHistoryEntry" EnableViewState="false" OnSelected="dataSourceMain_Selected">
        <SelectParameters>
            <asp:QueryStringParameter QueryStringField="id" Type="String" Name="id" DefaultValue="773BA455-6A2E-4A71-BDC7-AFE689789390" />
            <asp:Parameter Direction="Output" Type="String" Name="schemaType" />
        </SelectParameters>
    </asp:ObjectDataSource>
    </form>
    <script type="text/javascript">
        $pc.ui.gridBehavior("gridMain", "hover");
        $pc.ui.traceWindowWidth();
    </script>
</body>
</html>
