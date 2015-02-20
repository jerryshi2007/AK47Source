<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="PermissionCenter.Dialogs.Profile" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>用户配置</title>
	<link rel="icon" href="../favicon.ico" type="image/x-icon" />
	<link rel="Shortcut Icon" href="../favicon.ico" />
	<pc:HeaderControl ID="HeaderControl1" runat="server" />
	<style type="text/css">
		.pc-config-group
		{
			margin-bottom: 20px;
		}
	</style>
</head>
<body>
	<form id="form1" runat="server">
	<asp:ScriptManager runat="server">
	</asp:ScriptManager>
	<div class="pc-frame-header">
		<pc:Banner ID="pcBanner" runat="server" ActiveMenuIndex="-1" />
	</div>
	<pc:BannerNotice ID="notice" runat="server"></pc:BannerNotice>
	<div class="pc-frame-container">
		<div class="pc-container5">
			<div class="pc-tabs-header2">
				<span class="pc-active">个人设置</span> <span class="" runat="server" id="tabMaintain"><a
					href="Maintain.aspx">管理功能</a></span>
			</div>
			<div class="pc-profile-groups">
				<fieldset class="pc-profile-group">
					<legend>账户</legend>
					<div>
						<asp:HyperLink runat="server" ID="lnkPassword" CssClass="pc-button pc-cmd" onclick="$pc.showDialog(this.href,'','',false,400,300,true);return false; ">修改口令</asp:HyperLink>
					</div>
				</fieldset>
				<fieldset class="pc-profile-group">
					<legend>偏好设置</legend>
					<div class="pc-container5">
						<div class="pc-config-group">
							<div>
								<h3>
									组织搜索视图</h3>
							</div>
							<div>
								<asp:RadioButton Text="分层模式" runat="server" ID="orgModeHierarchical" GroupName="orgMode" />
								<asp:RadioButton Text="列表模式" runat="server" ID="orgModeList" GroupName="orgMode" />
								<asp:RadioButton Text="保持上次的设置" runat="server" ID="orgModeAuto" GroupName="orgMode" />
							</div>
						</div>
						<div class="pc-config-group">
							<div>
								<h3>
									人员列表显示模式</h3>
							</div>
							<div>
								<asp:RadioButton Text="常规列表" runat="server" ID="userViewDetail" GroupName="userMode" />
								<asp:RadioButton Text="精简列表" runat="server" ID="userViewReduced" GroupName="userMode" />
								<asp:RadioButton Text="精简表格" runat="server" ID="userViewTable" GroupName="userMode" />
								<asp:RadioButton Text="保持上次的设置" runat="server" ID="userViewAuto" GroupName="userMode" />
							</div>
						</div>
						<div class="pc-config-group">
							<div>
								<h3>
									常规列表显示模式</h3>
							</div>
							<div>
								<asp:RadioButton Text="常规列表" runat="server" ID="generalViewList" GroupName="generalMode" />
								<asp:RadioButton Text="精简表格" runat="server" ID="generalViewTable" GroupName="generalMode" />
								<asp:RadioButton Text="保持上次的设置" runat="server" ID="generalViewAuto" GroupName="generalMode" />
							</div>
						</div>
						<div class="pc-config-group">
							<div>
								<h3>
									列表每页显示数据个数</h3>
							</div>
							<div>
								<asp:DropDownList runat="server" ID="ddlSizePerPage">
									<asp:ListItem Text="10个" Value="0" />
									<asp:ListItem Text="20个" Value="1" />
									<asp:ListItem Text="50个" Value="2" />
									<asp:ListItem Text="100个" Value="3" />
								</asp:DropDownList>
							</div>
						</div>
						<div>
							&nbsp;
						</div>
					</div>
					<div>
						<asp:Button Text="保存" runat="server" CssClass="pc-button" ID="btnSave" OnClick="HandleSave" />
					</div>
				</fieldset>
				<div>
					修订
				</div>
				<div>
					<asp:Label runat="server" ID="lbVersion" />
				</div>
			</div>
		</div>
	</div>
	</form>
</body>
</html>
