<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MemberMatrix.aspx.cs" Inherits="PermissionCenter.lists.MemberMatrix" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>权限中心-应用角色</title>
    <link rel="icon" href="../favicon.ico" type="image/x-icon" />
    <link rel="Shortcut Icon" href="../favicon.ico" />
    <base target="_self" />
    <pc:HeaderControl ID="hc1" runat="server">
    </pc:HeaderControl>
</head>
<body>
    <form id="form1" runat="server">
    <soa:DataBindingControl runat="server" ID="binding1">
        <ItemBindings>
            <soa:DataBindingItem DataPropertyName="VisibleName" ControlID="txtUserName" ControlPropertyName="Text"
                Direction="DataToControl" />
        </ItemBindings>
    </soa:DataBindingControl>
    <%--	<pc:SceneControl ID="sc1" runat="server">
	</pc:SceneControl>--%>
    <pc:BannerNotice ID="notice" runat="server" RenderType="Auto"></pc:BannerNotice>
    <h1 class="pc-caption">
        <asp:Literal ID="txtUserName" runat="server" Mode="Encode"></asp:Literal>
        - 参与的角色矩阵<span class="pc-timepointmark"><mcs:TimePointDisplayControl ID="TimePointDisplayControl1"
            runat="server">
        </mcs:TimePointDisplayControl>
        </span>
    </h1>
    <div class="pc-frame-container">
        <div class="pc-container5">
            <div class="pc-listmenu-container">
                <ul class="pc-listmenu" id="listMenu">
                </ul>
            </div>
            <div class="pc-grid-container">
                <mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
                    AllowPaging="True" AllowSorting="True" ShowCheckBoxes="False" Category="PermissionCenter"
                    GridTitle="应用角色" DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title"
                    PagerSettings-Position="Bottom" DataSourceMaxRow="0" ShowExportControl="true"
                    TitleColor="141, 143, 149" TitleFontSize="Large">
                    <EmptyDataTemplate>
                        暂时没有您需要的数据
                    </EmptyDataTemplate>
                    <HeaderStyle CssClass="head" />
                    <Columns>
                        <asp:TemplateField HeaderText="角色名称" SortExpression="R.Name">
                            <ItemTemplate>
                                <div>
                                    <asp:HyperLink ID="hl1" runat="server" CssClass="pc-item-link" NavigateUrl='<%#Eval("ID","~/lists/AppRoleMembers.aspx?role={0}") %>'
                                        Target="_blank">
										<i class="pc-icon16 Roles"></i><%# Server.HtmlEncode((string)Eval("Name")) %>
                                    </asp:HyperLink></div>
                                <div>
                                    <div class="pc-action-tray" runat="server" id="d" visible='<%# this.gridMain.ExportingDeluxeGrid == false %>'>
                                        <asp:LinkButton ID="lnkEdit" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
                                            OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
                                        <asp:HyperLink ID="hl3" runat="server" CssClass="pc-item-cmd" Text="角色成员" onclick="return $pc.modalPopup(this);"
                                            NavigateUrl='<%#Eval("ID","~/lists/AppRoleMembers.aspx?role={0}") %>' Target="_blank"></asp:HyperLink>
                                        <%--<asp:HyperLink ID="HyperLink2" CssClass="pc-item-cmd" NavigateUrl='<%#Eval("ID","AppRoleMatrix.aspx?role={0}") %>'
											onclick="return $pc.modalPopup(this);" runat="server" Target="_blank" Text="活动矩阵" />--%>
                                        <asp:HyperLink ID="lnkRP" runat="server" NavigateUrl='<%#Eval("ID","~/lists/FlowList.aspx?id={0}") %>'
                                            onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">查看相关流程</asp:HyperLink>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DisplayName" HtmlEncode="true" HeaderText="角色显示名称" SortExpression="R.DisplayName" />
                        <asp:BoundField DataField="CodeName" HtmlEncode="true" HeaderText="代码名称" SortExpression="R.CodeName" />
                        <asp:TemplateField HeaderText="矩阵">
                            <ItemTemplate>
                                <div style="padding-left: 8px; padding-right: 8px">
                                    <soa:RoleMatrixEntryControl ID="rm1" runat="server" AppID='<%# Eval("AppID") %>'
                                        RoleID='<%#Eval("ID")%>' RoleName='<%# Eval("Name") %>' RoleCodeName='<%#Eval("CodeName") %>'
                                        RoleDescription='<%#Eval("DisplayName") %>' EnableAccessTicket="true" ReadOnly='<%# !this.EditRoleMembersEnabled((string)Eval("AppID")) %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="创建者" SortExpression="R.CreatorName">
                            <ItemTemplate>
                                <soa:UserPresence runat="server" ID="userPresence" ShowUserDisplayName="true" ShowUserIcon="false"
                                    UserID='<%#Eval("CreatorID") %>' UserDisplayName='<%# Server.HtmlEncode(Eval("CreatorName").ToString()) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CreateDate" HtmlEncode="true" HeaderText="创建日期" SortExpression="R.CreateDate"
                            DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                        <asp:TemplateField HeaderText="所属应用" SortExpression="A.DisplayName">
                            <ItemTemplate>
                                <asp:HyperLink ID="hl2" runat="server" NavigateUrl='<%#Eval("AppID","~/lists/AllApps.aspx?id={0}") %>'
                                    CssClass="pc-item-link" Target="_blank"><i class="pc-icon16 Applications"></i>
								<%# Server.HtmlEncode (PermissionCenter.Util.AutoName((string)Eval("AppName"),(string)Eval("AppName2"))) %>
                                </asp:HyperLink>
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
        </div>
    </div>
    <soa:DeluxeObjectDataSource ID="dataSourceMain" runat="server" EnablePaging="True"
        TypeName="MCS.Library.SOA.DataObjects.Security.DataSources.UserMatrixDataSource"
        EnableViewState="false" OnSelecting="dataSourceMain_Selecting" OnSelected="dataSourceMain_Selected">
        <SelectParameters>
            <asp:QueryStringParameter DbType="String" Name="userId" QueryStringField="id" />
        </SelectParameters>
    </soa:DeluxeObjectDataSource>
    <pc:Footer ID="footer" runat="server" />
    </form>
    <script type="text/javascript">
        $pc.ui.listMenuBehavior("listMenu");
        $pc.ui.gridBehavior("gridMain", "hover");
        $pc.ui.traceWindowWidth();
    </script>
</body>
</html>
