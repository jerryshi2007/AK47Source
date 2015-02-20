/* File Created: 八月 6, 2014 */
$HGRootNS.PropertyGridDisplayOrder = function () {
    throw Error.invalidOperation();
}

$HGRootNS.PropertyGridDisplayOrder.prototype = {
    ByCategory: 1,
    ByAlphabet: 2
}

$HGRootNS.PropertyGridDisplayOrder.registerEnum($HGRootNSName + ".PropertyGridDisplayOrder");

$HGRootNS.PropertyGrid = function (element) {

    $HGRootNS.PropertyGrid.initializeBase(this, [element]);

    this._displayOrder = $HGRootNS.PropertyGridDisplayOrder.ByAlphabet;
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
            case $HGRootNS.PropertyGridDisplayOrder.ByCategory:
                this._renderPropertiesTableByCategory(props, this._propertiesContainer);
                break;
            case $HGRootNS.PropertyGridDisplayOrder.ByAlphabet:
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
            if ("textContent" in this._captionElement)
                this._captionElement.textContent = this._caption;
            else
                this._captionElement.innerText = this._caption;
    },

    _renderPropertiesTableByCategory: function (props, container) {
        var categories = this._convertPropertiesToCategries(props);

        var table = document.createElement("table");
        table.className = "propertygrid-view view-category";
        container.appendChild(table);

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

        var categoryPropertiesRow = table.insertRow(-1);

        var categoryPropertiesCell = categoryPropertiesRow.insertCell(-1);

        this._renderPropertiesTable(category.properties, categoryPropertiesCell);
    },

    _onEditorLeaveDelegate: function (prop) {
        var property = prop;
        $(this._footer).text("");

        if (this._activeEditor) {
            this._activeEditor.commitValue();
        }
    },

    afterEditorEnterDelegate: function (prop) {
        $(this._footer).text(prop.description ? prop.description : "");
    },

    _set_buttonsStyle: function () {
        switch (this.get_displayOrder()) {
            case $HGRootNS.PropertyGridDisplayOrder.ByCategory:
                Sys.UI.DomElement.addCssClass(this._categorySortBtn, "active");
                Sys.UI.DomElement.removeCssClass(this._alphabetSortBtn, "active");
                break;
            case $HGRootNS.PropertyGridDisplayOrder.ByAlphabet:
                Sys.UI.DomElement.removeCssClass(this._categorySortBtn, "active");
                Sys.UI.DomElement.addCssClass(this._alphabetSortBtn, "active");
                break;
        }
    },

    _onSortByCategoryClick: function (eventElement) {
        this.set_displayOrder($HGRootNS.PropertyGridDisplayOrder.ByCategory);
        this.dataBind();
    },

    _onSortByAlphabetClick: function (eventElement) {
        this.set_displayOrder($HGRootNS.PropertyGridDisplayOrder.ByAlphabet);
        this.dataBind();
    },

    _renderPropertiesTable: function (props, container) {
        var table = document.createElement("table");
        table.className = "propertygrid-view view-alpha";
        container.appendChild(table);

        this._renderPropertyRows(props, table, 1);
    },

    _getPropertyDisplayElementName: function (prop) {
        return String.format("propertygrid_cell_{0}", prop.name)
    },

    //styleName
    _get_activePropertyStyleName: function () {
        return "active";
    },

    _getCategoryDisplayElementStyleName: function () {
        return "group-title";
    },

    _getPropertyDisplayElementStyleName: function () {
        return "valuecell";
    },

    _getPropertyNameElementStyleName: function () {
        return "namecell";
    },

    _getDefaultValueDiffStyleName: function () {
        return "propertygrid-dirty";
    },

    _renderTable: function () {
        var div;
        var table = document.createElement("div");
        table.className = "propertygrid-layout";

        this.get_element().appendChild(table);

        div = document.createElement("div");
        div.className = "propertygrid-header";
        table.appendChild(div);
        this._renderHeaderCell(div);

        div = document.createElement("div");
        div.className = "propertygrid-contents";
        table.appendChild(div);
        this._renderPropertiesCell(div);

        div = document.createElement("div");
        div.className = "propertygrid-footer";
        table.appendChild(div);
        this._renderFooterCell(div);

        return table;
    },

    _renderPropertyRows: function (props, table, colcount) {

        for (var i = 0; i < props.length; i++) {
            var prop = props[i];
            if (this.get_readOnly() == true && prop.readOnly == false)
                prop.readOnly = true;

            if (prop.visible) {
                var propertyRow = table.insertRow(-1);

                this._renderPropNameCell(prop, propertyRow, "50%");
                this._renderPropValueCell(prop, propertyRow, "50%");
            }
        }
    },

    _renderHeaderCell: function (cell) {

        var btn = document.createElement("button");
        btn.type = "button";
        btn.className = "btn btn-default sorter sorting-category";
        btn.title = "分类显示";
        cell.appendChild(btn);

        btn.appendChild(document.createElement("s"));

        $addHandlers(btn, this.sortByCategory$delegate);

        this._categorySortBtn = btn;

        btn = document.createElement("button");
        btn.type = "button"
        btn.className = "btn btn-default sorter sorting-alpha";
        btn.title = "按照字母顺序显示";
        cell.appendChild(btn);
        btn.appendChild(document.createElement("s"));

        $addHandlers(btn, this.sortByAlphabet$delegate);

        this._alphabetSortBtn = btn;

        this._captionElement = document.createElement("span");
        this._captionElement.className = "propertygrid-caption";
        cell.appendChild(this._captionElement);
        this._captionElement.appendChild(document.createTextNode(this.get_caption()));
    },

    _renderPropertiesCell: function (cell) {
        this._propertiesContainer = document.createElement("div");
        this._propertiesContainer.className = "propertygrid-property-layout";
        cell.appendChild(this._propertiesContainer);
    },

    _renderFooterCell: function (cell) {
        var div = document.createElement("div");
        cell.appendChild(div);
        this._footer = div;

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
