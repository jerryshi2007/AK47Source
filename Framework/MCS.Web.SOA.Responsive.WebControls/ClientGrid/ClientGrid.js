/* File Created: 七月 18, 2014 */
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

/--------------------------pageer 相关 -----------------------------{/

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
        //this.buildPager();
    },

    dispose: function () {
        this._clearEvents();
        this._pageIndex = null;
        this._pagerSetting = null;
        this._pagerStyle = null;
        this._pagerSize = null;
        this._rowCount = null;
        this._pagerIndexInput = null;
        this._autoPaging = true;

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
        while (elt.lastChild)
            elt.removeChild(elt.lastChild);
    },

    _buildTextPager: function () {
        var elt = this.get_element();
        var doc = elt.ownerDocument;
        var pageCount = this.get_pageCount();
        var description = ""
        var li = doc.createElement("li");
        elt.appendChild(li);

        if (this.get_autoPaging())
            description = String.format("共{1}页{0}条记录", this._rowCount, pageCount);
        else
            description = String.format("共{0}条记录", this._rowCount);
        this._pagerDesc = doc.createElement("span");
        this._pagerDesc.className = "from-group-addon";
        li.appendChild(this._pagerDesc);
        this._pagerDesc.appendChild(doc.createTextNode(description));

        var isFirstPage = (this._pageIndex == 0);
        var isLastPage = (this._pageIndex >= pageCount - 1);
        this._firstBtn = this._buildPagerBtn(elt, "icon-double-angle-left", this._firstBtnEvents);
        this._firstBtn.disabled = isFirstPage;
        this._prevBtn = this._buildPagerBtn(elt, "icon-angle-left", this._prevBtnEvents);
        this._prevBtn.disabled = isFirstPage;
        this._nextBtn = this._buildPagerBtn(elt, "icon-angle-right", this._nextBtnEvents);
        this._nextBtn.disabled = isLastPage;
        this._lastBtn = this._buildPagerBtn(elt, "icon-double-angle-right", this._lastBtnEvents);
        this._lastBtn.disabled = isLastPage;

        li = doc.createElement("li");
        elt.appendChild(li);
        var innerDiv = doc.createElement("div");
        innerDiv.className = "input-group clientgrid-pagination-group";
        li.appendChild(innerDiv);

        this._pagerIndexInput = doc.createElement("input");
        this._pagerIndexInput.type = "text";
        innerDiv.appendChild(this._pagerIndexInput);
        this._pagerIndexInput.className = "form-control";
        this._pagerIndexInput.value = this._pageIndex + 1;
        $addHandlers(this._pagerIndexInput, this._inputEvents);

        var grpBtn = doc.createElement("span");
        grpBtn.className = "input-group-btn";
        innerDiv.appendChild(grpBtn);

        this._gotoBtn = doc.createElement("button");
        this._gotoBtn.type = "button";
        grpBtn.appendChild(this._gotoBtn);
        this._gotoBtn.appendChild(doc.createTextNode("跳转到"));
        this._gotoBtn.className = "btn btn-default";
        $addHandlers(this._gotoBtn, this._gotoBtnEvents);
    },

    _buildPagerBtn: function (parent, iconClass, events) {
        var doc = this.get_element().ownerDocument, btn, li, i;
        li = doc.createElement("li");
        i = doc.createElement("i");
        i.className = iconClass;
        parent.appendChild(li);

        btn = document.createElement("a");
        li.appendChild(btn);
        btn.href = "javascript:void(0);";
        btn.appendChild(i);

        $addHandlers(btn, events);

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
            if (isNaN(pageIndex)) throw Error.create("请输入正确的页数！");
            var pageCount = this.get_pageCount();
            if (pageIndex < 1 || pageIndex > pageCount) throw Error.create(String.format("输入页数必须在1到{0}范围内！", pageCount));
            this._changePageIndex(pageIndex - 1);
        }
        catch (e) {
            $HGRootNS.ClientMsg.stop(e.message);
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
    get_autoPaging: function () {
        return this._autoPaging;
    },
    set_autoPaging: function (value) {
        this._autoPaging = value;
    },

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
    Asc: 0,
    Desc: 1
}

$HBRootNS.SortDirection.registerEnum($HBRootNSName + ".SortDirection");
/*****************************************}*********************************************/

/--------------------------pager 相关end ----------------------------}/

/**********************************GridColumn-列定义****************{*************************************/

$HBRootNS.GridColumn = function () {
    $HBRootNS.GridColumn.initializeBase(this);
    this.selectColumn = false;
    this.showSelectAll = false;
    this.dataField = "";
    this.dataType = $HGRootNS.ValidationDataType.String;
    this.maxLength = 0;
    this.formatString = "";
    this.footerStyle = {};
    this.footerText = "";
    this.headerImageUrl = "";
    this.editorStyle = {};
    this.editorTooltips = "";
    this.tag = "";
    this.editorReadOnly = false;
    this.editorEnabled = true;
    this.visible = true;
    this.isDynamicColumn = false;
    this.autoBindingValidation = true;
    this.headerStyle = {};
    this.headerCssClass = null;
    this.headerText = "";
    this.headerTips = "";
    this.headerTipsStyle = { color: 'Red' };
    this.nullDisplayText = "";
    this.itemStyle = {};
    this.itemCssClass = null;
    this.sortExpression = "";
    this.cellDataBound = null;
    this.editTemplate = null;
    this.isFixedLine = true;
    this.isStatistic = false;
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
            $HGClientMsg.info(this.dataField + "列EditorStyle样式格式不正确！", String(value), "格式错误");
        }
    },

    set_itemStyle: function (value) {
        try {
            this.itemStyle = Sys.Serialization.JavaScriptSerializer.deserialize(value);
        } catch (e) {
            this.itemStyle = {};
            $HGClientMsg.info(this.dataField + "列itemStyle样式格式不正确！", String(value), "格式错误");
        }
    },

    get_itemCssClass: function () {
        return this._itemCssClass;
    },

    set_itemCssClass: function (value) {
        this._itemCssClass = value;
    },

    get_headerCssClass: function () {
        return this._headerCssClass;
    },

    set_headerCssClass: function (value) {
        this._headerCssClass = value;
    },

    get_tag: function () {
        return this.tag;
    },
    set_tag: function (value) {
        this.tag = value;
    },

    get_editorTooltips: function () {
        return this.editorTooltips;
    },
    set_editorTooltips: function (value) {
        this.editorTooltips = value;
    },

    get_editorReadOnly: function () {
        return this.editorReadOnly;
    },
    set_editorReadOnly: function (value) {
        this.editorReadOnly = value;
    },

    get_editorEnabled: function () {
        return this.editorEnabled;
    },
    set_editorEnabled: function (value) {
        this.editorEnabled = value;
    },

    get_visible: function () {
        return this.visible;
    },
    set_visible: function (value) {
        this.visible = value;
    },

    get_isDynamicColumn: function () {
        return this.isDynamicColumn;
    },
    set_isDynamicColumn: function (value) {
        this.isDynamicColumn = value;
    },

    get_autoBindingValidation: function () {
        return this.autoBindingValidation;
    },
    set_autoBindingValidation: function (value) {
        this.autoBindingValidation = value;
    },

    get_isFixedLine: function () {
        return this.isFixedLine;
    },
    set_isFixedLine: function (value) {
        this.isFixedLine = value;
    },

    get_isStatistic: function () {
        return this.isStatistic;
    },
    set_isStatistic: function (value) {
        this.isStatistic = value;
    }
}

$HBRootNS.GridColumn.registerClass($HBRootNSName + ".GridColumn");
/*****************************************}*********************************************/

/**********************************GridCell*****************{*************************************/
$HBRootNS.GridCell = function (gridColumn, gridRow, htmlCell) {
    $HBRootNS.GridCell.initializeBase(this);

    this.gridRow = gridRow;
    this.gridColumn = gridColumn;
    this.htmlCell = htmlCell;
    this.editor = null;
    this.dataField = gridColumn.dataField;

    //每构造一个GridRow，同时把当前row加入到grid的_gridRows数组中
    gridRow.add_gridCell(this);
}

$HBRootNS.GridCell.prototype = {

    get_htmlCell: function () {
        return this.htmlCell;
    },

    get_htmlRow: function () {
        return this.gridRow.get_htmlRow();
    },

    get_gridRow: function () {
        return this.gridRow;
    },

    get_column: function () {
        return this.gridColumn;
    },

    get_data: function () {
        return this.editor.get_dataFieldData();
    },
    get_displayData: function () {
        return this.editor.get_displayValue();
    },

    get_editorElement: function () {
        return this.editor.get_editorElement();
    },

    get_editor: function () {
        return this.editor;
    },
    set_editor: function (value) {
        this.editor = value;
    },

    cellDataBind: function () {
        if (this.dataField && this.editor) {

            this.editor.dataToEditor();
        }
    }

}

$HBRootNS.GridCell.registerClass($HBRootNSName + ".GridCell");
/*****************************************}*********************************************/

/**********************************GridRow*****************{*************************************/
$HBRootNS.GridRow = function (grid, rowData, htmlRow) {

    $HBRootNS.GridRow.initializeBase(this);
    this.rowType = $HBRootNS.GridRowType.DataRow;   //$HBRootNS.GridRowType.HeadRow

    this.grid = grid;
    this.gridColumn = grid.get_columns();
    this.rowData = rowData;
    this.htmlRow = htmlRow;
    this.gridCells = [];
    this.rowIndex = grid.get_gridRowCount() + 1;

    //统计列的数量
    this.statisticColumnCount = 0;

    //给行数据也加上行号
    rowData.rowIndex = this.rowIndex;

    //每构造一个GridRow，同时把当前row加入到grid的_gridRows数组中
    grid.add_gridRow(this);
}

$HBRootNS.GridRow.prototype = {

    get_htmlRow: function () {
        return this.htmlRow;
    },

    get_gridCells: function () {
        return this.gridCells;
    },

    get_data: function () {
        return this.rowData;
    },

    get_grid: function () {
        return this.grid;
    },

    get_gridColumn: function () {
        return this.gridColumn;
    },

    add_gridCell: function (gridCell) {
        Array.add(this.gridCells, gridCell);
    },

    cleanGridCell: function () {
        this.gridCells = [];
    },

    get_gridCellByDataField: function (dataField) {
        var returnValue = [];
        for (var i = 0; i < this.gridCells.length; i++) {
            if (this.gridCells[i].dataField == dataField) {
                //return this.gridCells[i];
                Array.add(returnValue, this.gridCells[i]);
            }
        }
        if (returnValue.length == 1) {
            return returnValue[0];
        }
        return returnValue;
    },

    get_editorByDataField: function (dataField) {
        var gridCell = this.get_gridCellByDataField(dataField);
        if (gridCell) {
            return gridCell.get_editor();
        }
        return null;
    },

    get_editorElementByDataField: function (dataField) {
        var gridCell = this.get_gridCellByDataField(dataField);
        if (gridCell) {
            return gridCell.get_editorElement();
        }
        return null;
    },

    //给定rowData绑定当前行数据
    rowDataBind: function (rowData) {
        for (var i = 0; i < this.gridCells.length; i++) {
            //this.gridCells[i].cellDataBind(rowData[this.gridCells[i].dataField]);
            this.gridCells[i].cellDataBind();
        }
    },

    //求得被统计的字段的和
    get_sum: function () {
        var result = 0;
        this.statisticColumnCount = 0;
        for (var i = 0; i < this.gridColumn.length; i++) {
            if (this.gridColumn[i].isStatistic) {
                result = parseFloat(result) + parseFloat(this.rowData[this.gridColumn[i].dataField]);
                this.statisticColumnCount += 1;
            }
        }
        return result;
    },

    //求得被统计的字段的平均值
    get_avg: function () {
        var sum = this.get_sum();
        if (this.statisticColumnCount != 0)
            return sum / this.statisticColumnCount;
    }
}

$HBRootNS.GridRow.registerClass($HBRootNSName + ".GridRow");
/*****************************************}*********************************************/

/********************************ClientGrid 控件定义*****************{************************************/
/***********所有属性，方法，事件的含义，请参见Asp.net 2.0 的GridView控件***************/
$HBRootNS.ClientGrid = function (element) {

    $HBRootNS.ClientGrid.initializeBase(this, [element]);

    if (element.nodeName.toLowerCase() !== "div") {
        var e = Error.create("element必须为一个div");
        throw (e);
    }

    /*************************初始化ClientGrid控件字段值***************************************{*/
    this._dataObjectModel = null;
    this._saveClientStateNeedReturnState = true;
    this._deluxeDateTimePickerID = "";
    this._allowPaging = false;
    this._allowSorting = false;
    this._alternatingRowStyle = {};
    this._autoPaging = false;
    this._backColor = "";
    this._backImageUrl = "";
    this._borderColor = "";
    this._borderStyle = {};

    this._toolbarButtonCssClass = "btn btn-link";
    this.toolbarRowCssClass = "";
    this._borderWidth = 0;
    this._bottomPagerRow = null;
    this._caption = "";
    this._captionStyle = {};
    this._captionElement = null;
    //    this._cellPadding = "3px";
    //    this._cellSpacing = "1px";
    this._columns = [];
    //    this._cssClass = "clientGrid";
    //    this._hoveringItemCssClass = 'hoveringItem';
    //    this._selectedItemCssClass = 'selectedItem';
    this._dataSource = [];
    this._dataSourceType = null;
    this._currentPageDataSource = [];
    this._emptyDataRowStyle = {};
    this._emptyDataText = "";
    this._emptyDataHTML = "";
    this._footerRow = null;
    this._footerRowLeft = null;
    this._footerRowRight = null;
    this._footerStyle = {};
    this._headerRow = null;
    this._headerRowLeft = null;
    this._headerRowRight = null;
    this._headerStyle = {};
    this._height = "";
    this._keyFields = [];
    this._pageCount = 0;
    this._pageIndex = 0;
    this._pagerSetting = new $HBRootNS.PagerSetting();
    this._pagerStyle = {};
    this._bottomPagerRow = null;
    this._pageSize = 10;
    this._rowStyle = {};
    this._selectedData = [];
    this._showFooter = false;
    this._showHeader = true;
    this._showSelectColumn = false;
    this._sortDirection = $HBRootNS.SortDirection.Asc;
    this._sortExpression = "";
    this._style = {};
    this._topPagerRow = null;
    this._toolBarRow = null;

    this._hasSelectColumn = false;
    this._selectedRow = null;

    //    this._gridTable = null;
    //    this._mainTable = null;
    //    this._tHeader = null;
    this._tBody = null;
    this._tFooter = null;
    this._tPager = null;
    this._topContainer = null;

    this._tdFixedLine = null;
    this._tdNotFixedLine = null;
    this._divNotFixedLine = null;

    this._pagerControl = null;
    this._topPagerControl = null;

    this._headerSelectAllCheckbox = null;
    this._footerSelectAllCheckbox = null;
    this._selectCheckboxList = [];

    this._requireDataBind = false;

    this._autoBindOnLoad = true;
    this._renderBatchSize = 20;
    this._readOnly = false;
    this._showEditBar = false;
    this._showCheckBoxColumn = true;
    this._selectedByDefault = false;

    this._fixeLines = 0;
    this._notFixeLines = 0;
    this._rowHeightWithFixeLines = 30;
    this._widthOfNotFixeLines = 500;
    this._heightOfNotFixeLines = 0;
    this._totleWidthOfFixeLines = 0;
    this._totleWidthOfNotFixeLines = 0;
    this._headRowHeightWithFixeLines = 24;
    this._scrollBarHeight = 17;

    this._autoWidthOfNotFixeLines = true;

    this._gridRows = [];

    this._runDataToEditor = true;

    /*************************初始化ClientGrid控件字段值}*****************************************/

    /*************************事件委托---Function_createDelegate*********************{*************************/
    this._compareDataEqualDelegate = Function.createDelegate(this, this._compareDataEqual);

    this._pagerControlPageIndexChangedEvent = Function.createDelegate(this, this._onPagerControlPageIndexChanged);

    this._selectCheckboxEvents = { click: Function.createDelegate(this, this._onSelectCheckboxClick) };

    this._headerSelectAllCheckboxEvents = { click: Function.createDelegate(this, this._onSelectAllCheckboxClick) };
    this._footerSelectAllCheckboxEvents = { click: Function.createDelegate(this, this._onSelectAllCheckboxClick) };

    this._dataRowEvents = {
        //        mouseover: Function.createDelegate(this, this._onDataRowMouseOver),
        //        mouseout: Function.createDelegate(this, this._onDataRowMouseOut),
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

$HBRootNS.ClientGrid._Helper = {
    mergeStyle: function (elem, style) {
        if (typeof (style) === "object") {
            if (elem.nodeType == 1) {
                for (var n in style) {
                    elem.style[n] = style[n];
                }
            }
        }
    }
};

$HBRootNS.ClientGrid.prototype = {

    initialize: function () {
        $HBRootNS.ClientGrid.callBaseMethod(this, 'initialize');
        var elt = this.get_element();

        if (this._notFixeLines > 0 && !this._autoWidthOfNotFixeLines)
            elt.style.width = "";

        //        this._gridTable = elt;

        this._setGridTableProperties();

        this._buildContainers();

        this._ensureDataBind();

        Sys.Application.add_load(this._applicationLoad$delegate);
    },

    dispose: function () {
        this._dataObjectModel = null;
        this._saveClientStateNeedReturnState = null;
        this._deluxeDateTimePickerID = null;
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
        this._currentPageDataSource = null;
        this._dataSourceType = null;
        this._emptyDataRowStyle = null;
        this._emptyDataText = null;
        this._emptyDataHTML = null;
        this._footerRow = null;
        this._footerRowLeft = null;
        this._footerRowRight = null;
        this._footerStyle = null;
        this._headerRow = null;
        this._headerRowLeft = null;
        this._headerRowRight = null;
        this._headerStyle = null;
        this._height = null;
        this._pageCount = null;
        this._pageIndex = null;
        this._pagerSetting = null;
        this._pagerStyle = null;
        this._pageSize = null;
        this._bottomPagerRow = null;
        this._rowStyle = null;
        this._selectedData = null;
        this._showFooter = null;
        this._showHeader = null;
        this._showSelectColumn = null;
        this._sortDirection = null;
        this._sortExpression = null;
        this._style = null;
        //        this._topPagerRow = null;

        this._hasSelectColumn = null;
        this._selectedRow = null;

        //        this._gridTable = null;
        this._mainTable = null;
        this._tHeader = null;
        this._tBody = null;
        this._tFooter = null;
        this._tPager = null;
        this._topContainer = null;

        this._tdFixedLine = null;
        this._tdNotFixedLine = null;
        this._divNotFixedLine = null;

        this._mainTableLeft = null;
        this._mainTableRight = null;
        this._tHeaderLeft = null;
        this._tHeaderRight = null;
        this._tBodyLeft = null;
        this._tBodyRight = null;
        this._tFooterLeft = null;
        this._tFooterRight = null;

        this._pagerControl = null;
        this._topPagerControl = null;

        this._headerSelectAllCheckbox = null;
        this._footerSelectAllCheckbox = null;
        this._selectCheckboxList = [];

        this._runDataToEditor = true;

        if (this._applicationLoad$delegate) {
            Sys.Application.remove_load(this._applicationLoad$delegate);
            this._applicationLoad$delegate = null;
        }

        $HBRootNS.ClientGrid.callBaseMethod(this, 'dispose');
    },

    loadClientState: function (value) {
        if (value && value != "" && value.indexOf("%") != 0) {
            var state = Sys.Serialization.JavaScriptSerializer.deserialize(value);
            this._dataSource = state[0];
            this._dataSourceType = state[1];
        }
    },

    saveClientState: function () {
        var state = "";
        if (this._dataSource && this._saveClientStateNeedReturnState) {
            this._dataSource = this._raise_beforeSaveClientStateEvent(this._dataSource);

            if (this._dataObjectModel)
                this._conversionObject();

            state = encodeURIComponent(Sys.Serialization.JavaScriptSerializer.serialize([this._dataSource, this._dataSourceType]));
        }
        return state;
    },

    _applicationLoad: function () {
        if (this._autoBindOnLoad && this._dataSource) {
            this.set_dataSource(this._dataSource);
        }
    },

    /*************************一堆 function *************************{*************************/
    _dataBind: function () {
        //抛出开始绑定数据事件[可以修改_dataSource结构]
        this._dataSource = this._raiseBeforeDataBindEvent().dataSource;

        this.dataBind();
    },

    dataBind: function () {
        this._selectedRow = null;
        this._selectCheckboxList = [];
        this._selectedData = [];
        this._checkColumns();
        this._clearDataBindEvents();
        this._clearAllRows();
        this._pagerControlDataBind(this._topPagerControl);
        this._pagerControlDataBind(this._pagerControl);
        this._buildHeader();
        this._buildBody(this._afterBuildBody);
        this.followupSettleTable();
    },

    _afterBuildBody: function () {
        this._buildFooter();
        this._requireDataBind = false;
    },

    followupSettleTable: function () {
        if (this._notFixeLines == 0) {
            if (this._tdNotFixedLine.parentNode != null)
                this._tdNotFixedLine.parentNode.removeChild(this._tdNotFixedLine);
            //
        }
        else {
            if (this._autoWidthOfNotFixeLines) {
                var elt = this.get_element();
                this._divNotFixedLine.style["width"] = (elt.offsetWidth - this._totleWidthOfFixeLines).toString() + "px";
            }
            //div height
            this._divNotFixedLine.style["height"] = (this._mainTable_left.offsetHeight + this._scrollBarHeight).toString() + "px";
        }
    },

    defaultSelection: function (e, isChecked) {
        e.checkbox.checked = isChecked;
        this._checkboxSelectChanged(e.checkbox);
    },

    dataRowBindData: function (htmlRow, rowData) {
        this._raiseBeforeDataRowCreateEvent(htmlRow, rowData);
        this._dataRowBindData(htmlRow, rowData);
        this._raiseAfterDataRowCreateEvent(htmlRow, rowData);
    },

    _addDataRow: function (style, cssClass) {
        var doc = this.get_element().ownerDocument;
        var htmlRowLeft = this._tBodyLeft.insertRow(-1);
        if (cssClass)
            htmlRowLeft.className = cssClass;

        $addHandlers(htmlRowLeft, this._dataRowEvents);

        //        var htmlRowLeft = $HGDomElement.createElementFromTemplate(
        //			{
        //			    nodeName: "tr",
        //			    properties:
        //				{
        //				    style: style
        //				},
        //			    cssClasses: [cssClass],
        //			    events: this._dataRowEvents
        //			},
        //			this._tBodyLeft
        //		);
        //		htmlRowLeft.oldClassName = cssClass;

        var htmlRowRight = this._tBodyRight.insertRow(-1);
        if (cssClass)
            htmlRowRight.className = cssClass;

        $addHandlers(htmlRowRight, this._dataRowEvents);

        //        var htmlRowRight = $HGDomElement.createElementFromTemplate(
        //			{
        //			    nodeName: "tr",
        //			    properties:
        //				{
        //				    style: style
        //				},
        //			    cssClasses: [cssClass],
        //			    events: this._dataRowEvents
        //			},
        //			this._tBodyRight
        //		);
        //		htmlRowRight.oldClassName = cssClass;

        return { htmlRowLeft: htmlRowLeft, htmlRowRight: htmlRowRight };
    },

    _buildContainers: function () {
        var doc = this.get_element().ownerDocument;
        this._buildTopPagerContainer();
        this._buildMainTableContainer(this.get_element());


        var tb = this._mainTable_left = doc.createElement("table");
        tb.setAttribute("theID", "_mainTable_left");
        tb.className = "clientgrid-table-left";
        this._tdFixedLine.appendChild(tb);

        //        $HGDomElement.createElementFromTemplate({
        //            nodeName: "table",
        //            properties: { theID: "_mainTable_left", cellSpacing: 0, cellPadding: 0, border: 0, style: { width: "100%"} }
        //        }, this._tdFixedLine);


        tb = this._mainTable_right = doc.createElement("table");
        tb.setAttribute("theID", "_mainTable_right");
        tb.className = "clientgrid-table-right";
        this._divNotFixedLine.appendChild(tb);
        tb.style.width = this._totleWidthOfNotFixeLines + "px";

        //        this._mainTable_right = $HGDomElement.createElementFromTemplate({
        //            nodeName: "table",
        //            properties: { theID: "_mainTable_right", cellSpacing: 0, cellPadding: 0, border: 0, style: { width: this._totleWidthOfNotFixeLines.toString() + "px"} }
        //        }, this._divNotFixedLine);

        this._mainTableLeft = this._buildChildTable(this._mainTable_left);
        this._mainTableLeft.className = "table table-bordered table-hover clientgrid-maintable clientgrid-table-left";

        this._mainTableRight = this._buildChildTable(this._mainTable_right);
        this._mainTableRight.className = "table table-bordered table-hover clientgrid-maintable clientgrid-table-right";

        this._tHeaderLeft = doc.createElement("thead");
        this._mainTableLeft.appendChild(this._tHeaderLeft);

        //        $HGDomElement.createElementFromTemplate(
        //			{
        //			    nodeName: "tbody"
        //			},
        //			this._mainTableLeft
        //		);

        Sys.UI.DomElement.setVisible(this._tHeaderLeft, this._showHeader);

        this._tHeaderRight = doc.createElement("thead");
        this._mainTableRight.appendChild(this._tHeaderRight);

        //        $HGDomElement.createElementFromTemplate(
        //			{
        //			    nodeName: "tbody"
        //			},
        //			this._mainTableRight
        //		);

        Sys.UI.DomElement.setVisible(this._tHeaderRight, this._showHeader);

        this._tBodyLeft = doc.createElement("tbody");
        this._mainTableLeft.appendChild(this._tBodyLeft);

        //        $HGDomElement.createElementFromTemplate(
        //			{
        //			    nodeName: "tbody"
        //			},
        //			this._mainTableLeft
        //		);

        this._tBodyRight = doc.createElement("tbody");
        this._mainTableRight.appendChild(this._tBodyRight);
        //        this._tBodyRight = $HGDomElement.createElementFromTemplate(
        //			{
        //			    nodeName: "tbody"
        //			},
        //			this._mainTableRight
        //		);


        this._tFooterLeft = doc.createElement("tfoot");
        this._mainTableLeft.appendChild(this._tFooterLeft);
        //        $HGDomElement.createElementFromTemplate(
        //			{
        //			    nodeName: "tbody"
        //			},
        //			this._mainTableLeft
        //		);
        Sys.UI.DomElement.setVisible(this._tFooterLeft, this._showFooter);

        this._tFooterRight = doc.createElement("tfoot");
        this._mainTableRight.appendChild(this._tFooterRight);
        //        $HGDomElement.createElementFromTemplate(
        //			{
        //			    nodeName: "tbody"
        //			},
        //			this._mainTableRight
        //		);
        Sys.UI.DomElement.setVisible(this._tFooterRight, this._showFooter);

        this._buildFootPagerContainer();
    },

    _buildHeader: function () {
        //抛出创建标题行前事件
        this._raisePreHeaderRowCreateEvent(this._tHeaderLeft, this._tHeaderRight);

        this._headerRowLeft = this._tHeaderLeft.insertRow(-1);
        this._headerRowLeft.className = "clientgrid-header";
        if (this._headerStyle)
            this._headerRowLeft.style = this._headerStyle;
        //         $HGDomElement.createElementFromTemplate(
        //			{
        //			    nodeName: "tr",
        //			    properties:
        //				{
        //				    style: this._headerStyle
        //				},
        //			    cssClasses: ["header"]
        //			},
        //			this._tHeaderLeft
        //		);

        this._headerRowRight = this._tHeaderRight.insertRow(-1);
        this._headerRowRight.className = "clientgrid-header";
        if (this._headerStyle)
            this._headerRowRight.style = this._headerStyle;
        //        $HGDomElement.createElementFromTemplate(
        //			{
        //			    nodeName: "tr",
        //			    properties:
        //				{
        //				    style: this._headerStyle
        //				},
        //			    cssClasses: ["header"]
        //			},
        //			this._tHeaderRight
        //		);

        this._buildHeaderContent(this._headerRowLeft, this._headerRowRight, true);
    },

    _buildFooter: function () {
        this._footerRowLeft = this._tFooterLeft.insertRow(-1);
        this._footerRowLeft.className = "clientgrid-footer";
        if (this._footerStyle)
            this._footerRowLeft.style = this._footerStyle;
        //         $HGDomElement.createElementFromTemplate(
        //			{
        //			    nodeName: "tr",
        //			    properties:
        //				{
        //				    style: this._footerStyle
        //				},
        //			    cssClasses: ["footer"]
        //			},
        //			this._tFooterLeft
        //		);
        this._footerRowRight = this._tFooterRight.insertRow(-1);
        this._footerRowRight.className = "clientgrid-footer";
        if (this._footerStyle)
            this._footerRowRight.style = this._footerStyle;
        //         $HGDomElement.createElementFromTemplate(
        //			{
        //			    nodeName: "tr",
        //			    properties:
        //				{
        //				    style: this._footerStyle
        //				},
        //			    cssClasses: ["footer"]
        //			},
        //			this._tFooterRight
        //		);
        this._buildHeaderContent(this._footerRowLeft, this._footerRowRight, false);
    },

    _buildHeaderContent: function (headerRowLeft, headerRowRight, isHeader) {
        var doc = (headerRowLeft || headerRowRight).ownerDocument;
        for (var i = 0; i < this._columns.length; i++) {
            var col = this._columns[i];
            if (col.visible) {
                var headerRow = headerRowRight;
                if (col.isFixedLine)
                    headerRow = headerRowLeft;


                //                var headerInnerHtml = (isHeader ? col.headerText : col.footerStyle) || "";

                var cell = doc.createElement("th");
                headerRow.appendChild(cell);

                var style = isHeader ? col.headerStyle : col.footerStyle;
                if (style)
                    cell.style = style;

                if (isHeader && col._headerCssClass) {
                    cell.className = col._headerCssClass;
                }

                cell.appendChild(doc.createTextNode(col.headerText));

                if (col.headerTips) {
                    var span = doc.createElement("span");
                    span.appendChild(doc.createTextNode(col.headerTips));
                    cell.appendChild(span);

                    $HBRootNS.ClientGrid._Helper.mergeStyle(span, col.headerTipsStyle);
                    //                    $HGDomElement.createElementFromTemplate(
                    //                    {
                    //                        nodeName: "span",
                    //                        properties:
                    //					    {
                    //					        innerText: col.headerTips,
                    //					        style: col.headerTipsStyle || {}
                    //					    }
                    //                    });
                    //headerInnerHtml = span.outerHTML + headerInnerHtml;
                    //                    headerInnerHtml = headerInnerHtml + span.outerHTML;
                }

                //                 $HGDomElement.createElementFromTemplate(
                //				{
                //				    nodeName: "td",
                //				    properties:
                //					{
                //					    innerHTML: headerInnerHtml,
                //					    style: (isHeader ? col.headerStyle : col.footerStyle) || {}
                //					}
                //				}, headerRow);


                //抛出一个HEAD 行创建单元格事件
                if (isHeader) {
                    cell = this._raiseHeadCellCreatingEvent(col.dataField, cell, col);
                } else {
                    cell = this._raiseFootCellCreatingEvent(col.dataField, cell, col);
                }

                if (col.selectColumn && this._showCheckBoxColumn) {
                    var checkbox = document.createElement("input");
                    checkbox.type = 'checkbox';
                    cell.appendChild(checkbox);
                    $addCssClass(cell, "clientgrid-column-check");
                    checkbox.setAttribute("data-role", "checkall");
                    $addHandlers(checkbox, isHeader ? this._headerSelectAllCheckboxEvents : this._footerSelectAllCheckboxEvents);

                    //                    $HGDomElement.createElementFromTemplate(
                    //					        {
                    //					            nodeName: "input",
                    //					            properties:
                    //						        {
                    //						            name: "clientGrid_allSelectCheckbox_" + Math.random().toString(),
                    //						            type: "checkbox"
                    //						        },
                    //					            events: isHeader ? this._headerSelectAllCheckboxEvents : this._footerSelectAllCheckboxEvents
                    //					        }, cell);

                    if (!col.showSelectAll) {//之前showSelectAll为假时，直接不创建此控件。现修改为创建且隐藏。 
                        checkbox.style["visibility"] = "hidden";
                    }

                    //抛出复选框创建事件
                    this._raise_selectCheckboxCreatedEvent(cell, checkbox, null);

                    if (isHeader)
                        this._headerSelectAllCheckbox = checkbox;
                    else
                        this._footerSelectAllCheckbox = checkbox;
                }
                else {
                    if (isHeader && col.sortExpression) {
                        cell.style.cursor = "hand";
                        cell.setAttribute("data-sort-expression", col.sortExpression);

                        var imgSpan = doc.createElement("span");
                        imgSpan.setAttribute("data-role", "sorter");
                        cell.appendChild(imgSpan);
                        //                        
                        //                         $HGDomElement.createElementFromTemplate(
                        //						{
                        //						    nodeName: "span",
                        //						    properties: { style: { fontFamily: "Webdings"} },
                        //						    cssClasses: ["sortSymbol"]
                        //						}, cell);

                        //                        cell.sortImageSpan = imgSpan;
                        this._setSortHeaderCellState(cell);
                        $addHandlers(cell, this._headerCellEvents);
                    }
                }

                if (col.selectColumn && !this._showCheckBoxColumn) {
                    //如果是选择列，又设置为不显示该列，将cell的width设置为0px
                    cell.style["width"] = "0px";
                }
            }
        }
    },

    _buildBody: function (afterCallBack) {
        this._buildFirstDataRow();

        //删除所有gridRows元素
        this._cleanGridRows();

        if (this._dataSource.length > 0) {
            var dataSource = null;

            if (this._autoPaging) {
                dataSource = [];
                var startCount = this._pageIndex * this._pageSize;
                var endCount = Math.min(startCount + this._pageSize, this._dataSource.length);

                for (var i = startCount; i < endCount; i++) {
                    dataSource.push(this._dataSource[i]);
                }
            }
            else {
                if (this._allowPaging && this._currentPageDataSource.length)
                    dataSource = this._currentPageDataSource;
                else
                    dataSource = this._dataSource;
            }

            var pageSize = (this._autoPaging && this._pageSize <= dataSource.length) ? this._pageSize : dataSource.length;

            var blockSize = Math.min(pageSize, this._renderBatchSize);

            this._buildSomeDataRowsRepeatly(dataSource, blockSize, pageSize, 0, Function.createDelegate(this, afterCallBack));

        }
        else {
            var after = Function.createDelegate(this, afterCallBack);
            after();
            this._raiseAfterDataBindEvent();

        }
    },

    _buildSomeDataRowsRepeatly: function (dataSource, blockSize, totalCount, index, afterCallBack) {
        if (index < totalCount) {
            window.setTimeout(Function.createDelegate(this, function () {
                index = this._buildSomeDataRows(dataSource, Math.min(blockSize, totalCount - index), index);
                this._buildSomeDataRowsRepeatly(dataSource, blockSize, totalCount, index, afterCallBack);
            }), 0);
        }
        else {
            if (afterCallBack)
                afterCallBack();

            this._raiseAfterDataBindEvent();

            this.followupSettleTable();
        }
    },

    _buildSomeDataRows: function (dataSource, count, index) {
        for (var i = 0; i < count; i++) {
            var style = (index % 2) == 0 ? this._rowStyle : this._alternatingRowStyle;
            //            var cssClass = (index % 2) == 0 ? "item" : "alternatingItem";

            //            if (this._cssClass && this._cssClass != "clientGrid")
            //                cssClass = this._cssClass;

            this._buildDataRow(dataSource[index], style);

            index++;
        }

        return index;
    },

    _buildFirstDataRow: function () {
        var dataRow;
        var dataRow2;
        var cell;

        if (this.canShowEditBar() == true) {
            dataRow = this._tBodyLeft.insertRow(-1);
            dataRow.style = this._emptyDataRowStyle;
            //            $HGDomElement.createElementFromTemplate(
            //			{
            //			    nodeName: "tr",
            //			    properties:
            //				{
            //				    style: this._emptyDataRowStyle
            //				}
            //			}, this._tBodyLeft);

            dataRow2 = this._tBodyRight.insertRow(-1);
            dataRow2.style = this._emptyDataRowStyle;
            //            $HGDomElement.createElementFromTemplate(
            //			{
            //			    nodeName: "tr",
            //			    properties:
            //				{
            //				    style: this._emptyDataRowStyle
            //				}
            //			}, this._tBodyRight);


        }

        if (this._dataSource.length == 0 && this.canShowEditBar() == false) {
            dataRow = this._tBodyLeft.insertRow(-1);
            dataRow.style = this._emptyDataRowStyle;
            //            $HGDomElement.createElementFromTemplate(
            //			{
            //			    nodeName: "tr",
            //			    properties:
            //				{
            //				    style: this._emptyDataRowStyle
            //				}
            //			}, this._tBodyLeft);

            dataRow2 = this._tBodyRight.insertRow(-1);
            dataRow2.style = this._emptyDataRowStyle;
            //            $HGDomElement.createElementFromTemplate(
            //			{
            //			    nodeName: "tr",
            //			    properties:
            //				{
            //				    style: this._emptyDataRowStyle
            //				}
            //			}, this._tBodyRight);

            cell = dataRow.insertCell(-1);
            cell.colSpan = this.get_fixeLines() + 1;
            cell.className = "clientgrid-cell";
            if (this._emptyDataHTML) {
                cell.innerHTML = this._emptyDataHTML;
            } else if (this._emptyDataText) {
                cell.appendChild(cell.ownerDocument.createTextNode(this._emptyDataText));
            }

            //            $HGDomElement.createElementFromTemplate(
            //				{
            //				    nodeName: "td",
            //				    properties:
            //					{
            //					    colSpan: this.get_fixeLines() + 1,
            //					    align: "center",
            //					    innerText: this._emptyDataText,
            //					    innerHTML: this._emptyDataHTML
            //					},
            //				    cssClasses: ["item"]
            //				}, dataRow);

            cell = dataRow2.insertCell(-1);
            cell.colSpan = this.get_notFixeLines() == 0 ? 1 : this.get_notFixeLines();
            cell.className = "client-grid-cell";


            //            $HGDomElement.createElementFromTemplate(
            //				{
            //				    nodeName: "td",
            //				    properties:
            //					{
            //					    colSpan: this.get_notFixeLines() == 0 ? 1 : this.get_notFixeLines(),
            //					    align: "center"
            //					},
            //				    cssClasses: ["item"]
            //				}, dataRow2);
        }
        else {

        }
        //this._syncUIBySelectedData();
        dataRow = null;
        dataRow2 = null;
    },

    _buildDataRow: function (rowData, style) {
        var htmlRow = this._addDataRow(style, null);

        this._raiseBeforeDataRowCreateEvent(htmlRow, rowData);
        this._dataRowBindData(htmlRow, rowData);
        this._raiseAfterDataRowCreateEvent(htmlRow, rowData);

    },

    _buildTopPagerContainer: function () {
        var toolBarCss = this.get_toolbarRowCssClass();
        var toolButtonCss = this.get_toolbarButtonCssClass();
        var self = this.get_element();
        var doc = self.ownerDocument;
        this._topContainer = doc.createElement("div");
        this._topContainer.className = "clientgrid-topcontainer";
        self.appendChild(this._topContainer);
        this._topContainer.setAttribute("data-layout-role", "top-container");

        //        this._topPagerRow = this._buildPagerRow(this._topContainer);
        this._captionElement = doc.createElement("div");
        this._captionElement.className = "clientgrid-caption";
        this._topContainer.appendChild(this._captionElement);

        this._captionElement.appendChild(doc.createTextNode(this._caption));

        this._topPagerRow = doc.createElement("div");
        this._topPagerRow.className = "clientgrid-pagerrow text-center";
        this._topContainer.appendChild(this._topPagerRow);

        var controlCell = doc.createElement("ul");
        controlCell.className = "pagination clientgrid-pagination";
        this._topPagerRow.appendChild(controlCell);

        this._toolBarRow = doc.createElement("div");
        this._toolBarRow.className = "clientgrid-toolbar " + toolBarCss;
        this._topContainer.appendChild(this._toolBarRow);

        var span_add_delete = doc.createElement("div");  //添加、删除按钮的容器
        span_add_delete.className = "clientgrid-toolbar-standbuttons btn-group";
        this._toolBarRow.appendChild(span_add_delete);

        var span_custom = doc.createElement("div");
        span_custom.className = "clientgrid-toolbar-custombuttons";
        this._toolBarRow.appendChild(span_custom);

        this._addLink = doc.createElement("a");
        if (toolButtonCss !== '')
            this._addLink.className = toolButtonCss;
        this._addLink.href = "javascript:void(0);";
        span_add_delete.appendChild(this._addLink);
        this._addLink.appendChild(doc.createTextNode("添加"));
        $addHandlers(this._addLink, this._addLinkEvents);

        this._deleteLink = doc.createElement("a");
        if (toolButtonCss !== '')
            this._deleteLink.className = toolButtonCss;
        this._deleteLink.href = "javascript:void(0);";
        span_add_delete.appendChild(this._deleteLink);
        this._deleteLink.appendChild(doc.createTextNode("删除"));
        $addHandlers(this._deleteLink, this._deleteLinkEvents);

        if (this.canShowEditBar()) {
            //抛出事件~~ OnEditBarRowCreating
            Sys.UI.DomElement.setVisible(this._toolBarRow, true);
            this._raiseEditBarRowCreatingEvent(span_custom, this._addLink, this._deleteLink);
        }
        else {
            Sys.UI.DomElement.setVisible(this._toolBarRow, false);
        }

        Sys.UI.DomElement.setVisible(this._topPagerRow, this._allowPaging && (this._pagerSetting.position != $HBRootNS.PagerPosition.bottom));
        this._topPagerControl = this._buildPagerControl(controlCell);

        if (this.canShowEditBar())
            this._raiseEditBarRowCreatedEvent(this._toolBarRow);
    },

    _buildFootPagerContainer: function () {
        var self = this.get_element();
        var doc = self.ownerDocument;
        this._tPager = doc.createElement("div");
        self.appendChild(this._tPager);
        this._tPager.setAttribute("data-layout-role", "foot");

        this._bottomPagerRow = doc.createElement("div");
        this._bottomPagerRow.className = "clientgrid-pagerrow text-center";
        this._tPager.appendChild(this._bottomPagerRow);

        var controlCell = doc.createElement("ul");
        controlCell.className = "pagination clientgrid-pagination";
        this._bottomPagerRow.appendChild(controlCell);

        Sys.UI.DomElement.setVisible(this._bottomPagerRow, this._allowPaging && (this._pagerSetting.position != $HBRootNS.PagerPosition.top));
        this._pagerControl = this._buildPagerControl(controlCell);
    },

    _buildPagerControl: function (elt) {
        var rowCount = this.get_rowCount();
        var pagerControl = $create($HBRootNS.GridPager, { autoPaging: this._autoPaging }, { pageIndexChanged: this._pagerControlPageIndexChangedEvent }, null, elt);
        return pagerControl;
    },

    _buildChildTable: function (pTable) {
        var row = pTable.insertRow(-1);

        var cell = row.insertCell(0);
        cell.className = "clientgrid-splitter-cell table-responsive";
        var table = pTable.ownerDocument.createElement("table");
        cell.appendChild(table);
        return table;
    },

    _buildChildTable_2: function (pTable, properties) {
        var row = pTable.insertRow(-1);
        var cell = row.insertCell(0);
        cell.colSpan = 2;

        var table = pTable.ownerDocument.createElement("table");
        cell.appendChild(table);
        table.style.width = "100%";
        table.style.height = "100%";
        return table;
    },

    _buildMainTableContainer: function () {
        var self = this.get_element();
        var doc = self.ownerDocument;
        var container = doc.createElement("div");
        self.appendChild(container);

        container.setAttribute("data-layout-role", "spliterContainer");

        this._tdFixedLine = doc.createElement("div");
        container.appendChild(this._tdFixedLine);


        //        this._tdFixedLine = $HGDomElement.createElementFromTemplate(
        //			{
        //			    nodeName: "td",
        //			    properties: { style: { verticalAlign: "top"} }
        //			},
        //			row
        //		);

        this._tdNotFixedLine = doc.createElement("div");
        container.appendChild(this._tdNotFixedLine);
        //        this._tdNotFixedLine = $HGDomElement.createElementFromTemplate(
        //			{
        //			    nodeName: "td",
        //			    properties: {}
        //			},
        //			row
        //		);

        //////////////
        //        var tr_emptyLine = $HGDomElement.createElementFromTemplate(
        //			{
        //			    nodeName: "tr",
        //			    properties: { style: { display: "none"} }
        //			},
        //			container
        //		);

        //        var td_emptyLine = $HGDomElement.createElementFromTemplate(
        //			{
        //			    nodeName: "td",
        //			    properties: {}
        //			},
        //			tr_emptyLine
        //		); //what ?
        //////////////

        //移动列
        var div_height = (this._getCurrentRowCount() * this._rowHeightWithFixeLines + this._headRowHeightWithFixeLines + this._scrollBarHeight).toString() + "px"; //17:滚动条的高度
        if (this._heightOfNotFixeLines != 0)
            div_height = this._heightOfNotFixeLines.toString() + "px";

        this._divNotFixedLine = doc.createElement("div");
        this._tdNotFixedLine.appendChild(this._divNotFixedLine);
        this._tdNotFixedLine.style.width = this._widthOfNotFixeLines + "ox";
        this._tdNotFixedLine.style.height = div_height + "px";
        this._tdNotFixedLine.style.overflow = "auto";
        this._tdNotFixedLine.style.overflow.y = "hidden";

        //        this._divNotFixedLine = $HGDomElement.createElementFromTemplate(
        //			{
        //			    nodeName: "div",
        //			    properties:
        //                {
        //                    style:
        //                    {
        //                        width: this._widthOfNotFixeLines.toString() + "px",
        //                        height: div_height,
        //                        overflow: "auto",
        //                        'overflow-y': "hidden"
        //                    }
        //                }
        //			},
        //			this._tdNotFixedLine
        //		);
    },

    _getCurrentRowCount: function () {
        var rowCount = this._dataSource.length;
        if (this._allowPaging && this._autoPaging && rowCount > this._pageSize)
            rowCount = this._pageSize;

        if (this._showEditBar)
            rowCount += 1;

        return rowCount;
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

        if (this._selectCheckboxList.length > 0 && this._selectCheckboxList.length == this._selectedData.length) {
            this._headerSelectAllCheckbox.checked = true;
            this._footerSelectAllCheckbox.checked = true;
        }
        else {
            this._headerSelectAllCheckbox.checked = false;
            this._footerSelectAllCheckbox.checked = false;
        }
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
            if (!col.dataField && !col.selectColumn && col.editTemplate.EditMode != $HBRootNS.ClientGridColumnEditMode.A) {
                e = Error.create(String.format("给定列的对象{0}中dataFields属性不能为空！", $Serializer.serialize(col)));
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
        //this._checkColumns_visible();
    },

    _checkColumns_visible: function () {
        //去除visible==false的列定义，这里处理主要是支持客户端重绑
        var newColumn = [];
        for (var i = 0; i < this._columns.length; i++) {
            if (this._columns[i].visible)
                Array.add(newColumn, this._columns[i]);
        }
        this._columns = newColumn;
    },

    _clearAllRows: function () {
        this._clearRows(this._tHeader);
        this._clearRows(this._tBody);
        this._clearRows(this._tFooter);

        this._clearRows(this._tFooterLeft);
        this._clearRows(this._tFooterRight);
        this._clearRows(this._tHeaderLeft);
        this._clearRows(this._tHeaderRight);
        this._clearRows(this._tBodyLeft);
        this._clearRows(this._tBodyRight);
    },

    _clearDataBindEvents: function () {
        var i;
        if (this._headerSelectAllCheckbox) {
            $clearHandlers(this._headerSelectAllCheckbox);
        }

        if (this._footerSelectAllCheckbox) {
            $clearHandlers(this._footerSelectAllCheckbox);
        }

        if (this._tBody) {
            for (i = 0; i < this._tBody.childNodes.length; i++)
                $clearHandlers(this._tBody.childNodes[i]);
        }

        for (i = 0; i < this._selectCheckboxList.length; i++)
            $clearHandlers(this._selectCheckboxList[i]);
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

    _conversionObject: function () {
        var tmpArry = [];
        for (var i = 0; i < this._dataSource.length; i++) {
            var entity = {};
            for (var dataField in this._dataObjectModel) {
                entity[dataField] = this._dataSource[i][dataField];
            }
            Array.add(tmpArry, entity);
        }
        this._dataSource = tmpArry;
    },

    _cleanGridRows: function () {
        this._gridRows = [];
    },

    _createGridCell: function (col, gridRow, htmlCell) {
        var gcell = new $HBRootNS.GridCell(col, gridRow, htmlCell);
        return gcell;
    },

    _createNewDataObj: function () {
        var newObj = {};
        for (var i = 0; i < this._columns.length; i++) {
            var datafield = this._columns[i].dataField;
            if (datafield != "") {
                newObj[datafield] = this._createDefaultValue(datafield);
            }
            if (this._columns[i].editTemplate.TextFieldOfA)
                newObj[this._columns[i].editTemplate.TextFieldOfA] = "link";
            if (this._columns[i].editTemplate.HrefFieldOfA)
                newObj[this._columns[i].editTemplate.HrefFieldOfA] = "#";
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

    get_gridRowCount: function () {
        return this._gridRows.length;
    },

    //给当前gird添加行（gridRow）
    add_gridRow: function (gridRow) {
        Array.add(this._gridRows, gridRow);
    },

    //添加新行
    addNewRow: function (newRowData) {
        var rowIndex = this.get_gridRowCount();
        var style = (rowIndex % 2) == 0 ? this._rowStyle : this._alternatingRowStyle;
        //        var cssClass = (rowIndex % 2) == 0 ? "item" : "alternatingItem";
        //        if (this._cssClass && this._cssClass != "clientGrid")
        //            cssClass = this._cssClass;

        var newHtmlRow = this._addDataRow(style, null);

        this._raiseBeforeDataRowCreateEvent(newHtmlRow, newRowData);
        this._dataRowBindData_AddNewRow(newHtmlRow, newRowData);
        this._raiseAfterDataRowCreateEvent(newHtmlRow, newRowData);

        //增加div的高度
        this._div_height_manage("add", 1);
    },

    //添加新行并且绑定初始值
    _dataRowBindData_AddNewRow: function (htmlRow, rowData) {

        var gridRow = new $HBRootNS.GridRow(this, rowData, htmlRow);
        var editor = null;

        //行数据
        htmlRow.data = rowData;

        //遍历每个列
        for (var i = 0; i < this._columns.length; i++) {
            var col = this._columns[i];
            if (col.visible) {
                var container = htmlRow.htmlRowLeft;
                if (!col.isFixedLine)
                    container = htmlRow.htmlRowRight;

                container.data = rowData;

                var htmlCell = container.insertCell(-1);
                $HBRootNS.ClientGrid._Helper.mergeStyle(htmlCell, col.itemStyle);
                if (col._itemCssClass)
                    htmlCell.className = col._itemCssClass;
                //                $HGDomElement.createElementFromTemplate(
                //				{
                //				    nodeName: "td",
                //				    properties: { style: col.itemStyle || {} }
                //				}, container);

                //创建GridCell
                var gridCell = this._createGridCell(col, gridRow, htmlCell);

                if (col.selectColumn && this._showCheckBoxColumn) {
                    var checkbox = htmlCell.ownerDocument.createElement("input");
                    checkbox.type = "checkbox";
                    htmlCell.appendChild(checkbox);
                    $addCssClass(htmlCell, "clientgrid-column-check");
                    $addHandlers(checkbox, this._selectCheckboxEvents);

                    checkbox.checked = false;
                    checkbox.data = rowData;
                    Array.add(this._selectCheckboxList, checkbox);
                    //抛出复选框创建事件
                    this._raise_selectCheckboxCreatedEvent(htmlCell, checkbox, rowData);
                }
                else {
                    if (col.editTemplate)
                        editor = this._createEditor(col.editTemplate.EditMode, gridCell, col, rowData);
                    else
                        editor = this._createEditor($HBRootNS.ClientGridColumnEditMode.None, gridCell, col, rowData)

                    editor.createEditor(htmlCell);

                    if (this._runDataToEditor)
                        editor.dataToEditor();
                }

                if (col.selectColumn && !this._showCheckBoxColumn) {
                    //如果是选择列，又设置为不显示该列，将htmlCell的width设置为0px
                    htmlCell.style["width"] = "0px";
                }

                //抛出单元格数据绑定事件
                this._raiseCellDataBoundEvent(container, htmlCell, col, rowData);
            }
        }

        //将新行数据结构放入数据源中
        Array.add(this._dataSource, rowData);
    },

    //div高度的+，-操作
    _div_height_manage: function (type, rowCount) {
        if (this._notFixeLines > 0) {
            //var ch = this._divNotFixedLine.style["height"];
            //if (ch.indexOf("px") > 0)
            //    ch = ch.substring(0, ch.indexOf("px"));

            //var ah = parseInt(ch);
            //if (type == "add") {
            //    ah = ah + rowCount * this._rowHeightWithFixeLines;
            //}
            //else if (type == "sub") {
            //    ah = ah - rowCount * this._rowHeightWithFixeLines;
            //}

            //this._divNotFixedLine.style["height"] = ah.toString() + "px";

            //可以直接取左边表格的高度。。呃~
            this._divNotFixedLine.style["height"] = (this._mainTable_left.offsetHeight + this._scrollBarHeight).toString() + "px";
        }
    },

    //==========================数据绑定==========================
    _dataRowBindData: function (htmlRow, rowData) {
        var gridRow = new $HBRootNS.GridRow(this, rowData, htmlRow.htmlRowLeft);
        var editor = null;

        //行数据
        htmlRow.data = rowData;

        var checked = Array.indexOf(this._selectedData, rowData, 0, this._compareDataEqualDelegate) >= 0;
        for (var i = 0; i < this._columns.length; i++) {
            var col = this._columns[i];
            if (col.visible) {
                var container = htmlRow.htmlRowLeft;
                if (!col.isFixedLine)
                    container = htmlRow.htmlRowRight;

                container.data = rowData;

                var htmlCell = container.insertCell(-1);
                $HBRootNS.ClientGrid._Helper.mergeStyle(htmlCell, col.itemStyle);
                if (col._itemCssClass)
                    htmlCell.className = col._itemCssClass;

                //                $HGDomElement.createElementFromTemplate(
                //				{
                //				    nodeName: "td",
                //				    properties: { style: col.itemStyle || {} }
                //				}, container);

                //创建GridCell
                var gridCell = this._createGridCell(col, gridRow, htmlCell);

                if (col.selectColumn && this._showCheckBoxColumn) {
                    var checkbox = htmlCell.ownerDocument.createElement("input");
                    checkbox.type = "checkbox";
                    htmlCell.appendChild(checkbox);
                    $addCssClass(htmlCell, "clientgrid-column-check");
                    $addHandlers(checkbox, this._selectCheckboxEvents);

                    //                    var checkbox = $HGDomElement.createElementFromTemplate(
                    //					{
                    //					    nodeName: "input",
                    //					    properties: { type: "checkbox" },
                    //					    events: this._selectCheckboxEvents
                    //					}, htmlCell);
                    checkbox.checked = checked;
                    checkbox.data = rowData;
                    Array.add(this._selectCheckboxList, checkbox);

                    //抛出复选框创建事件
                    this._raise_selectCheckboxCreatedEvent(htmlCell, checkbox, rowData);
                }
                else {
                    if (this.get_readOnly() && col.editTemplate
                        && (col.editTemplate.EditMode == $HBRootNS.ClientGridColumnEditMode.OuUserInput || col.editTemplate.EditMode == $HBRootNS.ClientGridColumnEditMode.Material)) {
                        editor = this._createEditor(col.editTemplate.EditMode, gridCell, col, rowData);
                    }
                    else if (this.get_readOnly()) {
                        editor = this._createEditor($HBRootNS.ClientGridColumnEditMode.None, gridCell, col, rowData);
                    }
                    else {
                        if (col.editTemplate)
                            editor = this._createEditor(col.editTemplate.EditMode, gridCell, col, rowData);
                        else
                            editor = this._createEditor($HBRootNS.ClientGridColumnEditMode.None, gridCell, col, rowData);
                    }

                    editor.createEditor(htmlCell);

                    if (this._runDataToEditor)
                        editor.dataToEditor();
                }

                if (col.selectColumn && !this._showCheckBoxColumn) {
                    //如果是选择列，又设置为不显示该列，将htmlCell的width设置为0px
                    htmlCell.style["width"] = "0px";
                }

                //抛出单元格数据绑定事件
                this._raiseCellDataBoundEvent(container, htmlCell, col, rowData);
            }
        }

        if (checked && !this._hasSelectColumn)
            this._setSelectedRowState(container);
    },

    //创建一个新列的template Editor
    _createEditor: function (editMode, gridCell, column, rowData) {
        var editor = null;

        //抛出创建控件发生前的事件 可以修改 editMode
        var preCellCreatEditorEventResult = this._raisePreCellCreatEditorEvent(editMode, rowData, column, true);
        editMode = preCellCreatEditorEventResult.editMode;
        var allowCopyFromElement = preCellCreatEditorEventResult.allowCopyFromElement;

        switch (editMode) {
            case $HBRootNS.ClientGridColumnEditMode.None:
                editor = new $HBRootNS.GridColumnStaticTextEditor(allowCopyFromElement, gridCell, column, rowData, this);
                break;
            case $HBRootNS.ClientGridColumnEditMode.TextBox:
                editor = new $HBRootNS.GridColumnTextBoxEditor(allowCopyFromElement, gridCell, column, rowData, this);
                break;
            case $HBRootNS.ClientGridColumnEditMode.DropdownList:
                editor = new $HBRootNS.GridColumnDropDownListEditor(allowCopyFromElement, gridCell, column, rowData, this);
                break;
            case $HBRootNS.ClientGridColumnEditMode.CheckBox:
                editor = new $HBRootNS.GridColumnCheckBoxEditor(allowCopyFromElement, gridCell, column, rowData, this);
                break;
            case $HBRootNS.ClientGridColumnEditMode.DateInput:
                editor = new $HBRootNS.GridColumnDateInputEditor(allowCopyFromElement, gridCell, column, rowData, this);
                break;
            case $HBRootNS.ClientGridColumnEditMode.DateTimeInput:
                editor = new $HBRootNS.GridColumnDateTimeInputEditor(allowCopyFromElement, gridCell, column, rowData, this);
                break;
            case $HBRootNS.ClientGridColumnEditMode.OuUserInput:
                editor = new $HBRootNS.GridColumnOuUserInputEditor(allowCopyFromElement, gridCell, column, rowData, this);
                break;
            case $HBRootNS.ClientGridColumnEditMode.A:
                editor = new $HBRootNS.GridColumnAEditor(allowCopyFromElement, gridCell, column, rowData, this);
                break;
            case $HBRootNS.ClientGridColumnEditMode.Material:
                editor = new $HBRootNS.GridColumnMaterialEditor(allowCopyFromElement, gridCell, column, rowData, this);
                break;
            default:
                throw Error.create("EditMode: " + editMode + "未实现");
                break;
        }
        editor.RandomKey = Math.random(); //给editor一个随机值
        return editor;
    },

    //是否显示添加链接
    canShowEditBar: function () {
        return this.get_showEditBar() && this.get_readOnly() == false;
    },

    //获取某列值的总和
    get_sum: function (dataField) {

        var flag = true, i;
        var dataType;
        var cols = this.get_columns();

        for (i = 0; i < cols.length; i++) {
            if (dataField == cols[i].dataField) {
                if (cols[i].dataType != $HBRootNS.DataFieldType.Integer &&
                    cols[i].dataType != $HBRootNS.DataFieldType.Decimal) {
                    flag = false;
                }
                dataType = cols[i].dataType;
            }
        }

        if (flag) {
            var sum = 0;
            var rows = this.get_gridRows();
            for (i = 0; i < rows.length; i++) {

                if (dataType == $HBRootNS.DataFieldType.Integer)
                    sum = parseInt(sum) + parseInt(rows[i].get_data()[dataField]);
                else if (dataType == $HBRootNS.DataFieldType.Decimal)
                    sum = parseFloat(sum) + parseFloat(rows[i].get_data()[dataField]);
            }
            return sum;
        }
        else
            return 0;
    },

    //获取某列值的平均值
    get_avg: function (dataField) {
        var sum = this.get_sum(dataField);
        return sum / this.get_gridRowCount();
    },


    /*************************一堆 事件处理 start*************************{*************************/
    _onAddLinkClick: function () {
        var newRowData = this._createNewDataObj(); //构建一个新的数据对象
        var result = this._raisePreRowAddEvent(newRowData); //抛出添加行前事件
        if (result.cancel === false) {
            this.addNewRow(result.rowData);
        }
    },

    _deleteSelectedData: function () {
        var allData = this.get_dataSource();
        var selectedData = this.get_selectedData();

        for (var i = 0; i < selectedData.length; i++) {
            Array.remove(this.get_dataSource(), selectedData[i]);
        }

        //抛出删除行事件
        this._raiseRowDeleteEvent(selectedData, allData);

        Array.clear(selectedData);
        this.set_dataSource(this.get_dataSource());

        //减少div的高度 if ((!this._allowPaging && !this._autoPaging) || (this._allowPaging && this._autoPaging && allData.length < this._pageSize))
        this._div_height_manage("sub", selectedData.length);
    },

    _onDeleteLinkClick: function () {
        var hasDeleteData = this.get_selectedData().length > 0;

        if (hasDeleteData) {
            $HGClientMsg.confirm("确认要删除已选择的记录吗？", null, "删除", null, null, Function.createDelegate(this, function () {
                this._deleteSelectedData();
            }));
        }
        else {
            $HGClientMsg.info("请先选择要删除的数据", null, "删除");
        }
    },

    _ensureDataBind: function () {
        if (this._requireDataBind)
            this._dataBind();
    },

    _getColumnCount: function () {
        return this._columns.length + (this._showSelectColumn ? 1 : 0);
    },

    _onDataRowClick: function (e) {
        if (!this._hasSelectColumn) {
            var row = e.handlingElement;
            if (row) {
                this._setSelectedRowState(row);
                this._selectedData = [row.data];
            }
        }
    },

    _setSelectedRowState: function (row) {
        var acss = this.get_selectedItemCssClass();
        if (this._selectedRow) {
            if (acss) {
                Sys.UI.DomElement.removeCssClass(this._selectedRow, acss);
            }
        }

        if (acss) {
            Sys.UI.DomElement.addCssClass(row, acss);
        }

        this._selectedRow = row;
    },

    _onHeaderCellClick: function (e) {
        var oldSortCell = null;
        var tempCell, i;
        if (this._sortExpression) {
            var atLeftFlag = false;
            var cells = this._headerRowLeft.cells;
            for (i = 0; i < cells.length; i++) {
                tempCell = cells[i];
                if (tempCell.getAttribute("data-sort-expression") == this._sortExpression) {
                    oldSortCell = tempCell;
                    atLeftFlag = true;
                    break;
                }
            }
            if (!atLeftFlag) {
                cells = this._headerRowRight.cells;
                for (i = 0; i < cells.length; i++) {
                    tempCell = cells[i];
                    if (tempCell.getAttribute("data-sort-expression") == this._sortExpression) {
                        oldSortCell = tempCell;
                        break;
                    }
                }
            }
        }
        var cell = e.target;
        var exp = cell.getAttribute("data-sort-expression");
        if (exp != this._sortExpression) {
            this._sortExpression = exp;
            this._sortDirection = $HBRootNS.SortDirection.Asc;
        }
        else {
            if (this._sortDirection == $HBRootNS.SortDirection.Asc)
                this._sortDirection = $HBRootNS.SortDirection.Desc;
            else
                this._sortDirection = $HBRootNS.SortDirection.Asc;
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
        if (sortDirection == $HBRootNS.SortDirection.Asc)
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
        //抛出（全选）复选框单击事件
        this._raise_allSelectCheckboxClickedEvent(e.target);
    },

    _onSelectCheckboxClick: function (e) {
        this._checkboxSelectChanged(e.target);

        //抛出复选框单击事件
        this._raise_selectCheckboxClickEvent(e.target);
    },

    _pagerControlDataBind: function (pagerControl) {
        var rowCount = this.get_rowCount();
        $setProperties(pagerControl, { rowCount: rowCount, pagerSize: this._pageSize, pagerSetting: this._pagerSetting, pageIndex: this._pageIndex });
        pagerControl.buildPager();
    },

    //抛出开始绑定数据事件
    _raiseBeforeDataBindEvent: function () {
        var handler = this.get_events().getHandler(this._beforeDataBindEventKey);
        var e = new Sys.EventArgs;
        e.dataSource = this._dataSource;
        e.columns = this._columns;

        if (handler) {
            handler(this, e);
        }
        return e;
    },

    //抛出结束绑定数据事件 afterDataBind
    _raiseAfterDataBindEvent: function () {
        var handler = this.get_events().getHandler(this._afterDataBindEventKey);
        var e = new Sys.EventArgs;
        e.dataSource = this._dataSource;
        if (handler) {
            handler(this, e);
        }
        return e;
    },

    //客户端创建控件发生前的事件 可以修改 editMode
    _raisePreCellCreatEditorEvent: function (editMode, rowData, column, allowCopyFromElement) {
        var handler = this.get_events().getHandler(this._preCellCreatEditorEventKey);

        var e = new Sys.EventArgs;
        e.editMode = editMode;
        e.rowData = rowData;
        e.column = column;
        e.allowCopyFromElement = allowCopyFromElement;

        if (handler) {
            handler(this, e);
        }
        return e;
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

        e.firstItemSelected = true; //默认第一项非空项选中
        e.autoFormat = true;

        if (handler) {
            handler(this, e);
        }
        return e;
    },

    //构建Editor后抛出的事件  Editor （new）
    _raiseCellCreatedEditorEvent: function (editor, col, rowData) {
        var handler = this.get_events().getHandler(this._cellCreatedEditorEventKey);
        var e = new Sys.EventArgs;
        e.column = col;
        e.rowData = rowData;
        e.editor = editor;
        e.runDataToEditor = true;
        if (handler) {
            handler(this, e);
        }
        return e;
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

    //抛出初始化Editor处理事件 (最后的处理)
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

            e.gridRow = editor.get_gridCell().get_gridRow();

            e.valueChanged = valueChanged;
            handler(this, e);
        }
    },

    //添加行前事件
    _raisePreRowAddEvent: function (rowData) {
        var handler = this.get_events().getHandler(this._preRowAddEventKey);
        var e = new Sys.EventArgs;
        e.rowData = rowData;
        e.cancel = false;
        if (handler) {
            handler(this, e);
        }
        return e;
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

    //(全选)复选框单击事件 
    _raise_allSelectCheckboxClickedEvent: function (checkbox) {
        var handler = this.get_events().getHandler(this._allSelectCheckboxClickedEventKey);

        var e = new Sys.EventArgs;
        e.checkbox = checkbox;
        //e.continueRun = true;

        if (handler) {
            handler(this, e);
        }
        //return e.continueRun;
    },

    //复选框单击事件 
    _raise_selectCheckboxClickEvent: function (checkbox) {
        var handler = this.get_events().getHandler(this._selectCheckboxClickEventKey);

        var e = new Sys.EventArgs;
        e.checkbox = checkbox;

        if (handler) {
            handler(this, e);
        }
    },

    //复选框(包括全选)创建事件
    _raise_selectCheckboxCreatedEvent: function (container, checkbox, rowData) {
        var handler = this.get_events().getHandler(this._selectCheckboxCreatedEventKey);

        var e = new Sys.EventArgs;
        e.container = container;
        e.checkbox = checkbox;
        e.rowData = rowData;

        if (handler) {
            handler(this, e);
        }
    },

    //复选框(包括全选)创建事件
    _raise_beforeSaveClientStateEvent: function (dataSource) {
        var handler = this.get_events().getHandler(this._beforeSaveClientStateEventKey);

        var e = new Sys.EventArgs;
        e.dataSource = dataSource;

        if (handler) {
            handler(this, e);
        }

        return e.dataSource;
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

    //抛出Foot行创建事件
    _raiseFootCellCreatingEvent: function (dataField, cell, col) {
        var handler = this.get_events().getHandler(this._footCellCreatingEventKey);

        var e = new Sys.EventArgs;
        e.cell = cell;
        e.column = col;
        e.dataField = dataField;

        if (handler) {
            handler(this, e);
        }
        return e.cell;
    },

    //抛出editBarRow行创建事件
    _raiseEditBarRowCreatingEvent: function (container, addLink, deleteLink) {
        var handler = this.get_events().getHandler(this._editBarRowCreatingEventKey);

        var e = new Sys.EventArgs;
        e.container = container;
        e.addLink = addLink;
        e.deleteLink = deleteLink;

        if (handler) {
            handler(this, e);
        }
    },

    _raiseEditBarRowCreatedEvent: function (container) {
        var handler = this.get_events().getHandler(this._editBarRowCreatedEventKey);

        if (handler) {
            var e = new Sys.EventArgs();
            e.container = container;
            handler(this, e);
        }
    },

    //抛出HeaderRow创建事件
    _raisePreHeaderRowCreateEvent: function (containerLeft, containerRight) {
        var handler = this.get_events().getHandler(this._preHeaderRowCreateEventKey);

        var e = new Sys.EventArgs;

        e.container = containerLeft;
        e.containerLeft = containerLeft;
        e.containerRight = containerRight;

        if (handler) {
            handler(this, e);
        }
    },

    //抛出数据行创建前事件
    _raiseBeforeDataRowCreateEvent: function (htmlRow, rowData) {
        var handler = this.get_events().getHandler(this._beforeDataRowCreateEventKey);

        var e = new Sys.EventArgs;

        e.htmlRow = htmlRow;
        e.rowData = rowData;

        if (handler) {
            handler(this, e);
        }
    },

    //抛出数据行创建后事件
    _raiseAfterDataRowCreateEvent: function (htmlRow, rowData) {
        var handler = this.get_events().getHandler(this._afterDataRowCreateEventKey);

        var e = new Sys.EventArgs;

        e.htmlRow = htmlRow;
        e.rowData = rowData;

        if (handler) {
            handler(this, e);
        }
    },

    //抛出单元格数据绑定事件
    _raiseCellDataBoundEvent: function (htmlRow, htmlCell, col, data) {
        var handler = this.get_events().getHandler(this._cellDataBoundEventKey), e;
        if (handler) {
            e = new Sys.EventArgs;
            e.row = htmlRow;
            e.cell = htmlCell;
            e.column = col;
            e.data = data;
            handler(this, e);
        }
        if (col.cellDataBound && typeof (col.cellDataBound) === "function") {
            e = new Sys.EventArgs;
            e.row = htmlRow;
            e.cell = htmlCell;
            e.column = col;
            e.data = data;
            col.cellDataBound(this, e);
        }

        e = null;
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
        //        this._gridTable.cellPadding = 0;
        //        this._gridTable.cellSpacing = 0;
        $HGDomElement.setStyle(this.get_element(), this._style);
        //        $addCssClass(this._gridTable, this._cssClass);
    },

    _setSortHeaderCellState: function (cell) {
        var sortExp = cell.getAttribute("data-sort-expression");
        if (sortExp) {
            $addCssClass(cell, "sorting");
            if (sortExp == this._sortExpression) {
                if (this._sortDirection == $HBRootNS.SortDirection.Asc) {
                    $addCssClass(cell, "sorting-asc");
                    $removeCssClass(cell, "sorting-desc");

                } else {
                    $addCssClass(cell, "sorting-desc");
                    $removeCssClass(cell, "sorting-asc");
                }
            } else {
                $removeCssClass(cell, "sorting-asc");
                $removeCssClass(cell, "sorting-desc");
            }
        } else {
            $addCssClass(cell, "sorting-disabled");
        }
        //        if (cell.sortExpression == this._sortExpression)
        //            cell.sortImageSpan.innerText = this._sortDirection == $HBRootNS.SortDirection.Asc ? "5" : "6";
        //        else
        //            cell.sortImageSpan.innerText = "";
    },

    /*************************一堆 事件处理end*************************}*************************/

    /**********************************事件定义*************{******************************/
    _beforeDataBindEventKey: "beforeDataBind",
    add_beforeDataBind: function (value) {
        this.get_events().addHandler(this._beforeDataBindEventKey, value);
    },
    remove_beforeDataBind: function (value) {
        this.get_events().removeHandler(this._beforeDataBindEventKey, value);
    },

    _afterDataBindEventKey: "afterDataBind",
    add_afterDataBind: function (value) {
        this.get_events().addHandler(this._afterDataBindEventKey, value);
    },
    remove_afterDataBind: function (value) {
        this.get_events().removeHandler(this._afterDataBindEventKey, value);
    },

    _headCellCreatingEventKey: "headCellCreating",
    add_headCellCreating: function (value) {
        this.get_events().addHandler(this._headCellCreatingEventKey, value);
    },
    remove_headCellCreating: function (value) {
        this.get_events().removeHandler(this._headCellCreatingEventKey, value);
    },

    _footCellCreatingEventKey: "footCellCreating",
    add_footCellCreating: function (value) {
        this.get_events().addHandler(this._footCellCreatingEventKey, value);
    },
    remove_footCellCreating: function (value) {
        this.get_events().removeHandler(this._footCellCreatingEventKey, value);
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

    //客户端创建控件发生前的事件
    _preCellCreatEditorEventKey: "preCellCreatEditor",
    add_preCellCreatEditor: function (value) {
        this.get_events().addHandler(this._preCellCreatEditorEventKey, value);
    },
    remove_preCellCreatEditor: function (value) {
        this.get_events().removeHandler(this._preCellCreatEditorEventKey, value);
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

    //添加行前事件
    _preRowAddEventKey: "preRowAdd",
    add_preRowAdd: function (value) {
        this.get_events().addHandler(this._preRowAddEventKey, value);
    },
    remove_preRowAdd: function (value) {
        this.get_events().removeHandler(this._preRowAddEventKey, value);
    },

    //删除行事件
    _rowDeleteEventKey: "rowDelete",
    add_rowDelete: function (value) {
        this.get_events().addHandler(this._rowDeleteEventKey, value);
    },
    remove_rowDelete: function (value) {
        this.get_events().removeHandler(this._rowDeleteEventKey, value);
    },

    //标题头(全选)复选框单击事件
    _allSelectCheckboxClickedEventKey: "allSelectCheckboxClicked",
    add_allSelectCheckboxClicked: function (value) {
        this.get_events().addHandler(this._allSelectCheckboxClickedEventKey, value);
    },
    remove_allSelectCheckboxClicked: function (value) {
        this.get_events().removeHandler(this._allSelectCheckboxClickedEventKey, value);
    },

    //复选框单击事件
    _selectCheckboxClickEventKey: "selectCheckboxClick",
    add_selectCheckboxClick: function (value) {
        this.get_events().addHandler(this._selectCheckboxClickEventKey, value);
    },
    remove_selectCheckboxClick: function (value) {
        this.get_events().removeHandler(this._selectCheckboxClickEventKey, value);
    },

    //复选框(包括全选)创建事件
    _selectCheckboxCreatedEventKey: "selectCheckboxCreated",
    add_selectCheckboxCreated: function (value) {
        this.get_events().addHandler(this._selectCheckboxCreatedEventKey, value);
    },
    remove_selectCheckboxCreated: function (value) {
        this.get_events().removeHandler(this._selectCheckboxCreatedEventKey, value);
    },

    //beforeSaveClientState 事件
    _beforeSaveClientStateEventKey: "beforeSaveClientState",
    add_beforeSaveClientState: function (value) {
        this.get_events().addHandler(this._beforeSaveClientStateEventKey, value);
    },
    remove_beforeSaveClientState: function (value) {
        this.get_events().removeHandler(this._beforeSaveClientStateEventKey, value);
    },

    //editBarRowCreating 事件
    _editBarRowCreatingEventKey: "editBarRowCreating",


    add_editBarRowCreating: function (value) {
        this.get_events().addHandler(this._editBarRowCreatingEventKey, value);
    },
    remove_editBarRowCreating: function (value) {
        this.get_events().removeHandler(this._editBarRowCreatingEventKey, value);
    },

    _editBarRowCreatedEventKey: "editBarRowCreated",

    add_editBarRowCreated: function (value) {
        this.get_events().addHandler(this._editBarRowCreatedEventKey, value);
    },

    remove_editBarRowCreated: function (value) {
        this.get_events().removeHandler(this._editBarRowCreatedEventKey, value);
    },

    //preHeaderRowCreate 事件
    _preHeaderRowCreateEventKey: "preHeaderRowCreate",
    add_preHeaderRowCreate: function (value) {
        this.get_events().addHandler(this._preHeaderRowCreateEventKey, value);
    },
    remove_preHeaderRowCreate: function (value) {
        this.get_events().removeHandler(this._preHeaderRowCreateEventKey, value);
    },

    //beforeDataRowCreate 事件
    _beforeDataRowCreateEventKey: "beforeDataRowCreate",
    add_beforeDataRowCreate: function (value) {
        this.get_events().addHandler(this._beforeDataRowCreateEventKey, value);
    },
    remove_beforeDataRowCreate: function (value) {
        this.get_events().removeHandler(this._beforeDataRowCreateEventKey, value);
    },

    //afterDataRowCreate 事件
    _afterDataRowCreateEventKey: "afterDataRowCreate",
    add_afterDataRowCreate: function (value) {
        this.get_events().addHandler(this._afterDataRowCreateEventKey, value);
    },
    remove_afterDataRowCreate: function (value) {
        this.get_events().removeHandler(this._afterDataRowCreateEventKey, value);
    },
    /**************************************}******************************/

    /********************************** Get Set ************* {****************************/
    get_deleteLink: function () {
        return this._deleteLink;
    },
    get_addLink: function () {
        return this._addLink;
    },

    get_columns: function () {
        return this._columns;
    },
    set_columns: function (value) {
        var e = Function._validateParams(arguments, [{ name: "value", mayBeNull: false, type: Array}]);
        if (e) throw e;

        this._columns = value;
    },

    get_gridRows: function () {
        return this._gridRows;
    },
    set_gridRows: function (value) {
        this._gridRows = value;
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

        this._dataBind();

    },

    set_dataSourceNoBind: function (value) {
        var e = Function._validateParams(arguments, [{ name: "value", mayBeNull: false, type: Array}]);

        if (e)
            throw e;

        this._dataSource = value;
        if (this._dataSource.length == 0)
            this._selectedData = [];

        this._requireDataBind = true;

        if (this._autoPaging)
            this._selectedData = [];
    },

    get_deluxeDateTimePickerID: function () {
        return this._deluxeDateTimePickerID;
    },

    set_deluxeDateTimePickerID: function (value) {
        this._deluxeDateTimePickerID = value;
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

    //    get_alternatingRowStyle: function () {
    //        return this._alternatingRowStyle;
    //    },
    //    set_alternatingRowStyle: function (value) {
    //        this._alternatingRowStyle = value;
    //    },

    get_autoPaging: function () {
        return this._autoPaging;
    },
    set_autoPaging: function (value) {
        this._autoPaging = value;
    },

    get_currentPageDataSource: function () {
        return this._currentPageDataSource;
    },
    set_currentPageDataSource: function (value) {
        this._currentPageDataSource = value;
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

    get_renderBatchSize: function () {
        return this._renderBatchSize;
    },
    set_renderBatchSize: function (value) {
        this._renderBatchSize = value;
    },

    get_autoBindOnLoad: function () {
        return this._autoBindOnLoad;
    },
    set_autoBindOnLoad: function (value) {
        this._autoBindOnLoad = value;
    },

    get_readOnly: function () {
        return this._readOnly;
    },
    set_readOnly: function (value) {
        this._readOnly = value;
        if (value === true) {
            Sys.UI.DomElement.setVisible(this._toolBarRow, false);
        }
        else if (this.canShowEditBar()) {
            Sys.UI.DomElement.setVisible(this._toolBarRow, true);
        }
    },

    get_showEditBar: function () {
        return this._showEditBar;
    },
    set_showEditBar: function (value) {
        this._showEditBar = value;
    },

    get_selectedByDefault: function () {
        return this._selectedByDefault;
    },
    set_selectedByDefault: function (value) {
        this._selectedByDefault = value;
    },

    get_showCheckBoxColumn: function () {
        return this._showCheckBoxColumn;
    },
    set_showCheckBoxColumn: function (value) {
        this._showCheckBoxColumn = value;
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
        if (this.get_isInitialized()) {
            $HBRootNS.DomElement.innerText(this._captionElement, String(value));
        }
    },

    set_captionElementInnerHTML: function (value) {
        this._captionElement.innerHTML = value;
    },

    get_captionElement: function () {
        return this._captionElement;
    },

    get_toolBarRowElement: function () {
        return this._toolBarRow;
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

    get_cssClass: function () {
        return this._cssClass;
    },
    set_cssClass: function (value) {
        this._cssClass = value;
    },

    get_selectedItemCssClass: function () {
        return this._selectedItemCssClass;
    },
    set_selectedItemCssClass: function (value) {
        this._selectedItemCssClass = value;
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

    get_emptyDataHTML: function () {
        return this._emptyDataHTML;
    },
    set_emptyDataHTML: function (value) {
        this._emptyDataHTML = value;
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

    get_rowStyle: function () {
        return this._rowStyle;
    },
    set_rowStyle: function (value) {
        this._rowStyle = value;
    },

    get_toolbarRowCssClass: function () {
        return this._toolbarRowCssClass;
    },

    set_toolbarRowCssClass: function (value) {
        this._toolbarRowCssClass = value;
    },

    get_toolbarButtonCssClass: function () {
        return this._toolbarButtonCssClass;
    },

    set_toolbarButtonCssClass: function (value) {
        this._toolbarButtonCssClass = value;
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

    get_dataObjectModel: function () {
        return this._dataObjectModel;
    },
    set_dataObjectModel: function (value) {
        this._dataObjectModel = value;
    },

    get_saveClientStateNeedReturnState: function () {
        return this._saveClientStateNeedReturnState;
    },
    set_saveClientStateNeedReturnState: function (value) {
        this._saveClientStateNeedReturnState = value;
    },

    get_selectCheckboxList: function () {
        return this._selectCheckboxList;
    },


    get_fixeLines: function () {
        return this._fixeLines;
    },
    set_fixeLines: function (value) {
        this._fixeLines = value;
    },

    get_notFixeLines: function () {
        return this._notFixeLines;
    },
    set_notFixeLines: function (value) {
        this._notFixeLines = value;
    },

    get_rowHeightWithFixeLines: function () {
        return this._rowHeightWithFixeLines;
    },
    set_rowHeightWithFixeLines: function (value) {
        this._rowHeightWithFixeLines = value;
    },

    get_widthOfNotFixeLines: function () {
        return this._widthOfNotFixeLines;
    },
    set_widthOfNotFixeLines: function (value) {
        this._widthOfNotFixeLines = value;
    },

    get_heightOfNotFixeLines: function () {
        return this._heightOfNotFixeLines;
    },
    set_heightOfNotFixeLines: function (value) {
        this._heightOfNotFixeLines = value;
    },

    get_totleWidthOfFixeLines: function () {
        return this._totleWidthOfFixeLines;
    },
    set_totleWidthOfFixeLines: function (value) {
        this._totleWidthOfFixeLines = value;
    },

    get_totleWidthOfNotFixeLines: function () {
        return this._totleWidthOfNotFixeLines;
    },
    set_totleWidthOfNotFixeLines: function (value) {
        this._totleWidthOfNotFixeLines = value;
    },

    get_headRowHeightWithFixeLines: function () {
        return this._headRowHeightWithFixeLines;
    },
    set_headRowHeightWithFixeLines: function (value) {
        this._headRowHeightWithFixeLines = value;
    },

    get_autoWidthOfNotFixeLines: function () {
        return this._autoWidthOfNotFixeLines;
    },
    set_autoWidthOfNotFixeLines: function (value) {
        this._autoWidthOfNotFixeLines = value;
    },

    get_showFooterLeft: function () {
        return this._tFooterLeft;
    },

    set_showFooterLeft: function (value) {
        this._showFooter = value;
        if (this.get_isInitialized())
            Sys.UI.DomElement.setVisible(this._tFooterLeft, this._showFooter);
    },

    get_footerRowLeft: function () {
        return this._footerRowLeft;
    },

    get_footerRowRight: function () {
        return this._footerRowRight;
    },

    /**********************************get set}****************************/

    doNothing: function () {
    },
    
    rebind: function () {
        if (this._dataSource)
            this.set_dataSource(this._dataSource);
    },

    performDelete: function () {
        this._onDeleteLinkClick();
    },
}

$HBRootNS.ClientGrid.registerClass($HBRootNSName + ".ClientGrid", $HGRootNS.ControlBase);
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
    OuUserInput: 6,
    A: 7,
    Material: 8
}

$HBRootNS.ClientGridColumnEditMode.registerEnum($HBRootNSName + ".ClientGridColumnEditMode");
/*****************************************}*********************************************/

/**********************************GridColumnEditorBase*****************{********************************/
$HBRootNS.GridColumnEditorBase = function (allowCopyFromElement, gridCell, column, rowData, clientGrid) {
    $HBRootNS.GridColumnEditorBase.initializeBase(this);
    this._column = column;
    this._rowData = rowData;
    this._editorElement = null;
    this._clientGrid = clientGrid;
    this._gridCell = gridCell;
    this._allowCopyFromElement = allowCopyFromElement;

    //给当前的_gridCell附加上editor
    this._gridCell.set_editor(this);

    this._cellCreatingEditorResult = null;

    this.textBoxChange$delegate = {
        change: Function.createDelegate(this, this._onDataChangeWithNoValidation)
    };

}

$HBRootNS.GridColumnEditorBase.prototype = {
    get_htmlCell: function () {
        return this._gridCell.get_htmlCell();
    },

    get_htmlRow: function () {
        return this._gridCell.get_htmlRow();
    },

    get_gridCell: function () {
        return this._gridCell;
    },

    get_gridRow: function () {
        return this._gridCell.get_gridRow();
    },

    get_column: function () {
        return this._column;
    },

    get_rowData: function () {
        return this._rowData;
    },

    get_otherGridCellByDataField: function (dataField) {
        return this.get_gridRow().get_gridCellByDataField(dataField);
    },

    get_otherEditorByDataField: function (dataField) {
        return this.get_gridRow().get_editorByDataField(dataField);
    },

    get_otherEditorElementByDataField: function (dataField) {
        return this.get_gridRow().get_editorElementByDataField(dataField);
    },

    get_allowCopyFromElement: function () {
        return this._allowCopyFromElement;
    },
    set_allowCopyFromElement: function (value) {
        this._allowCopyFromElement = value;
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

    get_cellCreatingEditorResult: function () {
        return _cellCreatingEditorResult;
    },
    set_cellCreatingEditorResult: function (value) {
        this._cellCreatingEditorResult = value;
    },

    _getDropdownListText: function (value) {
        var rVal = value;
        if (this.get_clientGrid().get_readOnly() == true) {

            if (this.get_column().editTemplate.EditMode == $HBRootNS.ClientGridColumnEditMode.DropdownList) {
                var controlID = this._column.editTemplate.TemplateControlClientID;
                if (controlID == null) return rVal;
                var options = $get(controlID).options;
                if (options) {
                    for (var i = 0; i < options.length; i++) {
                        if (options[i].value == rVal) {
                            rVal = options[i].text;
                            break;
                        }
                    }
                }
            }
        }
        return String(rVal);
    },

    createEditor: function (container) {
        var newNode = null;

        if (this._column.editTemplate) {
            if (this._column.editTemplate.TemplateControlClientID != null &&
                this._column.editTemplate.TemplateControlClientID != "" &&
                this.get_clientGrid().get_readOnly() == false &&
                this.get_allowCopyFromElement()) {

                //注意这里cloneNode 会出现ID重复的问题，通过下面方法解决问题。
                if (!this._column.editTemplate.templateControl)
                    this._column.editTemplate.templateControl = $get(this._column.editTemplate.TemplateControlClientID);

                var templateControl = this._column.editTemplate.templateControl;
                //先判断当前模版节点类型
                if (templateControl.nodeName.toLowerCase() != "select") {
                    newNode = templateControl.cloneNode(true);
                    newNode.style.property = this.get_column().editorStyle;
                    newNode.id = newNode.uniqueID;
                    container.appendChild(newNode);
                }
                else {
                    newNode = container.ownerDocument.createElement("select");
                    container.appendChild(newNode);
                    $HBRootNS.ClientGrid._Helper.mergeStyle(newNode, this.get_column().editorStyle);

                    newNode.className = "form-control";
                    //                    $HGDomElement.createElementFromTemplate(
                    //					{
                    //					    nodeName: "select",
                    //					    properties: { style: this.get_column().editorStyle }
                    //					}, container);

                    for (var i = 0; i < templateControl.options.length; i++) {
                        var option = document.createElement("option");
                        option.value = templateControl.options[i].value;
                        option.text = templateControl.options[i].text;
                        //                        option.innerText = templateControl.options[i].text;
                        newNode.appendChild(option);
                    }
                }
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

    _onDataChangeWithNoValidation: function (e) {
        //这里添加一个"tagData"作为一个临时数据容器 供应用使用，如果"tagData"有值，则优先从"tagData"中取
        if (e.target.tagData)
            this.set_dataFieldDataByEvent(e.target.tagData);
        else
            this.set_dataFieldDataByEvent(e.target.value);

        this._raiseEditorValidateEvent();
    },

    //创建完控件之后抛出一个事件--客户端创建控件发生时的事件
    _raiseCellCreatingEditorEvent: function (container) {
        return this.get_clientGrid()._raiseCellCreatingEditorEvent(container, this, this.get_column(), this.get_rowData(), this.get_dataFieldData());
    },

    //创建完控件之后抛出一个事件--客户端创建控件发生时的事件
    _raiseCellCreatedEditorEvent: function () {
        var result = this.get_clientGrid()._raiseCellCreatedEditorEvent(this, this.get_column(), this.get_rowData());
        this.get_clientGrid()._runDataToEditor = result.runDataToEditor;
    },

    //抛出一个事件--客户端控件校验事件
    _raiseEditorValidateEvent: function () {
        this.get_clientGrid()._raiseEditorValidateEvent(this, this.get_column(), this.get_rowData());
    },

    //格式化
    formatValue: function () {
        var result = "";
        var dataValue = this.get_dataFieldData();
        if (this._column.formatString && typeof (dataValue) != "undefined" && dataValue != null && dataValue !== "") {
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

    get_displayValue: function () {
        switch (this.get_column().editTemplate.EditMode) {
            case $HBRootNS.ClientGridColumnEditMode.None:
                return $HBRootNS.DomElement.innerText(this._editorElement);
                break;
            case $HBRootNS.ClientGridColumnEditMode.TextBox:
                return this._editorElement.value;
                break;
            case $HBRootNS.ClientGridColumnEditMode.DropdownList:
                return this._editorElement.options[this._editorElement.selectedIndex].text;
                break;
            case $HBRootNS.ClientGridColumnEditMode.CheckBox:
                return this._editorElement.checked;
                break;
            case $HBRootNS.ClientGridColumnEditMode.DateInput:
                {
                    var value = this._editorElement.get_value();
                    if (value != Date.minDate)
                        return String.format("{0:yyyy-MM-dd}", value);
                    else
                        return "";
                    break;
                }
            default:
                throw Error.create("EditMode: " + editMode + "未实现");
                break;
        }
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
        this.get_clientGrid()._raiseDataChangedEvent(this, this.get_column(), this.get_rowData(), value);
    },

    set_editorTooltips: function () {
        if (this._column.editorTooltips)
            this._editorElement.title = this._column.editorTooltips;
    },

    set_tooltips: function (value) {
        if (this._column.editorTooltips)
            this._editorElement.title = value;
    }


}

$HBRootNS.GridColumnEditorBase.registerClass($HBRootNSName + ".GridColumnEditorBase");
/**********************************GridColumnEditorBase end***}*********************************/

/**********************************GridColumnStaticTextEditor*****************{********************************/
$HBRootNS.GridColumnStaticTextEditor = function (allowCopyFromElement, gridCell, column, rowData, clientGrid) {
    $HBRootNS.GridColumnStaticTextEditor.initializeBase(this, [allowCopyFromElement, gridCell, column, rowData, clientGrid]);
}

$HBRootNS.GridColumnStaticTextEditor.prototype = {

    _normalDataToEditor: function () {
        if (this._editorElement) {
            var result = this.formatValue();
            //this._editorElement.innerText = this._getDropdownListText(result);

            //抛出数据格式化事件
            var returnValue = this.get_clientGrid()._raiseDataFormattingEvent(this, this.get_column(), this.get_rowData(), result);

            $HBRootNS.DomElement.innerText(this._editorElement, this._getDropdownListText(returnValue.showValueTobeChange));
        }
    },

    createEditor: function (container) {
        $HBRootNS.GridColumnStaticTextEditor.callBaseMethod(this, 'createEditor', [container]);

        //抛出一个事件--客户端创建控件发生时的事件
        var eventResult = this._raiseCellCreatingEditorEvent(container);
        if (eventResult) {
            this.set_cellCreatingEditorResult(eventResult);

            if (eventResult.editor) {
                this._editorElement = eventResult.editor.get_editorElement();
            }
            this.set_dataFieldData(eventResult.valueTobeChange);
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
            this._editorElement = container.ownerDocument.createElement("span");
            container.appendChild(this._editorElement);
            //             $HGDomElement.createElementFromTemplate(
            //					{
            //					    nodeName: "span",
            //					    properties:
            //						{
            //						    style: this.get_column().editorStyle
            //						}
            //					},
            //					container
            //            );
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
        if (this._editorElement.type == "image") return;

        if (this._cellCreatingEditorResult) {

            if (this._cellCreatingEditorResult.showValueTobeChange != this._cellCreatingEditorResult.valueTobeChange)
                $HBRootNS.DomElement.innerText(this._editorElement, this._getDropdownListText(this._cellCreatingEditorResult.showValueTobeChange));
            else
                $HBRootNS.DomElement.innerText(this._editorElement, this._getDropdownListText(this._cellCreatingEditorResult.valueTobeChange));

            //Shen Zheng
            if (this._cellCreatingEditorResult.autoFormat) {
                this._normalDataToEditor();
            }

            //响应完_cellCreatingEditorEvent事件后将该值置为NULL
            this._cellCreatingEditorResult = null;
        }
        else {
            this._normalDataToEditor();
        }
    }
}

$HBRootNS.GridColumnStaticTextEditor.registerClass($HBRootNSName + ".GridColumnStaticTextEditor", $HBRootNS.GridColumnEditorBase);
/**********************************GridColumnStaticTextEditor end***}*********************************/

/**********************************GridColumnAEditor*****************{********************************/
$HBRootNS.GridColumnAEditor = function (allowCopyFromElement, gridCell, column, rowData, clientGrid) {
    $HBRootNS.GridColumnAEditor.initializeBase(this, [allowCopyFromElement, gridCell, column, rowData, clientGrid]);
}

$HBRootNS.GridColumnAEditor.prototype = {

    createEditor: function (container) {
        $HBRootNS.GridColumnAEditor.callBaseMethod(this, 'createEditor', [container]);
        var link;
        //抛出一个事件--客户端创建控件发生时的事件
        var eventResult = this._raiseCellCreatingEditorEvent(container);
        if (eventResult) {
            this.set_cellCreatingEditorResult(eventResult);
            if (eventResult.editor) {
                this._editorElement = eventResult.editor.get_editorElement();
            }
        }

        if (!this._editorElement) {
            var text = this._column.editTemplate.DefaultTextOfA;
            var href = this._column.editTemplate.DefaultHrefOfA;

            if (this._column.editTemplate.TextFieldOfA && this._column.editTemplate.TextFieldOfA != "#")
                text = this.get_rowData()[this._column.editTemplate.TextFieldOfA];
            if (this._column.editTemplate.HrefFieldOfA && this._column.editTemplate.HrefFieldOfA != "#")
                href = this.get_rowData()[this._column.editTemplate.HrefFieldOfA];

            if (!text) text = "Link";
            if (!href) href = "#";

            var target = "_blank";
            if (this._column.editTemplate.TargetOfA)
                target = this._column.editTemplate.TargetOfA;

            link = this._editorElement = container.ownerDocument.createElement("a");

            container.appendChild(link);
            link.target = target;
            link.href = href;
            link.appendChild(link.ownerDocument.createTextNode(text));
            link.style = this.get_column().editorStyle;


            //            $HGDomElement.createElementFromTemplate(
            //					{
            //					    nodeName: "a",
            //					    properties:
            //						{
            //						    style: this.get_column().editorStyle,
            //						    innerText: text,
            //						    href: href,
            //						    target: target
            //						}
            //					},
            //					container
            //            );
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
    }
}

$HBRootNS.GridColumnAEditor.registerClass($HBRootNSName + ".GridColumnAEditor", $HBRootNS.GridColumnEditorBase);
/**********************************GridColumnAEditor end***}*********************************/

/**********************************GridColumnTextBoxEditor*****************{********************************/
$HBRootNS.GridColumnTextBoxEditor = function (allowCopyFromElement, gridCell, column, rowData, clientGrid) {
    $HBRootNS.GridColumnTextBoxEditor.initializeBase(this, [allowCopyFromElement, gridCell, column, rowData, clientGrid]);
}

$HBRootNS.GridColumnTextBoxEditor.prototype = {

    createEditor: function (container) {
        $HBRootNS.GridColumnTextBoxEditor.callBaseMethod(this, 'createEditor', [container]);

        //抛出一个事件--客户端创建控件发生时的事件
        var eventResult = this._raiseCellCreatingEditorEvent(container);
        if (eventResult) {
            this.set_cellCreatingEditorResult(eventResult);
            if (eventResult.editor) {
                this._editorElement = eventResult.editor.get_editorElement();
            }
            this.set_dataFieldData(eventResult.valueTobeChange);
        }
        if (!this._editorElement) {
            this._editorElement = container.ownerDocument.createElement("input");
            this._editorElement.type = "text";
            this._editorElement.className = "form-control";

            container.appendChild(this._editorElement);

            //            $HGDomElement.createElementFromTemplate(
            //					{
            //					    nodeName: "input",
            //					    properties:
            //						{
            //						    type: "text",
            //						    style: this.get_column().editorStyle
            //						}
            //					},
            //					container
            //            );

            if (this.get_column().editorReadOnly)
                this._editorElement.readOnly = true;
            if (!this.get_column().editorEnabled)
                this._editorElement.disabled = true;
            if (this.get_column().maxLength > 0)
                this._editorElement.maxLength = parseInt(this.get_column().maxLength);
        }

        //抛出editor创建后事件
        this._raiseCellCreatedEditorEvent();

        //绑定校验
        if (this._column.autoBindingValidation)
            this.validationBinder(this._editorElement);
        else
            $addHandlers(this._editorElement, this.textBoxChange$delegate);

        if (this.get_clientGrid().get_readOnly() == false) {
            //抛出初始化editor事件
            this.get_clientGrid()._raiseInitializeEditorEvent(this, this.get_column(), this.get_rowData());
        }

        this.set_editorTooltips();
    },

    dataToEditor: function () {
        if (this._editorElement) {
            var result = this.formatValue();
            if (this._cellCreatingEditorResult && !this._cellCreatingEditorResult.autoFormat) {
                result = this._cellCreatingEditorResult.showValueTobeChange;
            }
            this._editorElement.value = result;

            //抛出数据格式化事件 text
            var returnValue = this.get_clientGrid()._raiseDataFormattingEvent(this, this.get_column(), this.get_rowData(), result);

            this._editorElement.value = returnValue.showValueTobeChange;

            //响应完_cellCreatingEditorEvent事件后将该值置为NULL
            this._cellCreatingEditorResult = null;
        }
    }

}

$HBRootNS.GridColumnTextBoxEditor.registerClass($HBRootNSName + ".GridColumnTextBoxEditor", $HBRootNS.GridColumnEditorBase);
/**********************************GridColumnTextBoxEditor end***}*********************************/

/**********************************GridColumnDropDownListEditor*****************{********************************/
$HBRootNS.GridColumnDropDownListEditor = function (allowCopyFromElement, gridCell, column, rowData, clientGrid) {
    $HBRootNS.GridColumnDropDownListEditor.initializeBase(this, [allowCopyFromElement, gridCell, column, rowData, clientGrid]);
    this._dropDownListEvents = { change: Function.createDelegate(this, this._onSelectedIndexChanged) };
}

$HBRootNS.GridColumnDropDownListEditor.prototype = {

    _getFirstNotEmptyItem: function (ddl) {
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
        var eventResult = this._raiseCellCreatingEditorEvent(container);
        if (eventResult) {
            if (eventResult.editor) {
                this._editorElement = eventResult.editor.get_editorElement();
                if (this._editorElement) {
                    //默认第一项非空项选中 注意eventResult.valueTobeChange != "" 表示是有数据，该哪哪，不管
                    if (this._editorElement.children.length > 0 && eventResult.valueTobeChange === "" && eventResult.firstItemSelected) {
                        var firstNotEmptyItem = this._getFirstNotEmptyItem(this._editorElement);
                        if (firstNotEmptyItem)
                            eventResult.valueTobeChange = firstNotEmptyItem.value;
                    }
                }
            }
            this.set_dataFieldData(eventResult.valueTobeChange);
        }

        if (this._editorElement) {
            var styles = this.get_column().editorStyle;
            for (var s in styles) {
                this._editorElement.style[s] = styles[s];
            }
            $addHandlers(this._editorElement, this._dropDownListEvents);
        }
        else {
            this._editorElement = $HGDomElement.createElementFromTemplate(
                {
                    nodeName: "select",
                    properties:
                    {
                        style: this.get_column().editorStyle
                    },
                    events: this._dropDownListEvents
                },
                container
            );
        }

        this._editorElement.className = "form-control";
        if (this.get_column().editorReadOnly)
            this._editorElement.readOnly = true;
        if (!this.get_column().editorEnabled)
            this._editorElement.disabled = true;

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

/**********************************GridColumnCheckBoxEditor*****************{********************************/
$HBRootNS.GridColumnCheckBoxEditor = function (allowCopyFromElement, gridCell, column, rowData, clientGrid) {
    $HBRootNS.GridColumnCheckBoxEditor.initializeBase(this, [allowCopyFromElement, gridCell, column, rowData, clientGrid]);
    this._CheckBoxEvents = { click: Function.createDelegate(this, this._onchange) };
}

$HBRootNS.GridColumnCheckBoxEditor.prototype = {

    createEditor: function (container) {
        $HBRootNS.GridColumnCheckBoxEditor.callBaseMethod(this, 'createEditor', [container]);

        //抛出一个事件--客户端创建控件发生时的事件
        var eventResult = this._raiseCellCreatingEditorEvent(container);
        if (eventResult) {
            if (eventResult.editor) {
                this._editorElement = eventResult.editor.get_editorElement();
            }
            this.set_dataFieldData(eventResult.valueTobeChange);
        }
        if (!this._editorElement) {
            this._editorElement = container.ownerDocument.createElement("input");
            this._editorElement.type = "checkbox";
            container.appendChild(this._editorElement);
            $addHandlers(this._editorElement, this._CheckBoxEvents);
            //            $HGDomElement.createElementFromTemplate(
            //				{
            //				    nodeName: "input",
            //				    properties: { type: "checkbox" },
            //				    events: this._CheckBoxEvents
            //				},
            //				container
            //            );
        }

        if (!this.get_column().editorEnabled)
            this._editorElement.disabled = true;

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
            this._editorElement.checked = result;

            //抛出数据格式化事件
            var returnValue = this.get_clientGrid()._raiseDataFormattingEvent(this, this.get_column(), this.get_rowData(), result);

            this._editorElement.checked = returnValue.showValueTobeChange;
        }
    },

    _onchange: function (e) {
        this.set_dataFieldDataByEvent(e.target.checked);

        this._raiseEditorValidateEvent();
    }
}

$HBRootNS.GridColumnCheckBoxEditor.registerClass($HBRootNSName + ".GridColumnCheckBoxEditor", $HBRootNS.GridColumnEditorBase);
/**********************************GridColumnCheckBoxEditor end***}*********************************/

/**********************************GridColumnDateInputEditor*****************{********************************/
$HBRootNS.GridColumnDateInputEditor = function (allowCopyFromElement, gridCell, column, rowData, clientGrid) {
    $HBRootNS.GridColumnDateInputEditor.initializeBase(this, [allowCopyFromElement, gridCell, column, rowData, clientGrid]);
}

$HBRootNS.GridColumnDateInputEditor.prototype = {
    get_generalCalendarID: function () {
        //取到预先下去的日历牌ID，然后找它的日历牌的按钮图片
        return this.get_clientGrid().get_deluxeDateTimePickerID();
    },

    createEditor: function (container) {
        $HBRootNS.GridColumnDateInputEditor.callBaseMethod(this, 'createEditor', [container]);

        //抛出一个事件--客户端创建控件发生时的事件
        var eventResult = this._raiseCellCreatingEditorEvent(container);
        if (eventResult) {
            if (eventResult.editor) {
                this._editorElement = eventResult.editor.get_editorElement();
            }
            this.set_dataFieldData(eventResult.valueTobeChange);
        }
        if (!this._editorElement) {
            var div = container.ownerDocument.createElement("div");
            div.className = "datepicker-input input-group";
            container.appendChild(div);
            //            var input = $HGDomElement.createElementFromTemplate(
            //				        {
            //				            nodeName: "input",
            //				            properties:
            //                            {
            //                                type: "text",
            //                                title: this._column.editorTooltips,
            //                                style: this.get_column().editorStyle
            //                            },
            //				            cssClasses: ["ajax_calendartextbox ajax__calendar_textbox"]
            //				        }, container);


            //            var calendar = new $HGRootNS.DateTimePicker(div);
            var calendar = $create($HGRootNS.DateTimePicker, {}, {}, null, div);
            calendar.add_onClientValueChanged(Function.createDelegate(this, this._calendarDataValueChange));
            //            calendar.clientInitialize(this.get_generalCalendarID());

            //日期发生变化事件
            //            calendar.add_clientValueChanged(Function.createDelegate(this, this._calendarDataValueChange)); //calendar里头抛出的事件

            this._editorElement = calendar;
        }

        if (this.get_column().editorReadOnly)
            this._editorElement.set_readOnly(true);
        if (!this.get_column().editorEnabled)
            this._editorElement.set_readOnly(true);

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

/**********************************GridColumnDateTimeInputEditor*****************{********************************/
$HBRootNS.GridColumnDateTimeInputEditor = function (allowCopyFromElement, gridCell, column, rowData, clientGrid) {
    $HBRootNS.GridColumnDateTimeInputEditor.initializeBase(this, [allowCopyFromElement, gridCell, column, rowData, clientGrid]);
}

$HBRootNS.GridColumnDateTimeInputEditor.prototype = {
    get_generalCalendarDateTimeID: function () {
        //取到预先下去的日历牌ID，然后找它的日历牌的按钮图片
        return this.get_clientGrid().get_deluxeDateTimePickerID();
    },

    createEditor: function (container) {
        $HBRootNS.GridColumnDateTimeInputEditor.callBaseMethod(this, 'createEditor', [container]);

        //抛出一个事件--客户端创建控件发生时的事件
        var eventResult = this._raiseCellCreatingEditorEvent(container);
        var doc = container.ownerDocument;
        if (eventResult) {
            if (eventResult.editor) {
                this._editorElement = eventResult.editor.get_editorElement();
            }
            this.set_dataFieldData(eventResult.valueTobeChange);
        }
        if (!this._editorElement) {
            var div = container.ownerDocument.createElement("div");
            div.className = "datepicker-input input-group";
            container.appendChild(div);
            //            var span = doc.createElement("div");
            //            var span = $HGDomElement.createElementFromTemplate(
            //				        {
            //				            nodeName: "span",
            //				            properties:
            //                            {
            //                                title: this._column.editorTooltips,
            //                                style: this.get_column().editorStyle
            //                            },
            //				            cssClasses: ["ajax_calendartextbox ajax__calendar_textbox"]
            //				        }, container);
            var picker = $create($HGRootNS.DateTimePicker, { mode: $HGRootNS.DateTimePickerMode.DateTimePicker }, {}, null, div);
            picker.add_onClientValueChanged(Function.createDelegate(this, this._calendarDataTimeValueChange));

            //            var deluxeDateTime = new $HGRootNS.DeluxeDateTimePicker(span);
            //            deluxeDateTime.clientInitialize(this.get_generalCalendarDateTimeID());

            //日期发生变化事件
            deluxeDateTime.add_clientValueChanged(Function.createDelegate(this, this._calendarDataTimeValueChange)); //calendar里头抛出的事件

            this._editorElement = deluxeDateTime;
        }

        if (this.get_column().editorReadOnly)
            this._editorElement.set_ReadOnly(true);
        if (!this.get_column().editorEnabled)
            this._editorElement.set_ReadOnly(true);

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

    _calendarDataTimeValueChange: function () {
        this.set_dataFieldDataByEvent(this._editorElement.get_value());

        this._raiseEditorValidateEvent();
    }
}

$HBRootNS.GridColumnDateTimeInputEditor.registerClass($HBRootNSName + ".GridColumnDateTimeInputEditor", $HBRootNS.GridColumnEditorBase);

/**********************************GridColumnDateTimeInputEditor end***}*********************************/

/**********************************GridColumnOuUserInputEditor*****************{********************************/
$HBRootNS.GridColumnOuUserInputEditor = function (allowCopyFromElement, gridCell, column, rowData, clientGrid) {
    $HBRootNS.GridColumnOuUserInputEditor.initializeBase(this, [allowCopyFromElement, gridCell, column, rowData, clientGrid]);
}

$HBRootNS.GridColumnOuUserInputEditor.prototype = {
    createEditor: function (container) {
        var newControl = null, s;

        if (this._column.editTemplate) {
            if (this._column.editTemplate.TemplateControlClientID != null &&
                this._column.editTemplate.TemplateControlClientID != "") {

                var template = $find(this._column.editTemplate.TemplateControlClientID);
                newControl = template.cloneAndAppendToContainer(container);
                newControl.add_selectedDataChanged(Function.createDelegate(this, this._selectedDataValueChanged));

                var styles = this.get_column().editorStyle;
                for (s in styles) {
                    if (s == "width" && status[s].indexOf("px") > 0) {
                        continue;
                    }
                    newControl._element.style[s] = styles[s];
                }

                var controlSettings = this._column.editTemplate.TemplateControlSettings;
                if (controlSettings) {
                    controlSettings = Sys.Serialization.JavaScriptSerializer.deserialize(controlSettings);
                    for (s in controlSettings) {
                        switch (s) {
                            case "multiSelect":
                                newControl.set_multiSelect(controlSettings[s].toLowerCase() == "true");
                                break; ;
                            case "rootPath":
                                newControl.set_rootPath(controlSettings[s]);
                                break; ;
                            case "listMask":
                                newControl.set_listMask(controlSettings[s]);
                                break;
                            case "selectMask":
                                newControl.set_selectMask(controlSettings[s]);
                                break;
                            case "enableUserPresence":
                                newControl.set_enableUserPresence(controlSettings[s].toLowerCase() == "true");
                                break;
                            case "showCheckButton":
                                newControl.set_showCheckIcon(controlSettings[s].toLowerCase() == "true");
                                newControl._setButtonStatus();
                                break;
                            case "showSelector":
                                newControl.set_showSelector(controlSettings[s].toLowerCase() == "true");
                                newControl._setButtonStatus();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        this._editorElement = newControl;

        if (this.get_clientGrid().get_readOnly() || this.get_column().editorReadOnly)
            this._editorElement.set_readOnly(true);
        if (!this.get_column().editorEnabled)
            this._editorElement.set_readOnly(true);

        //抛出editor创建后事件
        this._raiseCellCreatedEditorEvent();

        if (this.get_clientGrid().get_readOnly() == false) {
            //抛出初始化editor事件
            this.get_clientGrid()._raiseInitializeEditorEvent(this, this.get_column(), this.get_rowData());
        }

        this.set_editorTooltips();
    },

    dataToEditor: function () {
        var data = this.get_dataFieldData();
        var clientSetProName = this._column.editTemplate.ControlClientSetPropName ? this._column.editTemplate.ControlClientSetPropName : "set_selectedOuUserData";

        if (!data) {
            if (clientSetProName === "set_selectedOuUserData")
                data = new Array();
            else
                data = null;

            this.set_dataFieldDataByEvent(data);
        }

        this._editorElement[clientSetProName](data);

        this._editorElement.dataBind();
    },

    _selectedDataValueChanged: function () {
        var clientProName = this._column.editTemplate.ControlClientPropName ? this._column.editTemplate.ControlClientPropName : "get_selectedOuUserData";
        this.set_dataFieldDataByEvent(this._editorElement[clientProName]());
        this._raiseEditorValidateEvent();
    }
}

$HBRootNS.GridColumnOuUserInputEditor.registerClass($HBRootNSName + ".GridColumnOuUserInputEditor", $HBRootNS.GridColumnEditorBase);

/**********************************GridColumnOuUserInputEditor end***}*********************************/

/**********************************GridColumnMaterialEditor*****************{********************************/
$HBRootNS.GridColumnMaterialEditor = function (allowCopyFromElement, gridCell, column, rowData, clientGrid) {
    $HBRootNS.GridColumnMaterialEditor.initializeBase(this, [allowCopyFromElement, gridCell, column, rowData, clientGrid]);
}

$HBRootNS.GridColumnMaterialEditor.prototype = {
    createEditor: function (container) {
        var newControl = null;

        if (this._column.editTemplate) {
            if (this._column.editTemplate.TemplateControlClientID != null &&
                this._column.editTemplate.TemplateControlClientID != "") {

                var template = $find(this._column.editTemplate.TemplateControlClientID);
                newControl = template.cloneAndAppendToContainer(container);
                newControl.add_materialsChanged(Function.createDelegate(this, this._selectedDataValueChanged));
            }
        }

        this._editorElement = newControl;

        if (this.get_clientGrid().get_readOnly() || this.get_column().editorReadOnly) {
            this._editorElement.set_allowEdit(false);
            this._editorElement.set_allowEditContent(false);
        }
        if (!this.get_column().editorEnabled) {
            this._editorElement.set_allowEdit(false);
            this._editorElement.set_allowEditContent(false);
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
        var data = this.get_dataFieldData();

        if (data == "") {
            data = new Array();
            this.set_dataFieldDataByEvent(data);
        }

        var clientSetProName = this._column.editTemplate.ControlClientSetPropName ? this._column.editTemplate.ControlClientSetPropName : "set_materials";
        if (!(data.length == 0 && this._editorElement._materialUseMode == $HBRootNS.materialUseMode.SingleDraft)) {
            this._editorElement[clientSetProName](data);
        }
    },

    _selectedDataValueChanged: function () {
        var clientProName = this._column.editTemplate.ControlClientPropName ? this._column.editTemplate.ControlClientPropName : "get_materialsResult";
        this.set_dataFieldDataByEvent(this._editorElement[clientProName]());
        this._raiseEditorValidateEvent();
    }
}

$HBRootNS.GridColumnMaterialEditor.registerClass($HBRootNSName + ".GridColumnMaterialEditor", $HBRootNS.GridColumnEditorBase);

/**********************************GridColumnMaterialEditor end***}*********************************/
//window.onresize = function () { $HBRootNS.ClientGrid.followupSellteTable(); }
