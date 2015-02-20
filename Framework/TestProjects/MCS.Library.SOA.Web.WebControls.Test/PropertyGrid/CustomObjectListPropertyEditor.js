$HGRootNS.CustomObjectListPropertyEditor = function (prop, container, delegations) {
    $HGRootNS.CustomObjectListPropertyEditor.initializeBase(this, [prop, container, delegations])
}

$HGRootNS.CustomObjectListPropertyEditor.prototype =
{
    commitValue: function (value) {
        this.formatText(value);
        this.get_property().value = value;
    },

    formatText: function (value) {
        if (value) {
            for (var i = 0; i < value.length; i++) {
                this._valueInfoElement.innerText += value[i].RandyWang;
                this._valueInfoElement.innerText += ","
            }
        }
    },

    show: function () {
        if (this.get_property().value) {
            this.formatText(this.get_property().value);
        }
    }
}

$HGRootNS.CustomObjectListPropertyEditor.registerClass($HGRootNSName + ".CustomObjectListPropertyEditor", $HGRootNS.ObjectPropertyEditor);
