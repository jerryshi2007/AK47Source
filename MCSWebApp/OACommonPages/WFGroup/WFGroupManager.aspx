<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WFGroupManager.aspx.cs" Inherits="MCS.OA.CommonPages.WFGroup.WFGroupManager" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HBEX" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<base target="_self">
	<title>群组管理</title>
	<script type="text/javascript">
	    function onCreateOrUpdateClick(groupId) {

	        event.returnValue = false;

	        var feature = "dialogWidth:570px; dialogHeight:600px; center:yes; help:no; resizable:no;status:no;scroll:no";
	        var paramStr = '';
	        if (groupId != '') {
	            paramStr = '?groupid=' + groupId;
	        }
	        var sPath = "WFGroupEditor.aspx" + paramStr;

	        var result = window.showModalDialog(sPath, null, feature);
	        refreshPage();
	    }
	    function refreshPage() {
	        document.getElementById("RefreshButton").click();
	    }
	</script>
</head>
<body>
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
						群组管理
					</div>
				</div>
			</td>
		</tr>
		<tr>
			<td style="height: 24px">
				<input id="btnAdd" class="formButton" onclick="onCreateOrUpdateClick('');" type="button"
					value="新增群组" name="btnAdd" />
			</td>
		</tr>
		<tr>
			<td style="vertical-align: top">
				<MCS:DeluxeGrid ID="DeluxeGridGroup" runat="server" ShowExportControl="true"
					TitleFontSize="Small" Width="100%" DataSourceID="ObjectDataSourceGroup"
					AllowPaging="true" PageSize="10" AllowSorting="true" AutoGenerateColumns="false"
					CaptionAlign="Right" CssClass="dataList" TitleCssClass="title" GridTitle="群组管理" OnRowDataBound="DeluxeGridGroup_RowDataBound"
					OnRowCommand="DeluxeGridGroup_RowCommand" OnExportData="DeluxeGridGroup_ExportData">
					<HeaderStyle CssClass="head" />
					<RowStyle CssClass="item" />
					<AlternatingRowStyle CssClass="aitem" />
					<SelectedRowStyle CssClass="selecteditem" />
					<PagerStyle CssClass="pager" />
					<EmptyDataTemplate>
						没有查询到您所需要的数据，请重新输入查询条件进行查询
					</EmptyDataTemplate>
				      <Columns>
                     	<asp:BoundField DataField="GroupName" HeaderText="群组" SortExpression="Group_Name" />
						<asp:BoundField DataField="Category" HeaderText="类别" SortExpression="Category" />
                         <asp:TemplateField HeaderText="负责人">
                            <ItemTemplate>
                                <%#Server.HtmlEncode((string)Eval("Manager.DisplayName"))%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="创建人">
                            <ItemTemplate>
                                <%#Server.HtmlEncode((string)Eval("Creator.DisplayName"))%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CreateTime" HeaderText="创建时间" SortExpression="CREATE_TIME" />
                        <asp:TemplateField HeaderText="编辑">
                            <ItemStyle ForeColor="blue" HorizontalAlign="Center" Width="15%" />
                            <ItemTemplate>
                                <a id="LinkBtnEdit" runat="server" style="cursor: hand; text-decoration: underline;
                                    color: Blue;">修改</a> &nbsp;|&nbsp;
                                <asp:LinkButton ID="LinkBtnDel" runat="server" Style="cursor: hand; text-decoration: underline;
                                    color: Blue;" Text="删除" CommandName="DeleteGroup" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
					<PagerSettings Mode="NextPreviousFirstLast" NextPageText="下一页" Position="Bottom"
						PreviousPageText="上一页"></PagerSettings>
				</MCS:DeluxeGrid>
				<asp:ObjectDataSource ID="ObjectDataSourceGroup" runat="server" EnablePaging="True"
					SelectCountMethod="GetQueryCount" SelectMethod="Query" 
					TypeName="MCS.Library.SOA.DataObjects.WfGroupQuery"
					EnableViewState="False" OnSelected="ObjectDataSourceGroup_Selected"
					OnSelecting="ObjectDataSourceGroup_Selecting">
				</asp:ObjectDataSource>
			</td>
		</tr>
	</table>
    <asp:LinkButton ID="RefreshButton" runat="server" OnClick="RefreshButton_Click"></asp:LinkButton>
	</form>
</body>
</html>
