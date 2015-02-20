<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminUnitList.aspx.cs"
    Inherits="AUCenter.AdminUnitList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>管理单元列表</title>
    <base target="_self" />
</head>
<body class="au-full">
    <form id="form1" runat="server" class="au-full">
    <au:SceneControl ID="SceneControl1" runat="server" />
    <div>
        <div class="pc-banner">
            <h1>
                <img src="Images/icon_01.gif" alt="图标" />
                <span id="schemaNameLabel" runat="server"></span>-<span>管理单元列表</span> <span class="pc-timepointmark">
                    <mcs:TimePointDisplayControl ID="TimePointDisplayControl1" runat="server" />
                </span>
            </h1>
        </div>
        <div class="pc-nav-path">
            <asp:Repeater ID="path" runat="server" DataSourceID="navPathSource">
                <HeaderTemplate>
                    <span class="pc-nav-path-content">
                        <asp:HyperLink NavigateUrl='<%# "AdminUnitList.aspx?schemaId=" + this.SchemaID  %>'
                            ID="navSchema" runat="server" onclick="window.location.replace(this.href); return false;"
                            CssClass="pc-nav-path-node pc-nav-path-important"><%# Server.HtmlEncode(this.SchemaName) %></asp:HyperLink>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:HyperLink NavigateUrl='<%# "AdminUnitList.aspx?parentId=" + Eval("ID") + "&schemaId=" + this.SchemaID  %>'
                        ID="navNode" runat="server" CssClass="pc-nav-path-node"><%# Server.HtmlEncode((string)Eval("Name"))%></asp:HyperLink>
                </ItemTemplate>
                <FooterTemplate>
                    </span>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <div class="pc-search-box-wrapper">
            <soa:DeluxeSearch ID="DeluxeSearch" runat="server" CssClass="deluxe-search deluxe-left"
                HasCategory="False" SearchFieldTemplate="CONTAINS(${DataField}$, ${Data}$)" SearchField="S.SearchContent"
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
                        <asp:LinkButton runat="server" ID="lnkAdd" CssClass="button pc-list-cmd" OnClientClick="return ($pc.getEnabled(this)) && $pc.popups.newAdminUnit(this);"
                            OnClick="RefreshList">新建</asp:LinkButton>
                    </li>
                    <li>
                        <asp:LinkButton runat="server" ID="lnkDelete" OnClick="RefreshList" CssClass="button pc-list-cmd"
                            OnClientClick="return ($pc.getEnabled(this) && invokeBatchDelete() && false);">删除</asp:LinkButton>
                    </li>
                    <li>
                        <asp:LinkButton runat="server" ID="lnkMove" CssClass="button pc-list-cmd" OnClick="RefreshList"
                            OnClientClick="return ($pc.getEnabled(this) && invokeMove());">移动</asp:LinkButton>
                    </li>
                    <li>
                        <asp:LinkButton runat="server" ID="lnkCopy" CssClass="button pc-list-cmd" OnClick="RefreshList"
                            OnClientClick="return ($pc.getEnabled(this) && invokeCopy());">复制</asp:LinkButton>
                    </li>
                    <li class="pc-dropdownmenu"><span style="display: block; display: inline-block;">
                        <asp:LinkButton runat="server" CssClass="list-cmd" ID="btnImport" OnClientClick="return ($pc.getEnabled(this) && invokeImport() && false);"
                            OnClick="RefreshList">导入</asp:LinkButton></span> </li>
                    <li class="pc-dropdownmenu">
                        <asp:LinkButton runat="server" CssClass="list-cmd" ID="LinkButton1" OnClientClick="return ($pc.getEnabled(this) && invokeExport() && false);"
                            OnClick="RefreshList">导出<i class="pc-arrow"></i></asp:LinkButton>
                        <div style="position: relative; z-index: 9">
                            <div style="position: absolute;">
                                <ul class="pc-popup-nav">
                                    <li>
                                        <asp:LinkButton runat="server" CssClass="list-cmd" ID="btnExportSelected" OnClientClick="return ($pc.getEnabled(this) && invokeExport() && false);"
                                            OnClick="RefreshList">导出选定单元（默认）</asp:LinkButton>
                                    </li>
                                    <li>
                                        <asp:LinkButton runat="server" CssClass="list-cmd" ID="btnExportDeepSelected" OnClientClick="return ($pc.getEnabled(this) && invokeExportWithSubUnits() && false);"
                                            OnClick="RefreshList">导出选定单元及子单元</asp:LinkButton>
                                    </li>
                                    <li>
                                        <asp:LinkButton runat="server" CssClass="list-cmd" ID="btnExportAll" OnClientClick="return ($pc.getEnabled(this) && invokeExportAll() && false);"
                                            OnClick="RefreshList">导出当级所有单元</asp:LinkButton>
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </li>
                </ul>
            </div>
            <div style="display: none">
                <asp:Button Text="del" runat="server" ID="btnDeleteTrigger" />
                <asp:Button Text="del" runat="server" ID="btnDeleteItemTrigger" />
                <asp:HiddenField runat="server" ID="hfSchemaID" />
                <asp:HiddenField runat="server" ID="hfParentID" />
                <asp:Button Text="Move" runat="server" ID="moveActionButton" OnClick="DoMove" />
                <input type="hidden" runat="server" id="hfPostData" />
            </div>
            <div class="pc-grid-container">
                <mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
                    AllowPaging="True" AllowSorting="True" ShowCheckBoxes="True" Category="PermissionCenter"
                    ShowExportControl="true" GridTitle="管理单元" DataKeyNames="ID" CssClass="dataList pc-datagrid"
                    TitleCssClass="title" PagerSettings-Position="Bottom" DataSourceMaxRow="0" TitleColor="141, 143, 149"
                    TitleFontSize="Large" OnRowCommand="gridMain_RowCommand">
                    <EmptyDataTemplate>
                        暂时没有您需要的数据
                    </EmptyDataTemplate>
                    <HeaderStyle CssClass="head" />
                    <Columns>
                        <asp:TemplateField HeaderText="名称" SortExpression="S.Name">
                            <ItemTemplate>
                                <div>
                                    <a runat="server" href='<%# Eval("ID","AdminUnitList.aspx?parentID={0}") %>' class="pc-item-link"
                                        onclick="window.location.replace(this.href); return false;">
                                        <%# Server.HtmlEncode((string)Eval("Name")) %></a>
                                </div>
                                <div>
                                    <div id="Div1" class="pc-action-tray" runat="server" visible='<%# this.gridMain.ExportingDeluxeGrid == false %>'>
                                        <asp:LinkButton ID="lnkEdit1" runat="server" CssClass="pc-item-cmd" data-id='<%# System.Web.HttpUtility.HtmlAttributeEncode((string)Eval("ID"))%>'
                                            OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
                                        <asp:HyperLink ID="lnkRoleMembers" runat="server" CssClass="pc-item-cmd" Text="角色人员"
                                            NavigateUrl='<%#Eval("ID","~/AURoleMembers.aspx?id={0}") %>' onclick="return $pc.modalPopup(this);"
                                            Target="_blank"></asp:HyperLink>
                                        <a class="pc-item-cmd" data-id='<%#Eval("ID") %>' onclick="showScopes(this,event);"
                                            href="javascript:void(0);">管理范围</a>
                                        <asp:HyperLink ID="lnkAcl" runat="server" CssClass="pc-item-cmd" Text="授权控制" NavigateUrl='<%#Eval("ID","~/Dialogs/AclEdit.aspx?id={0}") %>'
                                            onclick="return $pc.modalPopup(this);" Target="_blank"></asp:HyperLink>
                                        <asp:LinkButton ID="lnkItemDelete" runat="server" Text="删除" CssClass="pc-item-cmd"
                                            data-id='<%#Eval("ID") %>' Enabled='<%# this.DeleteEnabled %>' CommandName="DeleteItem"
                                            CommandArgument='<%#Eval("ID") %>' OnClientClick="return ($pc.getEnabled(this) && $pc.confirmDelete('确定要删除?') && invokeItemDelete(this));"></asp:LinkButton>
                                        <asp:HyperLink ID="lnkHistory" runat="server" CssClass="pc-item-cmd" Target="_blank"
                                            onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/ObjectHistoryLog.aspx?id={0}") %>'>历史</asp:HyperLink>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="代码名称" SortExpression="S.CodeName" DataField="CodeName"
                            HtmlEncode="true" />
                        <asp:BoundField HeaderText="显示名称" SortExpression="S.DisplayName" DataField="DisplayName"
                            HtmlEncode="true" />
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
        <div id="scopesPanel" style="display: none;">
            <div class="pc-overlay-mask" style="z-index: 8">
            </div>
            <div class="pc-overlay-panel" style="z-index: 9">
                <div>
                    <asp:Repeater runat="server" ID="scopesRepeater">
                        <HeaderTemplate>
                            <ul id="scopesList" class="au-pop-menu">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li class="au-item"><a href="javascript:void(0);" data-schema-type='<%# Container.DataItem %>'
                                onclick="showSopeItem(this);">
                                <%# Server.HtmlEncode(this.GetSchemaName((string)Container.DataItem )) %></a></li>
                        </ItemTemplate>
                        <FooterTemplate>
                            <li class="au-item"><a href="javascript:void(0);" onclick="hideScopes()">取消</a></li>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
        <soa:DeluxeObjectDataSource runat="server" ID="dataSourceMain" TypeName="MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources.AdminUnitDataSource"
            EnablePaging="True" OnSelecting="dataSourceMain_Selecting">
            <SelectParameters>
                <asp:QueryStringParameter Type="String" ConvertEmptyStringToNull="true" Name="schemaID"
                    QueryStringField="schemaId" />
                <asp:QueryStringParameter Type="String" ConvertEmptyStringToNull="true" Name="parentID"
                    QueryStringField="parentId" />
            </SelectParameters>
        </soa:DeluxeObjectDataSource>
        <asp:ObjectDataSource ID="navPathSource" runat="server" SelectMethod="QueryParents"
            TypeName="MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources.AURecursiveDataSource"
            OnSelecting="navPathSource_Selecting">
            <SelectParameters>
                <asp:Parameter Name="unitID" Type="String" />
                <asp:Parameter DefaultValue="true" Name="includingSelf" Type="Boolean" />
                <asp:Parameter DefaultValue="" Name="timePoint" Type="DateTime" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <div style="display: none">
            <asp:Button Text="Refresh" runat="server" ID="btnRefresh" OnClick="RefreshList" />
            <soa:PostProgressControl runat="server" ID="deleteProgress" DialogTitle="删除管理单元"
                DialogHeaderText="删除进度..." OnClientCompleted="onDeleteCompleted" ControlIDToShowDialog="btnDeleteItemTrigger"
                OnClientBeforeStart="prepareItemToDelete" OnDoPostedData="DoDeleteProgress" />
            <soa:PostProgressControl runat="server" ID="deleteMultiProgress" DialogTitle="批量删除"
                DialogHeaderText="删除进度..." OnClientCompleted="onDeleteCompleted" ControlIDToShowDialog="btnDeleteTrigger"
                OnClientBeforeStart="prepareDataForDelete" OnDoPostedData="DoDeleteProgress" />
            <soa:UploadProgressControl runat="server" Category="管理单元" DialogHeaderText="导入管理单元"
                ID="ctlUpload" DialogTitle="导入管理单元" OnDoUploadProgress="DoFileUpload" OnLoadingDialogContent="ctlUpload_LoadingDialogContent"
                OnClientCompleted="postImportProcess" />
        </div>
        <script type="text/javascript">

            $pc.ui.gridBehavior("gridMain", "hover");
            $pc.ui.listMenuBehavior("listMenu");
            var currentUnitId;

            function onconditionClick(sender, e) {
                var content = Sys.Serialization.JavaScriptSerializer.deserialize(e.ConditionContent);
                var bindingControl = $find("searchBinding");
                bindingControl.dataBind(content);
            }

            function invokeImport() {
                var result = $find("ctlUpload").showDialog()
                if (result)
                    return true;
                return false;
            }

            function postImportProcess(e) {
                if (e.dataChanged)
                    __doPostBack('btnImport', '');
            }

            function invokeItemDelete(a) {
                var id = $pc.getAttr(a, "data-id");
                if (id) {
                    currentUnitId = id;
                    $find("deleteProgress").showDialog();
                    return true;
                }

                return false;
            }

            function invokeBatchDelete() {
                if ($find("gridMain").get_clientSelectedKeys().length) {
                    $find("deleteMultiProgress").showDialog();
                }
                return false;
            }

            function prepareItemToDelete(e) {
                e.steps = [currentUnitId];
            }

            function prepareDataForDelete(e) {
                e.steps = ($find("gridMain") || $find("grid2")).get_clientSelectedKeys();
                if (e.steps.length > 0) {
                    e.cancel = !confirm(e.steps.length === 1 ? "确实要删除这个项目？" : "确实要删除选定的项目？");
                }
            }

            function invokeMove() {
                var keys = $find("gridMain").get_clientSelectedKeys();
                if (keys.length) {
                    var targetId = $pc.popups.browseUnits($get("hfSchemaID").value, $get("hfParentID").value, keys);
                    if (targetId) {
                        $get("hfPostData").value = targetId;
                        $get("moveActionButton").click();
                    }
                }
                return false;
            }

            function invokeCopy() {
                var keys = $find("gridMain").get_clientSelectedKeys();
                if (keys.length === 1) {
                    if ($pc.showDialog('CopyUnit.aspx?fromUnit=' + keys[0], '', null, false, 800, 600, true) == true)
                        return true;
                } else {
                    alert("必须仅选择一个单元进行复制");
                }

                return false;
            }

            function showScopes(a, e) {
                currentUnitId = $pc.getAttr(a, "data-id");
                $pc.show('scopesPanel');
                var repe = document.getElementById('scopesList');
                repe.style.position = "absolute";
                var height = repe.offsetHeight;
                repe.style.left = e.clientX + "px";
                if (e.clientY + height > document.body.clientHeight) {
                    repe.style.top = (document.body.clientHeight - height) + "px";
                } else {
                    repe.style.top = e.clientY + "px";
                }
            }

            function hideScopes() {
                currentUnitId = null;
                $pc.hide('scopesPanel');
            }

            function showSopeItem(a) {
                try {
                    var t = $pc.getAttr(a, "data-schema-type");
                    if (currentUnitId) {
                        if (t) {
                            $pc.showDialog("AUScopes.aspx?schemaType=" + encodeURIComponent(t) + "&unitId=" + encodeURIComponent(currentUnitId), "", null, false, 800, 600, true);
                        }
                    }
                } catch (ex) {
                    alert(ex);
                }

                hideScopes();
            }

            function onDeleteCompleted() {
                $get("btnRefresh").click();
            }

            function invokeExport() {
                var grid = $find("gridMain");
                if (grid) {
                    var keys = grid.get_clientSelectedKeys();
                    if (keys.length > 0) {
                        $pc.postViaIFrame($pc.appRoot + "Services/Export.ashx", { context: "AdminUnitList", ids: keys, auSchemaId: $pc.get("hfSchemaID").value });
                    }
                }
                grid = false;
                return false;
            }

            function invokeExportWithSubUnits() {
                var grid = $find("gridMain");
                if (grid) {
                    var keys = grid.get_clientSelectedKeys();
                    if (keys.length > 0) {
                        $pc.postViaIFrame($pc.appRoot + "Services/Export.ashx", { context: "AdminUnitList", ids: keys, auSchemaId: $pc.get("hfSchemaID").value, deep: true });
                    }
                }
                grid = false;
                return false;
            }

            function invokeExportAll() {
                $pc.postViaIFrame($pc.appRoot + "Services/Export.ashx", { context: "AdminUnitList", auSchemaId: $pc.get("hfSchemaID").value });
                return false;
            }
        
        </script>
    </div>
    </form>
</body>
</html>
