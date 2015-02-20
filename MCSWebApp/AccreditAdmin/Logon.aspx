<%@ Page Language="c#" Codebehind="LogOn.aspx.cs" AutoEventWireup="True" Inherits="MCS.Applications.AccreditAdmin.LogOn" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
	<title>系统登录</title>
	<link rel="Shortcut Icon" href="./images/icon/key.ico">
	<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
	<meta name="CODE_LANGUAGE" content="C#">
	<meta name="vs_defaultClientScript" content="JavaScript">
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	<link href="./css/Input.css" type="text/css" rel="stylesheet">

	<script type="text/javascript" language="javascript" src="selfScript/individuality.js"></script>

	<!--script type="text/javascript" language="javascript" src="selfScript/security.js"></script-->

	<script type="text/javascript" language="javascript" src="oguScript/validate.js"></script>

	<script type="text/javascript" language="javascript">
		function onDocumentLoad()
		{
			LogOn.userName.focus();
			
			startclock();
		}
/*		
		function onButtonSubmit()
		{
			try
			{
				var strUserPwd = LogOn.userPassword.value;
				trueThrow(strUserPwd.length == 0, "对不起，口令不能为空！");
				LogOn.userPassword.value = MD5(strUserPwd);
				return true;
			}
			catch (e)
			{
				showError(e);
				return false;
			}
		}*/
	</script>

</head>
<body ms_positioning="GridLayout" onload="onDocumentLoad();">
	<form id="LogOn" method="post" runat="server">
		<table style="width: 100%; height: 100%">
			<tr>
				<td align="center" style="filter: progid:DXImageTransform.Microsoft.Shadow(color='silver', Direction=135, Strength=2);
					height: 128px">
					<strong style="font-size: 36pt; color: maroon; font-family: simsun">通用授权平台</strong>
				</td>
			</tr>
			<tr>
				<td align="center" valign="top">
					<table style="border-right: gray 1px solid; border-top: gray 1px solid; border-left: gray 1px solid;
						width: 480px; border-bottom: gray 1px solid; height: 260px" cellspacing="0">
						<tr>
							<td align="center">
								<img src="images/32/keys.gif" height="64" width="64">
							</td>
							<td>
								<table style="width: 100%; height: 100%" cellpadding="0" cellspacing="0">
									<tr>
										<td style="height: 50px" align="center">
											<strong style="font-size: 12pt; margin-left: 4px; color: gray">请输入登录信息</strong>
										</td>
									</tr>
									<tr style="height: 0%">
										<td align="center">
											<asp:Label ID="errorMsg" runat="server" Width="100%" ForeColor="Red"></asp:Label>
										</td>
									</tr>
									<tr>
										<td align="center">
											<hr size="1" width="80%" align="center" color="maroon">
										</td>
									</tr>
									<tr>
										<td>
											<table style="width: 100%; height: 100%">
												<tr>
													<td style="width: 80px" align="right">
														<strong>登录名称</strong>:
													</td>
													<td>
														<input type="text" id="userName" style="width: 90%" runat="server" name="userName">
													</td>
												</tr>
												<tr>
													<td align="right">
														<strong>登录口令</strong>:
													</td>
													<td>
														<input type="password" id="userPassword" style="width: 90%" runat="server" name="userPassword">
													</td>
												</tr>
												<tr style="display: none">
													<td align="right">
														<strong>加密算法</strong>:
													</td>
													<td>
														<asp:DropDownList ID="userPwdType" runat="server" name="userPwdType">
														</asp:DropDownList></td>
												</tr>
											</table>
										</td>
									</tr>
									<tr>
										<td style="height: 36px">
											<hr size="1" width="80%">
											<table style="width: 100%">
												<tr>
													<td align="center">
														<asp:Button ID="btnLogOn" name="btnLogOn" runat="server" Text="登录" Width="64px" OnClick="btnLogOn_Click">
														</asp:Button>
													</td>
													<!--td align="middle">
															<input type="button" style="WIDTH:64px" value="注册">
														</td-->
												</tr>
											</table>
										</td>
									</tr>
									<tr>
										<td>
											<a target="blank" href="http://wpa.qq.com/msgrd?V=1&Uin=66465431&Site=forever.china-pub.com&Menu=yes">
												<img border="0" src="http://wpa.qq.com/pa?p=1:66465431:13" alt="有事Q我"></a></td>
									</tr>
								</table>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</form>
</body>
</html>
