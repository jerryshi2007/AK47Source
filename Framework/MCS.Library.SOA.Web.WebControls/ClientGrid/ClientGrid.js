/**********************************DataFieldType*****************{*************************************/
$HBRootNS.DataFieldType = function () {
	throw Error.notImplemented();
}

$HBRootNS.DataFieldType.prototype = {
	Object: 1,
	Boolean: 3,
	Integer: 9,
	Decimal: 15,
	DateTime: 16,
	String: 18,
	Enum: 20
}

$HBRootNS.DataFieldType.registerEnum($HBRootNSName + ".DataFieldType");
/*****************************************}*********************************************/

/**********************************GridRowType*****************{*************************************/
$HBRootNS.GridRowType = function () {
	throw Error.notImplemented();
}

$HBRootNS.GridRowType.prototype = {
	HeadRow: 1,
	DataRow: 2
}

$HBRootNS.GridRowType.registerEnum($HBRootNSName + ".GridRowType");
/*****************************************}*********************************************/

/**********************************ColumnEditMode*****************{*************************************/
$HBRootNS.ClientGridColumnEditMode = function () {
	throw Error.notImplemented();
}

$HBRootNS.ClientGridColumnEditMode.prototype = {
	None: 0,
	TextBox: 1,
	DropdownList: 2,
	CheckBox: 3,
	DateInput: 4,
	DateTimeInput: 5,
	OuUserInput: 6
}

$HBRootNS.ClientGridColumnEditMode.registerEnum($HBRootNSName + ".ClientGridColumnEditMode");
/*****************************************}*********************************************/

/**********************************PageSettingMode*****************{*************************************/
$HBRootNS.PageSettingMode = function () {
	throw Error.notImplemented();
}

$HBRootNS.PageSettingMode.prototype = {
	text: 0,
	numberic: 1
}

$HBRootNS.PageSettingMode.registerEnum($HBRootNSName + ".PageSettingMode");
/*****************************************}*********************************************/

/**********************************PagerPosition*****************{*************************************/
$HBRootNS.PagerPosition = function () {
	throw Error.notImplemented();
}

$HBRootNS.PagerPosition.prototype = {
	bottom: 0,
	top: 1,
	topAndBottom: 2
}

$HBRootNS.PagerPosition.registerEnum($HBRootNSName + ".PagerPosition");
/*****************************************}*********************************************/

/**********************************PagerSetting*****************{*************************************/
$HBRootNS.PagerSetting = function () {
	$HBRootNS.PagerSetting.initializeBase(this);
	this.mode = $HBRootNS.PageSettingMode.text;
	this.position = $HBRootNS.PagerPosition.bottom;
	this.firstPageImageUrl = "";
	this.firstPageText = "首页";
	this.lastPageImageUrl = "";
	this.lastPageText = "末页";
	this.nextPageImageUrl = "";
	this.nextPageText = "下一页";
	this.previousPageImageUrl = "";
	this.prevPageText = "上一页";
}

$HBRootNS.PagerSetting.registerClass($HBRootNSName + ".PagerSetting");
/*****************************************}*********************************************/

/********************************ClientPager定义*****************{************************************/
$HBRootNS.GridPager = function (element) {
	$HBRootNS.GridPager.initializeBase(this, [element]);
	this._pageIndex = 0;
	this._pagerSetting = new $HBRootNS.PagerSetting();
	this._pagerStyle = {};
	this._pagerSize = 10;
	this._rowCount = 0;
	this._pagerIndexInput = null;

	this._firstBtn = null;
	this._prevBtn = null;
	this._nextBtn = null;
	this._lastBtn = null;
	this._gotoBtn = null;
	this._pagerDesc = null;

	this._firstBtnEvents = { click: Function.createDelegate(this, this._onFirstBtnClick) };
	this._prevBtnEvents = { click: Function.createDelegate(this, this._onPrevBtnClick) };
	this._nextBtnEvents = { click: Function.createDelegate(this, this._onNextBtnClick) };
	this._lastBtnEvents = { click: Function.createDelegate(this, this._onLastBtnClick) };
	this._gotoBtnEvents = { click: Function.createDelegate(this, this._onGotoBtnClick) };
	this._inputEvents = { keyup: Function.createDelegate(this, this._onpagerIndexInputkeyupdown),
		keydown: Function.createDelegate(this, this._onpagerIndexInputkeyupdown)
	}

	this._textPagerBuilded = false;
	this._numbericPagerBuilded = false;
}

$HBRootNS.GridPager.prototype = {

	initialize: function () {
		$HBRootNS.GridPager.callBaseMethod(this, 'initialize');

		// this.buildPager();
	},

	dispose: function () {
		this._clearEvents();
		this._pageIndex = null;
		this._pagerSetting = null;
		this._pagerStyle = null;
		this._pagerSize = null;
		this._rowCount = null;
		this._pagerIndexInput = null;
		$HBRootNS.GridPager.callBaseMethod(this, 'dispose');
	},

	buildPager: function () {
		this._clearEvents();
		this._clearChildElement();
		if (this._pagerSetting.mode == $HBRootNS.PageSettingMode.text) {
			this._buildTextPager();
		}
		else {
			this._buildNumericPager();
		}
	},

	_clearEvents: function () {
		if (this._firstBtn) {
			$HGDomEvent.removeHandlers(this._firstBtn, this._firstBtnEvents);
			$HGDomEvent.removeHandlers(this._prevBtn, this._prevBtnEvents);
			$HGDomEvent.removeHandlers(this._nextBtn, this._nextBtnEvents);
			$HGDomEvent.removeHandlers(this._lastBtn, this._lastBtnEvents);
			$HGDomEvent.removeHandlers(this._gotoBtn, this._gotoBtnEvents);
		}
	},

	_clearChildElement: function () {
		var elt = this.get_element();
		while (elt.childNodes.length > 0)
			elt.removeChild(elt.childNodes[elt.childNodes.length - 1]);
	},

	_buildTextPager: function () {
		var elt = this.get_element();
		var pageCount = this.get_pageCount();
		this._pagerDesc = $HGDomElement.createElementFromTemplate(
				{
					nodeName: "span",
					properties:
					{
						innerText: String.format("共{1}页{0}条记录", this._rowCount, pageCount)
					}
				},
				elt
			);
		var isFirstPage = (this._pageIndex == 0);
		var isLastPage = (this._pageIndex >= pageCount - 1);
		this._firstBtn = this._buildPagerBtn(elt, this._pagerSetting.firstPageImageUrl, this._pagerSetting.firstPageText, this._firstBtnEvents);
		this._firstBtn.disabled = isFirstPage;
		this._prevBtn = this._buildPagerBtn(elt, this._pagerSetting.previousPageImageUrl, this._pagerSetting.prevPageText, this._prevBtnEvents);
		this._prevBtn.disabled = isFirstPage;
		this._nextBtn = this._buildPagerBtn(elt, this._pagerSetting.nextPageImageUrl, this._pagerSetting.nextPageText, this._nextBtnEvents);
		this._nextBtn.disabled = isLastPage;
		this._lastBtn = this._buildPagerBtn(elt, this._pagerSetting.lastPageImageUrl, this._pagerSetting.lastPageText, this._lastBtnEvents);
		this._lastBtn.disabled = isLastPage;
		this._pagerIndexInput = $HGDomElement.createElementFromTemplate(
				{
					nodeName: "input",
					properties:
					{
						type: "text",
						style: { width: "35px" },
						value: this._pageIndex + 1
					},
					events: this._inputEvents
				},
				elt
			);
		this._gotoBtn = $HGDomElement.createElementFromTemplate(
				{
					nodeName: "input",
					properties:
					{
						type: "button",
						value: "跳转到"
					},
					cssClasses: ["pagerGotoButton"],
					events: this._gotoBtnEvents
				},
				elt
			);
	},

	_buildPagerBtn: function (parent, imgUrl, text, events) {
		var btn = imgUrl ?
			$HGDomElement.createElementFromTemplate(
				{
					nodeName: "img",
					properties:
					{
						src: imgUrl
					},
					events: events
				},
				parent
			)
			:
			$HGDomElement.createElementFromTemplate(
				{
					nodeName: "a",
					properties:
					{
						href: "#",
						innerText: text
					},
					cssClasses: ["pagerButton"],
					events: events
				},
				parent
			);
		$HGDomElement.createElementFromTemplate(
				{
					nodeName: "span",
					properties:
					{
						innerText: " "
					}
				},
				parent
			);
		return btn;
	},

	_buildNumericPager: function () {
	},

	_onFirstBtnClick: function (e) {
		e.rawEvent.returnValue = false;
		if (e.target.disabled)
			return;

		this._changePageIndex(0);
	},

	_onPrevBtnClick: function (e) {
		e.rawEvent.returnValue = false;
		if (e.target.disabled)
			return;

		var pageIndex = (this._pageIndex > 0) ? (this._pageIndex - 1) : 0;
		this._changePageIndex(pageIndex);
	},

	_onNextBtnClick: function (e) {
		e.rawEvent.returnValue = false;
		if (e.target.disabled)
			return;

		var pageIndex = (this._pageIndex < this.get_pageCount() - 1) ? (this._pageIndex + 1) : (this.get_pageCount() - 1);
		this._changePageIndex(pageIndex);
	},

	_onLastBtnClick: function (e) {
		e.rawEvent.returnValue = false;
		if (e.target.disabled)
			return;

		var pageIndex = this.get_pageCount() - 1;
		this._changePageIndex(pageIndex);
	},

	_onGotoBtnClick: function (e) {
		try {
			if (e.target.disabled)
				return;

			var pageIndex = Number.parseInvariant(this._pagerIndexInput.value);

			if (isNaN(pageIndex))
				throw Error.create("请输入正确的页数！");

			var pageCount = this.get_pageCount();

			if (pageIndex < 1 || pageIndex > pageCount)
				throw Error.create(String.format("输入页数必须在1到{0}范围内！", pageCount));

			this._changePageIndex(pageIndex - 1);
		}
		catch (e) {
			$HGRootNS.ClientMsg.inform(e.message);
		}
	},

	_onpagerIndexInputkeyupdown: function (e) {
		var key = e.keyCode;

		if (key == 13) {
			this._onGotoBtnClick(e);
		}
	},

	_changePageIndex: function (pageIndex) {
		this._pageIndex = pageIndex;
		this._raisePageIndexChangedEvent();
	},

	_raisePageIndexChangedEvent: function () {
		var handler = this.get_events().getHandler(this._pageIndexChangedEventKey);
		if (handler) {
			var e = new Sys.EventArgs;
			handler(this, e);
		}
	},

	/*******************************属性{************************************/
	get_pageCount: function () {
		var count = Math.floor((this._rowCount + this._pagerSize - 1) / this._pagerSize);
		if (count == 0) count = 1;
		return count;
	},

	get_pageIndex: function () {
		return this._pageIndex;
	},
	set_pageIndex: function (value) {
		this._pageIndex = value;
	},

	get_pagerSetting: function () {
		return this._pagerSetting;
	},
	set_pagerSetting: function (value) {
		this._pagerSetting = value;
	},

	get_pagerStyle: function () {
		return this._pagerStyle;
	},
	set_pagerStyle: function (value) {
		this._pagerStyle = value;
	},

	get_pagerSize: function () {
		return this._pagerSize;
	},
	set_pagerSize: function (value) {
		this._pagerSize = value;
	},

	get_rowCount: function () {
		return this._rowCount;
	},
	set_rowCount: function (value) {
		this._rowCount = value;
	},
	/**************************************}******************************/

	/**********************************事件{******************************/
	_pageIndexChangedEventKey: "pageIndexChanged",

	add_pageIndexChanged: function (value) {
		this.get_events().addHandler(this._pageIndexChangedEventKey, value);
	},

	remove_pageIndexChanged: function (value) {
		this.get_events().removeHandler(this._pageIndexChangedEventKey, value);
	}
	/**************************************}******************************/

}

$HBRootNS.GridPager.registerClass($HBRootNSName + ".GridPager", $HGRootNS.ControlBase);
/*****************************************}*********************************************/

/**********************************SortDirection*****************{*************************************/
$HBRootNS.SortDirection = function () {
	throw Error.notImplemented();
}

$HBRootNS.SortDirection.prototype = {
	asc: 0,
	desc: 1
}

$HBRootNS.SortDirection.registerEnum($HBRootNSName + ".SortDirection");
/*****************************************}*********************************************/

/**********************************GridCell*****************{*************************************/
$HBRootNS.GridCell = function () {
	$HBRootNS.GridCell.initializeBase(this);
	this.htmlCell = null;
	this.gridColumn = null;
	this.editor = null;
	this.dataField = "";
	this.rowIndex = 0;
}

$HBRootNS.GridCell.prototype = {

	cellDataBind: function (cellData) {
		if (this.dataField) {
			if (this.editor) {
				var value = cellData;
				if (this.editor.get_column().formatString)
					value = String.format(this.editor.get_column().formatString, value);

				switch (this.editor.get_column().editTemplate.EditMode) {
					case $HBRootNS.ClientGridColumnEditMode.None:
						//this.htmlCell.innerText = value;
						this.editor.get_editorElement().innerText = value;
						break;
					case $HBRootNS.ClientGridColumnEditMode.TextBox:
					case $HBRootNS.ClientGridColumnEditMode.DropdownList:
						this.editor.get_editorElement().value = value;
						break;
				}
			}
		}
	}
}

$HBRootNS.GridCell.registerClass($HBRootNSName + ".GridCell");
/*****************************************}*********************************************/

/**********************************GridRow*****************{*************************************/
$HBRootNS.GridRow = function () {
	$HBRootNS.GridRow.initializeBase(this);
	this.rowType = $HBRootNS.GridRowType.DataRow;   //$HBRootNS.GridRowType.HeadRow
	this.gridCells = [];
	this.rowIndex = 0;
	this.rowData = null;
}

$HBRootNS.GridRow.prototype = {

	//给定rowData绑定当前行数据
	rowDataBind: function (rowData) {
		for (var i = 0; i < this.gridCells.length; i++) {
			this.gridCells[i].cellDataBind(rowData[this.gridCells[i].dataField]);
		}
	}
}

$HBRootNS.GridRow.registerClass($HBRootNSName + ".GridRow");
/*****************************************}*********************************************/

/**********************************GridColumn*****************{*************************************/

$HBRootNS.GridColumn = function () {
	$HBRootNS.GridColumn.initializeBase(this);
	this.selectColumn = false;
	this.showSelectAll = false;
	this.dataField = "";
	this.dataType = $HGRootNS.ValidationDataType.String;
	this.maxlength = 0;
	this.formatString = "";
	this.footerStyle = {};
	this.footerText = "";
	this.headerImageUrl = "";
	this.editorStyle = {};
	this.editorTooltips = "";
	this.headerStyle = {};
	this.headerText = "";
	this.nullDisplayText = "";
	this.itemStyle = {};
	this.sortExpression = "";
	this.cellDataBound = null;
	this.editTemplate = null;
}

$HBRootNS.GridColumn.prototype = {

	get_editTemplate: function () {
		return this.editTemplate;
	},

	set_editTemplate: function (value) {
		this.editTemplate = value;
	},

	get_editorStyle: function () {
		return this.editorStyle;
	},

	set_editorStyle: function (value) {
		try {
			this.editorStyle = Sys.Serialization.JavaScriptSerializer.deserialize(value);
		} catch (e) {
			this.editorStyle = {};
			//throw Error.create(this.dataField + "列EditorStyle样式格式不正确！");
			alert(this.dataField + "列EditorStyle样式格式不正确！");
		}
	},

	get_editorTooltips: function () {
		return this.editorTooltips;
	},
	set_editorTooltips: function (value) {
		this.editorTooltips = value;
	}
}

$HBRootNS.GridColumn.registerClass($HBRootNSName + ".GridColumn");
/*****************************************}*********************************************/

/********************************ClientGrid控件定义*****************{************************************/
/***********所有属性，方法，事件的含义，请参见Asp.net 2.0 的GridView控件***************/
$HBRootNS.ClientGrid = function (element) {
	$HBRootNS.ClientGrid.initializeBase(this, [element]);

	if (element.tagName.toLowerCase() !== "table") {
		var e = Error.create("element必须为Table对象！");
		throw (e);
	}

	/*************************初始化ClientGrid控件字段值*{***************************************/
	this._deluxeCalendarControlClientID = "";
	this._allowPaging = false;
	this._allowSorting = false;
	this._alternatingRowStyle = {};
	this._autoPaging = false;
	this._backColor = "";
	this._backImageUrl = "";
	this._borderColor = "";
	this._borderStyle = {};
	this._borderWidth = 0;
	this._bottomPagerRow = null;
	this._caption = "";
	this._captionStyle = {};
	this._captionElement = null;
	this._cellPadding = "3px";
	this._cellSpacing = "1px";
	this._columns = [];
	this._cssClass = "clientGrid";
	this._dataSource = [];
	this._emptyDataRowStyle = {};
	this._emptyDataText = "";
	this._footerRow = null;
	this._footerStyle = {};
	this._headerRow = null;
	this._headerStyle = {};
	this._height = "";
	this._keyFields = [];
	this._pageCount = 0;
	this._pageIndex = 0;
	this._pagerSetting = new $HBRootNS.PagerSetting();
	this._pagerStyle = {};
	this._pagerRow = null;
	this._pageSize = 10;
	this._rows = [];
	this._rowStyle = {};
	this._selectedData = [];
	this._showFooter = false;
	this._showHeader = true;
	this._showSelectColumn = false;
	this._sortDirection = $HBRootNS.SortDirection.asc;
	this._sortExpression = "";
	this._style = {};
	this._topPagerRow = null;

	this._hasSelectColumn = false;
	this._selectedRow = null;

	this._gridTable = null;
	this._mainTable = null;
	this._tHeader = null;
	this._tBody = null;
	this._tFooter = null;
	this._tPager = null;
	this._tTopPager = null;

	this._pagerControl = null;
	this._topPagerControl = null;

	this._headerSelectAllCheckbox = null;
	this._footerSelectAllCheckbox = null;
	this._selectCheckboxList = [];

	this._requireDataBind = false;

	this._readOnly = false;
	this._showEditBar = false;

	this._gridRows = [];

	/*************************初始化ClientGrid控件字段值}*****************************************/

	/*************************事件委托---Function_createDelegate*********************{*************************/
	this._compareDataEqualDelegate = Function.createDelegate(this, this._compareDataEqual);

	this._pagerControlPageIndexChangedEvent = Function.createDelegate(this, this._onPagerControlPageIndexChanged);

	this._selectCheckboxEvents = { click: Function.createDelegate(this, this._onSelectCheckboxClick) };

	this._selectAllCheckboxEvents = { click: Function.createDelegate(this, this._onSelectAllCheckboxClick) };

	this._dataRowEvents = { mouseover: Function.createDelegate(this, this._onDataRowMouseOver),
		mouseout: Function.createDelegate(this, this._onDataRowMouseOut),
		click: Function.createDelegate(this, this._onDataRowClick)
	};

	this._headerCellEvents = { click: Function.createDelegate(this, this._onHeaderCellClick) };

	this._applicationLoad$delegate = Function.createDelegate(this, this._applicationLoad);

	this._addLink = null;
	this._addLinkEvents = { click: Function.createDelegate(this, this._onAddLinkClick) };

	this._deleteLink = null;
	this._deleteLinkEvents = { click: Function.createDelegate(this, this._onDeleteLinkClick) };
	/*************************事件委托---Function_createDelegate*************}*************************/

}

$HBRootNS.ClientGrid.prototype = {

	initialize: function () {
		$HBRootNS.ClientGrid.callBaseMethod(this, 'initialize');
		var elt = this.get_element();
		this._gridTable = elt;

		this._setGridTableProperties();

		this._buildContainers();

		this._ensureDataBind();

		Sys.Application.add_load(this._applicationLoad$delegate);
	},

	dispose: function () {
		this._deluxeCalendarControlClientID = null;
		this._clearDataBindEvents();
		this._allowPaging = null;
		this._allowSorting = null;
		this._alternatingRowStyle = null;
		this._backColor = null;
		this._backImageUrl = null;
		this._borderColor = null;
		this._borderStyle = null;
		this._borderWidth = null;
		this._bottomPagerRow = null;
		this._caption = null;
		this._captionStyle = null;
		this._captionElement = null;
		this._cellPadding = null;
		this._cellSpacing = null;
		this._columns = null;
		this._cssClass = null;
		this._dataSource = null;
		this._dataSourceType = null;
		this._emptyDataRowStyle = null;
		this._emptyDataText = null;
		this._footerRow = null;
		this._footerStyle = null;
		this._headerRow = null;
		this._headerStyle = null;
		this._height = null;
		this._pageCount = null;
		this._pageIndex = null;
		this._pagerSetting = null;
		this._pagerStyle = null;
		this._pageSize = null;
		this._pagerRow = null;
		this._rows = null;
		this._rowStyle = null;
		this._selectedData = null;
		this._showFooter = null;
		this._showHeader = null;
		this._showSelectColumn = null;
		this._sortDirection = null;
		this._sortExpression = null;
		this._style = null;
		this._topPagerRow = null;

		this._hasSelectColumn = null;
		this._selectedRow = null;

		this._gridTable = null;
		this._mainTable = null;
		this._tHeader = null;
		this._tBody = null;
		this._tFooter = null;
		this._tPager = null;
		this._tTopPager = null;

		this._pagerControl = null;
		this._topPagerControl = null;

		this._headerSelectAllCheckbox = null;
		this._footerSelectAllCheckbox = null;
		this._selectCheckboxList = [];

		if (this._applicationLoad$delegate) {
			Sys.Application.remove_load(this._applicationLoad$delegate);
			this._applicationLoad$delegate = null;
		}

		$HBRootNS.ClientGrid.callBaseMethod(this, 'dispose');
	},

	loadClientState: function (value) {
		if (value && value != "") {
			var state = Sys.Serialization.JavaScriptSerializer.deserialize(value);
			this._dataSource = state[0];
			this._dataSourceType = state[1];
		}
	},

	saveClientState: function () {
		var state = "";
		if (this._dataSource) {
			state = Sys.Serialization.JavaScriptSerializer.serialize([this._dataSource, this._dataSourceType]);
		}
		return state;
	},

	/*************************一堆 function*************************{*************************/

	_applicationLoad: function () {
		if (this._dataSource)
			this.set_dataSource(this._dataSource);
	},

	dataBind: function () {
		//		try
		//		{
		this._selectedRow = null;
		this._selectCheckboxList = [];
		this._checkColumns();
		this._clearDataBindEvents();
		this._clearAllRows();
		this._pagerControlDataBind(this._topPagerControl);
		this._pagerControlDataBind(this._pagerControl);
		this._buildHeader();
		this._buildBody();
		this._buildFooter();
		this._requireDataBind = false;
		//        }
		//        catch(e)
		//        {
		//        	$showError(e);
		//        }
	},

	dataRowBindData: function (dataRow, data) {
		this._dataRowBindData(dataRow, data);
	},

	_addDataRow: function (style, cssClass) {
		var dataRow = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "tr",
				properties:
				{
					style: style
				},
				cssClasses: [cssClass],
				events: this._dataRowEvents
			},
			this._tBody
		);
		dataRow.oldClassName = cssClass;
		return dataRow;
	},

	_buildContainers: function () {
		this._buildTopPagerContainer();
		this._mainTable = this._buildChildTable(this._gridTable,
					{
						border: this._borderWidth,
						borderColor: this._borderColor,
						cellPadding: this._cellPadding,
						cellSpacing: this._cellSpacing,
						style: { borderStyle: this._borderStyle }
					});
		$addCssClass(this._mainTable, "mainTable");

		this._tHeader = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "tbody"
			},
			this._mainTable
		);
		Sys.UI.DomElement.setVisible(this._tHeader, this._showHeader);

		this._tBody = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "tbody"
			},
			this._mainTable
		);

		this._tFooter = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "tbody"
			},
			this._mainTable
		);
		Sys.UI.DomElement.setVisible(this._tFooter, this._showFooter);

		this._buildPagerContainer();
	},

	_buildHeader: function () {
		this._headerRow = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "tr",
				properties:
				{
					style: this._headerStyle
				},
				cssClasses: ["header"]
			},
			this._tHeader
		);
		this._buildHeaderContent(this._headerRow, true);
	},

	_buildFooter: function () {
		this._footerRow = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "tr",
				properties:
				{
					style: this._footerStyle
				},
				cssClasses: ["footer"]
			},
			this._tFooter
		);
		this._buildHeaderContent(this._footerRow, false);
	},

	_buildHeaderContent: function (headerRow, isHeader) {
		for (var i = 0; i < this._columns.length; i++) {
			var col = this._columns[i];

			var cell = $HGDomElement.createElementFromTemplate(
				{
					nodeName: "td",
					properties:
					{
						innerText: (isHeader ? col.headerText : col.footerStyle) || "",
						style: (isHeader ? col.headerStyle : col.footerStyle) || {}
					}
				},
				headerRow
			);

			//抛出一个HEAD 行创建单元格事件
			cell = this._raiseHeadCellCreatingEvent(col.dataField, cell, col);

			if (col.selectColumn && col.showSelectAll) {
				var checkbox = $HGDomElement.createElementFromTemplate(
					{
						nodeName: "input",
						properties:
						{
							type: "checkbox"
						},
						events: this._selectAllCheckboxEvents
					},
					cell
				);

				if (isHeader)
					this._headerSelectAllCheckbox = checkbox;
				else
					this._footerSelectAllCheckbox = checkbox;
			}
			else {
				if (isHeader && col.sortExpression) {
					cell.style.cursor = "hand";
					cell.sortExpression = col.sortExpression;

					var imgSpan = $HGDomElement.createElementFromTemplate(
						{
							nodeName: "span",
							properties: { style: { fontFamily: "Webdings"} },
							cssClasses: ["sortSymbol"]
						},
						cell
					);
					cell.sortImageSpan = imgSpan;
					this._setSortHeaderCellState(cell);
					$addHandlers(cell, this._headerCellEvents);
				}
			}
		}
	},

	_buildBody: function () {
		this._buildFirstDataRow();

		if (this._dataSource.length > 0) {
			var dataSource = null;
			if (this._autoPaging) {
				dataSource = [];
				var startCount = this._pageIndex * this._pageSize;
				var endCount = Math.min(startCount + this._pageSize, this._dataSource.length);

				for (var i = startCount; i < endCount; i++) {
					Array.add(dataSource, this._dataSource[i]);
				}
			}
			else
				dataSource = this._dataSource;

			var pageSize = (this._autoPaging && this._pageSize <= dataSource.length) ? this._pageSize : dataSource.length;

			for (var i = 0; i < pageSize; i++) {
				var style = (i % 2) == 0 ? this._rowStyle : this._alternatingRowStyle;
				var cssClass = (i % 2) == 0 ? "item" : "alternatingItem";
				if (this._cssClass && this._cssClass != "clientGrid")
					cssClass = this._cssClass;

				this._buildDataRow(dataSource[i], style, cssClass);
			}
		}
	},

	_buildFirstDataRow: function () {
		var dataRow = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "tr",
				properties:
				{
					style: this._emptyDataRowStyle
				}
			},
			this._tBody
		);

		if (this._dataSource.length == 0 && this.canShowEditBar() == false) {
			$HGDomElement.createElementFromTemplate(
				{
					nodeName: "td",
					properties:
					{
						colSpan: this._columns.length,
						align: "center",
						innerText: this._emptyDataText
					},
					cssClasses: ["item"]
				},
				dataRow
			);
		}
		else {
			if (this.canShowEditBar()) {

				var td = $HGDomElement.createElementFromTemplate({
					nodeName: "td",
					properties:
					{
						colSpan: this._columns.length,
						align: "left"
					},
					cssClasses: ["item"]
				},
				dataRow
				);

				this._addLink = $HGDomElement.createElementFromTemplate({
					nodeName: "a",
					properties:
					{
						href: "#",
						innerText: "添加"
					},
					events: this._addLinkEvents,
					cssClasses: ["btn"]
				},
				td
				);

				this._deleteLink = $HGDomElement.createElementFromTemplate({
					nodeName: "a",
					properties:
					{
						href: "#",
						innerText: "删除"
					},
					events: this._deleteLinkEvents,
					cssClasses: ["btn"]
				},
				td
				);
			}
		}

		this._syncUIBySelectedData();
	},

	_onAddLinkClick: function () {
		this.addNewRow();
	},

	_onDeleteLinkClick: function () {
		if (window.confirm("确认要删除已选择的记录吗？")) {
			var allData = this.get_dataSource();
			var selectedData = this.get_selectedData();

			for (var i = 0; i < selectedData.length; i++) {
				Array.remove(this.get_dataSource(), selectedData[i]);
			}

			//抛出删除行事件
			this._raiseRowDeleteEvent(selectedData, allData);

			Array.clear(selectedData);

			//删除gridRow所有元素（会重新生成tbody...不担心）
			this._cleanGridRow();

			this.set_dataSource(this.get_dataSource());
		}
	},

	_buildDataRow: function (data, style, cssClass) {
		var dataRow = this._addDataRow(style, cssClass);

		this._dataRowBindData(dataRow, data);
	},

	_buildTopPagerContainer: function () {
		this._tTopPager = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "thead"
			},
			this._gridTable
		);
		this._topPagerRow = this._buildPagerRow(this._tTopPager);
		this._captionElement = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "td",
				properties:
				{
					innerText: this._caption,
					style: this._captionStyle
				},
				cssClasses: ["caption"]
			},
			this._topPagerRow
		);
		var controlCell = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "td",
				style: this._pagerStyle
			},
			this._topPagerRow
		);
		Sys.UI.DomElement.setVisible(controlCell, this._allowPaging && (this._pagerSetting.position != $HBRootNS.PagerPosition.bottom));
		this._topPagerControl = this._buildPagerControl(controlCell);

	},

	_buildPagerContainer: function () {
		this._tPager = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "tfoot"
			},
			this._gridTable
		);
		this._pagerRow = this._buildPagerRow(this._tPager);
		var controlCell = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "td",
				style: this._pagerStyle
			},
			this._pagerRow
		);
		Sys.UI.DomElement.setVisible(controlCell, this._allowPaging && (this._pagerSetting.position != $HBRootNS.PagerPosition.top));
		this._pagerControl = this._buildPagerControl(controlCell);
	},

	_buildPagerRow: function (tPager) {
		var table = this._buildChildTable(tPager);

		var pagerRow = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "tr",
				properties:
				{
					style: this._pagerStyle
				},
				cssClasses: ["pager"]
			},
			table
		);

		return pagerRow;
	},

	_buildPagerControl: function (elt) {
		var rowCount = this.get_rowCount();
		var pagerControl = $create($HBRootNS.GridPager, {}, { pageIndexChanged: this._pagerControlPageIndexChangedEvent }, null, elt);
		return pagerControl;
	},

	_buildChildTable: function (pTable, properties) {
		var row = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "tr"
			},
			pTable
		);
		var cell = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "td",
				properties:
				{
				}
			},
			row
		);
		var table = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "table",
				properties: properties || {}
			},
			cell
		);
		table.style.width = "100%";
		table.style.height = "100%";
		return table;
	},

	_changePageIndex: function (pageIndex) {
		this._pageIndex = pageIndex;
		this._requireDataBind = true;
		this._raisePageIndexChangedEvent();
		this._ensureDataBind();
	},

	_checkboxSelectChanged: function (checkbox) {
		var selected = checkbox.checked;
		var data = checkbox.data;

		var index = Array.indexOf(this._selectedData, data, 0, this._compareDataEqualDelegate);
		var hasSelected = (index >= 0);

		if (selected && !hasSelected) Array.add(this._selectedData, data);
		if (!selected && hasSelected) Array.removeAt(this._selectedData, index);

		this._syncUIBySelectedData();
	},

	_syncUIBySelectedData: function () {
		var deleteLinkVisible = this.get_selectedData().length > 0;

		if (this._deleteLink) {
			if (deleteLinkVisible)
				this._deleteLink.style.display = "inline";
			else
				this._deleteLink.style.display = "none";
		}
	},

	_checkColumns: function () {
		var e = null;
		var selectColumnNum = 0;
		for (var i = 0; i < this._columns.length; i++) {
			var col = this._columns[i];
			if (!$HBRootNS.GridColumn.isInstanceOfType(col)) {
				var newCol = new $HBRootNS.GridColumn();
				$setProperties(newCol, col);
				this._columns[i] = newCol;
				col = newCol;
			}
			if (!col.dataField && !col.selectColumn) {
				e = Error.create(String.format("给定列的对象{0}中dataFields属性不能为空！", $Serializer(col)));
				break;
			}

			if (col.selectColumn) selectColumnNum++;

			if (selectColumnNum > 1) {
				e = Error.create("给定列集合中不能包含多个选择列！");
				break;
			}
		}

		if (e) throw (e);

		this._hasSelectColumn = (selectColumnNum > 0);
	},

	_clearAllRows: function () {
		this._clearRows(this._tHeader);
		this._clearRows(this._tBody);
		this._clearRows(this._tFooter);
	},

	_clearDataBindEvents: function () {
		if (this._headerSelectAllCheckbox)
			$HGDomEvent.removeHandlers(this._headerSelectAllCheckbox, this._selectAllCheckboxEvents);

		if (this._footerSelectAllCheckbox)
			$HGDomEvent.removeHandlers(this._footerSelectAllCheckbox, this._selectAllCheckboxEvents);

		if (this._tBody) {
			for (var i = 0; i < this._tBody.childNodes.length; i++)
				$clearHandlers(this._tBody.childNodes[i]);
		}

		for (var i = 0; i < this._selectCheckboxList.length; i++)
			$HGDomEvent.removeHandlers(this._selectCheckboxList[i], this._selectCheckboxEvents);
	},

	_clearRows: function (container) {
		if (container) {
			while (container.rows.length > 0)
				container.deleteRow(container.rows.length - 1);
		}
	},

	_compareDataEqual: function (data1, data2) {
		if (data1 === data2) return true;

		var pNames = this._keyFields;
		if (pNames.length === 0) return false;

		for (var i = 0; i < pNames.length; i++) {
			var pName = pNames[i];
			var v1 = Object.getPropertyValue(data1, pName);
			var v2 = Object.getPropertyValue(data2, pName);
			if (v1 !== v2) return false;
		}

		return true;
	},

	/**********************************************}*************************/

	_cleanGridRow: function () {
		this._gridRows = [];
	},

	_get_gridRowByEditor: function (editor) {
		for (var i = 0; i < this._gridRows.length; i++) {
			for (var j = 0; j < this._gridRows[i].gridCells.length; j++) {
				if (editor === this._gridRows[i].gridCells[j].editor) {
					return this._gridRows[i];
				}
			}
		}
		return null;
	},

	get_gridRowCount: function () {
		return this._gridRows.length;
	},

	_createGridCell: function (gridRow, cell, col, editor, rowData) {
		var gcell = new $HBRootNS.GridCell();

		gcell.htmlCell = cell;
		gcell.gridColumn = col;
		gcell.editor = editor;
		gcell.dataField = col.dataField;
		gcell.rowIndex = gridRow.rowIndex;
		gcell.rowData = rowData;

		Array.add(gridRow.gridCells, gcell);
	},

	_createNewDataObj: function () {
		var newObj = {};
		for (var i = 0; i < this._columns.length; i++) {
			var datafield = this._columns[i].dataField;
			if (datafield != "") {
				newObj[datafield] = this._createDefaultValue(datafield);
			}
		}
		return newObj;
	},

	_createDefaultValue: function (dataField) {
		var result;
		var col = this.get_columns();
		var index = 0;

		for (var i = 0; i < col.length; i++) {
			if (dataField == col[i].dataField) {
				index = i;
				break;
			}
		}

		switch (col[index].dataType) {
			case $HBRootNS.DataFieldType.Object:
				result = null;
				break;
			case $HBRootNS.DataFieldType.Boolean:
				result = false;
				break;
			case $HBRootNS.DataFieldType.Integer:
				result = 0;
				break;
			case $HBRootNS.DataFieldType.Decimal:
				result = 0.0;
				break;
			case $HBRootNS.DataFieldType.DateTime:
				result = Date.minDate;
				break;
			case $HBRootNS.DataFieldType.String:
				result = "";
				break;
			case $HBRootNS.DataFieldType.Enum:
				result = "";
				break;
			default:
				result = "";
				break;
		}

		return result;
	},

	//添加新行
	addNewRow: function () {
		var rowIndex = this.get_gridRowCount();
		var style = (rowIndex % 2) == 0 ? this._rowStyle : this._alternatingRowStyle;
		var cssClass = (rowIndex % 2) == 0 ? "item" : "alternatingItem";
		if (this._cssClass && this._cssClass != "clientGrid")
			cssClass = this._cssClass;

		//如果当前列表没有记录 删除默认无记录行
		var newDataRow = this._getNewDataRow(style, cssClass);
		this._dataRowBindData_AddNewRow(newDataRow);
	},

	_getNewDataRow: function (style, cssClass) {
		var newDataRow = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "tr",
				properties:
				{
					style: style
				},
				cssClasses: [cssClass],
				events: this._dataRowEvents
			},
			this._tBody  //从当前的列表Body中的最后插入新行
		);
		newDataRow.oldClassName = cssClass;
		return newDataRow;
	},

	_dataRowBindData_AddNewRow: function (dataRow) {
		var GridRow = new $HBRootNS.GridRow();
		GridRow.rowIndex = this.get_gridRowCount() + 1;

		var editor = null;
		//行的数据
		dataRow.data = this._createNewDataObj();
		//定义一个新的数据对象
		var newRowData = dataRow.data;
		newRowData.rowIndex = GridRow.rowIndex;
		//遍历每个列
		for (var i = 0; i < this._columns.length; i++) {
			var col = this._columns[i];
			var cell = $HGDomElement.createElementFromTemplate(
				{
					nodeName: "td",
					properties:
					{
						style: col.itemStyle || {}
					}
				},
				dataRow
			);
			if (col.selectColumn) {
				var checkbox = $HGDomElement.createElementFromTemplate(
					{
						nodeName: "input",
						properties:
						{
							type: "checkbox"
						},
						events: this._selectCheckboxEvents
					},
					cell
				);
				checkbox.checked = false;
				checkbox.data = newRowData;
				Array.add(this._selectCheckboxList, checkbox);
			}
			else {

				if (col.editTemplate)
					editor = this._createEditor(col.editTemplate.EditMode, col, newRowData);
				else
					editor = this._createEditor($HBRootNS.ClientGridColumnEditMode.None, col, newRowData)

				editor.createEditor(cell);
				editor.dataToEditor();
			}
			//创建Cell  旧的
			this._createGridCell(GridRow, cell, col, editor, newRowData);
		}
		//
		Array.add(this._gridRows, GridRow);
		//将新行数据结构放入数据源中
		Array.add(this._dataSource, newRowData);
	},

	_dataRowBindData: function (dataRow, data) {
		var GridRow = new $HBRootNS.GridRow();
		GridRow.rowIndex = this.get_gridRowCount() + 1;
		data.rowIndex = GridRow.rowIndex;

		dataRow.data = data;
		var checked = Array.indexOf(this._selectedData, data, 0, this._compareDataEqualDelegate) >= 0;
		for (var i = 0; i < this._columns.length; i++) {
			var col = this._columns[i];
			var editor = null;
			var cell = $HGDomElement.createElementFromTemplate(
				{
					nodeName: "td",
					properties:
					{
						style: col.itemStyle || {}
					}
				},
				dataRow
			);
			if (col.selectColumn) {
				var checkbox = $HGDomElement.createElementFromTemplate(
					{
						nodeName: "input",
						properties:
						{
							type: "checkbox"
						},
						events: this._selectCheckboxEvents
					},
					cell
				);
				checkbox.checked = checked;
				checkbox.data = data;
				Array.add(this._selectCheckboxList, checkbox);
			}
			else {

				if (this.get_readOnly()) {
					editor = this._createEditor($HBRootNS.ClientGridColumnEditMode.None, col, data)
				}
				else {
					if (col.editTemplate)
						editor = this._createEditor(col.editTemplate.EditMode, col, data);
					else
						editor = this._createEditor($HBRootNS.ClientGridColumnEditMode.None, col, data)
				}
				editor.createEditor(cell);
				editor.dataToEditor();
			}
			//抛出单元格数据绑定事件
			this._raiseCellDataBoundEvent(dataRow, cell, col, data);

			//创建GridCell
			this._createGridCell(GridRow, cell, col, editor, data);
		}

		Array.add(this._gridRows, GridRow);

		if (checked && !this._hasSelectColumn)
			this._setSelectedRowState(dataRow);
	},

	//创建一个新列的template Editor
	_createEditor: function (editMode, column, rowData) {
		var editor = null;

		switch (editMode) {
			case $HBRootNS.ClientGridColumnEditMode.None:
				editor = new $HBRootNS.GridColumnStaticTextEditor(column, rowData, this);
				break;
			case $HBRootNS.ClientGridColumnEditMode.TextBox:
				editor = new $HBRootNS.GridColumnTextBoxEditor(column, rowData, this);
				break;
			case $HBRootNS.ClientGridColumnEditMode.DropdownList:
				editor = new $HBRootNS.GridColumnDropDownListEditor(column, rowData, this);
				break;
			case $HBRootNS.ClientGridColumnEditMode.DateInput:
				editor = new $HBRootNS.GridColumnDateInputEditor(column, rowData, this);
				break;
			default:
				throw Error.create("EditMode: " + editMode + "未实现");
				break;
		}
		editor.RandomKey = Math.random(); //给editor一个随机值
		return editor;
	},

	//给定rowData绑定当前行数据
	bindRowData: function (rowData) {
	},

	/*************************一堆 事件处理start*************************{*************************/
	_ensureDataBind: function () {
		if (this._requireDataBind)
			this.dataBind();
	},

	_getColumnCount: function () {
		return this._columns.length + (this._showSelectColumn ? 1 : 0);
	},

	_onDataRowMouseOut: function (e) {
		var row = e.handlingElement;
		if (this._selectedRow !== row)
			row.className = row.oldClassName;
	},

	_onDataRowMouseOver: function (e) {
		var row = e.handlingElement;
		if (this._selectedRow !== row)
			row.className = "hoveringItem";
	},

	_onDataRowClick: function (e) {
		if (!this._hasSelectColumn) {
			var row = e.handlingElement;
			this._setSelectedRowState(row);
			this._selectedData = [row.data];
		}
	},

	_setSelectedRowState: function (row) {
		if (this._selectedRow)
			this._selectedRow.className = this._selectedRow.oldClassName;
		row.className = "selectedItem";
		this._selectedRow = row;
	},

	_onHeaderCellClick: function (e) {
		var oldSortCell = null;
		if (this._sortExpression) {
			for (var i = 0; i < this._headerRow.cells.length; i++) {
				var tempCell = this._headerRow.cells[i];
				if (tempCell.sortExpression == this._sortExpression) {
					oldSortCell = tempCell;
					break;
				}
			}
		}
		var cell = e.handlingElement;
		if (cell.sortExpression != this._sortExpression) {
			this._sortExpression = cell.sortExpression;
			this._sortDirection = $HBRootNS.SortDirection.asc;
		}
		else {
			if (this._sortDirection == $HBRootNS.SortDirection.asc)
				this._sortDirection = $HBRootNS.SortDirection.desc;
			else
				this._sortDirection = $HBRootNS.SortDirection.asc;
		}
		if (oldSortCell)
			this._setSortHeaderCellState(oldSortCell);
		this._setSortHeaderCellState(cell);
		this._requireDataBind = true;
		this._raiseSortedEvent();
		if (this._requireDataBind)
			this._dataSource.sort(Function.createDelegate(this, this._defaultSortDataSource));
		this._ensureDataBind();
	},

	_defaultSortDataSource: function (data1, data2) {
		var pName = this.get_sortExpression();
		var sortDirection = this.get_sortDirection();

		v1 = data1[pName];
		v2 = data2[pName];

		if (v1 == v2) return 0;
		if (sortDirection == $HBRootNS.SortDirection.asc)
			result = v1 > v2 ? 1 : -1;
		else
			result = v1 > v2 ? -1 : 1;

		return result;
	},

	_onPagerControlPageIndexChanged: function (pagerControl, e) {
		this._changePageIndex(pagerControl.get_pageIndex());
	},

	_onSelectAllCheckboxClick: function (e) {
		var selected = e.target.checked;
		this._headerSelectAllCheckbox.checked = selected;
		this._footerSelectAllCheckbox.checked = selected;
		for (var i = 0; i < this._selectCheckboxList.length; i++) {
			var cb = this._selectCheckboxList[i];
			if (cb.checked !== selected) {
				cb.checked = selected;
				this._checkboxSelectChanged(cb);
			}
		}
	},

	_onSelectCheckboxClick: function (e) {
		this._checkboxSelectChanged(e.target);
	},

	_pagerControlDataBind: function (pagerControl) {
		var rowCount = this.get_rowCount();
		$setProperties(pagerControl, { rowCount: rowCount, pagerSize: this._pageSize, pagerSetting: this._pagerSetting, pageIndex: this._pageIndex });
		pagerControl.buildPager();
	},

	//构建Editor时抛出的事件  Editor （new）
	_raiseCellCreatingEditorEvent: function (container, editor, col, rowData, valueTobeChange) {
		var handler = this.get_events().getHandler(this._cellCreatingEditorEventKey);
		var e = new Sys.EventArgs;
		e.column = col;
		e.rowData = rowData;
		e.editor = editor;
		e.container = container;
		e.valueTobeChange = valueTobeChange;
		e.showValueTobeChange = valueTobeChange;
		e.autoFormat = true;

		if (handler) {
			handler(this, e);
		}
		return e;
	},

	//构建Editor后抛出的事件  Editor （new）
	_raiseCellCreatedEditorEvent: function (editor, col, rowData) {
		var handler = this.get_events().getHandler(this._cellCreatedEditorEventKey);
		if (handler) {
			var e = new Sys.EventArgs;
			e.column = col;
			e.rowData = rowData;
			e.editor = editor;
			handler(this, e);
		}
	},

	//抛出一个控件验证事件
	_raiseEditorValidateEvent: function (editor, col, rowData) {
		var handler = this.get_events().getHandler(this._editorValidateEventKey);
		if (handler) {
			var e = new Sys.EventArgs;
			e.column = col;
			e.rowData = rowData;
			e.editor = editor;
			handler(this, e);
		}
	},

	//抛出初始化Editor处理事件
	_raiseInitializeEditorEvent: function (editor, col, rowData) {
		var handler = this.get_events().getHandler(this._initializeEditorEventKey);
		if (handler) {
			var e = new Sys.EventArgs;
			e.column = col;
			e.rowData = rowData;
			e.editor = editor;
			handler(this, e);
		}
	},

	//抛出数据加工处理事件
	_raiseDataProcessingEvent: function (editor, col, rowData) {
		var handler = this.get_events().getHandler(this._dataProcessingEventKey);
		if (handler) {
			var e = new Sys.EventArgs;
			e.column = col;
			e.rowData = rowData;
			e.editor = editor;
			handler(this, e);
		}
	},

	//抛出数据格式化处理事件
	_raiseDataFormattingEvent: function (editor, col, rowData, showValueTobeChange) {
		var handler = this.get_events().getHandler(this._dataFormattingEventKey);
		var e = new Sys.EventArgs;
		e.column = col;
		e.rowData = rowData;
		e.editor = editor;
		e.showValueTobeChange = showValueTobeChange;
		if (handler) {
			handler(this, e);
		}
		return e;
	},

	//抛出数据Changing处理事件
	_raiseDataChangingEvent: function (editor, col, rowData, valueTobeChange) {
		var handler = this.get_events().getHandler(this._dataChangingEventKey);

		var e = new Sys.EventArgs;
		e.column = col;
		e.rowData = rowData;
		e.editor = editor;
		e.valueTobeChange = valueTobeChange;

		if (handler) {
			handler(this, e);
		}
		return e;
	},

	//抛出数据Changed处理事件
	_raiseDataChangedEvent: function (editor, col, rowData, valueChanged) {
		var handler = this.get_events().getHandler(this._dataChangedEventKey);
		if (handler) {
			var e = new Sys.EventArgs;
			e.column = col;
			e.rowData = rowData;
			e.editor = editor;

			e.gridRow = this._get_gridRowByEditor(editor);

			if (e.gridRow)
				e.rowIndex = e.gridRow.rowIndex;

			e.valueChanged = valueChanged;
			handler(this, e);
		}
	},

	//删除行事件
	_raiseRowDeleteEvent: function (deletedData, currentData) {
		var handler = this.get_events().getHandler(this._rowDeleteEventKey);
		if (handler) {
			var e = new Sys.EventArgs;
			e.deletedData = deletedData;
			e.currentData = currentData;
			handler(this, e);
		}
	},

	//抛出Head行创建事件
	_raiseHeadCellCreatingEvent: function (dataField, cell, col) {
		var handler = this.get_events().getHandler(this._headCellCreatingEventKey);

		var e = new Sys.EventArgs;
		e.cell = cell;
		e.column = col;
		e.dataField = dataField;

		if (handler) {
			handler(this, e);
		}
		return e.cell;
	},

	_raiseCellDataBoundEvent: function (dataRow, cell, col, data) {
		var handler = this.get_events().getHandler(this._cellDataBoundEventKey);
		if (handler) {
			var e = new Sys.EventArgs;
			e.row = dataRow;
			e.cell = cell;
			e.column = col;
			e.data = data;
			handler(this, e);
		}

		if (col.cellDataBound && typeof (col.cellDataBound) === "function") {
			var e = new Sys.EventArgs;
			e.row = dataRow;
			e.cell = cell;
			e.column = col;
			e.data = data;
			col.cellDataBound(this, e);
		}
	},

	_raisePageIndexChangedEvent: function () {
		var handler = this.get_events().getHandler(this._pageIndexChangedEventKey);
		if (handler) {
			var e = new Sys.EventArgs;
			handler(this, e);
		}
	},

	_raiseSortedEvent: function () {
		var handler = this.get_events().getHandler(this._sortedEventKey);
		if (handler) {
			var e = new Sys.EventArgs;
			handler(this, e);
		}
	},

	_setGridTableProperties: function () {
		this._gridTable.cellPadding = 0;
		this._gridTable.cellSpacing = 0;
		$HGDomElement.setStyle(this._gridTable, this._style);
		$addCssClass(this._gridTable, this._cssClass);
	},

	_setSortHeaderCellState: function (cell) {
		if (cell.sortExpression == this._sortExpression)
			cell.sortImageSpan.innerText = this._sortDirection == $HBRootNS.SortDirection.asc ?
				"5" : "6";
		else
			cell.sortImageSpan.innerText = "";
	},

	/*************************一堆 事件处理end*************************}*************************/

	/**********************************事件*************{******************************/
	_headCellCreatingEventKey: "headCellCreating",
	add_headCellCreating: function (value) {
		this.get_events().addHandler(this._headCellCreatingEventKey, value);
	},
	remove_headCellCreating: function (value) {
		this.get_events().removeHandler(this._headCellCreatingEventKey, value);
	},

	_cellDataBoundEventKey: "cellDataBound",
	add_cellDataBound: function (value) {
		this.get_events().addHandler(this._cellDataBoundEventKey, value);
	},
	remove_cellDataBound: function (value) {
		this.get_events().removeHandler(this._cellDataBoundEventKey, value);
	},

	_pageIndexChangedEventKey: "pageIndexChanged",
	add_pageIndexChanged: function (value) {
		this.get_events().addHandler(this._pageIndexChangedEventKey, value);
	},
	remove_pageIndexChanged: function (value) {
		this.get_events().removeHandler(this._pageIndexChangedEventKey, value);
	},

	_sortedEventKey: "sort",
	add_sorted: function (value) {
		this.get_events().addHandler(this._sortedEventKey, value);
	},
	remove_sorted: function (value) {
		this.get_events().removeHandler(this._sortedEventKey, value);
	},

	//客户端创建控件发生时的事件
	_cellCreatingEditorEventKey: "cellCreatingEditor",
	add_cellCreatingEditor: function (value) {
		this.get_events().addHandler(this._cellCreatingEditorEventKey, value);
	},
	remove_cellCreatingEditor: function (value) {
		this.get_events().removeHandler(this._cellCreatingEditorEventKey, value);
	},

	//客户端创建控件发生后的事件
	_cellCreatedEditorEventKey: "cellCreatedEditor",
	add_cellCreatedEditor: function (value) {
		this.get_events().addHandler(this._cellCreatedEditorEventKey, value);
	},
	remove_cellCreatedEditor: function (value) {
		this.get_events().removeHandler(this._cellCreatedEditorEventKey, value);
	},

	//客户端控件校验事件
	_editorValidateEventKey: "editorValidate",
	add_editorValidate: function (value) {
		this.get_events().addHandler(this._editorValidateEventKey, value);
	},
	remove_editorValidate: function (value) {
		this.get_events().removeHandler(this._editorValidateEventKey, value);
	},

	//初始化Editor事件
	_initializeEditorEventKey: "initializeEditor",
	add_initializeEditor: function (value) {
		this.get_events().addHandler(this._initializeEditorEventKey, value);
	},
	remove_initializeEditor: function (value) {
		this.get_events().removeHandler(this._initializeEditorEventKey, value);
	},

	//数据加工事件
	_dataProcessingEventKey: "dataProcessing",
	add_dataProcessing: function (value) {
		this.get_events().addHandler(this._dataProcessingEventKey, value);
	},
	remove_dataProcessing: function (value) {
		this.get_events().removeHandler(this._dataProcessingEventKey, value);
	},

	//数据格式化事件
	_dataFormattingEventKey: "dataFormatting",
	add_dataFormatting: function (value) {
		this.get_events().addHandler(this._dataFormattingEventKey, value);
	},
	remove_dataFormatting: function (value) {
		this.get_events().removeHandler(this._dataFormattingEventKey, value);
	},

	//数据dataChanging事件
	_dataChangingEventKey: "dataChanging",
	add_dataChanging: function (value) {
		this.get_events().addHandler(this._dataChangingEventKey, value);
	},
	remove_dataChanging: function (value) {
		this.get_events().removeHandler(this._dataChangingEventKey, value);
	},

	//数据Changed事件
	_dataChangedEventKey: "dataChanged",
	add_dataChanged: function (value) {
		this.get_events().addHandler(this._dataChangedEventKey, value);
	},
	remove_dataChanged: function (value) {
		this.get_events().removeHandler(this._dataChangedEventKey, value);
	},

	//删除行事件
	_rowDeleteEventKey: "rowDelete",
	add_rowDelete: function (value) {
		this.get_events().addHandler(this._rowDeleteEventKey, value);
	},
	remove_rowDelete: function (value) {
		this.get_events().removeHandler(this._rowDeleteEventKey, value);
	},
	/**************************************}******************************/

	/********************************** Get Set ************* {****************************/
	get_deluxeCalendarControlClientID: function () {
		return this._deluxeCalendarControlClientID;
	},

	set_deluxeCalendarControlClientID: function (value) {
		this._deluxeCalendarControlClientID = value;
	},

	get_allowPaging: function () {
		return this._allowPaging;
	},
	set_allowPaging: function (value) {
		this._allowPaging = value;
	},

	get_allowSorting: function () {
		return this._allowSorting;
	},
	set_allowSorting: function (value) {
		this._allowSorting = value;
	},

	get_alternatingRowStyle: function () {
		return this._alternatingRowStyle;
	},
	set_alternatingRowStyle: function (value) {
		this._alternatingRowStyle = value;
	},

	get_autoPaging: function () {
		return this._autoPaging;
	},
	set_autoPaging: function (value) {
		this._autoPaging = value;
	},

	get_backColor: function () {
		return this._backColor;
	},
	set_backColor: function (value) {
		this._backColor = value;
	},

	get_backImageUrl: function () {
		return this._backImageUrl;
	},
	set_backImageUrl: function (value) {
		this._backImageUrl = value;
	},

	get_borderColor: function () {
		return this._borderColor;
	},
	set_borderColor: function (value) {
		this._borderColor = value;
	},

	get_readOnly: function () {
		return this._readOnly;
	},
	set_readOnly: function (value) {
		this._readOnly = value;
	},

	get_showEditBar: function () {
		return this._showEditBar;
	},
	set_showEditBar: function (value) {
		this._showEditBar = value;
	},

	get_borderStyle: function () {
		return this._borderStyle;
	},
	set_borderStyle: function (value) {
		this._borderStyle = value;
	},

	get_borderWidth: function () {
		return this._borderWidth;
	},
	set_borderWidth: function (value) {
		this._borderWidth = value;
	},

	get_bottomPagerRow: function () {
		return this._bottomPagerRow;
	},
	set_bottomPagerRow: function (value) {
		this._bottomPagerRow = value;
	},

	get_caption: function () {
		return this._caption;
	},
	set_caption: function (value) {
		this._caption = value;
		if (this.get_isInitialized())
			this._captionElement.innerText = value;
	},

	get_captionElement: function () {
		return this._captionElement;
	},

	get_captionStyle: function () {
		return this._captionStyle;
	},
	set_captionStyle: function (value) {
		this._captionStyle = value;
	},

	get_cellPadding: function () {
		return this._cellPadding;
	},
	set_cellPadding: function (value) {
		this._cellPadding = value;
	},

	get_cellSpacing: function () {
		return this._cellSpacing;
	},
	set_cellSpacing: function (value) {
		this._cellSpacing = value;
	},

	get_columns: function () {
		return this._columns;
	},
	set_columns: function (value) {
		var e = Function._validateParams(arguments, [{ name: "value", mayBeNull: false, type: Array}]);
		if (e) throw e;

		this._columns = value;
	},

	get_cssClass: function () {
		return this._cssClass;
	},
	set_cssClass: function (value) {
		this._cssClass = value;
	},

	get_dataSource: function () {
		return this._dataSource;
	},

	set_dataSource: function (value) {
		var e = Function._validateParams(arguments, [{ name: "value", mayBeNull: false, type: Array}]);

		if (e)
			throw e;

		this._dataSource = value;
		if (this._dataSource.length == 0)
			this._selectedData = [];

		this._requireDataBind = true;

		if (this._autoPaging)
			this._selectedData = [];

		this.dataBind();
	},

	rebind: function () {
		if (this._dataSource)
			this.set_dataSource(this._dataSource);
	},

	get_emptyDataRowStyle: function () {
		return this._emptyDataRowStyle;
	},
	set_emptyDataRowStyle: function (value) {
		this._emptyDataRowStyle = value;
	},

	get_emptyDataText: function () {
		return this._emptyDataText;
	},
	set_emptyDataText: function (value) {
		this._emptyDataText = value;
	},

	get_footerRow: function () {
		return this._footerRow;
	},
	set_footerRow: function (value) {
		this._footerRow = value;
	},

	get_footerStyle: function () {
		return this._footerStyle;
	},
	set_footerStyle: function (value) {
		this._footerStyle = value;
	},

	get_headerRow: function () {
		return this._headerRow;
	},
	set_headerRow: function (value) {
		this._headerRow = value;
	},

	get_headerStyle: function () {
		return this._headerStyle;
	},
	set_headerStyle: function (value) {
		this._headerStyle = value;
	},

	get_height: function () {
		return this._height;
	},
	set_height: function (value) {
		this._height = value;
	},

	get_align: function () {
		return this._align;
	},
	set_align: function (value) {
		this._align = value;
	},

	get_keyFields: function () {

		return this._keyFields;
	},
	set_keyFields: function (value) {
		var e = Function._validateParams(arguments, [{ name: "keyFields", mayBeNull: false, type: Array}]);
		if (e) throw e;
		this._keyFields = value;
	},

	get_pageCount: function () {
		return this._pageCount;
	},
	set_pageCount: function (value) {
		this._pageCount = value;
	},

	get_pageIndex: function () {
		return this._pageIndex;
	},
	set_pageIndex: function (value) {
		this._pageIndex = value;
	},

	get_pagerSetting: function () {
		return this._pagerSetting;
	},
	set_pagerSetting: function (value) {
		$setProperties(this._pagerSetting, value);
	},

	get_pagerStyle: function () {
		return this._pagerStyle;
	},
	set_pagerStyle: function (value) {
		this._pagerStyle = value;
	},

	get_pageSize: function () {
		return this._pageSize;
	},
	set_pageSize: function (value) {
		this._pageSize = value;
	},

	get_rowCount: function () {
		return this._autoPaging ? this._dataSource.length :
			(this._rowCount || this._dataSource.length);
	},
	set_rowCount: function (value) {
		this._rowCount = value;
	},

	get_rows: function () {
		return this._rows;
	},
	set_rows: function (value) {
		this._rows = value;
	},

	get_rowStyle: function () {
		return this._rowStyle;
	},
	set_rowStyle: function (value) {
		this._rowStyle = value;
	},

	get_selectedData: function () {
		return this._selectedData;
	},
	set_selectedData: function (value) {
		var e = Function._validateParams(arguments, [{ name: "selectedData", mayBeNull: false, type: Array}]);
		if (e) throw e;
		this._selectedData = value;
	},

	get_showFooter: function () {
		return this._showFooter;
	},
	set_showFooter: function (value) {
		this._showFooter = value;
		if (this.get_isInitialized())
			Sys.UI.DomElement.setVisible(this._tFooter, this._showFooter);
	},

	get_showHeader: function () {
		return this._showHeader;
	},
	set_showHeader: function (value) {
		this._showHeader = value;
		if (this.get_isInitialized())
			Sys.UI.DomElement.setVisible(this._tHeader, this._showHeader);

	},

	get_showSelectColumn: function () {
		return this._showSelectColumn;
	},
	set_showSelectColumn: function (value) {
		this._showSelectColumn = value;
	},

	get_sortDirection: function () {
		return this._sortDirection;
	},
	set_sortDirection: function (value) {
		this._sortDirection = value;
	},

	get_sortExpression: function () {
		return this._sortExpression;
	},
	set_sortExpression: function (value) {
		this._sortExpression = value;
	},

	get_style: function () {
		return this.get_element().style;
	},
	set_style: function (value) {
		if (this.get_isInitialized()) {
			$HGDomElement.setStyle(this.get_element(), value);
		}
		this._style = value;
	},

	get_topPagerRow: function () {
		return this._topPagerRow;
	},
	set_topPagerRow: function (value) {
		this._topPagerRow = value;
	},
	/**********************************get set}****************************/

	canShowEditBar: function () {
		return this.get_showEditBar() && this.get_readOnly() == false;
	}
}

$HBRootNS.ClientGrid.registerClass($HBRootNSName + ".ClientGrid", $HGRootNS.ControlBase);
/*****************************************}*********************************************/

/**********************************GridColumnEditorBase*****************{********************************/
$HBRootNS.GridColumnEditorBase = function (column, rowData, clientGrid) {
	$HBRootNS.GridColumnEditorBase.initializeBase(this);
	this._column = column;
	this._rowData = rowData;
	this._editorElement = null;
	this._clientGrid = clientGrid;
	this._rowIndex = rowData.rowIndex;
}

$HBRootNS.GridColumnEditorBase.prototype = {
	get_column: function () {
		return this._column;
	},

	get_rowData: function () {
		return this._rowData;
	},

	get_editorElement: function () {
		return this._editorElement;
	},
	set_editorElement: function (value) {
		this._editorElement = value;
	},

	get_clientGrid: function () {
		return this._clientGrid;
	},

	createEditor: function (container) {
		var newNode = null;

		if (this._column.editTemplate) {
			if (this._column.editTemplate.TemplateControlClientID != null &&
                this._column.editTemplate.TemplateControlClientID != "" &&
                this.get_clientGrid().get_readOnly() == false) {

				//注意这里cloneNode 会出现ID重复的问题，通过下面方法解决问题。
				if (!this._column.editTemplate.templateControl)
					this._column.editTemplate.templateControl = $get(this._column.editTemplate.TemplateControlClientID);

				var templateControl = this._column.editTemplate.templateControl;

				newNode = templateControl.cloneNode(true);
				newNode.style.property = this.get_column().editorStyle;
				newNode.id = newNode.uniqueID;

				container.appendChild(newNode);
			}
		}

		this._editorElement = newNode;

		return this._editorElement;
	},

	dataToEditor: function () {
	},

	validationBinder: function (control) {
		switch (this.get_column().dataType) {
			case $HGRootNS.ValidationDataType.Integer:
				this.bindControl(control, $HGRootNS.ValidationDataType.Integer, this.get_column().formatString);
				break;
			case $HGRootNS.ValidationDataType.Decimal:
				this.bindControl(control, $HGRootNS.ValidationDataType.Decimal, this.get_column().formatString);
				break;
			default:
				this.bindControl(control, $HGRootNS.ValidationDataType.String, this.get_column().formatString);
				break;
		}
	},

	bindControl: function (control, dataType, formatString) {
		var binder = new $HBRootNS.TextBoxValidationBinder();
		binder.set_control(control);
		binder.set_dataType(dataType);
		binder.set_formatString(formatString);
		binder.add_dataChange(Function.createDelegate(this, this._onDataChange)); //binder里头抛出的事件  
		binder.bind();
	},

	_onDataChange: function (binder, e) {
		this.set_dataFieldDataByEvent(e.strongTypeValue);
		this._raiseEditorValidateEvent();
	},

	//创建完控件之后抛出一个事件--客户端创建控件发生时的事件
	_raiseCellCreatingEditorEvent: function (container) {
		return this.get_clientGrid()._raiseCellCreatingEditorEvent(container, this, this.get_column(), this.get_rowData(), this.get_dataFieldData());
	},

	//创建完控件之后抛出一个事件--客户端创建控件发生时的事件
	_raiseCellCreatedEditorEvent: function () {
		this.get_clientGrid()._raiseCellCreatedEditorEvent(this, this.get_column(), this.get_rowData());
	},

	//抛出一个事件--客户端控件校验事件
	_raiseEditorValidateEvent: function () {
		this.get_clientGrid()._raiseEditorValidateEvent(this, this.get_column(), this.get_rowData());
	},

	formatValue: function () {
		var result = "";
		var dataValue = this.get_dataFieldData();
		if (this._column.formatString && dataValue) {
			result = String.format(this._column.formatString, dataValue);
		}
		else if (typeof (dataValue) != "undefined") {
			result = dataValue;
		}
		return result;
	},

	get_dataFieldData: function () {
		return this.get_rowData()[this.get_column().dataField];
	},

	set_dataFieldData: function (value) {
		this.get_rowData()[this.get_column().dataField] = value;
	},

	set_dataFieldDataByEvent: function (value) {
		//抛出数据changing事件
		value = this.get_clientGrid()._raiseDataChangingEvent(this, this.get_column(), this.get_rowData(), value).valueTobeChange;

		//数据发生改变
		this.get_rowData()[this.get_column().dataField] = value;

		//抛出数据changed事件
		this.get_clientGrid()._raiseDataChangedEvent(this, this.get_column(), this.get_rowData(), value)
	},

	set_editorTooltips: function () {
		if (this._column.editorTooltips)
			this._editorElement.title = this._column.editorTooltips;
	}
}

$HBRootNS.GridColumnEditorBase.registerClass($HBRootNSName + ".GridColumnEditorBase");
/**********************************GridColumnEditorBase end***}*********************************/

/**********************************GridColumnStaticTextEditor*****************{********************************/
$HBRootNS.GridColumnStaticTextEditor = function (column, rowData, clientGrid) {
	$HBRootNS.GridColumnStaticTextEditor.initializeBase(this, [column, rowData, clientGrid]);
	this._cellCreatingEditorResult = null;
}

$HBRootNS.GridColumnStaticTextEditor.prototype = {

	get_cellCreatingEditorResult: function () {
		return _cellCreatingEditorResult;
	},
	set_cellCreatingEditorResult: function (value) {
		this._cellCreatingEditorResult = value;
	},

	createEditor: function (container) {
		$HBRootNS.GridColumnStaticTextEditor.callBaseMethod(this, 'createEditor', [container]);

		//抛出一个事件--客户端创建控件发生时的事件
		var temp = this._raiseCellCreatingEditorEvent(container);
		if (temp) {
			this.set_cellCreatingEditorResult(temp);
			if (temp.editor) {
				this._editorElement = temp.editor.get_editorElement();
			}
			this.set_dataFieldData(temp.valueTobeChange);
		}

		/*
		if (!this._editorElement) {
		this._editorElement = container;

		//container.style.property = this.get_column().editorStyle

		var styles = this.get_column().editorStyle;
		for (var s in styles) {
		this._editorElement.style[s] = styles[s];
		}
		}*/

		if (!this._editorElement) {
			this._editorElement = $HGDomElement.createElementFromTemplate(
					{
						nodeName: "span",
						properties:
						{
							style: this.get_column().editorStyle
						}
					},
					container
            );
		}

		if (this.get_clientGrid().get_readOnly() == false) {
			//抛出初始化editor事件
			this.get_clientGrid()._raiseInitializeEditorEvent(this, this.get_column(), this.get_rowData());
		}

		this.set_editorTooltips();
	},

	dataToEditor: function () {
		if (this._cellCreatingEditorResult) {
			if (this._cellCreatingEditorResult.showValueTobeChange != this._cellCreatingEditorResult.valueTobeChange)
				this._editorElement.innerText = this._cellCreatingEditorResult.showValueTobeChange;
			else
				this._editorElement.innerText = this._cellCreatingEditorResult.valueTobeChange;
		}

		//Shen Zheng
		if (this._cellCreatingEditorResult.autoFormat) {
			if (this._editorElement) {
				var result = this.formatValue();
				this._editorElement.innerText = result;

				//抛出数据格式化事件
				var returnValue = this.get_clientGrid()._raiseDataFormattingEvent(this, this.get_column(), this.get_rowData(), result);

				this._editorElement.innerText = returnValue.showValueTobeChange;
			}
		}
	}
}

$HBRootNS.GridColumnStaticTextEditor.registerClass($HBRootNSName + ".GridColumnStaticTextEditor", $HBRootNS.GridColumnEditorBase);
/**********************************GridColumnStaticTextEditor end***}*********************************/

/**********************************GridColumnTextBoxEditor*****************{********************************/
$HBRootNS.GridColumnTextBoxEditor = function (column, rowData, clientGrid) {
	$HBRootNS.GridColumnTextBoxEditor.initializeBase(this, [column, rowData, clientGrid]);
}

$HBRootNS.GridColumnTextBoxEditor.prototype = {

	createEditor: function (container) {
		$HBRootNS.GridColumnTextBoxEditor.callBaseMethod(this, 'createEditor', [container]);

		//抛出一个事件--客户端创建控件发生时的事件
		var temp = this._raiseCellCreatingEditorEvent(container);
		if (temp) {
			if (temp.editor) {
				this._editorElement = temp.editor.get_editorElement();
			}
			this.set_dataFieldData(temp.valueTobeChange);
		}

		if (!this._editorElement) {
			this._editorElement = $HGDomElement.createElementFromTemplate(
					{
						nodeName: "input",
						properties:
						{
							type: "text",
							style: this.get_column().editorStyle
						}
					},
					container
            );
		}

		if (this.get_column().maxlength > 0) {
			this._editorElement.setAttribute("maxlength", this.get_column().maxlength);
		}

		//绑定校验
		this.validationBinder(this._editorElement);

		if (this.get_clientGrid().get_readOnly() == false) {
			//抛出初始化editor事件
			this.get_clientGrid()._raiseInitializeEditorEvent(this, this.get_column(), this.get_rowData());
		}

		this.set_editorTooltips();
	},

	dataToEditor: function () {
		if (this._editorElement) {
			var result = this.formatValue();
			this._editorElement.value = result;

			//抛出数据格式化事件
			var returnValue = this.get_clientGrid()._raiseDataFormattingEvent(this, this.get_column(), this.get_rowData(), result);

			this._editorElement.value = returnValue.showValueTobeChange;
		}
	}

}

$HBRootNS.GridColumnTextBoxEditor.registerClass($HBRootNSName + ".GridColumnTextBoxEditor", $HBRootNS.GridColumnEditorBase);
/**********************************GridColumnTextBoxEditor end***}*********************************/

/**********************************GridColumnDropDownListEditor*****************{********************************/
$HBRootNS.GridColumnDropDownListEditor = function (column, rowData, clientGrid) {
	$HBRootNS.GridColumnDropDownListEditor.initializeBase(this, [column, rowData, clientGrid]);
	this._dropDownListEvents = { change: Function.createDelegate(this, this._onSelectedIndexChanged) };
}

$HBRootNS.GridColumnDropDownListEditor.prototype = {

	_get_firstNotEmptyItem: function (ddl) {
		for (var i = 0; i < ddl.children.length; i++) {
			if (ddl.children[i].value != "") {
				return ddl.children[i];
			}
		}
		return null;
	},

	createEditor: function (container) {
		$HBRootNS.GridColumnDropDownListEditor.callBaseMethod(this, 'createEditor', [container]);

		//抛出一个事件--客户端创建控件发生时的事件
		var temp = this._raiseCellCreatingEditorEvent(container);
		if (temp) {
			if (temp.editor) {
				this._editorElement = temp.editor.get_editorElement();
				if (this._editorElement) {
					//默认第一项非空项选中 注意temp.valueTobeChange != "" 表示是有数据，该哪哪，不管
					if (this._editorElement.children.length > 0 && temp.valueTobeChange == "") {
						var firstNotEmptyItem = this._get_firstNotEmptyItem(this._editorElement);
						if (firstNotEmptyItem)
							temp.valueTobeChange = firstNotEmptyItem.value;
					}
				}
			}
			this.set_dataFieldData(temp.valueTobeChange);
		}

		if (this._editorElement) {
			$addHandlers(this._editorElement, this._dropDownListEvents);
		}
		else {
			this._editorElement = $HGDomElement.createElementFromTemplate(
				{
					nodeName: "select",
					style: this.get_column().editorStyle,
					events: this._dropDownListEvents
				},
				container);
		}

		//抛出editor创建后事件
		this._raiseCellCreatedEditorEvent();

		if (this.get_clientGrid().get_readOnly() == false) {
			//抛出初始化editor事件
			this.get_clientGrid()._raiseInitializeEditorEvent(this, this.get_column(), this.get_rowData());
		}

		this.set_editorTooltips();
	},

	dataToEditor: function () {
		if (this._editorElement) {
			var result = this.formatValue();
			this._editorElement.value = result;

			//抛出数据格式化事件
			var returnValue = this.get_clientGrid()._raiseDataFormattingEvent(this, this.get_column(), this.get_rowData(), result);

			this._editorElement.value = returnValue.showValueTobeChange;
		}
	},

	_onSelectedIndexChanged: function (e) {
		this.set_dataFieldDataByEvent(e.target.value);

		this._raiseEditorValidateEvent();
	}
}

$HBRootNS.GridColumnDropDownListEditor.registerClass($HBRootNSName + ".GridColumnDropDownListEditor", $HBRootNS.GridColumnEditorBase);
/**********************************GridColumnDropDownListEditor end***}*********************************/

/**********************************GridColumnDateInputEditor*****************{********************************/
$HBRootNS.GridColumnDateInputEditor = function (column, rowData, clientGrid) {
	$HBRootNS.GridColumnDateInputEditor.initializeBase(this, [column, rowData, clientGrid]);
}

$HBRootNS.GridColumnDateInputEditor.prototype = {
	get_generalCalendarID: function () {
		//取到预先下去的日历牌ID，然后找它的日历牌的按钮图片
		var generalCalendarID = this.get_clientGrid().get_deluxeCalendarControlClientID();
		return generalCalendarID;
	},

	createEditor: function (container) {
		$HBRootNS.GridColumnDateInputEditor.callBaseMethod(this, 'createEditor', [container]);

		//抛出一个事件--客户端创建控件发生时的事件
		var temp = this._raiseCellCreatingEditorEvent(container);
		if (temp) {
			if (temp.editor) {
				this._editorElement = temp.editor.get_editorElement();
			}
			this.set_dataFieldData(temp.valueTobeChange);
		}
		if (!this._editorElement) {
			var input = $HGDomElement.createElementFromTemplate(
				        {
				        	nodeName: "input",
				        	properties: { type: "text", title: this._column.editorTooltips },
				        	cssClasses: ["ajax_calendartextbox ajax__calendar_textbox"]
				        }, container);

			var generalCalendarID = this.get_generalCalendarID();
			var calendar = new $HGRootNS.DeluxeCalendar(input);
			calendar.clientInitialize(generalCalendarID);

			//日期发生变化事件
			calendar.add_clientValueChanged(Function.createDelegate(this, this._calendarDataValueChange)); //calendar里头抛出的事件

			this._editorElement = calendar;
		}

		//抛出editor创建后事件
		this._raiseCellCreatedEditorEvent();

		if (this.get_clientGrid().get_readOnly() == false) {
			//抛出初始化editor事件
			this.get_clientGrid()._raiseInitializeEditorEvent(this, this.get_column(), this.get_rowData());
		}

		this.set_editorTooltips();
	},

	dataToEditor: function () {
		if (this._editorElement) {
			var result = this.get_dataFieldData();
			this._editorElement.set_value(result);
		}
	},

	_calendarDataValueChange: function () {
		this.set_dataFieldDataByEvent(this._editorElement.get_value());
		this._raiseEditorValidateEvent();
	}
}

$HBRootNS.GridColumnDateInputEditor.registerClass($HBRootNSName + ".GridColumnDateInputEditor", $HBRootNS.GridColumnEditorBase);
/**********************************GridColumnDateInputEditor end***}*********************************/



