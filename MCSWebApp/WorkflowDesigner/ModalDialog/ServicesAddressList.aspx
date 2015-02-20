<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServicesAddressList.aspx.cs"
	Inherits="WorkflowDesigner.ModalDialog.ServicesAddressList" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>服务地址列表</title>
	<meta http-equiv="Pragma" content="no-cache">
	<meta http-equiv="Cache-Control" content="no-cache">
	<meta http-equiv="Expires" content="0">
	<base target="_self" />
	<link href="../css/dlg.css" rel="stylesheet" type="text/css" />
	<link href="/MCSWebApp/Css/WebControls/ClientGrid.css" rel="stylesheet" type="text/css" />
	<script type="text/javascript" src="../js/jquery-1.4.3.js"></script>
	<script type="text/javascript">
		function addServicesAddress() {
			var sFeature = "dialogWidth:400px; dialogHeight:260px;center:yes;help:no;resizable:yes;scroll:no;status:no";
			var result;
			result = window.showModalDialog("ServicesAddressEditor.aspx", null, sFeature);
			if (result) {
				document.getElementById("btnRefresh").click();
			}
		}

		function modifyServicesAddress(key) {
			var sFeature = "dialogWidth:400px; dialogHeight:260px;center:yes;help:no;resizable:no;scroll:no;status:no";
			var result;
			result = window.showModalDialog(String.format("ServicesAddressEditor.aspx?key={0}", escape(key)), null, sFeature);
			if (result) {
				document.getElementById("btnRefresh").click();
			}
		}

		function onDeleteClick() {
			var selectedKeys = $find("ServicesAddressDeluxeGrid").get_clientSelectedKeys();
			if (selectedKeys.length <= 0) {
				alert("请选择要删除的服务地址！");
				return false;
			}
			var msg = "您确定要删除吗？";
			if (confirm(msg) == true) {
				document.getElementById("btnConfirm").click();
			}
		}

		function onRefreshClick() {
			$("#search_Key").val($("#text_Key").val());
			$("#search_Action").val($("#dropSendType").val());
			$("#search_form").submit();
		}

		function doConfirm() {
			var grid = $find("ServicesAddressDeluxeGrid");
			if (grid) {
				if (grid.get_clientSelectedKeys().length) {
					return true;
					//$get("btnOk").click();
				} else {
					alert("至少应选择一项");
				}
			} else {
				alert('表格呢?');
			}
			return false;
		}
	</script>
</head>
<body class="pcdlg">
	<form action="ServicesAddressList.aspx" method="get" id="search_form">
	<input type="hidden" runat="server" id="search_Key" />
	<input type="hidden" runat="server" id="search_Action" />
	</form>
	<form id="form1" runat="server" class="dialogContent">
	<div class="pcdlg-sky">
		<div class="gridHead">
			<div class="dialogTitle" style="line-height: 32px;">
				<span class="dialogLogo">服务地址列表</span>
			</div>
		</div>
	</div>
	<div class="pcdlg-content">
		<div style="position: static; zoom: 1">
			<table style="width: 100%; height: 100%; vertical-align: top;">
				<tr>
					<td style="height: 30px;">
						<fieldset>
							<table style="width: auto">
								<tr>
									<th style="width: 16%" class="label">
										KEY：
									</th>
									<td style="text-align: left; width: 16%">
										<SOA:HBTextBox ID="text_Key" runat="server" Height="20px" Width="200px" />
									</td>
									<th style="width: 16%" class="label">
										发送方式：
									</th>
									<td style="text-align: left; width: 16%">
										<SOA:HBDropDownList ID="dropSendType" runat="server">
											<asp:ListItem Selected="True" Text="全部" Value="All" />
											<asp:ListItem Text="GET" Value="Get" />
											<asp:ListItem Text="POST" Value="Post" />
											<asp:ListItem Text="SOAP" Value="Soap" />
										</SOA:HBDropDownList>
									</td>
									<td style="text-align: left;">
										<input type="submit" id="btnRefreshClient" class="formButton" value="查询" onclick="onRefreshClick(); return false;" />
										<div style="display: none">
											<SOA:SubmitButton runat="server" ID="btnRefresh" Style="display: none" OnClick="btnRefresh_Click"
												RelativeControlID="btnRefreshClient" PopupCaption="正在更新数据..." />
										</div>
									</td>
								</tr>
							</table>
						</fieldset>
					</td>
				</tr>
				<tr>
					<td style="height: 30px; background-color: #C0C0C0">
						<a href="#" onclick="addServicesAddress();">
							<img src="../../../MCSWebApp/Images/appIcon/15.gif" alt="新建" border="0" />
						</a><a href="#" onclick="onDeleteClick();">
							<img src="../../../MCSWebApp/Images/16/delete.gif" alt="删除" border="0" /></a>
						<SOA:SubmitButton runat="server" ID="btnConfirm" Style="display: none" OnClick="btnDelete_Click"
							RelativeControlID="btnDelete" PopupCaption="正在删除..." />
					</td>
				</tr>
				<tr>
					<td colspan="3" style="width: auto; height: 100%; vertical-align: top;">
						<div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
							height: 100%; overflow: auto">
							<MCS:DeluxeGrid ID="ServicesAddressDeluxeGrid" runat="server" AutoGenerateColumns="False"
								OnPageIndexChanging="ServicesAddressDeluxeGrid_PageIndexChanging" DataSourceMaxRow="0"
								AllowPaging="True" Width="100%" DataKeyNames="Key" GridTitle="Test" CssClass="dataList"
								ShowExportControl="False" ShowCheckBoxes="True" OnRowDataBound="ServicesAddressDeluxeGrid_RowDataBound"
								DataSourceID="dataSourceMain" CascadeControlID="" TitleColor="141, 143, 149"
								TitleFontSize="Large">
								<Columns>
									<asp:BoundField DataField="Key" HeaderText="Key" SortExpression="" ItemStyle-Width="100px"
										ItemStyle-Wrap="true" />
									<asp:BoundField DataField="Address" HeaderText="服务URL" SortExpression="" ItemStyle-HorizontalAlign="Left">
										<ItemStyle Width="500px" Wrap="true" />
									</asp:BoundField>
									<asp:BoundField DataField="RequestMethod" HeaderText="发送方式" ItemStyle-Width="80px"
										ItemStyle-Wrap="true" />
									<asp:BoundField DataField="Credential" HeaderText="凭据" ItemStyle-Width="80px" ItemStyle-Wrap="true" />
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
							<asp:ObjectDataSource runat="server" ID="dataSourceMain" EnablePaging="True" SelectCountMethod="GetQueryCount"
								SelectMethod="Query" SortParameterName="orderBy" TypeName="WorkflowDesigner.ModalDialog.ServicesAddressListDataSource">
								<SelectParameters>
									<asp:QueryStringParameter Name="key" QueryStringField="search_Key" />
									<asp:QueryStringParameter Name="actionMethod" QueryStringField="search_Action" />
								</SelectParameters>
							</asp:ObjectDataSource>
						</div>
					</td>
				</tr>
			</table>
		</div>
	</div>
	<div class="pcdlg-floor">
		<div>
			<input type="hidden" id="hiddenValue" runat="server" />
			<asp:Button runat="server" ID="btnOk" OnClick="ConfirmClick" CssClass="formButton"
				Text="确定(O)" AccessKey="O" OnClientClick="return doConfirm();" /><input type="button"
					class="formButton" onclick="window.close();" value="取消(C)" id="btnCancel" accesskey="C" /></div>
	</div>
	<asp:Literal ID="postScript" runat="server" Mode="PassThrough" EnableViewState="false"></asp:Literal>
	</form>
</body>
</html>
