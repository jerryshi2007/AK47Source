<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppRoleMembersView.aspx.cs"
    Inherits="PermissionCenter.AppRoleMembersView" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" scroll="no" style="overflow: hidden">
<head id="Head1" runat="server">
    <title>角色成员</title>
    <link rel="icon" href="../favicon.ico" type="image/x-icon" />
    <link rel="Shortcut Icon" href="../favicon.ico" />
    <base target="_self" />
    <pc:HeaderControl ID="HeaderControl1" runat="server">
    </pc:HeaderControl>
</head>
<body>
    <form id="form1" runat="server">
    <pc:SceneControl runat="server" ID="sc" />
    <soa:DataBindingControl runat="server" ID="binding1">
        <ItemBindings>
            <soa:DataBindingItem DataPropertyName="VisibleName" ControlID="roleName" ControlPropertyName="Text"
                Direction="DataToControl" />
            <soa:DataBindingItem DataPropertyName="RoleID" ControlID="lnkDynamic" ControlPropertyName="NavigateUrl"
                Format="~/lists/AppRoleConditionMembers.aspx?role={0}" Direction="DataToControl" />
            <soa:DataBindingItem DataPropertyName="RoleID" ControlID="lnkConst" ControlPropertyName="NavigateUrl"
                Format="~/lists/AppRoleMembers.aspx?role={0}" Direction="DataToControl" />
            <soa:DataBindingItem DataPropertyName="RoleID" ControlID="lnkPreview" ControlPropertyName="NavigateUrl"
                Format="~/lists/AppRoleMembersView.aspx?role={0}" Direction="DataToControl" />
        </ItemBindings>
    </soa:DataBindingControl>
    <div class="pc-banner">
        <h1 class="pc-caption">
            角色成员-<asp:Literal ID="roleName" runat="server" Mode="Encode"></asp:Literal>
            <span style="float: right">
                <soa:RoleMatrixEntryControl ID="roleMatrixEntryControl" runat="server" EnableAccessTicket="true" />
            </span><span class="pc-timepointmark">
                <mcs:TimePointDisplayControl ID="TimePointDisplayControl1" runat="server">
                </mcs:TimePointDisplayControl>
            </span>
        </h1>
    </div>
    <pc:BannerNotice ID="notice" runat="server"></pc:BannerNotice>
    <ul class="pc-tabs-header">
        <li>
            <asp:HyperLink ID="lnkConst" runat="server" Text="固定成员" />
        </li>
        <li>
            <asp:HyperLink ID="lnkDynamic" runat="server" Text="条件成员" />
        </li>
        <li class="pc-active">
            <asp:HyperLink ID="lnkPreview" runat="server" Text="预览成员" />
        </li>
    </ul>
    <div class="pc-frame-container">
        <div class="pc-search-box-wrapper">
            <soa:DeluxeSearch ID="DeluxeSearch" runat="server" CssClass="deluxe-search deluxe-left"
                HasCategory="False" SearchFieldTemplate="CONTAINS(${DataField}$, ${Data}$)" SearchField="O.SearchContent"
                OnSearching="SearchButtonClick" OnConditionClick="onconditionClick" CustomSearchContainerControlID="advSearchPanel"
                HasAdvanced="true">
            </soa:DeluxeSearch>
            <soa:DataBindingControl runat="server" ID="searchBinding" AllowClientCollectData="True">
                <ItemBindings>
                    <soa:DataBindingItem ControlID="sfCodeName" DataPropertyName="CodeName" />
                </ItemBindings>
            </soa:DataBindingControl>
            <div id="advSearchPanel" runat="server" style="display: none" class="pc-search-advpan">
                <asp:HiddenField runat="server" ID="sfAdvanced" Value="False" />
                <table border="0" cellpadding="0" cellspacing="0" class="pc-search-grid-duo">
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
                        <asp:LinkButton runat="server" ID="lnkProperties" CssClass="button pc-list-cmd pc-hide"
                            OnClientClick="return ($pc.getEnabled(this) && !!showBatchEditor());" OnClick="RefreshList">属性</asp:LinkButton>
                    </li>
                    <li>
                        <asp:LinkButton runat="server" CssClass="button pc-list-cmd" OnClick="RefreshList"
                            ID="btnRecalc">重新计算</asp:LinkButton>
                        <soa:PostProgressControl runat="server" ID="calcProgress" OnClientBeforeStart="onPrepareData"
                            OnClientCompleted="postProcess" ControlIDToShowDialog="btnRecalc" OnDoPostedData="ProcessCaculating"
                            DialogHeaderText="正在计算..." DialogTitle="正在计算..." />
                    </li>
                    <li>
                        <asp:LinkButton runat="server" CssClass="button pc-list-cmd" OnClick="RefreshList"
                            ID="btnRecalcAll">全局重新计算</asp:LinkButton>
                        <soa:PostProgressControl runat="server" ID="calcProgressAll" OnClientBeforeStart="onPrepareData"
                            OnClientCompleted="postProcess" ControlIDToShowDialog="btnRecalcAll" OnDoPostedData="ProcessGlobalCaculating"
                            DialogHeaderText="正在计算..." DialogTitle="正在计算..." />
                    </li>
                    <li class="pc-dropdownmenu" style="float: right"><span style="display: block; display: inline-block">
                        <asp:HyperLink ID="lnkViewMode" runat="server" CssClass="pc-toggler-dd list-cmd shortcut"
                            NavigateUrl="javascript:void(0);" Style="margin-right: 0; cursor: default">
                            <i class="pc-toggler-icon"></i><span runat="server" id="lblViewMode">常规列表</span><i
                                class="pc-arrow"></i>
                        </asp:HyperLink>
                    </span>
                        <div class="pc-popup-nav-pan pc-right" style="z-index: 9">
                            <div class="pc-popup-nav-wrapper">
                                <ul class="pc-popup-nav" style="text-align: left; color: #ffffff;">
                                    <li>
                                        <asp:LinkButton CssClass="pc-toggler-dd shortcut" ID="lnkToggleList" Style="padding-left: 0"
                                            runat="server" OnCommand="ToggleViewMode" CommandName="ToggleViewMode" CommandArgument="0"><i class="pc-toggler-icon"></i>常规列表</asp:LinkButton></li>
                                    <li>
                                        <asp:LinkButton CssClass="pc-toggler-dt shortcut" ID="lnkToggleTable" Style="padding-left: 0"
                                            runat="server" OnCommand="ToggleViewMode" CommandName="ToggleViewMode" CommandArgument="1"><i class="pc-toggler-icon">
									</i>精简表格</asp:LinkButton></li>
                                </ul>
                            </div>
                        </div>
                    </li>
                </ul>
            </div>
            <div>
                <asp:HiddenField ID="actionData" runat="server" EnableViewState="False" />
            </div>
            <div class="pc-grid-container">
                <asp:MultiView ActiveViewIndex="0" runat="server" ID="views">
                    <asp:View runat="server">
                        <mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
                            AllowPaging="True" AllowSorting="True" ShowCheckBoxes="True" Category="PermissionCenter"
                            GridTitle="角色成员预览" DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title"
                            ShowExportControl="true" PagerSettings-Position="Bottom" DataSourceMaxRow="0"
                            TitleColor="141, 143, 149" TitleFontSize="Large" OnSelectAllCheckBoxClick="toggleMenu"
                            OnSelectCheckBoxClick="toggleMenu">
                            <EmptyDataTemplate>
                                暂时没有您需要的数据
                            </EmptyDataTemplate>
                            <HeaderStyle CssClass="head" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <mcs:GridColumnSorter runat="server" ID="colSorter" DefaultOrderName="名称" DockPosition="Left"
                                            PreventRenderChildren='<%# this.gridMain.ExportingDeluxeGrid  %>'>
						<SortItems>
						<mcs:SortItem Text="名称" SortExpression="Name" />
						<mcs:SortItem Text="显示名称" SortExpression="DisplayName" />
						<mcs:SortItem Text="代码名称" SortExpression="CodeName" />
						</SortItems>
                                        </mcs:GridColumnSorter>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <div class="pc-photo-container" style="float: left;">
                                            <soa:UserPresence runat="server" ID="uc1" UserID='<%#Eval("ID") %>' StatusImage="LongBar"
                                                UserIconUrl='<%#Eval("ID","../Handlers/UserPhoto.ashx?id={0}") %>' ShowUserIcon="true"
                                                AccountDisabled='<%#0 !=(int)Eval("AccountDisabled") %>' />
                                        </div>
                                        <div>
                                            <%# Server.HtmlEncode((string)Eval("Name")) %>
                                        </div>
                                        <div>
                                            <%# Server.HtmlEncode((string)Eval("DisplayName")) %>
                                        </div>
                                        <div>
                                            <div id="divActions1" class="pc-action-tray" runat="server" visible='<%# this.gridMain.ExportingDeluxeGrid == false %>'>
                                                <asp:LinkButton ID="lnkEdit1" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
                                                    OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
                                                <asp:HyperLink ID="lnkOu1" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
                                                    Target="_blank" NavigateUrl='<%# "~/lists/MemberOrgView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
                                                    onclick="return $pc.modalPopup(this);">组织</asp:HyperLink>
                                                <asp:HyperLink ID="lnkGrp1" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
                                                    Target="_blank" NavigateUrl='<%# "~/lists/MemberGrpView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
                                                    onclick="return $pc.modalPopup(this);">群组</asp:HyperLink>
                                                <asp:HyperLink ID="lnkSecret1" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
                                                    Target="_blank" NavigateUrl='<%# "~/lists/UserSecretary.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
                                                    onclick="return $pc.modalPopup(this);">秘书</asp:HyperLink>
                                                <asp:HyperLink ID="lnkMemberRole1" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
                                                    Target="_blank" NavigateUrl='<%# "~/lists/MemberRoleView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
                                                    onclick="return $pc.modalPopup(this);">角色</asp:HyperLink>
                                                <asp:HyperLink ID="lnkHistory1" runat="server" CssClass="pc-item-cmd" Target="_blank"
                                                    onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/ObjectHistoryLog.aspx?id={0}") %>'>历史</asp:HyperLink>
                                            </div>
                                        </div>
                                        <div>
                                            <%# Server.HtmlEncode((string)Eval("CodeName")) %>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="创建者" SortExpression="CreatorName">
                                    <ItemTemplate>
                                        <soa:UserPresence runat="server" ID="userPresence" ShowUserDisplayName="true" ShowUserIcon="false"
                                            UserID='<%#Eval("CreatorID") %>' UserDisplayName='<%# Server.HtmlEncode(Eval("CreatorName").ToString()) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <pc:SubRowBoundField DataField="CreateDate" HtmlEncode="true" HeaderText="创建日期" SortExpression="CreateDate"
                                    SubColSpan="4" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}">
                                    <SubRowTemplate>
                                        <span class="pc-label">缺省组织</span>
                                        <soa:OuNavigator ID="OuNHa1" runat="server" StartLevel="0" TerminalVisible="true"
                                            ObjectID='<%#Eval("ParentID") %>' NavigateUrlFormat="~/lists/OUExplorer.aspx?ou={0}"
                                            Target="_parent" />
                                    </SubRowTemplate>
                                </pc:SubRowBoundField>
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
                    </asp:View>
                    <asp:View runat="server">
                        <mcs:DeluxeGrid ID="grid2" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
                            AllowPaging="True" AllowSorting="True" ShowCheckBoxes="True" Category="PermissionCenter"
                            GridTitle="角色成员预览" DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title"
                            ShowExportControl="true" PagerSettings-Position="Bottom" DataSourceMaxRow="0"
                            TitleColor="141, 143, 149" TitleFontSize="Large" OnSelectAllCheckBoxClick="toggleMenu"
                            OnSelectCheckBoxClick="toggleMenu">
                            <EmptyDataTemplate>
                                暂时没有您需要的数据
                            </EmptyDataTemplate>
                            <HeaderStyle CssClass="head" />
                            <Columns>
                                <asp:TemplateField HeaderText="名称" SortExpression="Name">
                                    <ItemTemplate>
                                        <div>
                                            <i class='<%# PermissionCenter.Util.CssSpritesFor(GetDataItem(),PermissionCenter.IconSizeType.Size16) %>'>
                                            </i>
                                            <soa:UserPresence ID="uc1" runat="server" UserID='<%#Eval("ID") %>' StatusImage="Ball"
                                                ShowUserDisplayName="true" UserDisplayName='<%# Eval("Name") %>' AccountDisabled='<%#0 !=(int)Eval("AccountDisabled") %>' />
                                        </div>
                                        <div>
                                            <div id="divActions1" class="pc-action-tray" runat="server" visible='<%# this.gridMain.ExportingDeluxeGrid == false %>'>
                                                <asp:LinkButton ID="lnkEdit1" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
                                                    OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
                                                <asp:HyperLink ID="lnkOu1" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
                                                    Target="_blank" NavigateUrl='<%# "~/lists/MemberOrgView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
                                                    onclick="return $pc.modalPopup(this);">组织</asp:HyperLink>
                                                <asp:HyperLink ID="lnkGrp1" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
                                                    Target="_blank" NavigateUrl='<%# "~/lists/MemberGrpView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
                                                    onclick="return $pc.modalPopup(this);">群组</asp:HyperLink>
                                                <asp:HyperLink ID="lnkSecret1" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
                                                    Target="_blank" NavigateUrl='<%# "~/lists/UserSecretary.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
                                                    onclick="return $pc.modalPopup(this);">秘书</asp:HyperLink>
                                                <asp:HyperLink ID="lnkMemberRole1" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
                                                    Target="_blank" NavigateUrl='<%# "~/lists/MemberRoleView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
                                                    onclick="return $pc.modalPopup(this);">角色</asp:HyperLink>
                                                <asp:HyperLink ID="lnkRP" runat="server" NavigateUrl='<%#Eval("ID","~/lists/FlowList.aspx?id={0}") %>'
                                                    onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">查看相关流程</asp:HyperLink>
                                                <asp:HyperLink ID="lnkHistory1" runat="server" CssClass="pc-item-cmd" Target="_blank"
                                                    onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/ObjectHistoryLog.aspx?id={0}") %>'>历史</asp:HyperLink>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="DisplayName" HtmlEncode="true" HeaderText="显示名称" SortExpression="DisplayName" />
                                <asp:BoundField DataField="CodeName" HtmlEncode="true" HeaderText="代码名称" SortExpression="CodeName" />
                                <asp:TemplateField HeaderText="创建者" SortExpression="CreatorName">
                                    <ItemTemplate>
                                        <soa:UserPresence runat="server" ID="userPresence" ShowUserDisplayName="true" ShowUserIcon="false"
                                            UserID='<%#Eval("CreatorID") %>' UserDisplayName='<%# Server.HtmlEncode(Eval("CreatorName").ToString()) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <pc:SubRowBoundField DataField="CreateDate" HtmlEncode="true" HeaderText="创建日期" SortExpression="CreateDate"
                                    SubColSpan="6" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}">
                                    <SubRowTemplate>
                                        <span class="pc-label">缺省组织</span>
                                        <soa:OuNavigator ID="OuNHa1" runat="server" StartLevel="0" TerminalVisible="true"
                                            ObjectID='<%#Eval("ParentID") %>' NavigateUrlFormat="~/lists/OUExplorer.aspx?ou={0}"
                                            Target="_parent" />
                                    </SubRowTemplate>
                                </pc:SubRowBoundField>
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
                    </asp:View>
                </asp:MultiView>
            </div>
        </div>
    </div>
    <soa:DeluxeObjectDataSource ID="dataSourceMain" runat="server" EnablePaging="True"
        TypeName="MCS.Library.SOA.DataObjects.Security.DataSources.ConditionalUsersDataSource"
        OnSelecting="dataSourceMain_Selecting">
        <SelectParameters>
            <asp:QueryStringParameter QueryStringField="role" Type="String" Name="containerId"
                DefaultValue="773BA455-6A2E-4A71-BDC7-AFE689789390" />
        </SelectParameters>
    </soa:DeluxeObjectDataSource>
    </form>
    <script type="text/javascript">
        $pc.ui.gridBehavior("gridMain", "hover");
        $pc.ui.gridBehavior("grid2", "hover");
        $pc.ui.listMenuBehavior("listMenu");
        $pc.ui.traceWindowWidth();

        function onconditionClick(sender, e) {
            var content = Sys.Serialization.JavaScriptSerializer.deserialize(e.ConditionContent);
            var bindingControl = $find("searchBinding");
            bindingControl.dataBind(content);
        }

        function onPrepareData(e) {

            e.steps = [1];
        }

        function postProcess(e) {
            __doPostBack('btnRecalc', '');
        }

        function toggleMenu() {
            var grid = $find("gridMain") || $find("grid2");
            if (grid) {
                if (grid.get_clientSelectedKeys().length) {
                    $pc.removeClass("lnkProperties", "pc-hide");
                } else {
                    $pc.addClass("lnkProperties", "pc-hide");
                };
            } else {
                $pc.console.error("未加载grid");
            }

            grid = null;
        }

        function showBatchEditor() {
            return $pc.popups.batchProperties($find("gridMain") || $find("grid2"));
        }

        Sys.Application.add_load(function () {
            toggleMenu();
        });
    </script>
</body>
</html>
