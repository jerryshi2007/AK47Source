
$HGRootNS.RelativeLink = function (element) {
    $HGRootNS.RelativeLink.initializeBase(this, [element]);

    this.outerContainerClientID = null;
    this.dragAndDropPointClientID = null;
    this.titleClientID = null;
    this.upArrowClientID = null;
    this.downArrowClientID = null;

    this.dock = null;
    this.oBox = null;
    this.line = null;
    this.titlebtn = null;
    this.upArrow = null;
    this.downArrow = null;
    this.count = 0;
    this.timer = null;
    this.title = null;
    this.titleWidth = null;
    this.titleHeight = null;
    this.containerHeight = null;
    this.containerWidth = null;
    this.relativeLinkPosition = null;
    this.containerStyle = null;
    this.isSelfAdaption = null;
    this.linkTarget = null;
    this.moveStep = null;
    this.titleRelativeLink = null;
    this.rightTitleRelativeLink = null;
    this.grayUpArrowImg = null;
    this.grayLineImg = null;
    this.grayDownArrowImg = null;
    this.lightUpArrowImg = null;
    this.lightLineImg = null;
    this.lightDownArrowImg = null;
    this.relativeLinkStatus = null;
    this.isOpen = null;
    this.alwaysVerticalCenter = null;
    this.timeInterval = null;
    this.hotPointCursor = null;
    this.movingCursor = null;
    this.stoppingCursor = null;
    this.dockContainer = null;
    this.extendContent = null;
    this.extendContentStyle = null;
    this._clickfixedDelegate = null;
    this._clickUpArrowDelegate = null;
    this._clickDownArrowDelegate = null;
    this._mouseoverUpArrowDelegate = null;
    this._mouseoutUpArrowDelegate = null;
    this._mouseoverLineDelegate = null;
    this._mouseoutLineDelegate = null;
    this._mouseoverDownArrowDelegate = null;
    this._mouseoutDownArrowDelegate = null;
    this._mousedownLineDelegate = null;
    this._onresizeWindowDelegate = null;
    this._applicationLoadDelegate = null;
};
$HGRootNS.RelativeLink.prototype = {
    get_title: function () {
        return this.title;
    },
    set_title: function (value) {
        this.title = value;
    },
    get_titleWidth: function () {
        return this.titleWidth;
    },
    set_titleWidth: function (value) {
        this.titleWidth = value;
    },
    get_titleHeight: function () {
        return this.titleHeight;
    },
    set_titleHeight: function (value) {
        this.titleHeight = value;
    },
    get_containerHeight: function () {
        return this.containerHeight;
    },
    set_containerHeight: function (value) {
        this.containerHeight = value;
    },
    get_containerWidth: function () {
        return this.containerWidth;
    },
    set_containerWidth: function (value) {
        this.containerWidth = value;
    },
    get_relativeLinkPosition: function () {
        return this.relativeLinkPosition;
    },
    set_relativeLinkPosition: function (value) {
        this.relativeLinkPosition = value;
    },
    get_containerStyle: function () {
        return this.containerStyle;
    },
    set_containerStyle: function (value) {
        this.containerStyle = value;
    },
    get_isSelfAdaption: function () {
        return this.isSelfAdaption;
    },
    set_isSelfAdaption: function (value) {
        this.isSelfAdaption = value;
    },
    get_linkTarget: function () {
        return this.linkTarget;
    },
    set_linkTarget: function (value) {
        this.linkTarget = value;
    },
    get_moveStep: function () {
        return this.moveStep;
    },
    set_moveStep: function (value) {
        this.moveStep = value;
    },
    get_titleRelativeLink: function () {
        return this.titleRelativeLink;
    },
    set_titleRelativeLink: function (value) {
        this.titleRelativeLink = value;
    },
    get_upArrowImg: function () {
        return this.grayUpArrowImg;
    },
    set_upArrowImg: function (value) {
        this.grayUpArrowImg = value;
    },
    get_downArrowImg: function () {
        return this.grayDownArrowImg;
    },
    set_downArrowImg: function (value) {
        this.grayDownArrowImg = value;
    },
    get_relativeLinkStatus: function () {
        return this.relativeLinkStatus;
    },
    set_relativeLinkStatus: function (value) {
        this.relativeLinkStatus = value;
    },
    get_alwaysVerticalCenter: function () {
        return this.alwaysVerticalCenter;
    },
    set_alwaysVerticalCenter: function (value) {
        this.alwaysVerticalCenter = value;
    },
    get_extendPanel: function () {
        return this.extendContent;
    },
    set_extendPanel: function (value) {
        this.extendContent = value;
    },
    get_extendPanelStyle: function () {
        return this.extendContentStyle;
    },
    set_extendPanelStyle: function (value) {
        this.extendContentStyle = value;
    },
    initialize: function () {

        $HGRootNS.RelativeLink.callBaseMethod(this, "initialize");

        this.oBox = $get(this.outerContainerClientID);
        this.dock = $get(this.dockContainer);
        this.line = $get(this.dragAndDropPointClientID);
        this.titlebtn = $get(this.titleClientID);
        this.upArrow = $get(this.upArrowClientID);
        this.downArrow = $get(this.downArrowClientID);

        //初始化标题和窗口的样式        
        this._setStyle({
            "position": this.IsSupportCSS("position", "fixed") ? (this.alwaysVerticalCenter ? this.dock ? "absolute" : "fixed" : "absolute") : "absolute"
            , "z-index": "2000"
        }, this.oBox);

        //设置容器的高度和宽度
        this._setSize();

        //加载用户定义样式         
        this.containerStyle = this.containerStyle == "" ? "" : eval('(' + this.containerStyle + ')');
        this._setStyle(this.containerStyle, this.oBox);

        //初始化事件
        this._initEvents();
    },
    _initEvents: function () {

        //委托事件              

        this._clickUpArrowDelegate = Function.createDelegate(this, this._onuparrowclick);
        this._clickDownArrowDelegate = Function.createDelegate(this, this._ondownarrowclick);
        this._clickfixedDelegate = Function.createDelegate(this, this._onfixedclick);
        this._mouseoverUpArrowDelegate = Function.createDelegate(this, this._onuparrowmouseover);
        this._mouseoutUpArrowDelegate = Function.createDelegate(this, this._onuparrowmouseout);
        this._mouseoverDownArrowDelegate = Function.createDelegate(this, this._ondownarrowmouseover);
        this._mouseoutDownArrowDelegate = Function.createDelegate(this, this._ondownarrowmouseout);
        this._mouseoverLineDelegate = Function.createDelegate(this, this._onlinemouseover);
        this._mouseoutLineDelegate = Function.createDelegate(this, this._onlinemouseout);
        this._mousedownLineDelegate = Function.createDelegate(this, this._onlinemousedown);
        this._onresizeWindowDelegate = Function.createDelegate(this, this._onwindowresize);
        this._applicationLoadDelegate = Function.createDelegate(this, this._applicationLoad);

        $addHandler(this.upArrow, "mouseover", this._mouseoverUpArrowDelegate);
        $addHandler(this.upArrow, "mouseout", this._mouseoutUpArrowDelegate);
        $addHandler(this.downArrow, "mouseout", this._mouseoutDownArrowDelegate);
        $addHandler(this.downArrow, "mouseover", this._mouseoverDownArrowDelegate);
        $addHandler(this.line, "mousedown", this._mousedownLineDelegate);
        $addHandler(this.line, "mouseout", this._mouseoutLineDelegate);
        $addHandler(this.line, "mouseover", this._mouseoverLineDelegate);
        $addHandler(this.titlebtn, "click", this._clickfixedDelegate);
        $addHandler(this.upArrow, "click", this._clickUpArrowDelegate);
        $addHandler(this.downArrow, "click", this._clickDownArrowDelegate);
        $addHandler(this.dock ? this.dock : window, "resize", this._onresizeWindowDelegate);
        if (this.dock != null) {
            Sys.Application.add_load(this._applicationLoadDelegate);
        }

    },
    _setSize: function () {
        if (this.isSelfAdaption) {
            this.containerHeight = this.oBox.offsetHeight;
            this.containerWidth = this.oBox.offsetWidth - this.titleWidth;
        } else {
            this._setStyle({ "height": this.containerHeight + "px", "width": this.containerWidth - 10 - parseInt(this._getStyle("borderWidth", this.oBox)) * 2 + "px" }, this.oBox);
        }
    },
    IsSupportCSS: function (attr, value) {
        var element = document.createElement('div');
        if (attr in element.style) {
            element.style[attr] = value;
            return (element.style[attr] === value) && document.compatMode == "CSS1Compat";
        } else {
            return false;
        }
    },
    _getComponents: function (position) {
        var result = new Array(); ;
        var components = Sys.Application.getComponents();
        for (var index = 0; index < components.length; index++) {
            var type = Object.getType(components[index]).getName();
            if (type == "MCS.Web.WebControls.RelativeLink") {
                var realPostion = components[index].get_relativeLinkPosition();
                if (position == realPostion) {
                    result.push(components[index]);
                }
            }
        }
        return result;
    },
    _setTop: function (position) {
        var components = this._getComponents(position);

        for (var index = 0; index < components.length; index++) {
            var element = components[index].get_element();
            if (element.id == this.get_element().id) {
                if (this.dock) {
                    this._setStyle({ "top": ((this.dock.offsetHeight - this.containerHeight * components.length) / 2) + this.containerHeight * index + "px" }, this.oBox);
                } else {
                    this._setStyle({ "top": (document.documentElement.offsetHeight - this.containerHeight) / 2 + "px" }, this.oBox);
                }
            }
        }
    },
    _setPosition: function () {
        this._setTop("0");
        this._setTop("1");

        //容器定位
        var vScrollWidth = this._computeScrollBarWidth(this.dock);
        switch (this.relativeLinkPosition) {
            case 0: //left
                if (this.isOpen) {
                    this._setStyle({ "left": "0px" }, this.oBox);

                    this.titlebtn.setAttribute("rollout", "true");
                } else {
                    this._setStyle({ "left": -this.containerWidth + "px" }, this.oBox);
                }
                break;

            case 1: //right
            default:
                if (this.isOpen)//default status is open  to do as following.
                {
                    if (this.dock) {
                        this._setStyle({ "left": this.dock.offsetWidth - vScrollWidth - this.oBox.clientWidth + "px" }, this.oBox);
                    } else {
                        this._setStyle({ "right": "0px" }, this.oBox);
                    }

                    this.titlebtn.setAttribute("rollout", "true");
                }
                else {
                    if (this.dock) {
                        this._setStyle({ "left": this.dock.offsetWidth - vScrollWidth - this.titleWidth + "px" }, this.oBox);
                    }
                    else {
                        this._setStyle({ "right": -(this.containerWidth) + "px" }, this.oBox);
                    }
                }
                break;
        }
    },
    adjustmentPostion: function (evt) {
        this._onwindowresize(evt);
    },
    _onwindowresize: function (evt) {

        if (this._isDockAndRight()) {
            //clear timer when the window changes size
            clearInterval(this.timer);

            var relativeLinkStatus = this.titlebtn.getAttribute("rollout");

            //fixed bug whice appears  ie restore window for the first time
            var vScrollWidth = this._computeScrollBarWidth(this.dock);

            if (relativeLinkStatus == "true") {
                this._setStyle({ "left": Math.abs(this.dock.offsetWidth - vScrollWidth - this.oBox.clientWidth) + "px" }, this.oBox);
            }
            else {
                this._setStyle({ "left": Math.abs(this.dock.offsetWidth - vScrollWidth - this.titleWidth) + "px" }, this.oBox);
            }
            //容器定位
            //this._setTop();
        }
    },
    _applicationLoad: function () {
        if (this.dock) {
            this._setStyle({ "overflowX": "hidden" }, this.dock);
            this.dock.insertBefore(this.get_element(), this.dock.childNodes[0]);
        }
        //设置位置
        window.setTimeout(Function.createDelegate(this, this._setPosition), 50);        
        //this._setPosition();
    },

    _computeScrollBarWidth: function (element) {
        if (this._hasVerticalScroll(element)) {
            return this._scrollBarWidth(element);
        } else {
            return 0;
        }
    },
    _hasVerticalScroll: function (element) {
        if (element.style.overflow === 'hidden') {
            return false;
        } else if (window.getComputedStyle) {
            if (window.getComputedStyle(element, null).overflowY === 'hidden') return false;
        } else if (element.currentStyle) {
            if (element.currentStyle.overflowY === 'hidden') return false;
        }


        var maxControls = Math.max(this._getComponents("0").length, this._getComponents("1").length);
        var condition3 = maxControls * this.containerHeight > this.dock.offsetHeight;

        var relativeLinkStatus = (this.titlebtn.getAttribute("rollout") == "true" ? true : false) || this.isOpen;

        //alert("\nclientWidth：" + element.clientWidth + "\noffsetWidth:" + element.offsetWidth + "\nscrollWidth:" + element.scrollWidth);
        //alert("\nclientHeight：" + element.clientHeight + "\noffsetHeight:" + element.offsetHeight + "\nscrollHeight:" + element.scrollHeight);

        if (element.scrollHeight > element.offsetHeight) {
            return true;
        }
        else {
            return false;
        }
    },
    _scrollBarWidth: function (element) {
        if (element == null) return 0;
        var noScroll, scroll, oDiv = document.createElement("DIV");
        oDiv.style.cssText = "position:absolute; top:-1000px; width:100px; height:100px; overflow:hidden;";
        noScroll = document.body.appendChild(oDiv).clientWidth;
        oDiv.style.overflowY = "scroll";
        scroll = oDiv.clientWidth;
        document.body.removeChild(oDiv);
        return noScroll - scroll;
    },
    _onuparrowmouseout: function () {
        this.upArrow.src = this.grayUpArrowImg;
    },
    _onuparrowmouseover: function () {
        this.upArrow.src = this.lightUpArrowImg;
    },
    _onuparrowclick: function () {
        if (parseInt(this._getStyle("top", this.oBox)) - this.moveStep > 0) {
            this._setStyle({ "top": (parseInt(this._getStyle("top", this.oBox)) - this.moveStep) + "px" }, this.oBox);
        } else {
            this._setStyle({ "top": parseInt(document.documentElement.clientHeight || document.body.clientHeight) - parseInt(this.oBox.offsetHeight) + "px" }, this.oBox);
        }
    },
    _onlinemouseover: function () {
        this.line.src = this.lightLineImg;
    },
    _onlinemouseout: function () {
        this.line.src = this.grayLineImg;
    },
    _onlinemousedown: function (e) {
        var that = this;
        var doc = document;
        e = e || window.event;
        var y = (e.layerY || e.offsetY) + that.oBox.scrollTop - (that.dock ? that.dock.scrollTop : 0) + 30;
        //设置捕获范围        
        if (that.line.setCapture) {
            that.line.setCapture();
        } else if (window.captureEvents) {
            window.captureEvents(Event.MOUSEMOVE | Event.MOUSEUP);
        }
        doc.onmousemove = function (e) {
            e = e || window.event;
            if (!e.pageY) e.pageY = e.clientY;
            var ty = e.pageY - y;
            that._setStyle({ "top": ty + "px" }, that.oBox);
        };
        doc.onmouseup = function () {
            if (that.line.releaseCapture) {
                that.line.releaseCapture();
            }
            else if (window.captureEvents) {
                window.captureEvents(Event.MOUSEMOVE | Event.MOUSEUP);
            }
            doc.onmousemove = null;
            doc.onmouseup = null;
        };
    },

    _ondownarrowmouseout: function () {
        this.downArrow.src = this.grayDownArrowImg;
    },
    _ondownarrowmouseover: function () {
        this.downArrow.src = this.lightDownArrowImg;
    },

    _ondownarrowclick: function () {
        var maxh = parseInt(document.documentElement.clientHeight || document.body.clientHeight) - parseInt(this.oBox.offsetHeight);
        if (parseInt(this._getStyle("top", this.oBox)) + this.moveStep < maxh) {
            this._setStyle({ "top": (parseInt(this._getStyle("top", this.oBox)) + this.moveStep) + "px" }, this.oBox);
        } else {
            this._setStyle({ "top": "0px" }, this.oBox);
        }
    },

    _onfixedclick: function () {

        var relativeLinkStatus = this.titlebtn.getAttribute("rollout");
        if (relativeLinkStatus == 'true') {
            this._onmouseout(this._isDockAndRight() ? parseInt(this._getStyle("left", this.oBox)) + this.containerWidth : -this.containerWidth);
            this.titlebtn.setAttribute("rollout", "false");
        }
        else {
            this._onmouseover(this._isDockAndRight() ? this.dock.clientWidth - this.oBox.clientWidth : 0);
            this.titlebtn.setAttribute("rollout", "true");
        }
    },
    dispose: function () {
        var element = this.get_element();

        //回收
        $clearHandlers(element);
        if (this._onmouseoutDelegate) delete this._onmouseoutDelegate;
        if (this._onmouseoutDelegate) delete this._onmouseoutDelegate;
        if (this._clickfixedDelegate) delete this._clickfixedDelegate;
        if (this._clickUpArrowDelegate) delete this._clickUpArrowDelegate;
        if (this._clickDownArrowDelegate) delete this._clickDownArrowDelegate;
        if (this._mouseoverUpArrowDelegate) delete this._mouseoverUpArrowDelegate;
        if (this._mouseoutUpArrowDelegate) delete this._mouseoutUpArrowDelegate;
        if (this._mouseoverLineDelegate) delete this._mouseoverLineDelegate;
        if (this._mouseoutLineDelegate) delete this._mouseoutLineDelegate;
        if (this._mousedownLineDelegate) delete this._mousedownLineDelegate;
        if (this._mouseoverDownArrowDelegate) delete this._mouseoverDownArrowDelegate;
        if (this._mouseoutDownArrowDelegate) delete this._mouseoutDownArrowDelegate;
        if (this._onresizeWindowDelegate) delete this._onresizeWindowDelegate;
        if (this._applicationLoadDelegate) delete this._applicationLoadDelegate;
        //调用基类方法
        $HGRootNS.RelativeLink.callBaseMethod(this, "dispose");
    },

    _getStyle: function (attr, obj) {
        return obj.currentStyle ? obj.currentStyle[attr] : getComputedStyle(obj, false)[attr];
    },

    _setStyle: function (style, element) {
        for (var p in style) {
            try {
                element.style[p] = style[p];
            } catch (e) {

            }
        }
    },
    _isDockAndRight: function () {
        return this.relativeLinkPosition == 1 && $get(this.dockContainer) != null;
    },
    //动画效果
    _doMove: function (iTarge) {
        var position = 0;
        var iSpeed = 0;

        var d = this.containerWidth % 20;
        var z = this.containerWidth - d;

        switch (this.relativeLinkPosition) {
            case 0:
                position = parseInt(this._getStyle('left', this.oBox));
                break;
            case 1:
                position = this._isDockAndRight() ? parseInt(this._getStyle('left', this.oBox)) || 0 : parseInt(this._getStyle('right', this.oBox)) || 0;
                break;
            default:
                break;
        }
        if (this._isDockAndRight() && iTarge - Math.abs(position) == d) {
            position < iTarge ? iSpeed = d : iSpeed = -d;
        }
        else {
            if (Math.abs(position) == d || Math.abs(position) == z) {
                position < iTarge ? iSpeed = d : iSpeed = -d;
            }
            else {
                position < iTarge ? iSpeed = 20 : iSpeed = -20;
            }
        }

        if (position == iTarge) {
            clearInterval(this.timer);
        }
        else {
            var currentWidth = position + iSpeed;
            switch (this.relativeLinkPosition) {
                case 0:
                    this._setStyle({ "left": currentWidth + "px" }, this.oBox);
                    break;
                case 1:
                    this._isDockAndRight() ? this._setStyle({ "left": currentWidth + "px" }, this.oBox) : this._setStyle({ "right": currentWidth + "px" }, this.oBox);
                    break;
            }

            // stop flashing
            this.count++;
            var rounded = parseInt(this.containerWidth / 20);
            if (d == 0) {
                if (this.count == rounded) {
                    this.count = 0;
                    this._onwindowresize(null);
                    clearInterval(this.timer);
                }
            } else {
                if (this.count == rounded + 1) {
                    this.count = 0;
                    this._onwindowresize(null);
                    clearInterval(this.timer);
                }
            }
        }
    },

    _onmouseover: function (num) {
        var that = this;
        clearInterval(this.timer);

        this.timer = setInterval(
            function () {
                that._doMove(num);
            }, that.timeInterval);
    },

    _onmouseout: function (num) {
        var that = this;
        clearInterval(this.timer);

        this.timer = setInterval(function () {
            that._doMove(num);
        }, that.timeInterval);
    }
};
$HGRootNS.RelativeLink.registerClass($HGRootNSName + ".RelativeLink", $HGRootNS.ControlBase);