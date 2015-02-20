/// <reference path="MicrosoftAjax.debug.js" />
Type.registerNamespace("PermissionCenter");

PermissionCenter.BannerNoticeActiveEventArgs = function () {
    //用于激活事件的参数
    if (arguments.length !== 0) throw Error.parameterCount();
    Sys.CancelEventArgs.initializeBase(this);
    this._index = -1;
    this._id = null;
    this._tag = null;
}

PermissionCenter.BannerNoticeActiveEventArgs.prototype = {
    get_index: function () {
        return this._index;
    }, set_index: function (v) {
        this._index = v;
    }, get_id: function () {
        return this._id;
    }, set_id: function (v) {
        this._id = v;
    }, get_tag: function () {
        return this._tag;
    }, set_tag: function (v) {
        this._tag = v;
    }

}
PermissionCenter.BannerNoticeActiveEventArgs.registerClass('PermissionCenter.BannerNoticeActiveEventArgs', Sys.EventArgs);

PermissionCenter.BannerNotice = function (element) {
    PermissionCenter.BannerNotice.initializeBase(this, [element]);
    this._mode = null;
    this._clientVisible = false;
    this._elemSummary = null;
    this._elemDetails = null;
    this._elemLinkDetails = null;
    this._elemLinkClose = null;
    this._elemLinkReport = null;
    this._detailItemHandlers = null;
    this._autoHideDuration = 0;

}

PermissionCenter.BannerNotice.prototype = {
    initialize: function () {
        var inst = this;
        this._detailItemHandlers = [];
        PermissionCenter.BannerNotice.callBaseMethod(this, 'initialize');
        var elem ;
        var role;
        for (var node = this.get_element().firstChild; node; node = node.nextSibling) {
            if (node.nodeType === 1) {
                if (this._hasRole(node, "summary")) {
                    this._elemSummary = node;
                } else if (this._hasRole(node, "details")) {
                    this._elemDetails = node;
                }
            }
        }
        if (this._elemSummary) {
            for (elem = this._elemSummary.firstChild; elem; elem = elem.nextSibling) {
                if (elem.nodeType === 1) {
                    role = this._getRole(elem);
                    if (role) {
                        if (role == "opendetails") {
                            this._elemLinkDetails = elem;
                            this._onDetailsHandler = Function.createDelegate(this, this._onDetails);
                            $addHandler(this._elemLinkDetails, "click", this._onDetailsHandler);
                        } else if (role == "close") {
                            this._elemLinkClose = elem;
                            this._onCloseHandler = Function.createDelegate(this, this._onClose);
                            $addHandler(this._elemLinkClose, "click", this._onCloseHandler);
                        } else if (role == "report") {
                            this._elemReport = elem;
                            this._onReportHandler = Function.createDelegate(this, this._onReport);
                            $addHandler(this._elemReport, "click", this._onReportHandler);
                        }
                    }
                }
            }
        }
        if (this._elemDetails) {
            for (elem = this._elemDetails.firstChild; elem; elem = elem.nextSibling) {
                if (elem.nodeType === 1 && elem.nodeName.toUpperCase() === "OL") {
                    for (node = elem.firstChild; node; node = node.nextSibling) {
                        if (node.nodeType === 1 && node.nodeName.toUpperCase() === "LI") {
                            this._prepareErrorItem(node);
                        }
                    }

                }

            }
        }

        //        this._onfocusHandler = Function.createDelegate(this, this._onFocus);
        //        this._onblurHandler = Function.createDelegate(this, this._onBlur);

        //        $addHandlers(this.get_element(),
        //                       { 'focus': this._onFocus,
        //                           'blur': this._onBlur
        //                       },
        //                       this);

        //        this.get_element().className = this._nohighlightCssClass;
        node = null;
        elem = null;
    },

    dispose: function () {
        $clearHandlers(this.get_element());
        this._linkDetailHandler = null;
        this._linkCloseHandler = null;
        this._linkReportHandler = null;
        this._detailItemHandlers = [];
        this._mode = null;
        this._clientVisible = false;
        this._elemSummary = null;
        this._elemDetails = null;
        PermissionCenter.BannerNotice.callBaseMethod(this, 'dispose');
    },

    //
    // HelpMethod
    //
    _hasRole: function (elem, role) {
        var r = null;
        if (elem.getAttribute) {
            r = elem.getAttribute("data-actrole");
        } else {
            r = elem["data-actrole"];
        }
        return r = (r == role);
        return r;
    },
    _getRole: function (elem) {
        var r = null;
        if (elem.getAttribute) {
            r = elem.getAttribute("data-actrole");
        } else {
            r = elem["data-actrole"];
        }
        return r;
    },
    _findElem: function (parent, actrole, action, firstOnly) {
        /// 查找指定角色的元素，进行操作
        var rst = false;
        for (var elem = parent.firstChild; elem; elem = elem.nextSibling) {
            if (elem.nodeType === 1) {
                if (this._hasRole(elem, actrole)) {
                    action(elem);
                    rst = true;
                    if (firstOnly)
                        break;
                }
            }
        }
        return rst;
    }
    ,
    _prepareErrorItem: function (nodeItem) {
        var fun;
        if (nodeItem) {
            for (var node = nodeItem.firstChild; node; node = node.nextSibling) {
                if (node.nodeType === 1 && this._hasRole(node, "toggle")) {
                    fun = Function.createDelegate(node, this._onErrorItemClick);
                    this._detailItemHandlers.push(fun);
                    $addHandler(node, "click", fun);
                }
            }
            node = null;

        }
    },
    //
    // Event delegates
    //
    _onDetails: function (e) {
        this._elemDetails.style.display = "block";
        this._elemLinkDetails.style.display = "none";
    },
    _onClose: function (e) {
        this.get_element().style.display = "none";
    },
    _onReport: function (e) {
        var addr = this.get_reportMailAddress();
        if (addr && addr.length > 0) {
            //找summary
            var sum = '';
            var details = '';
            var node;
            if (this._elemSummary) {
                for (node = this._elemSummary.firstChild; node; node = node.nextSibling) {
                    if (node.nodeType === 1 && node.nodeName.toUpperCase() == "SPAN") {
                        sum = node.textContent || node.innerText;
                        node = null;
                        break;
                    }
                }

            }
            if (this._elemDetails) {
                details = (this._elemDetails.textContent || this._elemDetails.innerText).replace(/\t/g, '');
                //                details = this._elemDetails.innerHTML.replace(/\s/g,'');
            }

            node = null;

            try {
                //尝试使用Outlook
                throw new ("不使用Outlook");
                var outlookApp = new ActiveXObject("Outlook.Application");
                var nameSpace = outlookApp.getNameSpace("MAPI");
                var mailItem = outlookApp.CreateItem(0);

                with (mailItem) {
                    Subject = sum;
                    To = addr;
                    HTMLBody = details;
                    Display(0);
                }
                mailItem = null;
                nameSpace = null;
                outlookApp = null;
            } catch (e) {
                //失败了，采取退化策略
                try {
                    var link = "mailto:" + addr + "?subject=" + encodeURIComponent(sum) + "&body=" + encodeURIComponent(details);
                    var elem = document.createElement("a");
                    document.body.appendChild(elem);
                    if (Sys.Browser.name == "Microsoft Internet Explorer") {
                        //link = link.substring(0, 128); // IE不允许太长的url
                    }
                    elem.href = link;

                    elem.click();
                    document.body.removeChild(elem);
                    elem = null;
                    //                    window.location.href = link;
                } catch (ex) {
                    alert(ex.message);
                }
            }

        }
    },

    _onErrorItemClick: function (e) {
        Sys.UI.DomElement.toggleCssClass(this.parentNode, "pc-open");
    },
    _onFocus: function (e) {
        if (this.get_element() && !this.get_element().disabled) {
            this.get_element().className = this._highlightCssClass;
        }
    },

    _onBlur: function (e) {
        if (this.get_element() && !this.get_element().disabled) {
            this.get_element().className = this._nohighlightCssClass;
        }
    },

    //
    // Control properties
    //

    get_clientVisible: function () {
        return this._clientVisible;
    },
    set_clientVisible: function (value) {
        value = value ? true : false;
        if (this._clientVisible != value) {
            this._clientVisible = value;
            this.get_element().style.display = value ? "block" : none;
            this.raisePropertyChanged('clientVisible');
        }
    },
    get_reportMailAddress: function () {
        return this._reportMailAddress;
    },
    set_reportMailAddress: function (value) {
        if (this._reportMailAddress != value) {
            this._reportMailAddress = value;
            this.raisePropertyChanged("reportMailAddress");
        }
    },
    get_autoHideDuration: function () {
        return this._autoHideDuration;
    },
    set_autoHideDuration: function (value) {
        var e = Function._validateParams(arguments, [{ name: "value", type: Number}]);
        if (e) throw e;
        if (value < 0) {
            throw Error.argumentOutOfRange("value", value, Sys.Res.invalidTimeout);
        }
        if (this._autoHideDuration != value) {
            this._autoHideDuration = value;
            this.raisePropertyChanged("autoHideDuration");
        }
    }

}

PermissionCenter.BannerNotice.descriptor = {
    properties: [{ name: 'active', type: String}]
}

PermissionCenter.BannerNotice.registerClass('PermissionCenter.BannerNotice', Sys.UI.Control);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
