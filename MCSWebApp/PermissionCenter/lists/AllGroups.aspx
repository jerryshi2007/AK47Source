<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AllGroups.aspx.cs" Inherits="PermissionCenter.AllGroups" %>

<%--怪异模式测试时将文档声明置于服务器注释块内--%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base target="_self" />
    <title>权限中心-所有群组</title>
    <link rel="icon" href="../favicon.ico" type="image/x-icon" />
    <link rel="Shortcut Icon" href="../favicon.ico" />
    <pc:HeaderControl runat="server">
    </pc:HeaderControl>
    <!--以下部分为智能感知的需要-->
    <%-- <link href="../styles/pccom.css" rel="stylesheet" type="text/css" />
    <link href="../styles/pccssreform.css" rel="stylesheet" type="text/css" />
    <link href="../styles/pcsprites.css" rel="stylesheet" type="text/css" />
    <script src="../scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../scripts/pc.js" type="text/javascript"></script>--%>
    <!--发布时应注释-->
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server" EnableScriptGlobalization="true" />
    <div class="pc-frame-header">
        <pc:Banner ID="pcBanner" runat="server" ActiveMenuIndex="3" OnTimePointChanged="RefreshList" />
    </div>
    <pc:SceneControl ID="SceneControl1" runat="server">
    </pc:SceneControl>
    <pc:BannerNotice ID="notice" runat="server"></pc:BannerNotice>
    <div class="pc-frame-container">
        <div class="pc-search-box-wrapper">
            <soa:DeluxeSearch ID="DeluxeSearch" runat="server" CssClass="deluxe-search deluxe-left"
                HasCategory="False" SearchFieldTemplate="CONTAINS(${DataField}$, ${Data}$)" SearchField="G.SearchContent"
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
                        <asp:LinkButton runat="server" CssClass="pc-list-cmd" ID="btnDeleteSelected" Text="删除"
                            OnClientClick="return ($pc.getEnabled(this) && checkDeletable() && $pc.popups.batchDelete(($find('gridMain')||$find('grid2')),'AllGroups'));"
                            OnClick="BatchDelete"></asp:LinkButton>
                    </li>
                    <li>
                        <asp:LinkButton runat="server" CssClass="pc-list-cmd" ID="btnMoveToGroup" Text="移动到组织"
                            OnClientClick="return ($pc.getEnabled(this) && invokeTransfer());"></asp:LinkButton>
                    </li>
                    <li>
                        <asp:LinkButton runat="server" CssClass="list-cmd" ID="btnImport" Text="导入" OnClientClick="return ($pc.getEnabled(this) && invokeImport() && false);"
                            OnClick="RefreshList"></asp:LinkButton>
                    </li>
                    <li>
                        <asp:LinkButton runat="server" CssClass="list-cmd" ID="btnExport" Text="导出" OnClientClick="return ($pc.getEnabled(this) && invokeExport() && false);"></asp:LinkButton>
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
            <div style="display: none">
                <input id="actionData" runat="server" />
                <asp:Button Text="" ID="transferTrigger" runat="server" />
                <asp:Button Text="" ID="refreshTrigger" runat="server" OnClick="RefreshList" />
                <input id="deleteLimitList" runat="server" type="hidden" />
                <asp:HiddenField runat="server" ID="acceptedLimitList" />
            </div>
            <div class="pc-grid-container">
                <asp:MultiView ActiveViewIndex="0" runat="server" ID="views">
                    <asp:View runat="server">
                        <mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
                            AllowPaging="True" AllowSorting="True" ShowCheckBoxes="True" Category="PermissionCenter"
                            GridTitle="群组" DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title"
                            PagerSettings-Position="Bottom" DataSourceMaxRow="0" TitleColor="141, 143, 149"
                            TitleFontSize="Large" OnRowCommand="HandleRowCommand" ShowExportControl="true"
                            OnSelectCheckBoxClick="handleCheckItem">
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
							<mcs:SortItem SortExpression="G.Name" Text="名称" />
							<mcs:SortItem SortExpression="G.DisplayName" Text="显示名称" />
							<mcs:SortItem SortExpression="G.CodeName" Text="代码名称" />
							</SortItems>
                                        </mcs:GridColumnSorter>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <div>
                                            <asp:HyperLink ID="iLnk" onclick="return $pc.modalPopup(this);" CssClass="pc-item-link"
                                                NavigateUrl='<%#Eval("ID","~/lists/GroupConstMembers.aspx?id={0}") %>' Target="_blank"
                                                runat="server"><span><i class="pc-icon16 Groups"></i>
										<%# Server.HtmlEncode((string)Eval("Name")) %></span></asp:HyperLink></div>
                                        <div>
                                            <div class="pc-action-tray" runat="server" id="d" visible='<%# this.gridMain.ExportingDeluxeGrid == false %>'>
                                                <asp:LinkButton ID="lnkEdit" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
                                                    OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
                                                <asp:HyperLink ID="lnkConstMembers" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
                                                    onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/GroupConstMembers.aspx?id={0}") %>'
                                                    Target="_blank">群组人员管理</asp:HyperLink>
                                                <asp:HyperLink ID="lnkRP" runat="server" NavigateUrl='<%#Eval("ID","~/lists/FlowList.aspx?id={0}") %>'
                                                    onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">查看相关流程</asp:HyperLink>
                                                <asp:LinkButton ID="lnkItemDelete" runat="server" Text="删除" CssClass="pc-item-cmd"
                                                    Enabled='<%# this.IsDeleteEnabled((string)Eval("ParentID")) %>' CommandName="DeleteItem"
                                                    CommandArgument='<%#Eval("ID") %>' OnClientClick="return ($pc.getEnabled(this) && $pc.confirmDelete('确定要删除?'));"></asp:LinkButton>
                                                <asp:HyperLink ID="lnkHistory" runat="server" CssClass="pc-item-cmd" Target="_blank"
                                                    onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/ObjectHistoryLog.aspx?id={0}") %>'>历史</asp:HyperLink>
                                            </div>
                                        </div>
                                        <div>
                                            <span>
                                                <%# Server.HtmlEncode((string)Eval("DisplayName")) %></span></div>
                                        <div>
                                            <span>
                                                <%# Server.HtmlEncode((string)Eval("CodeName")) %></span></div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="位置">
                                    <ItemTemplate>
                                        <div>
                                            <soa:OuNavigator ID="navPath" runat="server" StartLevel="0" LinkCssClass="pc-item-link"
                                                RootVisible="false" SplitterVisible="true" Target="_blank" NavigateUrlFormat="~/lists/OUExplorer.aspx?ou={0}"
                                                TerminalVisible="true" ObjectID='<%#Eval("ParentID") %>'></soa:OuNavigator>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="创建者" SortExpression="G.CreatorName">
                                    <ItemTemplate>
                                        <soa:UserPresence runat="server" ID="userPresence" ShowUserDisplayName="true" ShowUserIcon="false"
                                            UserID='<%#Eval("CreatorID") %>' UserDisplayName='<%# Server.HtmlEncode(Eval("CreatorName").ToString()) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="CreateDate" HtmlEncode="true" HeaderText="创建日期" SortExpression="G.CreateDate"
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
                    </asp:View>
                    <asp:View runat="server">
                        <mcs:DeluxeGrid ID="grid2" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
                            AllowPaging="True" AllowSorting="True" ShowCheckBoxes="True" Category="PermissionCenter"
                            GridTitle="群组" DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title"
                            PagerSettings-Position="Bottom" DataSourceMaxRow="0" TitleColor="141, 143, 149"
                            TitleFontSize="Large" OnRowCommand="HandleRowCommand" ShowExportControl="true"
                            OnSelectCheckBoxClick="handleCheckItem">
                            <EmptyDataTemplate>
                                暂时没有您需要的数据
                            </EmptyDataTemplate>
                            <HeaderStyle CssClass="head" />
                            <Columns>
                                <asp:TemplateField HeaderText="名称" SortExpression="G.Name">
                                    <ItemTemplate>
                                        <div>
                                            <asp:HyperLink ID="iLnk" onclick="return $pc.modalPopup(this);" CssClass="pc-item-link"
                                                NavigateUrl='<%#Eval("ID","~/lists/GroupConstMembers.aspx?id={0}") %>' Target="_blank"
                                                runat="server"><span><i class="pc-icon16 Groups"></i>
										<%# Server.HtmlEncode((string)Eval("Name")) %></span></asp:HyperLink></div>
                                        <div>
                                            <div class="pc-action-tray" runat="server" id="d" visible='<%# this.grid2.ExportingDeluxeGrid == false %>'>
                                                <asp:LinkButton ID="lnkEdit" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
                                                    OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
                                                <asp:HyperLink ID="lnkConstMembers" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
                                                    onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/GroupConstMembers.aspx?id={0}") %>'
                                                    Target="_blank">群组人员管理</asp:HyperLink>
                                                <asp:HyperLink ID="lnkRP" runat="server" NavigateUrl='<%#Eval("ID","~/lists/FlowList.aspx?id={0}") %>'
                                                    onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">查看相关流程</asp:HyperLink>
                                                <asp:LinkButton ID="lnkItemDelete" runat="server" Text="删除" CssClass="pc-item-cmd"
                                                    Enabled='<%# this.IsDeleteEnabled((string)Eval("ParentID")) %>' CommandName="DeleteItem"
                                                    CommandArgument='<%#Eval("ID") %>' OnClientClick="return ($pc.getEnabled(this) && $pc.confirmDelete('确定要删除?'));"></asp:LinkButton>
                                                <asp:HyperLink ID="lnkHistory" runat="server" CssClass="pc-item-cmd" Target="_blank"
                                                    onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/ObjectHistoryLog.aspx?id={0}") %>'>历史</asp:HyperLink>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="显示名称" DataField="DisplayName" SortExpression="G.DisplayName"
                                    HtmlEncode="true" />
                                <asp:BoundField HeaderText="代码名称" DataField="CodeName" SortExpression="G.CodeName"
                                    HtmlEncode="true" />
                                <asp:TemplateField HeaderText="位置">
                                    <ItemTemplate>
                                        <div>
                                            <soa:OuNavigator ID="navPath" runat="server" StartLevel="0" LinkCssClass="pc-item-link"
                                                RootVisible="false" SplitterVisible="true" Target="_blank" NavigateUrlFormat="~/lists/OUExplorer.aspx?ou={0}"
                                                TerminalVisible="true" ObjectID='<%#Eval("ParentID") %>'></soa:OuNavigator>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="创建者" SortExpression="G.CreatorName">
                                    <ItemTemplate>
                                        <soa:UserPresence runat="server" ID="userPresence" ShowUserDisplayName="true" ShowUserIcon="false"
                                            UserID='<%#Eval("CreatorID") %>' UserDisplayName='<%# Server.HtmlEncode(Eval("CreatorName").ToString()) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="CreateDate" HtmlEncode="true" HeaderText="创建日期" SortExpression="G.CreateDate"
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
                    </asp:View>
                </asp:MultiView>
            </div>
        </div>
    </div>
    <soa:DeluxeObjectDataSource ID="dataSourceMain" runat="server" EnablePaging="True"
        TypeName="MCS.Library.SOA.DataObjects.Security.DataSources.SchemaGroupDataSource"
        EnableViewState="false" OnSelecting="dataSourceMain_Selecting" OnSelected="dataSourceMain_Selected">
    </soa:DeluxeObjectDataSource>
    <pc:Footer ID="footer" runat="server" />
    <soa:UploadProgressControl runat="server" ID="ctlUpload" DialogHeaderText="导入群组数据(xml)"
        DialogTitle="导入" OnDoUploadProgress="DoFileUpload" OnLoadingDialogContent="ctlUpload_LoadingDialogContent"
        OnClientCompleted="postImportProcess" />
    <soa:PostProgressControl runat="server" DialogHeaderText="正在移动" DialogTitle="移动群组"
        OnClientBeforeStart="prepareTransfer" OnClientCompleted="afterTransfer" ControlIDToShowDialog="transferTrigger"
        OnDoPostedData="ProcessMoving" />
    </form>
    <script type="text/javascript">
        $pc.ui.listMenuBehavior("listMenu");
        $pc.ui.gridBehavior("gridMain", "hover");
        $pc.ui.gridBehavior("grid2", "hover");

        function onconditionClick(sender, e) {
            var content = Sys.Serialization.JavaScriptSerializer.deserialize(e.ConditionContent);
            var bindingControl = $find("searchBinding");
            bindingControl.dataBind(content);
        }

        function handleCheckItem(o, s) {
            var key = s.checkbox.value;
            var checked = s.checkbox.checked;
            var i;
            var accepted = document.getElementById("acceptedLimitList").value.split(",");

            var knownLimitString = document.getElementById("deleteLimitList").value;
            var knownLimit = knownLimitString.length ? Sys.Serialization.JavaScriptSerializer.deserialize(knownLimitString) : [];

            var valid = false;

            for (i = knownLimit.length - 1; i >= 0; i--) {
                if (knownLimit[i] == key) {
                    valid = true;
                    break;
                }

            }
            if (valid) {
                if (checked) {
                    var exists = false;
                    for (i = accepted.length - 1; i >= 0; i--) {
                        if (accepted[i] == key) {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists) {
                        accepted.push(key);
                    }
                } else {
                    for (i = accepted.length - 1; i >= 0; i--) {
                        if (accepted[i] == key) {
                            accepted.splice(i, 1);
                            break;
                        }
                    }
                }

                document.getElementById("acceptedLimitList").value = accepted.join(",");
            }

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

        function invokeExport() {
            var grid = $find("gridMain") || $find("grid2");
            if (grid) {
                var keys = grid.get_clientSelectedKeys();
                if (keys.length > 0) {
                    $pc.postViaIFrame($pc.appRoot + "Handlers/ObjectExport.ashx", { context: "AllGroups", id: keys });
                }
            }
            grid = false;
            return false;
        }

        function invokeTransfer() {
            if (($find("gridMain") || $find("grid2")).get_clientSelectedKeys().length) {
                if (checkAllDeleteable()) {
                    document.getElementById("transferTrigger").click();
                } else {
                    alert("请确保所有选定的群组都是可以删除的");
                }
            }

            return false;
        }

        function checkDeletable() {
            var result = false;
            if (($find("gridMain") || $find("grid2")).get_clientSelectedKeys().length) {
                result = checkAllDeleteable();
            } else {
                return false;
            }

            if (!result) {
                alert("请只选择管理范围中的群组");
            }

            result = !!result;
            return result;

        }

        function checkAllDeleteable() {
            var passed = true;

            var selected = ($find("gridMain") || $find("grid2")).get_clientSelectedKeys();
            var valid = document.getElementById("acceptedLimitList").value.split(",");
            //TODO: 继续写
            for (var i = selected.length - 1; i >= 0; i--) {
                var a = selected[i];
                var exists = false;
                for (var j = valid.length - 1; j >= 0; j--) {
                    if (valid[j] == a) {
                        exists = true;
                        break;
                    }
                }

                passed &= exists;
            }
            return passed;
        }

        function prepareTransfer(e) {
            var stop = true;
            var keys = ($find("gridMain") || $find("grid2")).get_clientSelectedKeys();
            if (keys.length > 0) {
                if ($pc.popups.searchOrg("actionData", 1, [{ "permission": "AddChildren"}]) == true) {
                    stop = false;
                    var data = { srcKeys: keys, targetKeys: $pc.get("actionData").value.split(",") };
                    e.steps = [Sys.Serialization.JavaScriptSerializer.serialize(data)];
                }
            }
            e.cancel = stop;
        }

        function afterTransfer() {
            $pc.get("refreshTrigger").click();
        }
    </script>
</body>
</html>
