$HBRootNS.WfSimpleActivityDescriptorEditor = function (element) {
	$HBRootNS.WfSimpleActivityDescriptorEditor.initializeBase(this, [element]);

	this._userInputClientID = "";
	this._actNameClientID = "";
	this._operation = "";
	this._allAgreeWhenConsignCheckboxClientID = "";
	this._currentActivityKey = "";
	this._smallDialogUrl = "";
	this._smallDialogFeature = "";
	this._userInputControl = null;
	this._maximizeAssigneeCount = -1;
}

$HBRootNS.WfSimpleActivityDescriptorEditor.prototype =
{
	initialize: function () {
		this._initialize();
		$HBRootNS.WfSimpleActivityDescriptorEditor.callBaseMethod(this, 'initialize');

		this._userInputControl = $find(this.get_userInputClientID());
	},

	dispose: function () {
		$HBRootNS.WfSimpleActivityDescriptorEditor.callBaseMethod(this, 'dispose');
	},

	_initialize: function () {
	},

	showDialog: function (currentActivityKey, op) {
		var url = this.get_dialogUrl();

		url = url + "&currentActivityKey=" + currentActivityKey + "&op=" + op;

		return this._showDialog(null, url);
	},

	showSmallDialog: function (currentActivityKey, op) {
		var url = this.get_smallDialogUrl();

		url = url + "&currentActivityKey=" + currentActivityKey + "&op=" + op;

		return window.showModalDialog(url, null, this.get_smallDialogFeature());
	},

	get_activityDescriptorInfo: function () {
		var result = {};

		result.operation = this._operation;
		result.currentActivityKey = this.get_currentActivityKey();
		result.allAgreeWhenConsign = false;
		result.name = $get(this.get_actNameClientID()).value;
		result.users = this._userInputControl.get_selectedOuUserData();
		result.circulateUsers = [];

		var variableList = document.getElementsByTagName("ul");

		if (variableList.length > 0) {
			variableCheckBoxes = variableList[0].getElementsByTagName("input");
			result.variables = new Array();

			for (var i = 0; i < variableCheckBoxes.length; i++) {
				var content = {};
				content["Key"] = variableCheckBoxes[i].value;
				content["OriginalValue"] = variableCheckBoxes[i].checked;
				content["OriginalType"] = 4;
				result.variables[i] = content;
			}
		}

		if (this.get_maximizeAssigneeCount() != -1) {
			if (result.users.length > this.get_maximizeAssigneeCount())
				throw Error.create(String.format("选择的用户数不能超过{0}位", this.get_maximizeAssigneeCount()));
		}

		return result;
	},

	get_smallDialogFeature: function () {
		return this._smallDialogFeature;
	},

	set_smallDialogFeature: function (value) {
		this._smallDialogFeature = value;
	},

	get_smallDialogUrl: function () {
		return this._smallDialogUrl;
	},

	set_smallDialogUrl: function (value) {
		this._smallDialogUrl = value;
	},

	get_actNameClientID: function () {
		return this._actNameClientID;
	},

	set_actNameClientID: function (value) {
		this._actNameClientID = value;
	},

	get_userInputClientID: function () {
		return this._userInputClientID;
	},

	set_userInputClientID: function (value) {
		this._userInputClientID = value;
	},

	get_operation: function () {
		return this._operation;
	},

	set_operation: function (value) {
		this._operation = value;
	},

	get_currentActivityKey: function () {
		return this._currentActivityKey;
	},

	set_currentActivityKey: function (value) {
		this._currentActivityKey = value;
	},

	get_allAgreeWhenConsignCheckboxClientID: function () {
		return this.allAgreeWhenConsignCheckboxClientID;
	},

	set_allAgreeWhenConsignCheckboxClientID: function (value) {
		this.allAgreeWhenConsignCheckboxClientID = value;
	},

	get_maximizeAssigneeCount: function () {
		return this._maximizeAssigneeCount;
	},

	set_maximizeAssigneeCount: function (value) {
		this._maximizeAssigneeCount = value;
	},

	_pseudo: function () {
	}
}

$HBRootNS.WfSimpleActivityDescriptorEditor.registerClass($HBRootNSName + ".WfSimpleActivityDescriptorEditor", $HBRootNS.DialogControlBase);