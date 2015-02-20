<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScheduleEditor.aspx.cs"
	Inherits="WorkflowDesigner.PlanScheduleDialog.ScheduleEditor" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>计划编辑</title>
	<base target="_self" />
	<style type="text/css">
		#recurDayContainer input
		{
			width: 70px;
			text-align: right;
		}
		legend
		{
			font-weight: bold;
			font-size: 14px;
		}
		#recurMonthContainer input
		{
			width: 70px;
			text-align: right;
		}
		#timeFrequencyContainer input
		{
			text-align: right;
		}
		.visible
		{
			display: block;
		}
		.invisible
		{
			display: none;
		}
	</style>
	<script src="../js/jquery-1.4.3.js" type="text/javascript"></script>
	<script type="text/javascript">
		var scheduleEditor;
		var currScheduleId;


		function onOKBtnClick() {
			var schedule = scheduleEditor.get_schedule();
			if (schedule) {
				$get("btnConfirm").click();
			}
		}

		function onClientEstimate() {
			$get("btnEstimate").click();
		}

		function onResetButtonClick() {
			SubmitButton.resetAllStates();
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server" target="innerFrame">
	<input type="hidden" runat="server" id="clientReturn" />
	<SOA:DataBindingControl runat="server" ID="scheduleBindingControl" AutoBinding="true"
		IsValidateOnSubmit="true">
		<ItemBindings>
			<SOA:DataBindingItem ControlID="txtScheduleName" DataPropertyName="Name" ValidationGroup="1">
			</SOA:DataBindingItem>
			<SOA:DataBindingItem ControlID="chbEnabled" DataPropertyName="Enabled" ValidationGroup="1">
			</SOA:DataBindingItem>
			<SOA:DataBindingItem ControlID="durationStartDate" DataPropertyName="StartTime" ValidationGroup="1">
			</SOA:DataBindingItem>
			<SOA:DataBindingItem ControlID="durationEndDate" DataPropertyName="EndTime" ValidationGroup="1">
			</SOA:DataBindingItem>
		</ItemBindings>
	</SOA:DataBindingControl>
	<SOA:DataBindingControl runat="server" ID="dailyScheduleFrequencyBindingControl"
		AutoBinding="true" IsValidateOnSubmit="true">
		<ItemBindings>
			<SOA:DataBindingItem ControlID="txtRecurDay" DataPropertyName="Name" ValidationGroup="1">
			</SOA:DataBindingItem>
		</ItemBindings>
	</SOA:DataBindingControl>
	<table style="height: 100%; width: 100%">
		<tr>
			<td class="gridHead">
				<div class="dialogTitle">
					<span class="dialogLogo">计划编辑</span>
				</div>
			</td>
		</tr>
		<tr>
			<td>
				<div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
					overflow: auto">
					计划名称：
					<SOA:HBTextBox ID="txtScheduleName" runat="server" Width="280" />
					<asp:CheckBox ID="chbEnabled" Text="是否启用" Checked="false" runat="server" />
				</div>
			</td>
		</tr>
		<tr>
			<td style="text-align: left; vertical-align: top">
				<fieldset title="">
					<legend>频率</legend>
					<table>
						<tr>
							<td style="width: 70px; text-align: right">
								发生频率：
							</td>
							<td>
								<select id="ddlFrequency" runat="server" style="width: 65px">
									<option value="daily">天</option>
									<option value="weekly">周</option>
									<option value="monthly">月</option>
								</select>
							</td>
						</tr>
					</table>
					<div id="recurDayContainer" class="visible" runat="server">
						<table>
							<tr>
								<td style="width: 70px; text-align: right">
									周期：
								</td>
								<td>
									每<input id="txtRecurDay" type="text" runat="server" value="1" style="width: 40px;
										text-align: right;" />天
								</td>
							</tr>
						</table>
					</div>
					<div id="recurWeekContainer" class="invisible" runat="server">
						<table>
							<tr>
								<td valign="top" rowspan="2" style="width: 70px; text-align: right">
									周期：
								</td>
								<td>
									每<input id="txtWeek" type="text" runat="server" value="1" style="width: 40px; text-align: right;" />周的
								</td>
							</tr>
							<tr>
								<td colspan="2" align="left">
									<table>
										<tr>
											<td>
												<input id="chbMonday" name="weekGroup" type="checkbox" value="1" runat="server" /><label
													for="chbMonday">星期一</label>
											</td>
											<td>
												<input id="chbTuesday" name="weekGroup" type="checkbox" value="2" runat="server" /><label
													for="chbTuesday">星期二</label>
											</td>
											<td>
												<input id="chbWednesday" name="weekGroup" type="checkbox" value="3" runat="server" /><label
													for="chbWednesday">星期三</label>
											</td>
											<td>
												<input id="chbThursday" name="weekGroup" type="checkbox" value="4" runat="server" /><label
													for="chbTuesday">星期四</label>
											</td>
										</tr>
										<tr>
											<td>
												<input id="chbFriday" name="weekGroup" type="checkbox" value="5" runat="server" /><label
													for="chbFriday">星期五</label>
											</td>
											<td>
												<input id="chbSaturday" name="weekGroup" type="checkbox" value="6" runat="server" /><label
													for="chbSaturday">星期六</label>
											</td>
											<td>
												<input id="chbSunday" name="weekGroup" type="checkbox" value="0" runat="server" /><label
													for="chbSunday">星期日</label>
											</td>
											<td>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</div>
					<div id="recurMonthContainer" class="invisible" runat="server">
						<table>
							<tr>
								<td style="width: 70px; text-align: right">
									周期：
								</td>
								<td>
									每<input id="txtMonthCount" type="text" runat="server" value="1" style="width: 40px;
										text-align: right;" />个月，第<input id="txtDayOfWeek" type="text" runat="server" value="1"
											style="width: 40px; text-align: right;" />天
								</td>
							</tr>
						</table>
					</div>
				</fieldset>
				<br />
				<fieldset>
					<legend>当天执行方式</legend>
					<div id="timeFrequencyContainer">
						<table>
							<tr>
								<td>
									<input id="radioRecurDayOnce" value="execOnce" checked type="radio" name="recurDayGroup"
										runat="server" />
									<label style="cursor: pointer" for="radioRecurDayOnce">
										仅一次，在</label>
									<MCS:DeluxeTime ID="execOnceTime" runat="server" MValue="12:00:00" />点
								</td>
							</tr>
							<tr>
								<td>
									<input id="radioRecurDayPeriod" value="execInterval" type="radio" name="recurDayGroup"
										runat="server" />
									<label style="cursor: pointer" for="radioRecurDayPeriod">
										间隔执行，</label>
									在<MCS:DeluxeTime ID="execTimeFrequencyStartTime" Enabled="false" runat="server" />点，到
									<MCS:DeluxeTime ID="execTimeFrequencyEndTime" Enabled="false" runat="server" />之间，每
									<input id="txtIntervalTimeFrequency" type="text" runat="server" value="1" disabled
										style="width: 50px;" />
									<select id="ddlIntervalTimeFrequencyUnit" runat="server" disabled>
										<option value="0">小时</option>
										<option value="1">分钟</option>
									</select>
								</td>
							</tr>
						</table>
					</div>
				</fieldset>
				<br />
				<fieldset>
					<legend>持续时间</legend>
					<table>
						<tr>
							<td>
								开始日期
							</td>
							<td>
								<MCS:DeluxeCalendar ID="durationStartDate" runat="server" />
							</td>
							<td>
								<table>
									<tr>
										<td>
											<input id="radioEndDate" type="radio" name="durationTimeGroup" value="endDate" runat="server" />结束日期
										</td>
										<td>
											<MCS:DeluxeCalendar ID="durationEndDate" runat="server" />
										</td>
									</tr>
									<tr>
										<td>
											<input id="radioNoEndDate" type="radio" name="durationTimeGroup" value="noEndDate"
												runat="server" />无结束时间
										</td>
									</tr>
								</table>
							</td>
						</tr>
					</table>
				</fieldset>
			</td>
		</tr>
		<tr>
			<td style="text-align: left; vertical-align: top;">
				<%--                <fieldset>
                    <legend>摘要</legend>
                        <table>
                        <tr>
                            <td style="width:80px;">
                                说明</td>
                            <td>
                    <textarea id="txtSummary" cols="20" rows="2" style="width:100%" name="S1" readonly=readonly></textarea></td>
                        </tr>
                    </table>
                </fieldset>
				--%>
			</td>
		</tr>
		<tr>
			<td style="height: 40px; text-align: center; vertical-align: middle">
				<table style="width: 100%; height: 100%">
					<tr>
						<td style="text-align: center;">
							<input type="button" class="formButton" runat="server" value="确定(O)" id="btnOK" accesskey="O" />
							<SOA:SubmitButton runat="server" ID="btnConfirm" Style="display: none" OnClick="btnConfirm_Click"
								RelativeControlID="btnOK" PopupCaption="正在保存..." />
						</td>
						<td style="text-align: center;">
							<input type="button" class="formButton" runat="server" title="预估后续执行时间..." value="预估(E)..."
								id="btnClientEstimate" accesskey="E" />
							<SOA:SubmitButton runat="server" ID="btnEstimate" Style="display: none" OnClick="btnEstimate_Click"
								RelativeControlID="btnClientEstimate" PopupCaption="正在预估..." />
						</td>
						<td style="text-align: center;">
							<input type="button" class="formButton" onclick="top.close();" value="关闭(C)" id="btnCancel"
								accesskey="C" />
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	</form>
	<div style="display: none">
		<input type="button" id="resetButton" />
		<iframe name="innerFrame"></iframe>
	</div>
	<script src="JobEditor.js" type="text/javascript"></script>
	<script type="text/javascript">
		Sys.Application.add_load(function () {
			currScheduleId = window.dialogArguments;
			scheduleEditor = new JobSchedule.ScheduleEditor(jQuery("#txtScheduleName"), jQuery("#ddlFrequency"),
				jQuery("#recurDayContainer"), jQuery("#recurWeekContainer"),
				jQuery("#recurMonthContainer"), jQuery("#timeFrequencyContainer"));

			jQuery("#btnOK").click(onOKBtnClick);
			jQuery("#btnClientEstimate").click(onClientEstimate);
			jQuery("#resetButton").click(onResetButtonClick);
		});
	</script>
</body>
</html>
