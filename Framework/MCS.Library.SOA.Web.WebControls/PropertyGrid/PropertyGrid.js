//Property数据类型

$HGRootNS.PropertiesDisplayOrder = function () {
    throw Error.invalidOperation();
}

$HGRootNS.PropertiesDisplayOrder.prototype = {
    ByCategory: 1,
    ByAlphabet: 2
}

$HGRootNS.PropertiesDisplayOrder.registerEnum($HGRootNSName + ".PropertiesDisplayOrder");

$HGRootNS.PropertyGrid = function (element) {
    $HGRootNS.PropertyGrid.initializeBase(this, [element]);

    this._displayOrder = $HGRootNS.PropertiesDisplayOrder.ByAlphabet;
    this._categorySortBtn = null;
    this._alphabetSortBtn = null;
    this._footer = null;
    this._caption = "";
    this._captionElement = null;

    this.sortByCategory$delegate = {
        click: Function.createDelegate(this, this._onSortByCategoryClick)
    };

    this.sortByAlphabet$delegate = {
        click: Function.createDelegate(this, this._onSortByAlphabetClick)
    };
}

$HGRootNS.PropertyGrid.prototype =
{
    initialize: function () {
        $HGRootNS.PropertyGrid.callBaseMethod(this, 'initialize');
    },

    dispose: function () {
        if (this._categorySortBtn) {
            $HGDomEvent.removeHandlers(this._categorySortBtn, this.sortByCategory$delegate);
            this._categorySortBtn = null;
        }

        if (this._alphabetSortBtn) {
            $HGDomEvent.removeHandlers(this._alphabetSortBtn, this.sortByAlphabet$delegate);
            this._alphabetSortBtn = null;
        }

        this._displayOrder = null;
        this._footer = null;

        $HGRootNS.PropertyGrid.callBaseMethod(this, 'dispose');
    },

    loadClientState: function (value) {
        if (value) {
            if (value != "") {
                var state = Sys.Serialization.JavaScriptSerializer.deserialize(value);

                this._enumDefinitions = state;
            }
        }
        this._renderTable();
        this.dataBind();
    },

    saveClientState: function () {
        if (this._autoSaveClientState == true)
            return Sys.Serialization.JavaScriptSerializer.serialize(this._properties);
    },

    dataBind: function () {
        this._set_buttonsStyle();

        $HGRootNS.PropertyGrid.callBaseMethod(this, 'dataBind');

        this._propertiesContainer.innerHTML = "";
        var props = this.get_properties();
        switch (this.get_displayOrder()) {
            case $HGRootNS.PropertiesDisplayOrder.ByCategory:
                this._renderPropertiesTableByCategory(props, this._propertiesContainer);
                break;
            case $HGRootNS.PropertiesDisplayOrder.ByAlphabet:
                this._renderPropertiesTable(props, this._propertiesContainer);
                break;
        }
    },

    get_caption: function () {
        return this._caption;
    },

    set_caption: function (value) {
        this._caption = value;

        if (this._captionElement != null)
            this._captionElement.innerText = this._caption;
    },

    _renderPropertiesTableByCategory: function (props, container) {
        var categories = this._convertPropertiesToCategries(props);

        var table = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "table",
				    properties:
					{
					    cellSpacing: 0,
					    cellPadding: 0
					},
				    cssClasses: ["ajax__propertyGrid_Editor_table"]
				},
				container
				);

        var sortedCategories = this._sortCategories(categories);

        for (var i = 0; i < sortedCategories.length; i++)
            this._renderCategory(sortedCategories[i], table);
    },

    _sortCategories: function (categories) {
        var result = [];

        for (var name in categories) {
            result.push(categories[name]);
        }

        result = result.sort(function (c1, c2) {
            return (c1.name.localeCompare(c2.name));
        });

        return result;
    },

    _renderCategory: function (category, table) {

        this._renderCategoryRow(category.name, table);

        var categoryPropertiesRow = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "tr"
				}, table
				);

        var categoryPropertiesCell = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "td"
				}, categoryPropertiesRow
				);

        this._renderPropertiesTable(category.properties, categoryPropertiesCell);
    },

    _onEditorLeaveDelegate: function (prop) {
        var property = prop;
        this._footer.innerText = "";

        if (this._activeEditor) {
            this._activeEditor.commitValue();
        }
    },

    afterEditorEnterDelegate: function (prop) {
        this._footer.innerText = prop.description ? prop.description : "";
    },

    _set_buttonsStyle: function () {
        switch (this.get_displayOrder()) {
            case $HGRootNS.PropertiesDisplayOrder.ByCategory:
                this._categorySortBtn.className = "ajax__propertyGrid_categorySortButton_hover";
                this._alphabetSortBtn.className = "ajax__propertyGrid_alphabetSortButton";
                break;
            case $HGRootNS.PropertiesDisplayOrder.ByAlphabet:
                this._categorySortBtn.className = "ajax__propertyGrid_categorySortButton";
                this._alphabetSortBtn.className = "ajax__propertyGrid_alphabetSortButton_hover";
                break;
        }
    },

    _onSortByCategoryClick: function (eventElement) {
        this.set_displayOrder($HGRootNS.PropertiesDisplayOrder.ByCategory);
        this.dataBind();
    },

    _onSortByAlphabetClick: function (eventElement) {
        this.set_displayOrder($HGRootNS.PropertiesDisplayOrder.ByAlphabet);
        this.dataBind();
    },

    _renderPropertiesTable: function (props, container) {
        var table = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "table",
				    properties:
					{
					    cellSpacing: 0,
					    cellPadding: 0
					},
				    cssClasses: ["ajax__propertyGrid_Editor_table"]
				},
				container
				);

        this._renderPropertyRows(props, table, 1);
    },

    _getPropertyDisplayElementName: function (prop) {
        return String.format("propertyGrid_cell_{0}", prop.name)
    },

    //styleName
    _get_activePropertyStyleName: function () {
        return "ajax__propertyGrid_nameCell_selected";
    },

    _getCategoryDisplayElementStyleName: function () {
        return "ajax__propertyGrid_categoryNameCell";
    },

    _getPropertyDisplayElementStyleName: function () {
        return "ajax__propertyGrid_valueCell";
    },

    _getPropertyNameElementStyleName: function () {
        return "ajax__propertyGrid_valueCell";
    },

    _getPropertyNameElementStyleName: function () {
        return "ajax__propertyGrid_nameCell";
    },

    _getDefaultValueDiffStyleName: function () {
        return "ajax_propertyGrid_defaultValue_diff_style";
    },

    _renderTable: function () {
        var table = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "table",
				    properties:
					{
					    cellSpacing: 0,
					    cellPadding: 0,
					    style:
						{
						    width: "100%",
						    height: "100%"
						}
					}
				},
				this.get_element()
				);

        var headerRow = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "tr"
				}, table
				);
        this._renderHeaderCell(headerRow);

        var propertiesRow = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "tr"
				}, table
				);

        this._renderPropertiesCell(propertiesRow);

        var footerRow = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "tr"
				}, table
				);

        this._renderFooterCell(footerRow);

        return table;
    },

    _renderPropertyRows: function (props, table, colcount) {

        for (var i = 0; i < props.length; i++) {
            var prop = props[i];
            if (this.get_readOnly() == true && prop.readOnly == false)
                prop.readOnly = true;

            if (prop.visible) {
                var propertyRow = $HGDomElement.createElementFromTemplate({
                    nodeName: "tr"
                }, table);

                this._renderPropNameCell(prop, propertyRow, "50%");
                this._renderPropValueCell(prop, propertyRow, "50%");
            }
        }
    },

    _renderHeaderCell: function (container) {
        var cell = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "td",
				    cssClasses: ["ajax__propertyGrid_header"]
				}, container
				);

        var categorySortBtn = $HGDomElement.createElementFromTemplate(
		{
		    nodeName: "span",
		    cssClasses: ["ajax__propertyGrid_categorySortButton"],
		    properties: {
		        title: $NT.getText("SOAWebControls", "分类显示")
		    }
		}, cell
		);

        $addHandlers(categorySortBtn, this.sortByCategory$delegate);

        this._categorySortBtn = categorySortBtn;

        var alphabetSortBtn = $HGDomElement.createElementFromTemplate(
		{
		    nodeName: "span",
		    cssClasses: ["ajax__propertyGrid_alphabetSortButton_hover"],
		    properties: {
		        title: $NT.getText("SOAWebControls", "按照字母排序显示")
		    }
		}, cell
		);

        $addHandlers(alphabetSortBtn, this.sortByAlphabet$delegate);

        this._alphabetSortBtn = alphabetSortBtn;

        this._captionElement = $HGDomElement.createElementFromTemplate(
		{
		    nodeName: "span",
		    cssClasses: ["ajax__propertyGrid_caption"],
		    properties: {
		        innerText: this.get_caption()
		    }
		}, cell
		);
    },

    _renderPropertiesCell: function (container) {
        var cell = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "td",
				    cssClasses: ["ajax__propertyGrid_propertiesCell"]
				}, container
				);

        var div = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "div",
				    cssClasses: ["ajax__propertyGrid_propertiesContainer"]
				}, cell
				);

        this._propertiesContainer = div;
    },

    _renderFooterCell: function (container) {
        var cell = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "td",
				    cssClasses: ["ajax__propertyGrid_footer"]
				}, container
				);
        this._footer = cell;

    },
    //Properties
    get_displayOrder: function () {
        return this._displayOrder;
    },

    set_displayOrder: function (value) {
        this._displayOrder = value;
    },
    //End
    pseudo: function () {

    }
}

$HGRootNS.PropertyGrid.registerClass($HGRootNSName + ".PropertyGrid", $HGRootNS.PropertyEditorControlBase);

