
$HGRootNS.ObjectListPropertyEditor = function (prop, container, delegations) {
	$HGRootNS.ObjectListPropertyEditor.initializeBase(this, [prop, container, delegations])
}

$HGRootNS.ObjectListPropertyEditor.prototype =
{
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

	formatText: function (value) {
		var currentPropertyName = this.get_property().name;
		var viewObject = value;

		this._valueInfoElement.innerText = "";
		var currentObjectLength = viewObject.length;
		if (currentObjectLength > 0) {
			for (var i = 0; i < currentObjectLength; i++) {
				if (i > 0) {
					this._valueInfoElement.innerText += ',';
				}

				switch (currentPropertyName) {
					case "Variables":
						this._valueInfoElement.innerText += viewObject[i].Key;
						break;
					case "Condition":
						this._valueInfoElement.innerText += viewObject[i].Expression;
						break;
					case "BranchProcessTemplates":
						this._valueInfoElement.innerText += viewObject[i].Key;
						break;
					case "EnterEventExecuteServices":
						this._valueInfoElement.innerText += viewObject[i].Key;
						break;
					case "CancelBeforeExecuteServices":
						this._valueInfoElement.innerText += viewObject[i].Key;
						break;
					case "CancelAfterExecuteServices":
						this._valueInfoElement.innerText += viewObject[i].Key;
						break;
					case "LeaveEventExecuteServices":
						this._valueInfoElement.innerText += viewObject[i].Key;
						break;
					case "ExternalUsers":
						this._valueInfoElement.innerText += viewObject[i].Name;
						break;
					case "RelativeLinks":
						this._valueInfoElement.innerText += viewObject[i].Key;
						break;
					case "ParametersNeedToBeCollected":
						this._valueInfoElement.innerText += viewObject[i].parameterName;
						break;
				}
			}
		}

		this._changeFormatStyle();
	}
}

$HGRootNS.ObjectListPropertyEditor.registerClass($HGRootNSName + ".ObjectListPropertyEditor", $HGRootNS.ObjectPropertyEditor);


$HGRootNS.ReceiversObjectListPropertyEditor = function (prop, container, delegations) {
	$HGRootNS.ReceiversObjectListPropertyEditor.initializeBase(this, [prop, container, delegations])
}

$HGRootNS.ReceiversObjectListPropertyEditor.prototype =
{
	_convertResourcePropertiesToCategries: function (resources) {
		var categories = {};

		for (var i = 0; i < resources.length; i++) {
			var prop = resources[i];
			var categoryName = prop.shortType;

			var category;
			if (categories.hasOwnProperty(categoryName)) {
				category = categories[categoryName];
			} else {
				category = { name: categoryName, properties: [] };
				categories[categoryName] = category;
			}

			category.properties.push(prop);
		}

		return categories;
	},

	formatCategoryText: function (courrentproperties, cateroryName, propertyName, viewName) {
		this._valueInfoElement.innerText += String.format("{0}:[", cateroryName);
		for (var i = 0; i < courrentproperties.length; i++) {
			var item = courrentproperties[i];
			if (item.hasOwnProperty(propertyName)) {
				if (typeof (item[propertyName]) != "undefined") {
					if (item[propertyName] != null) {
						if (i > 0) {
							this._valueInfoElement.innerText += ",";
						}

						if (item[propertyName].hasOwnProperty(viewName)) {
							this._valueInfoElement.innerText += item[propertyName][viewName];
						} else {
							this._valueInfoElement.innerText += item[propertyName];
						}
					}
				}
			}
		}
		this._valueInfoElement.innerText += "];"
	},

	formatText: function (value) {
		var viewObject = value;
		if (viewObject) {
			this._valueInfoElement.innerText = "";
			var categories = this._convertResourcePropertiesToCategries(viewObject);

			var courrentproperties;
			if (categories.hasOwnProperty("WfUserResourceDescriptor")) {
				courrentproperties = categories.WfUserResourceDescriptor.properties;
				this.formatCategoryText(courrentproperties, "用户", "User", "displayName");
			}

			if (categories.hasOwnProperty("WfDepartmentResourceDescriptor")) {
				courrentproperties = categories.WfDepartmentResourceDescriptor.properties;
				this.formatCategoryText(courrentproperties, "组织", "Department", "displayName");
			}

			if (categories.hasOwnProperty("WfGroupResourceDescriptor")) {
				courrentproperties = categories.WfGroupResourceDescriptor.properties;
				this.formatCategoryText(courrentproperties, "用户组", "Group", "name");
			}

			if (categories.hasOwnProperty("WfActivityOperatorResourceDescriptor")) {
				courrentproperties = categories.WfActivityOperatorResourceDescriptor.properties;
				this.formatCategoryText(courrentproperties, "执行人", "ActivityKey", "ActivityKey");
			}
			if (categories.hasOwnProperty("WfActivityAssigneesResourceDescriptor")) {
				courrentproperties = categories.WfActivityAssigneesResourceDescriptor.properties;
				this.formatCategoryText(courrentproperties, "指派人", "ActivityKey", "ActivityKey");
			}
			if (categories.hasOwnProperty("WfRoleResourceDescriptor")) {
				courrentproperties = categories.WfRoleResourceDescriptor.properties;
				this.formatCategoryText(courrentproperties, "角色", "Role", "name");
			}
			if (categories.hasOwnProperty("WfDynamicResourceDescriptor")) {
				courrentproperties = categories.WfDynamicResourceDescriptor.properties;
				this.formatCategoryText(courrentproperties, "动态角色", "Name", "Name");
			}
		}

		this._changeFormatStyle();
	}
}

$HGRootNS.ReceiversObjectListPropertyEditor.registerClass($HGRootNSName + ".ReceiversObjectListPropertyEditor", $HGRootNS.ObjectListPropertyEditor);

$HGRootNS.ConditionExpressionPropertyEditor = function (prop, container, delegations) {
	$HGRootNS.ConditionExpressionPropertyEditor.initializeBase(this, [prop, container, delegations])
}

$HGRootNS.ConditionExpressionPropertyEditor.prototype =
{
	checkIsChangeStyle: function () {
		var item = this.get_property();

		var isChange = false;
		if (Object.prototype.toString.apply(item.value) === "[object String]") {
			if (item.value != "") {
				var objCondition = Sys.Serialization.JavaScriptSerializer.deserialize(item.value);
				if (typeof (objCondition) != "undefined") {
					if (objCondition.hasOwnProperty("Expression")) {
						if (typeof (objCondition.Expression) != "undefined") {
							isChange = true;
						}
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
		} else {
			this._valueInfoElement.innerText = "";
		}

		this._changeFormatStyle();
	}

}

$HGRootNS.ConditionExpressionPropertyEditor.registerClass($HGRootNSName + ".ConditionExpressionPropertyEditor", $HGRootNS.ObjectPropertyEditor);


$HGRootNS.KeyPropertyEditor = function (prop, container, delegations) {
	$HGRootNS.KeyPropertyEditor.initializeBase(this, [prop, container, delegations])
}

$HGRootNS.KeyPropertyEditor.prototype =
{
	commitValue: function (value) {
		this.formatText(value);
		this.get_property().value = value;
	}
}

$HGRootNS.KeyPropertyEditor.registerClass($HGRootNSName + ".KeyPropertyEditor", $HGRootNS.ObjectPropertyEditor);


$HGRootNS.CanActivityKeysEditor = function (prop, container, delegations) {
	$HGRootNS.CanActivityKeysEditor.initializeBase(this, [prop, container, delegations])
}

$HGRootNS.CanActivityKeysEditor.prototype =
{
	checkIsChangeStyle: function () {
		var item = this.get_property();
		if ((item.value == "" || typeof (item.value) == "undefined") && (item.defaultValue == "" || typeof (item.defaultValue) == "undefined"))
			return false;
		else
			return (item.value == item.defaultValue) ? false : true;
	},

	commitValue: function (value) {
		if (Object.prototype.toString.apply(value) === "[object String]") {
			this.get_property().value = value;
			this.formatText(value);
		} else if (Object.prototype.toString.apply(value) === "[object Array]") {
			this.get_property().value = value.join(',');
			this.formatText(this.get_property().value);
		}

		this._changeFormatStyle();
	}
}

$HGRootNS.CanActivityKeysEditor.registerClass($HGRootNSName + ".CanActivityKeysEditor", $HGRootNS.ObjectPropertyEditor);


$HGRootNS.DynamicPropertyEditor = function (prop, container, delegations) {
	this.dropDownListSelectChange$delegate = {
		change: Function.createDelegate(this, this._dropDownListSelectChange)
	};
	$HGRootNS.DynamicPropertyEditor.initializeBase(this, [prop, container, delegations]);
	$addHandlers(this._editElement, this._editElement$delegate);
}

$HGRootNS.DynamicPropertyEditor.prototype =
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

		var propValue = !(this.get_property().value) ? this.get_property().defaultValue : this.get_property().value;
		for (var index = 0; index < boolEnum.length; index++) {
			var item = boolEnum[index];
			var option = $HGDomElement.addSelectOption(dropDownList, item.text, item.value);
			if (item.value == propValue) {
				option.selected = true;
				flag = true;
			}
		}

		if (flag == false && boolEnum.length > 0) {
			dropDownList.options[0].selected = true;
			this.get_property().value == boolEnum[0].value;
		}

		$addHandlers(dropDownList, this.dropDownListSelectChange$delegate);

		return dropDownList;
	},

	setDropDownListValue: function (propValue) {
		if (this._editElement) {
			for (var i = 0; i < this._editElement.options.length; i++) {
				if (this._editElement.options[i].value.toString().toLowerCase() == propValue.toString().toLowerCase()) {
					this._editElement.options[i].selected = true;
					break;
				}
			}

			this.show();
		}
	},

	_createReadOnlyElement: function () {
		this.get_container().innerHTML = "";

		var inputContainer = $HGDomElement.createElementFromTemplate(
				{
					nodeName: "div"
					,
					properties:
					{

					},
					cssClass: ["ajax__propertyGrid_input_container"]
				}, this.get_container()
				);
		var css = ["ajax__propertyGrid_input",
			this.get_property().dataType == $HGRootNS.PropertyDataType.Integer ? "ajax__propertyGrid_input_alignRight" : "ajax__propertyGrid_input_alignLeft"];

		var propValue = !(this.get_property().value) ? this.get_property().defaultValue : this.get_property().value;

		var htmlDomElement = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "lable",
				innerText: propValue.toString().toLowerCase(),
				cssClasses: css
			}, inputContainer
			);

		htmlDomElement.style.color = "#8B8B83";

		return htmlDomElement;
	},

	checkIsChangeStyle: function () {
		var item = this.get_property();
		var isChange;

		if ((item.defaultValue == null || item.defaultValue == undefined || item.defaultValue == "") && (item.value == null || item.value == undefined || item.value == "")) {
			isChange = false;
		} else if (Object.prototype.toString.apply(item.defaultValue) === "[object String]") {
			item.defaultValue = Boolean.parse(item.defaultValue);
		}
		else if (Object.prototype.toString.apply(item.value) === "[object String]") {
			item.value = Boolean.parse(item.value)
		}

		if (Object.prototype.toString.apply(item.value) === "[object Boolean]") {
			isChange = (item.value == item.defaultValue) ? false : true;
		}

		return isChange;
	},

	show: function () {
		var isChange = this.checkIsChangeStyle();

		this._changeDisplayNameElementStyle(isChange);
	},

	_getCodeNamePropertyEditor: function (strName) {
		return this._delegations._owner._propertyEditors[strName].editor;
	},

	_dropDownListSelectChange: function (sender) {
		this.get_property().value = sender.target.value;
		var isChange = this.checkIsChangeStyle();

		this._changeDisplayNameElementStyle(isChange);

		var keyValue = this._getCodeNamePropertyEditor("Key").get_property().value;

		//WFDesigner.DesignerInterAction.SLManager().UpdateDiagramData("Activity", keyValue, this.get_property().name, this.get_property().value);
	}
}
$HGRootNS.DynamicPropertyEditor.registerClass($HGRootNSName + ".DynamicPropertyEditor", $HGRootNS.StandardPropertyEditor);