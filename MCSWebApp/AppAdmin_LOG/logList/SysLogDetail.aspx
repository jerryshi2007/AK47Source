<%@ Page Language="c#" Codebehind="SysLogDetail.aspx.cs" AutoEventWireup="True" Inherits="MCS.Applications.AppAdmin_LOG.logList.SysLogDetail" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
	<title>系统日志详细查看</title>
	<meta http-equiv="Content-Type" content="text/html; charset=gb2312">
	<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<link href="../css/Input.css" type="text/css" rel="stylesheet">
</head>
<body class="modal">
	<table id="outerTable" height="100%" width="98%" align="center">
		<tr>
			<td style="height: 32px">
				<strong><font size="4"></font></strong><span id="logo" style="background-position: center center;
					 background-image: url(../images/ResourceFolder.gif);
					width: 32px; background-repeat: no-repeat; height: 32px"></span><font size="4"><strong
						id="Caption">系统日志详细信息</strong> </font>
				<hr>
			</td>
		</tr>
		<tr>
			<td class="modalEditable" valign="top">
				<table height="100%" cellspacing="0" cellpadding="0" width="90%" align="center">
					<tr>
						<td valign="top">
							<table style="width: 100%; height: 160px" bordercolor="#cc3333" cellspacing="0" cellpadding="0">
								<tr valign="bottom">
									<td align="right" width="80">
										<strong>日志时间:</strong>
									</td>
									<td>
										<label id="LOG_DATE" style="width: 100%" runat="server" name="LOG_DATE">
										</label>
									</td>
									<td align="right" width="80">
										<strong>登录名称:</strong>
									</td>
									<td style="width: 180px">
										<label id="USER_LOGONNAME" style="width: 100%" runat="server" name="USER_LOGONNAME">
										</label>
									</td>
								</tr>
								<tr valign="bottom">
									<td align="right">
										<strong>客户机IP:</strong>
									</td>
									<td>
										<label id="HOST_IP" style="width: 100%" runat="server" name="HOST_IP">
										</label>
									</td>
									<td align="right" width="80">
										<strong>客户机名称:</strong>
									</td>
									<td style="width: 180px">
										<label id="HOST_NAME" style="width: 100%" runat="server" name="HOST_NAME">
										</label>
									</td>
								</tr>
								<tr valign="bottom">
									<td align="right">
										<strong>浏览器版本:</strong>
									</td>
									<td>
										<label id="IE_VERSION" style="width: 100%" runat="server" name="IE_VERSION">
										</label>
									</td>
									<td align="right">
										<strong>系统版本:</strong>
									</td>
									<td align="left">
										<label id="WINDOWS_VERSION" style="width: 100%" runat="server" name="WINDOWS_VERSION">
										</label>
									</td>
								</tr>
								<tr valign="bottom">
									<td align="right">
										<strong>杀毒软件:</strong>
									</td>
									<td>
										<label id="KILL_VIRUS" style="width: 100%" runat="server" name="KILL_VIRUS">
										</label>
									</td>
									<td align="right">
										<strong>环境信息:</strong>
									</td>
									<td>
										<label id="STATUS" style="width: 100%" runat="server" name="STATUS">
										</label>
									</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr valign="top">
						<td>
							<table bordercolor="#ff0000" cellspacing="0" cellpadding="0" width="100%">
								<tr>
									<td style="width: 80px" align="right">
										<strong>用户信息:</strong>
									</td>
									<td valign="bottom" align="middle">
										<label id="USER_DISTINCTNAME" style="width: 100%" runat="server" name="USER_DISTINCTNAME">
										</label>
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td align="middle">
				<hr size="1">
				<input id="back" accesskey="C" onclick="window.close();" type="button" value="关闭(C)">
			</td>
		</tr>
	</table>
</body>
</html>
