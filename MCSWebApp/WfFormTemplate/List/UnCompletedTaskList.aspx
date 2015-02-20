<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnCompletedTaskList.aspx.cs"
	Inherits="WfFormTemplate.List.UnCompletedTaskList" %>

<%@ Register TagPrefix="CCPC" Namespace="MCS.Library.Web.Controls" Assembly="MCS.Library.Passport" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="hb" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="hbex" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="ccic" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>待办列表</title>
	<link href="../css/css.css" rel="stylesheet" type="text/css" />
	<script type="text/javascript" src="../JavaScript/Grid.js"></script>
	<script type="text/javascript" src="../JavaScript/taskLink.js"></script>
	<script type="text/javascript" src="../JavaScript/replaceAssignee.js"></script>
	<script type="text/javascript">

		//按回车时点击查询按钮
		document.onkeydown = function (event) {
			event = event || window.event;
			if (event.keyCode == 13) {
				document.getElementById("gridViewTask").focus();
				document.getElementById("gridViewTask").click();
			}
		}

		function onLogclick(resourceID, processID) {
			var strLink = "../../OACommonPages/UserOperationLog/UserOperationLogView.aspx?resourceID=" + resourceID + "&processID=" + processID;
			window.showModalDialog(strLink, "", "dialogHeight: 530px; dialogWidth: 800px; resizable:yes; edge: Raised; center: Yes; help: No; status: No;scroll: No;");
		}

		function onConditionClick(sender, e) {
			var content = Sys.Serialization.JavaScriptSerializer.deserialize(e.ConditionContent);
			var bindingControl = $find("searchBinding");
			bindingControl.dataBind(content);
		}
	</script>
</head>
<body class="portal" style="background-color: #f8f8f8;">
	<form id="serverForm" runat="server">
	<asp:ScriptManager runat="server" ID="scriptManager" EnableScriptGlobalization="True">
		<%--<Services>
			<asp:ServiceReference Path="../Services/PortalServices.asmx" />
		</Services>--%>
	</asp:ScriptManager>
	<div>
		<CCPC:SignInLogoControl runat="server" ID="SignInLogo" ReturnUrl="~/List/UnCompletedTaskList.aspx"
			AutoRedirect="True" />
	</div>
	<div>
		<a href="../default.aspx">首页</a>
	</div>
	<div id="container" class="t-container">
		<div>
			<input type="hidden" id="emergencySelector" runat="server" value="-1" />
			<input type="hidden" id="TextBoxPurpose" runat="server" />
			<input type="hidden" id="PersonID" runat="server" />
			<input type="hidden" id="OrgID" runat="server" />
			<input type="hidden" id="FromPerson" runat="server" />
			<input type="hidden" id="DDLFormCategory" runat="server" />
			<!--存储原始待办人的控件-->
			<input type="hidden" id="hiddenOriginalUserID" runat="server" />
		</div>
		<div class="t-dialog-caption">
			<span class="t-dialog-caption">待办列表</span></div>
		<div class="t-search-area">
			<%--<Services>
			<asp:ServiceReference Path="../Services/PortalServices.asmx" />
		</Services>--%>
			<soa:DeluxeSearch runat="server" ID="search1" HasCategory="false" CustomSearchContainerControlID="advSearchPanel"
				OnConditionClick="onConditionClick" SearchFieldTemplate="CONTAINS(${DataField}$, ${Data}$)"
				SearchField="TASK_TITLE" OnSearching="SearchButtonClick" HasAdvanced="True">
			</soa:DeluxeSearch>
			<soa:DataBindingControl runat="server" ID="searchBinding" AllowClientCollectData="True">
				<ItemBindings>
					<soa:DataBindingItem ControlID="sfTaskTitle" DataPropertyName="TaskTitle" />
					<soa:DataBindingItem ControlID="sfTaskStartTime" DataPropertyName="TaskStartTime" />
					<soa:DataBindingItem ControlID="sfApplicationName" DataPropertyName="ApplicationName" />
					<soa:DataBindingItem ControlID="sfPurpose" DataPropertyName="Purpose" />
					<soa:DataBindingItem ControlID="sfDraftUserName" DataPropertyName="DraftUserName" />
					<soa:DataBindingItem ControlID="sfDepartment" DataPropertyName="DraftDepartmentName" />
					<soa:DataBindingItem ControlID="sfDeliverTimeBegin" DataPropertyName="DeliverTimeBegin" />
					<soa:DataBindingItem ControlID="sfDeliverTimeEnd" DataPropertyName="DeliverTimeEnd" />
				</ItemBindings>
			</soa:DataBindingControl>
		</div>
		<div id="advSearchPanel" style="display: none">
			<asp:HiddenField runat="server" ID="sfAdvanced" Value="False" />
			<table>
				<tr>
					<td>
						<label for="sfTaskTitle" class="t-label">
							标题</label>
					</td>
					<td>
						<asp:TextBox runat="server" ID="sfTaskTitle" MaxLength="56" CssClass="pc-textbox" />
					</td>
					<td>
						<label for="sfTaskStartTime" class="t-label">
							开始时间</label>
					</td>
					<td>
						<mcs:DeluxeDateTime ID="sfTaskStartTime" runat="server" />
					</td>
				</tr>
				<tr>
					<td>
						<label for="sfApplicationName" class="t-label">
							应用名</label>
					</td>
					<td>
						<asp:TextBox runat="server" ID="sfApplicationName" MaxLength="56" CssClass="pc-textbox" />
					</td>
					<td>
						<label for="sfPurpose" class="t-label">
							目的</label>
					</td>
					<td>
						<asp:TextBox runat="server" ID="sfPurpose" MaxLength="56" CssClass="pc-textbox" />
					</td>
				</tr>
				<tr>
					<td>
						<label for="sfDraftUserName" class="t-label">
							拟单人名</label>
					</td>
					<td>
						<asp:TextBox runat="server" ID="sfDraftUserName" MaxLength="56" CssClass="pc-textbox" />
					</td>
					<td>
						<label for="sfDepartment" class="t-label">
							单位</label>
					</td>
					<td>
						<asp:TextBox runat="server" ID="sfDepartment" MaxLength="56" CssClass="pc-textbox" />
					</td>
				</tr>
				<tr>
					<td>
						<label for="sfDeliverTimeBegin" class="t-label">
							接收时间范围
						</label>
					</td>
					<td colspan="3">
						<mcs:DeluxeDateTime ID="sfDeliverTimeBegin" runat="server" />
						~
						<mcs:DeluxeDateTime ID="sfDeliverTimeEnd" runat="server" />
					</td>
				</tr>
			</table>
		</div>
		<!--下面是要修改的部分-->
		<div>
			<input type="button" class="portalButton" id="btnModify" runat="server" value="修改待办人" /></div>
		<div class="t-grid-container">
			<ccic:DeluxeGrid ID="gridViewTask" runat="server" AutoGenerateColumns="False" DataSourceID="src1"
				AllowPaging="True" AllowSorting="True" PageSize="20" ShowExportControl="False"
				OnExportData="GridViewTask_ExportData" GridTitle="待办列表" ShowCheckBoxes="true"
				CheckBoxPosition="Left" DataKeyNames="TASK_GUID" OnRowDataBound="GridViewTask_RowDataBound"
				CssClass="dataList gtasks" TitleCssClass="title" Width="100%" DataSourceMaxRow="0"
				ExportingDeluxeGrid="False" TitleColor="141, 143, 149" TitleFontSize="Large">
				<HeaderStyle CssClass="headbackground" />
				<RowStyle CssClass="titem" />
				<CheckBoxTemplateItemStyle Width="1%" />
				<AlternatingRowStyle CssClass="taitem" />
				<SelectedRowStyle CssClass="selecteditem" />
				<PagerStyle CssClass="pager" />
				<EmptyDataTemplate>
					暂时没有您需要的数据
				</EmptyDataTemplate>
				<Columns>
					<asp:BoundField DataField="ProjectName" SortExpression="PROJECT_NAME" HeaderText="服务名称"
						HeaderStyle-ForeColor="Black" Visible="false">
						<HeaderStyle ForeColor="Black"></HeaderStyle>
					</asp:BoundField>
					<asp:TemplateField HeaderText="文件类别" HeaderStyle-ForeColor="Black">
						<HeaderStyle ForeColor="Black"></HeaderStyle>
						<ItemTemplate>
							<%#Server.HtmlEncode((string)Eval("APPLICATION_NAME"))%>/<%#Server.HtmlEncode((string)Eval("PROGRAM_NAME"))%>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="标题" SortExpression="TASK_TITLE">
						<ItemTemplate>
							<asp:HyperLink runat="server" ID="lnkTaskTitle"></asp:HyperLink>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:BoundField DataField="DRAFT_DEPARTMENT_NAME" Visible="true" HeaderText="单位"
						SortExpression="DRAFT_DEPARTMENT_NAME">
						<ItemStyle CssClass="bg_td1" HorizontalAlign="Center" Width="10%" />
					</asp:BoundField>
					<asp:TemplateField SortExpression="DRAFT_USER_NAME" HeaderText="拟单人">
						<ItemTemplate>
							<span style="margin-left: 16px">
								<hbex:UserPresence ID="userPresence" runat="server" UserID='<%# Eval("DRAFT_USER_ID") %>'
									UserDisplayName='<%# Eval("DRAFT_USER_NAME") %>' />
							</span>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:BoundField DataField="DELIVER_TIME" SortExpression="DELIVER_TIME" HeaderText="接收时间"
						DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" HeaderStyle-ForeColor="Black"></asp:BoundField>
					<asp:BoundField DataField="EXPIRE_TIME" SortExpression="EXPIRE_TIME" HeaderText="计划完成时间"
						Visible="false" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" HeaderStyle-ForeColor="Black">
					</asp:BoundField>
					<asp:TemplateField HeaderText="详细信息" HeaderStyle-ForeColor="Black">
						<ItemTemplate>
							<a href="#" onclick="onLogclick('<%# Eval("RESOURCE_ID")%>','<%# Eval("PROCESS_ID")%>')">
								查看</a>
						</ItemTemplate>
					</asp:TemplateField>
				</Columns>
				<PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" Mode="NextPreviousFirstLast"
					NextPageText="下一页" Position="Bottom" PreviousPageText="上一页"></PagerSettings>
				<SelectedRowStyle CssClass="selecteditem" />
			</ccic:DeluxeGrid>
			<soa:DeluxeObjectDataSource runat="server" ID="src1" EnablePaging="true" TypeName="WfFormTemplate.List.UnCompletedTaskListSource"
				OnSelecting="ObjectDataSourceTask_Selecting">
				<SelectParameters>
				</SelectParameters>
			</soa:DeluxeObjectDataSource>
		</div>
		<div>
			<!--where查询条件-->
			<hb:CommandInput ID="CommandInputReceiver" runat="server" OnClientCommandInput="onCommandInput">
			</hb:CommandInput>
			<asp:LinkButton ID="RefreshButton" runat="server" OnClick="RefreshList"></asp:LinkButton>
			<!--目的待办人选择控件-->
			<hb:UserSelector runat="server" ID="targetUserSelector" MultiSelect="true" DialogHeaderText="选择目的待办人"
				DialogTitle="选择目的待办人" ShowingMode="Normal" ListMask="All" />
			<!--进度条控件-->
			<hb:MultiProcessControl runat="server" ID="multiProcess" DialogTitle="正在提交修改" ControlIDToShowDialog="btnModify"
				OnClientBeforeStart="onBeforeStart" OnClientFinishedProcess="onFinished" OnExecuteStep="multiProcess_ExecuteStep" />
		</div>
		<div style="display: none">
			<!--查询按钮-->
			<asp:Button ID="btnQuery" OnClick="RefreshList" runat="server" />
		</div>
	</div>
	</form>
</body>
</html>
