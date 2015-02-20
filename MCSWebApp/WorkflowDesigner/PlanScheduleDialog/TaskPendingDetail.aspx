<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TaskPendingDetail.aspx.cs"
	Inherits="WorkflowDesigner.PlanScheduleDialog.TaskPendingDetail" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="mcs" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="soa" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" scroll="auto" style="overflow: auto">
<head runat="server">
	<title>未终止的任务详情</title>
	<style type="text/css">
		table th
		{
			text-align: right;
		}
	</style>
</head>
<body>
	<form id="form1" runat="server">
	<div class="gridHead">
		<div class="dialogTitle" style="padding: 5px">
			<span class="dialogLogo">未终止的任务详情</span>
		</div>
	</div>
	<div>
		<asp:FormView ID="FormView1" runat="server" DataSourceID="dataSourceMain" DataKeyNames="TASK_GUID"
			Width="100%" PageIndex="0">
			<ItemTemplate>
				<table border="0" cellpadding="0" cellspacing="0" style="table-layout: fixed; width: 100%">
					<tr>
						<th style="width: 100px">
							ID
						</th>
						<td>
							<%#Eval("TASK_GUID") %>
						</td>
					</tr>
					<tr>
						<th>
							任务名
						</th>
						<td>
							<%# Server.HtmlEncode((string)Eval("TASK_TITLE"))%>
						</td>
					</tr>
					<tr>
						<th>
							任务类别
						</th>
						<td>
							<%# Server.HtmlEncode((string)Eval("CATEGORY"))%>
						</td>
					</tr>
					<tr>
						<th>
							RESOURCE_ID
						</th>
						<td>
							<%# Server.HtmlEncode((string)Eval("RESOURCE_ID"))%><a href='<%#Eval("RESOURCE_ID","JobEditor.aspx?jobId={0}") %>'
								target="_blank" style="padding-left: 20px" onclick="modalPopup(this); return false;">&gt;&gt;检查作业</a>
						</td>
					</tr>
					<tr>
						<th>
							任务类型
						</th>
						<td>
							<%# Server.HtmlEncode((string)Eval("TASK_TYPE"))%>
						</td>
					</tr>
					<tr>
						<th>
							任务状态
						</th>
						<td>
							<%# Server.HtmlEncode((string)Eval("STATUS"))%>
						</td>
					</tr>
					<tr>
						<th>
							创建时间
						</th>
						<td>
							<%# (string)Eval("CREATE_TIME","{0:yyyy-MM-dd HH:mm:ss}")%>
						</td>
					</tr>
					<tr>
						<th>
							开始时间
						</th>
						<td>
							<%# (string)Eval("START_TIME","{0:yyyy-MM-dd HH:mm:ss}")%>
						</td>
					</tr>
					<tr>
						<th>
							结束时间
						</th>
						<td>
							<%# (string)Eval("END_TIME","{0:yyyy-MM-dd HH:mm:ss}")%>
						</td>
					</tr>
					<tr>
						<th>
							创建人
						</th>
						<td>
							<%# Server.HtmlEncode(Eval("SOURCE_ID","{0}"))%>
						</td>
					</tr>
					<tr>
						<th>
							创建人名
						</th>
						<td>
							<%# Server.HtmlEncode(Eval("SOURCE_NAME","{0}"))%>
						</td>
					</tr>
					<tr>
						<th>
							任务数据
						</th>
						<td>
							<pre><%# Server.HtmlEncode(Eval("DATA","{0}"))%></pre>
						</td>
					</tr>
					<tr>
						<th>
							任务执行消息
						</th>
						<td>
							<pre><%# Server.HtmlEncode(Eval("STATUS_TEXT").ToString())%></pre>
						</td>
					</tr>
					<tr>
						<th>
							关联页面
						</th>
						<td>
							<a href='<%#Eval("URL") %>'>
								<%# Server.HtmlEncode(Eval("URL").ToString())%></a>
						</td>
					</tr>
				</table>
			</ItemTemplate>
		</asp:FormView>
		<script type="text/javascript">
			function modalPopup(lnk) {
				showModalDialog(lnk.href, "", "resizable=1; dialogWidth=800px");
			}
		</script>
	</div>
	<div class="gridfileBottom">
	</div>
	<soa:DeluxeObjectDataSource ID="dataSourceMain" runat="server" EnablePaging="True"
		SelectCountMethod="GetQueryCount" SelectMethod="Query" SortParameterName="orderBy"
		OnSelecting="dataSourceMain_Selecting" TypeName="WorkflowDesigner.PlanScheduleDialog.TaskQuerySource"
		EnableViewState="False">
	</soa:DeluxeObjectDataSource>
	</form>
</body>
</html>
