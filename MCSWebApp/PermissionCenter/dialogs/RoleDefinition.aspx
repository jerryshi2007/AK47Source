<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoleDefinition.aspx.cs"
	Inherits="PermissionCenter.RoleDefinition" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>权限中心-角色功能定义</title>
	<base target="_self" />
	<link rel="icon" href="../favicon.ico" type="image/x-icon" />
	<link rel="Shortcut Icon" href="../favicon.ico" />
	<link href="../styles/dlg.css" rel="stylesheet" type="text/css" />
	<pc:HeaderControl runat="server">
	</pc:HeaderControl>
	<style type="text/css">
		
	</style>
</head>
<body>
	<form id="form1" runat="server">
	<asp:Literal ID="preScript" runat="server" EnableViewState="false" />
	<soa:DataBindingControl runat="server" ID="binding1">
		<ItemBindings>
			<soa:DataBindingItem DataPropertyName="VisibleName" ControlID="roleDisplayName" ControlPropertyName="Text"
				Direction="DataToControl" />
		</ItemBindings>
	</soa:DataBindingControl>
	<div class="pcdlg-sky">
		<h1 class="pc-caption">
			<span>
				<asp:Literal Text="某角色的" runat="server" ID="roleDisplayName" Mode="Encode" />
			</span>的角色功能定义<span class="pc-timepointmark"><mcs:TimePointDisplayControl ID="TimePointDisplayControl1"
				runat="server">
			</mcs:TimePointDisplayControl>
			</span>
		</h1>
	</div>
	<div class="pcdlg-content">
		<pc:BannerNotice ID="notice" runat="server"></pc:BannerNotice>
		<pc:SceneControl ID="SceneControl1" runat="server">
		</pc:SceneControl>
		<div class="pc-search-box-wrapper">
			<soa:DeluxeSearch ID="DeluxeSearch" runat="server" CssClass="deluxe-search deluxe-left"
				HasCategory="False" SearchFieldTemplate="CONTAINS(${DataField}$, ${Data}$)" SearchField="P1.SearchContent"
				OnSearching="SearchButtonClick">
			</soa:DeluxeSearch>
		</div>
		<div class="pc-container5">
			<div class="pc-listmenu-container">
				<ul class="pc-listmenu" id="listMenu">
				</ul>
			</div>
			<div class="pc-grid-container">
				<mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
					AllowPaging="True" AllowSorting="True" Category="PermissionCenter" GridTitle="角色功能定义"
					DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title" PagerSettings-Position="Bottom"
					DataSourceMaxRow="0"  TitleColor="141, 143, 149" TitleFontSize="Large"
					ShowExportControl="False">
					<EmptyDataTemplate>
						暂时没有您需要的数据
					</EmptyDataTemplate>
					<HeaderStyle CssClass="head" />
					<Columns>
						<asp:TemplateField>
							<HeaderTemplate>
								<asp:CheckBox ID="CheckBox1" Text="全选" runat="server" onclick="checkAll(this.checked)" />
							</HeaderTemplate>
							<ItemTemplate>
								<input type="checkbox" data-key='<%#PermissionCenter.Util.HtmlAttributeEncode((string)Eval("ID")) %>'
									data-apply='<%#PermissionCenter.Util.HtmlAttributeEncode(Eval("ApplyID").ToString()) %>' />
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="名称">
							<ItemTemplate>
								<i class="pc-icon16 Permissions"></i>
								<%# Server.HtmlEncode((string)Eval("Name")) %>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:BoundField DataField="DisplayName" HtmlEncode="true" HeaderText="显示名称" />
						<asp:BoundField DataField="CodeName" HtmlEncode="true" HeaderText="代码名称" />
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
			TypeName="MCS.Library.SOA.DataObjects.Security.DataSources.SchemaRoleDifinitionDataSource"
			EnableViewState="false" onselecting="dataSourceMain_Selecting">
			<SelectParameters>
				<asp:QueryStringParameter DbType="String" Name="roleId" QueryStringField="role" />
			</SelectParameters>
		</soa:DeluxeObjectDataSource>
		<pc:Footer ID="footer" runat="server" />
		<asp:HiddenField runat="server" ID="hfAdded" EnableViewState="true" />
		<asp:HiddenField runat="server" ID="hfRemoved" EnableViewState="true" />
	</div>
	<div class="pcdlg-floor">
		<div class="pcdlg-button-bar">
			<soa:SubmitButton runat="server" ID="btnSave" OnClick="SaveClick" OnClientClick="return doValidation();"
				AccessKey="S" Text="保存(S)" CssClass="pcdlg-button btn-def" RelativeControlID="buttonSave"
				PopupCaption="正在保存..." />
			<%--<asp:Button Text="保存(S)" runat="server" ID="btnSave" AccessKey="S" CssClass="pcdlg-button btn-def"
				OnClick="SaveClick" OnClientClick="return doValidation()" />--%>
			<input type="button" accesskey="C" class="pcdlg-button btn-cancel" onclick="return onCancelClick();"
				value="关闭(C)" />
		</div>
	</div>
	</form>
	<script type="text/javascript">
		$pc.ui.listMenuBehavior("listMenu");
		$pc.ui.gridBehavior("gridMain", "hover");
		$pc.ui.traceWindowWidth();
		(function () {
			var toBeAdded = $get("hfAdded").value;
			var toBeRemoved = $get("hfRemoved").value;
			toBeAdded = (toBeAdded && toBeAdded.length > 0) ? toBeAdded.split(",") : [];
			toBeRemoved = (toBeRemoved && toBeRemoved.length > 0) ? toBeRemoved.split(",") : [];
			function initElem(elem) {
				//加载时初始化数据
				var key, applyKey, status, i;
				if (elem.getAttribute) {
					key = elem.getAttribute("data-key");
					applyKey = elem.getAttribute("data-apply");
					status = elem.checked;
				} else {
					key = elem['data-key'];
					applyKey = elem['data-apply'];
					status = elem.checked;
				}
				var assigned = ((typeof applyKey === 'string') && applyKey.length > 0);
				var changed = false;
				if (assigned) {
					//检查是否被设置取消
					for (i = toBeRemoved.length; i >= 0; i--) {
						if (toBeRemoved[i] == key) {
							changed = true;
							break;
						}
					}
				} else {
					//检查是否被设置添加
					for (i = toBeAdded.length; i >= 0; i--) {
						if (toBeAdded[i] == key) {
							changed = true;
							break;
						}
					}
				}
				elem.checked = changed ? !assigned : assigned;
				if (changed) {
					$pc.addClass(elem.parentNode, "pc-changed");
				}
			}

			function pushToRemove(key) {
				for (var i = toBeRemoved.length - 1; i >= 0; i--) {
					if (toBeRemoved[i] == key) {
						return;
					}
				}
				toBeRemoved.push(key);
				$get("hfRemoved").value = toBeRemoved.join(",");
			}

			function pushToAdd(key) {
				for (var i = toBeAdded.length - 1; i >= 0; i--) {
					if (toBeAdded[i] == key) {
						return;
					}
				}
				toBeAdded.push(key);
				$get("hfAdded").value = toBeAdded.join(",");

			}

			function popFromRemove(key) {
				for (var i = toBeRemoved.length - 1; i >= 0; i--) {
					if (toBeRemoved[i] == key) {
						toBeRemoved.splice(i, 1);
						$get("hfRemoved").value = toBeRemoved.join(",");
						break;
					}
				}
			}

			function popFromAdd(key) {
				for (var i = toBeAdded.length - 1; i >= 0; i--) {
					if (toBeAdded[i] == key) {
						toBeAdded.splice(i, 1);
						$get("hfAdded").value = toBeAdded.join(",");
						break;
					}
				}
			}

			function clickHandler() {
				//处理用户点击
				var key, applyKey, status;
				if (this.getAttribute) {
					key = this.getAttribute("data-key");
					applyKey = this.getAttribute("data-apply");
					status = this.checked;
				} else {
					key = this['data-key'];
					applyKey = this['data-apply'];
					status = this.checked;
				}
				var assigned = (applyKey && applyKey.length > 0); //检查是否已经被设置了值
				if (assigned) {
					if (status) { //如果被勾选，清除remove列表，并设置状态
						popFromRemove(key);
						$pc.removeClass(this.parentNode, "pc-changed");
					} else { //如果清除勾选，添加到remove列表，并设置状态
						pushToRemove(key);
						$pc.addClass(this.parentNode, "pc-changed");
					}
				} else {
					if (status) { //如果被勾选，添加到Add列表，并设置状态
						pushToAdd(key);
						$pc.addClass(this.parentNode, "pc-changed");
					} else { //如果清除勾选，弹出Add列表，并设置状态
						popFromAdd(key);
						$pc.removeClass(this.parentNode, "pc-changed");
					}

				}

				$pc.console.info("Key:" + key + ", apply:" + applyKey + "chcked:" + status);
			}
			var inp, td, tr, tbody;
			for (tbody = $get("gridMain").firstChild; tbody; tbody = tbody.nextSibling) {
				if (tbody.nodeType === 1 && tbody.nodeName.toUpperCase() == "TBODY") {
					for (tr = tbody.firstChild; tr; tr = tr.nextSibling) {
						if (tr.nodeType === 1 && tr.nodeName.toUpperCase() == "TR" && (Sys.UI.DomElement.containsCssClass(tr, "item") || Sys.UI.DomElement.containsCssClass(tr, "aitem"))) {
							for (td = tr.firstChild; td; td = td.nextSibling) {
								if (td.nodeType === 1 && td.nodeName.toUpperCase() == 'TD') {
									for (inp = td.firstChild; inp; inp = inp.nextSibling) {
										if (inp.nodeType === 1 && inp.nodeName.toUpperCase() == "INPUT") {
											initElem(inp);
											$addHandler(inp, "click", Function.createDelegate(inp, clickHandler));
										}
									}
									break;
								}
							}
						}
					}
				}
			}
			inp = null;
			td = null;
			tr = null;
			tbody = null;

		})();

		function checkAll(markAll) {
			var inp, td, tr, tbody;
			for (tbody = $get("gridMain").firstChild; tbody; tbody = tbody.nextSibling) {
				if (tbody.nodeType === 1 && tbody.nodeName.toUpperCase() == "TBODY") {
					for (tr = tbody.firstChild; tr; tr = tr.nextSibling) {
						if (tr.nodeType === 1 && tr.nodeName.toUpperCase() == "TR" && (Sys.UI.DomElement.containsCssClass(tr, "item") || Sys.UI.DomElement.containsCssClass(tr, "aitem"))) {
							for (td = tr.firstChild; td; td = td.nextSibling) {
								if (td.nodeType === 1 && td.nodeName.toUpperCase() == 'TD') {
									for (inp = td.firstChild; inp; inp = inp.nextSibling) {
										if (inp.nodeType === 1 && inp.nodeName.toUpperCase() == "INPUT") {
											if (markAll != inp.checked)
												inp.click();
										}
									}
								}
							}
						}
					}
				}
			}
			inp = null;
			td = null;
			tr = null;
			tbody = null;
		}

		function doValidation() {
			var toBeAdded = $get("hfAdded").value;
			var toBeRemoved = $get("hfRemoved").value;
			if ((toBeAdded && toBeAdded.length > 0) || (toBeRemoved && toBeRemoved.length > 0)) {
				return true;
			} else {
				alert("未进行改动，没有必要保存。");
				return false;
			}
		}

		function onCancelClick() {
			this.close();
		}
	</script>
</body>
</html>
