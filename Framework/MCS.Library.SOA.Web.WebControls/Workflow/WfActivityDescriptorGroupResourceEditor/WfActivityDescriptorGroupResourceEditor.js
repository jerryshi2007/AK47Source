$HBRootNS.WfActivityDescriptorGroupResourceEditor = function(element) {
	$HBRootNS.WfActivityDescriptorGroupResourceEditor.initializeBase(this, [element]);

	this._userInputClientID = "";
	this._circulateUserInputClientID = "";
	this._actNameClientID = "";
	this._operation = "";
	this._allAgreeWhenConsignCheckboxClientID = "";
	this._currentActivityKey = "";
	this._smallDialogUrl = "";
	this._smallDialogFeature = "";
}

$HBRootNS.WfActivityDescriptorGroupResourceEditor.prototype =
{
	initialize: function() {
		this._initialize();
		$HBRootNS.WfActivityDescriptorGroupResourceEditor.callBaseMethod(this, 'initialize');
	},

	dispose: function() {
		$HBRootNS.WfActivityDescriptorGroupResourceEditor.callBaseMethod(this, 'dispose');
	},

	_initialize: function() {
	},

	showDialog: function(currentActivityKey, op) {
		var url = this.get_dialogUrl();

		url = url + "&currentActivityKey=" + currentActivityKey + "&op=" + op;

		return this._showDialog(null, url);
	},

	showSmallDialog: function(currentActivityKey, op) {
		var url = this.get_smallDialogUrl();

		url = url + "&currentActivityKey=" + currentActivityKey + "&op=" + op;

		return window.showModalDialog(url, null, this.get_smallDialogFeature());
	},

	get_activityDescriptorInfo: function() {
		var result = {};

		result.operation = this._operation;
		result.currentActivityKey = this.get_currentActivityKey();

		if ($get(this.get_allAgreeWhenConsignCheckboxClientID()))
			result.allAgreeWhenConsign = $get(this.get_allAgreeWhenConsignCheckboxClientID()).checked;
		else
			result.allAgreeWhenConsign = false;

		if ($get(this.get_actNameClientID()))
			result.name = $get(this.get_actNameClientID()).value;

		result.users = $find(this.get_userInputClientID()).get_selectedObjects();

		var circulateUsersControl = $find(this.get_circulateUserInputClientID());

		if (circulateUsersControl)
			result.circulateUsers = circulateUsersControl.get_selectedOuUserData();

		var variableList = document.getElementsByTagName("ul");

		variableCheckBoxes = variableList[0].getElementsByTagName("input");
		result.variables = new Array();

		for (var i = 0; i < variableCheckBoxes.length; i++) {
			var content = {};
			content["Key"] = variableCheckBoxes[i].value;
			content["OriginalValue"] = variableCheckBoxes[i].checked;
			content["OriginalType"] = 4;
			result.variables[i] = content;
		}

		return result;
	},

	get_smallDialogFeature: function() {
		return this._smallDialogFeature;
	},

	set_smallDialogFeature: function(value) {
		this._smallDialogFeature = value;
	},

	get_smallDialogUrl: function() {
		return this._smallDialogUrl;
	},

	set_smallDialogUrl: function(value) {
		this._smallDialogUrl = value;
	},

	get_actNameClientID: function() {
		return this._actNameClientID;
	},

	set_actNameClientID: function(value) {
		this._actNameClientID = value;
	},

	get_userInputClientID: function() {
		return this._userInputClientID;
	},

	set_userInputClientID: function(value) {
		this._userInputClientID = value;
	},

	get_circulateUserInputClientID: function() {
		return this._circulateUserInputClientID;
	},

	set_circulateUserInputClientID: function(value) {
		this._circulateUserInputClientID = value;
	},

	get_operation: function() {
		return this._operation;
	},

	set_operation: function(value) {
		this._operation = value;
	},

	get_currentActivityKey: function() {
		return this._currentActivityKey;
	},

	set_currentActivityKey: function(value) {
		this._currentActivityKey = value;
	},

	get_allAgreeWhenConsignCheckboxClientID: function() {
		return this.allAgreeWhenConsignCheckboxClientID;
	},

	set_allAgreeWhenConsignCheckboxClientID: function(value) {
		this.allAgreeWhenConsignCheckboxClientID = value;
	},

	_pseudo: function() {
	}
}

$HBRootNS.WfActivityDescriptorGroupResourceEditor.registerClass($HBRootNSName + ".WfActivityDescriptorGroupResourceEditor", $HBRootNS.DialogControlBase);