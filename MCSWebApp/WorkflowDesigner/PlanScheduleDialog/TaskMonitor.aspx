<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TaskMonitor.aspx.cs" Inherits="WorkflowDesigner.PlanScheduleDialog.TaskMonitor" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="mcs" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="soa" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" style="overflow: auto">
<head runat="server">
	<title>任务监控</title>
	<style type="text/css">
		div.search-panel, div.grid-panel
		{
			margin: 5px;
			padding: 6px;
		}
		
		ul.tabs-header
		{
			font-size: 12px;
			line-height: 24px; /*border-bottom: solid 1px #ef8038;*/
			margin: 5px 5px 0;
		}
		
		ul.tabs-header li
		{
			display: inline;
			width: 120px;
			height: 18px;
			margin-left: 12px;
			margin-right: 10px;
			padding: 5px;
		}
		
		ul.tabs-header li.tabs-active
		{
			background: #ef8038;
			color: #ffffff;
		}
		
		ul.tabs-header li.tabs-active *
		{
			color: #ffffff;
		}
		
		div.tabs-body
		{
			margin: 0 5px 5px;
			border: solid 1px #ef8038;
		}
		
		ul.hidden
		{
			display: none;
		}
		
		.nohead
		{
			display: none;
		}
	</style>
</head>
<body>
	<form id="serverForm" runat="server" target="_self">
	<asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true" />
	<div style="display: none">
		<soa:SubmitButton Text="终止选定All" ID="btnRemoveAllSelected" runat="server" OnClick="MoveAllSelectedClick"
			PopupCaption="正在终止..." ProgressMode="BySteps" />
		<soa:SubmitButton Text="执行选定All" ID="btnExecuteAllSelected" runat="server" OnClick="ExecuteAllSelectedClick"
			PopupCaption="正在执行..." ProgressMode="BySteps" />
		<soa:SubmitButton Text="执行All" ID="btnExecuteAll" runat="server" OnClick="ExecuteAllClick"
			PopupCaption="正在执行..." ProgressMode="BySteps" />
	</div>
	<soa:DataBindingControl runat="server" ID="searchBinding">
		<ItemBindings>
			<soa:DataBindingItem DataPropertyName="StartTime" ControlID="searchStartTime" />
			<soa:DataBindingItem DataPropertyName="EndTime" ControlID="searchEndTime" />
			<soa:DataBindingItem DataPropertyName="Title" ControlID="searchTitle" />
		</ItemBindings>
	</soa:DataBindingControl>
	<div class='<%= this.Request["nohead"]=="yes"?"nohead":"gridHead" %>'>
		<div class="dialogTitle" style="padding: 5px">
			<span class="dialogLogo">任务监控</span>
		</div>
	</div>
	<ul class="tabs-header">
		<li class="tabs-active">
			<asp:HyperLink NavigateUrl="TaskMonitor.aspx" ID="lnk1" runat="server" Text="未完成任务" />
		</li>
		<li>
			<asp:HyperLink NavigateUrl="TaskMonitor.aspx?mode=accomplished" ID="lnk2" runat="server"
				Text="已完成任务" />
		</li>
	</ul>
	<div class="tabs-body">
		<div class='<%=Request.QueryString["cansearch"] == "no"?"search-panel hidden":"search-panel" %>'>
			<table border="0" cellpadding="0" cellspacing="0">
				<tr>
					<td>
						<label>
							开始执行时间</label>
					</td>
					<td>
						<mcs:DeluxeDateTime runat="server" ID="searchStartTime" />
					</td>
					<td>
						<label>
							结束执行时间</label>
					</td>
					<td>
						<mcs:DeluxeDateTime runat="server" ID="searchEndTime" />
					</td>
					<td>
						<label>
							标题</label>
					</td>
					<td>
						<asp:TextBox runat="server" ID="searchTitle" />
					</td>
					<td>
						<asp:Button Text="搜索" runat="server" OnClick="SearchClick" />
					</td>
				</tr>
			</table>
		</div>
		<div class="grid-panel">
			<mcs:DeluxeGrid ID="DeluxeGrid1" runat="server" AllowPaging="True" AllowSorting="True"
				CascadeControlID="" DataSourceID="dataSourceMain" DataSourceMaxRow="0" GridTitle="<button type='button' onclick='return moveSelected() && false;'>终止选定项</button>&nbsp;<button type='button' onclick='return executeSelected() && false;'>立即执行选定项</button>&nbsp;<button type='button' onclick='return executeAll() && false;'>执行全部</button>"
				CssClass="dataList" ShowExportControl="True" Width="100%" TitleColor="141, 143, 149"
				TitleFontSize="Large" AutoGenerateColumns="False" OnPreRender="DeluxeGrid1_PreRender"
				OnRowCommand="DeluxeGrid1_RowCommand" DataKeyNames="TASK_GUID" ShowCheckBoxes="True">
				<PagerStyle CssClass="pager" />
				<RowStyle CssClass="item" />
				<HeaderStyle CssClass="head" />
				<AlternatingRowStyle CssClass="aitem" />
				<EmptyDataTemplate>
					暂时没有您需要的数据
				</EmptyDataTemplate>
				<PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" Mode="NextPreviousFirstLast"
					NextPageText="下一页" Position="Bottom" PreviousPageText="上一页"></PagerSettings>
				<Columns>
					<asp:BoundField ReadOnly="true" HeaderText="类别" DataField="CATEGORY" HtmlEncode="true"
						SortExpression="CATEGORY" />
					<asp:TemplateField HeaderText="名称" SortExpression="TASK_TITLE">
						<ItemTemplate>
							<asp:HyperLink runat="server" NavigateUrl='<%#Eval("TASK_GUID","TaskPendingDetail.aspx?id={0}") %>'
								Target="_blank">
							<%# Server.HtmlEncode(Eval("TASK_TITLE").ToString())%>
							</asp:HyperLink>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:BoundField ReadOnly="true" HeaderText="任务类型" DataField="TASK_TYPE" HtmlEncode="true"
						SortExpression="TASK_TYPE" />
					<mcs:EnumDropDownField ReadOnly="true" HeaderText="状态" DataField="STATUS" EnumTypeName="MCS.Library.SOA.DataObjects.SysTaskStatus, MCS.Library.SOA.DataObjects"
						UseNameAsValue="true" SortExpression="STATUS" />
					<asp:BoundField ReadOnly="true" HeaderText="创建时间" DataField="CREATE_TIME" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}"
						SortExpression="CREATE_TIME" />
					<asp:BoundField ReadOnly="true" HeaderText="开始执行" DataField="START_TIME" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}"
						SortExpression="START_TIME" />
					<asp:BoundField ReadOnly="true" HeaderText="结束执行" DataField="END_TIME" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}"
						SortExpression="END_TIME" NullDisplayText="-" />
					<asp:BoundField ReadOnly="true" HeaderText="发起人" DataField="SOURCE_NAME" HtmlEncode="true"
						SortExpression="SOURCE_NAME" />
				</Columns>
			</mcs:DeluxeGrid>
			<soa:DeluxeObjectDataSource ID="dataSourceMain" runat="server" EnablePaging="True"
				SelectCountMethod="GetQueryCount" SelectMethod="Query" SortParameterName="orderBy"
				OnSelecting="dataSourceMain_Selecting" TypeName="WorkflowDesigner.PlanScheduleDialog.TaskQuerySource"
				EnableViewState="False">
			</soa:DeluxeObjectDataSource>
			<script type="text/javascript">
				function moveSelected() {
					var rst = false;
					var grid = $find("DeluxeGrid1");
					if (grid) {
						var keys = grid.get_clientSelectedKeys();
						if (keys.length) {
							if (confirm('确实要终止选定的任务吗？\r\n如果任务已经在执行，这可能会导致无法预期的结果。')) {
								executeInFrame('btnRemoveAllSelected');
								rst = true;
							}
						}
					}

					return rst;
				}

				function executeSelected() {
					var rst = false;
					var grid = $find("DeluxeGrid1");
					if (grid) {
						var keys = grid.get_clientSelectedKeys();
						if (keys.length) {
							if (confirm('确实要立即执行选定的任务吗？\r\n如果任务已经在执行，这可能会导致无法预期的结果。')) {
								executeInFrame('btnExecuteAllSelected');
								rst = true;
							}
						}
					}

					return rst;
				}

				function executeAll() {
					var rst = false;

					if (confirm('确实要立即所有的任务吗？\r\n如果任务已经在执行，这可能会导致无法预期的结果。')) {
						executeInFrame('btnExecuteAll');
						rst = true;
					}

					return rst;
				}

				function executeInFrame(buttonID) {
					$get("serverForm").target = "innerFrame";

					try {
						$get(buttonID).click();
					}
					finally {
						$get("serverForm").target = "_self";
					}
				}
			</script>
		</div>
	</div>
	<div style="display: none">
		<iframe name="innerFrame"></iframe>
		<asp:LinkButton ID="RefreshButton" runat="server" OnClick="RefreshList"></asp:LinkButton>
	</div>
	</form>
</body>
</html>
