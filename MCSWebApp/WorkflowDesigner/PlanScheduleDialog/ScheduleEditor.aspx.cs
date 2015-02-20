using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;
using System.Diagnostics;

namespace WorkflowDesigner.PlanScheduleDialog
{
	public partial class ScheduleEditor : System.Web.UI.Page
	{
		private PageMode PageEditMode { get; set; }
		private string ScheduleID { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
			Initialize();

			if (!IsPostBack)
			{
				try
				{
					InitControls();
				}
				catch (Exception ex)
				{
					WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
				}
			}
		}

		private void Initialize()
		{
			this.ScheduleID = WebUtility.GetRequestQueryValue<string>("scheduleId", "");
			if (string.IsNullOrEmpty(this.ScheduleID))
			{
				this.PageEditMode = PageMode.Create;
			}
			else
			{
				this.PageEditMode = PageMode.Edit;
			}
		}

		private void InitControls()
		{
			if (this.PageEditMode == PageMode.Create)
			{
				durationStartDate.Value = DateTime.Now;
				execTimeFrequencyStartTime.Enabled = false;
				execTimeFrequencyEndTime.Enabled = false;
				radioNoEndDate.Checked = true;
				chbEnabled.Checked = true;
			}
			else
			{
				var schedules = JobScheduleAdapter.Instance.Load(p => p.AppendItem("SCHEDULE_ID", this.ScheduleID));
				if (schedules.Count == 0)
				{
					throw new Exception(string.Format("不能找到ID为{0}的计划!", this.ScheduleID));
				}

				var schedule = schedules[0];

				txtScheduleName.Text = schedule.Name;
				txtScheduleName.ReadOnly = true;
				txtScheduleName.KeepControlWhenReadOnly = true;
				chbEnabled.Checked = schedule.Enabled;
				durationStartDate.Value = schedule.StartTime;

				if (schedule.EndTime != null)
				{
					durationEndDate.Value = schedule.EndTime.Value;
					radioEndDate.Checked = true;
				}
				else
				{
					radioNoEndDate.Checked = true;
				}

				if (schedule.ScheduleFrequency is DailyJobScheduleFrequency)
				{
					ddlFrequency.Items.FindByValue("daily").Selected = true;
					recurDayContainer.Attributes["class"] = "visible";
					recurWeekContainer.Attributes["class"] = "invisible";
					recurMonthContainer.Attributes["class"] = "invisible";
					txtRecurDay.Value = ((DailyJobScheduleFrequency)schedule.ScheduleFrequency).DurationDays.ToString();
				}
				else if (schedule.ScheduleFrequency is WeeklyJobScheduleFrequency)
				{
					ddlFrequency.Items.FindByValue("weekly").Selected = true;
					recurDayContainer.Attributes["class"] = "invisible";
					recurWeekContainer.Attributes["class"] = "visible";
					recurMonthContainer.Attributes["class"] = "invisible";
					txtWeek.Value = ((WeeklyJobScheduleFrequency)schedule.ScheduleFrequency).DurationWeeks.ToString();

					var chbs = serverForm.FindControls<HtmlInputCheckBox>(true);

					foreach (var item in ((WeeklyJobScheduleFrequency)schedule.ScheduleFrequency).DaysOfWeek)
					{
						foreach (HtmlInputCheckBox ctl in chbs)
						{
							if ((DayOfWeek)int.Parse(ctl.Value) == item)
							{
								ctl.Checked = true;
								break;
							}
						}
					}
				}
				else if (schedule.ScheduleFrequency is MonthlyJobScheduleFrequency)
				{
					ddlFrequency.Items.FindByValue("monthly").Selected = true;
					recurDayContainer.Attributes["class"] = "invisible";
					recurWeekContainer.Attributes["class"] = "invisible";
					recurMonthContainer.Attributes["class"] = "visible";

					txtDayOfWeek.Value = ((MonthlyJobScheduleFrequency)schedule.ScheduleFrequency).Day.ToString();
					txtMonthCount.Value = ((MonthlyJobScheduleFrequency)schedule.ScheduleFrequency).DurationMonths.ToString();
				}

				if (schedule.ScheduleFrequency.FrequencyTime is FixedTimeFrequency)
				{
					radioRecurDayOnce.Checked = true;
					execOnceTime.MValue = ((FixedTimeFrequency)schedule.ScheduleFrequency.FrequencyTime).OccurTime.ToString();

				}
				else if (schedule.ScheduleFrequency.FrequencyTime is RecurringTimeFrequency)
				{
					this.radioRecurDayPeriod.Checked = true;
					txtIntervalTimeFrequency.Value = ((RecurringTimeFrequency)schedule.ScheduleFrequency.FrequencyTime).Interval.ToString();
					txtIntervalTimeFrequency.Disabled = false;
					ddlIntervalTimeFrequencyUnit.Disabled = false;
					execTimeFrequencyStartTime.Enabled = true;
					execTimeFrequencyEndTime.Enabled = true;
					execTimeFrequencyStartTime.MValue = ((RecurringTimeFrequency)schedule.ScheduleFrequency.FrequencyTime).StartTime.ToString();
					execTimeFrequencyEndTime.MValue = ((RecurringTimeFrequency)schedule.ScheduleFrequency.FrequencyTime).EndTime.ToString();

					int unit = (int)((RecurringTimeFrequency)schedule.ScheduleFrequency.FrequencyTime).Unit;
					ddlIntervalTimeFrequencyUnit.Items.FindByValue(unit.ToString()).Selected = true;
				}
			}
		}

		protected void btnConfirm_Click(object sender, EventArgs e)
		{
			try
			{
				JobSchedule schedule = GetSchedule();

				JobScheduleAdapter.Instance.Update(schedule);

				clientReturn.Value = MCS.Web.Library.Script.JSONSerializerExecute.Serialize(schedule);
				Page.ClientScript.RegisterStartupScript(this.GetType(), "updateSchedule",
					string.Format("window.returnValue = document.getElementById('clientReturn').value ; top.close();"), true);
			}
			catch (Exception ex)
			{
				WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
			}
			finally
			{
				this.ClientScript.RegisterClientScriptBlock(this.GetType(), "resetButtons", "top.document.getElementById('resetButton').click();", true);
			}
		}

		protected void btnEstimate_Click(object sender, EventArgs e)
		{
			try
			{
				JobSchedule schedule = GetSchedule();
				Stopwatch sw = new Stopwatch();
				TimeSpan timeout = TimeSpan.FromSeconds(10);

				sw.Start();

				List<DateTime> estimateTime = schedule.EstimateExecuteTime(TimeSpan.FromSeconds(30), 20, timeout);


				for (int i = estimateTime.Count - 1; i >= 0; i--)
				{
					if (estimateTime[i] > schedule.NormalizedEndTime)
						estimateTime.RemoveAt(i);
				}

				StringBuilder strB = new StringBuilder();

				foreach (DateTime time in estimateTime)
				{
					if (strB.Length > 0)
						strB.Append("\n");

					strB.Append(time.ToString("yyyy-MM-dd HH:mm:ss"));
				}

				sw.Stop();

				if (sw.Elapsed >= timeout)
					strB.Append("超时时间已到，无法计算出预估时间");
				else
					if (strB.Length == 0)
						strB.Append("无法计算出预估时间");

				WebUtility.ShowClientMessage(strB.ToString(), string.Empty, "预估执行时间");
			}
			catch (Exception ex)
			{
				WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
			}
			finally
			{
				this.ClientScript.RegisterClientScriptBlock(this.GetType(), "resetButtons", "top.document.getElementById('resetButton').click();", true);
			}
		}

		private JobSchedule GetSchedule()
		{
			JobSchedule schedule;

			if (this.PageEditMode == PageMode.Create)
			{
				TimeFrequencyBase timeFrequency = GetTimeFrequency();
				JobScheduleFrequencyBase jobScheduleFrequency = GetJobScheduleFrequency();

				schedule = new JobSchedule(Guid.NewGuid().ToString(),
					txtScheduleName.Text, durationStartDate.Value, jobScheduleFrequency)
					{
						Enabled = chbEnabled.Checked
					};
			}
			else
			{
				schedule = JobScheduleAdapter.Instance.Load(p => p.AppendItem("SCHEDULE_ID", this.ScheduleID))[0];
				schedule.Name = txtScheduleName.Text;
				schedule.Enabled = chbEnabled.Checked;
				schedule.StartTime = durationStartDate.Value;
				schedule.ScheduleFrequency = GetJobScheduleFrequency();
			}

			if (radioEndDate.Checked && durationEndDate.Value != null)
			{
				schedule.EndTime = durationEndDate.Value;
			}
			else
			{
				schedule.EndTime = null;
			}

			return schedule;
		}

		private TimeFrequencyBase GetTimeFrequency()
		{
			TimeFrequencyBase TimeFrequency = null;
			if (this.radioRecurDayOnce.Checked)
			{
				string timeValue = this.execOnceTime.Value;
				var time = DateTime.Parse(timeValue);
				TimeFrequency = new FixedTimeFrequency(time.TimeOfDay);
			}
			else if (this.radioRecurDayPeriod.Checked)
			{
				int interval = int.Parse(txtIntervalTimeFrequency.Value);
				IntervalUnit unit = (IntervalUnit)int.Parse(ddlIntervalTimeFrequencyUnit.Items[ddlIntervalTimeFrequencyUnit.SelectedIndex].Value);

				string startTimeValue = this.execTimeFrequencyStartTime.Value;
				var startTime = DateTime.Parse(startTimeValue);

				string endTimeValue = this.execTimeFrequencyEndTime.Value;
				var endTime = DateTime.Parse(endTimeValue);

				TimeFrequency = new RecurringTimeFrequency(interval, unit, startTime.TimeOfDay, endTime.TimeOfDay);
			}

			return TimeFrequency;
		}

		private JobScheduleFrequencyBase GetJobScheduleFrequency()
		{
			JobScheduleFrequencyBase jobScheduleFrequency = null;
			var timeFrequency = GetTimeFrequency();
			string frequencyType = this.ddlFrequency.Items[ddlFrequency.SelectedIndex].Value;
			switch (frequencyType)
			{
				case "daily":
					int dayCount = int.Parse(txtRecurDay.Value);
					jobScheduleFrequency = new DailyJobScheduleFrequency(dayCount, timeFrequency);
					break;
				case "weekly":
					List<DayOfWeek> weekdays = GetSelectedWeekday();
					jobScheduleFrequency = new WeeklyJobScheduleFrequency(weekdays, int.Parse(txtWeek.Value), timeFrequency);
					break;
				case "monthly":
					int days = int.Parse(this.txtDayOfWeek.Value);
					int months = int.Parse(this.txtMonthCount.Value);
					jobScheduleFrequency = new MonthlyJobScheduleFrequency(days, months, timeFrequency);
					break;
				default:
					break;
			}
			//jobScheduleFrequency.LastModifyTime = DateTime.Now;

			return jobScheduleFrequency;
		}

		private List<DayOfWeek> GetSelectedWeekday()
		{
			List<DayOfWeek> weekdays = new List<DayOfWeek>();

			foreach (HtmlInputCheckBox item in serverForm.FindControls<HtmlInputCheckBox>(true))
			{
				if (item.Checked)
					weekdays.Add((DayOfWeek)int.Parse(item.Value));
			}

			return weekdays;
		}
	}
}