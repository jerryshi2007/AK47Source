$HBRootNS.UserControlObjectMask = function () {
	throw Error.invalidOperation();
}

$HBRootNS.UserControlObjectMask.prototype =
{
	Organization: 1,
	User: 2,
	Group: 4,
	Sideline: 8,
	All: 15
}

$HBRootNS.UserControlObjectMask.registerEnum($HBRootNSName + '.UserControlObjectMask');

$HBRootNS.UserOUGraphControl = function (element) {
	$HBRootNS.UserOUGraphControl.initializeBase(this, [element]);

	this._rootPath = "";
	this._listMask = $HBRootNS.UserControlObjectMask.All;
	this._selectMask = $HBRootNS.UserControlObjectMask.All;
	this._multiSelect = false;
	this._mergeSelectResult = false;
	this._showDeletedObjects = false;
	this._treeControlID = null;
	this._tree = null;
	this._dialogResult = new Array();
	this._treeNodeSelectingDelegate = null;
	this._treeBeforeAllDataBindDelegate = null;
	this._treeAfterAllDataBindDelegate = null;
	this._treeAfterDataBindDelegate = null;
	this._treeNodeCheckBoxClickedDelegate = null;
	this._callBackContext = "";
	this._enableUserPresence = true;
	this._presenceElements = [];
}

$HBRootNS.UserOUGraphControl.prototype =
{
	initialize: function () {
		$HBRootNS.UserOUGraphControl.callBaseMethod(this, 'initialize');
	},

	dispose: function () {
		if (this._tree) {
			this._tree.remove_nodeSelecting(this._treeNodeSelectingDelegate);
			this._tree.remove_beforeAllDataBind(this._treeBeforeAllDataBindDelegate);
			this._tree.remove_afterAllDataBind(this._treeAfterAllDataBindDelegate);
			this._tree.remove_afterDataBind(this._treeAfterDataBindDelegate);
			this._tree.remove_nodeCheckBoxAfterClick(this._treeNodeCheckBoxClickedDelegate);
		}

		this._tree = null;
		this._dialogResult = null;

		$HBRootNS.UserOUGraphControl.callBaseMethod(this, 'dispose');
	},

	loadClientState: function (value) {
		if (value) {
			if (value != "") {
				var state = Sys.Serialization.JavaScriptSerializer.deserialize(value);
				var rootNodesData = state[0];

				if (window.dialogArguments && window.dialogArguments.ugcSelectedObjStr) {
					this._dialogResult = Sys.Serialization.JavaScriptSerializer.deserialize(window.dialogArguments.ugcSelectedObjStr);
				}
				else
					this._dialogResult = state[1];

				if (this._treeControlID) {
					this._tree = $find(this._treeControlID);
					this._tree._owner = this;

					this._treeNodeSelectingDelegate = Function.createDelegate(this, this._onTreeNodeSelecting);
					this._tree.add_nodeSelecting(this._treeNodeSelectingDelegate);

					this._treeBeforeAllDataBindDelegate = Function.createDelegate(this, this._onTreeBeforeAllDataBind);
					this._tree.add_beforeAllDataBind(this._treeBeforeAllDataBindDelegate);

					this._treeAfterAllDataBindDelegate = Function.createDelegate(this, this._onTreeAfterAllDataBind);
					this._tree.add_afterAllDataBind(this._treeAfterAllDataBindDelegate);

					this._treeAfterDataBindDelegate = Function.createDelegate(this, this._onTreeAfterDataBind);
					this._tree.add_afterDataBind(this._treeAfterDataBindDelegate);

					this._treeNodeCheckBoxClickedDelegate = Function.createDelegate(this, this._onTreeNodeCheckBoxClicked);
					this._tree.add_nodeCheckBoxAfterClick(this._treeNodeCheckBoxClickedDelegate);

					this._tree._dataBind(rootNodesData);
				}
			}
		}
	},

	saveClientState: function () {
		return Sys.Serialization.JavaScriptSerializer.serialize([this.get_selectedObjects()]);
	},

	get_callBackContext: function () {
		return this._callBackContext;
	},

	get_enableUserPresence: function () {
		return this._enableUserPresence;
	},

	set_enableUserPresence: function (value) {
		this._enableUserPresence = value;
	},

	set_callBackContext: function (value) {
		this._callBackContext = value;
	},

	get_listMask: function () {
		return this._listMask;
	},

	set_listMask: function (value) {
		this._listMask = value;
	},

	get_selectMask: function () {
		return this._selectMask;
	},

	set_selectMask: function (value) {
		this._selectMask = value;
	},

	get_multiSelect: function () {
		return this._multiSelect;
	},

	set_multiSelect: function (value) {
		this._multiSelect = value;
	},

	get_showDeletedObjects: function () {
		return this._showDeletedObjects;
	},

	set_showDeletedObjects: function (value) {
		this._showDeletedObjects = value;
	},

	get_rootPath: function () {
		return this._rootPath;
	},

	set_rootPath: function (value) {
		this._rootPath = value;
	},

	_prepareCloneablePropertyValues: function (newElement) {
		var properties = $HGRootNS.UserOUGraphControl.callBaseMethod(this, "_prepareCloneablePropertyValues");

		var treeCtrl = this._tree.cloneAndAppendToContainer(newElement);

		properties["treeControlID"] = treeCtrl.get_element().id;

		return properties;
	},

	get_cloneableProperties: function () {
		var baseProperties = $HGRootNS.UserOUGraphControl.callBaseMethod(this, "get_cloneableProperties");
		var currentProperties = ["enableUserPresence", "callBackContext", "listMask", "selectMask",
			"multiSelect", "mergeSelectResult"];

		Array.addRange(currentProperties, baseProperties);

		return currentProperties;
	},

	adjustDialogParameters: function (parameters) {
		parameters["ucpRoot"] = encodeURIComponent(this.get_rootPath());
		parameters["ucpListMask"] = this.get_listMask();
		parameters["ucpSelectMask"] = this.get_selectMask();
		parameters["ucpMultiSelect"] = this.get_multiSelect().toString();
		parameters["msr"] = this.get_mergeSelectResult().toString();
		parameters["sdo"] = this.get_showDeletedObjects().toString();
		parameters["eup"] = this.get_enableUserPresence().toString();
	},

	showDialog: function () {
		try {
			var params = { ugcSelectedObjStr: Sys.Serialization.JavaScriptSerializer.serialize(this._dialogResult) };

			var resultStr = this._showDialog(params);

			if (resultStr) {
				this._dialogResult = Sys.Serialization.JavaScriptSerializer.deserialize(resultStr);
				this.raiseDialogConfirmed();
			}
		}
		catch (e) {
			$showError(e);
		}
	},

	get_selectedObjects: function () {
		var result = null;

		if (this.get_mergeSelectResult())
			result = this.get_mergedSelectedObjects();
		else
			result = this.get_allSelectedObjects();

		return result;
	},

	clearSelectedObjects: function () {
		if (this.get_multiSelect()) {
			this._tree.clearMultiSelectedNodes();
			this._dialogResult = [];
		}
	},

	get_treeControlID: function () {
		return this._treeControlID;
	},

	set_treeControlID: function (value) {
		this._treeControlID = value;
	},

	get_mergeSelectResult: function () {
		return this._mergeSelectResult;
	},

	set_mergeSelectResult: function (value) {
		this._mergeSelectResult = value;
	},

	//events begin
	add_isChildrenOf: function (handler) {
		this.get_events().addHandler("isChildrenOf", handler);
	},

	remove_isChildrenOf: function (handler) {
		this.get_events().removeHandler("isChildrenOf", handler);
	},

	raiseIsChildrenOf: function (objSrc, objTarget) {
		var handlers = this.get_events().getHandler("isChildrenOf");

		var e = { execDefault: true, result: false };

		e.objSrc = objSrc;
		e.objTarget = objTarget;

		if (handlers) {
			handlers(this, e);
		}

		return e;
	},

	add_dialogConfirmed: function (handler) {
		this.get_events().addHandler("dialogConfirmed", handler);
	},

	remove_dialogConfirmed: function (handler) {
		this.get_events().removeHandler("dialogConfirmed", handler);
	},

	raiseDialogConfirmed: function () {
		var handlers = this.get_events().getHandler("dialogConfirmed");

		if (handlers)
			handlers(this);
	},

	add_nodeSelecting: function (handler) {
		this.get_events().addHandler("nodeSelecting", handler);
	},

	remove_nodeSelecting: function (handler) {
		this.get_events().removeHandler("nodeSelecting", handler);
	},

	raiseNodeSelecting: function (obj) {
		var handlers = this.get_events().getHandler("nodeSelecting");
		var continueExec = true;

		var e = new Sys.EventArgs();

		e.object = obj;
		e.cancel = false;

		if (!this.get_multiSelect()) {
			if ((this.get_selectMask() & obj.objectType) == 0)
				e.cancel = true;
		}
		else {
			e.cancel = true;
		}

		if (handlers)
			handlers(this, e);

		if (e.cancel)
			continueExec = false;

		return continueExec;
	},
	//events end

	_onTreeNodeSelecting: function (sender, e) {
		e.cancel = !this.raiseNodeSelecting(e.node.get_extendedData());
	},

	_onTreeBeforeAllDataBind: function (sender, e) {
		this._presenceElements = [];
	},

	_onTreeAfterAllDataBind: function (sender, e) {
		window.setTimeout(Function.createDelegate(this, function () {
			ProcessImnMarkersByDiv(this._presenceElements);
			this._presenceElements = [];
		}), 0);
	},

	_onTreeAfterDataBind: function (sender, e) {
		var obj = e.node.get_extendedData();

		if (obj.objectType == $HBRootNS.UserControlObjectMask.User
			&& this.get_enableUserPresence()
			&& obj.clientContext.IMAddress) {
			//ChangeImgToImnElement(e.node._nodeImg, obj.clientContext.IMAddress);
			var div = document.createElement("div");
			var parent = e.node._nodeImg.parentNode;
			parent.innerHTML = "";
			parent.appendChild(div);
			ChangeDivToImnElement(div, obj.clientContext.IMAddress);
			this._presenceElements.push(div);

		}

		var index = this._finfObjIDInDialogResult(obj.id);

		var parent = e.node.get_parent();

		if (parent) {
			if (this.get_mergeSelectResult()) {
				if (parent.get_checked()) {
					if (index == -1)
						Array.add(this._dialogResult, obj);

					index = this._dialogResult.length - 1;
				}
			}
		}

		if (index != -1) {
			if (this.get_multiSelect())
				e.node.set_checked(true);
			else
				e.node._tree.selectNode(e.node);
		}
	},

	_getLevelPath: function (obj) {
		return obj.fullPath;
	},

	_finfObjIDInDialogResult: function (id) {
		var result = -1;

		for (var i = 0; i < this._dialogResult.length; i++) {
			if (this._dialogResult[i].id == id) {
				result = i;
				break;
			}
		}

		return result;
	},

	_onTreeNodeCheckBoxClicked: function (sender, e) {
		var obj = e.node.get_extendedData();

		var index = this._finfObjIDInDialogResult(obj.id);

		if (e.node.get_checked() == false) {
			if (index >= 0)
				Array.removeAt(this._dialogResult, index);
		}
		else {
			if (index == -1)
				Array.add(this._dialogResult, obj);
		}

		if (this.get_mergeSelectResult()) {
			this._setAllChildChecked(e.node, e.node.get_checked());
			this._setParentChecked(e.node, e.node.get_checked());
		}
	},

	_setAllChildChecked: function (parentNode, checked) {
		var children = parentNode.get_children();

		for (var i = 0; i < children.length; i++) {
			var node = children[i];
			var obj = node.get_extendedData();

			if (node.get_showCheckBox()) {
				var index = this._finfObjIDInDialogResult(obj.id);

				if (checked) {
					if (index == -1) {
						Array.add(this._dialogResult, obj);
					}
				}
				else {
					if (index >= 0) {
						Array.removeAt(this._dialogResult, index);
					}
				}

				node.set_checked(checked);

				this._setAllChildChecked(node, checked);
			}
		}
	},

	_setParentChecked: function (node, checked) {
		if (node) {
			var parent = node.get_parent();

			if (parent && parent.get_showCheckBox()) {
				var obj = parent.get_extendedData();

				if (obj) {
					var parentCheck = false;

					if (checked) {
						parentCheck = parent.get_allChildrenChecked();
					}

					var index = this._finfObjIDInDialogResult(obj.id);

					if (parentCheck) {
						if (index == -1) {
							Array.add(this._dialogResult, obj);
						}
					}
					else {
						if (index >= 0) {
							Array.removeAt(this._dialogResult, index);
						}
					}

					parent.set_checked(parentCheck);
				}

				this._setParentChecked(parent, parentCheck);
			}
		}
	},

	get_mergedSelectedObjects: function () {
		var selectedObjects = this.get_allSelectedObjects();
		var result = new Array();

		for (var i = 0; i < selectedObjects.length; i++) {
			var objSrc = selectedObjects[i];
			var highestLevel = true;

			for (var j = 0; j < selectedObjects.length; j++) {
				if (i != j) {
					var objTarget = selectedObjects[j];

					if (this.isChildrenOf(objSrc, objTarget)) {
						highestLevel = false;
						break;
					}
				}
			}

			if (highestLevel)
				result.push(objSrc);
		}

		return result;
	},

	isChildrenOf: function (objSrc, objTarget) {
		var e = this.raiseIsChildrenOf(objSrc, objTarget);
		var result = e.result;

		if (e.execDefault) {
			var pathSrc = this._getLevelPath(objSrc);
			var pathTarget = this._getLevelPath(objTarget);

			result = (pathSrc.length > pathTarget.length) && pathSrc.indexOf(pathTarget) == 0;
		}

		return result;
	},

	get_allSelectedObjects: function () {
		var result = new Array();

		if (this.get_showingMode() == $HBRootNS.ControlShowingMode.Dialog)
			result = this._dialogResult;
		else {
			if (this.get_multiSelect())
				result = this._dialogResult;
			else {
				var node = this._tree.get_selectedNode();

				if (node)
					result = [node.get_extendedData()];
			}
		}

		return result;
	}
}

$HBRootNS.UserOUGraphControl.registerClass($HBRootNSName + ".UserOUGraphControl", $HBRootNS.DialogControlBase);