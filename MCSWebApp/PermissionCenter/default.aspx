<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="PermissionCenter._default" %>

<%@ Register TagPrefix="MCSP" Namespace="MCS.Library.Web.Controls" Assembly="MCS.Library.Passport" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>欢迎使用权限中心</title>
	<meta http-equiv="X-UA-Compatible" content="IE=edge" />
	<link rel="icon" href="favicon.ico" type="image/x-icon" />
	<link rel="Shortcut Icon" href="favicon.ico" />
	<link href="styles/home.css" rel="stylesheet" type="text/css" />
	<pc:HeaderControl ID="HeaderControl1" runat="server" />
</head>
<body>
	<form id="serverForm" runat="server">
	<asp:ScriptManager runat="server" EnableScriptGlobalization="true">
		<Services>
			<asp:ServiceReference Path="~/Services/CommonService.asmx" />
		</Services>
	</asp:ScriptManager>
	<div class="pc-frame-header">
		<pc:Banner ID="pcBanner" runat="server" ActiveMenuIndex="-1" />
	</div>
	<div class="pc-frame-container">
		<div class="pc-search-box-wrapper">
			<soa:DeluxeSearch ID="DeluxeSearch" runat="server" CssClass="deluxe-search deluxe-left"
				HasCategory="False" HasAdvanced="True" CustomSearchContainerControlID="advSearchPanel"
				OnConditionClick="onconditionClick" SearchFieldTemplate="CONTAINS(${DataField}$, ${Data}$)"
				SearchField="SearchContent" OnSearching="SearchButtonClick">
			</soa:DeluxeSearch>
			<soa:DataBindingControl runat="server" ID="searchBinding" AllowClientCollectData="True">
				<ItemBindings>
					<%--<soa:DataBindingItem ControlID="sfAdvanced" DataPropertyName="" />--%>
					<soa:DataBindingItem ControlID="sfCodeName" DataPropertyName="CodeName" />
					<soa:DataBindingItem ControlID="sfDisabled" DataPropertyName="AccountDisabled" ControlPropertyName="Checked"
						ClientIsHtmlElement="true" ClientPropName="checked" />
					<soa:DataBindingItem ControlID="sfSchemaType" DataPropertyName="SchemaType" />
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
							<label for="sdSchemaType" class="pc-label">
								类型</label><soa:HBDropDownList ID="sfSchemaType" runat="server" AppendDataBoundItems="True"
									DataSourceID="schemaTypeDataSource" SelectedText="(未指定)" DataMember="allTypes"
									DataTextField="Description" DataValueField="Name" CssClass="pc-textbox">
									<asp:ListItem Text="(未指定)" Value="" Selected="True"></asp:ListItem>
								</soa:HBDropDownList>
							<pc:SchemaDataSource runat="server" ID="schemaTypeDataSource" />
						</td>
						<td>
						</td>
					</tr>
				</table>
			</div>
		</div>
		<div class="pc-container5">
			<asp:HiddenField ID="searchPerformed" runat="server" />
			<asp:MultiView runat="server" ActiveViewIndex="0" ID="views">
				<asp:View runat="server">
					<div class="pc-clear pc-center">
						<ul class="pc-metro-blocks">
							<li class="pc-metro-block-member pc-metro-orange" data-url="lists/AllMembers.aspx"
								onclick="metroClick(this);">
								<div class="pc-metro-icon">
								</div>
								<div class="pc-metro-block-content">
									<div class="pc-metro-title">
										所有人员
									</div>
									<div class="pc-metro-detail">
										(<span id="ltMemberCount" runat="server">0 </span>)
									</div>
								</div>
							</li>
							<li class="pc-metro-block-org pc-metro-blue" data-url="lists/OUExplorer.aspx" onclick="metroClick(this);">
								<div class="pc-metro-icon">
								</div>
								<div class="pc-metro-block-content">
									<div class="pc-metro-title">
										所有组织
									</div>
									<div class="pc-metro-detail">
										(<span id="ltOrgCount" runat="server">0</span>)
									</div>
								</div>
							</li>
							<li class="pc-metro-block-group pc-metro-red" data-url="lists/AllGroups.aspx" onclick="metroClick(this);">
								<div class="pc-metro-icon">
								</div>
								<div class="pc-metro-block-content">
									<div class="pc-metro-title">
										所有群组
									</div>
									<div class="pc-metro-detail">
										(<span id="ltGroupCount" runat="server">0</span>)
									</div>
								</div>
							</li>
							<li class="pc-metro-block-app pc-metro-green" data-url="lists/AllApps.aspx" onclick="metroClick(this);">
								<div class="pc-metro-icon">
								</div>
								<div class="pc-metro-block-content">
									<div class="pc-metro-title">
										应用授权
									</div>
									<div class="pc-metro-detail">
										(<span runat="server" id="ltAppCount">0 </span>)
									</div>
								</div>
							</li>
							<li class="pc-metro-block-log pc-dw" data-url="lists/LogList.aspx" onclick="metroClick(this);">
								<div style="width: 64px; float: left">
									<div class="pc-metro-icon">
									</div>
									<div class="pc-metro-block-content">
										<div class="pc-metro-title">
											日志
										</div>
									</div>
								</div>
								<div style="padding-left: 0; text-align: left; overflow: hidden; height: 100%;">
									<div class="pc-metro-logs-container">
										<asp:Repeater runat="server" ID="logItems">
											<HeaderTemplate>
												<div class="pc-metro-logs" id="clientLogList">
											</HeaderTemplate>
											<ItemTemplate>
												<div class="pc-metro-log even">
													<div class="pc-metro-log-title">
														<%#Eval("Subject")%></div>
													<div class="pc-metro-log-at">
														<%#Eval("CreateTime", "{0:yyyy-MM-dd HH:mm:ss}")%></div>
												</div>
											</ItemTemplate>
											<AlternatingItemTemplate>
												<div class="pc-metro-log">
													<div class="pc-metro-log-title">
														<%#Eval("Subject")%></div>
													<div class="pc-metro-log-at">
														<%#Eval("CreateTime", "{0:yyyy-MM-dd HH:mm:ss}")%></div>
												</div>
											</AlternatingItemTemplate>
											<FooterTemplate>
												</div>
											</FooterTemplate>
										</asp:Repeater>
									</div>
								</div>
							</li>
							<% var mainOrg = this.MainOrgnization; %>
							<li class="pc-metro-block-main pc-metro-red" data-url='<%= "lists/OUExplorer.aspx" + (mainOrg!=null? ("?ou=" + mainOrg.ID):"") %>'
								onclick="metroClick(this);">
								<div class="pc-metro-block-content">
									<div class="pc-metro-title">
										我的主职
									</div>
									<div class="" style="clear: both">
										<%= mainOrg!=null? this.Server.HtmlEncode( mainOrg.Name):"(无)" %>
									</div>
								</div>
							</li>
						</ul>
					</div>
				</asp:View>
				<asp:View runat="server">
					<div class="pc-listmenu-container">
						<ul class="pc-listmenu" id="listMenu">
							<li>
								<asp:LinkButton runat="server" ID="lnkProperties" CssClass="button pc-list-cmd pc-hide"
									OnClientClick="return ($pc.getEnabled(this) && !!showBatchEditor());" OnClick="RefreshList">属性</asp:LinkButton>
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
					<div class="pc-grid-container">
						<asp:MultiView ID="gridViews" ActiveViewIndex="0" runat="server">
							<asp:View runat="server">
								<mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
									AllowPaging="True" AllowSorting="True" ShowCheckBoxes="True" Category="PermissionCenter"
									GridTitle="对象列表" DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title"
									PagerSettings-Position="Bottom" DataSourceMaxRow="0" TitleColor="141, 143, 149"
									TitleFontSize="Large" ShowExportControl="True" OnRowDataBound="OnRowDataBound"
									OnSelectAllCheckBoxClick="toggleMenu" OnSelectCheckBoxClick="toggleMenu">
									<EmptyDataTemplate>
										暂时没有您需要的数据
									</EmptyDataTemplate>
									<HeaderStyle CssClass="head" />
									<Columns>
										<asp:TemplateField HeaderText="名称">
											<HeaderTemplate>
												<mcs:GridColumnSorter runat="server" ID="colSorter" DefaultOrderName="名称" PreventRenderChildren='<%# this.gridMain.ExportingDeluxeGrid  %>'
													DockPosition="Left">
									<SortItems>
									<mcs:SortItem SortExpression="Name" Text="名称" />
									<mcs:SortItem SortExpression="DisplayName" Text="显示名称" />
									<mcs:SortItem SortExpression="CodeName" Text="代码名称" />
									</SortItems>
												</mcs:GridColumnSorter>
											</HeaderTemplate>
											<ItemTemplate>
												<div class="pc-photo-container" style="float: left;">
													<soa:UserPresence runat="server" ID="uc1" StatusImage="LongBar" ShowUserIcon="true"
														Visible='<%# PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>' UserID='<%#Eval("ID") %>'
														UserIconUrl='<%# this.ResolveClientUrl(Eval("ID","~/Handlers/UserPhoto.ashx?id={0}")) %>'
														AccountDisabled='<%#0 !=(int)Eval("AccountDisabled") %>' />
													<div class="uc-user-container-long" runat="server" visible='<%# false == PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>'>
														<i id="ico" class='<%# PermissionCenter.Util.CssSpritesFor(GetDataItem(),PermissionCenter.IconSizeType.Size32) %>'>
														</i>
													</div>
												</div>
												<div>
													<pc:SchemaHyperLink runat="server" CssClass="pc-item-link" Target="_self" ID="shl"
														ObjectID='<%#Eval("ID") %>' ObjectSchemaType='<%#Eval("SchemaType") %>' data-id='<%# PermissionCenter.Util.HtmlAttributeEncode((string) Eval("ID")) %>'><%# Server.HtmlEncode((string) Eval("Name")) %></pc:SchemaHyperLink>
												</div>
												<div>
													<%# Server.HtmlEncode((string)Eval("DisplayName")) %>
												</div>
												<div>
													<div id="divActions" class="pc-action-tray" runat="server" visible='<%# this.gridMain.ExportingDeluxeGrid == false %>'>
														<asp:LinkButton ID="lnkEdit" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
															OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
														<asp:HyperLink ID="lnkAcl" runat="server" CssClass="pc-item-cmd" onclick="return $pc.modalPopup(this);"
															Target="_blank" Visible='<%#PermissionCenter.Util.IsAclContainer((string)Eval("SchemaType")) %>'
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
														<a href="javascript:void(0);" runat="server" class="pc-item-cmd" onclick='openMore(this)'
															data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'>更多...</a>
														<asp:HyperLink ID="lnkHistory" runat="server" CssClass="pc-item-cmd" Target="_blank"
															onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/ObjectHistoryLog.aspx?id={0}") %>'>历史</asp:HyperLink>
													</div>
												</div>
												<div>
													<%# Server.HtmlEncode((string)Eval("CodeName")) %>
												</div>
											</ItemTemplate>
										</asp:TemplateField>
										<asp:TemplateField HeaderText="类型" SortExpression="SchemaType">
											<ItemTemplate>
												<asp:Label ID="Label1" runat="server" Text='<%# Server.HtmlEncode(this.SchemaTypeToString((string)Eval("SchemaType"))) %>'></asp:Label>
											</ItemTemplate>
										</asp:TemplateField>
										<asp:TemplateField HeaderText="创建者" SortExpression="CreatorName">
											<ItemTemplate>
												<soa:UserPresence ID="userPresence" runat="server" ShowUserDisplayName="true" ShowUserIcon="false"
													UserDisplayName='<%# Server.HtmlEncode(Eval("CreatorName").ToString()) %>' UserID='<%#Eval("CreatorID") %>' />
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
									GridTitle="对象列表" DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title"
									PagerSettings-Position="Bottom" DataSourceMaxRow="0" TitleColor="141, 143, 149"
									TitleFontSize="Large" ShowExportControl="True" OnRowDataBound="OnRowDataBound"
									OnSelectAllCheckBoxClick="toggleMenu" OnSelectCheckBoxClick="toggleMenu">
									<EmptyDataTemplate>
										暂时没有您需要的数据
									</EmptyDataTemplate>
									<HeaderStyle CssClass="head" />
									<Columns>
										<asp:TemplateField HeaderText="名称" SortExpression="Name">
											<ItemTemplate>
												<div>
													<soa:UserPresence runat="server" ID="uc1" StatusImage="Ball" ShowUserIcon="false"
														Visible='<%# PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>' UserID='<%#Eval("ID") %>'
														AccountDisabled='<%#0 !=(int)Eval("AccountDisabled") %>' />
													<i id="ico" runat="server" visible='<%# false == PermissionCenter.Util.IsUser((string)Eval("SchemaType")) %>'
														class='<%# PermissionCenter.Util.CssSpritesFor(GetDataItem(),PermissionCenter.IconSizeType.Size16) %>'>
													</i>
													<pc:SchemaHyperLink runat="server" CssClass="pc-item-link" Target="_self" ID="shl"
														ObjectID='<%#Eval("ID") %>' ObjectSchemaType='<%#Eval("SchemaType") %>' data-id='<%# PermissionCenter.Util.HtmlAttributeEncode((string) Eval("ID")) %>'><%# Server.HtmlEncode((string) Eval("Name")) %></pc:SchemaHyperLink>
												</div>
												<div>
													<div id="divActions" class="pc-action-tray" runat="server" visible='<%# this.grid2.ExportingDeluxeGrid == false %>'>
														<asp:LinkButton ID="lnkEdit" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
															OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
														<asp:HyperLink ID="lnkAcl" runat="server" CssClass="pc-item-cmd" onclick="return $pc.modalPopup(this);"
															Target="_blank" Visible='<%#PermissionCenter.Util.IsAclContainer((string)Eval("SchemaType")) %>'
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
														<asp:HyperLink ID="lnkHistory" runat="server" CssClass="pc-item-cmd" Target="_blank"
															onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/ObjectHistoryLog.aspx?id={0}") %>'>历史</asp:HyperLink>
														<a href="javascript:void(0);" runat="server" class="pc-item-cmd" onclick='openMore(this);'
															data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'>更多...</a>
													</div>
												</div>
											</ItemTemplate>
										</asp:TemplateField>
										<asp:BoundField HeaderText="显示名称" DataField="DisplayName" HtmlEncode="true" SortExpression="DisplayName" />
										<asp:BoundField HeaderText="代码名称" DataField="CodeName" HtmlEncode="true" SortExpression="CodeName" />
										<asp:TemplateField HeaderText="类型" SortExpression="SchemaType">
											<ItemTemplate>
												<asp:Label ID="Label1" runat="server" Text='<%# Server.HtmlEncode(this.SchemaTypeToString((string)Eval("SchemaType"))) %>'></asp:Label>
											</ItemTemplate>
										</asp:TemplateField>
										<asp:TemplateField HeaderText="创建者" SortExpression="CreatorName">
											<ItemTemplate>
												<soa:UserPresence ID="userPresence" runat="server" ShowUserDisplayName="true" ShowUserIcon="false"
													UserDisplayName='<%# Server.HtmlEncode(Eval("CreatorName").ToString()) %>' UserID='<%#Eval("CreatorID") %>' />
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
				</asp:View>
			</asp:MultiView>
			<soa:DeluxeObjectDataSource ID="dataSourceMain" runat="server" EnablePaging="True"
				TypeName="MCS.Library.SOA.DataObjects.Security.DataSources.SchemaGlobalObjectDataSource"
				EnableViewState="false" OnSelecting="dataSourceMain_Selecting">
			</soa:DeluxeObjectDataSource>
		</div>
	</div>
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

		function metroClick(elem) {
			var url = $pc.getAttr(elem, "data-url");
			window.location.href = $pc.appRoot + url;
		}

		function openMore(lnk) {

			lnk.onclick = 'javascript:void(0);';
			lnk.innerHTML = '<span class="pc-icon-loader">&nbsp;</span>';
			var dataId = $pc.getAttr(lnk, "data-id");
			PermissionCenter.Services.CommonService.GetAditionOperations(dataId, function (data) {

				var nnn;
				if (data.length) {
					for (var i = data.length - 1; i >= 0; i--) {
						nnn = document.createElement("a");
						nnn.href = data[i].Url;
						nnn.appendChild(document.createTextNode(data[i].Label));
						$pc.addClass(nnn, "pc-item-cmd");
						lnk.parentNode.insertBefore(nnn, lnk);
						$pc.setAttr(nnn, 'data-id', dataId);
						if (data[i].Popup)
							nnn.onclick = $pc.createDelegate(nnn, function () { return $pc.modalPopup(this); });
						if (data[i].Target) {
							$pc.setAttr(nnn, 'target', data[i].Target);
						}
					}
				}

				lnk.parentNode.removeChild(lnk);

			}, function (e) {
				lnk.innerHTML = '失败';
				lnk.title = e.Message;

			});
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

		(function () {
			var logList = document.getElementById("clientLogList");
			if (logList) {
				var tall = logList.clientHeight;
				var itemsCount = 0;
				for (var node = logList.firstChild; node; node = node.nextSibling) {
					if (node.nodeType === 1 && $pc.hasClass(node, "pc-metro-log")) {
						itemsCount++;
					}
				}

				var marginTop = 0;
				var i = 0;
				if (itemsCount) {
					window.setInterval(function () {
						var startTop = tall * i;
						i = (i + 1) % itemsCount;
						finishTop = tall * i;
						var delta = finishTop - startTop;

						$pc.animation.circEaseOut(1000, function (p) {
							logList.style.marginTop = (-(startTop + p * delta)) + "px";
						}, function () {
							logList.style.marginTop = (-finishTop) + "px";
						});
					}, 5000);
				}
			}
		})();
	</script>
</body>
</html>
