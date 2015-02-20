<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WFPostManager.aspx.cs" Inherits="MCS.OA.CommonPages.WFPost.WFPostManager" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HBEX" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<base target="_self">
	<title>岗位管理</title>
	<script type="text/javascript">
	    function onCreateOrUpdateClick(postId) {

	        event.returnValue = false;

	        var feature = "dialogWidth:570px; dialogHeight:600px; center:yes; help:no; resizable:no;status:no;scroll:no";
	        var paramStr = '';
	        if (postId != '') {
	            paramStr = '?postid=' + postId;
	        }
	        var sPath = "WFPostEditor.aspx" + paramStr;

	        var result = window.showModalDialog(sPath, null, feature);
	        refreshPage();
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
		<input runat="server" type="hidden" id="whereCondition" />
	</div>
	<table id="tbClass" style="width:100%; height:100%;" border="0" cellpadding="0" cellspacing="0">
		<tr>
			<td style="height: 32px">
				<div style="border-bottom: groove 1px Silver; overflow: hidden; text-align: left;">
					<div style="padding-left: 32px; font-size: 11pt; font-weight: 600; background: url(../images/h2_icon.gif) no-repeat 5px 5px;
						line-height: 30px; padding-bottom: 0px">
						岗位管理
					</div>
				</div>
			</td>
		</tr>
		<tr>
			<td style="height: 24px">
				<input id="btnAdd" class="formButton" onclick="onCreateOrUpdateClick('');" type="button"
					value="新增岗位" name="btnAdd" />
			</td>
		</tr>
		<tr>
			<td style="vertical-align: top">
				<MCS:DeluxeGrid ID="DeluxeGridPost" runat="server" ShowExportControl="true"
					TitleFontSize="Small" Width="100%" DataSourceID="ObjectDataSourcePost"
					AllowPaging="true" PageSize="10" AllowSorting="true" AutoGenerateColumns="false"
					PagerSettings-Mode="NextPreviousFirstLast" PagerSettings-PreviousPageText="上一页"
					PagerSettings-NextPageText="下一页" PagerSettings-Position="Bottom" CaptionAlign="Right"
					CssClass="dataList" TitleCssClass="title" OnRowDataBound="DeluxeGridPost_RowDataBound"
					OnRowCommand="DeluxeGridPost_RowCommand" OnExportData="DeluxeGridPost_ExportData">
					<HeaderStyle CssClass="head" />
					<RowStyle CssClass="item" />
					<AlternatingRowStyle CssClass="aitem" />
					<SelectedRowStyle CssClass="selecteditem" />
					<PagerStyle CssClass="pager" />
					<EmptyDataTemplate>
						没有查询到您所需要的数据，请重新输入查询条件进行查询
					</EmptyDataTemplate>
				      <Columns>
                     	<asp:BoundField DataField="PostName" HeaderText="岗位名称" SortExpression="POST_NAME" />
						<asp:BoundField DataField="Category" HeaderText="类别" SortExpression="CATEGORY" />
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
                                    color: Blue;" Text="删除" CommandName="DeletePost" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
					<PagerSettings Mode="NextPreviousFirstLast" NextPageText="下一页" Position="TopAndBottom"
						PreviousPageText="上一页"></PagerSettings>
				</MCS:DeluxeGrid>
				<asp:ObjectDataSource ID="ObjectDataSourcePost" runat="server" EnablePaging="True"
					SelectCountMethod="GetQueryCount" SelectMethod="Query" 
					TypeName="MCS.Library.SOA.DataObjects.WfPostQuery"
					EnableViewState="False" OnSelected="ObjectDataSourcePost_Selected"
					OnSelecting="ObjectDataSourcePost_Selecting">
				</asp:ObjectDataSource>
			</td>
		</tr>
	</table>
	<asp:LinkButton ID="RefreshButton" runat="server" OnClick="RefreshButton_Click"></asp:LinkButton>
	</form>
</body>
</html>
