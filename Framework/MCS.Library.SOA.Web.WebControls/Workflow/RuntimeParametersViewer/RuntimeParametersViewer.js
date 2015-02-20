$HBRootNS.RuntimeParametersViewer = function (element) {
	$HBRootNS.RuntimeParametersViewer.initializeBase(this, [element]);

	this._editParametersUrl = "";
	this._refreshButtonClientID = "";
}

$HBRootNS.RuntimeParametersViewer.prototype =
{
	initialize: function () {
		$HBRootNS.RuntimeParametersViewer.callBaseMethod(this, 'initialize');
	},

	dispose: function () {
		$HBRootNS.RuntimeParametersViewer.callBaseMethod(this, 'dispose');
	},

	start: function (url) {
		this._showDialog(null, url);
	},

	get_editParametersUrl: function () {
		return this._editParametersUrl;
	},

	set_editParametersUrl: function (value) {
		this._editParametersUrl = value;
	},

	get_refreshButtonClientID: function () {
		return this._refreshButtonClientID;
	},

	set_refreshButtonClientID: function (value) {
		this._refreshButtonClientID = value;
	},

	showEditParametersDialog: function () {
		if (this._editParametersUrl && this._editParametersUrl != "") {
			var sFeature = "dialogWidth:400px; dialogHeight:560px;center:yes;help:no;resizable:yes;scroll:no;status:no";

			if (window.showModalDialog(this._editParametersUrl, null, sFeature)) {
				if (this._refreshButtonClientID != "")
					$get(this._refreshButtonClientID).click();
			}
		}
	}
}

$HBRootNS.RuntimeParametersViewer.registerClass($HBRootNSName + ".RuntimeParametersViewer", $HBRootNS.DialogControlBase);