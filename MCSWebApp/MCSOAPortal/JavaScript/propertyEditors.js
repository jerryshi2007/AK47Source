$HGRootNS.SignaturePropertyEditor = function (prop, container, delegations) {
    $HGRootNS.SignaturePropertyEditor.initializeBase(this, [prop, container, delegations]);
}

$HGRootNS.SignaturePropertyEditor.prototype =
{
    _get_userIDInputHiddenName: function () {
        return "SignaturePropertyEditor_UserIDHiddenInput";
    },

    _get_dialogUrl: function () {
        return "../../OACommonPages/ModalDialog/UserSignatureSetting.aspx";
    },

    _resetText: function () {
        var curValue = this.get_property().value;
        if (curValue && curValue != "") {
            this._valueInfoElement.innerText = "已设置";
        }
        else {
            this._valueInfoElement.innerText = "";
        }
    },

    _createReadOnlyElement: function () {
        this.get_container().innerHTML = "";

        var inputContainer = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "div",
				    cssClass: ["ajax__propertyGrid_input_container"]
				}, this.get_container());

        var css = ["ajax__propertyGrid_input", "ajax__propertyGrid_input_alignLeft"];

        var propValue = this.get_property().value;

        if (propValue && propValue != "") {
            propValue = "已设置";
        }
        else {
            propValue = "";
        }

        var htmlDomElement = $HGDomElement.createElementFromTemplate(
			{
			    nodeName: "lable",
			    innerText: propValue,
			    cssClasses: css
			}, inputContainer
			);

        htmlDomElement.style.color = "#8B8B83";
        return htmlDomElement;
    },

    getNeedToFormatValue: function (value) {
        if (value && value != "") {
            return "已设置";
        }
        else {
            return "";
        }
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
                isChange = true;
            } else {
                isChange = false;
            }

        } else {
            isChange = (item.value == item.defaultValue) ? false : true;
        }

        return isChange;
    },

    _createViewValueInfoElement: function (parentcontainer) {
        var propValue = this.get_property().value;
        var valueText = "";
        if (propValue && propValue != "") {
            valueText = "已设置";
        }
        else {
            valueText = "";
        }

        var css = ["ajax__propertyGrid_input", "ajax__propertyGrid_input_alignLeft"];

        var htmlDomElement = $HGDomElement.createElementFromTemplate(
			{
			    nodeName: "lable",
			    cssClasses: css
			}, parentcontainer);

        htmlDomElement.innerText = valueText;
        htmlDomElement.style.color = "#8B8B83";
        return htmlDomElement;
    },

    commitValue: function (value) {
        if (typeof (value) == "undefined" || value == null || value == "") {
            this.get_property().value = "";
        } else {
            this.get_property().value = this.getPropertyValue(value);
        }
    },

    _onOpenObjectEditorClick: function (prop) {

        if (this._delegations["editorClick"]) {
            this._delegations["editorClick"](this.get_property());
        }

        var userID = $get(this._get_userIDInputHiddenName()).value;
        var arg = "dialogHeight : 500px; dialogWidth : 700px; edge : Raised; center : Yes; help : No; resizable : No; status : No; scroll : no";
        var returnValueStr = window.showModalDialog(this._get_dialogUrl() + "?userID=" + escape(userID) + "&t=" + Date.parse(new Date()), null, arg);

        if (typeof (returnValueStr) === "undefined") {
            return;
        }

        this.commitValue(returnValueStr);
        this._resetText();
        this._changeFormatStyle();
    }
}

$HGRootNS.SignaturePropertyEditor.registerClass($HGRootNSName + ".SignaturePropertyEditor", $HGRootNS.ObjectPropertyEditor);