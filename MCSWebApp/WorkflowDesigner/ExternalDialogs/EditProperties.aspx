<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditProperties.aspx.cs"
	Inherits="WorkflowDesigner.ExternalDialogs.EditProperties" %>

<%@ Register Src="DescriptorEditor.ascx" TagName="DescriptorEditor" TagPrefix="uc" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="soa" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>
		<asp:Literal runat="server" ID="documentTitle" Mode="Encode" /></title>
</head>
<body>
	<form id="serverForm" runat="server" target="innerFrame">
	<div>
		<input type="hidden" id="postProcessData" name="postProcessData" />
	</div>
	<div>
		<uc:DescriptorEditor ID="descriptorEditor" runat="server" PropertyGridID="propertyGrid"
			SaveButtonID="okBtn" />
	</div>
	<div>
		<table width="100%" style="height: 100%; width: 100%">
			<tr>
				<td class="gridHead">
					<div class="dialogTitle">
						<span class="dialogLogo">
							<asp:Literal runat="server" ID="diagLogoText" Mode="Encode" /></span>
					</div>
				</td>
			</tr>
			<tr>
				<td style="vertical-align: top; text-align: center;">
					<div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
						vertical-align: top; height: 100%; overflow: auto; text-align: center;">
						<soa:PropertyGrid runat="server" ID="propertyGrid" Width="100%" Height="100%" DisplayOrder="ByCategory" />
					</div>
				</td>
			</tr>
			<tr id="syncMSObjRow" runat="server">
				<td style="height: 32px; text-align: center; vertical-align: middle">
					<label>
						<input id="syncMSObj" type="checkbox" runat="server" value="syncMSObj" />是否同步设置主线对象</label>
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
								<soa:SubmitButton runat="server" ID="okBtn" PopupCaption="正在保存..." Text="确定(O)" AccessKey="O"
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
