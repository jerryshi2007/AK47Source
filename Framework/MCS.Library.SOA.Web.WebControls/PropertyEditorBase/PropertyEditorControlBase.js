/// <reference path="../MCSWebApp/PermissionCenter/scripts/MicrosoftAjax.debug.js" />
/// <reference path="../Script/Resources/HBCommon.js" />
$HGRootNS.PropertyDataType = function () {
    throw Error.invalidOperation();
}

$HGRootNS.PropertyDataType.prototype =
{
    Object: 1,
    Boolean: 3,
    Integer: 9,
    Decimal: 15,
    DateTime: 16,
    String: 18,
    Enum: 20
}

$HGRootNS.PropertyDataType.registerEnum($HGRootNSName + ".PropertyDataType");

$HGRootNS.PropertyEditor$CheckBoxStateCollection = function () {
    this._innerArray = new Array(); // 元素 { elem: elem }
}

$HGRootNS.PropertyEditor$CheckBoxWrapper = function (elem, prop, owner) {
    this._elem = elem;
    this._prop = prop;
    this._owner = owner;
    this._checkBoxClickDelegate = Function.createDelegate(this, this._onCheckBoxClick);
}

$HGRootNS.PropertyEditor$CheckBoxWrapper.prototype = {
    initialize: function () {
        $addHandler(this._elem, "click", this._checkBoxClickDelegate);
    },

    dispose: function () {
        $clearHandlers(this._elem);
        this._checkBoxClickDelegate = null;
    },

    _onUpdated: function () {
        this._owner._propertyEditors[this._prop.name].editor.set_enabled(this._elem.checked);
    },

    _onCheckBoxClick: function () {
        this._owner._propertyEditors[this._prop.name].editor.set_enabled(this._elem.checked);
    }
}

$HGRootNS.PropertyEditor$CheckBoxStateCollection.prototype = {
    count: function () {
        return this._innerArray.length;
    },

    get_keys: function () {
        var arr = [];
        for (var t in this._innerArray) {
            arr.push(t);
        }

        return arr;
    },

    get_checked: function (key) {
        var result = null;
        var item = this._innerArray[key];
        if (item) {
            result = item._elem.checked;
        }

        elem = null;

        return result;
    },

    dispose: function () {

        for (var t in this._innerArray) {
            if (this._innerArray[t]) {
                if (typeof (this._innerArray[t].dispose) == "function")
                    this._innerArray[t].dispose();

                this._innerArray[t] = null;
            }
        }

        this._innerArray = [];
    }
};

$HGRootNS.PropertyEditorControlBase = function (element) {
    $HGRootNS.PropertyEditorControlBase.initializeBase(this, [element]);

    this._properties = new Array();
    this._propertyEditorsArray = new Array();
    this._enumDefinitions = new Array();
    //this._validList = new Array();
    this._activeEditor = null;
    this._autoSaveClientState = null;
    this._invalidLineImage = null;
    this._readOnly = false;
    this._showCheckBoxes = false;
    this._checkBoxStates = new $HGRootNS.PropertyEditor$CheckBoxStateCollection();

    this._delegations = {
        editorEnter: Function.createDelegate(this, this._onEditorEnterDelegate),
        editorLeave: Function.createDelegate(this, this._onEditorLeaveDelegate),
        editorClick: Function.createDelegate(this, this._onEditorClickDelegate),
        editorValidating: Function.createDelegate(this, this._onEditorValidatingDelegate),
        editorValidate: Function.createDelegate(this, this._onEditorValidateDelegate),
        editorValidated: Function.createDelegate(this, this._onEditorValidatedDelegate),

        editorEnterPress: Function.createDelegate(this, this._onEditorPressDelegate),

        bindEditorDropdownList: Function.createDelegate(this, this._onBindEditorDropdownList)
    };

    this._delegations._owner = this;
}

$HGRootNS.PropertyEditorControlBase.prototype = {

    initialize: function () {
        $HGRootNS.PropertyEditorControlBase.callBaseMethod(this, 'initialize');
    },

    dispose: function () {
        for (var i = 0; i < this._propertyEditorsArray.length; i++) {
            this._propertyEditorsArray[i].dispose();
        }

        this._properties = null;
        this._propertyEditorsArray = null;
        this._enumDefinitions = null;

        this._activeEditor = null;
        this._checkBoxStates.dispose();
        this._checkBoxStates = null;

        $HGRootNS.PropertyEditorControlBase.callBaseMethod(this, 'dispose');
    },

    dataBind: function () {
        for (var i = 0; i < this._properties.length; i++) {
            if (this._properties[i].dataType == $HGRootNS.PropertyDataType.Boolean) {
                this._properties[i].value = this._properties[i].value.toString().toLowerCase();
            }
        }

        this._propertyEditors = {};
        this._propertyEditorsArray = [];

        for (var editor in this._propertyEditors) {
            editor.dispose();
        }
        this._activeEditor = null;
    },

    loadClientState: function (value) {
        if (value) {
            if (value != "") {
                var state = Sys.Serialization.JavaScriptSerializer.deserialize(value);

                this._enumDefinitions = state;
            }
        }
    }, updated: function () {
        if (this.get_showCheckBoxes()) {
            for (var k in this._checkBoxStates._innerArray) {
                this._checkBoxStates._innerArray[k]._onUpdated();
            }
        }
    },

    //Delegate
    _onEditorEnterDelegate: function (prop) {
        try {
            var needChange = true;
            if (this._activeEditor != null) {
                if (this._activeEditor.get_property().name == prop.name)
                    needChange = false;
            }

            if (needChange) {
                this._raiseEnterEditor(prop);
                if (this._activeEditor != null)
                    this._deactivePropertyEditor(this._activeEditor.get_property());

                this._activeEditor = this._propertyEditors[prop.name].editor;

                this._activeEditor.edit();

                this._activePropertyEditor(prop);

                this.afterEditorEnterDelegate(prop);
            }
        }
        catch (e) {
            $showError(e);
        }
    },

    _activePropertyEditor: function (prop) {
        var activeElement = $get(this._getPropertyDisplayElementName(prop), this._get_PropertyEditorRow(prop));
        var activeStyleName = this._get_activePropertyStyleName();

        if (activeElement) {
            if (Sys.UI.DomElement.containsCssClass(activeElement, activeStyleName) == false) {
                Sys.UI.DomElement.addCssClass(activeElement, activeStyleName);
            }
        }
    },

    _deactivePropertyEditor: function (prop) {
        var activeElement = $get(this._getPropertyDisplayElementName(prop), this._get_PropertyEditorRow(prop));
        var activeStyleName = this._get_activePropertyStyleName();

        if (activeElement) {
            if (Sys.UI.DomElement.containsCssClass(activeElement, activeStyleName) == true) {
                Sys.UI.DomElement.removeCssClass(activeElement, activeStyleName);
            }
        }
    },

    _get_activePropertyStyleName: function () {

    },

    _findPredefinedEnumDescription: function (enumTypeName) {
        var result = [];

        if (enumTypeName) {
            for (var i = 0; i < this._enumDefinitions.length; i++) {
                var ed = this._enumDefinitions[i];
                if (ed.EnumTypeName == enumTypeName) {
                    result = ed.Items;
                    break;
                }
            }
        }

        return result;
    },

    _convertPropertiesToCategries: function (props) {
        var categories = {};
        for (var i = 0; i < props.length; i++) {
            var prop = props[i];

            if (prop.visible) {
                var categoryName = prop.category;
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

    _get_editorByType: function (prop, cell, delegations) {
        var editor
        switch (prop.dataType) {
            case ($HGRootNS.PropertyDataType.Boolean):
                editor = this._get_editorByKey("BooleanPropertyEditor", prop, cell, this._delegations);
                break;
            case ($HGRootNS.PropertyDataType.Object):
                editor = this._get_editorByKey("ObjectPropertyEditor", prop, cell, this._delegations);
                break;
            case ($HGRootNS.PropertyDataType.Enum):
                editor = this._get_editorByKey("EnumPropertyEditor", prop, cell, this._delegations);
                break;
            case ($HGRootNS.PropertyDataType.DateTime):
                editor = this._get_editorByKey("DatePropertyEditor", prop, cell, this._delegations);
                break;
            default:
                editor = this._get_editorByKey("StandardPropertyEditor", prop, cell, this._delegations);
        }

        return editor;
    },

    _get_editorByKey: function (editorKey, prop, cell, delegations) {
        if (typeof ($HGRootNS.PropertyEditors) == "undefined")
            throw Error.create("$HGRootNS.PropertyEditors没有注册");

        var editor = $HGRootNS.PropertyEditors[editorKey];

        if (!editor) {
            if (prop.dataType) {
                switch (prop.dataType) {
                    case ($HGRootNS.PropertyDataType.Enum):
                        editor = $HGRootNS.PropertyEditors.EnumPropertyEditor;
                        prop.editorKey = "EnumPropertyEditor";
                        break;
                    case ($HGRootNS.PropertyDataType.DateTime):
                        editor = $HGRootNS.PropertyEditors.DatePropertyEditor;
                        break;
                    case ($HGRootNS.PropertyDataType.Boolean):
                        editor = $HGRootNS.PropertyEditors.BooleanPropertyEditor;
                        break;
                }
            } else {
                //throw Error.create("不能找到Key为" + editorKey + "的属性编辑器");
                editor = $HGRootNS.PropertyEditors["ObjectPropertyEditor"];
            }
        }

        if (typeof (editor) == "undefined") {
            throw Error.create("不能找到Key为" + editorKey + "的属性编辑器");
        }

        var ct = eval(editor.componentType);

        if (typeof (ct) == "undefined")
            throw Error.create("不能找到类型为" + editor.componentType + "的属性编辑器");

        return new ct(prop, cell, delegations);
    },

    _get_PropertyEditorRow: function (prop) {
        return this._propertyEditors[prop.name].row;
    },

    _raiseEnterEditor: function (propValue) {
        var handler = this.get_events().getHandler("enterEditor");

        if (handler) {
            var e = new Sys.EventArgs();
            e.propertyValue = propValue;
            handler(this, e);
        }
    },

    _raiseEditorBindDropdownList: function (prop, e) {
        var handler = this.get_events().getHandler("bindEditorDropdownList");
        if (handler) {
            handler(this, e);
        }
    },

    afterEditorEnterDelegate: function (prop) {

    },

    _onEditorLeaveDelegate: function (prop) {
        if (this._activeEditor) {
            this._activeEditor.commitValue();
        }
    },

    //_renderPropertiesRow: function(
    //分组显示的行
    _renderCategoryRow: function (categoryDisplayName, table) {
        var categoryRow = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "tr"
				}, table
				);

        var categoryCell = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "td",
				    cssClasses: [this._getCategoryDisplayElementStyleName()]
				}, categoryRow
				);

        if (typeof (categoryDisplayName) != "undefined")
            categoryCell.innerText = categoryDisplayName;
    },

    _renderPropertyRows: function (propties, table, colcount) {

        var props = this._sortPropertiesByName(propties);

        var proplength = props.length;

        var cellwidth = String.format("{0}%", 100 / (colcount * 2));
        var doubleCellWidth = String(100 / colcount) + "%";

        for (var i = 0; i < proplength; i = i + colcount) {
            var propertyRow = $HGDomElement.createElementFromTemplate({
                nodeName: "tr"
            }, table);

            for (var j = 0; j < colcount; j++) {
                var itemIndex = j + i;
                if (itemIndex < proplength) {
                    var prop = props[itemIndex];
                    if (this.get_readOnly() == true && prop.readOnly == false)
                        prop.readOnly = true;

                    if (prop.showTitle) {
                        this._renderPropNameCell(prop, propertyRow, cellwidth);
                        this._renderPropValueCell(prop, propertyRow, cellwidth);
                    } else {
                        this._renderPropValueCell(prop, propertyRow, doubleCellWidth, 2);
                    }
                } else {
                    this._renderEmptyCell(propertyRow, cellwidth);
                    this._renderEmptyCell(propertyRow, cellwidth);
                }
            }
        }
    },

    _renderEmptyCell: function (row, cellwidth) {
        var cell = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "td",
				    properties:
					{
					    style:
						{
						    width: cellwidth
						}
					}
				}, row
				);

        cell.innerHTML = "&nbsp;";
    },

    _renderPropNameCell: function (prop, row, cellwidth) {
        var cell = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "td",
				    properties:
					{
					    id: this._getPropertyDisplayElementName(prop),
					    style:
						    {
						        width: cellwidth
						    }
					},
				    cssClasses: [this._getPropertyNameElementStyleName()]
				}, row
		);

        if (this.get_showCheckBoxes()) {
            //			var theId = this._getPropertyDisplayElementName(prop) + "_pptfcb";
            var thisNode = $HGDomElement.createElementFromTemplate(
				{
				    //					id: theId,
				    nodeName: "input",
				    properties:
					{
					    type: 'checkbox'
					}
				},
				cell
			);
            if (thisNode.setAttribute)
                thisNode.setAttribute("data-key", prop.name);
            else
                thisNode['data-key'] = prop.name;

            var pD = this.get_checkBoxStates()._innerArray[prop.name] = new $HGRootNS.PropertyEditor$CheckBoxWrapper(thisNode, prop, this);
            pD.initialize();
            pD = null;
            thisNode = null;
        }

        if (prop.isRequired == true) {
            $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "span",
				    properties:
					{
					    innerText: this._getPropertyDisplayName(prop),
					    title: prop.name
					}
				},
				cell
			);
            $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "span",
				    properties:
					{
					    innerText: "*",
					    style:
						{
						    color: "red"
						}
					}
				},
				cell
			);
        }
        else {
            $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "span",
				    properties:
					{
					    innerText: this._getPropertyDisplayName(prop),
					    title: prop.name
					}
				},
				cell
			);
        }
    },

    _renderPropValueCell: function (prop, row, cellwidth, colSpan) {
        var cell = $HGDomElement.createElementFromTemplate(
            {
                nodeName: "td",
                properties:
				{
				    style:
				    {
				        width: cellwidth
				    }
				},
                cssClasses: [this._getPropertyDisplayElementStyleName()]
            }, row
            );

        if (colSpan)
            cell.colSpan = colSpan;

        var editor;
        if (prop.editorKey) {
            editor = this._get_editorByKey(prop.editorKey, prop, cell, this._delegations);
        } else {
            editor = this._get_editorByType(prop, cell, this._delegations);
        }

        this._propertyEditors[prop.name] = { "editor": editor, "row": row };
        this._propertyEditorsArray.push(editor);

        editor.show();
    },

    // validateData: 

    validateProperties: function (groupID) {
        var isValidate = true;
        var doCompareGroupID = (groupID >= 0);
        //this.errorMessages = new Array();
        for (var i = 0; i < this._properties.length; i++) {

            var property = this._properties[i];

            if (property.visible == true) {

                var editor = this._propertyEditors[property.name].editor;
                if (editor.get_isValid() == false) {
                    isValidate = false;
                    break;
                } else if (property.isRequired && property.value === '') {
                    isValidate = false;
//                    break;
                }

                if (property.hasOwnProperty("clientVdtData")) {
                    var doValidate = true;
                    if (doCompareGroupID)
                        doValidate = (property.clientVdtData.ValidationGroup & groupID) != 0;

                    if (property.clientVdtData.hasOwnProperty("CvtList")) {
                        if (Object.prototype.toString.apply(property.clientVdtData.CvtList) === "[object Array]") {

                            if (this.validateProperty(property, editor) == false && isValidate == true)
                                isValidate = false;
                        }
                    }
                }
            }
        }

        return isValidate;
    },

    validateProperty: function (property, editor) {

        for (var j = 0; j < property.clientVdtData.CvtList.length; j++) {

            var propValid = property.clientVdtData.CvtList[j];

            if (typeof (propValid.ClientValidateMethodName) != "undefined" && propValid.ClientValidateMethodName.length > 0) {

                var va = eval($HGRootNS.ValidatorManager[propValid.ClientValidateMethodName]);

                if (typeof (va) == "undefined")
                    throw Error.create("不能找到" + propValidator.name + "验证类型");
                else {
                    va = new va();

                    if (va.validate(property.value, propValid.AdditionalData) == false) {
                        editor.invalidStyle(propValid.MessageTemplate);
                        //                        this.errorMessages.push(propValid.MessageTemplate);
                        return false;
                    } else {
                        editor.uninvalidStyle();
                        return true;
                    }

                }
            }
        }

        return true;
    },

    //style Name
    _getPropertyDisplayElementStyleName: function () {
        return "";
    },

    _getPropertyNameElementStyleName: function () {
        return "";
    },

    _getCategoryDisplayElementStyleName: function () {
        return "";
    },

    _getDefaultValueDiffStyleName: function () {
        return "";
    },

    _getPropertyDisplayName: function (prop) {
        return prop.displayName != null && prop.displayName != String.Empty ? prop.displayName : prop.name;
    },

    //并properties 排序
    _sortPropertiesByName: function (props) {
        var thisObj = this;

        return props.sort(function (p1, p2) {

            var result = 0;
            if (p1.sortOrder == p2.sortOrder) {
                var p1Name = thisObj._getPropertyDisplayName(p1);
                var p2Name = thisObj._getPropertyDisplayName(p2);

                result = p1Name.localeCompare(p2Name);
            } else {
                if (parseInt(p1.sortOrder) != NaN && parseInt(p2.sortOrder) != NaN) {
                    result = p1.sortOrder - p2.sortOrder;
                } else {
                    result = 1;

                    if (parseInt(p1.sortOrder) != NaN) {
                        result = -1;
                    }
                }
            }

            return result;
        });
    },

    _onEditorClickDelegate: function (prop) {
        if (this._activeEditor != null)
            this._deactivePropertyEditor(this._activeEditor.get_property());

        this._activeEditor = this._propertyEditors[prop.name].editor;
        this._activePropertyEditor(prop);

        this._raiseEvent("clickEditor", prop);
    },

    _onEditorValidatingDelegate: function (prop, ele, e) {
        this._raiseEditorValidateEvent("");
    },

    _onEditorValidateDelegate: function (prop, ele, e) {
        this._raiseEditorValidateEvent("");
    },

    _onBindEditorDropdownList: function (prop, e) {
        this._raiseEditorBindDropdownList(prop, e);
    },

    _raiseEditorValidateEvent: function (eventName, prop, handleElement) {
        var handler = this.get_events().getHandler(eventName);
        if (handler) {
            var e = new Sys.EventArgs();

            e.property = prop;
            e.validateElement = handleElement;
            handler(this, e);
        }
    },

    _onEditorValidatedDelegate: function (prop, ele, e) {
        if (this._activeEditor) {
            this._activeEditor.commitValue();
            this._activeEditor.show();
        }

        if (typeof (prop) != "undefined") {
            if (prop.hasOwnProperty("clientVdtData")) {
                if (prop.clientVdtData.hasOwnProperty("CvtList")) {
                    if (Object.prototype.toString.apply(prop.clientVdtData.CvtList) === "[object Array]") {
                        this.validateProperty(prop, this._activeEditor);
                    }
                }
            }
        }

        this._raiseEditorValidateEvent("editorValidated", prop, ele);
    },

    _onEditorPressDelegate: function (prop) {
    },

    //属性定义
    get_properties: function () {
        return this._properties;
    },

    set_properties: function (value) {
        this._properties = value;
    },

    get_autoSaveClientState: function () {
        return this._autoSaveClientState;
    },

    set_autoSaveClientState: function (value) {
        this._autoSaveClientState = value;
    },

    get_invalidLineImage: function () {
        return this._invalidLineImage;
    },

    set_invalidLineImage: function (value) {
        this._invalidLineImage = value;
    },

    get_readOnly: function () {
        return this._readOnly;
    },

    set_readOnly: function (value) {
        this._readOnly = value;
    },

    get_showCheckBoxes: function () {
        return this._showCheckBoxes;
    },

    set_showCheckBoxes: function (value) {
        this._showCheckBoxes = !!value;
    },

    get_checkBoxStates: function () {
        return this._checkBoxStates;
    },

    get_activeEditor: function () {
        return this._activeEditor;
    },

    // ------------- Events -------------
    _raiseEvent: function (eventName, eventArgs) {
        var handler = this.get_events().getHandler(eventName);

        if (handler)
            handler(this, eventArgs | Sys.EventArgs.Empty);
    },

    _raiseEditorBindDropdownList: function (prop, e) {
        var handler = this.get_events().getHandler("bindEditorDropdownList");
        if (handler) {
            handler(this, e);
        }
    },

    add_enterEditor: function (handler) {
        this.get_events().addHandler('enterEditor', handler);
    },
    remove_enterEditor: function (handler) {
        this.get_events().removeHandler('enterEditor', handler);
    },

    add_clickEditor: function (handler) {
        this.get_events().addHandler('clickEditor', handler);
    },
    remove_clickEditor: function (handler) {
        this.get_events().removeHandler('clickEditor', handler);
    },

    add_editorValidating: function (handler) {
        this.get_events().removeHandler('editorValidating', handler);
    },
    remove_editorValidating: function (handler) {
        this.get_events().removeHandler('editorValidating', handler);
    },

    add_editorValidate: function (handler) {
        this.get_events().addHandler('editorValidate', handler);
    },
    remove_validateEditor: function (handler) {
        this.get_events().removeHandler('editorValidate', handler);
    },

    add_editorValidated: function (handler) {
        this.get_events().addHandler('editorValidated', handler);
    },
    remove_editorValidated: function () {
        this.get_events().removeHandler('editorValidated', handler);
    },

    add_bindEditorDropdownList: function (handler) {
        this.get_events().addHandler('bindEditorDropdownList', handler);
    },
    remove_bindEditorDropdownList: function (handler) {
        this.get_events().removeHandler('bindEditorDropdownList', handler);
    }
}

$HGRootNS.PropertyEditorControlBase.ValidatePropertiesByGroup = function (groupID) {
    var isValidate = true;
    for (var controlID in Sys.Application._components) {
        var cValidate = true;

        if (Object.prototype.toString.apply(Sys.Application._components[controlID].validateProperties) === "[object Function]") {
            cValidate = Sys.Application._components[controlID].validateProperties(groupID);

            if (cValidate == false) {
                isValidate = false;
                //                for (var i = 0; i < Sys.Application._components[controlID].errorMessages.length; i++) {
                //                    allErrorMessages.push(Sys.Application._components[controlID].errorMessages[i]);
                //                    isValidate = false;
                //                }
            }
        }
    }

    return { isValid: isValidate };
}

$HGRootNS.PropertyEditorControlBase.ValidateProperties = function (val, args) {
    var result = $HGRootNS.PropertyEditorControlBase.ValidatePropertiesByGroup();

    if (args) {
        //        if (!result.isValid) {
        //            $HGClientMsg.stop(result.errorMessages.join('<br/>'), '', $NT.getText("SOAWebControls", "输入非法"));
        //        }
        args.IsValid = result.isValid;
    }

    return result;
}

$HGRootNS.PropertyEditorControlBase.registerClass($HGRootNSName + ".PropertyEditorControlBase", $HGRootNS.ControlBase);

$HGRootNS.StandardPropertyEditor = function (prop, container, delegations) {
    this._property = prop;
    this._container = container;
    this._isValid = true;
    this._delegations = delegations;

    if (prop.readOnly == true) {
        this._editElement = this._createReadOnlyElement();
    } else {
        this._editElement = this._createEditElement();
    }

    this._editElement$delegate = {
        focus: Function.createDelegate(this, this._editElement_onfocus),
        blur: Function.createDelegate(this, this._editElement_onblur),
        change: Function.createDelegate(this, this._editElement_onchange),
        keypress: Function.createDelegate(this, this._editElement_onkeypress)
    }

    if (this._editElement != null) {
        $addHandlers(this._editElement, this._editElement$delegate);
    }
}

$HGRootNS.StandardPropertyEditor.prototype =
{
    get_property: function () {
        return this._property;
    },

    get_container: function () {
        return this._container;
    },

    get_isValid: function () {
        return this._isValid;
    },
    //added by randy
    get_editElement: function () {
        return this._editElement;
    },

    get_currentEditorParams: function () {
        var result = null;
        if (typeof (this.get_property().editorParams) != "undefined") {
            if (this.get_property().editorParams != null || this.get_property().editorParams != "") {
                try {
                    result = Sys.Serialization.JavaScriptSerializer.deserialize(this.get_property().editorParams)
                } catch (e) {
                    result = this.get_property().editorParams;
                }
            }
        }

        return result;
    },

    _editElement_onfocus: function (eventElement) {
        var obj = eventElement.handlingElement;
        if (obj.createTextRange) {
            var range = obj.createTextRange();
            range.move("character", obj.value.length);
            range.collapse(true);
            range.select();
        }

        this._delegations["editorEnter"](this.get_property());
    },

    _isInteger: function (value) {
        var pattern = /^\d+$/;
        return pattern.test(value);
    },

    _editElement_onblur: function (eventElement) {
        this._delegations["editorLeave"](this.get_property());
    },

    _editElement_onchange: function (eventElement) {
        var validateEventArgs = new Sys.EventArgs();
        validateEventArgs.result = true;
        if (this._delegations["editorValidating"]) {
            this._delegations["editorValidating"](this.get_property(), eventElement.handlingElement);
            if (this.get_property().dataType == $HGRootNS.PropertyDataType.Integer) {
                //                if (!this._isInteger(eventElement.handlingElement.value)) {
                //                    alert("input error");
                //                    eventElement.preventDefault()
                //                    return;
                //                }
            }
            if (this._delegations["editorValidate"]) {
                this._delegations["editorValidate"](this.get_property(), eventElement.handlingElement, validateEventArgs);
            }
            if (validateEventArgs.result == false) {
                eventElement.preventDefault();
                return false;
            }
        }
        if (this._delegations["editorValidated"]) {
            this._delegations["editorValidated"](this.get_property(), eventElement.handlingElement, validateEventArgs);
        }
    },

    _editElement_onkeypress: function (eventElement) {
        //data type is integer
        if (this.get_property().dataType == $HGRootNS.PropertyDataType.Integer) {
            if (eventElement.charCode < 45 || eventElement.charCode > 57) {
                eventElement.preventDefault();
            }
        }
        if (eventElement.charCode == 13) {
            var validateEventArgs = new Sys.EventArgs();
            validateEventArgs.result = true;
            if (this._delegations["editorValidating"]) {
                this._delegations["editorValidating"](this.get_property(), eventElement.handlingElement);
                if (this.get_property().dataType == $HGRootNS.PropertyDataType.Integer) {
                    if (!this._isInteger(eventElement.handlingElement.value)) {
                        alert("input error");
                        eventElement.preventDefault()
                        return;
                    }
                }
                if (this._delegations["editorValidate"]) {
                    this._delegations["editorValidate"](this.get_property(), eventElement.handlingElement, validateEventArgs);
                }
                if (validateEventArgs.result == false) {
                    eventElement.preventDefault();
                    return false;
                }
            }
            if (this._delegations["editorValidated"]) {
                this._delegations["editorValidated"](this.get_property(), eventElement.handlingElement, validateEventArgs);
            }
            if (this._delegations["editorEnterPress"]) {
                this._delegations["editorEnterPress"]();
            }
        }
        this._changeFormatStyle();
    },

    commitValue: function () {
        if (this._editElement)
            this.get_property().value = this._editElement.value;
    },

    _createEditElement: function () {
        this.get_container().innerHTML = "";

        var inputContainer = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "div",
				    cssClass: ["ajax__propertyGrid_input_container"]
				}, this.get_container()
				);
        var css = ["ajax__propertyGrid_input",
			this.get_property().dataType == $HGRootNS.PropertyDataType.Integer ? "ajax__propertyGrid_input_alignRight" : "ajax__propertyGrid_input_alignLeft"];

        var htmlDomElement = $HGDomElement.createElementFromTemplate(
			{
			    nodeName: "input",
			    properties:
				{
				    value: ""
				},
			    cssClasses: css
			}, inputContainer
			);

        if (this.get_property().maxLength > 0)
            htmlDomElement.maxLength = this.get_property().maxLength.toString();

        return htmlDomElement;
    },

    _createReadOnlyElement: function () {

        this.get_container().innerHTML = "";

        var propValue = !(this.get_property().value) ? this.get_property().defaultValue : this.get_property().value;

        var htmlDomElement = $HGDomElement.createElementFromTemplate(
						{
						    nodeName: "span",
						    properties:
							{
							    style:
								{
								    //color: "#8B8B83"
								}
							}
						}, this.get_container());

        if (propValue)
            htmlDomElement.innerText = propValue.toString();

        Sys.UI.DomElement.addCssClass(htmlDomElement, "ajax__propertyEditor_readOnlyStyle");

        return htmlDomElement;
    },

    _formatText: function (inputSpan) {
        if (this.get_property().value) {
            inputSpan.innerText = this.get_property().value;
        }
    },

    _getPropertyDisplayElementID: function () {
        return this._delegations._owner._getPropertyDisplayElementName(this.get_property());
    },

    _getChangeFromatStyleName: function () {
        return this._delegations._owner._getDefaultValueDiffStyleName();
    },

    //根据条件改变是否加粗名称单元格
    _changeDisplayNameElementStyle: function (ischange) {
        var styleName = this._getChangeFromatStyleName();
        var changeElementID = this._getPropertyDisplayElementID();
        var changeElement = $get(changeElementID);

        if (ischange) {
            if (Sys.UI.DomElement.containsCssClass(changeElement, styleName) == false) {
                Sys.UI.DomElement.addCssClass(changeElement, styleName);
            }
        } else {
            if (Sys.UI.DomElement.containsCssClass(changeElement, styleName)) {
                Sys.UI.DomElement.removeCssClass(changeElement, styleName);
            }
        }
    },

    _changeEditElementStyle: function (ischange) {
        var styleName = this._getChangeFromatStyleName();
        if (ischange) {
            if (Sys.UI.DomElement.containsCssClass(this._editElement, styleName) == false) {
                Sys.UI.DomElement.addCssClass(this._editElement, styleName);
            }
        } else {
            if (Sys.UI.DomElement.containsCssClass(this._editElement, styleName)) {
                Sys.UI.DomElement.removeCssClass(this._editElement, styleName);
            }
        }
    },

    get_invalidStyleName: function () {
        return "ajax__property_invalidStyle";
    },

    invalidStyle: function (strMessage) {
        var strStyleName = this.get_invalidStyleName();
        if (Sys.UI.DomElement.containsCssClass(this._editElement, strStyleName) == false) {
            Sys.UI.DomElement.addCssClass(this._editElement, strStyleName);
            this._editElement.title = strMessage;
        }
    },

    uninvalidStyle: function () {
        var strStyleName = this.get_invalidStyleName();
        if (Sys.UI.DomElement.containsCssClass(this._editElement, strStyleName)) {
            Sys.UI.DomElement.removeCssClass(this._editElement, strStyleName);
            this._editElement.title = "";
        }
    },

    show: function () {
        this._formatText(this._editElement);
        this._changeFormatStyle();
    },

    checkIsChangeStyle: function () {
        var item = this.get_property();

        var isChange;
        if (typeof (item.defaultValue) == "undefined" && typeof (item.value) == "undefined") {
            isChange = false;
        } else if ((typeof (item.defaultValue) == "undefined" || typeof (item.value) == "undefined") && (item.value == "" || item.defaultValue == "")) {
            isChange = false;
        } else {
            isChange = (item.value == item.defaultValue) ? false : true;
        }

        return isChange;
    },

    _changeFormatStyle: function () {
        var isChange = this.checkIsChangeStyle();

        if (this._property.showTitle)
            this._changeDisplayNameElementStyle(isChange);

        this._changeEditElementStyle(isChange);
    },

    edit: function () {

    }, get_enabled: function () {
        if (this._editElement)
            return !this._editElement.disabled;
        return false;
    }, set_enabled: function (val) {
        if (this._editElement)
            return this._editElement.disabled = !val;
    },

    _disposeEditElement: function () {
        if (this._editElement) {
            $HGDomEvent.removeHandlers(this._editElement, this._editElement$delegate);
            this._editElement = null;
        }
    },

    dispose: function () {
        if (this.get_property().readOnly == false) {
            this._disposeEditElement();
        }

        this._editElement = null;
        this._property = null;
        this._container = null;
        this._delegations = null;
    },

    pseudo: function () {
    }
}

$HGRootNS.StandardPropertyEditor.registerClass($HGRootNSName + ".StandardPropertyEditor", null, Sys.IDisposable);

$HGRootNS.BooleanPropertyEditor = function (prop, container, delegations) {
    this.dropDownListSelectChange$delegate = {
        change: Function.createDelegate(this, this._dropDownListSelectChange)
    };
    $HGRootNS.BooleanPropertyEditor.initializeBase(this, [prop, container, delegations]);
    $addHandlers(this._editElement, this._editElement$delegate);
}

$HGRootNS.BooleanPropertyEditor.prototype =
{
    _createEditElement: function () {
        var dropDownList;
        dropDownList = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "select"
					,
				    properties:
					{
					    style:
						{
						    width: "100%"
						}
					}
				}, this.get_container()
				);

        var boolEnum = [];
        boolEnum.push({ value: "true", text: "True" });
        boolEnum.push({ value: "false", text: "False" });

        for (var index = 0; index < boolEnum.length; index++) {
            var item = boolEnum[index];
            var option = $HGDomElement.addSelectOption(dropDownList, item.text, item.value);
        }

        var propValue = !(this.get_property().value) ? this.get_property().defaultValue : this.get_property().value;

        for (var i = 0; i < dropDownList.options.length; i++) {
            if (dropDownList.options[i].value.toString().toLowerCase() == propValue.toString().toLowerCase()) {
                dropDownList.options[i].selected = true;
                break;
            }
        }

        $addHandlers(dropDownList, this.dropDownListSelectChange$delegate);

        return dropDownList;
    },

    _createReadOnlyElement: function () {
        this.get_container().innerHTML = "";

        var propValue = !(this.get_property().value) ? this.get_property().defaultValue : this.get_property().value;

        var htmlDomElement = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "span",
				    properties:
					{
					    style:
						{
						    color: "#8B8B83"
						}
					}
				}, this.get_container());

        if (propValue)
            htmlDomElement.innerText = propValue.toString(); ;

        return htmlDomElement;
    },

    checkIsChangeStyle: function () {
        var item = this.get_property();
        var isChange;

        if ((item.defaultValue == null || item.defaultValue == undefined || item.defaultValue == "") && (item.value == null || item.value == undefined || item.value == "")) {
            isChange = false;
        }
        else {
            if (Object.prototype.toString.apply(item.defaultValue) === "[object String]") {
                item.defaultValue = Boolean.parse(item.defaultValue);
            }
            if (Object.prototype.toString.apply(item.value) === "[object String]") {
                item.value = Boolean.parse(item.value);
            }

            isChange = (item.value == item.defaultValue) ? false : true;
        }

        return isChange;
    },

    show: function () {
        var isChange = this.checkIsChangeStyle();

        this._changeDisplayNameElementStyle(isChange);
    },

    _dropDownListSelectChange: function (sender) {
        this.get_property().value = sender.target.value;
        var isChange = this.checkIsChangeStyle();

        this._changeDisplayNameElementStyle(isChange);
    }
}
$HGRootNS.BooleanPropertyEditor.registerClass($HGRootNSName + ".BooleanPropertyEditor", $HGRootNS.StandardPropertyEditor);

$HGRootNS.ObjectPropertyEditor = function (prop, container, delegations) {
    this.openObjectEditor$delegate = {
        click: Function.createDelegate(this, this._onOpenObjectEditorClick)
    };
    this._valueInfoElement = null;
    this._editorBtn = null;
    $HGRootNS.ObjectPropertyEditor.initializeBase(this, [prop, container, delegations])
}

$HGRootNS.ObjectPropertyEditor.prototype =
{
    _createOperationElement: function (parentcontainer) {
        var buttonElement = $HGDomElement.createElementFromTemplate(
					{
					    nodeName: "input",
					    properties:
					{
					    type: "button",
					    value: "...",
					    cssClasses: ["formButton"],
					    style:
						{
						    width: "23px",
						    height: "22px",
						    cursor: "hand"
						}
					}
					}, parentcontainer);

        $addHandlers(buttonElement, this.openObjectEditor$delegate);
        return buttonElement;
    },

    _createViewValueInfoElement: function (parentcontainer) {
        var label = $HGDomElement.createElementFromTemplate(
			{
			    nodeName: "label",
			    cssClasses: ["ajax__propertyGrid_valueobject_lable"]
			}, parentcontainer
			);
        return label;
    },

    _createEditElement: function () {
        var htmlDomElement;
        htmlDomElement = $HGDomElement.createElementFromTemplate(
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
			}, this.get_container()
		);
        var row = $HGDomElement.createElementFromTemplate(
			{
			    nodeName: "tr"
			}, htmlDomElement
			);
        var leftCell = $HGDomElement.createElementFromTemplate(
			{
			    nodeName: "td",
			    properties:
				{
				    style:
				    {
				        width: "100%"
				    }
				}
			}, row
			);

        this._valueInfoElement = this._createViewValueInfoElement(leftCell);

        var rightCell = $HGDomElement.createElementFromTemplate(
			{
			    nodeName: "td",
			    cssClasses: ["ajax__propertyGrid_Editor_valueCell"]
			}, row
			);

        this._editorBtn = this._createOperationElement(rightCell);

        return this._valueInfoElement;
    },

    _onOpenObjectEditorClick: function () {
        if (this._delegations["editorClick"]) {
            this._delegations["editorClick"](this.get_property());
        }
    },

    commitValue: function (value) {
        var formattedValue = value;
        var propertyValue = value;

        if (typeof (value) == "undefined" || value == null || value == "") {
            formattedValue = "";
            if (Object.prototype.toString.apply(value) != "[object Array]") {
                propertyValue = "";
            }
        }
        else {
            formattedValue = this.getNeedToFormatValue(value);
            propertyValue = this.getPropertyValue(value);
        }

        this.get_property().value = propertyValue;
        this.formatText(formattedValue);
    },

    //value指定为object类型
    formatText: function (value) {
        if (this.get_property().readOnly == false) {
            if (typeof (this._valueInfoElement.innerText) != "undefined") {
                this._valueInfoElement.innerText = value;
                this._valueInfoElement.title = value;
            }
        }
    },

    getNeedToFormatValue: function (value) {
        var propertyFormatValue = value;
        if ((typeof (value) == "string" || value.constructor == String) && value != "") {
            propertyFormatValue = Sys.Serialization.JavaScriptSerializer.deserialize(value);
        }

        return propertyFormatValue;
    },

    getPropertyValue: function (value) {
        var propertyValue = value;

        if (typeof (value) == "object" || value.constructor == Array) {
            propertyValue = Sys.Serialization.JavaScriptSerializer.serialize(value);
        }

        return propertyValue;
    },

    show: function () {
        this.commitValue(this.get_property().value);
    },

    checkIsChangeStyle: function () {
        var item = this.get_property();
        var isChange;

        if (Object.prototype.toString.apply(item.value) === "[object Array]") {
            if (item.value.length == 0) {
                isChange = false;
            }
            else {
                isChange = true;
            }
        } else if (Object.prototype.toString.apply(item.value) === "[object String]") {
            if (item.value != "") {
                var arrlist = Sys.Serialization.JavaScriptSerializer.deserialize(item.value);
                if (arrlist) {
                    if (arrlist.length > 0) {
                        isChange = true;
                    }
                }
            } else {
                isChange = false;
            }

        } else {
            isChange = (item.value == item.defaultValue) ? false : true;
        }

        return isChange;
    },

    _changeEditElementStyle: function (ischange) {
        var styleName = this._getChangeFromatStyleName();
        if (ischange) {
            if (Sys.UI.DomElement.containsCssClass(this._valueInfoElement, styleName) == false) {
                Sys.UI.DomElement.addCssClass(this._valueInfoElement, styleName);
            }
        } else {
            if (Sys.UI.DomElement.containsCssClass(this._valueInfoElement, styleName)) {
                Sys.UI.DomElement.removeCssClass(this._valueInfoElement, styleName);
            }
        }
    },

    dispose: function () {
        if (this._editorBtn)
            $HGDomEvent.removeHandlers(this._editorBtn, this.openObjectEditor$delegate);
        $HGRootNS.ObjectPropertyEditor.callBaseMethod(this, 'dispose');
    }
}

$HGRootNS.ObjectPropertyEditor.registerClass($HGRootNSName + ".ObjectPropertyEditor", $HGRootNS.StandardPropertyEditor);

$HGRootNS.EnumPropertyEditor = function (prop, container, delegations) {
    this.dropDownListSelectChange$delegate = {
        change: Function.createDelegate(this, this._dropDownListSelectChange)
    };
    $HGRootNS.EnumPropertyEditor.initializeBase(this, [prop, container, delegations]);
    $addHandlers(this._editElement, this._editElement$delegate);
}

$HGRootNS.EnumPropertyEditor.prototype =
{
    _createEditElement: function () {
        var dropDownList = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "select",
				    properties:
					{
					    style:
						{
						    width: "100%"
						}
					}
				}, this.get_container()
				);

        this._bindEditorDropdownSource(dropDownList);

        $addHandlers(dropDownList, this.dropDownListSelectChange$delegate);

        return dropDownList;
    },

    _createReadOnlyElement: function () {
        this.get_container().innerHTML = "";

        var inputContainer = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "div"
					,
				    cssClass: ["ajax__propertyGrid_input_container"]
				}, this.get_container()
				);
        var css = ["ajax__propertyGrid_input",
			this.get_property().dataType == $HGRootNS.PropertyDataType.Integer ? "ajax__propertyGrid_input_alignRight" : "ajax__propertyGrid_input_alignLeft"];

        var htmlDomElement = $HGDomElement.createElementFromTemplate(
			{
			    nodeName: "lable",
			    cssClasses: css
			}, inputContainer
			);
        htmlDomElement.style.color = "#8B8B83";
        var enumList = this._getDownlistSource();

        if (enumList != null) {
            var propValue = !(this.get_property().value) ? this.get_property().defaultValue : this.get_property().value;
            for (var i = 0; i < enumList.length; i++) {
                var item = enumList[i];
                if (item.value == propValue) {
                    htmlDomElement.innerText = item.text;
                }
            }
        }

        return htmlDomElement;
    },

    _bindEditorDropdownSource: function (dropDownList) {

        dropDownList.options.length = 0;

        var enumList = this._getDownlistSource();

        if (enumList != null) {

            var propValue = (this.get_property().value == undefined) ? this.get_property().defaultValue : ((this.get_property().value == "" || this.get_property().value == "") && this.get_property().defaultValue) ? this.get_property().defaultValue : this.get_property().value;
            var flag = false;
            for (var i = 0; i < enumList.length; i++) {
                var item = enumList[i];
                var oOption = $HGDomElement.addSelectOption(dropDownList, item.text, item.value);
                if (item.value == propValue) {
                    oOption.selected = true;
                    flag = true;
                }
            }

            /*
            for (var j = 0; j < dropDownList.options.length; j++) {
            if (dropDownList.options[j].value == propValue) {
            dropDownList.options[j].selected = true;
            flag = true;
            break;
            }
            } */

            if (flag == false && enumList.length > 0) {
                dropDownList.options[0].selected = true;
                this.get_property().value == enumList[0].value;
            }
        }
    },

    _getDownlistSource: function () {

        var editorParam = this.get_currentEditorParams();

        var e = new Sys.EventArgs();
        e.property = this.get_property();
        if (editorParam != null) {
            if (typeof (editorParam) === "object") {
                if (editorParam.hasOwnProperty("enumTypeName")) {
                    e.enumDesc = this._delegations._owner._findPredefinedEnumDescription(editorParam.enumTypeName);
                } else if (editorParam.hasOwnProperty("dropDownDataSourceID")) {
                    e.enumDesc = this._delegations._owner._findPredefinedEnumDescription(editorParam.dropDownDataSourceID);
                }
            } else {
                e.enumDesc = this._delegations._owner._findPredefinedEnumDescription(editorParam);
            }
        }

        if (this._delegations["bindEditorDropdownList"]) {
            this._delegations["bindEditorDropdownList"](this.get_property(), e);
        }

        return e.enumDesc ? e.enumDesc : null;
    },

    _dropDownListSelectChange: function (sender) {
        this.get_property().value = sender.target.value;

        var isChange = (sender.target.value == this.get_property().defaultValue) ? false : true;

        this._changeDisplayNameElementStyle(isChange);

        var editorParam = this.get_currentEditorParams();
        if (editorParam != null) {
            if (typeof (editorParam) === "object") {

                if (editorParam.hasOwnProperty("childKeys")) {
                    if (editorParam.childKeys != null) {
                        var childs = editorParam.childKeys.split(",");

                        for (var i = 0; i < childs.length; i++) {
                            var childEditor = this._delegations._owner._propertyEditors[childs[i]];
                            if (childEditor) {
                                if (childEditor.editor.get_property().dataType == $HGRootNS.PropertyDataType.Enum || childEditor.editor.get_property().editorKey == "EnumPropertyEditor") {
                                    var element = childEditor.editor._editElement;
                                    childEditor.editor._bindEditorDropdownSource(element);

                                    element.fireEvent("onchange");
                                }
                            }
                        }
                    }
                }
            }
        }
    },

    show: function () {
        var isChange = (this.get_property().value == this.get_property().defaultValue) ? false : true;

        this._changeDisplayNameElementStyle(isChange);
    }
}

$HGRootNS.EnumPropertyEditor.registerClass($HGRootNSName + ".EnumPropertyEditor", $HGRootNS.StandardPropertyEditor);

$HGRootNS.DatePropertyEditor = function (prop, container, delegations) {

    this._dateInputElement = null;
    this._dateControl = null;
    $HGRootNS.DatePropertyEditor.initializeBase(this, [prop, container, delegations]);

    $addHandlers(this._editElement, this._editElement$delegate);
}

$HGRootNS.DatePropertyEditor.prototype =
{
    get_generalCalendarID: function () {
        //取到预先下去的日历牌ID，然后找它的日历牌的按钮图片
        return "DatePropertyEditor_DeluxeCalendar";
    },

    _createEditElement: function () {
        var template = $find(this.get_generalCalendarID());
        var calendar = template.cloneAndAppendToContainer(this.get_container());

        if (this.get_property().value != "") {
            var value = Date.parse(this.get_property().value.replace(/-/g, "/"));
            if (value != -2177481600000) {
                var currentDate = new Date(value);
                calendar.set_DateValue(currentDate);
            }
        }

        //日期发生变化事件
        calendar.add_clientValueChanged(Function.createDelegate(this, this._calendarDateTimeValueChanged)); //calendar里头抛出的事件
        this._dateInputElement = calendar.get_element();
        this._dateControl = calendar;

        this._changeFormatStyle();

        return this._dateInputElement;
    },

    _createReadOnlyElement: function () {
        var template = $find(this.get_generalCalendarID());
        var calendar = template.cloneAndAppendToContainer(this.get_container());

        if (this.get_property().value != "") {
            var value = Date.parse(this.get_property().value.replace(/-/g, "/"));
            if (value != -2177481600000) {
                var currentDate = new Date(value);
                calendar.set_DateValue(currentDate);
            }
        }
        calendar.set_readOnly(true);
        this._dateInputElement = calendar.get_element();

        return this._dateInputElement;
    },

    _calendarDateTimeValueChanged: function (sender) {
        if (sender.get_value() != Date.minDate)
            this.get_property().value = String.format("{0:yyyy-MM-dd}", sender.get_value());
        else
            this.get_property().value = '';

        this._changeFormatStyle();
    },

    _formatText: function (element) {

    },

    checkIsChangeStyle: function () {
        var item = this.get_property();
        var isChange;

        if (typeof (this.get_property().defaultValue) == "undefined" && typeof (this.get_property().value) == "undefined") {
            isChange = false;
        } else if ((typeof (this.get_property().defaultValue) == "undefined" || typeof (this.get_property().value) == "undefined") && ((this.get_property().value == "") || (this.get_property().defaultValue == ""))) {
            isChange = false;
        } else if (Object.prototype.toString.apply(item.value) === "[object String]") {
            var date = new Date(Date.parse(item.value.replace(/-/g, "/")));

            if (isNaN(date)) {
                isChange = false;
            } else {
                isChange = true;
            }
        } else if (Object.prototype.toString.apply(item.value) === "[object Date]") {
            isChange = (item.value == this.get_calendarDefaultValue()) ? false : true;
        }

        return isChange;
    },

    get_calendarDefaultValue: function () {
        var result = Date.minDate;
        if (typeof (this.get_property().defaultValue) != "undefined") {
            var date = new Date(Date.parse(this.get_property().defaultValue.replace(/-/g, "/")));

            if (isNaN(date) == false)
                result = date;
        }

        return result;
    },

    checkDate: function (date) {
        var isChange;
        if (item.value == Date.minDate) {
            isChange = true;
        }
        else if ((item.defaultValue == null || item.defaultValue == undefined || item.defaultValue == "") && (item.value != null || item.value != undefined || item.value != "")) {
            isChange = false;
        } else {
            isChange = (item.value == item.defaultValue) ? true : false;
        }

        return isChange;
    },

    get_enabled: function () {
        if (this._dateControl)
            return !this._dateControl.get_readOnly();
        return false;
    },

    set_enabled: function (val) {
        if (this._dateControl)
            this._dateControl.set_readOnly(!val);
    },

    commitValue: function () {

    },

    _changeEditElementStyle: function (ischange) {
        var styleName = this._getChangeFromatStyleName();
        if (ischange) {
            if (Sys.UI.DomElement.containsCssClass(this._dateInputElement, styleName) == false) {
                Sys.UI.DomElement.addCssClass(this._dateInputElement, styleName);
            }
        } else {
            if (Sys.UI.DomElement.containsCssClass(this._dateInputElement, styleName)) {
                Sys.UI.DomElement.removeCssClass(this._dateInputElement, styleName);
            }
        }
    }
}

$HGRootNS.DatePropertyEditor.registerClass($HGRootNSName + ".DatePropertyEditor", $HGRootNS.StandardPropertyEditor);

$HGRootNS.DateTimePropertyEditor = function (prop, container, delegations) {

    this._dateTimeContolElement = null;

    $HGRootNS.DateTimePropertyEditor.initializeBase(this, [prop, container, delegations]);

    this._editElement$delegate.click = Function.createDelegate(this, this._editElement_onfocus);

    $addHandlers(this._editElement, this._editElement$delegate);
}

$HGRootNS.DateTimePropertyEditor.prototype =
{
    get_generalDateTimeControlID: function () {
        //取到预先下去的日历牌ID，然后找它的日历牌的按钮图片
        return "DateTimePropertyEditor_DeluxeDateTime";
    },

    _createEditElement: function () {

        var template = $find(this.get_generalDateTimeControlID());
        var dateTimeContol = template.cloneAndAppendToContainer(this.get_container());

        dateTimeContol.set_DateTimeValue(Date.parseInvariant(this.get_property().value, "yyyy-MM-dd HH:mm:ss"));

        //日期发生变化事件
        dateTimeContol.add_clientValueChanged(Function.createDelegate(this, this._dateTimeSpanValueChange)); //calendar里头抛出的事件

        this._dateTimeContolElement = dateTimeContol;

        return dateTimeContol.get_element();
    },

    _createReadOnlyElement: function () {
        var template = $find(this.get_generalDateTimeControlID());
        var dateTimeContol = template.cloneAndAppendToContainer(this.get_container());

        dateTimeContol.set_DateTimeValue(Date.parseInvariant(this.get_property().value, "yyyy-MM-dd HH:mm:ss"));
        dateTimeContol.set_ReadOnly(true);
        this._dateTimeContolElement = dateTimeContol;

        return dateTimeContol.get_element();
    },

    _dateTimeSpanValueChange: function (sender) {
        if (sender.get_value() != Date.minDate)
            this.get_property().value = String.format("{0:yyyy-MM-dd HH:mm:ss}", sender.get_value());
        else
            this.get_property().value = '';

        this._changeFormatStyle();
    },

    show: function () {
        this._changeFormatStyle();
    }, get_enabled: function () {
        if (this._dateTimeContolElement)
            return !this._dateTimeContolElement.get_readOnly();
        return false;
    }, set_enabled: function (val) {
        if (this._dateTimeContolElement)
            this._dateTimeContolElement.set_readOnly(!val);
    },
    commitValue: function () {

    },

    _changeEditElementStyle: function (ischange) {
        var styleName = this._getChangeFromatStyleName();
        if (ischange) {
            if (Sys.UI.DomElement.containsCssClass(this._dateTimeContolElement.get_timeControl().get_element(), styleName) == false) {
                Sys.UI.DomElement.addCssClass(this._dateTimeContolElement.get_timeControl().get_element(), styleName);
            }
            if (Sys.UI.DomElement.containsCssClass(this._dateTimeContolElement.get_calendarControl().get_element(), styleName) == false) {
                Sys.UI.DomElement.addCssClass(this._dateTimeContolElement.get_calendarControl().get_element(), styleName);
            }
        } else {
            if (Sys.UI.DomElement.containsCssClass(this._dateTimeContolElement.get_timeControl().get_element(), styleName) == true) {
                Sys.UI.DomElement.removeCssClass(this._dateTimeContolElement.get_timeControl().get_element(), styleName);
            }
            if (Sys.UI.DomElement.containsCssClass(this._dateTimeContolElement.get_calendarControl().get_element(), styleName) == true) {
                Sys.UI.DomElement.removeCssClass(this._dateTimeContolElement.get_calendarControl().get_element(), styleName);
            }
        }
    }
}

$HGRootNS.DateTimePropertyEditor.registerClass($HGRootNSName + ".DateTimePropertyEditor", $HGRootNS.StandardPropertyEditor);

$HGRootNS.ImageUploaderPropertyEditor = function (prop, container, delegations) {

    $HGRootNS.ImageUploaderPropertyEditor.initializeBase(this, [prop, container, delegations]);

    this._editElement$delegate.click = Function.createDelegate(this, this._editElement_onfocus);

    $addHandlers(this._editElement, this._editElement$delegate);
}

$HGRootNS.ImageUploaderPropertyEditor.prototype =
{
    get_generalImageUploaderControlID: function () {
        var para = this.get_currentEditorParams();
        var cloneControlID = "ImageUploaderPropertyEditor_ImageUploader";
        if (para) {
            if (para.cloneControlID) {
                cloneControlID = para.cloneControlID;
            }
        }
        return cloneControlID;
    },

    _getFormatValue: function (value) {
        var propertyFormatValue = value;
        if ((typeof (value) == "string" || value.constructor == String) && value != "") {
            propertyFormatValue = Sys.Serialization.JavaScriptSerializer.deserialize(value);
        }

        return propertyFormatValue;
    },

    _createReadOnlyElement: function () {
        var templateContol = $find(this.get_generalImageUploaderControlID());

        var img = document.createElement("img");

        var curValue = this.get_property().value;
        if (curValue != "") {
            var imgProp = Sys.Serialization.JavaScriptSerializer.deserialize(curValue);
            var imageID = "", filePath = "";
            imageID = imgProp.ID;
            filePath = imgProp.FilePath;

            img.src = templateContol.getImageUrl(imageID, filePath);
            img.style.width = "98%";
            img.style.height = "98%";
        }
        else {
            img.src = templateContol.getImageUrl("");
            img.style.width = "98%";
            img.style.height = "98%";
        }

        this.get_container().appendChild(img);

        return img;
    },

    _createEditElement: function () {
        var template = $find(this.get_generalImageUploaderControlID());
        var imageUploaderContol = template.cloneAndAppendToContainer(this.get_container());

        var curValue = this.get_property().value;
        if (curValue != "") {
            var imgProp = Sys.Serialization.JavaScriptSerializer.deserialize(curValue);
            var imageID = "", filePath = "";

            imageID = imgProp.ID;
            filePath = imgProp.FilePath;

            imageUploaderContol.showImage(imageID, filePath);
        }

        //图片上传后发生
        imageUploaderContol.add_clientImageUploaded(Function.createDelegate(this, this._imageValueChange));
        imageUploaderContol.add_clientImageDeleted(Function.createDelegate(this, this._onImageDeleted));

        return imageUploaderContol.get_element();
    },

    _imageValueChange: function (sender, e) {
        if (e.IsSuccess) {
            this.commitValue(e.ImgProJsonStr);
        }
        this._changeFormatStyle();
    },

    _onImageDeleted: function () {
        this.commitValue("");
        this._changeFormatStyle();
    },

    show: function () {
        this._changeFormatStyle();
    },

    commitValue: function (value) {
        this.get_property().value = value;
    }
}

$HGRootNS.ImageUploaderPropertyEditor.registerClass($HGRootNSName + ".ImageUploaderPropertyEditor", $HGRootNS.StandardPropertyEditor);

$HGRootNS.ImageUploaderPropertyEditorForGrid = function (prop, container, delegations) {
    $HGRootNS.ImageUploaderPropertyEditorForGrid.initializeBase(this, [prop, container, delegations]);
}

$HGRootNS.ImageUploaderPropertyEditorForGrid.prototype =
{
    get_ImageUploaderDialogControlID: function () {
        return "ImageUploaderPropertyEditorForGrid_ImageUploaderDialog";
    },

    get_ImageUploaderControlID: function () {
        return "ImageUploaderPropertyEditorForGrid_ImageUploader";
    },

    _resetImageIcon: function () {
        var curValue = this.get_property().value;
        var imageID = "", filePath = "";
        if (curValue != "") {
            var imgProp = Sys.Serialization.JavaScriptSerializer.deserialize(curValue);
            imageID = imgProp.ID;
            filePath = imgProp.FilePath;
        }

        var imgUrl = $find(this.get_ImageUploaderControlID()).getImageUrl(imageID, filePath);
        this._valueInfoElement.src = imgUrl;
    },

    _createReadOnlyElement: function () {
        var template = $find(this.get_ImageUploaderControlID());

        var img = document.createElement("img");
        img.className = "ajax__propertyGrid_valueobject_image";
        img.width = 25;
        img.height = 25;

        var curValue = this.get_property().value;
        if (curValue != "") {
            var imgProp = Sys.Serialization.JavaScriptSerializer.deserialize(curValue);
            var imageID = "", filePath = "";
            imageID = imgProp.ID;
            filePath = imgProp.FilePath;
            img.src = template.getImageUrl(imageID, filePath);
        }
        else {
            img.src = template.getImageUrl("");
        }

        this.get_container().appendChild(img);
        return img;
    },

    _createViewValueInfoElement: function (parentcontainer) {
        var curValue = this.get_property().value;
        var imageID = "", filePath = "";
        if (curValue != "") {
            var imgProp = Sys.Serialization.JavaScriptSerializer.deserialize(curValue);
            imageID = imgProp.ID;
            filePath = imgProp.FilePath;
        }

        var imgUrl = $find(this.get_ImageUploaderControlID()).getImageUrl(imageID, filePath);

        var image = $HGDomElement.createElementFromTemplate(
			{
			    nodeName: "img",
			    cssClasses: ["ajax__propertyGrid_valueobject_image"],
			    properties:
					{
					    src: imgUrl
					}

			}, parentcontainer);

        image.width = 25;
        image.height = 25;
        return image;
    },

    _onOpenObjectEditorClick: function (prop) {
        if (this._delegations["editorClick"]) {
            this._delegations["editorClick"](this.get_property());
        }

        var curValue = this.get_property().value;
        var imageID = "", filePath = "";
        if (curValue != "") {
            var imgProp = Sys.Serialization.JavaScriptSerializer.deserialize(curValue);
            imageID = imgProp.ID;
            filePath = imgProp.FilePath;
        }

        var returnValueStr = $find(this.get_ImageUploaderDialogControlID()).showDialog(imageID, filePath);
        if (typeof (returnValueStr) === "undefined") {
            return;
        }
        if (returnValueStr.length > 0) {
            value = returnValueStr; //Sys.Serialization.JavaScriptSerializer.deserialize(returnValueStr);
        }
        else {
            value = "";
        }
        this.commitValue(value);
        this._resetImageIcon();
        this._changeFormatStyle();
    },

    formatText: function (value) {
    }
}

$HGRootNS.ImageUploaderPropertyEditorForGrid.registerClass($HGRootNSName + ".ImageUploaderPropertyEditorForGrid", $HGRootNS.ObjectPropertyEditor);

$HGRootNS.OUUserInputPropertyEditor = function (prop, container, delegations) {

    $HGRootNS.OUUserInputPropertyEditor.initializeBase(this, [prop, container, delegations]);

    this._editElement$delegate.click = Function.createDelegate(this, this._editElement_onfocus);

    $addHandlers(this._editElement, this._editElement$delegate);
}

$HGRootNS.OUUserInputPropertyEditor.prototype =
{
    get_generalOuUserInputControlID: function () {
        var para = this.get_currentEditorParams();
        var cloneControlID = "OuUserInputPropertyEditor_OuUserInputControl";
        if (para) {
            if (para.cloneControlID) {
                cloneControlID = para.cloneControlID;
            }
        }

        return cloneControlID;
    },

    formatText: function (value) {
    },

    _changeEditElementStyle: function (ischange) {
    },

    commitValue: function (value) {
        if (typeof (value) == "undefined" || value == null || value == "") {
            this.get_property().value = "";
        } else {
            this.get_property().value = this.getPropertyValue(value);
        }
    },

    _onSelectedDataChanged: function (selectedData) {
        this.commitValue(selectedData);
        this._changeFormatStyle();
    },

    _createReadOnlyElement: function () {
        var template = $find(this.get_generalOuUserInputControlID());
        var ouUserInputControl = template.cloneAndAppendToContainer(this.get_container());
        //ouUserInputControl.add_selectedDataChanged(Function.createDelegate(this, this._onSelectedDataChanged));
        var pr = this.get_property();

        if (pr.editorParams) {
            if (typeof (pr.editorParams) == "string") {
                var objParams = Sys.Serialization.JavaScriptSerializer.deserialize(pr.editorParams);

                for (var item in objParams) {
                    var str = String.format("set_{0}", item);
                    if (Object.prototype.toString.apply(ouUserInputControl[str]) === "[object Function]") {
                        ouUserInputControl[str](objParams[item]);
                    }
                }
            }
        }

        if (typeof (pr.value) == "string" && pr.value != "" && pr.value != null) {
            ouUserInputControl.set_selectedData(Sys.Serialization.JavaScriptSerializer.deserialize(pr.value));
            ouUserInputControl.dataBind();
        }

        ouUserInputControl.set_readOnly(true);
        //this._valueInfoElement = ouUserInputControl;

        return ouUserInputControl.get_element();
    },

    _createEditElement: function () {
        var template = $find(this.get_generalOuUserInputControlID());
        var ouUserInputControl = template.cloneAndAppendToContainer(this.get_container());
        ouUserInputControl.add_selectedDataChanged(Function.createDelegate(this, this._onSelectedDataChanged));
        var pr = this.get_property();

        if (pr.editorParams) {
            if (typeof (pr.editorParams) == "string") {
                var objParams = Sys.Serialization.JavaScriptSerializer.deserialize(pr.editorParams);

                for (var item in objParams) {
                    var str = String.format("set_{0}", item);
                    if (Object.prototype.toString.apply(ouUserInputControl[str]) === "[object Function]") {
                        ouUserInputControl[str](objParams[item]);
                    }
                }
            }
        }

        if (typeof (pr.value) == "string" && pr.value != "" && pr.value != null) {
            ouUserInputControl.set_selectedData(Sys.Serialization.JavaScriptSerializer.deserialize(pr.value));
            ouUserInputControl.dataBind();
        }

        this._valueInfoElement = ouUserInputControl;

        return this._valueInfoElement.get_element();
    }
}

$HGRootNS.OUUserInputPropertyEditor.registerClass($HGRootNSName + ".OUUserInputPropertyEditor", $HGRootNS.ObjectPropertyEditor);


$HGRootNS.RoleGraphPropertyEditor = function (prop, container, delegations) {
    $HGRootNS.RoleGraphPropertyEditor.initializeBase(this, [prop, container, delegations]);
}

$HGRootNS.RoleGraphPropertyEditor.prototype =
{
    get_generalRoleGraphControlID: function () {
        var para = this.get_currentEditorParams();
        var cloneControlID = "RoleGraphPropertyEditor_RoleGraphControl";
        if (para) {
            if (para.cloneControlID) {
                cloneControlID = para.cloneControlID;
            }
        }

        return cloneControlID;
    },

    commitValue: function (value) {
        if (typeof (value) == "undefined" || value == null || value == "") {
            this.get_property().value = "";
        } else {
            this.get_property().value = this.getPropertyValue(value);
        }

        this.formatText(value);
    },

    _onOpenObjectEditorClick: function () {
        this.roleGraphControl.showDialog();

        $HGRootNS.RoleGraphPropertyEditor.callBaseMethod(this, "_onOpenObjectEditorClick");

        this.commitValue(this.roleGraphControl.get_selectedFullCodeName());
    },

    _createEditElement: function () {
        $HGRootNS.RoleGraphPropertyEditor.callBaseMethod(this, "_createEditElement");

        var template = $find(this.get_generalRoleGraphControlID());

        var roleGraphControl = template.cloneAndAppendToContainer(this.get_container());

        var pr = this.get_property();

        if (pr.editorParams) {
            if (typeof (pr.editorParams) == "string") {
                var objParams = Sys.Serialization.JavaScriptSerializer.deserialize(pr.editorParams);

                for (var item in objParams) {
                    var str = String.format("set_{0}", item);
                    if (Object.prototype.toString.apply(roleGraphControl[str]) === "[object Function]") {
                        roleGraphControl[str](objParams[item]);
                    }
                }
            }
        }

        if (typeof (pr.value) == "string" && pr.value != "" && pr.value != null) {
            roleGraphControl.set_selectedFullCodeName(pr.value);
        }

        this.roleGraphControl = roleGraphControl;

        return this._valueInfoElement;
    }
}

$HGRootNS.RoleGraphPropertyEditor.registerClass($HGRootNSName + ".RoleGraphPropertyEditor", $HGRootNS.ObjectPropertyEditor);

$HGRootNS.MaterialPropertyEditor = function (prop, container, delegations) {

    $HGRootNS.MaterialPropertyEditor.initializeBase(this, [prop, container, delegations]);

    this._editElement$delegate.click = Function.createDelegate(this, this._editElement_onfocus);

    $addHandlers(this._editElement, this._editElement$delegate);
}

$HGRootNS.MaterialPropertyEditor.prototype =
{
    get_generalMaterialControlID: function () {
        var para = this.get_currentEditorParams();
        var cloneControlID = "MaterialPropertyEditor_MaterialControl";
        if (para) {
            if (para.cloneControlID) {
                cloneControlID = para.cloneControlID;
            }
        }
        return cloneControlID;
    },

    _getFormatValue: function (value) {
        var propertyFormatValue = value;
        if ((typeof (value) == "string" || value.constructor == String) && value != "") {
            propertyFormatValue = Sys.Serialization.JavaScriptSerializer.deserialize(value);
        }

        return propertyFormatValue;
    },

    _createReadOnlyElement: function () {
        var template = $find(this.get_generalMaterialControlID());
        var materialControl = template.cloneAndAppendToContainer(this.get_container());
        this._initMaterialControlByEditorParams(materialControl);

        var curValue = this.get_property().value;
        if (curValue) {
            var objValue = Sys.Serialization.JavaScriptSerializer.deserialize(curValue);
            materialControl.set_materials(objValue);
        }
        materialControl.set_allowEdit(false);
        materialControl.set_allowEditContent(false);

        return materialControl.get_element();
    },

    _initMaterialControlByEditorParams: function (materialControl) {
        var editorParams = this.get_currentEditorParams();
        for (var item in editorParams) {
            var str = String.format("set_{0}", item);
            if (Object.prototype.toString.apply(materialControl[str]) === "[object Function]") {
                materialControl[str](editorParams[item]);
            }
        }
    },

    _createEditElement: function () {
        var template = $find(this.get_generalMaterialControlID());
        var materialControl = template.cloneAndAppendToContainer(this.get_container());
        this._initMaterialControlByEditorParams(materialControl);

        var curValue = this.get_property().value;
        if (curValue) {
            var objValue = Sys.Serialization.JavaScriptSerializer.deserialize(curValue);
            materialControl.set_materials(objValue);
        }

        materialControl.add_materialsChanged(Function.createDelegate(this, this._dataValueChange));

        return materialControl.get_element();
    },

    _dataValueChange: function (sender, e) {
        var stringValue = "";
        if (e.materials && e.materials.length > 0) {
            stringValue = Sys.Serialization.JavaScriptSerializer.serialize(e.materials);
        }
        this.commitValue(stringValue);
        this._changeFormatStyle();
    },

    show: function () {
        this._changeFormatStyle();
    },

    commitValue: function (value) {
        this.get_property().value = value;
    }
}

$HGRootNS.MaterialPropertyEditor.registerClass($HGRootNSName + ".MaterialPropertyEditor", $HGRootNS.StandardPropertyEditor);

$HGRootNS.ClientGridPropertyEditor = function (prop, container, delegations) {

    $HGRootNS.ClientGridPropertyEditor.initializeBase(this, [prop, container, delegations]);

    //this._editElement$delegate.click = Function.createDelegate(this, this._editElement_onfocus);

    $addHandlers(this._editElement, this._editElement$delegate);
};

$HGRootNS.ClientGridPropertyEditor.prototype =
{
    get_ClonedControlID: function () {
        var para = this.get_currentEditorParams();
        var cloneControlID = "ClientGridEditor_ClientGrid";
        if (para) {
            if (para.cloneControlID) {
                cloneControlID = para.cloneControlID;
            }
        }
        return cloneControlID;
    },

    _getFormatValue: function (value) {
        var propertyFormatValue = value;
        if ((typeof (value) == "string" || value.constructor == String) && value != "") {
            propertyFormatValue = Sys.Serialization.JavaScriptSerializer.deserialize(value);
        }

        return propertyFormatValue;
    },

    _createReadOnlyElement: function () {
        var clientGrid = $find(this.get_ClonedControlID());
        this.get_container().appendChild(clientGrid.get_element());


        var curValue = this.get_property().value;
        if (curValue) {
            var objValue = Sys.Serialization.JavaScriptSerializer.deserialize(curValue);
            if (clientGrid.get_autoBindOnLoad() === true)
                clientGrid.set_dataSourceNoBind(objValue);
            else
                clientGrid.set_dataSource(objValue);
        }
        clientGrid.set_readOnly(true);

        return clientGrid.get_element();
    },

    _initMaterialControlByEditorParams: function (materialControl) {
        var editorParams = this.get_currentEditorParams();
        for (var item in editorParams) {
            var str = String.format("set_{0}", item);
            if (Object.prototype.toString.apply(materialControl[str]) === "[object Function]") {
                materialControl[str](editorParams[item]);
            }
        }
    },

    _createEditElement: function () {
        var clientGrid = $find(this.get_ClonedControlID());
        this.get_container().appendChild(clientGrid.get_element());


        var curValue = this.get_property().value;
        if (curValue) {
            var objValue = Sys.Serialization.JavaScriptSerializer.deserialize(curValue);
            if (clientGrid.get_autoBindOnLoad() === true)
                clientGrid.set_dataSourceNoBind(objValue);
            else
                clientGrid.set_dataSource(objValue);
        }

        clientGrid.add_dataChanged(Function.createDelegate(this, this._dataValueChange));
        clientGrid.add_afterDataRowCreate(Function.createDelegate(this, this._dataValueChange));
        clientGrid.add_rowDelete(Function.createDelegate(this, this._dataValueChange));
        clientGrid.add_selectCheckboxClick(Function.createDelegate(this, this._dataValueChange));
        clientGrid.add_allSelectCheckboxClicked(Function.createDelegate(this, this._dataValueChange));

        return clientGrid.get_element();
    },

    _dataValueChange: function (sender, e) {
        var stringValue = "";
        if (sender.get_dataSource().length > 0) {
            stringValue = Sys.Serialization.JavaScriptSerializer.serialize(sender.get_dataSource());
        }
        this.commitValue(stringValue);
    },

    _changeFormatStyle: function () {

    },

    show: function () {
        //this._changeFormatStyle();
    },

    commitValue: function (value) {
        this.get_property().value = value;
    }
};

$HGRootNS.ClientGridPropertyEditor.registerClass($HGRootNSName + ".ClientGridPropertyEditor", $HGRootNS.StandardPropertyEditor);