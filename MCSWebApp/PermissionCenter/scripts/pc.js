/// <reference path="MicrosoftAjax.debug.js" />

(function (window) {
	//确保string的方法可用
	String.prototype.trim = function () {
		return this.replace(/(^\s*)(\s*$)/g, "");
	}
	String.prototype.ltrim = function () {
		return this.replace(/(^\s*)/g, "");
	}
	String.prototype.rtrim = function () {
		return this.replace(/(\s*$)/g, "");
	}

	function nop() {
	}

	var rclass = /[\n\t\r]/g,
        rspace = /\s+/,
        rreturn = /\r/g,
        rtype = /^(?:button|input)$/i,
        rfocusable = /^(?:button|input|object|select|textarea)$/i,
        rclickable = /^a(?:rea)?$/i,
        rguid = /^[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}$/i, /*GUID验证*/
        rboolean = /^(?:autofocus|autoplay|async|checked|controls|defer|disabled|hidden|loop|multiple|open|readonly|required|scoped|selected)$/i;
	var pcHelper = {

		appRoot: '/MCSWebApp/PermissionCenter/',
		parseAspnetDate: function (value) {
			//从Aspnet日期格式恢复日期
			return new Date(parseInt(value.replace("/Date(", "").replace(")/", ""), 10));
		},
		get: function (id) {
			/// getElementById的快捷方式
			return document.getElementById(id);
		},
		getAttr: function (elem, name) {
			//获取DOM的属性（只支持元素）
			if (typeof elem === "string") {
				elem = document.getElementById(elem);
			}

			var rst = undefined;
			if (elem && elem.nodeType === 1) {
				if (elem.getAttribute) {
					rst = elem.getAttribute(name);
				} else {
					rst = elem[name];
				}
			}
			elem = null;
			return rst;
		},
		setAttr: function (elem, name, value) {
			//获取DOM的属性（只支持元素）
			if (typeof elem === "string") {
				elem = document.getElementById(elem);
			}
			if (elem && elem.nodeType === 1) {
				if (elem.setAttribute) {
					elem.setAttribute(name, value);
				} else {
					elem[name] = value;
				}
			}
			elem = null;
		}, getValue: function (elem) {

		}, setValue: function (elem, val) {

		},
		encodeAttributeValue: function (val) {
			//用于属性的特殊字符转义
			if (val && val.length > 0) {
				var i;
				val = val.split("");
				for (i = 0; i < val.length; i++) {
					switch (val[i]) {
						case '&':
							val[i] = "&amp;";
							break;
						case '\\':
							val[i] = ("&#39;");
							break;
						case '<':
							val[i] = ("&lt;");
							break;
						case '"':
							val[i] = ("&quot;");
							break;
						default:
							break;
					}
				}
				val = val.join('');
			}
			return val;
		}
        , show: function (elem) {
        	//设置元素为block元素
        	if (typeof elem === "string") {
        		elem = document.getElementById(elem);
        	}
        	if (elem && elem.nodeType === 1) {
        		elem.style.display = 'block';
        	}
        }, hide: function (elem) {
        	// 设置元素为不可见元素
        	if (typeof elem === "string") {
        		elem = document.getElementById(elem);
        	}
        	if (elem && elem.nodeType === 1) {
        		elem.style.display = 'none';
        	}
        }, setVisible: function (elem, visible) {
        	// 设置元素是否隐身
        	if (typeof elem === "string") {
        		elem = document.getElementById(elem);
        	}
        	if (elem && elem.nodeType === 1) {
        		elem.style.visibility = visible ? 'visible' : 'hidden';
        	}
        },
		bindEvent: function (elem, eventName, handler) {
			/// 绑定事件,注意对于Document的Load不推荐，会等到所有图片视频全部加载完毕才会执行，太迟
			if (typeof elem === "string") {
				elem = document.getElementById(elem);
			}
			var rtv = null;
			if (elem.addEventListener) { //W3C
				rtv = elem.addEventListener(eventName, handler, false);
			} else if (elem.attachEvent) { //IE
				rtv = elem.attachEvent("on" + eventName, handler);
			} else {
				rtv = null;
				elem = null;
				throw "当前浏览器不受支持";
			}
			elem = null;
			return rtv;
		}, removeEvent: function (elem, eventName, handler) {
			/// 解除事件绑定
			if (typeof elem === "string") {
				elem = document.getElementById(elem);
			}
			var rtv = null;
			if (elem.removeEventListener) { //W3C
				rtv = elem.removeEventListener(eventName, handler, false);
			} else if (elem.detachEvent) { //IE
				rtv = elem.detachEvent("on" + eventName, handler);
			} else {
				rtv = null;
				elem = null;
				throw "当前浏览器不受支持";
			}
			elem = null;
			return rtv;
		}, getChildNode: function (elem, nodeType, nodeName) {
			//获取当前节点的唯一子节点
			var node = null;
			nodeName = nodeName.toUpperCase();
			for (node = elem.firstChild; node; node = node.nextSibling) {
				if (node.nodeType === nodeType && node.nodeName.toUpperCase() === nodeName) {
					return node;
				}
			}
			node = null;
			return null;
		}, getText: function (dom) {
			if (typeof dom === "string") {
				dom = document.getElementById(dom);
			}
			if (dom) {
				var nt = dom.nodeType;
				if (nt === 1 || nt === 9 || nt === 11) {
					if (typeof dom.textContent === 'string') {
						return dom.textContent;
					} else if (typeof dom.innerText === 'string') {
						// Replace IE's carriage returns
						return dom.innerText;
					}
				} else if (nt === 3 || nt === 4) {
					return dom.nodeValue;
				}
			}
		}, setText: function (dom, text) {
			/// 设置指定DOM内的text（不支持子级)
			if (typeof dom === "string") {
				dom = document.getElementById(dom);
			}
			if (dom) {
				var nt = dom.nodeType;
				if (nt === 1 || nt === 9 || nt === 11) {
					if (typeof dom.textContent === 'string') {
						dom.textContent = text;
					} else if (typeof dom.innerText === 'string') {
						// Replace IE's carriage returns
						dom.innerText = text;
					}
				} else if (nt === 3 || nt === 4) {
					dom.nodeValue = text;
				}
			}

		}, confirmDelete: function (msg) {
			return confirm(msg || "确实要删除吗？");

		}, getDocumentMode: function () {
			//获取当前浏览器的工作模式
			return document.compatMode;
		}, getEnabled: function (elem) {
			var enabled = true;
			var val;
			//检查元素是否为禁用
			if (typeof elem === "string") {
				elem = document.getElementById(elem);
			}
			if (elem && elem.nodeType === 1) {
				if (elem.getAttribute) {
					val = elem.getAttribute('disabled');
				} else {
					val = elem['disabled'];
				}
			}
			if (typeof (val) === 'string')
				enabled = val != 'disabled';
			else
				enabled = !val;
			return enabled;
		}, createDelegate: function (instance, method) {
			return function () { return method.apply(instance, arguments); }
		}

	};

	pcHelper.hasClass = function (elem, value) {
		//检查是否在元素上应用了class名
		if (typeof elem === "string") {
			elem = document.getElementById(elem);
		}
		if (typeof Sys !== undefined && Sys.UI.DomElement.containsCssClass)
			return Sys.UI.DomElement.containsCssClass(elem, value); //使用Ajax框架
		else if (elem && elem.nodeType === 1 && value && typeof value === "string") {
			className = " " + value + " "
			if ((" " + elem.className + " ").replace(rclass, " ").indexOf(className) > -1) {
				return true;
			}

		};
		return false;
	}

	pcHelper.descendant = function (elem, pattern, action) {
		//pattern : 节点的XPath形式（只能单条链）
		var regexNameOnly = /^\w+$/i; /* 只有 */
		var regexNameWithIndex = /^\w+\[-?\d+\]$/i; /*带索引器的*/

		function directChild(elem, nodeName, arrToFill, position) {
			var success = false;
			var pos = 0;
			for (var anode = elem.firstChild; anode; anode = anode.nextSibling) {
				if (anode.nodeType === 1 && anode.nodeName.toLowerCase() === nodeName) {
					if (position === pos) {
						arrToFill.push(anode);
						success = true;
						break;
					}
					pos++;
				}
			}
			anode = null;
			return success;
		}

		function directChildReverse(elem, nodeName, arrToFill, position) {
			var success = false;
			var pos = 0;

			for (var anode = elem.lastChild; anode; anode = anode.previousSibling) {
				if (anode.nodeType === 1 && anode.nodeName.toLowerCase() === nodeName) {
					if (position === pos) {
						arrToFill.push(anode);
						success = true;
						break;
					}
					pos++;
				}
			}
			anode = null;
			return success;
		}

		var ptArr = pattern.split("/");
		var atf = [];
		var rst;
		var curElem = elem;
		var len = ptArr.length;
		var ok = true;
		for (var q = 0; q < len; q++) {
			var p = ptArr[q];
			rst = false;
			if (regexNameOnly.test(p)) {
				rst = directChild(curElem, p, atf, 0);
			} else if (regexNameWithIndex.test(p)) {
				var i = p.indexOf("[");
				var j = p.indexOf("]");
				var parm0 = p.substring(i + 1, j);
				var v = parseInt(parm0);
				rst = parm0.substring(0, 1) != "-" ? directChild(curElem, p.substring(0, i), atf, v) : directChildReverse(curElem, p.substring(0, i), atf, v);
			}


			if (!rst) {
				ok = false;
				break;
			} else {
				curElem = atf.pop();
			}
		}
		if (ok && typeof (action) === "function") {
			action.apply(curElem);
		}
		curElem = null;
		return ok;
	}

	pcHelper.addClass = function (elem, value) {
		/// 给元素添加class名（多个以空格分隔）
		if (typeof elem === "string") {
			elem = document.getElementById(elem);
		}
		if (typeof Sys !== undefined && Sys.UI.DomElement.addCssClass)
			return Sys.UI.DomElement.addCssClass(elem, value); //使用Ajax框架
		else if (value && typeof value === "string") {
			classNames = value.split(rspace);
			if (elem.nodeType === 1) {
				if (!elem.className && classNames.length === 1) {
					elem.className = value;

				} else {
					setClass = " " + elem.className + " ";

					for (c = 0, cl = classNames.length; c < cl; c++) {
						if (! ~setClass.indexOf(" " + classNames[c] + " ")) {
							setClass += classNames[c] + " ";
						}
					}
					elem.className = setClass.trim();
				}
			}
		};
	};

	pcHelper.removeClass = function (elem, value) {
		var classNames, i, l, className, c, cl;

		if ((value && typeof value === "string") || value === undefined) {
			classNames = (value || "").split(rspace);
			if (typeof elem === "string") {
				elem = document.getElementById(elem);
			}
			if (typeof Sys !== undefined && Sys.UI.DomElement.removeCssClass)
				return Sys.UI.DomElement.removeCssClass(elem, value); //使用Ajax框架
			else if (elem.nodeType === 1 && elem.className) {
				if (value) {
					className = (" " + elem.className + " ").replace(rclass, " ");
					for (c = 0, cl = classNames.length; c < cl; c++) {
						className = className.replace(" " + classNames[c] + " ", " ");
					}
					elem.className = className.trim();

				} else {
					elem.className = "";
				}
			}
		}
		elem = null;
	};

	pcHelper.toggleClass = function (elem, value) {
		if (pcHelper.hasClass(elem, value))
			pcHelper.removeClass(elem, value);
		else
			pcHelper.addClass(elem, value);
	}

	pcHelper.getDisabled = function (elem) {
		var rst = false;
		if (typeof elem === "string") {
			elem = document.getElementById(elem);
		}
		if (elem) {
			if (elem.getAttribute) {
				rst = elem.getAttribute("disabled");
			} else {
				rst = elem["disabled"];
			}
			if (rst === null)
				rst = false;
			else if (rst === 'disabled') {
				rst = true;
			}
		}
		elem = null;
		return rst;
	}

	pcHelper.confirmClose = function (fun) {
		//关闭页面要求确认
		window.onbeforeunload = fun;
	}

	pcHelper.modelessPopup = function (elem) {
		// 无模态弹出方式弹出超链接窗口
		if (elem && elem.nodeType === 1 && elem.nodeName.toUpperCase() === "A") {
			var href = '';
			if (elem.getAttribute)
				href = elem.getAttribute("href");
			else
				href = elem["href"];
			pcHelper.showDialog(href, '', null, true, 800, 600, true);
			return false;
		} else {
			return true;
		}
	}

	pcHelper.modalPopup = function (elem) {
		// 无模态弹出方式弹出超链接窗口
		if (elem && elem.nodeType === 1 && elem.nodeName.toUpperCase() === "A") {
			var href = '';
			if (elem.getAttribute)
				href = elem.getAttribute("href");
			else
				href = elem["href"];
			pcHelper.showDialog(href, '', null, false, 800, 600, true);
			return false;
		} else {
			return true;
		}
	}

	pcHelper.showDialog = function (url, param, callback, modeless, width, height, resizeable) {
		//显示对话框，本身返回对话框的返回值，但建议考虑callback用法
		var rst;
		var pp = [];
		if (url.indexOf('?') >= 0) {
			//url += "&magicTrick=" + Math.random();
		} else {
			//url += "?magicTrick=" + Math.random();
		}
		if (width)
			pp.push("dialogWidth=" + width + "px");
		if (height)
			pp.push("dialogHeight=" + height + "px");
		if (resizeable)
			pp.push("resizable=1");
		try {
			if (!modeless) {
				rst = showModalDialog(url, param, pp.join("; "));
			} else {
				rst = showModelessDialog(url, param, pp.join("; "));
			}
			if (callback)
				callback(rst);
			return rst;
		} catch (ex) {
			alert("您的浏览器可能阻止了弹出窗口，请更改浏览器的安全级别以允许弹出窗口");
			throw ex;
		}

	};
	pcHelper.getDialogParam = function () {
		return window.dialogArguments;
	};
	pcHelper.setDialogResult = function (rst) {
		window.returnValue = rst;
	};
	pcHelper.showWindow = function (url, name, feat) {
		return window.open(url, name, features);

	};

	//通信部分
	pcHelper.oneWayRequest = function (url, success, error) {
		var img = document.createElement("img");
		img.className = "display:none";
		document.body.appendChild(img);
		if (typeof (success) === 'function') {
			img.onload = success;
		}

		if (typeof (error) === 'function') {
			img.onerror = error;
		}

		img.src = url;
		img = null;
	}

	pcHelper.jsonRequest = function (url, method, body, additionHeader, onSuccess, onError) {
		//异步请求一个json服务。
		if (typeof Sys !== 'undefined') {
			var wRequest = new Sys.Net.WebRequest();
			wRequest.set_url(url);
			wRequest.set_httpVerb(method || "POST");
			wRequest.set_body(body);

			var header = wRequest.get_headers();
			if (additionHeader) {
				for (var h in additionHeader) {
					if (typeof h !== 'undefined') {
						header[h] = additionHeader[h];
					}
				}
			}
			wRequest.add_completed(function (executor, args) {
				if (executor.get_responseAvailable()) {
					var status = executor.get_statusCode();
					if (status === 200) {
						ct = executor.getResponseHeader("Content-Type");
						if (ct && ct.indexOf('application/json') >= 0) {
							if (onSuccess) {
								onSuccess(executor.get_object());
							}
						} else if (onError) {
							onError(status, "服务器返回非json数据");
						}
					} else {
						if (onError) {
							onError(status, "服务器返回错误状态");
						}
					}
				}
				else {
					if (executor.get_timedOut()) {
						pcHelper.console.error("通信超时");
						if (onError) {
							onError(0, "通信超时");
						}
					}
					else if (executor.get_aborted()) {
						pcHelper.console.error("通信操作已中断");
						if (onError) {
							onError(0, "通信中断");
						}
					}
				}
				executor = null;
				args = null;

			});
			wRequest.invoke();
			wRequest = null;
			return true;

		} else {
			pcHelper.console.error("没有ASP.NET Ajax扩展未定义，无法进行通信");
			return false;
		}
	}

	pcHelper.ex = pcHelper.ex || {};

	pcHelper.ex.frameAgent = function (elem) {
		//提供框架功能
		this._frameSet = elem;
		this._leftPanel = null;
		this._rightPanel = null;
		this._splitter = null;
		this._overlay = null;
		this._initTop = null;
		this._initBottom = null;

	}
	pcHelper.ex.frameAgent.prototype = {
		config: function () {
			if (this._frameSet) {
				var node, node1;
				for (node = this._frameSet.firstChild; node; node = node.nextSibling) {
					if (node.nodeType === 1) {
						if (pcHelper.hasClass(node, "pc-frame-left")) {
							this._leftPanel = node;
						} else if (pcHelper.hasClass(node, "pc-frame-right")) {
							this._rightPanel = node;
						} else if (pcHelper.hasClass(node, "pc-frame-splitter-mask")) {
							this._overlay = node;
							for (node1 = this._overlay.firstChild; node1; node1 = node1.nextSibling) {
								if (node1.nodeType === 1 && pcHelper.hasClass(node1, "pc-frame-splitter")) {
									this._splitter = node1;
									node1 = null;
									break;
								}
							}
						}
					}
				}
				var b = Sys.UI.DomElement.getBounds(this._frameSet);
				this._initTop = b.y;
				this._initBottom = 0; //document.body.clientHeight - (b.y + b.height);

				if (this._splitter) {
					this._startSplitMouseHandler = Function.createDelegate(this, this._startSplitMouse);
					$addHandler(this._splitter, "mousedown", this._startSplitMouseHandler);
				}
				$addHandler(window, "resize", Function.createDelegate(this, function () {
					this._handleResize();
				}));
				this._handleResize();
			}
		},
		_handleResize: function () {
			var b = Sys.UI.DomElement.getBounds(this._frameSet);
			var h = document.documentElement.clientHeight;
			var bt = h - this._initBottom;
			if (bt > 0) {
				bt = bt - b.y;
				if (bt > 10) {
					this._frameSet.style.height = (bt) + "px";
				}
			}

		}, _startSplitMouse: function (evt) {
			if (this._inResizing)
				return;
			this._leftPanelWidth = Sys.UI.DomElement.getBounds(this._leftPanel).width;
			this._mouseDownX = evt.clientX;
			this._doSplitMouseHanler = Function.createDelegate(this, this._doSplitMouse);
			this._endSplitMouseHandler = Function.createDelegate(this, this._endSplitMouse);
			$addHandler(document, "mousemove", this._doSplitMouseHanler);
			$addHandler(document, "mouseup", this._endSplitMouseHandler);
			if (this._overlay) {
				pcHelper.addClass(this._overlay, "pc-overlay-mask");
			}
			this._inResizing = true;
			document.ondragstart = function () { return false; };
			return false;

		}, _doSplitMouse: function (evt) {
			this._resplit(evt);

		}, _endSplitMouse: function (evt) {
			$removeHandler(document, "mousemove", this._doSplitMouseHanler);
			$removeHandler(document, "mouseup", this._endSplitMouseHandler);
			this._doSplitMouseHanler = null;
			this._endSplitMouseHandler = null;
			if (this._overlay) {
				pcHelper.removeClass(this._overlay, "pc-overlay-mask");
			}
			this._inResizing = false;

		}, _resplit: function (evt) {
			var x = evt.clientX;
			var offset = x - this._mouseDownX;
			var lw = x - 2;
			var tw = this._frameSet.offsetWidth;
			if (lw > 10 && tw - lw > 10) {
				this._leftPanel.style.width = lw + "px";
				this._rightPanel.style.marginLeft = (x + 3) + "px";
				this._splitter.style.marginLeft = (x - 2) + "px";
			}

		}
		, handleResize: function () {
			this._handleResize();
		}
	}

	pcHelper.ex.spinScroller = function (elem) {
		this._container = elem;
		this._spinUp = null;
		this._spinDown = null;
		this._spinContentContainer = null;
		this._spinContent = null;
		this._offset = 0;
		this._timer = null;
		this._delta = 0;
		this._spinUpVisible = false;
		this._spinDownVisible = false;
	};

	pcHelper.ex.spinScroller.prototype = {
		config: function (trigger) {
			if (this._container) {
				for (var node = this._container.firstChild; node; node = node.nextSibling) {
					if (node.nodeType === 1) {
						if (pcHelper.hasClass(node, "pc-spin-up"))
							this._spinUp = node;
						else if (pcHelper.hasClass(node, "pc-spin-down"))
							this._spinDown = node;
						else if (pcHelper.hasClass(node, "pc-spin-container")) {
							this._spinContentContainer = node;
						}
					}
				}

				if (this._spinContentContainer) {
					for (node = this._spinContentContainer.firstChild; node; node = node.nextSibling) {
						if (node.nodeType === 1 && node.nodeName.toUpperCase() === "UL") {
							this._spinContent = node;
							break;
						}
					}
				}

				this._handleTriggerHandler = Function.createDelegate(this, this._handleTrigger);
				this._handleSpinDownHandler = Function.createDelegate(this, this._handleSpinDown);
				this._handleSpinUpHandler = Function.createDelegate(this, this._handleSpinUp);
				this._handleSpinOutHandler = Function.createDelegate(this, this._handleSpinOut);
				this._handleSpinDeltaHandler = Function.createDelegate(this, this._handleSpinDelta);
				this._handleContentScrollHandler = Function.createDelegate(this, this._handleContentScroll);

				pcHelper.bindEvent(trigger, "mouseenter", this._handleTriggerHandler);
				pcHelper.bindEvent(this._spinUp, "mouseenter", this._handleSpinUpHandler);
				pcHelper.bindEvent(this._spinDown, "mouseenter", this._handleSpinDownHandler);
				pcHelper.bindEvent(this._spinUp, "mouseout", this._handleSpinOutHandler);
				pcHelper.bindEvent(this._spinDown, "mouseout", this._handleSpinOutHandler);
				pcHelper.bindEvent(this._spinContentContainer, "mousewheel", this._handleContentScrollHandler);
				pcHelper.bindEvent(this._spinContentContainer, "DOMMouseScroll", this._handleContentScrollHandler);

				node = null;
			}
		},
		_handleTriggerHandler: null,
		_handleSpinUpHandler: null,
		_handleSpinDownHandler: null,
		_handleSpinOutHandler: null,
		_handleSpinDeltaHandler: null,
		_handleContentScrollHandler: null,
		_handleTrigger: function () {
			var that = this;
			setTimeout(function () {
				that._onResize();
			}, 200);
		}, _handleSpinUp: function () {
			pcHelper.console.info("spin Up");
			this._delta = -5;
			this._timer = setInterval(this._handleSpinDeltaHandler, 100);
		}, _handleSpinDown: function () {
			pcHelper.console.info("spin Ddown");
			this._delta = 5;
			this._timer = setInterval(this._handleSpinDeltaHandler, 100);
		}, _handleSpinOut: function () {
			if (this._timer) {
				this._delta = 0;
				clearInterval(this._timer);
			}
		}, _handleContentScroll: function (e) {
			e = e || window.event;
			if (typeof e.preventDefault === 'function') {
				e.preventDefault();
			}
			if ("returnValue" in e)
				e.returnValue = false;

			this._delta = 0;
			if (e.wheelDelta) {
				//IE,Opera,Chrome
				if (e.wheelDelta > 0) {
					this._delta = -15;
				} else {
					this._delta = 15;
				}

			} else if (e.detail) {
				//FF
				if (e.detail > 0) {
					this._delta = 15;
				} else {
					this._delta = -15;
				}
			}
			this._handleSpinDelta();
		}, _handleSpinDelta: function () {
			var newTop = this._offset + this._delta;
			var upSpan = newTop;
			var downSpan = newTop - (this._contentHeight - this._containerHeight);
			pcHelper.console.info("up span /down span:" + upSpan + "," + downSpan);
			if (upSpan < 0) {
				newTop = 0;
				pcHelper.hide(this._spinUp);
				this._spinUpVisible = false;
				this._handleSpinOut();
			} else if (downSpan > 0) {
				newTop = this._contentHeight - this._containerHeight;
				pcHelper.hide(this._spinDown);
				this._spinDownVisible = false;
				this._handleSpinOut();
			} else {
				if (newTop > 0 && !this._spinUpVisible) {
					pcHelper.show(this._spinUp);
					this._spinUpVisible = true;
				}
				if (downSpan < 0 && !this._spinDownVisible) {
					pcHelper.show(this._spinDown);
					this._spinDownVisible = true;
				}
				this._offset = newTop;
				this._spinContent.style.marginTop = (-newTop) + "px";
				pcHelper.console.info("spin newTop: " + newTop);
			}
		}, _onResize: function () {
			pcHelper.console.info("spin content resize");
			this._contentHeight = this._spinContent.offsetHeight;
			this._containerHeight = this._spinContentContainer.parentNode.scrollHeight;
			pcHelper.console.info(" contentHeight: " + this._contentHeight + "  containerHeight:" + this._containerHeight);
			if (this._spinUpVisible = (this._offset > 0)) {
				pcHelper.show(this._spinUp);

			} else {
				pcHelper.hide(this._spinUp);
			}

			if (this._spinDownVisible = (this._contentHeight - this._containerHeight - this._offset > 0)) {
				pcHelper.show(this._spinDown);
			} else {
				pcHelper.hide(this._spinDown);
			}
		}
	}

	//ui部分
	pcHelper.ui = {

		gridBehavior: function (elem, type, param) {
			//向deluxeGrid应用客户端行为
			if (typeof elem === "string") {
				elem = document.getElementById(elem);
			}
			function doHoverMouseEnter(e) {
				if (this === window) { // IE damn it.
					pcHelper.addClass(e.srcElement, "hover");
				} else {
					pcHelper.addClass(this, "hover");
				}
			}
			function doHoverMouseLeave(e) {
				if (this === window) {
					pcHelper.removeClass(e.srcElement, "hover");
				} else {
					pcHelper.removeClass(this, "hover");
				}
			}
			function doToggle() {
				pcHelper.toggleClass(this, "toggle");
			}

			if (elem && elem.nodeType === 1 && elem.nodeName.toUpperCase() === 'TABLE') {
				var body = pcHelper.getChildNode(elem, 1, 'TBODY');
				if (body) {
					if (type === "hover") {
						for (node = body.firstChild; node; node = node.nextSibling) {
							if (node.nodeType === 1 && node.nodeName.toUpperCase() === 'TR') {
								if (pcHelper.hasClass(node, "item") || pcHelper.hasClass(node, "aitem")) {
									pcHelper.bindEvent(node, "mouseenter", doHoverMouseEnter);
									pcHelper.bindEvent(node, "mouseleave", doHoverMouseLeave);
								}
							}
						}
					} else if (type == "toggle") {
						for (node = body.firstChild; node; node = node.nextSibling) {
							if (node.nodeType === 1 && node.nodeName.toUpperCase() === 'TR') {
								if (pcHelper.hasClass(node, "item") || pcHelper.hasClass(node, "aitem")) {
									pcHelper.bindEvent(node, "click", pcHelper.createDelegate(node, doToggle));
								}
							}
						}
					}
					body = null;
					elem = null;
					node = null;
					return true;
				}

			}
			return false;

		}, listMenuBehavior: function (elem, type, param) {
			/// 给列表菜单的下拉列表指定行为，elem为菜单元素的ul
			if (typeof elem === "string") {
				elem = document.getElementById(elem);
			}
			function doHoverMouseEnter(e) {
				if (this === window) { // IE damn it.
					pcHelper.addClass(e.srcElement, "hover");
				} else {
					pcHelper.addClass(this, "hover");
				}
			}
			function doHoverMouseLeave(e) {
				if (this === window) {
					pcHelper.removeClass(e.srcElement, "hover");
				} else {
					pcHelper.removeClass(this, "hover");
				}
			}
			if (elem && elem.nodeType === 1 && elem.nodeName.toUpperCase() === 'UL') {
				for (node = elem.firstChild; node; node = node.nextSibling) {
					if (node.nodeType === 1 && node.nodeName.toUpperCase() === 'LI') {
						if (pcHelper.hasClass(node, "pc-dropdownmenu")) {
							pcHelper.bindEvent(node, "mouseenter", doHoverMouseEnter);
							pcHelper.bindEvent(node, "mouseleave", doHoverMouseLeave);
						}

					}
				}
				body = null;
				elem = null;
				node = null;
				return true;

			}
			return false;

		}, hoverBehavior: function (elem) {
			if (elem) {
				if (typeof elem === "string")
					elem = document.getElementById(elem);

				pcHelper.bindEvent(elem, "mouseenter", function () { pcHelper.addClass(elem, "hover"); });
				pcHelper.bindEvent(elem, "mouseleave", function () { pcHelper.removeClass(elem, "hover"); });

			}
		}, getRadioListValue: function (ul) {
			//获取RadioButtonList的选定项的value
			if (typeof ul === "string") {
				ul = document.getElementById(ul);
			}
			var rst = undefined;
			var node = null, rdo = null;
			if (ul && ul.nodeType === 1 && ul.nodeName.toUpperCase() === "UL") {
				for (node = ul.firstChild; node; node = node.nextSibling) {
					for (rdo = node.firstChild; rdo; rdo = rdo.nextSibling) {
						if (rdo.nodeType === 1 && rdo.nodeName.toUpperCase() === "INPUT" && pcHelper.getAttr(rdo, "type") === "radio") {
							if (rdo.checked) {
								rst = pcHelper.getAttr(rdo, "value");
								break;
							}
						}
					}
				}
			}
			rdo = null;
			node = null;
			ul = null;
			return rst;
		}, setRadioAnySelected: function (ul) {
			if (typeof ul === "string") {
				ul = document.getElementById(ul);
			}
			var rst = undefined;
			var node = null, rdo = null;
			if (ul && ul.nodeType === 1 && ul.nodeName.toUpperCase() === "UL") {
				for (node = ul.firstChild; node; node = node.nextSibling) {
					for (rdo = node.firstChild; rdo; rdo = rdo.nextSibling) {
						if (rdo.nodeType === 1 && rdo.nodeName.toUpperCase() === "INPUT" && pcHelper.getAttr(rdo, "type") === "radio") {
							rdo.checked = true;
							break;
						}
					}
				}
			}
			rdo = null;
			node = null;
			ul = null;
			return rst;
		},
		findScrollTop: function (e) {
			//查找scrollTop
			var v = 0;
			var p;
			for (p = e; p.nodeType === 1; p = p.parentNode) {
				v += p.scrollTop;
			}
			p = null;
			return v;
		},
		configFrame: function (elem) {
			//配置水平分割的框架
			if (typeof elem === 'string') {
				elem = document.getElementById(elem);
			}
			if (elem) {
				var agent = new pcHelper.ex.frameAgent(elem);
				agent.config();
			}
			elem = null;
		},
		configSpinner: function (list, trigger) {
			if (typeof list === 'string') {
				list = document.getElementById(list);
			}
			if (typeof trigger === 'string') {
				trigger = document.getElementById(trigger);
			}
			if (list && trigger) {
				var agent = new pcHelper.ex.spinScroller(list);
				agent.config(trigger);
			}
			elem = null;
		}, configHistoryList: function (elem) {
			if (typeof elem === 'string') {
				elem = document.getElementById(elem);
			}

			if (elem) {
				if (elem.nodeType === 1 && elem.nodeName.toUpperCase() === "DL") {
					var node = null;
					var lastTime = '';
					var nodeDt = null;
					var nodeT = null;
					var nodeI = null;

					for (node = elem.firstChild; node; node = node.nextSibling) {
						if (node.nodeType === 1 && node.nodeName.toUpperCase() === "DD") {
							var ve = pcHelper.getAttr(node, "data-ve");
							var vs = pcHelper.getAttr(node, "data-vs");
							if (ve !== lastTime) {
								lastTime = ve;
								nodeDt = document.createElement("DT");
								nodeDt.className = "pc-time-group";
								nodeI = document.createElement("span");
								nodeI.className = "pc-lead";
								nodeDt.appendChild(nodeI);
								nodeT = document.createTextNode(vs);
								nodeDt.appendChild(nodeT);
								elem.insertBefore(nodeDt, node);


							}
						}
					}

					node = null;
					nodeDt = null;
					nodeT = null;
					nodeI = null;
				}

			}

		},
		autoSticky: function (elem) {
			//滚动时固定（支持IE7之后的浏览器）
			if (typeof elem === 'string') {
				elem = document.getElementById(elem);
			}

			var adjustSticky = Function.createDelegate({ initScrollTop: pcHelper.ui.findScrollTop(elem), elem: elem }, function () {
				var scrollTop1 = pcHelper.ui.findScrollTop(this.elem);
				if (scrollTop1 > this.initScrollTop) {
					$pc.addClass(this.elem, "pc-sticky");
				} else {
					$pc.removeClass(this.elem, "pc-sticky");
				}

			});

			if (elem) {
				var scrollTop = pcHelper.ui.findScrollTop(elem);
				Sys.Application.add_init(function () {
					$addHandler(window.document, "scroll", adjustSticky, false);
				});
			}
		}, traceWindowWidth: function () {
			pcHelper.bindEvent(window, "resize", function () {
				document.body.style.width = window.document.documentElement.clientWidth + "px";
			});
		}
	}

	pcHelper.console = {
		//控制台方法
		debug: false, //发布时将此属性设置为false
		info: function (text) {
			if (this.debug && window.console && window.console.info) {
				console.info('权限中心调试信息:' + text);
			}
		}, error: function (text) {
			if (this.debug && window.console && window.console.error) {
				console.error('权限中心调试错误:' + text);
			}
		}, assert: function (expr, textIfFail) {
			if (this.debug && window.console && window.console.assert) {
				console.assert(expr, '权限中心调试断言失败:' + (textIfFail = textIfFail || "某处断言失败"));
			}
		}, log: function (text) {
			if (this.debug && window.console && window.console.log) {
				console.log('权限中心调试日志:' + text);
			}
		}
	}

	pcHelper.popups = {
		//基本的弹窗
		newMember: function (obj) {
			//新建人员弹窗，obj为项目菜单（将在其中查找参数），或者schema名
			if (typeof (event) !== 'undefined' && event.srcElement && pcHelper.getDisabled(event.srcElement))
				return false;
			function doPop(schema, parent, callback) {
				if (!schema) {
					schema = pcHelper.showDialog(pcHelper.appRoot + "dialogs/MemberSchemaPicker.aspx", '', null, false, 640, 480, true);
				}
				if (typeof schema != 'undefined' && schema != '') {
					if (pcHelper.showDialog(pcHelper.appRoot + "dialogs/showObjectInfo.aspx?schemaType=" + schema + (parent ? "&parentID=" + parent : ""), '', null, false, 800, 600, true) === true)
						return true;
				}
				return false;
			}

			if (obj.nodeType && obj.nodeType === 1) //DOM，从元素上查找参数
			{
				var schema = pcHelper.getAttr(obj, "data-schema");
				var parent = pcHelper.getAttr(obj, "data-parentid");
				if (pcHelper.getDisabled(obj)) {
					return; //该按钮被禁用
				}
				return doPop(schema, parent, null);
			}
			else if (typeof obj === "string")
				return doPop(obj, null, null);
		},
		editProperty: function (obj) {
			//编辑对象基本属性对话框，obj为命令元素，或者对象的ID
			if (typeof (event) !== 'undefined' && event.srcElement && pcHelper.getDisabled(event.srcElement))
				return false;
			function cbAfterCommit(rst) {
				pcHelper.console.log("对象编辑对话框返回了值：" + rst);
			}
			var key = null;
			if (obj.nodeType && obj.nodeType === 1) //DOM
			{
				key = pcHelper.getAttr(obj, "data-id");
			}
			else if (typeof obj === "string" && rguid.test(obj)) {
				key = obj;
			}
			if (key) {
				if (pcHelper.showDialog(pcHelper.appRoot + "dialogs/showObjectInfo.aspx?id=" + key, '', cbAfterCommit, false, 800, 600, true) === true)
					return true;
			}
			return false;
		}, batchProperties: function (gridId) {
			var grid = gridId;
			if (typeof grid === 'string')
				grid = $find(gridId);

			if (grid) {
				return pcHelper.showDialog(pcHelper.appRoot + "dialogs/BatchEditorIntro.htm", grid.get_clientSelectedKeys(), null, false, 900, 600, true);
			} else {
				pcHelper.console.error("无法找到Grid");
			}

			grid = null;
		}, historyProperty: function (obj, time) {
			//编辑对象基本属性对话框，obj为命令元素，或者对象的ID
			if (typeof (event) !== 'undefined' && event.srcElement && pcHelper.getDisabled(event.srcElement))
				return false;
			function cbAfterCommit(rst) {
				pcHelper.console.log("对象编辑对话框返回了值：" + rst);
			}

			var key = null;

			key = null;
			vtime = time;
			if (obj) {
				if (typeof (obj) === "string") {
					key = obj;
				} else {
					if (obj.nodeType && obj.nodeType === 1) //DOM
					{
						key = pcHelper.getAttr(obj, "data-id");
						vtime = pcHelper.getAttr(obj, "data-time");
					}
				}

				if (key != null && vtime != null) {
					if (pcHelper.showDialog(pcHelper.appRoot + "dialogs/showObjectInfo.aspx?reserved=1&id=" + key + "&time=" + vtime, '', cbAfterCommit, false, 800, 600, true) === true)
						return true;
				}
			}

			return false;
		}, newOrg: function (obj) {
			//新建组织弹窗
			if (typeof (event) !== 'undefined' && event.srcElement && pcHelper.getDisabled(event.srcElement))
				return false;
			var parentKey = null;
			if (typeof (obj) != 'undefined' && obj.nodeType === 1) { //从DOM捞数据
				parentKey = pcHelper.getAttr(obj, "data-parentid");
			} else if (typeof obj === "string" && rguid.test(obj)) {
				parentKey = obj;
			}
			if (parentKey) {
				if (pcHelper.showDialog(pcHelper.appRoot + "dialogs/showObjectInfo.aspx?schemaType=Organizations&parentID=" + encodeURIComponent(parentKey), '', null, false, 800, 600, true) === true)
					return true;
			}
			return false;

		},
		newGrp: function (obj) {
			//新建群组弹窗
			if (typeof (event) !== 'undefined' && event.srcElement && pcHelper.getDisabled(event.srcElement))
				return false;
			var parentKey = null;
			if (obj && obj.nodeType === 1) { //从DOM捞数据
				parentKey = pcHelper.getAttr(obj, "data-parentid");
			} else if (typeof obj === "string" && rguid.test(obj)) {
				parentKey = obj;
			}
			if (parentKey) {
				if (pcHelper.showDialog(pcHelper.appRoot + "dialogs/showObjectInfo.aspx?schemaType=Groups&parentID=" + encodeURIComponent(parentKey), '', null, false, 800, 600, true) === true)
					return true;
			} else {
				if (pcHelper.showDialog(pcHelper.appRoot + "dialogs/showObjectInfo.aspx?schemaType=Groups", '', null, false, 800, 600, true) === true)
					return true;
			}
			return false;
		},
		newApp: function (obj) {
			if (typeof (event) !== 'undefined' && event.srcElement && pcHelper.getDisabled(event.srcElement))
				return false;
			// 新建应用
			if (pcHelper.showDialog(pcHelper.appRoot + 'dialogs/showObjectInfo.aspx?schemaType=Applications', '', null, false, 800, 600, true) === true) {
				return true;
			}
			return false;
		}
        ,
		newRole: function (obj) {
			//新建角色，obj为应用ID或者按钮元素
			if (typeof (event) !== 'undefined' && event.srcElement && pcHelper.getDisabled(event.srcElement))
				return false;
			var parentKey = null;
			if (obj && obj.nodeType === 1) { //从DOM捞数据
				parentKey = pcHelper.getAttr(obj, "data-parentid");
			} else if (typeof obj === "string" && rguid.test(obj)) {
				parentKey = obj;
			}
			if (parentKey) {
				if (pcHelper.showDialog(pcHelper.appRoot + "dialogs/showObjectInfo.aspx?schemaType=Roles&parentID=" + encodeURIComponent(parentKey), '', null, false, 800, 600, true) === true)
					return true;
			} else {
				pcHelper.console.error("没有指定应用ID");
			}
			return false;
		},
		newFunction: function (obj) {
			if (typeof (event) !== 'undefined' && event.srcElement && pcHelper.getDisabled(event.srcElement))
				return false;
			var parentKey = null;
			if (obj && obj.nodeType === 1) { //从DOM捞数据
				parentKey = pcHelper.getAttr(obj, "data-parentid");
			} else if (typeof obj === "string" && rguid.test(obj)) {
				parentKey = obj;
			}
			if (parentKey) {
				if (pcHelper.showDialog(pcHelper.appRoot + "dialogs/showObjectInfo.aspx?schemaType=Permissions&parentID=" + encodeURIComponent(parentKey), '', null, false, 800, 600, true) === true)
					return true;
			} else {
				pcHelper.console.error("没有指定应用ID");
			}
			return false;
		}, conditonHistory: function (containerID) {
			pcHelper.showDialog(pcHelper.appRoot + "dialogs/ConditionHistory.aspx?id=" + encodeURIComponent(containerID), "", null, false, 400, 600, true);
		},
		batchDelete: function (gridId, context) {
			if (typeof (event) !== 'undefined' && event.srcElement && pcHelper.getDisabled(event.srcElement))
				return false;
			var ctrl = gridId;
			var rst = false;
			if (typeof ctrl === 'string') {
				ctrl = $find(gridId);
			}
			if (ctrl) {
				var i;
				keys = ctrl.get_clientSelectedKeys();
				if (keys.length > 0) {
					if (keys.length < 20) {
						if (confirm('准备要删除这' + keys.length + '个项目，确定要继续？')) {
							rst = true;
						}
					} else {
						if (confirm('您选择删除' + keys.length + '个项目，选择过多的项目可能导致响应缓慢或超时，确定要继续？')) {
							rst = true;
						}
					}
				} else {
					//alert("请先选择要删除的对象，然后执行此操作。");
				}
			}
			ctrl = null;
			return rst;
		},
		searchMember: function (fillIn, reserved, urlParamsArr) {

			if (typeof (event) !== 'undefined' && event.srcElement && pcHelper.getDisabled(event.srcElement))
				return false;
			var result = false;
			var fillElem = fillIn;
			if (typeof fillElem === 'string') {
				fillElem = pcHelper.get(fillIn);
			}
			if (fillElem && fillElem.nodeType === 1 && fillElem.nodeName.toUpperCase() === "INPUT") {

				var url = pcHelper.appRoot + "dialogs/MemberSearchDialog.aspx";
				urlParamsArr = urlParamsArr || [];

				var pp = [];
				for (var i = urlParamsArr.length - 1; i >= 0; i--) {
					for (var key in urlParamsArr[i]) {
						pp.push(encodeURIComponent(key) + "=" + encodeURIComponent(urlParamsArr[i][key]));
					}
				}

				if (pp.length)
					url += "?" + pp.join("&");

				delete pp;
				pp = null;
				var rst = pcHelper.showDialog(url, "", null, false, 800, 600, true);
				if (typeof (rst) != 'undefined' && rst != '') {
					pcHelper.setAttr(fillElem, "value", rst);
					result = true;
				}
			}

			return result;
		},
		searchGroup: function (fillIn, reserved, urlParamsArr) {
			if (typeof (event) !== 'undefined' && event.srcElement && pcHelper.getDisabled(event.srcElement))
				return false;
			var result = false;
			var fillElem = fillIn;
			if (typeof fillElem === 'string') {
				fillElem = pcHelper.get(fillIn);
			}
			if (fillElem && fillElem.nodeType === 1 && fillElem.nodeName.toUpperCase() === "INPUT") {
				var url = pcHelper.appRoot + "dialogs/GroupSearchDialog.aspx";
				urlParamsArr = urlParamsArr || [];

				var pp = [];
				for (var i = urlParamsArr.length - 1; i >= 0; i--) {
					for (var key in urlParamsArr[i]) {
						pp.push(encodeURIComponent(key) + "=" + encodeURIComponent(urlParamsArr[i][key]));
					}
				}

				if (pp.length)
					url += "?" + pp.join("&");

				delete pp;
				pp = null;

				var rst = pcHelper.showDialog(url, "", null, false, 800, 600, true);
				if (typeof (rst) != 'undefined' && rst != '') {
					pcHelper.setAttr(fillElem, "value", rst);
					result = true;
				}
			}
			return result;
		},
		searchOrg: function (fillIn, single, urlParamsArr, excludes) {
			var result = false;
			var fillElem = fillIn;

			if (typeof fillElem === 'string') {
				fillElem = pcHelper.get(fillIn);
			}
			if (fillElem && (fillElem.nodeType === 1 && fillElem.nodeName.toUpperCase() === "INPUT")) {
				var url = pcHelper.appRoot + "dialogs/InterOrgSearch.htm";
				urlParamsArr = urlParamsArr || [];
				if (single) {
					urlParamsArr.push({ "mode": "single" });
					//url += "?mode=single";
				}

				var pp = [];
				for (var i = urlParamsArr.length - 1; i >= 0; i--) {
					for (var key in urlParamsArr[i]) {
						pp.push(encodeURIComponent(key) + "=" + encodeURIComponent(urlParamsArr[i][key]));
					}
				}

				if (pp.length)
					url += "?" + pp.join("&");

				delete pp;
				pp = null;

				//				var rst = pcHelper.showDialog(url, (objMode ? fillElem : ""), null, false, 800, 600, true);
				//				if (typeof (rst) != 'undefined' && rst != '') {
				//					pcHelper.setAttr(fillElem, "value", rst);
				//					result = true;
				//				}

				var returnObj = {
					fillElem: fillElem
				};

				if (excludes) {
					returnObj.excludes = excludes;
				}

				if (pcHelper.showDialog(url, returnObj, null, false, 800, 600, true) === true) {
					result = true;
				}
			}

			return result;
		}, searchOrgOfUser: function (userId, fillIn, single) {
			var result = false;
			var fillElem = fillIn;
			if (typeof fillElem === 'string') {
				fillElem = pcHelper.get(fillIn);
			}

			if (fillElem && fillElem.nodeType === 1 && fillElem.nodeName.toUpperCase() === "INPUT") {
				var url = pcHelper.appRoot + "dialogs/OrgSearchDialog.aspx?userid=" + encodeURIComponent(userId);
				if (single) {
					url += "&mode=single";
				}
				var rst = pcHelper.showDialog(url, "", null, false, 800, 600, true);
				if (typeof (rst) != 'undefined' && rst != '') {
					pcHelper.setAttr(fillElem, "value", rst);
					result = true;
				}
			}
			return result;
		}, pickTime: function (trigger) {
			var url = pcHelper.appRoot + "dialogs/TimePicker.aspx";
			var success = false;
			var rst = pcHelper.showDialog(url, "", null, false, 300, 300, true);
			if (typeof (rst) != 'undefined' && rst != '') {
				if (typeof (trigger) === 'string')
					trigger = $get(trigger);
				if (typeof (trigger) !== 'undefined' && trigger.nodeType === 1) {
					var fillElem;
					if (trigger.getAttribute) {
						fillElem = $get(trigger.getAttribute('data-rlctl'));
					} else {
						fillElem = $get(trigger['data-rlctl']);
					}
					if (fillElem) {
						if (fillElem.nodeType === 1 && fillElem.nodeName.toUpperCase() === "INPUT") {
							fillElem.value = rst;
							success = true;
						}
					}

					fillElem = null;
				}
				trigger = null;
			}
			return success;
		}, browse: function (fillIn, schemaTypes) {
			if (typeof (event) !== 'undefined' && event.srcElement && pcHelper.getDisabled(event.srcElement))
				return false;
			var result = false;
			var fillElem = fillIn;
			if (typeof fillElem === 'string') {
				fillElem = pcHelper.get(fillIn);
			}

			urlParam = [];

			if (schemaTypes) {
				for (var i = schemaTypes.length - 1; i >= 0; i--) {
					urlParam.push("schemaType=" + schemaTypes[i]);
				}
			}

			urlParam = urlParam.join('&');

			if (fillElem && fillElem.nodeType === 1 && fillElem.nodeName.toUpperCase() === "INPUT") {
				var url = pcHelper.appRoot + "dialogs/UnitBrowseDialog.aspx?" + urlParam;
				var returnObj = {
					fillElem: fillElem
				};

				result = pcHelper.showDialog(url, returnObj, null, false, 800, 600, true);
			}

			return result;
		}

	};
	pcHelper.popups.batch = {
		membersToGroups: function (gridId, fillIn) {
			var grid = gridId;
			if (typeof grid === 'string')
				grid = $find(gridId);
			var fillInCtrl = $get(fillIn);
			var rst = false;
			if (grid && fillInCtrl) {
				keys = grid.get_clientSelectedKeys();
				if (keys && keys.length > 0) {
					rst = pcHelper.popups.searchGroup(fillInCtrl, 1, [{ "pp": "EditMembersOfGroups"}]);
				}
			}
			grid = null;
			fillInCtrl = null;
			return rst;
		},
		membersToOrgs: function (gridId, fillIn) {
			var grid = gridId;
			if (typeof grid === 'string')
				grid = $find(gridId);
			var fillInCtrl = $get(fillIn);
			var rst = false;
			if (grid && fillInCtrl) {
				keys = grid.get_clientSelectedKeys();
				if (keys && keys.length > 0) {
					rst = pcHelper.popups.searchOrg(fillInCtrl, 1, [{ "permission": "AddChildren"}]);

				}
			}
			grid = null;
			fillInCtrl = null;
			return rst;
		}
	};

	pcHelper.postViaIFrame = function (url, formItems) {
		if (typeof (url) !== 'string')
			throw Error();
		var _thePostForm = document.all ? document.all["_hidden_post_form"] : document.forms["_hidden_post_form"];
		if (!_thePostForm) {
			var div = document.createElement("div");
			document.body.appendChild(div);
			div.innerHTML = '<iframe id="_hidden_post_frame" name="_hidden_post_frame" scrolling="no" style="display: none">' +
        '您的浏览器不支持IFrame！，请尽快升级一个现代浏览器（当前仅支持IE7-9）。</iframe>' +
    '<form action="" name="_hidden_post_form" id="_hidden_post_form" method="post" target="_hidden_post_frame" style="display:none; height:0; width:0;">' +
    '</form>';
			_thePostForm = document.all ? document.all["_hidden_post_form"] : document.forms["_hidden_post_form"];
		}
		if (!_thePostForm)
			throw new "无法创建隐藏提交表单";

		_thePostForm.action = url;
		_thePostForm.innerHTML = '';

		var item, item2;
		var inp;
		var v;
		var isArray = false;
		if (formItems) {

			for (var key in formItems) {
				v = undefined;
				isArray = false;
				item = formItems[key];
				if (typeof (item) === 'string') {
					v = item;
				} else if (typeof (item) === 'object') {
					if (item.constructor === Array) {
						isArray = true;
					} else
						throw new "不可以提交复杂类型";

				} else if (typeof (item) === 'function') {
					throw new "不可以将函数作为表单数据：" + key;
				} else
					v = item;
				if (!isArray) {
					inp = document.createElement("input");
					inp['type'] = 'hidden';
					inp['name'] = key;
					inp['value'] = v;
					_thePostForm.appendChild(inp);
				} else {
					for (var i = item.length - 1; i >= 0; i--) {
						v = item[i];
						if (typeof (v) === 'undefined')
							throw new "不可以提交未定义类型";
						if (typeof (v) === 'object') {
							throw new "不可以提交复杂类型";
						} else if (typeof (v) === 'function') {
							throw new "不可以将函数作为表单数据：" + key;
						}

						inp = document.createElement("input");
						inp['type'] = 'hidden';
						inp['name'] = key;
						inp['value'] = v;
						_thePostForm.appendChild(inp);
					}
				}
			}
		}

		_thePostForm.submit();

	}

	pcHelper.observe = {
		autoAdjustHeight: function (iframe) {
			var cachedHeight = -1;
			var win = iframe;

			var fun = function () {

				try {

					var bHeight = iframe.contentWindow.document.body.scrollHeight;

					var dHeight = iframe.contentWindow.document.documentElement.scrollHeight;

					win.style.height = Math.max(bHeight, dHeight) + "px";

				} catch (ex) {
					pcHelper.console.error("自适应iframe 高度：" + ex.description)
				}

				//                if (win.document.body) {

				//                    $pc.console.info('scrollHeight:' + win.document.body.scrollHeight + ', scrollTop:' + win.document.body.scrollTop);
				//                    var t = win.document.body.scrollHeight - win.document.body.scrollTop;
				//                    if (t > cachedHeight) {
				//                        cachedHeight = t;
				//                        win.style.height = cachedHeight + "px";
				//                    }
				//                }

			};
			if (1/*window.document.documentMode && window.document.documentMode < 8*/) {
				fun();
				setInterval(fun, 300); //定时调整高度
			} else {
				fun = null;
			}
		}
	};

	pcHelper.animation = {};

	pcHelper.animation.circEaseOut = function (duration, action, actionFinish) {

		var context = { startTime: new Date().getTime(), duration: duration, action: action };
		context.timer = setInterval(function () {
			step(context);
			//clearInterval(context.timer);
		}, 50);

		function step(context) {
			var n = new Date().getTime() - context.startTime;
			p = n / context.duration;
			if (p < 1) {
				var firstNum = 0;
				var diff = 1;
				var c = firstNum + diff;
				var rst = c * Math.sqrt(1 - (p = p / 1 - 1) * p) + firstNum;
				context.action(rst);
			} else {
				clearInterval(context.timer);
				if (typeof actionFinish === 'function') {
					actionFinish();
				}
				delete context;
				context = null;
			}
		}

	};

	pcHelper.console.info("权限中心脚本初始化完毕");
	pcHelper.console.info("浏览器文档模式：" + pcHelper.getDocumentMode());
	window.$pc = window.$pc || pcHelper;
	try {
		//window.history.forward(1); //不许后退
	} catch (ex) {

	}

})(window);
