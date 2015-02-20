$HBRootNS.WfRuntimeViewerDialog = function (element) {
	$HBRootNS.WfRuntimeViewerDialog.initializeBase(this, [element]);
}

$HBRootNS.WfRuntimeViewerDialog.prototype =
{
	initialize: function () {
		$HBRootNS.WfRuntimeViewerDialog.callBaseMethod(this, 'initialize');

		if (this.get_currentMode() == $HBRootNS.ControlShowingMode.Dialog) {
			if (window.dialogArguments)
			{ }
		}
	},

	dispose: function () {
		$HBRootNS.WfRuntimeViewerDialog.callBaseMethod(this, 'dispose');
	},

    start: function (url) {
		var result = this._showDialog(null,url);
		if (result != null) {
		   return result;
		}
	}
}

$HBRootNS.WfRuntimeViewerDialog.registerClass($HBRootNSName + ".WfRuntimeViewerDialog", $HBRootNS.DialogControlBase);
