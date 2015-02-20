function onEditActivityClick(editorID, submitButtonID, hiddenValueID, op, smallMode) {
	var elem = event.srcElement;

	if (op == "delete") {
		if (window.confirm($NT.getText("SOAWebControls", "确认删除该环节吗？"))) {
			var data = {};
			data.operation = op;
			data.currentActivityKey = elem.currentActivityKey;
			data.users = [];

			$get(hiddenValueID).value = Sys.Serialization.JavaScriptSerializer.serialize(data);
			$get(submitButtonID).click();
		}
	}
	else {
		var result = null;

		if (smallMode)
			result = $find(editorID).showSmallDialog(elem.currentActivityKey, op);
		else
			result = $find(editorID).showDialog(elem.currentActivityKey, op);

		if (result) {
			$get(hiddenValueID).value = result;
			$HBRootNS.HBCommon.executeValidators = false;
			$get(submitButtonID).click();
		}
	}
}

function onRefreshProcessButtonClick() {
	$HBRootNS.HBCommon.executeValidators = false;
}

function changeProcessDescriptor(serializedData) {
	$get(opinionListViewHiddenDataID).value = serializedData;
	$get(opinionListViewChangeProcessButtonID).click();
}

function adjustProcessDescriptorEndRequestHandler(sender, args) {

	$HBRootNS.HBCommon.executeValidators = true;
	SubmitButton.resetAllStates();

	if (args.get_error() != undefined) {
		args.set_errorHandled(true);
		$showError(args.get_error());
	}
	else {
		var hidden = $get("adjustNextStepsHidden");

		if (hidden) {
			$HBRootNS.WfMoveToControl.setupProcessNextSteps(hidden.value);
			hidden.value = "";
		}
	}
}

var m_opinionValidators = [];

function pushOpinionValidator(validator) {
	var exists = false;

	for (var i = 0; i < m_opinionValidators.length; i++) {
		if (validator.opinionInputID == m_opinionValidators[i].opinionInputID) {
			exists = true;
			break;
		}
	}

	if (exists == false) {
		m_opinionValidators.push(validator);
	}
}

function checkOpinionEmpty(checkResult, isMoveTo) {
	var isValid = true;

	if (typeof (isMoveTo) == "undefined")
		isMoveTo = true;

	if (isMoveTo) {
		for (var i = 0; i < m_opinionValidators.length; i++) {
			var opinionInput = $get(m_opinionValidators[i].opinionInputID);

			if (opinionInput) {
				if (opinionInput.innerText == "") {
					isValid = false;
					checkResult.errorMessages.push(m_opinionValidators[i].errorMessage);
				}
			}
		}
	}

	checkResult.isValid = checkResult.isValid & isValid;
}

var existsAddedValidators = {};

function addMoveToValidator(moveToCtrlID) {

	if (!existsAddedValidators[moveToCtrlID]) {
		var moveToCtrl = $find(moveToCtrlID);

		if (moveToCtrl)
			moveToCtrl.add_validateBindingData(checkOpinionEmpty);

		existsAddedValidators[moveToCtrlID] = moveToCtrlID;
	}
}
