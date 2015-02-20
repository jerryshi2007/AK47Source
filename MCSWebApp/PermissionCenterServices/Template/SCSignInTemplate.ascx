<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SCSignInTemplate.ascx.cs"
	Inherits="PermissionCenter.Template.SCSignInTemplate" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<%--<style type="text/css">
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
</style>--%>
<div class="login" style="text-align: left; width: 800px; margin: auto; padding: 0;
	font-family: Arial, Helvetica, sans-serif; font-size: 12px; line-height: 22px;
	color: #666; text-decoration: none;">
	<div>
		<img src="../Images/Login/login_03.gif" width="298" height="48" />
	</div>
	<div style="width: 900px; height: 350px; overflow: hidden;">
		<div style="float: left; width: 246px; height: 333px; background: transparent url('../Images/Login/login_05.jpg') no-repeat scroll">
		</div>
		<div style="float: left; width: 247px; height: 333px; background: transparent url('../Images/Login/login_06.jpg') no-repeat scroll">
		</div>
		<div style="float: left; width: 380px; height: 333px; padding: 80px 5px; background: transparent url('../Images/Login/login_07.gif') no-repeat scroll">
			<ul style="width: 309px; list-style: none;" runat="server">
				<li>
					<div>
						<span style="display: inline-block; width: 80px; text-align: right">用户名</span>
						<asp:TextBox ID="signInName" Width="200px" Style="margin-left: 4px" runat="server"></asp:TextBox></div>
				</li>
				<li>
					<div>
						<span style="display: inline-block; width: 80px; text-align: right">密码</span>
						<input id="password" type="password" style="margin-left: 4px; width: 200px" runat="server"
							name="password" /></div>
				</li>
				<li id="simulateDateRow" runat="server">
					<div>
						<span style="display: inline-block; width: 80px; text-align: right">模拟日期</span>
						<MCS:DeluxeDateTime Style="margin-left: 4px" runat="server" ID="simulateDate"></MCS:DeluxeDateTime>
					</div>
				</li>
				<li>
					<div>
						<span style="display: inline-block; width: 80px; text-align: right"></span>
						<asp:CheckBox ID="autoSignIn" runat="server" name="autoSignIn" Text="下次自动登录" />&nbsp;&nbsp;&nbsp;&nbsp;
						<asp:CheckBox ID="dontSaveUserName" runat="server" Text="请不要记录我的登录名"></asp:CheckBox>
					</div>
				</li>
			</ul>
			<div>
				<div style="margin-top: 50px; text-align: center; margin-left: auto; margin-right: auto;">
					<div>
						<asp:ImageButton ID="signInImageBtn" runat="server" OnClientClick="document.getElementById(document.getElementById('buttonName').value).click();return false;"
							ImageUrl="~/Images/Login/login_btn.gif" />
					</div>
					<div>
						<img runat="server" id="shadowImage" src="~/Images/Login/login_btn_shadow.gif" />
					</div>
				</div>
				<asp:Button ID="SignInButton" runat="server" Width="64px" Text="登录" Style="display: none;">
				</asp:Button>
			</div>
		</div>
	</div>
	<%--<table width="100%" border="0" cellspacing="0" cellpadding="0">
		<tr>
			<td width="246" height="333" background="../Images/Login/login_05.jpg">
				&nbsp;
			</td>
			<td width="247" background="../Images/Login/login_06.jpg">
				&nbsp;
			</td>
			<td width="388" valign="top" background="../Images/Login/login_07.gif" class="box">
				<table width="309" border="0" align="center" cellpadding="0" cellspacing="2">
					<tr>
						<td width="47" height="30">
							用户名
						</td>
						<td>
							
						</td>
					</tr>
					<tr>
						<td height="30">
							密&nbsp;&nbsp;&nbsp;&nbsp;码
						</td>
						<td>
							
						</td>
					</tr>
					<tr id="simulateDateRow" runat="server">
						<td>
							模拟日期
						</td>
						<td>
							
						</td>
					</tr>
					<tr>
						<td>
							&nbsp;
						</td>
						<td>
							<asp:CheckBox ID="autoSignIn" runat="server" name="autoSignIn" Text="下次自动登录" />&nbsp;&nbsp;&nbsp;&nbsp;
							<asp:CheckBox ID="dontSaveUserName" runat="server" Text="请不要记录我的登录名"></asp:CheckBox>
							<a href="#">忘记密码?</a>
	</td> </tr>
	<tr>
		<td colspan="2">
			&nbsp;&nbsp;&nbsp;
		</td>
	</tr>
	<tr>
		<td height="105" colspan="2" style="text-align: center; vertical-align: bottom">
			<div>
				<div>
					<asp:ImageButton ID="signInImageBtn" runat="server" OnClientClick="document.getElementById(document.getElementById('buttonName').value).click();return false;"
						ImageUrl="~/Images/Login/login_btn.gif" />
				</div>
				<div>
					<img runat="server" id="shadowImage" src="~/Images/Login/login_btn_shadow.gif" />
				</div>
			</div>
			<asp:Button ID="SignInButton" runat="server" Width="64px" Text="登录" Style="display: none;">
			</asp:Button>
		</td>
	</tr>
	</table> </td> </tr> </table>--%>
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
