/* File Created: 八月 6, 2014 */

$HGRootNS.PropertyForm = function (element) {
    $HGRootNS.PropertyForm.initializeBase(this, [element]);

    this._formSections = [];
}

$HGRootNS.PropertyForm.prototype =
{
    initialize: function () {
        $HGRootNS.PropertyForm.callBaseMethod(this, 'initialize');
    },

    dispose: function () {
        this._formsections = null;
        $HGRootNS.PropertyForm.callBaseMethod(this, 'dispose');
    },

    loadClientState: function (value) {
        if (value) {
            if (value != "") {
                var state = Sys.Serialization.JavaScriptSerializer.deserialize(value);

                this._enumDefinitions = state;
            }
        }
        //$HGRootNS.PropertyForm.callBaseMethod(this, 'loadClientState');
        this.dataBind();
    },

    saveClientState: function () {
        if (this._autoSaveClientState == true)
            return Sys.Serialization.JavaScriptSerializer.serialize([this._properties, this._formsections]);
    },

    dataBind: function () {
        $HGRootNS.PropertyForm.callBaseMethod(this, 'dataBind');
        this._renderFormSectionTable();
    },

    ///
    _get_activePropertyStyleName: function () {
        return "selected";
    },

    _getCategoryDisplayElementStyleName: function () {
        return "ajax__propertyForm_header";
    },

    _renderFormSectionTable: function () {
        var table = document.createElement("table");
        table.className = "propertyform-section";
        this.get_element().appendChild(table);

        this._renderSectionRows(table);
    },

    _getSectionByName: function (strName) {
        var sections = this.get_formsections();
        var secount = sections.length;
        var result = null;

        for (var i = 0; i < secount; i++) {
            var item = sections[i];
            if (item.displayName == strName || item.name == strName) {
                result = item;
                break;
            }
        }

        if (result == null) {
            if (typeof (strName) == "undefined" || strName == "" || strName == "Default") {
                result = { "columns": 4, "displayName": "基本", "name": "Default" };
            } else {
                result = { "columns": 4, "displayName": strName, "name": strName };
            }

            this.get_formsections().push(result);
        }

        return result;
    },

    _renderSectionRows: function (sectionContainer) {
        //        var sections = this.get_formsections();
        //        // _formSections;
        //        var secount = sections.length;
        //        if (secount == 0) {
        //            sections.push({ "columns": 4, "displayName": "基本", "name": "Default" });
        //            secount = 1;
        //        }

        var categories = this._convertPropertiesToCategries(this.get_properties());

        for (var item in categories) {

            var se = this._getSectionByName(item.toString());

            var category = null;
            if (categories.hasOwnProperty(se.name))
                category = categories[se.name];
            else if (category == null && categories.hasOwnProperty(se.displayName))
                category = categories[se.displayName];
            else continue;

            var displayName = typeof (se.displayName) != "undefined" && se.displayName != "" ? se.displayName : se.name;
            this._renderCategoryRow(displayName, sectionContainer);

            var bodyRow = sectionContainer.insertRow(-1);

            var bodycell = bodyRow.insertCell(-1);

            this._renderBoydContent(bodycell, se, category.properties);
        }
    },

    _convertPropertiesToCategries: function (props) {
        var categories = {};
        for (var i = 0; i < props.length; i++) {
            var prop = props[i];
            if (prop.visible) {
                var categoryName = ((typeof (prop.category) == "undefined") || prop.category == "") ? "Default" : prop.category;

                var category = categories[categoryName];

                if (!category) {
                    category = { name: categoryName, properties: [] };
                    categories[categoryName] = category;
                }
                category.properties.push(prop);
            }
        }
        return categories;
    },

    _renderSectiontop: function (setopContainer, sectionObject) {
        var cell = setopContainer.insertCell(-1);
        cell.className = "propertyform-header-cell";
        cell.appendChild(document.createTextNode(sectionObject.displayName));
    },

    _renderBoydContent: function (sebodyContainer, sectionObject, sectionprops) {
        var table = document.createElement("table");
        table.className = "propertyfrom-table";

        sebodyContainer.appendChild(table);

        this._renderPropertyRows(sectionprops, table, sectionObject.columns);
    },

    _getPropertyDisplayElementName: function (prop) {
        return String.format("propertyForm_Cell_{0}", prop.name)
    },

    _getPropertyDisplayElementStyleName: function () {
        return "ajax__propertyForm_valueCell";
    },

    _getPropertyNameElementStyleName: function () {
        return "ajax__propertyFrom_nameCell";
    },

    _getDefaultValueDiffStyleName: function () {
        return "ajax_propertyForm_defaultValue_diff_style";
    },

    /*属性定义*/
    get_formsections: function () {
        return this._formsections;
    },

    set_formsections: function (value) {
        this._formsections = value;
    }
}

$HGRootNS.PropertyForm.registerClass($HGRootNSName + ".PropertyForm", $HGRootNS.PropertyEditorControlBase);