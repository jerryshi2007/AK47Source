<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OUSearch.aspx.cs" Inherits="PermissionCenter.OUSearch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>查找用户，组织及群组</title>
	<link rel="icon" href="../favicon.ico" type="image/x-icon" />
	<link rel="Shortcut Icon" href="../favicon.ico" />
	<base target="_self" />
	<pc:HeaderControl ID="HeaderControl1" runat="server" />
</head>
<body>
	<form id="form1" runat="server">
	<soa:DataBindingControl runat="server" ID="binding1">
		<ItemBindings>
			<soa:DataBindingItem DataPropertyName="ID" ControlID="hfOuId" ControlPropertyName="Value"
				Direction="DataToControl" />
			<soa:DataBindingItem DataPropertyName="Name" ControlID="txtScopeName" ControlPropertyName="Text"
				Direction="DataToControl" />
		</ItemBindings>
	</soa:DataBindingControl>
	<div class="pc-banner">
		<h1 class="pc-caption">
			<span>查找用户，组织及群组 </span>
		</h1>
	</div>
	<div class="pc-frame-container">
		<div class="pc-container5">
			<div style="float: left; padding-right: 10px; padding-left: 5px;">
				搜索范围</div>
			<div style="clear: right; border: solid 1px #808080;">
				<asp:Literal runat="server" ID="txtScopeName" Mode="Encode"></asp:Literal>
			</div>
		</div>
		<div class="pc-search-box-wrapper">
			<soa:DeluxeSearch ID="DeluxeSearch" runat="server" CssClass="deluxe-search deluxe-left"
				HasCategory="False" SearchFieldTemplate="CONTAINS(${DataField}$, ${Data}$)" SearchField="O.SearchContent"
				OnSearching="SearchButtonClick" OnConditionClick="onconditionClick" CustomSearchContainerControlID="advSearchPanel"
				HasAdvanced="true">
			</soa:DeluxeSearch>
			<soa:DataBindingControl runat="server" ID="searchBinding" AllowClientCollectData="True">
				<ItemBindings>
					<soa:DataBindingItem ControlID="sfCodeName" DataPropertyName="CodeName" />
					<soa:DataBindingItem ControlID="sfSchemaTypeOption" DataPropertyName="SchemaTypeOption"
						ControlPropertyName="SelectedIndex" />
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
							<label class="pc-label" for="sfSchemaTypeOption">
								查找类型</label><asp:DropDownList ID="sfSchemaTypeOption" runat="server">
									<asp:ListItem Text="全部" Value="0" Selected="True" />
									<asp:ListItem Text="人员" Value="1" />
									<asp:ListItem Text="群组" Value="2" />
									<asp:ListItem Text="组织" Value="3" />
								</asp:DropDownList>
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
							OnClientClick="return ($pc.getEnabled(this) && !!showBatchEditor());">属性</asp:LinkButton>
					</li>
				</ul>
			</div>
			<div class="pc-grid-container">
				<mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
					AllowPaging="True" AllowSorting="True" ShowCheckBoxes="true" Category="PermissionCenter"
					GridTitle="查找" DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title"
					PagerSettings-Position="Bottom" DataSourceMaxRow="0" ShowExportControl="false"
					TitleColor="141, 143, 149" TitleFontSize="Large" OnRowCommand="HandleRowCommand"
					OnSelectAllCheckBoxClick="toggleMenu" OnSelectCheckBoxClick="toggleMenu">
					<EmptyDataTemplate>
						请修改搜索条件后点击搜索按钮
					</EmptyDataTemplate>
					<HeaderStyle CssClass="head" />
					<Columns>
						<asp:TemplateField HeaderText="名称" SortExpression="O.Name">
							<ItemTemplate>
								<div>
									<i id="ico" class='<%#Eval("SchemaType","pc-icon16 {0}") %>'></i>
									<pc:SchemaHyperLink runat="server" ID="hl2" Text='<%# Server.HtmlEncode((string) Eval("Name")) %>'
										OUViewMode="false" ObjectID='<%#Eval("ID") %>' ObjectSchemaType='<%#Eval("SchemaType") %>'
										CssClass="pc-item-link" data-id='<%# PermissionCenter.Util.HtmlAttributeEncode((string) Eval("ID")) %>'
										Target="_parent" />
								</div>
								<div>
									<div id="divActions" class="pc-action-tray" runat="server" visible='<%# this.gridMain.ExportingDeluxeGrid == false %>'>
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
								<asp:Label ID="Label1" runat="server" Text='<%# Server.HtmlEncode(this.SchemaTypeToString((string)Eval("SchemaType"))) %>'></asp:Label>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="创建者" SortExpression="O.CreatorName">
							<ItemTemplate>
								<soa:UserPresence runat="server" ID="userPresence" ShowUserDisplayName="true" ShowUserIcon="false"
									UserID='<%#Eval("CreatorID") %>' UserDisplayName='<%# Server.HtmlEncode(Eval("CreatorName").ToString()) %>' />
							</ItemTemplate>
						</asp:TemplateField>
						<pc:SubRowBoundField DataField="CreateDate" HtmlEncode="true" HeaderText="创建日期" SortExpression="O.CreateDate"
							DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" SubColSpan="7">
							<SubRowTemplate>
								<span class="pc-label">位置</span>
								<soa:OuNavigator ID="OuNavigator1" runat="server" StartLevel="0" TerminalVisible="true"
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
				<soa:DeluxeObjectDataSource ID="dataSourceMain" runat="server" EnablePaging="True"
					TypeName="MCS.Library.SOA.DataObjects.Security.DataSources.SchemaOrgDescendantsDataSource"
					EnableViewState="false" OnSelecting="dataSourceMain_Selecting">
				</soa:DeluxeObjectDataSource>
			</div>
		</div>
	</div>
	<div style="display: none">
		<asp:HiddenField ID="hfOuId" runat="server" EnableViewState="false" />
		<asp:HiddenField ID="hfOuParentId" runat="server" />
	</div>
	<pc:Footer ID="footer" runat="server" />
	</form>
	<script type="text/javascript">
		$pc.ui.listMenuBehavior("listMenu");
		$pc.ui.gridBehavior("gridMain", "hover");

		if (window.parent.showLoader)
			window.parent.showLoader(false);

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

		Sys.Application.add_load(function () {
			toggleMenu();
		});
	</script>
</body>
</html>
