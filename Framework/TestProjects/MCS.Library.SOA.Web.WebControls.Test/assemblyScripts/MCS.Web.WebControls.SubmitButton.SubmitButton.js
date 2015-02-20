SubmitButton = function () {
    this._submitControl = false;
    this._submitControlCaption = "";
    this._submitButtons = [];
    this._stoppedFlag = false;
    this.AsyncCalled = false;
}

SubmitButton.prototype =
{
    _onSubmitButtonClick: function () {
        var btn = event.srcElement;

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
                    if (this._isFunction(eval(btn.AsyncInvokeFunction)) && eval(btn.AsyncInvokeFunction)() == false) {
                        this._setAllButtonsState(false);
                        event.returnValue = false;
                    }
                }

                this._setAllButtonsState(true);

                if (btn.popupCaption)
                    ProgressBarInstance.show(btn.popupCaption);
            }
        }
    },
    _isFunction: function ( fn ) {
         return  !!fn && !fn.nodeName && fn.constructor != String &&
          fn.constructor != RegExp && fn.constructor != Array &&
          /function/i.test( fn + "" );
    }
    ,
    _registerButton: function (button) {
        this._submitButtons.push(button);
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

            if (relControl)
                relControl.disabled = disabled;
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

document.attachEvent("onstop", SubmitButton.DocumentStopped);

ProgressBar = function () {
	this._frame = null;
	this._popupPad = null;
	this._slidingBar = null;
	this._timerID = -1;
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

		var oRow = table.insertRow();

		var oCellText = oRow.insertCell();
		oCellText.align = "center";
		oCellText.innerText = caption;
		oCellText.style.fontWeight = "bold";
		oCellText.style.fontSize = "16px";

		var divBarContainer = document.createElement("<div style='width:80%;height:16;border:1px solid silver'></div>");
		divBarContainer.innerHTML = "<table width='100%' height='100%' cellspacing=0 cellpadding=0></table>";

		oCellText.appendChild(divBarContainer);

		table.style.left = this._adjustScrollLeft((this._get_documentWidth() - table.offsetWidth) / 2);
		table.style.top = this._adjustScrollTop((this._get_documentHeight() - table.offsetHeight) / 2);

		var oBarRow = divBarContainer.firstChild.insertRow();
		var oBarCell = oBarRow.insertCell();

		var sBar = document.createElement("<div align='left' style='width:0%;height:20px;background-color:blue;filter:alpha(opacity=25, style=1, finishOpacity=100, startx=0, starty=0, finishx=0, finishy=24)'></div>");
		oBarCell.align = "left";
		oBarCell.appendChild(sBar);

		this._slidingBar = sBar;
		this._slidingBar.percent = 0;

		this._timerID = window.setInterval(ProgressBar.BarInterval, 200);
	},

	_adjustScrollLeft: function (l) {
		return l + document.documentElement.scrollLeft;
	},

	_adjustScrollTop: function (t) {
		return t + document.documentElement.scrollTop;
	},

	_pseudo: function () {
	}
}

ProgressBarInstance = new ProgressBar();

ProgressBar.BarInterval = function () {
	var sBar = ProgressBarInstance._slidingBar;

	if (sBar);
	{
		try {
			var percent = sBar.percent;

			if (percent < 100) {
				percent++;
				sBar.style.width = percent + "%";
				sBar.percent = percent;
			}
		}
		catch (e) {
		}
	}
}