<%@ Control Language="C#" AutoEventWireup="true" Codebehind="PassportLogonUC.ascx.cs"
	Inherits="MCS.Applications.AccreditAdmin.Template.PassportLogonUC" %>
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
-->
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

<input runat="server" id="clientEnv" type="hidden" />
<table style="background-image: url(./images/bg.gif); width: 100%; height: 100%"
	border="0" cellspacing="0" cellpadding="0">
	<tr>
		<td align="center" valign="middle" background="./Logonimages/bg.gif">
			<table width="639" border="0" cellspacing="0" cellpadding="0">
				<tr>
					<td height="137" align="left" valign="top">
						<img src='./Logonimages/denglu02_03.jpg' alt="" width="122" height="137" /></td>
					<td width="417" align="left" valign="top">
						<img src='./Logonimages/denglu02_04.jpg' alt="" width="417" height="137" /></td>
					<td width="100" align="left" valign="top">
						<img src='./Logonimages/denglu02_05.gif' alt="" width="100" height="137" /></td>
				</tr>
				<tr>
					<td align="left" valign="top" style="height: 200px">
						<img src="./Logonimages/denglu02_07.jpg" alt="" width="122" height="200" /></td>
					<td align="left" valign="top" bgcolor="#a75003" style="height: 200px">
						<table width="417" height="200" border="0" cellpadding="0" cellspacing="1">
							<tr>
								<td align="center" valign="middle" background="./Logonimages/denglu02_13.gif">
									<table width="96%" border="0" cellspacing="0" cellpadding="0">
										<tr>
											<td width="30%" height="35" align="right" class="color01">
												用户名：</td>
											<td width="39%" align="left">
												<asp:TextBox ID="signInName" MaxLength="32" Width="100%" CssClass="text_input" runat="server"></asp:TextBox>
											</td>
											<td width="31%" rowspan="2" align="center">
												<span style="display: none">
													<asp:Button ID="SignInButton" runat="server" Width="64px" Text="登录"></asp:Button>
												</span>
												<img src="./Logonimages/denglu.gif" width="69" height="59" id="SignInImge" alt="登录"
													onclick="onLogonClick();" />
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
											<td>
												&nbsp;</td>
											<td height="20" colspan="2" align="left">
												<asp:CheckBox ID="autoSignIn" runat="server" Text="自动登录"></asp:CheckBox>
											</td>
										</tr>
										<tr>
											<td>
												&nbsp;</td>
											<td height="20" colspan="3" align="left">
												<asp:CheckBox ID="dontSaveUserName" runat="server" Text="请不要为了便于将来登录而记录我的登录名"></asp:CheckBox>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
					<td align="left" valign="top" style="height: 200px">
						<img src="./Logonimages/denglu02_09.gif" alt="" width="100" height="200" /></td>
				</tr>
				<tr>
					<td height="58" align="center" valign="top">
						<img src="./Logonimages/denglu02_10.gif" alt="" width="122" height="58" />
					</td>
					<td align="left" valign="top">
						<img src="./Logonimages/denglu02_11.gif" alt="" width="417" height="58" /></td>
					<td align="left" valign="top">
						<img src="./Logonimages/denglu02_12.gif" alt="" width="100" height="58" /></td>
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
