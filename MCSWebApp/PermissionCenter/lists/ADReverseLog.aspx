<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ADReverseLog.aspx.cs" Inherits="PermissionCenter.lists.ADReverseLog" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>AD反向同步日志</title>
	<base target="_self" />
	<pc:HeaderControl ID="HeaderControl1" runat="server" />
	<style type="text/css">
		.col-chekbox input, .checkbox input
		{
			visibility: hidden;
		}
	</style>
</head>
<body>
	<form id="form1" runat="server">
	<pc:BannerNotice ID="notice" runat="server"></pc:BannerNotice>
	<div class="pc-frame-container">
		<div class="pc-container5">
			<div class="pc-listmenu-container">
			</div>
			<div class="pc-grid-container">
				<mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
					AllowPaging="True" AllowSorting="True" Category="PermissionCenter" GridTitle="活动目录同步日志"
					DataKeyNames="LogID" CssClass="dataList pc-datagrid" TitleCssClass="title" PagerSettings-Position="Bottom"
					DataSourceMaxRow="0" TitleColor="141, 143, 149" TitleFontSize="Large" ShowExportControl="true"
					OnRowCommand="HandleRowCommand" Width="100%" GridLines="None">
					<EmptyDataTemplate>
						暂时没有您需要的数据
					</EmptyDataTemplate>
					<HeaderStyle CssClass="head" />
					<Columns>
						<pc:CollapseHandleField CollapseCssClass="pc-collapse" OpenCssClass="pc-collapse-open">
						</pc:CollapseHandleField>
						<asp:TemplateField HeaderText="发起时间" SortExpression="CreateTime" ItemStyle-Width="200px">
							<ItemTemplate>
								<div style="width: 100%">
									<asp:Label ID="tt" runat="server" Text='<%# Bind("CreateTime", "{0:yyyy-MM-dd HH:mm:ss}") %>'></asp:Label>
								</div>
								<div>
								</div>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="执行时间">
							<HeaderTemplate>
								<mcs:GridColumnSorter runat="server" ID="colTimeSorter" DefaultOrderName="执行时间" DockPosition="Left"
									PreventRenderChildren='<%# this.gridMain.ExportingDeluxeGrid %>'>
							<SortItems>
							<mcs:SortItem SortExpression="StartTime" Text="开始时间" />
							<mcs:SortItem SortExpression="EndTime" Text="结束时间" />
							</SortItems>
								</mcs:GridColumnSorter>
							</HeaderTemplate>
							<ItemTemplate>
								<div>
									<span>开始</span>：<asp:Label ID="lblTime" runat="server" Text='<%# Bind("StartTime", "{0:yyyy-MM-dd HH:mm:ss}") %>'></asp:Label>
								</div>
								<div>
									<span>结束：</span><asp:Label ID="Label1" runat="server" Text='<%# Bind("EndTime", "{0:yyyy-MM-dd HH:mm:ss}") %>'></asp:Label>
								</div>
								<div>
									<div id="d" class="pc-action-tray" runat="server" visible='<%# this.gridMain.ExportingDeluxeGrid == false %>'>
										<asp:LinkButton ID="lnkTimeTrip" runat="server" Text="时间穿梭" CssClass="pc-item-cmd"
											CommandName="Shuttle" CommandArgument='<%#Eval("StartTime") %>' data-time='<%# ((DateTime)Eval("StartTime")).ToBinary() %>'
											OnClientClick="return shuttle(this);"></asp:LinkButton><asp:HyperLink ID="HyperLink1"
												runat="server" CssClass="pc-item-cmd" NavigateUrl='<%#Eval("LogID","~/lists/ADReverseLogDetail.aspx?syncID={0}") %>'
												onclick="return $pc.modalPopup(this);" Text="详细信息" ToolTip="详细信息" />
									</div>
								</div>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="已修改" SortExpression="NumberOfModifiedItems">
							<ItemTemplate>
								<div class="pc-rightalign">
									<span>
										<%#Eval("NumberOfModifiedItems", "{0:#,##0}")%></span>
								</div>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="错误" SortExpression="NumberOfExceptions">
							<ItemTemplate>
								<div class="pc-rightalign">
									<span>
										<%#Eval("NumberOfExceptions", "{0:#,##0}")%></span>
								</div>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="结果" SortExpression="Status">
							<ItemTemplate>
								<div class="pc-center">
									<pc:ADResultIcon runat="server" ID="ad1" Status='<%# Eval("Status") %>' TextOnly='<%# this.gridMain.ExportingDeluxeGrid == true %>' />
								</div>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="操作人" SortExpression="OperatorName">
							<ItemTemplate>
								<div>
									<span>
										<soa:UserPresence ID="UserPresence1" runat="server" UserID='<%#Eval("OperatorID") %>' />
										<%#Server.HtmlEncode(Eval("OperatorName").ToString()) %>
									</span>
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
	</div>
	<soa:DeluxeObjectDataSource ID="dataSourceMain" runat="server" EnablePaging="True"
		TypeName="MCS.Library.SOA.DataObjects.Security.DataSources.ADReverseSyncLogDataSource"
		EnableViewState="false" OnSelecting="dataSourceMain_Selecting">
		<SelectParameters>
		</SelectParameters>
	</soa:DeluxeObjectDataSource>
	</form>
	<script type="text/javascript">
		if (window.parent.showLoader)
			window.parent.showLoader(false);
		$pc.ui.gridBehavior("gridMain", "hover");
		$pc.ui.listMenuBehavior("listMenu");

		function shuttle(elem) {
			var time = $pc.getAttr(elem, "data-time");
			if (time) {
				window.top.shuttle.apply(window.top, [time]);
			}

			return false;
		}
	</script>
</body>
</html>
