SubmitButtonProgressMode = function () {
	throw Error.notImplemented();
}

SubmitButtonProgressMode.prototype = {
	ByTimeInterval: 0,
	BySteps: 1
}

SubmitButtonProgressMode.registerEnum("SubmitButtonProgressMode");

SubmitButton = function () {
	this._submitControl = false;
	this._submitControlCaption = "";
	this._submitButtons = [];
	this._originalButtons = {};
	this._stoppedFlag = false;
	this.AsyncCalled = false;
}

SubmitButton.prototype =
{
	_onSubmitButtonClick: function () {
		var btn = event.srcElement;

		event.returnValue = true;
		this._submitControl = true;

		if (btn.popupCaption)
			this._submitControlCaption = btn.popupCaption;

		if (btn.disabled)
			event.returnValue = false;
		else {
			if (typeof (Page_BlockSubmit) == "undefined" ||
				(typeof (Page_BlockSubmit) != "undefined" && Page_BlockSubmit == false))	//检查ASP.Net 的Validator是否阻止提交
			{
				if (btn.AsyncInvokeFunction != "undefined" && btn.AsyncInvokeFunction != "") {
					this.AsyncCalled = true;

					if (this._isFunction(eval(btn.AsyncInvokeFunction))) {
						var executeResult = eval(btn.AsyncInvokeFunction)();
						if (executeResult) {
							this._showPopupWindow(btn);
						}
						else {
							this._setAllButtonsState(false);
						}

						event.returnValue = false;
						return;
					}
				}

				this._showPopupWindow(btn);
			}
		}
	},

	_showPopupWindow: function (btn) {
		this._setAllButtonsState(true);

		if (btn.popupCaption) {
			ProgressBarInstance.set_interval(parseInt(btn.interval));
			ProgressBarInstance.set_mode(parseInt(btn.progressMode));
			ProgressBarInstance.set_minStep(parseInt(btn.minStep));
			ProgressBarInstance.set_maxStep(parseInt(btn.maxStep));

			ProgressBarInstance.show(btn.popupCaption);
		}
	},

	_isFunction: function (fn) {
		return !!fn && !fn.nodeName && fn.constructor != String &&
		  fn.constructor != RegExp && fn.constructor != Array &&
		  /function/i.test(fn + "");
	},

	_registerButton: function (button) {
		var index = this._findIndexByButtonID(button);

		if (index == -1)
			this._submitButtons.push(button);
		else
			this._submitButtons[index] = button;
	},

	_findIndexByButtonID: function (button) {
		var result = -1;

		for (var i = 0; i < this._submitButtons.length; i++) {
			if (this._submitButtons[i].id == button.id) {
				result = i;
				break;
			}
		}

		return result;
	},

	_setAllButtonsState: function (bDisabled) {
		for (var i = 0; i < this._submitButtons.length; i++) {
			var elem = this._submitButtons[i];

			if (bDisabled)
				this._setButtonDisabled(elem);
			else
				this._restoreButtonState(elem);
		}
	},

	_setButtonDisabled: function (btn) {
		btn.lastDisabled = btn.disabled;

		if (!btn.alreadyClicked) {
			btn.alreadyClicked = true;
			btn.originalColor = btn.style.color;
			btn.style.color = "gray";

			this._setRelativeControlDisabled(btn, true);
		}
		else {
			btn.disabled = true;
			this._setRelativeControlDisabled(btn, true);
		}
	},

	_setRelativeControlDisabled: function (btn, disabled) {
		if (btn.relControlID) {
			var relControl = document.getElementById(btn.relControlID);

			if (relControl) {
				if (disabled == true) {
					this._originalButtons[btn.relControlID] = relControl.disabled;
					relControl.disabled = disabled;
				} else if (this._originalButtons[btn.relControlID] == false) {
					relControl.disabled = disabled;
				}
			}
		}
	},

	_restoreButtonState: function (btn) {
		if (typeof (btn.originalColor) == "string")
			btn.style.color = btn.originalColor;

		if (btn.alreadyClicked)
			btn.alreadyClicked = false;

		if (typeof (btn.lastDisabled) != "undefined") {
			btn.disabled = btn.lastDisabled;
			this._setRelativeControlDisabled(btn, btn.lastDisabled);
		}

		ProgressBarInstance.hide();
		ProgressBarInstance.set_interval(200);
	},

	_pseudo: function () {
	}
}

var SubmitButtonIntance = new SubmitButton();

SubmitButton.resetAllStates = function () {
	SubmitButtonIntance._setAllButtonsState(false);
	SubmitButtonIntance._stoppedFlag = false;

	SubmitButtonIntance._submitControl = false;
	SubmitButtonIntance._submitControlCaption = "";
}

SubmitButton.DocumentStopped = function () {
	if (SubmitButtonIntance._stoppedFlag) {
		SubmitButton.resetAllStates();
	}
	else
		SubmitButtonIntance._stoppedFlag = true;
}

if (document.attachEvent)
	document.attachEvent("onstop", SubmitButton.DocumentStopped);
else if (document.addEventListener)
	document.addEventListener("click", SubmitButton.DocumentStopped, false);
else
	document.onstop = SubmitButton.DocumentStopped;

ProgressBar = function () {
	this._frame = null;
	this._popupPad = null;
	this._slidingBar = null;
	this._statusBar = null;
	this._timerID = -1;
	this._interval = 200;
	this._minStep = 0;
	this._maxStep = 0;
	this._currentStep = 0;
	this._statusText = "";
	this._mode = SubmitButtonProgressMode.ByTimeInterval;
}

ProgressBar.prototype =
{
	show: function (caption, width, height) {
		if (!width)
			width = 320;

		if (!height)
			height = 160;

		this._createFrame(width, height);
		var div = this._createPopupPad(width, height);
		this._createInnerPopupTable(div, caption);
	},

	hide: function () {
		if (this._popupPad)
			document.body.removeChild(this._popupPad);

		if (this._frame)
			document.body.removeChild(this._frame);

		if (this._timerID != -1) {
			window.clearInterval(this._timerID);
			this._timerID = -1;
		}

		this._popupPad = null;
		this._slidingBar = null;
		this._frame = null;
	},

	get_mode: function () {
		return this._mode;
	},

	set_mode: function (value) {
		this._mode = value;
	},

	get_minStep: function () {
		return this._minStep;
	},

	set_minStep: function (value) {
		this._minStep = value;
	},

	get_maxStep: function () {
		return this._maxStep;
	},

	get_currentStep: function () {
		return this._currentStep;
	},

	set_currentStep: function (value) {
		this._currentStep = value;
	},

	set_maxStep: function (value) {
		this._maxStep = value;
	},

	get_statusText: function () {
		return this._statusText;
	},

	set_statusText: function (value) {
		this._statusText = value;
	},

	onProcessInfoChanged: function () {
		var infoHidden = document.getElementById("submitButtonProgressInfoHiddenID");

		if (infoHidden != null && infoHidden.value != "") {
			if (infoHidden.value == "reset") {
				SubmitButton.resetAllStates();
			}
			else {
				var processInfo = Sys.Serialization.JavaScriptSerializer.deserialize(infoHidden.value);

				this.set_minStep(processInfo.MinStep);
				this.set_maxStep(processInfo.MaxStep);
				this.set_currentStep(processInfo.CurrentStep);
				this.set_statusText(processInfo.StatusText);
			}
		}
	},

	_get_documentWidth: function () {
		var w = document.documentElement.clientWidth;

		if (w == 0)
			w = document.body.offsetWidth;

		return w;
	},

	_get_documentHeight: function () {
		var h = document.documentElement.clientHeight;

		if (h == 0)
			h = document.body.offsetHeight;

		return h;
	},

	_createFrame: function (w, h) {
		if (!this._frame) {
			var frm = document.createElement("iframe");

			with (frm) {
				style.position = "absolute";
				style.zIndex = 1000;
				frameBorder = 0;

				style.width = w;
				style.height = h;
			}

			document.body.appendChild(frm);

			frm.style.left = this._adjustScrollLeft((this._get_documentWidth() - frm.offsetWidth) / 2);
			frm.style.top = this._adjustScrollTop((this._get_documentHeight() - frm.offsetHeight) / 2);

			this._frame = frm;
		}

		return this._frame;
	},

	_createPopupPad: function (w, h) {
		if (!this._popupPad) {
			var div = document.createElement("div");

			with (div) {
				style.position = "absolute";
				style.zIndex = 1500;
				style.borderLeft = "1px solid #666666";
				style.borderTop = "1px solid #666666";
				style.borderRight = "2px solid gray";
				style.borderBottom = "2px solid gray";

				style.width = w;
				style.height = h;
			}

			document.body.appendChild(div);

			div.style.left = this._adjustScrollLeft((this._get_documentWidth() - div.offsetWidth) / 2);
			div.style.top = this._adjustScrollTop((this._get_documentHeight() - div.offsetHeight) / 2);

			this._popupPad = div;
		}

		return this._popupPad;
	},

	_createInnerPopupTable: function (container, caption) {
		var table = document.createElement("table");

		with (table) {
			style.width = "100%";
			style.height = "100%";
			style.zIndex = 2500;
		}

		container.appendChild(table);

		var oRow = table.insertRow(-1);

		var oCellText = oRow.insertCell(-1);
		oCellText.align = "center";
		oCellText.innerText = caption;
		oCellText.style.fontWeight = "bold";
		oCellText.style.fontSize = "16px";

		var divBarContainer = document.createElement("<div style='width:80%;height:16;'></div>");
		divBarContainer.innerHTML = "<table width='100%' height='100%' cellspacing=0 cellpadding=0></table>";

		oCellText.appendChild(divBarContainer);

		table.style.left = this._adjustScrollLeft((this._get_documentWidth() - table.offsetWidth) / 2);
		table.style.top = this._adjustScrollTop((this._get_documentHeight() - table.offsetHeight) / 2);

		var oBarRow = divBarContainer.firstChild.insertRow(-1);
		var oBarCell = oBarRow.insertCell(-1);

		oBarCell.style.border = "1px solid silver";

		var sBar = document.createElement("<div align='left' style='width:0%;height:20px;background-color:blue;filter:alpha(opacity=25, style=1, finishOpacity=100, startx=0, starty=0, finishx=0, finishy=24)'></div>");
		oBarCell.align = "left";
		oBarCell.appendChild(sBar);

		var statusRow = divBarContainer.firstChild.insertRow(-1);

		var statusCell = document.createElement("td");

		statusCell.style.textAlign = "left";
		statusRow.appendChild(statusCell);

		var divStatus = document.createElement("div");
		statusCell.appendChild(divStatus);

		this._statusBar = divStatus;
		this._slidingBar = sBar;
		this._slidingBar.percent = 0;

		this._timerID = window.setInterval(ProgressBar.BarInterval, this._interval);
	},

	_adjustScrollLeft: function (l) {
		return l + document.documentElement.scrollLeft;
	},

	_adjustScrollTop: function (t) {
		return t + document.documentElement.scrollTop;
	},

	get_interval: function () {
		return this._interval;
	},

	set_interval: function (v) {
		this._interval = v;
	},

	_pseudo: function () {
	}
}

ProgressBarInstance = new ProgressBar();

ProgressBar.BarInterval = function () {
	var sBar = ProgressBarInstance._slidingBar;

	if (sBar);
	{
		var percent = sBar.percent;

		try {
			switch (ProgressBarInstance.get_mode()) {
				case SubmitButtonProgressMode.ByTimeInterval:
					{
						if (percent < 100) {
							percent++;
						}
					}
					break;
				case SubmitButtonProgressMode.BySteps:
					{
						if (ProgressBarInstance.get_maxStep() > 0) {
							percent = (ProgressBarInstance.get_currentStep() - ProgressBarInstance.get_minStep()) * 100 /
								(ProgressBarInstance.get_maxStep() - ProgressBarInstance.get_minStep());
						}

						if (ProgressBarInstance._statusBar)
							ProgressBarInstance._statusBar.innerText = ProgressBarInstance.get_statusText();
					}
					break;
			}

			sBar.style.width = percent + "%";
			sBar.percent = percent;
		}
		catch (e) {
		}
	}
}