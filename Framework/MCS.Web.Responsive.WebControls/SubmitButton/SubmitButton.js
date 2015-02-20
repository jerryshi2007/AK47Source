/// <reference path="../Common/MicrosoftAjax.debug.js" />


SubmitButtonProgressMode = function () {
    throw Error.notImplemented();
}

SubmitButtonProgressMode.prototype = {
    ByTimeInterval: 0,
    BySteps: 1,
    Continues: 2
}

SubmitButtonProgressMode.registerEnum("SubmitButtonProgressMode");

SubmitButton = function () {
    this._submitControl = false;
    this._submitControlCaption = "";
    this._submitButtons = [];
    this._originalButtons = {};
    this._stoppedFlag = false;
    this.AsyncCalled = false;
    this._win = window;
}

SubmitButton.prototype =
{
    _onSubmitButtonClick: function (button, event) {

        var returnValue = true;
        var btn = button;

        this._submitControl = true;

        if (btn.popupCaption)
            this._submitControlCaption = btn.popupCaption;

        if (btn.disabled)
            returnValue = false;
        else {
            if (typeof (Page_BlockSubmit) == "undefined" ||
				(typeof (Page_BlockSubmit) != "undefined" && Page_BlockSubmit == false))	//检查ASP.Net 的Validator是否阻止提交
            {
                var asyncFun = btn.getAttribute("data-async-function");
                if (typeof (asyncFun) === "string" && asyncFun.length) {
                    this.AsyncCalled = true;

                    if (this._win[asyncFun] && typeof (this._win[asyncFun]) === 'function') {
                        try {
                            var executeResult = this._win[asyncFun]();
                        } catch (ex) {
                            executeResult = false;
                        }

                        if (executeResult) {
                            this._showPopupWindow(btn);
                        }
                        else {
                            this._setAllButtonsState(false);
                        }

                        returnValue = false;
                    }
                }

                if (returnValue)
                    this._showPopupWindow(btn);
            }
        }

        if (!returnValue)
            if (event.preventDefault)
                event.preventDefault();

        if ("returnValue" in event)
            event.returnValue = returnValue;

        return returnValue;
    },

    _showPopupWindow: function (btn) {

        this._setAllButtonsState(true);
        var caption = btn.getAttribute("data-popup-caption"), interval = btn.getAttribute("data-interval"), progressMode = btn.getAttribute("data-progress-mode"), minStep = btn.getAttribute("data-min-step"), maxStep = btn.getAttribute("data-max-step");

        if (caption) {
            ProgressBarInstance.set_interval(Number(interval));
            ProgressBarInstance.set_mode(parseInt(progressMode));
            ProgressBarInstance.set_minStep(parseInt(minStep));
            ProgressBarInstance.set_maxStep(parseInt(maxStep));

            ProgressBarInstance.show(caption);
        }
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

SubmitButton.handleClick = function (button, event, validation) {
    if (button.alreadyClicked) {
        button.disabled = true;
        return false;
    } else {
        if (!!validation && typeof (Page_ClientValidate) == 'function')
            Page_ClientValidate();
        SubmitButtonIntance._onSubmitButtonClick(button, event);
        return true;
    }
}


SubmitButton.attachButton = function (btnID) {
    // 注册一个提交按钮
    Sys.Application.add_init(function () {
        var btn = $get(btnID);
        if (!btn)
            throw Error.create("无法根据ID找到按钮：" + btnID);
        SubmitButtonIntance._registerButton(btn);
    });

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

//if (document.attachEvent)
//    document.attachEvent("onstop", SubmitButton.DocumentStopped);
//else if (document.addEventListener)
//    document.addEventListener("click", SubmitButton.DocumentStopped, false);
//else
//    document.onstop = SubmitButton.DocumentStopped;

ProgressBar = function () {
    this._frame = null;
    this._popupPad = null;
    this._slidingBar = null;
    this._statusBar = null;
    this._captionPan = null;
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
        this._captionPan = null;
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


            frm.style.position = "absolute";
            frm.style.zIndex = 1000;
            frm.frameBorder = 0;

            frm.style.width = w + "px";
            frm.style.height = h + "px";

            document.body.appendChild(frm);

            frm.style.left = this._adjustScrollLeft((this._get_documentWidth() - frm.offsetWidth) / 2) + "px";
            frm.style.top = this._adjustScrollTop((this._get_documentHeight() - frm.offsetHeight) / 2) + "px";

            this._frame = frm;
        }

        return this._frame;
    },

    _createPopupPad: function (w, h) {
        if (!this._popupPad) {
            var div = document.createElement("div");
            div.className = "resp-submit-progress-panel";

            document.body.appendChild(div);

            this._popupPad = div;
        }

        return this._popupPad;
    },

    _createInnerPopupTable: function (container, caption) {

        var board = document.createElement("div"), captionPan = document.createElement("div"), pgPan = document.createElement("div");
        var pgBar = document.createElement("div"), pgBarIndicator = document.createElement("div");
        var divStatus = document.createElement("div");

        board.className = "resp-submit-progress";
        container.appendChild(board);

        captionPan.className = "resp-submit-progress-caption";
        board.appendChild(captionPan);

        captionPan.appendChild(document.createTextNode(caption));

        pgPan.className = "resp-submit-progress-bar";
        board.appendChild(pgPan);

        pgBar.className = "progress";
        pgPan.appendChild(pgBar);

        pgBarIndicator.className = "progress-bar";
        pgBar.appendChild(pgBarIndicator);

        pgBarIndicator.setAttribute("role", "progressbar");

        divStatus.className = "resp-submit-progress-message";
        board.appendChild(divStatus);

        this._statusBar = divStatus;
        this._slidingBar = pgBarIndicator;
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
    var sBar = ProgressBarInstance._slidingBar, percentText;
    if (sBar) {
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

                        if (ProgressBarInstance._statusBar) {
                            if (typeof (ProgressBarInstance._statusBar.textContent) === 'string') {
                                ProgressBarInstance._statusBar.textContent = ProgressBarInstance.get_statusText();
                            } else {
                                ProgressBarInstance._statusBar.innerText = ProgressBarInstance.get_statusText();
                            }
                        }
                    }

                    percentText = percent + "%";
                    if (typeof (sBar.textContent) === "string") {
                        sBar.textContent = percentText
                    } else {
                        sBar.innerText = percentText;
                    }
                    break;
            }



            sBar.style.width = percent + "%";

            sBar.percent = percent;
        }
        catch (e) {
            debugger;
        }
    }
}