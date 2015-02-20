$HBRootNS.OpinionInputDialog = function(element) {
	$HBRootNS.OpinionInputDialog.initializeBase(this, [element]);

	this._opinionTextClientID = "";
	this._lastError = null;
}

$HBRootNS.OpinionInputDialog.prototype =
{
	initialize: function() {
		this._initialize();
		$HBRootNS.OpinionInputDialog.callBaseMethod(this, 'initialize');
	},

	dispose: function() {
		$HBRootNS.OpinionInputDialog.callBaseMethod(this, 'dispose');
	},

	_initialize: function() {
	},

	showDialog: function() {
		var dialogResult = this._showDialog();

		var result = null;

		if (dialogResult)
			result = Sys.Serialization.JavaScriptSerializer.deserialize(dialogResult);

		return result;
	},

	_pseudo: function() {
	}
}

$HBRootNS.OpinionInputDialog.registerClass($HBRootNSName + ".OpinionInputDialog", $HBRootNS.DialogControlBase);
