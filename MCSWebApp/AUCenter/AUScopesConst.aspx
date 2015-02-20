<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AUScopesConst.aspx.cs"
    Inherits="AUCenter.AUScopesConst" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <div class="pcdlg-content">
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
            <asp:ListView ID="ListView1" runat="server">
                <ItemTemplate>
                </ItemTemplate>
            </asp:ListView>
        </div>
        <div class="pc-container5">
            <ul class="pc-tabs-header">
                <li class="pc-active">
                    <asp:HyperLink ID="lnkToConst" NavigateUrl="AUScopesConst.aspx" runat="server" Text="固定成员" />
                </li>
                <li>
                    <asp:HyperLink ID="lnkToCondition" NavigateUrl="AUScopesCondition.aspx" runat="server"
                        Text="条件成员" />
                </li>
                <li>
                    <asp:HyperLink ID="lnkToPreview" NavigateUrl="AUScopes.aspx" runat="server" Text="预览成员" />
                </li>
            </ul>
        </div>
        <div class="pc-search-box-wrapper">
            <soa:DeluxeSearch ID="DeluxeSearch" runat="server" CssClass="deluxe-search deluxe-left"
                HasCategory="False" SearchFieldTemplate="CONTAINS(${DataField}$, ${Data}$)" SearchField="S.SearchContent"
                OnSearching="SearchButtonClick" OnConditionClick="onconditionClick" CustomSearchContainerControlID="advSearchPanel"
                HasAdvanced="false">
            </soa:DeluxeSearch>
            <soa:DataBindingControl runat="server" ID="searchBinding" AllowClientCollectData="True">
                <ItemBindings>
                    <soa:DataBindingItem ControlID="sfCodeName" DataPropertyName="CodeName" />
                </ItemBindings>
            </soa:DataBindingControl>
            <div id="advSearchPanel" runat="server" style="display: none" class="pc-search-advpan">
                <asp:HiddenField runat="server" ID="sfAdvanced" Value="False" />
                <table class="pc-search-grid-duo">
                    <tr>
                        <td>
                            <label for="sfCodeName" class="pc-label">
                                代码名称</label><asp:TextBox runat="server" ID="sfCodeName" MaxLength="56" CssClass="pc-textbox" />(精确)
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
                        <asp:LinkButton runat="server" ID="lnkAdd" CssClass="button pc-list-cmd" OnClientClick="return ($pc.getEnabled(this)) && browseScopes() && false;"
                            OnClick="RefreshList">添加</asp:LinkButton>
                    </li>
                    <li>
                        <asp:LinkButton runat="server" ID="lnkDelete" CssClass="button pc-list-cmd" OnClientClick="return ($pc.getEnabled(this) && prepareDelete());"
                            OnClick="DoDelete">删除</asp:LinkButton>
                    </li>
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
                        <asp:BoundField HeaderText="描述" DataField="AUScopeItemName" HtmlEncode="true" SortExpression="S.AUScopeItemName" />
                        <asp:TemplateField HeaderText="创建者" SortExpression="S.CreatorName">
                            <ItemTemplate>
                                <soa:UserPresence runat="server" ID="userPresence" ShowUserDisplayName="true" ShowUserIcon="false"
                                    UserID='<%#Eval("CreatorID") %>' UserDisplayName='<%# Server.HtmlEncode(Eval("CreatorName").ToString()) %>' />
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
        <soa:DeluxeObjectDataSource runat="server" ID="dataSourceMain" TypeName="MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources.AUAdminScopeConstMemberDataSource"
            EnablePaging="True" OnSelecting="dataSourceMain_Selecting">
            <SelectParameters>
                <asp:QueryStringParameter Type="String" ConvertEmptyStringToNull="true" Name="unitID"
                    QueryStringField="unitID" />
                <asp:QueryStringParameter Type="String" ConvertEmptyStringToNull="true" Name="scopeType"
                    QueryStringField="schemaType" />
            </SelectParameters>
        </soa:DeluxeObjectDataSource>
    </div>
    <div class="pcdlg-floor">
        <div class="pcdlg-pcdlg-button-bar">
            <input type="button" runat="server" value="关闭" class="pcdlg-button" onclick="window.close();" />
        </div>
    </div>
    <div style="display: none">
        <asp:HiddenField runat="server" ID="postData" />
        <input type="hidden" runat="server" id="scopeType" />
        <asp:Button Text="add" runat="server" ID="btnAddScope" OnClick="AddScopesClick" />
    </div>
    </form>
    <script type="text/javascript">
        function onconditionClick(sender, e) {
            var content = Sys.Serialization.JavaScriptSerializer.deserialize(e.ConditionContent);
            var bindingControl = $find("searchBinding");
            bindingControl.dataBind(content);
        }

        function browseScopes() {
            if ($pc.popups.browseScopes($pc.get("postData"), $pc.get("scopeType").value, false, [], null)) {
                $pc.get("btnAddScope").click();
            }

            return false;
        }

        function prepareDelete() {
            return $find("gridMain").get_clientSelectedKeys().length > 0;
        }

    </script>
</body>
</html>
