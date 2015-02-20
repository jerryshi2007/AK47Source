
$HGRootNS.PasswordConfirmationEditor = function (prop, container, delegations) {

    this._confirmaInput$delegate = {
        focus: Function.createDelegate(this, this._confirmaInput_onfocus),
        blur: Function.createDelegate(this, this._confirmaInput_onblur),
        change: Function.createDelegate(this, this._confirmaInput_onchange),
        keypress: Function.createDelegate(this, this._confirmaInput_onkeypress)
    };

    this._confirmaInput = null;

    $HGRootNS.PasswordConfirmationEditor.initializeBase(this, [prop, container, delegations]);

    $addHandlers(this._confirmaInput, this._confirmaInput$delegate);
}

$HGRootNS.PasswordConfirmationEditor.prototype =
{
    _createEditElement: function () {
        this.get_container().innerHTML = "";
        var inputContainer = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "div",
				    cssClass: ["ajax__propertyGrid_input_container"]
				}, this.get_container()
				);

        var css = ["ajax__PasswordConfirmation_input"];

        var InputHtmlDomElement = $HGDomElement.createElementFromTemplate(
			{
			    nodeName: "input",
			    properties:
				{
				    value: "",
				    type: "password"
				},
			    cssClasses: css
			}, inputContainer
			);

        var ConfirmahtmlDomElement = $HGDomElement.createElementFromTemplate(
			{
			    nodeName: "input",
			    properties:
				{
				    type: "password",
				    value: ""
				},
			    cssClasses: css
			}, inputContainer
			);
        this._confirmaInput = ConfirmahtmlDomElement;

        return InputHtmlDomElement;
    },

    _confirmaInput_onfocus: function (eventElement) {
        var obj = eventElement.handlingElement;
        if (obj.createTextRange) {
            var range = obj.createTextRange();
            range.move("character", obj.value.length);
            range.collapse(true);
            range.select();
        }

        this._delegations["editorEnter"](this.get_property());
    },

    _confirmaInput_onblur: function (eventElement) {
        this.comparePassword();
    },

    _editElement_onblur: function (eventElement) {
        this.comparePassword();
    },

    _confirmaInput_onchange: function (eventElement) {
        this.comparePassword();
    },

    comparePassword: function (tag) {
        if (this._confirmaInput.value == this._editElement.value) {
            this.uninvalidConfirmaInputStyle();
            this.commitValue();
            this._isValid = false;
        } else {
            this.invalidConfirmaInputStyle("两次输入不一致");
        }
    },

    invalidConfirmaInputStyle: function (strMessage) {
        var strStyleName = this.get_invalidStyleName();
        if (Sys.UI.DomElement.containsCssClass(this._confirmaInput, strStyleName) == false) {
            Sys.UI.DomElement.addCssClass(this._confirmaInput, strStyleName);
            this._confirmaInput.title = strMessage;
        }
    },

    uninvalidConfirmaInputStyle: function () {
        var strStyleName = this.get_invalidStyleName();
        if (Sys.UI.DomElement.containsCssClass(this._confirmaInput, strStyleName)) {
            Sys.UI.DomElement.removeCssClass(this._confirmaInput, strStyleName);
            this._confirmaInput.title = "";
        }
    },

    _confirmaInput_onkeypress: function (eventElement) {

        if (eventElement.charCode == 13) {
            this.comparePassword();
        }
        //this._changeFormatStyle();
    },

    _changeFormatStyle: function () {
        var isChange = this.checkIsChangeStyle();

        this._changeDisplayNameElementStyle(isChange);
    }
}

$HGRootNS.PasswordConfirmationEditor.registerClass($HGRootNSName + ".PasswordConfirmationEditor", $HGRootNS.StandardPropertyEditor);
