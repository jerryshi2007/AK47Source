/// <reference path="MicrosoftAjax.debug.js" />
Type.registerNamespace("PermissionCenter");

PermissionCenter.RelaxedTabStripActiveEventArgs = function (index, id, tag) {
	//用于激活事件的参数
	if (arguments.length !== 0) throw Error.parameterCount();
	Sys.CancelEventArgs.initializeBase(this);
	this._index = -1;
	this._id = null;
	this._tag = null;
}

PermissionCenter.RelaxedTabStripActiveEventArgs.prototype = {
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
PermissionCenter.RelaxedTabStripActiveEventArgs.registerClass('PermissionCenter.RelaxedTabStripActiveEventArgs', Sys.EventArgs);

PermissionCenter.regex = {
	anchor: /^#\w+$/i
}

PermissionCenter.RelaxedTabPage = function (owner, headerElement, bodyElement) {
	this._header = headerElement;
	this._body = bodyElement;
	this._owner = owner;
	this._tag = null;
}

PermissionCenter.RelaxedTabPage.prototype = {
	get_owner: function () {
		return this._owner;
	}, get_header: function () {
		return this._header;
	}, get_body: function () {
		return this._body;
	}, get_tag: function () {
		return this._tag;
	}, initialize: function () {
		var header = this.get_header();
		if (typeof (header.getAttribute) === 'function') {
			this._tag = header.getAttribute("data-tag");
		} else {
			this._tag = header["data-tag"];
		}

		$addHandlers(header, { mouseover: this._handleMouseEnter, mouseout: this._handleMouseLeave, click: this._handleClick }, this, false);
		header = null;
	}, dispose: function () {
		$clearHandlers(this._header);
		$clearHandlers(this._body);
		this._header = null;
		this._body = null;
		this._owner = null;
		this._tag = null;
	}, _handleMouseEnter: function () {
		hoverCss = this.get_owner().get_hoverTabCssCass();
		if (hoverCss && hoverCss.length > 0)
			Sys.UI.DomElement.addCssClass(this.get_header(), hoverCss);
	}, _handleMouseLeave: function () {
		hoverCss = this.get_owner().get_hoverTabCssCass();
		if (hoverCss && hoverCss.length > 0)
			Sys.UI.DomElement.removeCssClass(this.get_header(), hoverCss);
	}, _handleClick: function () {
		this.get_owner()._moveToTab(this);
	}
};

PermissionCenter.RelaxedTabStrip = function (element) {
	PermissionCenter.RelaxedTabStrip.initializeBase(this, [element]);
	this._activeTabPageIndex = -1;
	this._hoverTabCssCass = null;
	this._activeTabCssClass = null;
	this._controlId = null;
	this._tabPages = [];
	this._headerElement = null;
	this._isInitDone = false;
}

PermissionCenter.RelaxedTabStrip.prototype = {
	initialize: function () {
		this._isInitDone = false;
		inst = this;
		PermissionCenter.RelaxedTabStrip.callBaseMethod(this, 'initialize');
		var elem = this.get_element();
		var node;
		var str;
		var headers = [];
		var bodies = [];
		for (node = elem.firstChild; node; node = node.nextSibling) {
			if (node.nodeType === 1) {
				str = node.nodeName.toUpperCase();
				if (str === "UL") {
					this._headerElement = node;
				} else if (str === "DIV") {
					bodies.push(node);
				}
			}
		}

		if (this._headerElement) {
			for (node = this._headerElement.firstChild; node; node = node.nextSibling) {
				if (node.nodeType === 1 && node.nodeName.toUpperCase() === "LI") {
					headers.push(node);
				}
			}
		}

		if (headers.length == bodies.length) {
			this._tabPages = [];
			var len = headers.length;
			for (var i = 0; i < len; i++) {
				this._tabPages.push(new PermissionCenter.RelaxedTabPage(this, headers[i], bodies[i]));
			}

			Sys.Application.add_load(Function.createDelegate(this, this._app_onload$delegate));
		}

		headers = null;
		bodies = null;

		//		PermissionCenter.helper.childElemEach(elem, "INPUT", false, function (ipt, idxIpt) {
		//			if (ipt.type == "hidden") {
		//				if (ipt.name.indexOf("__rts_cs_") == 0) {
		//					inst._elemActiveIndex = ipt;
		//				} else if (ipt.name.indexOf("__rts_cstag_") == 0) {
		//					inst._elemActiveTag = ipt;
		//				}
		//			}
		//			return true;
		//		});

		//		inst._tabBodies = PermissionCenter.helper.childElemAll(elem, "DIV");

		//		for (i = 0; i < inst._tabHeaders.length; i++) {
		//			$addHandler(inst._tabHeaders[i], "mouseenter", function (e) {
		//				src = (this == window) ? window.event.srcElement : this;
		//				hoverCss = inst.get_hoverTabCssCass();
		//				if (hoverCss && hoverCss.length > 0)
		//					Sys.UI.DomElement.addCssClass(src, hoverCss);
		//				src = null;
		//			});
		//			$addHandler(inst._tabHeaders[i], "mouseleave", function (e) {
		//				src = (this == window) ? window.event.srcElement : this;
		//				hoverCss = inst.get_hoverTabCssCass();
		//				if (hoverCss && hoverCss.length > 0)
		//					Sys.UI.DomElement.removeCssClass(src, hoverCss);
		//				src = null;
		//			});
		//			
		//			$addHandler(inst._tabHeaders[i], "click", Function.createDelegate(inst._tabHeaders[i], function () {
		//				if (inst._handleTabClick.apply(inst, [this, thisIndex])) {

		//				}
		//			}));
		//		}
		elem = null;
		this._isInitDone = true;
		//        this._onfocusHandler = Function.createDelegate(this, this._onFocus);
		//        this._onblurHandler = Function.createDelegate(this, this._onBlur);

		//        $addHandlers(this.get_element(),
		//                       { 'focus': this._onFocus,
		//                           'blur': this._onBlur
		//                       },
		//                       this);

		//        this.get_element().className = this._nohighlightCssClass;
	}, _app_onload$delegate: function (s, e) {
		var len = this._tabPages.length;
		if (len) {
			for (var i = 0; i < len; i++) {
				this._tabPages[i].initialize();
			}
		}
	},
	dispose: function () {
		var len = this._tabPages.length;
		if (len) {
			for (var i = 0; i < len; i++) {
				this._tabPages[i].dispose();
			}
		}
		this._tabPages = null;
		$clearHandlers(this.get_element());
		PermissionCenter.RelaxedTabStrip.callBaseMethod(this, 'dispose');

	},
	_moveToTab: function (tab) {
		if (tab == null) {
			this._activeTabPageIndex = -1;
			this.raisePropertyChanged('activeTabPageIndex');
		} else if (tab instanceof PermissionCenter.RelaxedTabPage) {
			var css = this.get_activeTabCssClass();
			var activePage = null;
			var newPageIndex = this.indexOf(tab);

			if (this.get_activeTabPageIndex() != -1) {
				activePage = this._tabPages[this.get_activeTabPageIndex()];
			}

			if (css && css.length > 0) {
				if (activePage) {
					Sys.UI.DomElement.removeCssClass(activePage.get_header(), css);
					Sys.UI.DomElement.removeCssClass(activePage.get_body(), css);
				}

				if (newPageIndex != -1) {
					Sys.UI.DomElement.addCssClass(this._tabPages[newPageIndex].get_header(), css);
					Sys.UI.DomElement.addCssClass(this._tabPages[newPageIndex].get_body(), css);
				}
			}

			this._activeTabPageIndex = newPageIndex;
			activePage = null;
			this.raisePropertyChanged('activeTabPageIndex');
		}
	},

	//
	// Event delegates
	//

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
	get_tabPageCount: function () {
		return this._tabPages.length;
	},
	get_tabPage: function (index) {
		return this._tabPages[index];
	},
	get_activeTabPageIndex: function () {
		return this._activeTabPageIndex;
	},

	set_activeTabPageIndex: function (value) {
		if (this._activeTabPageIndex !== value) {
			if (this._isInitDone) {
				if (value < -1 || value >= this._tabPages.length) {
					throw Error.argumentOutOfRange("value", value, "索引超出了范围");
				}
				var page = value >= 0 ? this._tabPages[i] : null;
				this._moveToTab(page);
				this.raisePropertyChanged('activeTabPageIndex');
			} else {
				this._activeTabPageIndex = value; //没有初始化之前先不验证
			}
		}
	},

	get_hoverTabCssCass: function () {
		return this._hoverTabCssCass;
	},

	set_hoverTabCssCass: function (value) {
		if (this._hoverTabCssCass !== value) {
			this._hoverTabCssCass = value;
			this.raisePropertyChanged('activeTabCssClass');
		}
	},
	get_activeTabCssClass: function () {
		return this._activeTabCssClass;
	},

	set_activeTabCssClass: function (value) {
		if (this._activeTabCssClass !== value) {
			this._activeTabCssClass = value;
			this.raisePropertyChanged('activeTabCssClass');
		}
	}, indexOf: function (tabPage) {
		for (var i = this._tabPages.length - 1; i >= 0; i--) {
			if (this._tabPages[i] == tabPage)
				return i;
		}

		return -1;
	}

}

PermissionCenter.RelaxedTabStrip.descriptor = {
	properties: [{ name: 'active', type: String}]
}

PermissionCenter.RelaxedTabStrip.registerClass('PermissionCenter.RelaxedTabStrip', Sys.UI.Control);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
