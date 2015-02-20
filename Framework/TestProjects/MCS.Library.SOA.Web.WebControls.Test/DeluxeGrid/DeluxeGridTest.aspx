<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeluxeGridTest.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.DeluxeGrid.DeluxeGridTest" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="MCS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <MCS:DeluxeGrid ID="ProcessDescInfoDeluxeGrid" runat="server" AutoGenerateColumns="False"
			DataSourceID="ObjectDataSource" DataSourceMaxRow="0" AllowPaging="True" PageSize="10"
			Width="100%" DataKeyNames="ProcessKey" ExportingDeluxeGrid="False" GridTitle="Test"
			CssClass="dataList" ShowExportControl="False" ShowCheckBoxes="True">
			<Columns>
				<asp:BoundField DataField="ProcessKey" HeaderText="流程模板Key" SortExpression="PROCESS_KEY" />
				<asp:BoundField DataField="ApplicationName" HeaderText="应用名称" SortExpression="APPLICATION_NAME"
					ItemStyle-HorizontalAlign="Center">
					<ItemStyle HorizontalAlign="Center"></ItemStyle>
                   
				</asp:BoundField>
				<asp:BoundField DataField="ProgramName" HeaderText="模块名称" SortExpression="PROGRAM_NAME" />
				<asp:BoundField DataField="ProcessName" HeaderText="流程模板名称" SortExpression="PROCESS_NAME" />
				<asp:BoundField DataField="CreateTime" HeaderText="创建时间" HtmlEncode="False" SortExpression="CreateTime"
					DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
				<asp:BoundField DataField="ModifyTime" HeaderText="修改时间" HtmlEncode="False" SortExpression="ModifyTime"
					DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
			</Columns>
			<PagerStyle CssClass="pager" />
			<RowStyle CssClass="item" />
			<CheckBoxTemplateItemStyle CssClass="checkbox" />
			<CheckBoxTemplateHeaderStyle CssClass="checkbox" />
			<HeaderStyle CssClass="head" />
			<AlternatingRowStyle CssClass="aitem" />
			<EmptyDataTemplate>
				暂时没有您需要的数据
			</EmptyDataTemplate>
			<PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" Mode="NextPreviousFirstLast"
				NextPageText="下一页" Position="Bottom" PreviousPageText="上一页"></PagerSettings>
		</MCS:DeluxeGrid>
    </div>
    	<asp:ObjectDataSource ID="objectDataSource" runat="server" EnablePaging="True" SelectCountMethod="GetQueryCount"
		SelectMethod="Query" SortParameterName="orderBy" OnSelecting="objectDataSource_Selecting"
		OnSelected="objectDataSource_Selected" TypeName="MCS.Library.SOA.Web.WebControls.Test.DeluxeGrid.ProcessDescriptorInfoQuery"
		EnableViewState="False">
		<SelectParameters>
			<asp:ControlParameter ControlID="whereCondition" Name="where" PropertyName="Value"
				Type="String" />
			<asp:Parameter Direction="InputOutput" Name="totalCount" Type="Int32" />
		</SelectParameters>
	</asp:ObjectDataSource>
    	<br />
    <br />
    	<input runat="server" type="hidden" id="whereCondition" />


    </form>
</body>
</html>
