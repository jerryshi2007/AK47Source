
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	DeluxeCalendar.js
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    吴伟	    20070430		创建
// -------------------------------------------------


$HGRootNS.DeluxeCalendar = function (element) {
	/// <summary>
	/// A behavior that attaches a calendar date selector to a textbox
	/// </summmary>
	/// <param name="element" type="Sys.UI.DomElement">The element to attach to</param>
	this.ReadOnly = false;
	this.DateValue = null;
	this._isInsideImg = false;
	///<summary>
	/// mask part
	/// </summmary>
	this._imgWarp = null;
	this._IsValidValue = true;
	this._MaskedEditButtonID = "";
	this._imageButton = null;

	//set format 9999-99-99
	this._Mask = "9999-99-99";

	this._Filtered = "";
	//input format prompt symbol
	this._PromptChar = "_";

	// Message
	//this._MessageValidatorTip = true;
	// AutoComplete
	this._AutoComplete = true;
	this._AutoCompleteValue = "";
	//behavior
	this._ClearTextOnInvalid = false;
	this._ClearMaskOnLostfocus = true;

	// CSS
	this._OnFocusCssClass = "MaskedEditFocus";
	this._OnInvalidCssClass = "MaskedEditError";

	// globalization 
	// this._CultureName = "zh-CN";
	// globalization Hidden 
	this._CultureTimePlaceholder = "-";

	// **************************************************
	// local var mask valid
	// **************************************************
	//  9 = only numeric

	this._MaskConv = "";
	// **************************************************
	// Others local Var
	// **************************************************
	// save the Direction selected Text (only for ie)
	this._DirectSelText = "";
	// save the initial value for verify changed
	this._initialvalue = "";

	this._LogicSymbol = "";
	// save logic mask with text input
	this._LogicTextMask = "";
	// save logic mask without text
	this._LogicMask = "";
	// save logic mask without text and without escape
	this._LogicMaskConv = "";
	// ID prompt char pane
	this._LogicPrompt = String.fromCharCode(1);
	// ID escape char space
	this._LogicEscape = String.fromCharCode(2);
	// first valid position
	this._LogicFirstPos = -1;
	// Last valid position 
	this._LogicLastPos = -1;
	// Qtd Valid input Position
	this._QtdValidInput = 0;
	// Flag to validate in lost focus not duplicate clearMask execute
	this._InLostfocus = false;

	// Save local Current MessageError
	//set erro massage
	this._CurrentMessageError = "";

	this._charNumbers = "0123456789";

	/// <param name="element" type="Sys.UI.DomElement">The element to maskededit</param>

	$HGRootNS.DeluxeCalendar.initializeBase(this, [element]);

	this._isOnlyCurrentMonth = true;
	this._selectMonthList = null;
	this._isComplexHeader = true;
	this._format = "d";
	this._cssClass = "ajax__calendar";
	this._enabled = true;
	this._animated = true;

	this._layoutRequested = 0;
	this._layoutSuspended = false;

	this._selectedDate = null;
	this._visibleDate = null;
	this._todaysDate = null;
	this._firstDayOfWeek = $HGRootNS.FirstDayOfWeek.Default;

	this._padding = null;

	this._popupDiv = null;
	this._prevArrow = null;
	this._prevArrowImage = null;
	this._nextArrow = null;
	this._nextArrowImage = null;
	this._title = null;

	//add a select,input,img
	this._titleselect = null;
	this._titleinput = null;
	this._titleimg = null;
	//
	this._today = null;
	this._daysRow = null;
	this._monthsRow = null;
	this._yearsRow = null;
	this._daysBody = null;
	this._monthsBody = null;
	this._yearsBody = null;
	this._button = null;

	this._calendarPopup = null;
	this._modeChangeAnimation = null;
	this._modeChangeMoveTopOrLeftAnimation = null;
	this._modeChangeMoveBottomOrRightAnimation = null;
	this._mode = "days";
	this._selectedDateChanging = false;
	this._isOpen = false;
	this._isAnimating = false;
	this._width = 170;
	this._height = 139;
	this._modes = { "days": null, "months": null, "years": null };
	this._modeOrder = { "days": 0, "months": 1, "years": 2 };

	this._value = null;  //add by wuwei 08.11.12与服务器端的value对应

	// Safari needs a longer delay in order to work properly
	this._blur = new $HGRootNS.DeferredOperation(((Sys.Browser.agent === Sys.Browser.Safari) ? 1000 : 1), this, this._onBlur);
	this._focus = new $HGRootNS.DeferredOperation(((Sys.Browser.agent === Sys.Browser.Safari) ? 1000 : 1), this, this._onFocus);

	this._element$delegates = {
		keypress: Function.createDelegate(this, this._onKeyPress),  //textbox add maskededit  
		focus: Function.createDelegate(this, this._onFocus),
		keydown: Function.createDelegate(this, this._onKeyPressdown),
		blur: Function.createDelegate(this, this._onBlur)
	}

	//add list event
	this._select$delegates = {
		change: Function.createDelegate(this, this._selectOnSelect)
	}

	this._yearSelect$delegates = {
		change: Function.createDelegate(this, this._yearSelectOnSelect)
	}

	this._img$delegates = {
		click: Function.createDelegate(this, this._imgOnClick)
	}
	this._cell$delegates = {
		mouseover: Function.createDelegate(this, this._cellOnMouseOver),
		mouseout: Function.createDelegate(this, this._cellOnMouseOut),
		click: Function.createDelegate(this, this._cellOnClick)
	}

	this._loaded = false;

	this._placeholderID = "";

	this._applicationLoad$delegate = Function.createDelegate(this, this._applicationLoad);

	this._startYear;
	this._endYear;
}

$HGRootNS.DeluxeCalendar.prototype = {

	//为在客户端添加新的DeluxeCalendar（new 出一个新的实例） 时做的重载
	//参数preLoadDeluxeCalendarID为预先下去的一个DeluxeCalendar
	clientInitialize: function (preLoadDeluxeCalendarID) {
		this._applicationLoad();
		this.initialize();
		this._imageButton.src = $find(preLoadDeluxeCalendarID).get_imageButtonPath();
		this._imageButton.style["cursor"] = "pointer";
	},

	_applicationLoad: function () {
		if (this._loaded == false) {
			var elt = this.get_element();

			// if this textbox is focused initially
			var hasInitialFocus = false;

			//only for ie , for firefox see keydown
			try {
				if (document.activeElement) {
					if (elt.id == document.activeElement.id) {
						hasInitialFocus = true;
					}
				}
			}
			catch (e) {
			}

			if (hasInitialFocus) {
				//this._onFocus(); //set textbox's focus
			}
			else if (elt.value != "") {
				this._initValue();
				if (this._ClearMaskOnLostfocus) {
					elt.value = (this._getClearMask(elt.value));
				}
			}
			this._loaded = true;
		}
	},

	initialize: function () {
		/// <summary>
		/// Initializes the components and parameters for this behavior
		/// </summary>

		$HGRootNS.DeluxeCalendar.callBaseMethod(this, "initialize");

		//aways bind this event delegates 
		var elt = this.get_element();
		$addHandlers(elt, this._element$delegates);

		$addCssClass(elt, "ajax__calendar_textbox");

		this._imageButton = $get(this._MaskedEditButtonID);

		if (this._MaskedEditButtonID == "" || this._MaskedEditButtonID == null || typeof (this._MaskedEditButtonID) == "undefined" || $get(this._MaskedEditButtonID) == null) {
			this._imageButton = $HGDomElement.createElementFromTemplate(
						{
							nodeName: "img",
							properties: {
								src: this._imageButtonPath,
								id: elt.id + "_image",
								align: "absmiddle",
								tabindex: "-1",
								style: { cursor: "pointer" }
							},
							cssClasses: ["ajax_calendarimagebutton"]
						}//,
					);
			elt.parentElement.insertBefore(this._imageButton, $get(this.get_placeholderID()));
		}

		this.set_readOnly(this.get_readOnly());

		// valid PromptChar length
		if (this._PromptChar.length > 1) {
			this.get_element().value = "掩码必须是一个字符!";
			this.get_element().disabled = "disabled";
			this._imageButton.disabled = "disabled";

			return;
		}

		//valid CultureTimePlaceholder length
		if (this._CultureTimePlaceholder.length > 1) {
			this.get_element().value = "分隔符必须是一个字符!";
			this.get_element().disabled = "disabled";
			this._imageButton.disabled = "disabled";

			return;
		}

		//AutoCompleteValue
		if (this._AutoCompleteValue != "") {
			var autoValueArray = this._AutoCompleteValue.split("-");
			if (autoValueArray.length != 3) {
				this.get_element().value = "默认自动填充的格式如：2008-08-18!";
				this.get_element().disabled = "disabled";
				this._imageButton.disabled = "disabled";

				return;
			}

			if (autoValueArray[0].length == 4 && autoValueArray[1].length == 2 && autoValueArray[2].length == 2) {
				for (var i = 0; i < 3; i++) {
					for (var j = 0; j < autoValueArray[i].length; j++) {
						if (this._charNumbers.indexOf(autoValueArray[i].substring(j, j + 1)) == -1) {
							this.get_element().value = "自动填充的的值必须是数字!";
							this.get_element().disabled = "disabled";
							this._imageButton.disabled = "disabled";

							return;
						}
					}
				}
			}
			else {
				this.get_element().value = "自动填充值格式错误!";
				this.get_element().disabled = "disabled";
				this._imageButton.disabled = "disabled";

				return;
			}
		}

		this._InLostfocus = true;

		Sys.Application.add_load(this._applicationLoad$delegate);

		//$HGDomElement.setDynamicLocation(elt, this._imageButton, this._setImageButtonLocation);
		//add by wuwei
		$addHandler(this._imageButton, "blur", Function.createDelegate(this, this._pickerOnBlur));
		$addHandler(this._imageButton, "beforeactivate", Function.createDelegate(this, this._beforePickerFocus));
		$addHandler(this._imageButton, "click", Function.createDelegate(this, this._buttonOnClick));

		this._modeChangeMoveTopOrLeftAnimation = new $HGRootNS.Animation.LengthAnimation(null, null, null, "style", null, 0, 0, "px");
		this._modeChangeMoveBottomOrRightAnimation = new $HGRootNS.Animation.LengthAnimation(null, null, null, "style", null, 0, 0, "px");
		this._modeChangeAnimation = new $HGRootNS.Animation.ParallelAnimation(null, .25, null, [this._modeChangeMoveTopOrLeftAnimation, this._modeChangeMoveBottomOrRightAnimation]);

		//add by wuwei 08.11.12  设置value的服务器端值
		this._value = this.get_value();
	},

	get_placeholderID: function () {
		return this._placeholderID;
	},
	set_placeholderID: function (value) {
		this._placeholderID = value;
	},

	//add by wuwei
	get_DateValue: function () {
		var result = Date.minDate;

		if (this.get_element().value != "") {
			var date = new Date(Date.parse(this.get_element().value.replace(/-/g, "/")));

			if (isNaN(date) == false)
				result = date;
		}

		return result;
	},
	set_DateValue: function (value) {
		if (this.DateValue != value) {
			if (typeof (value) != "object") {
				if (isNaN(Date.parse(value)) == false)
					this.DateValue = value;
				else
					this.DateValue = Date.minDate;
			}
			else {
				this.DateValue = value;
			}
			this._value = this.DateValue;
			var text = this._simpleFormat(this.DateValue);

			this.get_element().value = text;
			this.raisePropertyChanged("DateValue");
		}
	},

	get_readOnly: function () {
		return this.get_ReadOnly();
	},
	set_readOnly: function (value) {
		this.set_ReadOnly(value);
	},

	//Keep compatible
	get_ReadOnly: function () {
		return this.ReadOnly;
	},
	set_ReadOnly: function (value) {
		this.ReadOnly = value;
		this.get_element().readOnly = value;

		var button = this._imageButton;

		if (button) {
			if (value)
				button.style.display = "none";
			else
				button.style.display = "inline";

			button.disabled = this.get_element().disabled;
		}
	},

	get_isOnlyCurrentMonth: function () {
		return this._isOnlyCurrentMonth;
	},
	set_isOnlyCurrentMonth: function (value) {
		this._isOnlyCurrentMonth = value;
	},

	//add custom header
	get_isComplexHeader: function () {
		/// <summary>
		/// Whether changing modes is animated
		/// </summary>
		/// <value type="Boolean" />

		return this._isComplexHeader;
	},
	set_isComplexHeader: function (value) {
		if (this._isComplexHeader != value) {
			this._isComplexHeader = value;
			this.raisePropertyChanged("isComplexHeader");
		}
	},

	get_enabled: function () {
		/// <value type="Boolean">
		/// Whether this behavior is available for the current element
		/// </value>

		return this._enabled;
	},
	set_enabled: function (value) {
		if (this._enabled != value) {
			this._enabled = value;
			this.raisePropertyChanged("enabled");
		}
	},

	get_animated: function () {
		/// <summary>
		/// Whether changing modes is animated
		/// </summary>
		/// <value type="Boolean" />

		return this._animated;
	},
	set_animated: function (value) {
		if (this._animated != value) {
			this._animated = value;
			this.raisePropertyChanged("animated");
		}
	},

	get_format: function () {
		/// <value type="String">
		/// The format to use for the date value
		/// </value>

		return this._format;
	},
	set_format: function (value) {
		if (this._format != value) {
			this._format = value;
			this.raisePropertyChanged("format");
		}
	},

	get_selectedDate: function () {
		/// <value type="Date">
		/// The date value represented by the text box
		/// </value>
		/*
		if (this._selectedDate == null) {
		var elt = this.get_element();
		if (elt.value) {
		this._selectedDate = this._parseTextValue(elt.value);
		}
		}*/
		this._selectedDate = this.get_value();

		return this._selectedDate;
	},

	_setSelectedDate: function (value) {
		var elt = this.get_element();
		if (this._selectedDate != value) {
			this._selectedDate = value;

			this._selectedDateChanging = true;
			var text = "";
			if (value) {
				text = this._simpleFormat(value);
			}
			if (text != elt.value) {
				elt.value = text;
				//add by wuwei 08.11.12  在选择完日期，设置了文本框的值后，重置value值
				this._value = this.get_value();
				//this._fireChanged();
				this.raiseOnClientDateSelectionChanged();
				this.raiseClientValueChanged();
			}
			this.DateValue = value;
			this._selectedDateChanging = false;
			this.invalidate();
			this.raisePropertyChanged("selectedDate");
		}
	},

	get_value: function () {
		//modify by wuwei 08.11.12
		this._value = this.get_DateValue(); //this.get_selectedDate();
		return this._value;
	},
	set_value: function (value) {
		this.set_DateValue(value);
	},

	get_visibleDate: function () {
		/// <summary>
		/// The date currently visible in the calendar
		/// </summary>
		/// <value type="Date" />

		return this._visibleDate;
	},

	set_visibleDate: function (value) {
		if (value) value = value.getDateOnly();
		if (this._visibleDate != value) {
			this._switchMonth(value, !this._isOpen);
			this.raisePropertyChanged("visibleDate");
		}
	},

	get_todaysDate: function () {
		/// <value type="Date">
		/// The date to use for "Today"
		/// </value>

		if (this._todaysDate != null) {
			return this._todaysDate;
		}
		return new Date().getDateOnly();
	},

	set_todaysDate: function (value) {
		if (value) value = value.getDateOnly();
		if (this._todaysDate != value) {
			this._todaysDate = value;
			this.invalidate();
			this.raisePropertyChanged("todaysDate");
		}
	},

	get_firstDayOfWeek: function () {
		/// <value type=$HGRootNSName + ".FirstDayOfWeek">
		/// The day of the week to appear as the first day in the calendar
		/// </value>

		return this._firstDayOfWeek;
	},
	set_firstDayOfWeek: function (value) {
		if (this._firstDayOfWeek != value) {
			this._firstDayOfWeek = value;
			this.invalidate();
			this.raisePropertyChanged("firstDayOfWeek");
		}
	},

	get_cssClass: function () {
		/// <value type="Sys.UI.DomElement">
		/// The CSS class selector to use to change the calendar's appearance
		/// </value>

		return this._cssClass;
	},

	set_cssClass: function (value) {
		if (this._cssClass != value) {
			if (this._cssClass && this.get_isInitialized()) {
				Sys.UI.DomElement.removeCssClass(this._container, this._cssClass);
			}
			this._cssClass = value;
			if (this._cssClass && this.get_isInitialized()) {
				Sys.UI.DomElement.addCssClass(this._container, this._cssClass);
			}
			this.raisePropertyChanged("cssClass");
		}
	},

	get_startYear: function () {
		return this._startYear;
	},

	set_startYear: function (value) {
		this._startYear = value;
	},

	get_endYear: function () {
		return this._endYear;
	},

	set_endYear: function (value) {
		this._endYear = value;
	},
	/*@@@@@@@@@@@@@@@@@@@@@@@@
	property
	@@@@@@@@@@@@@@@@@@@@@@@@@@*/

	//
	// Helper properties
	//
	get_Mask: function () {
		if (this._MaskConv == "" && this._Mask != "") {
			this._convertMask();
		}
		return this._MaskConv;
	},
	set_Mask: function (value) {
		this._Mask = value;
		this.raisePropertyChanged('Mask');
	},

	get_PromptCharacter: function () {
		return this._PromptChar;
	},
	set_PromptCharacter: function (value) {
		this._PromptChar = value;
		this.raisePropertyChanged('PromptChar');
	},

	get_OnFocusCssClass: function () {
		return this._OnFocusCssClass;
	},
	set_OnFocusCssClass: function (value) {
		this._OnFocusCssClass = value;
		this.raisePropertyChanged('OnFocusCssClass');
	},

	get_OnInvalidCssClass: function () {
		return this._OnInvalidCssClass;
	},

	set_OnInvalidCssClass: function (value) {
		this._OnInvalidCssClass = value;
		this.raisePropertyChanged('OnInvalidCssClass');
	},

	get_CultureTimePlaceholder: function () {
		return this._CultureTimePlaceholder;
	},

	set_CultureTimePlaceholder: function (value) {
		this._CultureTimePlaceholder = value;
		this.raisePropertyChanged('CultureTimePlaceholder');
	},

	get_ClearMaskOnLostFocus: function () {
		return this._ClearMaskOnLostfocus;
	},
	set_ClearMaskOnLostFocus: function (value) {
		this._ClearMaskOnLostfocus = value;
		this.raisePropertyChanged('ClearMaskOnLostfocus');
	},

	get_AutoComplete: function () {
		return this._AutoComplete;
	},
	set_AutoComplete: function (value) {
		this._AutoComplete = value;
		this.raisePropertyChanged('AutoComplete');
	},

	get_AutoCompleteValue: function () {
		return this._AutoCompleteValue;
	},
	set_AutoCompleteValue: function (value) {
		this._AutoCompleteValue = value;
		this.raisePropertyChanged('AutoCompleteValue');
	},

	//component
	get_MaskedEditButtonID: function () {
		return this._MaskedEditButtonID;
	},
	set_MaskedEditButtonID: function (value) {
		this._MaskedEditButtonID = value;
	},

	get_imageButtonPath: function () {
		return this._imageButtonPath;
	},
	set_imageButtonPath: function (value) {
		this._imageButtonPath = value;
	},

	get_CurrentMessageError: function () {
		return decodeURI(this._CurrentMessageError);
	},
	set_CurrentMessageError: function (value) {
		this._CurrentMessageError = encodeURI(value);
	},

	get_IsValidValue: function () {
		return this._IsValidValue;
	},
	set_IsValidValue: function (value) {
		this._IsValidValue = value;
	},

	onBeforeCloneElement: function (sourceElement) {
		$clearHandlers(sourceElement);
	},

	onAfterCloneElement: function (sourceElement, newElement) {
		$addHandlers(sourceElement, this._element$delegates);
	},

	get_cloneableProperties: function () {
		var baseProperties = $HGRootNS.DeluxeCalendar.callBaseMethod(this, "get_cloneableProperties");
		var currentProperties = ["isOnlyCurrentMonth", "cssClass", "readOnly", "PromptCharacter", "AutoComplete", "AutoCompleteValue",
				"IsValidValue", "CurrentMessageError", "OnFocusCssClass", "OnInvalidCssClass", "imageButtonPath", "cValue", "enabled", "animated",
				"isComplexHeader", "firstDayOfWeek", "startYear", "endYear", "value"];

		Array.addRange(currentProperties, baseProperties);

		return currentProperties;
	},
	/**************maskededit property end***************/
	/****************************************************/

	add_onClientShowing: function (handler) {
		/// <summary>
		/// Adds an event handler for the <code>onClientShowing</code> event.
		/// </summary>
		/// <param name="handler" type="Function">
		/// The handler to add to the event.
		/// </param>
		/// <returns />

		this.get_events().addHandler("onClientShowing", handler);
	},

	remove_onClientShowing: function (handler) {
		/// <summary>
		/// Removes an event handler for the <code>onClientShowing</code> event.
		/// </summary>
		/// <param name="handler" type="Function">
		/// The handler to remove from the event.
		/// </param>
		/// <returns />

		this.get_events().removeHandler("onClientShowing", handler);
	},

	raiseOnClientShowing: function () {
		/// <summary>
		/// Raise the <code>onClientShowing</code> event
		/// </summary>
		/// <returns />

		var handlers = this.get_events().getHandler("onClientShowing");
		if (handlers) {
			handlers(this, Sys.EventArgs.Empty);
		}
	},

	add_onClientShown: function (handler) {
		/// <summary>
		/// Adds an event handler for the <code>onClientShown</code> event.
		/// </summary>
		/// <param name="handler" type="Function">
		/// The handler to add to the event.
		/// </param>
		/// <returns />

		this.get_events().addHandler("onClientShown", handler);
	},

	remove_onClientShown: function (handler) {
		/// <summary>
		/// Removes an event handler for the <code>onClientShown</code> event.
		/// </summary>
		/// <param name="handler" type="Function">
		/// The handler to remove from the event.
		/// </param>
		/// <returns />

		this.get_events().removeHandler("onClientShown", handler);
	},

	raiseOnClientShown: function () {
		/// <summary>
		/// Raise the <code>shown</code> event
		/// </summary>
		/// <returns />

		var handlers = this.get_events().getHandler("onClientShown");
		if (handlers) {
			handlers(this, Sys.EventArgs.Empty);
		}
	},

	add_onClientHiding: function (handler) {
		/// <summary>
		/// Adds an event handler for the <code>onClientHiding</code> event.
		/// </summary>
		/// <param name="handler" type="Function">
		/// The handler to add to the event.
		/// </param>
		/// <returns />

		this.get_events().addHandler("onClientHiding", handler);
	},

	remove_onClientHiding: function (handler) {
		/// <summary>
		/// Removes an event handler for the <code>onClientHiding</code> event.
		/// </summary>
		/// <param name="handler" type="Function">
		/// The handler to remove from the event.
		/// </param>
		/// <returns />

		this.get_events().removeHandler("onClientHiding", handler);
	},

	raiseOnClientHiding: function () {
		/// <summary>
		/// Raise the <code>onClientHiding</code> event
		/// </summary>
		/// <returns />

		var handlers = this.get_events().getHandler("onClientHiding");
		if (handlers) {
			handlers(this, Sys.EventArgs.Empty);
		}
	},

	add_onClientHidden: function (handler) {
		/// <summary>
		/// Adds an event handler for the <code>onClientHidden</code> event.
		/// </summary>
		/// <param name="handler" type="Function">
		/// The handler to add to the event.
		/// </param>
		/// <returns />

		this.get_events().addHandler("onClientHidden", handler);
	},

	remove_onClientHidden: function (handler) {
		/// <summary>
		/// Removes an event handler for the <code>onClientHidden</code> event.
		/// </summary>
		/// <param name="handler" type="Function">
		/// The handler to remove from the event.
		/// </param>
		/// <returns />

		this.get_events().removeHandler("onClientHidden", handler);
	},

	raiseOnClientHidden: function () {
		/// <summary>
		/// Raise the <code>onClientHidden</code> event
		/// </summary>
		/// <returns />

		var handlers = this.get_events().getHandler("onClientHidden");
		if (handlers) {
			handlers(this, Sys.EventArgs.Empty);
		}
	},

	add_onClientDateSelectionChanged: function (handler) {
		/// <summary>
		/// Adds an event handler for the <code>onClientDateSelectionChanged</code> event.
		/// </summary>
		/// <param name="handler" type="Function">
		/// The handler to add to the event.
		/// </param>
		/// <returns />

		this.get_events().addHandler("onClientDateSelectionChanged", handler);
	},

	remove_onClientDateSelectionChanged: function (handler) {
		/// <summary>
		/// Removes an event handler for the <code>onClientDateSelectionChanged</code> event.
		/// </summary>
		/// <param name="handler" type="Function">
		/// The handler to remove from the event.
		/// </param>
		/// <returns />

		this.get_events().removeHandler("onClientDateSelectionChanged", handler);
	},

	raiseOnClientDateSelectionChanged: function () {
		/// <summary>
		/// Raise the <code>onClientDateSelectionChanged</code> event
		/// </summary>
		/// <returns />

		var handlers = this.get_events().getHandler("onClientDateSelectionChanged");
		if (handlers) {
			handlers(this, Sys.EventArgs.Empty);
		}
	},

	/**************************************************************/
	/****************maskededit function begin********************/
	/************************************************************/
	_initValue: function () {
		var masktxt = this._createMask();
		var Inipos = this._LogicFirstPos;
		var initValue = "";
		var e = this.get_element();
		this._LogicSymbol = "";

		if (e.value != "" && e.value != masktxt) {
			initValue = e.value;
		}
		e.value = (masktxt);
		if (initValue != "") {
			this._loadValue(initValue, this._LogicFirstPos);
		}

		return Inipos;
	},

	//
	// Set/Remove CssClass
	//
	_addCssClassMaskedEdit: function (CssClass) {
		var e = this.get_element();

		Sys.UI.DomElement.removeCssClass(e, this._OnFocusCssClass);

		Sys.UI.DomElement.removeCssClass(e, this._OnInvalidCssClass);
		if (CssClass != "") {
			Sys.UI.DomElement.addCssClass(e, CssClass);
		}
	},

	//
	// Load initial value in mask
	//
	_loadValue: function (initValue, logicPosition) {
		var oldfocus = this._InLostfocus;
		var i = 0;
		this._InLostfocus = false;
		if (this._ClearMaskOnLostfocus == false) {
			logicPosition = 0;
		}
		for (var i = 0; i < parseInt(initValue.length, 10); i++) {
			var c = initValue.substring(i, i + 1);

			if (this._processKey(logicPosition, c)) {
				this._insertContent(c, logicPosition);
				if (this._ClearMaskOnLostfocus == false) {
					logicPosition = logicPosition + 1;
				}
				else {
					logicPosition = this._getNextPosition(logicPosition + 1);
				}
			}
			else {
				if (this._ClearMaskOnLostfocus == false) {
					logicPosition = logicPosition + 1;
				}
			}
		}
		this._InLostfocus = oldfocus;
	},

	_getNextPosition: function (pos) {
		while (!this._isValidMaskedEditPosition(pos) && pos < this._LogicLastPos + 1) {
			pos++;
		}
		if (pos > this._LogicLastPos + 1) {
			pos = this._LogicLastPos + 1;
		}
		return pos;
	},

	_isValidMaskedEditPosition: function (pos) {
		return (this._LogicMask.substring(pos, pos + 1) == this._LogicPrompt);
	},

	_insertContent: function (value, curpos) {
		//if input in placeholder,then pass
		if (this._Mask.indexOf(this._CultureTimePlaceholder) == curpos || this._Mask.lastIndexOf(this._CultureTimePlaceholder) == curpos)
			return;
		var masktext = this.get_element().value;
		masktext = masktext.substring(0, curpos) + value + masktext.substring(curpos + 1);
		this._LogicTextMask = this._LogicTextMask.substring(0, curpos) + value + this._LogicTextMask.substring(curpos + 1);
		this.get_element().value = (masktext);

	},

	_setSelectionRange: function (selectionStart, selectionEnd) {
		input = this.get_element();
		if (input.createTextRange) {
			var range = input.createTextRange();
			range.collapse(true);
			range.moveEnd('character', selectionEnd);
			range.moveStart('character', selectionStart);
			range.select();
		}
		else if (input._setSelectionRange) {
			input._setSelectionRange(selectionStart, selectionEnd);
		}

	},

	_setCancelEvent: function (evt) {
		if (Sys.Browser.agent == Sys.Browser.InternetExplorer) {
			evt.returnValue = false;
			event.keyCode = "0"; //add by fenglilei
		}
		else {
			if (typeof (evt.returnValue) != "undefined") {
				evt.returnValue = false;
			}
			if (evt.preventDefault) {
				evt.preventDefault();
			}
		}
	},

	_specialNavKey: function (keyCode, navkey) {
		if (Sys.Browser.agent == Sys.Browser.InternetExplorer) {
			return false;
		}
		return (keyCode >= 33 && keyCode <= 45 && navkey);
	},

	_onNavigator: function (scanCode, evt, navkey) {
		if (!navkey) {
			return true;
		}
		var curpos;
		if (this._processDeleteKey(scanCode)) {
			curpos = this._getCurrentPosition();
			this._setCancelEvent(evt);
			return false;
		}
		if ((evt.ctrlKey || evt.altKey || evt.shiftKey || evt.metaKey)) {
			if (scanCode == 39 && evt.ctrlKey) {
				this._DirectSelText = "R";
				curpos = this._getCurrentPosition();
				if (curpos >= this._LogicLastPos + 1) {
					this._setSelectionRange(this._LogicLastPos + 1, this._LogicLastPos + 1);
					this._setCancelEvent(evt);
					return false;
				}
				return true;
			}
			else if (scanCode == 37 && evt.ctrlKey) {
				this._DirectSelText = "L";
				curpos = this._getCurrentPosition();
				if (curpos <= this._LogicFirstPos) {
					this._setSelectionRange(this._LogicFirstPos, this._LogicFirstPos);
					this._setCancelEvent(evt);
					return false;
				}
				return true;
			}
			else if (scanCode == 35 && evt.shiftKey) //END 
			{
				this._DirectSelText = "R";
				curpos = this._getCurrentPosition();
				this._setSelectionRange(curpos, this._LogicLastPos + 1);
				this._setCancelEvent(evt);
				return false;
			}
			else if (scanCode == 36 && evt.shiftKey) //Home 
			{
				this._DirectSelText = "L";
				curpos = this._getCurrentPosition();
				this._setSelectionRange(this._LogicFirstPos, curpos);
				this._setCancelEvent(evt);
				return false;
			}
			else if (scanCode == 35 || scanCode == 34) //END or pgdown
			{
				this._DirectSelText = "R";
				this._setSelectionRange(this._LogicLastPos + 1, this._LogicLastPos + 1);
				this._setCancelEvent(evt);
				return false;
			}
			else if (scanCode == 36 || scanCode == 33) //Home or pgup
			{
				this._DirectSelText = "L";
				this._setSelectionRange(this._LogicFirstPos, this._LogicFirstPos);
				this._setCancelEvent(evt);
				return false;
			}
			return true;
		}
		if (scanCode == 35 || scanCode == 34) {   //END or pgdown

			this._DirectSelText = "R";
			this._setSelectionRange(this._LogicLastPos + 1, this._LogicLastPos + 1);
			this._setCancelEvent(evt);
			return false;
		}
		else if (scanCode == 36 || scanCode == 33) {   //Home or pgup

			this._DirectSelText = "L";
			this._setSelectionRange(this._LogicFirstPos, this._LogicFirstPos);
			this._setCancelEvent(evt);
			return false;
		}
		else if (scanCode == 37) {
			this._DirectSelText = "L";
			curpos = this._getCurrentPosition();
			if (curpos <= this._LogicFirstPos) {
				this._setSelectionRange(this._LogicFirstPos, this._LogicFirstPos);
				this._setCancelEvent(evt);
				return false;
			}
			return true;
		}
		else if (scanCode == 38 || scanCode == 40) {
			this._setCancelEvent(evt);
			return false;
		}
		else if (scanCode == 39) {
			this._DirectSelText = "R";
			curpos = this._getCurrentPosition();
			if (curpos >= this._LogicLastPos + 1) {
				this._setSelectionRange(this._LogicLastPos + 1, this._LogicLastPos + 1);
				this._setCancelEvent(evt);
				return false;
			}
			return true;
		}
		return true;
	},

	//check date
	_captureValidatorsControl: function (str) {
		var ar = str.replace(/\-0/g, "-").split("-");

		ar = new Array(parseInt(ar[0]), parseInt(ar[1]) - 1, parseInt(ar[2]));
		var d = new Date(ar[0], ar[1], ar[2]);

		//in c# year must be 0001-9999 
		var returnValue = ar[0] != 0000 && d.getMonth() == ar[1] && d.getDate() == ar[2];

		if (!returnValue) {
			this.set_CurrentMessageError("输入日期不正确，请重新输入！"); //this.set_CurrentMessageError("Input error, please enter the correct date!"); 
		}
		return returnValue;
	},

	_getClearMask: function (masktext) {
		var i = 0;
		var clearmask = "";
		var qtdok = 0;

		while (i < parseInt(this._LogicTextMask.length, 10)) {
			if (qtdok < this._QtdValidInput) {
				if (this._isValidMaskedEditPosition(i) && this._LogicTextMask.substring(i, i + 1) != this._LogicPrompt) {
					clearmask += this._LogicTextMask.substring(i, i + 1);
					qtdok++;
				}
				else if (this._LogicTextMask.substring(i, i + 1) != this._LogicPrompt && this._LogicTextMask.substring(i, i + 1) != this._LogicEscape) {

					if (this._LogicTextMask.substring(i, i + 1) == this._CultureTimePlaceholder) {
						clearmask += (clearmask == "") ? "" : this._CultureTimePlaceholder;
					}

				}
			}
			i++;
		}
		if (this._LogicSymbol != "" && clearmask != "") {
			clearmask += " " + this._LogicSymbol;
		}

		return clearmask;
	},

	//
	// Convert notation {Number} in PAD's Number
	_convertMask: function () {
		this._MaskConv = "";
		this._MaskConv = this._Mask;

		this._LogicMaskConv = "";
		this._LogicMaskConv = this._MaskConv;

	},

	_checkArgsEvents: function (args) {
		var ret = null;
		if (typeof (args) != "undefined" && args != null && typeof (args.rawEvent) != "undefined") {
			ret = args.rawEvent;
		}
		return ret;
	},

	// create mask empty , logic mask empty
	// convert escape code and Placeholder to culture
	//when clearing show format __:__:__
	_createMask: function () {
		var text;
		if (this._MaskConv == "" && this._Mask != "") {
			//set splite format 99:99:99
			this._convertMask();
		}
		text = this._MaskConv;
		var i = 0;
		var masktext = "";
		var flagescape = false;
		this._LogicTextMask = "";
		this._QtdValidInput = 0;
		while (i < parseInt(text.length, 10)) {
			this._QtdValidInput++;
			if (text.substring(i, i + 1) == "-") {
				masktext += this._CultureTimePlaceholder;
				this._LogicTextMask += this._CultureTimePlaceholder;
			}
			else {
				masktext += this._PromptChar;
				this._LogicTextMask += this._LogicPrompt;
			}
			i++;
		}
		// Set First and last logic position
		this._LogicFirstPos = -1;
		this._LogicLastPos = -1;
		this._LogicMask = this._LogicTextMask;
		for (i = 0; i < parseInt(this._LogicMask.length, 10); i++) {
			if (this._LogicFirstPos == -1 && this._LogicMask.substring(i, i + 1) == this._LogicPrompt) {
				this._LogicFirstPos = i;
			}
			if (this._LogicMask.substring(i, i + 1) == this._LogicPrompt) {
				this._LogicLastPos = i;
			}
		}
		return masktext;
	},

	//
	// delete current Selected
	// return position select or -1 if nothing select
	//
	_deleteTextSelection: function () {
		var masktext = this.get_element().value;
		var input = this.get_element();
		var ret = -1;
		var lenaux = -1;
		var begin = -1;
		if (document.selection) {
			sel = document.selection.createRange();
			//notdo
			if (sel.text != "") {
				var aux = sel.text + String.fromCharCode(3);
				sel.text = aux;
				dummy = input.createTextRange();
				dummy.findText(aux);
				dummy.select();
				begin = input.value.indexOf(aux);
				if (this._DirectSelText == "P") {
					this._DirectSelText = "";
					//ret = begin;   Backspace to delete all
				}
				else {
					ret = begin;
				}
				document.selection.clear();
				lenaux = parseInt(aux.length, 10) - 1;
			}
		}
		//notdo
		else if (input._setSelectionRange) {
			if (input.selectionStart != input.selectionEnd) {
				var ini = parseInt(input.selectionStart, 10);
				var fim = parseInt(input.selectionEnd, 10);
				lenaux = fim - ini;
				begin = input.selectionStart;
				if (this._DirectSelText == "P") {
					this._DirectSelText = "";
					input.selectionEnd = input.selectionStart;
					ret = begin;
				}
				else {

					input.selectionEnd = input.selectionStart;
					ret = begin;
				}
			}
		}
		//notdo
		if (ret != -1) {
			for (i = 0; i < lenaux; i++) {
				if (this._isValidMaskedEditPosition(begin + i)) {
					masktext = masktext.substring(0, begin + i) + this._PromptChar + masktext.substring(begin + i + 1);
					this._LogicTextMask = this._LogicTextMask.substring(0, begin + i) + this._LogicPrompt + this._LogicTextMask.substring(begin + i + 1);
				}
			}
			this.get_element().value = (masktext);

		}
		return ret;
	},

	_processKey: function (poscur, key) {
		var posmask = this._LogicMaskConv;
		//  9 = only numeric

		var filter;

		filter = this._charNumbers;

		return (filter.indexOf(key) != -1);
	},
	//
	// Previous valid Position
	//
	_getPreviousPosition: function (pos) {
		while (!this._isValidMaskedEditPosition(pos) && pos > this._LogicFirstPos) {
			pos--;
		}
		if (pos < this._LogicFirstPos) {
			pos = this._LogicFirstPos;
		}
		return pos;
	},
	//
	// Current Position
	//
	_getCurrentPosition: function () {
		begin = 0;
		input = this.get_element();
		var masktext = this.get_element().value;
		if (input._setSelectionRange) {
			begin = parseInt(input.selectionStart, 10);
		}
		else if (document.selection) {
			sel = document.selection.createRange();
			if (sel.text != "") {
				var aux = ""
				if (this._DirectSelText == "R") {
					aux = sel.text + String.fromCharCode(3);
				}
				else if (this._DirectSelText == "L") {
					aux = String.fromCharCode(3) + sel.text;
				}
				/*****************add by wuwei, 0724不修改其他，加选中位置的删除*******************/
				//默认选中，从左到右删和从右到左都一样
				aux = sel.text + String.fromCharCode(3);
				sel.text = aux;
				//有选中，就删除，返回0
				var begin = -1;
				var lenaux = -1;

				dummy = input.createTextRange();
				dummy.findText(aux);
				dummy.select();
				begin = input.value.indexOf(aux);

				document.selection.clear();
				lenaux = parseInt(aux.length, 10) - 1;


				for (i = 0; i < lenaux; i++) {
					if (this._isValidMaskedEditPosition(begin + i)) {
						masktext = masktext.substring(0, begin + i) + this._PromptChar + masktext.substring(begin + i + 1);
						this._LogicTextMask = this._LogicTextMask.substring(0, begin + i) + this._LogicPrompt + this._LogicTextMask.substring(begin + i + 1);
					}
				}
				this.get_element().value = (masktext);
				//手动再设置一次
				this._setSelectionRange(begin, begin);
				return 0;
				/********************************************/

				sel.text = aux;
				this._DirectSelText == "";
			}
			else {
				sel.text = String.fromCharCode(3);
				this._DirectSelText == "";
			}
			dummy = input.createTextRange();
			dummy.findText(String.fromCharCode(3));
			dummy.select();
			begin = input.value.indexOf(String.fromCharCode(3));
			document.selection.clear();
		}
		if (begin > this._LogicLastPos + 1) {
			begin = this._LogicLastPos + 1;
		}
		if (begin < this._LogicFirstPos) {
			begin = this._LogicFirstPos;
		}
		return begin;
	},

	_adjustElementTime: function (value, ValueDefault) {
		var emp = true;
		for (i = 0; i < parseInt(value.length, 10); i++) {
			if (value.substring(i, i + 1) != this._PromptChar) {
				emp = false;
			}
		}
		if (emp) {
			return ValueDefault;
		}
		for (i = 0; i < parseInt(value.length, 10); i++) {
			if (value.substring(i, i + 1) == this._PromptChar) {
				value = value.substring(0, i) + "0" + value.substring(i + 1);
			}
		}
		return value;
	},

	_processDeleteKey: function (scanCode) {
		if (scanCode == 46 /*delete*/) {
			var curpos = this._deleteTextSelection();
			if (curpos == -1) {
				curpos = this._getCurrentPosition();

				this._deleteAtPosition(curpos);

			}
			this._setSelectionRange(curpos, curpos);
			return true;
		}
		else if (scanCode == 8 /*back-space*/) {
			var curpos = this._getCurrentPosition();
			if (curpos <= this._LogicFirstPos) {
				return true;
			}

			curpos = this._deleteTextSelection();
			if (curpos == -1) {
				curpos = this._getPreviousPosition(this._getCurrentPosition() - 1);
				this._backspace(curpos);
			}
			this._setSelectionRange(curpos, curpos);
			return true;
		}
		return false;
	},
	//
	// delete at current position
	//
	_deleteAtPosition: function (curpos) {
		var masktext = this.get_element().value;
		if (this._isValidMaskedEditPosition(curpos)) {
			var resttext = masktext.substring(curpos + 1);
			var restlogi = this._LogicTextMask.substring(curpos + 1);
			masktext = masktext.substring(0, curpos) + this._PromptChar;
			this._LogicTextMask = this._LogicTextMask.substring(0, curpos) + this._LogicPrompt;
			// clear rest of mask
			for (i = 0; i < parseInt(resttext.length, 10); i++) {
				if (this._isValidMaskedEditPosition(curpos + 1 + i)) {
					masktext += this._PromptChar;
					this._LogicTextMask += this._LogicPrompt;
				}
				else {
					masktext += resttext.substring(i, i + 1);
					this._LogicTextMask += restlogi.substring(i, i + 1);
				}
			}
			// insert only valid text
			posaux = this._getNextPosition(curpos);
			for (i = 0; i < parseInt(resttext.length, 10); i++) {
				if (this._isValidMaskedEditPosition(curpos + 1 + i) && restlogi.substring(i, i + 1) != this._LogicPrompt) {
					masktext = masktext.substring(0, posaux) + resttext.substring(i, i + 1) + masktext.substring(posaux + 1);
					this._LogicTextMask = this._LogicTextMask.substring(0, posaux) + restlogi.substring(i, i + 1) + this._LogicTextMask.substring(posaux + 1);
					posaux = this._getNextPosition(posaux + 1);
				}
			}
			this.get_element().value = (masktext);
		}
	},
	//
	// this._backspace at current position
	//
	_backspace: function (curpos) {
		var masktext = this.get_element().value;
		if (this._isValidMaskedEditPosition(curpos)) {
			masktext = masktext.substring(0, curpos) + this._PromptChar + masktext.substring(curpos + 1);
			this._LogicTextMask = this._LogicTextMask.substring(0, curpos) + this._LogicPrompt + this._LogicTextMask.substring(curpos + 1);
			this.get_element().value = (masktext);
		}
	},
	//
	// delete current Selected
	// return position select or -1 if nothing select
	//
	_onKeyPressdown: function (args) {
		var evt = this._checkArgsEvents(args);
		if (evt == null) {
			return;
		}
		if (this.get_element().readOnly) {
			this._setCancelEvent(evt);
			return;
		}
		//enter
		if (evt.keyCode == 13) {
			return false;
		}

		// FOR FIREFOX (NOT IMPLEMENT document.activeElement)
		if (Sys.Browser.agent != Sys.Browser.InternetExplorer) {
			if (this._InLostfocus) {
				this._onFocus(evt);
			}
		}
		if (Sys.Browser.agent == Sys.Browser.InternetExplorer) {
			// IE
			var scanCode = evt.keyCode;
			this._onNavigator(scanCode, evt, true)
		}
	},

	/********************************************************************/
	/********************masked edit function end************************/
	/********************************************************************/
	dispose: function () {
		/// <summary>
		/// Disposes this behavior's resources
		/// </summary>

		if (this._calendarPopup) {
			this._calendarPopup.dispose();
			this._calendarPopup = null;
		}
		this._modes = null;
		this._modeOrder = null;
		if (this._modeChangeMoveTopOrLeftAnimation) {
			this._modeChangeMoveTopOrLeftAnimation.dispose();
			this._modeChangeMoveTopOrLeftAnimation = null;
		}
		if (this._modeChangeMoveBottomOrRightAnimation) {
			this._modeChangeMoveBottomOrRightAnimation.dispose();
			this._modeChangeMoveBottomOrRightAnimation = null;
		}
		if (this._modeChangeAnimation) {
			this._modeChangeAnimation.dispose();
			this._modeChangeAnimation = null;
		}


		if (this._selectMonthList) {
			$HGDomEvent.removeHandlers(this._selectMonthList, this._select$delegates);
			this._selectMonthList = null;
		}
		if (this._titleimg) {
			$HGDomEvent.removeHandlers(this._titleimg, this._img$delegates);
			this._titleimg = null;
		}
		//
		if (this._popupDiv) {
			$HGDomEvent.removeHandlers(this._popupDiv, this._popup$delegates);
			this._popupDiv = null;
		}
		if (this._prevArrow) {
			$HGDomEvent.removeHandlers(this._prevArrow, this._cell$delegates);
			this._prevArrow = null;
		}
		if (this._prevArrowImage) {
			$HGDomEvent.removeHandlers(this._prevArrowImage, this._cell$delegates);
			this._prevArrowImage = null;
		}
		if (this._nextArrow) {
			$HGDomEvent.removeHandlers(this._nextArrow, this._cell$delegates);
			this._nextArrow = null;
		}
		if (this._nextArrowImage) {
			$HGDomEvent.removeHandlers(this._nextArrowImage, this._cell$delegates);
			this._nextArrowImage = null;
		}
		if (this._title) {
			$HGDomEvent.removeHandlers(this._title, this._cell$delegates);
			this._title = null;
		}
		if (this._today) {
			$HGDomEvent.removeHandlers(this._today, this._cell$delegates);
			this._today = null;
		}
		if (this._daysRow) {
			for (var i = 0; i < this._daysBody.rows.length; i++) {
				var row = this._daysBody.rows[i];
				for (var j = 0; j < row.cells.length; j++) {
					$HGDomEvent.removeHandlers(row.cells[j].firstChild, this._cell$delegates);
				}
			}
			this._daysRow = null;
		}
		if (this._monthsRow) {
			for (var i = 0; i < this._monthsBody.rows.length; i++) {
				var row = this._monthsBody.rows[i];
				for (var j = 0; j < row.cells.length; j++) {
					$HGDomEvent.removeHandlers(row.cells[j].firstChild, this._cell$delegates);
				}
			}
			this._monthsRow = null;
		}
		if (this._yearsRow) {
			for (var i = 0; i < this._yearsBody.rows.length; i++) {
				var row = this._yearsBody.rows[i];
				for (var j = 0; j < row.cells.length; j++) {
					$HGDomEvent.removeHandlers(row.cells[j].firstChild, this._cell$delegates);
				}
			}
			this._yearsRow = null;
		}

		//aways has this._element$delegates
		var elt = this.get_element();
		$HGDomEvent.removeHandlers(elt, this._element$delegates);

		if (this._applicationLoad$delegate) {
			Sys.Application.remove_load(this._applicationLoad$delegate);
			this._applicationLoad$delegate = null;
		}

		$HGRootNS.DeluxeCalendar.callBaseMethod(this, "dispose");
	},

	show: function () {
		/// <summary>
		/// Shows the calendar
		/// </summary>

		this._ensureCalendar();

		if (!this._isOpen) {
			this.raiseOnClientShowing();
			this._isOpen = true;
			this._switchMonth(Date.isMinDate(this.get_value()) ? new Date() : this.get_value(), true);
			if ((this._calendarPopup.get_positionAndSize().Size.height + $HGDomElement.getLocation(this._calendarPopup._positionElement).y) > this.get_documentHeight()) {
				this._calendarPopup.set_positioningMode($HGRootNS.PositioningMode.TopLeft);
			}
			if ((this._calendarPopup.get_positionAndSize().Size.width + $HGDomElement.getLocation(this._calendarPopup._positionElement).x) > this.get_documentWidth()) {
				this._calendarPopup.set_positioningMode($HGRootNS.PositioningMode.BottomRight);
			}

			this._calendarPopup.showByPosition();
			//this._calendarPopup.show();

			this.raiseOnClientShown();
		}
	},
	get_documentWidth: function () {
		var w = document.documentElement.clientWidth;

		if (w == 0)
			w = document.body.offsetWidth;

		return w;
	},

	get_documentHeight: function () {
		var h = document.documentElement.clientHeight;

		if (h == 0)
			h = document.body.offsetHeight;

		return h;
	},

	hide: function () {
		/// <summary>
		/// Hides the calendar
		/// </summary>
		this.raiseOnClientHiding();
		if (this._container) {
			this._calendarPopup.hide();
			this._switchMode("days", true);
		}
		this._isOpen = false;
		this.raiseOnClientHidden();
	},

	suspendLayout: function () {
		/// <summary>
		/// Suspends layout of the behavior while setting properties
		/// </summary>

		this._layoutSuspended++;
	},

	invalidate: function () {
		/// <summary>
		/// Performs layout of the behavior unless layout is suspended
		/// </summary>

		if (this._layoutSuspended > 0) {
			this._layoutRequested = true;
		} else {
			this._performLayout();
		}
	},

	_buildCalendar: function () {
		/// <summary>
		/// Builds the calendar's layout
		/// </summary>

		var elt = this.get_element();

		this._container = $HGDomElement.createElementFromTemplate({
			nodeName: "div",
			cssClasses: [this._cssClass]
		}, this._calendarPopup.get_popupBody(), null, this._calendarPopup.get_popupDocument());

		this._popupDiv = $HGDomElement.createElementFromTemplate({
			nodeName: "div",
			events: this._popup$delegates,
			properties: {
				// tabIndex : 0
			},
			cssClasses: ["ajax__calendar_container"],
			visible: true
		}, this._container);
	},

	_buildHeader: function () {
		/// <summary>
		/// Builds the header for the calendar
		/// </summary>

		this._header = $HGDomElement.createElementFromTemplate({
			nodeName: "div",
			cssClasses: ["ajax__calendar_header"]
		}, this._popupDiv);

		//$HGDomElement.createElementFromTemplate({ nodeName : "span", properties : { innerText : "2b "}}, this._header);   

		if (!this._isComplexHeader) {       //不是复合的头，只显示左右的
			var prevArrowWrapper = $HGDomElement.createElementFromTemplate({ nodeName: "span" }, this._header);
			this._prevArrow = $HGDomElement.createElementFromTemplate({
				nodeName: "span",
				properties: { mode: "prev" },
				events: this._cell$delegates,
				cssClasses: ["ajax__calendar_prev"]
			}, prevArrowWrapper);

			var nextArrowWrapper = $HGDomElement.createElementFromTemplate({ nodeName: "span" }, this._header);
			this._nextArrow = $HGDomElement.createElementFromTemplate({
				nodeName: "span",
				properties: { mode: "next" },
				events: this._cell$delegates,
				cssClasses: ["ajax__calendar_next"]
			}, nextArrowWrapper);
		}
		else {  //只显示复合的头，填充右边部分
			//?????1755
			//add year
			var clickYearInput = $HGDomElement.createElementFromTemplate({ nodeName: "span", properties: {
			}, events: this._click$delegates
			}, this._header);
			//        this._titleinput = $HGDomElement.createElementFromTemplate({
			//            nodeName: "select",
			//            //events: this._cell$delegates,
			//            cssClasses: ["ajax__calendar_titleinput"]
			//        }, this._header);

			this._titleinput = $HGDomElement.createElementFromTemplate(
        {
        	nodeName: "select",
        	properties:
            {

            },
        	events: this._yearSelect$delegates,
        	cssClasses: ["ajax__calendar_yearselect"]
        },
        this._header);
			//clickYearInput.innerHTML = "<input type='text'>";
			//        var clickYearImg = $HGDomElement.createElementFromTemplate({ nodeName: "span" }, this._header);
			//        this._titleimg = $HGDomElement.createElementFromTemplate({
			//            nodeName: "span", properties: { mode: "next" },
			//            events: this._img$delegates, cssClasses: ["ajax__calendar_titleimg"]
			//        }, clickYearImg);
			//        //?????             
			var titlePadding = $HGDomElement.createElementFromTemplate({ nodeName: "span" }, this._header);
			this._padding = $HGDomElement.createElementFromTemplate({
				nodeName: "span",
				cssClasses: ["ajax__calendar_padding"]
			}, titlePadding);

			//add list
			this._selectMonthList = $HGDomElement.createElementFromTemplate({ nodeName: "select", properties: {
			}, events: this._select$delegates, cssClasses: ["ajax__calendar_titleselect"]
			}, this._header);
			var startYear = this.get_startYear();
			var endYear = this.get_endYear();
			startYear = startYear ? startYear : 1900;
			endYear = endYear ? endYear : 2500;

			for (var iYear = startYear; iYear <= endYear; iYear++) {
				var optionYear = $HGDomElement.createElementFromTemplate(
				{
					nodeName: "option"
				},
				this._titleinput);
				optionYear.value = iYear;
				optionYear.text = iYear;
			}

			for (var scount = 0; scount < 12; scount++) {
				var option = $HGDomElement.createElementFromTemplate(
				{
					nodeName: "option"
				},
				this._selectMonthList);
				option.value = scount;
				option.text = scount + 1;
			}
		}
		var titleWrapper = $HGDomElement.createElementFromTemplate({ nodeName: "span" }, this._header);
		this._title = $HGDomElement.createElementFromTemplate({
			nodeName: "span",
			properties: { mode: "title" },
			events: this._cell$delegates,
			cssClasses: ["ajax__calendar_title"]
		}, titleWrapper);
	},

	_buildBody: function () {
		/// <summary>
		/// Builds the body region for the calendar
		/// </summary>

		this._body = $HGDomElement.createElementFromTemplate({
			nodeName: "div",
			cssClasses: ["ajax__calendar_body"]
		}, this._popupDiv);

		this._buildDays();
		this._buildMonths();
		this._buildYears();
	},

	_buildFooter: function () {
		/// <summary>
		/// Builds the footer for the calendar
		/// </summary>

		var todayWrapper = $HGDomElement.createElementFromTemplate({ nodeName: "div" }, this._popupDiv);
		this._today = $HGDomElement.createElementFromTemplate({
			nodeName: "div",
			properties: { mode: "today" },
			events: this._cell$delegates,
			cssClasses: ["ajax__calendar_footer", "ajax__calendar_today"]
		}, todayWrapper);
	},

	_buildDays: function () {
		/// <summary>
		/// Builds a "days of the month" view for the calendar
		/// </summary>

		var dtf = Sys.CultureInfo.CurrentCulture.dateTimeFormat;

		this._days = $HGDomElement.createElementFromTemplate({
			nodeName: "div",
			cssClasses: ["ajax__calendar_days"]
		}, this._body);
		this._modes["days"] = this._days;

		this._daysTable = $HGDomElement.createElementFromTemplate({
			nodeName: "table",
			properties: {
				cellPadding: 0,
				cellSpacing: 0,
				border: 0,
				style: { margin: "auto" }
			}
		}, this._days);

		this._daysTableHeader = $HGDomElement.createElementFromTemplate({ nodeName: "thead" }, this._daysTable);
		this._daysTableHeaderRow = $HGDomElement.createElementFromTemplate({ nodeName: "tr" }, this._daysTableHeader);
		this._daysBody = $HGDomElement.createElementFromTemplate({ nodeName: "tbody" }, this._daysTable);

		for (var i = 0; i < 7; i++) {
			var dayCell = $HGDomElement.createElementFromTemplate({ nodeName: "td" }, this._daysTableHeaderRow);
			var dayDiv = $HGDomElement.createElementFromTemplate({
				nodeName: "div",
				cssClasses: ["ajax__calendar_day"]
			}, dayCell);
		}

		for (var i = 0; i < 6; i++) {
			var daysRow = $HGDomElement.createElementFromTemplate({ nodeName: "tr" }, this._daysBody);
			for (var j = 0; j < 7; j++) {
				var dayCell = $HGDomElement.createElementFromTemplate({ nodeName: "td" }, daysRow);
				var dayDiv = $HGDomElement.createElementFromTemplate({
					nodeName: "div",
					properties: {
						mode: "day",
						innerHTML: "&nbsp;",
						style: { lineHeight: '13px', padding: "3px", margin: "0px"}  //linbin add 避免外界样式影响~
					},
					events: this._cell$delegates,
					cssClasses: ["ajax__calendar_today"]
				}, dayCell);

				dayDiv.style.textAlign = "right";
			}
		}
	},

	_buildMonths: function () {
		/// <summary>
		/// Builds a "months of the year" view for the calendar
		/// </summary>

		var dtf = Sys.CultureInfo.CurrentCulture.dateTimeFormat;

		this._months = $HGDomElement.createElementFromTemplate({
			nodeName: "div",
			cssClasses: ["ajax__calendar_months"],
			visible: false
		}, this._body);
		this._modes["months"] = this._months;

		this._monthsTable = $HGDomElement.createElementFromTemplate({
			nodeName: "table",
			properties: {
				cellPadding: 0,
				cellSpacing: 0,
				border: 0,
				style: { margin: "auto" }
			}
		}, this._months);

		this._monthsBody = $HGDomElement.createElementFromTemplate({ nodeName: "tbody" }, this._monthsTable);
		for (var i = 0; i < 3; i++) {
			var monthsRow = $HGDomElement.createElementFromTemplate({ nodeName: "tr" }, this._monthsBody);
			for (var j = 0; j < 4; j++) {
				var monthCell = $HGDomElement.createElementFromTemplate({ nodeName: "td" }, monthsRow);
				var monthDiv = $HGDomElement.createElementFromTemplate({
					nodeName: "div",
					properties: {
						mode: "month",
						month: (i * 4) + j,
						innerHTML: "<br />" + dtf.AbbreviatedMonthNames[(i * 4) + j]
					},
					events: this._cell$delegates,
					cssClasses: ["ajax__calendar_month"]
				}, monthCell);
			}
		}
	},

	_buildYears: function () {
		/// <summary>
		/// Builds a "years in this decade" view for the calendar
		/// </summary>

		this._years = $HGDomElement.createElementFromTemplate({
			nodeName: "div",
			cssClasses: ["ajax__calendar_years"],
			visible: false
		}, this._body);
		this._modes["years"] = this._years;

		this._yearsTable = $HGDomElement.createElementFromTemplate({
			nodeName: "table",
			properties: {
				cellPadding: 0,
				cellSpacing: 0,
				border: 0,
				style: { margin: "auto" }
			}
		}, this._years);

		this._yearsBody = $HGDomElement.createElementFromTemplate({ nodeName: "tbody" }, this._yearsTable);
		for (var i = 0; i < 3; i++) {
			var yearsRow = $HGDomElement.createElementFromTemplate({ nodeName: "tr" }, this._yearsBody);
			for (var j = 0; j < 4; j++) {
				var yearCell = $HGDomElement.createElementFromTemplate({ nodeName: "td" }, yearsRow);
				var yearDiv = $HGDomElement.createElementFromTemplate({
					nodeName: "div",
					properties: {
						mode: "year",
						year: ((i * 4) + j) - 1
					},
					events: this._cell$delegates,
					cssClasses: ["ajax__calendar_year"]
				}, yearCell);
			}
		}
	},

	_performLayout: function () {
		/// <summmary>
		/// Updates the various views of the calendar to match the current selected and visible dates
		/// </summary>

		var elt = this.get_element();
		if (!elt) return;
		if (!this.get_isInitialized()) return;
		if (!this._isOpen) return;

		var dtf = Sys.CultureInfo.CurrentCulture.dateTimeFormat;
		var selectedDate = this.get_selectedDate();
		var visibleDate = this._getEffectiveVisibleDate();
		var todaysDate = this.get_todaysDate();

		switch (this._mode) {
			case "days":

				var firstDayOfWeek = this._getFirstDayOfWeek();
				var daysToBacktrack = visibleDate.getDay() - firstDayOfWeek;
				if (daysToBacktrack <= 0)
					daysToBacktrack += 7;

				var startDate = new Date(visibleDate.getFullYear(), visibleDate.getMonth(), visibleDate.getDate() - daysToBacktrack);
				var currentDate = startDate;

				for (var i = 0; i < 7; i++) {
					var dayCell = this._daysTableHeaderRow.cells[i].firstChild;
					if (dayCell.firstChild) {
						dayCell.removeChild(dayCell.firstChild);
					}
					dayCell.appendChild(this._calendarPopup.get_popupDocument().createTextNode(dtf.ShortestDayNames[(i + firstDayOfWeek) % 7]));
				}
				var currenMonth = currentDate.getMonth();
				for (var week = 0; week < 6; week++) {
					var weekRow = this._daysBody.rows[week];
					for (var dayOfWeek = 0; dayOfWeek < 7; dayOfWeek++) {
						var dayCell = weekRow.cells[dayOfWeek].firstChild;
						if (dayCell.firstChild) {
							dayCell.removeChild(dayCell.firstChild);
						}
						//is show current month
						//只显示当月,或分别显示补齐的上月末和下月初
						if (this._isOnlyCurrentMonth) {
							if (currentDate.getMonth() == (currenMonth + 1 == 12 ? 0 : currenMonth + 1)) {
								dayCell.appendChild(this._calendarPopup.get_popupDocument().createTextNode(currentDate.getDate()));

								dayCell.title = currentDate.localeFormat("D");
								dayCell.date = currentDate;

								$HGDomElement.removeCssClasses(dayCell.parentNode, ["ajax__calendar_other", "ajax__calendar_active"]);
								$HGDomElement.removeCssClasses(dayCell.parentNode, ["ajax__calendar_currentmonthday"]);
								if (currentDate.getDate() == new Date().getDate() && currentDate.getMonth() == new Date().getMonth() && currentDate.getFullYear() == new Date().getFullYear()) {
									Sys.UI.DomElement.addCssClass(dayCell.parentNode, "ajax__calendar_currentmonthday"); //this._getCssClass(dayCell.date, 'd'));
								}
								else {
									Sys.UI.DomElement.addCssClass(dayCell.parentNode, this._getCssClass(dayCell.date, 'd'));
								}
							}
							else  //不是当月的
							{
								dayCell.title = "";
								//清空除了默认的外所有的
								$HGDomElement.removeCssClasses(dayCell.parentNode, ["ajax__calendar_active", "ajax__calendar_today", "ajax__calendar_currentmonthday"]);
							}
						}
						else {
							dayCell.appendChild(this._calendarPopup.get_popupDocument().createTextNode(currentDate.getDate()));

							dayCell.title = currentDate.localeFormat("D");
							dayCell.date = currentDate;

							$HGDomElement.removeCssClasses(dayCell.parentNode, ["ajax__calendar_other", "ajax__calendar_active"]);
							$HGDomElement.removeCssClasses(dayCell.parentNode, ["ajax__calendar_currentmonthday"]);

							if (currentDate.getDate() == new Date().getDate() && currentDate.getMonth() == new Date().getMonth() && currentDate.getFullYear() == new Date().getFullYear()) {
								Sys.UI.DomElement.addCssClass(dayCell.parentNode, "ajax__calendar_currentmonthday"); //this._getCssClass(dayCell.date, 'd'));
							}
							else {
								Sys.UI.DomElement.addCssClass(dayCell.parentNode, this._getCssClass(dayCell.date, 'd'));
							}
						}
						currentDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate() + 1);
					}
				}
				if (!this._isComplexHeader) {
					this._prevArrow.date = new Date(visibleDate.getFullYear(), visibleDate.getMonth() - 1, 1);
					this._nextArrow.date = new Date(visibleDate.getFullYear(), visibleDate.getMonth() + 1, 1);

					if (this._title.firstChild) {
						this._title.removeChild(this._title.firstChild);
					}

					//Add List
					this._title.appendChild(this._calendarPopup.get_popupDocument().createTextNode(visibleDate.localeFormat("yyyy,MMMM")));

				}
				else {
					//add inputdate
					if (this._title.firstChild) {
						this._title.removeChild(this._title.firstChild);
					}

					//Add List

					this._title.appendChild(this._calendarPopup.get_popupDocument().createTextNode("   "));
					//add current selected
					this._selectMonthList.options[visibleDate.getMonth()].selected = true;
					this._titleinput.value = visibleDate.getFullYear();

				}
				this._title.date = visibleDate;
				break;

			case "months":

				for (var i = 0; i < this._monthsBody.rows.length; i++) {
					var row = this._monthsBody.rows[i];
					for (var j = 0; j < row.cells.length; j++) {
						var cell = row.cells[j].firstChild;
						cell.date = new Date(visibleDate.getFullYear(), cell.month, 1);
						$HGDomElement.removeCssClasses(cell.parentNode, ["ajax__calendar_other", "ajax__calendar_active"]);
						Sys.UI.DomElement.addCssClass(cell.parentNode, this._getCssClass(cell.date, 'M'));
					}
				}

				if (this._title.firstChild) {
					this._title.removeChild(this._title.firstChild);
				}
				this._title.appendChild(this._calendarPopup.get_popupDocument().createTextNode(visibleDate.localeFormat("yyyy")));
				this._title.date = visibleDate;
				this._prevArrow.date = new Date(visibleDate.getFullYear() - 1, 0, 1);
				this._nextArrow.date = new Date(visibleDate.getFullYear() + 1, 0, 1);

				break;

			case "years":

				var minYear = (Math.floor(visibleDate.getFullYear() / 10) * 10);
				for (var i = 0; i < this._yearsBody.rows.length; i++) {
					var row = this._yearsBody.rows[i];
					for (var j = 0; j < row.cells.length; j++) {
						var cell = row.cells[j].firstChild;
						cell.date = new Date(minYear + cell.year, 0, 1);
						if (cell.firstChild) {
							cell.removeChild(cell.lastChild);
						} else {
							cell.appendChild(this._calendarPopup.get_popupDocument().createElement("br"));
						}
						cell.appendChild(this._calendarPopup.get_popupDocument().createTextNode(minYear + cell.year));
						$HGDomElement.removeCssClasses(cell.parentNode, ["ajax__calendar_other", "ajax__calendar_active"]);
						Sys.UI.DomElement.addCssClass(cell.parentNode, this._getCssClass(cell.date, 'y'));
					}
				}

				if (this._title.firstChild) {
					this._title.removeChild(this._title.firstChild);
				}
				this._title.appendChild(this._calendarPopup.get_popupDocument().createTextNode(minYear.toString() + "-" + (minYear + 9).toString()));
				this._title.date = visibleDate;
				this._prevArrow.date = new Date(minYear - 10, 0, 1);
				this._nextArrow.date = new Date(minYear + 10, 0, 1);

				break;
		}
		if (this._today.firstChild) {
			this._today.removeChild(this._today.firstChild);
		}
		this._today.appendChild(this._calendarPopup.get_popupDocument().createTextNode(todaysDate.localeFormat(Sys.CultureInfo.CurrentCulture.dateTimeFormat.LongDatePattern)));

		//this._today.appendChild(this._calendarPopup.get_popupDocument().createTextNode(String.format("今天: {0}", todaysDate.localeFormat("yyyy年MMMMd日"))));

		this._today.date = todaysDate;
	},

	_ensureCalendar: function () {
		if (!this._container) {

			var elt = this.get_element();
			//popoup size 200,200

			var popupControl = $HGRootNS.PopupControl;
			//popupControl.set_height(400);

			this._calendarPopup = $create(popupControl, { positionElement: elt, positioningMode: $HGRootNS.PositioningMode.BottomLeft }, {}, {}, null);
			var doc = $HGDomElement.get_currentDocument();
			$HGDomElement.set_currentDocument(this._calendarPopup.get_popupDocument());
			try {
				//figure
				this._buildCalendar();
				this._buildHeader();
				this._buildBody();
				this._buildFooter();
				//?????             
			}
			finally {
				$HGDomElement.set_currentDocument(doc);
			}
		}
	},

	_switchMonth: function (date, dontAnimate) {
		/// <summary>
		/// Switches the visible month in the days view
		/// </summary>
		/// <param name="date" type="Date">The visible date to switch to</param>
		/// <param name="dontAnimate" type="Boolean">Prevents animation from occuring if the control is animated</param>

		// Check _isAnimating to make sure we don't animate horizontally and vertically at the same time
		if (this._isAnimating) {
			return;
		}

		var visibleDate = this._getEffectiveVisibleDate();
		if ((date && date.getFullYear() == visibleDate.getFullYear() && date.getMonth() == visibleDate.getMonth())) {
			dontAnimate = true;
		}

		if (this._animated && !dontAnimate) {
			this._isAnimating = true;

			var newElement = this._modes[this._mode];
			var oldElement = newElement.cloneNode(true);
			this._body.appendChild(oldElement);
			if (visibleDate > date) {
				// animating down
				// the newIndex element is the top
				// the oldIndex element is the bottom (visible)

				// move in, fade in
				$HGDomElement.get_currentDocument(newElement, { x: -162, y: 0 });
				Sys.UI.DomElement.setVisible(newElement, true);
				this._modeChangeMoveTopOrLeftAnimation.set_propertyKey("left");
				this._modeChangeMoveTopOrLeftAnimation.set_target(newElement);
				this._modeChangeMoveTopOrLeftAnimation.set_startValue(-this._width);
				this._modeChangeMoveTopOrLeftAnimation.set_endValue(0);

				// move out, fade out
				$HGDomElement.get_currentDocument(oldElement, { x: 0, y: 0 });
				Sys.UI.DomElement.setVisible(oldElement, true);
				this._modeChangeMoveBottomOrRightAnimation.set_propertyKey("left");
				this._modeChangeMoveBottomOrRightAnimation.set_target(oldElement);
				this._modeChangeMoveBottomOrRightAnimation.set_startValue(0);
				this._modeChangeMoveBottomOrRightAnimation.set_endValue(this._width);

			} else {
				// animating up
				// the oldIndex element is the top (visible)
				// the newIndex element is the bottom

				// move out, fade out
				$HGDomElement.get_currentDocument(oldElement, { x: 0, y: 0 });
				Sys.UI.DomElement.setVisible(oldElement, true);
				this._modeChangeMoveTopOrLeftAnimation.set_propertyKey("left");
				this._modeChangeMoveTopOrLeftAnimation.set_target(oldElement);
				this._modeChangeMoveTopOrLeftAnimation.set_endValue(-this._width);
				this._modeChangeMoveTopOrLeftAnimation.set_startValue(0);

				// move in, fade in
				$HGDomElement.get_currentDocument(newElement, { x: 162, y: 0 });
				Sys.UI.DomElement.setVisible(newElement, true);
				this._modeChangeMoveBottomOrRightAnimation.set_propertyKey("left");
				this._modeChangeMoveBottomOrRightAnimation.set_target(newElement);
				this._modeChangeMoveBottomOrRightAnimation.set_endValue(0);
				this._modeChangeMoveBottomOrRightAnimation.set_startValue(this._width);
			}
			this._visibleDate = date;
			this.invalidate();

			var endHandler = Function.createDelegate(this, function () {
				this._body.removeChild(oldElement);
				oldElement = null;
				this._isAnimating = false;
				this._modeChangeAnimation.remove_ended(endHandler);
			});
			this._modeChangeAnimation.add_ended(endHandler);
			this._modeChangeAnimation.play();
		} else {
			this._visibleDate = date;
			this.invalidate();
		}
	},

	_switchMode: function (mode, dontAnimate) {
		/// <summary>
		/// Switches the visible view from "days" to "months" to "years"
		/// </summary>
		/// <param name="mode" type="String">The view mode to switch to</param>
		/// <param name="dontAnimate" type="Boolean">Prevents animation from occuring if the control is animated</param>

		// Check _isAnimating to make sure we don't animate horizontally and vertically at the same time
		if (this._isAnimating || (this._mode == mode)) {
			return;
		}

		var moveDown = this._modeOrder[this._mode] < this._modeOrder[mode];
		var oldElement = this._modes[this._mode];
		var newElement = this._modes[mode];
		this._mode = mode;

		if (this._animated && !dontAnimate) {
			this._isAnimating = true;

			this.invalidate();

			if (moveDown) {
				// animating down
				// the newIndex element is the top
				// the oldIndex element is the bottom (visible)

				// move in, fade in
				$HGDomElement.get_currentDocument(newElement, { x: 0, y: -this._height });
				Sys.UI.DomElement.setVisible(newElement, true);
				this._modeChangeMoveTopOrLeftAnimation.set_propertyKey("top");
				this._modeChangeMoveTopOrLeftAnimation.set_target(newElement);
				this._modeChangeMoveTopOrLeftAnimation.set_startValue(-this._height);
				this._modeChangeMoveTopOrLeftAnimation.set_endValue(0);

				// move out, fade out
				$HGDomElement.get_currentDocument(oldElement, { x: 0, y: 0 });
				Sys.UI.DomElement.setVisible(oldElement, true);

				this._modeChangeMoveBottomOrRightAnimation.set_propertyKey("top");
				this._modeChangeMoveBottomOrRightAnimation.set_target(oldElement);
				this._modeChangeMoveBottomOrRightAnimation.set_startValue(0);
				this._modeChangeMoveBottomOrRightAnimation.set_endValue(this._height);

			} else {
				// animating up
				// the oldIndex element is the top (visible)
				// the newIndex element is the bottom

				// move out, fade out
				$HGDomElement.get_currentDocument(oldElement, { x: 0, y: 0 });
				Sys.UI.DomElement.setVisible(oldElement, true);
				this._modeChangeMoveTopOrLeftAnimation.set_propertyKey("top");
				this._modeChangeMoveTopOrLeftAnimation.set_target(oldElement);
				this._modeChangeMoveTopOrLeftAnimation.set_endValue(-this._height);
				this._modeChangeMoveTopOrLeftAnimation.set_startValue(0);

				// move in, fade in
				$HGDomElement.get_currentDocument(newElement, { x: 0, y: 139 });
				Sys.UI.DomElement.setVisible(newElement, true);
				this._modeChangeMoveBottomOrRightAnimation.set_propertyKey("top");
				this._modeChangeMoveBottomOrRightAnimation.set_target(newElement);
				this._modeChangeMoveBottomOrRightAnimation.set_endValue(0);
				this._modeChangeMoveBottomOrRightAnimation.set_startValue(this._height);
			}
			var endHandler = Function.createDelegate(this, function () {
				this._isAnimating = false;
				this._modeChangeAnimation.remove_ended(endHandler);
			});
			this._modeChangeAnimation.add_ended(endHandler);
			this._modeChangeAnimation.play();
		} else {
			this._mode = mode;
			Sys.UI.DomElement.setVisible(oldElement, false);
			this.invalidate();
			Sys.UI.DomElement.setVisible(newElement, true);
			$HGDomElement.get_currentDocument(newElement, { x: 0, y: 0 });
		}
	},

	_isSelected: function (date, part) {
		/// <summary>
		/// Gets whether the supplied date is the currently selected date
		/// </summary>
		/// <param name="date" type="Date">The date to match</param>
		/// <param name="part" type="String">The most significant part of the date to test</param>
		/// <returns type="Boolean" />

		var value = this.get_selectedDate();
		if (!value) return false;
		switch (part) {
			case 'd':
				if (date.getDate() != value.getDate()) return false;
				// goto case 'M';
			case 'M':
				if (date.getMonth() != value.getMonth()) return false;
				// goto case 'y';
			case 'y':
				if (date.getFullYear() != value.getFullYear()) return false;
				break;
		}
		return true;
	},

	_isOther: function (date, part) {
		/// <summary>
		/// Gets whether the supplied date is in a different view from the current visible month
		/// </summary>
		/// <param name="date" type="Date">The date to match</param>
		/// <param name="part" type="String">The most significant part of the date to test</param>
		/// <returns type="Boolean" />

		var value = this._getEffectiveVisibleDate();
		switch (part) {
			case 'd':
				return (date.getFullYear() != value.getFullYear() || date.getMonth() != value.getMonth());
			case 'M':
				return false;
			case 'y':
				var minYear = (Math.floor(value.getFullYear() / 10) * 10);
				return date.getFullYear() < minYear || (minYear + 10) <= date.getFullYear();
		}
		return false;
	},

	_getCssClass: function (date, part) {
		/// <summary>
		/// Gets the cssClass to apply to a cell based on a supplied date
		/// </summary>
		/// <param name="date" type="Date">The date to match</param>
		/// <param name="part" type="String">The most significant part of the date to test</param>
		/// <returns type="String" />

		if (this._isSelected(date, part)) {
			return "ajax__calendar_active";
		} else if (this._isOther(date, part)) {
			return "ajax__calendar_other";
		} else {
			return "";
		}
	},

	_getEffectiveVisibleDate: function () {
		var value = this.get_visibleDate();
		if (value == null)
			value = this.get_selectedDate();
		if (value == null)
			value = this.get_todaysDate();
		return new Date(value.getFullYear(), value.getMonth(), 1);
	},

	_getFirstDayOfWeek: function () {
		/// <summary>
		/// Gets the first day of the week
		/// </summary>

		if (this.get_firstDayOfWeek() != $HGRootNS.FirstDayOfWeek.Default) {
			return this.get_firstDayOfWeek();
		}
		return Sys.CultureInfo.CurrentCulture.dateTimeFormat.FirstDayOfWeek;
	},

	_parseTextValue: function (text) {
		/// <summary>
		/// Converts a text value from the textbox into a date
		/// </summary>
		/// <param name="text" type="String" mayBeNull="true">The text value to parse</param>
		/// <returns type="Date" />

		var value = null;
		if (text) {
			value = Date.parseLocale(text, this.get_format());
		}
		if (isNaN(value)) {
			value = null;
		}
		return value;
	},

	/*When img click,set year change*/
	_setYearToDate: function (coordinate) {
		//image 18px * 18px
		if (coordinate > 9) {
			this._titleinput.value = parseInt(this._titleinput.value) - 1;
			var visibleDate = this._getEffectiveVisibleDate();
			var selectdate = new Date(this._titleinput.value, visibleDate.getMonth(), 1);
			this._switchMonth(selectdate);
		}
		else {
			this._titleinput.value = parseInt(this._titleinput.value) + 1;
			var visibleDate = this._getEffectiveVisibleDate();
			var selectdate = new Date(this._titleinput.value, visibleDate.getMonth(), 1);
			this._switchMonth(selectdate);
		}
	},

	_simpleFormat: function (date) {
		/*      
		var c = "-";
		y = date.getFullYear();
		m = "0" + (date.getMonth() + 1);
		d = "0" + date.getDate();

		m = m.substr(m.length - 2, 2);
		d = d.substr(d.length - 2, 2);

		return y+c+m+c+d;
		*/
		var result = "";

		if (Date.isMinDate(date) == false)
			result = String.format("{0:yyyy-MM-dd}", date);

		return result;
	},

	_onFocus: function (args) {
		try {
			var evt = this._checkArgsEvents(args);
			var e = this.get_element();
			this._InLostfocus = false;

			if (this._OnFocusCssClass != "") {
				this._addCssClassMaskedEdit(this._OnFocusCssClass);
			}

			var Inipos = this._initValue();
			var ClearText = this._getClearMask(e.value);
			this._initialvalue = ClearText;

			this._setSelectionRange(Inipos, Inipos);
		}
		catch (e) {
		}
	},

	_popupOnfocus: function (e) {
		/// <summary> 
		/// Handles the focus event of the popup
		/// </summary>
		/// <param name="e" type="Sys.UI.DomEvent">The arguments for the event</param>

		if ((e.type == 'focus' && Sys.Browser.agent != Sys.Browser.InternetExplorer) ||
			(e.type == 'activate' && Sys.Browser.agent == Sys.Browser.InternetExplorer) ||
			(Sys.Browser.agent === Sys.Browser.Safari) ||
			(Sys.Browser.agent === Sys.Browser.Opera)) {

			this._focus.post();
		}
	},

	_cellOnMouseOver: function (e) {
		/// <summary> 
		/// Handles the mouseover event of a cell
		/// </summary>
		/// <param name="e" type="Sys.UI.DomEvent">The arguments for the event</param>

		if (Sys.Browser.agent === Sys.Browser.Safari) {
			// Safari doesn't reliably call _cellOnMouseOut, so clear other cells here to keep the UI correct
			for (var i = 0; i < this._daysBody.rows.length; i++) {
				var row = this._daysBody.rows[i];
				for (var j = 0; j < row.cells.length; j++) {
					Sys.UI.DomElement.removeCssClass(row.cells[j].firstChild.parentNode, "ajax__calendar_hover");
				}
			}
			//add other browser
			for (var k = 0; k < this._daysTableHeaderRow.cells.length; k++) {
				Sys.UI.DomElement.removeCssClass(this._daysTableHeaderRow.cells[k].firstChild.parentNode, "ajax__calendar_weekday");
			}
		}

		var target = e.target;

		Sys.UI.DomElement.addCssClass(target.parentNode, "ajax__calendar_hover");

		if (target.date != null && target.date != "undefined") {
			Sys.UI.DomElement.addCssClass(this._daysTableHeaderRow.cells[((target.date.getUTCDay() + (this._getFirstDayOfWeek() + 1)) % 7)].firstChild.parentNode, "ajax__calendar_weekday");
		}

		e.stopPropagation();
	},

	_cellOnMouseOut: function (e) {
		/// <summary> 
		/// Handles the mouseout event of a cell
		/// </summary>
		/// <param name="e" type="Sys.UI.DomEvent">The arguments for the event</param>

		var target = e.target;

		Sys.UI.DomElement.removeCssClass(target.parentNode, "ajax__calendar_hover");

		for (var k = 0; k < this._daysTableHeaderRow.cells.length; k++) {
			Sys.UI.DomElement.removeCssClass(this._daysTableHeaderRow.cells[k].firstChild.parentNode, "ajax__calendar_weekday");
		}

		e.stopPropagation();
	},

	_cellOnClick: function (e) {
		/// <summary> 
		/// Handles the click event of a cell
		/// </summary>
		/// <param name="e" type="Sys.UI.DomEvent">The arguments for the event</param>
		//if (1 == 2)
		//alert(e);

		if ((Sys.Browser.agent === Sys.Browser.Safari) ||
			(Sys.Browser.agent === Sys.Browser.Opera)) {
			// _popupOnfocus doesn't get called on Safari or Opera, so we call it manually now
			this._popupOnfocus(e);
		}

		if (!this._enabled)
			return;

		var target = e.target;
		var visibleDate = this._getEffectiveVisibleDate();

		Sys.UI.DomElement.removeCssClass(target.parentNode, "ajax__calendar_hover");

		switch (target.mode) {
			case "prev":
			case "next":
				this._switchMonth(target.date); //alert(target.date);
				break;
			case "title":
				switch (this._mode) {
					case "days": this._switchMode("months"); break;
					case "months": this._switchMode("years"); break;
				}
				break;
			case "month":
				if (target.month == visibleDate.getMonth()) {
					this._switchMode("days");
				} else {
					this._visibleDate = target.date;
					this._switchMode("days");
				}
				break;
			case "year":
				if (target.date.getFullYear() == visibleDate.getFullYear()) {
					this._switchMode("months");
				} else {
					this._visibleDate = target.date;
					this._switchMode("months");
				}
				break;
			case "day":
				this._setSelectedDate(target.date);
				this._switchMonth(target.date);
				this.hide();
				break;
			case "today":
				this._setSelectedDate(target.date);
				this._switchMonth(target.date);
				this.hide();
				break;
		}

		e.stopPropagation();
		e.preventDefault();
	},

	//add list event
	_selectOnSelect: function (e) {

		var visibleDate = this._getEffectiveVisibleDate();
		var selectdate = new Date(visibleDate.getFullYear(), e.target.value, 1);
		this._switchMonth(selectdate);

	},

	_yearSelectOnSelect: function (e) {
		var visibleDate = this._getEffectiveVisibleDate();
		var selectdate = new Date(e.target.value, visibleDate.getMonth(), 1);
		this._switchMonth(selectdate);

	},

	_imgOnClick: function (e) {
		this._setYearToDate(e.offsetY);

		e.stopPropagation();
		e.preventDefault();
	},

	_buttonOnClick: function (e) {
		/// <summary> 
		/// Handles the click event of the asociated button
		/// </summary>
		/// <param name="e" type="Sys.UI.DomEvent">The arguments for the event</param>
		if (!this._isOpen) {
			e.preventDefault();
			e.stopPropagation();
			if (this._enabled)
				this.show();
		} else {
			this._isOpen = false; this.show();
		}
		return false;
	},

	_onKeyPress: function (args) {
		var evt = this._checkArgsEvents(args);
		if (evt == null) {
			return;
		}
		if (this.get_element().readOnly) {
			this._setCancelEvent(evt);
			return;
		}

		var scanCode;
		var navkey = false;
		if (Sys.Browser.agent == Sys.Browser.InternetExplorer) {
			// IE
			scanCode = evt.keyCode;
		}
		//not ie
		else {
			if (evt.charCode) {
				scanCode = evt.charCode;
			}
			else {
				scanCode = evt.keyCode;
			}
			if (evt.keyIdentifier) {
				// Safari
				//3: 'KEY_ENTER', 13
				//63276: 'KEY_PAGE_UP', 33
				//63277: 'KEY_PAGE_DOWN', 34
				//63275: 'KEY_END', 35
				//63273: 'KEY_HOME', 36
				//63234: 'KEY_ARROW_LEFT', 37
				//63232: 'KEY_ARROW_UP', 38
				//63235: 'KEY_ARROW_RIGHT', 39
				//63233: 'KEY_ARROW_DOWN',40
				//63302: 'KEY_INSERT', 45
				//63272: 'KEY_DELETE', 46
				if (evt.ctrlKey || evt.altKey || evt.metaKey) {
					return;
				}
				if (evt.keyIdentifier.substring(0, 2) != "U+") {
					return;
				}
				if (scanCode == 63272) // delete
				{
					scanCode = 46; // convert to IE code
					navkey = true;
				}
				else if (scanCode == 63302) {
					scanCode = 45; // convert to IE code
					navkey = true;
				}
				else if (scanCode == 63233) {
					scanCode = 40; // convert to IE code
					navkey = true;
				}
				else if (scanCode == 63235) {
					scanCode = 39; // convert to IE code
					navkey = true;
				}
				else if (scanCode == 63232) {
					scanCode = 38; // convert to IE code
					navkey = true;
				}
				else if (scanCode == 63234) {
					scanCode = 37; // convert to IE code
					navkey = true;
				}
				else if (scanCode == 63273) {
					scanCode = 36; // convert to IE code
					navkey = true;
				}
				else if (scanCode == 63275) {
					scanCode = 35; // convert to IE code
					navkey = true;
				}
				else if (scanCode == 63277) {
					scanCode = 34; // convert to IE code
					navkey = true;
				}
				else if (scanCode == 63276) {
					scanCode = 33; // convert to IE code
					navkey = true;
				}
				else if (scanCode == 3) {
					scanCode = 13; // convert to IE code
					navkey = true;
				}
			}
			// key delete/backespace and key navigation for not IE browsers
			if (typeof (evt.which) != "undefined" && evt.which != null) {
				if (evt.which == 0) {
					navkey = true;
				}
			}
			if (scanCode == 8) {
				navkey = true;
			}
			if (!this._onNavigator(scanCode, evt, navkey)) {
				return;
			}
			if (this._specialNavKey(scanCode, navkey)) {
				return;
			}
		}
		//
		if (scanCode && scanCode >= 0x20 /* space */) {
			var c = String.fromCharCode(scanCode);
			var curpos = -1;
			//select and delete
			if (Sys.Browser.agent == Sys.Browser.InternetExplorer) {
				curpos = this._deleteTextSelection();
			}
			if (curpos == -1) {
				curpos = this._getCurrentPosition();
			}
			if (curpos <= this._LogicFirstPos) {
				this._setSelectionRange(this._LogicFirstPos, this._LogicFirstPos);
				curpos = this._LogicFirstPos;
			}
			//modify all logiclastpos+1
			else if (curpos >= this._LogicLastPos) {
				this._setSelectionRange(this._LogicLastPos, this._LogicLastPos);
				curpos = this._LogicLastPos;
			}
			var logiccur = curpos;

			//else if (this._processKey(logiccur,c)) 
			if (this._processKey(logiccur, c)) {

				this._insertContent(c, curpos);
				curpos = this._getNextPosition(curpos + 1);

				this._setSelectionRange(curpos, curpos);
				this._setCancelEvent(evt);
				return;
			}
			else {
				//valid key navigation for not IE browsers
				//key navigation for IE capture in keydown 
				if (!this._specialNavKey(scanCode, navkey)) {
					this._setCancelEvent(evt);
				}
				return;
			}
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
	},

	//add by wuwei
	_beforePickerFocus: function (args) {
		this._isInsideImg = true;
	},
	_pickerOnBlur: function (args) {
		if (!this._isInsideImg)
			this._onBlur(); //this.get_element().focus();
	},
	_pickerOnFocus: function (args) {
	},

	//When textbox onblur reorganize value for input
	_onBlur: function (args) {
		var oldValue = this._value;

		//add by wuwei
		if (!this._isInsideImg) {   //true不校验

			/////////////检验/////////////
			var vlen = this.get_element().value.length;
			var mlen = this._Mask.length;

			if (vlen > mlen)  //对于粘贴后长度处理
			{
				this.get_element().value = this.get_element().value.substring(0, mlen);
			}

			if (this.get_element().value == "" || this.get_element().value == this._Mask.replace(/9/g, '_')) {
				this.get_element().value = "";

				//if (this.get_value() != Date.minDate)
				//	this.raiseClientValueChanged();

				//return;
			}

			var evt = this._checkArgsEvents(args);

			this._InLostfocus = true;
			ValueText = this.get_element().value;
			ClearText = this._getClearMask(ValueText);

			// auto format empty text Time
			if (ClearText != "" && this._AutoComplete) {
				var CurDate = new Date();
				var Ycur = CurDate.getFullYear().toString();
				if (Ycur.length < 4) {
					Ycur = "0" + Ycur;
				}
				//support auto supply the lack: 00:00:00
				if (this._AutoCompleteValue != "") {
					Ycur = this._AutoCompleteValue.substring(0, 4);
				}
				var Symb = ""

				//add by wuwei 0724
				var Mcur = (CurDate.getMonth() + 1).toString();
				if (Mcur.length < 2) {
					Mcur = "0" + Mcur;
				}
				if (this._AutoCompleteValue != "") {
					Mcur = this._AutoCompleteValue.substring(5, 7);
				}
				var Dcur = CurDate.getDate().toString();
				if (Dcur.length < 2) {
					Dcur = "0" + Dcur;
				}
				var maskvalid = this._MaskConv.substring(this._LogicFirstPos, this._LogicFirstPos + this._LogicLastPos + 1);
				var PY = ValueText.substring(this._LogicFirstPos, this._LogicFirstPos + 4);
				PY = this._adjustElementTime(PY, Ycur);
				var PM = ValueText.substring(this._LogicFirstPos + 5, this._LogicFirstPos + 7);
				var PM1 = PM.substring(0, 1);
				var PM2 = PM.substring(1, 2);
				if (PM2 == "_" && PM1 * 1 > 0) {
					PM = PM2 + PM1;
				}
				PM = this._adjustElementTime(PM, Mcur);

				var PD;
				//if this is Year:Month:Day format
				if (maskvalid == "9999" + this._CultureTimePlaceholder + "99" + this._CultureTimePlaceholder + "99") {
					if (this._AutoCompleteValue != "") {
						Dcur = this._AutoCompleteValue.substring(8, 10);
					}
					PD = ValueText.substring(this._LogicFirstPos + 8, this._LogicLastPos + 1);
					var PD1 = PD.substring(0, 1);
					var PD2 = PD.substring(1, 2);
					if (PD2 == "_" && PD1 * 1 > 0) {
						PD = PD2 + PD1;
					}
					PD = this._adjustElementTime(PD, Dcur);

					ValueText = ValueText.substring(0, this._LogicFirstPos) + PY + this._CultureTimePlaceholder + PM + this._CultureTimePlaceholder + PD + ValueText.substring(this._LogicLastPos + 1);
					this._LogicTextMask = this._LogicTextMask.substring(0, this._LogicFirstPos) + PY + this._CultureTimePlaceholder + PM + this._CultureTimePlaceholder + PD + this._LogicTextMask.substring(this._LogicLastPos + 1);
				}
				else {
					ValueText = ValueText.substring(0, this._LogicFirstPos) + PY + this._CultureTimePlaceholder + PM + ValueText.substring(this._LogicLastPos + 1);
					this._LogicTextMask = this._LogicTextMask.substring(0, this._LogicFirstPos) + PY + this._CultureTimePlaceholder + PM + this._LogicTextMask.substring(this._LogicLastPos + 1);
				}

				this.get_element().value = (ValueText);
			}  //check ClearText end
			// auto format empty text Number   
			// auto format empty text Date
			// clear mask and set CSS

			this._addCssClassMaskedEdit("");

			/*****************valid******************/

			// perform validation
			// 
			//is use to valid,Valid default  
			if (this._IsValidValue) {
				var IsValid = this._captureValidatorsControl(ValueText) || this.get_element().value == "";

				if (!IsValid) {
					alert(this.get_CurrentMessageError());
					this.set_value("");
					this.get_element().focus();
					if (this._OnInvalidCssClass != "") {
						this._addCssClassMaskedEdit(this._OnInvalidCssClass);
					}

					//resume mask textbox after valided
					if (this._ClearTextOnInvalid) {
						this.get_element().value = (this._createMask());
					}
				}
				else {
					// trigger TextChanged with postback
					if (evt != null && typeof (this.get_element().onchange) != "undefined" && this.get_element().onchange != null && !this.get_element().readOnly) {
						if (this._initialvalue != this.get_element().value) {
							this.get_element().onchange(evt);
						}
					}
				}
			} //check _IsValidValue end
			///////////////////校验
			//add by wuwei 08.11.12  在文本框失去焦点逻辑处理完毕时设置value值
			this._value = this.get_value();
			this.DateValue = this._value;

			if (oldValue * 1 != this._value * 1)
				this.raiseClientValueChanged();

			return;
		} //check _isInsideImg end
		else {
			if (oldValue * 1 != this.get_value() * 1)
				this.raiseClientValueChanged();
		}

		this._isInsideImg = false;
	} //blur event end

}

$HGRootNS.DeluxeCalendar.registerClass($HGRootNSName + ".DeluxeCalendar", $HGRootNS.ControlBase);
