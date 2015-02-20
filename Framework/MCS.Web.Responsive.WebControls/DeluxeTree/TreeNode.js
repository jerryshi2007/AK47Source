$HGRootNS.ChildNodesLoadingTypeDefine = function () {
    throw Error.invalidOperation();
}

$HGRootNS.ChildNodesLoadingTypeDefine.prototype =
{
    Normal: 0,
    LazyLoading: 1
}

$HGRootNS.ChildNodesLoadingTypeDefine.registerEnum($HGRootNSName + '.ChildNodesLoadingTypeDefine');

$HGRootNS.VerticalAlign = function () {
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

$HGRootNS.DeluxeTreeNode = function (element) {
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
    this._childNodesLoadingType = $HGRootNS.ChildNodesLoadingTypeDefine.Normal;
    this._lazyLoadingText = "正在加载...";
    this._extendedDataKey = "";

    this._tree = null;

    this._expandImg = null;
    this._expandImg$delegate = {
        click: Function.createDelegate(this, this._expandImg_onclick)
        //		,
        //		contextmenu: Function.createDelegate(this, this._node_onContextMenu)
    };

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
    };

    this._extendedCell = null;
}

$HGRootNS.DeluxeTreeNode.InLoadingMode = false;

$HGRootNS.DeluxeTreeNode.createNode = function (properties) {
    var elt = $HGDomElement.get_currentDocument().createElement("div");

    var newNode = $create($HGRootNS.DeluxeTreeNode,
						properties,
						null,
						null,
						elt);

    return newNode;
}

$HGRootNS.DeluxeTreeNode.createLoadingNode = function (parentNode) {
    var subNode = $HGRootNS.DeluxeTreeNode.createNode(
					{
					    //						nodeOpenImg: parentNode._tree.get_defaultWaitingForImage(),
					    //						nodeCloseImg: parentNode._tree.get_defaultWaitingForImage(),
					    isLoadingNode: true,
					    text: parentNode.get_lazyLoadingText()
					}
				);
    parentNode.appendChild(subNode);
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
            if (this._tree.raiseBeforeAllDataBind()) {
                if (cb) {
                    this._bindDataSeperatly(nodesData, cb, batchSize, true);
                }
                else {
                    this._bindAllData(nodesData);
                    this._tree.raiseAfterAllDataBind();
                }
            }
        }
    },

    _innerDataBind: function (nodesData, cb, batchSize) {
        this.clear();

        if (nodesData) {
            if (cb) {
                this._bindDataSeperatly(nodesData, cb, batchSize, false);
            }
            else {
                this._bindAllData(nodesData);
            }
        }
    },

    _bindDataSeperatly: function (nodesData, cb, batchSize, raiseAfterEvent) {
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
            else {
                if (raiseAfterEvent)
                    this._tree.raiseAfterAllDataBind();
            }
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

                $HGRootNS.DeluxeTreeNode.createLoadingNode(newNode);
            }
            else
                newNode._innerDataBind(srcNode.nodes, cb, batchSize);
        }
    },

    _createPropertiesFromSrcNode: function (srcNode) {
        return {
            isLoadingNode: srcNode.isLoadingNode,
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
            childNodesLoadingType: srcNode.childNodesLoadingType,
            lazyLoadingText: srcNode.lazyLoadingText,
            extendedDataKey: srcNode.extendedDataKey
        };
    },

    _initialize: function () {
    },

    _toCanSerializedObject: function (recursively) {
        var result = {
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
            childNodesLoadingType: this.get_childNodesLoadingType(),
            lazyLoadingText: this.get_lazyLoadingText()
        };

        if (recursively) {
            var children = this.get_children();
            var childrenResult = new Array(children.length);

            for (var i = 0; i < children.length; i++) {
                childrenResult[i] = children[i]._toCanSerializedObject(recursively);
            }

            result["nodes"] = childrenResult;
        }

        return result;
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
            this._expandIcon.style.display = "none";
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
        eventElement.checked = !this.get_checked();
        if (continueExec) {
            this.set_checked(eventElement.checked);
            this._tree.raiseNodeCheckBoxAfterClick(this, eventElement);
        }

        return continueExec;
    },

    //林彬添加的，很诡异。沈峥注释掉
    //	_checkbox_onclick_copy: function (element) {
    //		var continueExec = this._tree.raiseNodeCheckBoxBeforeClick(this, element);
    //		if (continueExec) {
    //			this._tree.raiseNodeCheckBoxAfterClick(this, element);
    //		}
    //	},
    //end delegates

    //begin method
    drawNode: function () {
        var elt = this.get_element();

        elt.innerHTML = "";
        this._buildChildElements(elt);
    },

    _createExpandImg: function () {
        var doc = $HGDomElement.get_currentDocument();

        var icon = doc.createElement("i");
        icon.className = "icon-plus";

        $addHandlers(icon, this._expandImg$delegate);

        this._expandIcon = icon;

        return icon;
    },

    _createNodeImg: function () {
        var doc = $HGDomElement.get_currentDocument();

        var img = doc.createElement("img");
        var ncg = this.get_nodeCloseImg();

        if (ncg == "")
            img.style.display = "none";
        else
            img.src = ncg;

        img.style.cursor = "pointer";

        $addHandlers(img, this._nodeImg$delegate)

        return img;
    },

    _createCheckBox: function () {
        var doc = $HGDomElement.get_currentDocument();
        var icon = doc.createElement("i");
        icon.className = "icon-ok";

        if (this.get_checked()) {
            if (this.get_element().className.indexOf("tree-checked") < 0)
                this.get_element().className += " tree-checked";
        }

        $addHandlers(icon, this._checkbox$delegate);

        this._checkbox = icon;

        //		elt.appendChild(icon);
        //		
        //		var doc = $HGDomElement.get_currentDocument();

        //		var chk = doc.createElement("input");
        //		chk.type = "checkbox";

        //		$addHandlers(chk, this._checkbox$delegate);

        //		chk.checked = this.get_checked();

        //		//linbin 添加2010.8.1。沈峥注释掉。编程构造的checkBox不应该触发事件
        //		//this._checkbox_onclick_copy(chk);

        return icon;
    },
    _setExpanded: function (value) {
        var nodeImgSrc = "";

        if (value) {
            nodeImgSrc = this.get_nodeOpenImg();

            if (this._expandIcon) {
                this._expandIcon.className = "icon-minus";
            }

            this.get_childNodeContainer().style.display = "block";
        }
        else {
            nodeImgSrc = this.get_nodeCloseImg();

            if (this._expandIcon) {
                this._expandIcon.className = "icon-plus";
            }

            this.get_childNodeContainer().style.display = "none";
        }

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

        if (this.get_isLoadingNode()) {
            elt.className = "tree-loader";
            var divLoading = doc.createElement("div");
            divLoading.className = "tree-loading";
            elt.appendChild(divLoading);

            var i = doc.createElement("i");
            i.className = "icon-refresh icon-spin blue";
            divLoading.appendChild(i);

            var span = doc.createElement("span");
            $(span).text(this._text);
            divLoading.appendChild(span);

        } else {
            elt.className = "tree-item";
            this._createExpandImg();

            if (this.get_showCheckBox()) {
                var chk = this._createCheckBox();
                elt.appendChild(chk);
            }

            if (this.get_nodeCloseImg()) {
                var nodeImgContainer = doc.createElement("span");
                nodeImgContainer.className = "tree-img-container";
                nodeImgContainer.style.overflow = "hidden";
                nodeImgContainer.style.display = "inline-block";
                nodeImgContainer.style.cursor = "pointer";

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

                elt.appendChild(nodeImgContainer);
            }


            this._textNode = doc.createElement("div");
            this._textNode.className = "tree-item-name";

            if (this.get_html())
                this._textNode.innerHTML = this.get_html();
            else
                $(this._textNode).text(this._text);

            elt.appendChild(this._textNode);

            if (this.get_selected())
                this._set_selected(true);

            $addHandlers(this._textNode, this._textNode$delegate);

            //this._extendedCell = this._textNode;
        }
    },

    _insertNodeCell: function (row) {
        var newCell = row.insertCell(-1);

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
        this._childNodeContainer = this.get_childNodeContainer();
    },

    get_childNodeContainer: function () {
        var container = $HGRootNS.DeluxeTreeNode.callBaseMethod(this, 'get_childNodeContainer', []);
        if (!container) {
            var element = this.get_element();
            element.innerHTML = "";
            element.className = "tree-folder";
            if (this.get_checked()) {
                if (element.className.indexOf("tree-checked") < 0)
                    element.className += " tree-checked";
            }

            var doc = $HGDomElement.get_currentDocument();
            var header = doc.createElement("div");
            header.className = "tree-folder-header";
            element.appendChild(header);

            var iconExpand = this._createExpandImg();
            header.appendChild(iconExpand);

            if (this.get_showCheckBox()) {
                var iconChk = this._createCheckBox();
                header.appendChild(iconChk);
            }

            if (this.get_nodeCloseImg()) {
                var nodeImgContainer = doc.createElement("span");
                nodeImgContainer.className = "tree-img-container";
                nodeImgContainer.style.overflow = "hidden";
                nodeImgContainer.style.display = "inline-block";
                nodeImgContainer.style.cursor = "pointer";

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

                header.appendChild(nodeImgContainer);
            }

            this._textNode = doc.createElement("div");
            this._textNode.className = "tree-folder-name";

            if (this.get_html())
                this._textNode.innerHTML = this.get_html();
            else
                $(this._textNode).text(this._text);

            $addHandlers(this._textNode, this._textNode$delegate);

            header.appendChild(this._textNode);

            if (this.get_selected()) {
                this._set_selected(true);
            }

            container = doc.createElement("div");
            container.className = "tree-folder-content";

            element.appendChild(container);
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
            if (this._expandIcon && this._expandIcon.style.display == "none")
                this._expandIcon.style.display = "";
        } else {

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
            if (this._expandIcon && this._expandIcon.style.display == "none")
                this._expandIcon.style.display = "";
        }

        this._setExpanded(this.get_expanded());
        this.set_checked(this.get_checked());
    },

    clearChildren: function (recreateLoadingNode, autoCollapse) {
        this._children = [];

        var childNodeContainer = this.get_childNodeContainer();
        if (childNodeContainer) {
            childNodeContainer.innerHTML = "";
            childNodeContainer.style.display = "none";
        }

        if (recreateLoadingNode) {
            if (this.get_childNodesLoadingType() == $HGRootNS.ChildNodesLoadingTypeDefine.LazyLoading) {
                this._subNodesLoaded = false;
                $HGRootNS.DeluxeTreeNode.createLoadingNode(this);
            }
        }

        if (this._expandIcon) {
            if (this.get_children().length > 0)
                this._expandIcon.style.display = "";
            else
                this._expandIcon.style.display = "none";
        }

        if (autoCollapse)
            this._setExpanded(false);
    },

    reloadChildren: function () {
        if ($HGRootNS.DeluxeTreeNode.InLoadingMode == false) {
            if (this.get_childNodesLoadingType() == $HGRootNS.ChildNodesLoadingTypeDefine.LazyLoading) {
                this.clearChildren(true);
                this._autoLoadingSubNodes();
            }
        }
    },

    _openLink: function (url, target) {
        if (url.toLowerCase().indexOf("javascript:") == 0) {
            eval(url.substring(11));
        }
        else
            window.open(url, target);
        //		
        //		var link = $HGRootNS.DeluxeTreeNode._linkElement;

        //		if (link == null) {
        //			link = document.createElement("a");
        //			link.style.display = "none";

        //			document.body.appendChild(link);

        //			$HGRootNS.DeluxeTreeNode._linkElement = link;
        //		}

        //		link.href = url;

        //		if (target)
        //			link.target = target;
        //		else
        //			link.target = "";

        //		window.open(url, target);
    },

    //end method

    //begin properties
    get_isLoadingNode: function () {
        return this._isLoadingNode;
    },

    set_isLoadingNode: function (value) {
        this._isLoadingNode = value;
    },

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

            if (this._textNode)
                this._textNode.innerText = value;

            this.raisePropertyChanged("text");
        }
    },

    get_html: function () {
        return this._html;
    },

    set_html: function (value) {
        if (this._html != value) {
            this._html = value;

            if (this._textNode)
                this._textNode.innerHTML = value;

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
                this._textNode.className += " selected";
        }
        else {
            if (this._textNode)
                this._textNode.className = this._textNode.className.replace(" selected", "");

            if (this._tree)
                this._tree._selectedNode = null;
        }
    },

    set_selected: function (value) {
        this._set_selected(value);

        if (this._selected != value) {
            this.raisePropertyChanged("selected");
        }
    },

    get_checked: function () {
        //		if (this._checkbox)
        //			this._checked = this._checkbox.checked;

        return this._checked;
    },

    set_checked: function (value) {
        if (this._checked != value) {
            this._checked = value;
            //			if (this._checkbox) {
            //				this._checkbox.checked = value;
            //			}

            if (value) {
                var elt = this.get_element();
                if (elt.className.indexOf("tree-checked") < 0)
                    elt.className += " tree-checked";
            }
            else {
                var elt = this.get_element();
                elt.className = elt.className.replace(" tree-checked", "");
            }

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