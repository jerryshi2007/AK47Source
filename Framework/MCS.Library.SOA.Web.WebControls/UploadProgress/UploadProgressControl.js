$HBRootNS.UploadProgressControl = function (element) {
    $HBRootNS.UploadProgressControl.initializeBase(this, [element]);

    this._postedData = "";
}

$HBRootNS.UploadProgressControl.prototype =
{
    initialize: function () {
        $HBRootNS.UploadProgressControl.callBaseMethod(this, 'initialize');
    },

    dispose: function () {
        $HBRootNS.UploadProgressControl.callBaseMethod(this, 'dispose');
    },

    loadClientState: function (value) {
        if (value) {
            if (value != "") {
            }
        }
    },

    saveClientState: function () {

    },

    get_postedData: function () {
        return this._postedData;
    },

    set_postedData: function (value) {
        this._postedData = value;
    },

    showDialog: function () {
        var result = false;
        var e = this.raiseBeforeStart();

        if (e.cancel == false) {
            var params = new Object();

            params.postedData = e.postedData;

            result = this._showDialog(params);

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
        e.postedData = this._postedData;

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

    raiseCompleted: function (retVal) {
        var handlers = this.get_events().getHandler("completed");
        var e = new Sys.EventArgs();

        e.dataChanged = retVal.DataChanged;
        e.data = retVal.Data;

        if (handlers)
            handlers(e);

        return e;
    },

    _pseudo: function () {
    }
}

$HBRootNS.UploadProgressControl.registerClass($HBRootNSName + ".UploadProgressControl", $HBRootNS.DialogControlBase);