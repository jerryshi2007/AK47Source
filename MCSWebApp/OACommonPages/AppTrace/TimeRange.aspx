<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TimeRange.aspx.cs" Inherits="MCS.OA.CommonPages.AppTrace.TimeRange" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>选择时间范围</title>
	<link href="../../CSS/overrides.css" rel="stylesheet" type="text/css" />
	<link href="../../CSS/templatecss.css" rel="stylesheet" type="text/css" />
	<base target="_self" />
</head>
<body>
	<form id="serverForm" runat="server">
	<div class="t-container">
		<asp:MultiView ActiveViewIndex="0" runat="server" ID="views">
			<asp:View runat="server">
				<div class="t-dialog-caption">
					<span class="t-dialog-caption">时间范围选择</span>
				</div>
				<div style="padding: 5px; line-height: 32px;">
					<div class="label">
						时间范围</div>
					<mcs:DeluxeCalendar runat="server" ID="timeFrom" />
					~<mcs:DeluxeCalendar runat="server" ID="timeTo" />
				</div>
				<div class="inputCell">
					<input type="button" class="portalButton" id="btnContinue" value="继续" onclick="doThatAction();" />
				</div>
				<div style="display: none">
					<asp:Button Text="继续" runat="server" ID="btnServerContinue" OnClick="DoContinue" />
					<soa:SubmitButton runat="server" ID="progress" PopupCaption="查询中" RelativeControlID="btnServerContinue"
						UseSubmitBehavior="true" OnClick="DoContinue" />
				</div>
				<script type="text/javascript">
					function doThatAction() {
						var a = $find("timeFrom").get_selectedDate();
						var b = $find("timeTo").get_selectedDate();

						if (a == Date.minDate && b == Date.minDate) {
							alert("请填写起止时间");
						} else if (b != Date.minDate && a > b) {
							alert("开始时间不得大于结束时间");
						} else {
							$get("progress").click();
						}
					}
				</script>
			</asp:View>
			<asp:View runat="server">
				<div class="t-dialog-caption">
					<span class="t-dialog-caption">查找结果</span>
				</div>
				<div style="padding: 5px; line-height: 32px;">
					<div>
						共找到<span runat="server" id="lblCount"></span>个结果
					</div>
					<div>
						<asp:Button runat="server" Text="上一步" OnClick="Back" CssClass="portalButton" />
						<input type="button" value="重新计算" runat="server" class="portalButton" id="btnRecalc" />
					</div>
					<div style="display: none">
						<input type="hidden" runat="server" id="postVal" />
					</div>
				</div>
				<div>
					<soa:MultiProcessControl runat="server" ID="regenProcesses" DialogTitle="正在处理..."
						ShowStepErrors="true" ControlIDToShowDialog="btnRecalc" OnClientBeforeStart="onBeforeRegen"
						OnClientFinishedProcess="onFinished" OnExecuteStep="Regen_ExecuteStep" OnError="RegenProcesses_Error" />
					<script type="text/javascript">
						function onFinished(e) {
							//检查处理结果
							if (e.value) {
								alert("处理完成");
								window.close();
							} else {
								$showError(e.error);
							}
						}

						function onBeforeRegen(e) {
							var processIDs = Sys.Serialization.JavaScriptSerializer.deserialize($get("postVal").value);

							if (processIDs.length == 0) {
								e.cancel = true;
							} else {
								e.steps = [];
								for (var i = processIDs.length - 1; i >= 0; i--) {
									e.steps.push(processIDs[i]);
								}

								e.cancel = false;
							}
						}
					</script>
				</div>
			</asp:View>
		</asp:MultiView>
	</div>
	</form>
</body>
</html>
