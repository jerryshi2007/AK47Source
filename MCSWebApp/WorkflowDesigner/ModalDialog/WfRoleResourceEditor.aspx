<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WfRoleResourceEditor.aspx.cs"
	Inherits="WorkflowDesigner.ModalDialog.WfRoleResourceEditor" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>角色资源编辑</title>
	<base target="_self" />
	<style type="text/css">
		#roleUpdateProgress
		{
			width: 100px;
			bottom: 0%;
			left: 190px;
			position: absolute;
			top: 180px;
		}
	</style>
	<script type="text/javascript">
		var roleResDesc;
		var apps;
		function onDocumentLoad() {
		}

		function onClick() {
			$get("btnConfirm").click();
		}
	</script>
</head>
<body>
	<form id="form1" runat="server">
	<asp:ScriptManager ID="ScriptManager1" runat="server">
	</asp:ScriptManager>
	<table width="100%" style="height: 100%; width: 100%">
		<tr>
			<td class="gridHead">
				<div class="dialogTitle">
					<span class="dialogLogo">角色资源编辑</span>
				</div>
			</td>
		</tr>
		<tr>
			<td style="vertical-align: middle">
				<div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
					height: 100%; overflow: auto">
					<table width="100%" style="height: 100%; width: 100%;">
						<tr style="height: 10px;">
							<td style="width: 50px;">
							</td>
							<td>
							</td>
						</tr>
						<tr>
							<td class="label" style="vertical-align: middle; height: 64px">
								应用列表:
							</td>
							<td style="vertical-align: middle; text-align: left">
								<asp:DropDownList ID="ddlApps" runat="server" Width="200" OnSelectedIndexChanged="ddlApps_SelectedIndexChanged"
									AutoPostBack="True">
								</asp:DropDownList>
							</td>
						</tr>
						<tr>
							<td>
							</td>
							<td style="height: 24px">
								<asp:UpdatePanel ID="relativeLinkPanel" ChildrenAsTriggers="false" UpdateMode="Conditional"
									runat="server">
									<Triggers>
										<asp:AsyncPostBackTrigger ControlID="ddlApps" />
									</Triggers>
									<ContentTemplate>
										<a id="relativeLink" runat="server" href="#" target="_blank">
											相关链接...</a>
									</ContentTemplate>
								</asp:UpdatePanel>
							</td>
						</tr>
						<tr>
							<td class="label" style="vertical-align: middle">
								角色列表:
							</td>
							<td style="vertical-align: middle; text-align: left">
								<asp:UpdatePanel ID="roleUpdatePanel" ChildrenAsTriggers="false" UpdateMode="Conditional"
									runat="server">
									<Triggers>
										<asp:AsyncPostBackTrigger ControlID="ddlApps" />
									</Triggers>
									<ContentTemplate>
										<asp:DropDownList ID="ddlRoles" runat="server" Width="250">
										</asp:DropDownList>
									</ContentTemplate>
								</asp:UpdatePanel>
							</td>
						</tr>
					</table>
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
							<input type="button" onclick="onClick();" value="确定(O)" id="btnOK" accesskey="O"
								runat="server" class="formButton" />
							<SOA:SubmitButton runat="server" ID="btnConfirm" Style="display: none" OnClick="btnConfirm_Click"
								RelativeControlID="btnOK" />
						</td>
						<td style="text-align: center;">
							<input id="btnCancel" type="button" value="取消(C)" accesskey="C" onclick="top.close();"
								class="formButton" />
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	<input type="hidden" id="hiddenApps" runat="server" />
	<input type="hidden" id="resultData" runat="server" />
	</form>
	<script type="text/javascript">
		Sys.Application.add_load(function () {
			onDocumentLoad();
		});
		//]]>
	</script>
</body>
</html>
