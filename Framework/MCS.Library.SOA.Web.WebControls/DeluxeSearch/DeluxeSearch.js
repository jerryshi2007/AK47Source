
$HGRootNS.DeluxeSearch = function (element) {
	$HGRootNS.DeluxeSearch.initializeBase(this, [element]);
	this.defaultTip = null;
	//this.tipImagePath = null;
	this.controlClientID = null;
	//this.searchImagePath = null;
	this.advancedSearchCss = null;
	this.inputCss = null;
	this.hideButtonClientID = null;
	this.showButtonClientID = null;
	this.searchFieldCss = null;
	this.searchInputClientID = null;
	this.labelSearchTipClientID = null;
	this.searchButtonClientID = null;
	this.advancedSearchClientID = null;
	this.closeLinkButtonClientID = null;
	this.customConditionClientID = null;
	this.advanceSearchContainerClientID = null;
	this.categoryContainerClientID = null;
	this.customSearchContainerClientID = null;
	this.clientGridClientID = null;
	this.deleteButtonImageUrl = null;
	this.advancedSearch = null;
	this.focusInputDelegate = null;
	this.blurInputDelegate = null;
	this.clickAdvancedDelegate = null;
	this.whereSqlItems = null;
	this.sqlClauses = null;
	this._advanceSearching = false;
	this.advanceCondition = null;
	this.clientGrid = null;
	this.clientGridDataSource = null;
};

$HGRootNS.DeluxeSearch.prototype =
 {
 	initialize: function () {
 		$HGRootNS.DeluxeSearch.callBaseMethod(this, "initialize");

 		var input = $get(this.searchInputClientID);
 		var searchButton = $get(this.searchButtonClientID);
 		var advancedButton = $get(this.advancedSearchClientID);
 		var label = $get(this.labelSearchTipClientID);
 		//add style class              
 		if (input.value.length == 0) {
 			input.parentNode.className = this.searchFieldCss;
 		}
 		else {
 			input.parentNode.className = this.searchFieldCss + " focus";
 		}

 		if (advancedButton)
 			advancedButton.className = this.advancedSearchCss;

 		//events
 		this.focusInputDelegate = Function.createDelegate(this, this._onInputFocus);
 		this.blurInputDelegate = Function.createDelegate(this, this._onInputBlur);
 		this.clickAdvancedDelegate = Function.createDelegate(this, this.raiseAdvancedSearch);

 		$addHandler(input, "focus", this.focusInputDelegate);
 		$addHandler(label, "click", Function.createDelegate(this, this._onLabelClick));
 		$addHandler(input, "blur", this.blurInputDelegate);
 		if (advancedButton) {
 			this._initAdvanceContainer();
 			var showBtn = $get(this.showButtonClientID);
 			var hideBtn = $get(this.hideButtonClientID);

 			if (showBtn) {
 				$addHandler(showBtn, "mouseover", Function.createDelegate(this, this._showCondtionList));
 			}
 			if (hideBtn) {
 				$addHandler(hideBtn, "click", Function.createDelegate(this, this._hideCondtionList));
 			}

 			var closeButton = $get(this.closeLinkButtonClientID);
 			$addHandler(advancedButton, "click", Function.createDelegate(this, this._advancedSearchBtnClick));
 			$addHandler(closeButton, "click", Function.createDelegate(this, this._advancedSearchBtnClick)); 			
 		}
 		if (this.clientGridClientID) {
 			this.clientGrid = $find(this.clientGridClientID);
 			if (this.clientGrid) {
 				this.clientGrid.add_cellCreatedEditor(Function.createDelegate(this, this.onClientGridCellCreatedEditor));
 				this.clientGrid.set_dataSourceNoBind(this.clientGridDataSource);
 			}
 		}
 	},

 	_showCondtionList: function () {
 		var showBtn = $get(this.showButtonClientID);
 		Sys.UI.DomElement.setVisible(showBtn, false);

 		var customDiv = $get(this.customConditionClientID);
 		var container = $get(this.advanceSearchContainerClientID);
 		customDiv.style.left = container.clientWidth + "px";
 		Sys.UI.DomElement.setVisible(customDiv, true);
 	},

 	_hideCondtionList: function () {
 		var showBtn = $get(this.showButtonClientID);
 		Sys.UI.DomElement.setVisible(showBtn, true);
 		var customDiv = $get(this.customConditionClientID);
 		Sys.UI.DomElement.setVisible(customDiv, false);
 	},

 	//自定义搜索条件单击事件
 	_conditionClickEventKey: "conditionClick",
 	add_conditionClick: function (value) {
 		this.get_events().addHandler(this._conditionClickEventKey, value);
 	},
 	remove_conditionClick: function (value) {
 		this.get_events().removeHandler(this._conditionClickEventKey, value);
 	},

 	_raiseConditionClickEvent: function (rowData) {
 		var handler = this.get_events().getHandler(this._conditionClickEventKey);

 		var e = new Sys.EventArgs;
 		e.ID = rowData["ID"];
 		e.ConditionContent = rowData["ConditionContent"];

 		if (handler) {
 			handler(this, e);
 		}
 		return e;
 	},

 	onClientGridCellCreatedEditor: function (grid, e) {
 		switch (e.column.dataField) {
 			case "ConditionName":
 				var link = e.editor.get_editorElement();
 				link.innerText = e.rowData["ConditionName"];
 				var control = this;
 				$addHandler(link, "click", function () { control._raiseConditionClickEvent(e.rowData); });
 				break;
 			case "ID":
 				var link = e.editor.get_editorElement();
 				link.innerText = "";
 				$HGDomElement.createElementFromTemplate({
 					nodeName: "img",
 					properties: {
 						src: this.deleteButtonImageUrl,
 						style: { borderWidth: "0px" }
 					}
 				}, link);

 				var context = [];
 				context.push(this);
 				context.push(e.rowData["ID"]);
 				$addHandler(link, "click", Function.createCallback(this.deleteCustomCondition, context));
 				break;
 		}
 	},

 	deleteCustomCondition: function () {
 		if (confirm("确定要删除此搜索条件？")) {
 			var control = arguments[1][0];
 			var id = arguments[1][1];
 			control._invoke("DeleteCustomCondition",
                     [id],
                     Function.createDelegate(control, control._deleteConditionSuccess),
                     Function.createDelegate(control, control._deleteConditionFailed));
 		}
 	},

 	_deleteConditionSuccess: function (result) {
 		var dataSource = this.clientGrid.get_dataSource();
 		for (var i = 0; i < dataSource.length; i++) {
 			if (dataSource[i].ID === result) {
 				Array.removeAt(dataSource, i);
 				break;
 			}
 		}
 		this._rebindConditon(dataSource);
 	},

 	_rebindConditon: function (dataSource) {
 		this.clientGridDataSource = dataSource;
 		this.clientGrid.set_dataSource(dataSource);
 	},

 	_deleteConditionFailed: function (error) {
 		alert(error.message);
 	},

 	_initAdvanceContainer: function () {
 		if (this.customSearchContainerClientID != null && this.customSearchContainerClientID != "") {
 			var container = $get(this.advanceSearchContainerClientID);
 			var customDiv = $get(this.customSearchContainerClientID);
 			var titleTD = $get(this.searchInputClientID).parentNode;
 			if (container) {
 				//container.style.top = titleTD.clientHeight;
 				//                 Sys.UI.DomElement.setVisible(container, false);
 				//                 Sys.UI.DomElement.addCssClass(container, this.advanceSearchContainerCss);
 				customDiv.style.display = "block";
 				container.childNodes[0].appendChild(customDiv);
 				//this._dockContainer(container);
 			}
 			//             var customCondtionDiv = $get(this.customConditionClientID);
 			//             if (customCondtionDiv) {
 			//                 customCondtionDiv.style.top = titleTD.clientHeight;
 			//             }
 		}
 	},
 	_advancedSearchBtnClick: function () {
 		if (this.customSearchContainerClientID == null || this.customSearchContainerClientID == "") {
 			$HGClientMsg.stop("没有指定自定义高级搜索容器ID");
 		}
 		else {
 			var container = $get(this.advanceSearchContainerClientID);
 			if (container) {
 				var categoryContainerClient = $get(this.categoryContainerClientID);
 				if (this._advanceSearching === false) {
 					//                     if (categoryContainerClient) {
 					//                         Sys.UI.DomElement.setVisible(categoryContainerClient, false);
 					//                     }
 					Sys.UI.DomElement.setVisible(container, true);
 				}
 				else {
 					//                     if (categoryContainerClient) {
 					//                         Sys.UI.DomElement.setVisible(categoryContainerClient, true);
 					//                     }
 					Sys.UI.DomElement.setVisible(container, false);
 					this._hideCondtionList();
 				}
 			}
 		}
 		this._advanceSearching = !this._advanceSearching;
 	},

 	//     _dockContainer: function (container) {
 	//         var input = $get(this.searchInputClientID);
 	//         container.style.zIndex = 9999;
 	//         container.style.position = "fixed"; ;
 	//         container.style.left = this._pageX(input.parentNode) + "px";
 	//         container.style.top = this._pageY(input.parentNode) + input.parentNode.offsetHeight + "px";
 	//     },

 	//     _pageX: function (elem) {
 	//         var p = 0;
 	//         while (elem.offsetParent) {
 	//             p += elem.offsetLeft;
 	//             elem = elem.offsetParent;
 	//         }
 	//         return p;
 	//     },

 	//     _pageY: function (elem) {
 	//         var p = 0;
 	//         while (elem.offsetParent) {
 	//             p += elem.offsetTop;
 	//             elem = elem.offsetParent;
 	//         }
 	//         return p;
 	//     },

 	_onLabelClick: function () {
 		var element = $get(this.searchInputClientID);
 		element.focus();
 	},

 	_onInputFocus: function () {
 		var element = $get(this.searchInputClientID);
 		element.parentNode.className = this.searchFieldCss + " focus";
 	},
 	_onInputBlur: function () {
 		var element = $get(this.searchInputClientID);
 		if (element.value == "") {
 			//element.value = this.defaultTip;
 			element.parentNode.className = this.searchFieldCss;
 		}
 	},
 	_advancedSearchEventKey: "advancedSearch",

 	raiseAdvancedSearch: function () {
 		var handler = this.get_events().getHandler(this._advancedSearchEventKey);
 		if (handler) {
 			var e = new Sys.EventArgs();
 			//e.args = this.get_whereSql();
 			handler(this, e);
 			if (typeof (e.resultValue) != "undefined") {
 				this.advanceCondition = e.resultValue;
 				//                 this.loadClientState(e.resultValue);
 				//                 $get(this.searchButtonClientID).click();
 			}
 		}
 	},
 	loadClientState: function (value) {
 		if (value) {
 			var data = Sys.Serialization.JavaScriptSerializer.deserialize(value);

 			this.set_sqlClauses(data[0]);
 			//this._advanceSearching = data[1];
 			var dataSource = data[2];
 			this.clientGridDataSource = dataSource;
 			//this.set_whereSqlClause(data["List"]);
 		}
 	},
 	saveClientState: function () {
 		if (this._advanceSearching) {
 			this.raiseAdvancedSearch();
 		}
 		else {
 			this.advanceCondition = null;
 		}
 		var datas = [];
 		var sqlClauses = this.get_sqlClauses();
 		datas[0] = sqlClauses;
 		datas[1] = this._advanceSearching;
 		datas[2] = this.clientGridDataSource;
 		return Sys.Serialization.JavaScriptSerializer.serialize(datas);
 	},
 	get_whereSql: function () {
 		var element = this._clientStateField;
 		return element.value == "" ? element.defaultValue : element.value;
 	},

 	set_sqlClauses: function (array) {
 		if (this.sqlClauses == null) {
 			this.sqlClauses = new Array();
 		}
 		Array.clear(this.sqlClauses);
 		for (var index = 0; index < array.length; index++) {
 			Array.add(this.sqlClauses, array[index]);
 		}
 	},

 	get_sqlClauses: function () {
 		return this.sqlClauses;
 	},

 	set_whereSqlClause: function (array) {
 		if (this.whereSqlItems == null) {
 			this.whereSqlItems = new Array();
 		}
 		Array.clear(this.whereSqlItems);
 		for (var index = 0; index < array.length; index++) {
 			Array.add(this.whereSqlItems, array[index]);
 		}
 	},

 	get_whereSqlClause: function () {
 		return this.whereSqlItems;
 	},
 	add_advancedSearch: function (handler) {
 		this.get_events().addHandler(this._advancedSearchEventKey, handler);
 	},
 	remove_advancedSearch: function (handler) {
 		this.get_events().removeHandler(this._advancedSearchEventKey, handler);
 	},
 	dispose: function () {
 		var element = this.get_element();

 		$clearHandlers(element);
 		$HGRootNS.DeluxeSearch.callBaseMethod(this, "dispose");
 	}
 };
$HGRootNS.DeluxeSearch.registerClass($HGRootNSName + ".DeluxeSearch", $HGRootNS.ControlBase);