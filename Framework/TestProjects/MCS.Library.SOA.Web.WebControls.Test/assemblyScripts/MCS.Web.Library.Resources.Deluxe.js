if (typeof ($HGRootNS) === "undefined") {
	var MCS_Web = {};
	var $HGRootNS = MCS_Web;
}

$HGRootNS._WindowFeatureFunction = function() {
}

$HGRootNS._WindowFeatureFunction.prototype =
{
	adjustWindow: function (windowFeature) {
		if (typeof (window.dialogArguments) === "undefined")
			this._adjustOpenWindow(windowFeature);
		else
			this._adjustDialogWindow(windowFeature);

	},

	registerAdjustWindow: function (windowFeature) {
		window.attachEvent("onload", function () { $HGRootNS.WindowFeatureFunction.adjustWindow(windowFeature); });
	},

	_adjustOpenWindow: function (windowFeature) {
		if (window == window.top) {
			var width = this._getUnit(windowFeature.width, windowFeature.widthScript, document.documentElement.clientWidth);
			var height = this._getUnit(windowFeature.height, windowFeature.heightScript, document.documentElement.clientHeight);

			window.resizeTo(width, height);

			var centerLeft = windowFeature.center === true ? ((window.screen.width - width) / 2) : null;
			var centerTop = windowFeature.center === true ? ((window.screen.height - height) / 2) : null;

			var left = this._getUnit(windowFeature.left, windowFeature.leftScript, centerLeft, window.screenLeft);
			var top = this._getUnit(windowFeature.top, windowFeature.topScript, centerTop, window.screenTop);

			window.moveTo(left, top);
		}
	},

	_adjustDialogWindow: function (windowFeature) {
		var width = this._getUnit(windowFeature.width, windowFeature.widthScript, null, null);
		var height = this._getUnit(windowFeature.height, windowFeature.heightScript, null, null);

		if (width !== null) window.dialogWidth = width + "px";
		if (height !== null) window.dialogHeight = height + "px";

		var centerLeft = windowFeature.center === true ? ((window.screen.width - width) / 2) : null;
		var centerTop = windowFeature.center === true ? ((window.screen.height - height) / 2) : null;

		var left = this._getUnit(windowFeature.left, windowFeature.leftScript, centerLeft, null);
		var top = this._getUnit(windowFeature.top, windowFeature.topScript, centerTop, null);

		if (left !== null) window.dialogLeft = left + "px";
		if (top !== null) window.dialogTop = top + "px";
	},

	_getUnit: function (unit, unitScript, nullDefault1, nullDefault2) {
		return unit !== null ? unit :
                    unitScript ? eval(unitScript) :
                    nullDefault1 !== null ? nullDefault1 :
                    nullDefault2;
	}
}

$HGRootNS.WindowFeatureFunction = new $HGRootNS._WindowFeatureFunction();

$HGRootNS._WindowCommand = function() {
	this._commandInputID = "";
}

$HGRootNS._WindowCommand.prototype =
{
	openerExecuteCommand: function(strCommand) {
		if (typeof (top.opener) === "object")
			this._executeCommand(top.opener, strCommand);
	},

	executeCommand: function(strCommand) {
		this._executeCommand(window, strCommand);
	},

	set_commandInputID: function(value) {
		this._commandInputID = value;
	},

	_executeCommand: function(win, strCommand) {
		try {
			var cmdInput = win.document.getElementById(this._commandInputID);
			if (cmdInput)
				cmdInput.value = strCommand;
			else {
				switch (strCommand.toLowerCase()) {
					case "close":
						this._close(win);
						break;
				}
			}
		}
		catch (e) {
		}
	},

	_close: function(win) {
		win.top.close();
	},

	_refresh: function(win) {
		win.location.reload();
	}
}

$HGRootNS.WindowCommand = new $HGRootNS._WindowCommand();
