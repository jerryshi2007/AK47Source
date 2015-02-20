<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GroupSearchDialog.aspx.cs"
	Inherits="PermissionCenter.Dialogs.GroupSearchDialog" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" scroll="no" style="overflow: hidden">
<head runat="server">
	<title>权限中心-选择群组</title>
	<link rel="icon" href="../favicon.ico" type="image/x-icon" />
	<link rel="Shortcut Icon" href="../favicon.ico" />
	<base target="_self" />
	<link href="../styles/dlg.css" rel="stylesheet" type="text/css" />
	<pc:HeaderControl ID="HeaderControl1" runat="server">
	</pc:HeaderControl>
</head>
<body>
	<form id="form1" runat="server">
	<div class="pcdlg-sky">
		<h1 class="pc-caption">
			选择群组</h1>
	</div>
	<div class="pcdlg-content">
		<pc:BannerNotice ID="notice" runat="server"></pc:BannerNotice>
		<div class="pc-search-box-wrapper">
			<soa:DeluxeSearch ID="DeluxeSearch" runat="server" CssClass="deluxe-search deluxe-left"
				HasCategory="False" SearchFieldTemplate="CONTAINS(${DataField}$, ${Data}$)" SearchField="G.SearchContent"
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
			<div class="pc-grid-container">
				<mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
					AllowPaging="True" AllowSorting="True" ShowCheckBoxes="True" Category="PermissionCenter"
					GridTitle="群组" DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title"
					PagerSettings-Position="Bottom" DataSourceMaxRow="0" 
					TitleColor="141, 143, 149" TitleFontSize="Large">
					<EmptyDataTemplate>
						暂时没有您需要的数据
					</EmptyDataTemplate>
					<HeaderStyle CssClass="head" />
					<Columns>
						<asp:TemplateField HeaderText="名称" SortExpression="Name">
							<ItemTemplate>
								<pc:SchemaHyperLink runat="server" ID="gp1" CssClass="pc-item-link" ObjectID='<%#Eval("ID") %>'
									ObjectSchemaType='<%#Eval("SchemaType") %>'>
								<i class="pc-icon16 Groups"></i>
									<%# Server.HtmlEncode((string)Eval("Name")) %>
								</pc:SchemaHyperLink>
								<div>
									<div class="pc-action-tray">
										<asp:LinkButton ID="lnkEdit" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
											OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
										<asp:HyperLink ID="lnkConstMembers" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
											onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/GroupConstMembers.aspx?id={0}") %>'
											Target="_blank">群组人员管理</asp:HyperLink>
										<asp:HyperLink ID="lnkHistory" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
											Target="_blank" onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/ObjectHistoryLog.aspx?id={0}") %>'>历史</asp:HyperLink>
									</div>
								</div>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:BoundField DataField="DisplayName" HtmlEncode="true" HeaderText="显示名称" SortExpression="DisplayName" />
						<asp:BoundField DataField="CodeName" HtmlEncode="true" HeaderText="代码名称" SortExpression="CodeName" />
						<asp:TemplateField HeaderText="位置">
							<ItemTemplate>
								<div>
									<soa:OuNavigator ID="navPath" runat="server" StartLevel="0" SplitterVisible="true"
										LinkCssClass="pc-item-link" Target="_blank" NavigateUrlFormat="~/lists/OUExplorer.aspx?ou={0}"
										TerminalVisible="true" ObjectID='<%#Eval("ID") %>'></soa:OuNavigator>
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
			</div>
		</div>
		<soa:DeluxeObjectDataSource ID="dataSourceMain" runat="server" EnablePaging="True"
			TypeName="MCS.Library.SOA.DataObjects.Security.DataSources.SchemaGroupDataSource"
			EnableViewState="false" OnSelecting="dataSourceMain_Selecting" OnSelected="dataSourceMain_Selected">
		</soa:DeluxeObjectDataSource>
		<pc:Footer ID="footer" runat="server" />
	</div>
	<div class="pcdlg-floor">
		<div class="pcdlg-button-bar">
			<asp:Button ID="Button1" Text="确定" runat="server" CssClass="pcdlg-button" OnClientClick="doConfirm()" /><input
				type="button" class="pcdlg-button" value="取消" onclick="doCancel();" />
		</div>
	</div>
	</form>
	<script type="text/javascript">
		$pc.ui.gridBehavior("gridMain", "hover");
		$pc.ui.traceWindowWidth();

		function onconditionClick(sender, e) {
			var content = Sys.Serialization.JavaScriptSerializer.deserialize(e.ConditionContent);
			var bindingControl = $find("searchBinding");
			bindingControl.dataBind(content);
		}

		function doConfirm() {
			var keys = $find("gridMain").get_clientSelectedKeys();
			if (keys && keys.length && keys.length > 0) {
				window.returnValue = keys.join(",");
				window.close();
			}
		}
		function doCancel() {
			window.returnValue = '';
			window.close();
		}
	</script>
</body>
</html>
