<%@ Page Language="c#" Codebehind="UserLogDetail.aspx.cs" AutoEventWireup="True"
	Inherits="MCS.Applications.AppAdmin_LOG.logList.UserLogDetail" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
	<title>用户日志详细查看</title>
	<meta http-equiv="Content-Type" content="text/html; charset=gb2312">
	<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">

	<script type="text/javascript" language="javascript" src="../script/uiScript.js"></script>

	<link href="../css/Input.css" type="text/css" rel="stylesheet">

	<script type="text/javascript" language="javascript">
			function showWindow(valueIn)
			{
				try
				{
					strLink = "ShowContent.htm";
					var arg = new Object();
					arg.values = valueIn;
					var sFeature = "dialogWidth:800px; dialogHeight:400px;center:yes;help:no;resizable:no;scroll:no;status:no";
				
					var returnValue = showModalDialog(strLink, arg, sFeature);
				}
				catch(e)
				{
					showError(e);
				}
			}
		
			function onButAppDataClick()
			{
				try
				{
					var valueIn = ORIGINAL_DATA.innerText;
					showWindow(valueIn);
				}
				catch(e)
				{
					showError(e);
				}
			}
			
			function onButExpDataClick()
			{
				try
				{
					var valueIn = GOAL_EXPLAIN.innerText;
					showWindow(valueIn);
				}
				catch(e)
				{
					showError(e);
				}
			}
			
	</script>

</head>
<body class="modal">
	<table id="outerTable" height="100%" width="98%" align="center">
		<tr>
			<td style="height: 32px">
				<strong><font size="4"></font></strong><span id="logo" style="background-position: center center;
					background-image: url(../images/ResourceFolder.gif);
					width: 32px; background-repeat: no-repeat; height: 32px"></span><font size="4"><strong
						id="Caption">用户日志详细信息</strong> </font>
				<hr>
			</td>
		</tr>
		<tr>
			<td class="modalEditable" valign="top">
				<table height="100%" cellspacing="0" cellpadding="0" width="90%" align="center">
					<tr>
						<td valign="top" colspan="4">
							<table style="width: 100%" cellspacing="1" cellpadding="3">
								<tr>
									<td align="right" width="15%">
										<strong>用户名称:</strong>
									</td>
									<td width="35%" align="left">
										<label id="OP_USER_DISPLAYNAME" runat="server" name="OP_USER_DISPLAYNAME">
										</label>
									</td>
									<td align="right" width="15%">
										<strong>日志时间:</strong>
									</td>
									<td align="left" width="35%">
										<label id="LOG_DATE" runat="server" name="LOG_DATE">
										</label>
									</td>
								</tr>
								<tr>
									<td align="right" width="15%">
										<strong>登录名称:</strong>
									</td>
									<td width="35%">
										<label id="OP_USER_LOGONNAME" style="width: 100%" runat="server" name="OP_USER_LOGONNAME">
										</label>
									</td>
									<td align="right" width="15%">
										<strong>客户机IP:</strong>
									</td>
									<td>
										<label id="HOST_IP" width="35%" runat="server" name="HOST_IP">
										</label>
									</td>
								</tr>
								<tr>
									<td align="right" width="15%">
										<strong>用户信息:</strong>
									</td>
									<td width="35%" colspan="3">
										<label id="OP_USER_DISTINCTNAME" style="width: 100%" runat="server" name="OP_USER_DISTINCTNAME">
										</label>
									</td>
								</tr>
								<tr>
									<td align="right" width="15%">
										<strong>应用名称:</strong>
									</td>
									<td>
										<label id="APP_DISPLAYNAME" width="35%" runat="server" name="APP_DISPLAYNAME">
										</label>
									</td>
									<td align="right" width="15%">
										<strong>操作类型:</strong>
									</td>
									<td align="left" width="35%">
										<label id="OP_DISPLAYNAME" style="width: 100%" runat="server" name="OP_DISPLAYNAME">
										</label>
									</td>
								</tr>
								<tr>
									<td align="right" width="15%">
										<strong>是否成功:</strong>
									</td>
									<td>
										<label id="LOG_SUCCED" width="35%" runat="server" name="LOG_SUCCED">
										</label>
									</td>
									<td align="right" width="15%">
										<strong>操作说明:</strong>
									</td>
									<td width="35%">
										<button id="butExpData" onclick="onButExpDataClick()" style="width: 20%; height: 75%"
											type="button">
											详细</button></td>
								</tr>
								<tr style="display: none">
									<td>
										<label id="ORIGINAL_DATA" runat="server" name="ORIGINAL_DATA">
										</label>
									</td>
									<td>
										<label id="GOAL_EXPLAIN" runat="server" name="GOAL_EXPLAIN">
										</label>
									</td>
								</tr>
								<tr valign="top">
									<td width="15%" align="right">
										<strong>页面 URL:</strong>
									</td>
									<td colspan="3" align="left" width="35%">
										<label id="OP_URL" runat="server" name="OP_URL">
										</label>
									</td>
								</tr>
								<tr valign="top">
									<td valign="top" align="right" width="15%">
										<strong>数据描述:</strong>
									</td>
									<td align="left" width="35%">
										<button id="butAppData" onclick="onButAppDataClick()" type="button">
											详细查看</button>
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td align="center" valign="top">
				<hr size="1">
				<input id="back" accesskey="C" onclick="window.close();" type="button" value="关闭(C)">
			</td>
		</tr>
		<tr>
		</tr>
	</table>
</body>
</html>
