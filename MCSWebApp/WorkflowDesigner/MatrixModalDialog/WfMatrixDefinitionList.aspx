<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WfMatrixDefinitionList.aspx.cs"
	Inherits="WorkflowDesigner.MatrixModalDialog.WfMatrixDefinitionList" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>矩阵定义列表</title>
	<base target="_self" />
	<script type="text/javascript">
		function onClick() {
			var sFeature = "dialogWidth:800px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";
			var result;
			result = window.showModalDialog("MatrixDimensionDefinitionEditor.aspx", null, sFeature);

			if (result) {
				document.getElementById("hiddenServerBtn").click();
			}
		}

		function modifyMatrixDefine(matrixDefKey) {
			var sFeature = "dialogWidth:800px; dialogHeight:600px;center:yes;help:no;resizable:no;scroll:no;status:no";
			var result;
			result = window.showModalDialog(String.format("MatrixDimensionDefinitionEditor.aspx?matrixKey={0}", escape(matrixDefKey)), null, sFeature);

			if (result) {
				document.getElementById("hiddenServerBtn").click();
			}
		}

		function onOKBtnClick() {
			var selectedKeys = $find("MatrixDefDeluxeGrid").get_clientSelectedKeys();

			if (selectedKeys.length > 0)
				window.returnValue = selectedKeys[0];

			top.close();

        }

        function onDeleteClick() {
            var selectedKeys = $find("MatrixDefDeluxeGrid").get_clientSelectedKeys();
            if (selectedKeys.length <= 0) {
                alert("请选择要删除的矩阵定义！");
                return false; 
            }
            var msg = "您确定要删除吗？";
            if (confirm(msg) == true) {
                document.getElementById("btnConfirm").click();
            } 

		}
    </script>
</head>
<body>
	<form id="form1" runat="server">
	<div>
	</div>
	<table style="height: 100%; width: 100%">
		<tr>
			<td class="gridHead">
				<div class="dialogTitle">
					<span class="dialogLogo">矩阵定义列表</span>
				</div>
			</td>
		</tr>
		<tr>
			<td style="height: 40px; text-align: center; vertical-align: middle">
				<table style="height: 100%">
					<tr>
						<td style="text-align: left;">
							<input type="button" onclick="onClick();" value="新建(N)" id="Button1" accesskey="N"
								runat="server" class="formButton" />
						</td>
                        <td>
							<input type="button" onclick="onDeleteClick();" value="删除(D)" id="btnDelete" accesskey="D"
								runat="server" class="formButton" />
                            <SOA:SubmitButton runat="server" ID="btnConfirm" Style="display: none" OnClick="btnConfirm_Click"
								RelativeControlID="btnDelete" PopupCaption="正在删除..." />
                        </td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td style="vertical-align: top">
				<div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
					height: 100%; overflow: auto">
					<MCS:DeluxeGrid ID="MatrixDefDeluxeGrid" runat="server" AutoGenerateColumns="False"
						DataSourceID="ObjectDataSource" DataSourceMaxRow="0" AllowPaging="True" PageSize="10" Height="100%"
						Width="100%" DataKeyNames="Key" ExportingDeluxeGrid="False" GridTitle="Test"
						CssClass="dataList" ShowExportControl="False" ShowCheckBoxes="True" OnRowDataBound="MatrixDefDeluxeGrid_RowDataBound">
						<Columns>
							<asp:BoundField DataField="Key" HeaderText="矩阵定义Key" SortExpression="Key" />
							<asp:BoundField DataField="Name" HeaderText="矩阵定义名称" SortExpression="Name" ItemStyle-HorizontalAlign="Center">
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
							</asp:BoundField>
							<asp:BoundField DataField="Description" HeaderText="矩阵定义描述" />
							<asp:BoundField DataField="Enabled" HeaderText="是否启用" />
							<asp:TemplateField>
								<ItemTemplate>
									<a target="_blank" href="ExportMatrix.aspx?cmd=ExportMatrixDefinition&key=<%#Server.UrlEncode((string)Eval("Key"))%>">
										导出...</a>
								</ItemTemplate>
							</asp:TemplateField>
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
			</td>
		</tr>
		<tr>
			<td class="gridfileBottom">
			</td>
		</tr>
		<tr>
			<td style="height: 40px; text-align: center; vertical-align: middle">
				<table style="width: 100%; height: 100%">
					<tr>
						<td style="text-align: center;">
							<input type="button" class="formButton" onclick="onOKBtnClick();" runat="server"
								value="确定(O)" id="btnOK" accesskey="O" />
						</td>
						<td style="text-align: center;">
							<input type="button" class="formButton" onclick="top.close();" value="关闭(C)" id="btnCancel"
								accesskey="C" />
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	<div>
		<asp:ObjectDataSource ID="objectDataSource" runat="server" EnablePaging="True" SelectCountMethod="GetQueryCount"
			SelectMethod="Query" SortParameterName="orderBy" OnSelecting="objectDataSource_Selecting"
			OnSelected="objectDataSource_Selected" TypeName="WorkflowDesigner.MatrixModalDialog.MatrixDefinitionQuery"
			EnableViewState="False">
			<SelectParameters>
				<asp:ControlParameter ControlID="whereCondition" Name="where" PropertyName="Value"
					Type="String" />
				<asp:Parameter Direction="InputOutput" Name="totalCount" Type="Int32" />
			</SelectParameters>
		</asp:ObjectDataSource>
		<input runat="server" type="hidden" id="whereCondition" />
		<div style="display: none">
			<asp:Button ID="hiddenServerBtn" runat="server" Text="Button" 
                onclick="hiddenServerBtn_Click" /></div>
	</div>
	</form>
</body>
</html>
