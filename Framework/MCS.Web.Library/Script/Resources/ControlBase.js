
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


// Repository of old "Atlas" code that we're waiting to have integrated into the new Microsoft Ajax Library
var _Array$indexOf$old = Array.indexOf;
Array.indexOf = function (array, item, start, compareMethod) {
	if (compareMethod)
		return Array._indexOfExt(array, item, start, compareMethod);
	else
		return _Array$indexOf$old(array, item, start);
}
Array._indexOfExt = function (array, item, start, compareMethod) {
	/// <param name="array" type="Array" elementMayBeNull="true"></param>
	/// <param name="item" optional="true" mayBeNull="true"></param>
	/// <param name="start" optional="true" mayBeNull="true"></param>
	/// <returns type="Number"></returns>
	var e = Function._validateParams(arguments, [
        { name: "array", type: Array, elementMayBeNull: true },
        { name: "item", mayBeNull: true, optional: true },
        { name: "start", mayBeNull: true, optional: true },
        { name: "compareMethod", type: Function, mayBeNull: false }
    ]);
	if (e) throw e;

	if (typeof (item) === "undefined") return -1;
	var length = array.length;
	if (length !== 0) {
		start = start - 0;
		if (isNaN(start)) {
			start = 0;
		}
		else {
			if (isFinite(start)) {
				start = start - (start % 1);
			}
			if (start < 0) {
				start = Math.max(0, length + start);
			}
		}

		for (var i = start; i < length; i++) {
			if (compareMethod(array[i], item)) {
				return i;
			}
		}
	}
	return -1;
}

var _Array$containsExt$old = Array.contains;
Array.contains = function (array, item, compareMethod) {
	if (compareMethod)
		return Array._containsExt(array, item, compareMethod);
	else
		return _Array$containsExt$old(array, item);
}

Array._containsExt = function (array, item, compareMethod) {
	var e = Function._validateParams(arguments, [
        { name: "array", type: Array, elementMayBeNull: true },
        { name: "item", mayBeNull: true },
        { name: "compareMethod", type: Function, mayBeNull: false }
    ]);
	if (e) throw e;

	return (Array.indexOf(array, item, 0, compareMethod) >= 0);
}

Object.clone = function (obj, deepClone) {
	if (obj == null)
		return null;

	if (obj instanceof Array) {
		var result = new Array();
		for (var i = 0; i < obj.length; i++) {
			if (deepClone && (typeof (obj[i]) === 'object' || obj[i] instanceof Array))
				result.push(Object.clone(obj[i], true));
			else
				result.push(obj[i]);
		}
	}
	else {
		var result = {};
		for (var pName in obj) {
			var pValue = obj[pName];
			if (deepClone && (typeof (pValue) === "object" || pValue instanceof Array))
				result[pName] = Object.clone(pValue, true);
			else
				result[pName] = pValue;
		}
		if (obj.constructor)
			result.constructor = obj.constructor;
	}

	return result;
}

Object.getPropertyValue = function (obj, propertyName) {
	var v = obj[propertyName];
	if (typeof (v) === "undefined") {
		var getMethod = obj["get_" + propertyName];
		if (typeof (getMethod) === "function") {
			v = getMethod.call(obj);
		}
	}
	return v;
}


$setProperties = Sys.Component.setProperties = Sys$Component$_setProperties;
$Serializer = Sys.Serialization.JavaScriptSerializer;
Sys.Serialization.JavaScriptSerializer.setType = function (obj, typeKey) {
	var type = $HGRootNS[typeKey];

	if (!type) throw Error.create(String.format($HGRootNS.Resources.E_TypeKeyNotExist, typeKey));

	obj.__type = $HGRootNS[typeKey];

	return obj;
}

///////////////////////////
/// Sys.UI.DomElement

// DELTA - not present in codebase but called from PopupBehavior
Sys.UI.DomElement.setVisible = function (e, value) {

	if (!e) return;

	if (value != Sys.UI.DomElement.getVisible(e)) {

		if (value) {
			if (e.style.removeAttribute) {
				e.style.removeAttribute("display");
			} else {
				e.style.removeProperty("display");
			}
		}
		else {
			e.style.display = 'none';
		}

		e.style.visibility = value ? 'visible' : 'hidden';
	}
}

Sys.UI.DomElement.getVisible = function (e) {

	if (!e) return false;

	return (("none" != $HGDomElement.getCurrentStyle(e, "display")) &&
        ("hidden" != $HGDomElement.getCurrentStyle(e, "visibility")));
}


//////////////////////////////////////
// Sys.UI.Control.overlaps
//

Sys.UI.Control.overlaps = function overlaps(r1, r2) {
	var xLeft = (r1.x >= r2.x && r1.x <= (r2.x + r2.width));
	var xRight = ((r1.x + r1.width) >= r2.x && (r1.x + r1.width) <= r2.x + r2.width);
	var xComplete = ((r1.x < r2.x) && ((r1.x + r1.width) > (r2.x + r2.width)));

	var yLeft = (r1.y >= r2.y && r1.y <= (r2.y + r2.height));
	var yRight = ((r1.y + r1.height) >= r2.y && (r1.y + r1.height) <= r2.y + r2.height);
	var yComplete = ((r1.y < r2.y) && ((r1.y + r1.height) > (r2.y + r2.height)));
	if ((xLeft || xRight || xComplete) && (yLeft || yRight || yComplete)) {
		return true;
	}

	return false;
}

//override System Function
var $addCssClass = Sys.UI.DomElement.addCssClass;
var $removeCssClass = Sys.UI.DomElement.removeCssClass;
var $toggleCssClass = Sys.UI.DomElement.toggleCssClass;
Sys.UI.DomElement.getParentWindow = function (element) {
	var e = Function._validateParams(arguments, [
        { name: "element", domElement: true }
    ]);

	var elt = element;
	while (elt != null) {
		if (elt.parentWindow)
			return elt.parentWindow;
		elt = elt.parentElement;
	}

	return window;
}

Sys.UI.DomElement.getParentForm = function (element) {
	var form = null;

	while (element) {
		if (element.tagName == "FORM") {
			form = element;
			break;
		}

		element = element.parentElement;
	}

	return form;
}

Sys.UI.DomElement.submitToNewTargetForm = function (serverButton, newTarget) {
	var form = Sys.UI.DomElement.getParentForm(serverButton);

	if (form) {
		var oldTarget = form.target;

		try {
			form.target = newTarget;

			serverButton.click();
		}
		finally {
			form.target = oldTarget;
		}
	}
}

var $clearChildElementHandlers = Sys.UI.DomEvent.clearChildElementHandlers = function (parent) {
	var all = parent.all;
	for (var i = 0; i < all.length; i++) {
		$clearHandlers(all(i));
	}
}

var $addHandler = Sys.UI.DomEvent.addHandler = function (element, eventName, handler) {
	/// <param name="element" domElement="true"></param>
	/// <param name="eventName" type="String"></param>
	/// <param name="handler" type="Function"></param>
	var e = Function._validateParams(arguments, [
        { name: "element", domElement: true },
        { name: "eventName", type: String },
        { name: "handler", type: Function }
    ], false);
	if (e) throw e;

	if (!element._events) {
		element._events = {};
	}
	var eventCache = element._events[eventName];
	if (!eventCache) {
		element._events[eventName] = eventCache = [];
	}
	var browserHandler;
	if (element.addEventListener) {
		browserHandler = function (e) {
			return handler.call(element, new Sys.UI.DomEvent(e));
		}
		element.addEventListener(eventName, browserHandler, false);
	}
	else if (element.attachEvent) {
		var win = element.parentWindow || Sys.UI.DomElement.getParentWindow(element);
		if (win != window && win["__popupWindowEventDelegate"]) {
			if (!element.eventAchorCache)
				element.eventAchorCache = {};
			var a = element.eventAchorCache[eventName];
			if (!a) {
				a = document.createElement("a");
				a.style.display = "none";
				//a.innerText = eventName;
				document.body.appendChild(a);
				//a.attachEvent("onclick", function() { handler.call(element, new Sys.UI.DomEvent(a.event || event)) });
				$addHandler(a, "click",
                    function () {
                    	var e = new Sys.UI.DomEvent(a.event || event);
                    	e.handlingElement = element;
                    	handler.call(element, e);
                    });
				element.eventAchorCache[eventName] = a;
			}
			browserHandler = win["__popupWindowEventDelegate"](element);
		}
		else {
			browserHandler = function () {
				var e = new Sys.UI.DomEvent(event || win.event || {});
				e.handlingElement = element;
				return handler.call(element, e);
			}
		}
		element.attachEvent('on' + eventName, browserHandler);
	}
	eventCache[eventCache.length] = { handler: handler, browserHandler: browserHandler };
}

Sys.UI.DomEvent.writePopupWindowEventDelegate = function (win) {
	win.document.write(Sys.UI.DomEvent.getPopupWindowEventDelegateScriptString());
}

Sys.UI.DomEvent.getPopupWindowEventDelegateScriptString = function () {
	var strB = new Sys.StringBuilder("<script language='javascript'>");
	strB.append("__popupWindowEventDelegate = function(elt){return function(){");
	strB.append("if (elt && elt.eventAchorCache) {");
	strB.append("var a = elt.eventAchorCache[event.type];");
	strB.append("if (a)");
	strB.append("{a.event=event; a.click();}");
	strB.append("}");
	strB.append("}}");
	strB.append("</script>");

	return strB.toString();
}

//扩展了async属性，支持同步或异步传输
Sys.Net.XMLHttpExecutor2 = function () {
	Sys.Net.XMLHttpExecutor2.initializeBase(this);

	this._async = true;
}

Sys.Net.XMLHttpExecutor2.registerClass('Sys.Net.XMLHttpExecutor2', Sys.Net.XMLHttpExecutor);

function Sys$Net$XMLHttpExecutor2$executeRequest() {
	if (arguments.length !== 0) throw Error.parameterCount();
	this._webRequest = this.get_webRequest();

	if (this._started) {
		throw Error.invalidOperation(String.format(Sys.Res.cannotCallOnceStarted, 'executeRequest'));
	}
	if (this._webRequest === null) {
		throw Error.invalidOperation(Sys.Res.nullWebRequest);
	}

	var body = this._webRequest.get_body();
	var headers = this._webRequest.get_headers();
	this._xmlHttpRequest = new XMLHttpRequest();
	this._xmlHttpRequest.onreadystatechange = this._onReadyStateChange;
	var verb = this._webRequest.get_httpVerb();

	this._xmlHttpRequest.open(verb, this._webRequest.getResolvedUrl(), this.get_async());

	if (headers) {
		for (var header in headers) {
			var val = headers[header];
			if (typeof (val) !== "function")
				this._xmlHttpRequest.setRequestHeader(header, val);
		}
	}

	if (verb.toLowerCase() === "post") {
		if ((headers === null) || !headers['Content-Type']) {
			this._xmlHttpRequest.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
		}

		if (!body) {
			body = "";
		}
	}

	var timeout = this._webRequest.get_timeout();
	if (timeout > 0) {
		this._timer = window.setTimeout(Function.createDelegate(this, this._onTimeout), timeout);
	}
	this._xmlHttpRequest.send(body);
	this._started = true;
}

Sys.Net.XMLHttpExecutor2.prototype.executeRequest = Sys$Net$XMLHttpExecutor2$executeRequest;

Sys.Net.XMLHttpExecutor2.prototype.get_async = function () {
	return this._async;
}

Sys.Net.XMLHttpExecutor2.prototype.set_async = function (value) {
	this._async = value;
}

var _Original_WebForm_InitCallback = WebForm_InitCallback;
WebForm_InitCallback = function () {
	if (typeof (Sys) === "object" && typeof (Sys.Application) === "object") {
		Sys.Application.add_load(_Original_WebForm_InitCallback);
	}
	else
		_Original_WebForm_InitCallback();
}

function collectFormDataDictionary(postData) {
	var dict = {};

	var variableParts = postData.split("&");

	for (var i = 0; i < variableParts.length; i++) {
		var varPair = variableParts[i].split("=");
		var key = varPair[0];
		var value = "";

		if (varPair.length > 1)
			value = varPair[1];

		if (key != "")
			dict[key] = value;
	}

	return dict;
}

function combineKeyValueDictionaryToString(dict) {
	var ignorPriperties = [];

	if (arguments.length > 1) {
		for (var i = 1; i < arguments.length; i++)
			ignorPriperties.push(arguments[i]);
	}

	var strB = new Sys.StringBuilder();

	for (var key in dict) {
		if (Array.contains(ignorPriperties, key) == false) {
			if (strB.isEmpty() == false)
				strB.append("&");

			strB.append(key + "=" + dict[key]);
		}
	}

	return strB.toString();
}

/// Asp.net 2.0 回调函数修改
function WebForm_DoCallback(eventTarget, eventArgument, eventCallback, context, errorCallback, callbackUseAsync, xmlHttpSync, removeViewState) {
	var formDataDict = collectFormDataDictionary(__theFormPostData);

	var formPostData = "";

	if (removeViewState)
		formPostData = combineKeyValueDictionaryToString(formDataDict, "__VIEWSTATE");
	else
		formPostData = combineKeyValueDictionaryToString(formDataDict);

	if (formPostData.length > 0)
		formPostData += "&";

	var postData = formPostData +
                "__CALLBACKID=" + WebForm_EncodeCallback(eventTarget) +
                "&__CALLBACKPARAM=" + WebForm_EncodeCallback(eventArgument);
	if (theForm["__EVENTVALIDATION"]) {
		postData += "&__EVENTVALIDATION=" + WebForm_EncodeCallback(theForm["__EVENTVALIDATION"].value);
	}
	var xmlRequest, e;
	try {
		xmlRequest = new XMLHttpRequest();
	}
	catch (e) {
		try {
			xmlRequest = new ActiveXObject("Microsoft.XMLHTTP");
		}
		catch (e) {
		}
	}
	var setRequestHeaderMethodExists = true;
	try {
		setRequestHeaderMethodExists = (xmlRequest && xmlRequest.setRequestHeader);
	}
	catch (e) { }
	var callback = new Object();
	callback.eventCallback = eventCallback;
	callback.context = context;
	callback.eventTarget = eventTarget;
	callback.errorCallback = errorCallback;
	callback.async = callbackUseAsync;
	var callbackIndex = WebForm_FillFirstAvailableSlot(__pendingCallbacks, callback);
	//    if (!callbackUseAsync) {
	//        if (__synchronousCallBackIndex != -1) {
	//            __pendingCallbacks[__synchronousCallBackIndex] = null;
	//        }
	//        __synchronousCallBackIndex = callbackIndex;
	//    }
	if (setRequestHeaderMethodExists) {
		xmlRequest.onreadystatechange = WebForm_CallbackComplete;
		callback.xmlRequest = xmlRequest;
		xmlRequest.open("POST", theForm.action, !xmlHttpSync);
		xmlRequest.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
		xmlRequest.send(postData);
		return;
	}
	callback.xmlRequest = new Object();
	var callbackFrameID = "__CALLBACKFRAME" + callbackIndex;
	var xmlRequestFrame = document.frames[callbackFrameID];
	if (!xmlRequestFrame) {
		xmlRequestFrame = document.createElement("IFRAME");
		xmlRequestFrame.width = "1";
		xmlRequestFrame.height = "1";
		xmlRequestFrame.frameBorder = "0";
		xmlRequestFrame.id = callbackFrameID;
		xmlRequestFrame.name = callbackFrameID;
		xmlRequestFrame.style.position = "absolute";
		xmlRequestFrame.style.top = "-100px"
		xmlRequestFrame.style.left = "-100px";
		try {
			if (callBackFrameUrl) {
				xmlRequestFrame.src = callBackFrameUrl;
			}
		}
		catch (e) { }
		document.body.appendChild(xmlRequestFrame);
	}
	var interval = window.setInterval(function () {
		xmlRequestFrame = document.frames[callbackFrameID];
		if (xmlRequestFrame && xmlRequestFrame.document) {
			window.clearInterval(interval);
			xmlRequestFrame.document.write("");
			xmlRequestFrame.document.close();
			xmlRequestFrame.document.write('<html><body><form method="post"><input type="hidden" name="__CALLBACKLOADSCRIPT" value="t"></form></body></html>');
			xmlRequestFrame.document.close();
			xmlRequestFrame.document.forms[0].action = theForm.action;
			var count = __theFormPostCollection.length;
			var element;
			for (var i = 0; i < count; i++) {
				element = __theFormPostCollection[i];
				if (element) {
					var fieldElement = xmlRequestFrame.document.createElement("INPUT");
					fieldElement.type = "hidden";
					fieldElement.name = element.name;
					fieldElement.value = element.value;
					xmlRequestFrame.document.forms[0].appendChild(fieldElement);
				}
			}
			var callbackIdFieldElement = xmlRequestFrame.document.createElement("INPUT");
			callbackIdFieldElement.type = "hidden";
			callbackIdFieldElement.name = "__CALLBACKID";
			callbackIdFieldElement.value = eventTarget;
			xmlRequestFrame.document.forms[0].appendChild(callbackIdFieldElement);
			var callbackParamFieldElement = xmlRequestFrame.document.createElement("INPUT");
			callbackParamFieldElement.type = "hidden";
			callbackParamFieldElement.name = "__CALLBACKPARAM";
			callbackParamFieldElement.value = eventArgument;
			xmlRequestFrame.document.forms[0].appendChild(callbackParamFieldElement);
			if (theForm["__EVENTVALIDATION"]) {
				var callbackValidationFieldElement = xmlRequestFrame.document.createElement("INPUT");
				callbackValidationFieldElement.type = "hidden";
				callbackValidationFieldElement.name = "__EVENTVALIDATION";
				callbackValidationFieldElement.value = theForm["__EVENTVALIDATION"].value;
				xmlRequestFrame.document.forms[0].appendChild(callbackValidationFieldElement);
			}
			var callbackIndexFieldElement = xmlRequestFrame.document.createElement("INPUT");
			callbackIndexFieldElement.type = "hidden";
			callbackIndexFieldElement.name = "__CALLBACKINDEX";
			callbackIndexFieldElement.value = callbackIndex;
			xmlRequestFrame.document.forms[0].appendChild(callbackIndexFieldElement);
			xmlRequestFrame.document.forms[0].submit();
		}
	}, 10);
}

// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

// Add common toolkit scripts here.  To consume the scripts on a control add
// 
//      [RequiredScript(typeof(DeluxeAjaxScript))] 
//      public class SomeExtender : ...
// 
// to the controls extender class declaration.

var $HGRootNSName = 'MCS.Web.WebControls';
Type.registerNamespace($HGRootNSName);
if ($HGRootNS) var $HGRootNS_Bak = $HGRootNS;
var $HGRootNS = eval($HGRootNSName);

if (typeof ($HGRootNS_Bak) === "object") {
	for (var pName in $HGRootNS_Bak)
		$HGRootNS[pName] = $HGRootNS_Bak[pName];
}

$HGRootNS.BoxSide = function () {
	/// <summary>
	/// The BoxSide enumeration describes the sides of a DOM element
	/// </summary>
	/// <field name="Top" type="Number" integer="true" static="true" />
	/// <field name="Right" type="Number" integer="true" static="true" />
	/// <field name="Bottom" type="Number" integer="true" static="true" />
	/// <field name="Left" type="Number" integer="true" static="true" />
}
$HGRootNS.BoxSide.prototype = {
	Top: 0,
	Right: 1,
	Bottom: 2,
	Left: 3
}
$HGRootNS.BoxSide.registerEnum($HGRootNSName + ".BoxSide", false);

$HGRootNS._DomElement = function () {
	/// <summary>
	/// The _DomElement class contains functionality utilized across a number
	/// of controls (but not universally)
	/// </summary>
	/// <remarks>
	/// You should not create new instances of _DomElement.  Instead you should use the shared instance $HGDomElement (or $HGRootNS.DomElement).
	/// </remarks>

	this._currentDocument = null;
	// Populate the borderThicknesses lookup table
	this._borderThicknesses = {};

	var div0 = document.createElement('div');
	var div1 = document.createElement('div');

	div0.style.visibility = 'hidden';
	div0.style.position = 'absolute';
	div0.style.fontSize = '1px';

	div1.style.height = '0px';
	div1.style.overflow = 'hidden';

	document.body.appendChild(div0).appendChild(div1);

	var base = div0.offsetHeight;
	div1.style.borderTop = 'solid black';

	div1.style.borderTopWidth = 'thin';
	this._borderThicknesses['thin'] = div0.offsetHeight - base;

	div1.style.borderTopWidth = 'medium';
	this._borderThicknesses['medium'] = div0.offsetHeight - base;

	div1.style.borderTopWidth = 'thick';
	this._borderThicknesses['thick'] = div0.offsetHeight - base;

	div0.removeChild(div1);
	document.body.removeChild(div0);
	div0 = null;
	div1 = null;
}
$HGRootNS._DomElement.prototype = {
	// The order of these lookup tables is directly linked to the BoxSide enum defined above
	_borderStyleNames: ['borderTopStyle', 'borderRightStyle', 'borderBottomStyle', 'borderLeftStyle'],
	_borderWidthNames: ['borderTopWidth', 'borderRightWidth', 'borderBottomWidth', 'borderLeftWidth'],
	_paddingWidthNames: ['paddingTop', 'paddingRight', 'paddingBottom', 'paddingLeft'],
	_marginWidthNames: ['marginTop', 'marginRight', 'marginBottom', 'marginLeft'],

	get_currentDocument: function () {
		return this._currentDocument || document;
	},

	set_currentDocument: function (doc) {
		this._currentDocument = doc;
	},

	getCurrentStyle: function (element, attribute, defaultValue) {
		/// <summary>
		/// $HGDomElement.getCurrentStyle is used to compute the value of a style attribute on an
		/// element that is currently being displayed.  This is especially useful for scenarios where
		/// several CSS classes and style attributes are merged, or when you need information about the
		/// size of an element (such as its padding or margins) that is not exposed in any other fashion.
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement" domElement="true">
		/// Live DOM element to check style of
		/// </param>
		/// <param name="attribute" type="String">
		/// The style attribute's name is expected to be in a camel-cased form that you would use when
		/// accessing a JavaScript property instead of the hyphenated form you would use in a CSS
		/// stylesheet (i.e. it should be "backgroundColor" and not "background-color").
		/// </param>
		/// <param name="defaultValue" type="Object" mayBeNull="true" optional="true">
		/// In the event of a problem (i.e. a null element or an attribute that cannot be found) we
		/// return this object (or null if none if not specified).
		/// </param>
		/// <returns type="Object">
		/// Current style of the element's attribute
		/// </returns>

		var currentValue = null;
		if (element) {
			if (element.currentStyle) {
				currentValue = element.currentStyle[attribute];
			} else if (document.defaultView && document.defaultView.getComputedStyle) {
				var style = document.defaultView.getComputedStyle(element, null);
				if (style) {
					currentValue = style[attribute];
				}
			}

			if (!currentValue && element.style.getPropertyValue) {
				currentValue = element.style.getPropertyValue(attribute);
			}
			else if (!currentValue && element.style.getAttribute) {
				currentValue = element.style.getAttribute(attribute);
			}
		}

		if ((!currentValue || currentValue == "" || typeof (currentValue) === 'undefined')) {
			if (typeof (defaultValue) != 'undefined') {
				currentValue = defaultValue;
			}
			else {
				currentValue = null;
			}
		}
		return currentValue;
	},

	getInheritedBackgroundColor: function (element) {
		/// <summary>
		/// $HGDomElement.getInheritedBackgroundColor provides the ability to get the displayed
		/// background-color of an element.  In most cases calling $HGDomElement.getCurrentStyle
		/// won't do the job because it will return "transparent" unless the element has been given a
		/// specific background color.  This function will walk up the element's parents until it finds
		/// a non-transparent color.  If we get all the way to the top of the document or have any other
		/// problem finding a color, we will return the default value '#FFFFFF'.  This function is
		/// especially important when we're using opacity in IE (because ClearType will make text look
		/// horrendous if you fade it with a transparent background color).
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement" domElement="true">
		/// Live DOM element to get the background color of
		/// </param>
		/// <returns type="String">
		/// Background color of the element
		/// </returns>

		if (!element) return '#FFFFFF';
		var background = this.getCurrentStyle(element, 'backgroundColor');
		try {
			while (!background || background == '' || background == 'transparent' || background == 'rgba(0, 0, 0, 0)') {
				element = element.parentNode;
				if (!element) {
					background = '#FFFFFF';
				} else {
					background = this.getCurrentStyle(element, 'backgroundColor');
				}
			}
		} catch (ex) {
			background = '#FFFFFF';
		}
		return background;
	},

	getLocation: function (element) {
		/// <summary>Gets the coordinates of a DOM element.</summary>
		/// <param name="element" domElement="true"/>
		/// <returns type="Sys.UI.Point">
		///   A Point object with two fields, x and y, which contain the pixel coordinates of the element.
		/// </returns>

		// workaround for an issue in getLocation where it will compute the location of the document element.
		// this will return an offset if scrolled.
		//
		if (element === document.documentElement) {
			return new Sys.UI.Point(0, 0);
		}

		// Workaround for IE6 bug in getLocation (also required patching getBounds - remove that fix when this is removed)
		if (Sys.Browser.agent == Sys.Browser.InternetExplorer && Sys.Browser.version < 7) {
			if (element.window === element || element.nodeType === 9 || !element.getClientRects || !element.getBoundingClientRect) return new Sys.UI.Point(0, 0);

			// Get the first bounding rectangle in screen coordinates
			var screenRects = element.getClientRects();
			if (!screenRects || !screenRects.length) {
				return new Sys.UI.Point(0, 0);
			}
			var first = screenRects[0];

			// Delta between client coords and screen coords
			var dLeft = 0;
			var dTop = 0;

			var inFrame = false;
			try {
				inFrame = element.ownerDocument.parentWindow.frameElement;
			} catch (ex) {
				// If accessing the frameElement fails, a frame is probably in a different
				// domain than its parent - and we still want to do the calculation below
				inFrame = true;
			}

			// If we're in a frame, get client coordinates too so we can compute the delta
			if (inFrame) {
				// Get the bounding rectangle in client coords
				var clientRect = element.getBoundingClientRect();
				if (!clientRect) {
					return new Sys.UI.Point(0, 0);
				}

				// Find the minima in screen coords
				var minLeft = first.left;
				var minTop = first.top;
				for (var i = 1; i < screenRects.length; i++) {
					var r = screenRects[i];
					if (r.left < minLeft) {
						minLeft = r.left;
					}
					if (r.top < minTop) {
						minTop = r.top;
					}
				}

				// Compute the delta between screen and client coords
				dLeft = minLeft - clientRect.left;
				dTop = minTop - clientRect.top;
			}

			// Subtract 2px, the border of the viewport (It can be changed in IE6 by applying a border style to the HTML element,
			// but this is not supported by ASP.NET AJAX, and it cannot be changed in IE7.), and also subtract the delta between
			// screen coords and client coords
			var ownerDocument = element.document.documentElement;
			return new Sys.UI.Point(first.left - 2 - dLeft + ownerDocument.scrollLeft, first.top - 2 - dTop + ownerDocument.scrollTop);
		}

		return Sys.UI.DomElement.getLocation(element);
	},

	setLocation: function (element, point) {
		/// <summary>
		/// Sets the current location for an element.
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement" domElement="true">
		/// DOM element
		/// </param>
		/// <param name="point" type="Object">
		/// Point object (of the form {x,y})
		/// </param>
		/// <remarks>
		/// This method does not attempt to set the positioning mode of an element.
		/// The position is relative from the elements nearest position:relative or
		/// position:absolute element.
		/// </remarks>
		Sys.UI.DomElement.setLocation(element, point.x, point.y);
	},

	setDynamicLocation: function (elt1, elt2, setLocationFunction) {
		var handler = function () { setLocationFunction(elt1, elt2); };
		handler();
		$addHandlers(elt1, { "resize": handler, "move": handler });
		$addHandlers(window, { "resize": handler });
	},

	getContentSize: function (element) {
		/// <summary>
		/// Gets the "content-box" size of an element.
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement" domElement="true">
		/// DOM element
		/// </param>
		/// <returns type="Object">
		/// Size of the element (in the form {width,height})
		/// </returns>
		/// <remarks>
		/// The "content-box" is the size of the content area *inside* of the borders and
		/// padding of an element. The "content-box" size does not include the margins around
		/// the element.
		/// </remarks>

		if (!element) {
			throw Error.argumentNull('element');
		}
		var size = this.getSize(element);
		var borderBox = this.getBorderBox(element);
		var paddingBox = this.getPaddingBox(element);
		return {
			width: size.width - borderBox.horizontal - paddingBox.horizontal,
			height: size.height - borderBox.vertical - paddingBox.vertical
		}
	},

	getSize: function (element) {
		/// <summary>
		/// Gets the "border-box" size of an element.
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement" domElement="true">
		/// DOM element
		/// </param>
		/// <returns type="Object">
		/// Size of the element (in the form {width,height})
		/// </returns>
		/// <remarks>
		/// The "border-box" is the size of the content area *outside* of the borders and
		/// padding of an element.  The "border-box" size does not include the margins around
		/// the element.
		/// </remarks>

		if (!element) {
			throw Error.argumentNull('element');
		}
		return {
			width: element.offsetWidth,
			height: element.offsetHeight
		};
	},

	getMaxSize: function (elts) {
		var w = 0;
		var h = 0;
		for (var i = 0; i < elts.length; i++) {
			var size = this.getSize(elts[i]);
			if (size.width > w) w = size.width;
			if (size.height > h) h = size.height;
		}

		return { width: w, height: h };
	},

	setContentSize: function (element, size) {
		/// <summary>
		/// Sets the "content-box" size of an element.
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement" domElement="true">
		/// DOM element
		/// </param>
		/// <param name="size" type="Object">
		/// Size of the element (in the form {width,height})
		/// </param>
		/// <remarks>
		/// The "content-box" is the size of the content area *inside* of the borders and
		/// padding of an element. The "content-box" size does not include the margins around
		/// the element.
		/// </remarks>

		if (!element) {
			throw Error.argumentNull('element');
		}
		if (!size) {
			throw Error.argumentNull('size');
		}
		// FF respects -moz-box-sizing css extension, so adjust the box size for the border-box
		if (this.getCurrentStyle(element, 'MozBoxSizing') == 'border-box' || this.getCurrentStyle(element, 'BoxSizing') == 'border-box') {
			var borderBox = this.getBorderBox(element);
			var paddingBox = this.getPaddingBox(element);
			size = {
				width: size.width + borderBox.horizontal + paddingBox.horizontal,
				height: size.height + borderBox.vertical + paddingBox.vertical
			};
		}
		element.style.width = size.width.toString() + 'px';
		element.style.height = size.height.toString() + 'px';
	},

	setSize: function (element, size) {
		/// <summary>
		/// Sets the "border-box" size of an element.
		/// </summary>
		/// <remarks>
		/// The "border-box" is the size of the content area *outside* of the borders and 
		/// padding of an element.  The "border-box" size does not include the margins around
		/// the element.
		/// </remarks>
		/// <param name="element" type="Sys.UI.DomElement">DOM element</param>
		/// <param name="size" type="Object">Size of the element (in the form {width,height})</param>
		/// <returns />

		if (!element) {
			throw Error.argumentNull('element');
		}
		if (!size) {
			throw Error.argumentNull('size');
		}
		var borderBox = this.getBorderBox(element);
		var paddingBox = this.getPaddingBox(element);
		var contentSize = {
			width: size.width - borderBox.horizontal - paddingBox.horizontal,
			height: size.height - borderBox.vertical - paddingBox.vertical
		};
		this.setContentSize(element, contentSize);
	},

	getBounds: function (element) {
		/// <summary>Gets the coordinates, width and height of an element.</summary>
		/// <param name="element" domElement="true"/>
		/// <returns type="Sys.UI.Bounds">
		///   A Bounds object with four fields, x, y, width and height, which contain the pixel coordinates,
		///   width and height of the element.
		/// </returns>
		/// <remarks>
		///   Use the $HGDomElement version of getLocation to handle the workaround for IE6.  We can
		///   remove the below implementation and just call Sys.UI.DomElement.getBounds when the other bug
		///   is fixed.
		/// </remarks>

		var offset = this.getLocation(element);
		return new Sys.UI.Bounds(offset.x, offset.y, element.offsetWidth || 0, element.offsetHeight || 0);
	},

	setBounds: function (element, bounds) {
		/// <summary>
		/// Sets the "border-box" bounds of an element
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement" domElement="true">
		/// DOM element
		/// </param>
		/// <param name="bounds" type="Object">
		/// Bounds of the element (of the form {x,y,width,height})
		/// </param>
		/// <remarks>
		/// The "border-box" is the size of the content area *outside* of the borders and
		/// padding of an element.  The "border-box" size does not include the margins around
		/// the element.
		/// </remarks>

		if (!element) {
			throw Error.argumentNull('element');
		}
		if (!bounds) {
			throw Error.argumentNull('bounds');
		}
		this.setSize(element, bounds);
		this.setLocation(element, bounds);
	},

	getClientBounds: function () {
		/// <summary>
		/// Gets the width and height of the browser client window (excluding scrollbars)
		/// </summary>
		/// <returns type="Sys.UI.Bounds">
		/// Browser's client width and height
		/// </returns>

		var clientWidth;
		var clientHeight;
		switch (Sys.Browser.agent) {
			case Sys.Browser.InternetExplorer:
				clientWidth = document.documentElement.clientWidth;
				clientHeight = document.documentElement.clientHeight;
				break;
			case Sys.Browser.Safari:
				clientWidth = window.innerWidth;
				clientHeight = window.innerHeight;
				break;
			case Sys.Browser.Opera:
				clientWidth = Math.min(window.innerWidth, document.body.clientWidth);
				clientHeight = Math.min(window.innerHeight, document.body.clientHeight);
				break;
			default:  // Sys.Browser.Firefox, etc.
				clientWidth = Math.min(window.innerWidth, document.documentElement.clientWidth);
				clientHeight = Math.min(window.innerHeight, document.documentElement.clientHeight);
				break;
		}
		return new Sys.UI.Bounds(0, 0, clientWidth, clientHeight);
	},

	getMarginBox: function (element) {
		/// <summary>
		/// Gets the entire margin box sizes.
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement" domElement="true">
		/// DOM element
		/// </param>
		/// <returns type="Object">
		/// Element's margin box sizes (of the form {top,left,bottom,right,horizontal,vertical})
		/// </returns>

		if (!element) {
			throw Error.argumentNull('element');
		}
		var box = {
			top: this.getMargin(element, $HGRootNS.BoxSide.Top),
			right: this.getMargin(element, $HGRootNS.BoxSide.Right),
			bottom: this.getMargin(element, $HGRootNS.BoxSide.Bottom),
			left: this.getMargin(element, $HGRootNS.BoxSide.Left)
		}
		box.horizontal = box.left + box.right;
		box.vertical = box.top + box.bottom;
		return box;
	},

	getBorderBox: function (element) {
		/// <summary>
		/// Gets the entire border box sizes.
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement" domElement="true">
		/// DOM element
		/// </param>
		/// <returns type="Object">
		/// Element's border box sizes (of the form {top,left,bottom,right,horizontal,vertical})
		/// </returns>

		if (!element) {
			throw Error.argumentNull('element');
		}
		var box = {
			top: this.getBorderWidth(element, $HGRootNS.BoxSide.Top),
			right: this.getBorderWidth(element, $HGRootNS.BoxSide.Right),
			bottom: this.getBorderWidth(element, $HGRootNS.BoxSide.Bottom),
			left: this.getBorderWidth(element, $HGRootNS.BoxSide.Left)
		}
		box.horizontal = box.left + box.right;
		box.vertical = box.top + box.bottom;
		return box;
	},

	getPaddingBox: function (element) {
		/// <summary>
		/// Gets the entire padding box sizes.
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement" domElement="true">
		/// DOM element
		/// </param>
		/// <returns type="Object">
		/// Element's padding box sizes (of the form {top,left,bottom,right,horizontal,vertical})
		/// </returns>

		if (!element) {
			throw Error.argumentNull('element');
		}
		var box = {
			top: this.getPadding(element, $HGRootNS.BoxSide.Top),
			right: this.getPadding(element, $HGRootNS.BoxSide.Right),
			bottom: this.getPadding(element, $HGRootNS.BoxSide.Bottom),
			left: this.getPadding(element, $HGRootNS.BoxSide.Left)
		}
		box.horizontal = box.left + box.right;
		box.vertical = box.top + box.bottom;
		return box;
	},

	isBorderVisible: function (element, boxSide) {
		/// <summary>
		/// Gets whether the current border style for an element on a specific boxSide is not 'none'.
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement" domElement="true">
		/// DOM element
		/// </param>
		/// <param name="boxSide" type=$HGRootNSName + ".BoxSide">
		/// Side of the element
		/// </param>
		/// <returns type="Boolean">
		/// Whether the current border style for an element on a specific boxSide is not 'none'.
		/// </returns>

		if (!element) {
			throw Error.argumentNull('element');
		}
		if (boxSide < $HGRootNS.BoxSide.Top || boxSide > $HGRootNS.BoxSide.Left) {
			throw Error.argumentOutOfRange(String.format(Sys.Res.enumInvalidValue, boxSide, $HGRootNSName + '.BoxSide'));
		}
		var styleName = this._borderStyleNames[boxSide];
		var styleValue = this.getCurrentStyle(element, styleName);
		return styleValue != "none";
	},

	getMargin: function (element, boxSide) {
		/// <summary>
		/// Gets the margin thickness of an element on a specific boxSide.
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement" domElement="true">
		/// DOM element
		/// </param>
		/// <param name="boxSide" type=$HGRootNSName + ".BoxSide">
		/// Side of the element
		/// </param>
		/// <returns type="Number" integer="true">
		/// Margin thickness on the element's specified side
		/// </returns>

		if (!element) {
			throw Error.argumentNull('element');
		}
		if (boxSide < $HGRootNS.BoxSide.Top || boxSide > $HGRootNS.BoxSide.Left) {
			throw Error.argumentOutOfRange(String.format(Sys.Res.enumInvalidValue, boxSide, $HGRootNSName + '.BoxSide'));
		}
		var styleName = this._marginWidthNames[boxSide];
		var styleValue = this.getCurrentStyle(element, styleName);
		try { return this.parsePadding(styleValue); } catch (ex) { return 0; }
	},

	getBorderWidth: function (element, boxSide) {
		/// <summary>
		/// Gets the border thickness of an element on a specific boxSide.
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement" domElement="true">
		/// DOM element
		/// </param>
		/// <param name="boxSide" type=$HGRootNSName + ".BoxSide">
		/// Side of the element
		/// </param>
		/// <returns type="Number" integer="true">
		/// Border thickness on the element's specified side
		/// </returns>

		if (!element) {
			throw Error.argumentNull('element');
		}
		if (boxSide < $HGRootNS.BoxSide.Top || boxSide > $HGRootNS.BoxSide.Left) {
			throw Error.argumentOutOfRange(String.format(Sys.Res.enumInvalidValue, boxSide, $HGRootNSName + '.BoxSide'));
		}
		if (!this.isBorderVisible(element, boxSide)) {
			return 0;
		}
		var styleName = this._borderWidthNames[boxSide];
		var styleValue = this.getCurrentStyle(element, styleName);
		return this.parseBorderWidth(styleValue);
	},

	getPadding: function (element, boxSide) {
		/// <summary>
		/// Gets the padding thickness of an element on a specific boxSide.
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement" domElement="true">
		/// DOM element
		/// </param>
		/// <param name="boxSide" type=$HGRootNSName + ".BoxSide">
		/// Side of the element
		/// </param>
		/// <returns type="Number" integer="true">
		/// Padding on the element's specified side
		/// </returns>

		if (!element) {
			throw Error.argumentNull('element');
		}
		if (boxSide < $HGRootNS.BoxSide.Top || boxSide > $HGRootNS.BoxSide.Left) {
			throw Error.argumentOutOfRange(String.format(Sys.Res.enumInvalidValue, boxSide, $HGRootNSName + '.BoxSide'));
		}
		var styleName = this._paddingWidthNames[boxSide];
		var styleValue = this.getCurrentStyle(element, styleName);
		return this.parsePadding(styleValue);
	},

	parseBorderWidth: function (borderWidth) {
		/// <summary>
		/// Parses a border-width string into a pixel size
		/// </summary>
		/// <param name="borderWidth" type="String" mayBeNull="true">
		/// Type of border ('thin','medium','thick','inherit',px unit,null,'')
		/// </param>
		/// <returns type="Number" integer="true">
		/// Number of pixels in the border-width
		/// </returns>

		if (borderWidth) {
			switch (borderWidth) {
				case 'thin':
				case 'medium':
				case 'thick':
					return this._borderThicknesses[borderWidth];
				case 'inherit':
					return 0;
			}
			var unit = this.parseUnit(borderWidth);
			Sys.Debug.assert(unit.type == 'px', String.format($HGRootNS.Resources.Common_InvalidBorderWidthUnit, unit.type));
			return unit.size;
		}
		return 0;
	},

	parsePadding: function (padding) {
		/// <summary>
		/// Parses a padding string into a pixel size
		/// </summary>
		/// <param name="padding" type="String" mayBeNull="true">
		/// Padding to parse ('inherit',px unit,null,'')
		/// </param>
		/// <returns type="Number" integer="true">
		/// Number of pixels in the padding
		/// </returns>

		if (padding) {
			if (padding == 'inherit') {
				return 0;
			}
			var unit = this.parseUnit(padding);
			Sys.Debug.assert(unit.type == 'px', String.format($HGRootNS.Resources.Common_InvalidPaddingUnit, unit.type));
			return unit.size;
		}
		return 0;
	},

	parseUnit: function (value) {
		/// <summary>
		/// Parses a unit string into a unit object
		/// </summary>
		/// <param name="value" type="String" mayBeNull="true">
		/// Value to parse (of the form px unit,% unit,em unit,...)
		/// </param>
		/// <returns type="Object">
		/// Parsed unit (of the form {size,type})
		/// </returns>

		if (!value) {
			throw Error.argumentNull('value');
		}

		value = value.trim().toLowerCase();
		var l = value.length;
		var s = -1;
		for (var i = 0; i < l; i++) {
			var ch = value.substr(i, 1);
			if ((ch < '0' || ch > '9') && ch != '-' && ch != '.' && ch != ',') {
				break;
			}
			s = i;
		}
		if (s == -1) {
			throw Error.create($HGRootNS.Resources.Common_UnitHasNoDigits);
		}
		var type;
		var size;
		if (s < (l - 1)) {
			type = value.substring(s + 1).trim();
		} else {
			type = 'px';
		}
		size = parseFloat(value.substr(0, s + 1));
		if (type == 'px') {
			size = Math.floor(size);
		}
		return {
			size: size,
			type: type
		};
	},

	getElementOpacity: function (element) {
		/// <summary>
		/// Get the element's opacity
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement" domElement="true">
		/// Element
		/// </param>
		/// <returns type="Number">
		/// Opacity of the element
		/// </returns>

		if (!element) {
			throw Error.argumentNull('element');
		}

		var hasOpacity = false;
		var opacity;

		if (element.filters) {
			var filters = element.filters;
			if (filters.length !== 0) {
				var alphaFilter = filters['DXImageTransform.Microsoft.Alpha'];
				if (alphaFilter) {
					opacity = alphaFilter.opacity / 100.0;
					hasOpacity = true;
				}
			}
		}
		else {
			opacity = this.getCurrentStyle(element, 'opacity', 1);
			hasOpacity = true;
		}

		if (hasOpacity === false) {
			return 1.0;
		}
		return parseFloat(opacity);
	},

	setElementOpacity: function (element, value) {
		/// <summary>
		/// Set the element's opacity
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement" domElement="true">
		/// Element
		/// </param>
		/// <param name="value" type="Number">
		/// Opacity of the element
		/// </param>

		if (!element) {
			throw Error.argumentNull('element');
		}

		if (element.filters) {
			var filters = element.filters;
			var createFilter = true;
			if (filters.length !== 0) {
				var alphaFilter = filters['DXImageTransform.Microsoft.Alpha'];
				if (alphaFilter) {
					createFilter = false;
					alphaFilter.opacity = value * 100;
				}
			}
			if (createFilter) {
				element.style.filter = 'progid:DXImageTransform.Microsoft.Alpha(opacity=' + (value * 100) + ')';
			}
		}
		else {
			element.style.opacity = value;
		}
	},



	addCssClasses: function (element, classNames) {
		/// <summary>
		/// Adds multiple css classes to a DomElement
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement">The element to modify</param>
		/// <param name="classNames" type="Array">The class names to add</param>

		for (var i = 0; i < classNames.length; i++) {
			Sys.UI.DomElement.addCssClass(element, classNames[i]);
		}
	},
	removeCssClasses: function (element, classNames) {
		/// <summary>
		/// Removes multiple css classes to a DomElement
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement">The element to modify</param>
		/// <param name="classNames" type="Array">The class names to remove</param>

		for (var i = 0; i < classNames.length; i++) {
			Sys.UI.DomElement.removeCssClass(element, classNames[i]);
		}
	},
	setStyle: function (element, style) {
		/// <summary>
		/// Sets the style of the element using the supplied style template object
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement">The element to modify</param>
		/// <param name="style" type="Object">The template</param>

		var styleTemp = {};
		if (style) {
			for (var p in style) {
				if (p != "font" || style[p])
					styleTemp[p] = style[p];
			}
		}
		this.applyProperties(element.style, styleTemp);
	},

	containsPoint: function (rect, x, y) {
		/// <summary>
		/// Tests whether a point (x,y) is contained within a rectangle
		/// </summary>
		/// <param name="rect" type="Object">The rectangle</param>
		/// <param name="x" type="Number">The x coordinate of the point</param>
		/// <param name="y" type="Number">The y coordinate of the point</param>

		return x >= rect.x && x <= (rect.x + rect.width) && y >= rect.y && y <= (rect.y + rect.width);
	},

	isKeyDigit: function (keyCode) {
		/// <summary>
		/// Gets whether the supplied key-code is a digit
		/// </summary>
		/// <param name="keyCode" type="Number" integer="true">The key code of the event (from Sys.UI.DomEvent)</param>
		/// <returns type="Boolean" />

		return (0x30 <= keyCode && keyCode <= 0x39);
	},

	isKeyNavigation: function (keyCode) {
		/// <summary>
		/// Gets whether the supplied key-code is a navigation key
		/// </summary>
		/// <param name="keyCode" type="Number" integer="true">The key code of the event (from Sys.UI.DomEvent)</param>
		/// <returns type="Boolean" />

		return (Sys.UI.Key.left <= keyCode && keyCode <= Sys.UI.Key.down);
	},

	padLeft: function (text, size, ch, truncate) {
		/// <summary>
		/// Pads the left hand side of the supplied text with the specified pad character up to the requested size
		/// </summary>
		/// <param name="text" type="String">The text to pad</param>
		/// <param name="size" type="Number" integer="true" optional="true">The size to pad the text (default is 2)</param>
		/// <param name="ch" type="String" optional="true">The single character to use as the pad character (default is ' ')</param>
		/// <param name="truncate" type="Boolean" optional="true">Whether to truncate the text to size (default is false)</param>

		return this._pad(text, size || 2, ch || ' ', 'l', truncate || false);
	},

	padRight: function (text, size, ch, truncate) {
		/// <summary>
		/// Pads the right hand side of the supplied text with the specified pad character up to the requested size
		/// </summary>
		/// <param name="text" type="String">The text to pad</param>
		/// <param name="size" type="Number" integer="true" optional="true">The size to pad the text (default is 2)</param>
		/// <param name="ch" type="String" optional="true">The single character to use as the pad character (default is ' ')</param>
		/// <param name="truncate" type="Boolean" optional="true">Whether to truncate the text to size (default is false)</param>

		return this._pad(text, size || 2, ch || ' ', 'r', truncate || false);
	},

	_pad: function (text, size, ch, side, truncate) {
		/// <summary>
		/// Pads supplied text with the specified pad character up to the requested size
		/// </summary>
		/// <param name="text" type="String">The text to pad</param>
		/// <param name="size" type="Number" integer="true">The size to pad the text</param>
		/// <param name="ch" type="String">The single character to use as the pad character</param>
		/// <param name="side" type="String">Either 'l' or 'r' to siginfy whether to pad the Left or Right side respectively</param>
		/// <param name="truncate" type="Boolean">Whether to truncate the text to size</param>

		text = text.toString();
		var length = text.length;
		var builder = new Sys.StringBuilder();
		if (side == 'r') {
			builder.append(text);
		}
		while (length < size) {
			builder.append(ch);
			length++;
		}
		if (side == 'l') {
			builder.append(text);
		}
		var result = builder.toString();
		if (truncate && result.length > size) {
			if (side == 'l') {
				result = result.substr(result.length - size, size);
			} else {
				result = result.substr(0, size);
			}
		}
		return result;
	},


	wrapElement: function (innerElement, newOuterElement, newInnerParentElement) {
		/// <summary>
		/// Wraps an inner element with a new outer element at the same DOM location as the inner element
		/// </summary>
		/// <param name="innerElement" type="Sys.UI.DomElement">The element to be wrapped</param>
		/// <param name="newOuterElement" type="Sys.UI.DomElement">The new parent for the element</param>
		/// <returns />

		var parent = innerElement.parentNode;
		parent.replaceChild(newOuterElement, innerElement);
		(newInnerParentElement || newOuterElement).appendChild(innerElement);
	},

	unwrapElement: function (innerElement, oldOuterElement) {
		/// <summary>
		/// Unwraps an inner element from an outer element at the same DOM location as the outer element
		/// </summary>
		/// <param name="innerElement" type="Sys.UI.DomElement">The element to be wrapped</param>
		/// <param name="newOuterElement" type="Sys.UI.DomElement">The new parent for the element</param>
		/// <returns />

		var parent = oldOuterElement.parentNode;
		if (parent != null) {
			this.removeElement(innerElement);
			parent.replaceChild(innerElement, oldOuterElement);
		}
	},

	removeElement: function (element) {
		/// <summary>
		/// Removes an element from the DOM tree
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement">The element to be removed</param>
		/// <returns />

		var parent = element.parentNode;
		if (parent != null) {
			parent.removeChild(element);
		}
	},

	applyProperties: function (target, properties) {
		/// <summary>
		/// Quick utility method to copy properties from a template object to a target object
		/// </summary>
		/// <param name="target" type="Object">The object to apply to</param>
		/// <param name="properties" type="Object">The template to copy values from</param>

		for (var p in properties) {
			if (p == "__type")
				continue;
			var pv = properties[p];
			if (pv != null && Object.getType(pv) === Object) {
				var tv = target[p];
				this.applyProperties(tv, pv);
			} else {
				target[p] = pv;
			}
		}
	},

	createElementFromTemplate: function (template, appendToParent, nameTable, doc) {
		/// <summary>
		///	在当前document通过template对象创建一个DomElement
		/// Creates an element for the current document based on a template object
		/// </summary>
		/// <param name="template" type="Object">The template from which to create the element</param>
		/// <param name="appendToParent" type="Sys.UI.DomElement" optional="true" mayBeNull="true">A DomElement under which to append this element</param>
		/// <param name="nameTable" type="Object" optional="true" mayBeNull="true">An object to use as the storage for the element using template.name as the key</param>
		/// <returns type="Sys.UI.DomElement" />
		/// <remarks>
		/// This method is useful if you find yourself using the same or similar DomElement constructions throughout a class.  You can even set the templates
		/// as static properties for a type to cut down on overhead.  This method is often called with a JSON style template:
		/// <code>
		/// var elt = $HGDomElement.createElementFromTemplate({
		///     nodeName : "div",
		///     properties : {
		///         style : {
		///             height : "100px",
		///             width : "100px",
		///             backgroundColor : "white"
		///         },
		///         expandoAttribute : "foo"
		///     },
		///     events : {
		///         click : function() { alert("foo"); },
		///         mouseover : function() { elt.backgroundColor = "silver"; },
		///         mouseout : function() { elt.backgroundColor = "white"; }
		///     },
		///     cssClasses : [ "class0", "class1" ],
		///     visible : true,
		///     opacity : .5
		/// }, someParent);
		/// </code>
		/// </remarks>
		// if we wish to override the name table we do so here
		if (typeof (template.nameTable) != 'undefined') {
			var newNameTable = template.nameTable;
			if (String.isInstanceOfType(newNameTable)) {
				newNameTable = nameTable[newNameTable];
			}
			if (newNameTable != null) {
				nameTable = newNameTable;
			}
		}

		// get a name for the element in the nameTable
		var elementName = null;
		if (typeof (template.name) !== 'undefined') {
			elementName = template.name;
		}


		// if we wish to supply a default parent we do so here
		if (typeof (template.parent) !== 'undefined' && appendToParent == null) {
			var newParent = template.parent;
			if (String.isInstanceOfType(newParent)) {
				newParent = nameTable[newParent];
			}
			if (newParent != null) {
				appendToParent = newParent;
			}
		}

		// create or acquire the element
		var currentDocument = doc || this._currentDocument || document;
		var elt = this._createElement(currentDocument, template.nodeName, appendToParent);

		if (!elt.parentWindow)
			elt.parentWindow = currentDocument.parentWindow;

		// if our element is named, add it to the name table
		if (typeof (template.name) !== 'undefined' && nameTable) {
			nameTable[template.name] = elt;
		}

		// properties are applied as expando values to the element
		if (typeof (template.properties) !== 'undefined' && template.properties != null) {
			this.applyProperties(elt, template.properties);
		}

		// css classes are added to the element's className property
		if (typeof (template.cssClasses) !== 'undefined' && template.cssClasses != null) {
			this.addCssClasses(elt, template.cssClasses);
		}

		// events are added to the dom element using $addHandlers
		if (typeof (template.events) !== 'undefined' && template.events != null) {
			$addHandlers(elt, template.events);
		}

		// if the element is visible or not its visibility is set
		if (typeof (template.visible) !== 'undefined' && template.visible != null) {
			Sys.UI.DomElement.setVisible(elt, template.visible);
		}

		// if we have an appendToParent we will now append to it
		if (appendToParent && !elt.parentElement) {
			appendToParent.appendChild(elt);
		}

		// if we have opacity, apply it
		if (typeof (template.opacity) !== 'undefined' && template.opacity != null) {
			this.setElementOpacity(elt, template.opacity);
		}

		// if we have child templates, process them
		if (typeof (template.children) !== 'undefined' && template.children != null) {
			for (var i = 0; i < template.children.length; i++) {
				var subtemplate = template.children[i];
				this.createElementFromTemplate(subtemplate, elt, nameTable);
			}
		}

		// if we have a content presenter for the element get it (the element itself is the default presenter for content)
		var contentPresenter = elt;
		if (typeof (template.contentPresenter) !== 'undefined' && template.contentPresenter != null) {
			contentPresenter = nameTable[contentPresenter];
		}

		// if we have content, add it
		if (typeof (template.content) !== 'undefined' && template.content != null) {
			var content = template.content;
			if (String.isInstanceOfType(content)) {
				content = nameTable[content];
			}
			if (content.parentNode) {
				this.wrapElement(content, elt, contentPresenter);
			} else {
				contentPresenter.appendChild(content);
			}
		}

		// return the created element
		return elt;
	},

	_createElement: function (doc, nodeName, appendToParent) {
		var elt = null;
		if (appendToParent) {
			var parentNodeName = appendToParent.tagName.toLowerCase();
			nodeName = nodeName.toLowerCase();
			switch (parentNodeName) {
				case "table":
					if (nodeName == "tr") elt = appendToParent.insertRow(-1);
					else if (nodeName == "caption") elt = appendToParent.createCaption();
					else if (nodeName == "thead") elt = appendToParent.createTHead();
					else if (nodeName == "tfoot") elt = appendToParent.createTFoot();
					break;

				case "thead":
				case "tbody":
				case "tfoot":
					if (nodeName == "tr") elt = appendToParent.insertRow(-1);
					break;

				case "tr":
					if (nodeName == "td") elt = appendToParent.insertCell(-1);
					break;
			}
		}

		if (elt == null) elt = doc.createElement(nodeName);

		return elt;
	},

	addSelectOption: function (select, text, value, doc) {
		var oOption = (doc || document).createElement("option");
		oOption.text = text;
		oOption.value = value;
		select.options.add(oOption);

		return oOption;
	},

	createTextNode: function (text, appendToParent, doc) {
		var currentDocument = doc || this._currentDocument || document;
		var node = currentDocument.createTextNode(text);
		if (appendToParent)
			appendToParent.appendChild(node);

		return node;
	},

	copyCss: function (srcDoc, targetDoc) {
		for (var i = 0; i < srcDoc.styleSheets.length; i++) {
			var srcCss = srcDoc.styleSheets[i];
			var targetCss = targetDoc.createStyleSheet(srcCss.href);
			if (!srcCss.href)
				targetCss.cssText = srcCss.cssText;
		}
	},

	isDescendant: function (ancestor, descendant) {
		/// <summary>
		/// Whether the specified element is a descendant of the ancestor
		/// </summary>
		/// <param name="ancestor" type="Sys.UI.DomElement">Ancestor node</param>
		/// <param name="descendant" type="Sys.UI.DomElement">Possible descendant node</param>
		/// <returns type="Boolean" />

		for (var n = descendant.parentNode; n != null; n = n.parentNode) {
			if (n == ancestor) return true;
		}
		return false;
	},
	isDescendantOrSelf: function (ancestor, descendant) {
		/// <summary>
		/// Whether the specified element is a descendant of the ancestor or the same as the ancestor
		/// </summary>
		/// <param name="ancestor" type="Sys.UI.DomElement">Ancestor node</param>
		/// <param name="descendant" type="Sys.UI.DomElement">Possible descendant node</param>
		/// <returns type="Boolean" />

		if (ancestor === descendant)
			return true;
		return this.isDescendant(ancestor, descendant);
	},
	isAncestor: function (descendant, ancestor) {
		/// <summary>
		/// Whether the specified element is an ancestor of the descendant
		/// </summary>
		/// <param name="descendant" type="Sys.UI.DomElement">Descendant node</param>
		/// <param name="ancestor" type="Sys.UI.DomElement">Possible ancestor node</param>
		/// <returns type="Boolean" />

		return this.isDescendant(ancestor, descendant);
	},
	isAncestorOrSelf: function (descendant, ancestor) {
		/// <summary>
		/// Whether the specified element is an ancestor of the descendant or the same as the descendant
		/// </summary>
		/// <param name="descendant" type="Sys.UI.DomElement">Descendant node</param>
		/// <param name="ancestor" type="Sys.UI.DomElement">Possible ancestor node</param>
		/// <returns type="Boolean" />

		if (descendant === ancestor)
			return true;

		return this.isDescendant(ancestor, descendant);
	},
	isSibling: function (self, sibling) {
		/// <summary>
		/// Whether the specified element is a sibling of the self element
		/// </summary>
		/// <param name="self" type="Sys.UI.DomElement">Self node</param>
		/// <param name="sibling" type="Sys.UI.DomElement">Possible sibling node</param>
		/// <returns type="Boolean" />
		///	为什么不通过判断是否有同一个父对象来却确定？？
		var parent = self.parentNode;
		for (var i = 0; i < parent.childNodes.length; i++) {
			if (parent.childNodes[i] == sibling) return true;
		}
		return false;
	},

	clearChildren: function (elt) {
		while (elt.childNodes.length > 0)
			elt.removeChild(elt.childNodes[0])
	},

	ensureOneChild: function (parent, child) {
		if (!(child.parentElement == parent && parent.childNodes.length == 1)) {
			this.clearChildren(parent);
			parent.appendChild(child);
		}
	}
}
// Create the singleton instance of the $HGDomElement
var $HGDomElement = $HGRootNS.DomElement = new $HGRootNS._DomElement();

$HGRootNS._Error = function () {
}
$HGRootNS._Error.prototype =
{
	throwError: function (err) {
		throw Error.create("", err);
	}
}
$HGRootNS._Error.registerClass($HGRootNSName + "._Error");
var $HGError = $HGRootNS.Error = new $HGRootNS._Error();


$HGRootNS._DomEvent = function () {
}
$HGRootNS._DomEvent.prototype =
{
	__DOMEvents: {
		focusin: { eventGroup: "UIEvents", init: function (e, p) { e.initUIEvent("focusin", true, false, window, 1); } },
		focusout: { eventGroup: "UIEvents", init: function (e, p) { e.initUIEvent("focusout", true, false, window, 1); } },
		activate: { eventGroup: "UIEvents", init: function (e, p) { e.initUIEvent("activate", true, true, window, 1); } },
		focus: { eventGroup: "UIEvents", init: function (e, p) { e.initUIEvent("focus", false, false, window, 1); } },
		blur: { eventGroup: "UIEvents", init: function (e, p) { e.initUIEvent("blur", false, false, window, 1); } },
		click: { eventGroup: "MouseEvents", init: function (e, p) { e.initMouseEvent("click", true, true, window, 1, p.screenX || 0, p.screenY || 0, p.clientX || 0, p.clientY || 0, p.ctrlKey || false, p.altKey || false, p.shiftKey || false, p.metaKey || false, p.button || 0, p.relatedTarget || null); } },
		dblclick: { eventGroup: "MouseEvents", init: function (e, p) { e.initMouseEvent("click", true, true, window, 2, p.screenX || 0, p.screenY || 0, p.clientX || 0, p.clientY || 0, p.ctrlKey || false, p.altKey || false, p.shiftKey || false, p.metaKey || false, p.button || 0, p.relatedTarget || null); } },
		mousedown: { eventGroup: "MouseEvents", init: function (e, p) { e.initMouseEvent("mousedown", true, true, window, 1, p.screenX || 0, p.screenY || 0, p.clientX || 0, p.clientY || 0, p.ctrlKey || false, p.altKey || false, p.shiftKey || false, p.metaKey || false, p.button || 0, p.relatedTarget || null); } },
		mouseup: { eventGroup: "MouseEvents", init: function (e, p) { e.initMouseEvent("mouseup", true, true, window, 1, p.screenX || 0, p.screenY || 0, p.clientX || 0, p.clientY || 0, p.ctrlKey || false, p.altKey || false, p.shiftKey || false, p.metaKey || false, p.button || 0, p.relatedTarget || null); } },
		mouseover: { eventGroup: "MouseEvents", init: function (e, p) { e.initMouseEvent("mouseover", true, true, window, 1, p.screenX || 0, p.screenY || 0, p.clientX || 0, p.clientY || 0, p.ctrlKey || false, p.altKey || false, p.shiftKey || false, p.metaKey || false, p.button || 0, p.relatedTarget || null); } },
		mousemove: { eventGroup: "MouseEvents", init: function (e, p) { e.initMouseEvent("mousemove", true, true, window, 1, p.screenX || 0, p.screenY || 0, p.clientX || 0, p.clientY || 0, p.ctrlKey || false, p.altKey || false, p.shiftKey || false, p.metaKey || false, p.button || 0, p.relatedTarget || null); } },
		mouseout: { eventGroup: "MouseEvents", init: function (e, p) { e.initMouseEvent("mousemove", true, true, window, 1, p.screenX || 0, p.screenY || 0, p.clientX || 0, p.clientY || 0, p.ctrlKey || false, p.altKey || false, p.shiftKey || false, p.metaKey || false, p.button || 0, p.relatedTarget || null); } },
		load: { eventGroup: "HTMLEvents", init: function (e, p) { e.initMouseEvent("load", false, false); } },
		unload: { eventGroup: "HTMLEvents", init: function (e, p) { e.initMouseEvent("unload", false, false); } },
		select: { eventGroup: "HTMLEvents", init: function (e, p) { e.initMouseEvent("select", true, false); } },
		change: { eventGroup: "HTMLEvents", init: function (e, p) { e.initMouseEvent("change", true, false); } },
		submit: { eventGroup: "HTMLEvents", init: function (e, p) { e.initMouseEvent("submit", true, true); } },
		reset: { eventGroup: "HTMLEvents", init: function (e, p) { e.initMouseEvent("reset", true, false); } },
		resize: { eventGroup: "HTMLEvents", init: function (e, p) { e.initMouseEvent("resize", true, false); } },
		scroll: { eventGroup: "HTMLEvents", init: function (e, p) { e.initMouseEvent("scroll", true, false); } }
	},

	tryFireEvent: function (element, eventName, properties) {
		/// <summary>
		/// Attempts to fire a DOM event on an element
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement">The element to fire the event</param>
		/// <param name="eventName" type="String">The name of the event to fire (without an 'on' prefix)</param>
		/// <param name="properties" type="Object">Properties to add to the event</param>
		/// <returns type="Boolean">True if the event was successfully fired, otherwise false</returns>

		try {
			if (document.createEventObject) {
				var e = document.createEventObject();
				$HGDomElement.applyProperties(e, properties || {});
				element.fireEvent("on" + eventName, e);
				return true;
			} else if (document.createEvent) {
				var def = this.__DOMEvents[eventName];
				if (def) {
					var e = document.createEvent(def.eventGroup);
					def.init(e, properties || {});
					element.dispatchEvent(e);
					return true;
				}
			}
		} catch (e) {
		}
		return false;
	},

	removeHandlers: function (element, events) {
		/// <summary>
		/// Removes a set of event handlers from an element
		/// </summary>
		/// <param name="element" type="Sys.UI.DomElement">The element to modify</param>
		/// <param name="events" type="Object">The template object that contains event names and delegates</param>
		/// <remarks>
		/// This is NOT the same as $clearHandlers which removes all delegates from a DomElement.  This rather removes select delegates 
		/// from a specified element and has a matching signature as $addHandlers
		/// </remarks>
		for (var name in events) {
			$removeHandler(element, name, events[name]);
		}
	}
}
$HGRootNS._DomEvent.registerClass($HGRootNSName + "._DomEvent");
var $HGDomEvent = $HGRootNS.DomEvent = new $HGRootNS._DomEvent();


$HGRootNS._Function = function () {
}
$HGRootNS._Function.prototype =
{
	resolveFunction: function (value) {
		/// <summary>
		/// Returns a function reference that corresponds to the provided value
		/// </summary>
		/// <param name="value" type="Object">
		/// The value can either be a Function, the name of a function (that can be found using window['name']),
		/// or an expression that evaluates to a function.
		/// </param>
		/// <returns type="Function">
		/// Reference to the function, or null if not found
		/// </returns>

		if (value) {
			if (value instanceof Function) {
				return value;
			} else if (String.isInstanceOfType(value) && value.length > 0) {
				var func;
				if ((func = window[value]) instanceof Function) {
					return func;
				} else if ((func = eval(value)) instanceof Function) {
					return func;
				}
			}
		}
		return null;
	},

	appendMethodCall: function (method, methodObj, call, callObj, callArgs) {
		var m = method;
		return function () {
			m.apply(methodObj, arguments);
			call.call(callObj, callArgs);
		}
	},

	insertMethodCall: function (method, methodObj, call, callObj, callArgs) {
		var m = method;
		return function () {
			call.call(callObj, callArgs);
			m.apply(methodObj, arguments);
		}
	}
}
$HGRootNS._Function.registerClass($HGRootNSName + "._Function");
var $HGFunction = $HGRootNS.Function = new $HGRootNS._Function();


$HGRootNS._Date = function () {
}
$HGRootNS._Date.prototype =
{
	//可将obj对象中"\/Date(1191950527452)\/"转换成时间类型，函数返回值未转换后的obj
	convertDate: function (obj) {
		var result = obj;
		if (Array.isInstanceOfType(obj)) {
			for (var i = 0; i < obj.length; i++) {
				obj[i] = this.convertDate(obj[i]);
			}
		}
		else if (typeof (obj) == "string") {
			if (obj.startsWith("/Date(") && obj.endsWith(")/")) {
				var d = eval("new " + obj.substring(1, obj.length - 1));
				if (Date.isInstanceOfType(d))
					result = d;
			}
		}
		else if (typeof (obj) == "object") {
			for (var pName in obj) {
				obj[pName] = this.convertDate(obj[pName]);
			}
		}

		return result;
	}
}
$HGRootNS._Date.registerClass($HGRootNSName + "._Date");
var $HGDate = $HGRootNS.Date = new $HGRootNS._Date();

// Temporary fix null reference bug in Sys.CultureInfo._getAbbrMonthIndex
if (Sys.CultureInfo.prototype._getAbbrMonthIndex) {
	try {
		Sys.CultureInfo.prototype._getAbbrMonthIndex('');
	} catch (ex) {
		Sys.CultureInfo.prototype._getAbbrMonthIndex = function (value) {
			if (!this._upperAbbrMonths) {
				this._upperAbbrMonths = this._toUpperArray(this.dateTimeFormat.AbbreviatedMonthNames);
			}
			return Array.indexOf(this._upperAbbrMonths, this._toUpper(value));
		}
		Sys.CultureInfo.CurrentCulture._getAbbrMonthIndex = Sys.CultureInfo.prototype._getAbbrMonthIndex;
		Sys.CultureInfo.InvariantCulture._getAbbrMonthIndex = Sys.CultureInfo.prototype._getAbbrMonthIndex;
	}
}
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

// This is the base behavior for all extender behaviors
$HGRootNS.BehaviorBase = function (element) {
	$HGRootNS.BehaviorBase.initializeBase(this, [element]);
	this._isInvoking = false;
	this._autoClearClientStateFieldValue = true;
	this._clientStateField = null;
	this._callbackTarget = null;
	this._onsubmit$delegate = Function.createDelegate(this, this._onsubmit);
	this._oncomplete$delegate = Function.createDelegate(this, this._oncomplete);
	this._onerror$delegate = Function.createDelegate(this, this._onerror);
	this._invokeWithoutViewState = true;
}
$HGRootNS.BehaviorBase.prototype = {
	initialize: function () {
		$HGRootNS.BehaviorBase.callBaseMethod(this, "initialize");
		// load the client state if possible
		if (this._clientStateField) {
			this.loadClientState(this._clientStateField.value);
			if (this._autoClearClientStateFieldValue)
				this._clientStateField.value = "";
		}
		// attach an event to save the client state before a postback or updatepanel partial postback
		if (typeof (Sys.WebForms) !== "undefined" && typeof (Sys.WebForms.PageRequestManager) !== "undefined") {
			Array.add(Sys.WebForms.PageRequestManager.getInstance()._onSubmitStatements, this._onsubmit$delegate);
		} else {
			$addHandler(document.forms[0], "submit", this._onsubmit$delegate);
		}

		var doPostBackMethod = window["__doPostBack"];

		if (doPostBackMethod)
			window["__doPostBack"] = $HGFunction.insertMethodCall(doPostBackMethod, window, this._onsubmit, this);
	},
	dispose: function () {
		if (typeof (Sys.WebForms) !== "undefined" && typeof (Sys.WebForms.PageRequestManager) !== "undefined") {
			Array.remove(Sys.WebForms.PageRequestManager.getInstance()._onSubmitStatements, this._onsubmit$delegate);
		} else {
			$removeHandler(document.forms[0], "submit", this._onsubmit$delegate);
		}
		$HGRootNS.BehaviorBase.callBaseMethod(this, "dispose");
	},
	findElement: function (id) {
		// <summary>Finds an element within this control (ScriptControl/ScriptUserControl are NamingContainers);
		return $get(this.get_id() + '_' + id.split(':').join('_'));
	},
	get_clientStateField: function () {
		return this._clientStateField;
	},
	set_clientStateField: function (value) {
		if (this.get_isInitialized()) throw Error.invalidOperation($HGRootNS.Resources.ExtenderBase_CannotSetClientStateField);
		this._clientStateField = value;
	},
	loadClientState: function (value) {
		/// <remarks>override this method to intercept client state loading after a callback</remarks>
	},
	saveClientState: function () {
		/// <remarks>override this method to intercept client state acquisition before a callback</remarks>
		return null;
	},
	get_invokeWithoutViewState: function () {
		return this._invokeWithoutViewState;
	},
	set_invokeWithoutViewState: function (value) {
		this._invokeWithoutViewState = value;
	},
	get_serverControlType: function () {
		return this._serverControlType;
	},
	set_serverControlType: function (value) {
		this._serverControlType = value;
	},
	get_staticCallBackProxyID: function () {
		return this._staticCallBackProxyID;
	},
	set_staticCallBackProxyID: function (value) {
		this._staticCallBackProxyID = value;
	},

	get_cloneableProperties: function () {
		var baseProperties = $HGRootNS.BehaviorBase.callBaseMethod(this, "get_cloneableProperties");
		var currentProperties = ["staticCallBackProxyID", "serverControlType", "invokeWithoutViewState"];
		Array.addRange(currentProperties, baseProperties);

		return currentProperties;
	},

	_invoke: function (name, args, cb, errCb, bSync) {
		this._internalInvoke(this._callbackTarget, name, args, cb, errCb, bSync);
	},

	_staticInvoke: function (name, args, cb, errCb, bSync) {
		this._internalInvoke(this.get_staticCallBackProxyID(), name, args, cb, errCb, bSync);
	},

	_internalInvoke: function (callbackTarget, name, args, cb, errCb, bSync) {
		this._isInvoking = true;
		try {
			if (!callbackTarget) {
				throw Error.invalidOperation($HGRootNS.Resources.ExtenderBase_ControlNotRegisteredForCallbacks);
			}
			if (typeof (WebForm_DoCallback) === "undefined") {
				throw Error.invalidOperation($HGRootNS.Resources.ExtenderBase_PageNotRegisteredForCallbacks);
			}
			var ar = [];
			for (var i = 0; i < args.length; i++)
				ar[i] = args[i];

			var clientState = this.saveClientState();
			if (clientState != null && !String.isInstanceOfType(clientState)) {
				throw Error.invalidOperation($HGRootNS.Resources.ExtenderBase_InvalidClientStateType);
			}

			var payload = Sys.Serialization.JavaScriptSerializer.serialize({ name: name, args: ar, state: clientState, serverControlType: this.get_serverControlType(), originalControlID: this._callbackTarget });
			WebForm_DoCallback(callbackTarget, payload, this._oncomplete$delegate, [cb, errCb], this._onerror$delegate, false, bSync, this._invokeWithoutViewState);
		}
		catch (e) {
			this._isInvoking = false;
			throw (e);
		}
		finally {
			this._isInvoking = false;
		}
	},

	_oncomplete: function (result, context) {
		try {
			var cb = context[0];
			var errCb = context[1];
			result = Sys.Serialization.JavaScriptSerializer.deserialize(result);
			if (result.error) {
				var err = Error.create(result.error.message, result.error);
				if (errCb) {
					errCb(err);
				}
				else
					throw err;
			}
			else {
				this.loadClientState(result.state);
				cb(result.result);
			}
		}
		catch (e) {
			this._isInvoking = false;
			throw (e);
		}
		finally {
			this._isInvoking = false;
		}
	},
	_onerror: function (message, context) {
		try {
			var err = Error.create(message);
			var errCb = context[1];
			if (errCb) {
				errCb(err);
			}
			else
				throw err;
		}
		catch (e) {
			this._isInvoking = false;
			throw (e);
		}
		finally {
			this._isInvoking = false;
		}
	},
	_onsubmit: function () {
		if (this._clientStateField) {
			this._clientStateField.value = this.saveClientState() || "";
		}
		return true;
	}
}
$HGRootNS.BehaviorBase.registerClass($HGRootNSName + ".BehaviorBase", Sys.UI.Behavior);

//Retrieve properties from a component
function Sys$Component$_getProperties(target, propertyNames) {
	var isComponent = Sys.Component.isInstanceOfType(target) && !target.get_isUpdating();
	var result = {};

	for (var i = 0; i < propertyNames.length; i++) {
		var name = propertyNames[i];
		var getter = target["get_" + name];

		if (typeof (getter) == 'function') {
			result[name] = getter.apply(target);
		}
	}

	return result;
}

function Sys$Component$_clone(source, properties, element) {
	var type = Object.getType(source);

	return $create(type, properties, source.get_eventProperties(), null, element);
}

Sys.Component.prototype.get_cloneableProperties = function () {
	return [];
}

Sys.Component.prototype._prepareCloneablePropertyValues = function (newElement) {
	return Sys$Component$_getProperties(this, this.get_cloneableProperties());
}

Sys.Component.prototype.get_eventProperties = function () {
	var events = {};

	for (var name in this.get_events()._list) {
		events[name] = this.get_events()._list[name][0];
	}

	return events;
}

Sys.Component.prototype.onBeforeCloneComponent = function (sourceElement) {
}

Sys.Component.prototype.onAfterCloneComponent = function (newElement, newComponent) {
}

Sys.Component.prototype.onBeforeCloneElement = function (sourceElement) {
}

Sys.Component.prototype.onAfterCloneElement = function (newElement) {
}

Sys.Component.prototype.cloneComponent = function (element) {
	var propertyNames = this.get_cloneableProperties();

	var result = null;

	this.onBeforeCloneComponent(element);

	try {
		var properties = this._prepareCloneablePropertyValues(element);
		result = Sys$Component$_clone(this, properties, element);

		if (typeof (this._callbackTarget) != "undefined")
			result._callbackTarget = this._callbackTarget;

		return result;
	}
	finally {
		this.onAfterCloneComponent(element, result);
	}
}

Sys.UI.Control.prototype.cloneAndAppendToContainer = function (container, target) {
	var newElement = target;

	if (typeof (newElement) == "undefined") {
		newElement = this.cloneElement();
		container.appendChild(newElement);
	}

	var result = this.cloneComponent(newElement);

	if (typeof (target) != "undefined")
		target.control = result;

	return result;
}

Sys.UI.Behavior.prototype.cloneAndAppendToContainer = function (container, target) {
	var newElement = target;

	if (typeof (newElement) == "undefined") {
		newElement = this.cloneElement();
		container.appendChild(newElement);
	}

	var result = this.cloneComponent(newElement);

	if (typeof (target) != "undefined")
		target.control = result;

	return result;
}

//Clone the element in the behavior
Sys.UI.Behavior.prototype.cloneElement = function () {
	var result = null;
	var sourceElement = this.get_element();

	if (sourceElement != null) {
		this.onBeforeCloneElement(sourceElement);

		try {
			result = sourceElement.cloneNode(false);
			result.id = result.uniqueID;
			result.control = undefined;
		}
		finally {
			this.onAfterCloneElement(sourceElement, result);
		}
	}

	return result;
}

//Clone the element in the control
Sys.UI.Control.prototype.cloneElement = function () {
	var result = null;
	var sourceElement = this.get_element();

	if (sourceElement != null) {
		this.onBeforeCloneElement(sourceElement);

		try {
			result = sourceElement.cloneNode(false);
			result.id = result.uniqueID;
			result.control = undefined;
		}
		finally {
			this.onAfterCloneElement(sourceElement, result);
		}
	}

	return result;
}

$HGRootNS.ControlBase = function (element) {
	$HGRootNS.ControlBase.initializeBase(this, [element]);
	this._isInvoking = false;
	this._autoClearClientStateFieldValue = true;
	this._clientStateField = null;
	this._callbackTarget = null;
	this._onsubmit$delegate = Function.createDelegate(this, this._onsubmit);
	this._oncomplete$delegate = Function.createDelegate(this, this._oncomplete);
	this._onerror$delegate = Function.createDelegate(this, this._onerror);
	this._invokeWithoutViewState = false;
	this._serverControlType = "";
	this._staticCallBackProxyID = "";
}

$HGRootNS.ControlBase.prototype = {
	initialize: function () {
		$HGRootNS.ControlBase.callBaseMethod(this, "initialize");
		// load the client state if possible
		if (this._clientStateField) {
			this.loadClientState(this._clientStateField.value);
			if (this._autoClearClientStateFieldValue)
				this._clientStateField.value = "";
		}
		// attach an event to save the client state before a postback or updatepanel partial postback
		if (typeof (Sys.WebForms) !== "undefined" && typeof (Sys.WebForms.PageRequestManager) !== "undefined") {
			Array.add(Sys.WebForms.PageRequestManager.getInstance()._onSubmitStatements, this._onsubmit$delegate);
		} else {
			$addHandler(document.forms[0], "submit", this._onsubmit$delegate);
		}

		var doPostBackMethod = window["__doPostBack"];
		if (doPostBackMethod)
			window["__doPostBack"] = $HGFunction.insertMethodCall(doPostBackMethod, window, this._onsubmit, this);
	},
	dispose: function () {
		if (typeof (Sys.WebForms) !== "undefined" && typeof (Sys.WebForms.PageRequestManager) !== "undefined") {
			Array.remove(Sys.WebForms.PageRequestManager.getInstance()._onSubmitStatements, this._onsubmit$delegate);
		} else {
			$removeHandler(document.forms[0], "submit", this._onsubmit$delegate);
		}
		$HGRootNS.ControlBase.callBaseMethod(this, "dispose");
	},
	findElement: function (id) {
		// <summary>Finds an element within this control (ScriptControl/ScriptUserControl are NamingContainers);
		return $get(this.get_id() + '_' + id.split(':').join('_'));
	},
	get_clientStateField: function () {
		return this._clientStateField;
	},
	set_clientStateField: function (value) {
		if (this.get_isInitialized()) throw Error.invalidOperation($HGRootNS.Resources.ExtenderBase_CannotSetClientStateField);
		this._clientStateField = value;
	},
	loadClientState: function (value) {
		/// <remarks>override this method to intercept client state loading after a callback</remarks>
	},
	saveClientState: function () {
		/// <remarks>override this method to intercept client state acquisition before a callback</remarks>
		return null;
	},
	get_invokeWithoutViewState: function () {
		return this._invokeWithoutViewState;
	},
	set_invokeWithoutViewState: function (value) {
		this._invokeWithoutViewState = value;
	},
	get_serverControlType: function () {
		return this._serverControlType;
	},
	set_serverControlType: function (value) {
		this._serverControlType = value;
	},
	get_staticCallBackProxyID: function () {
		return this._staticCallBackProxyID;
	},
	set_staticCallBackProxyID: function (value) {
		this._staticCallBackProxyID = value;
	},

	get_cloneableProperties: function () {
		var baseProperties = $HGRootNS.ControlBase.callBaseMethod(this, "get_cloneableProperties");
		var currentProperties = ["staticCallBackProxyID", "serverControlType", "invokeWithoutViewState"];
		Array.addRange(currentProperties, baseProperties);

		return currentProperties;
	},

	_invoke: function (name, args, cb, errCb, bSync) {
		this._internalInvoke(this._callbackTarget, name, args, cb, errCb, bSync);
	},

	_staticInvoke: function (name, args, cb, errCb, bSync) {
		this._internalInvoke(this.get_staticCallBackProxyID(), name, args, cb, errCb, bSync);
	},

	_internalInvoke: function (callbackTarget, name, args, cb, errCb, bSync) {
		this._isInvoking = true;
		try {
			if (!callbackTarget) {
				throw Error.invalidOperation($HGRootNS.Resources.ExtenderBase_ControlNotRegisteredForCallbacks);
			}
			if (typeof (WebForm_DoCallback) === "undefined") {
				throw Error.invalidOperation($HGRootNS.Resources.ExtenderBase_PageNotRegisteredForCallbacks);
			}

			var ar = [];

			for (var i = 0; i < args.length; i++)
				ar[i] = args[i];

			var clientState = this.saveClientState();

			if (clientState != null && !String.isInstanceOfType(clientState)) {
				throw Error.invalidOperation($HGRootNS.Resources.ExtenderBase_InvalidClientStateType);
			}

			var payload = Sys.Serialization.JavaScriptSerializer.serialize({ name: name, args: ar, state: clientState, serverControlType: this.get_serverControlType(), originalControlID: this._callbackTarget });
			WebForm_DoCallback(callbackTarget, payload, this._oncomplete$delegate, [cb, errCb], this._onerror$delegate, false, bSync, this._invokeWithoutViewState);
		}
		catch (e) {
			this._isInvoking = false;
			throw (e);
		}
		finally {
			this._isInvoking = false;
		}
	},

	_oncomplete: function (strResult, context) {
		try {
			var cb = context[0];
			var errCb = context[1];
			var result = Sys.Serialization.JavaScriptSerializer.deserialize(strResult);
			if (result.error) {
				var err = Error.create(result.error.message, result.error);
				if (errCb) {
					errCb(err);
				}
				else
					throw err;
			}
			else {
				this.loadClientState(result.state);
				cb(result.result);
			}
		}
		catch (e) {
			this._isInvoking = false;
			throw (e);
		}
		finally {
			this._isInvoking = false;
		}
	},
	_onerror: function (message, context) {
		try {
			var err = Error.create(message);
			var errCb = context[1];
			if (errCb) {
				errCb(err);
			}
			else
				throw err;
		}
		catch (e) {
			this._isInvoking = false;
			throw (e);
		}
		finally {
			this._isInvoking = false;
		}
	},
	_onsubmit: function () {
		if (this._clientStateField) {
			this._clientStateField.value = this.saveClientState() || "";
		}
		return true;
	}
}
$HGRootNS.ControlBase.registerClass($HGRootNSName + ".ControlBase", Sys.UI.Control);

// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.
//var $HGRootNSName = 'MCS.Web.Library.Script';
//Type.registerNamespace($HGRootNSName);
//var $HGRootNS = eval($HGRootNSName);

$HGRootNS.TimeSpan = function () {
	/// <summary>
	/// Represents a period of time
	/// </summary>

	if (arguments.length == 0) this._ctor$0.apply(this, arguments);
	else if (arguments.length == 1) this._ctor$1.apply(this, arguments);
	else if (arguments.length == 3) this._ctor$2.apply(this, arguments);
	else if (arguments.length == 4) this._ctor$3.apply(this, arguments);
	else if (arguments.length == 5) this._ctor$4.apply(this, arguments);
	else throw Error.parameterCount();
}
$HGRootNS.TimeSpan.prototype = {

	_ctor$0: function () {
		/// <summary>
		/// Initializes a new TimeSpan
		/// </summary>

		this._ticks = 0;
	},
	_ctor$1: function (ticks) {
		/// <summary>
		/// Initializes a new TimeSpan
		/// </summary>
		/// <param name="ticks" type="Number" integer="true">The number of ticks in the TimeSpan</param>

		this._ctor$0();
		this._ticks = ticks;
	},
	_ctor$2: function (hours, minutes, seconds) {
		/// <summary>
		/// Initializes a new TimeSpan
		/// </summary>
		/// <param name="hours" type="Number">The number of hours in the TimeSpan</param>
		/// <param name="minutes" type="Number">The number of minutes in the TimeSpan</param>
		/// <param name="seconds" type="Number">The number of seconds in the TimeSpan</param>

		this._ctor$0();
		this._ticks =
            (hours * $HGRootNS.TimeSpan.TicksPerHour) +
            (minutes * $HGRootNS.TimeSpan.TicksPerMinute) +
            (seconds * $HGRootNS.TimeSpan.TicksPerSecond);
	},
	_ctor$3: function (days, hours, minutes, seconds) {
		/// <summary>
		/// Initializes a new TimeSpan
		/// </summary>
		/// <param name="days" type="Number">The number of days in the TimeSpan</param>
		/// <param name="hours" type="Number">The number of hours in the TimeSpan</param>
		/// <param name="minutes" type="Number">The number of minutes in the TimeSpan</param>
		/// <param name="seconds" type="Number">The number of seconds in the TimeSpan</param>

		this._ctor$0();
		this._ticks =
            (days * $HGRootNS.TimeSpan.TicksPerDay) +
            (hours * $HGRootNS.TimeSpan.TicksPerHour) +
            (minutes * $HGRootNS.TimeSpan.TicksPerMinute) +
            (seconds * $HGRootNS.TimeSpan.TicksPerSecond);
	},
	_ctor$4: function (days, hours, minutes, seconds, milliseconds) {
		/// <summary>
		/// Initializes a new TimeSpan
		/// </summary>
		/// <param name="days" type="Number">The number of days in the TimeSpan</param>
		/// <param name="hours" type="Number">The number of hours in the TimeSpan</param>
		/// <param name="minutes" type="Number">The number of minutes in the TimeSpan</param>
		/// <param name="seconds" type="Number">The number of seconds in the TimeSpan</param>
		/// <param name="milliseconds" type="Number">The number of milliseconds in the TimeSpan</param>

		this._ctor$0();
		this._ticks =
            (days * $HGRootNS.TimeSpan.TicksPerDay) +
            (hours * $HGRootNS.TimeSpan.TicksPerHour) +
            (minutes * $HGRootNS.TimeSpan.TicksPerMinute) +
            (seconds * $HGRootNS.TimeSpan.TicksPerSecond) +
            (milliseconds * $HGRootNS.TimeSpan.TicksPerMillisecond);
	},

	getDays: function () {
		/// <summary>
		/// Gets the days part of the TimeSpan
		/// </summary>
		/// <returns type="Number" />

		return Math.floor(this._ticks / $HGRootNS.TimeSpan.TicksPerDay);
	},
	getHours: function () {
		/// <summary>
		/// Gets the hours part of the TimeSpan
		/// </summary>
		/// <returns type="Number" />

		return Math.floor(this._ticks / $HGRootNS.TimeSpan.TicksPerHour) % 24;
	},
	getMinutes: function () {
		/// <summary>
		/// Gets the minutes part of the TimeSpan
		/// </summary>
		/// <returns type="Number" />

		return Math.floor(this._ticks / $HGRootNS.TimeSpan.TicksPerMinute) % 60;
	},
	getSeconds: function () {
		/// <summary>
		/// Gets the seconds part of the TimeSpan
		/// </summary>
		/// <returns type="Number" />

		return Math.floor(this._ticks / $HGRootNS.TimeSpan.TicksPerSecond) % 60;
	},
	getMilliseconds: function () {
		/// <summary>
		/// Gets the milliseconds part of the TimeSpan
		/// </summary>
		/// <returns type="Number" />

		return Math.floor(this._ticks / $HGRootNS.TimeSpan.TicksPerMillisecond) % 1000;
	},
	getDuration: function () {
		/// <summary>
		/// Gets the total duration of a TimeSpan
		/// </summary>
		/// <returns type=$HGRootNSName + ".TimeSpan" />

		return new $HGRootNS.TimeSpan(Math.abs(this._ticks));
	},
	getTicks: function () {
		/// <summary>
		/// Gets the ticks in the TimeSpan
		/// </summary>
		/// <returns type="Number" />

		return this._ticks;
	},
	getTotalDays: function () {
		/// <summary>
		/// Gets the total number of days in the TimeSpan
		/// </summary>
		/// <returns type="Number" />

		Math.floor(this._ticks / $HGRootNS.TimeSpan.TicksPerDay);
	},
	getTotalHours: function () {
		/// <summary>
		/// Gets the total hours in the TimeSpan
		/// </summary>
		/// <returns type="Number" />

		return Math.floor(this._ticks / $HGRootNS.TimeSpan.TicksPerHour);
	},
	getTotalMinutes: function () {
		/// <summary>
		/// Gets the total minutes in the TimeSpan
		/// </summary>
		/// <returns type="Number" />

		return Math.floor(this._ticks / $HGRootNS.TimeSpan.TicksPerMinute);
	},
	getTotalSeconds: function () {
		/// <summary>
		/// Gets the total seconds in the TimeSpan
		/// </summary>
		/// <returns type="Number" />

		return Math.floor(this._ticks / $HGRootNS.TimeSpan.TicksPerSecond);
	},
	getTotalMilliseconds: function () {
		/// <summary>
		/// Gets the total milliseconds in the TimeSpan
		/// </summary>
		/// <returns type="Number" />

		return Math.floor(this._ticks / $HGRootNS.TimeSpan.TicksPerMillisecond);
	},
	add: function (value) {
		/// <summary>
		/// Adds the supplied TimeSpan to this TimeSpan
		/// </summary>
		/// <param name="value" type=$HGRootNSName + ".TimeSpan">The TimeSpan to add</param>
		/// <returns type=$HGRootNSName + ".TimeSpan" />

		return new $HGRootNS.TimeSpan(this._ticks + value.getTicks());
	},
	subtract: function (value) {
		/// <summary>
		/// Subtracts the supplied TimeSpan to this TimeSpan
		/// </summary>
		/// <param name="value" type=$HGRootNSName + ".TimeSpan">The TimeSpan to subtract</param>
		/// <returns type=$HGRootNSName + ".TimeSpan" />

		return new $HGRootNS.TimeSpan(this._ticks - value.getTicks());
	},
	negate: function () {
		/// <summary>
		/// Negates the TimeSpan
		/// </summary>
		/// <returns type=$HGRootNSName + ".TimeSpan" />

		return new $HGRootNS.TimeSpan(-this._ticks);
	},
	equals: function (value) {
		/// <summary>
		/// Whether this TimeSpan equals another TimeSpan
		/// </summary>
		/// <param name="value" type=$HGRootNSName + ".TimeSpan">The TimeSpan to test</param>
		/// <returns type=$HGRootNSName + ".TimeSpan" />

		return this._ticks == value.getTicks();
	},
	compareTo: function (value) {
		/// <summary>
		/// Whether this TimeSpan greater or less than another TimeSpan
		/// </summary>
		/// <param name="value" type=$HGRootNSName + ".TimeSpan">The TimeSpan to test</param>
		/// <returns type=$HGRootNSName + ".TimeSpan" />

		if (this._ticks > value.getTicks())
			return 1;
		else if (this._ticks < value.getTicks())
			return -1;
		else
			return 0;
	},
	toString: function () {
		/// <summary>
		/// Gets the string representation of the TimeSpan
		/// </summary>
		/// <returns type="String" />

		return this.format("F");
	},
	format: function (format) {
		/// <summary>
		/// Gets the string representation of the TimeSpan
		/// </summary>
		/// <param name="format" type="String" mayBeNull="true">The format specifier used to format the TimeSpan</param>
		/// <returns type="String" />

		if (!format) {
			format = "F";
		}
		if (format.length == 1) {
			switch (format) {
				case "t": format = $HGRootNS.TimeSpan.ShortTimeSpanPattern; break;
				case "T": format = $HGRootNS.TimeSpan.LongTimeSpanPattern; break;
				case "F": format = $HGRootNS.TimeSpan.FullTimeSpanPattern; break;
				default: throw Error.createError(String.format($HGRootNS.Resources.Common_DateTime_InvalidTimeSpan, format));
			}
		}
		var regex = /dd|d|hh|h|mm|m|ss|s|nnnn|nnn|nn|n/g;
		var builder = new Sys.StringBuilder();
		var ticks = this._ticks;
		if (ticks < 0) {
			builder.append("-");
			ticks = -ticks;
		}
		for (; ; ) {
			var index = regex.lastIndex;
			var ar = regex.exec(format);
			builder.append(format.slice(index, ar ? ar.index : format.length));
			if (!ar) break;
			switch (ar[0]) {
				case "dd":
				case "d":
					builder.append($HGDomElement.padLeft(Math.floor(ticks / $HGRootNS.TimeSpan.TicksPerDay, ar[0].length, '0')));
					break;
				case "hh":
				case "h":
					builder.append($HGDomElement.padLeft(Math.floor(ticks / $HGRootNS.TimeSpan.TicksPerHour) % 24, ar[0].length, '0'));
					break;
				case "mm":
				case "m":
					builder.append($HGDomElement.padLeft(Math.floor(ticks / $HGRootNS.TimeSpan.TicksPerMinute) % 60, ar[0].length, '0'));
					break;
				case "ss":
				case "s":
					builder.append($HGDomElement.padLeft(Math.floor(ticks / $HGRootNS.TimeSpan.TicksPerSecond) % 60, ar[0].length, '0'));
					break;
				case "nnnn":
				case "nnn":
				case "nn":
				case "n":
					builder.append($HGDomElement.padRight(Math.floor(ticks / $HGRootNS.TimeSpan.TicksPerMillisecond) % 1000, ar[0].length, '0', true));
					break;
				default:
					Sys.Debug.assert(false);
			}
		}
		return builder.toString();
	}
}
$HGRootNS.TimeSpan.parse = function (text) {
	/// <summary>
	/// Parses a text value into a TimeSpan
	/// </summary>
	/// <param name="text" type="String">The text to parse</param>
	/// <returns type=$HGRootNSName + ".TimeSpan" />

	var parts = text.split(":");
	var d = 0;
	var h = 0;
	var m = 0;
	var s = 0;
	var n = 0;
	var ticks = 0;
	switch (parts.length) {
		case 1:
			if (parts[0].indexOf(".") != -1) {
				var parts2 = parts[0].split(".");
				s = parseInt(parts2[0]);
				n = parseInt(parts2[1]);
			} else {
				ticks = parseInt(parts[0]);
			}
			break;
		case 2:
			h = parseInt(parts[0]);
			m = parseInt(parts[1]);
			break;
		case 3:
			h = parseInt(parts[0]);
			m = parseInt(parts[1]);
			if (parts[2].indexOf(".") != -1) {
				var parts2 = parts[2].split(".");
				s = parseInt(parts2[0]);
				n = parseInt(parts2[1]);
			} else {
				s = parseInt(parts[2]);
			}
			break;
		case 4:
			d = parseInt(parts[0]);
			h = parseInt(parts[1]);
			m = parseInt(parts[2]);
			if (parts[3].indexOf(".") != -1) {
				var parts2 = parts[3].split(".");
				s = parseInt(parts2[0]);
				n = parseInt(parts2[1]);
			} else {
				s = parseInt(parts[3]);
			}
			break;
	}
	ticks += (d * $HGRootNS.TimeSpan.TicksPerDay) +
             (h * $HGRootNS.TimeSpan.TicksPerHour) +
             (m * $HGRootNS.TimeSpan.TicksPerMinute) +
             (s * $HGRootNS.TimeSpan.TicksPerSecond) +
             (n * $HGRootNS.TimeSpan.TicksPerMillisecond);
	if (!isNaN(ticks)) {
		return new $HGRootNS.TimeSpan(ticks);
	}
	throw Error.create($HGRootNS.Resources.Common_DateTime_InvalidFormat);
}
$HGRootNS.TimeSpan.fromTicks = function (ticks) {
	/// <summary>
	/// Creates a TimeSpan for the specified number of ticks
	/// </summary>
	/// <param name="ticks" type="Number" integer="true">The ticks for the TimeSpan instance</param>
	/// <returns type=$HGRootNSName + ".TimeSpan" />

	return new $HGRootNS.TimeSpan(ticks);
}
$HGRootNS.TimeSpan.fromDays = function (days) {
	/// <summary>
	/// Creates a TimeSpan for the specified number of days
	/// </summary>
	/// <param name="days" type="Number">The days for the TimeSpan instance</param>
	/// <returns type=$HGRootNSName + ".TimeSpan" />

	return new $HGRootNS.TimeSpan(days * $HGRootNS.TimeSpan.TicksPerDay);
}
$HGRootNS.TimeSpan.fromHours = function (hours) {
	/// <summary>
	/// Creates a TimeSpan for the specified number of hours
	/// </summary>
	/// <param name="hours" type="Number">The hours for the TimeSpan instance</param>
	/// <returns type=$HGRootNSName + ".TimeSpan" />

	return new $HGRootNS.TimeSpan(hours * $HGRootNS.TimeSpan.TicksPerHour);
}
$HGRootNS.TimeSpan.fromMinutes = function (minutes) {
	/// <summary>
	/// Creates a TimeSpan for the specified number of minutes
	/// </summary>
	/// <param name="minutes" type="Number">The minutes for the TimeSpan instance</param>
	/// <returns type=$HGRootNSName + ".TimeSpan" />

	return new $HGRootNS.TimeSpan(minutes * $HGRootNS.TimeSpan.TicksPerMinute);
}
$HGRootNS.TimeSpan.fromSeconds = function (seconds) {
	/// <summary>
	/// Creates a TimeSpan for the specified number of seconds
	/// </summary>
	/// <param name="seconds" type="Number">The seconds for the TimeSpan instance</param>
	/// <returns type=$HGRootNSName + ".TimeSpan" />

	return new $HGRootNS.TimeSpan(minutes * $HGRootNS.TimeSpan.TicksPerSecond);
}
$HGRootNS.TimeSpan.fromMilliseconds = function (milliseconds) {
	/// <summary>
	/// Creates a TimeSpan for the specified number of milliseconds
	/// </summary>
	/// <param name="days" type="Number">The milliseconds for the TimeSpan instance</param>
	/// <returns type=$HGRootNSName + ".TimeSpan" />

	return new $HGRootNS.TimeSpan(minutes * $HGRootNS.TimeSpan.TicksPerMillisecond);
}
$HGRootNS.TimeSpan.TicksPerDay = 864000000000;
$HGRootNS.TimeSpan.TicksPerHour = 36000000000;
$HGRootNS.TimeSpan.TicksPerMinute = 600000000;
$HGRootNS.TimeSpan.TicksPerSecond = 10000000;
$HGRootNS.TimeSpan.TicksPerMillisecond = 10000;
$HGRootNS.TimeSpan.FullTimeSpanPattern = "dd:hh:mm:ss.nnnn";
$HGRootNS.TimeSpan.ShortTimeSpanPattern = "hh:mm";
$HGRootNS.TimeSpan.LongTimeSpanPattern = "hh:mm:ss";

Date.prototype.getTimeOfDay = function Date$getTimeOfDay() {
	/// <summary>
	/// Gets a TimeSpan representing the current time of the Date
	/// </summary>
	/// <returns type=$HGRootNSName + ".TimeSpan" />

	return new $HGRootNS.TimeSpan(
        0,
        this.getHours(),
        this.getMinutes(),
        this.getSeconds(),
        this.getMilliseconds());
}
Date.prototype.getDateOnly = function Date$getDateOnly() {
	/// <summary>
	/// Gets a Date representing the Date only part of the Date
	/// </summary>
	/// <returns type="Date" />

	return new Date(this.getFullYear(), this.getMonth(), this.getDate());
}
Date.prototype.add = function Date$add(span) {
	/// <summary>
	/// Adds a TimeSpan to the current Date
	/// </summary>
	/// <param name="span" type=$HGRootNSName + ".TimeSpan">The amount of time to add to the date</param>
	/// <returns type="Date" />

	return new Date(this.getTime() + span.getTotalMilliseconds());
}
Date.prototype.subtract = function Date$subtract(span) {
	/// <summary>
	/// Subtracts a TimeSpan to the current Date
	/// </summary>
	/// <param name="span" type=$HGRootNSName + ".TimeSpan">The amount of time to subtract from the date</param>
	/// <returns type="Date" />

	return this.add(span.negate());
}
Date.prototype.getTicks = function Date$getTicks() {
	/// <summary>
	/// Gets the number of ticks in the date
	/// </summary>
	/// <returns type="Number" />

	return this.getTime() * $HGRootNS.TimeSpan.TicksPerMillisecond;
}
Date.minDate = new Date(-62135596800000);

Date.isMinDate = function (date) {
	return date * 1 == Date.minDate * 1;
}

$HGRootNS.FirstDayOfWeek = function () {
	/// <summary>
	/// Represents the first day of the week in a calendar
	/// </summary>
}
$HGRootNS.FirstDayOfWeek.prototype = {
	Sunday: 0,
	Monday: 1,
	Tuesday: 2,
	Wednesday: 3,
	Thursday: 4,
	Friday: 5,
	Saturday: 6,
	Default: 7
}
$HGRootNS.FirstDayOfWeek.registerEnum($HGRootNSName + ".FirstDayOfWeek");

// -------------------------------------------------
// FileName	：	PopupControl.js
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    周维海	    20070403		创建
// -------------------------------------------------


$HGRootNS.DisposePopupWindow = function () {
	$HGRootNS.DisposePopupWindow.initializeBase(this);
}
$HGRootNS.DisposePopupWindow.prototype =
{
	initialize: function () {
		$HGRootNS.DisposePopupWindow.callBaseMethod(this, 'initialize');
	},

	dispose: function () {
		if ($HGRootNS.childWindows) {
			for (var i = 0; i < $HGRootNS.childWindows.length; i++) {
				$HGRootNS.childWindows[i] = null;
			}
			$HGRootNS.childWindows = null;
		}
		$HGRootNS.DisposePopupWindow.callBaseMethod(this, 'dispose');
	}
}
$HGRootNS.DisposePopupWindow.registerClass($HGRootNSName + ".DisposePopupWindow", Sys.Component);

$HGRootNS.childWindows = [];
$HGRootNS.addChildWindow = function (win) {
	Array.add($HGRootNS.childWindows, win);
}

Sys.Application.add_unload(function () { $create($HGRootNS.DisposePopupWindow, {}, null, null, null); });

$HGRootNS.PopupMode = function () {
	throw Error.invalidOperation();
}
$HGRootNS.PopupMode.prototype =
{
	Popup: 0,
	Dialog: 1
}
$HGRootNS.PopupMode.registerEnum($HGRootNSName + '.PopupMode');

$HGRootNS.PositioningMode = function () {
	throw Error.invalidOperation();
}
$HGRootNS.PositioningMode.prototype = {
	Absolute: 0,
	Center: 1,
	BottomLeft: 2,
	BottomRight: 3,
	TopLeft: 4,
	TopRight: 5,
	LeftBottom: 6,
	LeftTop: 7,
	RigthBottom: 8,
	RightTop: 9,
	RelativeTopLeft: 10
}
$HGRootNS.PositioningMode.registerEnum($HGRootNSName + '.PositioningMode');

/// <summary>
/// Gets the "border-box" size of an element.
/// </summary>
$HGRootNS.PopupControlBase = function () {
	/// <param name="element">The DOM element the behavior is associated with.</param>
	$HGRootNS.PopupControlBase.initializeBase(this);

	this._x = 0;
	this._y = 0;
	this._width = 0;
	this._height = 0;
	this._positioningMode = $HGRootNS.PositioningMode.Absolute;
	this._positionElement = null;
	this._positionElementID = null;
	this._popupWindow = null;
	this._body = null;
	this._parent = null;
	this._style = null;
	this._applyFilter = true;
	this._usePublicPopupWindow = true;
	this._cancelSelectDelegate = Function.createDelegate(this, this._cancelSelect);
	this._bodyEvents =
    {
    	contextmenu: this._cancelSelectDelegate,
    	selectstart: this._cancelSelectDelegate,
    	select: this._cancelSelectDelegate
    }
}
$HGRootNS.PopupControlBase.prototype = {

	createElementFromTemplate: function (template, appendToParent, nameTable) {
		return $HGDomElement.createElementFromTemplate(template, appendToParent, nameTable, this.get_popupDocument());
	},

	add_beforeShow: function (handler) {
		/// <summary>Adds a event handler for the tick event.</summary>
		/// <param name="handler" type="Function">The handler to add to the event.</param>
		this.get_events().addHandler("beforeShow", handler);
	},
	remove_beforeShow: function (handler) {
		/// <summary>Removes a event handler for the tick event.</summary>
		/// <param name="handler" type="Function">The handler to remove from the event.</param>
		this.get_events().removeHandler("beforeShow", handler);
	},

	get_positionElement: function () {
		/// <value>Parent dom element.</value>        
		if (!this._positionElement && this._positionElementID) {
			this.set_positionElement($get(this._positionElementID));
			Sys.Debug.assert(this._positionElement != null, String.format($HGRootNS.Resources.PopupExtender_NopositionElement, this._positionElementID));
		}
		return this._positionElement;
	},
	set_positionElement: function (element) {
		this._positionElement = element;
		this.raisePropertyChanged('positionElement');
	},

	get_positionElementID: function () {
		/// <value>Parent dom element.</value>
		if (this._positionElement) return this._positionElement.id
		return this._positionElementID;
	},
	set_positionElementID: function (elementID) {
		this._positionElementID = elementID;
		if (this.get_isInitialized()) {
			this.set_positionElement($get(elementID));
		}
	},

	get_positioningMode: function () {
		/// <value type="$HGRootNS.PositioningMode">Positioning mode.</value>
		return this._positioningMode;
	},
	set_positioningMode: function (mode) {
		this._positioningMode = mode;
		this.raisePropertyChanged('positioningMode');
	},

	get_x: function () {
		/// <value type="Number">X coordinate.</value>
		return this._x;
	},
	set_x: function (value) {
		if (value != this._x) {
			this._x = value;
			this.raisePropertyChanged('x');
		}
	},

	get_y: function () {
		/// <value type="Number">Y coordinate.</value>
		return this._y;
	},
	set_y: function (value) {
		if (value != this._y) {
			this._y = value;
			this.raisePropertyChanged('y');
		}
	},

	get_width: function () {
		/// <value type="Number">width.</value>
		return this._width;
	},
	set_width: function (value) {
		if (value != this._width) {
			this._width = value;
			this.raisePropertyChanged('width');
		}
	},

	get_height: function () {
		/// <value type="Number">width.</value>
		return this._height;
	},
	set_height: function (value) {
		if (value != this._height) {
			this._height = value;
			this.raisePropertyChanged('height');
		}
	},

	get_parent: function () {
		return this._parent;
	},
	set_parent: function (value) {
		if (this._parent != value) {
			this._parent = value;
			this.raisePropertyChanged("parent");
		}
	},

	get_style: function () {
		return this._style;
	},

	set_style: function (value) {
		this._style = value;
	},

	get_applyFilter: function () {
		return this._applyFilter;
	},

	set_applyFilter: function (value) {
		this._applyFilter = value;
	},

	get_usePublicPopupWindow: function () {
		return this._usePublicPopupWindow;
	},

	set_usePublicPopupWindow: function (value) {
		this._usePublicPopupWindow = value;
	},

	get_popupWindow: function () {
		return this._popupWindow;
	},

	get_popupDocument: function () {
		return this.get_popupWindow().document;
	},

	get_popupBody: function () {
		return this._body;
	},

	addCssClass: function (className) {
		$addCssClass(this.get_popupBody(), className);
	},

	removeCssClass: function (className) {
		$removeCssClass(this.get_popupBody(), className);
	},

	toggleCssClass: function (className) {
		$toggleCssClass(this.get_popupBody(), className);
	},

	get_isOpen: function () {
	},

	hide: function () {
	},

	show: function () {
	},

	initialize: function () {
		$HGRootNS.PopupControlBase.callBaseMethod(this, 'initialize');
	},

	dispose: function () {
		this._positionElement = null;
		this._popupWindow = null;
		this._style = null;
		$HGRootNS.PopupControlBase.callBaseMethod(this, 'dispose');
	},

	_beforeShowCallback: function (x, y, width, height, element) {
		var handler = this.get_events().getHandler("beforeShow");
		if (handler) {
			var e = new Sys.EventArgs();
			e.positionElement = element;
			e.x = x;
			e.y = y;
			e.width = width;
			e.height = height;
			handler(this, e);

			return e;
		}
	},

	_cancelSelect: function () {
		var doc = this.get_popupDocument();
		doc.selection.empty();
		var win = doc.parentWindow;
		if (win.event)
			win.event.returnValue = false;
	}
}
$HGRootNS.PopupControlBase.registerClass($HGRootNSName + '.PopupControlBase', Sys.Component);

$HGRootNS.PopupControl = function () {
	/// <param name="element">The DOM element the behavior is associated with.</param>
	$HGRootNS.PopupControl.initializeBase(this);
	this._popupBoundDiv = null;
}

$HGRootNS.PopupControl.prototype = {
	get_isOpen: function () {
		return this._popupWindow.isOpen;
	},

	get_popupBody: function () {
		this._ensureBody();
		return this._body;
	},

	/// <summary>
	/// 绘制选项列表的一个Item
	/// </summary>
	/// <param name="itemText">Item的显示文本</param>
	/// <param name="itemValue">Item的Value值</param>
	/// <param name="currentIndex">Item的位置索引值,只是Item在显示的内容中的索引值</param>
	/// <param name="parentElement">所属的列表对象（父对象）</param>
	createItem: function (itemText, itemValue, itemEvents, currentIndex, parentElement, itemCssClass) {
		var itemDiv = $HGDomElement.createElementFromTemplate(
        {
        	nodeName: "div",
        	events: itemEvents,
        	cssClasses: [itemCssClass],
        	properties:
                {
                	id: "div_Item_" + currentIndex,
                	indexValue: currentIndex, //这个属性非常酷的保存了每个Item的Index值
                	border: 1,
                	value: itemValue,
                	style:
                    {
                    	margin: "1px 0 1px 0",
                    	fontSize: "12px",
                    	display: "block",
                    	color: this._itemFontColor
                    },
                	innerText: itemText,
                	title: itemText
                }
        }, parentElement, null,
            this._popupWindow.document);

		return itemDiv;
	},

	createListItem: function (parentElement, CssClass, hoverCssClass, itemEvents, dataSource, defaultSelectInfex) {
		if (dataSource) {
			for (var i = 0; i < dataSource.length; i++) {
				var itemDiv = this.createItem(dataSource[i].Text, dataSource[i].Value, itemEvents, i, parentElement, CssClass);

				if (i == defaultSelectInfex) {
					Sys.UI.DomElement.removeCssClass(itemDiv, CssClass);
					Sys.UI.DomElement.addCssClass(itemDiv, hoverCssClass);
				}
			}
		}
	},

	hide: function () {
		this._popupWindow.hide();
	},

	show: function () {
		this._ensureBody();
		var elt = this._positionElement || this._popupWindow.document.documentElement;
		parentBounds = $HGDomElement.getBounds(elt);

		if (!this._width || !this._height) {
			var span = document.createElement("span");

			span.style.visibility = "hidden";
			span.style.position = "absolute";
			span.style.top = "0px";
			span.style.left = "0px";
			document.body.appendChild(span);
			span.innerHTML = this.get_popupDocument().body.innerHTML;
			var size = $HGDomElement.getSize(span);
			span.innerHTML = "";
			document.body.removeChild(span);
		}

		var width = this._width ? this._width : size.width;
		var height = this._height ? this._height : size.height;

		var position;
		switch (this._positioningMode) {
			case $HGRootNS.PositioningMode.Center:
				position = {
					x: Math.round(parentBounds.width / 2 - width / 2),
					y: Math.round(parentBounds.height / 2 - height / 2)
				};
				break;
			case $HGRootNS.PositioningMode.BottomLeft:
				position = {
					x: 0,
					y: parentBounds.height
				};
				break;
			case $HGRootNS.PositioningMode.BottomRight:
				position = {
					x: parentBounds.width - width,
					y: parentBounds.height
				};
				break;
			case $HGRootNS.PositioningMode.TopLeft:
				position = {
					x: 0,
					y: -height
				};
				break;
			case $HGRootNS.PositioningMode.TopRight:
				position = {
					x: parentBounds.width - width,
					y: -height
				};
				break;


			case $HGRootNS.PositioningMode.LeftBottom:
				position = {
					x: -width,
					y: -parentBounds.height
				};
				break;
			case $HGRootNS.PositioningMode.LeftTop:
				position = {
					x: -width,
					y: 0
				};
				break;
			case $HGRootNS.PositioningMode.RightBottom:
				position = {
					x: parentBounds.width,
					y: -parentBounds.height
				};
				break;
			case $HGRootNS.PositioningMode.RightTop:
				position = {
					x: parentBounds.width,
					y: 0
				};
				break;
			default:
				position = { x: 0, y: 0 };
		}

		position.x += this._x;
		position.y += this._y;

		// if (!this._positionElement)
		this.get_popupBody().style.width = width;
		this.get_popupBody().style.height = height;

		var result = this._beforeShowCallback(position.x, position.y, width, height, this._positionElement);
		if (result)
			this._popupWindow.show(result.x, result.y, result.width, result.height, this._positionElement);
		else
			this._popupWindow.show(position.x, position.y, width, height, this._positionElement);

//		if (this._applyFilter == true) {
//			var body = this._popupWindow.document.body;
//			body.style.visibility = "hidden";
//			body.filters[0].Apply();
//			body.style.visibility = "visible";
//			body.filters[0].Play();
//		}
	},

	showByPosition: function () {
		this._ensureBody();
		var positionAndSize = this.get_positionAndSize();
		this.get_popupBody().style.width = positionAndSize.Size.width;
		this.get_popupBody().style.height = positionAndSize.Size.height;
		var result = this._beforeShowCallback(positionAndSize.Position.x, positionAndSize.Position.y, positionAndSize.Size.width, positionAndSize.Size.height, this._positionElement);
		if (result)
			this._popupWindow.show(result.x, result.y, result.width, result.height, this._positionElement);
		else
			this._popupWindow.show(positionAndSize.Position.x, positionAndSize.Position.y, positionAndSize.Size.width, positionAndSize.Size.height, this._positionElement);

//		if (this._applyFilter == true) {
//			var body = this._popupWindow.document.body;
//			body.style.visibility = "hidden";
//			body.filters[0].Apply();
//			body.style.visibility = "visible";
//			body.filters[0].Play();
//		}

	},
	get_positionAndSize: function () {
		//this._ensureBody();
		var positionAndSize = { Size: { width: 0, height: 0 }, Position: {} };
		var elt = this._positionElement || this._popupWindow.document.documentElement;
		parentBounds = $HGDomElement.getBounds(elt);

		if (!this._width || !this._height) {
			var span = document.createElement("span");

			span.style.visibility = "hidden";
			span.style.position = "absolute";
			span.style.top = "0px";
			span.style.left = "0px";
			document.body.appendChild(span);
			span.innerHTML = this.get_popupDocument().body.innerHTML;
			var size = $HGDomElement.getSize(span);
			span.innerHTML = "";
			document.body.removeChild(span);
		}

		var width = this._width ? this._width : size.width;
		var height = this._height ? this._height : size.height;

		positionAndSize.Size.width = width;
		positionAndSize.Size.height = height;

		var position;
		switch (this._positioningMode) {
			case $HGRootNS.PositioningMode.Center:
				position = {
					x: Math.round(parentBounds.width / 2 - width / 2),
					y: Math.round(parentBounds.height / 2 - height / 2)
				};
				break;
			case $HGRootNS.PositioningMode.BottomLeft:
				position = {
					x: 0,
					y: parentBounds.height
				};
				break;
			case $HGRootNS.PositioningMode.BottomRight:
				position = {
					x: parentBounds.width - width,
					y: parentBounds.height
				};
				break;
			case $HGRootNS.PositioningMode.TopLeft:
				position = {
					x: 0,
					y: -height
				};
				break;
			case $HGRootNS.PositioningMode.TopRight:
				position = {
					x: parentBounds.width - width,
					y: -height
				};
				break;
			case $HGRootNS.PositioningMode.LeftBottom:
				position = {
					x: -width,
					y: -parentBounds.height
				};
				break;
			case $HGRootNS.PositioningMode.LeftTop:
				position = {
					x: -width,
					y: 0
				};
				break;
			case $HGRootNS.PositioningMode.RightBottom:
				position = {
					x: parentBounds.width,
					y: -parentBounds.height
				};
				break;
			case $HGRootNS.PositioningMode.RightTop:
				position = {
					x: parentBounds.width,
					y: 0
				};
				break;
			default:
				position = { x: 0, y: 0 };
		}

		position.x += this._x;
		position.y += this._y;


		positionAndSize.Position = position;
		return positionAndSize;
	},

	_createNewPopup: function (pWin) {
		var popup = pWin.createPopup();
		var doc = popup.document;

		doc.writeln("<html><head>");
		Sys.UI.DomEvent.writePopupWindowEventDelegate(doc.parentWindow);
		doc.writeln("</head><body onselect=\"return false\"/></html>");

		$HGRootNS.addChildWindow(popup);

		$HGDomElement.copyCss(document, doc);

		doc.body.style.border = "none";
		doc.body.style.margin = "0px";
		doc.body.scroll = "no";
		//doc.body.style.filter = "progid:DXImageTransform.Microsoft.Fade(duration=0.1,overlap=0.75)";

		return popup;
	},

	_createNewBody: function () {
		var pBody = this._popupWindow.document.body;

		var body = this.createElementFromTemplate({ nodeName: "div" }, pBody);
		body.style.backgroundColor = "#ffffff";
		//body.style.border = "1px solid #cccccc";
		//body.style.filter = "progid:DXImageTransform.Microsoft.Fade(duration=0.5,overlap=0.75)";// progid:DXImageTransform.Microsoft.Shadow(direction=135,color=#333333,strength=3)";
		body.scroll = "no";
		$HGDomElement.setStyle(body, this._style);

		$addHandlers(body, this._bodyEvents);

		return body;
	},

	_ensureBody: function () {
		$HGDomElement.ensureOneChild(this._popupWindow.document.body, this._body);
	},

	_createPopupWindow: function () {
		if (!this._popupWindow) {
			var pWin = this._parent ? this._parent.get_popupWindow().document.parentWindow : window;

			var popupWindow = null;
			if (this._usePublicPopupWindow) {
				var popupCache = pWin.__popupCache;
				if (!popupCache) {
					popupCache = this._createNewPopup(pWin);
					pWin.__popupCache = popupCache;
				}

				popupWindow = popupCache;
			}
			else {
				popupWindow = this._createNewPopup(pWin);
			}

			this._popupWindow = popupWindow;
			this._body = this._createNewBody();
			this._ensureBody();
		}
	},

	initialize: function () {
		$HGRootNS.PopupControl.callBaseMethod(this, 'initialize');
		this._createPopupWindow();
	},

	dispose: function () {
		// $HGDomElement.removeHandlers(this.get_popupBody(), this._bodyEvents);
		$HGRootNS.PopupControl.callBaseMethod(this, 'dispose');
	}
}

$HGRootNS.PopupControl.registerClass($HGRootNSName + '.PopupControl', $HGRootNS.PopupControlBase);

// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

//var $HGRootNSName = 'MCS.Web.Library.Script';
//Type.registerNamespace($HGRootNSName);
//var $HGRootNS = eval($HGRootNSName);

$HGRootNS.DeferredOperation = function (delay, context, callback) {
	/// <summary>
	/// Used to define a cancellable async operation
	/// </summary>
	/// <param name="delay" type="Number" integer="true">the number of milliseconds to delay execution</param>
	/// <param name="context" type="Object" mayBeNull="true">an object used as the context for the callback method</param>
	/// <param name="callback" type="Function">The callback method to execute at the end of the delay</param>

	this._delay = delay;
	this._context = context;
	this._callback = callback;
	this._completeCallback = null;
	this._errorCallback = null;
	this._timer = null;
	this._callArgs = null;
	this._isComplete = false;
	this._completedSynchronously = false;
	this._asyncResult = null;
	this._exception = null;
	this._throwExceptions = true;
	this._oncomplete$delegate = Function.createDelegate(this, this._oncomplete);

	// post to ensure that attaching it always gets the port as its context
	this.post = Function.createDelegate(this, this.post);
}
$HGRootNS.DeferredOperation.prototype = {

	get_isPending: function () {
		/// <summary>
		/// Gets whether there is an asynchronous operation pending
		/// </summary>
		/// <returns type="Boolean" />

		return (this._timer != null);
	},

	get_isComplete: function () {
		/// <summary>
		/// Gets whether the asynchronous operation has completed
		/// </summary>
		/// <returns type="Boolean" />

		return this._isComplete;
	},

	get_completedSynchronously: function () {
		/// <summary>
		/// Gets whether the operation completed synchronously
		/// </summary>
		/// <returns type="Boolean" />

		return this._completedSynchronously;
	},

	get_exception: function () {
		/// <summary>
		/// Gets the current exception if there is one
		/// </summary>
		/// <returns type="Error" />

		return this._exception;
	},

	get_throwExceptions: function () {
		/// <summary>
		/// Gets whether to throw exceptions
		/// </summary>
		/// <returns type="Boolean" />

		return this._throwExceptions;
	},
	set_throwExceptions: function (value) {
		/// <summary>
		/// Sets whether to throw exceptions
		/// </summary>
		/// <param name="value" type="Boolean">True if exceptions should be thrown, otherwise false</param>

		this._throwExceptions = value;
	},

	get_delay: function () {
		/// <summary>
		/// Gets the current delay in milliseconds
		/// </summary>
		/// <returns type="Number" integer="true" />

		return this._delay;
	},
	set_delay: function (value) {
		/// <summary>
		/// Sets the current delay in milliseconds
		/// </summary>
		/// <param name="value" type="Number" integer="true">The delay in milliseconds</param>

		this._delay = value;
	},

	post: function (args) {
		/// <summary>
		/// A method that can be directly attached to a delegate
		/// </summary>
		/// <param name="args" type="Object" parameterArray="true">The arguments to the method</param>

		var ar = [];
		for (var i = 0; i < arguments.length; i++) {
			ar[i] = arguments[i];
		}
		this.beginPost(ar, null, null);
	},

	beginPost: function (args, completeCallback, errorCallback) {
		/// <summary>
		/// Posts a call to an async operation on this port
		/// </summary>
		/// <param name="args" type="Array">An array of arguments to the method</param>
		/// <param name="completeCallback" type="Function" optional="true" mayBeNull="true">The callback to execute after the delayed function completes</param>
		/// <param name="errorCallback" type="Function" optional="true" mayBeNull="true">The callback to execute in the event of an exception in the delayed function</param>

		// cancel any pending post
		this.cancel();

		// cache the call arguments
		this._callArgs = Array.clone(args || []);
		this._completeCallback = completeCallback;
		this._errorCallback = errorCallback;

		if (this._delay == -1) {
			// if there is no delay (-1), complete synchronously
			this._oncomplete();
			this._completedSynchronously = true;
		} else {
			// complete the post on a seperate call after a delay
			this._timer = setTimeout(this._oncomplete$delegate, this._delay);
		}
	},

	cancel: function () {
		/// <summary>
		/// Cancels a pending post
		/// </summary>

		if (this._timer) {
			clearTimeout(this._timer);
			this._timer = null;
		}
		this._callArgs = null;
		this._isComplete = false;
		this._asyncResult = null;
		this._completeCallback = null;
		this._errorCallback = null;
		this._exception = null;
		this._completedSynchronously = false;
	},

	complete: function () {
		/// <summary>
		/// Completes a pending post synchronously
		/// </summary>        

		if (this._timer) {
			try {
				this._oncomplete();
			} finally {
				this._completedSynchronously = true;
			}
			return this._asyncResult;
		} else if (this._isComplete) {
			return this._asyncResult;
		}
	},

	_oncomplete: function () {
		/// <summary>
		/// Completes a pending post asynchronously
		/// </summary>

		var args = this._callArgs;
		var completeCallback = this._completeCallback;
		var errorCallback = this._errorCallback;

		// clear the post state
		this.cancel();
		try {
			// call the post callback
			if (args) {
				this._asyncResult = this._callback.apply(this._context, args);
			} else {
				this._asyncResult = this._callback.call(this._context);
			}
			this._isComplete = true;
			this._completedSynchronously = false;
			if (completeCallback) {
				completeCallback(this);
			}
		} catch (e) {
			this._isComplete = true;
			this._completedSynchronously = false;
			this._exception = e;
			if (errorCallback) {
				if (errorCallback(this)) {
					return;
				}
			}
			if (this._throwExceptions) {
				throw e;
			}
		}
	}
}
$HGRootNS.DeferredOperation.registerClass($HGRootNSName + ".DeferredOperation");

// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

///////////////////////////////////////////////////////////////////////////////
// $HGRootNS.Timer

$HGRootNS.Timer = function () {
	$HGRootNS.Timer.initializeBase(this);

	this._interval = 1000;
	this._enabled = false;
	this._timer = null;
	this._timeCallbackDelegate = Function.createDelegate(this, this._timerCallback);

}

$HGRootNS.Timer.prototype = {
	get_interval: function () {

		return this._interval;
	},
	set_interval: function (value) {

		if (this._interval !== value) {
			this._interval = value;
			this.raisePropertyChanged('interval');

			if (!this.get_isUpdating() && (this._timer !== null)) {
				this._stopTimer();
				this._startTimer();
			}
		}
	},

	get_enabled: function () {

		return this._enabled;
	},
	set_enabled: function (value) {

		if (value !== this.get_enabled()) {
			this._enabled = value;
			this.raisePropertyChanged('enabled');
			if (!this.get_isUpdating()) {
				if (value) {
					this._startTimer();
				}
				else {
					this._stopTimer();
				}
			}
		}
	},


	add_tick: function (handler) {
		this.get_events().addHandler("tick", handler);
	},

	remove_tick: function (handler) {
		this.get_events().removeHandler("tick", handler);
	},

	dispose: function () {
		this.set_enabled(false);
		this._stopTimer();

		$HGRootNS.Timer.callBaseMethod(this, 'dispose');
	},

	updated: function () {
		$HGRootNS.Timer.callBaseMethod(this, 'updated');

		if (this._enabled) {
			this._stopTimer();
			this._startTimer();
		}
	},

	_timerCallback: function () {
		var handler = this.get_events().getHandler("tick");
		if (handler) {
			handler(this, Sys.EventArgs.Empty);
		}
	},

	_startTimer: function () {
		this._timer = window.setInterval(this._timeCallbackDelegate, this._interval);
	},

	_stopTimer: function () {
		window.clearInterval(this._timer);
		this._timer = null;
	}
}

$HGRootNS.Timer.descriptor = {
	properties: [{ name: 'interval', type: Number },
                    { name: 'enabled', type: Boolean}],
	events: [{ name: 'tick'}]
}

$HGRootNS.Timer.registerClass($HGRootNSName + '.Timer', Sys.Component);

// -------------------------------------------------
// FileName	：	TreeBase.js
// Remark	：	树的数据对象,树和菜单控件基类
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    周维海	    20070403		创建
// -------------------------------------------------

$HGRootNS.TreeNode = function (element) {
	$HGRootNS.TreeNode.initializeBase(this, [element]);
	this._parent = null;
	this._childNodeContainer = null;
	this._children = new Array();
	this._prevNode = null;
	this._nextNode = null;
}

$HGRootNS.TreeNode.prototype = {
	get_parent: function () {
		return this._parent;
	},
	set_parent: function (value) {
		if (this._parent != value) {
			this._parent = value;
			this.raisePropertyChanged("parent");
		}
	},

	get_childNodeContainer: function () {
		return this._childNodeContainer;
	},
	set_childNodeContainer: function (value) {
		if (this._childNodeContainer != value) {
			this._childNodeContainer = value;
			this.raisePropertyChanged("childNodeContainer");
		}
	},

	get_children: function () {
		return this._children;
	},

	get_prevNode: function () {
		return this._prevNode;
	},
	set_prevNode: function (value) {
		if (this._prevNode != value) {
			this._prevNode = value;
			this.raisePropertyChanged("prevNode");
		}
	},

	get_nextNode: function () {
		return this._nextNode;
	},
	set_nextNode: function (value) {
		if (this._nextNode != value) {
			this._nextNode = value;
			this.raisePropertyChanged("nextNode");
		}
	},

	get_level: function () {
		return this._parent ? this._parent.get_level() + 1 : 0;
	},

	get_hasChildNodes: function () {
		return this._children.length > 0;
	},

	get_child: function (index) {
		if (index >= 0 && index < this._children.length)
			return this._children[index];
		else
			return null;
	},

	get_firstChild: function () {
		return this.get_child(0);
	},

	get_lastChild: function () {
		return this.get_child(this._children.length - 1);
	},

	initialize: function () {
		$HGRootNS.TreeNode.callBaseMethod(this, 'initialize');
	},

	dispose: function () {
		$HGRootNS.TreeNode.callBaseMethod(this, 'dispose');
		this._parent = null;
		this._childNodeContainer = null;
		this._children = null;
		this._prevNode = null;
		this._nextNode = null;
	},

	appendChild: function (node) {
		if (this.get_hasChildNodes()) {
			var prevNode = this.get_lastChild();
			prevNode.set_nextNode(node);
			node.set_prevNode(prevNode);
		}
		node.set_parent(this);
		Array.add(this._children, node);

		if (this._childNodeContainer)
			this._childNodeContainer.appendChild(node.get_element());
	},

	insertChild: function (index, node) {
		var prevNode = this.get_child(index - 1); //modify 20071101
		var nextNode = this.get_child(index); //modify 20071101

		if (prevNode) {
			node.set_prevNode(prevNode);
			prevNode.set_nextNode(node);
		}

		if (nextNode) {
			node.set_nextNode(nextNode);
			nextNode.set_prevNode(node);
		}

		node.set_parent(this);
		Array.insert(this._children, index, node);

		if (this._childNodeContainer)
			this._childNodeContainer.insertBefore(node.get_element(), nextNode ? nextNode.get_element() : null);
	},

	_removeRelation: function (node) {
		var prevNode = node.prevNode;
		var nextNode = node.nextNode;

		if (prevNode)
			prevNode.set_nextNode(nextNode);

		if (nextNode)
			nextNode.set_prevNode(prevNode);
	},

	removeChild: function (node) {
		this._removeRelation(node);
		node.set_parent(null);
		Array.remove(this._children, node);
		if (this._childNodeContainer)
			this._childNodeContainer.removeChild(node.get_element());
	},

	removeChildAt: function (index) {
		var node = this.get_child(index);

		if (node) {
			this._removeRelation(node);
			node.set_parent(null);
			Array.removeAt(this._children, index);
			if (this._childNodeContainer)
				this._childNodeContainer.removeChild(node.get_element());
		}

		return node;
	},

	//新增clear 20071025
	clear: function () {
		while (this._children.length > 0) {
			this.removeChildAt(0);
		}
	},

	enumerateChildren: function (method, context, deep) {
		for (var i = 0; i < this._children.length; i++) {
			var childNode = this._children[i];
			method.call(childNode, context, i, this);
			if (deep)
				childNode.enumerateChildren(method, context, deep);
		}
	},

	createChildElement: function () {
		throw Error.notImplemented();
	},

	//新增by 林彬 2011-3-30 
	siblingNodeChangePosition: function (bePrevNode, beNextNode) {
		if (bePrevNode.get_parent() == beNextNode.get_parent()) {
			var parent = bePrevNode.get_parent();

			var index = beNextNode.get_index();

			parent.removeChild(bePrevNode);
			parent.removeChild(beNextNode);

			parent.insertChild(index, bePrevNode);
			parent.insertChild(index + 1, beNextNode);

			if (bePrevNode.get_index() == 0)
				bePrevNode.set_prevNode(null);

			if (beNextNode.get_index() == parent.get_children().length - 1)
				beNextNode.set_nextNode(null);

		}
		else {
			alert("Error! The two nodes is not siblingNode!");
		}
	},

	//新增by 林彬 2011-3-30 
	get_index: function () {
		var siblingNode = this.get_parent().get_children();
		for (var i = 0; i < siblingNode.length; i++) {
			if (siblingNode[i] == this)
				return i;
		}
		return 0;
	}
}

$HGRootNS.TreeNode.registerClass($HGRootNSName + ".TreeNode", $HGRootNS.ControlBase);

$HGRootNS.TreeControlBase = function (element) {
	$HGRootNS.TreeControlBase.initializeBase(this, [element]);
	this._treeNode = null();
}

$HGRootNS.TreeControlBase.prototype = {
	get_treeNode: function () {
		return this._treeNode;
	},

	set_treeNode: function (treeNode) {
		if (this._treeNode !== treeNode) {
			this._treeNode = treeNode;
			this.raisePropertyChanged('treeNode');
		}
	},

	initialize: function () {
		$HGRootNS.TreeControlBase.callBaseMethod(this, 'initialize');
	},


	dispose: function () {
		this._treeNode.dispose();
		this._treeNode = null;
		$HGRootNS.TreeControlBase.callBaseMethod(this, 'dispose');
	}
}
$HGRootNS.TreeControlBase.registerClass($HGRootNSName + '.TreeControlBase', $HGRootNS.ControlBase);

$HGRootNS.falseThrow = function (bCondition, strDescription) {
	if (!bCondition)
		throw Error.create(strDescription);
}

$HGRootNS.trueThrow = function (bCondition, strDescription) {
	if (bCondition)
		throw Error.create(strDescription);
}


//=============================================校验相关 start(Validation)=========================================={==========================================
// -------------------------------------------------
// Remark	：	校验器
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    林彬(迁移)	    20114		创建
// -------------------------------------------------

//======校验涉及的数据类型-开始={=========
$HGRootNS.ValidationDataType = function (element) {
	throw Error.invalidOperation();
}

$HGRootNS.ValidationDataType.prototype = {
	Object: 1,
	Boolean: 3,
	Integer: 9,
	Decimal: 15,
	DateTime: 16,
	String: 18,
	Enum: 20
}

$HGRootNS.ValidationDataType.registerEnum($HGRootNSName + ".ValidationDataType");
//=========校验涉及的数据类型-完成=}========

//=========文本校验器的基类-开始={========
$HGRootNS.TextValidatorBase = function (dataType) {
	this._dataType = $HGRootNS.ValidationDataType.String;
}

$HGRootNS.TextValidatorBase.prototype = {
	validate: function (text) {
		throw Error.notImplemented();
	},

	get_dataType: function () {
		return this._dataType;
	},

	createTextValidationResult: function (text) {
		return { message: "", normalizedText: text, isValid: true };
	}
}

$HGRootNS.TextValidatorBase.registerClass($HGRootNSName + ".TextValidatorBase");

//Numeric check: numericCheck(nr, [intDigit], [fracDigit], [minValue], [maxValue])
$HGRootNS.TextValidatorBase._numericCheck = function (nr) {
	var nArgs = arguments.length;
	var nCount = 0;
	var nPointIndex = -1;
	var nSignIndex = -1;

	for (var i = 0; i < nr.length; i++) {
		var ch = nr.substr(i, 1);

		if (ch < "0" || ch > "9") {
			if (ch == ".") {
				if (nPointIndex != -1)
					throw Error.create("数字类型只能有一个小数点");
				else
					nPointIndex = i;
			}
			else
				if (ch == "-" || ch == "+") {
					if (nSignIndex != -1)
						throw Error.create("数字类型只能有一个\"" + ch + "\"");
					else
						nSignIndex = i;
				}
				else
					if (ch != ",")	//过滤掉数字
						throw Error.create("必须输入合法的数字");
		}
	}

	if (nPointIndex == -1)
		nPointIndex = nr.length;

	if (nArgs > 1) { //参数个数大于1
		var nNumber = nr * 1;
		var intDigit = arguments[1];
		var fracDigit;
		var minValue;
		var maxvalue;

		if (nArgs > 2) {
			fracDigit = arguments[2];
			if (nArgs > 3) {
				minValue = arguments[3];
				if (nArgs > 4)
					maxValue = arguments[4];
			}
		}
	}

	$HGRootNS.trueThrow(typeof (intDigit) != "undefined" && (nr.substring(0, nPointIndex) * 1).toString().length > intDigit,
		 "整数部分的位数不能超过" + intDigit + "位");

	var strFrac = nr.substring(nPointIndex + 1, nr.length);

	if (strFrac.length > 0) {
		strFrac = "0." + strFrac;
		$HGRootNS.trueThrow(typeof (fracDigit) != "undefined" && (strFrac * 1).toString().length - 2 > fracDigit,
			"小数部分的位数不能超过" + fracDigit + "位");
	}

	if (typeof (minValue) != "undefined" && typeof (maxValue) != "undefined") {
		$HGRootNS.trueThrow((nr * 1) < minValue || (nr * 1) > maxValue, "数字必须在" + minValue + "和" + maxValue + "之间");
	}
	else
		if (typeof (minValue) != "undefined") {
			$HGRootNS.trueThrow((nr * 1) < minValue, "数字必须大于等于" + minValue);
		}
		else
			if (typeof (maxValue) != "undefined") {
				$HGRootNS.trueThrow((nr * 1) > maxValue, "数字必须小于等于" + maxValue);
			}
}

//=========文本校验器的基类-完成=}========

//=========整数文本校验器-开始={========
$HGRootNS.IntegerTextValidator = function () {
	$HGRootNS.IntegerTextValidator.initializeBase(this, [$HGRootNS.ValidationDataType.Integer]);
}

$HGRootNS.IntegerTextValidator.prototype = {
	validate: function (text) {
		var text = text.replace(/,/g, '');
		var result = this.createTextValidationResult(text);

		try {
			$HGRootNS.TextValidatorBase._numericCheck(text, 12, 0);
		}
		catch (e) {
			result.isValid = false;
			result.message = e.message;
		}
		return result;
	}
}

$HGRootNS.IntegerTextValidator.registerClass($HGRootNSName + ".IntegerTextValidator", $HGRootNS.TextValidatorBase);
//=========整数文本校验器-完成=}========

//=========数字文本校验器-开始={========
$HGRootNS.NumericTextValidator = function () {
	$HGRootNS.NumericTextValidator.initializeBase(this, [$HGRootNS.ValidationDataType.Integer]);
}

$HGRootNS.NumericTextValidator.prototype = {
	validate: function (text) {
		var text = text.replace(/,/g, '');
		var result = this.createTextValidationResult(text);

		try {
			$HGRootNS.TextValidatorBase._numericCheck(text);
		}
		catch (e) {
			result.isValid = false;
			result.message = e.message;
		}

		return result;
	}
}

$HGRootNS.NumericTextValidator.registerClass($HGRootNSName + ".NumericTextValidator", $HGRootNS.TextValidatorBase);
//=========数字文本校验器-完成=}========

$HGRootNS.ValidationDataType._textValidators = [];

$HGRootNS.ValidationDataType.registerTextValidator = function (dType, vdt) {
	$HGRootNS.ValidationDataType._textValidators.push({ dataType: dType, validator: vdt });
}

$HGRootNS.ValidationDataType.getTextValidator = function (dType) {
	var result = null;

	for (var i = 0; i < $HGRootNS.ValidationDataType._textValidators.length; i++) {
		if ($HGRootNS.ValidationDataType._textValidators[i].dataType == dType) {
			result = $HGRootNS.ValidationDataType._textValidators[i];
			break;
		}
	}

	return result;
}

$HGRootNS.ValidationDataType.get_strongTypeValue = function (dataType, text) {
	var result = text;

	switch (dataType) {
		case $HGRootNS.ValidationDataType.Integer:
			text = text == "" ? 0 : text;
			result = parseInt(text, 10);
			break;
		case $HGRootNS.ValidationDataType.Decimal:
			text = text == "" ? 0 : text;
			result = parseFloat(text);
			break;
	}

	return result;
}

$HGRootNS.Formatter = function () {
}

$HGRootNS.Formatter.pictureFormat = function (strExp, strFmt) {
	var vResult;
	var objFmt;
	var nI, nJ;

	strExp = strExp.toString().replace(/,/g, '');

	if (isNaN(strExp))
		return "";

	vResult = new Number(strExp);
	objFmt = this.paramFmt(strFmt);

	//根据格式串定义，对vResult进行舍入；结果整数部分->strIntPrt，小数部分->strDecPart；
	vResult *= Math.pow(10, objFmt.DecRndCount);
	vResult = Math.round(vResult);
	vResult = vResult.toString();

	var strIntPart, strDecPart;

	with (vResult) {
		strIntPart = substr(0, length - objFmt.DecRndCount);
		strDecPart = substr(length - objFmt.DecRndCount, objFmt.DecRndCount);
		//去除小数部分尾部多余的零；
		strDecPart = (Math.pow(10, -objFmt.DecRndCount) * strDecPart.valueOf()).toString();
		strDecPart = strDecPart.substr(2, objFmt.DecRndCount);
	}

	//定长整数部分(前补零)及小数部分(后补零)；
	for (nI = strIntPart.length; nI < objFmt.IntFixLen; nI++)
		strIntPart = "0" + strIntPart;

	for (nI = strDecPart.length; nI < objFmt.DecFixLen; nI++)
		strDecPart = strDecPart + "0";

	//整数部分根据格式串就位；
	var strBuf, aBuf;

	strBuf = objFmt.IntPart.replace(/#/g, "0");
	aBuf = strBuf.split("0");
	if (aBuf.length) {
		strBuf = aBuf[aBuf.length - 1];
		for (nI = aBuf.length - 2, nJ = strIntPart.length - 1; nI >= 0; nI--, nJ--)
			strBuf = aBuf[nI] + strIntPart.charAt(nJ) + strBuf;

		if (nJ >= 0)
			strBuf = strIntPart.substr(0, nJ + 1) + strBuf;

		strIntPart = strBuf;
	}

	//小数部分根据格式串就位；
	strBuf = objFmt.DecPart.replace(/#/g, "0");
	aBuf = strBuf.split("0");
	if (aBuf.length) {
		strBuf = aBuf[0];
		for (nI = 1, nJ = 0; nI < aBuf.length; nI++, nJ++)
			strBuf = strBuf + strDecPart.charAt(nJ) + aBuf[nI];

		if (nJ < strDecPart.length)
			strBuf = strBuf + strIntPart.substr(nJ);

		strDecPart = strBuf;
	}

	//处理逗分数值表示法；
	if (strIntPart.search(/,/g) >= 0)
		strIntPart = $HGRootNS.Formatter.numCommaSplit(strIntPart);

	//合并整数部分和小数部分，返回；
	if (strDecPart.length > 0)
		strDecPart = "." + strDecPart;

	return strIntPart + strDecPart;
}

$HGRootNS.Formatter.paramFmt = function (strFmt) {
	if (strFmt.indexOf(':') >= 0) {
		strFmt = strFmt.substr(strFmt.indexOf(':') + 1, strFmt.length - strFmt.indexOf(':') - 2);
	}
	var objFmt = new Object();
	var nPointPos;
	var nZeroPos;

	nPointPos = strFmt.search(/\./g);
	if (nPointPos < 0)
		nPointPos = strFmt.length

	//确定格式串的整数部分、小数部分及舍入位数；
	objFmt.IntPart = strFmt.substr(0, nPointPos);
	objFmt.DecPart = strFmt.substr(nPointPos + 1);
	objFmt.DecRndCount = $HGRootNS.Formatter.statCharCount(objFmt.DecPart, '0', '#');

	//确定整数部分的最小长度；
	with (objFmt.IntPart) {
		nZeroPos = search(/0/g);
		if (nZeroPos < 0)
			nZeroPos = length;

		objFmt.IntFixLen = $HGRootNS.Formatter.statCharCount(substr(nZeroPos), '0', '#');
	}

	//确定小数部分的最小长度；
	with (objFmt.DecPart) {
		var aMatch = match(/0/g);
		if (aMatch == null)
			objFmt.DecFixLen = 0
		else
			objFmt.DecFixLen = $HGRootNS.Formatter.statCharCount(substr(0, aMatch.lastIndex), '0', '#');
	}

	return objFmt;
}

//以下为格式化数字程序
$HGRootNS.Formatter.statCharCount = function (str, chr) {
	var aMatch;
	var strPattern = chr;
	var nI;

	for (nI = 2; nI < arguments.length; nI++)
		strPattern = strPattern.concat("|", arguments[nI]);

	aMatch = str.match(new RegExp(strPattern, "g"));

	if (aMatch == null)
		return 0;
	else
		return aMatch.length;
}

$HGRootNS.Formatter.numCommaSplit = function (str) {
	var nI;
	var strBuf = "";
	var bNegative = false;

	//if negative get rid of "-"
	if (str.substr(0, 1) == "-") {
		str = str.substr(1);
		bNegative = true;
	}

	str = $HGRootNS.Formatter.getRidOfChar(str, ',');

	for (nI = str.length - 3; nI > 0; nI -= 3)
		strBuf = "," + str.substr(nI, 3) + strBuf;

	strBuf = str.substr(0, 3 + nI) + strBuf;

	//if Negative add "-"
	if (bNegative)
		strBuf = "-" + strBuf;

	return strBuf;
}

$HGRootNS.Formatter.getRidOfChar = function (str, chr) {
	var aBuf;
	var strBuf = "";
	var nI;

	aBuf = str.split(",");

	if (aBuf != null) {
		for (nI = 0; nI < aBuf.length; nI++)
			strBuf = strBuf + aBuf[nI];

		return strBuf;
	}
	else
		return "";
}

//注册默认的文本校验器
$HGRootNS.ValidationDataType.registerTextValidator($HGRootNS.ValidationDataType.Integer, new $HGRootNS.IntegerTextValidator());
$HGRootNS.ValidationDataType.registerTextValidator($HGRootNS.ValidationDataType.Decimal, new $HGRootNS.NumericTextValidator());

//=========控件校验绑定校验器的基类-开始={========
$HGRootNS.ValidationBinderBase = function () {
	this._formatString = "";
	this._dataType = $HGRootNS.ValidationDataType.String;
	this._control = null;
}

$HGRootNS.ValidationBinderBase.prototype = {
	bind: function () {
		throw Error.notImplemented();
	},

	get_formatString: function () {
		return this._formatString;
	},

	set_formatString: function (value) {
		this._formatString = value;
	},

	get_dataType: function () {
		return this._dataType;
	},

	set_dataType: function (value) {
		this._dataType = value;
	},

	get_control: function () {
		return this._control;
	},

	set_control: function (value) {
		this._control = value;
	},

	get_controlValue: function () {
		throw Error.notImplemented();
	},

	set_controlValue: function (value) {
		throw Error.notImplemented();
	},

	checkControl: function () {
	},

	add_dataChange: function (handler) {
		this.get_events().addHandler('dataChange', handler);
	},

	remove_dataChange: function (handler) {
		this.get_events().removeHandler('dataChange', handler);
	},

	raise_dataChange: function (normalizedText, strongTypeValue) {
		var handler = this.get_events().getHandler("dataChange");

		if (handler) {
			var e = {};

			e.binder = this;
			e.normalizedText = normalizedText;
			e.strongTypeValue = strongTypeValue;

			handler(this, e);
		}
	},

	pseudo: function () {
	}
}

$HGRootNS.ValidationBinderBase.registerClass($HGRootNSName + ".ValidationBinderBase", Sys.Component);
//=========控件校验绑定校验器的基类-完成=}========

//=========文本控件校验绑定校验器-开始={========
$HGRootNS.TextBoxValidationBinder = function () {
	$HGRootNS.TextBoxValidationBinder.initializeBase(this);

	this._formatString = "";
	this._dataType = $HGRootNS.ValidationDataType.String;
	this._control = null;
	this._currentValidator = null;
	this._originalText = null;
}

$HGRootNS.TextBoxValidationBinder.prototype = {

	bind: function () {
		this.checkControl();

		this._originalText = this.get_controlValue();

		$addHandlers(this._control, {
			change: Function.createDelegate(this, this._onInputTextChange),
			keypress: Function.createDelegate(this, this._onInputTextKeyPressed)
		});
	},

	get_controlValue: function () {
		return this.get_control().value;
	},

	get_controlUnformattedValue: function () {
		var rawValue = this.get_control().value;
		var result = rawValue;

		switch (this.get_dataType()) {
			case $HGRootNS.ValidationDataType.Integer:
			case $HGRootNS.ValidationDataType.Decimal:
				result = rawValue.toString().replace(/,/g, '');
				break;
		}

		return result;
	},

	set_controlValue: function (value) {
		this.get_control().value = value;
	},

	_onInputTextChange: function (e) {
		try {
			var normalizedText = this._validateText();
			var controlValue = normalizedText;
			var strongTypeValue = $HGRootNS.ValidationDataType.get_strongTypeValue(this.get_dataType(), controlValue);
			if (this.get_formatString() != null && this.get_formatString() != "") {
				controlValue = String.format(this.get_formatString(), strongTypeValue);
			}

			this.raise_dataChange(controlValue, strongTypeValue);

			//这里需要重新的从input中获得新的值，因为可能用户会响应change事件处理了input的值(linbin)
			normalizedText = this._validateText();
			controlValue = normalizedText;
			strongTypeValue = $HGRootNS.ValidationDataType.get_strongTypeValue(this.get_dataType(), controlValue);
			if (this.get_formatString() != null && this.get_formatString() != "") {
				controlValue = String.format(this.get_formatString(), strongTypeValue);
			}

			this.set_controlValue(controlValue);

			this._originalText = controlValue;
		}
		catch (e) {
			$showError(e);
			this.set_controlValue(this._originalText);

			var ctrl = this.get_control();

			window.setTimeout(
                function () {
                	ctrl.focus();
                },
            0);
		}
	},

	_validateText: function () {
		var normalizedText = this.get_controlValue();

		if (this._currentValidator == null || this._currentValidator.dataType != this.get_dataType()) {
			this._currentValidator = $HGRootNS.ValidationDataType.getTextValidator(this.get_dataType());
		}

		if (this._currentValidator) {
			var validateResult = this._currentValidator.validator.validate(this.get_controlUnformattedValue());

			if (!validateResult.isValid)
				throw Error.create(validateResult.message);

			normalizedText = validateResult.normalizedText;
		}

		return normalizedText;
	},

	_onInputTextKeyPressed: function (e) {
		switch (this.get_dataType()) {
			case $HGRootNS.ValidationDataType.Integer:
				var value = e.target.value;
				if (value.indexOf('-') > -1) {
					if ((e.rawEvent.keyCode < 48 || e.rawEvent.keyCode > 57)) {
						e.rawEvent.keyCode = 0;
						e.rawEvent.returnValue = false;
					}
				}
				else {
					if ((e.rawEvent.keyCode < 48 || e.rawEvent.keyCode > 57) && e.rawEvent.keyCode != 45) {
						e.rawEvent.keyCode = 0;
						e.rawEvent.returnValue = false;
					}
				}
				break;
			case $HGRootNS.ValidationDataType.Decimal:
				var value = e.target.value;
				if (value.indexOf('-') > -1) {
					if ((e.rawEvent.keyCode < 48 || e.rawEvent.keyCode > 57) && e.rawEvent.keyCode != 46) {
						e.rawEvent.keyCode = 0;
						e.rawEvent.returnValue = false;
					}
				}
				else {
					if ((e.rawEvent.keyCode < 48 || e.rawEvent.keyCode > 57) && e.rawEvent.keyCode != 46 && e.rawEvent.keyCode != 45) {
						e.rawEvent.keyCode = 0;
						e.rawEvent.returnValue = false;
					}
				}
				break;
		}
	},

	checkControl: function () {
		$HGRootNS.falseThrow(this.get_control() != null && this.get_control().tagName == "INPUT",
			"TextBoxValidationBinder对应的Control不能为空，且必须是INPUT");
	}
}

$HGRootNS.TextBoxValidationBinder.registerClass($HGRootNSName + ".TextBoxValidationBinder", $HGRootNS.ValidationBinderBase);
//=========文本控件校验绑定校验器=}========

$HGRootNS.ValidatorManager = function () {
	throw Error.invalidOperation();
};
$HGRootNS.ValidatorManager.registerClass($HGRootNSName + ".ValidatorManager");

//=============================================校验相关end }==========================================

//======================= 解决LinkButton PostBack是不保存状态问题 begin{==============================
var Sys$WebForms$PageRequestManager$_doPostBack = function (eventTarget, eventArgument) {

	var event = window.event;
	if (!event) {
		var caller = arguments.callee ? arguments.callee.caller : null;
		if (caller) {
			var recursionLimit = 30;
			while (caller.arguments.callee.caller && --recursionLimit) {
				caller = caller.arguments.callee.caller;
			}
			event = (recursionLimit && caller.arguments.length) ? caller.arguments[0] : null;
		}
	}
	this._additionalInput = null;
	var form = this._form;
	if ((eventTarget === null) || (typeof (eventTarget) === "undefined") || (this._isCrossPost)) {
		this._postBackSettings = this._createPostBackSettings(false);
		this._isCrossPost = false;
	}
	else {
		var mpUniqueID = this._masterPageUniqueID;
		var clientID = this._uniqueIDToClientID(eventTarget);
		var postBackElement = document.getElementById(clientID);
		if (!postBackElement && mpUniqueID) {
			if (eventTarget.indexOf(mpUniqueID + "$") === 0) {
				postBackElement = document.getElementById(clientID.substr(mpUniqueID.length + 1));
			}
		}
		if (!postBackElement) {
			if (Array.contains(this._asyncPostBackControlIDs, eventTarget)) {
				this._postBackSettings = this._createPostBackSettings(true, null, eventTarget);
			}
			else {
				if (Array.contains(this._postBackControlIDs, eventTarget)) {
					this._postBackSettings = this._createPostBackSettings(false);
				}
				else {
					var nearestUniqueIDMatch = this._findNearestElement(eventTarget);
					if (nearestUniqueIDMatch) {
						this._postBackSettings = this._getPostBackSettings(nearestUniqueIDMatch, eventTarget);
					}
					else {
						if (mpUniqueID) {
							mpUniqueID += "$";
							if (eventTarget.indexOf(mpUniqueID) === 0) {
								nearestUniqueIDMatch = this._findNearestElement(eventTarget.substr(mpUniqueID.length));
							}
						}
						if (nearestUniqueIDMatch) {
							this._postBackSettings = this._getPostBackSettings(nearestUniqueIDMatch, eventTarget);
						}
						else {
							var activeElement;
							try {
								activeElement = event ? (event.target || event.srcElement) : null;
							}
							catch (ex) {
							}
							activeElement = activeElement || this._activeElement;
							var causesPostback = /__doPostBack\(|WebForm_DoPostBackWithOptions\(/;
							function testCausesPostBack(attr) {
								attr = attr ? attr.toString() : "";
								return (causesPostback.test(attr) &&
                                        (attr.indexOf("'" + eventTarget + "'") !== -1) || (attr.indexOf('"' + eventTarget + '"') !== -1));
							}
							if (activeElement && (
                                        (activeElement.name === eventTarget) ||
                                        testCausesPostBack(activeElement.href) ||
                                        testCausesPostBack(activeElement.onclick) ||
                                        testCausesPostBack(activeElement.onchange)
                                        )) {
								this._postBackSettings = this._getPostBackSettings(activeElement, eventTarget);
							}
							else {
								this._postBackSettings = this._createPostBackSettings(false);
							}
						}
					}
				}
			}
		}
		else {
			this._postBackSettings = this._getPostBackSettings(postBackElement, eventTarget);
		}
	}
	if (!this._postBackSettings.async) {
		for (i = 0, l = this._onSubmitStatements.length; i < l; i++) {
			if (!this._onSubmitStatements[i]()) {
				continueSubmit = false;
				break;
			}
		}
		form.onsubmit = this._onsubmit;
		this._originalDoPostBack(eventTarget, eventArgument);
		form.onsubmit = null;
		return;
	}
	form.__EVENTTARGET.value = eventTarget;
	form.__EVENTARGUMENT.value = eventArgument;
	this._onFormSubmit();
}

Sys.WebForms.PageRequestManager.prototype._doPostBack = Sys$WebForms$PageRequestManager$_doPostBack;

//======================= 解决LinkButton PostBack是不保存状态问题 end}==============================