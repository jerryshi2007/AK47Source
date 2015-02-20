<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkflowSimulation.aspx.cs"
	Inherits="WorkflowDesigner.Simulation.WorkflowSimulation" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>流程仿真</title>
	<style type="text/css">
		.paramContainer
		{
			padding-top: 8px;
			padding-bottom: 8px;
		}
	</style>
	<script type="text/javascript" src="WorkflowSimulation.js"></script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<asp:ScriptManager runat="server" EnableScriptGlobalization="true">
			<Services>
				<asp:ServiceReference Path="~/Simulation/WorkflowSimulationService.asmx" />
			</Services>
		</asp:ScriptManager>
	</div>
	<div>
		<input type="hidden" runat="server" id="processDescKeyHidden" />
	</div>
	<div>
		<table width="100%" style="height: 100%; width: 100%">
			<tr>
				<td class="gridHead">
					<div class="dialogTitle">
						<span class="dialogLogo">流程仿真</span>
					</div>
				</td>
			</tr>
			<tr>
				<td style="height: 24px; vertical-align: middle">
					<div runat="server" id="processDescCaption" style="height: 100%; padding-left: 8px;
						padding-top: 4px; font-weight: bold">
					</div>
				</td>
			</tr>
			<tr>
				<td style="vertical-align: middle">
					<table style="width: 100%; height: 100%">
						<tr>
							<td>
								<div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
									height: 100%; overflow: auto; border: 1px solid silver">
								</div>
							</td>
							<td style="width: 256px">
								<table style="width: 100%; height: 100%">
									<tr>
										<td style="padding-left: 8px; padding-top: 8px; vertical-align: top">
											<div class="paramContainer">
												<a onclick="onEditProcessParamsClick();" href="../ModalDialog/WfVariables.aspx">编辑流程上下文参数...</a>
											</div>
											<div class="paramContainer">
												<div>
													流程发起人
												</div>
												<div>
													<SOA:OuUserInputControl MultiSelect="false" ID="processCreator" runat="server" ListMask="Organization,User,Sideline"
														Width="180" ShowDeletedObjects="false" InvokeWithoutViewState="true" MergeSelectResult="false"
														SelectMask="User" />
												</div>
											</div>
											<div class="paramContainer">
												<label>
													<input type="checkbox" id="enableServiceCallBtn" />启用服务调用</label>
											</div>
											<div class="paramContainer">
												执行间隔：<select id="callbackInterval" name="callbackInterval">
													<option value="0.25">0.25</option>
													<option value="0.5" selected="selected">0.5</option>
													<option value="1">1</option>
													<option value="2">2</option>
													<option value="5">5</option>
													<option value="10">10</option>
													<option value="15">15</option>
													<option value="30">30</option>
												</select>秒
											</div>
											<div class="paramContainer">
												<a href="#" id="viewProcessLink" target="simulation_viewProcess" onclick="onViewProcessStatusClick();">
													查看流程状态...</a>
											</div>
										</td>
									</tr>
									<tr>
										<td style="height: 32px">
										</td>
									</tr>
									<tr>
										<td style="height: 32px">
											<input type="button" class="formButton" onclick="callStart();" value="开始(S)" id="startBtn"
												accesskey="S" style="width: 72px" />
											<input type="button" class="formButton" onclick="onPauseClick();" value="暂停(P)" id="pauseBtn"
												accesskey="P" style="width: 72px; display: none" />
											<input type="button" class="formButton" onclick="onResumeClick();" value="继续(R)"
												id="resumeBtn" accesskey="R" style="width: 72px; display: none" />
											<input type="button" class="formButton" onclick="onFinishClick();" value="结束(F)"
												id="finishBtn" accesskey="F" style="width: 72px; visibility: hidden" />
										</td>
									</tr>
								</table>
							</td>
						</tr>
					</table>
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
								<input type="button" class="formButton" onclick="top.close();" value="关闭(C)" id="btnClose"
									accesskey="C" />
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</div>
	<script type="text/javascript">
		Sys.Application.add_init(function () { initButtons(); });
	</script>
	</form>
</body>
</html>
