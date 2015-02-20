
$HGRootNS.CodeNameUniqueEditor = function (prop, container, delegations) {
    $HGRootNS.CodeNameUniqueEditor.initializeBase(this, [prop, container, delegations])
}

$HGRootNS.CodeNameUniqueEditor.prototype = {

    _editElement_onchange: function (eventElement) {
        var validateEventArgs = new Sys.EventArgs();
        validateEventArgs.result = true;
        if (this._delegations["editorValidating"]) {
            this._delegations["editorValidating"](this.get_property(), eventElement.handlingElement);

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

        var currentEditor = this;

        AUCenter.Services.CommonServices.ValidateCodeNameUnique($get("currentSchemaType").value, this.getObjID(), $get("currentParentID").value, this.get_property().value, this.getValidatorsDefineIncludingDeleted(), function (result) {
            if (!result) {
                currentEditor._editElement.value = "";
                currentEditor.commitValue();

                currentEditor._delegations._owner._activePropertyEditor(currentEditor.get_property());
                currentEditor._delegations._owner._raiseEvent("clickEditor", currentEditor.get_property());
                alert("因为此代码名称被已经被使用，请换另一个代码名称。");
                currentEditor._editElement.focus();
                //currentEditor.
                //window.alert("重复存在! ");
                
            }
        }, function (err) {
            alert("无法访问代码名称检查服务：" + err.message);
        });
    },

    getValidatorsDefineIncludingDeleted: function () {
        var includingDeleted = false;
        var validators = Sys.Serialization.JavaScriptSerializer.deserialize(this.get_property().validators);

        for (var i = 0; i < validators.length; i++) {
            var item = validators[i];
            if (item.name == "CodeNameUniqueValidator") {
                for (var j = 0; j < item.validatorParameters.length; j++) {
                    var pitem = item.validatorParameters[j];
                    if (pitem.paramName == "includingDeleted") {
                        includingDeleted = Boolean.parse(pitem.paramValue);
                        break;
                    }
                }
            }
            if (includingDeleted == true) {
                break;
            }
        }

        return includingDeleted;
    },

    getObjID: function () {
        var properties = this._delegations._owner.get_properties();
        var currentEditID = "";
        for (var i = 0; i < properties.length; i++) {
            var itemproperty = properties[i];
            if (itemproperty.name == "ID") {
                currentEditID = itemproperty.value;
            }
        }

        return currentEditID;
    }
}

$HGRootNS.CodeNameUniqueEditor.registerClass($HGRootNSName + ".CodeNameUniqueEditor", $HGRootNS.StandardPropertyEditor);

$HGRootNS.GetPinYinEditor = function (prop, container, delegations) {
    $HGRootNS.GetPinYinEditor.initializeBase(this, [prop, container, delegations])
}

$HGRootNS.PObjectNameEditor = function (prop, container, delegations) {
    $HGRootNS.PObjectNameEditor.initializeBase(this, [prop, container, delegations])
}

$HGRootNS.PObjectNameEditor.prototype = {

    commitValue: function () {
        if (this._editElement)
            this.get_property().value = this._editElement.value;

        this.changePinYin();
        this.changeDisplayName();
    },

    _getCodeNamePropertyEditor: function (strName) {
        return this._delegations._owner._propertyEditors[strName].editor;
    },

    changeDisplayName: function () {
        var displayNamePropertyEditor = this._getCodeNamePropertyEditor("DisplayName");

        if (displayNamePropertyEditor) {
            var displayNameProperty = displayNamePropertyEditor.get_property();
            if (displayNameProperty.value == "" || displayNameProperty.value == '') {
                displayNameProperty.value = this.get_property().value;
                displayNamePropertyEditor._editElement.value = this.get_property().value;
            }
        }
    },

    changePinYin: function () {
        var currentEditor = this;
        var codeNamePropertyEditor = currentEditor._getCodeNamePropertyEditor("CodeName")

        if (codeNamePropertyEditor) {
            var codeNameProperty = currentEditor._getCodeNamePropertyEditor("CodeName").get_property();
            if (codeNameProperty.value == "" || codeNameProperty.value == '') {
                var currentEditor = this;
                AUCenter.Services.CommonServices.GetPinYin($get("currentSchemaType").value, codeNamePropertyEditor.getObjID(), $get("currentParentID").value, this.get_property().value, codeNamePropertyEditor.getValidatorsDefineIncludingDeleted(), function (result) {
                    if (result && result != "") {
                        var codeNamePropertyEditor = currentEditor._getCodeNamePropertyEditor("CodeName");
                        codeNamePropertyEditor.get_property().value = result;
                        codeNamePropertyEditor._editElement.value = result;
                    }
                });
            }
        }
    }
}

$HGRootNS.PObjectNameEditor.registerClass($HGRootNSName + ".PObjectNameEditor", $HGRootNS.StandardPropertyEditor);

$HGRootNS.GetPinYinEditor = function (prop, container, delegations) {
    $HGRootNS.GetPinYinEditor.initializeBase(this, [prop, container, delegations])

}

$HGRootNS.GetPinYinEditor.prototype = {

    _isGetPinYin: function () {
        var result = false;
        var codeNamePropertyEditor = this._getCodeNamePropertyEditor("CodeName");
        if (codeNamePropertyEditor) {
            var codeNameValue = codeNamePropertyEditor.get_property().value;
            if (codeNameValue == "" || typeof (codeNameValue) == "undefined") {
                result = true;
            }
        }
        return result;
    },

    _getCodeNamePropertyEditor: function (strName) {
        return this._delegations._owner._propertyEditors[strName].editor;
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

    commitValue: function () {
        if (this._editElement)
            this.get_property().value = this._editElement.value;

        var nameEditor = this._getCodeNamePropertyEditor("Name");
        var displayNameEditor = this._getCodeNamePropertyEditor("DisplayName");
        var lastNameValue = "";
        var firstNameValue = "";

        if (this.get_property().name == "LastName") {
            lastNameValue = this.get_property().value;
            firstNameValue = this._getCodeNamePropertyEditor("FirstName").get_property().value;
        } else if (this.get_property().name == "FirstName") {
            lastNameValue = this._getCodeNamePropertyEditor("LastName").get_property().value;
            firstNameValue = this.get_property().value;
        }

        namevalue = lastNameValue + firstNameValue;

        if (nameEditor.get_property().value == lastNameValue || nameEditor.get_property().value == firstNameValue) {
            nameEditor.get_property().value = namevalue;
            nameEditor._editElement.value = namevalue;
        }

        if (displayNameEditor.get_property().value == lastNameValue || displayNameEditor.get_property().value == firstNameValue) {
            displayNameEditor.get_property().value = namevalue;
            displayNameEditor._editElement.value = namevalue;
        }

        if (firstNameValue != "" && lastNameValue != "") {
            if (this._getCodeNamePropertyEditor("CodeName").get_property().value == "" || this._getCodeNamePropertyEditor("CodeName").get_property().value == null) {
                nameEditor.changePinYin();
            }
        }
    },

    edit: function () {

    }
}

$HGRootNS.GetPinYinEditor.registerClass($HGRootNSName + ".GetPinYinEditor", $HGRootNS.StandardPropertyEditor);


$HGRootNS.AdminScopeEditor = function (prop, container, delegations) {
    $HGRootNS.AdminScopeEditor.initializeBase(this, [prop, container, delegations])
};

$HGRootNS.AdminScopeEditor.AdminScopes = null;

$HGRootNS.AdminScopeEditor.reloadScopes = function (callback, errCallback) {
    AUCenter.Services.CommonServices.GetScopeTypes(function (result) {
        if (result) {
            $HGRootNS.AdminScopeEditor.AdminScopes = result;

            if (typeof (callback) === 'function') {
                callback(result);
            }
        }
    }, function (err) {
        if (typeof (errCallback) === 'function') {
            errCallback(err);
        } else {
            alert("无法访问管理范围检查服务：" + err.get_message());
        }
    });
}

$HGRootNS.AdminScopeEditor.reloadScopes(function (r) {
    $HGRootNS.AdminScopeEditor.AdminScopes = r;
});

$HGRootNS.AdminScopeEditor.prototype = {
    _createEditElement: function () {
        $HGRootNS.AdminScopeEditor.callBaseMethod(this, "_createEditElement");
        var container = this.get_container();
        var c = document.createElement("div");
        container.appendChild(c);
        c.className = "au-pop-container scope-editor";
        this._dropDownElement = c;

        var d = document.createElement("div");
        c.appendChild(d);
        d.className = "au-pop-sub  scope-editor";

        var ul = document.createElement("ul");
        d.appendChild(ul);
        ul.className = "au-pop-list  scope-editor";

        if (!$HGRootNS.AdminScopeEditor.AdminScopes) {
            var that = this;
            $HGRootNS.AdminScopeEditor.reloadScopes(function (result) { that._generateScopeListItems(result); });
        } else {
            this._generateScopeListItems($HGRootNS.AdminScopeEditor.AdminScopes);
        }

        this._checkExternalClickHandler = Function.createDelegate(this, this._checkExternalClick);
        $addHandler(document, "mousedown", this._checkExternalClickHandler);
        this._scopeItemClickHandler = Function.createDelegate(this, this._scopeItemClick);
        $addHandler(ul, "click", this._scopeItemClickHandler);

        this._isDroppedDown = false;

        return this._valueInfoElement;

    }, commitValue: function (value) {
        if (typeof (value) == "undefined" || value == null || value == "") {
            this.get_property().value = "";
        } else {
            this.get_property().value = this.getPropertyValue(value);
        }

        this.formatText(value);
    }, _setChecked: function (arr) {
        var ul = this._dropDownElement.firstChild.firstChild;
        var key;
        var shouldCheck;
        var btn;
        for (var li = ul.firstChild; li; li = li.nextSibling) {
            btn = li.firstChild;
            if (btn.getAttribute) {
                key = btn.getAttribute("data-schema");
            } else {
                key = btn["data-schema"];
            }

            shouldCheck = false;

            for (var ind = arr.length - 1; ind >= 0; ind--) {
                if (arr[ind] == key) {
                    shouldCheck = true;
                    break;
                }
            }

            btn.firstChild.checked = shouldCheck;
            btn = null;
        }
    }, _getCheckedValues: function () {
        var rs = [];
        var ul = this._dropDownElement.firstChild.firstChild;
        var key;
        var shouldCheck;
        var btn;
        for (var li = ul.firstChild; li; li = li.nextSibling) {
            btn = li.firstChild;
            if (btn.firstChild.checked) {
                if (btn.getAttribute) {
                    key = btn.getAttribute("data-schema");
                } else {
                    key = btn["data-schema"];
                }

                rs.push(key);
            }

            btn = null;
        }

        return rs;
    }, showDropDown: function () {
        if (this._isDroppedDown === false) {
            var pr = this.get_property();
            v = pr.value.split(",");
            this._setChecked(v);
            this._dropDownElement.firstChild.style.display = "block";
            this._isDroppedDown = true;
        }

    }, hideDropDown: function () {
        if (this._isDroppedDown) {
            this._dropDownElement.firstChild.style.display = "none";
            this._isDroppedDown = false;
            if (this.get_enabled()) {
                this.commitValue(this._getCheckedValues().join(","));
            }
        }
    }, _checkExternalClick: function (event) {
        var target = event.target;
        if (target !== this._editorBtn && target !== this._dropDownElement) {
            var elem = target;
            var isHitElement = false;
            while (elem != null && elem !== window) {
                if (elem.parentNode === this._dropDownElement) {
                    isHitElement = true;
                    break;
                }
                elem = elem.parentNode;
            }

            if (!isHitElement)
                this.hideDropDown();
        }
    }, _scopeItemClick: function (event) {
        var chk = null;
        if (event.target.nodeName.toUpperCase() === "BUTTON") {
            if (this.get_enabled()) {
                chk = event.target.firstChild;
                chk.checked = !chk.checked;
            }
        }
        chk = null;
    }, _onOpenObjectEditorClick: function () {
        this.showDropDown();
    }, _editElement_onchange: function (eventElement) {

    }, _generateScopeListItems: function (src) {
        var ul = this._dropDownElement.firstChild.firstChild;
        var li;
        var btn;
        var chk;
        if (src) {
            ul.innerHtml = '';
            for (var ind = 0; ind < src.length; ind++) {
                li = document.createElement("li");
                li.className = "au-item  scope-editor";
                ul.appendChild(li);
                btn = document.createElement("button");
                li.appendChild(btn);
                if (btn.setAttribute) {
                    btn.setAttribute("data-schema", src[ind].SchemaType);
                } else {
                    btn["data-schema"] = src[ind].SchemaType;
                }
                chk = document.createElement("input");
                chk.type = "checkbox";
                chk.className = "scope-editor";
                btn.appendChild(chk);
                btn.appendChild(document.createTextNode(src[ind].SchemaName));
            }
        }

    }
};

$HGRootNS.AdminScopeEditor.registerClass($HGRootNSName + ".AdminScopeEditor", $HGRootNS.ObjectPropertyEditor);

$HGRootNS.SchemaCategoryEditor = function (prop, container, delegations) {
    this._displayElement = null;
    this._onCategoryChange = null;
    //    this._refreshDisplay$Delegation = Function.createDelegate(this, this._refreshDisplay)
    $HGRootNS.SchemaCategoryEditor.initializeBase(this, [prop, container, delegations])

}

$HGRootNS.SchemaCategoryEditor.prototype = {
    //    initialize: function () {
    //        debugger;
    //        $HGRootNS.SchemaCategoryEditor.callBaseMethod(this, 'initialize'); //咦，没进来
    //        debugger;
    //    },
    get_currentDocument: function () {
        return $HGDomElement.get_currentDocument();
    },
    refreshDisplay: function () {
        this._refreshDisplay();
    },
    show: function () {
        this._formatText(this._editElement);
        this._changeFormatStyle();
        this.refreshDisplay();
    },
    _refreshDisplay: function () {
        var propValue = this.get_property().value || this.get_property().defaultValue;
        var val = propValue;
        var that = this;
        AUCenter.Services.CommonServices.GetSchemaCategoryName(val, function (result) {
            if (that._displayElement) {
                var tElement = that.get_currentDocument().createTextNode(result);
                that._displayElement.innerHTML = '';
                that._displayElement.appendChild(tElement);
                that = null;
            }
        }, function (err) {
            $showError(err);
            that = null;
        });

    },
    _createEditElement: function () {
        this.get_container().innerHTML = "";

        var inputElem = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "input",
				    properties:
					{
					    type: "hidden",
					    style:
						{
						    display: "none"
						}
					}
				}, this.get_container()
				);

        this._displayElement = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "span"
				}, this.get_container()
				);
        return inputElem;
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
						    display: "none"
						}
					}
				}, this.get_container());
        this._displayElement = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "span"
				}, this.get_container()
				);

        if (propValue)
            htmlDomElement.innerText = propValue.toString(); ;

        return htmlDomElement;
    },
    dispose: function () {
        $HGRootNS.SchemaCategoryEditor.callBaseMethod(this, 'dispose');
    }
}

$HGRootNS.SchemaCategoryEditor.registerClass($HGRootNSName + ".SchemaCategoryEditor", $HGRootNS.StandardPropertyEditor);