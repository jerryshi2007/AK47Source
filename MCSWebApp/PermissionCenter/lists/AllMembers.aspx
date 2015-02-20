﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AllMembers.aspx.cs" Inherits="PermissionCenter.AllMembers" %>

<%--怪异模式测试时将文档声明置于服务器注释块内--%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>权限中心-所有人员</title>
	<link rel="icon" href="../favicon.ico" type="image/x-icon" />
	<link rel="Shortcut Icon" href="../favicon.ico" />
	<base target="_self" />
	<pc:HeaderControl runat="server" />
	<style type="text/css">
		span.pc-np
		{
			padding-left: 10px;
			padding-right: 10px;
		}
		
		span.pc-wrap
		{
			word-wrap: break-word;
			white-space: normal;
		}
	</style>
</head>
<body>
	<form id="form1" runat="server">
	<div class="pc-frame-header">
		<pc:Banner ID="pcBanner" runat="server" OnTimePointChanged="RefreshList" />
	</div>
	<pc:SceneControl ID="SceneControl1" runat="server">
	</pc:SceneControl>
	<pc:BannerNotice ID="notice" runat="server"></pc:BannerNotice>
	<div class="pc-frame-container">
		<div class="pc-search-box-wrapper">
			<soa:DeluxeSearch ID="DeluxeSearch" runat="server" CssClass="deluxe-search deluxe-left"
				HasCategory="False" HasAdvanced="True" CustomSearchContainerControlID="advSearchPanel"
				SearchFieldTemplate="CONTAINS(${DataField}$, ${Data}$)" SearchField="SearchContent"
				OnSearching="SearchButtonClick" OnConditionClick="onconditionClick">
			</soa:DeluxeSearch>
			<soa:DataBindingControl runat="server" ID="searchBinding" AllowClientCollectData="True">
				<ItemBindings>
					<%--<soa:DataBindingItem ControlID="sfAdvanced" DataPropertyName="" />--%>
					<soa:DataBindingItem ControlID="sfCodeName" DataPropertyName="CodeName" />
					<soa:DataBindingItem ControlID="sfDisabled" DataPropertyName="AccountDisabled" ControlPropertyName="Checked"
						ClientIsHtmlElement="true" ClientPropName="checked" />
					<soa:DataBindingItem ControlID="sfMP" DataPropertyName="MobilePhone" />
					<soa:DataBindingItem ControlID="sfWP" DataPropertyName="WorkPhone" />
					<soa:DataBindingItem ControlID="sfOwner" DataPropertyName="Owner" ControlPropertyName="SelectedSingleData"
						ClientPropName="get_selectedSingleData" ClientSetPropName="set_selectedSingleData"
						ClientIsHtmlElement="false" />
					<soa:DataBindingItem ControlID="sfNoOrg" DataPropertyName="Dissociated" ControlPropertyName="Checked" />
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
							<label for="sfDisabled" class="pc-label">
								已禁用</label><asp:CheckBox Text="" runat="server" ID="sfDisabled" />
						</td>
					</tr>
					<tr>
						<td>
							<label for="sfMP" class="pc-label">
								手机</label><asp:TextBox runat="server" ID="sfMP" MaxLength="30" CssClass="pc-textbox" />(前缀)
						</td>
						<td>
							<label for="sfWP" class="pc-label">
								工作电话</label><asp:TextBox runat="server" ID="sfWP" MaxLength="30" CssClass="pc-textbox" />（前缀）
						</td>
					</tr>
					<tr>
						<td colspan="1">
							<label for="sfOwner" class="pc-label" style="float: left">
								所有者</label>
							<div style="margin-left: 50px">
								<soa:OuUserInputControl ID="sfOwner" runat="server" SelectMask="Organization" ListMask="Organization"
									CssClass="pc-ou-input" CanSelectRoot="false" Text="选择组织" MultiSelect="false" />
							</div>
						</td>
						<td>
							<label for="sfNoOrg" class="pc-label" style="float: left">
								无组织</label>
							<asp:CheckBox Text="" runat="server" ID="sfNoOrg" />
						</td>
					</tr>
				</table>
			</div>
		</div>
		<div class="pc-container5">
			<div class="pc-listmenu-container">
				<ul class="pc-listmenu" id="listMenu">
					<li class="pc-dropdownmenu"><span style="display: block; display: inline-block">
						<asp:LinkButton runat="server" ID="lnkNew" CssClass="button pc-list-cmd" OnClientClick="return ($pc.getEnabled(this) && $pc.popups.newMember(this));"
							OnClick="RefreshList">新建人员<i class="pc-arrow"></i></asp:LinkButton></span>
						<div style="position: relative; z-index: 9;">
							<div style="position: absolute">
								<asp:Repeater runat="server" ID="shortcut" DataMember="users" DataSourceID="SchemaDataSource1">
									<HeaderTemplate>
										<ul class="pc-popup-nav">
									</HeaderTemplate>
									<ItemTemplate>
										<li>
											<asp:LinkButton ID="lb1" runat="server" CssClass="shortcut" data-schema='<%#PermissionCenter.Util.HtmlAttributeEncode(Eval("Name").ToString()) %>'
												OnClick="RefreshList" OnClientClick="return ($pc.getEnabled(this) && $pc.popups.newMember(this));"><%# Server.HtmlEncode(Eval("Description").ToString())%></asp:LinkButton>
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
						<asp:LinkButton runat="server" ID="lnkProperties" CssClass="button pc-list-cmd pc-hide"
							OnClientClick="return ($pc.getEnabled(this) && !!showBatchEditor());" OnClick="RefreshList">属性</asp:LinkButton>
					</li>
					<li>
						<asp:LinkButton runat="server" CssClass="list-cmd" ID="btnMoveToGroup" Text="添加到群组"
							OnClientClick="return ($pc.getEnabled(this) && $pc.popups.batch.membersToGroups($find('gridViewUsers')||$find('gridView2'),'actionData'));"
							OnClick="BatchAddToGroup"></asp:LinkButton>
					</li>
					<li>
						<asp:LinkButton runat="server" CssClass="list-cmd" ID="btnMoveToOrg" Text="添加到组织"
							OnClientClick="return ($pc.getEnabled(this) && $pc.popups.batch.membersToOrgs($find('gridViewUsers')||$find('gridView2'),'actionData'));"
							OnClick="BatchAddToOrg"></asp:LinkButton>
					</li>
					<li>
						<asp:LinkButton runat="server" CssClass="list-cmd" ID="btnDeleteSelected" Text="删除所选人员"
							OnClientClick="return ($pc.getEnabled(this) && $pc.popups.batchDelete($find('gridViewUsers')||$find('gridView2'),'Users'));"
							OnClick="BatchDelete"></asp:LinkButton>
					</li>
					<li>
						<asp:LinkButton runat="server" CssClass="list-cmd" ID="btnImport" Text="导入" OnClientClick="return ($pc.getEnabled(this) && invokeImport() && false);"
							OnClick="RefreshList"></asp:LinkButton>
					</li>
					<li>
						<asp:LinkButton runat="server" CssClass="list-cmd" ID="btnExport" Text="导出" OnClientClick="return ($pc.getEnabled(this) && invokeExport() && false);"></asp:LinkButton>
					</li>
					<li class="pc-dropdownmenu" style="float: right"><span style="display: block; display: inline-block">
						<asp:HyperLink ID="lnkDisplay" runat="server" CssClass="pc-toggler-dd list-cmd shortcut"
							NavigateUrl="" Style="margin-right: 0; cursor: default">
							<i class="pc-toggler-icon"></i><span runat="server" id="displayFilter">常规列表</span><i
								class="pc-arrow"></i>
						</asp:HyperLink>
					</span>
						<div class="pc-popup-nav-pan pc-right" style="z-index: 9">
							<div class="pc-popup-nav-wrapper">
								<ul class="pc-popup-nav" style="text-align: left; color: #ffffff;">
									<li><a class="pc-toggler-dd shortcut" style="padding-left: 0" href="javascript:void(0);"
										onclick="toggleViewMode('0');"><i class="pc-toggler-icon"></i>常规列表</a></li>
									<li><a class="pc-toggler-dr shortcut" id="lnkToggleReduced" style="padding-left: 0"
										href="javascript:void(0);" onclick="toggleViewMode('1');"><i class="pc-toggler-icon">
										</i>精简列表</a></li>
									<li><a class="pc-toggler-dt shortcut" id="lnkToggleTable" style="padding-left: 0"
										href="javascript:void(0);" onclick="toggleViewMode('2');"><i class="pc-toggler-icon">
										</i>精简表格</a></li>
								</ul>
							</div>
						</div>
					</li>
				</ul>
			</div>
			<div class="pc-grid-container" id="listContainer" runat="server">
				<div style="display: none">
					<asp:HiddenField ID="actionData" runat="server" EnableViewState="False" />
					<asp:HiddenField ID="hfViewMode" runat="server" Value="0" />
					<asp:Button ID="btnToggleViewMode" runat="server" OnClick="ToggleViewMode" />
				</div>
				<asp:MultiView runat="server" ID="views" ActiveViewIndex="0">
					<asp:View ID="viewNormal" runat="server">
						<mcs:DeluxeGrid ID="gridViewUsers" runat="server" AutoGenerateColumns="False" DataSourceID="ObjectDataSourceUsers"
							AllowPaging="True" AllowSorting="True" PageSize="10" ShowExportControl="true"
							ShowCheckBoxes="true" Category="PermissionCenter" GridTitle="人员列表" CheckBoxPosition="Left"
							DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title" PagerSettings-Position="Bottom"
							OnRowCommand="HandleRowCommand" OnSelectAllCheckBoxClick="toggleMenu" OnSelectCheckBoxClick="toggleMenu">
							<EmptyDataTemplate>
								暂时没有您需要的数据
							</EmptyDataTemplate>
							<HeaderStyle CssClass="head" />
							<Columns>
								<asp:TemplateField AccessibleHeaderText="照片" HeaderText="照片">
									<HeaderStyle CssClass="pc-col-photo" />
									<ItemStyle CssClass="pc-col-photo" />
									<ItemTemplate>
										<div class="pc-photo-container">
											<soa:UserPresence runat="server" ID="userPresence" ShowUserDisplayName="false" ShowUserIcon="true"
												StatusImage="LongBar" UserID='<%#Eval("ID") %>' UserIconUrl='<%#Eval("ID","~/Handlers/UserPhoto.ashx?id={0}") %>'
												AccountDisabled='<%#0 !=(int)Eval("AccountDisabled") %>' />
										</div>
										<div>
											<div class="pc-action-tray" runat="server" id="d" visible='<%# this.gridViewUsers.ExportingDeluxeGrid == false %>'>
												<asp:LinkButton ID="lnkEdit" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
												<asp:HyperLink ID="lnkItemOrg" runat="server" NavigateUrl='<%#Eval("ID","~/lists/MemberOrgView.aspx?id={0}") %>'
													onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">组织</asp:HyperLink>
												<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%#Eval("ID","~/lists/MemberGrpView.aspx?id={0}") %>'
													onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">群组</asp:HyperLink>
												<asp:HyperLink ID="lnkItemRoles" runat="server" NavigateUrl='<%#Eval("ID","~/lists/MemberRoleView.aspx?id={0}") %>'
													onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">人员角色</asp:HyperLink>
												<asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl='<%#Eval("ID","~/lists/MemberMatrix.aspx?id={0}") %>'
													onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">查看角色矩阵</asp:HyperLink>
												<asp:HyperLink ID="lnkRP" runat="server" NavigateUrl='<%#Eval("ID","~/lists/FlowList.aspx?id={0}") %>'
													onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">查看相关流程</asp:HyperLink>
												<asp:HyperLink ID="lnkSecretary" runat="server" NavigateUrl='<%#Eval("ID","~/lists/UserSecretary.aspx?id={0}") %>'
													onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">秘书</asp:HyperLink>
												<asp:LinkButton ID="lnkItemDelete" runat="server" Text="删除此人员" CssClass="pc-item-cmd"
													Enabled='<%# this.IsDeleteEnabled(Eval("OwnerID").ToString()) %>' CommandName="DeleteItem"
													CommandArgument='<%#Eval("ID") %>' OnClientClick="return ($pc.getEnabled(this) && $pc.confirmDelete('确定要删除此人员?'));"></asp:LinkButton>
												<asp:HyperLink ID="lnkHistory" runat="server" CssClass="pc-item-cmd" Target="_blank"
													onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/ObjectHistoryLog.aspx?id={0}") %>'>历史</asp:HyperLink>
											</div>
										</div>
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField>
									<HeaderTemplate>
										<mcs:GridColumnSorter ID="colSorter" runat="server" DefaultOrderName="属性" DockPosition="Right"
											PreventRenderChildren='<%# this.gridViewUsers.ExportingDeluxeGrid  %>'>
                                        <SortItems>
                                            <mcs:SortItem Text="名称" SortExpression="U.Name" />
                                            <mcs:SortItem Text="显示名称" SortExpression="U.DisplayName" />
                                            <mcs:SortItem Text="代码名称" SortExpression="U.CodeName" />
                                        </SortItems>                                       
										</mcs:GridColumnSorter>
									</HeaderTemplate>
									<ItemTemplate>
										<ul class="pc-subitems">
											<li><span class="pc-label">名称</span><span class="pc-content">
												<%# Server.HtmlEncode(Eval("Name").ToString())%>&nbsp; </span></li>
											<li><span class="pc-label">显示名称</span><span class="pc-content">
												<%# Server.HtmlEncode(Eval("DisplayName").ToString())%>&nbsp; </span></li>
											<li><span class="pc-label">代码名称</span><span class="pc-content">
												<%# Server.HtmlEncode(Eval("CodeName").ToString())%>&nbsp; </span></li>
											<li class="pc-optional"><span class="pc-label">姓</span><span class="pc-content">
												<%# Server.HtmlEncode(Eval("LastName").ToString())%>&nbsp; </span></li>
											<li class="pc-optional"><span class="pc-label">名</span><span class="pc-content">
												<%# Server.HtmlEncode(Eval("FirstName").ToString())%>&nbsp; </span></li>
											<li class="pc-optional"><span class="pc-label">邮件地址</span><span class="pc-content">
												<%# Server.HtmlEncode(Eval("Mail").ToString())%>&nbsp; </span></li>
											<li class="pc-optional"><span class="pc-label">UC地址</span><span class="pc-content">
												<%# Server.HtmlEncode(Eval("Sip").ToString())%>&nbsp; </span></li>
											<li class="pc-optional"><span class="pc-label">手机号</span><span class="pc-content">
												<%# Server.HtmlEncode(Eval("MP").ToString())%>&nbsp; </span></li>
											<li class="pc-optional"><span class="pc-label">工作电话</span><span class="pc-content"><%# Server.HtmlEncode(Eval("WP").ToString())%>&nbsp;</span></li>
											<li class="pc-optional"><span class="pc-label">通信地址</span><span class="pc-content"><%# Server.HtmlEncode(Eval("Address").ToString())%>&nbsp;</span></li>
											<li><span class="pc-label">所有者</span><span class="pc-content">
												<asp:HyperLink ID="hp1" NavigateUrl='<%# Eval("OwnerID","~/lists/OUExplorer.aspx?ou={0}") %>'
													CssClass="pc-item-link" runat="server"><%# Server.HtmlEncode(Eval("OwnerName").ToString())%></asp:HyperLink>&nbsp;</span></li>
											<li><span class="pc-label">主职</span><span class="pc-content">
												<soa:OuNavigator runat="server" ID="oun1" StartLevel="0" SplitterVisible="true" LinkCssClass="pc-item-link"
													CssClass="pc-wrap" SplitterCssClass="pc-np" ObjectID='<%#Eval("ParentID").ToString() %>'
													TerminalVisible="true" RootVisible="false" NavigateUrlFormat="~/lists/OUExplorer.aspx?ou={0}" />
												&nbsp;</span></li>
										</ul>
										<ul class="pc-subitems pc-optional">
											<li><span class="pc-label">创建日期</span><span class="pc-content">
												<%# Server.HtmlEncode(Eval("CreateDate", "{0:yyyy-MM-dd}"))%>&nbsp; </span></li>
											<li><span class="pc-label">创建者</span><span class="pc-content">
												<soa:UserPresence runat="server" ID="creator" UserDisplayName='<%# Server.HtmlEncode(Eval("CreatorName").ToString())%>'
													UserID='<%#Eval("CreatorID") %>' />
												&nbsp;</span> </li>
										</ul>
									</ItemTemplate>
									<ItemStyle />
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
					<asp:View ID="viewTable" runat="server">
						<mcs:DeluxeGrid ID="gridView2" runat="server" AutoGenerateColumns="False" DataSourceID="ObjectDataSourceUsers"
							AllowPaging="True" AllowSorting="True" PageSize="10" ShowExportControl="true"
							ShowCheckBoxes="true" Category="PermissionCenter" GridTitle="人员列表" CheckBoxPosition="Left"
							DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title" PagerSettings-Position="Bottom"
							OnRowCommand="HandleRowCommand" OnSelectAllCheckBoxClick="toggleMenu" OnSelectCheckBoxClick="toggleMenu">
							<EmptyDataTemplate>
								暂时没有您需要的数据
							</EmptyDataTemplate>
							<HeaderStyle CssClass="head" />
							<Columns>
								<asp:TemplateField AccessibleHeaderText="名称" HeaderText="名称" SortExpression="U.Name">
									<ItemTemplate>
										<div>
											<soa:UserPresence runat="server" ID="creator" UserDisplayName='<%# Server.HtmlEncode(Eval("Name").ToString())%>'
												UserID='<%#Eval("ID") %>' AccountDisabled='<%#0 !=(int)Eval("AccountDisabled") %>' />
										</div>
										<div>
											<div class="pc-action-tray" runat="server" id="d" visible='<%# this.gridView2.ExportingDeluxeGrid == false %>'>
												<asp:LinkButton ID="lnkEdit" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
													OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
												<asp:HyperLink ID="lnkItemOrg" runat="server" NavigateUrl='<%#Eval("ID","~/lists/MemberOrgView.aspx?id={0}") %>'
													onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">组织</asp:HyperLink>
												<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%#Eval("ID","~/lists/MemberGrpView.aspx?id={0}") %>'
													onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">群组</asp:HyperLink>
												<asp:HyperLink ID="lnkItemRoles" runat="server" NavigateUrl='<%#Eval("ID","~/lists/MemberRoleView.aspx?id={0}") %>'
													onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">人员角色</asp:HyperLink>
												<asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl='<%#Eval("ID","~/lists/MemberMatrix.aspx?id={0}") %>'
													onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">查看角色矩阵</asp:HyperLink>
												<asp:HyperLink ID="lnkRP" runat="server" NavigateUrl='<%#Eval("ID","~/lists/FlowList.aspx?id={0}") %>'
													onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">查看相关流程</asp:HyperLink>
												<asp:HyperLink ID="lnkSecretary" runat="server" NavigateUrl='<%#Eval("ID","~/lists/UserSecretary.aspx?id={0}") %>'
													onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">秘书</asp:HyperLink>
												<asp:LinkButton ID="lnkItemDelete" runat="server" Text="删除此人员" CssClass="pc-item-cmd"
													Enabled='<%# this.IsDeleteEnabled(Eval("OwnerID").ToString()) %>' CommandName="DeleteItem"
													CommandArgument='<%#Eval("ID") %>' OnClientClick="return ($pc.getEnabled(this) && $pc.confirmDelete('确定要删除此人员?'));"></asp:LinkButton>
												<asp:HyperLink ID="lnkHistory" runat="server" CssClass="pc-item-cmd" Target="_blank"
													onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/ObjectHistoryLog.aspx?id={0}") %>'>历史</asp:HyperLink>
											</div>
										</div>
									</ItemTemplate>
								</asp:TemplateField>
								<asp:BoundField HeaderText="显示名称" DataField="DisplayName" HtmlEncode="true" SortExpression="U.DisplayName" />
								<asp:BoundField HeaderText="代码名称" DataField="CodeName" HtmlEncode="true" SortExpression="U.CodeName" />
								<asp:TemplateField HeaderText="所有者">
									<ItemTemplate>
										<asp:HyperLink ID="hp1" NavigateUrl='<%# Eval("OwnerID","~/lists/OUExplorer.aspx?ou={0}") %>'
											CssClass="pc-item-link" runat="server">
								<%# Server.HtmlEncode(Eval("OwnerName").ToString())%>
										</asp:HyperLink>
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField HeaderText="主职">
									<ItemTemplate>
										<soa:OuNavigator runat="server" ID="oun2" SplitterVisible="true" LinkCssClass="pc-item-link"
											ObjectID='<%#Eval("ParentID").ToString() %>' StartLevel="0" TerminalVisible="true"
											RootVisible="false" NavigateUrlFormat="~/lists/OUExplorer.aspx?ou={0}" />
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
	<soa:DeluxeObjectDataSource ID="ObjectDataSourceUsers" runat="server" EnablePaging="True"
		TypeName="MCS.Library.SOA.DataObjects.Security.DataSources.SchemaUserDataSource"
		EnableViewState="false" OnSelecting="ObjectDataSourceUsers_Selecting" OnSelected="ObjectDataSourceUsers_Selected">
	</soa:DeluxeObjectDataSource>
	<pc:Footer ID="footer" runat="server" />
	<soa:UploadProgressControl runat="server" ID="ctlUpload" DialogHeaderText="导入人员数据(xml)"
		DialogTitle="导入" OnDoUploadProgress="DoFileUpload" OnLoadingDialogContent="ctlUpload_LoadingDialogContent"
		OnClientCompleted="postImportProcess" />
	</form>
	<script type="text/javascript">
		$pc.ui.listMenuBehavior("listMenu");
		$pc.ui.gridBehavior("gridViewUsers", "hover");
		$pc.ui.gridBehavior("gridView2", "hover");

		function onconditionClick(sender, e) {
			var content = Sys.Serialization.JavaScriptSerializer.deserialize(e.ConditionContent);
			var bindingControl = $find("searchBinding");
			bindingControl.dataBind(content);
		}

		function toggleMenu() {
			var grid = $find("gridViewUsers") || $find("gridView2");
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
			return $pc.popups.batchProperties($find("gridViewUsers") || $find("gridView2"));
		}

		function toggleViewMode(mode) {
			var elem = document.getElementById("hfViewMode");
			var needPostback = false;
			if (elem) {
				if (mode != elem.value) {
					if (elem.value == '2' || mode == '2') {
						needPostback = true;
					}

					elem.value = mode;

					if (needPostback) {
						__doPostBack('btnToggleViewMode', '');
					} else {
						switch (mode) {
							case "1":
								$pc.addClass(document.getElementById("listContainer"), "pc-reduced-view");
								document.getElementById("lnkDisplay").className = "pc-toggler-dr list-cmd shortcut";
								$pc.setText("displayFilter", "精简列表");
								break;
							case "2":
								$pc.removeClass(document.getElementById("listContainer"), "pc-reduced-view");
								document.getElementById("lnkDisplay").className = "pc-toggler-dt list-cmd shortcut";
								$pc.setText("displayFilter", "精简表格");
								break;
							case "0":
							default:
								$pc.removeClass(document.getElementById("listContainer"), "pc-reduced-view");
								document.getElementById("lnkDisplay").className = "pc-toggler-dd list-cmd shortcut";
								$pc.setText("displayFilter", "常规列表");
								break;

						}

						$pc.oneWayRequest($pc.appRoot + "Handlers/Toggle.ashx?tp=UserBrowseView&i=" + mode + "&magic=" + Math.random() * 1000, function () { }, function () {
							$pc.console.info("未能记忆用户选择的查看模式");
						});
					}
				}
			}
			elem = null;
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
			var grid = $find("gridViewUsers") || $find("gridView2");
			if (grid) {
				var keys = grid.get_clientSelectedKeys();
				if (keys.length > 0) {
					$pc.postViaIFrame($pc.appRoot + "Handlers/ObjectExport.ashx", { context: "AllUsers", id: keys });
				}
			}
			grid = null;
			return false;
		}

		Sys.Application.add_load(function () {
			toggleMenu();
		});
	</script>
</body>
</html>
