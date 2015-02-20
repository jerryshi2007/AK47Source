$HBRootNS.PredefinedOpinionDialog = function (element) {
	$HBRootNS.PredefinedOpinionDialog.initializeBase(this, [element]);

	this._opinionTextClientID = "";
	this._lastError = null;
}

$HBRootNS.PredefinedOpinionDialog.prototype =
{
	initialize: function () {
		this._initialize();
		$HBRootNS.PredefinedOpinionDialog.callBaseMethod(this, 'initialize');
	},

	dispose: function () {
		$HBRootNS.PredefinedOpinionDialog.callBaseMethod(this, 'dispose');
	},

	_initialize: function () {
	},

	_contentConvert: function (value) {
		var retVal;
		retVal = value.replace(/\s+/g, " ").replace(/\s+/g, " ");
		return retVal;
	},

	showDialog: function (userID, activityID) {
		var url = this.get_dialogUrl();

		url = url + "&userID=" + userID;

		if (url.indexOf("activityID") == -1)
			url = url + "&activityID=" + activityID;

		return this._showDialog(null, url);
	},

	save: function (text) {
		this._lastError = null;
		this._invoke("CallBackSavePredifinedOpinionMethod",
				[text],
				Function.createDelegate(this, this._onValidateSaveInvokeComplete),
				Function.createDelegate(this, this._onValidateSaveInvokeError),
				true,
				true);

		return this._lastError;
	},

	_onValidateSaveInvokeComplete: function (result) {
		return;
	},

	_onValidateSaveInvokeError: function (err) {
		this._lastError = err;
	},

	get_predefinedOpinion: function () {
		return $get(this.get_opinionTextClientID()).innerText;
	},

	get_opinionTextClientID: function () {
		return this._opinionTextClientID;
	},

	set_opinionTextClientID: function (value) {
		this._opinionTextClientID = value;
	},

	_pseudo: function () {
	}
}

$HBRootNS.PredefinedOpinionDialog.registerClass($HBRootNSName + ".PredefinedOpinionDialog", $HBRootNS.DialogControlBase);
