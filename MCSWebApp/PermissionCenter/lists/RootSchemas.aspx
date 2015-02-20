<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RootSchemas.aspx.cs" Inherits="PermissionCenter.RootSchemas" %>

<%--怪异模式测试时将文档声明置于服务器注释块内--%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>权限中心-所有人员</title>
	<link rel="icon" href="../favicon.ico" type="image/x-icon" />
	<link rel="Shortcut Icon" href="../favicon.ico" />
	<base target="_self" />
	<pc:HeaderControl runat="server">
	</pc:HeaderControl>
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
	<div class="pc-frame-header">
		<pc:Banner ID="pcBanner" runat="server" ActiveMenuIndex="1" OnTimePointChanged="RefreshList" />
	</div>
	<pc:SceneControl ID="SceneControl1" runat="server">
	</pc:SceneControl>
	<pc:BannerNotice ID="notice" runat="server"></pc:BannerNotice>
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
						<asp:LinkButton runat="server" ID="btnNewRoot" CssClass="button pc-list-cmd" OnClientClick="return ($pc.getEnabled(this) && $pc.popups.newOrg(this));"
							OnClick="RefreshList" OnPreRender="btnNewRoot_PreRender">新建组织架构</asp:LinkButton>
					</li>
					<li>
						<asp:LinkButton runat="server" CssClass="pc-list-cmd" ID="btnDeleteSelected" Text="删除"
							OnClientClick="return ($pc.getEnabled(this) && $pc.popups.batchDelete('gridMain','Orgnizations'));"
							OnClick="BatchDelete"></asp:LinkButton>
					</li>
					<li>
						<asp:LinkButton runat="server" CssClass="list-cmd" ID="btnImport" Text="导入" OnClientClick="return ($pc.getEnabled(this) && invokeImport() && false);"
							OnClick="RefreshList"></asp:LinkButton>
					</li>
					<li>
						<asp:LinkButton runat="server" CssClass="list-cmd" ID="btnExport" Text="导出" OnClientClick="return ($pc.getEnabled(this) && invokeExport() && false);"></asp:LinkButton>
					</li>
				</ul>
			</div>
			<div class="pc-clear pc-height2">
			</div>
			<div class="pc-grid-container">
				<mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
					AllowPaging="True" AllowSorting="True" ShowCheckBoxes="True" Category="PermissionCenter"
					GridTitle="组织机构" DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title"
					ShowExportControl="true" PagerSettings-Position="Bottom" DataSourceMaxRow="0"
					 TitleColor="141, 143, 149" TitleFontSize="Large"
					OnSorting="DoSortingList" OnRowCommand="HandleRowCommand">
					<EmptyDataTemplate>
						暂时没有您需要的数据
					</EmptyDataTemplate>
					<HeaderStyle CssClass="head" />
					<Columns>
						<asp:TemplateField HeaderText="名称" SortExpression="Name">
							<ItemTemplate>
								<div>
									<i id="ico" runat="server" class="pc-icon16 Organizations"></i>
									<asp:HyperLink ID="HyperLink1" NavigateUrl='<%#"~/lists/OUExplorer.aspx?ou="+Server.UrlEncode((string)Eval("ID"))%>'
										CssClass="pc-item-link" runat="server" Target="_blank" Text='<%#Server.HtmlEncode((string)Eval("Name")) %>' />
								</div>
								<div>
									<div id="d" class="pc-action-tray" runat="server" visible='<%# this.gridMain.ExportingDeluxeGrid == false %>'>
										<asp:LinkButton ID="lnkEdit" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
											OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
										<asp:HyperLink ID="lnkAcl" runat="server" CssClass="pc-item-cmd" onclick="return $pc.modalPopup(this);"
											Target="_blank" NavigateUrl='<%#Eval("ID","~/dialogs/AclEdit.aspx?id={0}") %>'>授权控制</asp:HyperLink>
										<asp:LinkButton ID="lnkItemDelete" runat="server" Text="删除" CssClass="pc-item-cmd"
											CommandName="DeleteItem" Enabled='<%# this.DeleteEnabled %>' CommandArgument='<%#Eval("ID") %>'
											OnClientClick="return ($pc.getEnabled(this) && $pc.confirmDelete('确定要删除?'));"></asp:LinkButton>
										<asp:HyperLink ID="lnkHistory" runat="server" CssClass="pc-item-cmd" Target="_blank"
											onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/ObjectHistoryLog.aspx?id={0}") %>'>历史</asp:HyperLink>
									</div>
								</div>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:BoundField HeaderText="显示名称" SortExpression="DisplayName" HtmlEncode="true"
							DataField="DisplayName" />
						<asp:BoundField HeaderText="代码名称" SortExpression="CodeName" HtmlEncode="true" DataField="CodeName" />
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
			</div>
		</div>
	</div>
	<soa:DeluxeObjectDataSource ID="dataSourceMain" runat="server" EnablePaging="True"
		TypeName="MCS.Library.SOA.DataObjects.Security.DataSources.SchemaRootOrgDataSource"
		EnableViewState="false" OnSelecting="dataSourceMain_Selecting">
	</soa:DeluxeObjectDataSource>
	<pc:Footer ID="footer" runat="server" />
	<soa:UploadProgressControl runat="server" ID="ctlUpload" DialogHeaderText="导入组织维度数据(xml)"
		DialogTitle="导入" OnDoUploadProgress="DoFileUpload" OnLoadingDialogContent="ctlUpload_LoadingDialogContent"
		OnClientCompleted="postImportProcess" />
	<asp:HiddenField runat="server" ID="hfOuId" />
	</form>
	<script type="text/javascript">
		$pc.ui.listMenuBehavior("listMenu");
		$pc.ui.gridBehavior("gridMain", "hover");

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
			var grid = $find("gridMain");
			if (grid) {
				var keys = grid.get_clientSelectedKeys();
				if (keys.length > 0) {
					$pc.postViaIFrame($pc.appRoot + "Handlers/ObjectExport.ashx", { context: "OguObjects", id: keys, parentId: document.getElementById("hfOuId").value });
				}
			}
			grid = false;
			return false;
		}
	</script>
</body>
</html>
