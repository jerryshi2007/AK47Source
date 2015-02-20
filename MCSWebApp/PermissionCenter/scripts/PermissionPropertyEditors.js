
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

		PermissionCenter.Services.CommonService.ValidateCodeNameUnique($get("currentSchemaType").value, this.getObjID(), $get("currentParentID").value, this.get_property().value, this.getValidatorsDefineIncludingDeleted(), function (result) {
			if (!result) {
				currentEditor._editElement.value = "";
				currentEditor.commitValue();

				currentEditor._delegations._owner._activePropertyEditor(currentEditor.get_property());
				currentEditor._delegations._owner._raiseEvent("clickEditor", currentEditor.get_property());
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
				PermissionCenter.Services.CommonService.GetPinYin($get("currentSchemaType").value, codeNamePropertyEditor.getObjID(), $get("currentParentID").value, this.get_property().value, codeNamePropertyEditor.getValidatorsDefineIncludingDeleted(), function (result) {
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

		//		var pinYinValue = "";
		//		if (this.get_property().name == "LastName") {
		//			pinYinValue = this.get_property().value;
		//		} else {
		//			pinYinValue = this._getCodeNamePropertyEditor("LastName").get_property().value;
		//		}

		//		if (this.get_property().name == "FirstName") {
		//			pinYinValue = pinYinValue + this.get_property().value;
		//		} else {
		//			pinYinValue = pinYinValue + this._getCodeNamePropertyEditor("FirstName").get_property().value;
		//		}

		//		if (pinYinValue != "") {
		//			var currentEditor = this;
		//			PermissionCenter.Services.CommonService.GetPinYin(pinYinValue, function (result) {
		//				if (result != "") {
		//					var codeNamePropertyEditor = currentEditor._getCodeNamePropertyEditor("CodeName");
		//					codeNamePropertyEditor.get_property().value = result;
		//					codeNamePropertyEditor._editElement.value = result;
		//				}
		//			});
		//		}
	},

	edit: function () {

	}
}

$HGRootNS.GetPinYinEditor.registerClass($HGRootNSName + ".GetPinYinEditor", $HGRootNS.StandardPropertyEditor);