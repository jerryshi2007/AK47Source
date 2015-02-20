<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserInfoExtendView.aspx.cs" Inherits="MCS.OA.CommonPages.UserInfoExtend.UserInfoExtendView" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
 
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<base target="_self" />
	<title>用户扩展信息</title>
	<link href="../CSS/Ajax.css" type="text/css" rel="stylesheet" />
	<link href="../CSS/Dialog.css" type="text/css" rel="stylesheet" />
	<link href="../CSS/htc.css" type="text/css" rel="stylesheet" />
	<link href="../CSS/ItemDetail.css" type="text/css" rel="stylesheet" />
	<link href="../CSS/Login.css" type="text/css" rel="stylesheet" />
	<link href="../CSS/Other.css" type="text/css" rel="stylesheet" />
	<link href="../CSS/style.css" type="text/css" rel="stylesheet" />
	<style type="text/css">
		table
		{
			background-color: #D2D2D2;
			border: 0px;
			width: 96%;
		}
		table tr
		{
			line-height: 26px;
			background-color: White;
		}
	</style>

	<script type="text/javascript">
	    function onUpdateClick(fullPath) {
	        var feature = "width=640,height=480,status=no,toolbar=no,menubar=no,location=no,resizable=yes";
	        var sPath = "UserInfoExtendEdit.aspx?fpath="+fullPath;
	        var wnd = window.open(sPath, "UserInfoExtend", feature);
	        wnd.focus();
	    }
	</script>

</head>
<body>
	<form id="Form1" runat="server">
	<div>
        <HB:DataBindingControl runat="server" ID="bindingControl" AutoValidate="false" IsValidateOnSubmit="false">
			<ItemBindings>
				<HB:DataBindingItem ControlID="lblName" DataPropertyName="DisplayName" />
				<HB:DataBindingItem ControlID="LabelName" DataPropertyName="DisplayName" />
				<HB:DataBindingItem ControlID="labLogonName" DataPropertyName="LogonName" />
				<HB:DataBindingItem ControlID="labFullPath" DataPropertyName="FullPath" />
				<HB:DataBindingItem ControlID="labGender" DataPropertyName="Gender" />
				<HB:DataBindingItem ControlID="labNation" DataPropertyName="Nation" />
				<HB:DataBindingItem ControlID="labMobile" DataPropertyName="Mobile" />
				<HB:DataBindingItem ControlID="labOffice" DataPropertyName="OfficeTel" />
				<HB:DataBindingItem ControlID="labBirthday" DataPropertyName="Birthday" Format="{0:yyyy-MM-dd}" />
				<HB:DataBindingItem ControlID="labWorkTime" DataPropertyName="StartWorkTime" Format="{0:yyyy-MM-dd}" />
				<HB:DataBindingItem ControlID="labIntranet" DataPropertyName="IntranetEmail" />
				<HB:DataBindingItem ControlID="labInternet" DataPropertyName="InternetEmail" />
				<HB:DataBindingItem ControlID="labIMAddress" DataPropertyName="IMAddress" />
				<HB:DataBindingItem ControlID="labMemo" DataPropertyName="MEMO" />
			</ItemBindings>
        </HB:DataBindingControl>
   
	</div>
	<div id="dcontainer">
		<table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;" border="0">
			<tr>
				<td valign="top">
					<div id="dheader">
						<h1>
							<asp:Label ID="LabelName" runat="server" Text="Label"></asp:Label>的个人信息
						</h1>
					</div>
				</td>
			</tr>
			<tr>
				<td valign="top">
					<div id="dcontent" style="overflow: auto; width: 100%; height: 350px">
						<table cellspacing="1" cellpadding="0" width="100%">
							<tr>
								<td class="fim_l">
									姓名：
								</td>
								<td>
									<HB:HBTextBox ID="lblName" runat="server" ReadOnly="true"></HB:HBTextBox>
                                  
								</td>
								<td class="fim_l">
									登录名：
								</td>
								<td>
									<HB:HBTextBox ID="labLogonName" runat="server" ReadOnly="true"></HB:HBTextBox>
								</td>
							</tr>
							<tr>
								<td class="fim_l">
									全路径：
								</td>
								<td colspan="3">
									<HB:HBTextBox ID="labFullPath" runat="server" ReadOnly="true"></HB:HBTextBox>
								</td>
							</tr>
							<tr>
								<td class="fim_l">
									性别：
								</td>
								<td>
									<HB:HBTextBox ID="labGender" runat="server" ReadOnly="true"></HB:HBTextBox>
								</td>
								<td class="fim_l">
									民族：
								</td>
								<td>
									<HB:HBTextBox ID="labNation" runat="server" ReadOnly="true"></HB:HBTextBox>
								</td>
							</tr>
							<tr>
								<td class="fim_l">
									手机号：
								</td>
								<td>
									<HB:HBTextBox ID="labMobile" runat="server" ReadOnly="true"></HB:HBTextBox>
								</td>
								<td class="fim_l">
									办公室电话：
								</td>
								<td>
									<HB:HBTextBox ID="labOffice" runat="server" ReadOnly="true"></HB:HBTextBox>
								</td>
							</tr>
							<tr>
								<td class="fim_l">
									出生日期：
								</td>
								<td>
									<HB:HBTextBox ID="labBirthday" runat="server" ReadOnly="true"></HB:HBTextBox>
								</td>
								<td class="fim_l">
									入职日期：
								</td>
								<td>
									<HB:HBTextBox ID="labWorkTime" runat="server" ReadOnly="true"></HB:HBTextBox>
								</td>
							</tr>
							<tr>
								<td class="fim_l">
									内网EMail：
								</td>
								<td>
									<HB:HBTextBox ID="labIntranet" runat="server" ReadOnly="true"></HB:HBTextBox>
								</td>
								<td class="fim_l">
									外网EMail：
								</td>
								<td>
									<HB:HBTextBox ID="labInternet" runat="server" ReadOnly="true"></HB:HBTextBox>
								</td>
							</tr>
							<tr>
								<td class="fim_l">
									即时消息：
								</td>
								<td>
									<HB:HBTextBox ID="labIMAddress" runat="server" ReadOnly="true"></HB:HBTextBox>
								</td>
								<td class="fim_l">
									备注：
								</td>
								<td>
									<HB:HBTextBox ID="labMemo" runat="server" ReadOnly="true"></HB:HBTextBox>
								</td>
							</tr>
							<tr>
								<td style="height: 4px;">
									&nbsp;
								</td>
								<td colspan="3">
								</td>
							</tr>
							<tr valign="top">
								<td class="fim_l">
									各应用中的角色：
								</td>
								<td colspan="3">
									<MCS:DeluxeGrid ID="gridAppRoles" runat="server" AutoGenerateColumns="False" ShowExportControl="false"
										CssClass="dataList" TitleCssClass="title" Width="100%">
										<HeaderStyle CssClass="head" />
										<RowStyle CssClass="item" />
										<AlternatingRowStyle CssClass="aitem" />
										<SelectedRowStyle CssClass="selecteditem" />
										<PagerStyle CssClass="pager" />
										<Columns>
											<asp:BoundField HtmlEncode="True" HeaderText="应用名称" DataField="AppName">
												<ItemStyle HorizontalAlign="Left" />
											</asp:BoundField>
											<asp:BoundField HtmlEncode="True" HeaderText="角色" DataField="Roles">
												<ItemStyle HorizontalAlign="Left" />
											</asp:BoundField>
										</Columns>
									</MCS:DeluxeGrid>
								</td>
							</tr>
						</table>
					</div>
				</td>
			</tr>
			<tr>
				<td style="height: 80px;" valign="middle">
					<div id="dfooter">
						<p style="vertical-align: middle; height: 40px;">
							<input accesskey="U" id="btnUpdate" class="formButton" style="width: 80px" type="button" value="修改(U)"
								name="btnUpdate" runat="server"/>
							<input accesskey="C" id="btnClose" class="formButton" onclick="window.close();" type="button"
								value="关闭(C)" name="btnClose"/>
						</p>
					</div>
				</td>
			</tr>
		</table>
	</div>
	</form>
</body>
</html>
