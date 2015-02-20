<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrgSearchDialog.aspx.cs"
	Inherits="PermissionCenter.Dialogs.OrgSearchDialog" %>

<%--
本对话框可以通过dialogArgument传入参数
fillElem:一个接受选定项的input元素


--%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" scroll="no" style="overflow: hidden">
<head runat="server">
	<title>权限中心-组织搜索</title>
	<link rel="icon" href="../favicon.ico" type="image/x-icon" />
	<link rel="Shortcut Icon" href="../favicon.ico" />
	<link href="../styles/dlg.css" rel="stylesheet" type="text/css" />
	<base target="_self" />
	<pc:HeaderControl ID="HeaderControl1" runat="server">
	</pc:HeaderControl>
</head>
<body>
	<form id="form1" runat="server">
	<div class="pcdlg-sky">
		<h1 class="pc-caption">
			搜索组织 <span runat="server" id="panMode" style="position: absolute; right: 5px; top: 2px;">
				<asp:LinkButton runat="server" ID="toggleButton" CssClass="pc-toggler-hl" OnClick="ToggleModeHandler"><i></i></asp:LinkButton>
			</span>
			<%--<asp:DropDownList runat="server" ID="lstMode" OnSelectedIndexChanged="ToggleModeHandler"
				AutoPostBack="True">
				<asp:ListItem Text="列表模式" />
				<asp:ListItem Text="分层模式" />
			</asp:DropDownList>--%>
		</h1>
	</div>
	<div class="pcdlg-content">
		<div>
			<asp:HiddenField ID="actionData" runat="server" EnableViewState="False" />
			<input type="hidden" runat="server" id="hfMode" />
			<input type="hidden" runat="server" id="hfGod" />
			<input type="hidden" runat="server" id="hfSingle" />
			<asp:HiddenField runat="server" ID="hfExcludes" />
			&nbsp;
		</div>
		<pc:BannerNotice ID="notice" runat="server" />
		<div>
			<asp:MultiView ID="mainView" ActiveViewIndex="0" runat="server">
				<asp:View runat="server">
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
					<div class="pc-container5" style="overflow-x: hidden; clear: both">
						<div id="noselpromptgrid" class="pc-required pc-hide" style="position: fixed; top: 30px;
							right: 5px;">
							不可以选择这个节点
						</div>
						<div class="pc-grid-container">
							<asp:MultiView ActiveViewIndex="0" runat="server" ID="views">
								<asp:View runat="server">
									<mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
										AllowPaging="True" AllowSorting="True" ShowCheckBoxes="True" Category="PermissionCenter"
										GridTitle="组织" DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title"
										PagerSettings-Position="Bottom" DataSourceMaxRow="0" TitleColor="141, 143, 149"
										TitleFontSize="Large" OnRowDataBound="HandleRowBound" OnSelectCheckBoxClick="onGridCheck">
										<EmptyDataTemplate>
											暂时没有您需要的数据
										</EmptyDataTemplate>
										<HeaderStyle CssClass="head" />
										<Columns>
											<asp:TemplateField>
												<HeaderTemplate>
													<mcs:GridColumnSorter runat="server" ID="colSorter" DefaultOrderName="名称" DockPosition="Left">
							<SortItems>
							<mcs:SortItem SortExpression="Name" Text="名称" />
							<mcs:SortItem SortExpression="DisplayName" Text="显示名称" />
							<mcs:SortItem SortExpression="CodeName" Text="代码名称" />
							</SortItems>
													</mcs:GridColumnSorter>
												</HeaderTemplate>
												<ItemTemplate>
													<div>
														<pc:SchemaHyperLink runat="server" ID="hl1" CssClass="pc-item-link" data-id='<%# PermissionCenter.Util.HtmlAttributeEncode((string) Eval("ID")) %>'
															Target="_self" ObjectID='<%#Eval("ID") %>' ObjectSchemaType='<%#Eval("SchemaType") %>'>
												<i class="pc-icon16 Organizations"></i><%# Server.HtmlEncode((string) Eval("Name")) %>
														</pc:SchemaHyperLink>
													</div>
													<div>
														<%# Server.HtmlEncode((string)Eval("DisplayName")) %>
													</div>
													<div>
														<%# Server.HtmlEncode((string)Eval("CodeName")) %>
													</div>
													<div>
														<div id="divActions" class="pc-action-tray" runat="server">
															<asp:LinkButton ID="lnkEdit" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
																OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
															<asp:HyperLink ID="lnkAcl" runat="server" CssClass="pc-item-cmd" onclick="return $pc.modalPopup(this);"
																Target="_blank" NavigateUrl='<%#Eval("ID","~/dialogs/AclEdit.aspx?id={0}") %>'>授权控制</asp:HyperLink>
															<asp:HyperLink ID="lnkHistory" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
																Target="_blank" onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/ObjectHistoryLog.aspx?id={0}") %>'>历史</asp:HyperLink>
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
											<asp:BoundField DataField="CreateDate" HtmlEncode="true" HeaderText="创建日期" SortExpression="O.CreateDate"
												DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
											<asp:TemplateField HeaderText="位置" ControlStyle-CssClass="pathCol">
												<ItemTemplate>
													<div data-path='<%# PermissionCenter.Util.HtmlAttributeEncode(Eval("FullPath").ToString()) %>'>
														<soa:OuNavigator ID="navPath" runat="server" StartLevel="0" SplitterVisible="true"
															LinkDataTag="data-pid" Target="_blank" NavigateUrlFormat="~/lists/OUExplorer.aspx?ou={0}"
															LinkCssClass="pc-item-link" TerminalVisible="false" ObjectID='<%#Eval("ID") %>'>
														</soa:OuNavigator>
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
										AllowPaging="True" AllowSorting="True" ShowCheckBoxes="True" Category="PermissionCenter"
										GridTitle="组织" DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title"
										PagerSettings-Position="Bottom" DataSourceMaxRow="0" TitleColor="141, 143, 149"
										TitleFontSize="Large" OnRowDataBound="HandleRowBound" OnSelectCheckBoxClick="onGridCheck">
										<EmptyDataTemplate>
											暂时没有您需要的数据
										</EmptyDataTemplate>
										<HeaderStyle CssClass="head" />
										<Columns>
											<asp:TemplateField HeaderText="名称" SortExpression="Name">
												<ItemTemplate>
													<div>
														<pc:SchemaHyperLink runat="server" ID="hl1" CssClass="pc-item-link" data-id='<%# PermissionCenter.Util.HtmlAttributeEncode((string) Eval("ID")) %>'
															Target="_self" ObjectID='<%#Eval("ID") %>' ObjectSchemaType='<%#Eval("SchemaType") %>'>
												<i class="pc-icon16 Organizations"></i><%# Server.HtmlEncode((string) Eval("Name")) %>
														</pc:SchemaHyperLink>
													</div>
													<div>
														<div id="divActions" class="pc-action-tray" runat="server">
															<asp:LinkButton ID="lnkEdit" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
																OnClientClick="return $pc.popups.editProperty(this);" OnClick="RefreshList">基本属性</asp:LinkButton>
															<asp:HyperLink ID="lnkAcl" runat="server" CssClass="pc-item-cmd" onclick="return $pc.modalPopup(this);"
																Target="_blank" NavigateUrl='<%#Eval("ID","~/dialogs/AclEdit.aspx?id={0}") %>'>授权控制</asp:HyperLink>
															<asp:HyperLink ID="lnkHistory" runat="server" CssClass="pc-item-cmd" data-id='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID"))%>'
																Target="_blank" onclick="return $pc.modalPopup(this);" NavigateUrl='<%#Eval("ID","~/lists/ObjectHistoryLog.aspx?id={0}") %>'>历史</asp:HyperLink>
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
											<asp:BoundField DataField="CreateDate" HtmlEncode="true" HeaderText="创建日期" SortExpression="O.CreateDate"
												DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
											<asp:TemplateField HeaderText="位置" ControlStyle-CssClass="pathCol">
												<ItemTemplate>
													<div data-path='<%# PermissionCenter.Util.HtmlAttributeEncode(Eval("FullPath").ToString()) %>'>
														<soa:OuNavigator ID="navPath" runat="server" StartLevel="0" SplitterVisible="true"
															LinkDataTag="data-pid" Target="_blank" NavigateUrlFormat="~/lists/OUExplorer.aspx?ou={0}"
															LinkCssClass="pc-item-link" TerminalVisible="false" ObjectID='<%#Eval("ID") %>'>
														</soa:OuNavigator>
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
					<soa:DeluxeObjectDataSource ID="dataSourceMain" runat="server" EnablePaging="True"
						TypeName="MCS.Library.SOA.DataObjects.Security.DataSources.SchemaOrgQueryDataSource"
						EnableViewState="false" OnSelecting="dataSourceMain_Selecting" SelectCountMethod="GetQueryCount"
						SelectMethod="Query">
					</soa:DeluxeObjectDataSource>
				</asp:View>
				<asp:View runat="server">
					<div id="noselprompt" class="pc-required pc-hide" style="position: fixed; top: 25px;
						right: 5px;">
						不可以选择这个节点
					</div>
					<mcs:DeluxeTree runat="server" CssClass="pc-mcstree" ID="tree" OnNodeSelecting="onNodeSelecting"
						OnGetChildrenData="tree_GetChildrenData" OnNodeCheckBoxBeforeClick="onBeforeCheckNode"
						OnNodeBeforeDataBind="onBeforeBindNode" OnNodeAfterExpand="onAfterExpandNode">
					</mcs:DeluxeTree>
				</asp:View>
			</asp:MultiView>
		</div>
	</div>
	<div class="pcdlg-floor">
		<div class="pcdlg-button-bar">
			<input type="button" class="pcdlg-button" accesskey="S" value="选定" onclick="return okClick();" />
			<input type="button" class="pcdlg-button" accesskey="C" value="取消" onclick="return cancelClick();" />
		</div>
	</div>
	</form>
	<script type="text/javascript">
		$pc.ui.gridBehavior("gridMain", "hover");
		$pc.ui.gridBehavior("grid2", "hover");
		$pc.ui.listMenuBehavior("listMenu");
		$pc.ui.hoverBehavior("toggleButton");
		$pc.ui.traceWindowWidth();


		function onconditionClick(sender, e) {
			var content = Sys.Serialization.JavaScriptSerializer.deserialize(e.ConditionContent);
			var bindingControl = $find("searchBinding");
			bindingControl.dataBind(content);
		}

		var excludeIds = document.getElementById("hfExcludes").value;
		if (excludeIds.length) {
			excludeIds = Sys.Serialization.JavaScriptSerializer.deserialize(excludeIds);
		} else {
			excludeIds = null;
		}

		function onNodeSelecting(sender, e) {
			if (e.node.get_extendedData() == "noselect") {
				e.cancel = true;
				$pc.show("noselprompt");
			} else {
				$pc.hide("noselprompt");
			}
			//			$get("frmView").src = $pc.appRoot + "lists/OUExplorerView.aspx?ou=" + e.node.get_value();
			//			showLoader(true);
			//			$get("lastVisitOrg")['value'] = e.node.get_value();
		}

		function onAfterExpandNode(s, e) {
			if (excludeIds) {
				if (e.node.get_hasChildNodes()) {
					var nodesToRemove = [];
					var hit = false;
					var v = null;
					for (var n = e.node.get_firstChild(); n; n = n.get_nextNode()) {
						v = n.get_value();
						hit = false;
						for (var k = excludeIds.length - 1; k >= 0; k--) {
							if (v == excludeIds[k]) {
								hit = true;
								break;
							}
						}

						if (hit) {
							nodesToRemove.push(n);
						}
					}

					for (k = nodesToRemove.length - 1; k >= 0; k--) {
						e.node.removeChild(nodesToRemove[k]);
					}

					delete nodesToRemove;
					nodesToRemove = null;
				}
			}

		}

		function gridInit(grid) {

			// 表格的最后一列必须是OUNavigator
			function configRow(row) {
				var checkbox = null;
				var td = null;

				td = row.firstChild;

				if (td.nodeType === 1 && td.nodeName.toUpperCase() == "TD") {
					checkbox = td.firstChild;
					if (checkbox != null && checkbox.nodeType === 1 && checkbox.nodeName.toUpperCase() == "INPUT") {
						var span = td.parentNode.lastChild.firstChild.firstChild;
						var node = null;
						$pc.console.info('span=' + span.nodeName);
						ids = [];

						if (span != null && span.tagName.toUpperCase() == "SPAN") {
							for (node = span.firstChild; node; node = node.nextSibling) {
								if (node.nodeType === 1 && node.tagName.toUpperCase() == "A") {
									ids.push($pc.getAttr(node, "data-pid"));
								}
							}
						}

						var hit = false;
						for (var j = excludeIds.length - 1; j >= 0; j--) {
							for (var i = ids.length - 1; i >= 0; i--) {
								if (ids[i] == excludeIds[j]) {
									hit = true;
								}
							}
						}

						if (hit) {
							$pc.setAttr(checkbox, "disabled", "disabled");
						}
					}
				}


				td = null;
				checkbox = null;
			}

			if (grid) {
				var tbody = grid.firstChild;
				var tbsuccess = false;
				if (grid.nodeType === 1 && grid.tagName.toUpperCase() == "TABLE") {
					for (tbody = grid.firstChild; tbody; tbody = tbody.nextSibling) {
						if (tbody.nodeType === 1 && tbody.tagName.toUpperCase() == "TBODY") {
							tbsuccess = true;
							break;
						}
					}
				}

				if (tbsuccess) {
					var row = null;
					for (row = tbody.firstChild; row; row = row.nextSibling) {
						if (row.nodeType == 1 && row.tagName.toUpperCase() == "TR") {
							if ($pc.hasClass(row, "item") || $pc.hasClass(row, "aitem")) {
								configRow(row);
							}
						}
					}
				}

				tbody = null;
			}
		}

		if (excludeIds) {
			Sys.Application.add_init(function () {
				gridInit($pc.get("gridMain") || $pc.get("grid2"));
			});
		}


		function onBeforeBindNode(s, e) {
			if (excludeIds) {
				var hit = false;
				var v = e.node.get_value();
				for (var i = excludeIds.length - 1; i >= 0; i--) {
					if (v == excludeIds[i]) {
						hit = true;
						break;
					}
				}


				if (hit) {
					e.node.set_extendedData("noselect");
					e.node.set_cssClass("pc-node-exclude");
					e.node.set_selectedCssClass("pc-node-exclude-select");
				}
			}
		}

		function onBeforeCheckNode(s, e) {
			if (e.node.get_extendedData() == "noselect") {
				e.cancel = true;
				$pc.show("noselprompt");
			} else {
				$pc.hide("noselprompt");
			}
		}

		function onGridCheck(s, e) {

		}

		function okClick() {

			var keys = null;
			if (document.getElementById("hfMode").value == '0') {
				keys = ($find("gridMain") || $find('grid2')).get_clientSelectedKeys();
			} else {
				keys = [];

				if (document.getElementById("hfSingle").value == "1") {
					//单选
					var n = $find("tree").get_selectedNode();
					if (n) {
						keys.push(n.get_value());
					}
				} else {
					var allObj = $find("tree").get_multiSelectedNodes();
					for (var i = allObj.length - 1; i >= 0; i--) {
						keys.push(allObj[i].get_value());
					}
				}

			}
			if (keys.length > 0) {
				if (document.getElementById("hfSingle").value == "1" && keys.length != 1) {
					alert("当前限制只能选择一个目标");
				} else {
					if (typeof (window.dialogArguments) == 'object' && typeof (window.dialogArguments.fillElem) == "object") {
						window.dialogArguments.fillElem.value = keys.join(",");
						window.returnValue = true;
					} else {
						window.returnValue = keys.join(",");
					}
					window.close();
				}
			}
		}

		function cancelClick() {
			window.returnValue = '';
			window.close();
		}
	</script>
</body>
</html>
