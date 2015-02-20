/// <reference path="../../scripts/MicrosoftAjax.debug.js" />

Type.registerNamespace('PermissionCenter');

PermissionCenter.GridHoverBehavior = function (element) {
    PermissionCenter.GridHoverBehavior.initializeBase(this, [element]);
    this._hoverCssClass = '';
    this._rowItemCssClass = '';
}

PermissionCenter.GridHoverBehavior.EventHelper = function (instance) {
    this.inst = instance;
    this._dirty = true;
    Sys.Observer.addPropertyChanged(this.inst, function () {
        this._dirty = true;
    });
    this._itemClasses = [];
    this._hoverClasses = [];
    this._makeAvailable = function () {
        if (this._dirty) {
            this._hoverClasses = this.inst.get_hoverCssClass().split(" ");
            this._itemClasses = this.inst.get_rowItemCssClass().split(" ");
            this._dirty = false;
        }
    }
    this.addClasses = function (elem) {
        this._makeAvailable();
        var isMatch = false;
        var cssClasses = ' ' + elem.className + ' ';
        for (i = 0; i < this._itemClasses.length; i++) {
            if (this._itemClasses[i].length > 0 && cssClasses.indexOf(this._itemClasses[i]) > 0) {
                isMatch = true;
                break;
            }
        }
        if (isMatch) {
            for (i = 0; i < this._hoverClasses.length; i++) {
                if (this._hoverClasses[i].length > 0) {
                    Sys.UI.DomElement.addCssClass(elem, this._hoverClasses[i]);
                }
            }
        }
    };
    this.removeClasses = function (elem) {
        this._makeAvailable();
        var isMatch = false;
        var cssClasses = ' ' + elem.className + ' ';
        for (i = 0; i < this._itemClasses.length; i++) {
            if (this._itemClasses[i].length > 0 && cssClasses.indexOf(this._itemClasses[i]) > 0) {
                isMatch = true;
                break;
            }
        }
        if (isMatch) {
            for (i = 0; i < this._hoverClasses.length; i++) {
                Sys.UI.DomElement.removeCssClass(elem, this._hoverClasses[i]);
            }
        }
    }
}

PermissionCenter.GridHoverBehavior.prototype = {
    initialize: function () {
        PermissionCenter.GridHoverBehavior.callBaseMethod(this, 'initialize');
        var eventHelper = new PermissionCenter.GridHoverBehavior.EventHelper(this);
        var that = this;
        this._mouseEnterHandler = function (e) {
            var src = this == window ? e.srcElement : this;
            eventHelper.addClasses.apply(eventHelper, [src]);
            src = null;
        };
        this._mouseOutHandler = function (e) {
            var src = this == window ? e.srcElement : this;
            eventHelper.removeClasses.apply(eventHelper, [src]);
            src = null;
        }
        var node = this.get_element();
        if (node && node.nodeType === 1 && node.nodeName.toUpperCase() == "TABLE") {
            for (node = node.firstChild; node; node = node.nextSibling) {
                if (node && node.nodeType === 1 && node.nodeName.toUpperCase() == "TBODY") {
                    for (row = node.firstChild; row; row = row.nextSibling) {
                        if (row && row.nodeType === 1 && row.nodeName.toUpperCase() == "TR") {
                            $addHandler(row, "mouseenter", this._mouseEnterHandler);
                            $addHandler(row, "mouseleave", this._mouseOutHandler);
                        }
                    }
                }
            }

        }

        //          $addHandlers(this.get_element(), 
        //                       { 'focus' : this._onFocus,
        //                         'blur' : this._onBlur },
        //                       this);
        //  
        //          this.get_element().className = this._nohighlightCssClass;

        node = null;
        row = null;
    },

    dispose: function () {
        //$clearHandlers(this.get_element());

        PermissionCenter.GridHoverBehavior.callBaseMethod(this, 'dispose');
    },

    //
    // Behavior properties
    //

    get_hoverCssClass: function () {
        return this._hoverCssClass;
    },

    set_hoverCssClass: function (value) {
        if (this._hoverCssClass !== value) {
            this._hoverCssClass = value;
            this.raisePropertyChanged('hoverCssClass');
        }
    },

    get_rowItemCssClass: function () {
        return this._rowItemCssClass;
    },

    set_rowItemCssClass: function (value) {
        if (this._rowItemCssClass !== value) {
            this._rowItemCssClass = value;
            this.raisePropertyChanged('rowItemCssClass');
        }
    }

}

// Optional descriptor for JSON serialization.
PermissionCenter.GridHoverBehavior.descriptor = {
    properties: [{ name: 'hoverCssClass', type: String },
                      { name: 'rowItemCssClass', type: String}]
}

// Register the class as a type that inherits from Sys.UI.Control.
PermissionCenter.GridHoverBehavior.registerClass('PermissionCenter.GridHoverBehavior', Sys.UI.Behavior);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();

