
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	MenuItem.js
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ��ΰ	    20070430		����
// -------------------------------------------------

$HGRootNS.MenuItem = function(element) {
	$HGRootNS.MenuItem.initializeBase(this, [element]);

	this._childItems = null;
	this._deluxeMenu = null;
	this._isSeparator = false;
	this._enable = true;
	this._visible = true;
	this._imageUrl = "";
	this._text = "";
	this._toolTip = "";
	this._value = "";
	this._target = "_blank";
	this._navigateUrl = "";
	//    this._popOutImageUrl = "";
	//��̬�˵����ʾ���Ӳ˵���ͼƬ
	this._staticPopOutImageUrl = "";
	//�˵���Ŀ��ʾ���Ӳ˵���ͼƬ
	this._dynamicPopOutImageUrl = "";
	//�˵�����ߵ�ͼƬ��
	this._popOutImage = null;
	this._trItem = null;
	this._tdContent = null;
	this._imageContainer = null;
	this._trChildContainer = null;
	this._elementContainer = null;
	this._popupChildControl = null;
	this._currentActiveChildNode = null;
	this._selected = false;
	this._moveOut = false;
	this._nodeID = "";
	this._childControlsCreated = false;
	this._itemEvents =
        {
        	mouseover: Function.createDelegate(this, this._onItemMouseOver),
        	mouseout: Function.createDelegate(this, this._onItemMouseOut),
        	click: Function.createDelegate(this, this._onItemClick)
        };
	this._showEvents = Function.createDelegate(this, this._beforeShowCall);

}

$HGRootNS.MenuItem.prototype =
{
	get_childItems: function() {
		//return this._childItems;
	},

	set_childItems: function(value) {
		//this._childItems = value;
	},

	get_deluxeMenu: function() {
		return this._deluxeMenu;
	},
	set_deluxeMenu: function(value) {
		if (this._deluxeMenu != value) {
			this._deluxeMenu = value;
			this.raisePropertyChanged("deluxeMenu");
		}
	},

	get_isSeparator: function() {
		return this._isSeparator;
	},
	set_isSeparator: function(value) {
		if (this._isSeparator != value) {
			this._isSeparator = value;
			this.raisePropertyChanged('isSeparator');
		}
	},

	get_enable: function() {
		return this._enable;
	},
	set_enable: function(value) {
		if (this._enable != value) {
			this._enable = value;
			this.raisePropertyChanged('enable');
		}
	},

	get_visible: function() {
		return this._visible;
	},
	set_visible: function(value) {
		if (this._visible != value) {
			this._visible = value;
			this.raisePropertyChanged('visible');
		}
	},

	get_imageUrl: function() {
		return this._parent;
	},
	set_imageUrl: function(value) {
		if (this._imageUrl != value) {
			this._imageUrl = value;
			this.raisePropertyChanged("imageUrl");
		}
	},

	get_text: function() {
		return this._text;
	},
	set_text: function(value) {
		if (this._text != value) {
			this._text = value;
			this.raisePropertyChanged("text");
		}
	},

	get_toolTip: function() {
		return this._text;
	},
	set_toolTip: function(value) {
		if (this._toolTip != value) {
			this._toolTip = value;
			this.raisePropertyChanged("toolTip");
		}
	},

	get_value: function() {
		return this._value;
	},
	set_value: function(value) {
		if (this._value != value) {
			this._value = value;
			this.raisePropertyChanged("value");
		}
	},

	get_target: function() {
		return this._target;
	},
	set_target: function(value) {
		if (this._target != value) {
			this._target = value;
			this.raisePropertyChanged("target");
		}
	},

	get_navigateUrl: function() {
		return this._navigateUrl;
	},
	set_navigateUrl: function(value) {
		if (this._navigateUrl != value) {
			this._navigateUrl = value;
			this.raisePropertyChanged("navigateUrl");
		}
	},

	//    get_popOutImageUrl : function() { 
	//        return this._popOutImageUrl; 
	//    },    
	//    set_popOutImageUrl : function(value) { 
	//        if (this._popOutImageUrl != value) {
	//            this._popOutImageUrl = value; 
	//            this.raisePropertyChanged("popOutImageUrl");
	//        }
	//    },

	//��̬�˵���ͼƬ
	get_staticPopOutImageUrl: function() {
		return this._staticPopOutImageUrl;
	},
	set_staticPopOutImageUrl: function(value) {
		if (this._staticPopOutImageUrl != value) {
			this._staticPopOutImageUrl = value;
			this.raisePropertyChanged("staticPopOutImageUrl");
		}
	},

	get_dynamicPopOutImageUrl: function() {
		return this._dynamicPopOutImageUrl;
	},
	set_dynamicPopOutImageUrl: function(value) {
		if (this._dynamicPopOutImageUrl != value) {
			this._dynamicPopOutImageUrl = value;
			this.raisePropertyChanged("dynamicPopOutImageUrl");
		}
	},

	get_currentActiveChildNode: function() {
		return this._currentActiveChildNode;
	},
	set_currentActiveChildNode: function(value) {
		if (this._currentActiveChildNode != value) {
			this._currentActiveChildNode = value;
			this.raisePropertyChanged("currentActiveChildNode");
		}
	},

	get_separatorMode: function() {
		return this._separatorMode;
	},
	set_separatorMode: function(value) {
		if (this._separatorMode != value) {
			this._separatorMode = value;
			this.raisePropertyChanged("separatorMode");
		}
	},

	get_selected: function() {
		return this._selected;
	},
	//������ټ���ѡ��ʱ����ʽ
	set_selected: function(value) {
		if (this._selected != value) {
			this._selected = value;
			this.raisePropertyChanged("selected");
		}
		if (this._trItem) {
			this._removeItemCssClass(this._trItem.className);
			this._trItem.className = this._get_itemCssClass();
		}
	},

	get_hasDynamicChild: function() {
		return this.get_level() >= this._deluxeMenu.get_staticDisplayLevels();
	},

	get_isDynamicItem: function() {
		return this.get_level() > this._deluxeMenu.get_staticDisplayLevels();
	},

	get_DynamicRoot: function() {
		var root = this;
		while (root) {
			if (!root.get_isDynamicItem())
				return root;

			root = root.get_parent();
		}
		return null;
	},

	get_document: function() {
		if (this.get_isDynamicItem())
			return Sys.UI.DomElement.getParentWindow(this._parent.get_childNodeContainer()).document;
		else
			return document;
	},

	get_popupChildControl: function() {
		return this._popupChildControl;
	},
	/*********************************************/
	get_nodeID: function() {
		return this._nodeID;
	},
	set_nodeID: function(value) {
		if (this._nodeID != value) {
			this._nodeID = value;
			this.raisePropertyChanged("nodeID");
		}
	},

	initialize: function() {
		$HGRootNS.MenuItem.callBaseMethod(this, 'initialize');
		//��շָ�������������
		if (this._isSeparator) {
			this._text = "";
		}

		this.buildControl();

	},

	dispose: function() {
		//         for (var i = 0; i < this._children.length; i++)
		//        {
		//         if(this._children[i]._trItem != null)
		//         {
		//          $HGDomEvent.removeHandlers(this._children[i]._trItem, this._itemEvents);
		//         }
		//        }

		//$HGDomEvent.removeHandlers(this,this._itemEvents);

		var e = this.get_element();
		e.click = null;
		e.mouseover = null;
		this._deluxeMenu = null;
		this._popOutImage = null;
		this._trItem = null;
		this._tdContent = null;
		this._imageContainer = null;
		this._trChildContainer = null;
		this._popupChildControl = null;
		this._currentActiveChildNode = null;
		try {
			$HGRootNS.MenuItem.callBaseMethod(this, 'dispose');
		}
		catch (e) {
		}
	},

	//�����˵���Ŀ������
	buildControl: function() {
		var doc = $HGDomElement.get_currentDocument();
		$HGDomElement.set_currentDocument(this.get_document());
		try {
			this._buildContainer();
			this._buildControl();
		}
		finally {
			$HGDomElement.set_currentDocument(doc);
		}
	},

	_buildContainer: function() {
		var elt = this.get_element();
		//�����td�򴴽� table tbody Ȼ���ٴ����˵������������td���У�
		if (elt.tagName.toLowerCase() == "td") {
			var table = $HGDomElement.createElementFromTemplate({ nodeName: "table" }, elt);

			var tbody = $HGDomElement.createElementFromTemplate({ nodeName: "tbody" }, table);

			this._elementContainer = tbody;
		}
		else {
			this._elementContainer = elt;
		}
	},
	//����ѡ��״̬��css
	_get_itemCssClass: function() {
		return this._selected ?
                this._deluxeMenu.get_selectedItemCssClass() :
                this._deluxeMenu.get_itemCssClass();

	},

	_get_itemHoverCssClass: function() {
		return this._deluxeMenu.get_hoverItemCssClass();
	},

	_buildControl: function() {
		this._buildItem();
	},

	_get_indent: function() {
		return this.get_isDynamicItem() ? 0 : this._deluxeMenu.get_subMenuIndent() * (this.get_level() - 1);
	},

	_get_textIndent: function() {
		return this._deluxeMenu.get_textHeadWidth() +
                (this._deluxeMenu.get_isImageIndent() ? 0 : this._get_indent());
	},

	_buildTDHead: function(trContent, indent) {
		var headIndent = this._deluxeMenu.get_isImageIndent() ?
                indent : 0;
		this._buildWidthTD(trContent, headIndent);
	},

	_buildWidthTD: function(trContent, width) {
		$HGDomElement.createElementFromTemplate(
            {
            	nodeName: "TD",
            	properties:
                {
                	style: { width: width + "px" }
                }
            },
            trContent
        );
	},

	_buildImageContainer: function(trContent) {
		//����Ƿָ������ͼƬ�������ʽ
		var imageColCssClass = "";
		//       if(!this._isSeparator)
		//       {
		imageColCssClass = [this._deluxeMenu.get_imageColCssClass(), " popupMenu_Fixed_ImageCol"];
		//       }

		var td = $HGDomElement.createElementFromTemplate(
            {
            	nodeName: "TD",
            	cssClasses: imageColCssClass
            },
            trContent
        );

		return td;
	},
	/*build container for children node,first table*/
	_buildTrInTd: function(td) {
		var tbContent = $HGDomElement.createElementFromTemplate(
                {
                	nodeName: "Table",
                	properties:
                    {
                    	style:
                        {
                        	width: "100%",
                        	height: "100%"
                        },

                    	cellPadding: 0,
                    	cellSpacing: 0,
                    	border: 0
                    }

                },
                td
            );

		var tbodyContent = $HGDomElement.createElementFromTemplate(
                {
                	nodeName: "TBody"
                },
                tbContent
            );

		var trContent = $HGDomElement.createElementFromTemplate(
                {
                	nodeName: "TR"
                },
                tbodyContent
            );

		return trContent;
	},

	_buildTrContent: function(trItem) {
		if (!this.get_isDynamicItem()) {
			var tdItem = $HGDomElement.createElementFromTemplate(
                {
                	nodeName: "TD",
                	properties:
                    {
                    	colSpan: 4
                    }
                },
                trItem
            );

			var trContent = this._buildTrInTd(tdItem);
		}
		else
			var trContent = trItem;

		return trContent;
	},

	//�����˵���Ľṹ
	_buildItem: function() {
		var separator = {};
		var trCss = "";
		trEvent = null;
		if (this._isSeparator) {
			//����Ǻ���ĵ�һ����û�зָ���
			if (this._deluxeMenu.get_Orientation() == $HGRootNS.MenuOrientation.Horizontal && this.get_level() == 1) {
				separator = { display: "none" };
			}
			//		else
			//		{
			//			//��ʼ�ָ���Ϊ����ʾ,�����̬�������1������ָ���
			//			if(this._deluxeMenu._staticDisplayLevels>1)
			//			{
			//				separator = {height: "1px"};
			//			}
			//			else
			//			{
			separator = { height: "1px"}//,display: "none"};
			//			}
			//		}
		}
		else {
			//�������������
			if (this._visible) {
				//����ǽ�����
				if (!this._enable) {
					separator = { color: "#C8C8C8" };
					trCss = "popupMenu_Default_Item_Enable";
				}
				else {
					trCss = this._get_itemCssClass();
					trEvent = this._itemEvents;
				}
			}
			else {
				separator = { display: "none" };
			}
		}

		var trItem = $HGDomElement.createElementFromTemplate(
		{
			nodeName: "TR",
			properties:
                {
                	title: this._toolTip
                    , style: separator   //�ı�˵���Ĵ�С

                },
			events: trEvent,
			cssClasses: [trCss]
		},
            this._elementContainer    //tbody
        );

		this._trItem = trItem;

		var trContent = this._buildTrContent(trItem);

		this._buildTDHead(trContent, this._get_indent());

		var imageContainer = this._buildImageContainer(trContent);
		//imageContainer.style.height = "100%";
		this._imageContainer = imageContainer;
		//����Ƿָ��߲�������ͼƬ
		if (!this._isSeparator) {
			var imageUrl = this._imageUrl || this._deluxeMenu.get_imageUrl();
			if (imageUrl)
				var image = $HGDomElement.createElementFromTemplate(
                {
                	nodeName: "img",
                	properties:
                    {
                    	src: imageUrl
                    },
                	cssClasses: [this._deluxeMenu.get_imageCssClass()]
                },
                imageContainer
            );
		}

		this._buildWidthTD(trContent, this._get_textIndent());
		//�ı����ұ�ͼƬ֮�µķָ���
		var tdCss = "";
		if (!this._isSeparator) {
			tdCss = this._get_itemCssClass();
		}
		else {
			tdCss = "popupMenu_Default_Separator";
		}

		//�˵��ı���Ԫ��
		var tdContent = $HGDomElement.createElementFromTemplate(
            {
            	nodeName: "TD",
            	properties:
                {
                	style: { whiteSpace: "nowrap" }
                },
            	cssClasses: [tdCss]
            },
            trContent
        );
		this._tdContent = tdContent;
		//�˵�text
		var cursor;
		if (this._enable) {
			cursor = "hand";
		}
		else {
			cursor = "default";
		}
		var spanText = $HGDomElement.createElementFromTemplate(
            {
            	nodeName: "span",
            	properties:
                {
                	innerText: this._text,
                	style: { cursor: cursor }
                }
            },
            tdContent
        );
		//�˵����ұߵı�ʶͼ��
		var tdPopupImage = $HGDomElement.createElementFromTemplate(
            {
            	nodeName: "TD",
            	properties:
                {
                	style: { width: "16px" },
                	align: "right"
                },
            	cssClasses: [tdCss, "imgright"]
            },
            trContent
        );
		if (!this._isSeparator) {
			//���ȼ���
			var outimg;
			if (this.get_level() <= this._deluxeMenu.get_staticDisplayLevels()) {
				outimg = this._staticPopOutImageUrl || this._deluxeMenu.get_staticPopOutImageUrl();
			}
			else {
				outimg = this._dynamicPopOutImageUrl || this._deluxeMenu.get_dynamicPopOutImageUrl();
			}
			if (outimg == "" || outimg == null) {
				outimg = this._deluxeMenu.get_defaultPopOutImageUrl();
			}

			this._popOutImage = $HGDomElement.createElementFromTemplate(
            {
            	nodeName: "img",
            	properties:
                {
                	src: outimg
                },
            	visible: false
            },
            tdPopupImage
        );
			//�ٴ����ڵ��ͼƬ
			if (this.get_hasChildNodes())
				Sys.UI.DomElement.setVisible(this._popOutImage, true);

		}

	},

	_removeChildContainer: function() {
		if (this._trChildContainer) {
			this.get_element().removeChild(this._trChildContainer);
			this._trChildContainer = null;
		}

		if (this._popupChildControl)
			this._popupChildControl = null;
	},

	_ensureBuildChildContainer: function() {
		if (this.get_hasDynamicChild()) {
			// if (!this._popupChildControl)
			// {
			var doc = $HGDomElement.get_currentDocument();
			$HGDomElement.set_currentDocument(this.get_document());
			this._buildDynamicChildNodeContainer();
			$HGDomElement.set_currentDocument(doc);
			//}
		}
		else {
			if (!this._trChildContainer)
				this._buildStaticChildNodeContainer();
		}
	},

	_buildStaticChildNodeContainer: function() {
		var trContent = $HGDomElement.createElementFromTemplate(
            {
            	nodeName: "TR"
            },
            this.get_element()
        );
		this._trChildContainer = trContent;

		var tdContent = $HGDomElement.createElementFromTemplate(
            {
            	nodeName: "TD",
            	properties:
                {
                	colSpan: 4
                }
            },
            trContent
        );
		/*children node table*/

		var childNodeContainer = $HGDomElement.createElementFromTemplate(
            {
            	nodeName: "Table",
            	properties:
                {
                	width: "100%",
                	cellPadding: 0,
                	cellSpacing: 0,
                	border: 0
                }

            },
            tdContent
        );

		this._childNodeContainer = childNodeContainer;
	},

	//������̬�ڵ�
	_buildDynamicChildNodeContainer: function() {

		//�Ƿָ��߲�����ͼƬ,�ж��Ƿ���ͼƬ
		if (this._popOutImage != null) {
			Sys.UI.DomElement.setVisible(this._popOutImage, true);
		}
		//�õ�ͬһ���ڵ�
		var parentPopupControl = this.get_parent().get_popupChildControl ? this.get_parent().get_popupChildControl() : null;
		var position;
		if (this._deluxeMenu.get_Orientation() == $HGRootNS.MenuOrientation.Horizontal && this.get_level() == 1)   //�?
		{
			position = $HGRootNS.PositioningMode.BottomLeft;
		}
		else {
			position = $HGRootNS.PositioningMode.RightTop;
		}

		if (!this._popupChildControl) {
			var popupControl = $create($HGRootNS.PopupControl,
                { parent: parentPopupControl, positionElement: this._trItem, positioningMode: position }
                , { "beforeShow": this._showEvents }, null, null);

			this._popupChildControl = popupControl;
			this._initDynamicChildNodeContainer();
		}
	},

	_initDynamicChildNodeContainer: function() {
		var popBodyCssClass = this._deluxeMenu.get_popBodyCssClass();
		/*children node table*/
		if (!this._childNodeContainer) {
			var childNodeContainer = $HGDomElement.createElementFromTemplate(
                {
                	nodeName: "Table",
                	properties:
                    {
                    	cellPadding: 0,
                    	cellSpacing: 0,
                    	border: 0
                    },
                	cssClasses: [popBodyCssClass, "popupMenu_Border"]
                },
                this._popupChildControl.get_popupBody(), null, this._popupChildControl.get_popupDocument()
            );

			this._childNodeContainer = childNodeContainer;
		}
	},

	createChildElement: function() {
		//����popup
		this._ensureBuildChildContainer();
		var doc = Sys.UI.DomElement.getParentWindow(this.get_childNodeContainer()).document;
		var tbody = doc.createElement("tbody");
		return tbody;
	},

	showPopup: function() {
		if (this._popupChildControl) {
			//            this._ensureBuildChildContainer();
			//            //����popup�Ͻڵ������
			//            this.buildChildControl();
			if (this._deluxeMenu._HasControlSeparator)
				this._loadFirstLevel();
			this._popupChildControl.show();

		}

	},

	_loadFirstLevel: function() {
		//�����ʾ�������
		var visibleItem = new Array();

		for (var i = 0; i < this._children.length; i++) {
			var hasVisibleGrandchild = 0;

			//�����������ʾ����
			if (this._children[i]._visible) {
				//����Ƿָ��߱�־Ϊtrue
				Array.add(visibleItem, new Array(i, this._children[i]._isSeparator));
			}

			//��Ҫ��ʾ��ÿ���ڵ���ӽڵ��Ƿ�Ϊ���أ�����������ָʾ��һ����ͼƬ
			if (this._children[i].get_hasChildNodes()) {
				for (var grandchild = 0; grandchild < this._children[i]._children.length; grandchild++) {
					if (this._children[i]._children[grandchild]._visible && !this._children[i]._children[grandchild]._isSeparator) {
						hasVisibleGrandchild++;
					}
				}
			}
			if (hasVisibleGrandchild == 0) {
				Sys.UI.DomElement.setVisible(this._children[i]._popOutImage, false);
			}
		} //for end
		if (visibleItem.length > 0) {
			//ɸѡ����,��һ���Ƿ��Ƿָ���
			while (visibleItem.length > 0 && visibleItem[0][1]) {
				Array.removeAt(visibleItem, 0);
			}
			//ɾ�����һ���Ƿָ��ߵ�
			while (visibleItem.length > 0 && visibleItem[visibleItem.length - 1][1]) {
				Array.removeAt(visibleItem, visibleItem.length - 1);
			}
			var finallyItem = new Array();
			//������Ҫ��ʾ�����������
			for (var remainder = 0; remainder < visibleItem.length; remainder++) {
				if (visibleItem[remainder][1]) {
					//��һ������Ƿָ��߾�����
					if (remainder < visibleItem.length - 1 && visibleItem[remainder + 1][1])
						continue;
				}
				Array.add(finallyItem, visibleItem[remainder]);
			}
			//���÷ָ��ߺͿ�����ĺ���λ��
			if (finallyItem.length > 0) {
				for (var i = 0; i < this._children.length; i++) {
					if (this._children[i]._isSeparator && !this._searchIsVieibleSeparator(finallyItem, i))
					{ this._children[i]._trItem.style.display = "none"; }
				}
			}
			else	//�����Ҫ��ʾ��ÿ���ڵ㶼Ϊ����
			{
				//ȫ������
				for (var i = 0; i < this._children.length; i++) {
					this._children[i]._trItem.style.display = "none";
				}
				Sys.UI.DomElement.removeCssClass(this._popupChildControl.get_popupBody().childNodes[0], "popupMenu_Default popupMenu_Border");
			}
		}
		else	//�����Ҫ��ʾ��ÿ���ڵ㶼Ϊ����
		{
			Sys.UI.DomElement.removeCssClass(this._popupChildControl.get_popupBody().childNodes[0], "popupMenu_Default popupMenu_Border");
		}
	},

	_searchIsVieibleSeparator: function(visibleArr, popupIndex) {
		var visible = false;
		for (var i = 0; i < visibleArr.length; i++) {
			if (visibleArr[i][0] == popupIndex) {
				visible = true;
				break;
			}
		}
		return visible;
	},

	_beforeShowCall: function(sender, e) {
		if (e.width < this._deluxeMenu.get_ItemFontWidth()) {
			e.width = this._deluxeMenu.get_ItemFontWidth();
			this.get_childNodeContainer().style.width = this._deluxeMenu.get_ItemFontWidth();
		}
	},

	hidePopup: function() {
		if (this._popupChildControl) {
			this._popupChildControl.hide();
		}
	},

	_addItemCssClass: function(className) {
		Sys.UI.DomElement.addCssClass(this._imageContainer, className);
		Sys.UI.DomElement.addCssClass(this._trItem, className);
		Sys.UI.DomElement.addCssClass(this._tdContent, className);
	},

	_removeItemCssClass: function(className) {
		Sys.UI.DomElement.removeCssClass(this._imageContainer, className);
		Sys.UI.DomElement.removeCssClass(this._trItem, className);
		Sys.UI.DomElement.removeCssClass(this._tdContent, className);
	},

	setActive: function() {
		var cn = this._deluxeMenu.get_currentActiveChildNode();
		if (cn !== this)
			this._deluxeMenu.set_currentActiveChildNode(this);

		if (this != cn) {
			if (!this.get_isDynamicItem()) {
				if (cn) {
					var dRoot = cn.get_DynamicRoot();
					dRoot.setInActive();
					dRoot.hidePopup();
				}
			}
			else {
				this.get_parent().enumerateChildren(this.setInActive);
			}

			this.showPopup();
			//�������ƹ��˵������ʽ
			var itemHoverCssClass = this._get_itemHoverCssClass();
			this._addItemCssClass(itemHoverCssClass);
		}
	},

	setInActive: function() {
		this.hidePopup();
		var itemHoverCssClass = this._get_itemHoverCssClass();
		this._removeItemCssClass(itemHoverCssClass);
	},
	//����ƶ��¼�
	_onItemMouseOver: function(e) {
		if (this.raiseMover(this, e)) {
			this.setActive();

			if (this.get_hasDynamicChild())
				this.enumerateChildren(this.setInActive);

			e.stopPropagation();
			e.preventDefault();
		}
	},

	_onItemMouseOut: function(e) {
		//		Sys.UI.DomElement.addCssClass(this._imageContainer, "popupMenu_Fixed_ImageCol");
		//        this.setInActive();
		//        e.stopPropagation();
		//        e.preventDefault();
	},

	//����¼�
	_onItemClick: function(e) {
		if (this.raiseMclick(this, e)) {
			if (this._deluxeMenu.get_MultiSelect()) {
				//�ı�ѡ��״̬
				this._setNodeSelected(this._deluxeMenu._items, this.nodeID);
			}
			//������ʽ
			this._deluxeMenu.set_selectedItem(this);

			if (this._navigateUrl) {
				//				this._deluxeMenu._link.href = this._navigateUrl;
				//				this._deluxeMenu._link.target = (this._target || this._deluxeMenu.get_target() || "_blank");
				//				this._deluxeMenu._link.click();
				if (this._navigateUrl.startsWith("javascript:"))
					eval(this._navigateUrl);
				else
					window.open(this._navigateUrl, this._target, "");
			}

			e.stopPropagation();
			e.preventDefault();

			var parent = this.get_parent();
			while (parent && parent.get_isDynamicItem && parent.get_isDynamicItem())
				parent = parent.get_parent();

			if (parent && parent.hidePopup) parent.hidePopup();

		}
	}
	//�ı�ѡ�к�item������
    , _setNodeSelected: function(menuItems, nodeID) {
    	if (menuItems) {
    		for (var i = 0; i < menuItems.length; i++) {
    			if (nodeID == menuItems[i].nodeID) {
    				//���ò˵����ݽṹ��selected״̬,viewstate������
    				if (this._selected) {
    					menuItems[i].selected = false;
    				}
    				else {
    					menuItems[i].selected = true;
    				}
    				break;
    			}

    			this._setNodeSelected(menuItems[i].childItems, nodeID);
    		}

    	}
    }
	/*************************************attach event*******************************************/

    , raiseMclick: function(item, eventElement) {
    	var handlers = this._deluxeMenu.get_events().getHandler("mclick");
    	var isContinue = true;
    	if (handlers) {
    		var e = new Sys.EventArgs;

    		e.item = item;
    		e.cancel = false;
    		e.eventElement = eventElement;

    		handlers(this, e);

    		if (e.cancel) {
    			isContinue = false;
    		}
    	}

    	return isContinue;
    }

    , raiseMover: function(item, eventElement) {
    	var handlers = this._deluxeMenu.get_events().getHandler("mover");
    	var isContinue = true;
    	if (handlers) {
    		var e = new Sys.EventArgs;

    		e.item = item;
    		e.cancel = false;
    		e.eventElement = eventElement;

    		handlers(this, e);

    		if (e.cancel) {
    			isContinue = false;
    		}
    	}

    	return isContinue;
    }

}

$HGRootNS.MenuItem.registerClass($HGRootNSName + '.MenuItem', $HGRootNS.TreeNode);
