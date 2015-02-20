<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ADReverseLogDetail.aspx.cs"
	Inherits="PermissionCenter.ADReverseLogDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>AD反向同步日志</title>
	<base target="_self" />
	<pc:HeaderControl ID="hc1" runat="server" />
	<style type="text/css">
		.pc-title
		{
			margin: 5px;
		}
		#gridMain
		{
			table-layout: fixed;
		}
		.pc-title .pc-label
		{
			width: 100px;
			display: inline-block;
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
					<asp:TemplateField SortExpression="CreateTime" HeaderText="时间">
						<ItemTemplate>
							<div class="pc-time-group">
								<div style="float: left">
									<span class="pc-lead"></span>
									<%#Eval("CreateTime", "{0:yyyy-MM-dd HH:mm:ss}")%>
								</div>
							</div>
							<div class="pc-title">
								<span class="pc-label">AD对象</span><span><%# Server.HtmlEncode( Eval("ADObjectName").ToString()) %>(<%# Server.HtmlEncode( Eval("ADObjectID").ToString()) %>)</span>
							</div>
							<div class="pc-title">
								<span class="pc-label">权限中心对象ID</span><span>(<%# Server.HtmlEncode( Eval("SCObjectID").ToString()) %>)</span>
							</div>
							<div class="pc-collapsable" onclick="toggleCollapsable(this);">
								<div style="float: left">
									<span class="pc-collapse"></span>
								</div>
								<div style="margin-left: 20px;">
									<%# Server.HtmlEncode( Eval("Summary").ToString()) %>
								</div>
								<div class="pc-collapsable-content detailPan">
									<pre><%# Server.HtmlEncode( Eval("Detail").ToString()) %></pre>
								</div>
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
		TypeName="MCS.Library.SOA.DataObjects.Security.DataSources.ADReverseSyncLogDetailDataSource"
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

		function toggleCollapsable(elem) {
			$pc.toggleClass(elem, "pc-showall");

		}
	</script>
</body>
</html>
