$HBRootNS.ExtConsignUserSelector = function (element) {
	$HBRootNS.ExtConsignUserSelector.initializeBase(this, [element]);

	this._userInputClientID = "";
	this._consignTypeSelectClientID = "";
	this._sequenceTypeSelectClientID = "";
}

$HBRootNS.ExtConsignUserSelector.prototype =
{
	initialize: function () {
		$HBRootNS.ExtConsignUserSelector.callBaseMethod(this, 'initialize');

		this._listMask = 15;
		this._selectMask = 15;
		this._multiSelect = false;
		this._isConsign = false;
	},

	dispose: function () {
		$HBRootNS.ExtConsignUserSelector.callBaseMethod(this, 'dispose');
	},

	loadClientState: function (value) {
		if (value) {
			if (value != "") {
			}
		}
	},

	get_listMask: function () {
		return this._listMask;
	},

	set_listMask: function (value) {
		this._listMask = value;
	},

	get_selectMask: function () {
		return this._selectMask;
	},

	set_selectMask: function (value) {
		this._selectMask = value;
	},

	get_multiSelect: function () {
		return this._multiSelect;
	},

	set_multiSelect: function (value) {
		this._multiSelect = value;
	},

	get_isConsign: function () {
		return this._isConsign;
	},

	set_isConsign: function (value) {
		this._isConsign = value;
	},

	get_userInputClientID: function () {
		return this._userInputClientID;
	},

	set_userInputClientID: function (value) {
		this._userInputClientID = value;
	},

	get_consignTypeSelectClientID: function () {
		return this._consignTypeSelectClientID;
	},

	set_consignTypeSelectClientID: function (value) {
		this._consignTypeSelectClientID = value;
	},

	get_sequenceTypeSelectClientID: function () {
		return this.sequenceTypeSelectClientID;
	},

	set_sequenceTypeSelectClientID: function (value) {
		this.sequenceTypeSelectClientID = value;
	},

	get_selectedOuUserData: function () {
		return $find(this.get_userInputClientID()).get_consignUsers();
	},

	get_circulateOuUserData: function () {
		return $find(this.get_userInputClientID()).get_circulators();
	},

	get_selectedConsignType: function () {
		return this._get_radioButtonValue(this.get_consignTypeSelectClientID());
	},

	get_selectedSequenceType: function () {
		return this._get_radioButtonValue(this.get_sequenceTypeSelectClientID());
	},

	_get_radioButtonValue: function (parentID) {
		var retVal = 0;
		var inputs = document.getElementsByTagName("input");

		for (var i = 0; i < inputs.length; i++) {
			if (inputs[i].parentElement.id == parentID && inputs[i].type == "radio" && inputs[i].checked) {
				retVal = parseInt(inputs[i].value);
				break;
			}
		}
		return retVal;
	},

	saveClientState: function () {

	},

	showDialog: function () {
		var params = new Object();

		var result = null;
		var resultStr = this._showDialog(params);
		if (resultStr) {
			result = Sys.Serialization.JavaScriptSerializer.deserialize(resultStr);
		}

		return result;
	},

	_pseudo: function () {
	}
}

$HBRootNS.ExtConsignUserSelector.registerClass($HBRootNSName + ".ExtConsignUserSelector", $HBRootNS.DialogControlBase);