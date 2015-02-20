<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ADLogDetail.aspx.cs" Inherits="PermissionCenter.ADLogDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>AD同步日志</title>
	<base target="_self" />
	<pc:HeaderControl ID="hc1" runat="server" />
	<style type="text/css">
		.pc-label
		{
			display: inline-block;
			width: 100px;
		}
		#gridMain
		{
			table-layout:fixed;
		}
		.detailPan
		{
			height: 20px;
			cursor: pointer;
		}
		
		.toggle .detailPan
		{
			height: auto;
		}
	</style>
</head>
<body>
	<form id="form1" runat="server">
	<pc:BannerNotice ID="notice" runat="server"></pc:BannerNotice>
	<div class="pc-container5">
		<div class="pc-listmenu-container">
		</div>
		<div class="pc-grid-container">
			<mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
				AllowPaging="True" AllowSorting="True" ShowCheckBoxes="false" Category="PermissionCenter"
				GridTitle="同步详情" DataKeyNames="LogDetailID" CssClass="dataList pc-datagrid pc-time-list"
				TitleCssClass="title" ShowExportControl="true" PagerSettings-Position="Bottom"
				DataSourceMaxRow="0" TitleColor="141, 143, 149" TitleFontSize="Large" OnRowCommand="HandleRowCommand">
				<EmptyDataTemplate>
					暂时没有您需要的数据
				</EmptyDataTemplate>
				<HeaderStyle CssClass="head" />
				<Columns>
					<asp:TemplateField>
						<HeaderTemplate>
							<mcs:GridColumnSorter runat="server" ID="colSorter" DefaultOrderName="时间" PreventRenderChildren='<%# this.gridMain.ExportingDeluxeGrid  %>'>
							<SortItems >
							<mcs:SortItem SortExpression="CreateTime" Text="时间" />
							<mcs:SortItem SortExpression="ActionName" Text="动作" />
							<mcs:SortItem SortExpression="SCObjectName" Text="权限中心对象名" />
							<mcs:SortItem SortExpression="ADObjectName" Text="AD对象名" />

							</SortItems>
							</mcs:GridColumnSorter>
						</HeaderTemplate>
						<ItemTemplate>
							<div class="pc-time-group">
								<div style="float: left">
									<span class="pc-lead"></span>
									<%#Eval("CreateTime", "{0:yyyy-MM-dd HH:mm:ss}")%>
								</div>
								<div style="text-align: right">
									<span>
										<%# Server.HtmlEncode( Eval("ActionName").ToString()) %>
									</span>
								</div>
							</div>
							<div>
								<span class="pc-label">权限中心对象</span><span><%# Server.HtmlEncode( Eval("SCObjectName").ToString()) %>(<%# Server.HtmlEncode( Eval("SCObjectID").ToString()) %>)</span>
							</div>
							<div>
								<span class="pc-label">AD对象</span><span><%# Server.HtmlEncode( Eval("ADObjectName").ToString()) %>(<%# Server.HtmlEncode( Eval("ADObjectID").ToString()) %>)</span>
							</div>
							<div class="detailPan">
								<pre><%# Server.HtmlEncode(Eval("Detail").ToString())%></pre>
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
		TypeName="MCS.Library.SOA.DataObjects.Security.DataSources.ADSyncLogDetailDataSource"
		EnableViewState="false" OnSelecting="dataSourceMain_Selecting">
		<SelectParameters>
			<asp:QueryStringParameter QueryStringField="syncID" Name="syncID" />
		</SelectParameters>
	</soa:DeluxeObjectDataSource>
	</form>
	<script type="text/javascript">
		if (window.parent.showLoader)
			window.parent.showLoader(false);
		$pc.ui.gridBehavior("gridMain", "hover");
		$pc.ui.gridBehavior("gridMain", "toggle");
		$pc.ui.traceWindowWidth();

		//$pc.ui.listMenuBehavior("listMenu");

		function shuttle(elem) {
			var time = $pc.getAttr(elem, "data-time");
			if (time) {
				window.top.shuttle.apply(window.top, time);
			}

			return false;
		}
	</script>
</body>
</html>
