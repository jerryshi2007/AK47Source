$HBRootNS.GangedDataBindingControl = function (element) {
	$HBRootNS.GangedDataBindingControl.initializeBase(this, [element]);
	this._itemBindings = [];
	//	this._dataPropertyToControlsMapping = {};
	//	this._controlToDataPropertiesMapping = {};
	this._subscribeDic = {};
	this._loaded = false;
	this._readOnly = false;
	this._viewData = null;

	this._applicationLoad$delegate = Function.createDelegate(this, this._applicationLoad);
};

$HBRootNS.GangedDataBindingControl.prototype = {
	initialize: function () {
		Sys.Application.add_load(this._applicationLoad$delegate);

		$HBRootNS.GangedDataBindingControl.callBaseMethod(this, 'initialize');

		this._createDefaultViewData();

		this._initializeBindings();

		this.dataBind();
	},

	dispose: function () {
		if (this._applicationLoad$delegate) {
			Sys.Application.remove_load(this._applicationLoad$delegate);
			this._applicationLoad$delegate = null;
		}

		this._itemBindings = null;
		this._viewData = null;
	},

	_applicationLoad: function () {
		if (this._loaded == false) {
			this._loaded = true;
		}
	},

	get_readOnly: function () {
		return this._readOnly;
	},
	set_readOnly: function (value) {
		this._readOnly = value;
	},


	_initializeBindings: function () {
		var itemBindings = this._itemBindings;
		if (itemBindings.length > 0) {
			for (var i = 0; i < itemBindings.length; i++) {
				var item = itemBindings[i];
				item.BindingSettings = Sys.Serialization.JavaScriptSerializer.deserialize(item.BindingSettings);
			}
		}
	},

	_createDefaultViewData: function () {
		var handler = this.get_events().getHandler(this._createDefaultViewDataEventKey);

		var e = new Sys.EventArgs;
		e.viewData = null;

		if (handler) {
			handler(this, e);
		}

		this._viewData = e.viewData;

		return e;
	},



	dataBind: function () {
		var itemBindings = this._itemBindings;
		if (itemBindings.length > 0) {
			for (var i = 0; i < itemBindings.length; i++) {
				var item = itemBindings[i];

				this.dataBindByItemBinding(item, this._viewData);
			}
		}
	},

	dataBindByItemBinding: function (itemBinding, itemData) {
		this.bindDataToOneControl($get(itemBinding.ControlClientID), itemBinding.BindingSettings, itemData);
	},

	bindDataToOneControl: function (controlDomElement, bindingSettings, itemData) {
		var controlAdapter = this.createControlAdapter(controlDomElement, this);
		controlAdapter.dataBind(bindingSettings, itemData);
	},

	createControlAdapter: function (element) {
		switch (element.tagName.toLowerCase()) {
			case "select":
				return new $HBRootNS.SelectControlAdapter(element, this);
			case "span":
			case "label":
				return new $HBRootNS.LabelAdapter(element, this);
			case "input":
				if (element.type == "text") {
					return new $HBRootNS.TextBoxAdapter(element, this);
				}
				else if (element.type == "checkbox" || element.type == "radio") {
					return new $HBRootNS.CheckButtonAdapter(element, this);
				}
			default:
				return new $HBRootNS.CommonAdapter(element, this);
		}
	},

	makeObservable: function (defaultValue) {
		var property = { observable: true, value: defaultValue };
		property.handlers = {};
		property.controlAdapter = {};
		property.contextList = [];
		property.val = function () {
			if (arguments.length) {
				var old = this.value;
				var newValue = arguments[0];
				var fromElement = arguments[1];
				var context = arguments[2];

				if (old !== newValue) {
					if (typeof (arguments[2]) == "undefined") {
						context = {};
						this.contextList.push(context);
					} else {
						if (_containsContext(this.contextList, context)) {
							return;
						}
					}
					this.value = newValue;

					for (var ctrID in this.controlAdapter) {
						for (var adapterType in this.controlAdapter[ctrID]) {
							this.controlAdapter[ctrID][adapterType](this.value, fromElement);
						}

					}

					this.notifySubscribers(context);

					if (typeof (arguments[2]) == "undefined") {
						_removeContext(this.contextList, context);
					}
				}
			} else {
				return this.value;
			}
		};

		function _containsContext(contextList, context) {
			for (var i = 0; i < contextList.length; i++) {
				if (contextList[i] === context) {
					return true;
				}
			}
			return false;
		}

		function _removeContext(contextList, context) {
			for (var i = 0; i < contextList.length; i++) {
				if (contextList[i] === context) {
					Array.removeAt(contextList, i);
					break;
				}
			}
		}

		property.notifySubscribers = function (context) {
			var list = this.subscribers;
			if (list && list.length > 0) {
				for (var i = 0; i < list.length; i++) {
					if (list[i].computable) {
						if (typeof (list[i].compute) === "function") {
							var newValue = list[i].compute();
							list[i].val(newValue, null, context);
						}
					}
				}
			}
		};

		return property;
	},

	makeComputable: function (computeFunc, scope) {
		var property = this.makeObservable();
		property.computable = true;
		property.parent = scope;
		property.value = computeFunc.call(scope);

		property.compute = (function () {
			function ret() {
				return computeFunc.call(scope);
			}

			return ret;
		})();

		property.subscribeFrom = function (fromArray, scope) {
			for (var i = 0; i < fromArray.length; i++) {
				scope[fromArray[i]].subscribers = scope[fromArray[i]].subscribers || [];
				var contains = false;
				for (var j = 0; j < scope[fromArray[i]].subscribers.length; j++) {
					if (scope[fromArray[i]].subscribers[j] == this) {
						contains = true;
					}
				}
				if (!contains) {
					scope[fromArray[i]].subscribers.push(this);
				}
			}
		};

		return property;
	},

	isArray: function (v) {

		if (!v) return false;
		return Object.prototype.toString.call(v) == '[object Array]';
	},

	observableArray: function (array) {
		var internalArray = [],
            result;

		if (this.isArray(array)) {
			for (var i in array) {
				internalArray.push(this.makeObservable(array[i]));
			}
		}

		result = this.makeObservable(internalArray);

		var bindingControl = this;

		result.push = function (value) {
			var result;

			if (bindingControl.isArray(value)) {
				result = [];
				for (var i in value) {
					var temp = bindingControl.makeObservable(value[i]);
					result.push(temp);
					internalArray.push(temp);
				}

			} else {
				result = bindingControl.makeObservable(value);
				internalArray.push(result);
			}

			this.notifySubscribers();

			return result;
		};

		result.remove = function (value) {

			var index = -1;
			for (var i = 0; i < internalArray.length; i++) {
				var data = internalArray[i];

				if (value === data || value === data.val()) {
					index = i;
					break;
				}
			}

			if (index > -1) {
				internalArray.splice(index, 1);
				this.notifySubscribers();
			}
		};

		result.removeAll = function () {
			internalArray.length = 0;
			this.notifySubscribers();
		};

		return result;
	},

	raiseViewDataPropertyChange: function (propertyName, newValue) {
		this._viewData.setValue(propertyName, newValue);
	},

	get_viewData: function () {
		return this._viewData;
	},

	get_itemBindings: function () {
		return this._itemBindings;
	},

	set_itemBindings: function (value) {
		this._itemBindings = value;
	},

	//客户端创建默认ViewData时的事件
	_createDefaultViewDataEventKey: "createDefaultViewData",
	add_createDefaultViewData: function (value) {
		this.get_events().addHandler(this._createDefaultViewDataEventKey, value);
	},
	remove_createDefaultViewData: function (value) {
		this.get_events().removeHandler(this._createDefaultViewDataEventKey, value);
	}
};

$HBRootNS.GangedDataBindingControl.registerClass($HBRootNSName + ".GangedDataBindingControl", $HGRootNS.ControlBase);

$HBRootNS.ControlAdapterBase = function (controlElement, container) {
	$HBRootNS.ControlAdapterBase.initializeBase(this);
	this._controlElement = controlElement;
	this._container = container;
};

$HBRootNS.ControlAdapterBase.prototype = {
	get_readOnly: function () {
		return this._container.get_readOnly();
	},

	dataBind: function (bindingSettings, itemData) {

	},

	bindOptions: function (dataPropertyName, itemData) {

	},

	bindValue: function (dataPropertyName, itemData) {

	},

	bindVisible: function (dataPropertyName, itemData) {
		if (dataPropertyName) {
			var label = this._controlElement;
			var curData = itemData[dataPropertyName];
			var data = curData.observable ? curData.val() : curData;

			Sys.UI.DomElement.setVisible(label, data);

			if (curData.observable) {
				curData.controlAdapter[label.id] = curData.controlAdapter[label.id] || {};
				curData.controlAdapter[label.id].visible = function (value) {
					Sys.UI.DomElement.setVisible(label, value);
				};
			}
		}
	},

	bindCssClass: function (dataPropertyName, itemData) {
		if (dataPropertyName) {
			var label = this._controlElement;
			var curData = itemData[dataPropertyName];
			var data = curData.observable ? curData.val() : curData;

			label.className = data;

			if (curData.observable) {
				curData.controlAdapter[label.id] = curData.controlAdapter[label.id] || {};
				curData.controlAdapter[label.id].cssClass = function (value) {
					label.className = value;
				};
			}
		}
	}
};


$HBRootNS.ControlAdapterBase.registerClass($HBRootNSName + ".ControlAdapterBase");

$HBRootNS.SelectControlAdapter = function (controlElement, container) {
	$HBRootNS.SelectControlAdapter.initializeBase(this, [controlElement, container]);
};

$HBRootNS.SelectControlAdapter.prototype = {

	dataBind: function (bindingSettings, itemData) {
		$HBRootNS.SelectControlAdapter.callBaseMethod(this, 'dataBind', [bindingSettings, itemData]);

		this.bindOptions(bindingSettings["options"], bindingSettings["optionValue"], bindingSettings["optionName"], itemData);

		this.bindValue(bindingSettings["value"], itemData);

		this.bindVisible(bindingSettings["visible"], itemData);

		this.bindCssClass(bindingSettings["cssClass"], itemData);

		if (this.get_readOnly()) {
			this._controlElement.disabled = true;
		}
		else {
			this._controlElement.disabled = false;
		}
	},

	bindOptions: function (dataPropertyName, optionValue, optionName, itemData) {
		if (dataPropertyName) {
			var select = this._controlElement;
			var curData = itemData[dataPropertyName];
			var data = curData.observable ? curData.val() : curData;
			select.options.length = 0;
			for (var i = 0; i < data.length; i++) {
				var option = document.createElement("option");
				option.value = data[i][optionValue];
				option.text = data[i][optionName];
				option.innerText = data[i][optionName];
				select.appendChild(option);
			}

			if (curData.observable) {
				curData.controlAdapter[select.id] = curData.controlAdapter[select.id] || {};
				curData.controlAdapter[select.id].options = function (data, fromElement) {
					if (fromElement != select) {
						select.options.length = 0;
						for (var i = 0; i < data.length; i++) {
							var option = document.createElement("option");
							option.value = data[i][optionValue];
							option.text = data[i][optionName];
							option.innerText = data[i][optionName];
							select.appendChild(option);
						}
					}
				};
			}
		}
	},

	bindValue: function (valuePropertyName, itemData) {
		if (valuePropertyName) {
			var select = this._controlElement;
			var curData = itemData[valuePropertyName];
			var data = curData.observable ? curData.val() : curData;

			for (var j = 0; j < select.options.length; j++) {
				if (select.options[j].value == data) {
					select.options[j].selected = true;
				}
			}

			if (curData.observable) {
				if (!curData.handlers[select.id]) {

					$addHandler(select, "propertychange", Function.createDelegate(curData, this._handledSelectDataChanged));
					curData.handlers[select.id] = true;
				}

				curData.controlAdapter[select.id] = curData.controlAdapter[select.id] || {};
				curData.controlAdapter[select.id].value = function (value, fromElement) {
					if (fromElement != select) {
						for (var j = 0; j < select.options.length; j++) {
							if (select.options[j].value == value) {
								select.options[j].selected = true;
							}
						}
					}
				};
			}
		}
	},

	_handledSelectDataChanged: function (e) {
		var targetElement = e.target;
		if (event.propertyName === "selectedIndex") {
			if (targetElement.selectedIndex != -1) {
				this.val(targetElement.options[targetElement.selectedIndex].value, targetElement);
			}
		}
	}
};

$HBRootNS.SelectControlAdapter.registerClass($HBRootNSName + ".SelectControlAdapter", $HBRootNS.ControlAdapterBase);

$HBRootNS.TextBoxAdapter = function (controlElement, container) {
	$HBRootNS.TextBoxAdapter.initializeBase(this, [controlElement, container]);
};

$HBRootNS.TextBoxAdapter.prototype = {

	dataBind: function (bindingSettings, itemData) {
		$HBRootNS.TextBoxAdapter.callBaseMethod(this, 'dataBind', [bindingSettings, itemData]);

		this.bindValue(bindingSettings["value"], itemData);

		this.bindVisible(bindingSettings["visible"], itemData);

		this.bindCssClass(bindingSettings["cssClass"], itemData);

		if (this.get_readOnly()) {
			this._controlElement.readOnly = true;
		}
		else {
			this._controlElement.readOnly = false;
		}
	},

	bindValue: function (valuePropertyName, itemData) {
		if (valuePropertyName) {
			var textBox = this._controlElement;
			textBox.autocomplete = "off";
			var curData = itemData[valuePropertyName];
			var data = curData.observable ? curData.val() : curData;

			textBox.value = data;

			if (curData.observable) {
				if (!curData.handlers[textBox.id]) {

					$addHandler(textBox, "change", Function.createDelegate(curData, this._handledDataChanged));
					curData.handlers[textBox.id] = true;
				}

				curData.controlAdapter[textBox.id] = curData.controlAdapter[textBox.id] || {};
				curData.controlAdapter[textBox.id].value = function (value, fromElement) {
					if (fromElement != textBox) {
						textBox.value = value;
					}
				};
			}
		}
	},

	_handledDataChanged: function (e) {
		var targetElement = e.target;
		this.val(targetElement.value, targetElement);
	}
};

$HBRootNS.TextBoxAdapter.registerClass($HBRootNSName + ".TextBoxAdapter", $HBRootNS.ControlAdapterBase);

$HBRootNS.CheckButtonAdapter = function (controlElement, container) {
	$HBRootNS.CheckButtonAdapter.initializeBase(this, [controlElement, container]);
};

$HBRootNS.CheckButtonAdapter.prototype = {

	dataBind: function (bindingSettings, itemData) {
		$HBRootNS.CheckButtonAdapter.callBaseMethod(this, 'dataBind', [bindingSettings, itemData]);

		this.bindValue(bindingSettings["value"], itemData);

		this.bindVisible(bindingSettings["visible"], itemData);

		this.bindCssClass(bindingSettings["cssClass"], itemData);

		if (this.get_readOnly()) {
			this._controlElement.readOnly = true;
		}
		else {
			this._controlElement.readOnly = false;
		}
	},

	bindValue: function (valuePropertyName, itemData) {
		if (valuePropertyName) {
			var checkButton = this._controlElement;
			var curData = itemData[valuePropertyName];
			var data = curData.observable ? curData.val() : curData;

			checkButton.checked = data == checkButton.value;

			if (curData.observable) {
				if (!curData.handlers[checkButton.id]) {

					$addHandler(checkButton, "click", Function.createDelegate(curData, this._handledDataChanged));
					curData.handlers[checkButton.id] = true;
				}

				curData.controlAdapter[checkButton.id] = curData.controlAdapter[checkButton.id] || {};
				curData.controlAdapter[checkButton.id].value = function (value, fromElement) {
					if (fromElement != checkButton) {
						checkButton.checked = value == checkButton.value;
					}
				};
			}
		}
	},

	_handledDataChanged: function (e) {
		var targetElement = e.target;
		if (targetElement.checked) {
			this.val(targetElement.value, targetElement);
		} else {
			this.val("", targetElement);
		}
	}
};

$HBRootNS.CheckButtonAdapter.registerClass($HBRootNSName + ".CheckButtonAdapter", $HBRootNS.ControlAdapterBase);

$HBRootNS.LabelAdapter = function (controlElement, container) {
	$HBRootNS.LabelAdapter.initializeBase(this, [controlElement, container]);
};

$HBRootNS.LabelAdapter.prototype = {
	dataBind: function (bindingSettings, itemData) {
		$HBRootNS.LabelAdapter.callBaseMethod(this, 'dataBind', [bindingSettings, itemData]);

		this.bindValue(bindingSettings["value"], itemData, bindingSettings["format"]);

		this.bindVisible(bindingSettings["visible"], itemData);

		this.bindCssClass(bindingSettings["cssClass"], itemData);

		if (this.get_readOnly()) {
			this._controlElement.readOnly = true;
		}
		else {
			this._controlElement.readOnly = false;
		}
	},

	bindValue: function (valuePropertyName, itemData, format) {
		if (valuePropertyName) {
			var label = this._controlElement;
			var curData = itemData[valuePropertyName];
			var data = curData.observable ? curData.val() : curData;

			label.innerText = data;

			if (curData.observable) {
				curData.controlAdapter[label.id] = curData.controlAdapter[label.id] || {};
				curData.controlAdapter[label.id].value = function (value, fromElement) {
					if (fromElement != label) {
						if (format) {
							label.innerText = String.format(format, value);
						} else {
							label.innerText = value;
						}

					}
				};
			}
		}
	}
};

$HBRootNS.LabelAdapter.registerClass($HBRootNSName + ".LabelAdapter", $HBRootNS.ControlAdapterBase);

$HBRootNS.CommonAdapter = function (controlElement, container) {
	$HBRootNS.CommonAdapter.initializeBase(this, [controlElement, container]);
};

$HBRootNS.CommonAdapter.prototype = {
	dataBind: function (bindingSettings, itemData) {
		$HBRootNS.CommonAdapter.callBaseMethod(this, 'dataBind', [bindingSettings, itemData]);

		this.bindVisible(bindingSettings["visible"], itemData);

		this.bindCssClass(bindingSettings["cssClass"], itemData);
	}
};

$HBRootNS.CommonAdapter.registerClass($HBRootNSName + ".CommonAdapter", $HBRootNS.ControlAdapterBase);