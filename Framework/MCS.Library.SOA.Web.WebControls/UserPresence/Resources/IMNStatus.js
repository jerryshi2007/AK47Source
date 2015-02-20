function Browseris() {
	var agt = navigator.userAgent.toLowerCase();
	this.osver = 1.0;

	if (agt) {
		var stOSVer = agt.substring(agt.indexOf("windows ") + 11);
		this.osver = parseFloat(stOSVer);
	}

	this.major = parseInt(navigator.appVersion);
	this.nav = ((agt.indexOf('mozilla') != -1) && ((agt.indexOf('spoofer') == -1) && (agt.indexOf('compatible') == -1)));
	this.nav6 = this.nav && (this.major == 5);
	this.nav6up = this.nav && (this.major >= 5);
	this.nav7up = false;

	if (this.nav6up) {
		var navIdx = agt.indexOf("netscape/");
		if (navIdx >= 0)
			this.nav7up = parseInt(agt.substring(navIdx + 9)) >= 7;
	}
	this.ie = (agt.indexOf("msie") != -1);
	this.aol = this.ie && agt.indexOf(" aol ") != -1;
	if (this.ie) {
		var stIEVer = agt.substring(agt.indexOf("msie ") + 5);
		this.iever = parseInt(stIEVer);
		this.verIEFull = parseFloat(stIEVer);
	}
	else
		this.iever = 0;
	this.ie4up = this.ie && (this.major >= 4);
	this.ie5up = this.ie && (this.iever >= 5);
	this.ie55up = this.ie && (this.verIEFull >= 5.5);
	this.ie6up = this.ie && (this.iever >= 6);
	this.winnt = ((agt.indexOf("winnt") != -1) || (agt.indexOf("windows nt") != -1));
	this.win32 = ((this.major >= 4) && (navigator.platform == "Win32")) ||
		(agt.indexOf("win32") != -1) || (agt.indexOf("32bit") != -1);
	this.mac = (agt.indexOf("mac") != -1);
	this.w3c = this.nav6up;
	this.safari = (agt.indexOf("safari") != -1);
	this.safari125up = false;

	if (this.safari && this.major >= 5) {
		var navIdx = agt.indexOf("safari/");
		if (navIdx >= 0)
			this.safari125up = parseInt(agt.substring(navIdx + 7)) >= 125;
	}
}

var browseris = new Browseris();
var bIMNControlInited = false;
var IMNDictionaryObj = null;
var bIMNOnloadAttached = false;
var IMNOrigScrollFunc = null;
var bIMNInScrollFunc = false;
var IMNSortableObj = null;
var IMNHeaderObj = null;
var IMNNameDictionaryObj = null;
var IMNShowOfflineObj = null;

//var IMNOOUIObj = null;
var IMNSerializeID = 0;

function initNotifyInput() {
	var notifier = document.getElementById("userPresenceNotifier");

	if (notifier == null) {
		notifier = document.createElement("input");
		notifier.type = 'hidden';
		notifier.id = "userPresenceNotifier";
		notifier.onpropertychange = onUserPresenceChanged;
		document.body.appendChild(notifier);
	}
}

function onUserPresenceChanged() {
	if (event.propertyName == "value") {
		if (event.srcElement.value != "") {
			var array = event.srcElement.value.split(";");

			IMNOnStatusChange(array[0], parseInt(array[1]), array[2]);
		}
	}
}

function EnsureIMNControl() {
	initNotifyInput();

	return true;
}

function IMNImageInfo() {
	this.img = null;
	this.alt = "";
}

var L_IMNOnline_Text = "空闲";
var L_IMNOffline_Text = "脱机";
var L_IMNAway_Text = "离开";
var L_IMNBusy_Text = "忙碌";
var L_IMNDoNotDisturb_Text = "请勿打扰";
var L_IMNIdle_Text = "可能已离开";
var L_IMNBlocked_Text = "阻止";
var L_IMNOnline_OOF_Text = "空闲(OOF)";
var L_IMNOffline_OOF_Text = "脱机(OOF)";
var L_IMNAway_OOF_Text = "离开(OOF)";
var L_IMNBusy_OOF_Text = "忙碌(OOF)";
var L_IMNDoNotDisturb_OOF_Text = "请勿打扰(OOF)";
var L_IMNIdle_OOF_Text = "可能已离开(OOF)";

function ConvertStatusImagePath(img) {
	var result = img;

	if (typeof (StatusImageDict) != "undefined")
		result = StatusImageDict[img];

	return result;
}


function MNChangeImageState(id, state, showoffline) {
	var img = document.getElementById(id);
	if (img == null) {
		return;
	}
	switch (state) {
		case 0:
			img.className = "uc-on";
			img.alt = L_IMNOnline_Text;
			break;
		case 11:
			img.className = "uc-onoff";
			img.alt = L_IMNOnline_OOF_Text;
			break;
		case 1:
			if (showoffline) {
				img.className = "uc-off";
				img.alt = L_IMNOffline_Text;
			}
			else {
				img.className = "uc-hdr";
				img.alt = "";
			}
			break;
		case 12:
			if (showoffline) {
				img.className = "uc-offoff";
				img.alt = L_IMNOffline_OOF_Text;
			}
			else {
				img.className = "uc-hdr";
				img.alt = "";
			}
			break;
		case 2:
			img.className = "uc-away";
			img.alt = L_IMNAway_Text;
			break;
		case 13:
			img.className = "uc-awayoof";
			img.alt = L_IMNAway_OOF_Text;
			break;
		case 3:
			img.className = "uc-busy";
			img.alt = L_IMNBusy_Text;
			break;
		case 14:
			img.className = "uc-busyoff";
			img.alt = L_IMNBusy_OOF_Text;
			break;
		case 4:
			img.className = "uc-away";
			img.alt = L_IMNAway_Text;
			break;
		case 5:
			img.className = "uc-busy";
			img.alt = L_IMNBusy_Text;
			break;
		case 6:
			img.className = "uc-away";
			img.alt = L_IMNAway_Text;
			break;
		case 7:
			img.className = "uc-busy";
			img.alt = L_IMNBusy_Text;
			break;
		case 8:
			img.className = "uc-away";
			img.alt = L_IMNAway_Text;
			break;
		case 9:
			img.className = "uc-dnd";
			img.alt = L_IMNDoNotDisturb_Text;
			break;
		case 15:
			img.className = "uc-dndoof";
			img.alt = L_IMNDoNotDisturb_OOF_Text;
			break;
		case 10:
			img.className = "uc-busy";
			img.alt = L_IMNBusy_Text;
			break;
		case 16:
			img.className = "uc-idle";
			img.alt = L_IMNIdle_Text;
			break;
		case 17:
			img.className = "uc-idleoof";
			img.alt = L_IMNIdle_OOF_Text;
			break;
		case 18:
			img.className = "uc-blocked";
			img.alt = L_IMNBlocked_Text;
			break;
		case 19:
			img.className = "uc-idlebusy";
			img.alt = L_IMNBusy_Text;
			break;
		case 20:
			img.className = "uc-idlebusyoof";
			img.alt = L_IMNBusy_OOF_Text;
			break;
	}
}

function IMNGetStatusImage(state, showoffline) {
	var img = "imnhdr.gif";
	var alt = "";

	switch (state) {
		case 0:
			img = ConvertStatusImagePath("imnon.png");
			alt = L_IMNOnline_Text;
			break;
		case 11:
			img = ConvertStatusImagePath("imnonoof.png");
			alt = L_IMNOnline_OOF_Text;
			break;
		case 1:
			if (showoffline) {
				img = ConvertStatusImagePath("imnoff.png");
				alt = L_IMNOffline_Text;
			}
			else {
				img = ConvertStatusImagePath("imnhdr.gif");
				alt = "";
			}
			break;
		case 12:
			if (showoffline) {
				img = ConvertStatusImagePath("imnoffoof.png");
				alt = L_IMNOffline_OOF_Text;
			}
			else {
				img = ConvertStatusImagePath("imnhdr.gif");
				alt = "";
			}
			break;
		case 2:
			img = ConvertStatusImagePath("imnaway.png");
			alt = L_IMNAway_Text;
			break;
		case 13:
			img = ConvertStatusImagePath("imnawayoof.png");
			alt = L_IMNAway_OOF_Text;
			break;
		case 3:
			img = ConvertStatusImagePath("imnbusy.png");
			alt = L_IMNBusy_Text;
			break;
		case 14:
			img = ConvertStatusImagePath("imnbusyoof.png");
			alt = L_IMNBusy_OOF_Text;
			break;
		case 4:
			img = ConvertStatusImagePath("imnaway.png");
			alt = L_IMNAway_Text;
			break;
		case 5:
			img = ConvertStatusImagePath("imnbusy.png");
			alt = L_IMNBusy_Text;
			break;
		case 6:
			img = ConvertStatusImagePath("imnaway.png");
			alt = L_IMNAway_Text;
			break;
		case 7:
			img = ConvertStatusImagePath("imnbusy.png");
			alt = L_IMNBusy_Text;
			break;
		case 8:
			img = ConvertStatusImagePath("imnaway.png");
			alt = L_IMNAway_Text;
			break;
		case 9:
			img = ConvertStatusImagePath("imndnd.png");
			alt = L_IMNDoNotDisturb_Text;
			break;
		case 15:
			img = ConvertStatusImagePath("imndndoof.png");
			alt = L_IMNDoNotDisturb_OOF_Text;
			break;
		case 10:
			img = ConvertStatusImagePath("imnbusy.png");
			alt = L_IMNBusy_Text;
			break;
		case 16:
			img = ConvertStatusImagePath("imnidle.png");
			alt = L_IMNIdle_Text;
			break;
		case 17:
			img = ConvertStatusImagePath("imnidleoof.png");
			alt = L_IMNIdle_OOF_Text;
			break;
		case 18:
			img = ConvertStatusImagePath("imnblocked.png");
			alt = L_IMNBlocked_Text;
			break;
		case 19:
			img = ConvertStatusImagePath("imnidlebusy.png");
			alt = L_IMNBusy_Text;
			break;
		case 20:
			img = ConvertStatusImagePath("imnidlebusyoof.png");
			alt = L_IMNBusy_OOF_Text;
			break;
	}

	var imnInfo = new IMNImageInfo();

	imnInfo.img = img;
	imnInfo.alt = alt;

	return imnInfo;
}

function IMNIsOnlineState(state) {
	if (state == 1) {
		return false;
	}

	return true;
}

function IMNOnStatusChange(name, state, id) {
	MNChangeImageState(id, state, IMNSortableObj[id] || IMNShowOfflineObj[id]);
}


function IMNUpdateImage(id, imgInfo) {
	var obj = document.images(id);

	if (obj) {
		var img = imgInfo.img;
		var alt = imgInfo.alt;

		var newImg = img;

		if (obj.altbase) {
			obj.alt = obj.altbase;
		}
		else {
			obj.alt = alt;
		}
		var useFilter = browseris.ie &&
					browseris.ie55up &&
					browseris.verIEFull < 7.0;
		var isPng = (newImg.toLowerCase().indexOf(".png") > 0);
		if (useFilter) {
			if (isPng) {
				obj.style.filter = "progid:DXImageTransform.Microsoft.AlphaImageLoader(src=" + newImg + "),sizingMethod=scale,enabled=true);";
				obj.src = ConvertStatusImagePath("imnhdr.gif");
			}
			else {
				obj.style.filter = "";
				obj.src = newImg;
			}
		}
		else {
			obj.src = newImg;
		}
	}
}

function IMNGetOOUILocation(obj) {
	var objRet = new Object;
	var objSpan = obj;
	var objOOUI = obj;
	var oouiX = 0, oouiY = 0, objDX = 0;
	var fRtl = document.dir == "rtl";
	while (objSpan && objSpan.tagName != "DIV" && objSpan.tagName != "SPAN" && objSpan.tagName != "TABLE") {
		objSpan = objSpan.parentNode;
	}

	if (objSpan) {
		var collNodes = objSpan.tagName == "TABLE" ?
			objSpan.rows(0).cells(0).childNodes :
			objSpan.childNodes;

		for (var i = 0; i < collNodes.length; ++i) {
			if (collNodes.item(i).tagName == "IMG" && collNodes.item(i).id) {
				objOOUI = collNodes.item(i);
				break;
			}
			if (collNodes.item(i).tagName == "A" &&
				collNodes.item(i).childNodes.length > 0 &&
				collNodes.item(i).childNodes.item(0).tagName == "IMG" &&
				collNodes.item(i).childNodes.item(0).id) {
				objOOUI = collNodes.item(i).childNodes.item(0);
				break;
			}
		}
	}

	obj = objSpan;

	while (obj) {
		if (fRtl) {
			if (obj.scrollWidth >= obj.clientWidth + obj.scrollLeft)
				objDX = obj.scrollWidth - obj.clientWidth - obj.scrollLeft;
			else
				objDX = obj.clientWidth + obj.scrollLeft - obj.scrollWidth;
			oouiX += obj.offsetLeft + objDX;
		}
		else
			oouiX += obj.offsetLeft - obj.scrollLeft;
		if (obj.tagName.toLowerCase() != "html") {
			oouiY += obj.offsetTop - obj.scrollTop;
		}
		obj = obj.offsetParent;
	}

	try {
		obj = window.frameElement;
		while (obj) {
			if (fRtl) {
				if (obj.scrollWidth >= obj.clientWidth + obj.scrollLeft)
					objDX = obj.scrollWidth - obj.clientWidth - obj.scrollLeft;
				else
					objDX = obj.clientWidth + obj.scrollLeft - obj.scrollWidth;
				oouiX += obj.offsetLeft + objDX;
			}
			else
				oouiX += obj.offsetLeft - obj.scrollLeft;
			oouiY += obj.offsetTop - obj.scrollTop;
			obj = obj.offsetParent;
		}
	}
	catch (e) {
	};

	objRet.objSpan = objSpan;
	objRet.objOOUI = objOOUI;
	objRet.oouiX = oouiX;
	objRet.oouiY = oouiY;

	if (fRtl)
		objRet.oouiX += objOOUI.offsetWidth;

	return objRet;
}

function IMNShowOOUI(inputType) {
	if (browseris.ie5up && browseris.win32) {
		var obj = window.event.srcElement;
		var objSpan = obj;
		var objOOUI = obj;
		var oouiX = 0, oouiY = 0;

		if (EnsureIMNControl() && IMNNameDictionaryObj) {
			var objRet = IMNGetOOUILocation(obj);

			objOOUI = objRet.objOOUI;

			oouiX = objRet.oouiX - document.documentElement.scrollLeft;
			oouiY = objRet.oouiY - document.documentElement.scrollTop;

			var name = IMNNameDictionaryObj[objOOUI.id];

			SendCommandToMonitor(IMNCreateCommandString({ commandName: "showOOUI", name: name, inputType: inputType, oouiX: oouiX, oouiY: oouiY }));
		}
	}
}

function IMNShowOOUIMouse() {
	IMNShowOOUI(0);
}

function IMNShowOOUIKyb() {
	IMNShowOOUI(1);
}

function IMNHideOOUI() {
	SendCommandToMonitor(IMNCreateCommandString({ commandName: "hideOOUI" }));

	return false;
}

function SendCommandToMonitor(commandString) {
	var innerDoc = IMNGetInnerDoc();

	if (innerDoc != null) {
		var ma = innerDoc.getElementById("monitorAddresses");
		if (ma) {
			ma.value = commandString;
			innerDoc.getElementById("monitorButton").click();
		}
	}
}

function IMNScroll() {
	if (!bIMNInScrollFunc) {
		bIMNInScrollFunc = true;
		IMNHideOOUI();
	}

	bIMNInScrollFunc = false;

	if (IMNOrigScrollFunc == IMNScroll)
		return true;

	return IMNOrigScrollFunc ? IMNOrigScrollFunc() : true;
}

function ResetData() {
	browseris = new Browseris();
	bIMNControlInited = false;
	IMNDictionaryObj = null;
	bIMNOnloadAttached = false;
	IMNOrigScrollFunc = null;
	bIMNInScrollFunc = false;
	IMNSortableObj = null;
	IMNHeaderObj = null;
	IMNNameDictionaryObj = null;
	IMNShowOfflineObj = null;
}

function ProcessImn() {
	ResetData();

	if (EnsureIMNControl()) {
		ProcessImnMarkers(document.getElementsByName("imnmark"));
	}
}

function ChangeImgToImnElement(img, sipAddress) {
	img.name = "imnmark";
	if (img.setAttribute)
		img.setAttribute("sip", sipAddress);
	else
		img.sip = sipAddress;

	if (img.id == null || img.id == "") {
		img.id = "imn_" + IMNSerializeID;
		IMNSerializeID++;
	}
}

function ChangeDivToImnElement(div, sipAddress) {
	var img = document.createElement("img");
	img.src = UserPresenceStatusUrl;
	img.className = "uc-hdr";
	img.ShowOfflinePawn = true;
	img.alt = "无联机状态信息";

	ChangeImgToImnElement(img, sipAddress);
	div.className = "uc-ball";
	div.innerHTML = "";
	div.appendChild(img);
}

function ProcessImnMarkersByDiv(divs) {
	var elements = [];
	for (var i = 0; i < divs.length; i++) {
		var div = divs[i];
		if (div.childNodes.length == 1) {
			elements.push(div.childNodes[0]);
		}
	}
	ProcessImnMarkers(elements);
}

function ProcessImnMarkers(imnElements) {
	var batchSize = 4;
	var delay = 40;
	var count = 0;

	if (arguments.length > 1)
		count = arguments[1];

	for (var i = 0; i < batchSize; ++i) {
		if (count >= imnElements.length)
			return;

		var imnElem = imnElements[count];
		var sip; //fixed by v-weirf
		if (imnElem.getAttribute)
			sip = imnElem.getAttribute("sip");
		else
			sip = imnElem['sip'];
		IMNRC(sip, imnElem);
		count++;
	}

	window.setTimeout(function () {
		ProcessImnMarkers(imnElements, count)
	}, delay);
}

function IMNRC(name, elem) {
	if (name == null || name == '')
		return;

	if (browseris.ie5up && browseris.win32) {
		var obj = (elem) ? elem : window.event.srcElement;
		var objSpan = obj;
		var id = obj.id;
		var fFirst = false;

		if (!IMNDictionaryObj) {
			IMNDictionaryObj = new Object();
			IMNNameDictionaryObj = new Object();
			IMNSortableObj = new Object();
			IMNShowOfflineObj = new Object();

			if (!IMNOrigScrollFunc) {
				IMNOrigScrollFunc = window.onscroll;
				window.onscroll = IMNScroll;
			}
		}

		if (IMNDictionaryObj) {
			if (!IMNNameDictionaryObj[id]) {
				IMNNameDictionaryObj[id] = name;
				fFirst = true;
			}

			if (typeof (IMNDictionaryObj[id]) == "undefined") {
				IMNDictionaryObj[id] = 1;
			}

			if (!IMNSortableObj[id] &&
				(typeof (obj.Sortable) != "undefined")) {
				IMNSortableObj[id] = obj.Sortable;
			}

			if (!IMNShowOfflineObj[id] &&
				(typeof (obj.ShowOfflinePawn) != "undefined")) {
				IMNShowOfflineObj[id] = obj.ShowOfflinePawn;
			}
		}

		if (fFirst) {
			var objRet = IMNGetOOUILocation(obj);
			objSpan = objRet.objSpan;
			if (objSpan) {
				objSpan.onmouseover = IMNShowOOUIMouse;
				objSpan.onfocusin = IMNShowOOUIKyb;
				objSpan.onmouseout = IMNHideOOUI;
				objSpan.onfocusout = IMNHideOOUI;
			}
		}

		SendCommandToMonitor(IMNCreateCommandString({ commandName: "subscribe", name: name, id: id }));
	}
}

function IMNCreateCommandString(command) {
	return Sys.Serialization.JavaScriptSerializer.serialize(command);
}

function IMNGetInnerDoc() {
	var innerDoc = null;

	if (document.frames["userPresenceInnerPage"] != null)
		innerDoc = document.frames["userPresenceInnerPage"].document;

	return innerDoc;
}