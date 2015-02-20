<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AllOriginalDelegationList.aspx.cs"
	Inherits="MCS.OA.CommonPages.DelegationAuthorized.AllOriginalDelegationList" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="HBEX" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>委托列表</title>
	<style type="text/css">
		table.search-table
		{
			border: solid 1px gray;
			margin: 20px;
			width: auto;
			table-layout: fixed;
		}
		
		
		table.search-table td
		{
			text-align: left;
			border-bottom:dotted 1px;
			border-collapse: collapse;
		}
		
		table.search-table td.caption
		{
			text-align: center;
			background: #F2F8F8;
		}
		
		.search-button
		{
			display: inline-block;
			padding-right: 22px;
			line-height: 30x;
			background: transparent url('../Images/search.gif') no-repeat scroll top right;
		}
		
		.fullSize
		{
			width: 100%;
		}
		
		.normalLink
		{
		}
	</style>
	<script type="text/javascript">

		function edit(elem) {
			var srcId = null;
			var destId = null;

			if (elem.nodeType === 1) {
				if (elem.getAttribute) {
					srcId = elem.getAttribute("data-srcid");
					destId = elem.getAttribute("data-destid");
				} else {
					srcId = elem['data-srcid'];
					destId = elem['data-destid'];
				}

				if (typeof srcId === 'string' && typeof destId === 'string') {
					onUpdateClick(srcId, destId);
				}
			}
		}

		function onUpdateClick(srcId, destId) {

			event.returnValue = false;

			var feature = "dialogWidth:420px; dialogHeight:320px; center:yes; help:no; resizable:no;status:no;scroll:no";
			var sPath = "AnyOriginalDelegationEdit.aspx";

			if (srcId) {
				if (destId) {
					sPath += "?sourceID=" + encodeURIComponent(srcId) + '&destinationID=' + encodeURIComponent(destId);
				}
			}

			var result = window.showModalDialog(sPath, null, feature);

			if (result) {
				refreshPage();
			}
		}

		function onDeleteClick(targetUserID) {
			if (window.confirm("确认要删除吗？"))
				document.getElementById("DeleteButton").click();
		}

		function refreshPage() {
			document.getElementById("RefreshButton").click();
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<HBEX:DataBindingControl runat="server" ID="searchBinding" AllowClientCollectData="True">
			<ItemBindings>
				<HBEX:DataBindingItem ControlID="sfActiveUser" DataPropertyName="ActiveUser" ControlPropertyName="SelectedSingleData"
					ClientPropName="get_selectedSingleData" ClientSetPropName="set_selectedSingleData"
					ClientIsHtmlElement="false" />
				<HBEX:DataBindingItem ControlID="sfActiveUserName" DataPropertyName="ActiveUserName" />
				<HBEX:DataBindingItem ControlID="sfPassiveUser" DataPropertyName="PassiveUser" ControlPropertyName="SelectedSingleData"
					ClientPropName="get_selectedSingleData" ClientSetPropName="set_selectedSingleData"
					ClientIsHtmlElement="false" />
				<HBEX:DataBindingItem ControlID="sfPassiveUserName" DataPropertyName="PassiveUserName" />
				<HBEX:DataBindingItem ControlID="sfEnabled" DataPropertyName="Enabled" ControlPropertyName="Checked"
					ClientIsHtmlElement="true" ClientPropName="checked" />
			</ItemBindings>
		</HBEX:DataBindingControl>
	</div>
	<table id="tbClass" style="width: 100%; height: 100%;" border="0" cellpadding="0"
		cellspacing="0">
		<tr>
			<td style="height: 32px">
				<div style="border-bottom: groove 1px Silver; overflow: hidden; text-align: left;">
					<div style="padding-left: 32px; font-size: 11pt; font-weight: 600; background: url(../images/h2_icon.gif) no-repeat 5px 5px;
						line-height: 30px; padding-bottom: 0px">
						委托授权
					</div>
				</div>
			</td>
		</tr>
		<tr>
			<td style="height: 24px">
				<input id="btnAdd" class="formButton" onclick="onUpdateClick('');" type="button"
					value="新增委托授权" name="btnAdd" /><asp:HyperLink runat="server" ID="lnkToLog" NavigateUrl=""
						Target="_blank" Text="查看日志" />
			</td>
		</tr>
		<tr>
			<td style="height: 100px;">
				<table border="0" cellpadding="0" cellspacing="0" class="search-table" style="width: 500px">
					<tr>
						<td class="caption">
							委托人
						</td>
						<td>
							<HBEX:OuUserInputControl runat="server" ID="sfActiveUser" CanSelectRoot="False" ListMask="User,Organization"
								SelectMask="User" MultiSelect="False" />
						</td>
						<td class="caption">
							委托人名称
						</td>
						<td>
							<asp:TextBox runat="server" ID="sfActiveUserName" CssClass="fullSize" MaxLength="30" />
						</td>
					</tr>
					<tr>
						<td class="caption">
							被委托人
						</td>
						<td>
							<HBEX:OuUserInputControl ID="sfPassiveUser" runat="server" CanSelectRoot="False"
								SelectMask="User" ListMask="User,Organization" />
						</td>
						<td class="caption">
							被委托人名称
						</td>
						<td>
							<asp:TextBox ID="sfPassiveUserName" runat="server" CssClass="fullSize" MaxLength="30" />
						</td>
					</tr>
					<tr>
						<td class="caption">
							在有效期内
						</td>
						<td>
							<asp:CheckBox Text="仅在有效期内" ID="sfEnabled" runat="server" Checked="true" />
						</td>
						<td class="caption">
							搜索
						</td>
						<td>
							<button runat="server" id="btnSearchAgent" class="search-button" onclick="document.getElementById('btnSearch').click();">
								搜索</button>
							<div style="display: none">
								<asp:Button Text="搜索" ID="btnSearch" runat="server" OnClick="ButtonSearchClick" /></div>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td style="vertical-align: top">
				<MCS:DeluxeGrid ID="DeluxeGridDelegation" runat="server" ShowExportControl="true"
					TitleFontSize="Small" Width="100%" DataSourceID="dataSourceMain" AllowPaging="true"
					PageSize="10" AllowSorting="true" AutoGenerateColumns="false" PagerSettings-Mode="NextPreviousFirstLast"
					PagerSettings-PreviousPageText="上一页" PagerSettings-NextPageText="下一页" PagerSettings-Position="TopAndBottom"
					CaptionAlign="Right" CssClass="dataList" TitleCssClass="title" GridTitle="委托授权记录浏览"
					OnRowDataBound="DeluxeGridDelegationList_RowDataBound" OnRowCommand="DeluxeGridDelegationList_RowCommand"
					OnExportData="DeluxeGridDelegationList_ExportData">
					<HeaderStyle CssClass="head" />
					<RowStyle CssClass="item" />
					<AlternatingRowStyle CssClass="aitem" />
					<SelectedRowStyle CssClass="selecteditem" />
					<PagerStyle CssClass="pager" />
					<EmptyDataTemplate>
						没有查询到您所需要的数据，请重新输入查询条件进行查询
					</EmptyDataTemplate>
					<Columns>
						<asp:TemplateField HeaderText="委托人">
							<ItemTemplate>
								<span style="margin-left: 16px">
									<HBEX:UserPresence ID="up0" runat="server" UserID='<%# Eval("SourceUserID")%>' UserDisplayName='<%# Eval("SourceUserName")%>'>
									</HBEX:UserPresence>
								</span>
							</ItemTemplate>
							<HeaderStyle Width="12%" />
							<ItemStyle HorizontalAlign="Center" Width="12%" />
						</asp:TemplateField>
						<asp:TemplateField HeaderText="被委托人">
							<ItemTemplate>
								<span style="margin-left: 16px">
									<HBEX:UserPresence ID="up1" runat="server" UserID='<%# Eval("DestinationUserID")%>'
										UserDisplayName='<%# Eval("DestinationUserName")%>'>
									</HBEX:UserPresence>
								</span>
							</ItemTemplate>
							<HeaderStyle Width="12%" />
							<ItemStyle HorizontalAlign="Center" Width="12%" />
						</asp:TemplateField>
						<asp:TemplateField HeaderText="授权开始时间">
							<ItemTemplate>
								<%#Eval("StartTime","{0:yyyy-MM-dd HH:mm:ss}") %>
							</ItemTemplate>
							<HeaderStyle Width="12%" />
							<ItemStyle HorizontalAlign="Center" Width="12%" />
						</asp:TemplateField>
						<asp:TemplateField HeaderText="授权结束时间">
							<ItemTemplate>
								<%#Eval("EndTime", "{0:yyyy-MM-dd HH:mm:ss}")%>
							</ItemTemplate>
							<HeaderStyle Width="12%" />
							<ItemStyle HorizontalAlign="Center" Width="12%" />
						</asp:TemplateField>
						<asp:TemplateField HeaderText="编辑">
							<ItemStyle ForeColor="blue" HorizontalAlign="Center" Width="15%" />
							<ItemTemplate>
								<a id="LinkBtnUpdate" runat="server" style="cursor: hand; text-decoration: underline;
									color: Blue;" data-destid='<%# HttpUtility.HtmlAttributeEncode((String)Eval("DestinationUserID")) %>'
									data-srcid='<%# HttpUtility.HtmlAttributeEncode((String)Eval("SourceUserID")) %>'
									onclick='edit(this)'>修改</a> &nbsp;|&nbsp;
								<asp:LinkButton ID="lkd1" runat="server" Style="cursor: hand; text-decoration: underline;
									color: Blue;" Text="删除" CommandName="DeleteDelegation" data-sid='<%#Eval("SourceUserID") %>'
									data-tid='<%#Eval("DestinationUserID") %>' CommandArgument='<%# Eval("DestinationUserID")+"," + Eval("SourceUserID") %>'
									OnClientClick="return confirm('确认要删除吗？');" />
							</ItemTemplate>
						</asp:TemplateField>
					</Columns>
					<PagerSettings Mode="NextPreviousFirstLast" NextPageText="下一页" Position="TopAndBottom"
						PreviousPageText="上一页"></PagerSettings>
				</MCS:DeluxeGrid>
				<HBEX:DeluxeObjectDataSource ID="dataSourceMain" runat="server" EnablePaging="True"
					TypeName="MCS.OA.CommonPages.DelegationAuthorized.OrigionalDelegationQuery" OnSelecting="ObjectDataSourceDelegationList_Selecting">
				</HBEX:DeluxeObjectDataSource>
			</td>
		</tr>
		<tr>
			<td style="height: 42px; text-align: center">
				<input id="closeButton" accesskey="C" class="formButton" onclick="top.close()" type="button"
					value="关闭(C)" />
			</td>
		</tr>
	</table>
	<asp:LinkButton ID="RefreshButton" runat="server" OnClick="RefreshButton_Click"></asp:LinkButton>
	</form>
</body>
</html>
