<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminAutoMoveToDialog.aspx.cs"
	Inherits="WorkflowDesigner.ExternalDialogs.AdminAutoMoveToDialog" %>

<%@ Register Src="OperationEditor.ascx" TagName="OperationEditor" TagPrefix="uc" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="soa" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>自动流转</title>
</head>
<body>
	<form id="serverForm" runat="server" target="innerFrame">
	<div>
		<asp:ScriptManager ID="scriptManager" runat="server" EnableScriptGlobalization="true">
		</asp:ScriptManager>
		<div>
			<uc:OperationEditor ID="operationEditor" runat="server" SaveButtonID="okBtn" />
		</div>
	</div>
	<div>
		<table width="100%" style="height: 100%; width: 100%">
			<tr>
				<td class="gridHead">
					<div class="dialogTitle">
						<span class="dialogLogo">自动流转</span>
					</div>
				</td>
			</tr>
			<tr>
				<td style="vertical-align: middle; text-align: center;">
					<div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
						vertical-align: middle; text-align: center;">
						<div style="width: 100%; height: 100%; padding-left: 24px; padding-right: 24px">
							<div style="text-align: left; font-weight: bold">
								请选择下一步收到待办的人员</div>
							<soa:OuUserInputControl runat="server" ID="userSelector" SelectMask="User,Sideline"
								ListMask="Organization,User,Sideline" />
						</div>
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
								<soa:SubmitButton runat="server" ID="okBtn" PopupCaption="正在流转..." Text="确定(O)" AccessKey="O"
									CssClass="formButton" />
							</td>
							<td style="text-align: center;">
								<input type="button" onclick="top.close();" class="formButton" value="取消(C)" id="cancelBtn"
									accesskey="C" />
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</div>
	</form>
	<div style="display: none">
		<input id="resetAllStateBtn" type="button" onclick="SubmitButton.resetAllStates();" />
		<iframe name="innerFrame" />
	</div>
</body>
</html>
