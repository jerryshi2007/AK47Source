Type.registerNamespace("JobSchedule");
JobSchedule.JobEditor = function (txtName, txtDesc, ddlEnabled, gridSchedule) {
	this.Job = null;
	this.txtName = txtName;
	this.txtDesc = txtDesc;
	this.ddlEnabled = ddlEnabled;
	this.gridSchedule = gridSchedule;
	//this.Schedules = [];
}
JobSchedule.JobEditor.prototype = {
	Get_Job: function () {
		return this.Job
	},
	Set_Job: function (value) {
		this.Job = value;
		//this.BindValue();
	},
	BindValue: function () {
		this.txtName.val(this.Job.Name);
		this.txtDesc.val(this.Job.Description);
		this.ddlEnabled.val(this.Job.Enabled.toString());
		//this.gridSchedule.set_dataSource(this.Job.Schedules);
	},
	AddSchedule: function (schedule) {
		this.Job.Schedules.push(schedule);

	},
	BindSchedule: function (schedules) {
		this.gridSchedule.set_dataSource(schedules);
	}
}
JobSchedule.JobEditor.registerClass("JobSchedule.JobEditor");

JobSchedule.ScheduleEditor = function (txtName, ddlFrequency, recurDay, recurWeek, recurMonth, timeFrequency, durationTime) {
	this.schedule = {};
	this.TextName = txtName;
	this.ddlFrequency = ddlFrequency;
	this.TimeFrequencyEditor = new JobSchedule.TimeFrequencyEditor(timeFrequency);
	this.RecurDayEditor = new JobSchedule.RecurDayEditor(recurDay, this.TimeFrequencyEditor);
	this.RecurWeekEditor = new JobSchedule.RecurWeekEditor(recurWeek, this.TimeFrequencyEditor);
	this.RecurMonthEditor = new JobSchedule.RecurMonthEditor(recurMonth, this.TimeFrequencyEditor);
	this.DurationTimeEditor = new JobSchedule.DurationTimeEditor(durationTime);
	this.ddlFrequency.change(Function.createDelegate(this, this.Toggle));

}
JobSchedule.ScheduleEditor.prototype = {
	Toggle: function (e) {
		switch (jQuery(e.srcElement).val()) {
			case "daily":
				this.RecurDayEditor.Show();
				this.RecurWeekEditor.Hide();
				this.RecurMonthEditor.Hide();
				break;
			case "weekly":
				this.RecurDayEditor.Hide();
				this.RecurWeekEditor.Show();
				this.RecurMonthEditor.Hide();
				break;
			case "monthly":
				this.RecurDayEditor.Hide();
				this.RecurWeekEditor.Hide();
				this.RecurMonthEditor.Show();
				break;
		}
	},

	get_schedule: function () {
		if (this.TextName.val().trim() == '') {
			alert('请填写计划名称！');
			this.TextName.focus();
			return;
		}

		if (this.DurationTimeEditor.checkInput() == false) {
			return;
		}

		if (this.TimeFrequencyEditor.checkInput() == false) {
			return;
		}

		this.schedule.Name = this.TextName.val();
		this.schedule.Enabled = jQuery("#chbEnabled").attr("checked");
		this.schedule.DailyScheduleFrequency = this.RecurDayEditor.get_dailyFrequency();
		this.schedule.WeeklyFrequency = this.RecurWeekEditor.get_WeeklyFrequency();
		this.schedule.MonthlyFrequency = this.RecurMonthEditor.get_monthlyFrequency();
		this.schedule.StartTime = this.DurationTimeEditor.get_durationTime().StartTime;
		this.schedule.EndTime = this.DurationTimeEditor.get_durationTime().EndTime;

		return this.schedule;
	}

}
JobSchedule.ScheduleEditor.registerClass("JobSchedule.ScheduleEditor");

JobSchedule.RecurDayEditor = function (recurDay, timeFrequency) {
	this.recurDay = recurDay;
	this.timeRrequencyEditor = timeFrequency;
	this.txtDurationDays = recurDay.find("input");
	this.durationDays = 0;
	this.frequencyTime = null;
	this.DailyFrequency = {};

	this.txtDurationDays.keypress(onKeydownHandler);
	this.txtDurationDays.blur(function (e) {
		var txtbox = e.srcElement;
		if (txtbox.value <= 0) {
			txtbox.value = 1;
		} else if (txtbox.value > 100) {
			txtbox.value = 100;
		}

	});
}
JobSchedule.RecurDayEditor.prototype = {
	Toggle: function () {
		this.recurDay.toggle();
	},
	Show: function () {
		this.recurDay.show();
	},
	Hide: function () {
		this.recurDay.hide();
	},

	get_frequencyTime: function () {
		return this.frequencyTime;
	},
	set_frequencyTime: function (value) {
		this.frequencyTime = value;
	},
	get_durationDays: function () {

		return txtDurationDays.value;
	},
	set_durationDays: function (value) {
		return
	},
	get_dailyFrequency: function () {
		this.DailyFrequency.DurationDays = this.txtDurationDays.val();
		this.DailyFrequency.FrequencyTime = this.timeRrequencyEditor.Get_TimeFrequency();
		return this.DailyFrequency;
	}
}
JobSchedule.RecurDayEditor.registerClass("JobSchedule.RecurDayEditor");

JobSchedule.RecurWeekEditor = function (recurWeek, timeFrequency) {
	this.recurWeek = recurWeek;
	this.timeRrequencyEditor = timeFrequency;

	this.WeeklyFrequency = {};
	this.frequencyTime = null;

	this.recurWeek.find("#txtWeek").keypress(onKeydownHandler);
	this.recurWeek.find("#txtWeek").blur(function (e) {
		var txtbox = e.srcElement;
		if (txtbox.value <= 0) {
			txtbox.value = 1;
		} else if (txtbox.value > 100) {
			txtbox.value = 100;
		}

	})
}
JobSchedule.RecurWeekEditor.prototype = {
	Toggle: function () {
		this.recurWeek.toggle();
	},
	Show: function () {
		this.recurWeek.show();
	},
	Hide: function () {
		this.recurWeek.hide();
	},

	get_frequencyTime: function () {
		return this.frequencyTime;
	},
	set_frequencyTime: function (value) {
		this.frequencyTime = value;
	},
	get_WeeklyFrequency: function () {
		var daysOfWeek = [];
		var chbs = this.recurWeek.find(":checkbox[checked]");
		chbs.each(function (index, ele) {
			daysOfWeek.push(jQuery(ele).val())
		})
		this.WeeklyFrequency.DurationWeeks = this.recurWeek.find("input[type='text']").val();
		this.WeeklyFrequency.DaysOfWeek = daysOfWeek.sort();
		this.WeeklyFrequency.FrequencyTime = this.timeRrequencyEditor.Get_TimeFrequency();

		return this.WeeklyFrequency;
	}


}
JobSchedule.RecurWeekEditor.registerClass("JobSchedule.RecurWeekEditor");

JobSchedule.RecurMonthEditor = function (recurMonth, timeFrequency) {
	this.recurMonth = recurMonth;
	this.timeRrequencyEditor = timeFrequency;
	this.MonthlyFrequency = {};
	this.radioRecurMonth = jQuery("input[name*='recurMonthGroup']");
	this.radioRecurMonth.change(Function.createDelegate(this, this.SetState));
	this.radioRecurMonth.click(Function.createDelegate(this, this.SetState));

	this.recurMonth.find("#txtDayOfWeek").keypress(onKeydownHandler);
	this.recurMonth.find("#txtDayOfWeek").blur(function (e) {
		var txtbox = e.srcElement;
		if (txtbox.value <= 0) {
			txtbox.value = 1;
		} else if (txtbox.value > 31) {
			txtbox.value = 31;
		}

	});
	this.recurMonth.find("#txtMonthCount").keypress(onKeydownHandler);
	this.recurMonth.find("#txtMonthCount").blur(function (e) {
		var txtbox = e.srcElement;
		if (txtbox.value <= 0) {
			txtbox.value = 1;
		} else if (txtbox.value > 99) {
			txtbox.value = 99;
		}

	});
	if (currScheduleId && currScheduleId == "undefined") {
		this.radioRecurMonth[0].click();
	}
}
JobSchedule.RecurMonthEditor.prototype = {
	Toggle: function () {
		this.recurMonth.toggle();
	},
	Show: function () {
		this.recurMonth.show();
	},
	Hide: function () {
		this.recurMonth.hide();
	},
	SetState: function (e) {
		var radioBtn = e.srcElement;

		var parentTable = this.recurMonth.find("table");
		var editElementsToBedisable;
		var editElementsToBeenable;
		switch (jQuery(radioBtn).val()) {
			case "anyDay":
				//var parent = jQuery(radioBtn).parent()
				var radio = parentTable.find("input[value='specificDay']");
				editElementsToBedisable = jQuery(radio).parent().parent().find("input[type='text'],select");
				editElementsToBeenable = jQuery(radioBtn).parent().parent().find("input[type='text']");
				//parent.find(":input[type='text']");
				break;
			case "specificDay":
				var radio = parentTable.find("input[value='anyDay']");
				editElementsToBedisable = jQuery(radio).parent().parent().find("input[type='text']");
				editElementsToBeenable = jQuery(radioBtn).parent().parent().find("input[type='text'],select");
				break;

		}
		jQuery(editElementsToBedisable).each(function (index, ele) {
			jQuery(ele).attr("disabled", "disabled");
		})
		jQuery(editElementsToBeenable).each(function (index, ele) {
			jQuery(ele).attr("disabled", "");
		})
	},
	get_monthlyFrequency: function () {
		this.MonthlyFrequency.FrequencyTime = this.timeRrequencyEditor.Get_TimeFrequency();
		return this.MonthlyFrequency;
	}

}
JobSchedule.RecurMonthEditor.registerClass("JobSchedule.RecurMonthEditor");

JobSchedule.TimeFrequencyEditor = function (timeFrequency) {
	this.timeFrequencyContainer = timeFrequency;
	this.timeFrequency = {};
	this.fixedTimeFrequency = {};
	this.recurringTimeFrequency = {};
	this.radioRecurDay = jQuery("input[name*='recurDayGroup']");
	this.ddlIntervalTimeFrequencyUnit = jQuery("#ddlIntervalTimeFrequencyUnit");
	this.ddlIntervalTimeFrequencyUnit.change(function (e) {
		var item = e.srcElement;
		switch (jQuery(this).val()) {
			case "0":
				if (jQuery("#txtIntervalTimeFrequency").val() > 12)
					jQuery("#txtIntervalTimeFrequency").val(12);
				break;
			case "1":
				if (jQuery("#txtIntervalTimeFrequency").val() > 60)
					jQuery("#txtIntervalTimeFrequency").val(60);
				break;
			case "0":
				if (jQuery("#txtIntervalTimeFrequency").val() > 60)
					jQuery("#txtIntervalTimeFrequency").val(60);
				break;

		}
	});
	this.radioRecurDay.change(Function.createDelegate(this, this.SetState));
	this.radioRecurDay.click(Function.createDelegate(this, this.SetState));
	jQuery("#txtIntervalTimeFrequency").blur(function (e) {
		var txtbox = e.srcElement;
		switch (jQuery("#ddlIntervalTimeFrequencyUnit").val()) {
			case "0":
				if (txtbox.value <= 0) {
					txtbox.value = 1;
				} else if (txtbox.value > 24) {
					txtbox.value = 24;
				}

				break;
			case "1":
				if (txtbox.value <= 0) {
					txtbox.value = 1;
				} else if (txtbox.value > 60) {
					txtbox.value = 60;
				}
			case "2":
				if (txtbox.value <= 0) {
					txtbox.value = 1;
				} else if (txtbox.value > 60) {
					txtbox.value = 60;
				}
				break;
		}

	});
	jQuery("#txtIntervalTimeFrequency").keypress(onKeydownHandler);

	if (currScheduleId == "undefined") {
		this.radioRecurDay[0].click();
	}
}

JobSchedule.TimeFrequencyEditor.prototype = {
	SetState: function (e) {
		switch (jQuery(e.srcElement).val()) {
			case "execOnce":
				var tr = this.timeFrequencyContainer.find("table").find("tr:even");
				var trEnable = this.timeFrequencyContainer.find("table").find("tr:odd");
				this.fixedTimeFrequency.OccurTime = $find("execOnceTime").get_TimeValue();
				this.timeFrequency = this.fixedTimeFrequency;
				jQuery("#txtIntervalTimeFrequency").attr("disabled", "disabled");
				jQuery("#ddlIntervalTimeFrequencyUnit").attr("disabled", "disabled");
				$find("execTimeFrequencyStartTime").set_ReadOnly(true);
				$find("execTimeFrequencyEndTime").set_ReadOnly(true);

				$find("execOnceTime").set_ReadOnly(false);

				break;
			case "execInterval":
				var tr = this.timeFrequencyContainer.find("table").find("tr:odd");
				var trEnable = this.timeFrequencyContainer.find("table").find("tr:even");

				this.recurringTimeFrequency.Interval = jQuery("#txtIntervalTimeFrequency").val();
				this.recurringTimeFrequency.Unit = jQuery("#ddlIntervalTimeFrequencyUnit").val()
				this.recurringTimeFrequency.StartTime = $find("execTimeFrequencyStartTime").get_TimeValue();
				this.recurringTimeFrequency.EndTime = $find("execTimeFrequencyEndTime").get_TimeValue();

				this.timeFrequency = this.recurringTimeFrequency;

				jQuery("#txtIntervalTimeFrequency").attr("disabled", "");
				jQuery("#ddlIntervalTimeFrequencyUnit").attr("disabled", "");
				$find("execTimeFrequencyStartTime").set_ReadOnly(false);
				$find("execTimeFrequencyEndTime").set_ReadOnly(false);

				$find("execOnceTime").set_ReadOnly(true);

				break;

		}
	},
	Get_TimeFrequency: function () {
		return this.timeFrequency;
	},
	checkInput: function () {
		if ($get('radioRecurDayOnce').checked) {
			if (!$find("execOnceTime").get_TimeValue()) {
				alert('请输入执行时间！');
				$get('execOnceTime').focus();
				return false;
			}
		}

		if ($get('radioRecurDayPeriod').checked) {
			var startTimeID = 'execTimeFrequencyStartTime';
			if (!$find(startTimeID).get_TimeValue()) {
				alert('请输入起始时间！');
				$get(startTimeID).focus();
				return false;
			}
			var endTimeID = 'execTimeFrequencyEndTime';
			if (!$find(endTimeID).get_TimeValue()) {
				alert('请输入结束时间！');
				$get(endTimeID).focus();
				return false;
			}

			if ($find(startTimeID).get_TimeValue().getTicks() > $find(endTimeID).get_TimeValue().getTicks()) {
				alert('起始时间不能小于结束时间！');
				$get(startTimeID).focus();
				return false;
			}
		}
	}
}

JobSchedule.TimeFrequencyEditor.registerClass("JobSchedule.TimeFrequencyEditor");

JobSchedule.DurationTimeEditor = function (durationTime) {
	this._isNoEndDate = true;
	this.durationTime = {
		StartTime: $find("durationStartDate").get_DateValue(),
		EndTime: $find("durationEndDate").get_DateValue()
	}

	jQuery("input[name*='durationTimeGroup']").change(Function.createDelegate(this, this.SetState));
	jQuery("input[name*='durationTimeGroup']").click(Function.createDelegate(this, this.SetState));

	jQuery("#durationStartDate").change(Function.createDelegate(this, this.SetState));
	jQuery("#durationEndDate").change(Function.createDelegate(this, this.SetState));
	if (jQuery("#radioNoEndDate").attr("checked")) {
		$find('durationEndDate').set_ReadOnly(true);
	}
	if (currScheduleId && currScheduleId == "undefined")
		this.radioDuration[0].click();
}

JobSchedule.DurationTimeEditor.prototype = {
	SetState: function (e) {
		switch (jQuery(e.srcElement).val()) {
			case "endDate":
				$find("durationEndDate").set_ReadOnly(false);
				this._isNoEndDate = false;
				break;
			case "noEndDate":
				$find("durationEndDate").set_ReadOnly(true);
				this._isNoEndDate = true;
				break;
		}
	},
	get_durationTime: function () {
		return this.durationTime;
	},
	checkInput: function () {
		var startDateCtrl = $find("durationStartDate");
		if (startDateCtrl.get_DateValue().getTicks() <= Date.minDate.getTicks()) {
			alert('请输入开始日期！');
			$get('durationStartDate').focus();
			return false;
		}

		if (!this._isNoEndDate) {
			var endDateCtrl = $find("durationEndDate");
			if (endDateCtrl.get_DateValue().getTicks() <= Date.minDate.getTicks()) {
				alert('请输入结束日期！');
				$get('durationEndDate').focus();
				return false;
			}

			if (startDateCtrl.get_DateValue().getTicks() > endDateCtrl.get_DateValue().getTicks()) {
				alert('开始日期不能小于结束日期！');
				$get('durationStartDate').focus();
				return false;
			}
		}

		return true;
	}
}

function HourLimitHandler(e) {
	var txtbox = e.srcElement;
	//
	if (txtbox.value <= 0) {
		txtbox.value = 1;
	} else if (txtbox.value > 24) {
		txtbox.value = 24;
	}
}

function onKeydownHandler(e) {
	//alert(e);
	if (e.keyCode < 45 || e.keyCode > 57) {
		e.preventDefault();
	}
	//this.value = this.value.replace(/\D/g, ''); //
}