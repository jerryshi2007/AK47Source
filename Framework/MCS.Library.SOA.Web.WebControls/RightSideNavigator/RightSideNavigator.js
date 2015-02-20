/*
RightSideNavigator
*/
$HGRootNS.RightSideNavigator = function (element) {
	$HGRootNS.RightSideNavigator.initializeBase(this, [element]);
};
$HGRootNS.RightSideNavigator.prototype = {
	initialize: function () {
		$HGRootNS.RightSideNavigator.callBaseMethod(this, "initialize");

		this.initHandlers();
	},

	initHandlers: function () {
		var element = this.get_element();
		for (var i = 0; i < element.childNodes.length; i++) {
			var sub = element.childNodes[i];
			if (sub.tagName == "DIV") {
				$addHandler(sub, "click", Function.createDelegate(this, this.categoryClick));
				$addHandler(sub, "mouseover", Function.createDelegate(this, this.activateCategory));
				$addHandler(sub, "mouseout", Function.createDelegate(this, this.deActivateCategory));
			}
		}
	},

	categoryClick: function (e) {
		var evt = e || event;
		var id = evt.target.id ? evt.target.id : evt.target.parentNode.id;
		if (id && id.indexOf("panel") < 0 && (id.indexOf("divCustomerService") > 0 || id.indexOf("divLinkCategory_") > 0)) {
			var element = $get(id + "_panel");
			element.style.display = "block";
		}
	},

	activateCategory: function (e) {
		var evt = e || event;
		var id = evt.target.id ? evt.target.id : evt.target.parentNode.id;
		if (id && id.indexOf("panel") < 0) {
			var element = $get(id);
			if (element.className.indexOf("muiActive") < 0)
				element.className += " muiActive";
		}
	},

	deActivateCategory: function (e) {
		var evt = e || event;
		var id = evt.target.id ? evt.target.id : evt.target.parentNode.id;
		if (id && id.indexOf("panel") < 0) {
			var element = $get(id);
			if (element.className.indexOf("muiActive") >= 0)
				element.className = element.className.replace(" muiActive", "");
		}

		if (event.fromElement.id.indexOf("panel") > 0 && !this._isChildNode(event.fromElement, event.toElement) && event.toElement != event.fromElement.parentNode) {
			$get(event.fromElement.id).style.display = "none";
		}

		if (event.fromElement.parentNode == this.get_element() && !this._isChildNode(event.fromElement, event.toElement)) {
			$get(event.fromElement.id + "_panel").style.display = "none";
		}
		event.cancelBubble = true;
	},

	_isChildNode: function (parent, child) {
		while (child) {
			if (child.parentNode == parent) {
				return true;
			}
			child = child.parentNode;
		}

		return false;
	},

	dispose: function () {
		//调用基类方法
		$HGRootNS.RightSideNavigator.callBaseMethod(this, "dispose");
	}
};
$HGRootNS.RightSideNavigator.registerClass($HGRootNSName + ".RightSideNavigator", $HGRootNS.ControlBase);