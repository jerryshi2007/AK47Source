/*
DropDownBox
*/
$HGRootNS.DropDownBox = function (element) {
    $HGRootNS.DropDownBox.initializeBase(this, [element]);
    this.sortItems = null;
}

$HGRootNS.DropDownBox.prototype = {
    initialize: function () {
        $HGRootNS.DropDownBox.callBaseMethod(this, 'initialize');
        this._onMouseEnterHandler = Function.createDelegate(this, this._onMouseEnter);
        this._onMouseLeaveHandler = Function.createDelegate(this, this._onMouseLeave);

        $addHandlers(this.get_element(),
                       { 'mouseenter': this._onMouseEnter,
                           'mouseleave': this._onMouseLeave
                       },
                       this);
        Sys.UI.DomElement.removeCssClass(this.get_element(), "hover");

    },

    loadClientState: function (value) {
        if (value) {
            var data = Sys.Serialization.JavaScriptSerializer.deserialize(value);
            this.sortItems = data;
           
        }
    },
    saveClientState: function () {
        return Sys.Serialization.JavaScriptSerializer.serialize(this.sortItems);
    },

    dispose: function () {
        $clearHandlers(this.get_element());
        this.sortItems = null;
        $HGRootNS.DropDownBox.callBaseMethod(this, 'dispose');
    },
    _onMouseEnter: function (e) {
        if (this.get_element() && !this.get_element().disabled) {
            Sys.UI.DomElement.addCssClass(this.get_element(), "hover");
        }
    },

    _onMouseLeave: function (e) {
        if (this.get_element() && !this.get_element().disabled) {
            Sys.UI.DomElement.removeCssClass(this.get_element(), "hover");
        }
    }
}

$HGRootNS.DropDownBox.registerClass($HGRootNSName + ".DropDownBox",  $HGRootNS.ControlBase);
