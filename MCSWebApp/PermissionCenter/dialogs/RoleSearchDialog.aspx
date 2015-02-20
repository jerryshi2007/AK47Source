<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoleSearchDialog.aspx.cs"
    Inherits="PermissionCenter.Dialogs.RoleSearchDialog" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" scroll="no" style="overflow: hidden">
<head id="Head1" runat="server">
    <title>权限中心-选择角色</title>
    <link rel="icon" href="../favicon.ico" type="image/x-icon" />
    <link rel="Shortcut Icon" href="../favicon.ico" />
    <base target="_self" />
    <link href="../styles/dlg.css" rel="stylesheet" type="text/css" />
    <pc:HeaderControl runat="server">
    </pc:HeaderControl>
    <script type="text/javascript">
        function finishDialog(rstString) {
            if (typeof (window.dialogArguments) == "object") {
                if ("window" in window.dialogArguments && "inputElem" in window.dialogArguments) {
                    var hiddenField = window.dialogArguments.window.document.getElementById(window.dialogArguments.inputElem);
                    if (hiddenField != null && hiddenField.nodeType == 1 && hiddenField.nodeName.toUpperCase() == "INPUT") {
                        hiddenField.value = rstString;
                        window.returnValue = true;
                        hiddenField = null;
                        window.close();
                    }
                }
            } else {
                alert("必须使用对话框");
            }
        }
	
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Literal runat="server" ID="preScript" EnableViewState="false" />
    <div class="pcdlg-sky">
        <div style="float: right; padding: 5px">
            显示以下应用中的角色：<asp:DropDownList runat="server" ID="appList" AutoPostBack="True" DataMember="Apps"
                DataSourceID="AppDataSource1" DataTextField="DisplayName" DataValueField="ID"
                OnSelectedIndexChanged="appList_SelectedIndexChanged">
            </asp:DropDownList>
            <pc:AppDataSource ID="AppDataSource1" runat="server">
            </pc:AppDataSource>
        </div>
        <h1 class="pc-caption">
            选择应用角色</h1>
    </div>
    <div class="pcdlg-content">
        <pc:BannerNotice ID="notice" runat="server"></pc:BannerNotice>
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
            <div>
                <div class="pc-listmenu-container">
                    <ul class="pc-listmenu" id="listMenu">
                        <li>
                            <asp:HyperLink NavigateUrl="" runat="server" ID="lnkAppMan" CssClass="button pc-list-cmd"
                                Text="管理此应用中的角色" Target="_blank" />
                        </li>
                        <li>
                            <asp:LinkButton runat="server" ID="btnRefresh" CssClass="button pc-list-cmd" OnClick="RefreshClick">刷新</asp:LinkButton>
                        </li>
                    </ul>
                </div>
            </div>
            <div class="pc-grid-container">
                <mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
                    AllowPaging="True" AllowSorting="True" ShowCheckBoxes="True" Category="PermissionCenter"
                    GridTitle="角色" DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title"
                    PagerSettings-Position="Bottom" DataSourceMaxRow="0" TitleColor="141, 143, 149"
                    TitleFontSize="Large">
                    <EmptyDataTemplate>
                        暂时没有您需要的数据
                    </EmptyDataTemplate>
                    <HeaderStyle CssClass="head" />
                    <Columns>
                        <asp:TemplateField HeaderText="名称" SortExpression="Name">
                            <ItemTemplate>
                                <span><i class="pc-icon16 Roles"></i>
                                    <%# Server.HtmlEncode((string)Eval("Name")) %></span>
                                <div>
                                    <div class="pc-action-tray">
                                        <asp:LinkButton ID="lnkEdit" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
                                            OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
                                        <asp:HyperLink ID="HyperLink1" runat="server" CssClass="pc-item-cmd" Text="角色功能定义"
                                            NavigateUrl='<%#Eval("ID","~/dialogs/RoleDefinition.aspx?role={0}") %>' onclick="return $pc.modalPopup(this);"
                                            Target="_blank"></asp:HyperLink>
                                        <asp:HyperLink ID="HyperLink3" runat="server" CssClass="pc-item-cmd" Text="角色成员"
                                            NavigateUrl='<%#Eval("ID","~/lists/AppRoleMembers.aspx?role={0}") %>' onclick="return $pc.modalPopup(this);"
                                            Target="_blank"></asp:HyperLink>
                                        <asp:HyperLink ID="lnkRP" runat="server" NavigateUrl='<%#Eval("ID","~/lists/FlowList.aspx?id={0}") %>'
                                            onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">查看相关流程</asp:HyperLink>
                                        <%--<asp:HyperLink ID="HyperLink2" CssClass="pc-item-cmd" NavigateUrl='<%#Eval("ID","AppRoleMatrix.aspx?role={0}") %>'
											onclick="return $pc.modalPopup(this);" runat="server" Target="_blank" Text="活动矩阵" />--%>
                                        <asp:HyperLink ID="lnkHistory" runat="server" CssClass="pc-item-cmd" Target="_blank"
                                            onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/ObjectHistoryLog.aspx?id={0}") %>'>历史</asp:HyperLink>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DisplayName" HtmlEncode="true" HeaderText="显示名称" SortExpression="DisplayName" />
                        <asp:BoundField DataField="CodeName" HtmlEncode="true" HeaderText="代码名称" SortExpression="CodeName" />
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
        <soa:DeluxeObjectDataSource ID="dataSourceMain" runat="server" EnablePaging="True"
            TypeName="MCS.Library.SOA.DataObjects.Security.DataSources.SchemaApplicationRolesDataSource"
            EnableViewState="false" OnSelecting="dataSourceMain_Selecting">
            <SelectParameters>
                <asp:ControlParameter ControlID="appList" Direction="Input" Name="parentId" Type="String" />
            </SelectParameters>
        </soa:DeluxeObjectDataSource>
        <pc:Footer ID="footer" runat="server" />
    </div>
    <div class="pcdlg-floor">
        <div class="pcdlg-button-bar">
            <asp:Button ID="btnOk" Text="确定" runat="server" CssClass="pcdlg-button" OnClick="HandleOk"
                OnClientClick="return isAnySelection();" /><input type="button" class="pcdlg-button"
                    value="取消" onclick="doCancel();" />
        </div>
    </div>
    </form>
    <script type="text/javascript">
        $pc.ui.gridBehavior("gridMain", "hover");
        $pc.ui.traceWindowWidth();

        function onconditionClick(sender, e) {
            var content = Sys.Serialization.JavaScriptSerializer.deserialize(e.ConditionContent);
            var bindingControl = $find("searchBinding");
            bindingControl.dataBind(content);
        }

        function isAnySelection() {
            var result = false;
            if ($find("gridMain").get_clientSelectedKeys().length > 0)
                result = true;
            return result;
        }

        function doCancel() {
            window.returnValue = false;
            window.close();
        }
    </script>
</body>
</html>
