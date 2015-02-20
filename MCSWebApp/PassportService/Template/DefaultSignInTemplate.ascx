<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DefaultSignInTemplate.ascx.cs"
	Inherits="MCS.Web.Passport.Template.DefaultSignInTemplate" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<style type="text/css">
	*
	{
		margin: 0;
		padding: 0;
	}
	body
	{
		margin: 0px;
	}
	img
	{
		margin: 0px;
		padding: 0px;
		border-top-style: none;
		border-right-style: none;
		border-bottom-style: none;
		border-left-style: none;
	}
	div
	{
		margin: 0px;
		padding: 0px;
		border-top-style: none;
		border-right-style: none;
		border-bottom-style: none;
		border-left-style: none;
	}
	td
	{
		font-family: Arial, Helvetica, sans-serif;
		font-size: 12px;
		line-height: 22px;
		color: #666;
		text-decoration: none;
	}
	.login
	{
		margin-right: auto;
		margin-left: auto;
		width: 881px;
		height: 333px;
		margin-top: 117px;
	}
	.login .box
	{
		padding-top: 83px;
	}
	.login .box #login-input
	{
		line-height: 18px;
		color: #336;
		border: 2px solid #aec7e5;
		width: 200px;
		height: 18px;
		font-size: 12px;
	}
	.login .box table tr td
	{
		font-family: Arial, Helvetica, sans-serif;
		font-size: 12px;
		color: #486785;
	}
	.login .box table tr td a
	{
		color: #C60;
		text-decoration: underline;
	}
</style>
<div class="login" style="text-align: left;">
	<img src="../images/login_03.gif" width="298" height="48" /><br />
	<table width="100%" border="0" cellspacing="0" cellpadding="0">
		<tr>
			<td width="246" height="333" background="../images/login_05.jpg">
				&nbsp;
			</td>
			<td width="247" background="../images/login_06.jpg">
				&nbsp;
			</td>
			<td width="388" valign="top" background="../images/login_07.gif" class="box">
				<table width="309" border="0" align="center" cellpadding="0" cellspacing="2">
					<tr>
						<td width="47" height="30">
							用户名
						</td>
						<td>
							<asp:TextBox ID="signInName" Width="200px" Style="margin-left: 4px" runat="server"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td height="30">
							密&nbsp;&nbsp;&nbsp;&nbsp;码
						</td>
						<td>
							<input id="password" type="password" style="margin-left: 4px; width: 200px" runat="server"
								name="password" />
						</td>
					</tr>
					<tr id="simulateDateRow" runat="server">
						<td>
							模拟日期
						</td>
						<td>
							<MCS:DeluxeDateTime Style="margin-left: 4px" runat="server" ID="simulateDate">
							</MCS:DeluxeDateTime>
						</td>
					</tr>
					<tr>
						<td>
							&nbsp;
						</td>
						<td>
							<asp:CheckBox ID="autoSignIn" runat="server" name="autoSignIn" Text="下次自动登录" />&nbsp;&nbsp;&nbsp;&nbsp;
							<asp:CheckBox ID="dontSaveUserName" runat="server" Text="请不要记录我的登录名"></asp:CheckBox>
							<%--<a href="#">忘记密码?</a>--%>
						</td>
					</tr>
					<tr>
						<td colspan="2">
							&nbsp;&nbsp;&nbsp;
						</td>
					</tr>
					<tr>
						<td height="105" colspan="2" style="text-align: center; vertical-align: bottom">
							<div>
								<div>
                                    <asp:ImageButton ID="SignInButton" runat="server"
										ImageUrl="~/images/login_btn.gif" />
								</div>
								<div>
									<img runat="server" id="shadowImage" src="~/images/login_btn_shadow.gif" />
								</div>
							</div>
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	<table width="100%" border="0" cellspacing="0" cellpadding="0">
		<tr>
			<td style="width: 16px">
			</td>
			<td align="center">
				<asp:Label ID="errorMessage" runat="server" ForeColor="red" Font-Bold="True" Style="line-height: 100%;
					word-break: break-all"></asp:Label>
				<a href="#" id="detailErrorMessageLink" runat="server" style="display: none; line-height: 100%">
					点击此处展开详细的错误信息...</a>
				<asp:Label ID="detailErrorMessage" runat="server" Style="display: none; word-break: break-all"></asp:Label>
			</td>
			<td style="width: 20px">
			</td>
		</tr>
	</table>
</div>
<%--<input runat="server" id="clientEnv" type="hidden" />
<table style="background-image: url(../images/bg.gif); width: 100%; height: 100%"
	cellpadding="0" cellspacing="0">
	<tr>
		<td style="text-align: center; vertical-align: middle">
			<div style="width: 300px">
				<table style="width: 100%; background-color: #EBF6FD" cellspacing="0" cellpadding="0"
					border="0">
					<tr>
						<td style="height: 50px; border: none 0 none">
							<table style="width: 100%; border: none 0px none" cellpadding="0" cellspacing="0">
								<tr>
									<td>
										<!--<img src="../images/hb2004banner.gif">-->
										<img src="../images/logo1.png" alt="" />
									</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td style="border-right-style: groove; border-right-width: 1px; border-right-color: #B9C1C6;
							border-left-style: solid; border-left-width: 1px; border-left-color: #B9C1C6;
							height: 1px">
						</td>
					</tr>
					<tr>
						<td valign="top" style="border-right-style: groove; border-right-width: 1px; border-color: #B9C1C6;
							border-left-style: solid; border-left-width: 1px; border-left-color: #B9C1C6;
							height: 130px">
							<table style="width: 100%" cellspacing="0" cellpadding="0">
								<tr>
									<td style="width: 120px; height: 48px" align="right">
										<strong>登录名:</strong>
									</td>
									<td>
										
									</td>
									<td style="width: 32px">
									</td>
								</tr>
								<tr>
									<td align="right" style="height: 48px">
										<strong>密码:</strong>
									</td>
									<td>
										<input id="password1" type="password" style="margin-left: 4px; width: 200px" runat="server"
											name="password" />
									</td>
									<td>
									</td>
								</tr>
								<tr>
									<td>
									</td>
									<td>
										<asp:CheckBox ID="CheckBox1" runat="server" Text="自动登录"></asp:CheckBox>
									</td>
									<td>
									</td>
								</tr>
								<tr>
									<td>
									</td>
									<td>
										<asp:CheckBox ID="dontSaveUserName" runat="server" Text="请不要记录我的登录名"></asp:CheckBox>
									</td>
									<td>
									</td>
								</tr>
								<tr>
									<td>
									</td>
									<td align="right" style="height: 26px">
										
									</td>
									<td>
									</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td style="border-right-style: groove; border-right-width: 1px; border-color: #B9C1C6;
							border-left-style: solid; border-left-width: 1px; border-left-color: #B9C1C6;">
							<table style="width: 100%" cellpadding="0" cellspacing="0">
								<tr>
									<td style="width: 16px">
									</td>
									<td align="center">
										<asp:Label ID="errorMessage" runat="server" ForeColor="red" Font-Bold="True" Style="line-height: 100%;
											word-break: break-all"></asp:Label>
										<a href="#" id="detailErrorMessageLink" runat="server" style="display: none; line-height: 100%">
											点击此处展开详细的错误信息...</a>
										<asp:Label ID="detailErrorMessage" runat="server" Style="display: none; word-break: break-all"></asp:Label>
									</td>
									<td style="width: 20px">
									</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td style="background-image: url(../images/bottombg.png); height: 6px; width: 300px;">
						</td>
					</tr>
				</table>
			</div>
		</td>
	</tr>
</table>--%>
