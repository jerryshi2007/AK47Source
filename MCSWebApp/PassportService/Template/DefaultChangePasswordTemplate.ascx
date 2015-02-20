<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DefaultChangePasswordTemplate.ascx.cs"
	Inherits="MCS.Web.Passport.Template.DefaultChangePasswordTemplate" %>
<style type="text/css">
	BODY
	{
		font-size: 9pt;
		color: black;
		font-family: SimSun;
	}
	TABLE
	{
		font-size: 9pt;
	}
	.btn
	{
		border-right: #7b9ebd 1px solid;
		padding-right: 2px;
		border-top: #7b9ebd 1px solid;
		padding-left: 2px;
		font-size: 12px;
		filter: progid:DXImageTransform.Microsoft.Gradient(GradientType=0, StartColorStr=#ffffff, EndColorStr=#cecfde);
		border-left: #7b9ebd 1px solid;
		cursor: hand;
		color: black;
		padding-top: 2px;
		border-bottom: #7b9ebd 1px solid;
	}
</style>
<input runat="server" id="clientEnv" type="hidden" />
<table style="background-image: url(../images/bg.gif); width: 100%; height: 100%"
	cellpadding="0" cellspacing="0">
	<tr>
		<td align="center" valign="middle">
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
										<asp:TextBox ID="signInName" Width="200px" Style="margin-left: 4px" runat="server"></asp:TextBox>
									</td>
									<td style="width: 32px">
									</td>
								</tr>
								<tr>
									<td align="right" style="height: 48px">
										<strong>旧密码:</strong>
									</td>
									<td>
										<input id="oldPassword" type="password" style="margin-left: 4px; width: 200px" runat="server"
											name="oldPassword" />
									</td>
									<td>
									</td>
								</tr>
								<tr>
									<td align="right" style="height: 48px">
										<strong>新密码:</strong>
									</td>
									<td>
										<input id="newPassword" type="password" style="margin-left: 4px; width: 200px" runat="server"
											name="newPassword" />
									</td>
									<td>
									</td>
								</tr>
								<tr>
									<td align="right" style="height: 48px">
										<strong>确认密码:</strong>
									</td>
									<td>
										<input id="confirmPassword" type="password" style="margin-left: 4px; width: 200px"
											runat="server" name="confirmPassword" />
									</td>
									<td>
									</td>
								</tr>
								<tr>
									<td>
									</td>
									<td align="right" style="height: 26px">
										<asp:Button ID="changePasswordButton" runat="server" Width="64px" Text="修改" CssClass="btn">
										</asp:Button>
										<input type="button" value="返回" id="backButton" class="btn" runat="server" style="width:64px" />
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
</table>
