$HBRootNS.WfAssigneeType = function () {
	throw Error.notImplemented();
}

$HBRootNS.WfAssigneeType.prototype = {
	Normal: 0,
	Delegated: 1
}

$HBRootNS.WfAssigneeType.changeUsersToAssignees = function (users) {
	var assignees = new Array();

	for (var i = 0; i < users.length; i++) {
		var assignee = { AssigneeType: $HBRootNS.WfAssigneeType.Normal, User: users[i] }
		assignees.push(assignee);
	}

	return assignees;
}

$HBRootNS.WfAssigneeType.registerEnum($HBRootNSName + ".WfAssigneeType");

$HBRootNS.WfActivityType = function () {
	throw Error.notImplemented();
}

$HBRootNS.WfActivityType.prototype = {
	NormalActivity: 1,
	InitialActivity: 2,
	CompletedActivity: 4
}

$HBRootNS.WfActivityType.registerEnum($HBRootNSName + ".WfActivityType");

$HBRootNS.WfControlOperationType = function () {
	throw Error.notImplemented();
}

$HBRootNS.WfControlOperationType.prototype = {
	None: 0,
	MoveTo: 1,
	Save: 2,
	Withdraw: 4,
	CancelProcess: 8,
	AdjustProcess: 16,
	ObligeEnd: 32,
	Aborted: 64,
	Return: 128,
	AddActivity: 256,
	EditActivity: 512,
	DeleteActivity: 1024,
	Startup: 2048,
	Consign: 4096,
	AddApprover: 8192
}

$HBRootNS.WfControlOperationType.registerEnum($HBRootNSName + ".WfControlOperationType");

//WfMoveToControl
$HBRootNS.WfMoveToControl = function (element) {
	$HBRootNS.WfMoveToControl.initializeBase(this, [element]);

	this._innerMoveToButtonClientID = "";
	this._innerSaveButtonClientID = "";
	this._resourceUserSelectorClientID = "";

	this._resourceUserSelector = null;
	this._autoShowResoureUserSelector = false;
	this._validateGroupWhenSave = -1;
	this._validateGroupWhenMoveTo = -1;
	this._opinionInputControlID = "";
	this._opinionTypeClientID = "";
	this._clientCheckSelectdUsers = true;
}

$HBRootNS.WfMoveToControl.refreshCurrentProcess = function () {
	if (typeof (_wfRefreshCurrentButtonID) != "undefined") {
		$get(_wfRefreshCurrentButtonID).click();
	}
	else {
		alert("_wfRefreshCurrentButtonID没有初始化！");
	}
}

$HBRootNS.WfMoveToControl.onInnerFrameStateChange = function () {
	try {
		var frm = event.srcElement;

		if (frm.readyState == "complete") {
			var html = frm.contentWindow.document.documentElement.innerHTML;

			if (html != "") {
				if (html.indexOf("WfControlBase") == -1 && frm.contentWindow.document.body.innerHTML != "") {
					win = window.open("about:blank", 'popup', 'toolbar = no, status = no, scrollbars = yes, resizable = yes');
					win.document.write(html);
					win.document.close();

					if (parent.document.all("wfOperationNotifier"))
						parent.document.all("wfOperationNotifier").value = "";
				}
			}
		}
	}
	catch (e) {
	}
}

$HBRootNS.WfMoveToControl.prototype = {
	initialize: function () {
		this._initialize();
		$HBRootNS.WfMoveToControl.callBaseMethod(this, 'initialize');

		this._resourceUserSelector = $find(this._resourceUserSelectorClientID);
	},

	dispose: function () {
		$HBRootNS.WfMoveToControl.callBaseMethod(this, 'dispose');
	},

	_initialize: function () {
		$HBRootNS.WfMoveToControl._initProcessNextSteps();
	},

	loadClientState: function (value) {
		if (value && value != "") {
			var lastNextStep = Sys.Serialization.JavaScriptSerializer.deserialize(value);

			if (lastNextStep)
				$HBRootNS.WfMoveToControl._selectedStep = lastNextStep;
		}
	},

	saveClientState: function () {
		return Sys.Serialization.JavaScriptSerializer.serialize($HBRootNS.WfMoveToControl._selectedStep);
	},

	_prepareMoveTo: function () {
		var result = this._prepareSave(true);

		try {
			if (result)
				result = this.raiseBeforeExecute($HBRootNS.WfMoveToControl._selectedStep, $HBRootNS.WfMoveToControl._currentStep);

			if (result)
				result = this._checkSelectedUser($HBRootNS.WfMoveToControl._selectedStep, $HBRootNS.WfMoveToControl._currentStep);
		}
		catch (e) {
			result = false;
			$showError(e);
		}

		return result;
	},

	_prepareSave: function (isMoveTo) {
		var result = true;

		try {
			this._validateDataBinding(isMoveTo);
		}
		catch (e) {
			$showError(e);
			result = false;
		}

		return result;
	},

	_validateDataBinding: function (isMoveTo) {
		var checkResult = { isValid: true, errorMessages: [] };
		var groupID = isMoveTo ? this._validateGroupWhenMoveTo : this._validateGroupWhenSave;

		if (typeof ($HBRootNS.DataBindingControl) != "undefined") {
			if (typeof ($HBRootNS.DataBindingControl.checkBindingControlDataByGroup) != "undefined") {
				checkResult = $HBRootNS.DataBindingControl.checkBindingControlDataByGroup(groupID);
			}
		}

		this.raiseValidateBindingData(checkResult, isMoveTo, $HBRootNS.WfMoveToControl._selectedStep);

		if (checkResult.isValid == false) {
			var message = checkResult.errorMessages.join("<br/>");

			throw Error.create(message);
		}
	},

	doMoveTo: function (opType) {
		if ($HBRootNS.WfMoveToControl._selectedStep && typeof (opType) != "undefined")
			$HBRootNS.WfMoveToControl._selectedStep.OperationType = opType;

		if (this._prepareMoveTo())
			$get(this._innerMoveToButtonClientID).click();
	},

	doSave: function () {
		if ($HBRootNS.WfMoveToControl._selectedStep)
			$HBRootNS.WfMoveToControl._selectedStep.OperationType = $HBRootNS.WfControlOperationType.Save;

		if (this._prepareSave(false))
			$get(this._innerSaveButtonClientID).click();
	},

	_checkSelectedUser: function (nextStep, currentStep) {
		var result = true;

		if (!nextStep.TargetActivityDescriptor.AllowInvalidCandidates)
			this._checkSelectObjectsStatus(nextStep.Assignees);

		var condition = nextStep.Assignees && nextStep.Assignees.length > 0 || nextStep.TargetActivityDescriptor.ActivityType == $HBRootNS.WfActivityType.CompletedActivity;

		if (!condition) {
			if (this.get_autoShowResoureUserSelector() && this._resourceUserSelector != null) {
				var selectedUsers = this._resourceUserSelector.showDialog();

				if (selectedUsers && selectedUsers.users && selectedUsers.users.length > 0) {
					nextStep.Assignees = selectedUsers.users;
					condition = nextStep.Assignees && nextStep.Assignees.length > 0 || nextStep.targetIsCompleteActivity;
					$HBRootNS.WfMoveToControl._selectedStep.extendedProperties["replaceCurrentUsers"] = "true";
				}
				else
					result = false;
			}
		}

		if (result) {
			this._falseThrow(condition == true ||
				this.get_clientCheckSelectdUsers() == false ||
				nextStep.TargetActivityDescriptor.AllowEmptyCandidates,
					$NT.getText("SOAWebControls", "没有选择下一步的流转人员"));
		}

		return result;
	},

	_getPropertyValue: function (properties, propName, defaultValue) {
		var result = defaultValue;
		for (var i = 0; i < properties.length; i++) {
			if (properties[i].name == propName) {
				result = properties[i].value;
				break;
			}
		}
		return result;
	},

	_checkSelectObjectsStatus: function (selectedObjects) {
		if (selectedObjects) {
			var strB = new Sys.StringBuilder("");

			for (var i = 0; i < selectedObjects.length; i++) {
				var obj = selectedObjects[i].User;

				if (obj.status == 3) {
					if (strB.isEmpty() == false)
						strB.append(", ");

					strB.append(obj.displayName);
				}
			}

			if (strB.isEmpty() == false)
				throw Error.create("\"" + strB.toString() + "\"" + $NT.getText("SOAWebControls", "已经被注销"));
		}
	},

	_falseThrow: function (condition, message) {
		if (!condition)
			throw Error.create(message);
	},

	get_autoShowResoureUserSelector: function () {
		return this._autoShowResoureUserSelector;
	},

	set_autoShowResoureUserSelector: function (value) {
		this._autoShowResoureUserSelector = value;
	},

	get_resourceUserSelectorClientID: function () {
		return this._resourceUserSelectorClientID;
	},

	set_resourceUserSelectorClientID: function (value) {
		this._resourceUserSelectorClientID = value;
	},

	get_innerMoveToButtonClientID: function () {
		return this._innerMoveToButtonClientID;
	},

	set_innerMoveToButtonClientID: function (value) {
		this._innerMoveToButtonClientID = value;
	},

	get_innerSaveButtonClientID: function () {
		return this._innerSaveButtonClientID;
	},

	set_innerSaveButtonClientID: function (value) {
		this._innerSaveButtonClientID = value;
	},

	get_validateGroupWhenSave: function () {
		return this._validateGroupWhenSave;
	},

	set_validateGroupWhenSave: function (value) {
		this._validateGroupWhenSave = value;
	},

	get_validateGroupWhenMoveTo: function () {
		return this._validateGroupWhenMoveTo;
	},

	set_validateGroupWhenMoveTo: function (value) {
		this._validateGroupWhenMoveTo = value;
	},

	get_opinionInputControlID: function () {
		return this._opinionInputControlID;
	},

	set_opinionInputControlID: function (value) {
		this._opinionInputControlID = value;
	},

	get_opinionTypeClientID: function () {
		return this._opinionTypeClientID;
	},

	set_opinionTypeClientID: function (value) {
		this._opinionTypeClientID = value;
	},

	get_opinionTypeControl: function () {
		var result = null;

		if (this._opinionTypeClientID != "")
			result = $get(this._opinionTypeClientID);

		return result;
	},

	get_opinionTypeText: function () {
		var text = "";

		if (this.get_opinionTypeControl())
			text = this.get_opinionTypeControl().value;

		return text;
	},

	get_opinionInputControl: function () {
		var result = null;

		if (this._opinionInputControlID != "")
			result = $get(this._opinionInputControlID);

		return result;
	},

	get_opinionInputText: function () {
		var text = "";

		if (this.get_opinionInputControl())
			text = this.get_opinionInputControl().innerText;

		return text;
	},

	get_allowEmptyOpinion: function () {
		return this._allowEmptyOpinion;
	},

	set_allowEmptyOpinion: function (value) {
		this._allowEmptyOpinion = value;
	},

	get_clientCheckSelectdUsers: function () {
		return this._clientCheckSelectdUsers;
	},

	set_clientCheckSelectdUsers: function (value) {
		this._clientCheckSelectdUsers = value;
	},

	add_beforeExecute: function (handler) {
		this.get_events().addHandler("beforeExecute", handler);
	},

	remove_beforeExecute: function (handler) {
		this.get_events().removeHandler("beforeExecute", handler);
	},

	raiseBeforeExecute: function (nextStep, currentStep) {
		var handlers = this.get_events().getHandler("beforeExecute");

		var e = new Sys.EventArgs();
		e.cancel = false;
		e.currentStep = currentStep;

		if (handlers)
			handlers(nextStep, e);

		return !e.cancel;
	},

	add_validateBindingData: function (handler) {
		this.get_events().addHandler("validateBindingData", handler);
	},

	remove_validateBindingData: function (handler) {
		this.get_events().removeHandler("validateBindingData", handler);
	},

	raiseValidateBindingData: function (checkResult, isMoveTo, nextStep) {
		var handlers = this.get_events().getHandler("validateBindingData");

		checkResult.nextStep = nextStep;

		if (handlers)
			handlers(checkResult, isMoveTo);
	},

	_pseudo: function () {
	}
}

$HBRootNS.WfMoveToControl._selectedStep = null;
$HBRootNS.WfMoveToControl._currentStep = null;
$HBRootNS.WfMoveToControl.inMoveToMode = false;

$HBRootNS.WfMoveToControl.setupProcessNextSteps = function (value) {
	if (value && value != "") {
		$HBRootNS.WfMoveToControl.processNextSteps = Sys.Serialization.JavaScriptSerializer.deserialize(value);

		if ($HBRootNS.WfMoveToControl.processNextSteps.length > 0) {
			var defaultStep = $HBRootNS.WfMoveToControl.processNextSteps[0];

			var selectedStep = {};

			selectedStep.OperationType = $HBRootNS.WfControlOperationType.MoveTo;
			selectedStep.TargetActivityDescriptor = defaultStep.ActivityDescriptor;
			selectedStep.BranchTemplate = null;
			selectedStep.Assignees = defaultStep.Candidates;
			selectedStep.Circulators = [];
			selectedStep.FromTransitionDescriptor = defaultStep.TransitionDescriptor;

			$HBRootNS.WfMoveToControl._selectedStep = selectedStep;
		}
	}
	else
		$HBRootNS.WfMoveToControl.processNextSteps = [];
}

$HBRootNS.WfMoveToControl._initProcessNextSteps = function () {
	if (typeof ($HBRootNS.WfMoveToControl.processNextSteps) == "undefined") {
		var hiddenNextSteps = $get("processNextStepsHidden");

		if (hiddenNextSteps) {
			$HBRootNS.WfMoveToControl.setupProcessNextSteps(hiddenNextSteps.value);
			hiddenNextSteps.value = "";
		}
		else
			$HBRootNS.WfMoveToControl.processNextSteps = [];

		var hiddenCurrentStep = $get("processCurrentStepsHidden");

		if (hiddenCurrentStep) {
			$HBRootNS.WfMoveToControl._currentStep = Sys.Serialization.JavaScriptSerializer.deserialize(hiddenCurrentStep.value);
			hiddenNextSteps.value = "";
		}

		var hiddenInMoveToMode = $get("processInMoveToMode");

		if (hiddenInMoveToMode) {
			$HBRootNS.WfMoveToControl.inMoveToMode = hiddenInMoveToMode.value.toLowerCase() == "true";
			hiddenInMoveToMode.value = "";
		}
	}
}

$HBRootNS.WfMoveToControl.registerClass($HBRootNSName + ".WfMoveToControl", $HGRootNS.ControlBase);

//WfProcessControlBase
$HBRootNS.WfProcessControlBase = function (element) {
	$HBRootNS.WfProcessControlBase.initializeBase(this, [element]);

	this._moveToControlClientID = "";
	this._callBackUrl = "";
	this._innerButtonClientID = "";
}

$HBRootNS.WfProcessControlBase.prototype =
{
	initialize: function () {
		$HBRootNS.WfProcessControlBase.callBaseMethod(this, 'initialize');
	},

	dispose: function () {
		$HBRootNS.WfProcessControlBase.callBaseMethod(this, 'dispose');
	},

	doInternalOperation: function () {
		this._hiddenButtonClick(true);
	},

	doExternalOperation: function () {
		var frameContainer = $get("wfExternalFrameContainer");

		if (frameContainer)
			if (this._confirm())
				frameContainer.innerHTML =
					"<iframe id='wfExternalFrame' name='wfExternalFrame' style='display:none' src='" + this._callBackUrl + "'></iframe>";

	},

	_hiddenButtonClick: function (needConfirm) {
		if (this._innerButtonClientID != "") {
			var btn = $get(this._innerButtonClientID);
			if (btn) {
				var bExec = false;

				if (needConfirm)
					bExec = this._confirm();
				else
					bExec = true;

				if (bExec)
					btn.click();
			}
		}
	},

	_confirm: function () {
		return window.confirm($NT.getText("SOAWebControls", "确认要执行吗？"));
	},

	get_callBackUrl: function () {
		return this._callBackUrl;
	},

	set_callBackUrl: function (value) {
		this._callBackUrl = value;
	},

	get_innerButtonClientID: function () {
		return this._innerButtonClientID;
	},

	set_innerButtonClientID: function (value) {
		this._innerButtonClientID = value;
	},

	get_moveToControlClientID: function () {
		return this._moveToControlClientID;
	},

	set_moveToControlClientID: function (value) {
		this._moveToControlClientID = value;
	},

	get_moveToClientControl: function () {
		var result = null;

		if (this.get_moveToControlClientID() != "")
			result = $find(this.get_moveToControlClientID());

		return result;
	},

	//Begin events
	add_operationComplete: function (handler) {
		this.get_events().addHandler("operationComplete", handler);
	},

	remove_operationComplete: function (handler) {
		this.get_events().removeHandler("operationComplete", handler);
	},

	raiseOperationComplete: function (err) {
		var handlers = this.get_events().getHandler("operationComplete");

		if (handlers) {
			handlers(this, err);
		}
		else {
			if (err) {
				$showError(err);
			}
			else {
				$HGClientMsg.alert("操作成功", "", "提示");
			}
		}
	},
	//End events

	_pseudo: function () {
	}
}

$HBRootNS.WfProcessControlBase.registerClass($HBRootNSName + ".WfProcessControlBase", $HGRootNS.ControlBase);

//WfWithdrawControl
$HBRootNS.WfWithdrawControl = function (element) {
	$HBRootNS.WfWithdrawControl.initializeBase(this, [element]);
}

$HBRootNS.WfWithdrawControl.prototype =
{
	initialize: function () {
		$HBRootNS.WfWithdrawControl.callBaseMethod(this, 'initialize');
	},

	dispose: function () {
		$HBRootNS.WfWithdrawControl.callBaseMethod(this, 'dispose');
	},

	_confirm: function () {
		return window.confirm($NT.getText("SOAWebControls", "确认要撤回吗？"));
	},
	//End events

	_pseudo: function () {
	}
}

$HBRootNS.WfWithdrawControl.registerClass($HBRootNSName + ".WfWithdrawControl", $HBRootNS.WfProcessControlBase);

$HBRootNS.WfRuntimeViewerWrapperControl = function (element) {
	$HBRootNS.WfRuntimeViewerWrapperControl.initializeBase(this, [element]);

	this._openWindowFeature = "";
	this._wfRuntimeViewerSLID = "";
	this._appLogViewUrl = "";

	this._linkPageUrl = "";
	this._useIndependentPage = false;
	this._lastSelectedProcessKey = "";
	this._advancedOpWrapperClientID = "";
	this._normalOpWrapperClientID = "";

	this._editProcessUrl = "";
	this._adminAddTransitionUrl = "";
	this._adminAddActivityUrl = "";
	this._adminDeleteObjectUrl = "";
	this._adminAutoMoveToUrl = "";

	this._processPopupMenuClientID = "";
	this._activityPopupMenuClientID = "";
	this._transitionPopupMenuClientID = "";

	this._showMainStream = false;

	this._processPopupMenu = null;
	this._activityPopupMenu = null;
	this._transitionPopupMenu = null;

	this._lastestExtraInfo = null;
	this._advancedVisible = _currentUserIsAdmin;
	this._wfRuntimeViewerWorkflowInfoHiddenFieldID = null;
}

$HBRootNS.WfRuntimeViewerWrapperControl.onProcessSelectChange = function (callerID) {
	var wrapperID = $get(callerID).getAttribute("wrapperControlID");

	var wrapper = $find(wrapperID);

	if (wrapper) {
		wrapper._changeProcess(wrapper._getSelectedProcessInfo());
	}
};

function onWfRuntimeViewerSilverlightError(sender, args) {
	var appSource = "";

	if (sender != null && sender != 0) {
		appSource = sender.getHost().Source;
	}

	var errorType = args.ErrorType;
	var iErrorCode = args.ErrorCode;

	if (errorType == "ImageError" || errorType == "MediaError") {
		return;
	}

	var errMsg = "Unhandled Error in Silverlight Application " + appSource + "\n";

	errMsg += "Code: " + iErrorCode + "    \n";
	errMsg += "Category: " + errorType + "       \n";
	errMsg += "Message: " + args.ErrorMessage + "     \n";

	if (errorType == "ParserError") {
		errMsg += "File: " + args.xamlFile + "     \n";
		errMsg += "Line: " + args.lineNumber + "     \n";
		errMsg += "Position: " + args.charPosition + "     \n";
	}
	else if (errorType == "RuntimeError") {
		if (args.lineNumber != 0) {
			errMsg += "Line: " + args.lineNumber + "     \n";
			errMsg += "Position: " + args.charPosition + "     \n";
		}
		errMsg += "MethodName: " + args.methodName + "     \n";
	}

	throw new Error(errMsg);
}

function onWfRuntimeViewerSilverlightLoaded(sender, args) {
	sender.getHost().Content.SLM.SelectedProcessInfoFuncName = "$HBRootNS.WfRuntimeViewerWrapperControl.onProcessSelectChange";
};

$HBRootNS.WfRuntimeViewerWrapperControl.prototype =
{
	initialize: function () {
		$HBRootNS.WfRuntimeViewerWrapperControl.callBaseMethod(this, 'initialize');

		if (this._processPopupMenuClientID != "") {
			this._processPopupMenu = $find(this._processPopupMenuClientID);
		}

		if (this._activityPopupMenuClientID != "") {
			this._activityPopupMenu = $find(this._activityPopupMenuClientID);
		}

		if (this._transitionPopupMenuClientID != "") {
			this._transitionPopupMenu = $find(this._transitionPopupMenuClientID);
		}

		var hidden = $get(this.get_wfRuntimeViewerWorkflowInfoHiddenFieldID());

		if (hidden != null) {
			processInfo = Sys.Serialization.JavaScriptSerializer.deserialize(hidden.value);
			this._changeProcess(processInfo);
		}
	},

	dispose: function () {
		$HBRootNS.WfRuntimeViewerWrapperControl.callBaseMethod(this, 'dispose');
	},

	get_openWindowFeature: function () {
		return this._openWindowFeature;
	},

	set_openWindowFeature: function (value) {
		this._openWindowFeature = value;
	},

	get_wfRuntimeViewerWorkflowInfoHiddenFieldID: function () {
		return this._wfRuntimeViewerWorkflowInfoHiddenFieldID;
	},

	set_wfRuntimeViewerWorkflowInfoHiddenFieldID: function (value) {
		this._wfRuntimeViewerWorkflowInfoHiddenFieldID = value;
	},

	get_appLogViewUrl: function () {
		return this._appLogViewUrl;
	},

	set_appLogViewUrl: function (value) {
		this._appLogViewUrl = value;
	},

	get_useIndependentPage: function () {
		return this._useIndependentPage;
	},

	set_useIndependentPage: function (value) {
		this._useIndependentPage = value;
	},

	get_linkPageUrl: function () {
		return this._linkPageUrl;
	},

	set_linkPageUrl: function (value) {
		this._linkPageUrl = value;
	},

	get_advancedOpWrapperClientID: function () {
		return this._advancedOpWrapperClientID;
	},

	set_advancedOpWrapperClientID: function (value) {
		this._advancedOpWrapperClientID = value;
	},

	get_normalOpWrapperClientID: function () {
		return this._normalOpWrapperClientID;
	},

	set_normalOpWrapperClientID: function (value) {
		this._normalOpWrapperClientID = value;
	},

	get_processPopupMenuClientID: function () {
		return this._processPopupMenuClientID;
	},

	set_processPopupMenuClientID: function (value) {
		this._processPopupMenuClientID = value;
	},

	get_activityPopupMenuClientID: function () {
		return this._activityPopupMenuClientID;
	},

	set_activityPopupMenuClientID: function (value) {
		this._activityPopupMenuClientID = value;
	},

	get_transitionPopupMenuClientID: function () {
		return this._transitionPopupMenuClientID;
	},

	set_transitionPopupMenuClientID: function (value) {
		this._transitionPopupMenuClientID = value;
	},

	get_showMainStream: function () {
		return this._showMainStream;
	},

	set_showMainStream: function (value) {
		this._showMainStream = value;
	},

	get_editProcessUrl: function () {
		return this._editProcessUrl;
	},

	set_editProcessUrl: function (value) {
		this._editProcessUrl = value;
	},

	get_adminAddTransitionUrl: function () {
		return this.adminAddTransitionUrl;
	},

	set_adminAddTransitionUrl: function (value) {
		this.adminAddTransitionUrl = value;
	},

	get_adminAddActivityUrl: function () {
		return this._adminAddActivityUrl;
	},

	set_adminAddActivityUrl: function (value) {
		this._adminAddActivityUrl = value;
	},

	get_adminDeleteObjectUrl: function () {
		return this._adminDeleteObjectUrl;
	},

	set_adminDeleteObjectUrl: function (value) {
		this._adminDeleteObjectUrl = value;
	},

	get_adminAutoMoveToUrl: function () {
		return this._adminAutoMoveToUrl;
	},

	set_adminAutoMoveToUrl: function (value) {
		this._adminAutoMoveToUrl = value;
	},

	_changeAdvancedControlStatusByWfInfo: function (processInfo) {
		var advancedWrapperControl = $get(this._advancedOpWrapperClientID);
		var normalWrapperControl = $get(this._normalOpWrapperClientID);
		var advancedVisible = false;
		var normalVisible = false;

		if (processInfo != null) {
			normalVisible = true;

			advancedVisible = _currentUserIsAdmin;

			if (advancedVisible == false)
				advancedVisible = _doesCurrentUserHavePermission(processInfo.ApplicationName, processInfo.ProgramName, 1);

			this._advancedVisible = advancedVisible;
		}

		if (advancedWrapperControl != null) {
			if (advancedVisible)
				advancedWrapperControl.style.display = "inline";
			else
				advancedWrapperControl.style.display = "none";
		}

		if (normalWrapperControl != null) {
			if (normalVisible)
				normalWrapperControl.style.display = "inline";
			else
				normalWrapperControl.style.display = "none";
		}
	},

	openLogView: function () {
		var url = this.get_appLogViewUrl();

		if (url && url != "") {

			var wfInfo = this._getSelectedResourceProcessID();

			if (wfInfo != '') {
				var objWfInfo = wfInfo.split(",");
				var firstChar = "?";

				if (url.indexOf("?") > 0)
					firstChar = "&";

				url = url + firstChar + "resourceID=" + encodeURIComponent(objWfInfo[0]) + "&processID=" + encodeURIComponent(objWfInfo[1]);

				window.showModalDialog(url, "",
					"dialogWidth: 800px; dialogHeight: 540px; edge: Raised; center: Yes; help: No; resizable: Yes; status: No;scroll: No;");
			}
		}
	},

	open: function (mainStream) {
		try {
			var feature = this.get_openWindowFeature();
			feature = eval("'" + feature + "'");

			var url = "";

			if (this.get_useIndependentPage() == true) {
				url = this.get_linkPageUrl();
			} else {
				url = this.get_dialogUrl();
			}

			var target = "Trace";

			if (mainStream) {
				var parameters = this._parseUrlParameters(url);
				parameters["mainStream"] = "true";

				var slc = this._getSilverlightControl();

				if (slc) {
					parameters["processID"] = slc.GetSelectedCurrentProcessKey();
					parameters["activityID"] = null;
				}

				url = this._combineUrlParameters(url, parameters);

				target = "mainStreamTrace";
			}

			window.open(url, target, feature);
		}
		catch (e) {
			$showError(e);
		}
	},

	openDesigner: function (url) {
		try {
			var processKey = this._getSilverlightControl().GetSelectedCurrentProcessKey();

			if (processKey != '') {
				url = url + "?processid=" + processKey;
			}

			//var feature = ''; eval("'" + feature + "'");

			window.open(url, "Design");
		}
		catch (e) {
			$showError(e);
		}
	},

	get_wfRuntimeViewerSLID: function () {
		return this._wfRuntimeViewerSLID;
	},

	set_wfRuntimeViewerSLID: function (value) {
		this._wfRuntimeViewerSLID = value;
	},

	getWorkflowInfo: function (processID) {
		var openedIdStr = this._getSilverlightControl().GetProcessIDList();
		var openedIdArr = openedIdStr.split(',');

		for (var i = 0; i < openedIdArr.length; i++) {
			if (openedIdArr[i] == processID) {
				this._getSilverlightControl().SetSelectedItem(processID);

				return;
			}
		}

		this._invoke("GetWorkflowInfo", [processID],
			Function.createDelegate(this, this._executeSucceed),
			Function.createDelegate(this, this._executeError));
	},

	changeActivityAssignee: function (selectedUsers, activityID) {
		var json = Sys.Serialization.JavaScriptSerializer.serialize(selectedUsers);
		this._invoke("ChangeActivityAssignee", [json, activityID],
			Function.createDelegate(this, function (r) {
				if (r == '') {
					window.location.reload();
				}
				else {
					alert($NT.getText("SOAWebControls", "修改活动操作人失败，原因：") + r);
				}
			}),
			Function.createDelegate(this, function (r) {
				alert(r);
			}));
	},

	getActivityRelatedUsers: function (activityID, userSelectorCtrlID) {
		this._invoke("GetActivityRelatedUsers", [activityID],
			Function.createDelegate(this, function (r) {
				var arg = {};

				arg.users = r;
				var result = $find(userSelectorCtrlID).showDialog(arg);
				if (result) {
					if (result.users.length > 0) {
						this.changeActivityAssignee(result.users, activityID);
					}
					else {
						alert($NT.getText("SOAWebControls", "请指定该活动资源！"));
						return;
					}
				}
			}),
			Function.createDelegate(this, function (r) {
				alert(r);
			}));
	},

	abortCurrentProcess: function () {
		this._invokeProcessOp("AbortCurrentProcess", "确定要作废或取消流程吗？", true);
	},

	restoreCurrentProcess: function () {
		this._invokeProcessOp("RestoreCurrentProcess", "确定要还原作废(取消)的流程吗？", true);
	},

	pauseCurrentProcess: function () {
		this._invokeProcessOp("PauseCurrentProcess", "确定要暂停流程吗？", true);
	},

	resumeCurrentProcess: function () {
		this._invokeProcessOp("ResumeCurrentProcess", "确定要恢复暂停的流程吗？", true);
	},

	withdrawCurrentProcess: function () {
		this._invokeProcessOp("WithdrawCurrentProcess", "确认要撤回吗？", true);
	},

	exitCurrentProcessMaintainingStatus: function () {
		this._invokeProcessOp("ExitCurrentProcessMaintainingStatus", "确认要退出维护状态吗？", true);
	},

	generateCurrentProcessCandidates: function () {
		this._invokeProcessOp("GenerateCurrentProcessCandidates", "确认要重新计算候选人吗？", false);
	},

	onChangePendingActivity: function () {
		this._invokeProcessOp("ChangeProcessPendingActivity", "", false);
	},

	openProcessParameters: function (controlID) {
		if (controlID) {
			var runtimeParameters = $find(controlID);

			if (runtimeParameters) {
				var url = runtimeParameters._dialogUrl;
				var processKey = this._getSilverlightControl().GetSelectedCurrentProcessKey();

				if (processKey != '') {
					var re = eval('/(' + "processID" + '=)([^&]*)/gi');

					url = url.replace(re, "processID" + '=' + processKey);
				}

				runtimeParameters.start(url);
			}
		}
	},

	onChangeAssigneeClick: function (userSelectorCtrlID) {
		var elementData = this._getSilverlightControl().GetCurrentTabSelectedElementData();

		if (elementData != '') {
			var nodeData = Sys.Serialization.JavaScriptSerializer.deserialize(elementData);
			if (nodeData.WfRuntimeIsComplete == true) {
				alert($NT.getText("SOAWebControls", "无法修改已完成的活动！"));
				return;
			}

			this.getActivityRelatedUsers(nodeData.InstanceID, userSelectorCtrlID);
		}
		else {
			alert($NT.getText("SOAWebControls", "请先选择需要修改的活动！"));
		}
	},

	_invokeProcessOp: function (methodName, promptText, autoClose) {
		var processInfo = this._getSelectedProcessInfo();

		if (processInfo != null) {
			var succeedMethod = this._executeProcessOpSucceedNotClose;

			if (autoClose)
				succeedMethod = this._executeProcessOpSucceed;

			if (typeof (promptText) == "undefined" || promptText == "" ||
				window.confirm($NT.getText("SOAWebControls", promptText))) {

				this._invoke(methodName,
					[processInfo.Key],
					succeedMethod,
					this._executeError);
			}
		}
	},

	_executeError: function (result) {
		$showError(result);
	},

	_executeProcessOpSucceed: function (result) {
		alert($NT.getText("SOAWebControls", "操作成功！"));

		if (window.opener) {
			window.opener.location.reload();
			window.close();
		}
	},

	_executeProcessOpSucceedNotClose: function (result) {
		alert($NT.getText("SOAWebControls", "操作成功！"));

		window.location.reload();

		if (window.opener) {
			window.opener.location.reload();
		}
	},

	_executeSucceed: function (result) {
		this._getSilverlightControl().OpenBranchProcess(result);
	},

	_getSelectedResourceProcessID: function () {
		return this._getSilverlightControl().GetSelectedCurrentProcessWfInfo();
	},

	_getSelectedProcessInfo: function () {
		var processInfo = null;

		var jsonProcessInfo = this._getSilverlightControl().GetSelectedProcessInfo();

		if (jsonProcessInfo != "")
			processInfo = Sys.Serialization.JavaScriptSerializer.deserialize(jsonProcessInfo);

		return processInfo;
	},

	_getSilverlightControl: function () {
		var slid = this.get_wfRuntimeViewerSLID();

		var result = null;

		if ($get(slid))
			result = $get(slid).Content.SLM;

		return result;
	},

	_changeProcess: function (processInfo) {
		this._changeAdvancedControlStatusByWfInfo(processInfo);

		var hidden = $get(this.get_wfRuntimeViewerWorkflowInfoHiddenFieldID());

		if (hidden != null) {
			if (processInfo)
				hidden.value = Sys.Serialization.JavaScriptSerializer.serialize(processInfo);
			else
				hidden.value = "";
		}
	},

	_showPopup: function (data) {
		if (this._editProcessUrl != null && this._editProcessUrl.length > 0) {
			var popupMenu = this._getPopupMenu(data);

			if (popupMenu != null && this._advancedVisible) {
				popupMenu.get_popupChildControl().set_positioningMode($HGRootNS.PositioningMode.RelativeTopLeft);
				popupMenu.get_popupChildControl().set_positionElementID(this.get_wfRuntimeViewerSLID());

				popupMenu.showPopupMenu(data.x, data.y);
			}
		}
	},

	_getPopupMenu: function (data) {
		var result = null;

		switch (data.srcElement) {
			case "0":
				result = this._processPopupMenu;
				this._lastestExtraInfo = this._getSelectedProcessInfo();
				break;
			case "1":
				result = this._activityPopupMenu;
				this._lastestExtraInfo = Sys.Serialization.JavaScriptSerializer.deserialize(data.extraInfo);
				break;
			case "2":
				result = this._transitionPopupMenu;
				this._lastestExtraInfo = Sys.Serialization.JavaScriptSerializer.deserialize(data.extraInfo);
				break;
			default:
				this._lastestExtraInfo = null;
				break;
		}

		return result;
	},

	editProcessProperties: function () {
		var processInfo = this._lastestExtraInfo;

		if (processInfo != null) {

			var url = this.get_editProcessUrl(url);

			if (url != "") {
				var parameters = this._parseUrlParameters();
				parameters["processID"] = processInfo.Key;
				parameters["updateTag"] = processInfo.UpdateTag;

				if (this.get_showMainStream())
					parameters["mainStream"] = "true";

				url = this._combineUrlParameters(url, parameters);

				this._showEditPropertiesDialog(url);
			}
		}
	},

	editActivityProperties: function () {
		var extraInfo = this._lastestExtraInfo;
		var processInfo = this._getSelectedProcessInfo();

		if (extraInfo != null && processInfo != null) {
			var url = this.get_editProcessUrl();

			if (url != "") {
				var parameters = this._parseUrlParameters(url);

				parameters["processID"] = processInfo.Key;
				parameters["activityKey"] = extraInfo.Key;
				parameters["updateTag"] = processInfo.UpdateTag;

				if (this.get_showMainStream())
					parameters["mainStream"] = "true";

				url = this._combineUrlParameters(url, parameters);

				this._showEditPropertiesDialog(url);
			}
		}
	},

	editTransitionProperties: function () {
		var extraInfo = this._lastestExtraInfo;
		var processInfo = this._getSelectedProcessInfo();

		if (extraInfo != null && processInfo != null) {
			var url = this.get_editProcessUrl();

			if (url != "") {
				var parameters = this._parseUrlParameters(url);

				parameters["processID"] = processInfo.Key;
				parameters["transitionKey"] = extraInfo.Key;
				parameters["updateTag"] = processInfo.UpdateTag;

				if (this.get_showMainStream())
					parameters["mainStream"] = "true";

				url = this._combineUrlParameters(url, parameters);

				this._showEditPropertiesDialog(url);
			}
		}
	},

	addTransition: function () {
		var extraInfo = this._lastestExtraInfo;
		var processInfo = this._getSelectedProcessInfo();

		if (extraInfo != null && processInfo != null) {
			var url = this.get_adminAddTransitionUrl();

			if (url != "") {
				var parameters = this._parseUrlParameters(url);

				parameters["processID"] = processInfo.Key;
				parameters["fromActivityKey"] = extraInfo.Key;
				parameters["updateTag"] = processInfo.UpdateTag;

				if (this.get_showMainStream())
					parameters["mainStream"] = "true";

				url = this._combineUrlParameters(url, parameters);

				this._showEditPropertiesDialog(url);
			}
		}
	},

	addActivity: function () {
		var extraInfo = this._lastestExtraInfo;
		var processInfo = this._getSelectedProcessInfo();

		if (extraInfo != null && processInfo != null) {
			var url = this.get_adminAddActivityUrl();

			if (url != "") {
				var parameters = this._parseUrlParameters(url);

				parameters["processID"] = processInfo.Key;
				parameters["updateTag"] = processInfo.UpdateTag;

				if (extraInfo.Key != processInfo.Key)
					parameters["fromActivityKey"] = extraInfo.Key;

				if (this.get_showMainStream())
					parameters["mainStream"] = "true";

				url = this._combineUrlParameters(url, parameters);

				this._showEditPropertiesDialog(url);
			}
		}
	},

	deleteActivity: function () {
		var extraInfo = this._lastestExtraInfo;
		var processInfo = this._getSelectedProcessInfo();

		if (extraInfo != null && processInfo != null) {
			var url = this.get_adminDeleteObjectUrl();

			if (url != "") {
				var parameters = this._parseUrlParameters(url);

				parameters["processID"] = processInfo.Key;
				parameters["updateTag"] = processInfo.UpdateTag;

				if (extraInfo.Key != processInfo.Key)
					parameters["activityKey"] = extraInfo.Key;

				if (this.get_showMainStream())
					parameters["mainStream"] = "true";

				url = this._combineUrlParameters(url, parameters);

				this._showOperationDialog(url);
			}
		}
	},

	deleteTransition: function () {
		var extraInfo = this._lastestExtraInfo;
		var processInfo = this._getSelectedProcessInfo();

		if (extraInfo != null && processInfo != null) {
			var url = this.get_adminDeleteObjectUrl();

			if (url != "") {
				var parameters = this._parseUrlParameters(url);

				parameters["processID"] = processInfo.Key;
				parameters["updateTag"] = processInfo.UpdateTag;

				if (extraInfo.Key != processInfo.Key)
					parameters["transitionKey"] = extraInfo.Key;

				if (this.get_showMainStream())
					parameters["mainStream"] = "true";

				url = this._combineUrlParameters(url, parameters);

				this._showOperationDialog(url);
			}
		}
	},

	autoMoveTo: function () {
		var processInfo = this._getSelectedProcessInfo();

		if (processInfo != null) {
			var url = this.get_adminAutoMoveToUrl();

			if (url != "") {
				var parameters = this._parseUrlParameters(url);

				parameters["processID"] = processInfo.Key;
				parameters["updateTag"] = processInfo.UpdateTag;

				if (this.get_showMainStream())
					parameters["mainStream"] = "true";

				url = this._combineUrlParameters(url, parameters);

				this._showOperationDialog(url);
			}
		}
	},

	_showEditPropertiesDialog: function (url) {
		if (window.showModalDialog(url, "",
					"dialogWidth: 540px; dialogHeight: 640px; edge: Raised; center: Yes; help: No; resizable: Yes; status: No;scroll: No;")) {

			window.location.reload();
		}
	},

	_showOperationDialog: function (url) {
		if (window.showModalDialog(url, "",
					"dialogWidth: 360px; dialogHeight: 240px; edge: Raised; center: Yes; help: No; resizable: No; status: No;scroll: No;")) {

			window.location.reload();
		}
	},

	_pseudo: function () {
	}
}

$HBRootNS.WfRuntimeViewerWrapperControl.registerClass($HBRootNSName + ".WfRuntimeViewerWrapperControl", $HBRootNS.DialogControlBase);

$HBRootNS.WfRuntimeViewerWrapperControl.registerHoverHelperOne = function (elem) {
	if (typeof (elem) !== 'undefined' && elem.nodeType === 1) {
		$addHandler(elem, 'mouseenter', function () { Sys.UI.DomElement.addCssClass(elem, "hover"); }, true);
		$addHandler(elem, 'mouseover', function () { Sys.UI.DomElement.addCssClass(elem, "hover"); }, true);
		$addHandler(elem, 'mouseout', function () { Sys.UI.DomElement.removeCssClass(elem, "hover"); }, true);
	}
}

$HBRootNS.WfRuntimeViewerWrapperControl.registerHoverHelper = function () {
	var len = arguments.length;
	if (len) {
		for (var i = 0; i < len; i++) {
			var elem = document.getElementById(arguments[i]);
			if (elem) {
				$HBRootNS.WfRuntimeViewerWrapperControl.registerHoverHelperOne(elem);
			}
		}
	}
}

$HBRootNS.WfRuntimeViewerWrapperControl.PopupMenus = [];

$HBRootNS.WfRuntimeViewerWrapperControl.registerPopupMenu = function (slid, wrapperID) {
	var subscriber = { SLID: slid, WrapperID: wrapperID };

	$HBRootNS.WfRuntimeViewerWrapperControl.PopupMenus.push(subscriber);
}

$HBRootNS.WfRuntimeViewerWrapperControl.showPopupMenu = function (slid, data) {
	if (data && data.eventType == "1") {
		for (var i = 0; i < $HBRootNS.WfRuntimeViewerWrapperControl.PopupMenus.length; i++) {
			var subscriber = $HBRootNS.WfRuntimeViewerWrapperControl.PopupMenus[i];

			if (subscriber.SLID == slid) {
				var wrapperControl = $find(subscriber.WrapperID);

				if (wrapperControl)
					wrapperControl._showPopup(data);
			}
		}
	}
}

$HBRootNS.WfConsignControl = function (element) {
	$HBRootNS.WfConsignControl.initializeBase(this, [element]);

	this._userSelectorClientID = "";
	this._moveToControlClientID = "";
}

$HBRootNS.WfConsignControl.prototype = {
	initialize: function () {
		this._initialize();
		$HBRootNS.WfConsignControl.callBaseMethod(this, 'initialize');
	},

	dispose: function () {
		$HBRootNS.WfConsignControl.callBaseMethod(this, 'dispose');
	},

	_initialize: function () {
	},

	get_userSelectorClientID: function () {
		return this._userSelectorClientID;
	},

	set_userSelectorClientID: function (value) {
		this._userSelectorClientID = value;
	},

	get_moveToControlClientID: function () {
		return this._moveToControlClientID;
	},

	set_moveToControlClientID: function (value) {
		this._moveToControlClientID = value;
	},

	_doOperation: function () {
		var retVal = $find(this.get_userSelectorClientID()).showDialog();

		if (retVal && retVal.userInfo.length > 0) {
			$HBRootNS.WfMoveToControl._selectedStep.OperationType = $HBRootNS.WfControlOperationType.Consign;

			$HBRootNS.WfMoveToControl._selectedStep.BlockingType = retVal["consignType"];
			$HBRootNS.WfMoveToControl._selectedStep.SequenceType = retVal["sequenceType"];

			var selectedUsers = retVal["userInfo"];
			var circulateUsers = retVal["circulateUserInfo"];

			if (selectedUsers && selectedUsers.length > 0) {
				$HBRootNS.WfMoveToControl._selectedStep.Assignees = $HBRootNS.WfAssigneeType.changeUsersToAssignees(selectedUsers);

				if (!circulateUsers)
					circulateUsers = [];

				$HBRootNS.WfMoveToControl._selectedStep.Circulators = $HBRootNS.WfAssigneeType.changeUsersToAssignees(circulateUsers);

				var moveToControl = $find(this._moveToControlClientID);
				if (moveToControl)
					moveToControl.doMoveTo($HBRootNS.WfControlOperationType.Consign);
			}
		}
	},

	_pseudo: function () {
	}
}

$HBRootNS.WfConsignControl.registerClass($HBRootNSName + ".WfConsignControl", $HGRootNS.ControlBase);

//WfCirculateControl
$HBRootNS.WfCirculateControl = function (element) {
	$HBRootNS.WfCirculateControl.initializeBase(this, [element]);

	this._userInputClientID = "";
	this._selectedResult = null;
}

$HBRootNS.WfCirculateControl.prototype =
{
	initialize: function () {
		$HBRootNS.WfCirculateControl.callBaseMethod(this, 'initialize');
	},

	dispose: function () {
		$HBRootNS.WfCirculateControl.callBaseMethod(this, 'dispose');
	},

	_confirm: function () {
		this._selectedResult = $find(this.get_userInputClientID()).showDialog(null);

		if (this._selectedResult != null && this._selectedResult.result == true) {
			return this._selectedResult.users.length > 0;
		} else
			return false;
	},

	saveClientState: function () {
		if (this._selectedResult != null && this._selectedResult.result == true)
			return Sys.Serialization.JavaScriptSerializer.serialize(this._selectedResult);
	},

	get_userInputClientID: function () {
		return this._userInputClientID;
	},

	set_userInputClientID: function (value) {
		this._userInputClientID = value;
	},

	_pseudo: function () {
	}
}

$HBRootNS.WfCirculateControl.registerClass($HBRootNSName + ".WfCirculateControl", $HBRootNS.WfProcessControlBase);

//WfRejectMode
$HBRootNS.WfRejectMode = function () {
	throw Error.notImplemented();
}

$HBRootNS.WfRejectMode.prototype =
{
	SelectRejectStep: 1,
	//RejectToPreviousStep: 2,
	RejectToDrafter: 4,
	LikeAddApprover: 8
}

$HBRootNS.WfRejectMode.registerEnum($HBRootNSName + ".WfRejectMode");


//WfReturnControl
$HBRootNS.WfReturnControl = function (element) {
	$HBRootNS.WfReturnControl.initializeBase(this, [element]);

	this._activityID = "";
	this._activitySelectorClientID = "";
	this._moveToControlClientID = "";

	this._rejectMode = $HBRootNS.WfRejectMode.SelectRejectStep;
	this._rejectNextStep = null;
}

$HBRootNS.WfReturnControl.prototype = {
	initialize: function () {
		this._initialize();
		$HBRootNS.WfReturnControl.callBaseMethod(this, 'initialize');
	},

	dispose: function () {
		$HBRootNS.WfReturnControl.callBaseMethod(this, 'dispose');
	},

	_initialize: function () {
	},

	loadClientState: function (value) {
		if (value && value != "") {
			this._rejectNextStep = Sys.Serialization.JavaScriptSerializer.deserialize(value);
		}
	},

	get_activityID: function () {
		return this._activityID;
	},

	set_activityID: function (value) {
		this._activityID = value;
	},

	get_activitySelectorClientID: function () {
		return this._activitySelectorClientID;
	},

	set_activitySelectorClientID: function (value) {
		this._activitySelectorClientID = value;
	},

	get_moveToControlClientID: function () {
		return this._moveToControlClientID;
	},

	set_moveToControlClientID: function (value) {
		this._moveToControlClientID = value;
	},

	get_rejectMode: function () {
		return this._rejectMode;
	},

	set_rejectMode: function (value) {
		this._rejectMode = value;
	},

	_doOperation: function () {
		var nextStep = null;
		// & $HBRootNS.WfRejectMode.SelectRejectStep
		var moveToControl = $find(this._moveToControlClientID);
		if (moveToControl) {
			if ((this.get_rejectMode()) != 0) {

				var arg = { activityID: this.get_activityID(), opinion: moveToControl.get_opinionInputText(), allowEmptyOpinion: moveToControl.get_allowEmptyOpinion() };
				var result = $find(this.get_activitySelectorClientID()).showDialog(arg);

				if (result) {
					nextStep = result.nextStep;

					var opinionInput = moveToControl.get_opinionInputControl();

					if (opinionInput != null) {
						opinionInput.value = result.opinion;
					}

					var opinionType = moveToControl.get_opinionTypeControl();

					if (opinionType != null) {
						opinionType.value = result.opinionType;
					}
				}
			}
			else {
				if (window.confirm($NT.getText("SOAWebControls", "确定要回退吗？")))
					nextStep = this._rejectNextStep;
			}
			if (nextStep) {
				//$HBRootNS.WfMoveToControl._selectedStep = nextStep;

				//if ((this.get_rejectMode() & $HBRootNS.WfRejectMode.LikeAddApprover) != 0)
				$HBRootNS.WfMoveToControl._selectedStep.RejectMode = this.get_rejectMode();
				$HBRootNS.WfMoveToControl._selectedStep.TargetActivityDescriptor = nextStep.ActivityDescriptor;
				$HBRootNS.WfMoveToControl._selectedStep.OperationType = $HBRootNS.WfControlOperationType.Return;
				$HBRootNS.WfMoveToControl._selectedStep.Assignees = nextStep.Candidates;
				moveToControl.doMoveTo($HBRootNS.WfControlOperationType.Return);
			}
		}
	},

	_pseudo: function () {
	}
}

$HBRootNS.WfReturnControl.registerClass($HBRootNSName + ".WfReturnControl", $HBRootNS.WfProcessControlBase);

$HBRootNS.WfAddApproverControl = function (element) {
	$HBRootNS.WfAddApproverControl.initializeBase(this, [element]);

	this._userInputClientID = "";
	this._selectedResult = null;
}

$HBRootNS.WfAddApproverControl.prototype = {

	initialize: function () {
		$HBRootNS.WfAddApproverControl.callBaseMethod(this, 'initialize');
	},

	dispose: function () {
		$HBRootNS.WfAddApproverControl.callBaseMethod(this, 'dispose');
	},

	get_userInputClientID: function () {
		return this._userInputClientID;
	},

	set_userInputClientID: function (value) {
		this._userInputClientID = value;
	},

	get_activityID: function () {
		return this._activityID;
	},

	set_activityID: function (value) {
		this._activityID = value;
	},

	get_moveToControlClientID: function () {
		return this._moveToControlClientID;
	},

	set_moveToControlClientID: function (value) {
		this._moveToControlClientID = value;
	},

	_confirm: function () {
		var result = false;

		var moveToControl = $find(this.get_moveToControlClientID());

		if (moveToControl) {

			var arg = { opinion: moveToControl.get_opinionInputText(), allowEmptyOpinion: moveToControl.get_allowEmptyOpinion() };

			this._selectedResult = $find(this.get_userInputClientID()).showDialog(arg);

			if (this._selectedResult != null && this._selectedResult.result == true) {
				result = this._selectedResult.users.length > 0;

				if (result == true) {
					var opinionInput = moveToControl.get_opinionInputControl();

					if (opinionInput != null) {
						opinionInput.value = this._selectedResult.opinion;
					}

					var opinionType = moveToControl.get_opinionTypeControl();

					if (opinionType != null) {
						opinionType.value = this._selectedResult.opinionType;
					}

					//					var nextStep = this._selectedResult.nextStep;
					//					$HBRootNS.WfMoveToControl._selectedStep.TargetActivityDescriptor = nextStep.ActivityDescriptor;
					//					$HBRootNS.WfMoveToControl._selectedStep.OperationType = $HBRootNS.WfControlOperationType.AddApprover;
					//					$HBRootNS.WfMoveToControl._selectedStep.Assignees = nextStep.Candidates;
					$HBRootNS.WfMoveToControl._selectedStep.Assignees = this._selectedResult;
					//moveToControl.doMoveTo($HBRootNS.WfControlOperationType.AddApprover);
				}
			}
		}

		return result;
	},

	saveClientState: function () {
		if (this._selectedResult != null && this._selectedResult.result == true)
			return Sys.Serialization.JavaScriptSerializer.serialize(this._selectedResult);
	},

	_pseudo: function () {
	}
}

$HBRootNS.WfAddApproverControl.registerClass($HBRootNSName + ".WfAddApproverControl", $HBRootNS.WfProcessControlBase);

//WfAbortControl
$HBRootNS.WfAbortControl = function (element) {
	$HBRootNS.WfAbortControl.initializeBase(this, [element]);

	this._needAbortReason = false;
	this._opinionInputDialogClientID = "";
	this._opinionText = "";
	this._opinionType = "";
}

$HBRootNS.WfAbortControl.prototype =
{
	initialize: function () {
		$HBRootNS.WfAbortControl.callBaseMethod(this, 'initialize');
	},

	dispose: function () {
		$HBRootNS.WfAbortControl.callBaseMethod(this, 'dispose');
	},

	_confirm: function () {
		var confirmed = false;
		this._opinionText = "";
		this._opinionType = "";

		if (this._needAbortReason) {
			var dialog = $find(this._opinionInputDialogClientID);

			var result = dialog.showDialog();

			if (result != null) {
				confirmed = true;
				this._opinionText = result.opinion;
				this._opinionType = result.opinionType;
			}
		}
		else {
			confirmed = window.confirm($NT.getText("SOAWebControls", "确认要作废或取消流程吗？"));
		}

		return confirmed;
	},

	get_needAbortReason: function () {
		return this._needAbortReason;
	},

	set_needAbortReason: function (value) {
		this._needAbortReason = value;
	},

	get_opinionInputDialogClientID: function () {
		return this._opinionInputDialogClientID;
	},

	set_opinionInputDialogClientID: function (value) {
		this._opinionInputDialogClientID = value;
	},

	saveClientState: function () {
		var state = new Array(this._opinionText, this._opinionType);

		return Sys.Serialization.JavaScriptSerializer.serialize(state);
	},
	//End events

	_pseudo: function () {
	}
}

$HBRootNS.WfAbortControl.registerClass($HBRootNSName + ".WfAbortControl", $HBRootNS.WfProcessControlBase);

//WfPauseControl
$HBRootNS.WfPauseControl = function (element) {
	$HBRootNS.WfPauseControl.initializeBase(this, [element]);

	this._operationText = "";
}

$HBRootNS.WfPauseControl.prototype =
{
	initialize: function () {
		$HBRootNS.WfPauseControl.callBaseMethod(this, 'initialize');
	},

	dispose: function () {
		$HBRootNS.WfPauseControl.callBaseMethod(this, 'dispose');
	},

	get_operationText: function () {
		return this._operationText;
	},

	set_operationText: function (value) {
		this._operationText = value;
	},

	_confirm: function () {
		return window.confirm($NT.getText("SOAWebControls", "确认要" + this._operationText + "流程吗？"));
	},

	_pseudo: function () {
	}
}

$HBRootNS.WfPauseControl.registerClass($HBRootNSName + ".WfPauseControl", $HBRootNS.WfProcessControlBase);

$HBRootNS.WfProcessControlBase.close = function () {
	try {
		var wb = WebBrowserWrapperFactory.Create();

		wb.close();
	}
	catch (e) {
		window.close();
	}
}