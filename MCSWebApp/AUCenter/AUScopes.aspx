<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AUScopes.aspx.cs" Inherits="AUCenter.AUScopes" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>管理范围</title>
    <link href="Styles/dlg.css" rel="stylesheet" type="text/css" />
    <base target="_self" />
</head>
<body class="pcdlg">
    <form id="form1" runat="server" class="au-full">
    <au:SceneControl ID="SceneControl1" runat="server" />
    <div class="pcdlg-sky">
        <h1 class="pc-caption">
            <img src="Images/icon_01.gif" alt="图标" />
            <span id="schemaLabel" runat="server"></span>\<span id="schemUnitLabel" runat="server"></span>-<span>管理范围</span>
            <span class="pc-timepointmark">
                <mcs:TimePointDisplayControl ID="TimePointDisplayControl1" runat="server" />
            </span>
        </h1>
    </div>
    <div class="pcdlg-content" style="width: 100%; overflow: auto">
        <div class="pc-container5">
            <asp:Repeater runat="server" ID="scopeRepeater">
                <HeaderTemplate>
                    <ul id="scopelist" class="au-scope-menu clearfix">
                </HeaderTemplate>
                <ItemTemplate>
                    <li class='<%# this.GetMenuCssClass((string)Container.DataItem) %>'><a href='<%# "AUScopes.aspx?schemaType=" + Container.DataItem +"&unitId=" + this.AdminUnitObject.ID %>'
                        target="_self">
                        <%# this.GetSchemaName ((string)Container.DataItem)  %></a></li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>
            <asp:ListView runat="server">
                <ItemTemplate>
                </ItemTemplate>
            </asp:ListView>
        </div>
        <ul class="pc-tabs-header">
            <li>
                <asp:HyperLink ID="lnkToConst" NavigateUrl="AUScopesConst.aspx" runat="server" Text="固定成员" />
            </li>
            <li>
                <asp:HyperLink ID="lnkToCondition" NavigateUrl="AUScopesCondition.aspx" runat="server"
                    Text="条件成员" />
            </li>
            <li class="pc-active">
                <asp:HyperLink ID="lnkToPreview" NavigateUrl="AUScopes.aspx" runat="server" Text="预览成员" />
            </li>
        </ul>
        <div class="pc-search-box-wrapper">
            <soa:DeluxeSearch ID="DeluxeSearch" runat="server" CssClass="deluxe-search deluxe-left"
                HasCategory="False" SearchFieldTemplate="CONTAINS(${DataField}$, ${Data}$)" SearchField="S.SearchContent"
                OnSearching="SearchButtonClick" OnConditionClick="onconditionClick" CustomSearchContainerControlID="advSearchPanel"
                HasAdvanced="False">
            </soa:DeluxeSearch>
            <soa:DataBindingControl runat="server" ID="searchBinding" AllowClientCollectData="True">
                <ItemBindings>
                </ItemBindings>
            </soa:DataBindingControl>
            <div id="advSearchPanel" runat="server" style="display: none" class="pc-search-advpan">
                <asp:HiddenField runat="server" ID="sfAdvanced" Value="False" />
                <table class="pc-search-grid-duo">
                    <tr>
                        <td>
                        </td>
                        <td>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="pc-container5">
            <div class="pc-listmenu-container">
                <ul class="pc-listmenu" id="listMenu">
                    <li>
                        <asp:DropDownList runat="server" AutoPostBack="True" ID="drpScopeMode" OnSelectedIndexChanged="drpScopeMode_SelectedIndexChanged">
                            <asp:ListItem Text="显示本单元管理范围" Value="1" />
                            <asp:ListItem Text="显示所有子单元管理范围" Value="2" />
                            <asp:ListItem Text="显示本单元和所有子单元管理范围" Value="3" />
                        </asp:DropDownList>
                    </li>
                    <li>
                        <asp:LinkButton runat="server" ID="btnRecalc" CssClass="button pc-list-cmd" OnClick="RefreshList">重新生成本单元</asp:LinkButton>
                    </li>
                    <li>
                        <asp:LinkButton runat="server" ID="btnRecalcAll" CssClass="button pc-list-cmd" OnClick="RefreshList">重新生成所有单元</asp:LinkButton></li>
                </ul>
            </div>
            <div class="pc-grid-container">
                <mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
                    AllowPaging="True" AllowSorting="True" ShowCheckBoxes="True" Category="PermissionCenter"
                    ShowExportControl="true" GridTitle="管理Schema" DataKeyNames="ID" CssClass="dataList pc-datagrid"
                    TitleCssClass="title" PagerSettings-Position="Bottom" DataSourceMaxRow="0" TitleColor="141, 143, 149"
                    TitleFontSize="Large">
                    <EmptyDataTemplate>
                        暂时没有您需要的数据
                    </EmptyDataTemplate>
                    <HeaderStyle CssClass="head" />
                    <Columns>
                        <asp:TemplateField HeaderText="名称" SortExpression="S.AUScopeItemName">
                            <ItemTemplate>
                                <%# Server.HtmlEncode ((string)Eval("AUScopeItemName"))%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="管理单元" SortExpression="AUS.Name">
                            <ItemTemplate>
                                <%# Server.HtmlEncode ((string)Eval("AU_Name"))%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CreateDate" HtmlEncode="true" HeaderText="创建日期" SortExpression="S.CreateDate"
                            DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
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
        </div>
        <soa:DeluxeObjectDataSource runat="server" ID="dataSourceMain" TypeName="MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources.AUAdminScopeFullMemberDataSource"
            EnablePaging="True" OnSelecting="dataSourceMain_Selecting">
            <SelectParameters>
                <asp:QueryStringParameter Type="String" ConvertEmptyStringToNull="true" Name="unitID"
                    QueryStringField="unitID" />
                <asp:QueryStringParameter Type="String" ConvertEmptyStringToNull="true" Name="scopeType"
                    QueryStringField="schemaType" />
                <asp:ControlParameter Type="Int32" Name="deepOption" ControlID="drpScopeMode" DefaultValue="1" />
            </SelectParameters>
        </soa:DeluxeObjectDataSource>
        <soa:PostProgressControl runat="server" ID="calcProgress" OnClientBeforeStart="onPrepareData"
            OnClientCompleted="postProcess" ControlIDToShowDialog="btnRecalc" OnDoPostedData="ProcessCaculating"
            DialogHeaderText="正在计算..." DialogTitle="计算进度" />
        <soa:PostProgressControl runat="server" ID="calcProgressAll" OnClientBeforeStart="onPrepareData"
            OnClientCompleted="postProcess" ControlIDToShowDialog="btnRecalcAll" OnDoPostedData="ProcessGlobalCaculating"
            DialogHeaderText="正在计算..." DialogTitle="计算进度" />
    </div>
    <div class="pcdlg-floor">
        <div class="pcdlg-button-bar">
            <input type="button" value="关闭" class="pcdlg-button" onclick="window.close();" />
        </div>
    </div>
    </form>
    <script type="text/javascript">
        function onPrepareData(e) {
            e.steps = [1];
        }

        function postProcess(e) {
            __doPostBack('btnRecalc', '');
        }

        function onconditionClick(sender, e) {
            var content = Sys.Serialization.JavaScriptSerializer.deserialize(e.ConditionContent);
            var bindingControl = $find("searchBinding");
            bindingControl.dataBind(content);
        }
    
    </script>
</body>
</html>
