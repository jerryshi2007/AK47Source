
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	DeluxeMenu.js
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ��ΰ	    20070430		����
// -------------------------------------------------

/*enum*/
$HGRootNS.MenuOrientation = function () {
	throw Error.invalidOperation();
}
$HGRootNS.MenuOrientation.prototype =
{
	Vertical: 0,
	Horizontal: 1
}
$HGRootNS.MenuOrientation.registerEnum($HGRootNSName + '.MenuOrientation');

$HGRootNS.DeluxeMenu = function (element) {
	$HGRootNS.DeluxeMenu.initializeBase(this, [element]);
	//���ص�link
	this._link = null;
	this._items = null;
	this._imageUrl = "";
	this._subMenuIndent = 10;
	this._staticDisplayLevels = 1;
	this._defaultPopOutImageUrl = null;

	this._staticPopOutImageUrl = null;
	this._dynamicPopOutImageUrl = null;
	this._target = "_blank";
	this._currentActiveChildNode = null;
	this._popBodyCssClass = "";
	this._cssClass = "";
	this._itemCssClass = "";
	this._hoverItemCssClass = "";
	this._selectedItemCssClass = "";
	this._separatorCssClass = "";
	this._textHeadWidth = 0;
	this._imageCssClass = "";
	this._imageColCssClass = "";
	this._isImageIndent = false;
	this._selectedItem = null;
	//
	//this._nodeID = "";
	//�Ƿ���ָ���
	this._HasControlSeparator = false;
	//�Ƿ��ѡ
	this._MultiSelect = false;
	this._ItemFontWidth = 150;
	this.counter = 0;
	//��¼��һ�����ֵĵ�ѡ��
	this._singleSelectedCount = 0;
	this._selectedArray = new Array();
	//��һ�������з���Ĭ��Ϊ��ֱ
	this._Orientation = $HGRootNS.MenuOrientation.Vertical;
	this._popup = null;
	this._childNodesCreated = false;

	this._showEvents = Function.createDelegate(this, this._beforeShowCall);
}

$HGRootNS.DeluxeMenu.prototype =
{
	get_HasControlSeparator: function () {
		return this._HasControlSeparator;
	},
	set_HasControlSeparator: function (value) {
		if (this._HasControlSeparator != value) {
			this._HasControlSeparator = value;
			this.raisePropertyChanged('HasControlSeparator');
		}
	},

	get_Orientation: function () {
		return this._Orientation;
	},
	set_Orientation: function (value) {
		if (this._Orientation != value) {
			this._Orientation = value;
			this.raisePropertyChanged('Orientation');
		}
	},

	get_MultiSelect: function () {
		return this._MultiSelect;
	},
	set_MultiSelect: function (value) {
		if (this._MultiSelect != value) {
			this._MultiSelect = value;
			this.raisePropertyChanged('MultiSelect');
		}
	},

	get_ItemFontWidth: function () {
		return this._ItemFontWidth;
	},
	set_ItemFontWidth: function (value) {
		if (this._ItemFontWidth != value) {
			this._ItemFontWidth = value;
			this.raisePropertyChanged('ItemFontWidth');
		}
	},

	get_items: function () {
		return this._items;
	},
	set_items: function (value) {
		//        this._items = $HGCommon.convertToCamelObject(value);
		this._items = value;
		this.raisePropertyChanged("items");
	},

	get_imageUrl: function () {
		return this._imageUrl;
	},
	set_imageUrl: function (value) {
		if (this._imageUrl != value) {
			this._imageUrl = value;
			this.raisePropertyChanged("imageUrl");
		}
	},

	get_subMenuIndent: function () {
		return this._subMenuIndent;
	},
	set_subMenuIndent: function (value) {
		if (this._subMenuIndent != value) {
			this._subMenuIndent = value;
			this.raisePropertyChanged("subMenuIndent");
		}
	},

	get_staticDisplayLevels: function () {
		return this._staticDisplayLevels;
	},
	set_staticDisplayLevels: function (value) {
		if (this._staticDisplayLevels != value) {
			this._staticDisplayLevels = value;
			this.raisePropertyChanged("staticDisplayLevels");
		}
	},

	get_defaultPopOutImageUrl: function () {
		return this._defaultPopOutImageUrl;
	},
	set_defaultPopOutImageUrl: function (value) {
		if (this._defaultPopOutImageUrl != value) {
			this._defaultPopOutImageUrl = value;
			this.raisePropertyChanged("defaultPopOutImageUrl");
		}
	},

	//��̬�˵���ͼƬ
	get_staticPopOutImageUrl: function () {
		return this._staticPopOutImageUrl;
	},
	set_staticPopOutImageUrl: function (value) {
		if (this._staticPopOutImageUrl != value) {
			this._staticPopOutImageUrl = value;
			this.raisePropertyChanged("staticPopOutImageUrl");
		}
	},

	get_dynamicPopOutImageUrl: function () {
		return this._dynamicPopOutImageUrl;
	},
	set_dynamicPopOutImageUrl: function (value) {
		if (this._dynamicPopOutImageUrl != value) {
			this._dynamicPopOutImageUrl = value;
			this.raisePropertyChanged("dynamicPopOutImageUrl");
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

	get_currentActiveChildNode: function () {
		return this._currentActiveChildNode;
	},
	set_currentActiveChildNode: function (value) {
		if (this._currentActiveChildNode != value) {
			this._currentActiveChildNode = value;
			this.raisePropertyChanged("currentActiveChildNode");
		}
	},

	get_selectedItem: function () {
		return this._selectedItem;
	},
	set_selectedItem: function (value) {
		//��ѡ��¼֮ǰѡ��Ľڵ�
		if (!this._MultiSelect && this._selectedItem != value) {
			if (this._selectedItem) {
				this._selectedItem.set_selected(false);
			}

			this._selectedItem = value;
		}
		//��ѡ����¼
		//��������״̬����ʽ���͵�ǰ���෴��
		if (!value._selected) {
			//this._selectedItem.set_selected(false);
			//��ǰnode��selected
			//                value._selected = false;
			value.set_selected(true);
		}
		else {
			//                value._selected = true;
			value.set_selected(false);

		}

		this.raisePropertyChanged("selectedItem");

	},

	get_cssClass: function () {
		return this._cssClass || " popupMenu_Default";
	},
	set_cssClass: function (value) {
		if (this._cssClass != value) {
			this._cssClass = value;
			this.raisePropertyChanged("cssClass");
		}
	},

	get_popBodyCssClass: function () {
		return this._popBodyCssClass || this.get_cssClass();
	},
	set_popBodyCssClass: function (value) {
		if (this._popBodyCssClass != value) {
			this._popBodyCssClass = value;
			this.raisePropertyChanged("popBodyCssClass");
		}
	},

	get_itemCssClass: function () {
		return this._itemCssClass || " popupMenu_Default_Item";
	},
	set_itemCssClass: function (value) {
		if (this._itemCssClass != value) {
			this._itemCssClass = value;
			this.raisePropertyChanged("itemCssClass");
		}
	},

	get_hoverItemCssClass: function () {
		return this._hoverItemCssClass || " popupMenu_Default_ItemHover";
	},
	set_hoverItemCssClass: function (value) {
		if (this._hoverItemCssClass != value) {
			this._hoverItemCssClass = value;
			this.raisePropertyChanged("hoverItemCssClass");
		}
	},

	get_selectedItemCssClass: function () {
		return this._selectedItemCssClass || " popupMenu_Default_ItemSelected";
	},
	set_selectedItemCssClass: function (value) {
		if (this._selectedItemCssClass != value) {
			this._selectedItemCssClass = value;
			this.raisePropertyChanged("selectedItemCssClass");
		}
	},

	get_imageCssClass: function () {
		return this._imageCssClass || " popupMenu_Default_Image";
	},
	set_imageCssClass: function (value) {
		if (this._imageCssClass != value) {
			this._imageCssClass = value;
			this.raisePropertyChanged("imageCssClass");
		}
	},

	get_imageColCssClass: function () {
		return this._imageColCssClass || " popupMenu_Default_ImageCol";
	},
	set_imageColCssClass: function (value) {
		if (this._imageColCssClass != value) {
			this._imageColCssClass = value;
			this.raisePropertyChanged("imageColCssClass");
		}
	},

	get_separatorCssClass: function () {
		return this._separatorCssClass || " popupMenu_Default_Separator";
	},
	set_separatorCssClass: function (value) {
		if (this._separatorCssClass != value) {
			this._separatorCssClass = value;
			this.raisePropertyChanged("separatorCssClass");
		}
	},

	get_isImageIndent: function () {
		return this._isImageIndent;
	},
	set_isImageIndent: function (value) {
		if (this._isImageIndent != value) {
			this._isImageIndent = value;
			this.raisePropertyChanged("isImageIndent");
		}
	},

	get_textHeadWidth: function () {
		return this._textHeadWidth || 5;
	},
	set_textHeadWidth: function (value) {
		if (this._textHeadWidth != value) {
			this._textHeadWidth = value;
			this.raisePropertyChanged("textHeadWidth");
		}
	},

	get_popupChildControl: function () {
		return this._popupChildControl;
	},

	initialize: function () {
		$HGRootNS.DeluxeMenu.callBaseMethod(this, 'initialize');

		//Sys.UI.DomElement.addCssClass(this.get_element(), this.get_cssClass());
		this._ensureBuildChildContainer();
		this._buildItems(this._items, this);

		//�ı�˵���Ŀ��
		this._changeItemWidth();

		//�����ͣ���ʱ�����˴���ָ���?
		if (this._staticDisplayLevels <= 1 && this._HasControlSeparator)
			this._loadFirstLevel();

		//�������ص�link,�˵���clickʱ��ֵ������
		this._link = $HGDomElement.createElementFromTemplate(
            {
            	nodeName: "A",
            	properties:
                {
                	style:
                  {
                  	display: "none"
                  }
                }
            },
			this.get_element()
        );
	},

	dispose: function () {
		this._items = null;
		this._currentActiveChildNode = null;
		this._selectedItem = null;

		$HGRootNS.DeluxeMenu.callBaseMethod(this, 'dispose');
	},

	_ensureBuildChildContainer: function () {
		if (this.get_hasDynamicChild()) {
			this._buildDynamicChildNodeContainer();
		}
		else {
			if (!this._trChildContainer)
				this._buildStaticChildNodeContainer();
		}
	},

	_buildStaticChildNodeContainer: function () {
		var elt = this.get_element();

		var table = $HGDomElement.createElementFromTemplate(
            {
            	nodeName: "Table",
            	properties:
                {
                	cellPadding: 0,
                	cellSpacing: 0
                },
            	cssClasses: [this.get_cssClass(), "popupMenu_Border"]
            },
            elt
        );

		if (this._Orientation == $HGRootNS.MenuOrientation.Vertical)   //��
		{
			this.set_childNodeContainer(table);
		}
		else if (this._Orientation == $HGRootNS.MenuOrientation.Horizontal)   //����
		{
			var tbody = $HGDomElement.createElementFromTemplate({ nodeName: "tbody" }, table);
			var tr = $HGDomElement.createElementFromTemplate({ nodeName: "tr" }, tbody);
			this.set_childNodeContainer(tr);
		}
	},

	//������̬�ڵ�
	_buildDynamicChildNodeContainer: function () {
		if (!this._popupChildControl) {
			var popupControl = $create($HGRootNS.PopupControl, {}
                , { "beforeShow": this._showEvents }, null, null);
			this._popupChildControl = popupControl;
			this._initDynamicChildNodeContainer();
		}

	},

	_initDynamicChildNodeContainer: function () {
		var popupBody = this._popupChildControl.get_popupBody();

		var popBodyCssClass = this._popBodyCssClass;
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
                	cssClasses: [this.get_cssClass(), "popupMenu_Border"]
                },
                this._popupChildControl.get_popupBody(), null, this._popupChildControl.get_popupDocument()
            );

			this._childNodeContainer = childNodeContainer;
		}
		else {
			$HGDomElement.clearChildren(this._childNodeContainer);
		}
	},

	get_hasDynamicChild: function () {
		return this._staticDisplayLevels == 0;
	},

	//��������
	_buildItems: function (items, parentNode) {
		if (items) {
			var visibleItem = new Array();
			for (var i = 0; i < items.length; i++) {
				var item = items[i];
				//�ı�����
				//��ѡ��ʱ��
				if (!this._MultiSelect && this._singleSelectedCount < 1 && item.selected) {
					++this._singleSelectedCount;
					var menuNode = this.createNode(item, null, null, parentNode);
					this._selectedItem = menuNode;
				}
				else if (!this._MultiSelect)   //ȫ��Ϊfalse
				{
					item.selected = false;
					var menuNode = this.createNode(item, null, null, parentNode);
				}
				//����
				else {
					var menuNode = this.createNode(item, null, null, parentNode);
				}

				//�в㼶�����޸�idֵ
				if (menuNode.get_level() != 1) {
					menuNode.nodeID = menuNode.get_parent().nodeID + "," + (i + 1);

					item.nodeID = menuNode.get_parent().nodeID + "," + (i + 1);
				}
				else   //node is root,first json element(i=0)
				{
					menuNode.nodeID = i + 1;

					item.nodeID = i + 1;
				}

				parentNode.appendChild(menuNode);

				this._buildItems(item.childItems, menuNode);


			} //for end

		} //if end
	},

	_changeItemWidth: function () {
		if (this._Orientation == $HGRootNS.MenuOrientation.Horizontal) {
			var items = 0;
			for (var i = 0; i < this._children.length; i++) {
				if (!this._children[i]._isSeparator || this._children[i]._visible)
					items++;
			}
			if (items > 0) {
				if (this._childNodeContainer.childNodes[0].offsetWidth < this._ItemFontWidth) {
					this._element.childNodes[0].style.width = this._ItemFontWidth * items;
				}
			}
			return;
		}
		//����̬��һ����ʼ��ʱ�Ĵ�С
		if (this._Orientation == $HGRootNS.MenuOrientation.Vertical) {
			var width = this._ItemFontWidth;
			//��̬��һ��
			if (this._staticDisplayLevels == 1) {
				if (this._childNodeContainer.offsetWidth < this._ItemFontWidth) {
					width = this._childNodeContainer.width = this._ItemFontWidth;
				}
			}
		}
	},

	_loadFirstLevel: function ()	//firstLevel
	{
		if (this._children.length > 0) {
			//
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
						//����ָ���
						if (this._children[i]._isSeparator) {
							//������ʾ�ļ�����
							if (!this._searchIsVieibleSeparator(finallyItem, i)) {
								this._children[i]._trItem.style.display = "none";
							}
							//������ʾ�ķָ���
							else {
								if (typeof (this._staticDisplayLevels) != "undefined" && this._staticDisplayLevels != 0 && this._children[i]._trItem.tagName == "TR") {
									this._children[i]._trItem.childNodes[0].childNodes[0].childNodes[0].childNodes[0].childNodes[3].style.width = width - 30;
								}
							}
						}
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
		}
	},

	//��ѯ��ʾ�ļ����зָ����Ƿ��Ǻ�����
	_searchIsVieibleSeparator: function (visibleArr, popupIndex) {
		var visible = false;
		for (var i = 0; i < visibleArr.length; i++) {
			if (visibleArr[i][0] == popupIndex) {
				visible = true;
				break;
			}
		}
		return visible;
	},

	//�˵���table�����Ԫ��
	createChildElement: function () {
		var doc = Sys.UI.DomElement.getParentWindow(this.get_childNodeContainer()).document;

		if (this._Orientation == $HGRootNS.MenuOrientation.Vertical)   //����
		{
			var tbody = doc.createElement("tbody");
			return tbody;
		}
		else if (this._Orientation == $HGRootNS.MenuOrientation.Horizontal)   //����
		{
			var td = doc.createElement("td");
			return td;
		}
	},

	///properties:item����;  parent:�������Ѵ����Ľڵ�
	createNode: function (properties, events, references, parent) {
		var elt = parent.createChildElement();

		//��item��menuItemCollection���ݣ�item�ڵ��ȫ�����ݣ�
		properties.deluxeMenu = this;
		properties.parent = parent;

		var node = $create($HGRootNS.MenuItem, properties, events, references, elt);

		return node;
	}

	//�Զ����¼�
    , add_mclick: function (handler) {
    	this.get_events().addHandler("mclick", handler);
    }
    , remove_mclick: function (handler) {
    	this.get_events().removeHandler("mclick", handler);
    }

    , add_mover: function (handler) {
    	this.get_events().addHandler("mover", handler);
    }
    , remove_mover: function (handler) {
    	this.get_events().removeHandler("mover", handler);
    }
	/**************************postdata************************************/
     , loadClientState: function (value) {

     	if (value) {
     		var deItems = Sys.Serialization.JavaScriptSerializer.deserialize(value);

     		if (deItems != null && deItems.length != 0) {
     			this._receiveArray(this._items, deItems, this.counter);
     		}

     	}
     }

    , saveClientState: function () {

    	if (this._items != null) {
    		this._bulidArray(this._items);
    	}

    	return Sys.Serialization.JavaScriptSerializer.serialize(this._selectedArray);
    }

	/*recursion*/
    , _bulidArray: function (items) {
    	if (items) {
    		for (var i = 0; i < items.length; i++) {
    			Array.add(this._selectedArray, new Array(items[i].nodeID, items[i].selected)); //SetValue(this._Items[i].NodeID

    			this._bulidArray(items[i].childItems);
    		}
    	}
    }

    , _receiveArray: function (items, deItems) {

    	if (items.length != 0) {
    		for (var i = 0; i < items.length; i++) {

    			items[i].nodeID = deItems[this.counter][0];

    			//                    items[i].selected = deItems[this.counter][1];
    			++this.counter;
    			this._receiveArray(items[i].childItems, deItems);

    		}
    	}

    },

	showPopupMenu: function (x, y) {
		if (this._popupChildControl != null && typeof (this._popupChildControl) != "undefined") {
			this._popupChildControl.set_x(x);
			this._popupChildControl.set_y(y);

			this._popupChildControl.show();
		}
	},

	hidePopup: function () {
		if (this._popupChildControl)
			this._popupChildControl.hide();
	},

	_beforeShowCall: function (sender, e) {
		if (e.width < this._ItemFontWidth) {
			e.width = this._ItemFontWidth;
			this.get_childNodeContainer().style.width = this._ItemFontWidth;
		}
	}

}
$HGRootNS.DeluxeMenu.registerClass($HGRootNSName + '.DeluxeMenu', $HGRootNS.TreeNode);
