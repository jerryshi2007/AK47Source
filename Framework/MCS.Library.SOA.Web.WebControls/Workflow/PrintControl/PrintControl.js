$HBRootNS.PrintControl = function (element) {
    $HBRootNS.PrintControl.initializeBase(this, [element]);
    this._onPrint = null;
    this.targetControl = null;
    this.targetControlID = "";
}

$HBRootNS.PrintControl.prototype = {
    initialize: function () {
        $HBRootNS.PrintControl.callBaseMethod(this, 'initialize');
        this.targetControl = $get(this.get_targetControlID());
        this.delegations =
        {
            click: Function.createDelegate(this, this._click)
        };
        $addHandlers(this.targetControl, this.delegations);
    },
    _click: function () {

        var handler = this.get_events().getHandler("onPrint");
        var e = new Sys.EventArgs();
        e.proceed = true;

        if (handler) {
            handler(this, e);
        }
        if (e.proceed) {
            if (window.confirm("确实要打印吗？")) {
                window.print();
            }
        }
        return false;

    },

    add_onPrint: function (handler) {
        this.get_events().addHandler('onPrint', handler);
    },

    remove_onPrint: function (handler) {
        this.get_events().removeHandler('onPrint', handler);
    },
    get_targetControlID: function () {
        return this._targetControlID;
    },

    set_targetControlID: function (value) {
        this._targetControlID = value;
    },

    dispose: function () {
    }



}

$HBRootNS.PrintControl.registerClass($HBRootNSName + ".PrintControl", $HBRootNS.ControlBase);