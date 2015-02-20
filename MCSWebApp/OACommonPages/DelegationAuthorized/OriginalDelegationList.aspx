<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OriginalDelegationList.aspx.cs"
    Inherits="MCS.OA.CommonPages.DelegationAuthorized.OriginalDelegationList" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HBEX" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
 <html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>委托列表</title>
 
	<script type="text/javascript">
	    function onUpdateClick(targetUserID) {

	        event.returnValue = false;

	        var feature = "dialogWidth:420px; dialogHeight:320px; center:yes; help:no; resizable:no;status:no;scroll:no";
	        var sPath = "OriginalDelegationEdit.aspx?delegateUserID=" + targetUserID;

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
<body  >
<form id="serverForm" runat="server">
	<div>
		<input runat="server" type="hidden" id="whereCondition" />
	</div>
	<table id="tbClass" style="width:100%; height:100%;" border="0" cellpadding="0" cellspacing="0">
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
					value="新增委托授权" name="btnAdd" />
			</td>
		</tr>
		<tr>
			<td style="vertical-align: top">
				<MCS:DeluxeGrid ID="DeluxeGridDelegation" runat="server" ShowExportControl="true"
					TitleFontSize="Small" Width="100%" DataSourceID="ObjectDataSourceOriginalDelegation"
					AllowPaging="true" PageSize="10" AllowSorting="true" AutoGenerateColumns="false"
					PagerSettings-Mode="NextPreviousFirstLast" PagerSettings-PreviousPageText="上一页"
					PagerSettings-NextPageText="下一页" PagerSettings-Position="TopAndBottom" CaptionAlign="Right"
					CssClass="dataList" TitleCssClass="title" GridTitle="委托授权记录浏览" OnRowDataBound="DeluxeGridDelegationList_RowDataBound"
					OnRowCommand="DeluxeGridDelegationList_RowCommand" OnExportData="DeluxeGridDelegationList_ExportData">
					<HeaderStyle CssClass="head" />
					<RowStyle CssClass="item" />
					<AlternatingRowStyle CssClass="aitem" />
					<SelectedRowStyle CssClass="selecteditem" />
					<PagerStyle CssClass="pager" />
					<EmptyDataTemplate>
						没有查询到您所需要的数据，请重新输入查询条件进行查询
					</EmptyDataTemplate>
				      <Columns>
                        <asp:TemplateField HeaderText="被委托人">
                            <ItemTemplate>
                                <span style="margin-left: 16px">
                                    <HBEX:UserPresence ID="userPresence" runat="server" UserID='<%# Eval("DestinationUserID")%>'
                                        UserDisplayName='<%# Eval("DestinationUserName")%>'>
                                    </HBEX:UserPresence>
                                </span>
                            </ItemTemplate>
                            <HeaderStyle Width="12%" />
                            <ItemStyle HorizontalAlign="Center" Width="12%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="授权开始时间">
                            <ItemTemplate>
                            </ItemTemplate>
                            <HeaderStyle Width="12%" />
                            <ItemStyle HorizontalAlign="Center" Width="12%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="授权结束时间">
                            <ItemTemplate>
                            </ItemTemplate>
                            <HeaderStyle Width="12%" />
                            <ItemStyle HorizontalAlign="Center" Width="12%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="编辑">
                            <ItemStyle ForeColor="blue" HorizontalAlign="Center" Width="15%" />
                            <ItemTemplate>
                                <a id="LinkBtnUpdate" runat="server" style="cursor: hand; text-decoration: underline;
                                    color: Blue;">修改</a> &nbsp;|&nbsp;
                                <asp:LinkButton ID="LinkBtnDel" runat="server" Style="cursor: hand; text-decoration: underline;
                                    color: Blue;" Text="删除" CommandName="DeleteDelegation" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
					<PagerSettings Mode="NextPreviousFirstLast" NextPageText="下一页" Position="TopAndBottom"
						PreviousPageText="上一页"></PagerSettings>
				</MCS:DeluxeGrid>
				<asp:ObjectDataSource ID="ObjectDataSourceOriginalDelegation" runat="server" EnablePaging="True"
					SelectCountMethod="GetQueryCount" SelectMethod="Query"
					SortParameterName="orderBy" TypeName="MCS.OA.CommonPages.DelegationAuthorized.OrigionalDelegationQuery"
					EnableViewState="False" OnSelected="ObjectDataSourceDelegationList_Selected"
					OnSelecting="ObjectDataSourceDelegationList_Selecting">
					<SelectParameters>
						<asp:ControlParameter ControlID="whereCondition" Name="where" PropertyName="Value"
							Type="String" />
						<asp:Parameter Direction="InputOutput" Name="totalCount" Type="Int32" />
					</SelectParameters>
				</asp:ObjectDataSource>
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
