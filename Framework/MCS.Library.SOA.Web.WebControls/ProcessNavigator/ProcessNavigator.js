
$HBRootNS.ProcessNavigator = function (element) {
    $HBRootNS.ProcessNavigator.initializeBase(this, [element]);

    this.navContainer = null;
    this.processDivContainer = null;
    this.leftBtn = null;
    this.rightBtn = null;
    this._mainContainerClientID = "";
    this._processDivContainerClientID = "";
    this._processUlContainerClientID = "";
    this._btnLeftClientID = "";
    this._btnRightClientID = "";
    this._buttonContainerClientID = "";
    this.timer = null;
    this.tickHanlder = null;
    this.aninateStep = 4;
    this.mouseDownDelegate = null;
    this.isLeft = false;
    this.isRight = false;
}

$HBRootNS.ProcessNavigator.prototype =
{
    initialize: function () {
        $HBRootNS.ProcessNavigator.callBaseMethod(this, 'initialize');
        this.mouseDownUpDelegate = {
            mousedown: Function.createDelegate(this, this._mouseDown),
            mouseup: Function.createDelegate(this, this._mouseUp)
        };
        this.mainContainer = $get(this.get_mainContainerClientID());
        this.processDivContainer = $get(this.get_processDivContainerClientID());
        this.navContainer = $get(this.get_processUlContainerClientID());
        this.buttonContainer = $get(this.get_buttonContainerClientID());
        this.leftBtn = $get(this.get_buttonLeftClientID());
        this.rightBtn = $get(this.get_buttonRightClientID());

        this.aninateStep = 2;

        this.tickHanlder = Function.createDelegate(this, this._onTimerTick);

        this.timer = new $HGRootNS.Timer();
        this.timer.add_tick(this.tickHanlder);
        this.timer.set_interval(1000 / 25);

        if (this.processDivContainer != null && this.navContainer != null) {

            this._setOffset(); this._setPage();
            if (this.processDivContainer.offsetWidth < this.navContainer.offsetWidth) {
                this._setCenter();
            }

        }
    },

    _refreshProcess: function (newGraph) {
        $clearHandlers(this.leftBtn);
        $clearHandlers(this.rightBtn);
        this.get_element().innerHTML = newGraph;

        this.processDivContainer = $get(this.get_processDivContainerClientID());
        this.navContainer = $get(this.get_processUlContainerClientID());
        this.buttonContainer = $get(this.get_buttonContainerClientID());
        this.leftBtn = $get(this.get_buttonLeftClientID());
        this.rightBtn = $get(this.get_buttonRightClientID());

        if (this.processDivContainer != null && this.navContainer != null) {

            this._setOffset(); this._setPage();
            if (this.processDivContainer.offsetWidth < this.navContainer.offsetWidth) {
                this._setCenter();
            }

            $addHandlers(this.leftBtn, this.mouseDownUpDelegate);
            $addHandlers(this.rightBtn, this.mouseDownUpDelegate);
        }
    },

    _onTimerTick: function () {
        var step = 0;
        var loc = Sys.UI.DomElement.getLocation(this.navContainer);
        if (this.isLeft) step = -2;
        if (this.isRight) step = 2;
        var s = parseInt(this.navContainer.style.left) || 0;
        s += this.aninateStep; ;
        var offsetLeft = this.navContainer.offsetLeft;
        this.navContainer.style.left = (s) + "px";
        if (this.isLeft && loc.x < Sys.UI.DomElement.getLocation(this.processDivContainer).x - Math.abs((this.processDivContainer.offsetWidth - this.navContainer.offsetWidth))) {
            this.timer.set_enabled(false);
            //$clearHandlers(this.leftBtn);
            $addHandlers(this.rightBtn, this.mouseDownUpDelegate);

        }
        if (this.isRight && loc.x > this.processDivContainer.offsetLeft) {
            this.timer.set_enabled(false); //$clearHandlers(this.rightBtn);
            $addHandlers(this.leftBtn, this.mouseDownUpDelegate);

        }

    },
    _mouseDown: function (eventElement) {
        var sourceBtn = eventElement.handlingElement;
        var loc = Sys.UI.DomElement.getLocation(this.navContainer);

        //alert(id);
        this.timer.set_enabled(false);
        if (sourceBtn == this.leftBtn) {
            this.isLeft = false;
            this.isRight = true;
            this.aninateStep = 2;
        }
        if (sourceBtn == this.rightBtn) {
            this.isLeft = true;
            this.isRight = false;
            this.aninateStep = -2;
        }
        if (this.isLeft) {
            if (this.isLeft && loc.x < Sys.UI.DomElement.getLocation(this.processDivContainer).x - Math.abs((this.processDivContainer.offsetWidth - this.navContainer.offsetWidth))) {
                return;
            }
        }
        if (this.isRight) {
            if (this.isRight && loc.x > this.processDivContainer.offsetLeft) {
                return;
            }
        }
        this.timer.set_enabled(true);

    },
    _mouseUp: function (eventElement) {
        this.timer.set_enabled(false);
    },

    _setPage: function () {
        //var ulMaxLength = this._getMaxProcessRowWidth()[0];
        if (this.processDivContainer.offsetWidth < this.navContainer.offsetWidth) {//) {this.processDivContainer.offsetWidth < ulMaxLength
            Sys.UI.DomElement.setVisible(this.leftBtn, true);
            Sys.UI.DomElement.setVisible(this.rightBtn, true);
            this.leftBtn.style.display = "block";
            this.rightBtn.style.display = "block";
            $addHandlers(this.leftBtn, this.mouseDownUpDelegate);
            $addHandlers(this.rightBtn, this.mouseDownUpDelegate);
        }
        else {
            Sys.UI.DomElement.setVisible(this.leftBtn, false);
            Sys.UI.DomElement.setVisible(this.rightBtn, false);
            this.leftBtn.style.display = "none";
            this.rightBtn.style.display = "none";
        }
    },

    _getMaxProcessRowWidth: function () {
        var uls = this.navContainer.getElementsByTagName("ul");
        var arrUlLength = [];

        for (var i = 0; i < uls.length; i++) {
            var ulLength = 0;
            var lis = uls[i].getElementsByTagName("li");
            for (var j = 0; j < lis.length; j++) {
                ulLength += lis[j].offsetWidth + 2;
            }
            uls[i].style.width = ulLength;
            arrUlLength.push(ulLength);
        }
        return arrUlLength.sort(function (a, b) {
            return b - a;
        });
    },

    _setOffset: function () {
        var navUl = this.navContainer.children;
        for (var i = 0; i < navUl.length; i++) {
            if (navUl[i].ownerActivityId && navUl[i].ownerActivityId != "") {
                var offset = this._getOffset(navUl[i].ownerActivityId, navUl[i].associatedOwnerActivityId, navUl[i - 1]);
                navUl[i].style.paddingLeft = offset;
            }
        }
    },

    _setCenter: function () {
        var currenAct;
        if (this.navContainer.children.length > 1) {
            var currProcessUl = this.navContainer.children[this.navContainer.children.length - 1];
            for (var i = 0; i < currProcessUl.children.length; i++) {
                if (currProcessUl.children[i].currentActivityKey) {
                    this.navContainer.style.left = ((this.processDivContainer.offsetLeft + this.processDivContainer.style.width / 2) - currProcessUl.children[i].offsetLeft) + "px";
                }
            }
        }
    },
    _setActivityVisible: function (activityId, processUl) {
        for (var i = 0; i < processUl.children.length; i++) {
            if (processUl.children[i].activityId && processUl.children[i].activityId != activityId) {
                Sys.UI.DomElement.setVisible(processUl.children[i], false);
            } else if (processUl.children[i].activityId && processUl.children[i].activityId == activityId) {
                var img = processUl.children[i].getElementsByTagName("img")[0]; //$get("imgArrow", processUl.children[i])
                Sys.UI.DomElement.setVisible(img, false);
            }
        }
    },
    _getOffset: function (activityId, associatedOwnerActivityId, processUl) {
        var offset;
        var actSpan;
        for (var i = 0; i < processUl.children.length; i++) {
            if (processUl.children[i].activityId && processUl.children[i].activityId == activityId) {
                actSpan = processUl.children[i].children.length > 0 ? processUl.children[i].children[0] : null;
                if (actSpan != null)
                    offset = processUl.children[i].offsetLeft + actSpan.offsetWidth / 2;
                break;
            }
        }
        if (!offset) {
            for (var i = 0; i < processUl.children.length; i++) {
                if (processUl.children[i].activityId && processUl.children[i].activityId == associatedOwnerActivityId) {
                    actSpan = processUl.children[i].children.length > 0 ? processUl.children[i].children[0] : null;
                    if (actSpan != null)
                        offset = processUl.children[i].offsetLeft + actSpan.offsetWidth / 2;
                    //offset = processUl.children[i].offsetLeft + processUl.children[i].children[0].offsetWidth / 2;
                    break;
                }
            }
        }

        if (!offset) {
            offset = this._getOffsetRecursive(activityId, associatedOwnerActivityId, processUl);
        }

        return offset;
    },

    _getOffsetRecursive: function (activityId, associatedOwnerActivityId, processUl) {
        var offset;
        //        for (var i = 0; i < processUl.parent.children; i++) {
        //            var processUl = processUl.parent.children[i];
        //            for (var j = 0; j < processUl.childern.length; j++) {
        //                if (processUl.children[i].activityId && processUl.children[i].activityId == activityId) {
        //                    offset = processUl.children[i].offsetLeft + processUl.children[i].offsetWidth / 2;
        //                    return offset;
        //                }
        //            }
        //        }
        if (processUl.ownerActivityId.length != 0) {
            while (processUl.previousSibling) {
                var processUl = processUl.previousSibling;
                for (var i = 0; i < processUl.children.length; i++) {
                    if (processUl.children[i].activityId && processUl.children[i].activityId == activityId) {
                        offset = processUl.children[i].offsetLeft + processUl.children[i].offsetWidth / 2;
                        return offset;
                    }
                }

            }
        }

    },

    get_mainContainerClientID: function () {
        return this._mainContainerClientID;
    },
    set_mainContainerClientID: function (value) {
        this._mainContainerClientID = value;
    },

    get_processDivContainerClientID: function () {
        return this._processDivContainerClientID;
    },
    set_processDivContainerClientID: function (value) {
        this._processDivContainerClientID = value;
    },
    get_processUlContainerClientID: function () {
        return this._processUlContainerClientID;
    },
    set_processUlContainerClientID: function (value) {
        this._processUlContainerClientID = value;
    },
    get_buttonLeftClientID: function () {
        return this._btnLeftClientID;
    },
    set_buttonLeftClientID: function (value) {
        this._btnLeftClientID = value;
    },
    get_buttonRightClientID: function () {
        return this._btnRightClientID;
    },
    set_buttonRightClientID: function (value) {
        this._btnRightClientID = value;
    },
    get_buttonContainerClientID: function () {
        return this._buttonContainerClientID;
    },
    set_buttonContainerClientID: function (value) {
        this._buttonContainerClientID = value;
    },

    dispose: function () {
    }
}

$HBRootNS.ProcessNavigator.registerClass($HBRootNSName + ".ProcessNavigator", $HBRootNS.ControlBase);

//CssExpressionMethod = function (el) {
//    el.className = 'process_div_container';
//    el.width = el.parentElement.offsetWidth - 10;
//}