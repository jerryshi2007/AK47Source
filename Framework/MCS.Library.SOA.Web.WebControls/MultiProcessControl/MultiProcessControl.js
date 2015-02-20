$HBRootNS.MultiProcessControl = function (element) {
	$HBRootNS.MultiProcessControl.initializeBase(this, [element]);
	this._steps = [];
	this._currentStepIndex = 0;
	this._timer = null;
	this._delayExecuteDelegate = null;
	this._addPrepareDataStep = false;
	this._prepareDataStepButtonID = null;
	this._errorMessages = [];
	this._showStepErrors = false;
	this._errorMessagesClientID = "";
}

$HBRootNS.MultiProcessControl.prototype =
{
	initialize: function () {
		$HBRootNS.MultiProcessControl.callBaseMethod(this, 'initialize');

		if (this.get_currentMode() == $HBRootNS.ControlShowingMode.Dialog) {
			var steps = new Array(window.dialogArguments.steps.length);
			for (var i = 0; i < window.dialogArguments.steps.length; i++)
				steps[i] = window.dialogArguments.steps[i];

			$get("preparedData").value = Sys.Serialization.JavaScriptSerializer.serialize(steps);
			$get("allsteps").innerText = 0;
			$get("succeed").innerText = 0;

			this._timer = $create($HGRootNS.Timer, { interval: 10, enabled: false }, null, null, null);
			this._delayExecuteDelegate = Function.createDelegate(this, this._delayExecute);
			this._timer.add_tick(this._delayExecuteDelegate);

			if (window.dialogArguments.addPrepareDataStep) {
				var button = $get(this._prepareDataStepButtonID);
				button.click();
			}
			else
				this._doSteps();
		}
	},

	dispose: function () {
		if (this._delayExecuteDelegate) {
			this._timer.remove_tick(this._delayExecuteDelegate);
			this._delayExecuteDelegate = null;
		}

		$HBRootNS.MultiProcessControl.callBaseMethod(this, 'dispose');
	},

	_doSteps: function () {
		this._steps = Sys.Serialization.JavaScriptSerializer.deserialize($get("preparedData").value);
		$get("allsteps").innerText = this._steps.length;
		$get("succeed").innerText = 0;

		this._timer.set_enabled(true);
	},

	_delayExecute: function () {//延迟执行
		try {
			if (this._currentStepIndex < this._steps.length) {
				this._invoke("OnExecuteStep", [this._steps[this._currentStepIndex]],
							Function.createDelegate(this, this._executeStepSucceed),
							Function.createDelegate(this, this._executeError));
			}
			else {
				var retValue = { error: this.get_errorMessages(), value: this._steps.length > this.get_errorMessages().length };

				var retObj = { retValue: Sys.Serialization.JavaScriptSerializer.serialize(retValue) };

				window.returnValue = retObj;

				if (retValue.error.length == 0)
					window.close();
			}
		}
		finally {
			this._timer.set_enabled(false);
		}
	},

	_executeError: function (result) {
		this._timer.set_enabled(false);

		var retValue = { error: result, value: this._currentStepIndex > 0 };

		var retObj = { retValue: Sys.Serialization.JavaScriptSerializer.serialize(retValue) };

		window.returnValue = retObj;
		window.close();
	},

	_executeStepSucceed: function (r) {//每步执行的回调
		if (r != "") {
			this._errorMessages.push(r);

			if (this._showStepErrors) {
				var output = $get(this._errorMessagesClientID);

				if (output != null) {
					var originalText = output.innerText;

					if (originalText != "")
						originalText += "\r\n";

					originalText += r;
					output.innerText = originalText;
				}
			}
		}

		this._currentStepIndex++;

		var progressBar = $get("progressBar");

		if (progressBar) {
			$get("succeed").innerText = this._currentStepIndex;
			progressBar.style.width = ((this._currentStepIndex) * 100 / this._steps.length) + "%";
			this._timer.set_enabled(true);
		}
	},

	start: function () {
		//开始执行
		var e = this.raiseBeforeStart();

		if (e.cancel == false) {
			var result = this._showDialog(e);

			if (result != null) {
				var retValue = Sys.Serialization.JavaScriptSerializer.deserialize(result.retValue);
				this._errorMessages = retValue.error;

				this.raiseFinishedProcess(retValue);
			}
			else {
				this.raiseCancelProcess();
			}
		}
	},

	add_beforeStart: function (handler) {
		this.get_events().addHandler("beforeStart", handler);
	},

	remove_beforeStart: function (handler) {
		this.get_events().removeHandler("beforeStart", handler);
	},

	raiseBeforeStart: function () {
		var handlers = this.get_events().getHandler("beforeStart");
		var e = new Sys.EventArgs();

		e.cancel = false;
		e.addPrepareDataStep = this.get_addPrepareDataStep();

		if (this._steps == null)
			e.steps = [];
		else
			e.steps = this._steps;

		if (handlers)
			handlers(e);

		return e;
	},

	add_finishedProcess: function (handler) {
		this.get_events().addHandler("finishedProcess", handler);
	},

	remove_finishedProcess: function (handler) {
		this.get_events().removeHandler("finishedProcess", handler);
	},

	raiseFinishedProcess: function (returnValue) {
		var handler = this.get_events().getHandler("finishedProcess");
		var e = new Sys.EventArgs();

		e = returnValue;

		if (handler) {
			handler(e);
		}

		return e;
	},

	add_cancelProcess: function (handler) {
		this.get_events().addHandler("cancelProcess", handler);
	},

	remove_cancelProcess: function (handler) {
		this.get_events().removeHandler("cancelProcess", handler);
	},

	raiseCancelProcess: function () {
		var handler = this.get_events().getHandler("cancelProcess");
		var e = new Sys.EventArgs();

		e = {};

		if (handler) {
			handler(e);
		}
		return e;
	},

	get_showStepErrors: function () {
		return this._showStepErrors;
	},

	set_showStepErrors: function (value) {
		this._showStepErrors = value;
	},

	get_errorMessages: function () {
		return this._errorMessages;
	},

	get_addPrepareDataStep: function () {
		return this._addPrepareDataStep;
	},

	set_addPrepareDataStep: function (value) {
		this._addPrepareDataStep = value;
	},

	get_prepareDataStepButtonID: function () {
		return this._prepareDataStepButtonID;
	},

	set_prepareDataStepButtonID: function (value) {
		this._prepareDataStepButtonID = value;
	},

	get_errorMessagesClientID: function () {
		return this._errorMessagesClientID;
	},

	set_errorMessagesClientID: function (value) {
		this._errorMessagesClientID = value;
	}
}

$HBRootNS.MultiProcessControl.registerClass($HBRootNSName + ".MultiProcessControl", $HBRootNS.DialogControlBase);
