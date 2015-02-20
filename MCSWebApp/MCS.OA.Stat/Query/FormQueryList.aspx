<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormQueryList.aspx.cs"
	Inherits="MCS.OA.Stat.FormQueryList" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="HB" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="HBEX" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="CCIC" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>表单查询</title>
	<script type="text/javascript" src="../JavaScript/Grid.js"></script>
	<script type="text/javascript" src="../JavaScript/taskQuery.js"></script>
	<link href="../css/portalpage.css" rel="Stylesheet" type="text/css" />
	<link href="http://localhost/MCSWebApp/MCSOAPortal/css.css" rel="stylesheet" type="text/css" />
	<style type="text/css">
		.title1
		{
			float: left;
			border: solid 1px red;
		}
	</style>
	<script type="text/javascript">
		function onBeforeArchiveStart(e) {
			var resourceIDs = $find("GridViewFormQuery").get_clientSelectedKeys();

			if (resourceIDs.length == 0) {
				e.cancel = true;
				return e;
			}

			e.steps = resourceIDs;

			return e;
		}

		function onFinished(e) {
			//检查处理结果
			if (e.value) {
				alert("归档完成");
				$get("btnQueryHistory").click();
			}
			else {
				$showError(e.error.message);
			}

			//处理完成后，不论成功与否，均主动执行一遍查询
			//$get("RefreshButton").click();
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
	</asp:ScriptManager>
	<div>
		<HBEX:MultiProcessControl runat="server" ID="archiveProcesses" DialogTitle="正在归档..."
			ControlIDToShowDialog="archiveBtn" OnClientBeforeStart="onBeforeArchiveStart"
			OnClientFinishedProcess="onFinished" OnExecuteStep="archiveProcess_ExecuteStep" />
		<input type="hidden" runat="server" id="queryHistoryFlag" />
	</div>
	<div>
		<HB:DataBindingControl runat="server" ID="bindingControl">
			<ItemBindings>
				<HB:DataBindingItem ControlID="txtSubject" ControlPropertyName="Text" DataPropertyName="Subject"
					Direction="ControlToData" />
				<HB:DataBindingItem ControlID="CreateTimeBegin" ControlPropertyName="Value" DataPropertyName="CreateTimeBegin"
					Direction="ControlToData" />
				<HB:DataBindingItem ControlID="CreateTimeEnd" ControlPropertyName="Value" DataPropertyName="CreateTimeEnd"
					Direction="ControlToData" />
				<HB:DataBindingItem ControlID="txtDraftDepartmentName" ControlPropertyName="Text"
					DataPropertyName="DraftDepartmentName" Direction="ControlToData" />
			</ItemBindings>
		</HB:DataBindingControl>
	</div>
	<div id="container">
		<div class="topblock">
			<div style="padding-left: 32px; font-size: 11pt; font-weight: 600; background: url(../images/h2_icon.gif) no-repeat 5px 5px;
				line-height: 30px; padding-bottom: 0px">
				表单查询
			</div>
		</div>
		<div class="contentblock">
			<table border="0" cellpadding="0" cellspacing="0" class="texttable">
				<colgroup>
					<col style="width: 9%" />
					<col style="width: 25%" />
					<col style="width: 9%" />
					<col style="width: 18%" />
					<col style="width: 9%" />
					<col style="width: 25%" />
				</colgroup>
				<tr>
					<td class="texttd">
						标&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;题：
					</td>
					<td>
						<asp:TextBox ID="txtSubject" runat="server" MaxLength="100" Width="98%" />
					</td>
					<td class="texttd">
						起草时间：
					</td>
					<td>
						<CCIC:DeluxeCalendar ID="CreateTimeBegin" runat="server" Width="75">
						</CCIC:DeluxeCalendar>
						至<CCIC:DeluxeCalendar ID="CreateTimeEnd" runat="server" Width="75">
						</CCIC:DeluxeCalendar>
					</td>
				</tr>
				<tr>
					<td>
						申请部门：
					</td>
					<td>
						<asp:TextBox ID="txtDraftDepartmentName" runat="server" MaxLength="100" Width="80%" />
					</td>
					<td colspan="2">
						<asp:Button runat="server" ID="btnQuery" OnClick="btnQuery_Click" Text="查询" />&nbsp;&nbsp;&nbsp;&nbsp;
						<asp:Button runat="server" ID="btnQueryHistory" OnClick="btnQueryHistory_Click" Text="查询历史库" />
						<input runat="server" id="archiveBtn" type="button" value="归档" />
						<asp:Label runat="server" ID="showHistoryFlag" />
					</td>
				</tr>
			</table>
		</div>
		<div class="contentblock">
			<CCIC:DeluxeGrid ID="GridViewFormQuery" runat="server" AutoGenerateColumns="False"
				DataSourceID="ObjectDataSourceFormQuery" AllowPaging="True" AllowSorting="True"
				PageSize="20" ShowExportControl="false" GridTitle="搜索结果" DataKeyNames="ResourceID"
				OnRowDataBound="GridViewFormQuery_RowDataBound" TitleFontSize="Small" ShowCheckBoxes="true"
				CssClass="dataList" TitleCssClass="title" Width="99%">
				<HeaderStyle CssClass="headbackground" />
				<RowStyle CssClass="item" />
				<AlternatingRowStyle CssClass="aitem" />
				<SelectedRowStyle CssClass="selecteditem" />
				<PagerStyle CssClass="pager" />
				<EmptyDataTemplate>
					暂时没有您需要的数据
				</EmptyDataTemplate>
				<HeaderStyle CssClass="head" />
				<Columns>
					<asp:TemplateField HeaderText="缓急" SortExpression="EMERGENCY" Visible="false">
						<ItemStyle ForeColor="Red" HorizontalAlign="Center" Width="10%" />
						<ItemTemplate>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="标题" SortExpression="SUBJECT">
						<ItemStyle HorizontalAlign="Center" Width="35%" />
						<ItemTemplate>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField SortExpression="Creator.DisplayName" HeaderText="拟单人" HeaderStyle-Width="80">
						<ItemStyle HorizontalAlign="Left" Width="80" />
						<ItemTemplate>
							<%# Eval("Creator.DisplayName")%>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:BoundField DataField="DraftDepartmentName" SortExpression="DRAFT_DEPARTMENT_NAME"
						HeaderText="单位">
						<ItemStyle HorizontalAlign="Center" Width="10%" />
					</asp:BoundField>
					<asp:BoundField HeaderText="创建时间" SortExpression="CREATE_TIME" DataField="CreateTime"
						HeaderStyle-Font-Bold="true" HtmlEncode="False" DataFormatString="{0:yyyy-MM-dd}">
						<ItemStyle HorizontalAlign="Center" Wrap="false" Width="80" />
					</asp:BoundField>
					<asp:TemplateField HeaderText="状态" SortExpression="PROCESS_STATUS">
						<ItemStyle HorizontalAlign="Center" Wrap="false" Width="100" />
						<ItemTemplate>
							<HBEX:WfStatusControl runat="server" ProcessID='<%# Eval("ProcessID")%>' EnableUserPresence="true" />
						</ItemTemplate>
					</asp:TemplateField>
				</Columns>
				<HeaderStyle CssClass="head" />
				<RowStyle CssClass="item" />
				<AlternatingRowStyle CssClass="aitem" />
				<SelectedRowStyle CssClass="selecteditem" />
				<PagerStyle CssClass="pager" />
				<CheckBoxTemplateHeaderStyle CssClass="checkbox" />
				<PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" Mode="NextPreviousFirstLast"
					NextPageText="下一页" Position="Bottom" PreviousPageText="上一页"></PagerSettings>
			</CCIC:DeluxeGrid>
			<asp:ObjectDataSource ID="ObjectDataSourceFormQuery" runat="server" EnablePaging="True"
				SelectCountMethod="GetQueryCount" SelectMethod="Query" SortParameterName="orderBy"
				TypeName="MCS.OA.Stat.Query.FormQuery" EnableViewState="False" OnSelecting="ObjectDataSourceFormQuery_Selecting"
				OnSelected="ObjectDataSourceFormQuery_Selected">
				<SelectParameters>
					<asp:ControlParameter ControlID="whereCondition" Name="where" PropertyName="Value"
						Type="String" />
					<asp:Parameter Direction="InputOutput" Name="totalCount" Type="Int32" />
				</SelectParameters>
			</asp:ObjectDataSource>
		</div>
	</div>
	<div>
		<input runat="server" type="hidden" id="whereCondition" />
		<HB:CommandInput ID="CommandInputReceiver" runat="server" OnClientCommandInput="onCommandInput">
		</HB:CommandInput>
		<asp:LinkButton ID="RefreshButton" runat="server" OnClick="RefreshButton_Click"></asp:LinkButton>
	</div>
	</form>
</body>
</html>
<script language="javascript" type="text/javascript">
	//按回车时点击查询按钮
	function document.onkeydown() {
		if (event.keyCode == 13) {
			document.getElementById("btnQuery").focus();
			document.getElementById("btnQuery").click();
		}
	}
	function onTaskLinkClick(url, feature) {
		var a = event.srcElement;

		if (!feature) {
			var width = 800;
			var height = 580;

			var left = (window.screen.width - width) / 2;
			var top = (window.screen.height - height) / 2;

			feature = "width=" + width + ",height=" + height + ",left=" + left + ",top=" + top + ",status=no,resizable=yes,toolbar=no,menubar=no,location=no,scrollbars=yes";
		}

		window.open(url, "_blank", feature);

		event.cancelBubble = true;
	}
</script>
