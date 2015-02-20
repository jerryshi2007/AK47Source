<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MemberOrgView.aspx.cs"
	Inherits="PermissionCenter.MemberOrgView" %>

<%--怪异模式测试时将文档声明置于服务器注释块内--%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" scroll="no" style="overflow: hidden">
<head id="Head1" runat="server">
	<title>权限中心-人员组织</title>
	<link rel="icon" href="../favicon.ico" type="image/x-icon" />
	<link rel="Shortcut Icon" href="../favicon.ico" />
	<base target="_self" />
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
	<soa:DataBindingControl runat="server" ID="binding1">
		<ItemBindings>
			<soa:DataBindingItem DataPropertyName="VisibleName" ControlID="userDisplayName" ControlPropertyName="Text"
				Direction="DataToControl" />
			<soa:DataBindingItem DataPropertyName="ID" ControlID="lnkToGroup" ControlPropertyName="NavigateUrl"
				Direction="DataToControl" Format="~/lists/MemberGrpView.aspx?id={0}" />
			<soa:DataBindingItem DataPropertyName="ID" ControlID="lnkToOrg" ControlPropertyName="NavigateUrl"
				Direction="DataToControl" Format="~/lists/MemberOrgView.aspx?id={0}" />
		</ItemBindings>
	</soa:DataBindingControl>
	<div class="pc-banner">
		<h1 class="pc-caption">
			<span>
				<asp:Literal Text="某某" runat="server" ID="userDisplayName" Mode="Encode" />
			</span>的组织单元<span class="pc-timepointmark"><mcs:TimePointDisplayControl ID="TimePointDisplayControl1"
				runat="server">
			</mcs:TimePointDisplayControl>
			</span>
		</h1>
	</div>
	<pc:BannerNotice ID="notice" runat="server"></pc:BannerNotice>
	<pc:SceneControl ID="SceneControl1" runat="server">
	</pc:SceneControl>
	<ul class="pc-tabs-header">
		<li class="pc-active">
			<asp:HyperLink runat="server" ID="lnkToOrg">组织</asp:HyperLink>
		</li>
		<li>
			<asp:HyperLink runat="server" ID="lnkToGroup">群组</asp:HyperLink>
		</li>
	</ul>
	<div class="pc-tabs-content pc-active">
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
		<div class="pc-container5 pc-icon16">
			<div class="pc-listmenu-container">
				<div style="float: left">
					<span class="pc-label">缺省组织</span>
					<asp:HyperLink runat="server" ID="lnkMainOrg" Target="_blank"></asp:HyperLink>
					<asp:HiddenField runat="server" ID="hfOrgId" />
					&nbsp; <span class="pc-label">所有者</span>&nbsp;
					<asp:HyperLink runat="server" ID="lnkToOwner" Target="_blank"></asp:HyperLink>
				</div>
				<ul class="pc-listmenu" id="listMenu">
					<li class="pc-dropdownmenu" style="float: right"><span style="display: block; display: inline-block">
						<asp:HyperLink ID="lnkViewMode" runat="server" CssClass="pc-toggler-dd list-cmd shortcut"
							NavigateUrl="javascript:void(0);" Style="margin-right: 0; cursor:default">
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
			<div class="pc-grid-container clear">
				<asp:MultiView ActiveViewIndex="0" runat="server" ID="views">
					<asp:View runat="server">
						<mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
							AllowPaging="True" AllowSorting="True" Category="PermissionCenter" GridTitle="组织"
							DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title" PagerSettings-Position="Bottom"
							DataSourceMaxRow="0"  TitleColor="141, 143, 149" TitleFontSize="Large"
							MultiSelect="false" ShowExportControl="true" OnRowCommand="HandleRowCommand">
							<EmptyDataTemplate>
								暂时没有您需要的数据
							</EmptyDataTemplate>
							<HeaderStyle CssClass="head" />
							<Columns>
								<asp:TemplateField>
									<HeaderTemplate>
										<mcs:GridColumnSorter ID="colSorter" runat="server" DefaultOrderName="名称" DockPosition="Left"
											PreventRenderChildren='<%# this.gridMain.ExportingDeluxeGrid  %>'>
						<SortItems>
						<mcs:SortItem SortExpression="O.Name" Text="名称" />
						<mcs:SortItem SortExpression="O.DisplayName" Text="显示名称" />
						<mcs:SortItem SortExpression="O.CodeName" Text="代码名称" />
						</SortItems>
										</mcs:GridColumnSorter>
									</HeaderTemplate>
									<ItemTemplate>
										<div>
											<asp:HyperLink ID="hl1" NavigateUrl='<%#"~/lists/OUExplorer.aspx?ou="+Server.UrlEncode((string)Eval("ID"))%>'
												CssClass="pc-item-link" runat="server" data-id='<%# PermissionCenter.Util.HtmlAttributeEncode((string) Eval("ID")) %>'
												Target="_blank">
												<i class="pc-icon16 Organizations"></i>
												<%# Server.HtmlEncode((string) Eval("Name")) %>
											</asp:HyperLink><asp:Image ImageUrl="~/images/HomeHS.png" runat="server" Visible='<%# this.IsDefaultOrg((string)Eval("ID")) %>'
												ToolTip="缺省组织" /><asp:Image ImageUrl="~/images/key.png" runat="server" Visible='<%# this.IsOwner((string)Eval("ID")) %>'
													ToolTip="所有者" />
										</div>
										<div>
											<%# Server.HtmlEncode((string)Eval("DisplayName")) %>
										</div>
										<div>
											<div id="divActions" class="pc-action-tray" runat="server" visible='<%# this.gridMain.ExportingDeluxeGrid == false %>'>
												<asp:LinkButton ID="lnkEdit" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
												<asp:LinkButton ID="lb1" Text="设为缺省组织" runat="server" CssClass="pc-item-cmd" Enabled='<%# this.IsSetDefaultEnabled((string)Eval("ID")) %>'
													data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													CommandName="SetDefault" CommandArgument='<%# Eval("ID") %>' Visible='<%# this.IsDefaultOrg((string)Eval("ID")) == false %>' />
												<asp:LinkButton ID="lb2" Text="设为所有者" runat="server" CssClass="pc-item-cmd" Enabled='<%# this.IsSetOwnerEnabled((string)Eval("ID")) %>'
													data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													CommandName="SetOwner" CommandArgument='<%# Eval("ID") %>' Visible='<%# this.IsOwner((string)Eval("ID")) == false %>' />
												<asp:LinkButton ID="lnkItemDelete" runat="server" Text="退出组织" CssClass="pc-item-cmd"
													CommandName="DeleteItem" Enabled='<%# this.IsExitEnabled((string)Eval("ID")) %>'
													CommandArgument='<%#Eval("ID") %>' OnClientClick="return ($pc.getEnabled(this) && $pc.confirmDelete('将目标从组织中移除（条件组织无效），是否确定?'));"></asp:LinkButton>
												<asp:HyperLink ID="lnkHistory" runat="server" CssClass="pc-item-cmd" Target="_blank"
													onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/ObjectHistoryLog.aspx?id={0}") %>'>历史</asp:HyperLink>
											</div>
										</div>
										<div>
											<%# Server.HtmlEncode((string)Eval("CodeName")) %>
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField HeaderText="创建者" SortExpression="O.CreatorName">
									<ItemTemplate>
										<soa:UserPresence runat="server" ID="userPresence" ShowUserDisplayName="true" ShowUserIcon="false"
											UserID='<%#Eval("CreatorID") %>' UserDisplayName='<%# Server.HtmlEncode(Eval("CreatorName").ToString()) %>' />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField HeaderText="位置">
									<ItemTemplate>
										<div>
											<soa:OuNavigator ID="navPath" runat="server" StartLevel="0" SplitterVisible="true"
												Target="_blank" NavigateUrlFormat="~/lists/OUExplorer.aspx?ou={0}" LinkCssClass="pc-item-link"
												TerminalVisible="false" ObjectID='<%#Eval("ID") %>'></soa:OuNavigator>
										</div>
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
					</asp:View>
					<asp:View runat="server">
						<mcs:DeluxeGrid ID="grid2" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
							AllowPaging="True" AllowSorting="True" Category="PermissionCenter" GridTitle="组织"
							DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title" PagerSettings-Position="Bottom"
							DataSourceMaxRow="0"  TitleColor="141, 143, 149" TitleFontSize="Large"
							MultiSelect="false" ShowExportControl="true" OnRowCommand="HandleRowCommand">
							<EmptyDataTemplate>
								暂时没有您需要的数据
							</EmptyDataTemplate>
							<HeaderStyle CssClass="head" />
							<Columns>
								<asp:TemplateField HeaderText="名称" SortExpression="O.Name">
									<ItemTemplate>
										<div>
											<asp:HyperLink ID="hl1" NavigateUrl='<%#"~/lists/OUExplorer.aspx?ou="+Server.UrlEncode((string)Eval("ID"))%>'
												Text='<%# Server.HtmlEncode((string) Eval("Name")) %>' CssClass="pc-item-link"
												runat="server" data-id='<%# PermissionCenter.Util.HtmlAttributeEncode((string) Eval("ID")) %>'
												Target="_blank" /><asp:Image ID="Image1" ImageUrl="~/images/HomeHS.png" runat="server"
													Visible='<%# this.IsDefaultOrg((string)Eval("ID")) %>' ToolTip="缺省组织" /><asp:Image
														ID="Image2" ImageUrl="~/images/key.png" runat="server" Visible='<%# this.IsOwner((string)Eval("ID")) %>'
														ToolTip="所有者" />
										</div>
										<div>
											<div id="divActions" class="pc-action-tray" runat="server" visible='<%# this.gridMain.ExportingDeluxeGrid == false %>'>
												<asp:LinkButton ID="lnkEdit" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
												<asp:LinkButton ID="lb1" Text="设为缺省组织" runat="server" CssClass="pc-item-cmd" Enabled='<%# this.IsSetDefaultEnabled((string)Eval("ID")) %>'
													data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													CommandName="SetDefault" CommandArgument='<%# Eval("ID") %>' Visible='<%# this.IsDefaultOrg((string)Eval("ID")) == false %>' />
												<asp:LinkButton ID="lb2" Text="设为所有者" runat="server" CssClass="pc-item-cmd" Enabled='<%# this.IsSetOwnerEnabled((string)Eval("ID")) %>'
													data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													CommandName="SetOwner" CommandArgument='<%# Eval("ID") %>' Visible='<%# this.IsOwner((string)Eval("ID")) == false %>' />
												<asp:LinkButton ID="lnkItemDelete" runat="server" Text="退出组织" CssClass="pc-item-cmd"
													CommandName="DeleteItem" Enabled='<%# this.IsExitEnabled((string)Eval("ID")) %>'
													CommandArgument='<%#Eval("ID") %>' OnClientClick="return ($pc.getEnabled(this) && $pc.confirmDelete('将目标从组织中移除（条件组织无效），是否确定?'));"></asp:LinkButton>
												<asp:HyperLink ID="lnkHistory" runat="server" CssClass="pc-item-cmd" Target="_blank"
													onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/ObjectHistoryLog.aspx?id={0}") %>'>历史</asp:HyperLink>
											</div>
										</div>
									</ItemTemplate>
								</asp:TemplateField>
								<asp:BoundField DataField="DisplayName" HtmlEncode="true" HeaderText="显示名称" SortExpression="O.DisplayName" />
								<asp:BoundField DataField="CodeName" HtmlEncode="true" HeaderText="代码名称" SortExpression="O.CodeName" />
								<asp:TemplateField HeaderText="创建者" SortExpression="O.CreatorName">
									<ItemTemplate>
										<soa:UserPresence runat="server" ID="userPresence" ShowUserDisplayName="true" ShowUserIcon="false"
											UserID='<%#Eval("CreatorID") %>' UserDisplayName='<%# Server.HtmlEncode(Eval("CreatorName").ToString()) %>' />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField HeaderText="位置">
									<ItemTemplate>
										<div>
											<soa:OuNavigator ID="navPath" runat="server" StartLevel="0" SplitterVisible="true"
												Target="_blank" NavigateUrlFormat="~/lists/OUExplorer.aspx?ou={0}" LinkCssClass="pc-item-link"
												TerminalVisible="false" ObjectID='<%#Eval("ID") %>'></soa:OuNavigator>
										</div>
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
					</asp:View>
				</asp:MultiView>
			</div>
		</div>
	</div>
	<soa:DeluxeObjectDataSource ID="dataSourceMain" runat="server" EnablePaging="True"
		TypeName="MCS.Library.SOA.DataObjects.Security.DataSources.SchemaUsersOrgDataSource"
		EnableViewState="false" OnSelecting="dataSourceMain_Selecting" OnSelected="dataSourceMain_Selected">
		<SelectParameters>
			<asp:QueryStringParameter DbType="String" Name="parentId" QueryStringField="id" />
		</SelectParameters>
	</soa:DeluxeObjectDataSource>
	<pc:Footer ID="footer" runat="server" />
	</form>
	<script type="text/javascript">
		$pc.ui.listMenuBehavior("listMenu");
		$pc.ui.gridBehavior("gridMain", "hover");
		$pc.ui.gridBehavior("grid2", "hover");
		$pc.ui.traceWindowWidth();

		function onconditionClick(sender, e) {
			var content = Sys.Serialization.JavaScriptSerializer.deserialize(e.ConditionContent);
			var bindingControl = $find("searchBinding");
			bindingControl.dataBind(content);
		}
	</script>
</body>
</html>
