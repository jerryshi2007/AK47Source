<%@ Control Language="C#" AutoEventWireup="true" Codebehind="PassportLogonUC.ascx.cs"
	Inherits="MCS.Applications.AppAdmin.Template.PassportLogonUC" %>
<style type="text/css">
<!--
body {
	margin-left: 0px;
	margin-top: 0px;
	margin-right: 0px;
	margin-bottom: 0px;
}
.color01 {
	font-size: 14px;
	font-weight: bold;
	color: #0660e7;
}
.text_input {
	font-size: 14px;
	color: #000000;
	background-color: #f0f0f0;
	border: 1px solid #0660e7;
}
td {
	font-size: 12px;
	color: #393939;
}
//-->
</style>

<script type="text/javascript" language="javascript">
<!--
function onLogonClick()
{
	var button = event.srcElement.previousSibling.children[0];
	button.click();
}

//-->
</script>

<table style="width: 100%; height: 100%" border="0" cellspacing="0" cellpadding="0">
	<tr>
		<td align="center" valign="middle" background="./Logonimages/bg.gif">
			<table width="641" border="0" cellspacing="0" cellpadding="0">
				<tr>
					<td height="120" align="left" valign="top">
						<img src="./Logonimages/denglu01_03.gif" width="641" height="120" /></td>
				</tr>
				<tr>
					<td height="139" align="left" valign="top">
						<table width="641" height="139" border="0" cellpadding="0" cellspacing="0">
							<tr>
								<td width="250" align="left" valign="top">
									<img src="./Logonimages/denglu01_05.jpg" width="250" height="139" /></td>
								<td width="383" align="center" bgcolor="#FFFFFF">
									<table width="96%" border="0" cellspacing="0" cellpadding="0">
										<tr>
											<td width="24%" height="35" align="right" class="color01">
												用户名：</td>
											<td width="42%" align="left">
												<asp:TextBox ID="signInName" MaxLength="32" Width="100%" CssClass="text_input" runat="server"></asp:TextBox>
											</td>
											<td width="34%" rowspan="2" align="left">
												<span style="display: none">
													<asp:Button ID="SignInButton" runat="server" Width="64px" Text="登录"></asp:Button>
												</span>
												<img src="./Logonimages/denglu.gif" width="69" height="59" id="SignInImge" alt="登录"
													onclick="onLogonClick();" />
												<!--<img src="./Logonimages/denglu.gif" width="69" height="59" />-->
											</td>
										</tr>
										<tr>
											<td height="35" align="right" class="color01">
												密&nbsp;&nbsp;码：</td>
											<td align="left">
												<input id="password" type="password" width="100%" maxlength="32" style="margin-left: 4px"
													runat="server" name="password" />
											</td>
										</tr>
										<tr>
											<td height="40" colspan="3">
												<asp:CheckBox ID="autoSignIn" runat="server" Text="自动登录"></asp:CheckBox>
												&nbsp;&nbsp;
												<asp:CheckBox ID="dontSaveUserName" runat="server" Text="请不要为了便于将来登录而记录我的登录名"></asp:CheckBox>
											</td>
										</tr>
									</table>
								</td>
								<td width="8" align="right" valign="top">
									<img src="./Logonimages/denglu01_07.gif" width="8" height="139" /></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td height="85" align="left" valign="top">
						<img src="./Logonimages/denglu01_08.gif" width="641" height="85" /></td>
				</tr>
				<tr>
					<td colspan="9">
						<table style="width: 100%" cellpadding="0" cellspacing="0">
							<tr>
								<td style="width: 16px">
								</td>
								<td align="center">
									<asp:Label ID="errorMessage" runat="server" ForeColor="red" Font-Bold="True" Style="line-height: 150%;
										word-break: break-all"></asp:Label>
									<a href="#" id="detailErrorMessageLink" runat="server" style="display: none; line-height: 150%">
										点击此处展开详细的错误信息...</a>
									<asp:Label ID="detailErrorMessage" runat="server" Style="display: none; word-break: break-all"></asp:Label>
								</td>
								<td style="width: 20px">
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
