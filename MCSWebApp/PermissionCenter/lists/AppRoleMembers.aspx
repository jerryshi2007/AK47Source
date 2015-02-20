<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppRoleMembers.aspx.cs"
	Inherits="PermissionCenter.AppRoleMembers" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" scroll="no" style="overflow: hidden">
<head runat="server">
	<title>角色成员</title>
	<link rel="icon" href="../favicon.ico" type="image/x-icon" />
	<link rel="Shortcut Icon" href="../favicon.ico" />
	<base target="_self" />
	<style type="text/css">
		
	</style>
	<pc:HeaderControl runat="server">
	</pc:HeaderControl>
</head>
<body>
	<form id="form1" runat="server">
	<soa:DataBindingControl runat="server" ID="binding1">
		<ItemBindings>
			<soa:DataBindingItem ControlID="ctlUpload" ControlPropertyName="Tag" DataPropertyName="RoleID"
				Direction="DataToControl" />
			<soa:DataBindingItem DataPropertyName="RoleID" ControlID="hfRoleID" ControlPropertyName="Value"
				Direction="DataToControl" />
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
	<pc:SceneControl ID="SceneControl1" runat="server">
	</pc:SceneControl>
	<div class="pc-banner">
		<h1 class="pc-caption">
			角色成员-<asp:Literal ID="roleName" runat="server" Mode="Encode"></asp:Literal>
			<span style="float: right">
				<soa:RoleMatrixEntryControl ID="roleMatrixEntryControl" runat="server" EnableAccessTicket="true" />
			</span><span class="pc-timepointmark">
				<mcs:TimePointDisplayControl ID="TimePointDisplayControl1" runat="server" />
			</span>
		</h1>
	</div>
	<pc:BannerNotice ID="notice1" runat="server" />
	<ul class="pc-tabs-header">
		<li class="pc-active">
			<asp:HyperLink ID="lnkConst" runat="server" Text="固定成员" />
		</li>
		<li>
			<asp:HyperLink ID="lnkDynamic" runat="server" Text="条件成员" />
		</li>
		<li>
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
					<li class="pc-dropdownmenu"><span style="display: block; display: inline-block"><a
						runat="server" id="lnkNew" class="button pc-list-cmd" href="javascript:void(0)">
						添加成员<i class="pc-arrow"></i></a></span>
						<div style="position: relative; z-index: 9;">
							<div style="position: absolute">
								<ul class="pc-popup-nav">
									<li>
										<asp:LinkButton runat="server" ID="lnkAddAny" CssClass="shortcut" OnClientClick="return ($pc.getEnabled(this) && $pc.popups.browse('actionData'));"
											OnClick="AddMembersClick" OnPreRender="HandleMenuItemPreRender">从组织树</asp:LinkButton></li>
									<li>
										<asp:LinkButton runat="server" ID="lnkAddMember" CssClass="shortcut" OnClientClick="return ($pc.getEnabled(this) && $pc.popups.searchMember('actionData'));"
											OnClick="AddMembersClick" OnPreRender="HandleMenuItemPreRender">查找人员</asp:LinkButton>
									</li>
									<li>
										<asp:LinkButton runat="server" ID="lnkNewOrg" CssClass="shortcut" OnClientClick="return ($pc.getEnabled(this) && $pc.popups.searchGroup('actionData'));"
											OnClick="AddMembersClick" OnPreRender="HandleMenuItemPreRender">查找群组</asp:LinkButton>
									</li>
									<li>
										<asp:LinkButton runat="server" ID="lnkNewGroup" CssClass="shortcut" OnClientClick="return ($pc.getEnabled(this) && $pc.popups.searchOrg('actionData'));"
											OnClick="AddMembersClick" OnPreRender="HandleMenuItemPreRender">查找组织</asp:LinkButton>
									</li>
								</ul>
							</div>
						</div>
					</li>
					<li>
						<asp:LinkButton runat="server" CssClass="pc-list-cmd" ID="btnDeleteSelected" Text="删除"
							OnClientClick="return ($pc.getEnabled(this) && $pc.popups.batchDelete($find('gridMain')||$find('grid2'),'RoleMembers'));"
							OnClick="BatchRemove"></asp:LinkButton>
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
				<asp:HiddenField ID="actionData" runat="server" EnableViewState="False" />
				<asp:HiddenField ID="hfRoleID" runat="server" />
			</div>
			<div class="pc-grid-container">
				<asp:MultiView ActiveViewIndex="0" runat="server" ID="views">
					<asp:View runat="server">
						<mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
							AllowPaging="True" AllowSorting="True" ShowCheckBoxes="True" Category="PermissionCenter"
							GridTitle="角色固定人员" DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title"
							PagerSettings-Position="Bottom" DataSourceMaxRow="0" ShowExportControl="true"
							TitleColor="141, 143, 149" TitleFontSize="Large" OnRowCommand="HandleRowCommand"
							OnSelectAllCheckBoxClick="toggleMenu" OnSelectCheckBoxClick="toggleMenu">
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
										<div>
											<i class='<%# PermissionCenter.Util.CssSpritesFor(GetDataItem(),PermissionCenter.IconSizeType.Size16) %>'>
											</i>
											<%--<soa:UserPresence ID="uc1" runat="server" ShowUserIcon="false" UserID='<%#Eval("ID") %>'
												StatusImage="Ball" UserDisplayName='<%# Server.HtmlEncode((string)Eval("Name")) %>'
												Visible='<%# PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>' AccountDisabled='<%#0 !=(int)Eval("AccountDisabled") %>' />--%>
											<pc:SchemaHyperLink runat="server" ID="hl1" ObjectID='<%#Eval("ID") %>' ObjectSchemaType='<%#Eval("SchemaType") %>'
												Text='' CssClass="pc-item-link" data-id='<%# PermissionCenter.Util.HtmlAttributeEncode((string) Eval("ID")) %>'
												Target="_self"><%# Server.HtmlEncode((string) Eval("Name")) %></pc:SchemaHyperLink>
										</div>
										<div>
											<%# Server.HtmlEncode((string)Eval("DisplayName")) %>
										</div>
										<div>
											<div id="divActions" class="pc-action-tray" runat="server" visible='<%# this.gridMain.ExportingDeluxeGrid == false %>'>
												<asp:LinkButton ID="lnkEdit" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
												<asp:HyperLink ID="lnkAcl" runat="server" CssClass="pc-item-cmd" onclick="return $pc.modalPopup(this);"
													Target="_blank" Visible='<%#PermissionCenter.Util.IsOrganization((string)Eval("SchemaType")) %>'
													NavigateUrl='<%#Eval("ID","~/dialogs/AclEdit.aspx?id={0}") %>'>授权控制</asp:HyperLink>
												<asp:HyperLink ID="lnkOu" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													Target="_blank" NavigateUrl='<%# "~/lists/MemberOrgView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
													onclick="return $pc.modalPopup(this);" Visible='<%#PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>'>组织</asp:HyperLink>
												<asp:HyperLink ID="lnkGrp" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													Target="_blank" NavigateUrl='<%# "~/lists/MemberGrpView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
													onclick="return $pc.modalPopup(this);" Visible='<%#PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>'>群组</asp:HyperLink>
												<asp:HyperLink ID="lnkSecret" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													Target="_blank" NavigateUrl='<%# "~/lists/UserSecretary.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
													onclick="return $pc.modalPopup(this);" Visible='<%#PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>'>秘书</asp:HyperLink>
												<asp:HyperLink ID="lnkMemberRole" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													Target="_blank" NavigateUrl='<%# "~/lists/MemberRoleView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
													onclick="return $pc.modalPopup(this);" Visible='<%#PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>'>人员角色</asp:HyperLink>
												<asp:HyperLink ID="lnkMemberMatrix" runat="server" NavigateUrl='<%#Eval("ID","~/lists/MemberMatrix.aspx?id={0}") %>'
													onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Visible='<%#PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>'
													Target="_blank">查看角色矩阵</asp:HyperLink>
												<asp:HyperLink ID="lnkRP" runat="server" NavigateUrl='<%#Eval("ID","~/lists/FlowList.aspx?id={0}") %>'
													onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">查看相关流程</asp:HyperLink>
												<asp:HyperLink ID="lnkGropuMembers" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													Target="_blank" NavigateUrl='<%# "~/lists/GroupMembersView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
													onclick="return $pc.modalPopup(this);" Visible='<%# "Groups".Equals((string)Eval("SchemaType")) %>'>群组人员</asp:HyperLink>
												<asp:LinkButton ID="lnkItemDelete" runat="server" Text="删除" CssClass="pc-item-cmd"
													Enabled='<%# this.DeleteEnabled %>' CommandName="DeleteItem" CommandArgument='<%#Eval("ID") %>'
													OnClientClick="return ($pc.getEnabled(this) && confirm('确定要删除？'));"></asp:LinkButton>
												<asp:HyperLink ID="lnkHistory" runat="server" CssClass="pc-item-cmd" Target="_blank"
													onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/ObjectHistoryLog.aspx?id={0}") %>'>历史</asp:HyperLink>
											</div>
										</div>
										<div>
											<%# Server.HtmlEncode((string)Eval("CodeName")) %>
										</div>
									</ItemTemplate>
								</asp:TemplateField>
								<asp:BoundField DataField="SchemaType" HtmlEncode="true" HeaderText="类型" SortExpression="SchemaType" />
								<asp:TemplateField HeaderText="创建者" SortExpression="CreatorName">
									<ItemTemplate>
										<soa:UserPresence runat="server" ID="userPresence" ShowUserDisplayName="true" ShowUserIcon="false"
											UserID='<%#Eval("CreatorID") %>' UserDisplayName='<%# Server.HtmlEncode(Eval("CreatorName").ToString()) %>' />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:BoundField DataField="CreateDate" HtmlEncode="true" HeaderText="创建日期" SortExpression="CreateDate"
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
							GridTitle="角色固定人员" DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title"
							PagerSettings-Position="Bottom" DataSourceMaxRow="0" ShowExportControl="true"
							TitleColor="141, 143, 149" TitleFontSize="Large" OnRowCommand="HandleRowCommand"
							OnSelectAllCheckBoxClick="toggleMenu" OnSelectCheckBoxClick="toggleMenu">
							<EmptyDataTemplate>
								暂时没有您需要的数据
							</EmptyDataTemplate>
							<HeaderStyle CssClass="head" />
							<Columns>
								<asp:TemplateField HeaderText="名称" SortExpression="Name">
									<ItemTemplate>
										<div>
											<pc:SchemaHyperLink runat="server" ID="hl1" ObjectID='<%#Eval("ID") %>' ObjectSchemaType='<%#Eval("SchemaType") %>'
												Text='' CssClass="pc-item-link" data-id='<%# PermissionCenter.Util.HtmlAttributeEncode((string) Eval("ID")) %>'
												Target="_self"><i id="ico" runat="server" class='<%#Eval("SchemaType","pc-icon16 {0}") %>'></i><%# Server.HtmlEncode((string) Eval("Name")) %></pc:SchemaHyperLink>
										</div>
										<div>
											<div id="divActions" class="pc-action-tray" runat="server" visible='<%# this.grid2.ExportingDeluxeGrid == false %>'>
												<asp:LinkButton ID="lnkEdit" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
												<asp:HyperLink ID="lnkAcl" runat="server" CssClass="pc-item-cmd" onclick="return $pc.modalPopup(this);"
													Target="_blank" Visible='<%#PermissionCenter.Util.IsOrganization((string)Eval("SchemaType")) %>'
													NavigateUrl='<%#Eval("ID","~/dialogs/AclEdit.aspx?id={0}") %>'>授权控制</asp:HyperLink>
												<asp:HyperLink ID="lnkOu" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													Target="_blank" NavigateUrl='<%# "~/lists/MemberOrgView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
													onclick="return $pc.modalPopup(this);" Visible='<%#PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>'>组织</asp:HyperLink>
												<asp:HyperLink ID="lnkGrp" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													Target="_blank" NavigateUrl='<%# "~/lists/MemberGrpView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
													onclick="return $pc.modalPopup(this);" Visible='<%#PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>'>群组</asp:HyperLink>
												<asp:HyperLink ID="lnkSecret" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													Target="_blank" NavigateUrl='<%# "~/lists/UserSecretary.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
													onclick="return $pc.modalPopup(this);" Visible='<%#PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>'>秘书</asp:HyperLink>
												<asp:HyperLink ID="lnkMemberRole" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													Target="_blank" NavigateUrl='<%# "~/lists/MemberRoleView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
													onclick="return $pc.modalPopup(this);" Visible='<%#PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>'>人员角色</asp:HyperLink>
												<asp:HyperLink ID="lnkMemberMatrix" runat="server" NavigateUrl='<%#Eval("ID","~/lists/MemberMatrix.aspx?id={0}") %>'
													onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Visible='<%#PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>'
													Target="_blank">查看角色矩阵</asp:HyperLink>
												<asp:HyperLink ID="lnkRP" runat="server" NavigateUrl='<%#Eval("ID","~/lists/FlowList.aspx?id={0}") %>'
													onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">查看相关流程</asp:HyperLink>
												<asp:HyperLink ID="lnkGropuMembers" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													Target="_blank" NavigateUrl='<%# "~/lists/GroupMembersView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
													onclick="return $pc.modalPopup(this);" Visible='<%# "Groups".Equals((string)Eval("SchemaType")) %>'>群组人员</asp:HyperLink>
												<asp:LinkButton ID="lnkItemDelete" runat="server" Text="删除" CssClass="pc-item-cmd"
													Enabled='<%# this.DeleteEnabled %>' CommandName="DeleteItem" CommandArgument='<%#Eval("ID") %>'
													OnClientClick="return ($pc.getEnabled(this) && confirm('确定要删除？'));"></asp:LinkButton>
												<asp:HyperLink ID="lnkHistory" runat="server" CssClass="pc-item-cmd" Target="_blank"
													onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/ObjectHistoryLog.aspx?id={0}") %>'>历史</asp:HyperLink>
											</div>
										</div>
									</ItemTemplate>
								</asp:TemplateField>
								<asp:BoundField DataField="DisplayName" HtmlEncode="true" HeaderText="显示名称" SortExpression="DisplayName" />
								<asp:BoundField DataField="CodeName" HtmlEncode="true" HeaderText="代码名称" SortExpression="CodeName" />
								<asp:BoundField DataField="SchemaType" HtmlEncode="true" HeaderText="类型" SortExpression="SchemaType" />
								<asp:TemplateField HeaderText="创建者" SortExpression="CreatorName">
									<ItemTemplate>
										<soa:UserPresence runat="server" ID="userPresence" ShowUserDisplayName="true" ShowUserIcon="false"
											UserID='<%#Eval("CreatorID") %>' UserDisplayName='<%# Server.HtmlEncode(Eval("CreatorName").ToString()) %>' />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:BoundField DataField="CreateDate" HtmlEncode="true" HeaderText="创建日期" SortExpression="CreateDate"
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
		TypeName="MCS.Library.SOA.DataObjects.Security.DataSources.SchemaRoleMembersDataSource"
		EnableViewState="false" OnSelecting="dataSourceMain_Selecting">
		<SelectParameters>
			<asp:QueryStringParameter QueryStringField="role" Type="String" Name="roleId" DefaultValue="773BA455-6A2E-4A71-BDC7-AFE689789390" />
		</SelectParameters>
	</soa:DeluxeObjectDataSource>
	<soa:UploadProgressControl runat="server" ID="ctlUpload" DialogHeaderText="导入角色固定成员数据(xml)"
		DialogTitle="导入" OnDoUploadProgress="DoFileUpload" OnLoadingDialogContent="ctlUpload_LoadingDialogContent"
		OnClientCompleted="postImportProcess" />
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
					$pc.postViaIFrame($pc.appRoot + "Handlers/ObjectExport.ashx", { context: "AppRoleMembers", memberIds: keys, roleId: document.getElementById('hfRoleID').value });
				}
			}

			grid = false;
			return false;
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
