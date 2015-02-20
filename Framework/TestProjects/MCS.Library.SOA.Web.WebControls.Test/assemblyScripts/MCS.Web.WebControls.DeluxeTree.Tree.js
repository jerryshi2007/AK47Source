$HGRootNS.DeluxeTree = function (element) {
	$HGRootNS.DeluxeTree.initializeBase(this, [element]);

	this._nodeOpenImg = "";
	this._nodeCloseImg = "";
	this._expandImage = "";
	this._collapseImage = "";
	this._defaultWaitingForImage = "";
	this._defaultExpandImage = "";
	this._defaultCollapseImage = "";
	this._nodeIndent = 16;
	this._showCheckBox = false;
	this._supportPostBack = false;

	var elt = $HGDomElement.get_currentDocument().createElement("div");

	elt.style.textAlign = "left";
	this._rootNode = $create($HGRootNS.DeluxeTreeNode, null, null, null, elt);
	this._rootNode._tree = this;
	this._rootNode._expanded = true;

	this._selectedNode = null;
	this._lastNodeCssClass = "";
	this._callBackContext = "";

	var childNodeContainer = $HGDomElement.get_currentDocument().createElement("div");
	this._rootNode.set_childNodeContainer(childNodeContainer);

	elt.appendChild(childNodeContainer);

	element.appendChild(elt);
}

$HGRootNS.DeluxeTree.prototype =
{
	initialize: function () {
		$HGRootNS.DeluxeTree.callBaseMethod(this, 'initialize');
		this._initialize();
	},

	dispose: function () {
		this._rootNode = null;
		this._selectedNode = null;
		$HGRootNS.DeluxeTree.callBaseMethod(this, 'dispose');
	},

	get_callBackContext: function () {
		return this._callBackContext;
	},

	set_callBackContext: function (value) {
		this._callBackContext = value;
	},

	get_nodeOpenImg: function () {
		return this._nodeOpenImg;
	},

	set_nodeOpenImg: function (value) {
		this._nodeOpenImg = value;
	},

	get_nodeCloseImg: function () {
		return this._nodeCloseImg;
	},

	set_nodeCloseImg: function (value) {
		this._nodeCloseImg = value;
	},

	get_expandImage: function () {
		var result = this._expandImage;

		if (result == "")
			result = this.get_defaultExpandImage();

		return result;
	},

	set_expandImage: function (value) {
		this._expandImage = value;
	},

	get_defaultWaitingForImage: function () {
		return this._defaultWaitingForImage;
	},

	set_defaultWaitingForImage: function (value) {
		this._defaultWaitingForImage = value;
	},

	get_defaultExpandImage: function () {
		return this._defaultExpandImage;
	},

	set_defaultExpandImage: function (value) {
		this._defaultExpandImage = value;
	},

	get_collapseImage: function () {
		var result = this._collapseImage;

		if (result == "")
			result = this.get_defaultCollapseImage();

		return result;
	},

	set_collapseImage: function (value) {
		this._collapseImage = value
	},

	get_defaultCollapseImage: function () {
		return this._defaultCollapseImage;
	},

	set_defaultCollapseImage: function (value) {
		this._defaultCollapseImage = value;
	},

	get_nodeIndent: function () {
		return this._nodeIndent;
	},

	set_nodeIndent: function (value) {
		this._nodeIndent = value;
	},

	get_showCheckBox: function () {
		return this._showCheckBox;
	},

	set_showCheckBox: function (value) {
		this._showCheckBox = value;
	},

	get_supportPostBack: function () {
		return this._supportPostBack;
	},

	set_supportPostBack: function (value) {
		this._supportPostBack = value;
	},

	get_nodes: function () {
		return this._rootNode.get_children();
	},

	get_selectedNode: function () {
		return this._selectedNode;
	},

	selectNode: function (node) {
		if (node) {
			node.set_selected(true);
		}
	},

	get_multiSelectedNodes: function () {
		var resultArray = new Array();

		this._getSelectedChildren(this._rootNode, resultArray);

		return resultArray;
	},

	clearMultiSelectedNodes: function () {
		this._clearMultiSelectedChildren(this._rootNode);
	},

	_clearMultiSelectedChildren: function (parent) {
		var children = parent.get_children();

		for (var i = 0; i < children.length; i++) {
			var child = children[i];

			if (child.get_checked())
				child.set_checked(false);
		}

		for (var i = 0; i < children.length; i++)
			this._clearMultiSelectedChildren(children[i]);
	},

	_getSelectedChildren: function (parent, resultArray) {
		var children = parent.get_children();

		for (var i = 0; i < children.length; i++) {
			var child = children[i];

			if (child.get_checked())
				resultArray.push(child);
		}

		for (var i = 0; i < children.length; i++)
			this._getSelectedChildren(children[i], resultArray);
	},

	_initialize: function () {
	},

	loadClientState: function (value) {
		if (value) {
			if (value != "") {
				var data = Sys.Serialization.JavaScriptSerializer.deserialize(value);

				var nodes = data[2];
				this.get_element().attachEvent("onselectstart", new Function("return false;"));
				this.get_element().style.overflow = "auto";

				var cb = Function.createDelegate(this, function () {
					var div = this.get_element();

					div.scrollLeft = data[0];
					div.scrollTop = data[1];
				});

				this._dataBind(nodes, cb);
			}
		}
	},

	saveClientState: function () {
		var data = "";

		if (this._isInvoking == false && this._supportPostBack) {
			var div = this.get_element();

			state = new Array(3);

			state[0] = div.scrollLeft;
			state[1] = div.scrollTop;
			state[2] = this._getTreeCanSerializedData(this._rootNode.get_children());

			data = Sys.Serialization.JavaScriptSerializer.serialize(state);
		}

		return data;
	},

	_dataBind: function (nodesData, cb) {
		this._rootNode.dataBind(nodesData, cb);
	},

	_getTreeCanSerializedData: function (nodes) {
		var nodePack = new Array(nodes.length);

		for (var i = 0; i < nodes.length; i++) {
			var node = nodes[i]._toCanSerializedObject();
			node.nodes = this._getTreeCanSerializedData(nodes[i].get_children());

			nodePack[i] = node;
		}

		return nodePack;
	},

	//Begin events
	//selecting
	add_nodeSelecting: function (handler) {
		this.get_events().addHandler("nodeSelecting", handler);
	},

	remove_nodeSelecting: function (handler) {
		this.get_events().removeHandler("nodeSelecting", handler);
	},

	raiseNodeSelecting: function (node, eventElement) {
		var handlers = this.get_events().getHandler("nodeSelecting");
		var continueExec = true;

		if (handlers) {
			var e = new Sys.EventArgs();

			e.node = node;
			e.cancel = false;
			e.eventElement = eventElement;

			handlers(this, e);

			if (e.cancel)
				continueExec = false;
		}

		return continueExec;
	},

	add_nodeContextMenu: function (handler) {
		this.get_events().addHandler("nodeContextMenu", handler);
	},

	remove_nodeContextMenu: function (handler) {
		this.get_events().removeHandler("nodeContextMenu", handler);
	},

	raiseNodeContextMenu: function (node, eventElement) {
		var handlers = this.get_events().getHandler("nodeContextMenu");
		var defaultContextMenu = false;

		if (handlers) {
			var e = new Sys.EventArgs();

			e.node = node;
			e.defaultContextMenu = defaultContextMenu;
			e.eventElement = eventElement;

			handlers(this, e);

			defaultContextMenu = e.defaultContextMenu;
		}

		return defaultContextMenu;
	},

	add_nodeDblClick: function (handler) {
		this.get_events().addHandler("nodeDblClick", handler);
	},

	remove_nodeDblClick: function (handler) {
		this.get_events().removeHandler("nodeDblClick", handler);
	},

	raiseNodeDblClick: function (node, eventElement) {
		var handlers = this.get_events().getHandler("nodeDblClick");

		if (handlers) {
			var e = new Sys.EventArgs();

			e.node = node;
			e.eventElement = eventElement;

			handlers(this, e);
		}
	},

	add_nodeBeforeExpand: function (handler) {
		this.get_events().addHandler("nodeBeforeExpand", handler);
	},

	remove_nodeBeforeExpand: function (handler) {
		this.get_events().removeHandler("nodeBeforeExpand", handler);
	},

	raiseNodeBeforeExpand: function (node, eventElement) {
		var handlers = this.get_events().getHandler("nodeBeforeExpand");
		var continueExec = true;

		if (handlers) {
			var e = new Sys.EventArgs();

			e.node = node;
			e.cancel = false;
			e.eventElement = eventElement;

			handlers(this, e);

			if (e.cancel)
				continueExec = false;
		}

		return continueExec;
	},

	add_nodeAfterExpand: function (handler) {
		this.get_events().addHandler("nodeAfterExpand", handler);
	},

	remove_nodeAfterExpand: function (handler) {
		this.get_events().removeHandler("nodeAfterExpand", handler);
	},

	raiseNodeAfterExpand: function (node, eventElement) {
		var handlers = this.get_events().getHandler("nodeAfterExpand");

		if (handlers) {
			var e = new Sys.EventArgs();

			e.node = node;
			e.eventElement = eventElement;

			handlers(this, e);
		}
	},

	add_nodeCheckBoxBeforeClick: function (handler) {
		this.get_events().addHandler("nodeCheckBoxBeforeClick", handler);
	},

	remove_nodeCheckBoxBeforeClick: function (handler) {
		this.get_events().removeHandler("nodeCheckBoxBeforeClick", handler);
	},

	raiseNodeCheckBoxBeforeClick: function (node, eventElement) {
		var handlers = this.get_events().getHandler("nodeCheckBoxBeforeClick");
		var continueExec = true;

		if (handlers) {
			var e = new Sys.EventArgs();

			e.node = node;
			e.cancel = false;
			e.eventElement = eventElement;

			handlers(this, e);

			if (e.cancel)
				continueExec = false;
		}

		return continueExec;
	},

	add_nodeCheckBoxAfterClick: function (handler) {
		this.get_events().addHandler("nodeCheckBoxAfterClick", handler);
	},

	remove_nodeCheckBoxAfterClick: function (handler) {
		this.get_events().removeHandler("nodeCheckBoxAfterClick", handler);
	},

	raiseNodeCheckBoxAfterClick: function (node, eventElement) {
		var handlers = this.get_events().getHandler("nodeCheckBoxAfterClick");

		if (handlers) {
			var e = new Sys.EventArgs();

			e.node = node;
			e.eventElement = eventElement;

			handlers(this, e);
		}
	},

	add_beforeDataBind: function (handler) {
		this.get_events().addHandler("beforeDataBind", handler);
	},

	remove_beforeDataBind: function (handler) {
		this.get_events().removeHandler("beforeDataBind", handler);
	},

	raiseBeforeDataBind: function (node) {
		var handlers = this.get_events().getHandler("beforeDataBind");
		var continueExec = true;

		if (handlers) {
			var e = new Sys.EventArgs();

			e.node = node;
			e.cancel = false;

			handlers(this, e);

			if (e.cancel)
				continueExec = false;
		}

		return continueExec;
	},

	add_afterDataBind: function (handler) {
		this.get_events().addHandler("afterDataBind", handler);
	},

	remove_afterDataBind: function (handler) {
		this.get_events().removeHandler("afterDataBind", handler);
	},

	raiseAfterDataBind: function (node) {
		var handlers = this.get_events().getHandler("afterDataBind");

		if (handlers) {
			var e = new Sys.EventArgs();

			e.node = node;

			handlers(this, e);
		}
	}
	//selecting
	//end events
}

$HGRootNS.DeluxeTree.registerClass($HGRootNSName + ".DeluxeTree", $HGRootNS.ControlBase);