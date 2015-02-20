<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GroupConstMembers.aspx.cs"
	Inherits="PermissionCenter.GroupConstMembers" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" scroll="no" style="overflow: hidden">
<head id="Head1" runat="server">
	<title>权限中心-群组固定人员</title>
	<link rel="icon" href="../favicon.ico" type="image/x-icon" />
	<link rel="Shortcut Icon" href="../favicon.ico" />
	<base target="_self" />
	<pc:HeaderControl runat="server">
	</pc:HeaderControl>
</head>
<body>
	<form id="form1" runat="server">
	<soa:DataBindingControl runat="server" ID="binding1">
		<ItemBindings>
			<soa:DataBindingItem ControlID="ctlUpload" ControlPropertyName="Tag" DataPropertyName="ID"
				Direction="DataToControl" />
			<soa:DataBindingItem DataPropertyName="ID" ControlID="hfGroupID" ControlPropertyName="Value" />
			<soa:DataBindingItem DataPropertyName="VisibleName" ControlID="groupName" ControlPropertyName="Text"
				Direction="DataToControl" />
			<soa:DataBindingItem DataPropertyName="ID" ControlID="lnkConst" ControlPropertyName="NavigateUrl"
				Format="~/lists/GroupConstMembers.aspx?id={0}" Direction="DataToControl" />
			<soa:DataBindingItem DataPropertyName="ID" ControlID="lnkDynamic" ControlPropertyName="NavigateUrl"
				Format="~/lists/GroupDynamicMembers.aspx?id={0}" Direction="DataToControl" />
			<soa:DataBindingItem DataPropertyName="ID" ControlID="lnkPreview" ControlPropertyName="NavigateUrl"
				Format="~/lists/GroupMembersView.aspx?id={0}" Direction="DataToControl" />
		</ItemBindings>
	</soa:DataBindingControl>
	<pc:SceneControl ID="SceneControl1" runat="server">
	</pc:SceneControl>
	<div class="pc-banner">
		<h1 class="pc-caption">
			群组人员管理-
			<asp:Literal ID="groupName" runat="server" Mode="Encode"></asp:Literal><span class="pc-timepointmark"><mcs:TimePointDisplayControl
				ID="TimePointDisplayControl1" runat="server">
			</mcs:TimePointDisplayControl>
			</span>
		</h1>
	</div>
	<pc:BannerNotice ID="notice" runat="server"></pc:BannerNotice>
	<ul class="pc-tabs-header">
		<li class="pc-active">
			<asp:HyperLink ID="lnkConst" runat="server" Text="固定人员" />
		</li>
		<li>
			<asp:HyperLink ID="lnkDynamic" runat="server" Text="条件人员" />
		</li>
		<li>
			<asp:HyperLink ID="lnkPreview" runat="server" Text="预览人员" />
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
					<li class="pc-dropdownmenu"><span style="display: block; display: inline-block"><a
						runat="server" id="lnkAdd" class="button pc-list-cmd" href="javascript:void(0);">
						添加成员<i class="pc-arrow"></i></a></span>
						<div style="position: relative; z-index: 9;">
							<div style="position: absolute">
								<ul class="pc-popup-nav">
									<li>
										<asp:LinkButton runat="server" ID="lnkAddAny" CssClass="shortcut" OnClientClick="return ($pc.getEnabled(this) && $pc.popups.browse('actionData',['Users']));"
											OnClick="HandleAddUser" OnPreRender="HandleMenuItemPreRender">从组织树</asp:LinkButton></li>
									<li>
										<asp:LinkButton runat="server" ID="lnkAddMember" CssClass="button pc-list-cmd" OnClientClick="return ($pc.getEnabled(this) && $pc.popups.searchMember('actionData'));"
											OnPreRender="HandleMenuItemPreRender" OnClick="HandleAddUser">查找人员</asp:LinkButton>
									</li>
								</ul>
							</div>
						</div>
					</li>
					<li>
						<asp:LinkButton runat="server" ID="lnkProperties" CssClass="button pc-list-cmd pc-hide"
							OnClientClick="return ($pc.getEnabled(this) && !!showBatchEditor());" OnClick="RefreshList">属性</asp:LinkButton>
					</li>
					<li>
						<asp:LinkButton runat="server" CssClass="pc-list-cmd" ID="btnDeleteSelected" Text="删除"
							OnClientClick="return ($pc.getEnabled(this) && $pc.popups.batchDelete($find('gridMain')||$find('grid2'),'GroupConstMembers'));"
							OnClick="BatchDelete"></asp:LinkButton>
					</li>
					<li>
						<asp:LinkButton runat="server" CssClass="list-cmd" ID="btnImport" Text="导入" OnClientClick="return ($pc.getEnabled(this) && invokeImport() && false);"></asp:LinkButton>
					</li>
					<li>
						<asp:LinkButton runat="server" CssClass="list-cmd" ID="btnExport" Text="导出" OnClientClick="return ($pc.getEnabled(this) && invokeExport() && false);"></asp:LinkButton>
					</li>
					<li class="pc-dropdownmenu" style="float: right"><span style="display: block; display: inline-block">
						<asp:HyperLink ID="lnkViewMode" runat="server" CssClass="pc-toggler-dd list-cmd shortcut"
							NavigateUrl="javascript:void(0);" Style="margin-right: 0">
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
			<div class="pc-grid-container">
				<asp:MultiView ActiveViewIndex="0" runat="server" ID="views">
					<asp:View runat="server">
						<mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
							AllowPaging="True" AllowSorting="True" ShowCheckBoxes="True" Category="PermissionCenter"
							GridTitle="群组固定人员" DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title"
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
										<div class="pc-photo-container" style="float: left;">
											<soa:UserPresence ID="uc1" runat="server" ShowUserIcon="true" UserIconUrl='<%#Eval("ID","~/Handlers/UserPhoto.ashx?id={0}") %>'
												UserID='<%#Eval("ID") %>' StatusImage="LongBar" AccountDisabled='<%#0 !=(int)Eval("AccountDisabled") %>' />
										</div>
										<div>
											<%# Server.HtmlEncode((string)Eval("Name")) %>
										</div>
										<div>
											<%# Server.HtmlEncode((string)Eval("DisplayName")) %>
										</div>
										<div>
											<div class="pc-action-tray" id="d" runat="server" visible='<%# this.gridMain.ExportingDeluxeGrid == false %>'>
												<asp:LinkButton ID="lnkEdit" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
												<asp:HyperLink ID="lnkOu" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													Target="_blank" NavigateUrl='<%# "~/lists/MemberOrgView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
													onclick="return $pc.modalPopup(this);">组织</asp:HyperLink>
												<asp:HyperLink ID="lnkGrp" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													Target="_blank" NavigateUrl='<%# "~/lists/MemberGrpView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
													onclick="return $pc.modalPopup(this);">群组</asp:HyperLink>
												<asp:HyperLink ID="lnkSecret" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													Target="_blank" NavigateUrl='<%# "~/lists/UserSecretary.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
													onclick="return $pc.modalPopup(this);">秘书</asp:HyperLink>
												<asp:HyperLink ID="lnkMemberRole" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													Target="_blank" NavigateUrl='<%# "~/lists/MemberRoleView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
													onclick="return $pc.modalPopup(this);">人员角色</asp:HyperLink>
												<asp:HyperLink ID="lnkMemberMatrix" runat="server" NavigateUrl='<%#Eval("ID","~/lists/MemberMatrix.aspx?id={0}") %>'
													onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">查看角色矩阵</asp:HyperLink>
												<asp:HyperLink ID="lnkRP" runat="server" NavigateUrl='<%#Eval("ID","~/lists/FlowList.aspx?id={0}") %>'
													onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">查看相关流程</asp:HyperLink>
												<asp:LinkButton ID="lnkItemDelete" runat="server" Text="删除" CssClass="pc-item-cmd"
													CommandName="DeleteItem" CommandArgument='<%#Eval("ID") %>' Enabled='<%# this.CanEditMembers %>'
													OnClientClick="return ($pc.getEnabled(this) && $pc.confirmDelete('确定要删除?'));"></asp:LinkButton>
											</div>
											<div>
												<%# Server.HtmlEncode((string)Eval("CodeName")) %>
											</div>
										</div>
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField HeaderText="创建者" SortExpression="O.CreatorName">
									<ItemTemplate>
										<soa:UserPresence runat="server" ID="userPresence" ShowUserDisplayName="true" ShowUserIcon="false"
											UserID='<%#Eval("CreatorID") %>' UserDisplayName='<%# Server.HtmlEncode(Eval("CreatorName").ToString()) %>' />
									</ItemTemplate>
								</asp:TemplateField>
								<pc:SubRowBoundField DataField="CreateDate" HtmlEncode="true" HeaderText="创建日期" SortExpression="O.CreateDate"
									SubColSpan="4" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}">
									<SubRowTemplate>
										<span class="pc-label">缺省组织</span>
										<soa:OuNavigator ID="OuNHa" runat="server" StartLevel="0" TerminalVisible="true"
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
							GridTitle="群组固定人员" DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title"
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
											<i class='<%# PermissionCenter.Util.CssSpritesFor(GetDataItem(),PermissionCenter.IconSizeType.Size16) %>'>
											</i>
											<soa:UserPresence ID="uc1" runat="server" UserID='<%#Eval("ID") %>' UserDisplayName='<%# Server.HtmlEncode(Eval("Name").ToString()) %>'
												StatusImage="Ball" AccountDisabled='<%#0 !=(int)Eval("AccountDisabled") %>' />
										</div>
										<div>
											<div class="pc-action-tray" id="d" runat="server" visible='<%# this.grid2.ExportingDeluxeGrid == false %>'>
												<asp:LinkButton ID="lnkEdit" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
												<asp:HyperLink ID="lnkOu" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													Target="_blank" NavigateUrl='<%# "~/lists/MemberOrgView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
													onclick="return $pc.modalPopup(this);">组织</asp:HyperLink>
												<asp:HyperLink ID="lnkGrp" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													Target="_blank" NavigateUrl='<%# "~/lists/MemberGrpView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
													onclick="return $pc.modalPopup(this);">群组</asp:HyperLink>
												<asp:HyperLink ID="lnkSecret" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													Target="_blank" NavigateUrl='<%# "~/lists/UserSecretary.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
													onclick="return $pc.modalPopup(this);">秘书</asp:HyperLink>
												<asp:HyperLink ID="lnkMemberRole" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													Target="_blank" NavigateUrl='<%# "~/lists/MemberRoleView.aspx?id=" + Server.UrlEncode((string)Eval("ID")) %>'
													onclick="return $pc.modalPopup(this);">人员角色</asp:HyperLink>
												<asp:HyperLink ID="lnkMemberMatrix" runat="server" NavigateUrl='<%#Eval("ID","~/lists/MemberMatrix.aspx?id={0}") %>'
													onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">查看角色矩阵</asp:HyperLink>
												<asp:HyperLink ID="lnkRP" runat="server" NavigateUrl='<%#Eval("ID","~/lists/FlowList.aspx?id={0}") %>'
													onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">查看相关流程</asp:HyperLink>
												<asp:LinkButton ID="lnkItemDelete" runat="server" Text="删除" CssClass="pc-item-cmd"
													CommandName="DeleteItem" CommandArgument='<%#Eval("ID") %>' Enabled='<%# this.CanEditMembers %>'
													OnClientClick="return ($pc.getEnabled(this) && $pc.confirmDelete('确定要删除?'));"></asp:LinkButton>
											</div>
										</div>
									</ItemTemplate>
								</asp:TemplateField>
								<asp:BoundField DataField="DisplayName" HtmlEncode="true" HeaderText="显示名称" SortExpression="DisplayName" />
								<asp:BoundField DataField="CodeName" HtmlEncode="true" HeaderText="代码名称" SortExpression="CodeName" />
								<asp:TemplateField HeaderText="创建者" SortExpression="O.CreatorName">
									<ItemTemplate>
										<soa:UserPresence runat="server" ID="userPresence" ShowUserDisplayName="true" ShowUserIcon="false"
											UserID='<%#Eval("CreatorID") %>' UserDisplayName='<%# Server.HtmlEncode(Eval("CreatorName").ToString()) %>' />
									</ItemTemplate>
								</asp:TemplateField>
								<pc:SubRowBoundField DataField="CreateDate" HtmlEncode="true" HeaderText="创建日期" SortExpression="O.CreateDate"
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
		TypeName="MCS.Library.SOA.DataObjects.Security.DataSources.SchemaGroupConstMembersDataSource"
		EnableViewState="false" OnSelecting="dataSourceMain_Selecting">
		<SelectParameters>
			<asp:QueryStringParameter DbType="String" Name="parentId" QueryStringField="id" />
		</SelectParameters>
	</soa:DeluxeObjectDataSource>
	<soa:UploadProgressControl runat="server" ID="ctlUpload" DialogHeaderText="导入角色固定成员数据(xml)"
		DialogTitle="导入" OnDoUploadProgress="DoFileUpload" OnLoadingDialogContent="ctlUpload_LoadingDialogContent"
		OnClientCompleted="postImportProcess" />
	<asp:HiddenField ID="actionData" runat="server" EnableViewState="False" />
	<asp:HiddenField ID="hfGroupID" runat="server" />
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
					$pc.postViaIFrame($pc.appRoot + "Handlers/ObjectExport.ashx", { context: "GroupConstMembers", memberIds: keys, groupId: document.getElementById('hfGroupID').value });
				}
			}

			grid = false;
			return false;
		}

		Sys.Application.add_load(function () {
			toggleMenu();
		});
	</script>
</body>
</html>
