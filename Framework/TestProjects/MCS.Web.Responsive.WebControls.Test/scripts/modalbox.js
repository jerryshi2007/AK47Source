var Bind = function (object, fun) {
	var args = Array.prototype.slice.call(arguments).slice(2);
	return function () {
		return fun.apply(object, args);
	}
};

var BindAsEventListener = function (object, fun) {
	var args = Array.prototype.slice.call(arguments).slice(2);
	return function (event) {
		return fun.apply(object, [event || window.event].concat(args));
	}
};


function ModalBox() {
	this.boxes = new Array();
	this.page = function () {
		return {
			top: function () { return document.documentElement.scrollTop || document.body.scrollTop },
			width: function () { return self.innerWidth || document.documentElement.clientWidth || document.body.clientWidth },
			height: function () { return self.innerHeight || document.documentElement.clientHeight || document.body.clientHeight },
			total: function (d) {
				var b = document.body, elem = document.documentElement;
				return d ? Math.max(Math.max(b.scrollHeight, elem.scrollHeight), Math.max(b.clientHeight, elem.clientHeight)) :
                Math.max(Math.max(b.scrollWidth, elem.scrollWidth), Math.max(b.clientWidth, elem.clientWidth))
			}
		}
	} ()
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
		this.bg = $("<div class='box-bg' />").css({ 'z-Index': 1000 + options.level * 10, 'display': "none" }).attr('id', 'boxbg-' + options.level)[0];
		this.container = $("<div class='box-container' />").css({
			'z-Index': 1000 + options.level * 10 + 1,
			'position': "absolute", 'display': "none",
			'width': options.width ? options.width : 'auto',
			'height': options.height ? options.height : 'auto'
		}).attr('id', 'boxcontainer-' + options.level)[0];

		this.rTop = $("<div class='box-resizable box-top' />")[0];
		this.rRight = $("<div class='box-resizable box-right' />")[0];
		this.rBottom = $("<div class='box-resizable box-bottom' />")[0];
		this.rLeft = $("<div class='box-resizable box-left' />")[0];
		this.rTopRight = $("<div class='box-resizable box-top-right' />").css({ 'z-Index': 1000 + options.level + 1 })[0];
		this.rTopLeft = $("<div class='box-resizable box-top-left' />").css({ 'z-Index': 1000 + options.level + 1 })[0];
		this.rBottomRight = $("<div class='box-resizable box-bottom-right' />").css({ 'z-Index': 1000 + options.level + 1 })[0];
		this.rBottomLeft = $("<div class='box-resizable box-bottom-left' />").css({ 'z-Index': 1000 + options.level + 1 })[0];
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
			if (Object.prototype.toString.call(options.onCancel) == "[object Function]") {
				options.onCancel.call();
			}
			box.parent.close();
		});

		if (options.cancelBtn.visible) {
			$(this.cancelBtn).show();
			$(this.cancelBtn).text(options.cancelBtn.text ? options.cancelBtn.text : "取消");

			$(this.cancelBtn).bind("click", function () {
				if (Object.prototype.toString.call(options.onCancel) == "[object Function]") {
					options.onCancel.call();
				}
				box.parent.close();
			});

		} else {
			$(this.cancelBtn).hide();
		}
		if (options.okBtn.visible) {
			$(this.okBtn).show();
			$(this.okBtn).text(options.okBtn.text ? options.okBtn.text : "确定");

			$(this.okBtn).bind("click", function () {
				if (Object.prototype.toString.call(options.onOk) == "[object Function]") {
					options.onOk.call();
				}
				box.parent.close();
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
		}
		stylearea.onmousedown = function (ev) {
			var oEvent = ev || event;
			startX = oEvent.clientX - elem.offsetLeft;
			startY = oEvent.clientY - elem.offsetTop;
			$(document).bind("mousemove", mouse.mousemove);
			$(document).bind("mouseup", mouse.mouseup);
		}
	},
	up: function (e) {
		//this.height > e.clientY ? $(this.container).css({ 'top': e.clientY + "px", 'height': this.height - e.clientY + "px" }) : this.turnDown(e);
		if (this.height > e.clientY) {
			$(this.container).css({ 'top': e.clientY + "px", 'height': this.height - e.clientY + "px" });
			$(this.widgetbox).css({ 'height': $(this.container).height() });
			$(this.widgetbody).css({ 'height': $(this.widgetbox).height() - $(this.widgetheader).outerHeight() - $(this.widgetfooter).outerHeight() + 'px' });
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
		this.resizeelm.onblur = function () { if (this.fS) this.fS() };
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
				var tmpcontrol = $("#" + options.control.id).clone().attr('id', options.control.id + '1');
			}
			else {
				var tmpcontrol = $("#" + options.control.id);
			}
			tmpcontrol.css({
				'display': 'block'
			}).appendTo(box.widgetbody);
		}
		else {
			if (options.iframe) {
				var innerHtml = '<iframe src="' + options.iframe.src + '" width="' + options.iframe.width + '" height="' + options.iframe.height + '" frameborder="0"></iframe>';
			} else {
				var innerHtml = content;
			}
			if (innerHtml)
				$(innerHtml).appendTo(box.widgetbody);
		}
		box.oHeight = $(box.container).height();
		box.oWidth = $(box.container).width();

		this.init(box);
		this.alpha(box.bg, 50, 1, box);
		//this.drag(box.container, $(box.container).find(".widget-header")[0]);

		if (timesec) {
			box.time = setTimeout(function () { this.hide() }, timesec * 1000);
		}
	},
	close: function () {
		if (this.boxes.length > 0) {
			var box = this.boxes.pop();
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
		}
	},
	winResize: function () {
		var boxes = window.modalbox.boxes;
		var box = boxes[boxes.length - 1];
		var page = window.modalbox.page;

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
		var boxes = window.modalbox.boxes;
		var box = boxes[boxes.length - 1];
		var page = window.modalbox.page;

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

}



//定义全局变量
window.modalbox = new ModalBox();


function inheritBox(superBox, subBox) {
	var prototype = new Object(superBox.prototype);
	prototype.constructor = subBox;
	subBox.prototype = prototype;
}

function BtnBox(options) {
	Box.apply(this, options);

	this.btns = null;
}