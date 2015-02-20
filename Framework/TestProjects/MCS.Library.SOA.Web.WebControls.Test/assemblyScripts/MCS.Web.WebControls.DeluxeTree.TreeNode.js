$HGRootNS.ChildNodesLoadingTypeDefine = function() {
	throw Error.invalidOperation();
}

$HGRootNS.ChildNodesLoadingTypeDefine.prototype =
{
	Normal: 0,
	LazyLoading: 1
}

$HGRootNS.ChildNodesLoadingTypeDefine.registerEnum($HGRootNSName + '.ChildNodesLoadingTypeDefine');

$HGRootNS.VerticalAlign = function() {
	throw Error.invalidOperation();
}

$HGRootNS.VerticalAlign.prototype =
{
	NotSet: 0,
	Top: 1,
	Middle: 2,
	Bottom: 3
}

$HGRootNS.VerticalAlign.registerEnum($HGRootNSName + '.VerticalAlign');

$HGRootNS.DeluxeTreeNode = function(element) {
	$HGRootNS.DeluxeTreeNode.initializeBase(this, [element]);

	this._nodeOpenImg = "";
	this._nodeCloseImg = "";
	this._text = "";
	this._html = "";
	this._imgWidth = "";
	this._imgHeight = "";
	this._imgMarginLeft = "";
	this._imgMarginTop = "";
	this._value = "";
	this._toolTip = "";
	this._enableToolTip = true;
	this._expanded = false;
	this._selected = false;
	this._checked = false;
	this._showCheckBox = false;
	this._navigateUrl = "";
	this._target = "";
	this._subNodesLoaded = false;
	this._extendedData = null;
	this._textNoWrap = true;
	this._nodeVerticalAlign = $HGRootNS.VerticalAlign.NotSet;
	this._cssClass = "";
	this._selectedCssClass = "";
	this._childNodesLoadingType = $HGRootNS.ChildNodesLoadingTypeDefine.Normal;
	this._lazyLoadingText = "正在加载...";
	this._extendedDataKey = "";

	this._tree = null;

	this._expandImg = null;
	this._expandImg$delegate = {
		click: Function.createDelegate(this, this._expandImg_onclick),
		contextmenu: Function.createDelegate(this, this._node_onContextMenu)
	}

	this._textNode = null;
	this._textNode$delegate = {
		click: Function.createDelegate(this, this._textNode_onclick),
		contextmenu: Function.createDelegate(this, this._node_onContextMenu),
		dblclick: Function.createDelegate(this, this._node_ondblclick)
	}

	this._nodeImg = null;
	this._nodeImg$delegate = this._textNode$delegate;

	this._checkbox = null;
	this._checkbox$delegate = {
		click: Function.createDelegate(this, this._checkbox_onclick)
	}

	this._extendedCell = null;
}

$HGRootNS.DeluxeTreeNode.InLoadingMode = false;

$HGRootNS.DeluxeTreeNode.createNode = function(properties) {
	var elt = $HGDomElement.get_currentDocument().createElement("div");

	var newNode = $create($HGRootNS.DeluxeTreeNode,
						properties,
						null,
						null,
						elt);

	return newNode;
}

$HGRootNS.DeluxeTreeNode.prototype =
{
	initialize: function () {
		$HGRootNS.DeluxeTreeNode.callBaseMethod(this, 'initialize');
		this._initialize();
	},

	dispose: function () {
		if (this._expandImg) {
			$HGDomEvent.removeHandlers(this._expandImg, this._expandImg$delegate);
			this._expandImg = null;
		}

		if (this._nodeImg) {
			$HGDomEvent.removeHandlers(this._nodeImg, this._nodeImg$delegate);
			this._nodeImg = null;
		}

		if (this._textNode) {
			$HGDomEvent.removeHandlers(this._textNode, this._textNode$delegate);
			this._textNode = null;
		}

		if (this._checkbox) {
			$HGDomEvent.removeHandlers(this._checkbox, this._checkbox$delegate);
			this._checkbox = null;
		}

		this._extendedData = null;
		this._extendedCell = null;
		this._tree = null;
		$HGRootNS.DeluxeTreeNode.callBaseMethod(this, 'dispose');
	},

	dataBind: function (nodesData, cb, batchSize) {
		this.clear();

		if (nodesData) {
			if (cb) {
				this._bindDataSeperatly(nodesData, cb, batchSize);
			}
			else {
				this._bindAllData(nodesData);
			}
		}
	},

	_bindDataSeperatly: function (nodesData, cb, batchSize) {
		var processCount = 0;

		if (!batchSize)
			batchSize = 25;

		var bindFunc = Function.createDelegate(this, function () {
			var i = processCount;

			while (i < nodesData.length && i < processCount + batchSize) {
				var srcNode = nodesData[i];
				this._bindOneNodeData(srcNode, cb, batchSize);

				i++;
			}

			processCount = i;

			if (processCount < nodesData.length)
				window.setTimeout(bindFunc, 0);
		});

		bindFunc();
		cb();
	},

	_bindAllData: function (nodesData) {
		for (var i = 0; i < nodesData.length; i++) {
			var srcNode = nodesData[i];
			this._bindOneNodeData(srcNode);
		}
	},

	_bindOneNodeData: function (srcNode, cb, batchSize) {
		var properties = this._createPropertiesFromSrcNode(srcNode);

		var newNode = $HGRootNS.DeluxeTreeNode.createNode(properties);

		if (this._tree.raiseBeforeDataBind(newNode)) {
			this.appendChild(newNode);

			this._tree.raiseAfterDataBind(newNode);

			if (newNode.get_childNodesLoadingType() == $HGRootNS.ChildNodesLoadingTypeDefine.LazyLoading
				&& !newNode._subNodesLoaded) {
				var subNode = $HGRootNS.DeluxeTreeNode.createNode(
									{
										nodeOpenImg: this._tree.get_defaultWaitingForImage(),
										nodeCloseImg: this._tree.get_defaultWaitingForImage(),
										text: newNode.get_lazyLoadingText()
									}
								)
				newNode.appendChild(subNode);
			}
			else
				newNode.dataBind(srcNode.nodes, cb, batchSize);
		}
	},

	_createPropertiesFromSrcNode: function (srcNode) {
		return {
			nodeOpenImg: srcNode.nodeOpenImg,
			nodeCloseImg: srcNode.nodeCloseImg,
			text: srcNode.text,
			imgWidth: srcNode.imgWidth,
			imgHeight: srcNode.imgHeight,
			imgMarginLeft: srcNode.imgMarginLeft,
			imgMarginTop: srcNode.imgMarginTop,
			html: srcNode.html,
			value: srcNode.value,
			toolTip: srcNode.toolTip,
			enableToolTip: srcNode.enableToolTip,
			expanded: srcNode.expanded,
			subNodesLoaded: srcNode.subNodesLoaded,
			checked: srcNode.checked,
			selected: srcNode.selected,
			showCheckBox: srcNode.showCheckBox,
			textNoWrap: srcNode.textNoWrap,
			nodeVerticalAlign: srcNode.nodeVerticalAlign,
			extendedData: srcNode.extendedData,
			navigateUrl: srcNode.navigateUrl,
			target: srcNode.target,
			cssClass: srcNode.cssClass,
			selectedCssClass: srcNode.selectedCssClass,
			childNodesLoadingType: srcNode.childNodesLoadingType,
			lazyLoadingText: srcNode.lazyLoadingText,
			extendedDataKey: srcNode.extendedDataKey
		};
	},

	_initialize: function () {
	},

	_toCanSerializedObject: function () {
		return {
			nodeOpenImg: this.get_nodeOpenImg(),
			nodeCloseImg: this.get_nodeCloseImg(),
			text: this.get_text(),
			html: this.get_html(),
			value: this.get_value(),
			imgWidth: this.get_imgWidth(),
			imgHeight: this.get_imgHeight(),
			imgMarginLeft: this.get_imgMarginLeft(),
			imgMarginTop: this.get_imgMarginTop(),
			toolTip: this.get_toolTip(),
			enableToolTip: this.get_enableToolTip(),
			expanded: this.get_expanded(),
			subNodesLoaded: this.get_subNodesLoaded(),
			checked: this.get_checked(),
			selected: this.get_selected(),
			showCheckBox: this.get_showCheckBox(),
			textNoWrap: this.get_textNoWrap(),
			nodeVerticalAlign: this.get_nodeVerticalAlign(),
			navigateUrl: this.get_navigateUrl(),
			target: this.get_target(),
			extendedDataKey: this.get_extendedDataKey(),
			extendedData: this.get_extendedDataKey() && this.get_extendedDataKey() != "" ?
					$Serializer.setType(this.get_extendedData(), this.get_extendedDataKey()) :
					this.get_extendedData(),
			cssClass: this.get_cssClass(),
			selectedCssClass: this.get_selectedCssClass(),
			childNodesLoadingType: this.get_childNodesLoadingType(),
			lazyLoadingText: this.get_lazyLoadingText()
		}
	},

	_autoLoadingSubNodes: function (eventElement) {
		if (this.get_childNodesLoadingType() == $HGRootNS.ChildNodesLoadingTypeDefine.LazyLoading
			&& !this._subNodesLoaded) {
			if (this._tree) {
				if ($HGRootNS.DeluxeTreeNode.InLoadingMode == false) {
					this._tree._staticInvoke("GetChildren", [this._toCanSerializedObject(), this._tree.get_callBackContext()],
					    Function.createDelegate(this, this._loadChildrenCallback),
					    Function.createDelegate(this, this._loadChildrenErrorCallback));

					$HGRootNS.DeluxeTreeNode.InLoadingMode = true;
				}
				else
					this.set_expanded(false);
			}
		}
	},

	_loadChildrenErrorCallback: function (err) {
		alert(String.format("Call method {0} exception: {1}", "GetChildren", err.message));
		$HGRootNS.DeluxeTreeNode.InLoadingMode = false;

		this.set_expanded(false);
	},

	_loadChildrenCallback: function (result) {
		$HGRootNS.DeluxeTreeNode.InLoadingMode = false;

		this.clearChildren();

		this.dataBind(result, Function.createDelegate(this, this._dataBindCallBack));
	},

	_dataBindCallBack: function () {
		if (this.get_children().length == 0) {
			this.get_childNodeContainer().style.display = "none";
			this._expandImg.style.visibility = "hidden"
		}

		this._subNodesLoaded = true;

		this._tree.raiseNodeAfterExpand(this, null);
	},

	//begin delegates
	_expandImg_onclick: function (eventElement) {
		var continueExec = true;
		var originalExpanded = this.get_expanded();

		if (originalExpanded == false)
			continueExec = this._tree.raiseNodeBeforeExpand(this, eventElement);

		if (continueExec) {
			this.set_expanded(!originalExpanded);

			if (originalExpanded == false &&
				this._childNodesLoadingType == $HGRootNS.ChildNodesLoadingTypeDefine.Normal)
				this._tree.raiseNodeAfterExpand(this, eventElement);
		}
	},

	_textNode_onclick: function (eventElement) {
		if (this._tree.raiseNodeSelecting(this, eventElement)) {
			this._tree.selectNode(this);

			var url = this.get_navigateUrl();

			if (url && url != "")
				this._openLink(url, this.get_target());
		}
	},

	_node_onContextMenu: function (eventElement) {
		return this._tree.raiseNodeContextMenu(this, eventElement);
	},

	_node_ondblclick: function (eventElement) {
		return this._tree.raiseNodeDblClick(this, eventElement);
	},

	_checkbox_onclick: function (eventElement) {
		var continueExec = this._tree.raiseNodeCheckBoxBeforeClick(this, eventElement);

		if (continueExec) {
			this.set_checked(eventElement.target.checked);
			this._tree.raiseNodeCheckBoxAfterClick(this, eventElement);
		}

		return continueExec;
	},
	//end delegates

	//begin method
	drawNode: function () {
		var elt = this.get_element();

		elt.innerHTML = "";
		this._buildChildElements(elt);
	},

	_createExpandImg: function () {
		var doc = $HGDomElement.get_currentDocument();

		var img = doc.createElement("img");
		var src = this.get_expanded() ?
			this._tree.get_collapseImage() : this._tree.get_expandImage();

		img.src = src;
		img.style.cursor = "hand";
		img.isExpander = true;
		img.expanded = this.get_expanded();
		img.style.visibility = "hidden";

		$addHandlers(img, this._expandImg$delegate);

		return img;
	},

	_createNodeImg: function () {
		var doc = $HGDomElement.get_currentDocument();

		var img = doc.createElement("img");
		var ncg = this.get_nodeCloseImg();

		if (ncg == "")
			img.style.display = "none";
		else
			img.src = ncg;

		img.style.cursor = "hand";

		$addHandlers(img, this._nodeImg$delegate)

		return img;
	},

	_createCheckBox: function () {
		var doc = $HGDomElement.get_currentDocument();

		var chk = doc.createElement("input");
		chk.type = "checkbox";
		chk.checked = this.get_checked();

		$addHandlers(chk, this._checkbox$delegate);

		return chk;
	},

	_setExpanded: function (value) {
		var expandImgSrc = "";
		var nodeImgSrc = "";

		if (value) {
			expandImgSrc = this._tree.get_collapseImage();
			nodeImgSrc = this.get_nodeOpenImg();

			this.get_childNodeContainer().style.display = "block";
		}
		else {
			expandImgSrc = this._tree.get_expandImage();
			nodeImgSrc = this.get_nodeCloseImg();

			this.get_childNodeContainer().style.display = "none";
		}

		if (this._expandImg)
			this._expandImg.src = expandImgSrc;

		if (this._nodeImg) {
			if (nodeImgSrc != "") {
				this._nodeImg.src = nodeImgSrc;
				this._nodeImg.style.display = "inline";
			}
			else
				this._nodeImg.style.display = "none";
		}
	},

	_buildChildElements: function (elt) {
		var doc = $HGDomElement.get_currentDocument();

		var labelTable = doc.createElement("table");
		elt.appendChild(labelTable);

		labelTable.cellPadding = 0;
		labelTable.cellSpacing = 0;

		this._expandImg = this._createExpandImg();
		var newRow = labelTable.insertRow();
		var newCell = this._insertNodeCell(newRow);

		newCell.appendChild(this._expandImg);

		newCell = this._insertNodeCell(newRow);
		if (this.get_showCheckBox()) {
			this._checkbox = this._createCheckBox();
			newCell.appendChild(this._checkbox);
		}
		else {
			newCell.innerText = " ";
			newCell.style.width = "1px";
		}

		newCell = this._insertNodeCell(newRow);

		var nodeImgContainer = doc.createElement("span");
		nodeImgContainer.style.overflow = "hidden";
		nodeImgContainer.style.display = "inline-block";
		nodeImgContainer.style.cursor = "hand";

		newCell.appendChild(nodeImgContainer);

		this._nodeImg = this._createNodeImg();

		if (this.get_imgWidth())
			nodeImgContainer.style.width = this.get_imgWidth();

		if (this.get_imgHeight())
			nodeImgContainer.style.height = this.get_imgHeight();

		if (this.get_imgMarginLeft())
			this._nodeImg.style.marginLeft = this.get_imgMarginLeft();

		if (this.get_imgMarginTop())
			this._nodeImg.style.marginTop = this.get_imgMarginTop();

		nodeImgContainer.appendChild(this._nodeImg);

		newCell = this._insertNodeCell(newRow);

		this._textNode = newCell;

		newCell.style.paddingLeft = "3px";
		newCell.noWrap = this.get_textNoWrap();

		if (!newCell.noWrap)
			newCell.style.wordBreak = "break-all";

		if (this.get_html() && this.get_html() != "")
			newCell.innerHTML = this.get_html();
		else
			newCell.innerText = this.get_text();

		if (this.get_enableToolTip())
			if (this.get_toolTip())
				newCell.title = this.get_toolTip();
			else
				newCell.title = newCell.innerText;

		if (this.get_selected())
			this._set_selected(this.get_selected());
		else
			newCell.className = this.get_cssClass();

		$addHandlers(newCell, this._textNode$delegate);

		this._extendedCell = this._insertNodeCell(newRow);
	},

	_insertNodeCell: function (row) {
		var newCell = row.insertCell();

		switch (this.get_nodeVerticalAlign()) {
			case $HGRootNS.VerticalAlign.Top:
				newCell.style.verticalAlign = "top";
				break;
			case $HGRootNS.VerticalAlign.Bottom:
				newCell.style.verticalAlign = "bottom";
				break;
			case $HGRootNS.VerticalAlign.Middle:
				newCell.style.verticalAlign = "middle";
				break;
		}

		return newCell;
	},

	_ensureChildNodeContainer: function () {
		this.get_childNodeContainer();
	},

	get_childNodeContainer: function () {
		var container = $HGRootNS.DeluxeTreeNode.callBaseMethod(this, 'get_childNodeContainer', []);
		if (!container) {
			container = $HGDomElement.get_currentDocument().createElement("div");
			container.style.marginLeft = this._tree.get_nodeIndent();
			container.style.display = "none";
			this.set_childNodeContainer(container);
			this.get_element().appendChild(container);
		}

		return container;
	},

	get_allChildrenChecked: function () {
		var existCheckBox = false;
		var existUnchecked = false;

		var children = this.get_children();

		for (var i = 0; i < children.length; i++) {
			var node = children[i];

			if (node.get_showCheckBox()) {
				existCheckBox = true;
				existUnchecked = true;

				if (node.get_checked() == false) {
					existUnchecked = false;
					break;
				}
			}
		}

		return (existCheckBox && existUnchecked);
	},

	appendChild: function (node) {
		node._tree = this._tree;
		node.drawNode();

		this._ensureChildNodeContainer();

		$HGRootNS.DeluxeTreeNode.callBaseMethod(this, 'appendChild', [node]);

		if (this.get_children().length > 0) {
			if (this._expandImg && this._expandImg.style.visibility == "hidden")
				this._expandImg.style.visibility = "visible";
		}

		this._setExpanded(this.get_expanded());
		this.set_checked(this.get_checked());
	},

	//add by zwh 20071105
	insertChild: function (index, node) {
		node._tree = this._tree;
		node.drawNode();

		this._ensureChildNodeContainer();

		$HGRootNS.DeluxeTreeNode.callBaseMethod(this, 'insertChild', [index, node]);

		if (this.get_children().length > 0) {
			if (this._expandImg && this._expandImg.style.visibility == "hidden")
				this._expandImg.style.visibility = "visible";
		}

		this._setExpanded(this.get_expanded());
		this.set_checked(this.get_checked());
	},

	clearChildren: function () {
		this._children = [];

		var childNodeContainer = this.get_childNodeContainer();
		if (childNodeContainer)
			childNodeContainer.innerHTML = "";
	},

	_openLink: function (url, target) {
		var link = $HGRootNS.DeluxeTreeNode._linkElement;

		if (link == null) {
			link = document.createElement("a");
			link.style.display = "none";
			document.body.appendChild(link);

			$HGRootNS.DeluxeTreeNode._linkElement = link;
		}

		link.href = url;

		if (target)
			link.target = target;
		else
			link.target = "";

		link.click();
	},

	//end method

	//begin properties
	get_nodeOpenImg: function () {
		if ((this._nodeOpenImg == "" || this._nodeOpenImg == null) && this._tree)
			this._nodeOpenImg = this._tree.get_nodeOpenImg();

		return this._nodeOpenImg;
	},

	set_nodeOpenImg: function (value) {
		if (this._nodeOpenImg != value) {
			this._nodeOpenImg = value;
			this.raisePropertyChanged("nodeOpenImg");
		}
	},

	get_nodeCloseImg: function () {
		if ((this._nodeCloseImg == "" || this._nodeCloseImg == null) && this._tree)
			this._nodeCloseImg = this._tree.get_nodeCloseImg();

		return this._nodeCloseImg;
	},

	set_nodeCloseImg: function (value) {
		if (this._nodeCloseImg != value) {
			this._nodeCloseImg = value;
			this.raisePropertyChanged("nodeCloseImg");
		}
	},

	get_text: function () {
		return this._text;
	},

	set_text: function (value) {
		if (this._text != value) {
			this._text = value;
			this.raisePropertyChanged("text");
		}
	},

	get_html: function () {
		return this._html;
	},

	set_html: function (value) {
		if (this._html != value) {
			this._html = value;
			this.raisePropertyChanged("html");
		}
	},

	get_imgWidth: function () {
		return this._imgWidth;
	},

	set_imgWidth: function (value) {
		if (this._imgWidth != value) {
			this._imgWidth = value;
			this.raisePropertyChanged("imgWidth");
		}
	},

	get_imgHeight: function () {
		return this._imgHeight;
	},

	set_imgHeight: function (value) {
		if (this._imgHeight != value) {
			this._imgHeight = value;
			this.raisePropertyChanged("imgHeight");
		}
	},

	get_imgMarginLeft: function () {
		return this._imgMarginLeft;
	},

	set_imgMarginLeft: function (value) {
		if (this._imgMarginLeft != value) {
			this._imgMarginLeft = value;
			this.raisePropertyChanged("imgMarginLeft");
		}
	},

	get_imgMarginTop: function () {
		return this._imgMarginTop;
	},

	set_imgMarginTop: function (value) {
		if (this._imgMarginTop != value) {
			this._imgMarginTop = value;
			this.raisePropertyChanged("imgMarginTop");
		}
	},

	get_value: function () {
		return this._value;
	},

	set_value: function (value) {
		if (this._value != value) {
			this._value = value;
			this.raisePropertyChanged("value");
		}
	},

	get_toolTip: function () {
		return this._toolTip;
	},

	set_toolTip: function (value) {
		if (this._toolTip != value) {
			this._toolTip = value;

			if (this._textNode)
				this._textNode.title = value;

			this.raisePropertyChanged("toolTip");
		}
	},

	get_enableToolTip: function () {
		if (typeof (this._enableToolTip) == "undefined")
			this._enableToolTip = true;

		return this._enableToolTip;
	},

	set_enableToolTip: function (value) {
		if (this._enableToolTip != value) {
			this._enableToolTip = value;

			this.raisePropertyChanged("enableToolTip");
		}
	},

	get_expanded: function () {
		return this._expanded;
	},

	set_expanded: function (value) {
		if (this._expanded != value) {
			this._expanded = value;

			if (this._tree) {
				this._setExpanded(value);

				if (value)
					this._autoLoadingSubNodes();
			}

			this.raisePropertyChanged("expanded");
		}
	},

	get_selected: function () {
		return this._selected;
	},

	_set_selected: function (value) {
		this._selected = value;

		if (value) {
			if (this._tree) {
				var lastSelectedNode = this._tree.get_selectedNode();

				if (lastSelectedNode)
					lastSelectedNode.set_selected(false);

				this._tree._selectedNode = this;
			}

			if (this._textNode)
				this._textNode.className = this.get_selectedCssClass();
		}
		else {
			if (this._textNode)
				this._textNode.className = this.get_cssClass();

			if (this._tree)
				this._tree._selectedNode = null;
		}
	},

	set_selected: function (value) {
		if (this._selected != value) {
			this._set_selected(value);

			this.raisePropertyChanged("selected");
		}
	},

	get_checked: function () {
		if (this._checkbox)
			this._checked = this._checkbox.checked;

		return this._checked;
	},

	set_checked: function (value) {
		if (this._checked != value) {
			this._checked = value;

			if (this._checkbox)
				this._checkbox.checked = value;

			this.raisePropertyChanged("checked");
		}
	},

	get_showCheckBox: function () {
		return this._showCheckBox;
	},

	set_showCheckBox: function (value) {
		if (this._showCheckBox != value) {
			this._showCheckBox = value;
			this.raisePropertyChanged("showCheckBox");
		}
	},

	get_navigateUrl: function () {
		return this._navigateUrl;
	},

	set_navigateUrl: function (value) {
		if (this._navigateUrl != value) {
			this._navigateUrl = value;
			this.raisePropertyChanged("navigateUrl");
		}
	},

	get_target: function () {
		return this._target;
	},

	set_target: function (value) {
		if (this._target != value) {
			this._target = value;
			this.raisePropertyChanged("target");
		}
	},

	get_subNodesLoaded: function () {
		return this._subNodesLoaded;
	},

	set_subNodesLoaded: function (value) {
		if (this._subNodesLoaded != value) {
			this._subNodesLoaded = value;
			this.raisePropertyChanged("subNodesLoaded");
		}
	},

	get_nodeVerticalAlign: function () {
		if (!this._nodeVerticalAlign)
			this._nodeVerticalAlign = $HGRootNS.VerticalAlign.NotSet;

		return this._nodeVerticalAlign;
	},

	set_nodeVerticalAlign: function (value) {
		if (this._nodeVerticalAlign != value) {
			this._nodeVerticalAlign = value;
			this.raisePropertyChanged("nodeVerticalAlign");
		}
	},

	get_textNoWrap: function () {
		if (typeof (this._textNoWrap) == "undefined")
			this._textNoWrap = true;

		return this._textNoWrap;
	},

	set_textNoWrap: function (value) {
		if (this._textNoWrap != value) {
			this._textNoWrap = value;
			this.raisePropertyChanged("textNoWrap");
		}
	},

	get_extendedData: function () {
		return this._extendedData;
	},

	set_extendedData: function (value) {
		if (this._extendedData != value) {
			this._extendedData = value;
			this.raisePropertyChanged("extendedData");
		}
	},

	get_extendedDataKey: function () {
		return this._extendedDataKey;
	},

	set_extendedDataKey: function (value) {
		this._extendedDataKey = value;
	},

	get_cssClass: function () {
		var cssClass = this._cssClass;

		if (!cssClass || cssClass == "")
			cssClass = "ajax__tree_nodetext";

		return cssClass;
	},

	set_cssClass: function (value) {
		if (this._cssClass != value) {
			this._cssClass = value;

			if (this._textNode)
				this._textNode.className = value;

			this.raisePropertyChanged("cssClass");
		}
	},

	get_selectedCssClass: function () {
		var cssClass = this._selectedCssClass;

		if (!cssClass || cssClass == "")
			cssClass = "ajax__tree_nodetext_selected";

		return cssClass;
	},

	set_selectedCssClass: function (value) {
		if (this._selectedCssClass != value) {
			this._selectedCssClass = value;
			this.raisePropertyChanged("selectedCssClass");
		}
	},

	get_childNodesLoadingType: function () {
		return this._childNodesLoadingType;
	},

	set_childNodesLoadingType: function (value) {
		if (this._childNodesLoadingType != value) {
			this._childNodesLoadingType = value;
			this.raisePropertyChanged("childNodesLoadingType");
		}
	},

	get_lazyLoadingText: function () {
		return this._lazyLoadingText;
	},

	set_lazyLoadingText: function (value) {
		if (this._lazyLoadingText != value) {
			this._lazyLoadingText = value;
			this.raisePropertyChanged("lazyLoadingText");
		}
	},

	//add by zwh 20071107 用于插入附件控件等
	get_extendedCell: function () {
		return this._extendedCell;
	}
	//end properties
}

$HGRootNS.DeluxeTreeNode.registerClass($HGRootNSName + ".DeluxeTreeNode", $HGRootNS.TreeNode);
$HGRootNS.DeluxeTreeNode._linkElement = null;