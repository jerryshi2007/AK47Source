
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	DeluxeCalendar.js
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    周维海	    20080129		创建
// -------------------------------------------------


$HGRootNS.DeluxeDateTime = function (element) {
	$HGRootNS.DeluxeDateTime.initializeBase(this, [element]);
	this._calendarControl = null;
	this._calendarControlID = null;
	this._calendarControlProperties = {};
	this._calendarControlEvents = {};
	this._timeControl = null;
	this._timeControlID = null;
	this._timeControlProperties = {};
	this._timeControlEvents = {};
	this._value = null;
	//add by wuwei
	this.ReadOnly = false;
	this.DateTimeValue = null;

	this._calandarValueChanged$delegate = null;
	this._timeValueChanged$delegate = null;

	this._tag = null;

	this._elementStyle = element.style;
}

$HGRootNS.DeluxeDateTime.prototype = {

	//为在客户端添加新的DeluxeDateTime（new 出一个新的实例） 时做的重载
	//参数preLoadDeluxeDateTimeID为预先下去的一个DeluxeCalendar
	clientInitialize: function (preLoadDeluxeDateTimeID) {
		this.initialize();
		this.get_calendarControl()._imageButton.src = $find(preLoadDeluxeDateTimeID).get_calendarControl().get_imageButtonPath();
		this.get_calendarControl()._imageButton.style["cursor"] = "pointer";
	},

	initialize: function () {
		$HGRootNS.DeluxeDateTime.callBaseMethod(this, "initialize");
		this._createChildControl();
	},

	dispose: function () {
		if (this._calendarControl != null && this._calandarValueChanged$delegate != null)
			this._calendarControl.remove_clientValueChanged(this._calandarValueChanged$delegate);

		if (this._timeControl != null && this._timeValueChanged$delegate != null)
			this._timeControl.remove_OnClientValueChanged(this._timeValueChanged$delegate);

		$HGRootNS.DeluxeDateTime.callBaseMethod(this, "dispose");
	},

	_createChildControl: function () {
		var elt = this.get_element();
		if (this._calendarControlID) {
			this._calendarControl = $find(this._calendarControlID);
		}
		else {
			var calendarInput = $HGDomElement.createElementFromTemplate
				(
					{
						nodeName: "input",
						properties: { type: "text" }
					},
					elt
				);

			if (this._elementStyle) {
				for (var s in this._elementStyle) {
					if (this._elementStyle[s])
						calendarInput.style[s] = this._elementStyle[s];
				}
			}

			this._calendarControl = $create($HGRootNS.DeluxeCalendar,
				this._calendarControlProperties,
				this._calendarControlEvents,
				null,
				calendarInput);
		}

		if (this._timeControlID) {
			this._timeControl = $find(this._timeControlID);
		}
		else {
			var timeInput = $HGDomElement.createElementFromTemplate
				(
					{
						nodeName: "input",
						properties: { type: "text" }
					},
					elt
				);
			this._timeControl = $create($HGRootNS.DeluxeTime,
				this._timeControlProperties,
				this._timeControlEvents,
				null,
				timeInput);
		}

		this._calandarValueChanged$delegate = Function.createDelegate(this, this.onClientValueChanged);
		if (this._calendarControl) {
			this._calendarControl.add_clientValueChanged(this._calandarValueChanged$delegate);
		}

		this._timeValueChanged$delegate = Function.createDelegate(this, this.onClientValueChanged);
		if (this._timeControl) {
			this._timeControl.add_OnClientValueChanged(this._timeValueChanged$delegate);
		}
	},

	/****************properties****************{****************/
	get_calendarControl: function () {
		return this._calendarControl;
	},
	set_calendarControl: function (value) {
		this._calendarControl = value;
	},

	get_calendarControlID: function () {
		return this._calendarControlID;
	},
	set_calendarControlID: function (value) {
		this._calendarControlID = value;
	},

	get_calendarControlProperties: function () {
		return this._calendarControlProperties;
	},
	set_calendarControlProperties: function (value) {
		this._calendarControlProperties = value;
	},

	get_timeControl: function () {
		return this._timeControl;
	},
	set_timeControl: function (value) {
		this._timeControl = value;
	},

	get_timeControlID: function () {
		return this._timeControlID;
	},
	set_timeControlID: function (value) {
		this._timeControlID = value;
	},

	get_timeControlProperties: function () {
		return this._timeControlProperties;
	},
	set_timeControlProperties: function (value) {
		this._timeControlProperties = value;
	},

	get_value: function () {
		this._value = this.get_DateTimeValue();
		return this._value;
	},

	set_value: function (value) {
		this.set_DateTimeValue(value);
		this._value = value;
	},

	//Datetime properties add by wuwei
	//add by wuwei
	get_DateTimeValue: function () {

		var date = this.get_calendarControl().get_DateValue();
		var time = this.get_timeControl().get_TimeValue();

		if (date == null)
			return null;

		if (time == null) {
			time = new Date();
			time.setHours(0, 0, 0, 0);
		}

		if (isNaN(Date.parse(date)) || isNaN(Date.parse(time)))
			return Date.minDate;

		var newDate = new Date(date.getFullYear(), date.getMonth(), date.getDate(), time.getHours(), time.getMinutes(), time.getSeconds());

		return newDate;
	},

	set_DateTimeValue: function (value) {
		if (!isNaN(Date.parse(value))) {
			this.DateTimeValue = value;
			this.get_calendarControl().set_DateValue(value);
			this.get_timeControl().set_TimeValue(value);
		} else {
			value = Date.minDate;
			this.DateTimeValue = null;
			this.get_calendarControl().set_DateValue(value);
			this.get_timeControl().set_TimeValue(value);
		}

		this.raisePropertyChanged("DateTimeValue");
	},

	get_ReadOnly: function () {
		return this.ReadOnly;
	},
	set_ReadOnly: function (value) {
		this.ReadOnly = value;
		this.get_calendarControl().set_ReadOnly(value);
		this.get_timeControl().set_ReadOnly(value);
	},

	get_tag: function () {
		return this._tag;
	},
	set_tag: function (value) {
		this._tag = value;
	},

	get_cloneableProperties: function () {
		var baseProperties = $HGRootNS.DeluxeDateTime.callBaseMethod(this, "get_cloneableProperties");
		var currentProperties = ["tag"];

		Array.addRange(currentProperties, baseProperties);

		return currentProperties;
	},

	_prepareCloneablePropertyValues: function (newElement) {
		var properties = $HGRootNS.DeluxeDateTime.callBaseMethod(this, "_prepareCloneablePropertyValues", [newElement]);

		var calendarCtrl = this._calendarControl.cloneAndAppendToContainer(newElement);
		var timeCtrl = this._timeControl.cloneAndAppendToContainer(newElement);

		properties["calendarControlID"] = calendarCtrl.get_element().id;
		properties["timeControlID"] = timeCtrl.get_element().id;

		return properties;
	},

	onClientValueChanged: function () {
		var newValue = this.get_DateTimeValue();

		if (this._value != newValue) {
			this._value = newValue;
			this.raiseClientValueChanged();
		}
	},

	raiseClientValueChanged: function () {
		var handlers = this.get_events().getHandler("clientValueChanged");
		if (handlers) {
			handlers(this, Sys.EventArgs.Empty);
		}
	},
	remove_clientValueChanged: function (handler) {
		this.get_events().removeHandler("clientValueChanged", handler);
	},
	add_clientValueChanged: function (handler) {
		this.get_events().addHandler("clientValueChanged", handler);
	}
	//properties end
	/****************}*************************/
}

$HGRootNS.DeluxeDateTime.registerClass($HGRootNSName + ".DeluxeDateTime", $HGRootNS.ControlBase);