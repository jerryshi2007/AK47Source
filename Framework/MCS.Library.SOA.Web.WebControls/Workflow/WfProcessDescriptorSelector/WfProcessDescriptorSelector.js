$HBRootNS.WfProcessDescriptorSelector = function (element) {
    this.multiSelect = false;
    $HBRootNS.WfProcessDescriptorSelector.initializeBase(this, [element]);
}

$HBRootNS.WfProcessDescriptorSelector.prototype =
{
    initialize: function () {
        $HBRootNS.WfProcessDescriptorSelector.callBaseMethod(this, 'initialize');

        if (this.get_currentMode() == $HBRootNS.ControlShowingMode.Dialog) {
            if (window.dialogArguments)
            { }
        }
    },

    dispose: function () {
        $HBRootNS.WfProcessDescriptorSelector.callBaseMethod(this, 'dispose');
    },

    start: function (url) {
        var result = this._showDialog(null, url);
        if (result != null) {
            return result;
        }
    },

    get_multiSelect: function () {
        return this.multiSelect;
    },
    set_multiSelect: function (value) {
        this.multiSelect = value;
    }

}

$HBRootNS.WfProcessDescriptorSelector.registerClass($HBRootNSName + ".WfProcessDescriptorSelector", $HBRootNS.DialogControlBase);
