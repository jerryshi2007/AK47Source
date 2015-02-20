$HBRootNS.UserSelector = function (element) {
    $HBRootNS.UserSelector.initializeBase(this, [element]);

    this._userInputClientID = "";
}

$HBRootNS.UserSelector.prototype =
{
    initialize: function () {
        $HBRootNS.UserSelector.callBaseMethod(this, 'initialize');

        this._listMask = 15;
        this._selectMask = 15;
        this._multiSelect = false;
    },

    dispose: function () {
        $HBRootNS.UserSelector.callBaseMethod(this, 'dispose');
    },

    loadClientState: function (value) {
        if (value) {
            if (value != "") {
            }
        }
    },

    saveClientState: function () {

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

    get_userInputClientID: function () {
        return this._userInputClientID;
    },

    set_userInputClientID: function (value) {
        this._userInputClientID = value;
    },

    get_selectedOuUserData: function () {
        return $find(this.get_userInputClientID()).get_selectedOuUserData();
    },

    showDialog: function (arg) {
        var result = null;
        var resultStr = this._showDialog(arg);
        if (resultStr) {
            result = Sys.Serialization.JavaScriptSerializer.deserialize(resultStr);
        }
        return result;
    },

    _pseudo: function () {
    }
}

$HBRootNS.UserSelector.registerClass($HBRootNSName + ".UserSelector", $HBRootNS.DialogControlBase);