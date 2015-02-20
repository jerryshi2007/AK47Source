<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MemberSearchDialog.aspx.cs"
	Inherits="PermissionCenter.Dialogs.MemberSearchDialog" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" scroll="no" style="overflow: hidden">
<head runat="server">
	<title>权限中心-选择人员</title>
	<link rel="icon" href="../favicon.ico" type="image/x-icon" />
	<link rel="Shortcut Icon" href="../favicon.ico" />
	<base target="_self" />
	<link href="../styles/dlg.css" rel="stylesheet" type="text/css" />
	<pc:HeaderControl runat="server">
	</pc:HeaderControl>
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
	<div class="pcdlg-sky">
		<h1 class="pc-caption">
			选择人员</h1>
	</div>
	<div class="pcdlg-content">
		<pc:BannerNotice ID="notice" runat="server"></pc:BannerNotice>
		<div class="pc-frame-container">
			<div class="pc-search-box-wrapper">
				<soa:DeluxeSearch ID="DeluxeSearch" runat="server" CssClass="deluxe-search deluxe-left"
					HasCategory="False" SearchFieldTemplate="CONTAINS(${DataField}$, ${Data}$)" SearchField="SearchContent"
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
			<div class="pc-container5" style="overflow-x: hidden">
				<div style="display: none">
					<asp:HiddenField ID="hfViewMode" runat="server" Value="0" />
					<asp:Button ID="btnToggleViewMode" runat="server" OnClick="ToggleViewMode" />
				</div>
				<div class="pc-listmenu-container">
					<ul class="pc-listmenu" id="listMenu">
						<li>
							<asp:LinkButton runat="server" ID="lnkProperties" CssClass="button pc-list-cmd pc-hide"
								OnClientClick="return ($pc.getEnabled(this) && !!showBatchEditor());" OnClick="RefreshList">属性</asp:LinkButton>
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
						<asp:HiddenField ID="HiddenField1" runat="server" Value="0" />
						<asp:Button ID="Button1" runat="server" OnClick="ToggleViewMode" />
					</div>
					<asp:MultiView ID="views" runat="server">
						<asp:View runat="server">
							<mcs:DeluxeGrid ID="gridViewUsers" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
								AllowPaging="True" AllowSorting="True" PageSize="10" ShowExportControl="true"
								ShowCheckBoxes="true" Category="PermissionCenter" GridTitle="人员列表" CheckBoxPosition="Left"
								DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title" PagerSettings-Position="Bottom"
								OnSelectAllCheckBoxClick="toggleMenu" OnSelectCheckBoxClick="toggleMenu">
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
													AccountDisabled='<%#0 !=(int)Eval("AccountDisabled") %>' StatusImage="LongBar"
													UserID='<%#Eval("ID") %>' UserIconUrl='<%#Eval("ID","~/Handlers/UserPhoto.ashx?id={0}") %>' />
											</div>
											<div>
												<div id="d" class="pc-action-tray" runat="server" visible='<%# this.gridViewUsers.ExportingDeluxeGrid == false %>'>
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
														onclick="return $pc.modalPopup(this);">角色</asp:HyperLink>
													<asp:HyperLink ID="lnkHistory" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
														Target="_blank" onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/ObjectHistoryLog.aspx?id={0}") %>'>历史</asp:HyperLink>
												</div>
											</div>
										</ItemTemplate>
									</asp:TemplateField>
									<asp:TemplateField>
										<HeaderTemplate>
											<mcs:GridColumnSorter runat="server" ID="colSorter" DefaultOrderName="名称" PreventRenderChildren='<%# this.gridViewUsers.ExportingDeluxeGrid  %>'>
								   <SortItems>
								   <mcs:SortItem SortExpression="U.Name" Text="名称" />
								   <mcs:SortItem SortExpression="U.DisplayName" Text="显示名称" />
								   <mcs:SortItem SortExpression="U.CodeName" Text="代码名称" />
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
						<asp:View runat="server">
							<mcs:DeluxeGrid ID="gridView2" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
								AllowPaging="True" AllowSorting="True" PageSize="10" ShowExportControl="true"
								ShowCheckBoxes="true" Category="PermissionCenter" GridTitle="人员列表" CheckBoxPosition="Left"
								DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title" PagerSettings-Position="Bottom"
								OnSelectAllCheckBoxClick="toggleMenu" OnSelectCheckBoxClick="toggleMenu">
								<EmptyDataTemplate>
									暂时没有您需要的数据
								</EmptyDataTemplate>
								<HeaderStyle CssClass="head" />
								<Columns>
									<asp:TemplateField AccessibleHeaderText="名称" HeaderText="名称" SortExpression="U.Name">
										<ItemTemplate>
											<div>
												<i class='<%# PermissionCenter.Util.CssSpritesFor(GetDataItem(),PermissionCenter.IconSizeType.Size16) %>'>
												</i>
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
													<asp:HyperLink ID="lnkMemberMatrix" runat="server" NavigateUrl='<%#Eval("ID","~/lists/MemberMatrix.aspx?id={0}") %>'
														onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">查看角色矩阵</asp:HyperLink>
													<asp:HyperLink ID="lnkRP" runat="server" NavigateUrl='<%#Eval("ID","~/lists/FlowList.aspx?id={0}") %>'
														onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">查看相关流程</asp:HyperLink>
													<asp:HyperLink ID="lnkSecretary" runat="server" NavigateUrl='<%#Eval("ID","~/lists/UserSecretary.aspx?id={0}") %>'
														onclick="return $pc.modalPopup(this);" CssClass="pc-item-cmd" Target="_blank">秘书</asp:HyperLink>
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
		<soa:DeluxeObjectDataSource ID="dataSourceMain" runat="server" EnablePaging="True"
			TypeName="MCS.Library.SOA.DataObjects.Security.DataSources.SchemaUserDataSource"
			EnableViewState="false" OnSelecting="ObjectDataSourceUsers_Selecting">
		</soa:DeluxeObjectDataSource>
		<pc:Footer ID="footer" runat="server" />
	</div>
	<div class="pcdlg-floor">
		<div class="pcdlg-button-bar">
			<asp:Button Text="确定" runat="server" CssClass="pcdlg-button" OnClientClick="doConfirm()" /><input
				type="button" class="pcdlg-button" value="取消" onclick="doCancel();" />
		</div>
	</div>
	</form>
	<script type="text/javascript">
		$pc.ui.listMenuBehavior("listMenu");
		$pc.ui.gridBehavior("gridViewUsers", "hover");
		$pc.ui.gridBehavior("gridView2", "hover");
		$pc.ui.traceWindowWidth();

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

		function onconditionClick(sender, e) {
			var content = Sys.Serialization.JavaScriptSerializer.deserialize(e.ConditionContent);
			var bindingControl = $find("searchBinding");
			bindingControl.dataBind(content);
		}

		function doConfirm() {
			var keys = ($find("gridViewUsers") || $find("gridView2")).get_clientSelectedKeys();
			if (keys && keys.length && keys.length > 0) {
				window.returnValue = keys.join(",");
				window.close();
			}
		}

		function doCancel() {
			window.returnValue = '';
			window.close();
		}

		Sys.Application.add_load(function () {
			toggleMenu();
		});
	</script>
</body>
</html>
