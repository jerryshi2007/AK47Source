<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserMappingManager.aspx.cs" Inherits="MCS.OA.CommonPages.UserMapping.UserMappingManager" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HBEX" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<base target="_self">
	<title>外网用户映射</title>
	<script type="text/javascript">
	    function onCreateOrUpdateClick(extUserId,mappingUserId) {

	        event.returnValue = false;

	        var feature = "dialogWidth:570px; dialogHeight:400px; center:yes; help:no; resizable:no;status:no;scroll:no";
	        var paramStr = '';
	        if (extUserId != '') {
	            paramStr = '?extUserId=' + extUserId + '&mappingUserId=' + mappingUserId;
	        }
	        var sPath = "SetUserMappingRelation.aspx" + paramStr;

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
						关系人管理
					</div>
				</div>
			</td>
		</tr>
		<tr>
			<td style="vertical-align: top">
				<MCS:DeluxeGrid ID="DeluxeGridExtUserMapping" runat="server" ShowExportControl="true"
					TitleFontSize="Small" Width="100%" DataSourceID="ObjectDataSourceUserMapping"
					AllowPaging="true" PageSize="10" AllowSorting="true" AutoGenerateColumns="false"
					CaptionAlign="Right" CssClass="dataList" TitleCssClass="title" GridTitle="关系列表" OnRowDataBound="DeluxeGridExtUserMapping_RowDataBound"
					OnExportData="DeluxeGridExtUserMapping_ExportData"  OnRowCommand="DeluxeGridExtUserMapping_RowCommand">
					<HeaderStyle CssClass="head" />
					<RowStyle CssClass="item" />
					<AlternatingRowStyle CssClass="aitem" />
					<SelectedRowStyle CssClass="selecteditem" />
					<PagerStyle CssClass="pager" />
					<EmptyDataTemplate>
						没有查询到您所需要的数据，请重新输入查询条件进行查询
					</EmptyDataTemplate>
				      <Columns>
                     	<asp:BoundField DataField="UserName" HeaderText="外部用户" SortExpression="UserName" />
                         <asp:BoundField DataField="DisplayName" HeaderText="显示名称" SortExpression="DisplayName" />
						<asp:BoundField DataField="UserType" HeaderText="用户类型" SortExpression="UserType" />
						<asp:BoundField DataField="Status" HeaderText="状态" SortExpression="Status" />
                        <asp:BoundField DataField="CreateTime" HeaderText="创建时间" SortExpression="CREATETIME" />
                        <asp:BoundField DataField="MappingUserLoginName" HeaderText="内部用户" />
                        <asp:TemplateField HeaderText="编辑">
                            <ItemStyle ForeColor="blue" HorizontalAlign="Center" Width="15%" />
                            <ItemTemplate>
                                <a id="LinkBtnEdit" runat="server" style="cursor: hand; text-decoration: underline;
                                    color: Blue;">设置关系</a> 
                                    <asp:LinkButton ID="LinkBtnDel" runat="server" Style="cursor: hand; text-decoration: underline;
                                    color: Blue;" Text="解除关系" CommandName="DeleteMapping" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
					<PagerSettings Mode="NextPreviousFirstLast" NextPageText="下一页" Position="Bottom"
						PreviousPageText="上一页"></PagerSettings>
				</MCS:DeluxeGrid>
				<asp:ObjectDataSource ID="ObjectDataSourceUserMapping" runat="server" EnablePaging="True"
					SelectCountMethod="GetQueryCount" SelectMethod="Query" 
					TypeName="MCS.Library.SOA.DataObjects.ExtUserMappingQuery"
					EnableViewState="False" OnSelected="ObjectDataSourceUserMapping_Selected"
					OnSelecting="ObjectDataSourceUserMapping_Selecting">
				</asp:ObjectDataSource>
			</td>
		</tr>
	</table>
    <asp:LinkButton ID="RefreshButton" runat="server" OnClick="RefreshButton_Click"></asp:LinkButton>
	</form>
</body>
</html>

