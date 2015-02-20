<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="SchemaRoleList.aspx.cs"
    Inherits="AUCenter.SchemaAdmin.SchemaRoleList" %>

<%@ Register TagPrefix="mcs" Namespace="MCS.Web.WebControls" Assembly="MCS.Library.Passport" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Schema角色定义</title>
    <base target="_self" />
    <link href="../Styles/dlg.css" rel="stylesheet" type="text/css" />
</head>
<body class="au-full">
    <form id="form1" runat="server">
    <au:SceneControl ID="SceneControl1" runat="server" />
    <div class="pcdlg-sky">
        <h1 class="pc-caption">
            <asp:Label runat="server" ID="schemaInfoLabel" />
            - 角色定义<span class="pc-timepointmark">
                <mcs:TimePointDisplayControl ID="tpdc" runat="server" />
            </span>
        </h1>
    </div>
    <div class="pcdlg-content">
        <soa:MultiProcessControl runat="server" ID="ctlProgress" DialogHeaderText="操作进行中"
            DialogTitle="删除" OnClientBeforeStart="doBeforeStart" OnClientFinishedProcess="doFinishBatch"
            OnExecuteStep="ctlProgress_ExecuteStep" ShowingMode="Normal" ShowStepErrors="True" />
        <soa:MultiProcessControl runat="server" ID="ctlProgressSingle" DialogHeaderText="操作进行中"
            DialogTitle="删除/恢复" OnClientBeforeStart="doBeforeStartSingle" OnClientFinishedProcess="doFinish"
            OnExecuteStep="ctlProgress_ExecuteSingleStep" ShowingMode="Normal" ShowStepErrors="True" />
        <asp:ScriptManager runat="server" ID="sm" EnablePageMethods="true" EnableScriptGlobalization="true" />
        <div class="au-progress" id="progress">
        </div>
        <div class="pc-listmenu-container">
            <div class="pc-container5">
                <ul class="pc-listmenu" id="listMenu">
                    <li>
                        <asp:LinkButton runat="server" ID="lnkAdd" CssClass="button pc-list-cmd" OnClientClick="return ($pc.getEnabled(this) && $pc.popups.newAdminSchemaRole(this));"
                            OnClick="RefreshList">新建</asp:LinkButton>
                    </li>
                    <li>
                        <asp:LinkButton runat="server" ID="lnkDelete" CssClass="button pc-list-cmd" OnClientClick="return ($pc.getEnabled(this) && doDeleting());">删除</asp:LinkButton>
                    </li>
                    <li>
                        <asp:HyperLink runat="server" ID="lnkToggle" Text=">>显示已删除的角色" EnableViewState="false"></asp:HyperLink>
                    </li>
                    <li class="pc-dropdownmenu"><span style="display: block; display: inline-block;">
                        <asp:LinkButton runat="server" CssClass="list-cmd" ID="btnImport" OnClientClick="return ($pc.getEnabled(this) && invokeImport());"
                            OnClick="RefreshList">导入</asp:LinkButton></span> </li>
                    <li class="pc-dropdownmenu">
                        <asp:LinkButton runat="server" CssClass="list-cmd" ID="LinkButton1" OnClientClick="return ($pc.getEnabled(this) && invokeExport());"
                            OnClick="RefreshList">导出<i class="pc-arrow"></i></asp:LinkButton>
                        <div style="position: relative; z-index: 9">
                            <div style="position: absolute;">
                                <ul class="pc-popup-nav">
                                    <li>
                                        <asp:LinkButton runat="server" CssClass="list-cmd" ID="btnExportSelected" OnClientClick="return ($pc.getEnabled(this) && invokeExport() && false);"
                                            OnClick="RefreshList">导出选定角色（默认）</asp:LinkButton>
                                    </li>
                                    <li>
                                        <asp:LinkButton runat="server" CssClass="list-cmd" ID="btnExportAll" OnClientClick="return ($pc.getEnabled(this) && invokeExportAll() && false);"
                                            OnClick="RefreshList">导出全部角色</asp:LinkButton>
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </li>
                </ul>
            </div>
        </div>
        <div class="pc-container5">
            <div class="pc-grid-container">
                <mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
                    AllowPaging="True" AllowSorting="True" ShowCheckBoxes="True" Category="PermissionCenter"
                    ShowExportControl="true" GridTitle="Schema角色" DataKeyNames="ID" CssClass="dataList pc-datagrid"
                    TitleCssClass="title" PagerSettings-Position="Bottom" DataSourceMaxRow="0" TitleColor="141, 143, 149"
                    TitleFontSize="Large" OnRowCommand="gridMain_RowCommand" OnRowDataBound="gridMain_RowDataBound">
                    <EmptyDataTemplate>
                        暂时没有您需要的数据
                    </EmptyDataTemplate>
                    <HeaderStyle CssClass="head" />
                    <Columns>
                        <asp:TemplateField HeaderText="名称" SortExpression="S.Name">
                            <ItemTemplate>
                                <div>
                                    <%# Server.HtmlEncode((string)Eval("Name")) %>
                                </div>
                                <div>
                                    <div id="Div1" class="pc-action-tray" runat="server" visible='<%# this.gridMain.ExportingDeluxeGrid == false %>'>
                                        <asp:LinkButton ID="lnkEdit1" runat="server" CssClass="pc-item-cmd" data-id='<%# System.Web.HttpUtility.HtmlAttributeEncode((string)Eval("ID"))%>'
                                            OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
                                        <a runat="server" onclick="return assignTicket(this)" data-roleid='<%#Eval("ID") %>'
                                            readonly='<%# this.EditEnabled?"":"readOnly" %>' href="javascript:void(0);" class="pc-item-cmd"
                                            target="_blank">矩阵定义</a>
                                        <asp:HyperLink runat="server" ID="lnkItemDelete" class="pc-item-cmd" NavigateUrl="javascript:void(0);"
                                            Text='<%# (1== (int)Eval("Status"))?"删除":"撤销删除"  %>' Enabled='<%# this.EditEnabled %>'
                                            data-status='<%#Eval("Status") %>' data-id='<%#Eval("ID") %>' onclick="return ($pc.getEnabled(this) && doDeleteItem(this) && false);"></asp:HyperLink>
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
                        <asp:TemplateField HeaderText="矩阵">
                            <ItemTemplate>
                                <div style="padding-left: 8px; padding-right: 8px">
                                    <soa:RoleMatrixEntryControl runat="server" RoleID='<%#Eval("ID")%>' RoleName='<%# Eval("Name") %>'
                                        RoleCodeName='<%#Eval("CodeName") %>' RoleDescription='<%#Eval("DisplayName") %>'
                                        ID="matrixEntry" EnableAccessTicket="true" ReadOnly='<%# this.EditEnabled ==false %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <mcs:EnumDropDownField DataField="Status" SortExpression="S.Status" HeaderText="状态"
                            ReadOnly="true" EnumTypeName="MCS.Library.SOA.DataObjects.Schemas.SchemaProperties.SchemaObjectStatus, MCS.Library.SOA.DataObjects.Schemas" />
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
            <soa:DeluxeObjectDataSource runat="server" ID="dataSourceMain" TypeName="MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources.AUSchemaRoleDataSource"
                EnablePaging="True" OnSelecting="dataSourceMain_Selecting">
                <SelectParameters>
                    <asp:QueryStringParameter Type="String" ConvertEmptyStringToNull="true" Name="schemaID"
                        QueryStringField="id" />
                </SelectParameters>
            </soa:DeluxeObjectDataSource>
            <div style="display: none">
                <input type="hidden" runat="server" id="schemaIDField" />
                <a id="lnkRefresh" runat="server" href="" />
                <mcs:AccessTicketHtmlAnchor runat="server" ID="ticketGenerator" href="#" OnClientAccquiredAccessTicket="onAccquiredTicket" />
                <soa:UploadProgressControl runat="server" Category="管理架构角色" DialogHeaderText="导入管理架构角色"
                    ID="ctlUpload" DialogTitle="导入管理架构角色" OnDoUploadProgress="DoFileUpload" OnLoadingDialogContent="ctlUpload_LoadingDialogContent"
                    OnClientCompleted="postImportProcess" />
            </div>
        </div>
    </div>
    <div class="pcdlg-floor">
        <div class="pcdlg-button-bar">
            <input type="button" onclick="window.close();" class="pcdlg-button" value="关闭" />
        </div>
    </div>
    <script type="text/javascript">
        $pc.ui.gridBehavior("gridMain", "hover");
        $pc.ui.listMenuBehavior("listMenu");
        var singleid = '';

        function doBeforeStart(e) {
            var keys = $find("gridMain").get_clientSelectedKeys();
            if (keys.length) {
                var result = new Array(keys.length);
                for (var i = keys.length - 1; i >= 0; i--) {
                    result[i] = keys[i];
                }
                e.steps = result;
            } else {
                e.cancel = true;
            }
        }

        function doBeforeStartSingle(e) {
            if (singleid) {
                e.steps = [singleid];
            } else {
                e.cancel = true;
            }
        }

        function doFinish(e) {
            if (e.error.length) {
                for (var i = 0; i < e.error.length; i++) {
                    alert(e.error[i]);
                }
            } else {

                $get("lnkRefresh").click();
                return false;
            }
        }

        function doFinishBatch(e) {
            if (e.error.length) {
                for (var i = 0; i < e.error.length; i++) {
                    alert(e.error[i]);
                }
            } else {
                e.cancel = true;
                window.setTimeout(function () {
                    window.location.href = "#";
                    window.location.replace($get("lnkRefresh").href);
                }, 200);
            }
        }

        function doDeleting() {
            var keys = $find("gridMain").get_clientSelectedKeys();
            if (keys.length) {
                $find("ctlProgress").start();
            }
        }

        function doDeleteItem(elem) {
            if (elem) {
                var id = $pc.getAttr(elem, "data-id");
                var status = $pc.getAttr(elem, "data-status");
                var msg = status == 1 ? "确实要删除这个角色？" : "确实要恢复这个角色？";
                if (confirm(msg)) {
                    singleid = id;
                    $find("ctlProgressSingle").start();
                }
            }
        }

        function assignTicket(a) {
            var link = $get("ticketGenerator");
            var editMode = "normal";
            if ($pc.getAttr(a, 'readOnly') == 'readOnly') {
                editMode = "readOnly";
            }

            link.href = document.location.protocol + "//" + document.location.hostname + "/MCSWebApp/WorkflowDesigner/MatrixModalDialog/RolePropertyExtension.aspx?RoleID=" + $pc.getAttr(a, "data-roleId") + "&editMode=" + editMode;
            link.click();
            return false;
        }

        function onAccquiredTicket(a) {
            return $pc.modalPopup(a, 460, 240);
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
            var grid = $find("gridMain");
            if (grid) {
                var keys = grid.get_clientSelectedKeys();
                if (keys.length > 0) {
                    $pc.postViaIFrame($pc.appRoot + "Services/Export.ashx", { context: "SchemaRoleList", auSchemaId: $pc.get("schemaIDField").value, roleIds: keys });
                }
            }
            grid = false;
            return false;
        }

        function invokeExportAll() {
            $pc.postViaIFrame($pc.appRoot + "Services/Export.ashx", { context: "SchemaRoleList", auSchemaId: $pc.get("schemaIDField").value });
            return false;
        }
    </script>
    </form>
</body>
</html>
