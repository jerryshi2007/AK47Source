
$HGRootNS.GenerateTypePropertyEditor = function (prop, container, delegations) {
    this.dropDownListSelectChange$delegate = {
        change: Function.createDelegate(this, this._dropDownListSelectChange)
    };
    this._dropDownList = null;
    $HGRootNS.GenerateTypePropertyEditor.initializeBase(this, [prop, container, delegations]);
}

$HGRootNS.GenerateTypePropertyEditor.prototype =
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

        var option1 = $HGDomElement.addSelectOption(dropDownList, "指定子流程Key", 0);

        var option2 = $HGDomElement.addSelectOption(dropDownList, "调用服务创建", 1);

        var propValue = (this.get_property().value == undefined || this.get_property().value == "") ? this.get_property().defaultValue : this.get_property().value;


        for (var i = 0; i < dropDownList.options.length; i++) {
            if (dropDownList.options[i].value == propValue) {
                dropDownList.options[i].selected = true;
                break;
            }
        }
        this._dropDownList = dropDownList;
        $addHandlers(dropDownList, this.dropDownListSelectChange$delegate);
        //        $addHandler(dropDownList, "onchange", this._dropDownListSelectChange);
        return dropDownList;
    },

    _dropDownListSelectChange: function (sender) {
        var itemvalue = parseInt(sender.target.value);
        this.get_property().value = itemvalue;

        if (itemvalue == 0) {
            this._setproperty_visible("OperationDefinition", "BranchProcessKey");
        }
        else if (itemvalue == 1) {
            this._setproperty_visible("BranchProcessKey", "OperationDefinition");
        }
    },

    _setproperty_visible: function (visibleName, viewName) {
        var _properties = this._delegations._owner._properties;
        var count = 0;
        for (var i = 0; i < _properties.length; i++) {
            var item = _properties[i];
            if (item.name == visibleName) {
                item.visible = false;
                count++;
            }
            if (item.name == viewName) {
                item.visible = true;
                count++;
            }
            if (count == 2) { break; }
        }

        this._delegations._owner.dataBind(_properties);
    },

    show: function () {

    },

    dispose: function () {
        if (this._dropDownList) {
            $HGDomEvent.removeHandlers(this._dropDownList, this.dropDownListSelectChange$delegate);
            delete this._dropDownList;
        }

        $HGRootNS.GenerateTypePropertyEditor.callBaseMethod(this, 'dispose');
    }
}

$HGRootNS.GenerateTypePropertyEditor.registerClass($HGRootNSName + ".GenerateTypePropertyEditor", $HGRootNS.StandardPropertyEditor);

$HGRootNS.BranchProcessKeyPropertyEditor = function (prop, container, delegations) {
    this.dropDownListSelectChange$delegate = {
        change: Function.createDelegate(this, this._dropDownListSelectChange)
    };

    $HGRootNS.BranchProcessKeyPropertyEditor.initializeBase(this, [prop, container, delegations]);

    if (this._editElement != null) {
        $HGDomEvent.removeHandlers(this._editElement, this._editElement$delegate);
    }


}

$HGRootNS.BranchProcessKeyPropertyEditor.prototype =
{
    _createViewValueInfoElement: function (parentcontainer) {
        var dropDownList = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "select",
				    properties:
					{
					    style:
						{
						    width: "100%",
						    height: "22px"
						}
					}
				}, parentcontainer);

        var editorParam = this.get_currentEditorParams();

        var enumList = [];
        if (editorParam != null) {
            if (typeof (editorParam) === "object") {
                if (editorParam.hasOwnProperty("enumTypeName")) {
                    enumList = this._delegations._owner._findPredefinedEnumDescription(editorParam.enumTypeName);
                }
            } else {
                enumList = this._delegations._owner._findPredefinedEnumDescription(editorParam);
            }
        }

        //var enumList = this._delegations._owner._findPredefinedEnumDescription(this.get_property().editorParams);

        for (var i = 0; i < enumList.length; i++) {
            var item = enumList[i];
            $HGDomElement.addSelectOption(dropDownList, item.text, item.value);
        }

        this._valueInfoElement = dropDownList;
        var propValue = (this.get_property().value == undefined || this.get_property().value == '') ? this.get_property().defaultValue : this.get_property().value;

        if (propValue) {

            var inEnumList = this.changeSelectItem(propValue);

            if (inEnumList == false) {
                var url = '/MCSWebApp/WorkflowDesigner/CommonHttpHandler.ashx';
                var webRequest = new Sys.Net.WebRequest();
                var requestBody = String.format("Action={0}&ProcKey={1}", "GetProcName", propValue);
                webRequest.set_body(requestBody);
                //webRequest.get_headers()["Action"] = "GetProcessName";
                webRequest.set_httpVerb("POST");
                //webRequest.get_headers()["Action"] = "GetProcessName";
                webRequest.set_url(url);
                webRequest.add_completed(function (response, eventArgs) {
                    if (response.get_responseAvailable()) {
                        var rtnObj = Sys.Serialization.JavaScriptSerializer.deserialize(response.get_responseData());

                        var item = { value: propValue };
                        if (rtnObj.Success) {
                            item.text = propValue + '-' + rtnObj.Message;
                        }
                        else {
                            item.text = propValue;
                        }

                        // this.addSelectItem(item.text, item.value);

                        var option = $HGDomElement.addSelectOption(dropDownList, item.text, item.value);
                        option.selected = true;
                    }
                });

                webRequest.invoke();
            }
        }

        $addHandlers(dropDownList, this.dropDownListSelectChange$delegate);

        return dropDownList;
    },

    _dropDownListSelectChange: function (sender) {
        //var itemvalue = parseInt(sender.target.value);
        this.get_property().value = sender.target.value;
    },

    changeSelectItem: function (value) {
        var result = false;
        for (var i = 0; i < this._valueInfoElement.options.length; i++) {
            var optionItem = this._valueInfoElement.options[i];
            if (optionItem.value == value) {
                this._valueInfoElement.value = value;
                optionItem.selected = true;
                // this.get_property().value = value;
                result = true;
                break;
            }
        }
        return result;
    },

    addSelectItem: function (textName, value) {
        $HGDomElement.addSelectOption(this._valueInfoElement, textName, value);
        this.changeSelectItem(value);
    },

    commitValue: function (value) {
        var result = this.changeSelectItem(value[0].Key);
        this.get_property().value = value[0].Key;
        if (result == false) {
            this.addSelectItem(value[0].Key + '-' + value[0].Name, value[0].Key);
        }
    },

    show: function () {

    },

    dispose: function () {
        if (this._valueInfoElement != null) {
            $HGDomEvent.removeHandlers(this._valueInfoElement, this.dropDownListSelectChange$delegate);
            delete this._editElement;
        }

        $HGRootNS.BranchProcessKeyPropertyEditor.callBaseMethod(this, 'dispose');
    }
}

$HGRootNS.BranchProcessKeyPropertyEditor.registerClass($HGRootNSName + ".BranchProcessKeyPropertyEditor", $HGRootNS.ObjectPropertyEditor);

$HGRootNS.ResourcePropertyEditor = function (prop, container, delegations) {
    $HGRootNS.ResourcePropertyEditor.initializeBase(this, [prop, container, delegations]);
}
//ReceiversObjectListPropertyEditor在\js\WFObjectListPropertyEditor.js
$HGRootNS.ResourcePropertyEditor.prototype = {

    getPropertyValue: function (value) {
        var propertyValue = value;
        if ((typeof value == "string" || value.constructor == String) && value != "") {
            propertyValue = Sys.Serialization.JavaScriptSerializer.deserialize(value);
        }

        return propertyValue;
    }
}

$HGRootNS.ResourcePropertyEditor.registerClass($HGRootNSName + ".ResourcePropertyEditor", $HGRootNS.ReceiversObjectListPropertyEditor);

$HGRootNS.BranchConditionPropertyEditor = function (prop, container, delegations) {
    $HGRootNS.BranchConditionPropertyEditor.initializeBase(this, [prop, container, delegations]);
}

$HGRootNS.BranchConditionPropertyEditor.prototype = {

    getPropertyValue: function (value) {
        var propertyValue = value;
        if ((typeof (value) == "string" || value.constructor == String) && value != "") {
            propertyValue = Sys.Serialization.JavaScriptSerializer.deserialize(value);
        }
        return propertyValue;
    },

    checkIsChangeStyle: function () {
        var item = this.get_property();

        var isChange = false;
        if (Object.prototype.toString.apply(item.value) === "[object Object]") {
            if (typeof (item.value) != "undefined") {
                if (item.value.hasOwnProperty("Expression")) {
                    if (typeof (item.value.Expression) != "undefined") {
                        isChange = true;
                    }
                }
            }
        }

        return isChange;
    },

    formatText: function (value) {
        var viewObject = value;
        if (viewObject.hasOwnProperty("Expression")) {
            this._valueInfoElement.innerText = viewObject.Expression;
        }

        this._changeFormatStyle();
    }
}

$HGRootNS.BranchConditionPropertyEditor.registerClass($HGRootNSName + ".BranchConditionPropertyEditor", $HGRootNS.ObjectPropertyEditor);

$HGRootNS.ServiceOperationPropertyEditor = function (prop, container, delegations) {
    $HGRootNS.ServiceOperationPropertyEditor.initializeBase(this, [prop, container, delegations]);
}

$HGRootNS.ServiceOperationPropertyEditor.prototype = {

    /*
    getPropertyValue: function (value) {
    var propertyValue = value;
    if ((typeof (value) == "string" || value.constructor == String) && value != "") {
    propertyValue = Sys.Serialization.JavaScriptSerializer.deserialize(value);
    }
    return propertyValue;
    }, */

    formatText: function (value) {
        if (value) {
            this._valueInfoElement.innerText = value.Key;
        } else {
            this._valueInfoElement.innerText = "";
        }
        this._changeFormatStyle();
    }
}

$HGRootNS.ServiceOperationPropertyEditor.registerClass($HGRootNSName + ".ServiceOperationPropertyEditor", $HGRootNS.ObjectPropertyEditor);
