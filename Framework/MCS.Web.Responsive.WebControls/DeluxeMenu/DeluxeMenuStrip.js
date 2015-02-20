/// <reference path="../Common/MicrosoftAjax.debug.js" />
/// <reference path="../../MCS.Web.Responsive.Library/Resources/ControlBase.js" />

/* File Created: 五月 4, 2014 */

$HGRootNS.DeluxeMenuStrip = function (element) {
    //客户端对象集合

    if (typeof (jQuery) === 'undefnied') {
        throw new Error("没有加载jQuery");
    }

    this._created = false;
    this._handlerMenuOpen = Function.createDelegate(this, this._onMenuOpen);
    this._handlerMenuClose = Function.createDelegate(this, this._onMenuClose);
    this._handlerItemClick = Function.createDelegate(this, this._onMenuItemClick);

    $HGRootNS.DeluxeMenuStrip.initializeBase(this, [element]);
}

$HGRootNS.DeluxeMenuStrip.prototype =
{
    initialize: function () {
        //初始化控件
        $HGRootNS.DeluxeMenuStrip.callBaseMethod(this, 'initialize');

        jQuery(this.get_element()).dropdown().on("click", "[role=menuitem]", this._handlerItemClick);
        jQuery(this.get_element().parentNode).on("hide.bs.dropdown", this._handlerMenuClose).on("show.bs.dropdown", this._handlerMenuOpen);
        this._created = true;
    },

    add_onClientItemClick: function (handler) {
        this.get_events().addHandler("onClientItemClick", handler);
    },

    remove_onClientItemClick: function (handler) {
        this.get_events().addHandler("onClientItemClick", handler);
    },

    raise_onClientItemClick: function (eventArgs) {
        var handlers = this.get_events().getHandler("onClientItemClick");
        if (handlers) {
            handlers(this, eventArgs);
        }
    },

    add_onClientMenuOpen: function (handler) {
        this.get_events().addHandler("onClientMenuOpen", handler);
    },

    remove_onClientMenuOpen: function (handler) {
        this.get_events().addHandler("onClientMenuOpen", handler);
    },

    raise_onClientMenuOpen: function (eventArgs) {
        var handlers = this.get_events().getHandler("onClientMenuOpen");
        if (handlers) {
            handlers(this, eventArgs);
        }
    },

    add_onClientMenuClose: function (handler) {
        this.get_events().addHandler("onClientMenuClose", handler);
    },

    remove_onClientMenuClose: function (handler) {
        this.get_events().addHandler("onClientMenuClose", handler);
    },

    raise_onClientMenuClose: function (eventArgs) {
        var handlers = this.get_events().getHandler("onClientMenuClose");
        if (handlers) {
            handlers(this, eventArgs);
        }
    },

    _onMenuOpen: function (e) {
        this.raise_onClientMenuOpen(e);
    },

    _onMenuClose: function (e) {
        this.raise_onClientMenuClose(e);
    },

    _onMenuItemClick: function (e) {
        if (!e.target.hasAttribute("disabled"))
            this.raise_onClientItemClick(e);
        else
            e.stopPropagation()
    },

    toggle: function () {
        jQuery(this.get_element()).dropdown("toggle");
    },

    setPosition: function (x, y, autoAdjust) {
        // 注意，会设置容器对象的位置,x 和 y 是基于文档客户端区域的位置
        var elem = this.get_element();
        if (elem && elem.parentNode) {
            elem.parentNode.style.position = "absolute";
            elem.parentNode.style.left = "";
            elem.parentNode.style.top = "";
            var oldDisplay = elem.style.display;
            if (oldDisplay !== 'block') {
                elem.style.display = "block";
            }

            var b = elem.getBoundingClientRect();
            var docScrollHeight = document.documentElement.scrollHeight;
            var docScrollWidth = document.documentElement.scrollWidth;
            var docTop = document.documentElement.scrollTop;
            var docLeft = document.documentElement.scrollLeft;
            var offsetX = x - b.left + elem.parentNode.offsetLeft;
            var offsetY = y - b.top + elem.parentNode.offsetTop;
            elem.parentNode.style.left = offsetX + "px";
            elem.parentNode.style.top = offsetY + "px";

            if (typeof (autoAdjust) === 'boolean' && autoAdjust) {
                elem.style.display = "none";
                var docScrollWidth2 = document.documentElement.scrollWidth;
                var docScrollHeight2 = document.documentElement.scrollHeight;
                var docWidth = document.documentElement.clientWidth;
                var docHeight = document.documentElement.clientHeight;

                var deltaX = docScrollWidth - docScrollWidth2;
                var deltaY = docScrollHeight - docScrollHeight2;

                if (deltaX > 0) {
                    //出现了横向滚动条
                    if (offsetX - deltaX + docLeft + b.Left > 0) {
                        offsetX = offsetX - deltaX;
                        elem.parentNode.style.left = offsetX + "px";
                    }
                }

                if (deltaY > 0) {
                    //出现了纵向滚动条
                    if (offsetY - deltaY + docTop + b.top > 0) {
                        offsetY = offsetY - deltaY;
                        elem.parentNode.style.top = offsetY + "px";
                    }
                }


            }

            elem.style.display = oldDisplay; // 恢复原始形态
        }
    },

    resetPosition: function () {
        var elem = this.get_element();
        if (elem && elem.parentNode) {
            elem.parentNode.style.position = 'relative';
            elem.parentNode.style.left = "";
            elem.parentNode.style.top = "";
        }
    }
}

$HGRootNS.DeluxeMenuStrip.registerClass($HGRootNSName + ".DeluxeMenuStrip", $HGRootNS.ControlBase);