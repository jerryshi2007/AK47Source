<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OUExplorerView.aspx.cs"
	Inherits="PermissionCenter.OUExplorerView" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>组织成员</title>
	<base target="_self" />
	<pc:HeaderControl runat="server" />
</head>
<body>
	<form id="form1" runat="server">
	<soa:DataBindingControl runat="server" ID="binding1">
		<ItemBindings>
			<soa:DataBindingItem DataPropertyName="DisplayName" ControlID="gridMain" ControlPropertyName="GridTitle"
				Direction="DataToControl" />
			<soa:DataBindingItem DataPropertyName="DisplayName" ControlID="grid2" ControlPropertyName="GridTitle"
				Direction="DataToControl" />
			<soa:DataBindingItem DataPropertyName="ID" ControlID="hfOuId" ControlPropertyName="Value"
				Direction="DataToControl" />
			<soa:DataBindingItem DataPropertyName="ID" ControlID="ctlUpload" ControlPropertyName="Tag"
				Direction="DataToControl" />
			<soa:DataBindingItem DataPropertyName="ID" ControlID="ctlFullUpload" ControlPropertyName="Tag"
				Direction="DataToControl" />
			<soa:DataBindingItem DataPropertyName="ID" ControlID="deleteProgress" ControlPropertyName="Tag"
				Direction="DataToControl" />
			<soa:DataBindingItem DataPropertyName="ID" ControlID="deleteProgress2" ControlPropertyName="Tag"
				Direction="DataToControl" />
			<soa:DataBindingItem DataPropertyName="ID" ControlID="deleteFullProgress" ControlPropertyName="Tag"
				Direction="DataToControl" />
			<soa:DataBindingItem DataPropertyName="ID" ControlID="topNav" ControlPropertyName="ObjectID"
				Direction="DataToControl" />
		</ItemBindings>
	</soa:DataBindingControl>
	<pc:BannerNotice ID="notice" runat="server" />
	<asp:ScriptManager runat="server" EnableScriptGlobalization="true" EnablePageMethods="true"
		ID="scriptMan1">
	</asp:ScriptManager>
	<pc:SceneControl ID="SceneControl1" runat="server">
	</pc:SceneControl>
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
	<div class="">
		<div class="pc-nav-path">
			<soa:OuNavigator ID="topNav" runat="server" StartLevel="0" CssClass="pc-nav-path-content"
				LinkCssClass="pc-nav-path-node" SplitterVisible="false" OnClientLinkClick="return nav(this);"
				Target="_parent" LinkDataTag="data-key" NavigateUrlFormat="~/lists/OUExplorer.aspx?ou={0}"
				RootVisible="false" TerminalVisible="true"></soa:OuNavigator>
		</div>
	</div>
	<div class="pc-container5">
		<div class="pc-listmenu-container">
			<ul class="pc-listmenu" id="listMenu">
				<li class="pc-dropdownmenu"><span style="display: block; display: inline-block">
					<asp:LinkButton runat="server" ID="lnkNew" CssClass="button pc-list-cmd" OnClientClick="return ($pc.getEnabled(this) && $pc.popups.newMember(this));"
						OnClick="RefreshList" OnPreRender="HandleMenuItemPreRender">新建人员<i class="pc-arrow"></i></asp:LinkButton>
				</span>
					<div style="position: relative; z-index: 9">
						<div style="position: absolute;">
							<asp:Repeater ID="shortCuts" runat="server" DataMember="users" DataSourceID="SchemaDataSource1">
								<HeaderTemplate>
									<ul class="pc-popup-nav">
								</HeaderTemplate>
								<ItemTemplate>
									<li>
										<asp:LinkButton ID="lb1" runat="server" CssClass="shortcut" data-schema='<%#PermissionCenter.Util.HtmlAttributeEncode((string) Eval("Name")) %>'
											OnClick="RefreshList" OnClientClick="return $pc.popups.newMember(this);" OnPreRender="HandleMenuItemPreRender"><%# Server.HtmlEncode((string)Eval("Description")) %></asp:LinkButton>
									</li>
								</ItemTemplate>
								<FooterTemplate>
									</ul>
								</FooterTemplate>
							</asp:Repeater>
							<pc:SchemaDataSource ID="SchemaDataSource1" runat="server">
							</pc:SchemaDataSource>
						</div>
					</div>
				</li>
				<li>
					<asp:LinkButton runat="server" ID="lnkAddExistsMembers" CssClass="button pc-list-cmd"
						OnClientClick="return ($pc.getEnabled(this) && $pc.popups.searchMember('actionData'));"
						OnClick="AddExistMembers" OnPreRender="HandleMenuItemPreRender">添加现有人员</asp:LinkButton>
				</li>
				<li>
					<asp:LinkButton runat="server" ID="lnkNewOrg" CssClass="button pc-list-cmd" OnClientClick="return ($pc.getEnabled(this) && $pc.popups.newOrg(this));"
						OnClick="RefreshOwnerTree" OnPreRender="HandleMenuItemPreRender">新建组织</asp:LinkButton>
				</li>
				<li>
					<asp:LinkButton runat="server" ID="lnkNewGroup" CssClass="button pc-list-cmd" OnClientClick="return ($pc.getEnabled(this) && $pc.popups.newGrp(this));"
						OnClick="RefreshList" OnPreRender="HandleMenuItemPreRender">新建群组</asp:LinkButton>
				</li>
				<li>
					<asp:LinkButton runat="server" ID="lnkProperties" CssClass="button pc-list-cmd pc-hide"
						OnClientClick="return ($pc.getEnabled(this) && !!showBatchEditor());" OnClick="RefreshList"
						OnPreRender="HandleMenuItemPreRender">属性</asp:LinkButton>
				</li>
				<li>
					<asp:HyperLink NavigateUrl="javascript:void(0);" CssClass="pc-list-cmd pc-hide" runat="server"
						ID="transfer" onclick="return $pc.getEnabled(this) && preTransfer();">复制/移动</asp:HyperLink>
					<asp:LinkButton ID="btnTransfer" runat="server" OnClick="BatchTransfer" Style="display: none;"></asp:LinkButton>
					<asp:LinkButton ID="btnTransferTriggr" runat="server" Style="display: none"></asp:LinkButton>
				</li>
				<li class="pc-dropdownmenu"><span style="display: block; display: inline-block"><a
					runat="server" id="A1" class="button pc-list-cmd" href="javascript:void(0)">删除<i
						class="pc-arrow"></i></a></span>
					<div style="position: relative; z-index: 9;">
						<div style="position: absolute">
							<ul class="pc-popup-nav">
								<li>
									<asp:LinkButton runat="server" CssClass="pc-list-cmd" ID="btnDeleteSelected" Text="常规删除"
										OnClick="RefreshOwnerTree"></asp:LinkButton>
								</li>
								<li>
									<asp:LinkButton runat="server" CssClass="pc-list-cmd" ID="btnDeleteSelectedFull"
										ToolTip="如果选择了人员，则将人员从权限中心删除。不含选定组织中的人员。" Text="完全删除" OnClick="RefreshOwnerTree"></asp:LinkButton>
								</li>
							</ul>
						</div>
					</div>
				</li>
				<li class="pc-dropdownmenu"><span style="display: block; display: inline-block;">
					<asp:LinkButton runat="server" CssClass="list-cmd" ID="btnImport" OnClientClick="return ($pc.getEnabled(this) && invokeImport());"
						OnClick="RefreshList">导入<i class="pc-arrow"></i></asp:LinkButton></span>
					<div style="position: relative; z-index: 9">
						<div style="position: absolute;">
							<ul class="pc-popup-nav">
								<li>
									<asp:LinkButton runat="server" CssClass="list-cmd" ID="btnImportThis" OnClientClick="return ($pc.getEnabled(this) && invokeImport() && false);"
										OnClick="RefreshList">导入直接子对象（默认）</asp:LinkButton>
								</li>
								<li>
									<asp:LinkButton runat="server" CssClass="list-cmd" ID="btnImportAll" OnClientClick="return ($pc.getEnabled(this) && invokeImportFull() && false);"
										OnClick="RefreshList">导入各级子对象</asp:LinkButton>
								</li>
							</ul>
						</div>
					</div>
				</li>
				<li class="pc-dropdownmenu"><span style="display: block; display: inline-block;">
					<asp:LinkButton runat="server" CssClass="list-cmd" ID="btnExport" OnClientClick="return ($pc.getEnabled(this) && invokeExport() && false);">导出<i class="pc-arrow"></i></asp:LinkButton>
				</span>
					<div style="position: relative; z-index: 9">
						<div style="position: absolute;">
							<ul class="pc-popup-nav">
								<li>
									<asp:LinkButton runat="server" CssClass="list-cmd" ID="btnExportThis" OnClientClick="return ($pc.getEnabled(this) && invokeExport() && false);">导出选定的对象（默认）</asp:LinkButton>
								</li>
								<li>
									<asp:LinkButton runat="server" CssClass="list-cmd" ID="btnExportFull" OnClientClick="return ($pc.getEnabled(this) && invokeExportSelectedFull() && false);">导出选定的对象及子对象</asp:LinkButton>
								</li>
								<li>
									<asp:LinkButton runat="server" CssClass="list-cmd" ID="btnExportFull2" OnClientClick="return ($pc.getEnabled(this) && invokeExportFull() && false);">导出全部对象及子对象</asp:LinkButton>
								</li>
							</ul>
						</div>
					</div>
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
				<li class="pc-dropdownmenu" style="float: right"><span style="display: block; display: inline-block">
					<asp:HyperLink ID="lnkDisplay" runat="server" CssClass="shortcut" NavigateUrl="javascript:void(0);"
						Style="margin-right: 0; cursor: default">
						显示：<span runat="server" id="displayFilter">所有类别</span><i class="pc-arrow"></i>
					</asp:HyperLink>
				</span>
					<div style="z-index: 9;" class="pc-popup-nav-pan pc-right">
						<div class="pc-popup-nav-wrapper">
							<ul class="pc-popup-nav" style="text-align: left; color: #ffffff;">
								<li>
									<asp:CheckBox runat="server" ID="filterUsers" Text="人员" CssClass="shortcut" Checked="true"
										OnCheckedChanged="ToggleVisibleClick" AutoPostBack="true" />
								</li>
								<li>
									<asp:CheckBox runat="server" ID="filterGroups" Text="群组" CssClass="shortcut" Checked="true"
										OnCheckedChanged="ToggleVisibleClick" AutoPostBack="true" />
								</li>
								<li>
									<asp:CheckBox runat="server" ID="filterOrgs" Text="组织" CssClass="shortcut" Checked="true"
										OnCheckedChanged="ToggleVisibleClick" AutoPostBack="true" />
								</li>
							</ul>
						</div>
					</div>
				</li>
			</ul>
		</div>
		<div style="display: none">
			<input type="hidden" id="actionData" runat="server" />
			<input type="hidden" id="actionType" runat="server" />
			<asp:HiddenField ID="hfOuId" runat="server" EnableViewState="false" />
			<asp:HiddenField ID="hfOuParentId" runat="server" />
			<input type="hidden" id="hfPReload" runat="server" value="" />
			<asp:Button runat="server" ID="actionButton" OnClick="BatchTransfer" />
			<asp:Button Text="" ID="btItemDeleteTrigger" runat="server" OnClick="RefreshList" />
			<input type="hidden" id="hfDeleteEnabled" runat="server" />
		</div>
		<div class="pc-grid-container">
			<asp:MultiView ActiveViewIndex="0" runat="server" ID="views">
				<asp:View runat="server">
					<mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
						AllowPaging="True" AllowSorting="True" ShowCheckBoxes="True" Category="PermissionCenter"
						ShowExportControl="true" GridTitle="组织" DataKeyNames="ID" CssClass="dataList pc-datagrid"
						TitleCssClass="title" PagerSettings-Position="Bottom" DataSourceMaxRow="0" TitleColor="141, 143, 149"
						TitleFontSize="Large" OnRowCommand="gridMain_RowCommand" OnSelectAllCheckBoxClick="toggleMenu"
						OnSelectCheckBoxClick="toggleMenu">
						<EmptyDataTemplate>
							暂时没有您需要的数据
						</EmptyDataTemplate>
						<HeaderStyle CssClass="head" />
						<Columns>
							<asp:TemplateField HeaderText="名称">
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
									<div class="pc-photo-container" style="float: left;">
										<soa:UserPresence runat="server" ID="uc1" StatusImage="LongBar" ShowUserIcon="true"
											Visible='<%# PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>' UserID='<%#Eval("ID") %>'
											UserIconUrl='<%# this.ResolveClientUrl(Eval("ID","~/Handlers/UserPhoto.ashx?id={0}")) %>'
											AccountDisabled='<%#0 !=(int)Eval("AccountDisabled") %>' />
										<div id="Div1" class="uc-user-container-long" runat="server" visible='<%# false == PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>'>
											<i id="i1" class='<%# PermissionCenter.Util.CssSpritesFor(GetDataItem(),PermissionCenter.IconSizeType.Size32) %>'>
											</i>
										</div>
									</div>
									<div>
										<pc:SchemaHyperLink runat="server" ID="hl2" Text='<%# Server.HtmlEncode((string) Eval("Name")) %>'
											OUViewMode="true" ObjectID='<%#Eval("ID") %>' ObjectSchemaType='<%#Eval("SchemaType") %>'
											CssClass="pc-item-link" data-id='<%# PermissionCenter.Util.HtmlAttributeEncode((string) Eval("ID")) %>'
											Target="_self" />
									</div>
									<div>
										<%# Server.HtmlEncode((string)Eval("DisplayName")) %>
									</div>
									<div>
										<div class="pc-action-tray" runat="server" visible='<%# this.gridMain.ExportingDeluxeGrid == false %>'>
											<asp:LinkButton ID="lnkEdit1" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
												OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshOwnerTree">基本属性</asp:LinkButton>
											<asp:HyperLink ID="lnkAcl1" runat="server" CssClass="pc-item-cmd" onclick="return $pc.modalPopup(this);"
												Target="_blank" Visible='<%#PermissionCenter.Util.IsOrganization((string)Eval("SchemaType")) %>'
												NavigateUrl='<%#Eval("ID","~/dialogs/AclEdit.aspx?id={0}") %>'>授权控制</asp:HyperLink>
											<asp:HyperLink ID="lnkOu1" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
												Target="_blank" NavigateUrl='<%# "~/lists/MemberOrgView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
												onclick="return $pc.modalPopup(this);" Visible='<%#PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>'>组织</asp:HyperLink>
											<asp:HyperLink ID="lnkGrp1" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
												Target="_blank" NavigateUrl='<%# "~/lists/MemberGrpView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
												onclick="return $pc.modalPopup(this);" Visible='<%#PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>'>群组</asp:HyperLink>
											<asp:HyperLink ID="lnkSecret1" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
												Target="_blank" NavigateUrl='<%# "~/lists/UserSecretary.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
												onclick="return $pc.modalPopup(this);" Visible='<%#PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>'>秘书</asp:HyperLink>
											<asp:HyperLink ID="lnkMemberRole1" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
												Target="_blank" NavigateUrl='<%# "~/lists/MemberRoleView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
												onclick="return $pc.modalPopup(this);" Visible='<%#PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>'>人员角色</asp:HyperLink>
											<asp:HyperLink ID="lnkMemberMatrix1" runat="server" NavigateUrl='<%#Eval("ID","~/lists/MemberMatrix.aspx?id={0}") %>'
												onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Visible='<%#PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>'
												Target="_blank">查看角色矩阵</asp:HyperLink>
											<asp:HyperLink ID="lnkRP" runat="server" NavigateUrl='<%#Eval("ID","~/lists/FlowList.aspx?id={0}") %>'
												onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">查看相关流程</asp:HyperLink>
											<asp:HyperLink ID="lnkGropuMembers1" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
												Target="_blank" NavigateUrl='<%# "~/lists/GroupMembersView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
												onclick="return $pc.modalPopup(this);" Visible='<%# "Groups".Equals((string)Eval("SchemaType")) %>'>群组人员</asp:HyperLink>
											<asp:LinkButton ID="lnkItemToTop1" runat="server" Text="移到顶部" CssClass="pc-item-cmd"
												Enabled='<%# this.UpdateEnabled %>' CommandName="MoveTop" OnClientClick="return $pc.getEnabled(this);"
												CommandArgument='<%#Eval("ID") %>'></asp:LinkButton>
											<asp:LinkButton ID="lnkItemUp1" runat="server" Text="上移" CssClass="pc-item-cmd" CommandName="MoveUp"
												Enabled='<%# this.UpdateEnabled %>' OnClientClick="return $pc.getEnabled(this);"
												CommandArgument='<%#Eval("ID") %>'></asp:LinkButton>
											<asp:LinkButton ID="lnkItemDown1" runat="server" Text="下移" CssClass="pc-item-cmd"
												Enabled='<%# this.UpdateEnabled %>' OnClientClick="return $pc.getEnabled(this);"
												CommandName="MoveDown" CommandArgument='<%#Eval("ID") %>'></asp:LinkButton>
											<asp:LinkButton ID="lnkItemToBottom1" runat="server" Text="移至底部" CssClass="pc-item-cmd"
												Enabled='<%# this.UpdateEnabled %>' CommandName="MoveBottom" OnClientClick="return $pc.getEnabled(this);"
												CommandArgument='<%#Eval("ID") %>'></asp:LinkButton>
											<asp:LinkButton ID="lnkItemDelete1" runat="server" Text="删除" CssClass="pc-item-cmd"
												data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
												Enabled='<%# this.DeleteEnabled %>' OnClientClick="return ($pc.getEnabled(this)) && prepareItemToDelete(this);"></asp:LinkButton>
											<asp:HyperLink ID="lnkHistory1" runat="server" CssClass="pc-item-cmd" Target="_blank"
												onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/ObjectHistoryLog.aspx?id={0}") %>'>历史</asp:HyperLink>
										</div>
									</div>
									<div>
										<%# Server.HtmlEncode((string)Eval("CodeName")) %>
									</div>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="类型" SortExpression="O.SchemaType">
								<ItemTemplate>
									<%# Server.HtmlEncode(this.SchemaTypeToString((string)Eval("SchemaType"))) %>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="创建者" SortExpression="O.CreatorName">
								<ItemTemplate>
									<soa:UserPresence runat="server" ID="userPresence" ShowUserDisplayName="true" ShowUserIcon="false"
										UserID='<%#Eval("CreatorID") %>' UserDisplayName='<%# Server.HtmlEncode(Eval("CreatorName").ToString()) %>' />
								</ItemTemplate>
							</asp:TemplateField>
							<asp:BoundField DataField="CreateDate" HtmlEncode="true" HeaderText="创建日期" SortExpression="O.CreateDate"
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
						ShowExportControl="true" GridTitle="组织" DataKeyNames="ID" CssClass="dataList pc-datagrid"
						TitleCssClass="title" PagerSettings-Position="Bottom" DataSourceMaxRow="0" TitleColor="141, 143, 149"
						TitleFontSize="Large" OnRowCommand="gridMain_RowCommand" OnSelectAllCheckBoxClick="toggleMenu"
						OnSelectCheckBoxClick="toggleMenu">
						<EmptyDataTemplate>
							暂时没有您需要的数据
						</EmptyDataTemplate>
						<HeaderStyle CssClass="head" />
						<Columns>
							<asp:TemplateField HeaderText="名称" SortExpression="O.Name">
								<ItemTemplate>
									<div>
										<i id="ico" runat="server" class='<%# PermissionCenter.Util.CssSpritesFor(this.GetDataItem(), PermissionCenter.IconSizeType.Size16) %>'>
										</i>
										<soa:UserPresence runat="server" ID="uc2" StatusImage="Ball" UserID='<%#Eval("ID") %>'
											Visible='<%#PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>' AccountDisabled='<%#0 !=(int)Eval("AccountDisabled") %>' />
										<pc:SchemaHyperLink runat="server" ID="hl22" Text='<%# Server.HtmlEncode((string) Eval("Name")) %>'
											OUViewMode="true" ObjectID='<%#Eval("ID") %>' ObjectSchemaType='<%#Eval("SchemaType") %>'
											CssClass="pc-item-link" data-id='<%# PermissionCenter.Util.HtmlAttributeEncode((string) Eval("ID")) %>'
											Target="_self" />
									</div>
									<div>
										<div id="divActions" class="pc-action-tray" runat="server" visible='<%# this.grid2.ExportingDeluxeGrid == false %>'>
											<asp:LinkButton ID="lnkEdit" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
												OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshOwnerTree">基本属性</asp:LinkButton>
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
											<asp:LinkButton ID="lnkItemToTop" runat="server" Text="移到顶部" CssClass="pc-item-cmd"
												Enabled='<%# this.UpdateEnabled %>' CommandName="MoveTop" OnClientClick="return $pc.getEnabled(this);"
												CommandArgument='<%#Eval("ID") %>'></asp:LinkButton>
											<asp:LinkButton ID="lnkItemUp" runat="server" Text="上移" CssClass="pc-item-cmd" CommandName="MoveUp"
												Enabled='<%# this.UpdateEnabled %>' OnClientClick="return $pc.getEnabled(this);"
												CommandArgument='<%#Eval("ID") %>'></asp:LinkButton>
											<asp:LinkButton ID="lnkItemDown" runat="server" Text="下移" CssClass="pc-item-cmd"
												Enabled='<%# this.UpdateEnabled %>' OnClientClick="return $pc.getEnabled(this);"
												CommandName="MoveDown" CommandArgument='<%#Eval("ID") %>'></asp:LinkButton>
											<asp:LinkButton ID="lnkItemToBottom" runat="server" Text="移至底部" CssClass="pc-item-cmd"
												Enabled='<%# this.UpdateEnabled %>' CommandName="MoveBottom" OnClientClick="return $pc.getEnabled(this);"
												CommandArgument='<%#Eval("ID") %>'></asp:LinkButton>
											<asp:LinkButton ID="lnkItemDelete" runat="server" Text="删除" CssClass="pc-item-cmd"
												data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
												Enabled='<%# this.DeleteEnabled %>' OnClientClick="return ($pc.getEnabled(this)) && prepareItemToDelete(this);"></asp:LinkButton>
											<asp:HyperLink ID="lnkHistory" runat="server" CssClass="pc-item-cmd" Target="_blank"
												onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/ObjectHistoryLog.aspx?id={0}") %>'>历史</asp:HyperLink>
										</div>
									</div>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:BoundField HeaderText="显示名称" DataField="DisplayName" HtmlEncode="true" SortExpression="O.DisplayName" />
							<asp:BoundField HeaderText="代码名称" DataField="CodeName" HtmlEncode="true" SortExpression="O.CodeName" />
							<asp:TemplateField HeaderText="类型" SortExpression="O.SchemaType">
								<ItemTemplate>
									<%# Server.HtmlEncode(this.SchemaTypeToString((string)Eval("SchemaType"))) %>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="创建者" SortExpression="O.CreatorName">
								<ItemTemplate>
									<soa:UserPresence runat="server" ID="userPresence1" ShowUserDisplayName="true" ShowUserIcon="false"
										UserID='<%#Eval("CreatorID") %>' UserDisplayName='<%# Server.HtmlEncode(Eval("CreatorName").ToString()) %>' />
								</ItemTemplate>
							</asp:TemplateField>
							<asp:BoundField DataField="CreateDate" HtmlEncode="true" HeaderText="创建日期" SortExpression="O.CreateDate"
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
	<soa:DeluxeObjectDataSource ID="dataSourceMain" runat="server" EnablePaging="True"
		TypeName="MCS.Library.SOA.DataObjects.Security.DataSources.SchemaOrgChildrenDataSource"
		EnableViewState="false" OnSelecting="dataSourceMain_Selecting">
		<SelectParameters>
			<asp:QueryStringParameter QueryStringField="ou" Type="String" Name="ou" DefaultValue="773BA455-6A2E-4A71-BDC7-AFE689789390" />
		</SelectParameters>
	</soa:DeluxeObjectDataSource>
	<pc:Footer ID="footer" runat="server" />
	<soa:UploadProgressControl runat="server" ID="ctlUpload" DialogHeaderText="导入组织机构数据(xml)"
		DialogTitle="导入" OnDoUploadProgress="DoFileUpload" OnLoadingDialogContent="ctlUpload_LoadingDialogContent"
		OnClientCompleted="postImportProcess" />
	<soa:UploadProgressControl runat="server" ID="ctlFullUpload" DialogHeaderText="导入深度组织机构数据(xml)"
		DialogTitle="导入" OnDoUploadProgress="DoDeepFileUpload" OnLoadingDialogContent="ctlFullUpload_LoadingDialogContent"
		OnClientCompleted="postImportProcess" />
	<soa:PostProgressControl runat="server" ID="deleteProgress" DialogTitle="批量删除" DialogHeaderText="删除进度..."
		OnClientCompleted="onDeleteCompleted" ControlIDToShowDialog="btnDeleteSelected"
		OnClientBeforeStart="prepareDataForDelete" OnDoPostedData="DoDeleteProgress" />
	<soa:PostProgressControl runat="server" ID="deleteProgress2" DialogTitle="单条删除" DialogHeaderText="删除进度..."
		OnClientCompleted="onDeleteCompleted" ControlIDToShowDialog="btItemDeleteTrigger"
		OnClientBeforeStart="prepareDataForDelete2" OnDoPostedData="DoDeleteProgress2" />
	<soa:PostProgressControl runat="server" ID="deleteFullProgress" DialogTitle="完全删除"
		DialogHeaderText="删除进度..." OnClientCompleted="onDeleteCompleted" ControlIDToShowDialog="btnDeleteSelectedFull"
		OnClientBeforeStart="prepareDataForDelete" OnDoPostedData="DoDeleteProgressFull" />
	<soa:PostProgressControl runat="server" ID="transferProgress" DialogTitle="执行复制/移动..."
		DialogHeaderText="复制移动..." OnClientCompleted="doTransferComplete" ControlIDToShowDialog="btnTransferTriggr"
		OnClientBeforeStart="prepareDataForTransfer" OnDoPostedData="DoTransferProgress" />
	<script type="text/javascript">
		function onconditionClick(sender, e) {
			var content = Sys.Serialization.JavaScriptSerializer.deserialize(e.ConditionContent);
			var bindingControl = $find("searchBinding");
			bindingControl.dataBind(content);
		}

		function toggleMenu() {
			var grid = $find("gridMain") || $find("grid2");
			if (grid) {
				if (grid.get_clientSelectedKeys().length) {
					$pc.removeClass("lnkProperties", "pc-hide");
					$pc.removeClass("transfer", "pc-hide");
				} else {
					$pc.addClass("lnkProperties", "pc-hide");
					$pc.addClass("transfer", "pc-hide");
				};
			} else {
				$pc.console.error("未加载grid");
			}

			grid = null;
		}

		function showBatchEditor() {
			return $pc.popups.batchProperties($find('gridMain') || $find('grid2'));
		}

		function nav(elem) {
			var key;
			if (elem.getAttribute) {
				key = elem.getAttribute("data-key");
			} else {
				key = elem['data-key'];
			}
			if (key) {
				if (key.length > 0) {
					window.location.href = "OUExplorerView.aspx?ou=" + encodeURIComponent(key);
					return false;
				}
			}
			return true;
		}
		if (window.parent.showLoader)
			window.parent.showLoader(false);
		$pc.ui.gridBehavior("gridMain", "hover");
		$pc.ui.gridBehavior("grid2", "hover");
		$pc.ui.listMenuBehavior("listMenu");

		function invokeImport() {
			var result = $find("ctlUpload").showDialog()
			if (result)
				return true;
			return false;
		}

		function invokeImportFull() {
			var result = $find("ctlFullUpload").showDialog()
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
					$pc.postViaIFrame($pc.appRoot + "Handlers/ObjectExport.ashx", { context: "OguObjects", id: keys, parentId: document.getElementById("hfOuId").value });
				}
			}
			grid = false;
			return false;
		}

		function invokeExportFull() {
			if (confirm('进行全部导出时，可能需要很长的数据准备时间，是否继续？')) {
				$pc.postViaIFrame($pc.appRoot + "Handlers/ObjectExport.ashx", { context: "OguObjectsFull", parentId: document.getElementById("hfOuId").value });
			}

			return false;
		}

		function invokeExportSelectedFull() {
			var grid = $find("gridMain") || $find("grid2");
			if (grid) {
				var keys = grid.get_clientSelectedKeys();
				if (keys.length > 0) {
					if (confirm('导出包含子对象时，可能需要很长的数据准备时间，是否继续？')) {
						$pc.postViaIFrame($pc.appRoot + "Handlers/ObjectExport.ashx", { context: "OguObjectsFull", parentId: document.getElementById("hfOuId").value, id: keys });
					}
				}
			}
			grid = false;
			return false;
		}

		var dataToTransfer = null;

		function preTransfer() {
			var result = false;
			var keys = ($find("gridMain") || $find("grid2")).get_clientSelectedKeys();
			if (keys.length > 0) {
				var enableDelete = document.getElementById("hfDeleteEnabled").value == "1";
				$pc.console.info("移动权限：" + enableDelete);
				result = $pc.showDialog($pc.appRoot + (enableDelete ? "dialogs/MoveRouter.htm?v=1" : "dialogs/MoveRouter.htm?v=1&nomove=1"), keys, null, false, 800, 600, true);
				if (result) {
					var r = undefined;
					switch (result) {
						case 'UserCopyToGroup':
							r = $pc.popups.searchGroup('actionData', 1, [{ "pp": "EditMembersOfGroups"}]);
							break;
						case 'UserCopyToOrg':
							r = $pc.popups.searchOrg('actionData', 0, [{ "permission": "AddChildren", "exceptOrg": $get("hfOuId").value}]);
							break
						case 'UserMoveToOrg':
							r = $pc.popups.searchOrg('actionData', 1, [{ "permission": "AddChildren", "exceptOrg": $get("hfOuId").value}]);
							break;
						case 'GroupMoveToOrg':
							r = $pc.popups.searchOrg('actionData', 1, [{ "permission": "AddChildren", "exceptOrg": $get("hfOuId").value}]);
							break;
						case 'OrgTransfer':
							r = $pc.popups.searchOrg('actionData', 1, [{ "permission": "AddChildren", "exceptOrg": $get("hfOuId").value}], keys);
							break;
						case "MixedToOrg":
							r = $pc.popups.searchOrg('actionData', 1, [{ "permission": "AddChildren", "exceptOrg": $get("hfOuId").value}], keys);
							break;
					}
					if (r == true) {
						//$get("actionType").value = result;
						dataToTransfer = { orgKey: $get("hfOuId").value, actionType: result, srcKeys: keys, targetKeys: $get("actionData").value.split(",") };
						$get("actionData").value = Sys.Serialization.JavaScriptSerializer.serialize(dataToTransfer);
						$get("btnTransferTriggr").click();
					}
				}
			}

			return false;
		}

		function refreshOwnerTree() {
			if (window.parent.reloadNode) {
				window.parent.reloadNode.call(window.parent, document.getElementById("hfOuId").value);
			}
		}

		function prepareItemToDelete(elem) {
			var id = $pc.getAttr(elem, "data-id");
			if (typeof (id) == 'string' && id.length > 0) {
				$get("actionData").value = id;
				$get("btItemDeleteTrigger").click();
			}
			return false;
		}
		function prepareDataForDelete(e) {
			e.steps = ($find("gridMain") || $find("grid2")).get_clientSelectedKeys();
			if (e.steps.length > 0) {
				e.cancel = !confirm(e.steps.length === 1 ? "确实要删除这个项目？" : "确实要删除选定的项目？");
			}
		}

		function prepareDataForDelete2(e) {
			e.steps = [$get("actionData").value];
			if (e.steps.length > 0) {
				e.cancel = !confirm(e.steps.length === 1 ? "确实要删除这个项目？" : "确实要删除选定的项目？");
			}
		}

		function prepareDataForTransfer(e) {
			e.steps = [$get("actionData").value];
		}

		function onDeleteCompleted(e) {
			__doPostBack("btnDeleteSelected", "");
		}

		function doTransferComplete() {
			if (typeof window.parent.hardReload === 'function') {
				window.parent.parent.hardReload.call(window.parent);
			}
			window.location.href = window.location.href;
		}

		$pc.bindEvent(document.getElementById("form1"), "submit", function () {

			if (typeof (window.parent.showLoader) === 'function') {
				window.parent.showLoader.apply(window.parent, [1]);
			}
		});

		(function (w) {

			// 确保树和本节点一致
			if (w.parent.getCurrentNodeId) {
				var treeNodeId = w.parent.getCurrentNodeId();
				var elemOuId = document.getElementById("hfOuId");
				var elemParentOuId = document.getElementById("hfOuParentId");
				var elemPReload = document.getElementById("hfPReload");
				if (elemOuId != null && elemParentOuId != null && elemPReload != null) {
					var key = elemOuId.value;
					var parentKey = elemParentOuId.value;
					if (key != treeNodeId && typeof (key) === 'string' && key.length > 0) {
						if (w.parent.selectNode) {
							w.parent.selectNode.call(w.parent, key, (elemPReload.value == '1'), parentKey);
						}
					}
				}

				elemOuId = null;
				elemParentOuId = null;
				elemPReload = null;
				treeNodeId = null;
			}
		})(window);

		Sys.Application.add_load(function () {
			toggleMenu();
		});
	</script>
	</form>
</body>
</html>
