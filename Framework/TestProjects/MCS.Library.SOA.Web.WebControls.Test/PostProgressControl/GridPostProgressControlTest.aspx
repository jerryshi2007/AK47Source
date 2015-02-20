<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridPostProgressControlTest.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.PostProgressControl.GridPostProgressControlTest" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="MCS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>带Grid的PostProgressControl测试页面</title>
	<script type="text/javascript">
		function onCompleted(e) {
			if (e.dataChanged)
				$get("refreshButton").click();
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<p>
			这个页面演示了如何处理Grid中选中的数据行。这些选中的数据进行上传，服务器端按照数组的每一项进行处理，处理过程中反馈处理进度。
		</p>
		<p>
			处理完成后，OnClientComplete事件可以返回数据是否更新，客户端由此判断是否进行刷新页面等后续处理。
		</p>
		<p>
			客户端的数据会通过JSON序列化Post到服务器端，服务器端可以通过PrepareData来反序列化数据。如果不响应此事件，会按照默认的反序列化规则。然后应用通过响应DoPostedData来处理数据。
		</p>
	</div>
	<div>
		<MCS:PostProgressControl runat="server" ID="uploadProgress" DialogTitle="Post data test"
			ControlIDToShowDialog="doUploadBtn" OnClientCompleted="onCompleted" OnDoPostedData="uploadProgress_DoPostedData"
			DataSelectorControlID="dataGrid" />
	</div>
	<div>
		<input runat="server" id="doUploadBtn" type="button" value="Process..." />
		<asp:Button runat="server" ID="refreshButton" Text="Refresh" 
			Style="display: none" onclick="refreshButton_Click" />
	</div>
	<div>
		<MCS:DeluxeGrid ID="dataGrid" runat="server" CellPadding="4" Width="100%" ForeColor="Blue"
			GridLines="None" PagerStyle-BackColor="blue" CssClassMouseOver="OverRow" AllowPaging="True"
			SetGridViewTitle="测试列表" BackColor="RoyalBlue" BorderColor="Crimson" AutoGenerateColumns="False"
			UseAccessibleHeader="False" CellSpacing="1" CaptionAlign="Top" RecordCount="0"
			PagerGotoMode="true" TitleColor="141, 143, 149" TitleFontSize="Large" ShowCheckBoxes="true"
			CheckBoxPosition="Left" DataKeyNames="OrderID" OnPageIndexChanging="dataGrid_PageIndexChanging">
			<FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
			<RowStyle BackColor="#EFF3FB" />
			<EditRowStyle BackColor="#2461BF" />
			<SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
			<PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
			<HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
			<AlternatingRowStyle BackColor="White" />
			<Columns>
				<asp:BoundField DataField="OrderID" Visible="False">
					<ItemStyle HorizontalAlign="Center" />
				</asp:BoundField>
				<asp:BoundField DataField="SortID" HeaderText="Sort ID">
					<ItemStyle HorizontalAlign="Right" Width="48px" />
				</asp:BoundField>
				<asp:BoundField DataField="OrderName" HeaderText="Order Name">
					<ItemStyle HorizontalAlign="Center" />
				</asp:BoundField>
				<asp:BoundField DataField="Priority" HeaderText="Priority">
					<ItemStyle HorizontalAlign="Center" />
				</asp:BoundField>
				<asp:BoundField DataField="CreateTime" HeaderText="Create Time">
					<ItemStyle HorizontalAlign="Center" />
				</asp:BoundField>
				<asp:BoundField DataField="CreateUser" HeaderText="Create User">
					<ItemStyle HorizontalAlign="Center" />
				</asp:BoundField>
				<asp:BoundField DataField="Status" HeaderText="Status">
					<ItemStyle HorizontalAlign="Center" />
				</asp:BoundField>
			</Columns>
			<PagerSettings Position="TopAndBottom" Mode="NextPreviousFirstLast" />
		</MCS:DeluxeGrid>
	</div>
	</form>
</body>
</html>
