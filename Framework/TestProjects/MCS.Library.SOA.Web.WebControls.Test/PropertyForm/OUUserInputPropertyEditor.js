

$HGRootNS.OUUserInputPropertyEditor = function (prop, container, delegations) {

	$HGRootNS.OUUserInputPropertyEditor.initializeBase(this, [prop, container, delegations]);

	$addHandlers(this._editElement, this._editElement$delegate);
}

$HGRootNS.OUUserInputPropertyEditor.prototype =
{
	get_generalOuUserInputControlID: function () {
		return "OuUserInputPropertyEditor_OuUserInputControl";
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
		this._valueInfoElement = ouUserInputControl;

		return this._valueInfoElement.get_element();
	}
}

$HGRootNS.OUUserInputPropertyEditor.registerClass($HGRootNSName + ".OUUserInputPropertyEditor", $HGRootNS.ObjectPropertyEditor);
