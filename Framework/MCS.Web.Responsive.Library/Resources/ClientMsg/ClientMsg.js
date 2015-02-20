var Bind = function (object, fun) {
	var args = Array.prototype.slice.call(arguments).slice(2);
	return function () {
		return fun.apply(object, args);
	};
};

var BindAsEventListener = function (object, fun) {
	var args = Array.prototype.slice.call(arguments).slice(2);
	return function (event) {
		return fun.apply(object, [event || window.event].concat(args));
	};
};


function ModalBox() {
	this.boxes = new Array();
	this.page = function () {
		return {
			top: function () { return document.documentElement.scrollTop || document.body.scrollTop; },
			width: function () { return self.innerWidth || document.documentElement.clientWidth || document.body.clientWidth; },
			height: function () { return self.innerHeight || document.documentElement.clientHeight || document.body.clientHeight; },
			total: function (d) {
				var b = document.body, elem = document.documentElement;
				return d ? Math.max(Math.max(b.scrollHeight, elem.scrollHeight), Math.max(b.clientHeight, elem.clientHeight)) :
                Math.max(Math.max(b.scrollWidth, elem.scrollWidth), Math.max(b.clientWidth, elem.clientWidth));
			}
		};
	} ();
}

function Box(options) {
	this.bg = null;
	this.container = null;
	this.widgetbody = null;
	this.rTop = null;
	this.rRight = null;
	this.rBottom = null;
	this.rLeft = null;
	this.rTopRight = null;
	this.rTopLeft = null;
	this.rBottomRight = null;
	this.rBottomLeft = null;
	this.oWidth = 0; //原始宽度
	this.oHeight = 0;
	this.height = 0;
	this.width = 0; //放大或缩小后的目标宽度

	this.init(options);
}

Box.prototype = {
	constructor: Box,
	init: function (options) {
		this.bg = $("<div class='box-bg' />").css({ 'z-Index': 2000 + options.level * 10, 'display': "none" }).attr('id', 'boxbg-' + options.level)[0];
		this.container = $("<div class='box-container' />").css({
			'z-Index': 2000 + options.level * 10 + 1,
			'position': "absolute", 'display': "none",
			'width': options.width ? options.width : 'auto',
			'height': options.height ? options.height : 'auto'
		}).attr('id', 'boxcontainer-' + options.level)[0];

		this.rTop = $("<div class='box-resizable box-top' />")[0];
		this.rRight = $("<div class='box-resizable box-right' />")[0];
		this.rBottom = $("<div class='box-resizable box-bottom' />")[0];
		this.rLeft = $("<div class='box-resizable box-left' />")[0];
		this.rTopRight = $("<div class='box-resizable box-top-right' />").css({ 'z-Index': 2000 + options.level + 1 })[0];
		this.rTopLeft = $("<div class='box-resizable box-top-left' />").css({ 'z-Index': 2000 + options.level + 1 })[0];
		this.rBottomRight = $("<div class='box-resizable box-bottom-right' />").css({ 'z-Index': 2000 + options.level + 1 })[0];
		this.rBottomLeft = $("<div class='box-resizable box-bottom-left' />").css({ 'z-Index': 2000 + options.level + 1 })[0];
		$(this.container).append(this.rTop).append(this.rRight).append(this.rBottom).append(this.rLeft);
		$(this.container).append(this.rTopLeft).append(this.rTopRight).append(this.rBottomLeft).append(this.rBottomRight);


		var widget = "<div class='widget-box'> "
        + "                        <div class='widget-header'>"
        + "                            <button type='button' class='close'>×</button>"
        + "                            <span class='title' >"
        + options.title
        + "                            </span>"
        + "                        </div>"
        + "                        <div class='widget-body'>"
        + "                        </div>"
        + "                        <div class='widget-footer' style='height:50px'>"
        + "                            <button type='button' class='btn btn-default' data-dismiss='modal'>取消</button>"
        + "                            <button type='button' class='btn btn-primary'>保存</button>"
        + "                        </div>"
        + "                 </div>";

		$(this.container).append(widget);
		this.widgetbox = $(this.container).find(".widget-box")[0];
		this.widgetbody = $(this.container).find(".widget-body")[0];
		this.widgetheader = $(this.container).find(".widget-header")[0];
		this.closeBtn = $(this.widgetheader).find("button")[0];
		this.widgetfooter = $(this.container).find(".widget-footer")[0];
		this.cancelBtn = $(this.widgetfooter).find("button")[0];
		this.okBtn = $(this.widgetfooter).find("button")[1];

		var box = this;

		$(this.closeBtn).bind("click", function () {
			if (typeof(options.onCancel) == "function") {
				options.onCancel.call();
			}
			box.parent.close();
			if(typeof(options.onCancelCallBack) == "function") {
				options.onCancelCallBack.call();
			}
		});

		options.cancelBtn = options.cancelBtn || {};

		if (typeof (options.cancelBtn.visible) == "undefined") {
			options.cancelBtn.visible = true;
		}

		if (options.cancelBtn.visible) {
			$(this.cancelBtn).show();
			$(this.cancelBtn).text(options.cancelBtn.text ? options.cancelBtn.text : "取消");

			$(this.cancelBtn).bind("click", function () {
				if (typeof(options.onCancel) == "function") {
					options.onCancel.call();
				}
				box.parent.close();
			});

		} else {
			$(this.cancelBtn).hide();
		}

		options.okBtn = options.okBtn || {};
		if (typeof (options.okBtn.visible) == "undefined") {
			options.okBtn.visible = true;
		}

		if (options.okBtn.visible) {
			$(this.okBtn).show();
			$(this.okBtn).text(options.okBtn.text ? options.okBtn.text : "确定");

			$(this.okBtn).bind("click", function () {
				var canceled = false;
				var args = {};
				
				if (typeof(options.onOk) == "function") {
					args.canceled = false;
					options.onOk.call(this,args);
					canceled = args.canceled;
				}
				if(!canceled){
				    box.parent.close();
					if(typeof(options.onOkCallBack) == "function") {
						options.onOkCallBack.call(box, box, args);
					}
				}
			});

		} else {
			$(this.okBtn).hide();
		}



		this.bind();
	},
	bind: function () {
		this.resizeelm = null;
		this.fun = null; //记录触发什么事件的索引 
		this.original = []; //记录开始状态的数组 
		this.width = null;
		this.height = null;
		this.fR = BindAsEventListener(this, this.resize);
		this.fS = Bind(this, this.stop);
		//this.bg.onclick =this.hide.apply(this);//点击背景关闭弹出层

		this.set(this.rTop, 'up');
		this.set(this.rTopLeft, 'leftUp');
		this.set(this.rTopRight, 'rightUp');

		this.set(this.rBottom, 'down');
		this.set(this.rBottomLeft, 'leftDown');
		this.set(this.rBottomRight, 'rightDown');

		this.set(this.rLeft, 'left');
		this.set(this.rRight, 'right');

		this.drag(this.container, this.widgetheader);
	},
	drag: function (elem, stylearea) {
		var startX, startY, mouse;
		mouse = {
			mouseup: function () {
				$(document).unbind("mousemove", mouse.mousemove);
				$(document).unbind("mouseup", mouse.mouseup);
			},
			mousemove: function (ev) {
				var oEvent = ev || event;
				elem.style.left = oEvent.clientX - startX + "px";
				elem.style.top = oEvent.clientY - startY + "px";
			}
		};
		stylearea.onmousedown = function (ev) {
			var oEvent = ev || event;
			startX = oEvent.clientX - elem.offsetLeft;
			startY = oEvent.clientY - elem.offsetTop;
			$(document).bind("mousemove", mouse.mousemove);
			$(document).bind("mouseup", mouse.mouseup);
		};
	},
	up: function (e) {
		//this.height > e.clientY ? $(this.container).css({ 'top': e.clientY + "px", 'height': this.height - e.clientY + "px" }) : this.turnDown(e);
		if (this.height > e.clientY) {
			$(this.container).css({ 'top': e.clientY + "px", 'height': this.height - e.clientY + "px" });
			$(this.widgetbox).css({ 'height': $(this.container).height() });
			$(this.widgetbody).css({ 'height': $(this.widgetbox).height() - $(this.widgetheader).outerHeight() - $(this.widgetfooter).outerHeight() + 'px' });
			if(this.iframe)
				$(this.iframe).attr({ 'height': ($(this.widgetbody).height()-5)+"px" });
		} else {
			this.turnDown(e);
		}

	},
	down: function (e) {
		//e.clientY > this.original[3] ? $(this.container).css({ 'top': this.original[3] + 'px', 'height': e.clientY - this.original[3] + 'px' }) : this.turnUp(e);
		if (e.clientY > this.original[3]) {
			$(this.container).css({ 'top': this.original[3] + 'px', 'height': e.clientY - this.original[3] + 'px' });
			$(this.widgetbox).css({ 'height': $(this.container).innerHeight() });
			$(this.widgetbody).css({ 'height': $(this.widgetbox).height() - $(this.widgetheader).outerHeight() - $(this.widgetfooter).outerHeight() + 'px' });
			if(this.iframe)
				$(this.iframe).attr({ 'height': ($(this.widgetbody).height()-5)+"px" });
		} else {
			this.turnUp(e);
		}
	},
	left: function (e) {
		e.clientX < this.width ? $(this.container).css({ 'left': e.clientX + 'px', 'width': this.width - e.clientX + "px" }) : this.turnRight(e);
	},
	right: function (e) {
		e.clientX > this.original[2] ? $(this.container).css({ 'left': this.original[2] + 'px', 'width': e.clientX - this.original[2] + "px" }) : this.turnLeft(e);
	},
	leftUp: function (e) {
		this.up(e); this.left(e);
	},
	leftDown: function (e) {
		this.left(e); this.down(e);
	},
	rightUp: function (e) {
		this.up(e); this.right(e);
	},
	rightDown: function (e) {
		this.right(e); this.down(e);
	},
	turnDown: function (e) {
		$(this.container).css({ 'top': this.height + 'px', 'height': e.clientY - this.height + 'px' });
	},
	turnUp: function (e) {
		$(this.container).css({ 'top': e.clientY + 'px', 'height': this.original[3] - e.clientY + 'px' });
	},
	turnRight: function (e) {
		$(this.container).css({ 'left': this.width + 'px', 'width': e.clientX - this.width + 'px' });
	},
	turnLeft: function (e) {
		$(this.container).css({ 'left': e.clientX + 'px', 'width': this.original[2] - e.clientX + 'px' });
	},
	start: function (e, fun) {
		this.fun = fun;
		this.original = [parseInt($(this.container).css('width')),
            parseInt($(this.container).css('height')),
            parseInt($(this.container).css('left')),
            parseInt($(this.container).css('top'))];
		this.width = (this.original[2] || 0) + this.original[0];
		this.height = (this.original[3] || 0) + this.original[1];

		$(document).bind("mousemove", this.fR);
		$(document).bind("mouseup", this.fS);
	},
	set: function (elm, direction) {
		if (!elm) return;
		this.resizeelm = elm;
		$(this.resizeelm).bind('mousedown', BindAsEventListener(this, this.start, this[direction]));
		return this;
	},
	stop: function () {
		$(document).unbind("mousemove", this.fR);
		$(document).unbind("mouseup", this.fS);
		window.getSelection ? window.getSelection().removeAllRanges() : document.selection.empty();
	},
	resize: function (e) {
		this.fun(e);
		this.resizeelm.onblur = function () { if (this.fS) this.fS(); };
	}

};

ModalBox.prototype = {
	constructor: ModalBox,
	show: function (options, content, timesec) {
		options.level = this.boxes.length;
		var box = new Box(options);
		box.parent = this;
		this.boxes.push(box);

		document.body.appendChild(box.bg);
		document.body.appendChild(box.container);

		$(window).resize(this.winResize);
		$(window).scroll(this.winScrolls);

		if (options.control && options.control.id) {
			if (options.control.clone) {
				box.tmpcontrol = $("#" + options.control.id).clone().attr('id', options.control.id + '1');
				box.cloned = true;
			}
			else {
				box.tmpcontrol = $("#" + options.control.id);
			}
			box.tmpcontrol.css({
				'display': 'block'
			}).appendTo(box.widgetbody);
		}
		else {
			if (options.iframe) {
				//$(box.widgetbody).css("overflow","hidden");
				$(box.widgetbody).css("-webkit-overflow-scrolling","touch");
				$(box.widgetbody).css(" -webkit-overflow","auto");
				box.iframe = $('<iframe src="' + options.iframe.src + '" width="100%" height="100%" frameborder="0"></iframe>')[0];
				$(box.iframe).appendTo(box.widgetbody);
			} else {
				var innerHtml = content;
				if (innerHtml)
					$(innerHtml).appendTo(box.widgetbody);
			}
		}
		box.oHeight = $(box.container).height();
		box.oWidth = $(box.container).width();

		this.init(box);
		this.alpha(box.bg, 50, 1, box);
		//this.drag(box.container, $(box.container).find(".widget-header")[0]);

		if (timesec) {
			box.time = setTimeout(function () { this.hide(); }, timesec * 1000);
		}
		
		return box;
	},
	close: function () {
		if (this.boxes.length > 0) {
			var box = this.boxes.pop();
			if(box.tmpcontrol&&!box.cloned) {
				$(box.tmpcontrol).hide().appendTo(window.document.body);
			}
			this.alpha(box.container, 0, -1, box);
			clearTimeout(this.time);
			$(box.container).remove();
			$(box.bg).remove();
		}
	},
	hide: function () {
		if (this.boxes.length > 0) {
			var box = this.boxes.pop();
			this.alpha(box.container, 0, -1, box);
			clearTimeout(this.time);
		}
	},
	init: function (box) {
		this.setModal(box, this.page);
	},
	alpha: function (elem, opacity, display, box) {
		clearInterval(elem.ai);
		if (display == 1) {
			elem.style.opacity = 0;
			elem.style.filter = 'alpha(opacity=0)';
			elem.style.display = 'block';
		}

		var that = this;
		elem.ai = setInterval(function () {
			that.filter(elem, opacity, display, box);
		}, 30);
	},
	filter: function (elem, opacity, display, box) {
		var anum = Math.round(elem.style.opacity * 100);
		if (anum == opacity) {
			clearInterval(elem.ai);
			if (display == -1) {
				elem.style.display = 'none';
				if (elem == box.container) {
					this.alpha(box.bg, 0, -1, box);
				}
			} else {
				if (elem == box.bg) {
					this.alpha(box.container, 100, 1, box);
				}
			}
		} else {
			var n = Math.ceil((anum + ((opacity - anum) * .5)));
			n = n == 1 ? 0 : n;
			elem.style.opacity = n / 100;
			elem.style.filter = 'alpha(opacity=' + n + ')';
		}
	},

	set: function (elm, direction) {
		if (!elm) return;
		var box = this.boxes[this.boxes.length - 1];
		box.resizeelm = elm;
		$(box.resizeelm).bind('mousedown', BindAsEventListener(box, box.start, box[direction]));
		return this;
	},
	setModal: function (box, page) {
		if (box) {
			$(box.bg).css({
				'height': page.total(1) + 'px',
				'width': page.total(0) + 'px'
			});

			$(box.container).css({
				'top': ((page.height() - box.oHeight) / 2 + page.top()) + 'px',
				'left': (page.width() - box.oWidth) / 2 + 'px'
			});

			$(box.widgetbox).css({ 'height': $(box.container).height() });
			$(box.widgetbody).css({ 'height': $(box.widgetbox).height() - $(box.widgetheader).outerHeight() - $(box.widgetfooter).outerHeight() + 'px' });
			if(box.iframe)
				$(box.iframe).attr({ 'height': ($(box.widgetbody).height()-5)+"px" });
		}
	},
	winResize: function () {
		var boxes = $HGModalBox.boxes;
		var box = boxes[boxes.length - 1];
		var page = $HGModalBox.page;

		if (box) {
			$(box.bg).css({
				'height': page.total(1) + 'px',
				'width': page.total(0) + 'px'
			});

			$(box.container).css({
				'top': ((page.height() - box.oHeight) / 2 + page.top()) + 'px',
				'left': (page.width() - box.oWidth) / 2 + 'px'
			});
		}
	},
	winScrolls: function () {
		var boxes = $HGModalBox.boxes;
		var box = boxes[boxes.length - 1];
		var page = $HGModalBox.page;

		if (box) {
			$(box.bg).css({
				'height': page.total(1) + 'px',
				'width': page.total(0) + 'px'
			});

			$(box.container).css({
				'top': ((page.height() - box.oHeight) / 2 + page.top()) + 'px',
				'left': (page.width() - box.oWidth) / 2 + 'px'
			});
		}
	}

};

//定义全局变量
$HGModalBox = $HGRootNS.ModalBox = new ModalBox();

function inheritBox(superBox, subBox) {
	var prototype = new Object(superBox.prototype);
	prototype.constructor = subBox;
	subBox.prototype = prototype;
}

function BtnBox(options) {
	Box.apply(this, options);

	this.btns = null;
}

$HGRootNS._ClientMsg = function () {
	this._images = {
		"inform":"<%=WebResource("MCS.Web.Responsive.Library.Resources.ClientMsg.inform.gif")%>",
		"alert":"<%=WebResource("MCS.Web.Responsive.Library.Resources.ClientMsg.alert.gif")%>",
		"stop":"<%=WebResource("MCS.Web.Responsive.Library.Resources.ClientMsg.stop.gif")%>",
		"confirm":"<%=WebResource("MCS.Web.Responsive.Library.Resources.ClientMsg.confirm.gif")%>"
	};

	this._detailID = "HGClientMsgDetailDiv";
	this._msgID = "HGClientMsgTitle";
};

$HGRootNS._ClientMsg.prototype =
{
    info:function (msg, detailMsg, title) {
		if (!title)
			title = "提示";

		this._popMsgWindow("inform", msg, detailMsg, title);
	},

	inform: function (msg, detailMsg, title) {
		if (!title)
			title = "提示";

		this._popMsgWindow("inform", msg, detailMsg, title);
	},

	alert: function (msg, detailMsg, title) {
		if (!title)
			title = "警告";

		this._popMsgWindow("alert", msg, detailMsg, title);
	},

	stop: function (msg, detailMsg, title) {
		if (!title)
			title = "错误";

		this._popMsgWindow("stop", msg, detailMsg, title);
	},

	confirm: function (msg, detailMsg, title, okBtnText, cancelBtnText, okBtnMethod, cancelBtnMethod) {
		if (!title)
			title = "选择";

		return this._popMsgWindow("confirm", msg, detailMsg, title, okBtnText, cancelBtnText, okBtnMethod, cancelBtnMethod);
	},

	_getImgUrl: function (msgType) {
		for (var i = 0; i < this._images.length; i++) {
			if (msgType == this._images[i].msgType)
				return this._images[i].url;
		}

		return "";
	},

	_setClickElement: function (elem) {
		elem.onclick = Function.createDelegate(this, this._onCopyMsgClick);
		elem.onmouseover = this._onClickElemMouseOver;
		elem.onmouseout = this._onClickElemMouseOut;
		elem.onmousedown = this._onClickElemMouseDown;
		elem.onmouseup = this._onClickElemMouseUp;
	},

	_onClickElemMouseOver: function (event) {
		var elem = event.srcElement || event.target;
		elem.style.border = "#316AC5 1px solid";
		elem.style.backgroundColor = "#C1D2EE";
		elem.style.position = "relative";
		elem.style.top = -1;
	},

	_onClickElemMouseOut: function (event) {
		var elem = event.srcElement || event.target;
		elem.style.backgroundColor = "transparent";
		elem.style.border = "none";
		elem.style.top = 0;
	},

	_onClickElemMouseDown: function (event) {
		var elem = event.srcElement || event.target;
		elem.style.top = 1;
	},

	_onClickElemMouseUp: function (event) {
		var elem = event.srcElement || event.target;
		elem.style.top = -1;
	},

	_copyMessages: function () {
		var txt = $("#" + this._detailID).text();
		this._copyToClipboard(txt);
	},

	_copyToClipboard: function (txt) {
		if (window.clipboardData) {
			window.clipboardData.clearData();
			window.clipboardData.setData("Text", txt);
		} else if (navigator.userAgent.indexOf("Opera") != -1) {
			window.location = txt;
		} else if (window.netscape) {
			try {
				netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect");
			} catch (e) {
				alert("被浏览器拒绝！\n请在浏览器地址栏输入'about:config'并回车\n然后将'signed.applets.codebase_principal_support'设置为'true'");
			}
			var clip = Components.classes['@mozilla.org/widget/clipboard;1'].createInstance(Components.interfaces.nsIClipboard);
			if (!clip) return;
			var trans = Components.classes['@mozilla.org/widget/transferable;1'].createInstance(Components.interfaces.nsITransferable);
			if (!trans) return;
			trans.addDataFlavor('text/unicode');
			var str = Components.classes["@mozilla.org/supports-string;1"].createInstance(Components.interfaces.nsISupportsString);
			var copytext = txt;
			str.data = copytext;
			trans.setTransferData("text/unicode", str, copytext.length * 2);
			var clipid = Components.interfaces.nsIClipboard;
			if (!clip) return false;
			clip.setData(trans, null, clipid.kGlobalClipboard);
		}
	},

	_onCopyMsgClick: function () {
		this._copyMessages();
	},

	_onMsgLinkClick: function () {
		$("#" + this._detailID).toggle();
	},

	_getDefaultWidth: function () {
		var min = 280;
		var max = $(document.body).width()-20;
		var cur = 100 + $("#" + this._msgID).width();
		return cur < min ? min : (cur > max ? max : cur);
	},

	_popMsgWindow: function (msgType, msg, detailMsg, title, okBtnText, cancelBtnText, okBtnMethod, cancelBtnMethod) {
		var containerID = "HGControlMsgContainer";
		var msgContainer;
		var msgContainerObj = $("#" + containerID);

		if (msgContainerObj.length == 0) {
			msgContainer = $("<div />").css({
				'text-Align': 'center',
				'width': '100%',
				'padding-top': '5px'
			}).attr('id', containerID)[0];
		} else {
			msgContainer = msgContainerObj[0];
		}

		msgContainer.innerHTML = "";
		$(msgContainer).show();

		var imgID = "HGClientMsgLogoImg";
		var linkID = "HGClientMsgLink";

		var tableStr = " <table style='width: 100%;text-align:center'>"
	         + "   <tr>"
	         + "       <td style='width: 60px;vertical-align:top'>"
	         + "           	<img id='" + imgID + "' src='" + this._images[msgType] + "' alt='复制信息'>"
	         + "       </td>"
             + "       <td style='text-align:left'>"
             + "           <div style='white-space: nowrap;'>"
             + "               <a id='" + this._msgID + "'>" + msg + "</a>"
             + "           </div>"
             + "           <div>"
             + "              <a id='" + linkID + "' href='javascript:void(0);' >点击此处展开详细信息...</a>"
             + "           </div>"
             + "           <div id='" + this._detailID + "' style='width:100%; display:none'>" + detailMsg + "</div>"
             + "       </td>"
	         + "   </tr>"
	         + "</table>";

		$(msgContainer).append(tableStr);
		$(msgContainer).appendTo(window.document.body);

		this._setClickElement($get("HGClientMsgLogoImg"));

		var titleElement = $get(this._msgID);
		if (typeof( mseeageNotifyMailAddress) === 'string') {
			titleElement.href = "mailto://" + mseeageNotifyMailAddress + "&subject=" + encodeURIComponent(msg);
		}
		titleElement.onclick = Function.createDelegate(this, this._onCopyMsgClick);

		$get(linkID).onclick = Function.createDelegate(this, this._onMsgLinkClick);

		var bResult;

		try {
			var options = {
				title: title,
				width: this._getDefaultWidth() + "px",
				height: "160px",
				onOk: okBtnMethod,
				onCancel: cancelBtnMethod,
				okBtn: {
					visible: true,
					text: "确定"
				},
				cancelBtn: {
					visible: msgType === "confirm",
					text: "取消"
				},
				control: {
					id: containerID,
					clone: false
				}
			};

			$HGModalBox.show(options);
		}
		catch (e) {
			if (msgType == "confirm")
				bResult = window.confirm(msg);
			else {
				bResult = true;
				alert(msg);
			}
		}

		return bResult;
	}
};

$HGClientMsg = $HGRootNS.ClientMsg = new $HGRootNS._ClientMsg();

$showError = function (err) {
	var description = "";
	var message = "";

	if (typeof (err.message) != "undefined")
		message = err.message;
	else
		if (typeof (err.get_message) != "undefined")
			message = err.get_message();
		else
			message = err;

	if (typeof (err.description) != "undefined")
		description = err.description;
	else
		if (typeof (err.get_stackTrace) != "undefined")
			description = err.get_stackTrace();

	$HGClientMsg.stop(message, description, "错误");
}




//----------------------------------------------------------------------------------------------------------------------------------------

$HGRootNS.BalloonMessage = function (forElem, messageType, title, message ,width,height, dir) {
    var boxCss,iconClass,dirCss;
    switch (message) {
        case "success":
            boxCss = "alert alert-success ";
            iconClass = "icon glyphicon glyphicon-ok-sign";
            break;
        case "error":
            boxCss = "alert alert-error ";
            iconClass = "icon glyphicon glyphicon-remove-sign";
            break;
        case "warning":
            boxCss = "alert alert-warning ";
            iconClass = "icon glyphicon glyphicon-exclamation-sign";
            break;
        case "info":
        default:
            boxCss = "alert alert-info ";
            iconClass = "icon glyphicon glyphicon-info-sign";
            break;    
    }

    dirCss = "dir-" + (dir || "default");
    
    var cc = document.createElement("div");
    cc.className = "balloon-message-container";
    forElem.parentNode.appendChild(cc);
    var c = document.createElement("div"), elem;
    c.className = "balloon-message " + boxCss + dirCss ;
    (cc).appendChild(c);
    var titleBar = document.createElement("div");
    titleBar.className = "title-bar";
    c.appendChild(titleBar);

    var icon = document.createElement("div");
    icon.className = iconClass;
    titleBar.appendChild(icon);

    var elemTitle = document.createElement("div");
    elemTitle.className = "title";
    titleBar.appendChild(elemTitle);

    var msgBar = document.createElement("div");
    msgBar.className = "content";
    c.appendChild(msgBar);

    elemTitle.appendChild(document.createTextNode(title));
    msgBar.appendChild(document.createTextNode(message));

    // 开始计算位置
    var pp = cc.offsetParent;
    var pWidth = pp.clientWidth, pHeight = pp.clientHeight;
    var cWidth = c.offsetWidth, cHeight = c.offsetHeight;
    
     switch (dir) {
        case "up":
            cc.style.top = pHeight/2 -cHeight - 10 + "px";
            cc.style.left = pWidth/2 - cWidth /2 + "px";
            break;
        case "down":
            cc.style.top = pHeight/2 + 10 + "px";
            cc.style.left = pWidth/2 - cWidth /2 + "px";
            break;
        case "left":
            cc.style.top = pHeight/2 - cHeight/2 + "px";
            cc.style.left = pWidth /2 - cWidth -10  + "px";
            break;
        case "right":
            cc.style.top = pHeight/2 -cHeight/2 + "px";
            cc.style.left = pWidth /2 + 10  + "px";
            break;
        default:
    }

    $HGRootNS.BalloonMessage._configEvent(cc,forElem);

    msgBar = null;
    elemTitle = null;
    titleBar = null;
    icon = null;

    return c;
}

$HGRootNS.BalloonMessage._configEvent = function(balloon,triggerElem){
    var handler = function ClickEventHandler(e) {
        if(balloon !=null){
            balloon.parentNode.removeChild(balloon);
            balloon = null;

            e.preventDefault();
            return false;
        }
    }

    $(balloon).click(handler);
    $(triggerElem).click(handler);
}
