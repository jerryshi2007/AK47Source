<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchContent.aspx.cs"
	Inherits="MCS.OA.CommonPages.UserInfoExtend.SearchContent" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="HBEX" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="HBXC" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<style type="text/css">
		.divTD td
		{
			padding: 2px 4px 2px 4px;
		}
		.useOOUI
		{
		}
		.style1
		{
			width: 194px;
		}
	</style>
	<script type="text/javascript">
		function onUpdateClick(fullpath) {
			var feature = "width=520,height=530,status=no,toolbar=no,menubar=no,location=no,resizable=yes";
			var sPath = "UserInfoExtendView.aspx?fpath=" + fullpath + "&mode=1";

			var wnd = window.open(sPath, "UserInfoExtend", feature);
			wnd.focus();
		}

		function onMobileClick(moblieNum) {
			var feature = "width=420,height=390,status=no,toolbar=no,menubar=no,location=no,resizable=no";
			var sPath = "http://eip/SMS.aspx?mobile=" + moblieNum;

			var wnd = window.open(sPath, "SendMoblie", feature);
			wnd.focus();
		}

		function document.onkeydown() {
			onEnterKeyDown("BtnSearch");
		}

		function onEditReportToClick() {
			var sFeature = "dialogWidth:320px; dialogHeight:200px;center:yes;help:no;resizable:no;scroll:no;status:no";

			event.returnValue = false;

			var link = event.srcElement;
			var result = window.showModalDialog(link, null, sFeature);

			if (result)
				document.getElementById("hiddenServerBtn").click();

			return false;
		}
	</script>
</head>
<body>
	<form id="form1" runat="server" style="margin: 1px; width: 100%; height: 100%">
	<div id="container">
		<div style="border-bottom: groove 1px Silver; overflow: hidden; text-align: left;
			margin-right: -13px;">
			<div style="padding-left: 32px; font-size: 11pt; font-weight: 600; background: url(../images/h2_icon.gif) no-repeat 5px 5px;
				line-height: 30px; padding-bottom: 0px">
				个人信息
			</div>
		</div>
		<table border="0" style="height: 35px; width: 100%; margin-left: 5px; margin-right: 5px;
			margin-top: 5px;" cellspacing="1" cellpadding="0" bgcolor="Silver">
			<tr align="left">
				<td valign="middle" align="right" style="background-color: #f2f8f8; width: 60px;">
				</td>
				<td valign="middle" align="center" style="background-color: #f7fbfa; width: 160px;">
					<asp:TextBox ID="tbFormName" runat="server" Width="150px"></asp:TextBox>
				</td>
				<td valign="middle" align="left" style="background-color: #f2f8f8;">
					&nbsp;&nbsp;
					<asp:LinkButton runat="server" ID="BtnSearch" OnClick="BtnSearch_Click"><img src="../Images/search.gif" alt="搜索" style="border:0;"/>搜索</asp:LinkButton>
				</td>
			</tr>
		</table>
		<div style="margin: 5px;" class="divTD">
			<HBXC:DeluxeGrid ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="ObjectDataSource1"
				AllowPaging="True" AllowSorting="True" PageSize="20" ShowExportControl="true"
				GridTitle="查询结果列表" ShowCheckBoxes="False" CheckBoxPosition="Right" DataKeyNames="ID"
				TitleFontSize="Small" CssClass="dataList" TitleCssClass="title" Width="100%"
				OnRowDataBound="GridView1_RowDataBound">
				<HeaderStyle CssClass="head" />
				<RowStyle CssClass="item" />
				<AlternatingRowStyle CssClass="aitem" />
				<SelectedRowStyle CssClass="selecteditem" />
				<PagerStyle CssClass="pager" />
				<EmptyDataTemplate>
					暂时没有您需要的数据
				</EmptyDataTemplate>
				<Columns>
					<asp:TemplateField HeaderText="姓名">
						<ItemStyle HorizontalAlign="Left" Wrap="false" />
						<ItemTemplate>
							<span style="padding-left: 16px">
								<HBEX:UserPresence ID="userPresence" runat="server" UserID='<%# Eval("ID") %>' UserDisplayName='<%# Eval("DisplayName") %>' />
							</span>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:BoundField HtmlEncode="True" HeaderText="职位" DataField="Occupation" Visible="false">
						<ItemStyle HorizontalAlign="Left" />
					</asp:BoundField>
					<asp:TemplateField HeaderText="手机号码">
						<ItemStyle />
						<ItemTemplate>
							<asp:LinkButton ID="lblMobile" runat="server" Text="">LinkButton 
							</asp:LinkButton>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="办公室电话">
						<ItemStyle HorizontalAlign="Left" />
						<ItemTemplate>
							<asp:Label ID="lblTel" runat="server" Text=""></asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="内网Email" Visible="false">
						<ItemStyle HorizontalAlign="Left" />
						<ItemTemplate>
							<asp:Label ID="lblIntranetEmail" runat="server" Text=""></asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="外网Email" Visible="false">
						<ItemStyle HorizontalAlign="left" />
						<ItemTemplate>
							<asp:Label ID="lblInternetEmail" runat="server" Text=""></asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Email">
						<ItemStyle HorizontalAlign="Center" />
						<ItemTemplate>
							<asp:HyperLink ID="HlEmail" runat="server" Text='<%#Eval("Email") %>' NavigateUrl='<%# Eval("Email", "mailto:{0}") %>' />
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="即时消息" Visible="false">
						<ItemStyle HorizontalAlign="left" />
						<ItemTemplate>
							<asp:Label ID="lblIMAddress" runat="server" Text=""></asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="博客">
						<ItemStyle ForeColor="blue" HorizontalAlign="Center" />
						<ItemTemplate>
							<a id="LinkBtnViewBlogs" runat="server" style="cursor: hand; text-decoration: underline;
								color: Blue;" target="_blank">查看</a>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="我的门户">
						<ItemStyle ForeColor="blue" HorizontalAlign="Center" />
						<ItemTemplate>
							<a id="LinkBtnViewKM" runat="server" style="cursor: hand; text-decoration: underline;
								color: Blue;" target="_blank">查看</a>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="汇报给">
						<ItemStyle HorizontalAlign="Left" />
						<ItemTemplate>
							<HBEX:UserPresence ID="reportTouserPresence" runat="server" ShowUserDisplayName="false" />
							<asp:HyperLink ID="linkReportTo" ForeColor="Blue" runat="server" onclick="onEditReportToClick();" />
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="编辑">
						<ItemStyle ForeColor="blue" HorizontalAlign="Center" Width="36px" />
						<ItemTemplate>
							<a id="LinkBtnViewDetails" runat="server" style="cursor: hand; text-decoration: underline;
								color: Blue;">编辑</a>
						</ItemTemplate>
					</asp:TemplateField>
				</Columns>
				<PagerSettings Mode="NextPreviousFirstLast" NextPageText="下一页" Position="TopAndBottom"
					PreviousPageText="上一页"></PagerSettings>
			</HBXC:DeluxeGrid>
			<asp:ObjectDataSource ID="ObjectDataSource1" runat="server" EnablePaging="false"
				SelectMethod="GetIUserWithCount" OnSelected="ObjectDataSource1_Selected" OnSelecting="ObjectDataSource1_Selecting"
				TypeName="MCS.OA.CommonPages.UserInfoExtend.SearchQuery" EnableViewState="False">
				<SelectParameters>
					<asp:ControlParameter ControlID="userName" Name="userName" PropertyName="Value" Type="String" />
					<asp:ControlParameter ControlID="departmentId" Name="departmentId" PropertyName="Value"
						Type="String" />
					<asp:Parameter Direction="InputOutput" Name="totalCount" Type="Int32" />
				</SelectParameters>
			</asp:ObjectDataSource>
		</div>
		<div>
			<input runat="server" type="hidden" id="userName" />
			<input runat="server" type="hidden" id="departmentId" />
			<table border="0" style="height: 80px; width: 100%; margin-left: 5px; margin-right: 5px;
				margin-top: 5px;" cellpadding="0" bgcolor="Silver" runat="server" id="company">
				<tr align="left">
					<td valign="middle" align="left" style="background-color: #f2f8f8; width: 100%;"
						class="style1">
						&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 公司：
						<asp:DropDownList ID="DDLCompany" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DDLCompany_SelectedIndexChanged">
						</asp:DropDownList>
					</td>
				</tr>
				<tr align="left">
					<td valign="middle" align="left" style="background-color: #f7fbfa; width: 100%;">
						&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="address" runat="server"></asp:Label>
					</td>
				</tr>
				<tr align="left">
					<td valign="middle" align="left" style="background-color: #f7fbfa; width: 100%;">
						&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="Fax" runat="server"></asp:Label>
					</td>
				</tr>
			</table>
		</div>
		<div>
		</div>
	</div>
	<div style="display: none">
		<asp:Button ID="hiddenServerBtn" runat="server" Text="Button" OnClick="hiddenServerBtn_Click" /></div>
	</form>
</body>
</html>
