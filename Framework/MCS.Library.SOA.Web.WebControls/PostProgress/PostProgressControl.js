$HBRootNS.PostProgressControl = function (element) {
	$HBRootNS.PostProgressControl.initializeBase(this, [element]);

	this._dataSelectorControlClientID = null;
	this._dataSelector = null;
	this._clientExtraPostedData = "";
}

$HBRootNS.PostProgressControl.prototype =
{
	initialize: function () {
		$HBRootNS.PostProgressControl.callBaseMethod(this, 'initialize');
	},

	dispose: function () {
		$HBRootNS.PostProgressControl.callBaseMethod(this, 'dispose');
	},

	get_dataSelectorControlClientID: function () {
		return this._dataSelectorControlClientID;
	},

	set_dataSelectorControlClientID: function (value) {
		this._dataSelectorControlClientID = value;
	},

	_get_dataSelector: function () {
		if (this._dataSelector == null) {
			if (this._dataSelectorControlClientID != null && this._dataSelectorControlClientID != "")
				this._dataSelector = $find(this._dataSelectorControlClientID);
		}

		return this._dataSelector;
	},

	_prepareDataFromSelector: function () {
		var result = [];
		var dataselector = this._get_dataSelector();

		if (dataselector != null) {
			if (typeof (dataselector.get_clientSelectedKeys) != "undefined") {
				result = dataselector.get_clientSelectedKeys();
			}
		}

		return result;
	},

	get_clientExtraPostedData: function () {
		return this._clientExtraPostedData;
	},

	set_clientExtraPostedData: function (value) {
		this._clientExtraPostedData = value;
	},

	loadClientState: function (value) {
		if (value) {
			if (value != "") {
			}
		}
	},

	saveClientState: function () {

	},

	showDialog: function () {
		var e = this.raiseBeforeStart();
		var result = null;

		if (e.cancel === false && e.steps.length > 0) {
			var arg = { 
				serializedParams: Sys.Serialization.JavaScriptSerializer.serialize(e.steps),
				clientExtraPostedData: e.clientExtraPostedData
			};

			result = this._showDialog(arg);

			this.raiseCompleted(result);
		}

		return result;
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
		e.steps = this._prepareDataFromSelector();
		e.clientExtraPostedData = this._clientExtraPostedData;

		if (handlers)
			handlers(e);

		return e;
	},

	add_completed: function (handler) {
		this.get_events().addHandler("completed", handler);
	},

	remove_completed: function (handler) {
		this.get_events().removeHandler("completed", handler);
	},

	raiseCompleted: function (dataChanged) {
		var handlers = this.get_events().getHandler("completed");
		var e = new Sys.EventArgs();

		e.dataChanged = dataChanged;

		if (handlers)
			handlers(e);

		return e;
	},

	_pseudo: function () {
	}
}

$HBRootNS.PostProgressControl.registerClass($HBRootNSName + ".PostProgressControl", $HBRootNS.DialogControlBase);