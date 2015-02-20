<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Banner.ascx.cs" Inherits="PermissionCenter.WebControls.Banner" %>
<%@ Register TagPrefix="mcsp" Namespace="MCS.Library.Web.Controls" Assembly="MCS.Library.Passport" %>
<h1 class="pc-frame-logo">
	<asp:HyperLink ID="bannerBtnHome" runat="server" NavigateUrl="~/default.aspx">远洋地产</asp:HyperLink>
</h1>
<ul class="pc-frame-top-nav" id="frameTopNav">
	<li>
		<asp:HyperLink ID="bannerBtnAllMembers" runat="server" NavigateUrl="~/lists/AllMembers.aspx">所有人员</asp:HyperLink>
	</li>
	<li>
		<asp:HyperLink ID="bannerBtnRootSchemas" runat="server" NavigateUrl="~/lists/RootSchemas.aspx">组织架构</asp:HyperLink>
	</li>
	<li id="dimension_menu" class="pc-dimension-menu"><span style="display: block; display: inline-block">
		<asp:HyperLink ID="bannerBtnOrgs" CssClass="pc-menu-arrow" runat="server" NavigateUrl="~/lists/OUExplorer.aspx">组织<i></i></asp:HyperLink>
	</span>
		<div style="position: relative">
			<dl style="position: absolute">
				<dd>
					<div class="pc-popup-nav" id="dimension_menu_coms">
						<pc:SchemaDataSource runat="server" ID="dsRoot">
						</pc:SchemaDataSource>
						<div style="max-height: 300px; overflow: hidden; position: relative;" class="pc-spin-container">
							<asp:Repeater runat="server" ID="bannerOrgList" OnItemDataBound="bannerOrgList_ItemDataBound"
								DataSourceID="dsRoot" DataMember="rootorgs">
								<HeaderTemplate>
									<ul id="dimension_menu_content">
								</HeaderTemplate>
								<ItemTemplate>
									<li>
										<asp:HyperLink ID="banOrgItem" runat="server"><%#Server.HtmlEncode((string)Eval("DisplayName")) %></asp:HyperLink>
									</li>
								</ItemTemplate>
								<FooterTemplate>
									</ul>
								</FooterTemplate>
							</asp:Repeater>
						</div>
						<div class="pc-spin-up">
							<i></i>
						</div>
						<div class="pc-spin-down">
							<i></i>
						</div>
					</div>
				</dd>
			</dl>
		</div>
	</li>
	<li>
		<asp:HyperLink ID="bannerBtnAllGroups" runat="server" NavigateUrl="~/lists/AllGroups.aspx">所有群组</asp:HyperLink></li>
	<li>
		<asp:HyperLink ID="bannerBtnApps" runat="server" NavigateUrl="~/lists/AllApps.aspx">授权管理</asp:HyperLink></li>
	<li>
		<asp:HyperLink ID="bannerBtnLogs" runat="server" NavigateUrl="~/lists/LogList.aspx">操作日志</asp:HyperLink></li>
</ul>
<ul class="pc-user-menu">
	<li class="pc-timetrap" id="timetrap">
		<div class="clear pc-timetrap-sub">
			<div>
				<asp:LinkButton ID="btnPresent" runat="server" CssClass="pc-cmd" OnClick="ShuttleNow">现在</asp:LinkButton>
			</div>
			<div>
				<asp:LinkButton ID="btnPickTime" runat="server" CssClass="pc-cmd" OnClientClick="return $pc.popups.pickTime(this);"
					OnClick="ShuttleAny">过去……</asp:LinkButton>
				<asp:HiddenField runat="server" ID="bannerCustomTime" />
			</div>
			<asp:Repeater runat="server" ID="recentList" OnItemCommand="HandleItemCommand">
				<HeaderTemplate>
					<dl>
						<dt>最近的时间点</dt>
				</HeaderTemplate>
				<ItemTemplate>
					<dd>
						<div class="pc-recent">
							<asp:LinkButton ID="btnRecent" runat="server" CssClass="pc-recent-item pc-cmd" CommandName="TimeShuttle"
								CommandArgument='<%#Eval("TimePoint") %>'><%#Eval("TimePoint","{0:yyyy-MM-dd HH:mm:ss}")%></asp:LinkButton>
							<asp:LinkButton ID="btnDeleteRecent" runat="server" CommandName="Delete" CssClass="pc-recent-delete"
								CommandArgument='<%#Eval("TimePoint") %>'></asp:LinkButton>
						</div>
					</dd>
				</ItemTemplate>
				<FooterTemplate>
					</dl>
				</FooterTemplate>
			</asp:Repeater>
			<div class="clear pc-timetrap-mt">
				<asp:LinkButton ID="btnClearRecent" runat="server" CssClass="pc-cmd" CommandName="ClearTimes"
					OnClick="ClearRecent">清除所有</asp:LinkButton></div>
		</div>
		<a href="javascript:void(0)" class="hd pc-menu-switch pc-menu-arrow"><span id="timemark"
			runat="server">现在</span><i></i></a> </li>
	<li id="userprofile" class="pc-userprofile">
		<div class="clear pc-userprofile-sub">
			<ul class="pc-p">
				<li class="pc-lp">
					<div class="pc-photo-pan" style="margin-left: -15px">
						<soa:UserPresence runat="server" ID="userPresence" ShowUserIcon="true" StatusImage="LongBar">
						</soa:UserPresence>
						<%--<a href="#">
							<asp:Image runat="server" ID="imgLogonImage" Height="32" Width="32" AlternateText="修改头像"
								ToolTip="修改头像" />
						</a>--%>
					</div>
				</li>
				<li class="pc-rp">
					<div>
						<div>
							<asp:HyperLink runat="server" ID="lnkSysMan" CssClass="pc-cmd" NavigateUrl="~/dialogs/Maintain.aspx">管理功能</asp:HyperLink>
						</div>
						<div>
							<asp:HyperLink ID="lnkProfile1" runat="server" CssClass="pc-cmd" NavigateUrl="~/dialogs/Profile.aspx">个人账户设置
							</asp:HyperLink>
						</div>
						<div>
							<asp:HyperLink runat="server" ID="lnkPassword" CssClass="pc-cmd" onclick="$pc.showDialog(this.href,'','',false,400,300,true);return false; ">修改口令</asp:HyperLink>
						</div>
						<div>
							<mcsp:SignInLogoControl runat="server" ID="SignInLogo" CssClass="pc-signin-logo"
								AutoRedirect="true" />
						</div>
					</div>
				</li>
			</ul>
		</div>
		<asp:HyperLink ID="lnkProfile" runat="server" CssClass="pc-menu-switch pc-menu-arrow"
			NavigateUrl="~/dialogs/Profile.aspx">
			<span id="userLogonName" runat="server">登录用户名</span><i></i>
		</asp:HyperLink>
	</li>
</ul>
<div>
	<script type="text/javascript">
		$pc.ui.hoverBehavior("dimension_menu");
		$pc.ui.hoverBehavior("timetrap");
		$pc.ui.hoverBehavior("userprofile");
		$pc.ui.autoSticky($pc.get('frameTopNav').parentNode);
		$pc.ui.configSpinner("dimension_menu_coms", "dimension_menu");
	</script>
</div>
