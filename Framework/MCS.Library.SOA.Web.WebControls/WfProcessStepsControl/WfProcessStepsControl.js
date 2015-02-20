$HBRootNS.WfProcessStepsControl = function (element) {
	$HBRootNS.WfProcessStepsControl.initializeBase(this, [element]);
}

$HBRootNS.WfProcessStepsControl.prototype =
{
	initialize: function () {
		$HBRootNS.WfProcessStepsControl.callBaseMethod(this, "initialize");
	},

	dispose: function () {
		$HBRootNS.WfProcessStepsControl.callBaseMethod(this, "dispose");
	},

	pseudo: function () {
	}
}

$HBRootNS.WfProcessStepsControl.registerClass($HBRootNSName + ".WfProcessStepsControl", $HGRootNS.ControlBase);