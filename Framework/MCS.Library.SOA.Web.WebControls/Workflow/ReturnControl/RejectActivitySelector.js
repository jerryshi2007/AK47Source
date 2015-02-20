$HBRootNS.RejectActivitySelector = function(element) {
	$HBRootNS.RejectActivitySelector.initializeBase(this, [element]);

	this._activitiesListClientID = "";
}

$HBRootNS.RejectActivitySelector.prototype =
{
	initialize: function() {
		this._initialize();
		$HBRootNS.RejectActivitySelector.callBaseMethod(this, 'initialize');
	},

	dispose: function() {
		$HBRootNS.RejectActivitySelector.callBaseMethod(this, 'dispose');
	},

	_initialize: function() {
	},

	showDialog: function(arg) {
		var url = this.get_dialogUrl();

		url = url + "&activityID=" + arg.activityID;
		var resultStr = this._showDialog(arg, url);

		var result = null;

		if (resultStr) {
			result = {
				nextStep: Sys.Serialization.JavaScriptSerializer.deserialize(resultStr.nextStep),
				opinion: resultStr.opinion,
				opinionType: resultStr.opinionType
			};
		}

		return result;
	},

	get_selectedActDescKey: function() {
		var list = $get(this.get_activitiesListClientID());

		var retVal = "false";
		if (list.selectedIndex >= 0)
			retVal = list.options[list.selectedIndex].value;

		return retVal;
	},

	get_activitiesListClientID: function() {
		return this._activitiesListClientID;
	},

	set_activitiesListClientID: function(value) {
		this._activitiesListClientID = value;
	},

	_pseudo: function() {
	}
}

$HBRootNS.RejectActivitySelector.registerClass($HBRootNSName + ".RejectActivitySelector", $HBRootNS.DialogControlBase);
