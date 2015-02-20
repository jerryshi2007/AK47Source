/// <reference path="../Common/MicrosoftAjax.debug.js" />
/// <reference path="../../MCS.Web.Responsive.Library/Resources/ControlBase.js" />

$HGRootNS.DateTimePickerMode = function $HGRootNS$DateTimePickerMode() {
    /// <summary locid="M:J#$HGRootNS.DateTimePickerMode.#ctor" />
    /// <field name="DatePicker" type="Number" integer="true" static="true" locid="F:J#$HGRootNS.DateTimePickerMode.DatePicker"></field>
    /// <field name="TimePicker" type="Number" integer="true" static="true" locid="F:J#$HGRootNS.DateTimePickerMode.TimePicker"></field>
    /// <field name="DateTimePicker" type="Number" integer="true" static="true" locid="F:J#$HGRootNS.DateTimePickerMode.DateTimePicker"></field>
    if (arguments.length !== 0) throw Error.parameterCount();
    throw Error.notImplemented();
}
$HGRootNS.DateTimePickerMode.prototype = {
    DatePicker: 0,
    TimePicker: 1,
    DateTimePicker: 2
}
$HGRootNS.DateTimePickerMode.registerEnum($HGRootNSName + '.DateTimePickerMode');

$HGRootNS.DateTimePicker = function (element) {

    //客户端对象集合

    if (typeof (jQuery) === 'undefnied') {
        throw new Error("没有加载jQuery");
    }

    this._elemInput = null;
    this._elemHandle = null;

    this._mode = 0;
    this._dateControlReady = false;
    this._innerElementsReady = false;
    this._created = false;

    this.readOnly = false;
    this.enabled = true;
    this._placeHolder = "";
    this._startDateTime = Date.minDate;
    this._endDateTime = Date.minDate;
    this._value = Date.minDate;
    this._displayFormat = '';
    this._firstDayOfWeek = 0;
    this._dayOfToday = new Date();
    this._asComponent = false;
    this._asFormControl = false;
    this._autoComplete = true;

    $HGRootNS.DateTimePicker.initializeBase(this, [element]);

}

$HGRootNS.DateTimePicker.prototype =
{
    initialize: function () {
        //初始化控件

        this._initializeControlElements();

        $HGRootNS.DateTimePicker.callBaseMethod(this, 'initialize');

        this._created = true;
    },

    _initializeControlElements: function () {

        this._destroyControlElements();

        var elem = this.get_element(), pickDate = false, pickTime = false, format;
        //elem.className = "input-append date";
        var doc = elem.ownerDocument || elem.document;
        this._elemInput = doc.createElement("input");
        this._elemHandle = doc.createElement("span");
        this._elemHandleIcon = doc.createElement("i");

        this._elemInput.type = "text";
        this._elemInput.className = "form-control input-field";
        this._elemHandle.className = "input-group-addon ";

        if (this.get_asComponent())
            this._elemInput.readOnly = true;

        this._elemInput.placeholder = this.get_placeHolder();

        if (this.get_asFormControl())
            this._elemInput.name = this.get_id();


        elem.insertBefore(this._elemHandle, elem.firstChild);
        elem.insertBefore(this._elemInput, this._elemHandle);

        this._elemHandle.appendChild(this._elemHandleIcon);

        this._innerElementsReady = true;

        switch (this._mode) {
            case $HGRootNS.DateTimePickerMode.TimePicker:
                elem.className = "timepicker-input " + elem.className;
                this._elemHandleIcon.className = "icon-time";
                pickTime = true; pickDate = false;
                format = "HH:mm";
                break;
            case $HGRootNS.DateTimePickerMode.DatePicker:
                elem.className = "datepicker-input " + elem.className;
                this._elemHandleIcon.className = "icon-calendar";
                pickTime = false; pickDate = true;
                format = "YYYY-MM-DD";
                break;
            case $HGRootNS.DateTimePickerMode.DateTimePicker:
                elem.className = "datetimepicker-input " + elem.className;
                this._elemHandleIcon.className = "icon-calendar";
                pickDate = pickTime = true;
                format = "YYYY-MM-DD HH:mm";
                break;
            default:
                throw new Error("无法初始化，无法识别的模式：" + this._mode);
                break;
        }

        if (!$HGRootNS.DateTimePicker.cultureInitialized) {
            if (typeof (__cultureInfo) !== "undefined") {
                (function ($) {
                    try {
                        var langbig = {
                            days: Array.clone(__cultureInfo.dateTimeFormat.DayNames),
                            daysShort: Array.clone(__cultureInfo.dateTimeFormat.AbbreviatedDayNames),
                            daysMin: Array.clone(__cultureInfo.dateTimeFormat.ShortestDayNames),
                            months: Array.clone(__cultureInfo.dateTimeFormat.MonthNames),
                            monthsShort: Array.clone(__cultureInfo.dateTimeFormat.AbbreviatedMonthNames),
                            weekdaysMin: Array.clone(__cultureInfo.dateTimeFormat.ShortestDayNames),
                            today: "Today"
                        };

                        if (typeof (__datetimepickerRes_today) === 'string')
                            langbig.today = __datetimepickerRes_today;

                        moment.lang(__cultureInfo.name, langbig);
                    } catch (e) { }
                })(jQuery);
            }

            $HGRootNS.DateTimePicker.cultureInitialized = true;
        }

        var options = {};
        if (typeof (__cultureInfo) !== "undefined") {
            options["language"] = __cultureInfo.name;
        }

        var _self = this;

        options["format"] = format;
        options["pickDate"] = pickDate;
        options["pickTime"] = pickTime;
        options["weekStart"] = this.get_firstDayOfWeek() || 0;

        if (!Date.isMinDate(this.get_startDateTime()))
            options["endDate"] = this.get_startDateTime();

        if (!Date.isMinDate(this.get_endDateTime()))
            options["endDate"] = this.get_endDateTime();

        if (!Date.isMinDate(this.get_dayOfToday()))
            options["todayDate"] = this.get_dayOfToday();

        $(elem).datetimepicker(options).on("change.dp", function (ev) {
            var d = _self.get_jQueryPicker().getDate();
            if (d) {
                _self._value = d._d || Date.minDate;
            }
            else {
                _self._value = Date.minDate;
            }
            _self.raise_onClientValueChanged.apply(_self, [ev]);
        }).on("beforeparse.dp", function (ev) {
            return _self._innerBeforeParse.apply(_self, [ev]);
        });

        this.get_jQueryPicker().setDate(Date.isMinDate(this.get_value()) ? null : this.get_value());

        this._dateControlReady = true;
    },

    _destroyControlElements: function () {
        var elem = this.get_element();
        if (elem) {
            if (this._dateControlReady) {
                this._detatchEventsFromPicker();
                this.get_jQueryPicker().destroy();
                this._dateControlReady = false;
            }

            if (this._innerElementsReady) {
                if (this._elemHandle) {
                    if (this._elemHandleIcon) {
                        this._elemHandle.removeChild(this._elemHandleIcon);
                        this._elemHandleIcon = null;
                    }

                    elem.removeChild(this._elemHandle);
                    this._elemHandle = null;
                }

                if (this._elemInput) {
                    elem.removeChild(this._elemInput);
                    this._elemInput = null;
                }
            }

            this._innerElementsReady = false;
        }
    },

    _handlerPickerValueChanged: function (e) {

    },

    _innerConfigMode: function () {
        if (this._dateControlReady) {
            var timePicker = false;
            var datePicker = false;
            switch (this._mode) {
                case $HGRootNS.DateTimePickerMode.DatePicker:
                    datePicker = true;
                    break;
                case $HGRootNS.DateTimePickerMode.TimePicker:
                    timePicker = true;
                    break;
                case $HGRootNS.DateTimePickerMode.DateTimePicker:
                    datePicker = timePicker = true;
                    break;
                default:
                    break;
            }

            //不支持修改模式
        }
    },

    _innerBeforeParse: function (jQueryEv) {

        function toCorrectValue(str, rangeStart, rangeEnd, defVal) {
            if (typeof (str) === "string") {
                var intVall = parseInt(str);
                if (intVall >= rangeStart && intVall <= rangeEnd)
                    return intVall;
                else
                    return defVal;

            } else {
                return defVal;
            }
        }

        function validateDateObj(dateObj) {
            var validDate = true;
            if (dateObj.year < 70) {
                dateObj.year = 2000 + dateObj.year;
            } else if (dateObj.year < 100) {
                dateObj.year = 1900 + dateObj.year;
            }

            if (dateObj.year % 4 == 0 && dateObj.year % 100 != 0) {
                dateObj.leapYear = true;
            }

            if (((dateObj.month % 2 == 0 && dateObj.month <= 7) || (dateObj.month % 2 != 0 && dateObj.month >= 7)) && dateObj.day > 30) {
                validDate = false;
                dateObj.day = 30;
            }

            if (dateObj.month == 2) {
                if (dateObj.leapYear && dateObj.day > 29) {
                    validDate = false;
                    dateObj.day = 29;
                } else if (dateObj.leapYear == false && dateObj.day > 28) {
                    validDate = false;
                    dateObj.day = 28;
                }
            }

            return validDate;
        }

        var val = jQueryEv.strValue.trim(), newDate = new Date(), meet, dateObj, validateResult = false;
        if (val.length) {
            if ((meet = val.match(/^(((\d{2,4})(-|\/))?(\d{1,2})(-|\/)(\d{1,2}))?((\s?)(\d{1,2}):(\d{1,2})(:(\d+(\.\d+)?))?)?$/i))) {
                /*
                0: "2014-04-25 05:54:22.23",
                1: "2014-04-25",
                2: "2014-",
                3: "2014",
                4: "-",
                5: "04",
                6: "-",
                7: "25",
                8: " 05:54:22.23",
                9: " ",
                10: "05",
                11: "54",
                12: ":22.23",
                13: "22.23",
                14: ".23",
                */

                dateObj = {
                    year: toCorrectValue(meet[3], 0, 9998, newDate.getFullYear()),
                    month: toCorrectValue(meet[5], 1, 12, newDate.getMonth()),
                    day: toCorrectValue(meet[7], 1, 31, newDate.getDay()),
                    h: toCorrectValue(meet[10], 0, 23, 0),
                    m: toCorrectValue(meet[11], 0, 59, 0),
                    s: toCorrectValue(meet[13], 0, 59, 0),
                    leapYear: false
                };

                validateResult = validateDateObj(dateObj);

                val = String.format("{0:d2}-{1:d2}-{2:d2} {3:d2}:{4:d2}:{5:d2}", dateObj.year, dateObj.month, dateObj.day, dateObj.h, dateObj.m, dateObj.s);
            } else if (this.get_autoComplete() && this.get_mode() !== $HGRootNS.DateTimePickerMode.TimePicker && (meet = val.match(/^((0?[1-9])|(1[0-2]))((0?[1-9])|([1-2][0-9])|(3[0,1]))$/i))) {
                /*
                检查是否输入日期
                0: "0123",
                1: "01",
                2: "01",
                3: undefined,
                4: "23",
                5: undefined,
                6: "23",
                7: undefined,
                */
                dateObj = {
                    year: newDate.getFullYear(),
                    month: parseInt(meet[1]),
                    day: parseInt(meet[4]),
                    h: 0,
                    m: 0,
                    s: 0,
                    leapYear: false
                };

                validateResult = validateDateObj(dateObj);

                val = String.format("{0:d2}-{1:d2}-{2:d2}", dateObj.year, dateObj.month, dateObj.day);
            } else if (this.get_autoComplete() && this.get_mode() === $HGRootNS.DateTimePickerMode.TimePicker && (meet = val.match(/^((0?[0-9])|(1[0-9])|(2[0-4]))((0?[0-9])|([1-5][0-9]))$/i))) {
                //时间模式
                /*
                0: "955",
                1: "9",
                2: "9",
                3: undefined,
                4: undefined,
                5: "55",
                6: undefined,
                7: "55",
                */
                dateObj = {
                    year: newDate.getFullYear(),
                    month: newDate.getMonth(),
                    day: newDate.getDay(),
                    h: parseInt(meet[1]),
                    m: parseInt(meet[5]),
                    s: 0,
                    leapYear: false
                };

                validateResult = validateDateObj(dateObj);
                val = String.format("{0:d2}-{1:d2}-{2:d2} {3:d2}:{4:d2}:{5:d2}", dateObj.year, dateObj.month, dateObj.day, dateObj.h, dateObj.m, dateObj.s);
            }

            if (validateResult)
                this._elemInput.value = val;
            else {
                this.raise_onClientErrorDate(null);
            }
        }
    },

    get_cloneableProperties: function () {
        var baseProperties = $HGRootNS.DateTimePicker.callBaseMethod(this, "get_cloneableProperties");
        var currentProperties = ["mode", "readOnly", "enabled", "value", "todayDate"];

        Array.addRange(currentProperties, baseProperties);

        return currentProperties;
    },

    _prepareCloneablePropertyValues: function (newElement) {
        var properties = $HGRootNS.DateTimePicker.callBaseMethod(this, "_prepareCloneablePropertyValues", [newElement]);

        //        var calendarCtrl = this._calendarControl.cloneAndAppendToContainer(newElement);
        //        var timeCtrl = this._timeControl.cloneAndAppendToContainer(newElement);

        //        properties["calendarControlID"] = calendarCtrl.get_element().id;
        //        properties["timeControlID"] = timeCtrl.get_element().id;

        return properties;
    },

    // 华丽的开始--------------------------------属性-------------------------------------------------------------------------
    get_mode: function () {
        return this._mode;
    },
    set_mode: function (value) {
        var e = Function._validateParams(arguments, [
            { name: "value", mayBeNull: false, integer: true }
        ]);
        if (e) throw e;

        if (this._mode != value) {
            this._mode = value;

            this._innerConfigMode();
        }
    },
    get_value: function () {
        return this._value;
    },

    set_value: function (value) {
        var e = Function._validateParams(arguments, [
            { name: "value", mayBeNull: true, type: Date }
        ]);
        if (e) throw e;

        this._value = value || Date.minDate;

        if (this._dateControlReady) {
            var picker = this.get_jQueryPicker();
            if (picker) {
                picker.setValue(this._toPickerDate(value));
            }
        }
    },

    get_dateValue: function () {
        var val = this.get_value();
        if (Date.isMinDate(val) || isNaN(val * 1))
            return Date.minValue;
        else
            return new Date(val.getFullYear(), val.getMonth(), val.getDate());
    },

    set_dateValue: function (val) {
        if (!val || isNaN(val * 1) || Date.isMinDate(val)) {
            this.set_value(Date.minDate);
        } else {
            var date = new Date();
            date.setFullYear(val.getFullYear(), val.getMonth(), val.getDate());
            this.set_value(date);
        }
    },

    get_timeValue: function () {
        var val = this.get_value();
        if (isNaN(val * 1) || Date.isMinDate(val))
            return null;
        else
            return String.format("{0:d2}:{1:d2}", val.getHours(), val.getMinutes());
    },

    set_timeValue: function (value) {
        var e = Function._validateParams(arguments, [
            { name: "value", mayBeNull: true, type: String }
        ]);
        if (e) throw e;
        if (value) {
            var match = value.match(/^(\d{1,2}):(\d{1,2})(:(\d+(\.\d+)?))?$/i);
            if (match) {
                var h = parseInt(match[1]) || 0;
                var m = parseInt(match[2]) || 0;
                var s = parseInt(match[4]) || 0;

                var date = this.get_value();
                if (!(date instanceof Date) || isNaN(date * 1) || Date.isMinDate(date)) {
                    date = new Date();
                }

                date.setHours(h, m, s);

                this.set_value(date);
            }
            else {

            }
        }
    },

    get_asComponent: function () {
        return this._asComponent;
    },

    set_asComponent: function (value) {
        var e = Function._validateParams(arguments, [
            { name: "value", mayBeNull: false, type: Boolean }
        ]);
        if (e) throw e;

        this._asComponent = value || false;

        if (this._innerElementsReady) {
            this._elemInput.readOnly = this._asComponent == true;
        }
    },

    get_asFormControl: function () {
        return this._asFormControl;
    },

    set_asFormControl: function (value) {
        var e = Function._validateParams(arguments, [
            { name: "value", mayBeNull: false, type: Boolean }
        ]);
        if (e) throw e;

        this._asFormControl = value || false;

        if (this._innerElementsReady) {

            this._elemInput.name = this._asFormControl ? this.get_id() : "";
        }
    },

    get_placeHolder: function () {
        return this._placeHolder;
    },

    set_placeHolder: function (value) {
        var e = Function._validateParams(arguments, [
            { name: "value", mayBeNull: true, type: String }
        ]);
        if (e) throw e;

        this._placeHolder = value || "";

        if (this._innerElementsReady) {
            this._elemInput.placeholder = this._placeHolder;
        }
    },

    get_startDateTime: function () {
        return this._startDateTime;
    },

    set_startDateTime: function (value) {
        var e = Function._validateParams(arguments, [
            { name: "value", mayBeNull: false, type: Date }
        ]);

        this._startDateTime = value;

        if (this._dateControlReady) {
            var picker = this.get_jQueryPicker();
            if (picker) {
                picker.setStartDate(value);
            }
        }
    },

    get_endDateTime: function () {
        return this._endDateTime;
    },

    set_endDateTime: function (value) {
        var e = Function._validateParams(arguments, [
            { name: "value", mayBeNull: false, type: Date }
        ]);
        this._endDateTime = value;

        if (this._dateControlReady) {
            var picker = this.get_jQueryPicker();
            if (picker) {
                picker.setEndDate(value);
            }
        }
    },

    get_displayFormat: function () {
        return this._displayFormat;
    },

    set_displayFormat: function (value) {
        var e = Function._validateParams(arguments, [
            { name: "value", mayBeNull: true, type: String }
        ]);
        this._displayFormat = value || "";

        if (this._dateControlReady) {
            //            var picker = this.get_jQueryPicker();
            //            if (picker) {
            //                picker.setEndDate(value);
            //            }
        }
    },

    get_firstDayOfWeek: function () {
        return this._firstDayOfWeek;
    },

    set_firstDayOfWeek: function (value) {
        var e = Function._validateParams(arguments, [
            { name: "value", mayBeNull: false, Integer: true }
        ]);

        if (e == null)
            if (value < 0 || value > 6)
                e = Error.argumentOutOfRange(paramName, param, Sys.Res.argumentOutOfRange);

        if (e) throw e;

        this._firstDayOfWeek = value;

        if (this._dateControlReady) {
            //            var picker = this.get_jQueryPicker();
            //            if (picker) {
            //                picker.setEndDate(value);
            //            }
        }
    },

    get_dayOfToday: function () {
        return this._dayOfToday;
    },

    set_dayOfToday: function (value) {
        var e = Function._validateParams(arguments, [
            { name: "value", mayBeNull: false, type: Date }
        ]);

        if (e) throw e;

        this._dayOfToday = value;

        if (this._dateControlReady) {
            //            var picker = this.get_jQueryPicker();
            //            if (picker) {
            //                picker.setEndDate(value);
            //            }
        }
    },

    get_readOnly: function () {
        return this._readOnly;
    },

    set_readOnly: function (value) {
        this._readOnly = !!value;

        if (this._dateControlReady) {
            var picker = this.get_jQueryPicker();
            if (value)
                picker.disable();
            else
                picker.enable();
        }
    },

    get_enabled: function () {
        return this._enabled;
    },

    set_enabled: function (value) {
        this._enabled = !value;

        if (this._dateControlReady) {
            var picker = this.get_jQueryPicker();
            if (!value)
                picker.disable();
            else
                picker.enable();
        }
    },

    get_autoComplete: function () {

        return this._autoComplete;
    },

    set_autoComplete: function (value) {
        this._autoComplete = value;
    },

    get_jQueryPicker: function () {
        return jQuery(this.get_element()).data('DateTimePicker');
    },

    get_pickerInData: function () {
        return $(this.get_element()).data("DateTimePicker");
    },

    // 华丽的结束--------------------------------属性-------------------------------------------------------------------------

    // 华丽的开始--------------------------------事件-------------------------------------------------------------------------

    add_onClientSelectionChanged: function (handler) {
        this.get_events().addHandler("onClientSelectionChanged", handler);
    },

    remove_onClientSelectionChanged: function (handler) {
        this.get_events().addHandler("onClientSelectionChanged", handler);
    },

    raise_onClientSelectionChanged: function (eventArgs) {
        var handlers = this.get_events().getHandler("onClientSelectionChanged");
        if (handlers) {
            handlers(this, eventArgs);
        }
    },

    add_onClientValueChanged: function (handler) {
        this.get_events().addHandler("onClientValueChanged", handler);
    },

    remove_onClientValueChanged: function (handler) {
        this.get_events().removeHandler("onClientValueChanged", handler);
    },

    raise_onClientValueChanged: function (eventArgs) {
        var handlers = this.get_events().getHandler("onClientValueChanged");
        if (handlers) {
            handlers(this, eventArgs);
        }
    },

    add_onClientErrorDate: function (handler) {
        this.get_events().addHandler("onClientErrorDate", handler);
    },

    remove_onClientErrorDate: function (handler) {
        this.get_events().removeHandler("onClientErrorDate", handler);
    },

    raise_onClientErrorDate: function (eventArgs) {
        var handlers = this.get_events().getHandler("onClientErrorDate");
        if (handlers) {
            handlers(this, eventArgs);
        }
    },
    // 华丽的结束--------------------------------事件-------------------------------------------------------------------------

    //********
    dispose: function () {
        $HGRootNS.DateTimePicker.callBaseMethod(this, 'dispose');
        this._destroyControlElements();
    },

    loadClientState: function (value) {
        if (value) {
            var obj = Sys.Serialization.JavaScriptSerializer.deserialize(value);
            this.set_mode(obj.Mode);
            this.set_value(obj.Value);
        }
    },

    saveClientState: function () {
        var obj = Sys.Serialization.JavaScriptSerializer.serialize({ Mode: this.get_mode(), Value: this.get_value() });
        return obj;
    },

    formatDate: function (date) {
        if (Date.isMinDate(date) || isNaN(date * 1)) {
            return "";
        } else {
            var y = date.getFullYear();
            var m = date.getMonth() + 1;
            var d = date.getDate();
            var h = date.getHours();
            var mm = date.getMinutes();
            var ss = date.getSeconds();

            return String.format("{0:d2}-{1:d2}-{2:d2} {3:d2}:{4:d2}:{5:d2}", y, m, d, h, mm, ss);
        }
    },

    _toPickerDate: function (date) {
        if (date) {
            if ((isNaN(date * 1) || Date.isMinDate(date)) == false) {
                return date;
            }
        }

        return null;
    },

    setDisabledDates: function (dates) {
        this.get_jQueryPicker().options.disabledDates = Array.clone(dates);
    }
}

$HGRootNS.DateTimePicker.registerClass($HGRootNSName + ".DateTimePicker", $HGRootNS.ControlBase);

$HGRootNS.DateTimePicker.cultureInitialized = false;