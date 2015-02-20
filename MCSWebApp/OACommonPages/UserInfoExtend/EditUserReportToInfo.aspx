<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditUserReportToInfo.aspx.cs"
	Inherits="MCS.OA.CommonPages.UserInfoExtend.EditUserReportToInfo" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="MCS" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>汇报人编辑</title>
	<base target="_self">
	<script type="text/javascript">
		function onClick() {
			$get("btnConfirm").click();
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server" target="innerFrame">
	<table width="100%" style="height: 100%; width: 100%">
		<tr>
			<td class="gridHead">
				<div class="dialogTitle">
					<span class="dialogLogo">请选择汇报人</span>
				</div>
			</td>
		</tr>
		<tr>
			<td style="vertical-align: middle; text-align: center;">
				<div class="dialogContent" id="dialogContent" runat="server" style="width: 80%; vertical-align: middle
					height: 100%; overflow: auto; text-align: center;">
					<!--Put your dialog content here... -->
					<MCS:OuUserInputControl MultiSelect="False" ID="OuUserInputControl" runat="server"
						ShowDeletedObjects="true" InvokeWithoutViewState="true" MergeSelectResult="False"
						SelectMask="User" />
				</div>
			</td>
		</tr>
		<tr>
			<td class="gridfileBottom">
			</td>
		</tr>
		<tr>
			<td style="height: 40px; text-align: center; vertical-align: middle">
				<table style="width: 100%; height: 100%">
					<tr>
						<td style="text-align: center;">
							<input type="button" onclick="onClick();" class="formButton" value="确定(O)" id="btnOK"
								accesskey="O" />
							<MCS:SubmitButton runat="server" ID="btnConfirm" Style="display: none" OnClick="btnConfirm_Click"
								RelativeControlID="btnOK" PopupCaption="正在保存..." />
						</td>
						<td style="text-align: center;">
							<input type="button" onclick="top.close();" class="formButton" value="取消(C)" id="Button1"
								accesskey="C" />
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	</form>
	<iframe id="innerFrame" name="innerFrame" style="display: none"></iframe>
</body>
</html>
